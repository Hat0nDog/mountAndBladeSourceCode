// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticSallyOutDefense
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class TacticSallyOutDefense : TacticComponent
  {
    private bool _hasBattleBeenJoined;
    private int _AIControlledFormationCount;
    private WorldPosition SallyOutDefensePosition;
    private Formation _cavalryFormation;
    private readonly TeamAISallyOutDefender _teamAISallyOutDefender;
    private List<SiegeWeapon> _destructableSiegeWeapons;
    private TacticSallyOutDefense.WeaponsToBeDefended _weaponsToBeDefendedState;

    protected override void ManageFormationCounts()
    {
      if (this._weaponsToBeDefendedState == TacticSallyOutDefense.WeaponsToBeDefended.TwoPrimary)
      {
        this.ManageFormationCounts(1, 1, 1, 1);
        this._mainInfantry = TacticComponent.ChooseAndSortByPriority(this.Formations, (Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation), (Func<Formation, bool>) (f => f.IsAIControlled), (Func<Formation, float>) (f => f.QuerySystem.FormationPower)).FirstOrDefault<Formation>();
        if (this._mainInfantry != null)
          this._mainInfantry.AI.IsMainFormation = true;
        this._archers = TacticComponent.ChooseAndSortByPriority(this.Formations, (Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation), (Func<Formation, bool>) (f => f.IsAIControlled), (Func<Formation, float>) (f => f.QuerySystem.FormationPower)).FirstOrDefault<Formation>();
        this._cavalryFormation = TacticComponent.ChooseAndSortByPriority(this.Formations, (Func<Formation, bool>) (f => f.QuerySystem.IsCavalryFormation), (Func<Formation, bool>) (f => f.IsAIControlled), (Func<Formation, float>) (f => f.QuerySystem.FormationPower)).FirstOrDefault<Formation>();
        this._rangedCavalry = TacticComponent.ChooseAndSortByPriority(this.Formations, (Func<Formation, bool>) (f => f.QuerySystem.IsRangedCavalryFormation), (Func<Formation, bool>) (f => f.IsAIControlled), (Func<Formation, float>) (f => f.QuerySystem.FormationPower)).FirstOrDefault<Formation>();
      }
      else
        this.AssignTacticFormations1121();
    }

    private void Engage()
    {
      if (this._leftCavalry != null)
      {
        this._leftCavalry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._leftCavalry);
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorFlank>(1f);
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
      }
      if (this._rightCavalry == null)
        return;
      this._rightCavalry.AI.ResetBehaviorWeights();
      TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
      this._rightCavalry.AI.SetBehaviorWeight<BehaviorFlank>(1f);
      this._rightCavalry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
    }

    private void DetermineState()
    {
      if (this._destructableSiegeWeapons.Count == 0)
      {
        this._weaponsToBeDefendedState = TacticSallyOutDefense.WeaponsToBeDefended.NoWeapons;
      }
      else
      {
        switch (this._destructableSiegeWeapons.Count<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => dsw is IPrimarySiegeWeapon && !dsw.IsDisabled)))
        {
          case 0:
            this._weaponsToBeDefendedState = TacticSallyOutDefense.WeaponsToBeDefended.OnlyRangedWeapons;
            break;
          case 1:
            this._weaponsToBeDefendedState = TacticSallyOutDefense.WeaponsToBeDefended.OnePrimary;
            break;
          case 2:
            this._weaponsToBeDefendedState = TacticSallyOutDefense.WeaponsToBeDefended.TwoPrimary;
            break;
          case 3:
            this._weaponsToBeDefendedState = TacticSallyOutDefense.WeaponsToBeDefended.ThreePrimary;
            break;
        }
      }
    }

    public TacticSallyOutDefense(Team team)
      : base(team)
    {
      this._teamAISallyOutDefender = team.TeamAI as TeamAISallyOutDefender;
      this._destructableSiegeWeapons = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeWeapon>().Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (sw => sw.Side == team.Side && sw.IsDestructible)).ToList<SiegeWeapon>();
      this.DetermineState();
      this.SallyOutDefensePosition = team.TeamAI is TeamAISallyOutDefender ? (team.TeamAI as TeamAISallyOutDefender).DefensePosition() : WorldPosition.Invalid;
      this.ManageFormationCounts();
      this.ApplyDefenseBasedOnState();
      this._AIControlledFormationCount = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
    }

    private bool CalculateHasBattleBeenJoined() => this._mainInfantry?.QuerySystem.ClosestEnemyFormation == null || this._mainInfantry.AI.ActiveBehavior is BehaviorCharge || this._mainInfantry.AI.ActiveBehavior is BehaviorTacticalCharge || (double) this._mainInfantry.QuerySystem.MedianPosition.AsVec2.Distance(this._mainInfantry.QuerySystem.ClosestEnemyFormation.MedianPosition.AsVec2) / (double) this._mainInfantry.QuerySystem.ClosestEnemyFormation.MovementSpeedMaximum <= 3.0 + (this._hasBattleBeenJoined ? 2.0 : 0.0);

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

    private void DefendCenterLocation()
    {
      if (this._mainInfantry != null)
      {
        this._mainInfantry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefendSiegeWeapon>(1f);
        BehaviorDefendSiegeWeapon behavior = this._mainInfantry.AI.GetBehavior<BehaviorDefendSiegeWeapon>();
        behavior.SetDefensePositionFromTactic(this._teamAISallyOutDefender.CalculateSallyOutReferencePosition(FormationAI.BehaviorSide.Middle).ToWorldPosition());
        behavior.SetDefendedSiegeWeaponFromTactic(this._teamAISallyOutDefender.PrimarySiegeWeapons.FirstOrDefault<SiegeWeapon>((Func<SiegeWeapon, bool>) (psw => psw is IPrimarySiegeWeapon && (psw as IPrimarySiegeWeapon).WeaponSide == FormationAI.BehaviorSide.Middle)));
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
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
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorDefendSiegeWeapon>(1f);
        BehaviorDefendSiegeWeapon behavior = this._leftCavalry.AI.GetBehavior<BehaviorDefendSiegeWeapon>();
        behavior.SetDefensePositionFromTactic(this._teamAISallyOutDefender.CalculateSallyOutReferencePosition(FormationAI.BehaviorSide.Left).ToWorldPosition());
        behavior.SetDefendedSiegeWeaponFromTactic(this._teamAISallyOutDefender.PrimarySiegeWeapons.FirstOrDefault<SiegeWeapon>((Func<SiegeWeapon, bool>) (psw => psw is IPrimarySiegeWeapon && (psw as IPrimarySiegeWeapon).WeaponSide == FormationAI.BehaviorSide.Left)));
        this._leftCavalry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
        BehaviorProtectFlank behaviorProtectFlank = this._leftCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f);
        behaviorProtectFlank.FlankSide = FormationAI.BehaviorSide.Left;
        this._leftCavalry.AI.AddSpecialBehavior((BehaviorComponent) behaviorProtectFlank, true);
      }
      if (this._rightCavalry != null)
      {
        this._rightCavalry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorDefendSiegeWeapon>(1f);
        BehaviorDefendSiegeWeapon behavior = this._rightCavalry.AI.GetBehavior<BehaviorDefendSiegeWeapon>();
        behavior.SetDefensePositionFromTactic(this._teamAISallyOutDefender.CalculateSallyOutReferencePosition(FormationAI.BehaviorSide.Right).ToWorldPosition());
        behavior.SetDefendedSiegeWeaponFromTactic(this._teamAISallyOutDefender.PrimarySiegeWeapons.FirstOrDefault<SiegeWeapon>((Func<SiegeWeapon, bool>) (psw => psw is IPrimarySiegeWeapon && (psw as IPrimarySiegeWeapon).WeaponSide == FormationAI.BehaviorSide.Right)));
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
        BehaviorProtectFlank behaviorProtectFlank = this._leftCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f);
        behaviorProtectFlank.FlankSide = FormationAI.BehaviorSide.Right;
        this._rightCavalry.AI.AddSpecialBehavior((BehaviorComponent) behaviorProtectFlank, true);
      }
      if (this._rangedCavalry == null)
        return;
      this._rangedCavalry.AI.ResetBehaviorWeights();
      TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
    }

    private void DefendTwoMainPositions()
    {
      FormationAI.BehaviorSide infantrySide = FormationAI.BehaviorSide.BehaviorSideNotSet;
      if (this._mainInfantry != null)
      {
        this._mainInfantry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefendSiegeWeapon>(1f);
        BehaviorDefendSiegeWeapon behavior = this._mainInfantry.AI.GetBehavior<BehaviorDefendSiegeWeapon>();
        SiegeWeapon siegeWeapon = this._destructableSiegeWeapons.FirstOrDefault<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => dsw is IPrimarySiegeWeapon && dsw is IMoveableSiegeWeapon && (dsw as IPrimarySiegeWeapon).WeaponSide == FormationAI.BehaviorSide.Middle));
        if (siegeWeapon != null)
        {
          infantrySide = FormationAI.BehaviorSide.Middle;
        }
        else
        {
          siegeWeapon = this._destructableSiegeWeapons.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => dsw is IPrimarySiegeWeapon && dsw is IMoveableSiegeWeapon)).MinBy<SiegeWeapon, float>((Func<SiegeWeapon, float>) (dsw => dsw.GameEntity.GlobalPosition.AsVec2.DistanceSquared(this._mainInfantry.QuerySystem.AveragePosition)));
          infantrySide = (siegeWeapon as IPrimarySiegeWeapon).WeaponSide;
        }
        behavior.SetDefensePositionFromTactic(this._teamAISallyOutDefender.CalculateSallyOutReferencePosition(infantrySide).ToWorldPosition());
        behavior.SetDefendedSiegeWeaponFromTactic(siegeWeapon);
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
      }
      if (this._archers != null)
      {
        this._archers.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._archers);
        this._archers.AI.SetBehaviorWeight<BehaviorSkirmishLine>(1f);
        this._archers.AI.SetBehaviorWeight<BehaviorScreenedSkirmish>(1f);
        this._archers.AI.SetBehaviorWeight<BehaviorSkirmish>(1f);
      }
      if (this._cavalryFormation != null)
      {
        if (infantrySide != FormationAI.BehaviorSide.BehaviorSideNotSet)
        {
          this._cavalryFormation.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(this._cavalryFormation);
          this._cavalryFormation.AI.SetBehaviorWeight<BehaviorDefendSiegeWeapon>(1f);
          BehaviorDefendSiegeWeapon behavior = this._cavalryFormation.AI.GetBehavior<BehaviorDefendSiegeWeapon>();
          SiegeWeapon siegeWeapon = this._destructableSiegeWeapons.FirstOrDefault<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => dsw is IPrimarySiegeWeapon && (dsw as IPrimarySiegeWeapon).WeaponSide != infantrySide));
          behavior.SetDefensePositionFromTactic(this._teamAISallyOutDefender.CalculateSallyOutReferencePosition(siegeWeapon == null ? (this._destructableSiegeWeapons.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => dsw is IPrimarySiegeWeapon && dsw is IMoveableSiegeWeapon)).MinBy<SiegeWeapon, float>((Func<SiegeWeapon, float>) (dsw => dsw.GameEntity.GlobalPosition.AsVec2.DistanceSquared(this._cavalryFormation.QuerySystem.AveragePosition))) as IPrimarySiegeWeapon).WeaponSide : (siegeWeapon as IPrimarySiegeWeapon).WeaponSide).ToWorldPosition());
          behavior.SetDefendedSiegeWeaponFromTactic(siegeWeapon);
          this._cavalryFormation.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
        }
        else
        {
          this._cavalryFormation.AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(this._cavalryFormation);
          this._cavalryFormation.AI.SetBehaviorWeight<BehaviorDefendSiegeWeapon>(1f);
          BehaviorDefendSiegeWeapon behavior = this._cavalryFormation.AI.GetBehavior<BehaviorDefendSiegeWeapon>();
          SiegeWeapon siegeWeapon = this._destructableSiegeWeapons.FirstOrDefault<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => dsw is IPrimarySiegeWeapon && (dsw as IPrimarySiegeWeapon).WeaponSide == FormationAI.BehaviorSide.Middle));
          FormationAI.BehaviorSide side;
          if (siegeWeapon != null)
          {
            side = FormationAI.BehaviorSide.Middle;
          }
          else
          {
            siegeWeapon = this._destructableSiegeWeapons.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => dsw is IPrimarySiegeWeapon && dsw is IMoveableSiegeWeapon)).MinBy<SiegeWeapon, float>((Func<SiegeWeapon, float>) (dsw => dsw.GameEntity.GlobalPosition.AsVec2.DistanceSquared(this._cavalryFormation.QuerySystem.AveragePosition)));
            side = (siegeWeapon as IPrimarySiegeWeapon).WeaponSide;
          }
          behavior.SetDefensePositionFromTactic(this._teamAISallyOutDefender.CalculateSallyOutReferencePosition(side).ToWorldPosition());
          behavior.SetDefendedSiegeWeaponFromTactic(siegeWeapon);
          this._cavalryFormation.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
        }
      }
      if (this._rangedCavalry == null)
        return;
      this._rangedCavalry.AI.ResetBehaviorWeights();
      TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
    }

    private void DefendSingleMainPosition()
    {
      if (this._mainInfantry != null)
      {
        this._mainInfantry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
        if (this._destructableSiegeWeapons.FirstOrDefault<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => dsw is IPrimarySiegeWeapon && dsw is IMoveableSiegeWeapon)) is IPrimarySiegeWeapon primarySiegeWeapon2)
        {
          this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefendSiegeWeapon>(1f);
          BehaviorDefendSiegeWeapon behavior = this._mainInfantry.AI.GetBehavior<BehaviorDefendSiegeWeapon>();
          behavior.SetDefensePositionFromTactic(this._teamAISallyOutDefender.CalculateSallyOutReferencePosition(primarySiegeWeapon2.WeaponSide).ToWorldPosition());
          behavior.SetDefendedSiegeWeaponFromTactic(primarySiegeWeapon2 as SiegeWeapon);
        }
        else
        {
          if (this._destructableSiegeWeapons.Any<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => !dsw.IsDisabled)))
          {
            SiegeWeapon siegeWeapon = this._destructableSiegeWeapons.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (dsw => !dsw.IsDisabled)).MinBy<SiegeWeapon, float>((Func<SiegeWeapon, float>) (dsw => dsw.GameEntity.GlobalPosition.AsVec2.DistanceSquared(this._mainInfantry.QuerySystem.AveragePosition)));
            this._mainInfantry.AI.ResetBehaviorWeights();
            TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
            this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefendSiegeWeapon>(1f);
            BehaviorDefendSiegeWeapon behavior = this._mainInfantry.AI.GetBehavior<BehaviorDefendSiegeWeapon>();
            behavior.SetDefensePositionFromTactic(siegeWeapon.GameEntity.GlobalPosition.ToWorldPosition());
            behavior.SetDefendedSiegeWeaponFromTactic(siegeWeapon);
          }
          else
          {
            this._mainInfantry.AI.ResetBehaviorWeights();
            TacticComponent.SetDefaultBehaviorWeights(this._mainInfantry);
            this._mainInfantry.AI.SetBehaviorWeight<BehaviorDefend>(1f);
            this._mainInfantry.AI.GetBehavior<BehaviorDefend>().DefensePosition = this._teamAISallyOutDefender.CalculateSallyOutReferencePosition(FormationAI.BehaviorSide.Middle).ToWorldPosition();
          }
          this._mainInfantry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
        }
        this._mainInfantry.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
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
      }
      if (this._rightCavalry != null)
      {
        this._rightCavalry.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._rightCavalry);
        this._rightCavalry.AI.SetBehaviorWeight<BehaviorProtectFlank>(1f).FlankSide = FormationAI.BehaviorSide.Right;
      }
      if (this._rangedCavalry == null)
        return;
      this._rangedCavalry.AI.ResetBehaviorWeights();
      TacticComponent.SetDefaultBehaviorWeights(this._rangedCavalry);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorMountedSkirmish>(1f);
      this._rangedCavalry.AI.SetBehaviorWeight<BehaviorHorseArcherSkirmish>(1f);
    }

    private void ApplyDefenseBasedOnState()
    {
      this._destructableSiegeWeapons.RemoveAll((Predicate<SiegeWeapon>) (dsw => dsw.IsDisabled));
      switch (this._weaponsToBeDefendedState)
      {
        case TacticSallyOutDefense.WeaponsToBeDefended.TwoPrimary:
          this.DefendTwoMainPositions();
          break;
        case TacticSallyOutDefense.WeaponsToBeDefended.ThreePrimary:
          this.DefendCenterLocation();
          break;
        default:
          this.DefendSingleMainPosition();
          break;
      }
    }

    protected internal override void TickOccasionally()
    {
      if (!this.AreFormationsCreated)
        return;
      if (this.CheckAndSetAvailableFormationsChanged())
      {
        this.ManageFormationCounts();
        this.ApplyDefenseBasedOnState();
        if (this._hasBattleBeenJoined)
          this.Engage();
      }
      int toBeDefendedState1 = (int) this._weaponsToBeDefendedState;
      this.DetermineState();
      int toBeDefendedState2 = (int) this._weaponsToBeDefendedState;
      if (toBeDefendedState1 != toBeDefendedState2)
      {
        this.ApplyDefenseBasedOnState();
        this.IsTacticReapplyNeeded = false;
      }
      bool battleBeenJoined = this.CalculateHasBattleBeenJoined();
      if (battleBeenJoined != this._hasBattleBeenJoined || this.IsTacticReapplyNeeded)
      {
        this._hasBattleBeenJoined = battleBeenJoined;
        if (this._hasBattleBeenJoined)
          this.Engage();
        this.IsTacticReapplyNeeded = false;
      }
      base.TickOccasionally();
    }

    internal override float GetTacticWeight() => 10f;

    private enum WeaponsToBeDefended
    {
      NoWeapons,
      OnlyRangedWeapons,
      OnePrimary,
      TwoPrimary,
      ThreePrimary,
    }
  }
}
