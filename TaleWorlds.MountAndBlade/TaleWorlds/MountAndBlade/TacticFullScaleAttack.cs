// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticFullScaleAttack
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
  public class TacticFullScaleAttack : TacticComponent
  {
    private bool _hasBattleBeenJoined;
    private int _AIControlledFormationCount;

    protected override void ManageFormationCounts() => this.AssignTacticFormations1121();

    private void Advance()
    {
      if (this.team.IsPlayerTeam && !this.team.IsPlayerGeneral && this.team.IsPlayerSergeant)
        this.SoundTacticalHorn(TacticComponent.MoveHornSoundIndex);
      if (this._mainInfantry != null)
      {
        this._mainInfantry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorAdvance>(1f);
      }
      if (this._archers != null)
      {
        this._archers.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._archers);
        this._archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(1f);
        this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
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

    private void Attack()
    {
      if (this.team.IsPlayerTeam && !this.team.IsPlayerGeneral && this.team.IsPlayerSergeant)
        this.SoundTacticalHorn(TacticComponent.AttackHornSoundIndex);
      if (this._mainInfantry != null)
      {
        this._mainInfantry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorAdvance>(1f);
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
      }
      if (this._archers != null)
      {
        this._archers.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._archers);
        this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
        this._archers.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
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

    public TacticFullScaleAttack(Team team)
      : base(team)
    {
      this._hasBattleBeenJoined = false;
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
      if (this.CheckAndSetAvailableFormationsChanged())
      {
        this.ManageFormationCounts();
        if (this._hasBattleBeenJoined)
          this.Attack();
        else
          this.Advance();
      }
      bool flag = this.HasBattleBeenJoined();
      if (flag != this._hasBattleBeenJoined || this.IsTacticReapplyNeeded)
      {
        this._hasBattleBeenJoined = flag;
        if (this._hasBattleBeenJoined)
          this.Attack();
        else
          this.Advance();
        this.IsTacticReapplyNeeded = false;
      }
      base.TickOccasionally();
    }

    internal override float GetTacticWeight() => (float) ((double) this.team.QuerySystem.InfantryRatio * (double) this.team.QuerySystem.MemberCount / ((double) this.team.QuerySystem.MemberCount - (double) (this.team.QuerySystem.RangedCavalryRatio * (float) this.team.QuerySystem.MemberCount))) * (float) Math.Sqrt((double) this.team.QuerySystem.OverallPowerRatio);
  }
}
