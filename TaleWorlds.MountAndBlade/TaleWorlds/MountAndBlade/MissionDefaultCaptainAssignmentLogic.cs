// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionDefaultCaptainAssignmentLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class MissionDefaultCaptainAssignmentLogic : MissionLogic
  {
    public void AssignCaptainsForMission()
    {
      foreach (Team team in (ReadOnlyCollection<Team>) Mission.Current.Teams)
        this.AssignCaptainsForTeam(team);
    }

    public override void OnFormationUnitsSpawned(Team team) => this.AssignCaptainsForTeam(team);

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
      Agent captain = affectedAgent?.Formation?.Captain;
      if (captain == null || captain != affectedAgent)
        return;
      Agent bestCaptain = this.FindBestCaptain(affectedAgent.Formation);
      affectedAgent.Formation.Captain = bestCaptain;
    }

    public override void OnAgentPanicked(Agent affectedAgent)
    {
      Agent captain = affectedAgent?.Formation?.Captain;
      if (captain == null || captain != affectedAgent)
        return;
      Agent bestCaptain = this.FindBestCaptain(affectedAgent.Formation);
      affectedAgent.Formation.Captain = bestCaptain;
    }

    private void AssignCaptainsForTeam(Team team)
    {
      foreach (Formation formation in team.Formations.ToList<Formation>())
      {
        if (formation.Captain == null)
        {
          Agent bestCaptain = this.FindBestCaptain(formation);
          if (bestCaptain != null)
            formation.Captain = bestCaptain;
        }
      }
    }

    private Agent FindBestCaptain(Formation formation)
    {
      Agent agent = (Agent) null;
      int num1 = int.MinValue;
      for (int unitIndex = 0; unitIndex < formation.CountOfUnits; ++unitIndex)
      {
        Agent unitWithIndex = formation.GetUnitWithIndex(unitIndex);
        if (unitWithIndex != null && unitWithIndex.IsHero && unitWithIndex.IsActive())
        {
          int num2 = 0;
          if (num2 > num1)
          {
            agent = unitWithIndex;
            num1 = num2;
          }
        }
      }
      return agent;
    }
  }
}
