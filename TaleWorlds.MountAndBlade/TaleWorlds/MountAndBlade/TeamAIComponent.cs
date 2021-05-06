// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TeamAIComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class TeamAIComponent
  {
    internal const int BattleTokenForceSize = 10;
    private readonly List<TacticComponent> _availableTactics;
    private static bool _retreatScriptActive;
    protected readonly Mission Mission;
    protected readonly Team Team;
    private readonly Timer _thinkTimer;
    private readonly Timer _applyTimer;
    private TacticComponent _currentTactic;
    internal List<TacticalPosition> TacticalPositions;
    internal List<TacticalRegion> TacticalRegions;
    private List<StrategicArea> _strategicAreas;
    private readonly float _occasionalTickTime;
    private MissionTime _nextTacticChooseTime;
    private MissionTime _nextOccasionalTickTime;

    internal bool IsDefenseApplicable { get; private set; }

    internal bool GetIsFirstTacticChosen { get; private set; }

    protected TacticComponent CurrentTactic
    {
      get => this._currentTactic;
      private set
      {
        if (this._currentTactic != null)
          this._currentTactic.OnCancel();
        this._currentTactic = value;
        if (this._currentTactic == null)
          return;
        this._currentTactic.OnApply();
        this._currentTactic.TickOccasionally();
      }
    }

    protected TeamAIComponent(
      Mission currentMission,
      Team currentTeam,
      float thinkTimerTime,
      float applyTimerTime)
    {
      this.Mission = currentMission;
      this.Team = currentTeam;
      this._thinkTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), thinkTimerTime);
      this._applyTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), applyTimerTime);
      this._occasionalTickTime = applyTimerTime;
      this._availableTactics = new List<TacticComponent>();
      this.TacticalPositions = currentMission.ActiveMissionObjects.FindAllWithType<TacticalPosition>().ToList<TacticalPosition>();
      this.TacticalRegions = currentMission.ActiveMissionObjects.FindAllWithType<TacticalRegion>().ToList<TacticalRegion>();
      this._strategicAreas = currentMission.ActiveMissionObjects.Where<MissionObject>((Func<MissionObject, bool>) (amo => amo is StrategicArea && (amo as StrategicArea).IsActive && (amo as StrategicArea).IsUsableBy(this.Team.Side))).Select<MissionObject, StrategicArea>((Func<MissionObject, StrategicArea>) (amo => amo as StrategicArea)).ToList<StrategicArea>();
    }

    internal List<StrategicArea> GetStrategicAreas() => this._strategicAreas;

    internal void AddStrategicArea(StrategicArea strategicArea) => this._strategicAreas.Add(strategicArea);

    internal void RemoveStrategicArea(StrategicArea strategicArea)
    {
      if (this.Team.DetachmentManager.Detachments.Contains((IDetachment) strategicArea))
        this.Team.DetachmentManager.DestroyDetachment((IDetachment) strategicArea);
      this._strategicAreas.Remove(strategicArea);
    }

    public void AddTacticOption(TacticComponent tacticOption) => this._availableTactics.Add(tacticOption);

    internal void RemoveTacticOption(System.Type tacticType) => this._availableTactics.RemoveAll((Predicate<TacticComponent>) (at => tacticType == at.GetType()));

    internal void ClearTacticOptions() => this._availableTactics.Clear();

    [Conditional("DEBUG")]
    internal void AssertTeam(Team team)
    {
    }

    internal void OnMissionEnded()
    {
      MBDebug.Print("Mission end received by teamAI");
      foreach (Formation formation in this.Team.FormationsIncludingSpecial)
      {
        foreach (UsableMachine usable in formation.GetUsedMachines().ToList<UsableMachine>())
          formation.StopUsingMachine(usable);
      }
    }

    internal void ResetTactic(bool keepCurrentTactic = true)
    {
      if (!keepCurrentTactic)
        this.CurrentTactic = (TacticComponent) null;
      this._thinkTimer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission));
      this._applyTimer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission));
      this.MakeDecision();
      this.TickOccasionally();
    }

    internal virtual void Tick(float dt)
    {
      if (this.Team.BodyGuardFormation != null && this.Team.BodyGuardFormation.CountOfUnits > 0 && (this.Team.GeneralsFormation == null || this.Team.GeneralsFormation.CountOfUnits == 0))
      {
        this.Team.BodyGuardFormation.AI.ResetBehaviorWeights();
        this.Team.BodyGuardFormation.AI.SetBehaviorWeight<BehaviorCharge>(1f);
      }
      if (this._nextTacticChooseTime.IsPast)
      {
        this.MakeDecision();
        this._nextTacticChooseTime = MissionTime.SecondsFromNow(5f);
      }
      if (!this._nextOccasionalTickTime.IsPast)
        return;
      this.TickOccasionally();
      this._nextOccasionalTickTime = MissionTime.SecondsFromNow(this._occasionalTickTime);
    }

    internal void CheckIsDefenseApplicable()
    {
      if (this.Team.Side != BattleSideEnum.Defender)
      {
        this.IsDefenseApplicable = false;
      }
      else
      {
        int memberCount = this.Team.QuerySystem.MemberCount;
        float rangedAttackRatio = this.Team.QuerySystem.MaxUnderRangedAttackRatio;
        double num1 = (double) memberCount * (double) rangedAttackRatio;
        int deathByRangedCount = this.Team.QuerySystem.DeathByRangedCount;
        int deathCount = this.Team.QuerySystem.DeathCount;
        double num2 = (double) deathByRangedCount;
        double num3 = (double) MBMath.ClampFloat((float) (num1 + num2) / (float) (memberCount + deathCount), 0.05f, 1f);
        int enemyAliveUnitCount = this.Team.QuerySystem.EnemyUnitCount;
        float num4 = enemyAliveUnitCount == 0 ? 0.0f : this.Team.QuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (et => et.MaxUnderRangedAttackRatio * ((float) et.MemberCount / (enemyAliveUnitCount > 0 ? (float) enemyAliveUnitCount : 1f))));
        double num5 = (double) enemyAliveUnitCount * (double) num4;
        int num6 = this.Team.QuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (et => et.DeathByRangedCount));
        int num7 = this.Team.QuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (et => et.DeathCount));
        int num8 = enemyAliveUnitCount + num7;
        double num9 = (double) num6;
        double num10 = (double) MBMath.ClampFloat((float) ((num5 + num9) / (num8 > 0 ? (double) num8 : 1.0)), 0.05f, 1f);
        if (Math.Pow(num3 / num10, 3.0 * ((double) this.Team.QuerySystem.EnemyRangedRatio + (double) this.Team.QuerySystem.EnemyRangedCavalryRatio)) > 1.5)
          this.IsDefenseApplicable = false;
        else
          this.IsDefenseApplicable = true;
      }
    }

    internal void OnTacticAppliedForFirstTime()
    {
      this.GetIsFirstTacticChosen = false;
      Mission.Current.GetMissionBehaviour<AssignPlayerRoleInTeamMissionController>()?.OnTacticAppliedForFirstTime(this.Team);
    }

    private void MakeDecision()
    {
      List<TacticComponent> availableTactics = this._availableTactics;
      if (this.Mission.CurrentState != Mission.State.Continuing && !availableTactics.Any<TacticComponent>() || this.Team.FormationsIncludingSpecial.IsEmpty<Formation>())
        return;
      if (this.Mission.Teams.GetEnemiesOf(this.Team).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).IsEmpty<Formation>())
      {
        if (this.Mission.MissionEnded())
          return;
        if (!(this.CurrentTactic is TacticCharge))
        {
          foreach (TacticComponent tacticComponent in availableTactics)
          {
            if (tacticComponent is TacticCharge)
            {
              this.CurrentTactic = tacticComponent;
              break;
            }
          }
          if (!(this.CurrentTactic is TacticCharge))
            this.CurrentTactic = availableTactics.FirstOrDefault<TacticComponent>();
        }
      }
      this.CheckIsDefenseApplicable();
      TacticComponent tacticComponent1 = availableTactics.MaxBy<TacticComponent, float>((Func<TacticComponent, float>) (to => to.GetTacticWeight() * (to == this._currentTactic ? 1.5f : 1f)));
      bool flag = false;
      if (this.CurrentTactic == null)
        flag = true;
      else if (this.CurrentTactic != tacticComponent1)
      {
        if (!this.CurrentTactic.ResetTacticalPositions())
          flag = true;
        else if ((double) tacticComponent1.GetTacticWeight() > (double) (this.CurrentTactic.GetTacticWeight() * 1.5f))
          flag = true;
      }
      if (!flag)
        return;
      if (this.CurrentTactic == null)
        this.GetIsFirstTacticChosen = true;
      this.CurrentTactic = tacticComponent1;
      if (Mission.Current.MainAgent == null || this.Team.GeneralAgent == null || (!this.Team.IsPlayerTeam || !this.Team.IsPlayerSergeant))
        return;
      InformationManager.AddQuickInformation(GameTexts.FindText("str_team_ai_tactic_text", tacticComponent1.GetType().Name), 4000, this.Team.GeneralAgent.Character);
    }

    internal void TickOccasionally()
    {
      if (!Mission.Current.AllowAiTicking || !this.Team.HasBots)
        return;
      this.CurrentTactic.TickOccasionally();
    }

    internal bool IsCurrentTactic(TacticComponent tactic) => tactic == this.CurrentTactic;

    [Conditional("DEBUG")]
    protected virtual void DebugTick(float dt)
    {
      if (!MBDebug.IsDisplayingHighLevelAI)
        return;
      TacticComponent currentTactic = this.CurrentTactic;
      if (Input.DebugInput.IsHotKeyPressed("UsableMachineAiBaseHotkeyRetreatScriptActive"))
        TeamAIComponent._retreatScriptActive = true;
      else if (Input.DebugInput.IsHotKeyPressed("UsableMachineAiBaseHotkeyRetreatScriptPassive"))
        TeamAIComponent._retreatScriptActive = false;
      int num = TeamAIComponent._retreatScriptActive ? 1 : 0;
    }

    internal virtual void CreateMissionSpecificBehaviours()
    {
    }

    internal virtual void InitializeDetachments(Mission mission) => this.Mission.GetMissionBehaviour<DeploymentHandler>()?.InitializeDeploymentPoints();

    protected class TacticOption
    {
      public string Id { get; private set; }

      public Lazy<TacticComponent> Tactic { get; private set; }

      public float Weight { get; set; }

      public TacticOption(string id, Lazy<TacticComponent> tactic, float weight)
      {
        this.Id = id;
        this.Tactic = tactic;
        this.Weight = weight;
      }
    }
  }
}
