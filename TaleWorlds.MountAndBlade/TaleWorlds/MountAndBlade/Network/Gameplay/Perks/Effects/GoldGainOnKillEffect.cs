// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.GoldGainOnKillEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class GoldGainOnKillEffect : MPPerkEffect
  {
    protected static string StringType = "GoldGainOnKill";
    private int _value;
    private GoldGainOnKillEffect.EnemyValue _enemyValue;

    protected GoldGainOnKillEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string s = node?.Attributes?["value"]?.Value;
      if (s != null)
        int.TryParse(s, out this._value);
      string str = node?.Attributes?["enemy_value"]?.Value;
      this._enemyValue = GoldGainOnKillEffect.EnemyValue.Any;
      if (str == null || Enum.TryParse<GoldGainOnKillEffect.EnemyValue>(str, true, out this._enemyValue))
        return;
      this._enemyValue = GoldGainOnKillEffect.EnemyValue.Any;
    }

    public override int GetGoldOnKill(float attackerValue, float victimValue)
    {
      switch (this._enemyValue)
      {
        case GoldGainOnKillEffect.EnemyValue.Any:
          return this._value;
        case GoldGainOnKillEffect.EnemyValue.Higher:
          return (double) victimValue <= (double) attackerValue ? 0 : this._value;
        case GoldGainOnKillEffect.EnemyValue.Lower:
          return (double) victimValue >= (double) attackerValue ? 0 : this._value;
        default:
          return 0;
      }
    }

    private enum EnemyValue
    {
      Any,
      Higher,
      Lower,
    }
  }
}
