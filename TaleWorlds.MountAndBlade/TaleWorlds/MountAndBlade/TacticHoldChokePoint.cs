// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticHoldChokePoint
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TacticHoldChokePoint : TacticComponent
  {
    private int _AIControlledFormationCount;
    private const float DefendersAdvantage = 1.3f;
    private TacticalPosition _chokePointTacticalPosition;
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
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefend>(1f).TacticalDefendPosition = this._chokePointTacticalPosition;
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

    public TacticHoldChokePoint(Team team)
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

    internal override float GetTacticWeight()
    {
      if (!this.team.TeamAI.IsDefenseApplicable || !this.CheckAndDetermineFormation(ref this._mainInfantry, (Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)))
        return 0.0f;
      if (!this.team.TeamAI.IsCurrentTactic((TacticComponent) this) || this._chokePointTacticalPosition == null || !this.IsTacticalPositionEligible(this._chokePointTacticalPosition))
        this.DetermineChokePoints();
      if (this._chokePointTacticalPosition == null)
        return 0.0f;
      double infantryRatio = (double) this.team.QuerySystem.InfantryRatio;
      return (float) ((infantryRatio + (double) Math.Min((float) infantryRatio, this.team.QuerySystem.RangedRatio)) * (double) MBMath.ClampFloat((float) this.team.QuerySystem.EnemyUnitCount / (float) this.team.QuerySystem.MemberCount, 0.33f, 3f) * (double) this.GetTacticalPositionScore(this._chokePointTacticalPosition) * (double) TacticComponent.CalculateNotEngagingTacticalAdvantage(this.team.QuerySystem) * 1.29999995231628) / (float) Math.Sqrt((double) this.team.QuerySystem.OverallPowerRatio);
    }

    private bool IsTacticalPositionEligible(TacticalPosition tacticalPosition)
    {
      if (!this.CheckAndDetermineFormation(ref this._mainInfantry, (Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)))
        return false;
      float num1 = this.team.QuerySystem.AveragePosition.Distance(tacticalPosition.Position.AsVec2);
      Vec2 vec2 = this.team.QuerySystem.AverageEnemyPosition;
      float num2 = vec2.Distance(this._mainInfantry.QuerySystem.AveragePosition);
      if ((double) num1 > 20.0 && (double) num1 > (double) num2 * 0.5)
        return false;
      int countOfUnits = this._mainInfantry.CountOfUnits;
      if ((double) this._mainInfantry.MaximumInterval * (double) (countOfUnits - 1) + (double) this._mainInfantry.UnitDiameter * (double) countOfUnits < (double) tacticalPosition.Width)
        return false;
      vec2 = this.team.QuerySystem.AverageEnemyPosition - tacticalPosition.Position.AsVec2;
      vec2 = vec2.Normalized();
      float num3 = vec2.DotProduct(tacticalPosition.Direction);
      return tacticalPosition.IsInsurmountable ? (double) Math.Abs(num3) >= 0.5 : (double) num3 >= 0.5;
    }

    private float GetTacticalPositionScore(TacticalPosition tacticalPosition)
    {
      if (!this.CheckAndDetermineFormation(ref this._mainInfantry, (Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation)))
        return 0.0f;
      double num1 = (double) MBMath.Lerp(1f, 1.5f, MBMath.ClampFloat(tacticalPosition.Slope, 0.0f, 60f) / 60f);
      int countOfUnits = this._mainInfantry.CountOfUnits;
      float num2 = MBMath.Lerp(0.67f, 1.5f, (float) (((double) MBMath.ClampFloat((float) ((double) this._mainInfantry.Interval * (double) (countOfUnits - 1) + (double) this._mainInfantry.UnitDiameter * (double) countOfUnits) / tacticalPosition.Width, 0.5f, 3f) - 0.5) / 2.5));
      float num3 = 1f;
      if (this.CheckAndDetermineFormation(ref this._archers, (Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)) && tacticalPosition.LinkedTacticalPositions.Where<TacticalPosition>((Func<TacticalPosition, bool>) (lcp => lcp.TacticalPositionType == TacticalPosition.TacticalPositionTypeEnum.Cliff)).ToList<TacticalPosition>().Any<TacticalPosition>())
        num3 = MBMath.Lerp(1f, 1.5f, (float) (((double) MBMath.ClampFloat(this.team.QuerySystem.RangedRatio, 0.05f, 0.25f) - 0.0500000007450581) * 5.0));
      float num4 = MBMath.Lerp(0.7f, 1f, (float) ((150.0 - (double) MBMath.ClampFloat(this._mainInfantry.QuerySystem.AveragePosition.Distance(tacticalPosition.Position.AsVec2), 50f, 150f)) / 100.0));
      double num5 = (double) num2;
      return (float) (num1 * num5) * num3 * num4;
    }

    internal override bool ResetTacticalPositions()
    {
      this.DetermineChokePoints();
      return true;
    }

    private void DetermineChokePoints()
    {
      // ISSUE: unable to decompile the method.
    }
  }
}
