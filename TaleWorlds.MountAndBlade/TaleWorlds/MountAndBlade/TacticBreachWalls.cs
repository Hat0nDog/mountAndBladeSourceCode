// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticBreachWalls
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class TacticBreachWalls : TacticComponent
  {
    private List<UsableMachine> secondarySiegeWeapons;
    internal const float _sameBehaviourFactor = 3f;
    internal const float _sameSideFactor = 5f;
    private TeamAISiegeAttacker _teamAISiegeAttacker;
    private TacticBreachWalls.BreachWallsProgressIndicators _indicators;
    private List<Formation> _meleeFormations;
    private List<Formation> _rangedFormations;
    private int _laneCount;
    private int _lanesInUse;
    private int _AIControlledFormationCount;
    private TacticBreachWalls.TacticState _tacticState;

    public TacticBreachWalls(Team team)
      : base(team)
    {
      IEnumerable<RangedSiegeWeapon> rangedSiegeWeapons = Mission.Current.ActiveMissionObjects.FindAllWithType<RangedSiegeWeapon>().Where<RangedSiegeWeapon>((Func<RangedSiegeWeapon, bool>) (rsw => rsw.Side == team.Side));
      this.secondarySiegeWeapons = new List<UsableMachine>();
      this.secondarySiegeWeapons.AddRange((IEnumerable<UsableMachine>) rangedSiegeWeapons);
      this._teamAISiegeAttacker = team.TeamAI as TeamAISiegeAttacker;
      this._meleeFormations = new List<Formation>();
      this._rangedFormations = new List<Formation>();
    }

    private float GetFormationBehaviourLaneEffectiveness(
      SiegeLane siegeLane,
      Formation formation,
      BehaviorComponent behaviour)
    {
      switch (behaviour)
      {
        case BehaviorAssaultWalls _:
          return (float) ((formation.AI.Side == siegeLane.LaneSide ? 5.0 : 1.0) * (formation.AI.ActiveBehavior == null || !(formation.AI.ActiveBehavior.GetType() == behaviour.GetType()) ? 1.0 : 3.0)) * TacticComponent.GetFormationEffectivenessOverOrder(formation, OrderType.Charge) * siegeLane.PrimarySiegeWeapons.Where<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw.HasCompletedAction())).Max<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, float>) (psw => psw.SiegeWeaponPriority));
        case BehaviorUseSiegeMachines _:
          return (float) ((formation.AI.Side == siegeLane.LaneSide ? 5.0 : 1.0) * (formation.AI.ActiveBehavior == null || !(formation.AI.ActiveBehavior.GetType() == behaviour.GetType()) ? 1.0 : 3.0)) * TacticComponent.GetFormationEffectivenessOverOrder(formation, OrderType.Charge) * siegeLane.PrimarySiegeWeapons.Where<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => !psw.HasCompletedAction())).Max<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, float>) (psw => psw.SiegeWeaponPriority));
        default:
          return 0.0f;
      }
    }

    private void BalanceAssaultLanes(List<Formation> attackerFormations)
    {
      if (attackerFormations.Count < 2)
        return;
      int num1 = attackerFormations.Sum<Formation>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes));
      int idealCount = num1 / attackerFormations.Count;
      int num2 = Math.Max((int) ((double) num1 * 0.100000001490116), 1);
      using (List<Formation>.Enumerator enumerator = attackerFormations.GetEnumerator())
      {
label_7:
        while (enumerator.MoveNext())
        {
          Formation current = enumerator.Current;
          int num3 = 0;
          while (true)
          {
            if (idealCount - current.CountOfUnitsWithoutDetachedOnes > num2 && (attackerFormations.Any<Formation>((Func<Formation, bool>) (af => af.CountOfUnitsWithoutDetachedOnes > idealCount)) && num3 < attackerFormations.Count))
            {
              int val1 = idealCount - current.CountOfUnitsWithoutDetachedOnes;
              Formation formation = attackerFormations.MaxBy<Formation, int>((Func<Formation, int>) (df => df.CountOfUnitsWithoutDetachedOnes - idealCount));
              int unitCount = Math.Min(val1, formation.CountOfUnitsWithoutDetachedOnes - idealCount);
              formation.TransferUnits(current, unitCount);
              ++num3;
            }
            else
              goto label_7;
          }
        }
      }
    }

    private bool ShouldRetreat(IEnumerable<SiegeLane> lanes, int insideFormationCount) => this._indicators != null && (double) this.team.QuerySystem.OverallPowerRatio / (double) this._indicators.StartingPowerRatio < (double) this._indicators.GetRetreatThresholdRatio(lanes, insideFormationCount);

    private void AssignMeleeFormationsToLanes(
      List<Formation> meleeFormationsSource,
      List<SiegeLane> currentLanes)
    {
      List<Formation> meleeFormations = new List<Formation>(meleeFormationsSource.Count);
      meleeFormations.AddRange((IEnumerable<Formation>) meleeFormationsSource);
      List<SiegeLane> list = currentLanes.ToList<SiegeLane>();
      Formation strongestFormation = meleeFormations.Any<Formation>() ? meleeFormations.MaxBy<Formation, float>((Func<Formation, float>) (mf => mf.QuerySystem.FormationPower)) : (Formation) null;
      bool flag = meleeFormations.Count<Formation>() <= 1 || (double) strongestFormation.QuerySystem.FormationPower > (double) meleeFormations.Where<Formation>((Func<Formation, bool>) (f => f != strongestFormation)).Average<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationPower)) * 2.0;
      while (meleeFormations.Any<Formation>() && list.Any<SiegeLane>())
      {
        SiegeLane assaultedLane = list.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (l => meleeFormations.Any<Formation>((Func<Formation, bool>) (mf => mf.AI.Side == l.LaneSide))));
        if (assaultedLane != null)
        {
          IEnumerable<Formation> source = meleeFormations.Where<Formation>((Func<Formation, bool>) (mf => mf.AI.Side == assaultedLane.LaneSide));
          Formation f = source.Any<Formation>((Func<Formation, bool>) (pdf => pdf.IsAIControlled)) ? source.Where<Formation>((Func<Formation, bool>) (pdf => pdf.IsAIControlled)).MaxBy<Formation, float>((Func<Formation, float>) (pdf => pdf.QuerySystem.FormationPower)) : source.MaxBy<Formation, float>((Func<Formation, float>) (pdf => pdf.QuerySystem.FormationPower));
          f.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(f);
          f.AI.SetBehaviorWeight<BehaviorAssaultWalls>(1f);
          f.AI.SetBehaviorWeight<BehaviorUseSiegeMachines>(1f);
          f.AI.SetBehaviorWeight<BehaviorWaitForLadders>(1f);
          meleeFormations.Remove(f);
          list.Remove(assaultedLane);
        }
        else
        {
          Formation f = meleeFormations.Any<Formation>((Func<Formation, bool>) (mf => mf.IsAIControlled)) ? meleeFormations.Where<Formation>((Func<Formation, bool>) (mf => mf.IsAIControlled)).MaxBy<Formation, float>((Func<Formation, float>) (mf => mf.QuerySystem.FormationPower)) : meleeFormations.MaxBy<Formation, float>((Func<Formation, float>) (mf => mf.QuerySystem.FormationPower));
          SiegeLane siegeLane = flag ? list.MinBy<SiegeLane, float>((Func<SiegeLane, float>) (l => l.LaneDifficulty)) : list.MaxBy<SiegeLane, float>((Func<SiegeLane, float>) (l => l.LaneDifficulty));
          f.AI.Side = siegeLane.LaneSide;
          f.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(f);
          f.AI.SetBehaviorWeight<BehaviorAssaultWalls>(1f);
          f.AI.SetBehaviorWeight<BehaviorUseSiegeMachines>(1f);
          f.AI.SetBehaviorWeight<BehaviorWaitForLadders>(1f);
          meleeFormations.Remove(f);
          list.Remove(siegeLane);
        }
      }
      while (meleeFormations.Any<Formation>())
      {
        if (list.IsEmpty<SiegeLane>())
          list.AddRange((IEnumerable<SiegeLane>) currentLanes);
        Formation f = meleeFormations.MaxBy<Formation, float>((Func<Formation, float>) (mf => mf.QuerySystem.FormationPower));
        SiegeLane siegeLane = list.MaxBy<SiegeLane, float>((Func<SiegeLane, float>) (l => l.GetLaneCapacity()));
        f.AI.Side = siegeLane.LaneSide;
        f.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(f);
        f.AI.SetBehaviorWeight<BehaviorAssaultWalls>(1f);
        f.AI.SetBehaviorWeight<BehaviorUseSiegeMachines>(1f);
        f.AI.SetBehaviorWeight<BehaviorWaitForLadders>(1f);
        meleeFormations.Remove(f);
        list.Remove(siegeLane);
      }
    }

    private void WellRoundedAssault()
    {
      List<SiegeLane> currentLanes = this.DetermineCurrentLanes();
      this.AssignMeleeFormationsToLanes(this._meleeFormations, currentLanes);
      List<ArcherPosition> currentArcherPositions = this.DetermineCurrentArcherPositions(currentLanes);
      foreach (Formation rangedFormation in this._rangedFormations)
      {
        rangedFormation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(rangedFormation);
        rangedFormation.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
      }
      if (!currentArcherPositions.Any<ArcherPosition>())
        return;
      foreach (Formation rangedFormation in this._rangedFormations)
      {
        if (currentArcherPositions.IsEmpty<ArcherPosition>())
          currentArcherPositions.AddRange((IEnumerable<ArcherPosition>) this._teamAISiegeAttacker.ArcherPositions);
        ArcherPosition randomElement = currentArcherPositions.GetRandomElement<ArcherPosition>();
        rangedFormation.AI.SetBehaviorWeight<BehaviorSparseSkirmish>(1f);
        rangedFormation.AI.GetBehavior<BehaviorSparseSkirmish>().ArcherPosition = randomElement.Entity;
        currentArcherPositions.Remove(randomElement);
      }
    }

    private void AllInAssault() => this.AssignMeleeFormationsToLanes(this.Formations.ToList<Formation>(), this.DetermineCurrentLanes());

    private void StartTacticalRetreat()
    {
      this.StopUsingAllMachines();
      foreach (Formation formation in this.Formations)
      {
        formation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(formation);
        formation.AI.SetBehaviorWeight<BehaviorRetreatToKeep>(1f);
      }
    }

    protected override bool CheckAndSetAvailableFormationsChanged()
    {
      bool flag1 = false;
      int count = this.DetermineCurrentLanes().Count;
      if (this._laneCount != count)
      {
        this._laneCount = count;
        flag1 = true;
      }
      int num = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
      bool flag2 = num != this._AIControlledFormationCount;
      if (flag2)
        this._AIControlledFormationCount = num;
      bool flag3 = false;
      bool flag4 = false;
      if (this._tacticState == TacticBreachWalls.TacticState.AssaultUnderRangedCover)
      {
        if (this._meleeFormations.Count != this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)) || this._rangedFormations.Count != this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)))
        {
          flag3 = true;
          this._meleeFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation && f.CountOfUnitsWithoutDetachedOnes > 0)).ToList<Formation>();
          this._rangedFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedCavalryFormation && f.CountOfUnitsWithoutDetachedOnes > 0)).ToList<Formation>();
        }
      }
      else if (this._tacticState == TacticBreachWalls.TacticState.TotalAttack && (this.Formations.Count<Formation>() < count && this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled)) > 0 || this.Formations.Count<Formation>() > count && (this.Formations.Count<Formation>((Func<Formation, bool>) (f => !f.IsAIControlled)) < count || this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled)) > 1)))
        flag4 = true;
      return flag1 | flag2 | flag3 | flag4;
    }

    protected override void ManageFormationCounts()
    {
      List<SiegeLane> currentLanes = this.DetermineCurrentLanes();
      if (this._indicators == null && this.team.QuerySystem.EnemyUnitCount > 0)
      {
        this._indicators = new TacticBreachWalls.BreachWallsProgressIndicators(this.team, currentLanes);
        this._indicators.StartingPowerRatio = this.team.QuerySystem.OverallPowerRatio;
        this._indicators.InitialLaneCount = currentLanes.Count;
      }
      if (this._tacticState == TacticBreachWalls.TacticState.Retreating)
        return;
      int count = currentLanes.Count;
      if (this._tacticState == TacticBreachWalls.TacticState.AssaultUnderRangedCover)
      {
        int rangedCount = Math.Min(this.DetermineCurrentArcherPositions(currentLanes).Count, 8 - count);
        this.ManageFormationCounts(count, rangedCount, 0, 0);
        this._meleeFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation && f.CountOfUnitsWithoutDetachedOnes > 0)).ToList<Formation>();
        this._rangedFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation && f.CountOfUnitsWithoutDetachedOnes > 0)).ToList<Formation>();
      }
      else
      {
        if (this._tacticState != TacticBreachWalls.TacticState.TotalAttack)
          return;
        this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => true), count);
      }
    }

    private void CheckAndChangeState()
    {
      this.Formations.Where<Formation>((Func<Formation, bool>) (f => TeamAISiegeComponent.IsFormationInsideCastle(f, true)));
      if (this._tacticState == TacticBreachWalls.TacticState.Retreating)
        return;
      if (this.ShouldRetreat((IEnumerable<SiegeLane>) this.DetermineCurrentLanes(), this.Formations.Count<Formation>((Func<Formation, bool>) (f => TeamAISiegeComponent.IsFormationInsideCastle(f, true)))))
      {
        this._tacticState = TacticBreachWalls.TacticState.Retreating;
        this.StartTacticalRetreat();
        this.IsTacticReapplyNeeded = false;
      }
      else
      {
        TacticBreachWalls.TacticState tacticState = TacticBreachWalls.TacticState.TotalAttack;
        if (this._tacticState != TacticBreachWalls.TacticState.TotalAttack)
        {
          double num1 = (double) Math.Max((float) this._meleeFormations.Sum<Formation>((Func<Formation, int>) (mf => mf.CountOfUnits)), 1f);
          float num2 = Math.Max((float) this._rangedFormations.Sum<Formation>((Func<Formation, int>) (rf => rf.CountOfUnits)), 1f);
          float num3 = (float) num1 + num2;
          float num4 = num3 - (float) this.Formations.Sum<Formation>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes));
          tacticState = num1 / (double) num2 <= 0.5 || (double) num4 / (double) num3 >= 0.200000002980232 ? TacticBreachWalls.TacticState.TotalAttack : TacticBreachWalls.TacticState.AssaultUnderRangedCover;
        }
        if (tacticState == this._tacticState)
          return;
        if (tacticState != TacticBreachWalls.TacticState.AssaultUnderRangedCover)
        {
          if (tacticState != TacticBreachWalls.TacticState.TotalAttack)
            return;
          this._tacticState = TacticBreachWalls.TacticState.TotalAttack;
          this.ManageFormationCounts();
          this.AllInAssault();
          this.IsTacticReapplyNeeded = false;
        }
        else
        {
          this._tacticState = TacticBreachWalls.TacticState.AssaultUnderRangedCover;
          this.ManageFormationCounts();
          this.WellRoundedAssault();
          this.IsTacticReapplyNeeded = false;
        }
      }
    }

    private List<SiegeLane> DetermineCurrentLanes()
    {
      List<SiegeLane> list1 = TeamAISiegeComponent.SiegeLanes.Where<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.IsBreach)).ToList<SiegeLane>();
      if (list1.Count >= 2)
        return list1;
      List<SiegeLane> list2 = TeamAISiegeComponent.SiegeLanes.Where<SiegeLane>((Func<SiegeLane, bool>) (sl => !sl.IsLaneUnusable)).ToList<SiegeLane>();
      if (!list2.Any<SiegeLane>())
        return TeamAISiegeComponent.SiegeLanes.Where<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.HasGate)).ToList<SiegeLane>();
      return list1.Count >= 1 ? list2.Where<SiegeLane>((Func<SiegeLane, bool>) (l => l.IsBreach || l.PrimarySiegeWeapons.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => !(psw is SiegeLadder))))).ToList<SiegeLane>() : list2;
    }

    private List<ArcherPosition> DetermineCurrentArcherPositions(
      List<SiegeLane> currentLanes)
    {
      return this._teamAISiegeAttacker.ArcherPositions.Where<ArcherPosition>((Func<ArcherPosition, bool>) (ap => currentLanes.Any<SiegeLane>((Func<SiegeLane, bool>) (cl => ap.IsArcherPositionRelatedToSide(cl.LaneSide))))).ToList<ArcherPosition>();
    }

    protected internal override void TickOccasionally()
    {
      if (!this.AreFormationsCreated)
        return;
      this._meleeFormations.RemoveAll((Predicate<Formation>) (mf => mf.CountOfUnitsWithoutDetachedOnes == 0));
      this._rangedFormations.RemoveAll((Predicate<Formation>) (rf => rf.CountOfUnitsWithoutDetachedOnes == 0));
      bool flag = this.CheckAndSetAvailableFormationsChanged();
      List<SiegeLane> currentLanes = this.DetermineCurrentLanes();
      int num1 = 0;
      foreach (SiegeLane siegeLane in currentLanes)
        num1 |= (int) Math.Pow(2.0, (double) siegeLane.LaneSide);
      this.IsTacticReapplyNeeded = num1 != this._lanesInUse;
      this._lanesInUse = num1;
      if (flag)
        this.ManageFormationCounts();
      this.CheckAndChangeState();
      int num2 = flag ? 1 : 0;
      switch (this._tacticState)
      {
        case TacticBreachWalls.TacticState.AssaultUnderRangedCover:
          if (flag || this.IsTacticReapplyNeeded)
          {
            this.WellRoundedAssault();
            this.IsTacticReapplyNeeded = false;
          }
          this.BalanceAssaultLanes(this._meleeFormations.Where<Formation>((Func<Formation, bool>) (mf => mf.IsAIControlled && mf.IsAITickedAfterSplit)).ToList<Formation>());
          break;
        case TacticBreachWalls.TacticState.TotalAttack:
          if (flag || this.IsTacticReapplyNeeded)
          {
            this.AllInAssault();
            this.IsTacticReapplyNeeded = false;
          }
          this.BalanceAssaultLanes(this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.IsAIControlled && f.IsAITickedAfterSplit)).ToList<Formation>());
          break;
        case TacticBreachWalls.TacticState.Retreating:
          if (flag || this.IsTacticReapplyNeeded)
          {
            this.StartTacticalRetreat();
            this.IsTacticReapplyNeeded = false;
            break;
          }
          break;
      }
      this._teamAISiegeAttacker.SetAreLaddersReady(currentLanes.Count<SiegeLane>((Func<SiegeLane, bool>) (l => l.IsBreach)) > 1 || !currentLanes.Any<SiegeLane>((Func<SiegeLane, bool>) (l => l.PrimarySiegeWeapons.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw.HoldLadders)))) || currentLanes.Any<SiegeLane>((Func<SiegeLane, bool>) (l => l.PrimarySiegeWeapons.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw.SendLadders)))));
      this.CheckAndSetAvailableFormationsChanged();
      base.TickOccasionally();
    }

    internal override float GetTacticWeight() => 10f;

    private class BreachWallsProgressIndicators
    {
      public float StartingPowerRatio;
      public int InitialLaneCount;
      private readonly float _insideFormationEffect;
      private readonly float _openLaneEffect;
      private readonly float _existingLaneEffect;

      public BreachWallsProgressIndicators(Team team, List<SiegeLane> lanes)
      {
        this.StartingPowerRatio = team.QuerySystem.OverallPowerRatio;
        this.InitialLaneCount = lanes.Count;
        this._insideFormationEffect = 1f / (float) this.InitialLaneCount;
        this._openLaneEffect = 0.7f / (float) this.InitialLaneCount;
        this._existingLaneEffect = 0.4f / (float) this.InitialLaneCount;
      }

      public float GetRetreatThresholdRatio(IEnumerable<SiegeLane> lanes, int insideFormationCount)
      {
        float num1 = 1f - (float) insideFormationCount * this._insideFormationEffect;
        int num2 = lanes.Count<SiegeLane>((Func<SiegeLane, bool>) (l => !l.IsOpen));
        int num3 = lanes.Count<SiegeLane>() - num2 - insideFormationCount;
        if (num3 > 0)
          num1 -= (float) num3 * this._openLaneEffect;
        return num1 - (float) num2 * this._existingLaneEffect;
      }
    }

    private enum TacticState
    {
      Unset,
      AssaultUnderRangedCover,
      TotalAttack,
      Retreating,
    }
  }
}
