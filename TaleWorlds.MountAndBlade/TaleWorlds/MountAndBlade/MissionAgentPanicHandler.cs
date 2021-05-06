// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionAgentPanicHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  public class MissionAgentPanicHandler : MissionLogic
  {
    private readonly List<Agent> _panickedAgents;
    private readonly List<Formation> _panickedFormations;
    private readonly List<Team> _panickedTeams;

    public MissionAgentPanicHandler()
    {
      this._panickedAgents = new List<Agent>(256);
      this._panickedFormations = new List<Formation>(24);
      this._panickedTeams = new List<Team>(2);
    }

    public override void OnAgentPanicked(Agent agent)
    {
      this._panickedAgents.Add(agent);
      if (agent.Formation == null || agent.Team == null)
        return;
      if (!this._panickedFormations.Contains(agent.Formation))
        this._panickedFormations.Add(agent.Formation);
      if (this._panickedTeams.Contains(agent.Team))
        return;
      this._panickedTeams.Add(agent.Team);
    }

    public override void OnPreMissionTick(float dt)
    {
      if (this._panickedAgents.Count <= 0)
        return;
      foreach (Team panickedTeam in this._panickedTeams)
        panickedTeam.UpdateCachedEnemyDataForFleeing();
      foreach (Formation panickedFormation in this._panickedFormations)
        panickedFormation.OnBatchUnitRemovalStart();
      foreach (Agent panickedAgent in this._panickedAgents)
      {
        panickedAgent.GetComponent<MoraleAgentComponent>()?.Retreat();
        Mission.Current.OnAgentFleeing(panickedAgent);
      }
      foreach (Formation panickedFormation in this._panickedFormations)
        panickedFormation.OnBatchUnitRemovalEnd();
      this._panickedAgents.Clear();
      this._panickedFormations.Clear();
      this._panickedTeams.Clear();
    }

    public override void OnRemoveBehaviour()
    {
      this._panickedAgents.Clear();
      this._panickedFormations.Clear();
      this._panickedTeams.Clear();
    }
  }
}
