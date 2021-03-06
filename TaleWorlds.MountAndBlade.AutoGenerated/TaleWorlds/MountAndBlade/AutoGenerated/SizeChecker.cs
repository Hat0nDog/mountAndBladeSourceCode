// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AutoGenerated.SizeChecker
// Assembly: TaleWorlds.MountAndBlade.AutoGenerated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D02C25D1-9727-49C6-A24A-EE3800F0364C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.AutoGenerated.dll

using System;
using System.Runtime.InteropServices;

namespace TaleWorlds.MountAndBlade.AutoGenerated
{
  internal class SizeChecker
  {
    private static void CheckTypeSizeAux(int managedSize, string name) => MBAPI.IMBBannerlordChecker.GetEngineStructSize(name);

    public static void CheckSharedStructureSizes()
    {
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (AgentSpawnData)), "Agent_spawn_data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (AgentVisualsNativeData)), "Agent_visuals_native_data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (BehaviorValues)), "behavior_values_struct");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (AnimationSystemData)), "Animation_system_data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (AttackCollisionData)), "Attack_collision_data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (HitParticleResultData)), "Hit_particle_result_data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (Blow)), "Blow");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (BlowWeaponRecord)), "Blow_weapon_record");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (SkinGenerationParams)), "Skin_generation_params");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (FaceGenerationParams)), "Face_generation_params");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (KillingBlow)), "Killing_blow");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(Enum.GetUnderlyingType(typeof (AnimFlags))), "Anim_flags");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (DeformKeyData)), "Deform_Key_Data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (BoneBodyTypeData)), "Bone_body_type_data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (NavigationData)), "Navigation_data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (WeaponDataAsNative)), "Weapon_data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (WeaponStatsData)), "Weapon_stats_data");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(Enum.GetUnderlyingType(typeof (Agent.ControllerType))), "Agent_controller_type");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(Enum.GetUnderlyingType(typeof (Agent.GuardMode))), "Agent_guard_mode");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(Enum.GetUnderlyingType(typeof (Agent.UsageDirection))), "Usage_direction");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(Enum.GetUnderlyingType(typeof (Agent.WeaponWieldActionType))), "Weapon_wield_action_type");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (AgentProximityMap.ProximityMapSearchStructInternal)), "Managed_proximity_map_search_struct");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(Enum.GetUnderlyingType(typeof (Mission.MissionCombatType))), "Mission_combat_type");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (Mission.AgentCreationResult)), "Agent_creation_result");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (CompressionInfo.Integer)), "Integer_compression_info");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (CompressionInfo.UnsignedInteger)), "Unsigned_integer_compression_info");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (CompressionInfo.LongInteger)), "Integer64_compression_info");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (CompressionInfo.UnsignedLongInteger)), "Unsigned_integer64_compression_info");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (CompressionInfo.Float)), "Float_compression_info");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (GameNetwork.DebugNetworkPositionCompressionStatisticsStruct)), "Debug_network_position_compression_statistics_struct");
      SizeChecker.CheckTypeSizeAux(Marshal.SizeOf(typeof (GameNetwork.DebugNetworkPacketStatisticsStruct)), "Debug_network_packet_statistics_struct");
    }
  }
}
