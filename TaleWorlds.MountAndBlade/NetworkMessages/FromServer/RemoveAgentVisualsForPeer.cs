// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RemoveAgentVisualsForPeer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class RemoveAgentVisualsForPeer : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public RemoveAgentVisualsForPeer(NetworkCommunicator peer) => this.Peer = peer;

    public RemoveAgentVisualsForPeer()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite() => GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Agents;

    protected override string OnGetLogFormat() => "Removing all AgentVisuals for peer: " + this.Peer.UserName;
  }
}
