// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorAssaultWalls
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorAssaultWalls : BehaviorComponent
  {
    private BehaviorAssaultWalls.BehaviorState _behaviourState;
    private List<IPrimarySiegeWeapon> _primarySiegeWeapons;
    private WallSegment _wallSegment;
    private CastleGate _innerGate;
    private TeamAISiegeComponent _teamAISiegeComponent;
    private MovementOrder _attackEntityOrderInnerGate;
    private MovementOrder _attackEntityOrderOuterGate;
    private MovementOrder _chargeOrder;
    private MovementOrder _stopOrder;
    private MovementOrder _castleGateMoveOrder;
    private MovementOrder _wallSegmentMoveOrder;
    private FacingOrder _facingOrder;
    protected ArrangementOrder CurrentArrangementOrder;
    private bool _isGateLane;

    private void ResetOrderPositions()
    {
      this._primarySiegeWeapons = Mission.Current.ActiveMissionObjects.FindAllWithType<UsableMachine>().Where<UsableMachine>((Func<UsableMachine, bool>) (amo => amo is IPrimarySiegeWeapon)).Select<UsableMachine, IPrimarySiegeWeapon>((Func<UsableMachine, IPrimarySiegeWeapon>) (amo => amo as IPrimarySiegeWeapon)).ToList<IPrimarySiegeWeapon>();
      this._primarySiegeWeapons.RemoveAll((Predicate<IPrimarySiegeWeapon>) (uM => uM.WeaponSide != this.behaviorSide));
      IEnumerable<ICastleKeyPosition> source = TeamAISiegeComponent.SiegeLanes.Where<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == this.behaviorSide)).SelectMany<SiegeLane, ICastleKeyPosition>((Func<SiegeLane, IEnumerable<ICastleKeyPosition>>) (sila => (IEnumerable<ICastleKeyPosition>) sila.DefensePoints));
      this._innerGate = this._teamAISiegeComponent.InnerGate;
      this._isGateLane = this._teamAISiegeComponent.OuterGate.DefenseSide == this.behaviorSide;
      this._wallSegment = !this._isGateLane ? (!(source.FirstOrDefault<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is WallSegment && (dp as WallSegment).IsBreachedWall)) is WallSegment wallSegment) ? this._primarySiegeWeapons.MaxBy<IPrimarySiegeWeapon, float>((Func<IPrimarySiegeWeapon, float>) (psw => psw.SiegeWeaponPriority)).TargetCastlePosition as WallSegment : wallSegment) : (WallSegment) null;
      this._stopOrder = MovementOrder.MovementOrderStop;
      this._chargeOrder = MovementOrder.MovementOrderCharge;
      bool flag = this._teamAISiegeComponent.OuterGate != null && this.behaviorSide == this._teamAISiegeComponent.OuterGate.DefenseSide;
      this._attackEntityOrderOuterGate = !flag || this._teamAISiegeComponent.OuterGate.IsDeactivated || this._teamAISiegeComponent.OuterGate.State == CastleGate.GateState.Open ? MovementOrder.MovementOrderStop : MovementOrder.MovementOrderAttackEntity(this._teamAISiegeComponent.OuterGate.GameEntity, false);
      this._attackEntityOrderInnerGate = !flag || this._teamAISiegeComponent.InnerGate == null || (this._teamAISiegeComponent.InnerGate.IsDeactivated || this._teamAISiegeComponent.InnerGate.State == CastleGate.GateState.Open) ? MovementOrder.MovementOrderStop : MovementOrder.MovementOrderAttackEntity(this._teamAISiegeComponent.InnerGate.GameEntity, false);
      this._castleGateMoveOrder = MovementOrder.MovementOrderMove(this._teamAISiegeComponent.OuterGate.MiddleFrame.Origin);
      this._wallSegmentMoveOrder = !this._isGateLane ? MovementOrder.MovementOrderMove(this._wallSegment.MiddleFrame.Origin) : this._castleGateMoveOrder;
      this._facingOrder = FacingOrder.FacingOrderLookAtEnemy;
    }

    public BehaviorAssaultWalls(Formation formation)
      : base(formation)
    {
      this.BehaviorCoherence = 0.0f;
      this.behaviorSide = formation.AI.Side;
      this._teamAISiegeComponent = (TeamAISiegeComponent) formation.Team.TeamAI;
      this._behaviourState = BehaviorAssaultWalls.BehaviorState.Deciding;
      this.ResetOrderPositions();
      this.CurrentOrder = this._stopOrder;
    }

    private BehaviorAssaultWalls.BehaviorState CheckAndChangeState()
    {
      switch (this._behaviourState)
      {
        case BehaviorAssaultWalls.BehaviorState.Deciding:
          if (!this._isGateLane && this._wallSegment == null)
            return BehaviorAssaultWalls.BehaviorState.Charging;
          return this._isGateLane ? BehaviorAssaultWalls.BehaviorState.AttackEntity : BehaviorAssaultWalls.BehaviorState.ClimbWall;
        case BehaviorAssaultWalls.BehaviorState.ClimbWall:
          if (this._wallSegment == null)
            return BehaviorAssaultWalls.BehaviorState.Charging;
          bool flag = false;
          if (this.behaviorSide < FormationAI.BehaviorSide.BehaviorSideNotSet)
          {
            SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes[(int) this.behaviorSide];
            flag = siegeLane.IsUnderAttack() && !siegeLane.IsDefended();
          }
          return flag | (double) this.formation.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(this._wallSegment.MiddleFrame.Origin.GetNavMeshVec3()) < (double) this.formation.Depth * (double) this.formation.Depth ? BehaviorAssaultWalls.BehaviorState.TakeControl : BehaviorAssaultWalls.BehaviorState.ClimbWall;
        case BehaviorAssaultWalls.BehaviorState.AttackEntity:
          return this._teamAISiegeComponent.OuterGate.IsGateOpen && this._teamAISiegeComponent.InnerGate.IsGateOpen ? BehaviorAssaultWalls.BehaviorState.Charging : BehaviorAssaultWalls.BehaviorState.AttackEntity;
        case BehaviorAssaultWalls.BehaviorState.TakeControl:
          if (this.formation.QuerySystem.ClosestEnemyFormation == null)
            return BehaviorAssaultWalls.BehaviorState.Deciding;
          if (TeamAISiegeComponent.SiegeLanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == this.behaviorSide)).IsDefended())
            return BehaviorAssaultWalls.BehaviorState.TakeControl;
          return !this._teamAISiegeComponent.OuterGate.IsGateOpen || !this._teamAISiegeComponent.InnerGate.IsGateOpen ? BehaviorAssaultWalls.BehaviorState.MoveToGate : BehaviorAssaultWalls.BehaviorState.Charging;
        case BehaviorAssaultWalls.BehaviorState.MoveToGate:
          return this._teamAISiegeComponent.OuterGate.IsGateOpen && this._teamAISiegeComponent.InnerGate.IsGateOpen ? BehaviorAssaultWalls.BehaviorState.Charging : BehaviorAssaultWalls.BehaviorState.MoveToGate;
        case BehaviorAssaultWalls.BehaviorState.Charging:
          if (!TeamAISiegeComponent.IsFormationInsideCastle(this.formation, true))
            return BehaviorAssaultWalls.BehaviorState.Deciding;
          return this.formation.QuerySystem.ClosestEnemyFormation == null ? BehaviorAssaultWalls.BehaviorState.Stop : BehaviorAssaultWalls.BehaviorState.Charging;
        default:
          return this.formation.QuerySystem.ClosestEnemyFormation != null ? BehaviorAssaultWalls.BehaviorState.Deciding : BehaviorAssaultWalls.BehaviorState.Stop;
      }
    }

    protected override void CalculateCurrentOrder()
    {
      switch (this._behaviourState)
      {
        case BehaviorAssaultWalls.BehaviorState.Deciding:
          this.CurrentOrder = this._stopOrder;
          break;
        case BehaviorAssaultWalls.BehaviorState.ClimbWall:
          this.CurrentOrder = this._wallSegmentMoveOrder;
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(-this._wallSegment.MiddleFrame.Rotation.f.AsVec2.Normalized());
          this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLine;
          break;
        case BehaviorAssaultWalls.BehaviorState.AttackEntity:
          if (!this._teamAISiegeComponent.OuterGate.IsGateOpen)
            this.CurrentOrder = this._attackEntityOrderOuterGate;
          else
            this.CurrentOrder = this._attackEntityOrderInnerGate;
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
          this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLine;
          break;
        case BehaviorAssaultWalls.BehaviorState.TakeControl:
          if (this.formation.QuerySystem.ClosestEnemyFormation != null)
            this.CurrentOrder = MovementOrder.MovementOrderChargeToTarget(this.formation.QuerySystem.ClosestEnemyFormation.Formation);
          else
            this.CurrentOrder = MovementOrder.MovementOrderCharge;
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(-this._wallSegment.MiddleFrame.Rotation.f.AsVec2.Normalized());
          this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLine;
          break;
        case BehaviorAssaultWalls.BehaviorState.MoveToGate:
          this.CurrentOrder = this._castleGateMoveOrder;
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(-this._innerGate.MiddleFrame.Rotation.f.AsVec2.Normalized());
          this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLine;
          break;
        case BehaviorAssaultWalls.BehaviorState.Charging:
          this.CurrentOrder = this._chargeOrder;
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
          this.CurrentArrangementOrder = ArrangementOrder.ArrangementOrderLoose;
          break;
        case BehaviorAssaultWalls.BehaviorState.Stop:
          this.CurrentOrder = this._stopOrder;
          break;
      }
    }

    internal override void OnValidBehaviorSideSet()
    {
      base.OnValidBehaviorSideSet();
      this.ResetOrderPositions();
      this._behaviourState = BehaviorAssaultWalls.BehaviorState.Deciding;
    }

    protected internal override void TickOccasionally()
    {
      this._behaviourState = this.CheckAndChangeState();
      this.CalculateCurrentOrder();
      foreach (IPrimarySiegeWeapon primarySiegeWeapon in this._primarySiegeWeapons)
      {
        UsableMachine usable = primarySiegeWeapon as UsableMachine;
        if (!usable.IsDeactivated && !primarySiegeWeapon.HasCompletedAction() && !this.formation.IsUsingMachine(usable))
          this.formation.StartUsingMachine(primarySiegeWeapon as UsableMachine);
      }
      if (this._behaviourState == BehaviorAssaultWalls.BehaviorState.MoveToGate)
      {
        CastleGate innerGate = this._teamAISiegeComponent.InnerGate;
        if (innerGate != null && !innerGate.IsGateOpen && !innerGate.IsDestroyed)
        {
          if (!this.formation.IsUsingMachine((UsableMachine) innerGate))
            this.formation.StartUsingMachine((UsableMachine) innerGate);
        }
        else
        {
          CastleGate outerGate = this._teamAISiegeComponent.OuterGate;
          if (outerGate != null && !outerGate.IsGateOpen && (!outerGate.IsDestroyed && !this.formation.IsUsingMachine((UsableMachine) outerGate)))
            this.formation.StartUsingMachine((UsableMachine) outerGate);
        }
      }
      else
      {
        if (this.formation.Detachments.Contains((IDetachment) this._teamAISiegeComponent.OuterGate))
          this.formation.StopUsingMachine((UsableMachine) this._teamAISiegeComponent.OuterGate);
        if (this.formation.Detachments.Contains((IDetachment) this._teamAISiegeComponent.InnerGate))
          this.formation.StopUsingMachine((UsableMachine) this._teamAISiegeComponent.InnerGate);
      }
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = this.CurrentArrangementOrder;
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderDeep;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight()
    {
      float num = 0.0f;
      if (this._teamAISiegeComponent != null)
      {
        if (this._primarySiegeWeapons.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw.HasCompletedAction())) || this._wallSegment != null)
          num = !this._teamAISiegeComponent.IsCastleBreached() ? 0.25f : 0.75f;
        else if (this._teamAISiegeComponent.OuterGate.DefenseSide == this.behaviorSide)
          num = 0.1f;
      }
      return num;
    }

    private enum BehaviorState
    {
      Deciding,
      ClimbWall,
      AttackEntity,
      TakeControl,
      MoveToGate,
      Charging,
      Stop,
    }
  }
}
