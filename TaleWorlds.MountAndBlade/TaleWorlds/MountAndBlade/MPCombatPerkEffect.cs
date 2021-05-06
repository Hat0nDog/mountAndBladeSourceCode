// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MPCombatPerkEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MPCombatPerkEffect : MPPerkEffect
  {
    protected MPCombatPerkEffect.HitType EffectHitType;
    protected DamageTypes? DamageType;
    protected TaleWorlds.Core.WeaponClass? WeaponClass;

    protected override void Deserialize(XmlNode node)
    {
      string str1 = node?.Attributes?["hit_type"]?.Value;
      this.EffectHitType = MPCombatPerkEffect.HitType.Any;
      if (str1 != null && !Enum.TryParse<MPCombatPerkEffect.HitType>(str1, true, out this.EffectHitType))
        this.EffectHitType = MPCombatPerkEffect.HitType.Any;
      string str2 = node?.Attributes?["damage_type"]?.Value;
      DamageTypes result1;
      this.DamageType = str2 == null || str2.ToLower() == "any" ? new DamageTypes?() : (!Enum.TryParse<DamageTypes>(str2, true, out result1) ? new DamageTypes?() : new DamageTypes?(result1));
      string str3 = node?.Attributes?["weapon_class"]?.Value;
      if (str3 == null || str3.ToLower() == "any")
      {
        this.WeaponClass = new TaleWorlds.Core.WeaponClass?();
      }
      else
      {
        TaleWorlds.Core.WeaponClass result2;
        if (Enum.TryParse<TaleWorlds.Core.WeaponClass>(str3, true, out result2))
          this.WeaponClass = new TaleWorlds.Core.WeaponClass?(result2);
        else
          this.WeaponClass = new TaleWorlds.Core.WeaponClass?();
      }
    }

    protected bool IsSatisfied(WeaponComponentData attackerWeapon, DamageTypes damageType)
    {
      if (!this.DamageType.HasValue || this.DamageType.Value == damageType)
      {
        if (this.WeaponClass.HasValue)
        {
          int num = (int) this.WeaponClass.Value;
          TaleWorlds.Core.WeaponClass? weaponClass = attackerWeapon?.WeaponClass;
          int valueOrDefault = (int) weaponClass.GetValueOrDefault();
          if (!(num == valueOrDefault & weaponClass.HasValue))
            goto label_7;
        }
        switch (this.EffectHitType)
        {
          case MPCombatPerkEffect.HitType.Any:
            return true;
          case MPCombatPerkEffect.HitType.Melee:
            return !this.IsWeaponRanged(attackerWeapon);
          case MPCombatPerkEffect.HitType.Ranged:
            return this.IsWeaponRanged(attackerWeapon);
        }
      }
label_7:
      return false;
    }

    protected bool IsWeaponRanged(WeaponComponentData attackerWeapon)
    {
      if (attackerWeapon == null)
        return false;
      return attackerWeapon.IsConsumable || attackerWeapon.IsRangedWeapon;
    }

    protected enum HitType
    {
      Any,
      Melee,
      Ranged,
    }
  }
}
