// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CompressionBasic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  public static class CompressionBasic
  {
    public const float MaxPossibleAbsValueForSecondMaxQuaternionComponent = 0.7071068f;
    public const float MaxPositionZForCompression = 2521f;
    public const float MaxPositionForCompression = 10385f;
    public const float MinPositionForCompression = -100f;
    public static readonly CompressionInfo.Integer PingValueCompressionInfo = new CompressionInfo.Integer(0, 1023, true);
    public static readonly CompressionInfo.Integer LossValueCompressionInfo = new CompressionInfo.Integer(0, 100, true);
    public static readonly CompressionInfo.Integer ServerPerformanceStateCompressionInfo = new CompressionInfo.Integer(0, 2, true);
    public static CompressionInfo.Float PositionCompressionInfo = new CompressionInfo.Float(-100f, 10385f, 22);
    public static CompressionInfo.Float LocalPositionCompressionInfo = new CompressionInfo.Float(-32f, 32f, 16);
    public static CompressionInfo.Float LowResLocalPositionCompressionInfo;
    public static CompressionInfo.Float BigRangeLowResLocalPositionCompressionInfo = new CompressionInfo.Float(-1000f, 1000f, 16);
    public static CompressionInfo.Integer ByteCompressionInfo;
    public static CompressionInfo.Integer PlayerCompressionInfo;
    public static CompressionInfo.UnsignedInteger PeerComponentCompressionInfo;
    public static CompressionInfo.UnsignedInteger GUIDCompressionInfo;
    public static CompressionInfo.Integer FlagsCompressionInfo;
    public static CompressionInfo.Integer GUIDIntCompressionInfo;
    public static CompressionInfo.Integer RPCBufferSizeCompressionInfo;
    public static CompressionInfo.Integer MissionObjectIDCompressionInfo;
    public static CompressionInfo.Float UnitVectorCompressionInfo;
    public static CompressionInfo.Float LowResRadianCompressionInfo;
    public static CompressionInfo.Float RadianCompressionInfo;
    public static CompressionInfo.Float HighResRadianCompressionInfo;
    public static CompressionInfo.Float UltResRadianCompressionInfo;
    public static CompressionInfo.Float ScaleCompressionInfo;
    public static CompressionInfo.Float LowResQuaternionCompressionInfo;
    public static CompressionInfo.Integer OmittedQuaternionComponentIndexCompressionInfo;
    public static CompressionInfo.Float ImpulseCompressionInfo;
    public static CompressionInfo.Integer AnimationKeyCompressionInfo;
    public static CompressionInfo.Float AnimationSpeedCompressionInfo;
    public static CompressionInfo.Float AnimationProgressCompressionInfo;
    public static CompressionInfo.Float VertexAnimationSpeedCompressionInfo;
    public static CompressionInfo.Integer ItemDataCompressionInfo;
    public static CompressionInfo.Integer PercentageCompressionInfo;
    public static CompressionInfo.Integer FloatPercentageCompressionInfo;
    public static CompressionInfo.Integer EntityChildCountCompressionInfo;
    public static CompressionInfo.Integer AgentHitDamageCompressionInfo;
    public static CompressionInfo.Float AgentHitRelativeSpeedCompressionInfo;
    public static CompressionInfo.Integer AgentHitArmorCompressionInfo;
    public static CompressionInfo.Integer AgentHitBoneIndexCompressionInfo;
    public static CompressionInfo.Integer AgentHitBodyPartCompressionInfo;
    public static CompressionInfo.Integer AgentHitDamageTypeCompressionInfo;
    public static CompressionInfo.Integer RoundGoldAmountCompressionInfo;
    public static CompressionInfo.Integer DebugIntNonCompressionInfo;
    public static CompressionInfo.UnsignedLongInteger DebugULongNonCompressionInfo;
    public static CompressionInfo.Float AgentAgeCompressionInfo;
    public static CompressionInfo.Float FaceKeyDataCompressionInfo;
    public static CompressionInfo.Integer PlayerChosenBadgeCompressionInfo;
    public static CompressionInfo.Integer MaxNumberOfPlayersCompressionInfo;
    public static CompressionInfo.Integer PlayerCountLimitForWarmupCompressionInfo;
    public static CompressionInfo.Integer MinNumberOfPlayersForMatchStartCompressionInfo;
    public static CompressionInfo.Integer MapTimeLimitCompressionInfo;
    public static CompressionInfo.Integer RoundTotalCompressionInfo;
    public static CompressionInfo.Integer RoundTimeLimitCompressionInfo;
    public static CompressionInfo.Integer RoundPreparationTimeLimitCompressionInfo;
    public static CompressionInfo.Integer FlagRemovalTimeCompressionInfo;
    public static CompressionInfo.Integer RespawnPeriodCompressionInfo;
    public static CompressionInfo.Integer GoldGainChangePercentageCompressionInfo;
    public static CompressionInfo.Integer SpectatorCameraTypeCompressionInfo;
    public static CompressionInfo.Integer PollAcceptThresholdCompressionInfo;
    public static CompressionInfo.Integer NumberOfBotsTeamCompressionInfo;
    public static CompressionInfo.Integer NumberOfBotsPerFormationCompressionInfo;
    public static CompressionInfo.Integer AutoTeamBalanceLimitCompressionInfo;
    public static CompressionInfo.Integer FriendlyFireDamageCompressionInfo;
    public static CompressionInfo.Integer SiegeEngineSpeedPercentCompressionInfo;
    public static CompressionInfo.Integer ForcedAvatarIndexCompressionInfo;
    public static CompressionInfo.Integer IntermissionStateCompressionInfo;
    public static CompressionInfo.Float IntermissionTimerCompressionInfo;
    public static CompressionInfo.Integer ActionCodeCompressionInfo;
    public static CompressionInfo.Integer AnimationIndexCompressionInfo;
    public static CompressionInfo.Integer CultureIndexCompressionInfo;
    public static CompressionInfo.Integer SoundEventsCompressionInfo;
    public static CompressionInfo.Integer NetworkComponentEventTypeFromServerCompressionInfo;
    public static CompressionInfo.Integer NetworkComponentEventTypeFromClientCompressionInfo;

    static CompressionBasic()
    {
      CompressionBasic.LowResLocalPositionCompressionInfo = new CompressionInfo.Float(-32f, 32f, 12);
      CompressionBasic.ByteCompressionInfo = new CompressionInfo.Integer(0, 8);
      CompressionBasic.PlayerCompressionInfo = new CompressionInfo.Integer(-1, 8);
      CompressionBasic.PeerComponentCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);
      CompressionBasic.GUIDCompressionInfo = new CompressionInfo.UnsignedInteger(0U, 32);
      CompressionBasic.FlagsCompressionInfo = new CompressionInfo.Integer(0, 30);
      CompressionBasic.GUIDIntCompressionInfo = new CompressionInfo.Integer(-1, 31);
      CompressionBasic.RPCBufferSizeCompressionInfo = new CompressionInfo.Integer(0, 30);
      CompressionBasic.MissionObjectIDCompressionInfo = new CompressionInfo.Integer(-1, 4095, true);
      CompressionBasic.UnitVectorCompressionInfo = new CompressionInfo.Float(-1.024f, 10, 1f / 500f);
      CompressionBasic.LowResRadianCompressionInfo = new CompressionInfo.Float(-3.151593f, 3.151593f, 8);
      CompressionBasic.RadianCompressionInfo = new CompressionInfo.Float(-3.151593f, 3.151593f, 10);
      CompressionBasic.HighResRadianCompressionInfo = new CompressionInfo.Float(-3.151593f, 3.151593f, 13);
      CompressionBasic.UltResRadianCompressionInfo = new CompressionInfo.Float(-3.151593f, 3.151593f, 30);
      CompressionBasic.ScaleCompressionInfo = new CompressionInfo.Float(-1f / 1000f, 10, 0.01f);
      CompressionBasic.LowResQuaternionCompressionInfo = new CompressionInfo.Float(-0.7071068f, 0.7071068f, 6);
      CompressionBasic.OmittedQuaternionComponentIndexCompressionInfo = new CompressionInfo.Integer(0, 3, true);
      CompressionBasic.ImpulseCompressionInfo = new CompressionInfo.Float(-500f, 16, 0.0153f);
      CompressionBasic.AnimationKeyCompressionInfo = new CompressionInfo.Integer(0, 8000, true);
      CompressionBasic.AnimationSpeedCompressionInfo = new CompressionInfo.Float(0.0f, 9, 0.01f);
      CompressionBasic.AnimationProgressCompressionInfo = new CompressionInfo.Float(0.0f, 1.01f, 7);
      CompressionBasic.VertexAnimationSpeedCompressionInfo = new CompressionInfo.Float(0.0f, 9, 0.1f);
      CompressionBasic.ItemDataCompressionInfo = new CompressionInfo.Integer(0, 1023, true);
      CompressionBasic.PercentageCompressionInfo = new CompressionInfo.Integer(0, 100, true);
      CompressionBasic.FloatPercentageCompressionInfo = new CompressionInfo.Integer(0, 10000, true);
      CompressionBasic.DebugIntNonCompressionInfo = new CompressionInfo.Integer(int.MinValue, 32);
      CompressionBasic.DebugULongNonCompressionInfo = new CompressionInfo.UnsignedLongInteger(0UL, 64);
      CompressionBasic.AgentAgeCompressionInfo = new CompressionInfo.Float(0.0f, 128f, 10);
      CompressionBasic.FaceKeyDataCompressionInfo = new CompressionInfo.Float(0.0f, 1f, 10);
      CompressionBasic.EntityChildCountCompressionInfo = new CompressionInfo.Integer(-1, (int) sbyte.MaxValue, true);
      CompressionBasic.AgentHitDamageCompressionInfo = new CompressionInfo.Integer(0, 2000, true);
      CompressionBasic.AgentHitRelativeSpeedCompressionInfo = new CompressionInfo.Float(0.0f, 17, 0.01f);
      CompressionBasic.AgentHitArmorCompressionInfo = new CompressionInfo.Integer(0, 200, true);
      CompressionBasic.AgentHitBoneIndexCompressionInfo = new CompressionInfo.Integer(-1, 63, true);
      CompressionBasic.AgentHitBodyPartCompressionInfo = new CompressionInfo.Integer(-1, 11, true);
      CompressionBasic.AgentHitDamageTypeCompressionInfo = new CompressionInfo.Integer(-1, 3, true);
      CompressionBasic.RoundGoldAmountCompressionInfo = new CompressionInfo.Integer(-1, 2000, true);
      CompressionBasic.PlayerChosenBadgeCompressionInfo = new CompressionInfo.Integer(-1, 6);
      CompressionBasic.MaxNumberOfPlayersCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetMinimumValue(), MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetMaximumValue(), true);
      CompressionBasic.MinNumberOfPlayersForMatchStartCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetMinimumValue(), MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetMaximumValue(), true);
      CompressionBasic.MapTimeLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.MapTimeLimit.GetMinimumValue(), MultiplayerOptions.OptionType.MapTimeLimit.GetMaximumValue(), true);
      CompressionBasic.RoundTotalCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RoundTotal.GetMinimumValue(), MultiplayerOptions.OptionType.RoundTotal.GetMaximumValue(), true);
      CompressionBasic.RoundTimeLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RoundTimeLimit.GetMinimumValue(), MultiplayerOptions.OptionType.RoundTimeLimit.GetMaximumValue(), true);
      CompressionBasic.RoundPreparationTimeLimitCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetMinimumValue(), MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetMaximumValue(), true);
      CompressionBasic.RespawnPeriodCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.RespawnPeriodTeam1.GetMinimumValue(), MultiplayerOptions.OptionType.RespawnPeriodTeam1.GetMaximumValue(), true);
      CompressionBasic.GoldGainChangePercentageCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.GetMinimumValue(), MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.GetMaximumValue(), true);
      CompressionBasic.SpectatorCameraTypeCompressionInfo = new CompressionInfo.Integer(-1, 7, true);
      CompressionBasic.PollAcceptThresholdCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.PollAcceptThreshold.GetMinimumValue(), MultiplayerOptions.OptionType.PollAcceptThreshold.GetMaximumValue(), true);
      CompressionBasic.NumberOfBotsTeamCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetMinimumValue(), MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetMaximumValue(), true);
      CompressionBasic.NumberOfBotsPerFormationCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetMinimumValue(), MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetMaximumValue(), true);
      CompressionBasic.AutoTeamBalanceLimitCompressionInfo = new CompressionInfo.Integer(0, 5, true);
      CompressionBasic.FriendlyFireDamageCompressionInfo = new CompressionInfo.Integer(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetMinimumValue(), MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.GetMaximumValue(), true);
      CompressionBasic.ForcedAvatarIndexCompressionInfo = new CompressionInfo.Integer(-1, 100, true);
      CompressionBasic.IntermissionStateCompressionInfo = new CompressionInfo.Integer(0, Enum.GetNames(typeof (MultiplayerIntermissionState)).Length, false);
      CompressionBasic.IntermissionTimerCompressionInfo = new CompressionInfo.Float(0.0f, 240f, 14);
    }
  }
}
