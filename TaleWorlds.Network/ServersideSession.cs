// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.ServersideSession
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

namespace TaleWorlds.Network
{
  public abstract class ServersideSession : NetworkSession
  {
    public int Index { get; internal set; }

    internal ServersideSessionManager Server { get; private set; }

    protected ServersideSession(ServersideSessionManager server) => this.Server = server;

    protected internal override void OnDisconnected()
    {
    }

    protected internal override void OnConnected()
    {
    }

    protected internal override void OnSocketSet()
    {
    }

    internal void InitializeSocket(int id, TcpSocket socket)
    {
      this.Index = id;
      this.Socket = socket;
      this.Socket.MessageReceived += new TcpMessageReceiverDelegate(this.OnTcpSocketMessageReceived);
      this.Socket.Closed += new TcpCloseDelegate(this.OnTcpSocketClosed);
    }

    private void OnTcpSocketMessageReceived(MessageBuffer messageBuffer) => this.Server.AddIncomingMessage(new IncomingServerSessionMessage()
    {
      Peer = this,
      NetworkMessage = NetworkMessage.CreateForReading(messageBuffer)
    });

    private void OnTcpSocketClosed() => this.Server.AddDisconnectedPeer(this);
  }
}
