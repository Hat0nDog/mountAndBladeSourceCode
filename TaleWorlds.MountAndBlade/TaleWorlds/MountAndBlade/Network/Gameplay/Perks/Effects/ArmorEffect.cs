// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.ArmorEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class ArmorEffect : MPOnSpawnPerkEffect
  {
    protected static string StringType = "ArmorOnSpawn";
    private float _value;

    protected ArmorEffect()
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

    public override float GetDrivenPropertyBonusOnSpawn(
      bool isPlayer,
      DrivenProperty drivenProperty,
      float baseValue)
    {
      return (drivenProperty == DrivenProperty.ArmorHead || drivenProperty == DrivenProperty.ArmorTorso || (drivenProperty == DrivenProperty.ArmorLegs || drivenProperty == DrivenProperty.ArmorArms)) && (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Any || (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player ? 1 : 0) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops ? 1 : 0)) != 0) ? this._value : 0.0f;
    }
  }
}
