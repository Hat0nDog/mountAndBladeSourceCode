// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorUseSiegeMachines
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorUseSiegeMachines : BehaviorComponent
  {
    private List<UsableMachine> _primarySiegeWeapons;
    private TeamAISiegeComponent _teamAISiegeComponent;
    private MovementOrder _followEntityOrder;
    private MovementOrder _stopOrder;
    private BehaviorUseSiegeMachines.BehaviourState _behaviourState;

    public BehaviorUseSiegeMachines(Formation formation)
      : base(formation)
    {
      this.behaviorSide = formation.AI.Side;
      this._primarySiegeWeapons = Mission.Current.ActiveMissionObjects.FindAllWithType<UsableMachine>().ToList<UsableMachine>();
      this._primarySiegeWeapons.RemoveAll((Predicate<UsableMachine>) (uM => !(uM is IPrimarySiegeWeapon) || (uM as IPrimarySiegeWeapon).WeaponSide != this.behaviorSide));
      this._teamAISiegeComponent = (TeamAISiegeComponent) formation.Team.TeamAI;
      this.BehaviorCoherence = 0.0f;
      this._stopOrder = MovementOrder.MovementOrderStop;
      this.RecreateFollowEntityOrder();
      if (this._followEntityOrder != (object) null)
      {
        this._behaviourState = BehaviorUseSiegeMachines.BehaviourState.Follow;
        this.CurrentOrder = this._followEntityOrder;
      }
      else
      {
        this._behaviourState = BehaviorUseSiegeMachines.BehaviourState.Stop;
        this.CurrentOrder = this._stopOrder;
      }
    }

    private void RecreateFollowEntityOrder()
    {
      this._followEntityOrder = MovementOrder.MovementOrderStop;
      UsableMachine usableMachine = this._primarySiegeWeapons.Where<UsableMachine>((Func<UsableMachine, bool>) (psw => !psw.IsDeactivated && psw is IPrimarySiegeWeapon && !(psw as IPrimarySiegeWeapon).HasCompletedAction())).FirstOrDefault<UsableMachine>();
      if (usableMachine == null)
        return;
      this._followEntityOrder = MovementOrder.MovementOrderFollowEntity(usableMachine.WaitEntity);
    }

    internal override void OnValidBehaviorSideSet()
    {
      base.OnValidBehaviorSideSet();
      this._primarySiegeWeapons = Mission.Current.ActiveMissionObjects.FindAllWithType<UsableMachine>().ToList<UsableMachine>();
      this._primarySiegeWeapons.RemoveAll((Predicate<UsableMachine>) (uM => !(uM is IPrimarySiegeWeapon) || (uM as IPrimarySiegeWeapon).WeaponSide != this.behaviorSide));
      this.RecreateFollowEntityOrder();
      this._behaviourState = BehaviorUseSiegeMachines.BehaviourState.Unset;
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      this._primarySiegeWeapons.RemoveAll((Predicate<UsableMachine>) (psw => psw.IsDestroyed));
      IEnumerable<UsableMachine> source = this._primarySiegeWeapons.Where<UsableMachine>((Func<UsableMachine, bool>) (psw => !psw.IsDeactivated && !(psw as IPrimarySiegeWeapon).HasCompletedAction()));
      if (source.IsEmpty<UsableMachine>())
      {
        this.CurrentOrder = this._stopOrder;
      }
      else
      {
        BehaviorUseSiegeMachines.BehaviourState behaviourState = !source.Any<UsableMachine>((Func<UsableMachine, bool>) (uw => uw is SiegeTower)) || !(source.FirstOrDefault<UsableMachine>((Func<UsableMachine, bool>) (uw => uw is SiegeTower)) as SiegeTower).HasArrivedAtTarget ? (this._followEntityOrder != (object) null ? BehaviorUseSiegeMachines.BehaviourState.Follow : BehaviorUseSiegeMachines.BehaviourState.Stop) : BehaviorUseSiegeMachines.BehaviourState.ClimbSiegeTower;
        if (behaviourState != this._behaviourState)
        {
          switch (behaviourState)
          {
            case BehaviorUseSiegeMachines.BehaviourState.Follow:
              this.CurrentOrder = this._followEntityOrder;
              break;
            case BehaviorUseSiegeMachines.BehaviourState.ClimbSiegeTower:
              this.RecreateFollowEntityOrder();
              this.CurrentOrder = this._followEntityOrder;
              break;
            default:
              this.CurrentOrder = this._stopOrder;
              break;
          }
          this._behaviourState = behaviourState;
          if (this._behaviourState == BehaviorUseSiegeMachines.BehaviourState.ClimbSiegeTower || !this.formation.QuerySystem.HasShield)
            this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
          else
            this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
        }
        if (this.formation.AI.ActiveBehavior == this)
        {
          foreach (UsableMachine usable in source)
          {
            if (!this.formation.IsUsingMachine(usable))
              this.formation.StartUsingMachine(usable);
          }
        }
        this.formation.MovementOrder = this.CurrentOrder;
      }
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.formation.ArrangementOrder = this.formation.QuerySystem.HasShield ? ArrangementOrder.ArrangementOrderShieldWall : ArrangementOrder.ArrangementOrderLine;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight()
    {
      float num = 0.0f;
      if (this._teamAISiegeComponent != null && this._primarySiegeWeapons.Any<UsableMachine>() && this._primarySiegeWeapons.All<UsableMachine>((Func<UsableMachine, bool>) (psw => !(psw as IPrimarySiegeWeapon).HasCompletedAction())))
        num = this._teamAISiegeComponent.IsCastleBreached() ? 0.25f : 0.75f;
      return num;
    }

    private enum BehaviourState
    {
      Unset,
      Follow,
      ClimbSiegeTower,
      Stop,
    }
  }
}
