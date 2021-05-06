// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.MountDamageTakenEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class MountDamageTakenEffect : MPCombatPerkEffect
  {
    protected static string StringType = "MountDamageTaken";
    private float _value;

    protected MountDamageTakenEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      base.Deserialize(node);
      string s = node?.Attributes?["value"]?.Value;
      if (s == null)
        return;
      float.TryParse(s, out this._value);
    }

    public override float GetMountDamageTaken(
      WeaponComponentData attackerWeapon,
      DamageTypes damageType)
    {
      return !this.IsSatisfied(attackerWeapon, damageType) ? 0.0f : this._value;
    }
  }
}
