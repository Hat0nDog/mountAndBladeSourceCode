// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.DrivenPropertyOnSpawnEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class DrivenPropertyOnSpawnEffect : MPOnSpawnPerkEffect
  {
    protected static string StringType = "DrivenPropertyOnSpawn";
    private DrivenProperty _drivenProperty;
    private float _value;
    private bool _isRatio;

    protected DrivenPropertyOnSpawnEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      base.Deserialize(node);
      this._isRatio = node?.Attributes?["is_ratio"]?.Value?.ToLower() == "true";
      Enum.TryParse<DrivenProperty>(node?.Attributes?["driven_property"]?.Value, true, out this._drivenProperty);
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
      if (drivenProperty != this._drivenProperty || this.EffectTarget != MPOnSpawnPerkEffectBase.Target.Any && (isPlayer ? (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Player ? 1 : 0) : (this.EffectTarget == MPOnSpawnPerkEffectBase.Target.Troops ? 1 : 0)) == 0)
        return 0.0f;
      return !this._isRatio ? this._value : baseValue * this._value;
    }
  }
}
