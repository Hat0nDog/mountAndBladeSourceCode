// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ComponentInterfaces.AgentApplyDamageModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.ComponentInterfaces
{
  public abstract class AgentApplyDamageModel : GameModel
  {
    public abstract float CalculateDamage(
      ref AttackInformation attackInformation,
      ref AttackCollisionData collisionData,
      in MissionWeapon weapon,
      float baseDamage);

    public abstract float CalculateEffectiveMissileSpeed(
      Agent attackerAgent,
      WeaponComponentData missileWeapon,
      ref Vec3 missileStartDirection,
      float missileStartSpeed);

    public abstract float CalculateDismountChanceBonus(
      Agent attackerAgent,
      WeaponComponentData weapon);

    public abstract float CalculateKnockBackChanceBonus(
      Agent attackerAgent,
      WeaponComponentData weapon);

    public abstract float CalculateKnockDownChanceBonus(
      Agent attackerAgent,
      WeaponComponentData weapon);

    public abstract void DecideMissileWeaponFlags(
      Agent attackerAgent,
      MissionWeapon missileWeapon,
      ref WeaponFlags missileWeaponFlags);

    public abstract void CalculateCollisionStunMultipliers(
      Agent attackerAgent,
      Agent defenderAgent,
      bool isAlternativeAttack,
      CombatCollisionResult collisionResult,
      WeaponComponentData attackerWeapon,
      WeaponComponentData defenderWeapon,
      out float attackerStunMultiplier,
      out float defenderStunMultiplier);

    public abstract float CalculateStaggerThresholdMultiplier(Agent defenderAgent);

    public abstract float CalculatePassiveAttackDamage(
      BasicCharacterObject attackerCharacter,
      ref AttackCollisionData collisionData,
      float baseDamage);

    public abstract MeleeCollisionReaction DecidePassiveAttackCollisionReaction(
      Agent attacker,
      Agent defender,
      bool isFatalHit);

    public abstract float CalculateShieldDamage(float baseDamage);

    public abstract float GetDamageMultiplierForBodyPart(
      BoneBodyPartType bodyPart,
      DamageTypes type);

    public abstract bool DecideCrushedThrough(
      Agent attackerAgent,
      Agent defenderAgent,
      float totalAttackEnergy,
      Agent.UsageDirection attackDirection,
      StrikeType strikeType,
      WeaponComponentData defendItem,
      bool isPassiveUsageHit);
  }
}
