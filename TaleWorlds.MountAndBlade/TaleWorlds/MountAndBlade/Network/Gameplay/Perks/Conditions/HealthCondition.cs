// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.HealthCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class HealthCondition : MPPerkCondition
  {
    protected static string StringType = "Health";
    private bool _isRatio;
    private float _min;
    private float _max;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.HealthChange;

    protected HealthCondition()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      this._isRatio = node?.Attributes?["is_ratio"]?.Value?.ToLower() == "true";
      string s1 = node?.Attributes?["min"]?.Value;
      if (s1 == null)
        this._min = 0.0f;
      else
        float.TryParse(s1, out this._min);
      string s2 = node?.Attributes?["max"]?.Value;
      if (s2 == null)
        this._max = this._isRatio ? 1f : float.MaxValue;
      else
        float.TryParse(s2, out this._max);
    }

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      if (agent == null)
        return false;
      float num = this._isRatio ? agent.Health / agent.HealthLimit : agent.Health;
      return (double) num >= (double) this._min && (double) num <= (double) this._max;
    }
  }
}
