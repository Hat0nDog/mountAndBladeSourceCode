// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerStrikeMagnitudeModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  internal class MultiplayerStrikeMagnitudeModel : StrikeMagnitudeCalculationModel
  {
    public override float CalculateStrikeMagnitudeForSwing(
      BasicCharacterObject attackerCharacter,
      BasicCharacterObject attackerCaptainCharacter,
      float swingSpeed,
      float impactPoint,
      float weaponWeight,
      float weaponLength,
      float weaponInertia,
      float weaponCoM,
      float extraLinearSpeed,
      bool doesAttackerHaveMount,
      WeaponClass weaponClass)
    {
      return CombatStatCalculator.CalculateStrikeMagnitudeForSwing(swingSpeed, impactPoint, weaponWeight, weaponLength, weaponInertia, weaponCoM, extraLinearSpeed);
    }

    public override float CalculateStrikeMagnitudeForThrust(
      BasicCharacterObject attackerCharacter,
      BasicCharacterObject attackerCaptainCharacter,
      float thrustWeaponSpeed,
      float weaponWeight,
      float extraLinearSpeed,
      bool doesAttackerHaveMount,
      WeaponClass weaponClass,
      bool isThrown = false)
    {
      return CombatStatCalculator.CalculateStrikeMagnitudeForThrust(thrustWeaponSpeed, weaponWeight, extraLinearSpeed, isThrown);
    }

    public override float ComputeRawDamage(
      DamageTypes damageType,
      float magnitude,
      float armorEffectiveness,
      float absorbedDamageRatio)
    {
      return CombatStatCalculator.ComputeRawDamageOld(damageType, magnitude, armorEffectiveness, absorbedDamageRatio);
    }

    public override float CalculateHorseArcheryFactor(BasicCharacterObject characterObject) => 100f;
  }
}
