// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TeamQuerySystem
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Missions.Handlers;

namespace TaleWorlds.MountAndBlade
{
  public class TeamQuerySystem
  {
    public readonly Team Team;
    private readonly Mission _mission;
    private readonly QueryData<int> _memberCount;
    private readonly QueryData<WorldPosition> _medianPosition;
    private readonly QueryData<Vec2> _averagePosition;
    private readonly QueryData<Vec2> _averageEnemyPosition;
    private readonly QueryData<WorldPosition> _medianTargetFormationPosition;
    private readonly QueryData<WorldPosition> _leftFlankEdgePosition;
    private readonly QueryData<WorldPosition> _rightFlankEdgePosition;
    private readonly QueryData<float> _infantryRatio;
    private readonly QueryData<float> _rangedRatio;
    private readonly QueryData<float> _cavalryRatio;
    private readonly QueryData<float> _rangedCavalryRatio;
    private readonly QueryData<int> _allyMemberCount;
    private readonly QueryData<int> _enemyMemberCount;
    private readonly QueryData<float> _allyInfantryRatio;
    private readonly QueryData<float> _allyRangedRatio;
    private readonly QueryData<float> _allyCavalryRatio;
    private readonly QueryData<float> _allyRangedCavalryRatio;
    private readonly QueryData<float> _enemyInfantryRatio;
    private readonly QueryData<float> _enemyRangedRatio;
    private readonly QueryData<float> _enemyCavalryRatio;
    private readonly QueryData<float> _enemyRangedCavalryRatio;
    private readonly QueryData<float> _overallPowerRatio;
    private readonly QueryData<float> _teamPower;
    private readonly QueryData<float> _powerRatioIncludingCasualties;
    private readonly QueryData<float> _insideWallsRatio;
    private readonly QueryData<float> _maxUnderRangedAttackRatio;

    internal IEnumerable<TeamQuerySystem> AllyTeams => this._mission.Teams.GetAlliesOf(this.Team, true).Select<Team, TeamQuerySystem>((Func<Team, TeamQuerySystem>) (t => t.QuerySystem));

    internal IEnumerable<TeamQuerySystem> EnemyTeams => this._mission.Teams.GetEnemiesOf(this.Team).Select<Team, TeamQuerySystem>((Func<Team, TeamQuerySystem>) (t => t.QuerySystem));

    public int MemberCount => this._memberCount.Value;

    public WorldPosition MedianPosition => this._medianPosition.Value;

    public Vec2 AveragePosition => this._averagePosition.Value;

    public Vec2 AverageEnemyPosition => this._averageEnemyPosition.Value;

    public WorldPosition MedianTargetFormationPosition => this._medianTargetFormationPosition.Value;

    public WorldPosition LeftFlankEdgePosition => this._leftFlankEdgePosition.Value;

    public WorldPosition RightFlankEdgePosition => this._rightFlankEdgePosition.Value;

    public float InfantryRatio => this._infantryRatio.Value;

    public float RangedRatio => this._rangedRatio.Value;

    public float CavalryRatio => this._cavalryRatio.Value;

    public float RangedCavalryRatio => this._rangedCavalryRatio.Value;

    public int AllyUnitCount => this._allyMemberCount.Value;

    public int EnemyUnitCount => this._enemyMemberCount.Value;

    public float AllyInfantryRatio => this._allyInfantryRatio.Value;

    public float AllyRangedRatio => this._allyRangedRatio.Value;

    public float AllyCavalryRatio => this._allyCavalryRatio.Value;

    public float AllyRangedCavalryRatio => this._allyRangedCavalryRatio.Value;

    public float EnemyInfantryRatio => this._enemyInfantryRatio.Value;

    public float EnemyRangedRatio => this._enemyRangedRatio.Value;

    public float EnemyCavalryRatio => this._enemyCavalryRatio.Value;

    public float EnemyRangedCavalryRatio => this._enemyRangedCavalryRatio.Value;

    public float OverallPowerRatio => this._overallPowerRatio.Value;

    public float TeamPower => this._teamPower.Value;

    public float PowerRatioIncludingCasualties => this._powerRatioIncludingCasualties.Value;

    public float InsideWallsRatio => this._insideWallsRatio.Value;

    public float MaxUnderRangedAttackRatio => this._maxUnderRangedAttackRatio.Value;

    internal int DeathCount { get; private set; }

    internal int DeathByRangedCount { get; private set; }

    public int AllyRangedUnitCount => (int) ((double) this.AllyRangedRatio * (double) this.AllyUnitCount);

    public int AllCavalryUnitCount => (int) ((double) this.AllyCavalryRatio * (double) this.AllyUnitCount);

    public int EnemyRangedUnitCount => (int) ((double) this.EnemyRangedRatio * (double) this.EnemyUnitCount);

    internal void Expire()
    {
      foreach (FieldInfo field in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
      {
        if (field.GetValue((object) this) is IQueryData queryData1)
        {
          queryData1.Expire();
          field.SetValue((object) this, (object) queryData1);
        }
      }
      foreach (Formation formation in this.Team.FormationsIncludingSpecial)
        formation.QuerySystem.Expire();
    }

    private void InitializeTelemetryScopeNames()
    {
    }

    public TeamQuerySystem(Team team)
    {
      TeamQuerySystem teamQuerySystem = this;
      this.Team = team;
      this._mission = Mission.Current;
      this._memberCount = new QueryData<int>((Func<int>) (() => teamQuerySystem.Team.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, int>) (f => f.CountOfUnits))), 2f);
      this._allyMemberCount = new QueryData<int>((Func<int>) (() => teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (t => t.MemberCount))), 2f);
      this._enemyMemberCount = new QueryData<int>((Func<int>) (() => teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (t => t.MemberCount))), 2f);
      this._averagePosition = new QueryData<Vec2>((Func<Vec2>) (() => teamQuerySystem._mission.GetAveragePositionOfTeam(team)), 5f);
      this._medianPosition = new QueryData<WorldPosition>((Func<WorldPosition>) (() => teamQuerySystem._mission.GetMedianPositionOfTeam(team, teamQuerySystem.AveragePosition)), 5f);
      this._averageEnemyPosition = new QueryData<Vec2>((Func<Vec2>) (() =>
      {
        Vec2 positionOfEnemies = teamQuerySystem._mission.GetAveragePositionOfEnemies(team);
        if (positionOfEnemies.IsValid)
          return positionOfEnemies;
        if (team.Side == BattleSideEnum.Attacker)
        {
          SiegeDeploymentHandler missionBehaviour = teamQuerySystem._mission.GetMissionBehaviour<SiegeDeploymentHandler>();
          if (missionBehaviour != null)
            return missionBehaviour.GetEstimatedAverageDefenderPosition();
        }
        return teamQuerySystem.AveragePosition.IsValid ? teamQuerySystem.AveragePosition : teamQuerySystem._mission.GetAveragePositionOfTeam(team);
      }), 5f);
      this._medianTargetFormationPosition = new QueryData<WorldPosition>((Func<WorldPosition>) (() => !teamQuerySystem._mission.Teams.Where<Team>((Func<Team, bool>) (t => t.IsEnemyOf(teamQuerySystem.Team))).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).Any<Formation>() ? teamQuerySystem.MedianPosition : teamQuerySystem._mission.Teams.Where<Team>((Func<Team, bool>) (t => t.IsEnemyOf(teamQuerySystem.Team))).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).MinBy<Formation, float>((Func<Formation, float>) (f => f.QuerySystem.MedianPosition.AsVec2.DistanceSquared(teamQuerySystem.AverageEnemyPosition))).QuerySystem.MedianPosition), 1f);
      QueryData<WorldPosition>.SetupSyncGroup((IQueryData) this._averageEnemyPosition, (IQueryData) this._medianTargetFormationPosition);
      this._leftFlankEdgePosition = new QueryData<WorldPosition>((Func<WorldPosition>) (() =>
      {
        Vec2 vec2 = (teamQuerySystem.MedianTargetFormationPosition.AsVec2 - teamQuerySystem.AveragePosition).RightVec();
        double num = (double) vec2.Normalize();
        WorldPosition formationPosition = teamQuerySystem.MedianTargetFormationPosition;
        formationPosition.SetVec2(formationPosition.AsVec2 - vec2 * 50f);
        return formationPosition;
      }), 5f);
      this._rightFlankEdgePosition = new QueryData<WorldPosition>((Func<WorldPosition>) (() =>
      {
        Vec2 vec2 = (teamQuerySystem.MedianTargetFormationPosition.AsVec2 - teamQuerySystem.AveragePosition).RightVec();
        double num = (double) vec2.Normalize();
        WorldPosition formationPosition = teamQuerySystem.MedianTargetFormationPosition;
        formationPosition.SetVec2(formationPosition.AsVec2 + vec2 * 50f);
        return formationPosition;
      }), 5f);
      this._infantryRatio = new QueryData<float>((Func<float>) (() => teamQuerySystem.MemberCount != 0 ? (teamQuerySystem.Team.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.InfantryUnitRatio * (float) f.CountOfUnits)) + (float) team.Heroes.Count<Agent>((Func<Agent, bool>) (h => QueryLibrary.IsInfantry(h)))) / (float) teamQuerySystem.MemberCount : 0.0f), 15f);
      this._rangedRatio = new QueryData<float>((Func<float>) (() => teamQuerySystem.MemberCount != 0 ? (teamQuerySystem.Team.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.RangedUnitRatio * (float) f.CountOfUnits)) + (float) team.Heroes.Count<Agent>((Func<Agent, bool>) (h => QueryLibrary.IsRanged(h)))) / (float) teamQuerySystem.MemberCount : 0.0f), 15f);
      this._cavalryRatio = new QueryData<float>((Func<float>) (() => teamQuerySystem.MemberCount != 0 ? (teamQuerySystem.Team.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.CavalryUnitRatio * (float) f.CountOfUnits)) + (float) team.Heroes.Count<Agent>((Func<Agent, bool>) (h => QueryLibrary.IsCavalry(h)))) / (float) teamQuerySystem.MemberCount : 0.0f), 15f);
      this._rangedCavalryRatio = new QueryData<float>((Func<float>) (() => teamQuerySystem.MemberCount != 0 ? (teamQuerySystem.Team.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.RangedCavalryUnitRatio * (float) f.CountOfUnits)) + (float) team.Heroes.Count<Agent>((Func<Agent, bool>) (h => QueryLibrary.IsRangedCavalry(h)))) / (float) teamQuerySystem.MemberCount : 0.0f), 15f);
      QueryData<float>.SetupSyncGroup((IQueryData) this._infantryRatio, (IQueryData) this._rangedRatio, (IQueryData) this._cavalryRatio, (IQueryData) this._rangedCavalryRatio);
      this._allyInfantryRatio = new QueryData<float>((Func<float>) (() =>
      {
        int num = teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (a => a.MemberCount));
        return num == 0 ? 0.0f : teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (a => a.InfantryRatio * (float) a.MemberCount)) / (float) num;
      }), 15f);
      this._allyRangedRatio = new QueryData<float>((Func<float>) (() =>
      {
        int num = teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (a => a.MemberCount));
        return num == 0 ? 0.0f : teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (a => a.RangedRatio * (float) a.MemberCount)) / (float) num;
      }), 15f);
      this._allyCavalryRatio = new QueryData<float>((Func<float>) (() =>
      {
        int num = teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (a => a.MemberCount));
        return num == 0 ? 0.0f : teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (a => a.CavalryRatio * (float) a.MemberCount)) / (float) num;
      }), 15f);
      this._allyRangedCavalryRatio = new QueryData<float>((Func<float>) (() =>
      {
        int num = teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (a => a.MemberCount));
        return num == 0 ? 0.0f : teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (a => a.RangedCavalryRatio * (float) a.MemberCount)) / (float) num;
      }), 15f);
      QueryData<float>.SetupSyncGroup((IQueryData) this._allyInfantryRatio, (IQueryData) this._allyRangedRatio, (IQueryData) this._allyCavalryRatio, (IQueryData) this._allyRangedCavalryRatio);
      this._enemyInfantryRatio = new QueryData<float>((Func<float>) (() => teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (e => e.MemberCount)) != 0 ? teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (e => e.InfantryRatio * (float) e.MemberCount)) / (float) teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (e => e.MemberCount)) : 0.0f), 15f);
      this._enemyRangedRatio = new QueryData<float>((Func<float>) (() => teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (e => e.MemberCount)) != 0 ? teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (e => e.RangedRatio * (float) e.MemberCount)) / (float) teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (e => e.MemberCount)) : 0.0f), 15f);
      this._enemyCavalryRatio = new QueryData<float>((Func<float>) (() => teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (e => e.MemberCount)) != 0 ? teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (e => e.CavalryRatio * (float) e.MemberCount)) / (float) teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (e => e.MemberCount)) : 0.0f), 15f);
      this._enemyRangedCavalryRatio = new QueryData<float>((Func<float>) (() => teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (e => e.MemberCount)) != 0 ? teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (e => e.RangedCavalryRatio * (float) e.MemberCount)) / (float) teamQuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, int>) (e => e.MemberCount)) : 0.0f), 15f);
      this._teamPower = new QueryData<float>((Func<float>) (() => team.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.GetFormationPower()))), 5f);
      this._overallPowerRatio = new QueryData<float>((Func<float>) (() => (float) ((double) teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (at => at.Team.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationPower)) + 1f)) * 1.0 / ((double) teamQuerySystem._mission.Teams.GetEnemiesOf(team).Sum<Team>((Func<Team, float>) (et => et.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationPower)))) + 1.0))), 5f);
      this._powerRatioIncludingCasualties = new QueryData<float>((Func<float>) (() =>
      {
        CasualtyHandler casualtyHandler = teamQuerySystem._mission.GetMissionBehaviour<CasualtyHandler>();
        return (float) (((double) teamQuerySystem.AllyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (at => at.Team.FormationsIncludingSpecialAndEmpty.Sum<Formation>((Func<Formation, float>) (f => f.GetFormationPower() + casualtyHandler.GetCasualtyPowerLossOfFormation(f))))) + 1.0) / ((double) teamQuerySystem._mission.Teams.GetEnemiesOf(team).Sum<Team>((Func<Team, float>) (et => et.FormationsIncludingSpecialAndEmpty.Sum<Formation>((Func<Formation, float>) (f => f.GetFormationPower() + casualtyHandler.GetCasualtyPowerLossOfFormation(f))))) + 1.0));
      }), 10f);
      this._insideWallsRatio = new QueryData<float>((Func<float>) (() =>
      {
        if (!(team.TeamAI is TeamAISiegeComponent))
          return 1f;
        if (teamQuerySystem.AllyUnitCount == 0)
          return 0.0f;
        int num = 0;
        foreach (Team team1 in Mission.Current.Teams.GetAlliesOf(team, true))
        {
          foreach (Formation formation in team1.FormationsIncludingSpecial)
            num += formation.CountUnitsOnNavMeshIDMod10(1, false);
        }
        return (float) num / (float) teamQuerySystem.AllyUnitCount;
      }), 10f);
      this._maxUnderRangedAttackRatio = new QueryData<float>((Func<float>) (() =>
      {
        float num1;
        if (teamQuerySystem.AllyUnitCount == 0)
        {
          num1 = 0.0f;
        }
        else
        {
          float currentTime = MBCommon.TimeType.Mission.GetTime();
          int num2 = 0;
          foreach (TeamQuerySystem allyTeam in teamQuerySystem.AllyTeams)
          {
            foreach (Formation formation in allyTeam.Team.Formations)
              num2 += formation.GetCountOfUnitsWithCondition((Func<Agent, bool>) (agent => (double) currentTime - (double) agent.LastRangedHitTime < 10.0 && !agent.Equipment.HasShield()));
          }
          num1 = (float) num2 / (float) teamQuerySystem.AllyUnitCount;
        }
        return (double) num1 > (double) teamQuerySystem._maxUnderRangedAttackRatio.GetCachedValue() ? num1 : teamQuerySystem._maxUnderRangedAttackRatio.GetCachedValue();
      }), 3f);
      this.DeathCount = 0;
      this.DeathByRangedCount = 0;
      this.InitializeTelemetryScopeNames();
    }

    internal void RegisterDeath() => ++this.DeathCount;

    internal void RegisterDeathByRanged() => ++this.DeathByRangedCount;

    public float GetLocalAllyPower(Vec2 target) => this.Team.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationPower / f.QuerySystem.AveragePosition.Distance(target)));

    public float GetLocalEnemyPower(Vec2 target) => Mission.Current.Teams.GetEnemiesOf(this.Team).Sum<Team>((Func<Team, float>) (t => t.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationPower / f.QuerySystem.AveragePosition.Distance(target)))));
  }
}
