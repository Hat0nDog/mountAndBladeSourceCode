// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SetWeaponAmmoData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SetWeaponAmmoData : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public EquipmentIndex WeaponEquipmentIndex { get; private set; }

    public EquipmentIndex AmmoEquipmentIndex { get; private set; }

    public short Ammo { get; private set; }

    public SetWeaponAmmoData(
      Agent agent,
      EquipmentIndex weaponEquipmentIndex,
      EquipmentIndex ammoEquipmentIndex,
      short ammo)
    {
      this.Agent = agent;
      this.WeaponEquipmentIndex = weaponEquipmentIndex;
      this.AmmoEquipmentIndex = ammoEquipmentIndex;
      this.Ammo = ammo;
    }

    public SetWeaponAmmoData()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid);
      this.WeaponEquipmentIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      this.AmmoEquipmentIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.WieldSlotCompressionInfo, ref bufferReadValid);
      this.Ammo = (short) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemDataCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket((int) this.WeaponEquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this.AmmoEquipmentIndex, CompressionMission.WieldSlotCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this.Ammo, CompressionMission.ItemDataCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.EquipmentDetailed;

    protected override string OnGetLogFormat() => "Set ammo: " + (object) this.Ammo + " for weapon with EquipmentIndex: " + (object) this.WeaponEquipmentIndex + " on Agent with name: " + this.Agent.Name + " and agent-index: " + (object) this.Agent.Index;
  }
}
