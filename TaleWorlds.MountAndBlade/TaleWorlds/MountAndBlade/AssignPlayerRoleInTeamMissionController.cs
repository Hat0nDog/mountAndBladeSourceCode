// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AssignPlayerRoleInTeamMissionController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class AssignPlayerRoleInTeamMissionController : MissionLogic
  {
    private bool _isPlayerSergeant;
    private FormationClass _preassignedFormationClass;
    private List<string> _charactersInPlayerSideByPriority = new List<string>();
    private Queue<string> _characterNamesInPlayerSideByPriorityQueue;
    private List<Formation> _remainingFormationsToAssignSergeantsTo;
    private Dictionary<int, Agent> _formationsLockedWithSergeants;
    private Dictionary<int, Agent> _formationsWithLooselyChosenSergeants;
    private int _playerChosenIndex = -1;

    public event PlayerTurnToChooseFormationToLeadEvent OnPlayerTurnToChooseFormationToLead;

    public event AllFormationsAssignedSergeantsEvent OnAllFormationsAssignedSergeants;

    public bool IsPlayerInArmy { get; }

    public bool IsPlayerGeneral { get; }

    public AssignPlayerRoleInTeamMissionController(
      bool isPlayerGeneral,
      bool isPlayerSergeant,
      bool isPlayerInArmy,
      List<string> charactersInPlayerSideByPriority = null,
      FormationClass preassignedFormationClass = FormationClass.NumberOfRegularFormations)
    {
      this.IsPlayerGeneral = isPlayerGeneral;
      this._isPlayerSergeant = isPlayerSergeant;
      this.IsPlayerInArmy = isPlayerInArmy;
      this._charactersInPlayerSideByPriority = charactersInPlayerSideByPriority;
      this._preassignedFormationClass = preassignedFormationClass;
    }

    public override void AfterStart() => Mission.Current.PlayerTeam.SetPlayerRole(this.IsPlayerGeneral, this._isPlayerSergeant);

    private Formation ChooseFormationToLead(
      IEnumerable<Formation> formationsToChooseFrom,
      Agent agent)
    {
      bool hasMount = agent.HasMount;
      bool hasRangedWeapon = agent.GetHasRangedWeapon();
      List<Formation> list = formationsToChooseFrom.ToList<Formation>();
      while (list.Any<Formation>())
      {
        Formation formation = list.MaxBy<Formation, float>((Func<Formation, float>) (ftcf => ftcf.QuerySystem.FormationPower));
        list.Remove(formation);
        if ((hasRangedWeapon || !formation.QuerySystem.IsRangedFormation && !formation.QuerySystem.IsRangedCavalryFormation) && (hasMount || !formation.QuerySystem.IsCavalryFormation && !formation.QuerySystem.IsRangedCavalryFormation))
          return formation;
      }
      return (Formation) null;
    }

    private void AssignSergeant(Formation formationToLead, Agent sergeant)
    {
      sergeant.Formation = formationToLead;
      if (!sergeant.IsAIControlled)
        formationToLead.PlayerOwner = sergeant;
      formationToLead.Captain = sergeant;
    }

    public void OnPlayerChoiceMade(int chosenIndex, bool isFinal)
    {
      if (this._playerChosenIndex != chosenIndex)
      {
        this._playerChosenIndex = chosenIndex;
        this._formationsWithLooselyChosenSergeants.Clear();
        List<Formation> list = this.Mission.PlayerTeam.Formations.Where<Formation>((Func<Formation, bool>) (f => !this._formationsLockedWithSergeants.ContainsKey(f.Index))).ToList<Formation>();
        if (chosenIndex != -1)
        {
          Formation formation = list.FirstOrDefault<Formation>((Func<Formation, bool>) (fr => fr.Index == chosenIndex));
          this._formationsWithLooselyChosenSergeants.Add(chosenIndex, this.Mission.PlayerTeam.PlayerOrderController.Owner);
          list.Remove(formation);
        }
        Queue<string> stringQueue = new Queue<string>((IEnumerable<string>) this._characterNamesInPlayerSideByPriorityQueue);
        while (list.Any<Formation>() && stringQueue.Count > 0)
        {
          string nextAgentNameToProcess = stringQueue.Dequeue();
          Agent agent = this.Mission.PlayerTeam.ActiveAgents.FirstOrDefault<Agent>((Func<Agent, bool>) (aa => aa.Character.StringId.Equals(nextAgentNameToProcess)));
          if (agent != null)
          {
            Formation lead = this.ChooseFormationToLead((IEnumerable<Formation>) list, agent);
            if (lead != null)
            {
              this._formationsWithLooselyChosenSergeants.Add(lead.Index, agent);
              list.Remove(lead);
            }
          }
        }
        if (this.OnAllFormationsAssignedSergeants == null)
          return;
        this.OnAllFormationsAssignedSergeants(this._formationsWithLooselyChosenSergeants);
      }
      else
      {
        if (!isFinal)
          return;
        foreach (KeyValuePair<int, Agent> lockedWithSergeant in this._formationsLockedWithSergeants)
          this.AssignSergeant(lockedWithSergeant.Value.Team.GetFormation((FormationClass) lockedWithSergeant.Key), lockedWithSergeant.Value);
        foreach (KeyValuePair<int, Agent> looselyChosenSergeant in this._formationsWithLooselyChosenSergeants)
          this.AssignSergeant(looselyChosenSergeant.Value.Team.GetFormation((FormationClass) looselyChosenSergeant.Key), looselyChosenSergeant.Value);
      }
    }

    public void OnTacticAppliedForFirstTime(Team team)
    {
      if (!team.IsPlayerTeam)
        return;
      this._formationsLockedWithSergeants = new Dictionary<int, Agent>();
      this._formationsWithLooselyChosenSergeants = new Dictionary<int, Agent>();
      this._characterNamesInPlayerSideByPriorityQueue = this._charactersInPlayerSideByPriority != null ? new Queue<string>((IEnumerable<string>) this._charactersInPlayerSideByPriority) : new Queue<string>();
      this._remainingFormationsToAssignSergeantsTo = team.Formations.ToList<Formation>();
      while (this._remainingFormationsToAssignSergeantsTo.Any<Formation>() && this._characterNamesInPlayerSideByPriorityQueue.Count > 0)
      {
        string nextAgentNameToProcess = this._characterNamesInPlayerSideByPriorityQueue.Dequeue();
        Agent agent = team.ActiveAgents.FirstOrDefault<Agent>((Func<Agent, bool>) (aa => aa.Character.StringId.Equals(nextAgentNameToProcess)));
        if (agent != null)
        {
          if (agent.IsAIControlled)
          {
            Formation lead = this.ChooseFormationToLead((IEnumerable<Formation>) this._remainingFormationsToAssignSergeantsTo, agent);
            if (lead != null)
            {
              this._formationsLockedWithSergeants.Add(lead.Index, agent);
              this._remainingFormationsToAssignSergeantsTo.Remove(lead);
            }
          }
          else
            break;
        }
      }
      if (this.OnPlayerTurnToChooseFormationToLead == null)
        return;
      this.OnPlayerTurnToChooseFormationToLead(this._formationsLockedWithSergeants, this._remainingFormationsToAssignSergeantsTo.Select<Formation, int>((Func<Formation, int>) (ftcsf => ftcsf.Index)).ToList<int>());
    }

    public override void OnFormationUnitsSpawned(Team team)
    {
      base.OnFormationUnitsSpawned(team);
      if (team != this.Mission.PlayerTeam)
        return;
      team.PlayerOrderController.Owner = Agent.Main;
      if (team.IsPlayerGeneral)
      {
        foreach (Formation formation in team.FormationsIncludingEmpty)
          formation.PlayerOwner = Agent.Main;
      }
      team.PlayerOrderController.SelectAllFormations();
    }

    public IEnumerable<Formation> GetFormationsAvailableForPlayer()
    {
      FormationClass playerFormationClass = Team.GetPlayerTeamFormationClass();
      return this.Mission.PlayerTeam.Formations.Where<Formation>((Func<Formation, bool>) (f => Team.DoesFirstFormationClassContainSecond(playerFormationClass, Team.GetFormationFormationClass(f))));
    }

    public void OnPlayerChoiceMade(
      FormationClass chosenFormationClass,
      FormationAI.BehaviorSide formationBehaviorSide = FormationAI.BehaviorSide.Middle)
    {
      Team playerTeam = this.Mission.PlayerTeam;
      Formation formation = playerTeam.Formations.Where<Formation>((Func<Formation, bool>) (f => f.PrimaryClass == chosenFormationClass && f.AI.Side == formationBehaviorSide)).MaxBy<Formation, float>((Func<Formation, float>) (f => f.QuerySystem.FormationPower));
      if (playerTeam.IsPlayerSergeant)
      {
        formation.PlayerOwner = Agent.Main;
        formation.IsAIControlled = false;
        formation.IsPlayerInFormation = true;
      }
      else
        formation.IsPlayerInFormation = true;
      Agent.Main.Formation = formation;
      playerTeam.TriggerOnFormationsChanged(formation);
    }
  }
}
