// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects.RewardGoldOnDeathEffect
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Effects
{
  public class RewardGoldOnDeathEffect : MPPerkEffect
  {
    protected static string StringType = "RewardGoldOnDeath";
    private int _value;
    private int _count;
    private RewardGoldOnDeathEffect.OrderBy _orderBy;

    protected RewardGoldOnDeathEffect()
    {
    }

    protected override void Deserialize(XmlNode node)
    {
      string s1 = node?.Attributes?["value"]?.Value;
      if (s1 != null)
        int.TryParse(s1, out this._value);
      string s2 = node?.Attributes?["number_of_receivers"]?.Value;
      if (s2 != null)
        int.TryParse(s2, out this._count);
      string str = node?.Attributes?["order_by"]?.Value;
      this._orderBy = RewardGoldOnDeathEffect.OrderBy.Random;
      if (str == null || Enum.TryParse<RewardGoldOnDeathEffect.OrderBy>(str, true, out this._orderBy))
        return;
      this._orderBy = RewardGoldOnDeathEffect.OrderBy.Random;
    }

    public override bool GetIsTeamRewardedOnDeath() => true;

    public override void CalculateRewardedGoldOnDeath(
      Agent agent,
      List<(MissionPeer, int)> teamMembers)
    {
      if (agent?.MissionPeer == null)
      {
        Agent agent1 = agent;
        if (agent1 != null)
        {
          MissionPeer agentMissionPeer = agent1.OwningAgentMissionPeer;
        }
      }
      switch (this._orderBy)
      {
        case RewardGoldOnDeathEffect.OrderBy.WealthAscending:
          teamMembers.Sort((Comparison<(MissionPeer, int)>) ((a, b) => a.Item1.Representative.Gold.CompareTo(b.Item1.Representative.Gold)));
          break;
        case RewardGoldOnDeathEffect.OrderBy.WealthDescending:
          teamMembers.Sort((Comparison<(MissionPeer, int)>) ((a, b) => b.Item1.Representative.Gold.CompareTo(a.Item1.Representative.Gold)));
          break;
        case RewardGoldOnDeathEffect.OrderBy.DistanceAscending:
          teamMembers.Sort((Comparison<(MissionPeer, int)>) ((a, b) => this.SortByDistance(agent, a.Item1.Representative.ControlledAgent, b.Item1.Representative.ControlledAgent)));
          break;
        case RewardGoldOnDeathEffect.OrderBy.DistanceDescending:
          teamMembers.Sort((Comparison<(MissionPeer, int)>) ((a, b) => this.SortByDistance(agent, b.Item1.Representative.ControlledAgent, a.Item1.Representative.ControlledAgent)));
          break;
      }
      int count = this._count;
      for (int index = 0; index < teamMembers.Count && count > 0; ++index)
      {
        int num;
        if (this._orderBy >= RewardGoldOnDeathEffect.OrderBy.DistanceAscending)
        {
          MissionRepresentativeBase representative = teamMembers[index].Item1.Representative;
          if (representative == null)
          {
            num = 0;
          }
          else
          {
            bool? nullable = representative.ControlledAgent?.IsActive();
            bool flag = true;
            num = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
          }
        }
        else
          num = 1;
        if (num != 0)
        {
          (MissionPeer, int) teamMember = teamMembers[index];
          teamMember.Item2 += this._value;
          teamMembers[index] = teamMember;
          --count;
        }
      }
    }

    private int SortByDistance(Agent from, Agent a, Agent b) => a == null || !a.IsActive() ? (b != null && b.IsActive() ? 1 : 0) : (b == null || !b.IsActive() ? -1 : a.Position.DistanceSquared(from.Position).CompareTo(b.Position.DistanceSquared(from.Position)));

    private enum OrderBy
    {
      Random = 0,
      WealthAscending = 1,
      WealthDescending = 2,
      DeadCanReceiveEnd = 3,
      DistanceAscending = 3,
      DistanceDescending = 4,
    }
  }
}
