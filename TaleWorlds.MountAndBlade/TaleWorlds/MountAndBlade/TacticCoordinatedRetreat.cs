// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticCoordinatedRetreat
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TacticCoordinatedRetreat : TacticComponent
  {
    private bool _canWeSafelyRunAway;
    private int _AIControlledFormationCount;
    private Vec2 _retreatPosition = Vec2.Invalid;
    private const float RetreatThresholdValue = 2f;

    protected override void ManageFormationCounts()
    {
      if (this._canWeSafelyRunAway)
        return;
      this.AssignTacticFormations1121();
    }

    private void OrganizedRetreat()
    {
      if (this.team.IsPlayerTeam && !this.team.IsPlayerGeneral && this.team.IsPlayerSergeant)
        this.SoundTacticalHorn(TacticComponent.RetreatHornSoundIndex);
      if (this._mainInfantry != null)
      {
        this._mainInfantry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
        BehaviorDefend behaviorDefend = this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefend>(1f);
        WorldPosition positionForFormation = Mission.Current.GetClosestFleePositionForFormation(this._mainInfantry);
        positionForFormation.SetVec2(Mission.Current.GetClosestBoundaryPosition(positionForFormation.AsVec2));
        this._retreatPosition = positionForFormation.AsVec2;
        WorldPosition worldPosition = positionForFormation;
        behaviorDefend.DefensePosition = worldPosition;
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorPullBack>(1f);
      }
      if (this._archers != null)
      {
        this._archers.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._archers);
        this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
        this._archers.AI.SetBehaviorWeight<BehaviorPullBack>(1.5f);
      }
      if (this._leftCavalry != null)
      {
        this._leftCavalry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._leftCavalry);
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Left;
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorPullBack>(1.5f);
      }
      if (this._rightCavalry != null)
      {
        this._rightCavalry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Right;
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorCavalryScreen>(1f);
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorPullBack>(1.5f);
      }
      if (this._rangedCavalry == null)
        return;
      this._rangedCavalry.AI.ResetBehaviorWeights();
      TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorPullBack>(1.5f);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
    }

    private void RunForTheBorder()
    {
      if (this.team.IsPlayerTeam && !this.team.IsPlayerGeneral && this.team.IsPlayerSergeant)
        this.SoundTacticalHorn(TacticComponent.RetreatHornSoundIndex);
      foreach (Formation formation in this.Formations)
      {
        formation.AI.ResetBehaviorWeights();
        formation.AI.SetBehaviorWeight<BehaviorRetreat>(1f);
      }
    }

    public TacticCoordinatedRetreat(Team team)
      : base(team)
    {
      this._AIControlledFormationCount = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
    }

    private bool HasRetreatDestinationBeenReached() => this.Formations.All<Formation>((Func<Formation, bool>) (f => !f.QuerySystem.IsInfantryFormation || (double) f.QuerySystem.AveragePosition.DistanceSquared(this._retreatPosition) < 5625.0));

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
      bool flag = this.HasRetreatDestinationBeenReached();
      if (this.CheckAndSetAvailableFormationsChanged())
      {
        this._canWeSafelyRunAway = flag;
        this.ManageFormationCounts();
        if (this._canWeSafelyRunAway)
          this.RunForTheBorder();
        else
          this.OrganizedRetreat();
      }
      if (flag != this._canWeSafelyRunAway || this.IsTacticReapplyNeeded)
      {
        this._canWeSafelyRunAway = flag;
        if (this._canWeSafelyRunAway)
          this.RunForTheBorder();
        else
          this.OrganizedRetreat();
        this.IsTacticReapplyNeeded = false;
      }
      base.TickOccasionally();
    }

    internal override float GetTacticWeight()
    {
      float num1 = this.team.QuerySystem.PowerRatioIncludingCasualties / this.team.QuerySystem.OverallPowerRatio;
      double num2 = (double) MBMath.LinearExtrapolation(0.0f, Math.Max(this.team.QuerySystem.InfantryRatio, Math.Max(this.team.QuerySystem.RangedRatio, this.team.QuerySystem.CavalryRatio)), MBMath.ClampFloat(num1, 0.0f, 2.66f) / 2f);
      float num3 = this.team.QuerySystem.EnemyTeams.Average<TeamQuerySystem>((Func<TeamQuerySystem, float>) (et => et.CavalryRatio)) + this.team.QuerySystem.EnemyTeams.Average<TeamQuerySystem>((Func<TeamQuerySystem, float>) (et => et.RangedCavalryRatio));
      double num4 = (double) num3 != 0.0 ? (double) MBMath.Lerp(0.8f, 1.2f, MBMath.ClampFloat((this.team.QuerySystem.CavalryRatio + this.team.QuerySystem.RangedCavalryRatio) / num3, 0.0f, 2f) / 2f) : 1.20000004768372;
      return (float) (num2 * num4) * (float) Math.Min(1.0, Math.Sqrt((double) this.team.QuerySystem.OverallPowerRatio));
    }
  }
}
