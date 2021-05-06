// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeMissionController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.MountAndBlade.AI;
using TaleWorlds.MountAndBlade.Missions;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
  public class SiegeMissionController : MissionLogic
  {
    private SiegeDeploymentHandler _siegeDeploymentHandler;
    private MissionBoundaryPlacer _missionBoundaryPlacer;
    private MissionAgentSpawnLogic _missionAgentSpawnLogic;
    private readonly Dictionary<Type, int> _availableSiegeWeaponsOfAttackers;
    private readonly Dictionary<Type, int> _availableSiegeWeaponsOfDefenders;
    private readonly bool _isPlayerAttacker;
    private bool _teamSetupOver;
    private bool _siegeEngineSetupOver;

    public bool IsSallyOut { get; private set; }

    public SiegeMissionController(
      Dictionary<Type, int> siegeWeaponsCountOfAttackers,
      Dictionary<Type, int> siegeWeaponsCountOfDefenders,
      bool isPlayerAttacker,
      bool isSallyOut)
    {
      this._availableSiegeWeaponsOfAttackers = siegeWeaponsCountOfAttackers;
      this._availableSiegeWeaponsOfDefenders = siegeWeaponsCountOfDefenders;
      this._isPlayerAttacker = isPlayerAttacker;
      this.IsSallyOut = isSallyOut;
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._siegeDeploymentHandler = this.Mission.GetMissionBehaviour<SiegeDeploymentHandler>();
      this._missionBoundaryPlacer = this.Mission.GetMissionBehaviour<MissionBoundaryPlacer>();
      this._missionAgentSpawnLogic = this.Mission.GetMissionBehaviour<MissionAgentSpawnLogic>();
    }

    public override void AfterStart()
    {
      base.AfterStart();
      Mission.Current.AllowAiTicking = false;
      this.Mission.GetMissionBehaviour<DeploymentHandler>().InitializeDeploymentPoints();
      bool isSallyOut = this.IsSallyOut;
      for (int index = 0; index < 2; ++index)
      {
        this._missionAgentSpawnLogic.SetSpawnHorses((BattleSideEnum) index, isSallyOut);
        this._missionAgentSpawnLogic.SetSpawnTroops((BattleSideEnum) index, false);
      }
    }

    private void SetupTeams()
    {
      if (this._isPlayerAttacker)
      {
        this.SetupTeam(this.Mission.AttackerTeam);
      }
      else
      {
        this.SetupTeam(this.Mission.AttackerTeam);
        this.OnTeamDeploymentFinish(BattleSideEnum.Attacker);
        this.SetupTeam(this.Mission.DefenderTeam);
      }
      if (this.Mission.PlayerTeam.IsPlayerSergeant || this.IsSallyOut && this._isPlayerAttacker)
        this.OnPlayerDeploymentFinish();
      else
        this.SetupAllyTeam(this.Mission.PlayerAllyTeam);
      this._teamSetupOver = true;
    }

    private void SetupSiegeEngines()
    {
      MissionGameModels.Current.BattleInitializationModel.SetupSiegeEngines();
      this._siegeEngineSetupOver = true;
    }

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if (!this._teamSetupOver)
      {
        this.SetupTeams();
        if (this._teamSetupOver)
          return;
        foreach (Team team in (ReadOnlyCollection<Team>) this.Mission.Teams)
        {
          foreach (Formation formation in team.Formations)
            formation.QuerySystem.EvaluateAllPreliminaryQueryData();
        }
      }
      else
      {
        if (this._siegeEngineSetupOver)
          return;
        this.SetupSiegeEngines();
      }
    }

    [Conditional("DEBUG")]
    private void DebugTick()
    {
      if (!Input.DebugInput.IsHotKeyPressed("SwapToEnemy"))
        return;
      this.Mission.MainAgent.Controller = Agent.ControllerType.AI;
      this.Mission.PlayerEnemyTeam.Leader.Controller = Agent.ControllerType.Player;
      this.SwapTeams();
    }

    private void SwapTeams() => this.Mission.PlayerTeam = this.Mission.PlayerEnemyTeam;

    private void SetupAllyTeam(Team team)
    {
      if (team == null)
        return;
      foreach (Formation formation in team.FormationsIncludingSpecial)
        formation.IsAIControlled = true;
      team.QuerySystem.Expire();
    }

    private void RemoveUnavailableDeploymentPoints(BattleSideEnum side)
    {
      SiegeWeaponCollection weapons = new SiegeWeaponCollection(side == BattleSideEnum.Attacker ? this._availableSiegeWeaponsOfAttackers : this._availableSiegeWeaponsOfDefenders);
      foreach (DeploymentPoint deploymentPoint in this.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>().Where<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => dp.Side == side)).ToArray<DeploymentPoint>())
      {
        if (!deploymentPoint.DeployableWeaponTypes.Any<Type>((Func<Type, bool>) (wt => weapons.GetMaxDeployableWeaponCount(wt) > 0)))
        {
          foreach (SynchedMissionObject synchedMissionObject in deploymentPoint.DeployableWeapons.Select<SynchedMissionObject, SiegeWeapon>((Func<SynchedMissionObject, SiegeWeapon>) (sw => sw as SiegeWeapon)))
            synchedMissionObject.SetDisabledSynched();
          deploymentPoint.SetDisabledSynched();
        }
      }
    }

    private void SetupTeam(Team team)
    {
      BattleSideEnum side = team.Side;
      this._siegeDeploymentHandler.RemoveAllBoundaries();
      this._siegeDeploymentHandler.SetDeploymentBoundary(side);
      if (team == this.Mission.PlayerTeam)
      {
        this.RemoveUnavailableDeploymentPoints(side);
        foreach (DeploymentPoint deploymentPoint in this.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>().Where<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => !dp.IsDisabled && dp.Side == side)))
          deploymentPoint.Show();
      }
      else
        this.DeploySiegeWeaponsByAi(side);
      this._missionAgentSpawnLogic.SetSpawnTroops(side, true, true);
      foreach (Formation formation in team.FormationsIncludingSpecial)
        formation.ApplyActionOnEachUnit((Action<Agent>) (agent =>
        {
          if (!agent.IsAIControlled)
            return;
          agent.SetIsAIPaused(true);
        }));
      this._missionAgentSpawnLogic.OnInitialSpawnForSideEnded(team.Side);
      Mission.Current.IsTeleportingAgents = true;
    }

    public TeamAIComponent GetTeamAI(
      Team team,
      float thinkTimerTime = 5f,
      float applyTimerTime = 1f)
    {
      if (this.IsSallyOut)
        return (TeamAIComponent) null;
      if (team.Side == BattleSideEnum.Attacker)
        return (TeamAIComponent) new TeamAISiegeAttacker(this.Mission, team, thinkTimerTime, applyTimerTime);
      return team.Side == BattleSideEnum.Defender ? (TeamAIComponent) new TeamAISiegeDefender(this.Mission, team, thinkTimerTime, applyTimerTime) : (TeamAIComponent) null;
    }

    private void DeploySiegeWeaponsByAi(BattleSideEnum side)
    {
      new SiegeWeaponDeploymentAI(this.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>().Where<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => dp.Side == side)).ToList<DeploymentPoint>(), side == BattleSideEnum.Attacker ? this._availableSiegeWeaponsOfAttackers : this._availableSiegeWeaponsOfDefenders).DeployAll(this.Mission, side);
      this.RemoveDeploymentPoints(side);
    }

    private void RemoveDeploymentPoints(BattleSideEnum side)
    {
      foreach (DeploymentPoint deploymentPoint in this.Mission.ActiveMissionObjects.FindAllWithType<DeploymentPoint>().Where<DeploymentPoint>((Func<DeploymentPoint, bool>) (dp => dp.Side == side)).ToArray<DeploymentPoint>())
      {
        foreach (SynchedMissionObject synchedMissionObject in deploymentPoint.DeployableWeapons.ToArray<SynchedMissionObject>())
        {
          if ((deploymentPoint.DeployedWeapon == null || !synchedMissionObject.GameEntity.IsVisibleIncludeParents()) && synchedMissionObject is SiegeWeapon siegeWeapon3)
            siegeWeapon3.SetDisabledSynched();
        }
        deploymentPoint.SetDisabledSynched();
      }
    }

    private void OnTeamDeploymentFinish(BattleSideEnum side)
    {
      this.RemoveDeploymentPoints(side);
      foreach (SynchedMissionObject synchedMissionObject in Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeLadder>().Where<SiegeLadder>((Func<SiegeLadder, bool>) (sl => !sl.GameEntity.IsVisibleIncludeParents())).ToList<SiegeLadder>())
        synchedMissionObject.SetDisabledSynched();
      Team team = side == BattleSideEnum.Attacker ? Mission.Current.AttackerTeam : Mission.Current.DefenderTeam;
      if (side != Mission.Current.PlayerTeam.Side)
      {
        Mission.Current.IsTeleportingAgents = true;
        this.DeployFormationsOfTeam(team);
        Mission.Current.IsTeleportingAgents = false;
      }
      this._siegeDeploymentHandler.RemoveAllBoundaries();
      this._missionBoundaryPlacer.AddBoundaries();
    }

    private void DeployFormationsOfTeam(Team team)
    {
      foreach (Formation formation in team.Formations)
        formation.IsAIControlled = true;
      Mission.Current.AllowAiTicking = true;
      Mission.Current.ForceTickOccasionally = true;
      team.ResetTactic();
      bool teleportingAgents = Mission.Current.IsTeleportingAgents;
      Mission.Current.IsTeleportingAgents = true;
      team.Tick(0.0f);
      Mission.Current.IsTeleportingAgents = teleportingAgents;
      Mission.Current.AllowAiTicking = false;
      Mission.Current.ForceTickOccasionally = false;
    }

    public void OnPlayerDeploymentFinish()
    {
      this.OnTeamDeploymentFinish(this.Mission.PlayerTeam.Side);
      if (this.Mission.PlayerTeam.Side == BattleSideEnum.Attacker)
      {
        Mission.Current.IsTeleportingAgents = false;
        this.SetupTeam(this.Mission.DefenderTeam);
        this.OnTeamDeploymentFinish(BattleSideEnum.Defender);
      }
      this.Mission.RemoveMissionBehaviour((MissionBehaviour) this._siegeDeploymentHandler);
      CampaignSiegeMissionEvent deploymentFinish = this.PlayerDeploymentFinish;
      if (deploymentFinish != null)
        deploymentFinish();
      Mission.Current.IsTeleportingAgents = false;
      foreach (Agent agent in (IEnumerable<Agent>) this.Mission.Agents)
      {
        if (agent.IsAIControlled)
          agent.SetIsAIPaused(false);
      }
      Mission.Current.AllowAiTicking = true;
      if (this.IsSallyOut)
        Mission.Current.AddMissionBehaviour((MissionBehaviour) new SallyOutEndLogic());
      this.Mission.RemoveMissionBehaviour((MissionBehaviour) this);
    }

    public event CampaignSiegeMissionEvent PlayerDeploymentFinish;
  }
}
