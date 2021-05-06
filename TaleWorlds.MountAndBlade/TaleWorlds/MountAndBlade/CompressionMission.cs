// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CompressionMission
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public static class CompressionMission
  {
    public static readonly CompressionInfo.Float DebugScaleValueCompressionInfo = new CompressionInfo.Float(0.5f, 1.5f, 13);
    public static CompressionInfo.Integer AgentCompressionInfo = new CompressionInfo.Integer(-1, 11);
    public static CompressionInfo.Integer WeaponAttachmentIndexCompressionInfo = new CompressionInfo.Integer(0, 8);
    public static CompressionInfo.Integer AgentOffsetCompressionInfo = new CompressionInfo.Integer(0, 7);
    public static CompressionInfo.Integer AgentHealthCompressionInfo = new CompressionInfo.Integer(-1, 11);
    public static CompressionInfo.Integer AgentControllerCompressionInfo = new CompressionInfo.Integer(0, 2, true);
    public static CompressionInfo.Integer TeamCompressionInfo = new CompressionInfo.Integer(-1, 10);
    public static CompressionInfo.Integer TeamSideCompressionInfo = new CompressionInfo.Integer(-1, 4);
    public static CompressionInfo.Integer RoundEndReasonCompressionInfo = new CompressionInfo.Integer(-1, 2, true);
    public static CompressionInfo.Integer TeamScoreCompressionInfo = new CompressionInfo.Integer(-120000, 120000, true);
    public static CompressionInfo.Integer FactionCompressionInfo = new CompressionInfo.Integer(0, 4);
    public static CompressionInfo.Integer MissionOrderTypeCompressionInfo = new CompressionInfo.Integer(-1, 5);
    public static CompressionInfo.Integer MissionRoundCountCompressionInfo = new CompressionInfo.Integer(-1, 128, true);
    public static CompressionInfo.Integer MissionRoundStateCompressionInfo = new CompressionInfo.Integer(-1, 4);
    public static CompressionInfo.Integer RoundTimeCompressionInfo = new CompressionInfo.Integer(0, MultiplayerOptions.OptionType.RoundTimeLimit.GetMaximumValue(), true);
    public static CompressionInfo.Integer SelectedTroopIndexCompressionInfo = new CompressionInfo.Integer(-1, 15, true);
    public static CompressionInfo.Integer MissileCompressionInfo = new CompressionInfo.Integer(0, 10);
    public static CompressionInfo.Float MissileSpeedCompressionInfo = new CompressionInfo.Float(0.0f, 12, 0.05f);
    public static CompressionInfo.Integer MissileCollisionReactionCompressionInfo = new CompressionInfo.Integer(0, 4, true);
    public static CompressionInfo.Integer FlagCapturePointIndexCompressionInfo = new CompressionInfo.Integer(0, 3);
    public static CompressionInfo.Integer FlagpoleIndexCompressionInfo = new CompressionInfo.Integer(0, 5, true);
    public static CompressionInfo.Float FlagCapturePointDurationCompressionInfo = new CompressionInfo.Float(-1f, 14, 0.01f);
    public static CompressionInfo.Float FlagProgressCompressionInfo = new CompressionInfo.Float(-1f, 1f, 12);
    public static CompressionInfo.Float FlagClassicProgressCompressionInfo = new CompressionInfo.Float(0.0f, 1f, 11);
    public static CompressionInfo.Integer FlagDirectionEnumCompressionInfo = new CompressionInfo.Integer(-1, 2, true);
    public static CompressionInfo.Float FlagSpeedCompressionInfo = new CompressionInfo.Float(-1f, 14, 0.01f);
    public static CompressionInfo.Integer FlagCaptureResultCompressionInfo = new CompressionInfo.Integer(0, 3, true);
    public static CompressionInfo.Integer UsableGameObjectDestructionStateCompressionInfo = new CompressionInfo.Integer(0, 3);
    public static CompressionInfo.Float UsableGameObjectHealthCompressionInfo = new CompressionInfo.Float(-1f, 18, 0.1f);
    public static CompressionInfo.Float UsableGameObjectBlowMagnitude = new CompressionInfo.Float(0.0f, DestructableComponent.MaxBlowMagnitude, 8);
    public static CompressionInfo.Float UsableGameObjectBlowDirection = new CompressionInfo.Float(-1f, 1f, 7);
    public static CompressionInfo.Float CapturePointProgressCompressionInfo = new CompressionInfo.Float(0.0f, 1f, 10);
    public static CompressionInfo.Integer ItemSlotCompressionInfo = new CompressionInfo.Integer(0, 4, true);
    public static CompressionInfo.Integer WieldSlotCompressionInfo = new CompressionInfo.Integer(-1, 4, true);
    public static CompressionInfo.Integer ItemDataCompressionInfo = new CompressionInfo.Integer(0, 10);
    public static CompressionInfo.Integer WeaponReloadPhaseCompressionInfo = new CompressionInfo.Integer(0, 2);
    public static CompressionInfo.Integer WeaponUsageIndexCompressionInfo = new CompressionInfo.Integer(0, 2);
    public static CompressionInfo.Integer UsageDirectionCompressionInfo = new CompressionInfo.Integer(-1, 10, true);
    public static CompressionInfo.Float SpawnedItemVelocityCompressionInfo = new CompressionInfo.Float(-100f, 100f, 12);
    public static CompressionInfo.Float SpawnedItemAngularVelocityCompressionInfo = new CompressionInfo.Float(-100f, 100f, 12);
    public static CompressionInfo.Integer SpawnedItemWeaponSpawnFlagCompressionInfo = new CompressionInfo.Integer(0, (int) byte.MaxValue, true);
    public static CompressionInfo.Integer RangedSiegeWeaponAmmoCompressionInfo = new CompressionInfo.Integer(0, 7);
    public static CompressionInfo.Integer RangedSiegeWeaponAmmoIndexCompressionInfo = new CompressionInfo.Integer(0, 3);
    public static CompressionInfo.Integer RangedSiegeWeaponStateCompressionInfo = new CompressionInfo.Integer(0, 9, true);
    public static CompressionInfo.Integer SiegeLadderStateCompressionInfo = new CompressionInfo.Integer(0, 10, true);
    public static CompressionInfo.Integer BatteringRamStateCompressionInfo = new CompressionInfo.Integer(0, 3, true);
    public static CompressionInfo.Integer SiegeLadderAnimationStateCompressionInfo = new CompressionInfo.Integer(0, 3, true);
    public static CompressionInfo.Float SiegeMachineComponentAngularSpeedCompressionInfo = new CompressionInfo.Float(-20f, 20f, 12);
    public static CompressionInfo.Integer SiegeTowerGateStateCompressionInfo = new CompressionInfo.Integer(0, 4, true);
    public static CompressionInfo.Integer NumberOfPacesCompressionInfo = new CompressionInfo.Integer(0, 3);
    public static CompressionInfo.Float WalkingSpeedLimitCompressionInfo = new CompressionInfo.Float(-0.01f, 9, 0.01f);
    public static CompressionInfo.Float StepSizeCompressionInfo = new CompressionInfo.Float(-0.01f, 7, 0.01f);
    public static CompressionInfo.Integer BoneIndexCompressionInfo = new CompressionInfo.Integer(0, 64, true);
    public static CompressionInfo.Integer AgentPrefabComponentIndexCompressionInfo = new CompressionInfo.Integer(0, 16, true);
    public static CompressionInfo.Integer MultiplayerNotificationCompressionInfo = new CompressionInfo.Integer(0, MultiplayerGameNotificationsComponent.NotificationCount, true);
    public static CompressionInfo.Integer MultiplayerNotificationParameterCompressionInfo = new CompressionInfo.Integer(-1, (int) byte.MaxValue, true);
    public static CompressionInfo.Integer PerkListIndexCompressionInfo = new CompressionInfo.Integer(0, 2);
    public static CompressionInfo.Integer PerkIndexCompressionInfo = new CompressionInfo.Integer(0, 4);
    public static CompressionInfo.Float FlagDominationMoraleCompressionInfo = new CompressionInfo.Float(-1f, 8, 0.01f);
    public static CompressionInfo.Integer TdmGoldChangeCompressionInfo;
    public static CompressionInfo.Integer TdmGoldGainTypeCompressionInfo;
    public static CompressionInfo.Integer SiegeMoraleCompressionInfo = new CompressionInfo.Integer(0, 1440, true);
    public static CompressionInfo.Integer SiegeMoralePerFlagCompressionInfo = new CompressionInfo.Integer(0, 90, true);
    public static CompressionInfo.Integer ActionSetCompressionInfo;
    public static CompressionInfo.Integer MonsterUsageSetCompressionInfo;

    static CompressionMission()
    {
      CompressionMission.TdmGoldChangeCompressionInfo = new CompressionInfo.Integer(0, 2000, true);
      CompressionMission.TdmGoldGainTypeCompressionInfo = new CompressionInfo.Integer(0, 11);
    }
  }
}
