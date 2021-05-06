// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.EquipEquipmentToPeer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  internal sealed class EquipEquipmentToPeer : GameNetworkMessage
  {
    public NetworkCommunicator Peer { get; private set; }

    public Equipment Equipment { get; private set; }

    public EquipEquipmentToPeer(NetworkCommunicator peer, Equipment equipment)
    {
      this.Peer = peer;
      this.Equipment = new Equipment();
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; ++equipmentIndex)
        this.Equipment[equipmentIndex] = equipment.GetEquipmentFromSlot(equipmentIndex);
    }

    public EquipEquipmentToPeer()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
      if (bufferReadValid)
      {
        this.Equipment = new Equipment();
        for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; ++equipmentIndex)
        {
          if (bufferReadValid)
            this.Equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, ModuleNetworkData.ReadItemReferenceFromPacket(MBObjectManager.Instance, ref bufferReadValid));
        }
      }
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.Peer);
      for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; ++equipmentIndex)
        ModuleNetworkData.WriteItemReferenceToPacket(this.Equipment.GetEquipmentFromSlot(equipmentIndex));
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Equipment;

    protected override string OnGetLogFormat() => "Equip equipment to peer: " + this.Peer.UserName + " with peer-index:" + (object) this.Peer.Index;
  }
}
