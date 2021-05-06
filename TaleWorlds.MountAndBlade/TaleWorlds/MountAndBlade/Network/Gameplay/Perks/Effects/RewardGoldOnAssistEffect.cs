// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.RewardGoldOnAssistEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class RewardGoldOnAssistEffect : MPPerkEffect
  {
    protected static string StringType = "RewardGoldOnAssist";
    private int _value;

    protected RewardGoldOnAssistEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string s = node?.Attributes?["value"]?.Value;
      if (s == null)
        return;
      int.TryParse(s, out this._value);
    }

    public override int GetRewardedGoldOnAssist() => this._value;
  }
}
