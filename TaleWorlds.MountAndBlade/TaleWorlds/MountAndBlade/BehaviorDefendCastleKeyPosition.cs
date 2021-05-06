// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorDefendCastleKeyPosition
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
  public class BehaviorDefendCastleKeyPosition : BehaviorComponent
  {
    private List<IPointDefendable> _castleKeyPositions = new List<IPointDefendable>();
    private TeamAISiegeComponent _teamAISiegeDefender;
    private CastleGate _innerGate;
    private CastleGate _outerGate;
    private List<SiegeLadder> _laddersOnThisSide;
    private BehaviorDefendCastleKeyPosition.BehaviourState _behaviourState;
    private MovementOrder _waitOrder;
    private MovementOrder _readyOrder;
    private FacingOrder _waitFacingOrder;
    private FacingOrder _readyFacingOrder;
    private TacticalPosition _tacticalMiddlePos;
    private TacticalPosition _tacticalWaitPos;
    private bool _isDefendingWideGap;
    private bool _isInShieldWallDistance;

    public BehaviorDefendCastleKeyPosition(Formation formation)
      : base(formation)
    {
      this._teamAISiegeDefender = formation.Team.TeamAI as TeamAISiegeComponent;
      this._behaviourState = BehaviorDefendCastleKeyPosition.BehaviourState.UnSet;
      this._laddersOnThisSide = new List<SiegeLadder>();
      this.ResetOrderPositions();
    }

    protected override void CalculateCurrentOrder()
    {
      base.CalculateCurrentOrder();
      this.CurrentOrder = this._behaviourState == BehaviorDefendCastleKeyPosition.BehaviourState.Ready ? this._readyOrder : this._waitOrder;
      this.CurrentFacingOrder = this.formation.QuerySystem.ClosestEnemyFormation == null || !TeamAISiegeComponent.IsFormationInsideCastle(this.formation.QuerySystem.ClosestEnemyFormation.Formation, true) ? (this._behaviourState == BehaviorDefendCastleKeyPosition.BehaviourState.Ready ? this._readyFacingOrder : this._waitFacingOrder) : FacingOrder.FacingOrderLookAtEnemy;
    }

    private void ResetOrderPositions()
    {
      this.behaviorSide = this.formation.AI.Side;
      this._innerGate = (CastleGate) null;
      this._outerGate = (CastleGate) null;
      this._laddersOnThisSide.Clear();
      int num1 = Mission.Current.ActiveMissionObjects.FindAllWithType<CastleGate>().Any<CastleGate>((Func<CastleGate, bool>) (cg => cg.DefenseSide == this.behaviorSide && cg.GameEntity.HasTag("outer_gate"))) ? 1 : 0;
      this._isDefendingWideGap = false;
      WorldFrame worldFrame1;
      WorldFrame worldFrame2;
      if (num1 != 0)
      {
        CastleGate outerGate = this._teamAISiegeDefender.OuterGate;
        this._innerGate = this._teamAISiegeDefender.InnerGate;
        this._outerGate = this._teamAISiegeDefender.OuterGate;
        worldFrame1 = outerGate.MiddleFrame;
        worldFrame2 = outerGate.DefenseWaitFrame;
        this._tacticalMiddlePos = outerGate.MiddlePosition;
        this._tacticalWaitPos = outerGate.WaitPosition;
        this._isDefendingWideGap = false;
      }
      else
      {
        WallSegment wallSegment = Mission.Current.ActiveMissionObjects.FindAllWithType<WallSegment>().Where<WallSegment>((Func<WallSegment, bool>) (ws => ws.DefenseSide == this.behaviorSide && ws.IsBreachedWall)).FirstOrDefault<WallSegment>();
        if (wallSegment != null)
        {
          worldFrame1 = wallSegment.MiddleFrame;
          worldFrame2 = wallSegment.DefenseWaitFrame;
          this._tacticalMiddlePos = wallSegment.MiddlePosition;
          this._tacticalWaitPos = wallSegment.WaitPosition;
          this._isDefendingWideGap = false;
        }
        else
        {
          IEnumerable<SiegeWeapon> source = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeWeapon>().Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (sw =>
          {
            if (!(sw is IPrimarySiegeWeapon) || (sw as IPrimarySiegeWeapon).WeaponSide != this.behaviorSide)
              return false;
            return !sw.IsDestroyed || (sw as IPrimarySiegeWeapon).HasCompletedAction();
          }));
          if (!source.Any<SiegeWeapon>())
          {
            worldFrame1 = WorldFrame.Invalid;
            worldFrame2 = WorldFrame.Invalid;
            this._tacticalMiddlePos = (TacticalPosition) null;
            this._tacticalWaitPos = (TacticalPosition) null;
          }
          else
          {
            this._laddersOnThisSide = source.Where<SiegeWeapon>((Func<SiegeWeapon, bool>) (aw => aw is SiegeLadder)).Select<SiegeWeapon, SiegeLadder>((Func<SiegeWeapon, SiegeLadder>) (aw => aw as SiegeLadder)).ToList<SiegeLadder>();
            ICastleKeyPosition targetCastlePosition = (source.FirstOrDefault<SiegeWeapon>() as IPrimarySiegeWeapon).TargetCastlePosition as ICastleKeyPosition;
            worldFrame1 = targetCastlePosition.MiddleFrame;
            worldFrame2 = targetCastlePosition.DefenseWaitFrame;
            this._tacticalMiddlePos = targetCastlePosition.MiddlePosition;
            this._tacticalWaitPos = targetCastlePosition.WaitPosition;
          }
        }
      }
      if (this._tacticalMiddlePos != null)
      {
        this._readyOrder = MovementOrder.MovementOrderMove(this._tacticalMiddlePos.Position);
        this._readyFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._tacticalMiddlePos.Direction);
      }
      else if (worldFrame1.Origin.IsValid)
      {
        double num2 = (double) worldFrame1.Rotation.f.Normalize();
        this._readyOrder = MovementOrder.MovementOrderMove(worldFrame1.Origin);
        this._readyFacingOrder = FacingOrder.FacingOrderLookAtDirection(worldFrame1.Rotation.f.AsVec2);
      }
      else
      {
        this._readyOrder = MovementOrder.MovementOrderStop;
        this._readyFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      if (this._tacticalWaitPos != null)
      {
        this._waitOrder = MovementOrder.MovementOrderMove(this._tacticalWaitPos.Position);
        this._waitFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._tacticalWaitPos.Direction);
      }
      else if (worldFrame2.Origin.IsValid)
      {
        double num2 = (double) worldFrame2.Rotation.f.Normalize();
        this._waitOrder = MovementOrder.MovementOrderMove(worldFrame2.Origin);
        this._waitFacingOrder = FacingOrder.FacingOrderLookAtDirection(worldFrame2.Rotation.f.AsVec2);
      }
      else
      {
        this._waitOrder = MovementOrder.MovementOrderStop;
        this._waitFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      this.CurrentOrder = this._behaviourState == BehaviorDefendCastleKeyPosition.BehaviourState.Ready ? this._readyOrder : this._waitOrder;
      this.CurrentFacingOrder = this.formation.QuerySystem.ClosestEnemyFormation == null || !TeamAISiegeComponent.IsFormationInsideCastle(this.formation.QuerySystem.ClosestEnemyFormation.Formation, true) ? (this._behaviourState == BehaviorDefendCastleKeyPosition.BehaviourState.Ready ? this._readyFacingOrder : this._waitFacingOrder) : FacingOrder.FacingOrderLookAtEnemy;
    }

    internal override void OnValidBehaviorSideSet()
    {
      base.OnValidBehaviorSideSet();
      this.ResetOrderPositions();
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      BehaviorDefendCastleKeyPosition.BehaviourState behaviourState = (this._teamAISiegeDefender == null ? 0 : (TeamAISiegeComponent.SiegeLanes.Any<SiegeLane>((Func<SiegeLane, bool>) (sl =>
      {
        if (sl.LaneSide != this.behaviorSide)
          return false;
        return sl.IsOpen || sl.PrimarySiegeWeapons.Any<IPrimarySiegeWeapon>((Func<IPrimarySiegeWeapon, bool>) (psw => psw is SiegeWeapon && (psw as SiegeWeapon).GetComponent<SiegeWeaponMovementComponent>() != null && (psw as SiegeWeapon).GetComponent<SiegeWeaponMovementComponent>().HasArrivedAtTarget));
      })) ? 1 : 0)) != 0 ? BehaviorDefendCastleKeyPosition.BehaviourState.Ready : BehaviorDefendCastleKeyPosition.BehaviourState.Waiting;
      if (behaviourState != this._behaviourState)
      {
        this._behaviourState = behaviourState;
        this.CurrentOrder = this._behaviourState == BehaviorDefendCastleKeyPosition.BehaviourState.Ready ? this._readyOrder : this._waitOrder;
        this.CurrentFacingOrder = this._behaviourState == BehaviorDefendCastleKeyPosition.BehaviourState.Ready ? this._readyFacingOrder : this._waitFacingOrder;
      }
      if (Mission.Current.MissionTeamAIType == Mission.MissionTeamAITypeEnum.Siege)
      {
        if (this._outerGate != null && this._outerGate.State == CastleGate.GateState.Open && !this._outerGate.IsDestroyed)
        {
          if (!this.formation.IsUsingMachine((UsableMachine) this._outerGate))
            this.formation.StartUsingMachine((UsableMachine) this._outerGate);
        }
        else if (this._innerGate != null && this._innerGate.State == CastleGate.GateState.Open && (!this._innerGate.IsDestroyed && !this.formation.IsUsingMachine((UsableMachine) this._innerGate)))
          this.formation.StartUsingMachine((UsableMachine) this._innerGate);
        foreach (SiegeLadder siegeLadder in this._laddersOnThisSide)
        {
          if (!siegeLadder.IsDisabledForBattleSide(BattleSideEnum.Defender) && !this.formation.IsUsingMachine((UsableMachine) siegeLadder))
            this.formation.StartUsingMachine((UsableMachine) siegeLadder);
        }
      }
      this.CalculateCurrentOrder();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this._behaviourState == BehaviorDefendCastleKeyPosition.BehaviourState.Ready && this._tacticalMiddlePos != null)
        this.formation.FormOrder = FormOrder.FormOrderCustom(this._tacticalMiddlePos.Width);
      else if (this._behaviourState == BehaviorDefendCastleKeyPosition.BehaviourState.Waiting && this._tacticalWaitPos != null)
        this.formation.FormOrder = FormOrder.FormOrderCustom(this._tacticalWaitPos.Width);
      bool flag = this._isDefendingWideGap && this._behaviourState == BehaviorDefendCastleKeyPosition.BehaviourState.Ready && this.formation.QuerySystem.ClosestEnemyFormation != null && (this.formation.QuerySystem.IsUnderRangedAttack || (double) this.formation.QuerySystem.AveragePosition.DistanceSquared(this.CurrentOrder.GetPosition(this.formation).AsVec2) < 25.0 + (this._isInShieldWallDistance ? 75.0 : 0.0));
      if (flag == this._isInShieldWallDistance)
        return;
      this._isInShieldWallDistance = flag;
      if (this._isInShieldWallDistance && this.formation.QuerySystem.HasShield)
      {
        if (!(this.formation.ArrangementOrder != ArrangementOrder.ArrangementOrderShieldWall))
          return;
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderShieldWall;
      }
      else
      {
        if (!(this.formation.ArrangementOrder == ArrangementOrder.ArrangementOrderLine))
          return;
        this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      }
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.ResetOrderPositions();
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      this.formation.ArrangementOrder = ArrangementOrder.ArrangementOrderLine;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight() => 1f;

    private enum BehaviourState
    {
      UnSet,
      Waiting,
      Ready,
    }
  }
}
