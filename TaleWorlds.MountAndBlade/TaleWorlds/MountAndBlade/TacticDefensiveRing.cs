// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticDefensiveRing
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TacticDefensiveRing : TacticComponent
  {
    private int _AIControlledFormationCount;
    private const float DefendersAdvantage = 1.5f;
    private TacticalPosition _mainRingPosition;

    protected override void ManageFormationCounts() => this.AssignTacticFormations1121();

    private void Defend()
    {
      if (this.team.IsPlayerTeam && !this.team.IsPlayerGeneral && this.team.IsPlayerSergeant)
        this.SoundTacticalHorn(TacticComponent.MoveHornSoundIndex);
      if (this._mainInfantry != null)
      {
        this._mainInfantry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefensiveRing>(1f).TacticalDefendPosition = this._mainRingPosition;
      }
      if (this._archers != null && this._mainInfantry != null)
      {
        this._archers.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._archers);
        this._archers.AI.SetBehaviorWeight<BehaviorFireFromInfantryCover>(1f);
        this._archers.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
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

    public TacticDefensiveRing(Team team)
      : base(team)
    {
      this._AIControlledFormationCount = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
    }

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
      if (this.CheckAndSetAvailableFormationsChanged() || this.IsTacticReapplyNeeded)
      {
        this.ManageFormationCounts();
        this.Defend();
        this.IsTacticReapplyNeeded = false;
      }
      base.TickOccasionally();
    }

    internal override bool ResetTacticalPositions()
    {
      this.DetermineRingPosition();
      return true;
    }

    internal override float GetTacticWeight()
    {
      if (!this.team.TeamAI.IsDefenseApplicable || !this.CheckAndDetermineFormation(ref this._mainInfantry, (Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)) || !this.CheckAndDetermineFormation(ref this._archers, (Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)))
        return 0.0f;
      double num1 = (double) Math.Max(0, this._mainInfantry.CountOfUnits - 1) * ((double) this._mainInfantry.MaximumInterval + (double) this._mainInfantry.UnitDiameter) * 0.5 / Math.PI;
      double num2 = Math.Sqrt((double) this._archers.CountOfUnits);
      double num3 = (double) this._archers.UnitDiameter * num2 + (double) this._archers.Interval * (num2 - 1.0);
      if (num1 < num3)
        return 0.0f;
      if (!this.team.TeamAI.IsCurrentTactic((TacticComponent) this) || this._mainRingPosition == null || !this.IsTacticalPositionEligible(this._mainRingPosition))
        this.DetermineRingPosition();
      return this._mainRingPosition == null ? 0.0f : (float) ((double) Math.Min(this.team.QuerySystem.InfantryRatio, this.team.QuerySystem.RangedRatio) * 2.0 * 1.5) * this.GetTacticalPositionScore(this._mainRingPosition) * TacticComponent.CalculateNotEngagingTacticalAdvantage(this.team.QuerySystem) / (float) Math.Sqrt((double) this.team.QuerySystem.OverallPowerRatio);
    }

    private bool IsTacticalPositionEligible(TacticalPosition tacticalPosition)
    {
      float num1 = this._mainInfantry != null ? this._mainInfantry.QuerySystem.AveragePosition.Distance(tacticalPosition.Position.AsVec2) : this.team.QuerySystem.AveragePosition.Distance(tacticalPosition.Position.AsVec2);
      float num2 = this.team.QuerySystem.AverageEnemyPosition.Distance(this._mainInfantry != null ? this._mainInfantry.QuerySystem.AveragePosition : this.team.QuerySystem.AveragePosition);
      return ((double) num1 <= 20.0 || (double) num1 <= (double) num2 * 0.5) && tacticalPosition.IsInsurmountable;
    }

    private float GetTacticalPositionScore(TacticalPosition tacticalPosition)
    {
      if (!this.CheckAndDetermineFormation(ref this._mainInfantry, (Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)) || !this.CheckAndDetermineFormation(ref this._archers, (Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)))
        return 0.0f;
      double num1 = (double) MBMath.Lerp(1f, 1.5f, MBMath.ClampFloat(tacticalPosition.Slope, 0.0f, 60f) / 60f);
      Formation formation = this.team.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)).MaxBy<Formation, int>((Func<Formation, int>) (f => f.CountOfUnits));
      float num2 = Math.Max(formation.arrangement.Depth, formation.arrangement.Width);
      float num3 = MBMath.ClampFloat(tacticalPosition.Width / num2, 0.7f, 1f);
      float num4 = tacticalPosition.IsInsurmountable ? 1.5f : 1f;
      float cavalryFactor = this.GetCavalryFactor(tacticalPosition);
      float num5 = MBMath.Lerp(0.7f, 1f, (float) ((150.0 - (double) MBMath.ClampFloat(this._mainInfantry.QuerySystem.AveragePosition.Distance(tacticalPosition.Position.AsVec2), 50f, 150f)) / 100.0));
      double num6 = (double) num3;
      return (float) (num1 * num6) * num4 * cavalryFactor * num5;
    }

    private List<TacticalPosition> ExtractPossibleTacticalPositionsFromTacticalRegion(
      TacticalRegion tacticalRegion)
    {
      List<TacticalPosition> tacticalPositionList = new List<TacticalPosition>();
      tacticalRegion.LinkedTacticalPositions.Where<TacticalPosition>((Func<TacticalPosition, bool>) (ltp => ltp.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.HighGround));
      if (tacticalRegion.tacticalRegionType == TacticalRegion.TacticalRegionTypeEnum.DifficultTerrain || tacticalRegion.tacticalRegionType == TacticalRegion.TacticalRegionTypeEnum.Opening)
      {
        Vec2 direction = (this.team.QuerySystem.AverageEnemyPosition - tacticalRegion.Position.AsVec2).Normalized();
        TacticalPosition tacticalPosition = new TacticalPosition(tacticalRegion.Position, direction, tacticalRegion.radius, isInsurmountable: true, tacticalRegionMembership: tacticalRegion.tacticalRegionType);
        tacticalPositionList.Add(tacticalPosition);
      }
      return tacticalPositionList;
    }

    private float GetCavalryFactor(TacticalPosition tacticalPosition)
    {
      if (tacticalPosition.TacticalRegionMembership == TacticalRegion.TacticalRegionTypeEnum.Opening)
        return 1f;
      double overallPowerRatio = (double) this.team.QuerySystem.OverallPowerRatio;
      double teamPower = (double) this.team.QuerySystem.TeamPower;
      float num = this.team.QuerySystem.EnemyTeams.Sum<TeamQuerySystem>((Func<TeamQuerySystem, float>) (et => et.TeamPower));
      return (float) (teamPower - teamPower * ((double) this.team.QuerySystem.CavalryRatio + (double) this.team.QuerySystem.RangedCavalryRatio) * 0.5) / (num - (float) ((double) num * ((double) this.team.QuerySystem.EnemyCavalryRatio + (double) this.team.QuerySystem.EnemyRangedCavalryRatio) * 0.5)) / this.team.QuerySystem.OverallPowerRatio;
    }

    private void DetermineRingPosition()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
