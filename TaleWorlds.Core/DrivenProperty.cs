// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.DrivenProperty
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public enum DrivenProperty
  {
    None = -1, // 0xFFFFFFFF
    AiRangedHorsebackMissileRange = 0,
    AiFacingMissileWatch = 1,
    AiFlyingMissileCheckRadius = 2,
    AiShootFreq = 3,
    AiWaitBeforeShootFactor = 4,
    AIBlockOnDecideAbility = 5,
    AIParryOnDecideAbility = 6,
    AiTryChamberAttackOnDecide = 7,
    AIAttackOnParryChance = 8,
    AiAttackOnParryTiming = 9,
    AIDecideOnAttackChance = 10, // 0x0000000A
    AIParryOnAttackAbility = 11, // 0x0000000B
    AiKick = 12, // 0x0000000C
    AiAttackCalculationMaxTimeFactor = 13, // 0x0000000D
    AiDecideOnAttackWhenReceiveHitTiming = 14, // 0x0000000E
    AiDecideOnAttackContinueAction = 15, // 0x0000000F
    AiDecideOnAttackingContinue = 16, // 0x00000010
    AIParryOnAttackingContinueAbility = 17, // 0x00000011
    AIDecideOnRealizeEnemyBlockingAttackAbility = 18, // 0x00000012
    AIRealizeBlockingFromIncorrectSideAbility = 19, // 0x00000013
    AiAttackingShieldDefenseChance = 20, // 0x00000014
    AiAttackingShieldDefenseTimer = 21, // 0x00000015
    AiCheckMovementIntervalFactor = 22, // 0x00000016
    AiMovemetDelayFactor = 23, // 0x00000017
    AiParryDecisionChangeValue = 24, // 0x00000018
    AiDefendWithShieldDecisionChanceValue = 25, // 0x00000019
    AiMoveEnemySideTimeValue = 26, // 0x0000001A
    AiMinimumDistanceToContinueFactor = 27, // 0x0000001B
    AiStandGroundTimerValue = 28, // 0x0000001C
    AiStandGroundTimerMoveAlongValue = 29, // 0x0000001D
    AiHearingDistanceFactor = 30, // 0x0000001E
    AiChargeHorsebackTargetDistFactor = 31, // 0x0000001F
    AiRangerLeadErrorMin = 32, // 0x00000020
    AiRangerLeadErrorMax = 33, // 0x00000021
    AiRangerVerticalErrorMultiplier = 34, // 0x00000022
    AiRangerHorizontalErrorMultiplier = 35, // 0x00000023
    AIAttackOnDecideChance = 36, // 0x00000024
    AiRaiseShieldDelayTimeBase = 37, // 0x00000025
    AiUseShieldAgainstEnemyMissileProbability = 38, // 0x00000026
    AiSpeciesIndex = 39, // 0x00000027
    AiRandomizedDefendDirectionChance = 40, // 0x00000028
    AISetNoAttackTimerAfterBeingHitAbility = 41, // 0x00000029
    AISetNoAttackTimerAfterBeingParriedAbility = 42, // 0x0000002A
    AISetNoDefendTimerAfterHittingAbility = 43, // 0x0000002B
    AISetNoDefendTimerAfterParryingAbility = 44, // 0x0000002C
    AIEstimateStunDurationPrecision = 45, // 0x0000002D
    AIHoldingReadyMaxDuration = 46, // 0x0000002E
    AIHoldingReadyVariationPercentage = 47, // 0x0000002F
    MountChargeDamage = 48, // 0x00000030
    MountDifficulty = 49, // 0x00000031
    ArmorEncumbrance = 50, // 0x00000032
    ArmorHead = 51, // 0x00000033
    ArmorTorso = 52, // 0x00000034
    ArmorLegs = 53, // 0x00000035
    ArmorArms = 54, // 0x00000036
    UseRealisticBlocking = 55, // 0x00000037
    DrivenPropertiesCalculatedAtSpawnEnd = 56, // 0x00000038
    WeaponsEncumbrance = 56, // 0x00000038
    SwingSpeedMultiplier = 57, // 0x00000039
    ThrustOrRangedReadySpeedMultiplier = 58, // 0x0000003A
    HandlingMultiplier = 59, // 0x0000003B
    ReloadSpeed = 60, // 0x0000003C
    WeaponInaccuracy = 61, // 0x0000003D
    WeaponWorstMobileAccuracyPenalty = 62, // 0x0000003E
    WeaponWorstUnsteadyAccuracyPenalty = 63, // 0x0000003F
    WeaponBestAccuracyWaitTime = 64, // 0x00000040
    WeaponUnsteadyBeginTime = 65, // 0x00000041
    WeaponUnsteadyEndTime = 66, // 0x00000042
    WeaponRotationalAccuracyPenaltyInRadians = 67, // 0x00000043
    LongestRangedWeaponSlotIndex = 68, // 0x00000044
    LongestRangedWeaponInaccuracy = 69, // 0x00000045
    AttributeRiding = 70, // 0x00000046
    AttributeShield = 71, // 0x00000047
    AttributeShieldMissileCollisionBodySizeAdder = 72, // 0x00000048
    ShieldBashStunDurationMultiplier = 73, // 0x00000049
    KickStunDurationMultiplier = 74, // 0x0000004A
    ReloadMovementPenaltyFactor = 75, // 0x0000004B
    TopSpeedReachDuration = 76, // 0x0000004C
    MaxSpeedMultiplier = 77, // 0x0000004D
    CombatMaxSpeedMultiplier = 78, // 0x0000004E
    AttributeHorseArchery = 79, // 0x0000004F
    AttributeCourage = 80, // 0x00000050
    MountManeuver = 81, // 0x00000051
    MountSpeed = 82, // 0x00000052
    BipedalRangedReadySpeedMultiplier = 83, // 0x00000053
    BipedalRangedReloadSpeedMultiplier = 84, // 0x00000054
    Count = 85, // 0x00000055
  }
}
