// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.RemovePeerComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class RemovePeerComponent : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public uint ComponentId { get; private set; }

    public RemovePeerComponent(NetworkCommunicator peer, uint componentId)
    {
      this.Peer = peer;
      this.ComponentId = componentId;
    }

    public RemovePeerComponent()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteUintToPacket(this.ComponentId, CompressionBasic.PeerComponentCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.ComponentId = GameNetworkMessage.ReadUintFromPacket(CompressionBasic.PeerComponentCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Peers;

    protected override string OnGetLogFormat() => "Remove component with ID: " + (object) this.ComponentId + " from peer: " + this.Peer.UserName + " with peer-index: " + (object) this.Peer.Index;
  }
}
