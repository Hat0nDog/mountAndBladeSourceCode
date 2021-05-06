// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerAgentApplyDamageModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerAgentApplyDamageModel : AgentApplyDamageModel
  {
    public override float CalculateDamage(
      ref AttackInformation attackInformation,
      ref AttackCollisionData collisionData,
      in MissionWeapon weapon,
      float baseDamage)
    {
      return baseDamage;
    }

    public override float CalculateEffectiveMissileSpeed(
      Agent attackerAgent,
      WeaponComponentData missileWeapon,
      ref Vec3 missileStartDirection,
      float missileStartSpeed)
    {
      return missileStartSpeed;
    }

    public override void DecideMissileWeaponFlags(
      Agent attackerAgent,
      MissionWeapon missileWeapon,
      ref WeaponFlags missileWeaponFlags)
    {
    }

    public override float CalculateKnockBackChanceBonus(
      Agent attackerAgent,
      WeaponComponentData weapon)
    {
      return 0.0f;
    }

    public override float CalculateKnockDownChanceBonus(
      Agent attackerAgent,
      WeaponComponentData weapon)
    {
      return 0.0f;
    }

    public override float CalculateDismountChanceBonus(
      Agent attackerAgent,
      WeaponComponentData weapon)
    {
      return 0.0f;
    }

    public override void CalculateCollisionStunMultipliers(
      Agent attackerAgent,
      Agent defenderAgent,
      bool isAlternativeAttack,
      CombatCollisionResult collisionResult,
      WeaponComponentData attackerWeapon,
      WeaponComponentData defenderWeapon,
      out float attackerStunMultiplier,
      out float defenderStunMultiplier)
    {
      attackerStunMultiplier = 1f;
      defenderStunMultiplier = 1f;
    }

    public override float CalculateStaggerThresholdMultiplier(Agent defenderAgent) => 1f;

    public override float CalculatePassiveAttackDamage(
      BasicCharacterObject attackerCharacter,
      ref AttackCollisionData collisionData,
      float baseDamage)
    {
      return baseDamage;
    }

    public override MeleeCollisionReaction DecidePassiveAttackCollisionReaction(
      Agent attacker,
      Agent defender,
      bool isFatalHit)
    {
      return MeleeCollisionReaction.Bounced;
    }

    public override float CalculateShieldDamage(float baseDamage)
    {
      baseDamage *= 1.25f;
      MissionMultiplayerFlagDomination missionBehaviour = Mission.Current.GetMissionBehaviour<MissionMultiplayerFlagDomination>();
      return missionBehaviour != null && missionBehaviour.GetMissionType() == MissionLobbyComponent.MultiplayerGameType.Captain ? baseDamage * 0.5f : baseDamage;
    }

    public override float GetDamageMultiplierForBodyPart(
      BoneBodyPartType bodyPart,
      DamageTypes type)
    {
      float num = 1f;
      switch (bodyPart)
      {
        case BoneBodyPartType.None:
          num = 1f;
          break;
        case BoneBodyPartType.Head:
          switch (type)
          {
            case DamageTypes.Invalid:
              num = 2f;
              break;
            case DamageTypes.Cut:
              num = 1.2f;
              break;
            case DamageTypes.Pierce:
              num = 2f;
              break;
            case DamageTypes.Blunt:
              num = 1.2f;
              break;
          }
          break;
        case BoneBodyPartType.Neck:
          switch (type)
          {
            case DamageTypes.Invalid:
              num = 2f;
              break;
            case DamageTypes.Cut:
              num = 1.2f;
              break;
            case DamageTypes.Pierce:
              num = 2f;
              break;
            case DamageTypes.Blunt:
              num = 1.2f;
              break;
          }
          break;
        case BoneBodyPartType.Chest:
        case BoneBodyPartType.Abdomen:
        case BoneBodyPartType.ShoulderLeft:
        case BoneBodyPartType.ShoulderRight:
        case BoneBodyPartType.BipedalArmLeft:
        case BoneBodyPartType.BipedalArmRight:
          num = 1f;
          break;
        case BoneBodyPartType.BipedalLegs:
          num = 0.8f;
          break;
        case BoneBodyPartType.QuadrupedalArmLeft:
        case BoneBodyPartType.QuadrupedalArmRight:
        case BoneBodyPartType.QuadrupedalLegs:
          num = 0.8f;
          break;
      }
      return num;
    }

    public override bool DecideCrushedThrough(
      Agent attackerAgent,
      Agent defenderAgent,
      float totalAttackEnergy,
      Agent.UsageDirection attackDirection,
      StrikeType strikeType,
      WeaponComponentData defendItem,
      bool isPassiveUsage)
    {
      EquipmentIndex wieldedItemIndex = attackerAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
      if (wieldedItemIndex == EquipmentIndex.None)
        wieldedItemIndex = attackerAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      WeaponComponentData weaponComponentData = wieldedItemIndex != EquipmentIndex.None ? attackerAgent.Equipment[wieldedItemIndex].CurrentUsageItem : (WeaponComponentData) null;
      if (weaponComponentData == null | isPassiveUsage || !weaponComponentData.WeaponFlags.HasAnyFlag<WeaponFlags>(WeaponFlags.CanCrushThrough) || (strikeType != StrikeType.Swing || attackDirection != Agent.UsageDirection.AttackUp))
        return false;
      float num = 58f;
      if (defendItem != null && defendItem.IsShield)
        num *= 1.2f;
      return (double) totalAttackEnergy > (double) num;
    }
  }
}
