// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.ClientsideSession
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.Collections.Concurrent;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
  public abstract class ClientsideSession : NetworkSession
  {
    private bool _connectionResultHandled;
    private Thread _thread;
    private ConcurrentQueue<MessageBuffer> _incomingMessages;
    private bool _useSessionThread;

    protected void SendMessagePeerAlive() => this.Socket.SendPeerAliveMessage();

    protected internal override void OnDisconnected()
    {
    }

    public int Port { get; set; }

    protected ClientsideSession() => this._incomingMessages = new ConcurrentQueue<MessageBuffer>();

    public virtual void Connect(string ip, int port, bool useSessionThread = true)
    {
      this._useSessionThread = useSessionThread;
      this.Socket = new TcpSocket();
      this.Port = port;
      this.Socket.MessageReceived += new TcpMessageReceiverDelegate(this.OnSocketMessageReceived);
      this.Socket.Connect(ip, port);
      if (!this._useSessionThread)
        return;
      this._thread = new Thread(new ThreadStart(this.Process));
      this._thread.IsBackground = true;
      this._thread.Name = this.ToString() + " - Client Thread";
      this._thread.Start();
    }

    private void OnSocketMessageReceived(MessageBuffer messageBuffer) => this._incomingMessages.Enqueue(messageBuffer);

    public void Process()
    {
      while (this.ProcessTick())
        Thread.Sleep(1);
    }

    private bool ProcessTick()
    {
      this.Socket.ProcessWrite();
      this.Socket.ProcessRead();
      if (this.Socket != null && this.Socket.IsConnected && (double) ((Environment.TickCount - this.Socket.LastMessageSentTime) / 1000) > 5.0)
        this.SendMessagePeerAlive();
      return this.Socket.Status != TcpStatus.SocketClosed && this.Socket.Status != TcpStatus.ConnectionClosed;
    }

    public override void Tick()
    {
      if (this.Socket == null)
        return;
      if (this.Socket.IsConnected)
      {
        if (!this._connectionResultHandled)
        {
          Debug.Print("Client connected! Connection result handle begin.");
          this.OnConnected();
          this._connectionResultHandled = true;
        }
        if (!this._useSessionThread)
          this.ProcessTick();
        int count = this._incomingMessages.Count;
        for (int index = 0; index < count; ++index)
        {
          MessageBuffer result = (MessageBuffer) null;
          this._incomingMessages.TryDequeue(out result);
          NetworkMessage forReading = NetworkMessage.CreateForReading(result);
          forReading.BeginRead();
          byte id = forReading.ReadByte();
          Type messageContractType = this.GetMessageContractType(id);
          MessageContract messageContract = MessageContract.CreateMessageContract(messageContractType);
          Debug.Print("Message with id: " + (object) id + " / contract: " + (object) messageContractType + " received from server");
          messageContract.DeserializeFromNetworkMessage((INetworkMessageReader) forReading);
          this.HandleMessage(messageContract);
        }
      }
      else
      {
        if (this.Socket.Status != TcpStatus.ConnectionClosed)
          return;
        if (!this._connectionResultHandled)
        {
          Debug.Print("ClientTcpSession can't connect!");
          this._connectionResultHandled = true;
          this.OnCantConnect();
        }
        else
        {
          Debug.Print("Peer disconnected!");
          this.OnDisconnected();
        }
        this.Socket.Close();
        this._connectionResultHandled = false;
      }
    }
  }
}
