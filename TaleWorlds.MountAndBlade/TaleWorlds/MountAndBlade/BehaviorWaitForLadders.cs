// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorWaitForLadders
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorWaitForLadders : BehaviorComponent
  {
    private List<SiegeLadder> _ladders;
    private WallSegment _breachedWallSegment;
    private TeamAISiegeComponent _teamAISiegeComponent;
    private MovementOrder _stopOrder;
    private MovementOrder _followOrder;
    private BehaviorWaitForLadders.BehaviourState _behaviourState;
    private const string WallWaitPositionTag = "attacker_wait_pos";
    private GameEntity _followedEntity;
    private TacticalPosition _followTacticalPosition;

    public BehaviorWaitForLadders(Formation formation)
      : base(formation)
    {
      this.behaviorSide = formation.AI.Side;
      this._ladders = Mission.Current.ActiveMissionObjects.Where<MissionObject>((Func<MissionObject, bool>) (amo => amo is SiegeLadder)).Select<MissionObject, SiegeLadder>((Func<MissionObject, SiegeLadder>) (amo => amo as SiegeLadder)).ToList<SiegeLadder>();
      this._ladders.RemoveAll((Predicate<SiegeLadder>) (l => l.IsDeactivated || l.WeaponSide != this.behaviorSide));
      this._teamAISiegeComponent = (TeamAISiegeComponent) formation.Team.TeamAI;
      SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == this.behaviorSide));
      this._breachedWallSegment = (siegeLane != null ? siegeLane.DefensePoints.FirstOrDefault<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is WallSegment && (dp as WallSegment).IsBreachedWall)) : (ICastleKeyPosition) null) as WallSegment;
      this.ResetFollowOrder();
      this._stopOrder = MovementOrder.MovementOrderStop;
      if (this._followOrder != (object) null)
      {
        this.CurrentOrder = this._followOrder;
        this._behaviourState = BehaviorWaitForLadders.BehaviourState.Follow;
      }
      else
      {
        this.CurrentOrder = this._stopOrder;
        this._behaviourState = BehaviorWaitForLadders.BehaviourState.Stop;
      }
    }

    private void ResetFollowOrder()
    {
      this._followedEntity = (GameEntity) null;
      this._followTacticalPosition = (TacticalPosition) null;
      if (this._ladders.Any<SiegeLadder>())
      {
        this._followedEntity = (this._ladders.FirstOrDefault<SiegeLadder>((Func<SiegeLadder, bool>) (l => !l.IsDeactivated && l.InitialWaitPosition.HasScriptOfType<TacticalPosition>())) ?? this._ladders.FirstOrDefault<SiegeLadder>((Func<SiegeLadder, bool>) (l => !l.IsDeactivated))).InitialWaitPosition;
        if ((NativeObject) this._followedEntity == (NativeObject) null)
          this._followedEntity = this._ladders.FirstOrDefault<SiegeLadder>((Func<SiegeLadder, bool>) (l => !l.IsDeactivated)).InitialWaitPosition;
        this._followOrder = MovementOrder.MovementOrderFollowEntity(this._followedEntity);
      }
      else if (this._breachedWallSegment != null)
      {
        this._followedEntity = this._breachedWallSegment.GameEntity.CollectChildrenEntitiesWithTag("attacker_wait_pos").FirstOrDefault<GameEntity>();
        this._followOrder = MovementOrder.MovementOrderFollowEntity(this._followedEntity);
      }
      else
        this._followOrder = MovementOrder.MovermentOrderNull;
      if (!((NativeObject) this._followedEntity != (NativeObject) null))
        return;
      this._followTacticalPosition = this._followedEntity.GetFirstScriptOfType<TacticalPosition>();
    }

    internal override void OnValidBehaviorSideSet()
    {
      base.OnValidBehaviorSideSet();
      this._ladders = Mission.Current.ActiveMissionObjects.Where<MissionObject>((Func<MissionObject, bool>) (amo => amo is SiegeLadder)).Select<MissionObject, SiegeLadder>((Func<MissionObject, SiegeLadder>) (amo => amo as SiegeLadder)).ToList<SiegeLadder>();
      this._ladders.RemoveAll((Predicate<SiegeLadder>) (l => l.IsDeactivated || l.WeaponSide != this.behaviorSide));
      SiegeLane siegeLane = TeamAISiegeComponent.SiegeLanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (sl => sl.LaneSide == this.behaviorSide));
      this._breachedWallSegment = (siegeLane != null ? siegeLane.DefensePoints.FirstOrDefault<ICastleKeyPosition>((Func<ICastleKeyPosition, bool>) (dp => dp is WallSegment && (dp as WallSegment).IsBreachedWall)) : (ICastleKeyPosition) null) as WallSegment;
      this.ResetFollowOrder();
      this._behaviourState = BehaviorWaitForLadders.BehaviourState.Unset;
    }

    protected override void CalculateCurrentOrder()
    {
      BehaviorWaitForLadders.BehaviourState behaviourState = this._followOrder != (object) null ? BehaviorWaitForLadders.BehaviourState.Follow : BehaviorWaitForLadders.BehaviourState.Stop;
      if (behaviourState == this._behaviourState)
        return;
      if (behaviourState == BehaviorWaitForLadders.BehaviourState.Follow)
      {
        this.CurrentOrder = this._followOrder;
        if (this._followTacticalPosition != null)
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtDirection(this._followTacticalPosition.Direction);
        else
          this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      else
      {
        this.CurrentOrder = this._stopOrder;
        this.CurrentFacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      }
      this._behaviourState = behaviourState;
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      this._ladders.RemoveAll((Predicate<SiegeLadder>) (psw => psw.IsDestroyed));
      this.formation.MovementOrder = this.CurrentOrder;
      this.formation.FacingOrder = this.CurrentFacingOrder;
      if (this._behaviourState != BehaviorWaitForLadders.BehaviourState.Follow || this._followTacticalPosition == null)
        return;
      this.formation.FormOrder = FormOrder.FormOrderCustom(this._followTacticalPosition.Width);
    }

    protected override void OnBehaviorActivatedAux()
    {
      this.formation.ArrangementOrder = this.formation.QuerySystem.HasShield ? ArrangementOrder.ArrangementOrderShieldWall : ArrangementOrder.ArrangementOrderLine;
      this.formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;
      this.formation.FiringOrder = FiringOrder.FiringOrderFireAtWill;
      this.formation.FormOrder = FormOrder.FormOrderWide;
      this.formation.WeaponUsageOrder = WeaponUsageOrder.WeaponUsageOrderUseAny;
    }

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight()
    {
      float num = 0.0f;
      if (this._followOrder != (object) null && !this._teamAISiegeComponent.AreLaddersReady)
        num = this._teamAISiegeComponent.IsCastleBreached() ? 0.5f : 1f;
      return num;
    }

    private enum BehaviourState
    {
      Unset,
      Stop,
      Follow,
    }
  }
}
