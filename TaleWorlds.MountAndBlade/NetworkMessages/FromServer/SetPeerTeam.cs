// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetPeerTeam
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SetPeerTeam : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public Team Team { get; private set; }

    public SetPeerTeam(NetworkCommunicator peer, Team team)
    {
      this.Peer = peer;
      this.Team = team;
    }

    public SetPeerTeam()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.Team = GameNetworkMessage.ReadTeamReferenceFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteTeamReferenceToPacket(this.Team);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Peers;

    protected override string OnGetLogFormat() => "Set Team: " + (object) this.Team + " of NetworkPeer with name: " + this.Peer.UserName + " and peer-index" + (object) this.Peer.Index;
  }
}
