// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticSallyOutHitAndRun
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
  public class TacticSallyOutHitAndRun : TacticComponent
  {
    private TacticSallyOutHitAndRun.TacticState _state;
    private Formation _mainInfantryFormation;
    private List<Formation> _archerFormations;
    private List<Formation> _cavalryFormations;
    private int _AIControlledFormationCount;
    private readonly TeamAISallyOutAttacker _teamAISallyOutAttacker;
    private List<SiegeWeapon> _destructibleEnemySiegeWeapons;

    protected override void ManageFormationCounts()
    {
      List<IPrimarySiegeWeapon> list = this._teamAISallyOutAttacker.PrimarySiegeWeapons.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (psw => psw is IPrimarySiegeWeapon && !psw.IsDisabled && psw.IsDestructible)).Select<SiegeWeapon, IPrimarySiegeWeapon>((Func<SiegeWeapon, IPrimarySiegeWeapon>) (psw => psw as IPrimarySiegeWeapon)).ToList<IPrimarySiegeWeapon>();
      int count1 = list.Count;
      bool flag1 = this._teamAISallyOutAttacker.BesiegerRangedSiegeWeapons.Count > 0;
      int count2 = Math.Max(count1, 1 + (list.Count > 0 & flag1 ? 1 : 0));
      int count3 = Math.Min(this._teamAISallyOutAttacker.ArcherPositions.Count<GameEntity>(), 7 - count2);
      this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation), 1);
      this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation), count3);
      this.SplitFormationClassIntoGivenNumber((Func<Formation, bool>) (f => f.QuerySystem.IsCavalryFormation || f.QuerySystem.IsRangedCavalryFormation), count2);
      this._mainInfantryFormation = this.Formations.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsInfantryFormation));
      if (this._mainInfantryFormation != null)
        this._mainInfantryFormation.AI.IsMainFormation = true;
      this._archerFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsRangedFormation)).ToList<Formation>();
      this._cavalryFormations.Clear();
      this._cavalryFormations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.QuerySystem.IsCavalryFormation || f.QuerySystem.IsRangedCavalryFormation)).ToList<Formation>();
      bool flag2 = list.Count == 0 || list.Count == 1 & flag1;
      for (int index = 0; index < this._cavalryFormations.Count - (flag2 ? 1 : 0); ++index)
        this._cavalryFormations[index].AI.Side = list[index % list.Count].WeaponSide;
      if (!(this._cavalryFormations.Count > 0 & flag2))
        return;
      if (list.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw != null && psw.WeaponSide == FormationAI.BehaviorSide.Middle)))
        this._cavalryFormations.Last<Formation>().AI.Side = FormationAI.BehaviorSide.Left;
      else
        this._cavalryFormations.Last<Formation>().AI.Side = FormationAI.BehaviorSide.Middle;
    }

    private void DestroySiegeWeapons()
    {
      if (this._mainInfantryFormation != null)
      {
        this._mainInfantryFormation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantryFormation);
        BehaviorDefend behaviorDefend = this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorDefend>(1f);
        Vec2 vec2 = (this._teamAISallyOutAttacker.OuterGate.GameEntity.GlobalPosition.AsVec2 - this._teamAISallyOutAttacker.InnerGate.GameEntity.GlobalPosition.AsVec2).Normalized();
        WorldPosition worldPosition1 = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, this._teamAISallyOutAttacker.OuterGate.GameEntity.GlobalPosition, false);
        worldPosition1.SetVec2(worldPosition1.AsVec2 + (3f + this._mainInfantryFormation.Depth) * vec2);
        WorldPosition worldPosition2 = worldPosition1;
        behaviorDefend.DefensePosition = worldPosition2;
        this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorDestroySiegeWeapons>(1f);
      }
      GameEntity[] array1 = this._teamAISallyOutAttacker.ArcherPositions.ToArray<GameEntity>();
      int length = array1.Length;
      if (length > 0)
      {
        Formation[] array2 = this._archerFormations.ToArray();
        for (int index = 0; index < array2.Length; ++index)
        {
          array2[index].AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(array2[index]);
          array2[index].AI.SetBehaviorWeight<BehaviorShootFromCastleWalls>(1f);
          array2[index].AI.GetBehavior<BehaviorShootFromCastleWalls>().ArcherPosition = array1[index % length];
        }
      }
      foreach (Formation cavalryFormation in this._cavalryFormations)
      {
        cavalryFormation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(cavalryFormation);
        cavalryFormation.AI.SetBehaviorWeight<BehaviorDestroySiegeWeapons>(1f);
      }
    }

    private void CavalryRetreat()
    {
      if (this._mainInfantryFormation != null)
      {
        this._mainInfantryFormation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantryFormation);
        BehaviorDefend behaviorDefend = this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorDefend>(1f);
        Vec2 vec2 = (this._teamAISallyOutAttacker.OuterGate.GameEntity.GlobalPosition.AsVec2 - this._teamAISallyOutAttacker.InnerGate.GameEntity.GlobalPosition.AsVec2).Normalized();
        WorldPosition worldPosition1 = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, this._teamAISallyOutAttacker.OuterGate.GameEntity.GlobalPosition, false);
        worldPosition1.SetVec2(worldPosition1.AsVec2 + (3f + this._mainInfantryFormation.Depth) * vec2);
        WorldPosition worldPosition2 = worldPosition1;
        behaviorDefend.DefensePosition = worldPosition2;
      }
      foreach (Formation cavalryFormation in this._cavalryFormations)
      {
        cavalryFormation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(cavalryFormation);
        cavalryFormation.AI.SetBehaviorWeight<BehaviorRetreatToKeep>(1f);
      }
    }

    private void InfantryRetreat()
    {
      if (this._mainInfantryFormation == null)
        return;
      this._mainInfantryFormation.AI.Side = FormationAI.BehaviorSide.Middle;
      this._mainInfantryFormation.AI.ResetBehaviorWeights();
      TacticComponent.SetDefaultBehaviorWeights(this._mainInfantryFormation);
      this._mainInfantryFormation.AI.SetBehaviorWeight<BehaviorDefendCastleKeyPosition>(1f);
    }

    private void HeadOutFromTheCastle()
    {
      if (this._mainInfantryFormation != null)
      {
        this._mainInfantryFormation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(this._mainInfantryFormation);
      }
      GameEntity[] array1 = this._teamAISallyOutAttacker.ArcherPositions.ToArray<GameEntity>();
      if (array1.Length != 0)
      {
        Formation[] array2 = this._archerFormations.ToArray();
        for (int index = 0; index < array2.Length; ++index)
        {
          array2[index].AI.ResetBehaviorWeights();
          TacticComponent.SetDefaultBehaviorWeights(array2[index]);
          array2[index].AI.SetBehaviorWeight<BehaviorShootFromCastleWalls>(1f);
          array2[index].AI.GetBehavior<BehaviorShootFromCastleWalls>().ArcherPosition = array1[index % array1.Length];
        }
      }
      foreach (Formation cavalryFormation in this._cavalryFormations)
      {
        cavalryFormation.AI.ResetBehaviorWeights();
        TacticComponent.SetDefaultBehaviorWeights(cavalryFormation);
        cavalryFormation.AI.SetBehaviorWeight<BehaviorDestroySiegeWeapons>(1f);
      }
    }

    public TacticSallyOutHitAndRun(Team team)
      : base(team)
    {
      this._archerFormations = new List<Formation>();
      this._cavalryFormations = new List<Formation>();
      this._teamAISallyOutAttacker = team.TeamAI as TeamAISallyOutAttacker;
      this._state = TacticSallyOutHitAndRun.TacticState.HeadingOutFromCastle;
      this._destructibleEnemySiegeWeapons = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeWeapon>().Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (sw => sw.Side != team.Side && sw.IsDestructible)).ToList<SiegeWeapon>();
      this.ManageFormationCounts();
      this._AIControlledFormationCount = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
      this.HeadOutFromTheCastle();
    }

    protected override bool CheckAndSetAvailableFormationsChanged()
    {
      int num1 = this.Formations.Count<Formation>((Func<Formation, bool>) (f => f.IsAIControlled));
      int num2 = num1 != this._AIControlledFormationCount ? 1 : 0;
      if (num2 != 0)
        this._AIControlledFormationCount = num1;
      if (num2 != 0 || this._mainInfantryFormation != null && (this._mainInfantryFormation.CountOfUnits == 0 || !this._mainInfantryFormation.QuerySystem.IsInfantryFormation) || this._archerFormations.Any<Formation>() && this._archerFormations.Any<Formation>((Func<Formation, bool>) (af => af.CountOfUnits == 0 || !af.QuerySystem.IsRangedFormation)))
        return true;
      return this._cavalryFormations.Any<Formation>() && this._cavalryFormations.Any<Formation>((Func<Formation, bool>) (cf =>
      {
        if (cf.CountOfUnits == 0)
          return true;
        return !cf.QuerySystem.IsCavalryFormation && !cf.QuerySystem.IsRangedCavalryFormation;
      }));
    }

    private void CheckAndChangeState()
    {
      switch (this._state)
      {
        case TacticSallyOutHitAndRun.TacticState.HeadingOutFromCastle:
          if (!this._cavalryFormations.All<Formation>((Func<Formation, bool>) (cf => !TeamAISiegeComponent.IsFormationInsideCastle(cf, false))))
            break;
          this._state = TacticSallyOutHitAndRun.TacticState.DestroyingSiegeWeapons;
          this.DestroySiegeWeapons();
          break;
        case TacticSallyOutHitAndRun.TacticState.DestroyingSiegeWeapons:
          if (!this._destructibleEnemySiegeWeapons.All<SiegeWeapon>((Func<SiegeWeapon, bool>) (desw => desw.IsDestroyed)) && !this._cavalryFormations.All<Formation>((Func<Formation, bool>) (cf =>
          {
            if (!(cf.AI.ActiveBehavior is BehaviorDestroySiegeWeapons) || cf.MovementOrder == (object) MovementOrder.MovementOrderRetreat)
              return true;
            if ((cf.AI.ActiveBehavior as BehaviorDestroySiegeWeapons).LastTargetWeapon == null)
              return false;
            Vec3 globalPosition = (cf.AI.ActiveBehavior as BehaviorDestroySiegeWeapons).LastTargetWeapon.GameEntity.GlobalPosition;
            return (double) this.team.QuerySystem.GetLocalEnemyPower(globalPosition.AsVec2) > (double) this.team.QuerySystem.GetLocalAllyPower(globalPosition.AsVec2) + (double) cf.QuerySystem.FormationPower;
          })))
            break;
          this._state = TacticSallyOutHitAndRun.TacticState.CavalryRetreating;
          this.CavalryRetreat();
          break;
        case TacticSallyOutHitAndRun.TacticState.CavalryRetreating:
          if (!this._cavalryFormations.IsEmpty<Formation>() && !TeamAISiegeComponent.IsFormationGroupInsideCastle((IEnumerable<Formation>) this._cavalryFormations, false))
            break;
          this._state = TacticSallyOutHitAndRun.TacticState.InfantryRetreating;
          this.InfantryRetreat();
          break;
      }
    }

    protected internal override void TickOccasionally()
    {
      if (!this.AreFormationsCreated)
        return;
      if (this.CheckAndSetAvailableFormationsChanged() || this.IsTacticReapplyNeeded)
      {
        this.ManageFormationCounts();
        switch (this._state)
        {
          case TacticSallyOutHitAndRun.TacticState.HeadingOutFromCastle:
            this.HeadOutFromTheCastle();
            break;
          case TacticSallyOutHitAndRun.TacticState.DestroyingSiegeWeapons:
            this.DestroySiegeWeapons();
            break;
          case TacticSallyOutHitAndRun.TacticState.CavalryRetreating:
            this.CavalryRetreat();
            break;
          case TacticSallyOutHitAndRun.TacticState.InfantryRetreating:
            this.InfantryRetreat();
            break;
        }
        this.IsTacticReapplyNeeded = false;
      }
      this.CheckAndChangeState();
      base.TickOccasionally();
    }

    internal override float GetTacticWeight() => 10f;

    private enum TacticState
    {
      HeadingOutFromCastle,
      DestroyingSiegeWeapons,
      CavalryRetreating,
      InfantryRetreating,
    }
  }
}
