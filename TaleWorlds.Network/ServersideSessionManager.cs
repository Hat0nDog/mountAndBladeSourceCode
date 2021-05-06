// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.ServersideSessionManager
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace TaleWorlds.Network
{
  public abstract class ServersideSessionManager
  {
    private int _readWriteThreadCount = 1;
    private ServersideSessionManager.ThreadType _threadType;
    private ushort _listenPort;
    private TcpSocket _serverSocket;
    private int _lastUniqueClientId;
    private Thread _serverThread;
    private long _lastPeerAliveCheck;
    private List<ConcurrentQueue<IncomingServerSessionMessage>> _incomingMessages;
    private List<ConcurrentDictionary<int, ServersideSession>> _peers;
    private List<ConcurrentDictionary<int, ServersideSession>> _disconnectedPeers;
    private List<Thread> _readerThreads;
    private List<Thread> _writerThreads;
    private List<Thread> _singleThreads;

    public float PeerAliveCoeff { get; set; }

    protected ServersideSessionManager()
    {
      this._readerThreads = new List<Thread>();
      this._writerThreads = new List<Thread>();
      this._singleThreads = new List<Thread>();
      this._peers = new List<ConcurrentDictionary<int, ServersideSession>>();
      this.PeerAliveCoeff = 3f;
      this._incomingMessages = new List<ConcurrentQueue<IncomingServerSessionMessage>>();
      this._disconnectedPeers = new List<ConcurrentDictionary<int, ServersideSession>>();
    }

    public void Activate(
      ushort port,
      ServersideSessionManager.ThreadType threadType = ServersideSessionManager.ThreadType.Single,
      int readWriteThreadCount = 1)
    {
      this._threadType = threadType;
      this._readWriteThreadCount = readWriteThreadCount;
      this._listenPort = port;
      this._serverSocket = new TcpSocket();
      this._serverSocket.Listen((int) this._listenPort);
      if (this._threadType == ServersideSessionManager.ThreadType.Single)
      {
        this._peers.Add(new ConcurrentDictionary<int, ServersideSession>(this._readWriteThreadCount * 3, 8192));
        this._incomingMessages.Add(new ConcurrentQueue<IncomingServerSessionMessage>());
        this._disconnectedPeers.Add(new ConcurrentDictionary<int, ServersideSession>());
        this._readWriteThreadCount = 1;
        this._serverThread = new Thread(new ThreadStart(this.ProcessSingle));
        this._serverThread.IsBackground = true;
        this._serverThread.Name = this.ToString() + " - Server Thread";
        this._serverThread.Start();
      }
      else
      {
        for (int index = 0; index < this._readWriteThreadCount; ++index)
        {
          this._peers.Add(new ConcurrentDictionary<int, ServersideSession>(this._readWriteThreadCount * 4, 8192));
          this._incomingMessages.Add(new ConcurrentQueue<IncomingServerSessionMessage>());
          this._disconnectedPeers.Add(new ConcurrentDictionary<int, ServersideSession>());
        }
        for (int index = 0; index < this._readWriteThreadCount; ++index)
        {
          if (this._threadType == ServersideSessionManager.ThreadType.MultipleSeperateIOAndListener)
          {
            Thread thread1 = new Thread(new ParameterizedThreadStart(this.ProcessRead))
            {
              IsBackground = true,
              Name = this.ToString() + " - Server Reader Thread - " + (object) index
            };
            thread1.IsBackground = true;
            thread1.Start((object) index);
            Thread thread2 = new Thread(new ParameterizedThreadStart(this.ProcessWriter))
            {
              IsBackground = true,
              Name = this.ToString() + " - Server Writer Thread" + (object) index
            };
            thread2.IsBackground = true;
            thread2.Start((object) index);
            this._readerThreads.Add(thread1);
            this._writerThreads.Add(thread2);
          }
          else
          {
            Thread thread = new Thread(new ParameterizedThreadStart(this.ProcessReaderWriter));
            thread.Name = this.ToString() + " - Server ReaderWriter Thread - " + (object) index;
            thread.IsBackground = true;
            thread.Start((object) index);
            this._singleThreads.Add(thread);
          }
        }
        this._serverThread = new Thread(new ThreadStart(this.ProcessListener));
        this._serverThread.IsBackground = true;
        this._serverThread.Name = this.ToString() + " - Server Listener Thread";
        this._serverThread.Start();
      }
    }

    private void ProcessRead(object indexObject)
    {
      int index = (int) indexObject;
      TickManager tickManager = new TickManager(5000, (TickManager.TickDelegate) (() =>
      {
        foreach (ServersideSession serversideSession in (IEnumerable<ServersideSession>) this._peers[index].Values)
          serversideSession?.Socket.ProcessRead();
      }));
      while (true)
        tickManager.Tick();
    }

    private void ProcessWriter(object indexObject)
    {
      int index = (int) indexObject;
      TickManager tickManager = new TickManager(5000, (TickManager.TickDelegate) (() =>
      {
        foreach (ServersideSession serversideSession in (IEnumerable<ServersideSession>) this._peers[index].Values)
          serversideSession?.Socket.ProcessWrite();
      }));
      while (true)
        tickManager.Tick();
    }

    private void ProcessReaderWriter(object indexObject)
    {
      int index = (int) indexObject;
      TickManager tickManager = new TickManager(5000, (TickManager.TickDelegate) (() =>
      {
        foreach (ServersideSession serversideSession in (IEnumerable<ServersideSession>) this._peers[index].Values)
        {
          if (serversideSession != null)
          {
            serversideSession.Socket.ProcessWrite();
            serversideSession.Socket.ProcessRead();
          }
        }
      }));
      while (true)
        tickManager.Tick();
    }

    private void ProcessListener()
    {
      TickManager tickManager = new TickManager(500, (TickManager.TickDelegate) (() => this._serverSocket.CheckAcceptClient()));
      while (true)
        tickManager.Tick();
    }

    private void ProcessSingle()
    {
      TickManager tickManager = new TickManager(5000, (TickManager.TickDelegate) (() =>
      {
        foreach (ServersideSession serversideSession in (IEnumerable<ServersideSession>) this._peers[0].Values)
        {
          if (serversideSession != null)
          {
            serversideSession.Socket.ProcessWrite();
            serversideSession.Socket.ProcessRead();
          }
        }
        this._serverSocket.CheckAcceptClient();
      }));
      while (true)
        tickManager.Tick();
    }

    private void RemovePeer(int peerNo)
    {
      ServersideSession peer = (ServersideSession) null;
      if (!this._peers[peerNo % this._readWriteThreadCount].TryRemove(peerNo, out peer))
        return;
      peer.Socket.Close();
      peer.OnDisconnected();
      this.OnRemoveConnection(peer);
      this._disconnectedPeers[peerNo % this._readWriteThreadCount].TryRemove(peer.Index, out peer);
    }

    public ServersideSession GetPeer(int peerIndex)
    {
      ServersideSession serversideSession = (ServersideSession) null;
      this._peers[peerIndex % this._readWriteThreadCount].TryGetValue(peerIndex, out serversideSession);
      return serversideSession;
    }

    public virtual void Tick()
    {
      this.IncomingConnectionsTick();
      this.MessagingTick();
      this.PeerAliveCheckTick();
      this.HandleRemovedPeersTick();
    }

    private void IncomingConnectionsTick()
    {
      TcpSocket incomingConnection = this._serverSocket.GetLastIncomingConnection();
      if (incomingConnection == null)
        return;
      ServersideSession serversideSession = this.OnNewConnection();
      ++this._lastUniqueClientId;
      serversideSession.InitializeSocket(this._lastUniqueClientId, incomingConnection);
      this._peers[this._lastUniqueClientId % this._readWriteThreadCount].TryAdd(this._lastUniqueClientId, serversideSession);
    }

    private void MessagingTick()
    {
      foreach (ConcurrentQueue<IncomingServerSessionMessage> incomingMessage in this._incomingMessages)
      {
        int count = incomingMessage.Count;
        for (int peerNo = 0; peerNo < count; ++peerNo)
        {
          IncomingServerSessionMessage result = (IncomingServerSessionMessage) null;
          incomingMessage.TryDequeue(out result);
          ServersideSession peer = result.Peer;
          NetworkMessage networkMessage = result.NetworkMessage;
          try
          {
            networkMessage.BeginRead();
            byte id = networkMessage.ReadByte();
            if (!peer.ContainsMessageHandler(id))
            {
              networkMessage.ResetRead();
              peer.OnMessageReceived((INetworkMessageReader) networkMessage);
            }
            else
            {
              MessageContract messageContract = MessageContract.CreateMessageContract(peer.GetMessageContractType(id));
              messageContract.DeserializeFromNetworkMessage((INetworkMessageReader) networkMessage);
              peer.HandleMessage(messageContract);
            }
          }
          catch (Exception ex)
          {
            this.RemovePeer(peerNo);
          }
        }
      }
    }

    private void PeerAliveCheckTick()
    {
      if ((long) Environment.TickCount <= this._lastPeerAliveCheck + 3000L)
        return;
      foreach (ConcurrentDictionary<int, ServersideSession> peer in this._peers)
      {
        foreach (KeyValuePair<int, ServersideSession> keyValuePair in peer)
        {
          int key = keyValuePair.Key;
          if ((double) ((Environment.TickCount - keyValuePair.Value.Socket.LastMessageArrivalTime) / 1000) > (double) this.PeerAliveCoeff * 5.0)
            this.RemovePeer(key);
        }
      }
      this._lastPeerAliveCheck = (long) Environment.TickCount;
    }

    private void HandleRemovedPeersTick()
    {
      foreach (ConcurrentDictionary<int, ServersideSession> disconnectedPeer in this._disconnectedPeers)
      {
        foreach (ServersideSession serversideSession in (IEnumerable<ServersideSession>) disconnectedPeer.Values)
          this.RemovePeer(serversideSession.Index);
      }
    }

    internal void AddIncomingMessage(IncomingServerSessionMessage incomingMessage) => this._incomingMessages[incomingMessage.Peer.Index % this._readWriteThreadCount].Enqueue(incomingMessage);

    internal void AddDisconnectedPeer(ServersideSession peer) => this._disconnectedPeers[peer.Index % this._readWriteThreadCount].TryAdd(peer.Index, peer);

    protected abstract ServersideSession OnNewConnection();

    protected abstract void OnRemoveConnection(ServersideSession peer);

    public enum ThreadType
    {
      Single,
      MultipleIOAndListener,
      MultipleSeperateIOAndListener,
    }
  }
}
