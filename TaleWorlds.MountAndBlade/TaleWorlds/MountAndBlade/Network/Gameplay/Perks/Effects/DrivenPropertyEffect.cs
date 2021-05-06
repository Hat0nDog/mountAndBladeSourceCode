// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.DrivenPropertyEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class DrivenPropertyEffect : MPPerkEffect
  {
    protected static string StringType = "DrivenProperty";
    private DrivenProperty _drivenProperty;
    private float _value;
    private bool _isRatio;

    protected DrivenPropertyEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      this._isRatio = node?.Attributes?["is_ratio"]?.Value?.ToLower() == "true";
      Enum.TryParse<DrivenProperty>(node?.Attributes?["driven_property"]?.Value, true, out this._drivenProperty);
      string s = node?.Attributes?["value"]?.Value;
      if (s == null)
        return;
      float.TryParse(s, out this._value);
    }

    public override void OnUpdate(Agent agent, bool newState)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      agent?.UpdateAgentProperties();
    }

    public override float GetDrivenPropertyBonus(DrivenProperty drivenProperty, float baseValue)
    {
      if (drivenProperty != this._drivenProperty)
        return 0.0f;
      return !this._isRatio ? this._value : baseValue * this._value;
    }
  }
}
