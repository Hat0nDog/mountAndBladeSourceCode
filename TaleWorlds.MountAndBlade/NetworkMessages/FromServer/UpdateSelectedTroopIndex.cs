// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.UpdateSelectedTroopIndex
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class UpdateSelectedTroopIndex : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public int SelectedTroopIndex { get; private set; }

    public UpdateSelectedTroopIndex(NetworkCommunicator peer, int selectedTroopIndex)
    {
      this.Peer = peer;
      this.SelectedTroopIndex = selectedTroopIndex;
    }

    public UpdateSelectedTroopIndex()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.SelectedTroopIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SelectedTroopIndexCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteIntToPacket(this.SelectedTroopIndex, CompressionMission.SelectedTroopIndexCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Equipment;

    protected override string OnGetLogFormat() => "Update SelectedTroopIndex to: " + (object) this.SelectedTroopIndex + ", on peer: " + this.Peer.UserName + " with peer-index:" + (object) this.Peer.Index;
  }
}
