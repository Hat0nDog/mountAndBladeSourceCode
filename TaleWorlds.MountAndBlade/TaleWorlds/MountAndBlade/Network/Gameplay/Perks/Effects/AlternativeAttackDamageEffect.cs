// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.AlternativeAttackDamageEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class AlternativeAttackDamageEffect : MPPerkEffect
  {
    protected static string StringType = "AlternativeAttackDamage";
    private AlternativeAttackDamageEffect.AttackType _attackType;
    private float _value;

    protected AlternativeAttackDamageEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string str = node?.Attributes?["attack_type"]?.Value;
      this._attackType = AlternativeAttackDamageEffect.AttackType.Any;
      if (str != null && !Enum.TryParse<AlternativeAttackDamageEffect.AttackType>(str, true, out this._attackType))
        this._attackType = AlternativeAttackDamageEffect.AttackType.Any;
      string s = node?.Attributes?["value"]?.Value;
      if (s == null)
        return;
      float.TryParse(s, out this._value);
    }

    public override float GetDamage(
      WeaponComponentData attackerWeapon,
      DamageTypes damageType,
      bool isAlternativeAttack)
    {
      if (isAlternativeAttack)
      {
        switch (this._attackType)
        {
          case AlternativeAttackDamageEffect.AttackType.Any:
            return this._value;
          case AlternativeAttackDamageEffect.AttackType.Kick:
            return attackerWeapon != null ? 0.0f : this._value;
          case AlternativeAttackDamageEffect.AttackType.Bash:
            return attackerWeapon == null ? 0.0f : this._value;
        }
      }
      return 0.0f;
    }

    private enum AttackType
    {
      Any,
      Kick,
      Bash,
    }
  }
}
