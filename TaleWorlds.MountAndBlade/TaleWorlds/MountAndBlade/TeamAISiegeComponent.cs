// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TeamAISiegeComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class TeamAISiegeComponent : TeamAIComponent
  {
    internal const int InsideCastleNavMeshID = 1;
    internal const int SiegeTokenForceSize = 15;
    private const float FormationInsideCastleThresholdPercentage = 0.4f;
    private const float CastleBreachThresholdPercentage = 0.7f;
    protected readonly IEnumerable<CastleGate> CastleGates;
    protected readonly IEnumerable<WallSegment> WallSegments;
    private readonly IEnumerable<SiegeLadder> _ladders;
    protected BatteringRam Ram;
    protected readonly List<SiegeTower> SiegeTowers;
    protected List<MissionObject> CastleKeyPositions;
    private bool _noProperLaneRemains;

    internal static List<SiegeLane> SiegeLanes { get; private set; }

    protected internal static SiegeQuerySystem QuerySystem { get; protected set; }

    public CastleGate OuterGate { get; }

    public CastleGate InnerGate { get; }

    internal MBReadOnlyList<SiegeLadder> Ladders => new MBReadOnlyList<SiegeLadder>(this._ladders.ToList<SiegeLadder>());

    internal List<SiegeWeapon> PrimarySiegeWeapons { get; }

    protected TeamAISiegeComponent(
      Mission currentMission,
      Team currentTeam,
      float thinkTimerTime,
      float applyTimerTime)
      : base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
    {
      this.CastleGates = (IEnumerable<CastleGate>) currentMission.ActiveMissionObjects.FindAllWithType<CastleGate>().ToList<CastleGate>();
      this.WallSegments = (IEnumerable<WallSegment>) currentMission.ActiveMissionObjects.FindAllWithType<WallSegment>().ToList<WallSegment>();
      this.OuterGate = this.CastleGates.FirstOrDefault<CastleGate>((Func<CastleGate, bool>) (g => g.GameEntity.HasTag("outer_gate")));
      this.InnerGate = this.CastleGates.FirstOrDefault<CastleGate>((Func<CastleGate, bool>) (g => g.GameEntity.HasTag("inner_gate")));
      this._ladders = currentMission.ActiveMissionObjects.FindAllWithType<SiegeLadder>();
      this.Ram = currentMission.ActiveMissionObjects.FindAllWithType<BatteringRam>().FirstOrDefault<BatteringRam>();
      this.SiegeTowers = currentMission.ActiveMissionObjects.FindAllWithType<SiegeTower>().ToList<SiegeTower>();
      this.PrimarySiegeWeapons = new List<SiegeWeapon>();
      this.PrimarySiegeWeapons.AddRange((IEnumerable<SiegeWeapon>) this._ladders);
      if (this.Ram != null)
        this.PrimarySiegeWeapons.Add((SiegeWeapon) this.Ram);
      this.PrimarySiegeWeapons.AddRange((IEnumerable<SiegeWeapon>) this.SiegeTowers);
      this.CastleKeyPositions = new List<MissionObject>();
      this.CastleKeyPositions.AddRange((IEnumerable<MissionObject>) this.CastleGates);
      this.CastleKeyPositions.AddRange((IEnumerable<MissionObject>) this.WallSegments);
      TeamAISiegeComponent.SiegeLanes = new List<SiegeLane>();
      for (int i = 0; i < 3; i++)
      {
        TeamAISiegeComponent.SiegeLanes.Add(new SiegeLane((FormationAI.BehaviorSide) i, TeamAISiegeComponent.QuerySystem));
        TeamAISiegeComponent.SiegeLanes[i].SetPrimarySiegeWeapons(this.PrimarySiegeWeapons.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (psw => psw is IPrimarySiegeWeapon && (psw as IPrimarySiegeWeapon).WeaponSide == (FormationAI.BehaviorSide) i)).Select<SiegeWeapon, IPrimarySiegeWeapon>((Func<SiegeWeapon, IPrimarySiegeWeapon>) (um => um as IPrimarySiegeWeapon)).ToList<IPrimarySiegeWeapon>());
        TeamAISiegeComponent.SiegeLanes[i].SetDefensePoints(this.CastleKeyPositions.Where<MissionObject>((Func<MissionObject, bool>) (ckp => (ckp as ICastleKeyPosition).DefenseSide == (FormationAI.BehaviorSide) i)).Select<MissionObject, ICastleKeyPosition>((Func<MissionObject, ICastleKeyPosition>) (dp => dp as ICastleKeyPosition)).ToList<ICastleKeyPosition>());
        TeamAISiegeComponent.SiegeLanes[i].RefreshLane();
      }
      TeamAISiegeComponent.SiegeLanes.ForEach((Action<SiegeLane>) (sl => sl.SetSiegeQuerySystem(TeamAISiegeComponent.QuerySystem)));
    }

    internal override void Tick(float dt)
    {
      if (!this._noProperLaneRemains)
      {
        List<SiegeLane> siegeLaneList = new List<SiegeLane>();
        foreach (SiegeLane siegeLane in TeamAISiegeComponent.SiegeLanes)
        {
          siegeLane.RefreshLane();
          siegeLane.DetermineLaneState();
        }
        if (!TeamAISiegeComponent.SiegeLanes.Any<SiegeLane>())
        {
          this._noProperLaneRemains = true;
          foreach (FormationAI.BehaviorSide behaviorSide in this.CastleKeyPositions.Where<MissionObject>((Func<MissionObject, bool>) (ckp => ckp is CastleGate && (ckp as CastleGate).DefenseSide != FormationAI.BehaviorSide.BehaviorSideNotSet)).Select<MissionObject, FormationAI.BehaviorSide>((Func<MissionObject, FormationAI.BehaviorSide>) (ckp => (ckp as CastleGate).DefenseSide)))
          {
            FormationAI.BehaviorSide difficultLaneSide = behaviorSide;
            SiegeLane siegeLane = new SiegeLane(difficultLaneSide, TeamAISiegeComponent.QuerySystem);
            siegeLane.SetPrimarySiegeWeapons(new List<IPrimarySiegeWeapon>());
            siegeLane.SetDefensePoints(this.CastleKeyPositions.Where<MissionObject>((Func<MissionObject, bool>) (ckp => (ckp as ICastleKeyPosition).DefenseSide == difficultLaneSide && ckp is CastleGate)).Select<MissionObject, ICastleKeyPosition>((Func<MissionObject, ICastleKeyPosition>) (dp => dp as ICastleKeyPosition)).ToList<ICastleKeyPosition>());
            siegeLane.RefreshLane();
            siegeLane.DetermineLaneState();
            TeamAISiegeComponent.SiegeLanes.Add(siegeLane);
          }
        }
      }
      else
      {
        foreach (SiegeLane siegeLane in TeamAISiegeComponent.SiegeLanes)
        {
          siegeLane.RefreshLane();
          siegeLane.DetermineLaneState();
        }
      }
      base.Tick(dt);
    }

    internal bool IsChargePastWallsApplicable
    {
      get
      {
        switch (this)
        {
          case TeamAISiegeAttacker _ when TeamAISiegeComponent.SiegeLanes.All<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.IsOpen)):
            return true;
          case TeamAISiegeDefender _:
            return !(this.CurrentTactic is TacticDefendCastle) || (this.CurrentTactic as TacticDefendCastle)._tacticState == TacticDefendCastle.TacticState.SallyOut;
          default:
            return false;
        }
      }
    }

    internal static void OnMissionFinalize()
    {
      if (TeamAISiegeComponent.SiegeLanes != null)
      {
        TeamAISiegeComponent.SiegeLanes.Clear();
        TeamAISiegeComponent.SiegeLanes = (List<SiegeLane>) null;
      }
      TeamAISiegeComponent.QuerySystem = (SiegeQuerySystem) null;
    }

    internal bool CalculateIsChargePastWallsApplicable(FormationAI.BehaviorSide side)
    {
      if (side == FormationAI.BehaviorSide.BehaviorSideNotSet && this.InnerGate != null && !this.InnerGate.IsGateOpen)
        return false;
      foreach (SiegeLane siegeLane in TeamAISiegeComponent.SiegeLanes)
      {
        if (side == FormationAI.BehaviorSide.BehaviorSideNotSet)
        {
          if (!siegeLane.IsOpen)
            return false;
        }
        else if (side == siegeLane.LaneSide)
          return siegeLane.IsOpen && (siegeLane.IsBreach || siegeLane.HasGate && (this.InnerGate == null || this.InnerGate.IsGateOpen));
      }
      return true;
    }

    internal bool AreLaddersReady { get; private set; }

    internal void SetAreLaddersReady(bool areLaddersReady) => this.AreLaddersReady = areLaddersReady;

    public bool IsAnyLaneThroughWallsOpen() => TeamAISiegeComponent.SiegeLanes.Any<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.IsOpen));

    public static bool IsFormationGroupInsideCastle(
      IEnumerable<Formation> formationGroup,
      bool includeOnlyPositionedUnits,
      float thresholdPercentage = 0.4f)
    {
      int num1 = 0;
      foreach (Formation formation in formationGroup)
        num1 += includeOnlyPositionedUnits ? formation.arrangement.PositionedUnitCount : formation.CountOfUnits;
      float num2 = (float) num1 * thresholdPercentage;
      foreach (Formation formation in formationGroup)
      {
        num2 -= (float) formation.CountUnitsOnNavMeshIDMod10(1, includeOnlyPositionedUnits);
        if ((double) num2 <= 0.0)
          return true;
      }
      return false;
    }

    public static bool IsFormationInsideCastle(
      Formation formation,
      bool includeOnlyPositionedUnits,
      float thresholdPercentage = 0.4f)
    {
      int num1 = includeOnlyPositionedUnits ? formation.arrangement.PositionedUnitCount : formation.CountOfUnits;
      float num2 = (float) num1 * thresholdPercentage;
      if (num1 != 0)
        return (double) formation.CountUnitsOnNavMeshIDMod10(1, includeOnlyPositionedUnits) >= (double) num2;
      return !(formation.Team.TeamAI is TeamAISiegeAttacker) && !(formation.Team.TeamAI is TeamAISallyOutDefender) && (formation.Team.TeamAI is TeamAISiegeDefender || formation.Team.TeamAI is TeamAISallyOutAttacker);
    }

    public IEnumerable<Formation> GetAttackerFormationsInsideWalls()
    {
      IEnumerable<Formation> formations = this.Mission.AttackerTeam.FormationsIncludingSpecial;
      if (this.Mission.AttackerAllyTeam != null)
        formations = formations.Concat<Formation>(this.Mission.AttackerAllyTeam.FormationsIncludingSpecial);
      return formations.Where<Formation>((Func<Formation, bool>) (af => TeamAISiegeComponent.IsFormationInsideCastle(af, true)));
    }

    public int GetFormationsInsideWallsCount() => this.GetAttackerFormationsInsideWalls().Count<Formation>();

    public bool IsCastleBreached()
    {
      IEnumerable<Formation> formations = this.Mission.AttackerTeam.FormationsIncludingSpecial;
      if (this.Mission.AttackerAllyTeam != null)
        formations = formations.Concat<Formation>(this.Mission.AttackerAllyTeam.FormationsIncludingSpecial);
      float num1 = (float) formations.Count<Formation>() * 0.7f;
      int num2 = 0;
      foreach (Formation formation in formations)
      {
        if (TeamAISiegeComponent.IsFormationInsideCastle(formation, true))
          ++num2;
        if ((double) num2 >= (double) num1)
          return true;
      }
      return false;
    }
  }
}
