// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.AssignFormationToPlayer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class AssignFormationToPlayer : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public FormationClass FormationClass { get; private set; }

    public AssignFormationToPlayer(NetworkCommunicator peer, FormationClass formationClass)
    {
      this.Peer = peer;
      this.FormationClass = formationClass;
    }

    public AssignFormationToPlayer()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.FormationClass = (FormationClass) GameNetworkMessage.ReadIntFromPacket(CompressionOrder.FormationClassCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteIntToPacket((int) this.FormationClass, CompressionOrder.FormationClassCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Formations;

    protected override string OnGetLogFormat() => "Assign formation with index: " + (object) (int) this.FormationClass + " to NetworkPeer with name: " + this.Peer.UserName + " and peer-index" + (object) this.Peer.Index + " and make him captain.";
  }
}
