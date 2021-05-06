// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.CombatStatCalculator
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public static class CombatStatCalculator
  {
    public const float ReferenceSwingSpeed = 22f;
    public const float ReferenceThrustSpeed = 8.5f;
    public const float SwingSpeedConst = 4.545455f;
    public const float ThrustSpeedConst = 11.76471f;
    public const float DefaultImpactDistanceFromTip = 0.07f;
    public const float ArmLength = 0.5f;
    public const float ArmWeight = 2.5f;

    public static float CalculateStrikeMagnitudeForSwing(
      float swingSpeed,
      float impactPointAsPercent,
      float weaponWeight,
      float weaponLength,
      float weaponInertia,
      float weaponCoM,
      float extraLinearSpeed)
    {
      float num1 = weaponLength * impactPointAsPercent - weaponCoM;
      float num2 = swingSpeed * (0.5f + weaponCoM) + extraLinearSpeed;
      double num3 = 0.5 * (double) weaponWeight * (double) num2 * (double) num2;
      float num4 = swingSpeed;
      double num5 = (double) (0.5f * weaponInertia * num4 * num4);
      double num6 = num3 + num5;
      float num7 = (float) (((double) num2 + (double) num4 * (double) num1) / (1.0 / (double) weaponWeight + (double) num1 * (double) num1 / (double) weaponInertia));
      float num8 = num2 - num7 / weaponWeight;
      float num9 = num4 - num7 * num1 / weaponInertia;
      double num10 = (double) (0.5f * weaponWeight * num8 * num8 + 0.5f * weaponInertia * num9 * num9);
      return 0.067f * (float) (num6 - num10 + 0.5);
    }

    public static float CalculateStrikeMagnitudeForThrust(
      float thrustWeaponSpeed,
      float weaponWeight,
      float extraLinearSpeed,
      bool isThrown)
    {
      float num = thrustWeaponSpeed + extraLinearSpeed;
      if ((double) num <= 0.0)
        return 0.0f;
      if (!isThrown)
        weaponWeight += 2.5f;
      return 0.125f * (0.5f * weaponWeight * num * num);
    }

    private static float CalculateStrikeMagnitudeForPassiveUsage(
      float weaponWeight,
      float extraLinearSpeed)
    {
      float magnitudeForThrust = CombatStatCalculator.CalculateStrikeMagnitudeForThrust(0.0f, (float) (20.0 / ((double) extraLinearSpeed > 0.0 ? (double) MathF.Pow(extraLinearSpeed, 0.1f) : 1.0)) + weaponWeight, extraLinearSpeed * 0.83f, false);
      return (double) magnitudeForThrust < 10.0 ? 0.0f : magnitudeForThrust;
    }

    public static float ComputeRawDamageOld(
      DamageTypes damageType,
      float magnitude,
      float armorEffectiveness,
      float absorbedDamageRatio)
    {
      float num1 = 0.0f;
      float factorByDamageType = CombatStatCalculator.GetBluntDamageFactorByDamageType(damageType);
      float num2 = magnitude * factorByDamageType;
      float num3 = num1 + num2 * (float) (100.0 / (100.0 + (double) armorEffectiveness));
      float num4;
      switch (damageType)
      {
        case DamageTypes.Cut:
          num4 = Math.Max(0.0f, magnitude * (float) (1.0 - 0.600000023841858 * (double) armorEffectiveness / (20.0 + 0.400000005960464 * (double) armorEffectiveness)));
          break;
        case DamageTypes.Pierce:
          num4 = Math.Max(0.0f, magnitude * (float) (45.0 / (45.0 + (double) armorEffectiveness)));
          break;
        case DamageTypes.Blunt:
label_5:
          return num3 * absorbedDamageRatio;
        default:
          return 0.0f;
      }
      num3 += num4 * (1f - factorByDamageType);
      goto label_5;
    }

    public static float ComputeRawDamageNew(
      DamageTypes damageType,
      float magnitude,
      float armorEffectiveness,
      float absorbedDamageRatio)
    {
      float num1 = 0.0f;
      float factorByDamageType = CombatStatCalculator.GetBluntDamageFactorByDamageType(damageType);
      float num2 = magnitude * factorByDamageType;
      float num3 = (float) (100.0 / (100.0 + (double) armorEffectiveness));
      float num4 = num1 + num2 * num3;
      float num5;
      switch (damageType)
      {
        case DamageTypes.Cut:
          num5 = Math.Max(0.0f, (float) ((double) magnitude * (double) num3 - (double) armorEffectiveness * 0.5));
          break;
        case DamageTypes.Pierce:
          num5 = Math.Max(0.0f, (float) ((double) magnitude * (double) num3 - (double) armorEffectiveness * 0.330000013113022));
          break;
        case DamageTypes.Blunt:
label_5:
          return num4 * absorbedDamageRatio;
        default:
          return 0.0f;
      }
      num4 += num5 * (1f - factorByDamageType);
      goto label_5;
    }

    private static float GetBluntDamageFactorByDamageType(DamageTypes damageType)
    {
      float num = 0.0f;
      switch (damageType)
      {
        case DamageTypes.Cut:
          num = 0.1f;
          break;
        case DamageTypes.Pierce:
          num = 0.25f;
          break;
        case DamageTypes.Blunt:
          num = 1f;
          break;
      }
      return num;
    }

    public static float CalculateBaseBlowMagnitudeForSwing(
      float angularSpeed,
      float weaponReach,
      float weaponWeight,
      float weaponInertia,
      float weaponCoM,
      float impactPoint,
      float exraLinearSpeed)
    {
      impactPoint = Math.Min(impactPoint, 0.93f);
      float num1 = MBMath.ClampFloat(0.4f / weaponReach, 0.0f, 1f);
      float num2 = 0.0f;
      for (int index = 0; index < 5; ++index)
      {
        float impactPointAsPercent = impactPoint + (float) index / 4f * num1;
        if ((double) impactPointAsPercent < 1.0)
        {
          float magnitudeForSwing = CombatStatCalculator.CalculateStrikeMagnitudeForSwing(angularSpeed, impactPointAsPercent, weaponWeight, weaponReach, weaponInertia, weaponCoM, exraLinearSpeed);
          if ((double) num2 < (double) magnitudeForSwing)
            num2 = magnitudeForSwing;
        }
        else
          break;
      }
      return num2;
    }

    public static float CalculateBaseBlowMagnitudeForThrust(
      float linearSpeed,
      float weaponWeight,
      float exraLinearSpeed)
    {
      return CombatStatCalculator.CalculateStrikeMagnitudeForThrust(linearSpeed, weaponWeight, exraLinearSpeed, false);
    }

    public static float CalculateBaseBlowMagnitudeForPassiveUsage(
      float weaponWeight,
      float exraLinearSpeed)
    {
      return CombatStatCalculator.CalculateStrikeMagnitudeForPassiveUsage(weaponWeight, exraLinearSpeed);
    }
  }
}
