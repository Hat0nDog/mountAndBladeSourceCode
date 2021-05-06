// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.TroopCountCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class TroopCountCondition : MPPerkCondition
  {
    protected static string StringType = "TroopCount";
    private bool _isRatio;
    private float _min;
    private float _max;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.AliveBotCountChange | MPPerkCondition.PerkEventFlags.SpawnEnd;

    public override bool IsPeerCondition => true;

    protected TroopCountCondition()
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

    public override bool Check(MissionPeer peer)
    {
      if (peer == null || MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() <= 0 || peer.ControlledFormation == null)
        return false;
      int num1 = peer.IsControlledAgentActive ? peer.BotsUnderControlAlive + 1 : peer.BotsUnderControlAlive;
      if (this._isRatio)
      {
        float num2 = (float) num1 / (float) (peer.BotsUnderControlTotal + 1);
        return (double) num2 >= (double) this._min && (double) num2 <= (double) this._max;
      }
      return (double) num1 >= (double) this._min && (double) num1 <= (double) this._max;
    }

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      return this.Check(agent?.MissionPeer ?? agent?.OwningAgentMissionPeer);
    }
  }
}
