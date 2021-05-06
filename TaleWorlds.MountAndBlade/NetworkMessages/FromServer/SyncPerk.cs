// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SyncPerk
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SyncPerk : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public int PerkListIndex { get; private set; }

    public int PerkIndex { get; private set; }

    public int SelectedTroopIndex { get; private set; }

    public SyncPerk()
    {
    }

    public SyncPerk(
      NetworkCommunicator peer,
      int perkListIndex,
      int perkIndex,
      int selectedTroopIndex = -1)
    {
      this.Peer = peer;
      this.PerkListIndex = perkListIndex;
      this.PerkIndex = perkIndex;
      this.SelectedTroopIndex = selectedTroopIndex;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      GameNetworkMessage.WriteIntToPacket(this.PerkListIndex, CompressionMission.PerkListIndexCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.PerkIndex, CompressionMission.PerkIndexCompressionInfo);
      GameNetworkMessage.WriteIntToPacket(this.SelectedTroopIndex, CompressionMission.SelectedTroopIndexCompressionInfo);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      this.PerkListIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkListIndexCompressionInfo, ref bufferReadValid);
      this.PerkIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.PerkIndexCompressionInfo, ref bufferReadValid);
      this.SelectedTroopIndex = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SelectedTroopIndexCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.GameMode;

    protected override string OnGetLogFormat() => this.Peer.UserName + " has selected the perk #" + (object) this.PerkIndex + " in the list #" + (object) this.PerkListIndex;
  }
}
