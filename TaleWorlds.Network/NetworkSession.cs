// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.NetworkSession
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;

namespace TaleWorlds.Network
{
  public abstract class NetworkSession
  {
    public const double AliveMessageIntervalInSecs = 5.0;
    private MessageContractHandlerManager _messageContractHandlerManager;
    private TcpSocket _socket;

    protected NetworkSession() => this._messageContractHandlerManager = new MessageContractHandlerManager();

    public void SendDisconnectMessage() => this.Socket.SendDisconnectMessage();

    protected internal virtual void OnConnected()
    {
    }

    protected internal virtual void OnSocketSet()
    {
    }

    protected internal virtual void OnDisconnected()
    {
    }

    protected internal virtual void OnCantConnect()
    {
    }

    protected internal virtual void OnMessageReceived(INetworkMessageReader networkMessage)
    {
    }

    public virtual void Tick()
    {
    }

    internal void HandleMessage(MessageContract messageContract) => this._messageContractHandlerManager.HandleMessage(messageContract);

    public void AddMessageHandler<T>(MessageContractHandlerDelegate<T> handler) where T : MessageContract => this._messageContractHandlerManager.AddMessageHandler<T>(handler);

    internal Type GetMessageContractType(byte id) => this._messageContractHandlerManager.GetMessageContractType(id);

    internal bool ContainsMessageHandler(byte id) => this._messageContractHandlerManager.ContainsMessageHandler(id);

    public void SendMessage(MessageContract message)
    {
      if (!this.IsActive)
        return;
      NetworkMessage forWriting = NetworkMessage.CreateForWriting();
      forWriting.BeginWrite();
      forWriting.Write(message);
      forWriting.FinalizeWrite();
      int dataLength = forWriting.DataLength;
      forWriting.DataLength = dataLength;
      forWriting.UpdateHeader();
      this.Socket.SendMessage(forWriting.MessageBuffer);
    }

    protected void SendPlainMessage(MessageContract message)
    {
      NetworkMessage forWriting = NetworkMessage.CreateForWriting();
      forWriting.BeginWrite();
      forWriting.Write(message);
      forWriting.FinalizeWrite();
      forWriting.UpdateHeader();
      this.Socket.SendMessage(forWriting.MessageBuffer);
    }

    public bool IsActive => this.Socket != null;

    internal TcpSocket Socket
    {
      get => this._socket;
      set
      {
        this._socket = value;
        this.OnSocketSet();
      }
    }

    public string Address => this.Socket.IPAddress;

    public int LastMessageSentTime => this.Socket.LastMessageSentTime;

    public bool IsConnected => this.Socket != null && this.Socket.IsConnected;

    public delegate void ComponentMessageHandlerDelegate(NetworkMessage networkMessage);
  }
}
