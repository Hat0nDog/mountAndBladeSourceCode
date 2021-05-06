// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticDefendCastle
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class TacticDefendCastle : TacticComponent
  {
    private const float InfantrySallyOutEffectiveness = 1f;
    private const float RangedSallyOutEffectiveness = 0.3f;
    private const float CavalrySallyOutEffectiveness = 2f;
    private const float SallyOutDecisionPenalty = 3f;
    private const float _insideEnemyThresholdRatio = 0.5f;
    private TeamAISiegeDefender _teamAISiegeDefender;
    private List<MissionObject> castleKeyPositions;
    private CastleGate castleGate;
    private List<SiegeLane> _lanes;
    private BatteringRam _batteringRam;
    private float _startingPowerRatio;
    private float _meleeDefenderPower;
    private float _laneThreatCapacity;
    private float _initialLaneDefensePowerRatio = -1f;
    private List<StonePile> _stonePiles;
    private bool _isTacticFailing;
    private Formation _invadingEnemyFormation;
    private Formation _emergencyFormation;
    private List<Formation> _meleeFormations;
    private List<Formation> _laneDefendingFormations = new List<Formation>();
    private List<Formation> _rangedFormations;
    private int _laneCount;
    private int _AIControlledFormationCount;
    private bool _isSallyingOut;
    internal TacticDefendCastle.TacticState _tacticState;
    private List<Threat> debugThreats;
    private List<(TacticDefendCastle.ThreatCounter, Formation)> debugAssignments;

    protected IEnumerable<Formation> EnemyFormations => Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.Side == this.team.Side.GetOppositeSide())).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (tf => tf.FormationsIncludingSpecial));

    public TacticDefendCastle(Team team)
      : base(team)
    {
      Mission current = Mission.Current;
      this.castleKeyPositions = new List<MissionObject>();
      IEnumerable<CastleGate> allWithType1 = current.ActiveMissionObjects.FindAllWithType<CastleGate>();
      IEnumerable<WallSegment> allWithType2 = current.ActiveMissionObjects.FindAllWithType<WallSegment>();
      this.castleGate = allWithType1.FirstOrDefault<CastleGate>();
      this.castleKeyPositions.AddRange((IEnumerable<MissionObject>) allWithType1);
      this.castleKeyPositions.AddRange((IEnumerable<MissionObject>) allWithType2);
      this._batteringRam = current.ActiveMissionObjects.FindAllWithType<BatteringRam>().FirstOrDefault<BatteringRam>((Func<BatteringRam, bool>) (br => !br.IsDeactivated));
      this._teamAISiegeDefender = team.TeamAI as TeamAISiegeDefender;
      this._lanes = TeamAISiegeComponent.SiegeLanes;
      int num = 3;
      IEnumerable<Formation> formations1 = this.Formations;
      if (formations1.Count<Formation>() < num)
      {
        IEnumerable<Formation> formations2 = (IEnumerable<Formation>) formations1.OrderByDescending<Formation, int>((Func<Formation, int>) (f => f.CountOfUnits));
      }
      this._stonePiles = current.ActiveMissionObjects.FindAllWithType<StonePile>().ToList<StonePile>();
    }

    private static float GetFormationSallyOutPower(Formation formation)
    {
      float typeMultiplier = formation.IsCavalry() ? 2f : (formation.IsRanged() ? 0.3f : 1f);
      float sum = 0.0f;
      formation.ApplyActionOnEachUnit((Action<Agent>) (agent => sum += agent.CharPowerCached * typeMultiplier));
      return sum;
    }

    private Formation GetStrongestSallyOutFormation()
    {
      float num = 0.0f;
      Formation formation1 = (Formation) null;
      foreach (Formation formation2 in this._meleeFormations.Where<Formation>((Func<Formation, bool>) (f => TeamAISiegeComponent.IsFormationInsideCastle(f, true))))
      {
        float formationSallyOutPower = TacticDefendCastle.GetFormationSallyOutPower(formation2);
        if ((double) formationSallyOutPower > (double) num)
        {
          formation1 = formation2;
          num = formationSallyOutPower;
        }
      }
      return formation1;
    }

    private bool MustRetreatToCastle() => false;

    private bool IsSallyOutApplicable() => (double) this.Formations.Sum<Formation>((Func<Formation, float>) (formation => TacticDefendCastle.GetFormationSallyOutPower(formation))) > (double) Mission.Current.Teams.Where<Team>((Func<Team, bool>) (t => t.Side.GetOppositeSide() == BattleSideEnum.Defender)).SelectMany<Team, Formation>((Func<Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).Sum<Formation>((Func<Formation, float>) (formation => TacticDefendCastle.GetFormationSallyOutPower(formation))) * 3.0 && (double) this.team.QuerySystem.OverallPowerRatio / (double) this._startingPowerRatio > 3.0;

    private void BalanceLaneDefenders(List<Formation> defenderFormations)
    {
      int length = 3;
      SiegeLane[] siegeLaneArray = new SiegeLane[length];
      for (int i = 0; i < length; i++)
        siegeLaneArray[i] = this._lanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (l => l.LaneSide == (FormationAI.BehaviorSide) i));
      float[] numArray1 = new float[length];
      for (int index = 0; index < siegeLaneArray.Length; ++index)
      {
        SiegeLane siegeLane = siegeLaneArray[index];
        numArray1[index] = siegeLane == null || siegeLane.GetDefenseState() == SiegeLane.LaneDefenseStates.Token ? 0.0f : siegeLane.GetLaneCapacity();
      }
      float num1 = ((IEnumerable<float>) numArray1).Sum();
      float[] numArray2 = new float[length];
      for (int index = 0; index < length; ++index)
        numArray2[index] = numArray1[index] / num1;
      int num2 = ((IEnumerable<SiegeLane>) siegeLaneArray).Count<SiegeLane>((Func<SiegeLane, bool>) (l => l != null && l.GetDefenseState() == SiegeLane.LaneDefenseStates.Token));
      int num3 = 15;
      int num4 = num2 * 15;
      int num5 = defenderFormations.Sum<Formation>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes));
      int num6 = num5 - num4;
      IEnumerable<float> source = ((IEnumerable<float>) numArray2).Where<float>((Func<float, bool>) (ltp => (double) ltp > 0.0));
      if (source.Any<float>() && (double) num6 * (double) source.Min() <= (double) num3)
      {
        num6 = num5;
        num3 = Math.Max((int) ((double) num6 * 0.100000001490116), 1);
      }
      int[] numArray3 = new int[length];
      for (int index = 0; index < length; ++index)
      {
        int num7 = (int) ((double) numArray2[index] * (double) num6);
        numArray3[index] = num7 == 0 ? num3 : num7;
      }
      int[] numArray4 = new int[length];
      foreach (Formation defenderFormation in defenderFormations)
      {
        int side = (int) defenderFormation.AI.Side;
        numArray4[side] = defenderFormation.UnitsWithoutLooseDetachedOnes.Count<IFormationUnit>() - numArray3[side];
      }
      int num8 = 0;
      foreach (Formation defenderFormation in defenderFormations)
      {
        Formation receiverDefenderFormation = defenderFormation;
        int side1 = (int) receiverDefenderFormation.AI.Side;
        if (numArray4[side1] < -num8)
        {
          bool flag = false;
          foreach (Formation formation in defenderFormations.Where<Formation>((Func<Formation, bool>) (df => df != receiverDefenderFormation)))
          {
            int side2 = (int) formation.AI.Side;
            if (numArray4[side2] > num8)
            {
              int unitCount = Math.Min(-numArray4[side1], numArray4[side2]);
              numArray4[side1] += unitCount;
              numArray4[side2] -= unitCount;
              formation.TransferUnits(receiverDefenderFormation, unitCount);
              flag = true;
              if (numArray4[side1] == 0)
                break;
            }
          }
          if (!flag)
            break;
        }
      }
    }

    private void ArcherShiftAround(List<Formation> p_RangedFormations)
    {
      List<Formation> list = p_RangedFormations.Where<Formation>((Func<Formation, bool>) (rf => rf.AI.ActiveBehavior is BehaviorShootFromCastleWalls)).ToList<Formation>();
      if (list.Count<Formation>() < 2)
        return;
      float smallerFormationUnitPercentage = 0.1f;
      float mediumFormationUnitPercentage = 0.2f;
      float largerFormationUnitPercentage = 0.4f;
      float num1 = list.Sum<Formation>((Func<Formation, float>) (f =>
      {
        if ((f.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("many"))
          return largerFormationUnitPercentage;
        return !(f.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("few") ? mediumFormationUnitPercentage : smallerFormationUnitPercentage;
      }));
      smallerFormationUnitPercentage /= num1;
      mediumFormationUnitPercentage /= num1;
      largerFormationUnitPercentage /= num1;
      int num2 = list.Sum<Formation>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes));
      int smallFormationCount = Math.Max((int) ((double) num2 * (double) smallerFormationUnitPercentage), 1);
      int mediumFormationCount = Math.Max((int) ((double) num2 * (double) mediumFormationUnitPercentage), 1);
      int largeFormationCount = Math.Max((int) ((double) num2 * (double) largerFormationUnitPercentage), 1);
      int num3 = Math.Max((int) ((double) num2 * 0.0500000007450581), 1);
      using (List<Formation>.Enumerator enumerator = list.GetEnumerator())
      {
label_7:
        while (enumerator.MoveNext())
        {
          Formation current = enumerator.Current;
          int num4 = (current.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("many") ? largeFormationCount : ((current.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("few") ? smallFormationCount : mediumFormationCount);
          int num5 = 0;
          while (true)
          {
            if (num4 - current.CountOfUnitsWithoutDetachedOnes > num3 && (list.Any<Formation>((Func<Formation, bool>) (rf => rf.CountOfUnitsWithoutDetachedOnes > ((rf.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("many") ? largeFormationCount : ((rf.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("few") ? smallFormationCount : mediumFormationCount)))) && num5 < list.Count))
            {
              int val1 = num4 - current.CountOfUnitsWithoutDetachedOnes;
              Formation formation = list.MaxBy<Formation, int>((Func<Formation, int>) (rf => rf.CountOfUnitsWithoutDetachedOnes - ((rf.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("many") ? largeFormationCount : ((rf.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("few") ? smallFormationCount : mediumFormationCount))));
              int unitCount = Math.Min(val1, formation.CountOfUnitsWithoutDetachedOnes - ((formation.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("many") ? largeFormationCount : ((formation.AI.ActiveBehavior as BehaviorShootFromCastleWalls).ArcherPosition.HasTag("few") ? smallFormationCount : mediumFormationCount)));
              formation.TransferUnits(current, unitCount);
              ++num5;
            }
            else
              goto label_7;
          }
        }
      }
    }

    protected override bool CheckAndSetAvailableFormationsChanged()
    {
      bool flag1 = false;
      if (this._laneCount != this._lanes.Count)
      {
        this._laneCount = this._lanes.Count;
        flag1 = true;
      }
      int num = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
      bool flag2 = num != this._AIControlledFormationCount;
      if (flag2)
        this._AIControlledFormationCount = num;
      return flag1 | flag2;
    }

    private int GetRequiredMeleeDefenderCount() => this._lanes.Count<SiegeLane>((Func<SiegeLane, bool>) (l => l.IsOpen || l.PrimarySiegeWeapons.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (lsw => lsw is SiegeLadder)) || l.PrimarySiegeWeapons.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw.HasCompletedAction() || (psw as SiegeWeapon).IsUsed))));

    protected override void ManageFormationCounts()
    {
      if ((double) this._startingPowerRatio == 0.0)
        this._startingPowerRatio = this.team.QuerySystem.OverallPowerRatio;
      switch (this._tacticState)
      {
        case TacticDefendCastle.TacticState.ProperDefense:
          int meleeDefenderCount1 = this.GetRequiredMeleeDefenderCount();
          int rangedCount1 = Math.Min(this._teamAISiegeDefender.ArcherPositions.Count<ArcherPosition>(), 8 - meleeDefenderCount1);
          this.ManageFormationCounts(meleeDefenderCount1, rangedCount1, 0, 0);
          this._meleeFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)).ToList<Formation>();
          this._rangedFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)).ToList<Formation>();
          break;
        case TacticDefendCastle.TacticState.DesparateDefense:
          int num1 = this.Formations.Count<Formation>();
          int count = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
          int num2 = num1 - count;
          int meleeDefenderCount2 = this.GetRequiredMeleeDefenderCount();
          if (count > 0 && num1 != meleeDefenderCount2 && num2 <= meleeDefenderCount2)
          {
            List<Formation> list = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.IsAIControlled)).ToList<Formation>();
            Formation biggestFormation = list.MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnitsWithoutDetachedOnes));
            foreach (Formation formation in list.Where<Formation>((Func<Formation, bool>) (f => f != biggestFormation)))
              formation.TransferUnits(biggestFormation, formation.CountOfUnits);
            if (count > 1)
            {
              biggestFormation.Split(count);
              break;
            }
            break;
          }
          break;
        case TacticDefendCastle.TacticState.SallyOut:
          int infantryCount = 1;
          int rangedCount2 = Math.Min(this._teamAISiegeDefender.ArcherPositions.Count<ArcherPosition>(), 8 - infantryCount);
          this.ManageFormationCounts(infantryCount, rangedCount2, 0, 0);
          this._meleeFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)).ToList<Formation>();
          this._rangedFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)).ToList<Formation>();
          break;
      }
      if ((double) this._initialLaneDefensePowerRatio != -1.0)
        return;
      this._meleeDefenderPower = this.Formations.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationMeleeFightingPower));
      this._laneThreatCapacity = this._lanes.Sum<SiegeLane>((Func<SiegeLane, float>) (l => l.GetLaneCapacity()));
      float val2 = this.team.QuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (et => et.Team.FormationsIncludingSpecial.Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationPower))));
      int enemyUnitCount = this.team.QuerySystem.EnemyUnitCount;
      float num3 = enemyUnitCount == 0 ? 0.0f : val2 / (float) enemyUnitCount;
      this._laneThreatCapacity = Math.Min(this._lanes.Where<SiegeLane>((Func<SiegeLane, bool>) (l => l.IsOpen || l.PrimarySiegeWeapons.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => !(psw as SiegeWeapon).IsDeactivated)))).Sum<SiegeLane>((Func<SiegeLane, float>) (l => l.GetLaneCapacity())) * num3, val2);
      this._initialLaneDefensePowerRatio = this._meleeDefenderPower / this._laneThreatCapacity;
    }

    protected override void StopUsingAllMachines()
    {
      base.StopUsingAllMachines();
      foreach (IDetachment detachment in this.team.DetachmentManager.Detachments.Where<IDetachment>((Func<IDetachment, bool>) (d => d is StrategicArea)).ToList<IDetachment>())
        this.team.DetachmentManager.DestroyDetachment(detachment);
    }

    private void StartRetreatToKeep()
    {
      this.StopUsingAllMachines();
      foreach (Formation formation in this.Formations)
      {
        formation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(formation);
        formation.AI.SetBehaviorWeight<BehaviorRetreatToKeep>(1f);
      }
    }

    private void DistributeRangedFormations()
    {
      List<Tuple<Formation, ArcherPosition>> source = this._rangedFormations.CombineWith<Formation, ArcherPosition>((IEnumerable<ArcherPosition>) this._teamAISiegeDefender.ArcherPositions);
      while (source.Any<Tuple<Formation, ArcherPosition>>())
      {
        Tuple<Formation, ArcherPosition> tuple = source.MinBy<Tuple<Formation, ArcherPosition>, float>((Func<Tuple<Formation, ArcherPosition>, float>) (c => c.Item1.QuerySystem.MedianPosition.AsVec2.DistanceSquared(c.Item2.Entity.GlobalPosition.AsVec2)));
        Formation bestFormation = tuple.Item1;
        ArcherPosition bestArcherPosition = tuple.Item2;
        bestFormation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(bestFormation);
        bestFormation.AI.SetBehaviorWeight<BehaviorShootFromCastleWalls>(1f);
        bestFormation.AI.GetBehavior<BehaviorShootFromCastleWalls>().ArcherPosition = bestArcherPosition.Entity;
        source.RemoveAll((Predicate<Tuple<Formation, ArcherPosition>>) (c => c.Item1 == bestFormation || c.Item2 == bestArcherPosition));
      }
    }

    private void ManageGatesForSallyingOut()
    {
      if (this._teamAISiegeDefender.InnerGate.IsGateOpen && this._teamAISiegeDefender.OuterGate.IsGateOpen || !this._meleeFormations.Any<Formation>((Func<Formation, bool>) (mf => TeamAISiegeComponent.IsFormationInsideCastle(mf, true))))
        return;
      CastleGate gateToOpen = !this._teamAISiegeDefender.InnerGate.IsGateOpen ? this._teamAISiegeDefender.InnerGate : this._teamAISiegeDefender.OuterGate;
      if (!this.Formations.All<Formation>((Func<Formation, bool>) (f => !f.IsUsingMachine((UsableMachine) gateToOpen))))
        return;
      this.GetStrongestSallyOutFormation().StartUsingMachine((UsableMachine) gateToOpen);
    }

    private void StartSallyOut()
    {
      this.DistributeRangedFormations();
      foreach (Formation meleeFormation in this._meleeFormations)
      {
        meleeFormation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(meleeFormation);
        meleeFormation.AI.SetBehaviorWeight<BehaviorSallyOut>(1000f);
      }
    }

    private void CarryOutDefense(
      IEnumerable<SiegeLane> defendedLanes,
      IEnumerable<SiegeLane> lanesToBeRetaken,
      bool isEnemyInside,
      bool doRangedJoinMelee)
    {
      List<Formation> source1 = new List<Formation>();
      List<Formation> formationList1 = new List<Formation>();
      int neededCount = defendedLanes.Count<SiegeLane>() + Math.Max(lanesToBeRetaken.Count<SiegeLane>(), isEnemyInside ? 1 : 0);
      List<Formation> formationList2 = this._meleeFormations.Where<Formation>((Func<Formation, bool>) (mf => mf.CountOfUnitsWithoutDetachedOnes > 0)).ToList<Formation>();
      if (!formationList2.Any<Formation>())
        formationList2 = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsMeleeFormation)).ToList<Formation>();
      int num1 = formationList2.Count<Formation>();
      List<ArcherPosition> list1 = this._teamAISiegeDefender.ArcherPositions.Where<ArcherPosition>((Func<ArcherPosition, bool>) (ap => this._lanes.Where<SiegeLane>((Func<SiegeLane, bool>) (l => ap.IsArcherPositionRelatedToSide(l.LaneSide))).Any<SiegeLane>((Func<SiegeLane, bool>) (l => l.LaneState > SiegeLane.LaneStateEnum.Used && l.LaneState < SiegeLane.LaneStateEnum.Conceited)))).ToList<ArcherPosition>();
      List<Formation> rangedFormations = this._rangedFormations.Where<Formation>((Func<Formation, bool>) (rf => rf.CountOfUnitsWithoutDetachedOnes > 0)).Except<Formation>((IEnumerable<Formation>) formationList2).ToList<Formation>();
      if (!rangedFormations.Any<Formation>())
        rangedFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)).Except<Formation>((IEnumerable<Formation>) formationList2).ToList<Formation>();
      int num2 = rangedFormations.Count<Formation>();
      foreach (Formation formation in this.Formations.Except<Formation>((IEnumerable<Formation>) formationList2).Except<Formation>((IEnumerable<Formation>) rangedFormations).Where<Formation>((Func<Formation, bool>) (f => f.CountOfUnitsWithoutDetachedOnes > 0)))
      {
        if (formation.QuerySystem.IsRangedFormation)
        {
          rangedFormations.Add(formation);
          ++num2;
        }
        else
        {
          formationList2.Add(formation);
          ++num1;
        }
      }
      List<ArcherPosition> source2 = !doRangedJoinMelee ? list1.Where<ArcherPosition>((Func<ArcherPosition, bool>) (ap =>
      {
        if (this._lanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (l => l.LaneSide == ap.GetArcherPositionClosestSide())) != null)
          return this._lanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (l => l.LaneSide == ap.GetArcherPositionClosestSide())).LaneState >= SiegeLane.LaneStateEnum.Conceited;
        if (this._lanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (l => ap.IsArcherPositionRelatedToSide(l.LaneSide))) != null)
          return this._lanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (l => ap.IsArcherPositionRelatedToSide(l.LaneSide))).LaneState >= SiegeLane.LaneStateEnum.Conceited;
        return this._lanes.Any<SiegeLane>() && this._lanes.MinBy<SiegeLane, int>((Func<SiegeLane, int>) (l => SiegeQuerySystem.SideDistance(ap.ConnectedSides, 1 << (int) (l.LaneSide & (FormationAI.BehaviorSide) 31)))).LaneState >= SiegeLane.LaneStateEnum.Conceited;
      })).ToList<ArcherPosition>() : list1.Where<ArcherPosition>((Func<ArcherPosition, bool>) (ap => this._lanes.Where<SiegeLane>((Func<SiegeLane, bool>) (l => l.LaneState > SiegeLane.LaneStateEnum.Unused && l.IsUnderAttack())).Any<SiegeLane>((Func<SiegeLane, bool>) (l => ap.IsArcherPositionRelatedToSide(l.LaneSide))))).ToList<ArcherPosition>();
      IEnumerable<Formation> formations1 = source2.Select<ArcherPosition, Formation>((Func<ArcherPosition, Formation>) (aptba => aptba.GetLastAssignedFormation(this.team.TeamIndex))).Where<Formation>((Func<Formation, bool>) (laf => laf != null && rangedFormations.Contains(laf)));
      int num3 = formations1.Count<Formation>();
      if (num1 > neededCount)
      {
        IEnumerable<Formation> formations2 = defendedLanes.Concat<SiegeLane>(lanesToBeRetaken).Where<SiegeLane>((Func<SiegeLane, bool>) (l => l.GetLastAssignedFormation(this.team.TeamIndex) != null)).Select<SiegeLane, Formation>((Func<SiegeLane, Formation>) (dl => dl.GetLastAssignedFormation(this.team.TeamIndex)));
        if (formations2.Any<Formation>())
        {
          List<Formation> formationList3 = new List<Formation>();
          foreach (Formation formation in formationList2.Except<Formation>(formations2).Where<Formation>((Func<Formation, bool>) (mf => mf.IsAIControlled)))
          {
            Formation excessFormation = formation;
            Formation target1 = formations2.FirstOrDefault<Formation>((Func<Formation, bool>) (aff => aff.AI.Side == excessFormation.AI.Side));
            if (target1 != null)
            {
              excessFormation.TransferUnits(target1, excessFormation.CountOfUnits);
            }
            else
            {
              Formation target2 = formations2.MinBy<Formation, int>((Func<Formation, int>) (aff => aff.CountOfUnits));
              excessFormation.TransferUnits(target2, excessFormation.CountOfUnits);
            }
            formationList3.Add(excessFormation);
          }
          formationList2 = formationList2.Except<Formation>((IEnumerable<Formation>) formationList3).ToList<Formation>();
        }
        else
          formationList2 = this.ConsolidateFormations(formationList2, neededCount);
        num1 = formationList2.Count<Formation>();
      }
      List<Formation> list2 = formationList2.Concat<Formation>(formations1).ToList<Formation>();
      if (!list2.Any<Formation>())
      {
        list2 = this.Formations.ToList<Formation>();
        rangedFormations.Clear();
      }
      if (num1 + num3 < neededCount)
      {
        List<Formation> list3 = list2.Where<Formation>((Func<Formation, bool>) (mf => mf.IsSplittableByAI)).ToList<Formation>();
        if (list3.Any<Formation>())
        {
          int num4 = 0;
          while (num1 + num3 + num4 < neededCount)
          {
            Formation largestFormation = list3.MaxBy<Formation, int>((Func<Formation, int>) (rf => rf.UnitsWithoutLooseDetachedOnes.Count<IFormationUnit>()));
            IEnumerable<Formation> source3 = largestFormation.Split();
            if (source3.Count<Formation>() >= 2)
            {
              ++num4;
              Formation formation = source3.FirstOrDefault<Formation>((Func<Formation, bool>) (rf => rf != largestFormation));
              list3.Add(formation);
              list2.Add(formation);
            }
            else
              break;
          }
        }
      }
      List<SiegeLane> siegeLaneList1 = new List<SiegeLane>();
      List<Formation> formationList4 = new List<Formation>();
      foreach (SiegeLane defendedLane in defendedLanes)
      {
        SiegeLane toBeDefendedLane = defendedLane;
        Formation formation = list2.FirstOrDefault<Formation>((Func<Formation, bool>) (affml => affml == toBeDefendedLane.GetLastAssignedFormation(this.team.TeamIndex)));
        if (formation != null)
        {
          formation.AI.Side = toBeDefendedLane.LaneSide;
          formation.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(formation);
          formation.AI.SetBehaviorWeight<BehaviorDefendCastleKeyPosition>(1f);
          toBeDefendedLane.SetLastAssignedFormation(this.team.TeamIndex, formation);
          list2.Remove(formation);
          siegeLaneList1.Add(toBeDefendedLane);
          formationList4.Add(formation);
          source1.Add(formation);
        }
      }
      List<SiegeLane> list4 = defendedLanes.Except<SiegeLane>((IEnumerable<SiegeLane>) siegeLaneList1).ToList<SiegeLane>();
      List<SiegeLane> siegeLaneList2 = new List<SiegeLane>();
      foreach (SiegeLane siegeLane in lanesToBeRetaken)
      {
        SiegeLane toBeRetakenLane = siegeLane;
        Formation formation = list2.FirstOrDefault<Formation>((Func<Formation, bool>) (affml => affml == toBeRetakenLane.GetLastAssignedFormation(this.team.TeamIndex)));
        if (formation != null)
        {
          formation.AI.Side = toBeRetakenLane.LaneSide;
          formation.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(formation);
          formation.AI.SetBehaviorWeight<BehaviorRetakeCastleKeyPosition>(1f);
          toBeRetakenLane.SetLastAssignedFormation(this.team.TeamIndex, formation);
          list2.Remove(formation);
          siegeLaneList2.Add(toBeRetakenLane);
          formationList4.Add(formation);
          source1.Add(formation);
          siegeLaneList1.Add(toBeRetakenLane);
        }
      }
      bool flag1 = false;
      while (list4.Any<SiegeLane>())
      {
        SiegeLane firstToDefend = list4.MaxBy<SiegeLane, float>((Func<SiegeLane, float>) (tbdl => tbdl.GetLaneCapacity()));
        Formation formation1 = list2.FirstOrDefault<Formation>((Func<Formation, bool>) (affml => affml.AI.Side == firstToDefend.LaneSide));
        if (formation1 != null)
        {
          formation1.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(formation1);
          formation1.AI.SetBehaviorWeight<BehaviorDefendCastleKeyPosition>(1f);
          firstToDefend.SetLastAssignedFormation(this.team.TeamIndex, formation1);
          list2.Remove(formation1);
          formationList4.Add(formation1);
          source1.Add(formation1);
          siegeLaneList1.Add(firstToDefend);
        }
        else if (list2.Any<Formation>())
        {
          Formation formation2 = list2.MaxBy<Formation, float>((Func<Formation, float>) (f => f.QuerySystem.FormationPower));
          formation2.AI.Side = firstToDefend.LaneSide;
          formation2.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(formation2);
          formation2.AI.SetBehaviorWeight<BehaviorDefendCastleKeyPosition>(1f);
          firstToDefend.SetLastAssignedFormation(this.team.TeamIndex, formation2);
          list2.Remove(formation2);
          formationList4.Add(formation2);
          source1.Add(formation2);
          siegeLaneList1.Add(firstToDefend);
        }
        else
        {
          flag1 = true;
          list4.Clear();
          break;
        }
        list4.Remove(firstToDefend);
      }
      List<SiegeLane> source4 = flag1 ? new List<SiegeLane>() : lanesToBeRetaken.Except<SiegeLane>((IEnumerable<SiegeLane>) siegeLaneList2).ToList<SiegeLane>();
      while (source4.Any<SiegeLane>() && list2.Any<Formation>())
      {
        SiegeLane firstToRetake = lanesToBeRetaken.MaxBy<SiegeLane, float>((Func<SiegeLane, float>) (ltbr => ltbr.GetLaneCapacity()));
        Formation formation1 = list2.FirstOrDefault<Formation>((Func<Formation, bool>) (affml => affml.AI.Side == firstToRetake.LaneSide));
        if (formation1 != null)
        {
          formation1.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(formation1);
          formation1.AI.SetBehaviorWeight<BehaviorRetakeCastleKeyPosition>(1f);
          firstToRetake.SetLastAssignedFormation(this.team.TeamIndex, formation1);
          list2.Remove(formation1);
          formationList4.Add(formation1);
          source1.Add(formation1);
          siegeLaneList1.Add(firstToRetake);
        }
        else if (list2.Any<Formation>())
        {
          Formation formation2 = list2.MaxBy<Formation, float>((Func<Formation, float>) (f => f.QuerySystem.FormationPower));
          formation2.AI.Side = firstToRetake.LaneSide;
          formation2.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(formation2);
          formation2.AI.SetBehaviorWeight<BehaviorRetakeCastleKeyPosition>(1f);
          firstToRetake.SetLastAssignedFormation(this.team.TeamIndex, formation2);
          list2.Remove(formation2);
          formationList4.Add(formation2);
          source1.Add(formation2);
          siegeLaneList1.Add(firstToRetake);
        }
        else
          break;
        source4.Remove(firstToRetake);
      }
      Formation formation3 = (Formation) null;
      if (isEnemyInside && list2.Any<Formation>())
      {
        Formation f = this._emergencyFormation == null || !list2.Contains(this._emergencyFormation) ? list2.MaxBy<Formation, float>((Func<Formation, float>) (affml => affml.QuerySystem.FormationPower)) : this._emergencyFormation;
        f.AI.Side = FormationAI.BehaviorSide.BehaviorSideNotSet;
        f.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(f);
        f.AI.SetBehaviorWeight<BehaviorEliminateEnemyInsideCastle>(1f);
        list2.Remove(f);
        formationList4.Add(f);
        source1.Add(f);
        formation3 = f;
      }
      IEnumerable<Formation> second = formations1.Except<Formation>((IEnumerable<Formation>) formationList4);
      foreach (Formation formation1 in second)
      {
        Formation rangedFormation = formation1;
        ArcherPosition foundPreviousArcherPosition = source2.FirstOrDefault<ArcherPosition>((Func<ArcherPosition, bool>) (aptba => aptba.GetLastAssignedFormation(this.team.TeamIndex) == rangedFormation));
        List<SiegeLane> list3 = defendedLanes.Union<SiegeLane>(lanesToBeRetaken).Where<SiegeLane>(closure_0 ?? (closure_0 = (Func<SiegeLane, bool>) (arl => arl.GetLastAssignedFormation(this.team.TeamIndex) != null))).ToList<SiegeLane>();
        bool flag2 = list3.Any<SiegeLane>();
        SiegeLane siegeLane = (SiegeLane) null;
        if (foundPreviousArcherPosition == null)
        {
          if (flag2)
            siegeLane = list3.MaxBy<SiegeLane, SiegeLane.LaneStateEnum>((Func<SiegeLane, SiegeLane.LaneStateEnum>) (arl => arl.LaneState));
        }
        else if (flag2)
        {
          List<SiegeLane> list5 = list3.Where<SiegeLane>((Func<SiegeLane, bool>) (l => foundPreviousArcherPosition.IsArcherPositionRelatedToSide(l.LaneSide))).ToList<SiegeLane>();
          siegeLane = !list5.Any<SiegeLane>() ? list3.MaxBy<SiegeLane, SiegeLane.LaneStateEnum>((Func<SiegeLane, SiegeLane.LaneStateEnum>) (arl => arl.LaneState)) : list5.MaxBy<SiegeLane, SiegeLane.LaneStateEnum>((Func<SiegeLane, SiegeLane.LaneStateEnum>) (rl => rl.LaneState));
        }
        Formation target = siegeLane != null ? siegeLane.GetLastAssignedFormation(this.team.TeamIndex) : formation3;
        rangedFormation.TransferUnits(target, rangedFormation.CountOfUnits);
      }
      List<ArcherPosition> list6 = list1.Except<ArcherPosition>((IEnumerable<ArcherPosition>) source2).ToList<ArcherPosition>();
      List<Formation> formationList5 = rangedFormations.Except<Formation>((IEnumerable<Formation>) formationList4).Except<Formation>(second).ToList<Formation>();
      List<ArcherPosition> archerPositionList = new List<ArcherPosition>();
      if (formationList5.Count > list6.Count)
      {
        if (list6.Count > 0)
          formationList5 = this.ConsolidateFormations(formationList5, list6.Count);
      }
      else if (formationList5.Count < list6.Count && formationList5.Count > 0)
      {
        int num4 = list6.Count - formationList5.Count;
        Formation nextBiggest = formationList5.MaxBy<Formation, int>((Func<Formation, int>) (rrf => rrf.CountOfUnits));
        List<Formation> list3 = nextBiggest.Split(num4 + 1).Where<Formation>((Func<Formation, bool>) (nb => nb != nextBiggest)).ToList<Formation>();
        formationList5.AddRange((IEnumerable<Formation>) list3);
      }
      foreach (ArcherPosition archerPosition in list6.Where<ArcherPosition>((Func<ArcherPosition, bool>) (rap => rap.GetLastAssignedFormation(this.team.TeamIndex) != null)))
      {
        ArcherPosition remainingArcherPosition = archerPosition;
        if (remainingArcherPosition.GetLastAssignedFormation(this.team.TeamIndex) != null && formationList5.Contains(remainingArcherPosition.GetLastAssignedFormation(this.team.TeamIndex)))
        {
          Formation assignedFormation = remainingArcherPosition.GetLastAssignedFormation(this.team.TeamIndex);
          assignedFormation.AI.Side = remainingArcherPosition.GetArcherPositionClosestSide();
          assignedFormation.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(assignedFormation);
          assignedFormation.AI.SetBehaviorWeight<BehaviorShootFromCastleWalls>(1f).ArcherPosition = remainingArcherPosition.Entity;
          remainingArcherPosition.SetLastAssignedFormation(this.team.TeamIndex, assignedFormation);
          formationList5.Remove(assignedFormation);
          formationList4.Add(remainingArcherPosition.GetLastAssignedFormation(this.team.TeamIndex));
          formationList1.Add(remainingArcherPosition.GetLastAssignedFormation(this.team.TeamIndex));
          archerPositionList.Add(remainingArcherPosition);
        }
        else
        {
          Formation formation1 = formationList5.FirstOrDefault<Formation>((Func<Formation, bool>) (rrf => remainingArcherPosition.IsArcherPositionRelatedToSide(rrf.AI.Side))) ?? formationList5.FirstOrDefault<Formation>();
          if (formation1 != null)
          {
            formation1.AI.Side = remainingArcherPosition.GetArcherPositionClosestSide();
            formation1.AI.ResetBehaviorWeights();
            TacticComponent.SetDefaultBehaviorWeights(formation1);
            formation1.AI.SetBehaviorWeight<BehaviorShootFromCastleWalls>(1f).ArcherPosition = remainingArcherPosition.Entity;
            remainingArcherPosition.SetLastAssignedFormation(this.team.TeamIndex, formation1);
            formationList5.Remove(formation1);
            formationList4.Add(formation1);
            formationList1.Add(formation1);
            archerPositionList.Add(remainingArcherPosition);
          }
          else
            break;
        }
      }
      foreach (ArcherPosition archerPosition in list6.Except<ArcherPosition>((IEnumerable<ArcherPosition>) archerPositionList).ToList<ArcherPosition>())
      {
        ArcherPosition remainingArcherPosition = archerPosition;
        Formation formation1 = formationList5.FirstOrDefault<Formation>((Func<Formation, bool>) (rrf => remainingArcherPosition.IsArcherPositionRelatedToSide(rrf.AI.Side))) ?? formationList5.FirstOrDefault<Formation>();
        if (formation1 != null)
        {
          formation1.AI.Side = remainingArcherPosition.GetArcherPositionClosestSide();
          formation1.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(formation1);
          formation1.AI.SetBehaviorWeight<BehaviorShootFromCastleWalls>(1f).ArcherPosition = remainingArcherPosition.Entity;
          remainingArcherPosition.SetLastAssignedFormation(this.team.TeamIndex, formation1);
          formationList5.Remove(formation1);
          formationList4.Add(formation1);
          formationList1.Add(formation1);
          archerPositionList.Add(remainingArcherPosition);
        }
        else
          break;
      }
      this._meleeFormations = source1;
      this._laneDefendingFormations = source1.Where<Formation>((Func<Formation, bool>) (amf => amf.AI.Side < FormationAI.BehaviorSide.BehaviorSideNotSet)).ToList<Formation>();
      this._rangedFormations = formationList1;
      foreach (SiegeLane siegeLane in this._lanes.Except<SiegeLane>((IEnumerable<SiegeLane>) siegeLaneList1))
        siegeLane.SetLastAssignedFormation(this.team.TeamIndex, (Formation) null);
      this._emergencyFormation = formation3;
      foreach (ArcherPosition archerPosition in this._teamAISiegeDefender.ArcherPositions.Except<ArcherPosition>((IEnumerable<ArcherPosition>) archerPositionList))
        archerPosition.SetLastAssignedFormation(this.team.TeamIndex, (Formation) null);
    }

    private void CheckAndChangeState()
    {
      this.Formations.Where<Formation>((Func<Formation, bool>) (f => TeamAISiegeComponent.IsFormationInsideCastle(f, true)));
      if (this.MustRetreatToCastle())
      {
        if (this._tacticState == TacticDefendCastle.TacticState.RetreatToKeep)
          return;
        this._tacticState = TacticDefendCastle.TacticState.RetreatToKeep;
        this.ManageFormationCounts();
        this.StartRetreatToKeep();
      }
      else if (this.IsSallyOutApplicable())
      {
        if (this._tacticState == TacticDefendCastle.TacticState.SallyOut)
        {
          if (this._isSallyingOut)
            return;
          this.ManageGatesForSallyingOut();
          if (!this._teamAISiegeDefender.InnerGate.IsGateOpen || !this._teamAISiegeDefender.OuterGate.IsGateOpen)
            return;
          this.StartSallyOut();
          this._isSallyingOut = true;
        }
        else
        {
          this._tacticState = TacticDefendCastle.TacticState.SallyOut;
          this.ManageFormationCounts();
        }
      }
      else
      {
        bool flag = false;
        if (this._invadingEnemyFormation != null)
        {
          flag = TeamAISiegeComponent.IsFormationInsideCastle(this._invadingEnemyFormation, true);
          if (!flag)
            this._invadingEnemyFormation = (Formation) null;
        }
        if (!flag)
        {
          flag = TeamAISiegeComponent.QuerySystem.InsideAttackerCount > 30;
          if (flag)
          {
            IEnumerable<Formation> source = this.team.QuerySystem.EnemyTeams.SelectMany<TeamQuerySystem, Formation>((Func<TeamQuerySystem, IEnumerable<Formation>>) (et => et.Team.Formations)).Where<Formation>((Func<Formation, bool>) (f => TeamAISiegeComponent.IsFormationInsideCastle(f, true)));
            if (source.Any<Formation>())
              this._invadingEnemyFormation = source.FirstOrDefault<Formation>();
            else
              flag = false;
          }
        }
        List<SiegeLane> list = this._lanes.Where<SiegeLane>((Func<SiegeLane, bool>) (l => l.LaneState == SiegeLane.LaneStateEnum.Conceited)).ToList<SiegeLane>();
        List<SiegeLane> activeLanes = this._lanes.Except<SiegeLane>((IEnumerable<SiegeLane>) list).Where<SiegeLane>((Func<SiegeLane, bool>) (l => (uint) l.GetDefenseState() > 0U)).ToList<SiegeLane>();
        if (flag)
          list.Clear();
        int num1 = list.Any<SiegeLane>() ? 1 : 0;
        if (num1 == 0 && !flag && activeLanes.Count == 0)
          activeLanes = this._lanes.Where<SiegeLane>((Func<SiegeLane, bool>) (l => l.HasGate)).ToList<SiegeLane>();
        if (num1 != 0 && activeLanes.Any<SiegeLane>())
        {
          SiegeLane siegeLane = list.MinBy<SiegeLane, int>((Func<SiegeLane, int>) (cl => activeLanes.Min<SiegeLane>((Func<SiegeLane, int>) (al => SiegeQuerySystem.SideDistance(1 << (int) (al.LaneSide & (FormationAI.BehaviorSide) 31), 1 << (int) (cl.LaneSide & (FormationAI.BehaviorSide) 31))))));
          list.Clear();
          list.Add(siegeLane);
        }
        int num2 = num1 | (flag ? 1 : 0);
        this._meleeFormations = this._meleeFormations.Where<Formation>((Func<Formation, bool>) (mf => mf.CountOfUnits > 0)).ToList<Formation>();
        this._rangedFormations = this._rangedFormations.Where<Formation>((Func<Formation, bool>) (rf => rf.CountOfUnits > 0)).ToList<Formation>();
        float num3 = Math.Max((float) this._meleeFormations.Sum<Formation>((Func<Formation, int>) (mf => mf.CountOfUnits)), 1f);
        float num4 = Math.Max((float) this._rangedFormations.Sum<Formation>((Func<Formation, int>) (rf => rf.CountOfUnits)), 1f);
        float num5 = num3 + num4;
        this.Formations.Sum<Formation>((Func<Formation, int>) (f => f.CountOfUnitsWithoutLooseDetachedOnes));
        bool doRangedJoinMelee = (double) num3 < (double) num5 * 0.330000013113022;
        int num6 = 0;
        if (num2 != 0)
        {
          float num7 = num3 - this._lanes.Sum<SiegeLane>((Func<SiegeLane, float>) (l => l.GetLaneCapacity()));
          num6 = !flag ? Math.Min((int) num7 / 15, list.Count<SiegeLane>()) : ((double) num7 >= 15.0 ? 1 : 0);
        }
        if (activeLanes.Count<SiegeLane>() + list.Count<SiegeLane>() + num6 <= 0)
        {
          this._isTacticFailing = true;
          num6 = 1;
        }
        this.CarryOutDefense((IEnumerable<SiegeLane>) activeLanes, (IEnumerable<SiegeLane>) list, flag && num6 > 0, doRangedJoinMelee);
        this.BalanceLaneDefenders(this._laneDefendingFormations);
        this.ArcherShiftAround(this._rangedFormations);
      }
    }

    protected internal override void TickOccasionally()
    {
      if (!this.AreFormationsCreated)
        return;
      if ((double) this.team.QuerySystem.EnemyTeams.FirstOrDefault<TeamQuerySystem>().InsideWallsRatio > 0.5)
        this.StopUsingAllMachines();
      this.CheckAndChangeState();
      this.CheckAndSetAvailableFormationsChanged();
      base.TickOccasionally();
    }

    private bool IsApplicable_ => !this.castleKeyPositions.IsEmpty<MissionObject>();

    [Conditional("DEBUG")]
    private void DebugSetThreats(IEnumerable<Threat> threats) => this.debugThreats = threats.ToList<Threat>();

    [Conditional("DEBUG")]
    private void DebugResetAssignments() => this.debugAssignments = new List<(TacticDefendCastle.ThreatCounter, Formation)>();

    [Conditional("DEBUG")]
    private void DebugAddAssignment(
      (TacticDefendCastle.ThreatCounter, Formation) assignment)
    {
      this.debugAssignments.Add(assignment);
    }

    internal override float GetTacticWeight() => this._isTacticFailing ? 5f : 10f;

    protected class ThreatCounter
    {
      public Threat Threat;
      public float CounterValue;
      public ScriptComponentBehaviour Constraint;
      public Func<OrderComponent> GetBehaviour;
      public Func<Formation, float> GetFormationEffectiveness;
    }

    internal enum TacticState
    {
      ProperDefense,
      DesparateDefense,
      RetreatToKeep,
      SallyOut,
    }
  }
}
