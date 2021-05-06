// Decompiled with JetBrains decompiler
// Type: NetworkMessages.FromServer.SpawnWeaponAsDropFromAgent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace NetworkMessages.FromServer
{
  [DefineGameNetworkMessageType(GameNetworkMessageSendType.FromServer)]
  public sealed class SpawnWeaponAsDropFromAgent : GameNetworkMessage
  {
    public Agent Agent { get; private set; }

    public EquipmentIndex EquipmentIndex { get; private set; }

    public Vec3 Velocity { get; private set; }

    public Vec3 AngularVelocity { get; private set; }

    public Mission.WeaponSpawnFlags WeaponSpawnFlags { get; private set; }

    public int ForcedIndex { get; private set; }

    public SpawnWeaponAsDropFromAgent(
      Agent agent,
      EquipmentIndex equipmentIndex,
      Vec3 velocity,
      Vec3 angularVelocity,
      Mission.WeaponSpawnFlags weaponSpawnFlags,
      int forcedIndex)
    {
      this.Agent = agent;
      this.EquipmentIndex = equipmentIndex;
      this.Velocity = velocity;
      this.AngularVelocity = angularVelocity;
      this.WeaponSpawnFlags = weaponSpawnFlags;
      this.ForcedIndex = forcedIndex;
    }

    public SpawnWeaponAsDropFromAgent()
    {
    }

    protected override bool OnRead()
    {
      bool bufferReadValid = true;
      this.Agent = GameNetworkMessage.ReadAgentReferenceFromPacket(ref bufferReadValid, true);
      this.EquipmentIndex = (EquipmentIndex) GameNetworkMessage.ReadIntFromPacket(CompressionMission.ItemSlotCompressionInfo, ref bufferReadValid);
      this.WeaponSpawnFlags = (Mission.WeaponSpawnFlags) GameNetworkMessage.ReadIntFromPacket(CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo, ref bufferReadValid);
      if (this.WeaponSpawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithPhysics))
      {
        this.Velocity = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.SpawnedItemVelocityCompressionInfo, ref bufferReadValid);
        this.AngularVelocity = GameNetworkMessage.ReadVec3FromPacket(CompressionMission.SpawnedItemAngularVelocityCompressionInfo, ref bufferReadValid);
      }
      else
      {
        this.Velocity = Vec3.Zero;
        this.AngularVelocity = Vec3.Zero;
      }
      this.ForcedIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.MissionObjectIDCompressionInfo, ref bufferReadValid);
      return bufferReadValid;
    }

    protected override void OnWrite()
    {
      GameNetworkMessage.WriteAgentReferenceToPacket(this.Agent);
      GameNetworkMessage.WriteIntToPacket((int) this.EquipmentIndex, CompressionMission.ItemSlotCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this.WeaponSpawnFlags, CompressionMission.SpawnedItemWeaponSpawnFlagCompressionInfo);
      if (this.WeaponSpawnFlags.HasAnyFlag<Mission.WeaponSpawnFlags>(Mission.WeaponSpawnFlags.WithPhysics))
      {
        GameNetworkMessage.WriteVec3ToPacket(this.Velocity, CompressionMission.SpawnedItemVelocityCompressionInfo);
        GameNetworkMessage.WriteVec3ToPacket(this.AngularVelocity, CompressionMission.SpawnedItemAngularVelocityCompressionInfo);
      }
      GameNetworkMessage.WriteIntToPacket(this.ForcedIndex, CompressionBasic.MissionObjectIDCompressionInfo);
    }

    protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Items;

    protected override string OnGetLogFormat() => "Spawn Weapon from agent with index: " + (object) this.Agent.Index + " from equipment index: " + (object) this.EquipmentIndex + ", and with ID: " + (object) this.ForcedIndex;
  }
}
