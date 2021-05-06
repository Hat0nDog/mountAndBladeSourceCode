// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentDrivenProperties
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class AgentDrivenProperties
  {
    private readonly float[] _statValues;

    internal float[] Values => this._statValues;

    public AgentDrivenProperties()
    {
      this._statValues = new float[85];
      this.LongestRangedWeaponSlotIndex = -1f;
    }

    public float GetStat(DrivenProperty propertEnum) => this._statValues[(int) propertEnum];

    public void SetStat(DrivenProperty propertEnum, float value) => this._statValues[(int) propertEnum] = value;

    public float SwingSpeedMultiplier
    {
      get => this.GetStat(DrivenProperty.SwingSpeedMultiplier);
      set => this.SetStat(DrivenProperty.SwingSpeedMultiplier, value);
    }

    public float ThrustOrRangedReadySpeedMultiplier
    {
      get => this.GetStat(DrivenProperty.ThrustOrRangedReadySpeedMultiplier);
      set => this.SetStat(DrivenProperty.ThrustOrRangedReadySpeedMultiplier, value);
    }

    public float HandlingMultiplier
    {
      get => this.GetStat(DrivenProperty.HandlingMultiplier);
      set => this.SetStat(DrivenProperty.HandlingMultiplier, value);
    }

    public float ReloadSpeed
    {
      get => this.GetStat(DrivenProperty.ReloadSpeed);
      set => this.SetStat(DrivenProperty.ReloadSpeed, value);
    }

    public float WeaponInaccuracy
    {
      get => this.GetStat(DrivenProperty.WeaponInaccuracy);
      set => this.SetStat(DrivenProperty.WeaponInaccuracy, value);
    }

    public float WeaponMaxMovementAccuracyPenalty
    {
      get => this.GetStat(DrivenProperty.WeaponWorstMobileAccuracyPenalty);
      set => this.SetStat(DrivenProperty.WeaponWorstMobileAccuracyPenalty, value);
    }

    public float WeaponMaxUnsteadyAccuracyPenalty
    {
      get => this.GetStat(DrivenProperty.WeaponWorstUnsteadyAccuracyPenalty);
      set => this.SetStat(DrivenProperty.WeaponWorstUnsteadyAccuracyPenalty, value);
    }

    public float WeaponBestAccuracyWaitTime
    {
      get => this.GetStat(DrivenProperty.WeaponBestAccuracyWaitTime);
      set => this.SetStat(DrivenProperty.WeaponBestAccuracyWaitTime, value);
    }

    public float WeaponUnsteadyBeginTime
    {
      get => this.GetStat(DrivenProperty.WeaponUnsteadyBeginTime);
      set => this.SetStat(DrivenProperty.WeaponUnsteadyBeginTime, value);
    }

    public float WeaponUnsteadyEndTime
    {
      get => this.GetStat(DrivenProperty.WeaponUnsteadyEndTime);
      set => this.SetStat(DrivenProperty.WeaponUnsteadyEndTime, value);
    }

    public float WeaponRotationalAccuracyPenaltyInRadians
    {
      get => this.GetStat(DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians);
      set => this.SetStat(DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians, value);
    }

    public float ArmorEncumbrance
    {
      get => this.GetStat(DrivenProperty.ArmorEncumbrance);
      set => this.SetStat(DrivenProperty.ArmorEncumbrance, value);
    }

    public float WeaponsEncumbrance
    {
      get => this.GetStat(DrivenProperty.WeaponsEncumbrance);
      set => this.SetStat(DrivenProperty.WeaponsEncumbrance, value);
    }

    public float ArmorHead
    {
      get => this.GetStat(DrivenProperty.ArmorHead);
      set => this.SetStat(DrivenProperty.ArmorHead, value);
    }

    public float ArmorTorso
    {
      get => this.GetStat(DrivenProperty.ArmorTorso);
      set => this.SetStat(DrivenProperty.ArmorTorso, value);
    }

    public float ArmorLegs
    {
      get => this.GetStat(DrivenProperty.ArmorLegs);
      set => this.SetStat(DrivenProperty.ArmorLegs, value);
    }

    public float ArmorArms
    {
      get => this.GetStat(DrivenProperty.ArmorArms);
      set => this.SetStat(DrivenProperty.ArmorArms, value);
    }

    public float LongestRangedWeaponSlotIndex
    {
      get => this.GetStat(DrivenProperty.LongestRangedWeaponSlotIndex);
      set => this.SetStat(DrivenProperty.LongestRangedWeaponSlotIndex, value);
    }

    public float LongestRangedWeaponInaccuracy
    {
      get => this.GetStat(DrivenProperty.LongestRangedWeaponInaccuracy);
      set => this.SetStat(DrivenProperty.LongestRangedWeaponInaccuracy, value);
    }

    public float AttributeRiding
    {
      get => this.GetStat(DrivenProperty.AttributeRiding);
      set => this.SetStat(DrivenProperty.AttributeRiding, value);
    }

    public float AttributeShield
    {
      get => this.GetStat(DrivenProperty.AttributeShield);
      set => this.SetStat(DrivenProperty.AttributeShield, value);
    }

    public float AttributeShieldMissileCollisionBodySizeAdder
    {
      get => this.GetStat(DrivenProperty.AttributeShieldMissileCollisionBodySizeAdder);
      set => this.SetStat(DrivenProperty.AttributeShieldMissileCollisionBodySizeAdder, value);
    }

    public float ShieldBashStunDurationMultiplier
    {
      get => this.GetStat(DrivenProperty.ShieldBashStunDurationMultiplier);
      set => this.SetStat(DrivenProperty.ShieldBashStunDurationMultiplier, value);
    }

    public float KickStunDurationMultiplier
    {
      get => this.GetStat(DrivenProperty.KickStunDurationMultiplier);
      set => this.SetStat(DrivenProperty.KickStunDurationMultiplier, value);
    }

    public float ReloadMovementPenaltyFactor
    {
      get => this.GetStat(DrivenProperty.ReloadMovementPenaltyFactor);
      set => this.SetStat(DrivenProperty.ReloadMovementPenaltyFactor, value);
    }

    public float TopSpeedReachDuration
    {
      get => this.GetStat(DrivenProperty.TopSpeedReachDuration);
      set => this.SetStat(DrivenProperty.TopSpeedReachDuration, value);
    }

    public float MaxSpeedMultiplier
    {
      get => this.GetStat(DrivenProperty.MaxSpeedMultiplier);
      set => this.SetStat(DrivenProperty.MaxSpeedMultiplier, value);
    }

    public float CombatMaxSpeedMultiplier
    {
      get => this.GetStat(DrivenProperty.CombatMaxSpeedMultiplier);
      set => this.SetStat(DrivenProperty.CombatMaxSpeedMultiplier, value);
    }

    public float AttributeHorseArchery
    {
      get => this.GetStat(DrivenProperty.AttributeHorseArchery);
      set => this.SetStat(DrivenProperty.AttributeHorseArchery, value);
    }

    public float AttributeCourage
    {
      get => this.GetStat(DrivenProperty.AttributeCourage);
      set => this.SetStat(DrivenProperty.AttributeCourage, value);
    }

    public float MountManeuver
    {
      get => this.GetStat(DrivenProperty.MountManeuver);
      set => this.SetStat(DrivenProperty.MountManeuver, value);
    }

    public float MountSpeed
    {
      get => this.GetStat(DrivenProperty.MountSpeed);
      set => this.SetStat(DrivenProperty.MountSpeed, value);
    }

    public float MountChargeDamage
    {
      get => this.GetStat(DrivenProperty.MountChargeDamage);
      set => this.SetStat(DrivenProperty.MountChargeDamage, value);
    }

    public float MountDifficulty
    {
      get => this.GetStat(DrivenProperty.MountDifficulty);
      set => this.SetStat(DrivenProperty.MountDifficulty, value);
    }

    public float BipedalRangedReadySpeedMultiplier
    {
      get => this.GetStat(DrivenProperty.BipedalRangedReadySpeedMultiplier);
      set => this.SetStat(DrivenProperty.BipedalRangedReadySpeedMultiplier, value);
    }

    public float BipedalRangedReloadSpeedMultiplier
    {
      get => this.GetStat(DrivenProperty.BipedalRangedReloadSpeedMultiplier);
      set => this.SetStat(DrivenProperty.BipedalRangedReloadSpeedMultiplier, value);
    }

    public float AiRangedHorsebackMissileRange
    {
      get => this.GetStat(DrivenProperty.AiRangedHorsebackMissileRange);
      set => this.SetStat(DrivenProperty.AiRangedHorsebackMissileRange, value);
    }

    public float AiFacingMissileWatch
    {
      get => this.GetStat(DrivenProperty.AiFacingMissileWatch);
      set => this.SetStat(DrivenProperty.AiFacingMissileWatch, value);
    }

    public float AiFlyingMissileCheckRadius
    {
      get => this.GetStat(DrivenProperty.AiFlyingMissileCheckRadius);
      set => this.SetStat(DrivenProperty.AiFlyingMissileCheckRadius, value);
    }

    public float AiShootFreq
    {
      get => this.GetStat(DrivenProperty.AiShootFreq);
      set => this.SetStat(DrivenProperty.AiShootFreq, value);
    }

    public float AiWaitBeforeShootFactor
    {
      get => this.GetStat(DrivenProperty.AiWaitBeforeShootFactor);
      set => this.SetStat(DrivenProperty.AiWaitBeforeShootFactor, value);
    }

    public float AIBlockOnDecideAbility
    {
      get => this.GetStat(DrivenProperty.AIBlockOnDecideAbility);
      set => this.SetStat(DrivenProperty.AIBlockOnDecideAbility, value);
    }

    public float AIParryOnDecideAbility
    {
      get => this.GetStat(DrivenProperty.AIParryOnDecideAbility);
      set => this.SetStat(DrivenProperty.AIParryOnDecideAbility, value);
    }

    public float AiTryChamberAttackOnDecide
    {
      get => this.GetStat(DrivenProperty.AiTryChamberAttackOnDecide);
      set => this.SetStat(DrivenProperty.AiTryChamberAttackOnDecide, value);
    }

    public float AIAttackOnParryChance
    {
      get => this.GetStat(DrivenProperty.AIAttackOnParryChance);
      set => this.SetStat(DrivenProperty.AIAttackOnParryChance, value);
    }

    public float AiAttackOnParryTiming
    {
      get => this.GetStat(DrivenProperty.AiAttackOnParryTiming);
      set => this.SetStat(DrivenProperty.AiAttackOnParryTiming, value);
    }

    public float AIDecideOnAttackChance
    {
      get => this.GetStat(DrivenProperty.AIDecideOnAttackChance);
      set => this.SetStat(DrivenProperty.AIDecideOnAttackChance, value);
    }

    public float AIParryOnAttackAbility
    {
      get => this.GetStat(DrivenProperty.AIParryOnAttackAbility);
      set => this.SetStat(DrivenProperty.AIParryOnAttackAbility, value);
    }

    public float AiKick
    {
      get => this.GetStat(DrivenProperty.AiKick);
      set => this.SetStat(DrivenProperty.AiKick, value);
    }

    public float AiAttackCalculationMaxTimeFactor
    {
      get => this.GetStat(DrivenProperty.AiAttackCalculationMaxTimeFactor);
      set => this.SetStat(DrivenProperty.AiAttackCalculationMaxTimeFactor, value);
    }

    public float AiDecideOnAttackWhenReceiveHitTiming
    {
      get => this.GetStat(DrivenProperty.AiDecideOnAttackWhenReceiveHitTiming);
      set => this.SetStat(DrivenProperty.AiDecideOnAttackWhenReceiveHitTiming, value);
    }

    public float AiDecideOnAttackContinueAction
    {
      get => this.GetStat(DrivenProperty.AiDecideOnAttackContinueAction);
      set => this.SetStat(DrivenProperty.AiDecideOnAttackContinueAction, value);
    }

    public float AiDecideOnAttackingContinue
    {
      get => this.GetStat(DrivenProperty.AiDecideOnAttackingContinue);
      set => this.SetStat(DrivenProperty.AiDecideOnAttackingContinue, value);
    }

    public float AIParryOnAttackingContinueAbility
    {
      get => this.GetStat(DrivenProperty.AIParryOnAttackingContinueAbility);
      set => this.SetStat(DrivenProperty.AIParryOnAttackingContinueAbility, value);
    }

    public float AIDecideOnRealizeEnemyBlockingAttackAbility
    {
      get => this.GetStat(DrivenProperty.AIDecideOnRealizeEnemyBlockingAttackAbility);
      set => this.SetStat(DrivenProperty.AIDecideOnRealizeEnemyBlockingAttackAbility, value);
    }

    public float AIRealizeBlockingFromIncorrectSideAbility
    {
      get => this.GetStat(DrivenProperty.AIRealizeBlockingFromIncorrectSideAbility);
      set => this.SetStat(DrivenProperty.AIRealizeBlockingFromIncorrectSideAbility, value);
    }

    public float AiAttackingShieldDefenseChance
    {
      get => this.GetStat(DrivenProperty.AiAttackingShieldDefenseChance);
      set => this.SetStat(DrivenProperty.AiAttackingShieldDefenseChance, value);
    }

    public float AiAttackingShieldDefenseTimer
    {
      get => this.GetStat(DrivenProperty.AiAttackingShieldDefenseTimer);
      set => this.SetStat(DrivenProperty.AiAttackingShieldDefenseTimer, value);
    }

    public float AiCheckMovementIntervalFactor
    {
      get => this.GetStat(DrivenProperty.AiCheckMovementIntervalFactor);
      set => this.SetStat(DrivenProperty.AiCheckMovementIntervalFactor, value);
    }

    public float AiMovemetDelayFactor
    {
      get => this.GetStat(DrivenProperty.AiMovemetDelayFactor);
      set => this.SetStat(DrivenProperty.AiMovemetDelayFactor, value);
    }

    public float AiParryDecisionChangeValue
    {
      get => this.GetStat(DrivenProperty.AiParryDecisionChangeValue);
      set => this.SetStat(DrivenProperty.AiParryDecisionChangeValue, value);
    }

    public float AiDefendWithShieldDecisionChanceValue
    {
      get => this.GetStat(DrivenProperty.AiDefendWithShieldDecisionChanceValue);
      set => this.SetStat(DrivenProperty.AiDefendWithShieldDecisionChanceValue, value);
    }

    public float AiMoveEnemySideTimeValue
    {
      get => this.GetStat(DrivenProperty.AiMoveEnemySideTimeValue);
      set => this.SetStat(DrivenProperty.AiMoveEnemySideTimeValue, value);
    }

    public float AiMinimumDistanceToContinueFactor
    {
      get => this.GetStat(DrivenProperty.AiMinimumDistanceToContinueFactor);
      set => this.SetStat(DrivenProperty.AiMinimumDistanceToContinueFactor, value);
    }

    public float AiStandGroundTimerValue
    {
      get => this.GetStat(DrivenProperty.AiStandGroundTimerValue);
      set => this.SetStat(DrivenProperty.AiStandGroundTimerValue, value);
    }

    public float AiStandGroundTimerMoveAlongValue
    {
      get => this.GetStat(DrivenProperty.AiStandGroundTimerMoveAlongValue);
      set => this.SetStat(DrivenProperty.AiStandGroundTimerMoveAlongValue, value);
    }

    public float AiHearingDistanceFactor
    {
      get => this.GetStat(DrivenProperty.AiHearingDistanceFactor);
      set => this.SetStat(DrivenProperty.AiHearingDistanceFactor, value);
    }

    public float AiChargeHorsebackTargetDistFactor
    {
      get => this.GetStat(DrivenProperty.AiChargeHorsebackTargetDistFactor);
      set => this.SetStat(DrivenProperty.AiChargeHorsebackTargetDistFactor, value);
    }

    public float AiRangerLeadErrorMin
    {
      get => this.GetStat(DrivenProperty.AiRangerLeadErrorMin);
      set => this.SetStat(DrivenProperty.AiRangerLeadErrorMin, value);
    }

    public float AiRangerLeadErrorMax
    {
      get => this.GetStat(DrivenProperty.AiRangerLeadErrorMax);
      set => this.SetStat(DrivenProperty.AiRangerLeadErrorMax, value);
    }

    public float AiRangerVerticalErrorMultiplier
    {
      get => this.GetStat(DrivenProperty.AiRangerVerticalErrorMultiplier);
      set => this.SetStat(DrivenProperty.AiRangerVerticalErrorMultiplier, value);
    }

    public float AiRangerHorizontalErrorMultiplier
    {
      get => this.GetStat(DrivenProperty.AiRangerHorizontalErrorMultiplier);
      set => this.SetStat(DrivenProperty.AiRangerHorizontalErrorMultiplier, value);
    }

    public float AIAttackOnDecideChance
    {
      get => this.GetStat(DrivenProperty.AIAttackOnDecideChance);
      set => this.SetStat(DrivenProperty.AIAttackOnDecideChance, value);
    }

    public float AiRaiseShieldDelayTimeBase
    {
      get => this.GetStat(DrivenProperty.AiRaiseShieldDelayTimeBase);
      set => this.SetStat(DrivenProperty.AiRaiseShieldDelayTimeBase, value);
    }

    public float AiUseShieldAgainstEnemyMissileProbability
    {
      get => this.GetStat(DrivenProperty.AiUseShieldAgainstEnemyMissileProbability);
      set => this.SetStat(DrivenProperty.AiUseShieldAgainstEnemyMissileProbability, value);
    }

    public int AiSpeciesIndex
    {
      get => (int) Math.Round((double) this.GetStat(DrivenProperty.AiSpeciesIndex));
      set => this.SetStat(DrivenProperty.AiSpeciesIndex, (float) value);
    }

    public float AiRandomizedDefendDirectionChance
    {
      get => this.GetStat(DrivenProperty.AiRandomizedDefendDirectionChance);
      set => this.SetStat(DrivenProperty.AiRandomizedDefendDirectionChance, value);
    }

    public float AISetNoAttackTimerAfterBeingHitAbility
    {
      get => this.GetStat(DrivenProperty.AISetNoAttackTimerAfterBeingHitAbility);
      set => this.SetStat(DrivenProperty.AISetNoAttackTimerAfterBeingHitAbility, value);
    }

    public float AISetNoAttackTimerAfterBeingParriedAbility
    {
      get => this.GetStat(DrivenProperty.AISetNoAttackTimerAfterBeingParriedAbility);
      set => this.SetStat(DrivenProperty.AISetNoAttackTimerAfterBeingParriedAbility, value);
    }

    public float AISetNoDefendTimerAfterHittingAbility
    {
      get => this.GetStat(DrivenProperty.AISetNoDefendTimerAfterHittingAbility);
      set => this.SetStat(DrivenProperty.AISetNoDefendTimerAfterHittingAbility, value);
    }

    public float AISetNoDefendTimerAfterParryingAbility
    {
      get => this.GetStat(DrivenProperty.AISetNoDefendTimerAfterParryingAbility);
      set => this.SetStat(DrivenProperty.AISetNoDefendTimerAfterParryingAbility, value);
    }

    public float AIEstimateStunDurationPrecision
    {
      get => this.GetStat(DrivenProperty.AIEstimateStunDurationPrecision);
      set => this.SetStat(DrivenProperty.AIEstimateStunDurationPrecision, value);
    }

    public float AIHoldingReadyMaxDuration
    {
      get => this.GetStat(DrivenProperty.AIHoldingReadyMaxDuration);
      set => this.SetStat(DrivenProperty.AIHoldingReadyMaxDuration, value);
    }

    public float AIHoldingReadyVariationPercentage
    {
      get => this.GetStat(DrivenProperty.AIHoldingReadyVariationPercentage);
      set => this.SetStat(DrivenProperty.AIHoldingReadyVariationPercentage, value);
    }

    internal float[] InitializeDrivenProperties(
      Agent agent,
      Equipment spawnEquipment,
      AgentBuildData agentBuildData)
    {
      MissionGameModels.Current.AgentStatCalculateModel.InitializeAgentStats(agent, spawnEquipment, this, agentBuildData);
      return this._statValues;
    }

    internal float[] UpdateDrivenProperties(Agent agent)
    {
      MissionGameModels.Current.AgentStatCalculateModel.UpdateAgentStats(agent, this);
      return this._statValues;
    }
  }
}
