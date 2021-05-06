// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.EquipWeaponFromSpawnedItemEntity
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class EquipWeaponFromSpawnedItemEntity : GameNetworkMessage
  {
    public SpawnedItemEntity SpawnedItemEntity { get; private set; }

    public EquipmentIndex SlotIndex { get; private set; }

    public Agent Agent { get; private set; }

    public bool RemoveWeapon { get; private set; }

    public EquipWeaponFromSpawnedItemEntity(
      Agent a,
      EquipmentIndex slot,
      SpawnedItemEntity spawnedItemEntity,
      bool removeWeapon)
    {
      this.Agent = a;
      this.SlotIndex = slot;
      this.SpawnedItemEntity = spawnedItemEntity;
      this.RemoveWeapon = removeWeapon;
    }

    public EquipWeaponFromSpawnedItemEntity()
    {
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteMissionObjectReferenceToPacket((MissionObject) this.SpawnedItemEntity);
      GameNetworkMessage.WriteIntToPacket((int) this.SlotIndex, CompressionMission.ItemSlotCompressionInfo);
      GameNetworkMessage.WriteBoolToPacket(this.RemoveWeapon);
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid, true);
      this.SpawnedItemEntity = GameNetworkMessage.ReadMissionObjectReferenceFromPacket(ref bufferReadValid) as SpawnedItemEntity;
      this.SlotIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      this.RemoveWeapon = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      return bufferReadValid;
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items | MultiplayerMessageFilter.AgentsDetailed;

    protected override string OnGetLogFormat() => "EquipWeaponFromSpawnedItemEntity with entity name: " + (this.SpawnedItemEntity != null ? ((NativeObject) this.SpawnedItemEntity.GameEntity != (NativeObject) null ? (object) this.SpawnedItemEntity.GameEntity.Name : (object) "null entity") : (object) "null spawned item") + " to SlotIndex: " + (object) this.SlotIndex + " on agent: " + this.Agent.Name + " with index: " + (object) this.Agent.Index + " RemoveWeapon: " + this.RemoveWeapon.ToString();
  }
}
