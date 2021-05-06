// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.TroopCountEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class TroopCountEffect : MPOnSpawnPerkEffect
  {
    protected static string StringType = "TroopCountOnSpawn";
    private bool _isMultiplier;
    private float _value;

    protected TroopCountEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      base.Deserialize(node);
      string s = node?.Attributes?["value"]?.Value;
      if (s != null)
        float.TryParse(s, out this._value);
      this._isMultiplier = node?.Attributes?["is_multiplier"]?.Value?.ToLower() == "true";
    }

    public override float GetTroopCountMultiplier() => !this._isMultiplier ? 0.0f : this._value;

    public override float GetExtraTroopCount() => !this._isMultiplier ? this._value : 0.0f;
  }
}
