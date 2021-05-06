// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.DefaultStrikeMagnitudeModel
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public class DefaultStrikeMagnitudeModel : StrikeMagnitudeCalculationModel
  {
    public override float CalculateStrikeMagnitudeForSwing(
      BasicCharacterObject attackerCharacter,
      BasicCharacterObject attackerCaptainCharacter,
      float swingSpeed,
      float impactPointAsPercent,
      float weaponWeight,
      float weaponLength,
      float weaponInertia,
      float weaponCoM,
      float extraLinearSpeed,
      bool doesAttackerHaveMount,
      WeaponClass weaponClass)
    {
      return CombatStatCalculator.CalculateStrikeMagnitudeForSwing(swingSpeed, impactPointAsPercent, weaponWeight, weaponLength, weaponInertia, weaponCoM, extraLinearSpeed);
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
      return CombatStatCalculator.ComputeRawDamageNew(damageType, magnitude, armorEffectiveness, absorbedDamageRatio);
    }

    public override float CalculateHorseArcheryFactor(BasicCharacterObject characterObject) => 100f;
  }
}
