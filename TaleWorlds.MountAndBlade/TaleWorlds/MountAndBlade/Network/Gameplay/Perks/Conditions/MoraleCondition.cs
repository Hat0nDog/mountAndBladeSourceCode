// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions.MoraleCondition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Xml;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions
{
  public class MoraleCondition : MPPerkCondition<MissionMultiplayerFlagDomination>
  {
    protected static string StringType = "FlagDominationMorale";
    private float _min;
    private float _max;

    public override MPPerkCondition.PerkEventFlags EventFlags => MPPerkCondition.PerkEventFlags.MoraleChange;

    public override bool IsPeerCondition => true;

    protected MoraleCondition()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string s1 = node?.Attributes?["min"]?.Value;
      if (s1 == null)
        this._min = -1f;
      else
        float.TryParse(s1, out this._min);
      string s2 = node?.Attributes?["max"]?.Value;
      if (s2 == null)
        this._max = 1f;
      else
        float.TryParse(s2, out this._max);
    }

    public override bool Check(MissionPeer peer) => this.Check(peer?.ControlledAgent);

    public override bool Check(Agent agent)
    {
      agent = agent == null || !agent.IsMount ? agent : agent.RiderAgent;
      Team team = agent?.Team;
      if (team == null)
        return false;
      MissionMultiplayerFlagDomination gameModeInstance = this.GameModeInstance;
      float num = team.Side == BattleSideEnum.Attacker ? gameModeInstance.MoraleRounded : -gameModeInstance.MoraleRounded;
      return (double) num >= (double) this._min && (double) num <= (double) this._max;
    }
  }
}
