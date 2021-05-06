// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.TcpSocket
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
  internal class TcpSocket
  {
    public const int MaxMessageSize = 16777216;
    internal const int PeerAliveCode = -1234;
    internal const int DisconnectCode = -9999;
    private int _uniqueSocketId;
    private static int _socketCount;
    private Socket _dotNetSocket;
    private SocketAsyncEventArgs _socketAsyncEventArgsWrite;
    private SocketAsyncEventArgs _socketAsyncEventArgsListener;
    private SocketAsyncEventArgs _socketAsyncEventArgsRead;
    internal MessageBuffer LastReadMessage;
    private ConcurrentQueue<MessageBuffer> _writeNetworkMessageQueue;
    private MessageBuffer _currentlySendingMessage;
    private bool _currentlyAcceptingClients;
    private ConcurrentQueue<TcpSocket> _incomingConnections;
    private int _lastMessageTotalRead;
    private TcpStatus _status;
    private bool _processingReceive;
    private string _remoteEndComputerName;
    private readonly MessageBuffer _peerAliveMessageBuffer;
    private readonly MessageBuffer _disconnectMessageBuffer;

    internal int LastMessageSentTime { get; set; }

    internal int LastMessageArrivalTime { get; set; }

    internal TcpStatus Status
    {
      get => this._status;
      private set
      {
        if (value == TcpStatus.ConnectionClosed && this._status != value && this.Closed != null)
          this.Closed();
        this._status = value;
      }
    }

    internal string RemoteEndComputerName
    {
      get
      {
        if (this._remoteEndComputerName == "")
        {
          try
          {
            this._remoteEndComputerName = Dns.GetHostEntry(((IPEndPoint) this._dotNetSocket.RemoteEndPoint).Address).HostName;
          }
          catch (Exception ex)
          {
            this._remoteEndComputerName = "Unknown";
          }
        }
        return this._remoteEndComputerName;
      }
    }

    internal string IPAddress => ((IPEndPoint) this._dotNetSocket.RemoteEndPoint).Address.ToString();

    internal TcpSocket()
    {
      this._uniqueSocketId = TcpSocket._socketCount;
      ++TcpSocket._socketCount;
      this.LastReadMessage = (MessageBuffer) null;
      this.LastMessageSentTime = 0;
      this._writeNetworkMessageQueue = new ConcurrentQueue<MessageBuffer>();
      this._socketAsyncEventArgsWrite = new SocketAsyncEventArgs();
      this._socketAsyncEventArgsWrite.Completed += new EventHandler<SocketAsyncEventArgs>(this.ProcessIO);
      this._socketAsyncEventArgsRead = new SocketAsyncEventArgs();
      this._socketAsyncEventArgsRead.Completed += new EventHandler<SocketAsyncEventArgs>(this.ProcessIO);
      this._socketAsyncEventArgsListener = new SocketAsyncEventArgs();
      this._socketAsyncEventArgsListener.Completed += new EventHandler<SocketAsyncEventArgs>(this.ProcessIO);
      byte[] buffer1 = new byte[4]
      {
        (byte) 46,
        (byte) 251,
        byte.MaxValue,
        byte.MaxValue
      };
      this._peerAliveMessageBuffer = new MessageBuffer(buffer1, buffer1.Length);
      byte[] buffer2 = new byte[4]
      {
        (byte) 241,
        (byte) 216,
        byte.MaxValue,
        byte.MaxValue
      };
      this._disconnectMessageBuffer = new MessageBuffer(buffer2, buffer2.Length);
    }

    internal TcpSocket GetLastIncomingConnection()
    {
      TcpSocket result;
      if (this._incomingConnections.TryDequeue(out result))
      {
        result.LastMessageArrivalTime = Environment.TickCount;
        result.LastMessageSentTime = Environment.TickCount;
      }
      return result;
    }

    internal void Connect(string address, int port)
    {
      try
      {
        this.LastMessageArrivalTime = Environment.TickCount;
        this.LastMessageSentTime = 0;
        this._dotNetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        System.Net.IPAddress[] hostAddresses = Dns.GetHostAddresses(address);
        System.Net.IPAddress address1 = (System.Net.IPAddress) null;
        for (int index = 0; index < hostAddresses.Length; ++index)
        {
          if (hostAddresses[index].AddressFamily == AddressFamily.InterNetwork)
            address1 = hostAddresses[index];
        }
        this._socketAsyncEventArgsListener.RemoteEndPoint = (EndPoint) new IPEndPoint(address1, port);
        this.Status = TcpStatus.Connecting;
        if (this._dotNetSocket.ConnectAsync(this._socketAsyncEventArgsListener))
          return;
        this.ProcessIO((object) this._dotNetSocket, this._socketAsyncEventArgsListener);
      }
      catch (Exception ex)
      {
        this.Status = TcpStatus.SocketClosed;
        Debug.Print("Tcp Connection Error: " + (object) ex);
        Thread.Sleep(250);
      }
    }

    internal void CheckAcceptClient()
    {
      if (this._currentlyAcceptingClients)
        return;
      this._currentlyAcceptingClients = true;
      if (this._dotNetSocket.AcceptAsync(this._socketAsyncEventArgsListener))
        return;
      this.ProcessIO((object) this._dotNetSocket, this._socketAsyncEventArgsListener);
    }

    internal void Listen(int port)
    {
      this._incomingConnections = new ConcurrentQueue<TcpSocket>();
      this._dotNetSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      this._dotNetSocket.Bind((EndPoint) new IPEndPoint(System.Net.IPAddress.Any, port));
      this._dotNetSocket.Listen(1024);
      this.CheckAcceptClient();
    }

    internal bool IsConnected
    {
      get
      {
        if ((this.Status == TcpStatus.DataReady || this.Status == TcpStatus.WaitingDataLength || this.Status == TcpStatus.WaitingData) && (this._dotNetSocket == null || !this._dotNetSocket.Connected))
          this.Status = TcpStatus.ConnectionClosed;
        return this.Status != TcpStatus.SocketClosed && this.Status != TcpStatus.ConnectionClosed && (uint) this.Status > 0U;
      }
    }

    internal void ProcessRead()
    {
      if (!this.IsConnected || this._processingReceive || this.Status != TcpStatus.WaitingData && this.Status != TcpStatus.WaitingDataLength)
        return;
      if (this.LastReadMessage == null)
        this.LastReadMessage = new MessageBuffer(new byte[16777216], 0);
      if (this.Status == TcpStatus.WaitingDataLength)
      {
        this._processingReceive = true;
        this._socketAsyncEventArgsRead.SetBuffer(this.LastReadMessage.Buffer, this._lastMessageTotalRead, 4 - this._lastMessageTotalRead);
        if (this._dotNetSocket.ReceiveAsync(this._socketAsyncEventArgsRead))
          return;
        this.ProcessIO((object) this._dotNetSocket, this._socketAsyncEventArgsRead);
      }
      else
      {
        if (this.Status != TcpStatus.WaitingData)
          return;
        this._processingReceive = true;
        this._socketAsyncEventArgsRead.SetBuffer(this.LastReadMessage.Buffer, this._lastMessageTotalRead, this.LastReadMessage.DataLength - this._lastMessageTotalRead);
        if (this._dotNetSocket.ReceiveAsync(this._socketAsyncEventArgsRead))
          return;
        this.ProcessIO((object) this._dotNetSocket, this._socketAsyncEventArgsRead);
      }
    }

    internal void ProcessWrite()
    {
      if (!this.IsConnected || this._currentlySendingMessage != null)
        return;
      if (!this._writeNetworkMessageQueue.TryDequeue(out this._currentlySendingMessage))
        return;
      try
      {
        this._socketAsyncEventArgsWrite.SetBuffer(this._currentlySendingMessage.Buffer, 0, this._currentlySendingMessage.DataLength);
        if (!this._dotNetSocket.SendAsync(this._socketAsyncEventArgsWrite))
          this.ProcessIO((object) this._dotNetSocket, this._socketAsyncEventArgsWrite);
        this.LastMessageSentTime = Environment.TickCount;
      }
      catch (Exception ex)
      {
        Debug.Print("SendMessage Error: " + (object) ex);
        this.Status = TcpStatus.ConnectionClosed;
      }
    }

    private void ProcessIO(object sender, SocketAsyncEventArgs e)
    {
      try
      {
        if (e.LastOperation == SocketAsyncOperation.Accept)
        {
          if (e.SocketError != SocketError.Success)
            return;
          this.Status = TcpStatus.WaitingDataLength;
          this.AddIncomingConnection(e);
        }
        else if (e.LastOperation == SocketAsyncOperation.Connect)
        {
          if (e.SocketError == SocketError.Success)
          {
            this.Status = TcpStatus.WaitingDataLength;
          }
          else
          {
            Debug.Print("Connection error: " + (object) e.SocketError);
            this.Status = TcpStatus.ConnectionClosed;
          }
        }
        else if (e.LastOperation == SocketAsyncOperation.Send)
        {
          if (e.SocketError == SocketError.Success)
          {
            if (this._currentlySendingMessage == this._disconnectMessageBuffer)
              this.Status = TcpStatus.ConnectionClosed;
            this._currentlySendingMessage = (MessageBuffer) null;
          }
          else
          {
            Debug.Print("Message Send, error: " + (object) e.SocketError);
            this.Status = TcpStatus.ConnectionClosed;
          }
        }
        else
        {
          if (e.LastOperation == SocketAsyncOperation.Disconnect || e.LastOperation != SocketAsyncOperation.Receive)
            return;
          this.LastMessageArrivalTime = Environment.TickCount;
          if (this.Status == TcpStatus.WaitingDataLength)
          {
            if (e.BytesTransferred != 4 - this._lastMessageTotalRead)
              return;
            this._lastMessageTotalRead += e.BytesTransferred;
            if (this._lastMessageTotalRead != 4)
              return;
            int int32 = BitConverter.ToInt32(this.LastReadMessage.Buffer, 0);
            switch (int32)
            {
              case -9999:
                this.Status = TcpStatus.ConnectionClosed;
                break;
              case -1234:
                this.Status = TcpStatus.WaitingDataLength;
                this._lastMessageTotalRead = 0;
                this.LastReadMessage = (MessageBuffer) null;
                break;
              default:
                if (int32 > 16777216)
                  throw new Exception(string.Format("Message length too big: {0}", (object) this.LastReadMessage.DataLength));
                if (int32 <= 0)
                  throw new Exception(string.Format("Message length too small: {0}", (object) this.LastReadMessage.DataLength));
                this.LastReadMessage.DataLength = int32 + 4;
                this.Status = TcpStatus.WaitingData;
                break;
            }
            this._processingReceive = false;
          }
          else
          {
            if (this.Status != TcpStatus.WaitingData)
              return;
            this._lastMessageTotalRead += e.BytesTransferred;
            if (this._lastMessageTotalRead == this.LastReadMessage.DataLength)
            {
              if (this.MessageReceived != null)
                this.MessageReceived(this.LastReadMessage);
              this.Status = TcpStatus.WaitingDataLength;
              this._lastMessageTotalRead = 0;
              this.LastReadMessage = (MessageBuffer) null;
            }
            this._processingReceive = false;
          }
        }
      }
      catch (Exception ex)
      {
        Debug.Print("Exception on TcpSocket::ProcessIO " + (object) ex + " - " + ex.Message + " " + ex.StackTrace);
      }
    }

    private void AddIncomingConnection(SocketAsyncEventArgs e)
    {
      TcpSocket tcpSocket = new TcpSocket();
      tcpSocket._dotNetSocket = e.AcceptSocket;
      e.AcceptSocket = (Socket) null;
      tcpSocket.Status = TcpStatus.WaitingDataLength;
      this._incomingConnections.Enqueue(tcpSocket);
      this._currentlyAcceptingClients = false;
    }

    private void EnqueueMessage(MessageBuffer messageBuffer) => this._writeNetworkMessageQueue.Enqueue(messageBuffer);

    internal void SendDisconnectMessage() => this.EnqueueMessage(this._disconnectMessageBuffer);

    internal void SendPeerAliveMessage() => this.EnqueueMessage(this._peerAliveMessageBuffer);

    internal void SendMessage(MessageBuffer messageBuffer) => this.EnqueueMessage(messageBuffer);

    internal void Close()
    {
      if (this.Status == TcpStatus.SocketClosed)
      {
        Debug.Print("Socket already closed.");
      }
      else
      {
        this.Status = TcpStatus.SocketClosed;
        if (this._dotNetSocket != null)
        {
          try
          {
            if (this._dotNetSocket.Available > 0)
              Debug.Print("Closing socket but there were " + (object) this._dotNetSocket.Available + " bytes data");
          }
          catch
          {
          }
          this._dotNetSocket.Close(0);
          Debug.Print("Socket " + (object) this._uniqueSocketId + " closed and destroyed!");
        }
        this._dotNetSocket = (Socket) null;
        if (this._socketAsyncEventArgsRead != null)
        {
          this._socketAsyncEventArgsRead.Dispose();
          this._socketAsyncEventArgsRead = (SocketAsyncEventArgs) null;
        }
        if (this._socketAsyncEventArgsWrite != null)
        {
          this._socketAsyncEventArgsWrite.Dispose();
          this._socketAsyncEventArgsWrite = (SocketAsyncEventArgs) null;
        }
        if (this._socketAsyncEventArgsListener != null)
        {
          this._socketAsyncEventArgsListener.Dispose();
          this._socketAsyncEventArgsListener = (SocketAsyncEventArgs) null;
        }
        if (this._dotNetSocket == null)
          return;
        this._dotNetSocket.Dispose();
        this._dotNetSocket = (Socket) null;
      }
    }

    internal event TcpMessageReceiverDelegate MessageReceived;

    internal event TcpCloseDelegate Closed;
  }
}
