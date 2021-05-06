// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticDefensiveLine
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TacticDefensiveLine : TacticComponent
  {
    private bool _hasBattleBeenJoined;
    private int _AIControlledFormationCount;
    private const float DefendersAdvantage = 1.2f;
    private TacticalPosition _mainDefensiveLineObject;
    private TacticalPosition _linkedRangedDefensivePosition;

    protected override void ManageFormationCounts() => this.AssignTacticFormations1121();

    private void Defend()
    {
      if (this.team.IsPlayerTeam && !this.team.IsPlayerGeneral && this.team.IsPlayerSergeant)
        this.SoundTacticalHorn(TacticComponent.MoveHornSoundIndex);
      if (this._mainInfantry != null)
      {
        this._mainInfantry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefend>(1f).TacticalDefendPosition = this._mainDefensiveLineObject;
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
      }
      if (this._archers != null)
      {
        this._archers.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._archers);
        this._archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(1f);
        this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
        if (this._linkedRangedDefensivePosition != null)
          this._archers.AI.SetBehaviorWeight<BehaviorDefend>(10f).TacticalDefendPosition = this._linkedRangedDefensivePosition;
      }
      if (this._leftCavalry != null)
      {
        this._leftCavalry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._leftCavalry);
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Left;
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
      }
      if (this._rightCavalry != null)
      {
        this._rightCavalry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Right;
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
      }
      if (this._rangedCavalry == null)
        return;
      this._rangedCavalry.AI.ResetBehaviorWeights();
      TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
    }

    private void Engage()
    {
      if (this.team.IsPlayerTeam && !this.team.IsPlayerGeneral && this.team.IsPlayerSergeant)
        this.SoundTacticalHorn(TacticComponent.AttackHornSoundIndex);
      if (this._mainInfantry != null)
      {
        this._mainInfantry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefend>(1f).TacticalDefendPosition = this._mainDefensiveLineObject;
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
      }
      if (this._archers != null)
      {
        this._archers.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._archers);
        this._archers.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
        this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
        if (this._linkedRangedDefensivePosition != null)
          this._archers.AI.SetBehaviorWeight<BehaviorDefend>(1f).TacticalDefendPosition = this._linkedRangedDefensivePosition;
      }
      if (this._leftCavalry != null)
      {
        this._leftCavalry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._leftCavalry);
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorFlank>(1f);
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
      }
      if (this._rightCavalry != null)
      {
        this._rightCavalry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorFlank>(1f);
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
      }
      if (this._rangedCavalry == null)
        return;
      this._rangedCavalry.AI.ResetBehaviorWeights();
      TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
    }

    public TacticDefensiveLine(Team team)
      : base(team)
    {
      this._AIControlledFormationCount = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
    }

    private bool HasBattleBeenJoined() => this._mainInfantry?.QuerySystem.ClosestEnemyFormation == null || this._mainInfantry.AI.ActiveBehavior is BehaviorCharge || this._mainInfantry.AI.ActiveBehavior is BehaviorTacticalCharge || (double) this._mainInfantry.QuerySystem.MedianPosition.AsVec2.Distance(this._mainInfantry.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / (double) this._mainInfantry.QuerySystem.ClosestEnemyFormation.MovementSpeedMaximum <= 5.0 + (this._hasBattleBeenJoined ? 5.0 : 0.0);

    protected override bool CheckAndSetAvailableFormationsChanged()
    {
      int num1 = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
      int num2 = num1 != this._AIControlledFormationCount ? 1 : 0;
      if (num2 != 0)
        this._AIControlledFormationCount = num1;
      if (num2 != 0 || this._mainInfantry != null && (this._mainInfantry.CountOfUnits == 0 || !this._mainInfantry.QuerySystem.IsInfantryFormation) || (this._archers != null && (this._archers.CountOfUnits == 0 || !this._archers.QuerySystem.IsRangedFormation) || this._leftCavalry != null && (this._leftCavalry.CountOfUnits == 0 || !this._leftCavalry.QuerySystem.IsCavalryFormation)) || this._rightCavalry != null && (this._rightCavalry.CountOfUnits == 0 || !this._rightCavalry.QuerySystem.IsCavalryFormation))
        return true;
      if (this._rangedCavalry == null)
        return false;
      return this._rangedCavalry.CountOfUnits == 0 || !this._rangedCavalry.QuerySystem.IsRangedCavalryFormation;
    }

    protected internal override void TickOccasionally()
    {
      if (!this.AreFormationsCreated)
        return;
      bool flag = this.HasBattleBeenJoined();
      if (this.CheckAndSetAvailableFormationsChanged())
      {
        this._hasBattleBeenJoined = flag;
        this.ManageFormationCounts();
        if (this._hasBattleBeenJoined)
          this.Engage();
        else
          this.Defend();
      }
      if (flag != this._hasBattleBeenJoined || this.IsTacticReapplyNeeded)
      {
        this._hasBattleBeenJoined = flag;
        if (this._hasBattleBeenJoined)
          this.Engage();
        else
          this.Defend();
        this.IsTacticReapplyNeeded = false;
      }
      base.TickOccasionally();
    }

    internal override bool ResetTacticalPositions()
    {
      this.DetermineMainDefensiveLine();
      return true;
    }

    internal override float GetTacticWeight()
    {
      if (!this.team.TeamAI.IsDefenseApplicable || !this.CheckAndDetermineFormation(ref this._mainInfantry, (Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)))
        return 0.0f;
      if (!this.team.TeamAI.IsCurrentTactic((TacticComponent) this) || this._mainDefensiveLineObject == null || !this.IsTacticalPositionEligible(this._mainDefensiveLineObject))
        this.DetermineMainDefensiveLine();
      return this._mainDefensiveLineObject == null ? 0.0f : (float) (((double) this.team.QuerySystem.InfantryRatio + (double) this.team.QuerySystem.RangedRatio) * 1.20000004768372) * this.GetTacticalPositionScore(this._mainDefensiveLineObject) * TacticComponent.CalculateNotEngagingTacticalAdvantage(this.team.QuerySystem) / (float) Math.Sqrt((double) this.team.QuerySystem.OverallPowerRatio);
    }

    private bool IsTacticalPositionEligible(TacticalPosition tacticalPosition)
    {
      if (tacticalPosition.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.SpecialMissionPosition)
        return true;
      Vec2 vec2;
      WorldPosition position;
      double num1;
      if (this._mainInfantry == null)
      {
        vec2 = this.team.QuerySystem.AveragePosition;
        ref Vec2 local = ref vec2;
        position = tacticalPosition.Position;
        Vec2 asVec2 = position.AsVec2;
        num1 = (double) local.Distance(asVec2);
      }
      else
      {
        vec2 = this._mainInfantry.QuerySystem.AveragePosition;
        ref Vec2 local = ref vec2;
        position = tacticalPosition.Position;
        Vec2 asVec2 = position.AsVec2;
        num1 = (double) local.Distance(asVec2);
      }
      float num2 = (float) num1;
      vec2 = this.team.QuerySystem.AverageEnemyPosition;
      float num3 = vec2.Distance(this._mainInfantry != null ? this._mainInfantry.QuerySystem.AveragePosition : this.team.QuerySystem.AveragePosition);
      if ((double) num2 > 20.0 && (double) num2 > (double) num3 * 0.5)
        return false;
      if (tacticalPosition.IsInsurmountable)
        return true;
      Vec2 averageEnemyPosition = this.team.QuerySystem.AverageEnemyPosition;
      position = tacticalPosition.Position;
      Vec2 asVec2_1 = position.AsVec2;
      vec2 = averageEnemyPosition - asVec2_1;
      vec2 = vec2.Normalized();
      return (double) vec2.DotProduct(tacticalPosition.Direction) > 0.5;
    }

    private float GetTacticalPositionScore(TacticalPosition tacticalPosition)
    {
      if (tacticalPosition.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.SpecialMissionPosition)
        return 100f;
      if (!this.CheckAndDetermineFormation(ref this._mainInfantry, (Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)))
        return 0.0f;
      double num1 = (double) MBMath.Lerp(1f, 1.5f, MBMath.ClampFloat(tacticalPosition.Slope, 0.0f, 60f) / 60f);
      int countOfUnits = this._mainInfantry.CountOfUnits;
      float num2 = MBMath.Lerp(0.67f, 1f, (float) ((6.0 - (double) MBMath.ClampFloat((float) ((double) this._mainInfantry.MaximumInterval * (double) (countOfUnits - 1) + (double) this._mainInfantry.UnitDiameter * (double) countOfUnits) / tacticalPosition.Width, 3f, 6f)) / 3.0));
      float num3 = tacticalPosition.IsInsurmountable ? 1.3f : 1f;
      float num4 = 1f;
      if (this._archers != null && tacticalPosition.LinkedTacticalPositions.Where<TacticalPosition>((Func<TacticalPosition, bool>) (lcp => lcp.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.Cliff)).ToList<TacticalPosition>().Any<TacticalPosition>())
        num4 = MBMath.Lerp(1f, 1.5f, (float) (((double) MBMath.ClampFloat(this.team.QuerySystem.RangedRatio, 0.05f, 0.25f) - 0.0500000007450581) * 5.0));
      float rangedFactor = this.GetRangedFactor(tacticalPosition);
      float cavalryFactor = this.GetCavalryFactor(tacticalPosition);
      float num5 = MBMath.Lerp(0.7f, 1f, (float) ((150.0 - (double) MBMath.ClampFloat(this._mainInfantry.QuerySystem.AveragePosition.Distance(tacticalPosition.Position.AsVec2), 50f, 150f)) / 100.0));
      double num6 = (double) num2;
      return (float) (num1 * num6) * num4 * rangedFactor * cavalryFactor * num5 * num3;
    }

    private List<TacticalPosition> ExtractPossibleTacticalPositionsFromTacticalRegion(
      TacticalRegion tacticalRegion)
    {
      List<TacticalPosition> tacticalPositionList = new List<TacticalPosition>();
      tacticalRegion.LinkedTacticalPositions.Where<TacticalPosition>((Func<TacticalPosition, bool>) (ltp => ltp.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.HighGround));
      if (tacticalRegion.tacticalRegionType == TacticalRegion.TacticalRegionTypeEnum.Forest)
      {
        Vec2 direction = (this.team.QuerySystem.AverageEnemyPosition - tacticalRegion.Position.AsVec2).Normalized();
        TacticalPosition tacticalPosition1 = new TacticalPosition(tacticalRegion.Position, direction, tacticalRegion.radius, tacticalRegionMembership: TacticalRegion.TacticalRegionTypeEnum.Forest);
        tacticalPositionList.Add(tacticalPosition1);
        float num = tacticalRegion.radius * 0.87f;
        TacticalPosition tacticalPosition2 = new TacticalPosition(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, tacticalRegion.Position.GetNavMeshVec3() + new Vec3(num * direction), false), direction, tacticalRegion.radius, tacticalRegionMembership: TacticalRegion.TacticalRegionTypeEnum.Forest);
        tacticalPositionList.Add(tacticalPosition2);
        TacticalPosition tacticalPosition3 = new TacticalPosition(new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, tacticalRegion.Position.GetNavMeshVec3() - new Vec3(num * direction), false), direction, tacticalRegion.radius, tacticalRegionMembership: TacticalRegion.TacticalRegionTypeEnum.Forest);
        tacticalPositionList.Add(tacticalPosition3);
      }
      return tacticalPositionList;
    }

    private float GetCavalryFactor(TacticalPosition tacticalPosition)
    {
      if (tacticalPosition.TacticalRegionMembership != TacticalRegion.TacticalRegionTypeEnum.Forest)
        return 1f;
      double overallPowerRatio = (double) this.team.QuerySystem.OverallPowerRatio;
      double teamPower = (double) this.team.QuerySystem.TeamPower;
      float num1 = this.team.QuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (et => et.TeamPower));
      double num2 = teamPower - teamPower * ((double) this.team.QuerySystem.CavalryRatio + (double) this.team.QuerySystem.RangedCavalryRatio) * 0.5;
      float num3 = num1 - (float) ((double) num1 * ((double) this.team.QuerySystem.EnemyCavalryRatio + (double) this.team.QuerySystem.EnemyRangedCavalryRatio) * 0.5);
      if ((double) num3 == 0.0)
        num3 = 0.01f;
      double num4 = (double) num3;
      return (float) (num2 / num4) / this.team.QuerySystem.OverallPowerRatio;
    }

    private float GetRangedFactor(TacticalPosition tacticalPosition)
    {
      bool isOuterEdge = tacticalPosition.IsOuterEdge;
      if (tacticalPosition.TacticalRegionMembership != TacticalRegion.TacticalRegionTypeEnum.Forest)
        return 1f;
      double overallPowerRatio = (double) this.team.QuerySystem.OverallPowerRatio;
      float teamPower = this.team.QuerySystem.TeamPower;
      float num1 = this.team.QuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (et => et.TeamPower));
      float num2 = num1 - (float) ((double) num1 * ((double) this.team.QuerySystem.EnemyRangedRatio + (double) this.team.QuerySystem.EnemyRangedCavalryRatio) * 0.5);
      if ((double) num2 == 0.0)
        num2 = 0.01f;
      if (!isOuterEdge)
        teamPower -= (float) ((double) teamPower * ((double) this.team.QuerySystem.RangedRatio + (double) this.team.QuerySystem.RangedCavalryRatio) * 0.5);
      return teamPower / num2 / this.team.QuerySystem.OverallPowerRatio;
    }

    private void DetermineMainDefensiveLine()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
