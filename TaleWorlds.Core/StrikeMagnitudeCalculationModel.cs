// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.StrikeMagnitudeCalculationModel
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public abstract class StrikeMagnitudeCalculationModel : GameModel
  {
    public abstract float CalculateStrikeMagnitudeForSwing(
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
      WeaponClass weaponClass);

    public abstract float CalculateStrikeMagnitudeForThrust(
      BasicCharacterObject attackerCharacter,
      BasicCharacterObject attackerCaptainCharacter,
      float thrustWeaponSpeed,
      float weaponWeight,
      float extraLinearSpeed,
      bool doesAtttackerHaveMount,
      WeaponClass weaponClass,
      bool isThrown = false);

    public abstract float ComputeRawDamage(
      DamageTypes damageType,
      float magnitude,
      float armorEffectiveness,
      float absorbedDamageRatio);

    public abstract float CalculateHorseArcheryFactor(BasicCharacterObject characterObject);

    public virtual float CalculateAdjustedArmorForBlow(
      float baseArmor,
      BasicCharacterObject attackerCharacter,
      BasicCharacterObject attackerCaptainCharacter,
      BasicCharacterObject victimCharacter,
      BasicCharacterObject victimCaptainCharacter,
      WeaponComponentData weaponComponent)
    {
      return baseArmor;
    }
  }
}
