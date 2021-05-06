// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MovementOrder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public struct MovementOrder
  {
    private MovementOrder.FollowState _followState;
    private float _departStartTime;
    internal readonly MovementOrder.MovementOrderEnum OrderEnum;
    private Func<Formation, WorldPosition> _positionLambda;
    private WorldPosition _position;
    private WorldPosition _getPositionResultCache;
    private bool _getPositionIsNavmeshlessCache;
    private WorldPosition _getPositionFirstSectionCache;
    public Formation TargetFormation;
    public GameEntity TargetEntity;
    private bool _surroundEntity;
    private List<Agent> _scriptedAgents;
    private const int ScriptedAgentCount = 8;
    private readonly Timer _tickTimer;
    private readonly Timer _attackEntityReassignTimer;
    private WorldPosition _lastPosition;
    public readonly bool _isFacingDirection;
    public static readonly MovementOrder MovermentOrderNull = new MovementOrder(MovementOrder.MovementOrderEnum.Invalid);
    public static readonly MovementOrder MovementOrderCharge = new MovementOrder(MovementOrder.MovementOrderEnum.Charge);
    public static readonly MovementOrder MovementOrderRetreat = new MovementOrder(MovementOrder.MovementOrderEnum.Retreat);
    public static readonly MovementOrder MovementOrderStop = new MovementOrder(MovementOrder.MovementOrderEnum.Stop);
    public static readonly MovementOrder MovementOrderAdvance = new MovementOrder(MovementOrder.MovementOrderEnum.Advance);
    public static readonly MovementOrder MovementOrderFallBack = new MovementOrder(MovementOrder.MovementOrderEnum.FallBack);

    public Agent _targetAgent { get; }

    private MovementOrder(MovementOrder.MovementOrderEnum orderEnum)
    {
      this.OrderEnum = orderEnum;
      switch (orderEnum)
      {
        case MovementOrder.MovementOrderEnum.Charge:
          this._positionLambda = (Func<Formation, WorldPosition>) null;
          break;
        case MovementOrder.MovementOrderEnum.Retreat:
          this._positionLambda = (Func<Formation, WorldPosition>) null;
          break;
        case MovementOrder.MovementOrderEnum.Advance:
          this._positionLambda = (Func<Formation, WorldPosition>) null;
          break;
        case MovementOrder.MovementOrderEnum.FallBack:
          this._positionLambda = (Func<Formation, WorldPosition>) null;
          break;
        default:
          this._positionLambda = (Func<Formation, WorldPosition>) null;
          break;
      }
      this.TargetFormation = (Formation) null;
      this.TargetEntity = (GameEntity) null;
      this._surroundEntity = false;
      this._targetAgent = (Agent) null;
      this._scriptedAgents = (List<Agent>) null;
      this._tickTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.5f);
      this._attackEntityReassignTimer = (Timer) null;
      this._lastPosition = WorldPosition.Invalid;
      this._isFacingDirection = false;
      this._position = WorldPosition.Invalid;
      this._getPositionResultCache = WorldPosition.Invalid;
      this._getPositionFirstSectionCache = WorldPosition.Invalid;
      this._getPositionIsNavmeshlessCache = false;
      this._followState = MovementOrder.FollowState.Stop;
      this._departStartTime = -1f;
    }

    public static MovementOrder MovementOrderAttach(
      Formation targetFormation,
      MovementOrder.Side side)
    {
      return new MovementOrder(MovementOrder.MovementOrderEnum.Attach, targetFormation, side);
    }

    private MovementOrder(
      MovementOrder.MovementOrderEnum orderEnum,
      Formation targetFormation,
      MovementOrder.Side side)
    {
      this.OrderEnum = orderEnum;
      this._positionLambda = (Func<Formation, WorldPosition>) (f => MovementOrder.GetAttachPosition(f, targetFormation, side));
      this.TargetFormation = targetFormation;
      this.TargetEntity = (GameEntity) null;
      this._surroundEntity = false;
      this._targetAgent = (Agent) null;
      this._scriptedAgents = (List<Agent>) null;
      this._tickTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.5f);
      this._attackEntityReassignTimer = (Timer) null;
      this._lastPosition = WorldPosition.Invalid;
      this._isFacingDirection = false;
      this._position = WorldPosition.Invalid;
      this._getPositionResultCache = WorldPosition.Invalid;
      this._getPositionFirstSectionCache = WorldPosition.Invalid;
      this._getPositionIsNavmeshlessCache = false;
      this._followState = MovementOrder.FollowState.Stop;
      this._departStartTime = -1f;
    }

    public static MovementOrder MovementOrderAttackEntity(
      GameEntity targetEntity,
      bool surroundEntity)
    {
      return new MovementOrder(MovementOrder.MovementOrderEnum.AttackEntity, targetEntity, surroundEntity);
    }

    private MovementOrder(
      MovementOrder.MovementOrderEnum orderEnum,
      GameEntity targetEntity,
      bool surroundEntity)
    {
      targetEntity.GetFirstScriptOfType<UsableMachine>();
      this.OrderEnum = orderEnum;
      this._positionLambda = (Func<Formation, WorldPosition>) (f =>
      {
        WorldPosition worldPosition = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, targetEntity.GlobalPosition, false);
        Vec2 vec2_1 = f.QuerySystem.AveragePosition - worldPosition.AsVec2;
        Vec2 v = targetEntity.GetGlobalFrame().rotation.f.AsVec2.Normalized();
        Vec2 vec2_2 = (double) vec2_1.DotProduct(v) >= 0.0 ? v : -v;
        WorldPosition position1 = worldPosition;
        position1.SetVec2(worldPosition.AsVec2 + vec2_2 * 3f);
        if (Mission.Current.Scene.DoesPathExistBetweenPositions(position1, f.QuerySystem.MedianPosition))
          return position1;
        WorldPosition position2 = worldPosition;
        position2.SetVec2(worldPosition.AsVec2 - vec2_2 * 3f);
        if (Mission.Current.Scene.DoesPathExistBetweenPositions(position2, f.QuerySystem.MedianPosition))
          return position2;
        WorldPosition position3 = worldPosition;
        position3.SetVec2(worldPosition.AsVec2 + targetEntity.GetGlobalFrame().rotation.s.AsVec2.Normalized() * 3f);
        if (Mission.Current.Scene.DoesPathExistBetweenPositions(position3, f.QuerySystem.MedianPosition))
          return position3;
        WorldPosition position4 = worldPosition;
        position4.SetVec2(worldPosition.AsVec2 - targetEntity.GetGlobalFrame().rotation.s.AsVec2.Normalized() * 3f);
        return Mission.Current.Scene.DoesPathExistBetweenPositions(position4, f.QuerySystem.MedianPosition) ? position4 : position1;
      });
      this.TargetEntity = targetEntity;
      this._surroundEntity = surroundEntity;
      this._scriptedAgents = new List<Agent>();
      this._tickTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.5f);
      this._attackEntityReassignTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 3f);
      this.TargetFormation = (Formation) null;
      this._targetAgent = (Agent) null;
      this._lastPosition = WorldPosition.Invalid;
      this._isFacingDirection = false;
      this._position = WorldPosition.Invalid;
      this._getPositionResultCache = WorldPosition.Invalid;
      this._getPositionFirstSectionCache = WorldPosition.Invalid;
      this._getPositionIsNavmeshlessCache = false;
      this._followState = MovementOrder.FollowState.Stop;
      this._departStartTime = -1f;
    }

    public static MovementOrder MovementOrderChargeToTarget(Formation targetFormation) => new MovementOrder(MovementOrder.MovementOrderEnum.ChargeToTarget, targetFormation);

    private MovementOrder(MovementOrder.MovementOrderEnum orderEnum, Formation targetFormation)
    {
      this.OrderEnum = orderEnum;
      this._positionLambda = (Func<Formation, WorldPosition>) null;
      this.TargetFormation = targetFormation;
      this.TargetEntity = (GameEntity) null;
      this._surroundEntity = false;
      this._targetAgent = (Agent) null;
      this._scriptedAgents = (List<Agent>) null;
      this._tickTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.5f);
      this._attackEntityReassignTimer = (Timer) null;
      this._lastPosition = WorldPosition.Invalid;
      this._isFacingDirection = false;
      this._position = WorldPosition.Invalid;
      this._getPositionResultCache = WorldPosition.Invalid;
      this._getPositionFirstSectionCache = WorldPosition.Invalid;
      this._getPositionIsNavmeshlessCache = false;
      this._followState = MovementOrder.FollowState.Stop;
      this._departStartTime = -1f;
    }

    public static MovementOrder MovementOrderFollow(Agent targetAgent) => new MovementOrder(MovementOrder.MovementOrderEnum.Follow, targetAgent);

    public static MovementOrder MovementOrderGuard(Agent targetAgent) => new MovementOrder(MovementOrder.MovementOrderEnum.Guard, targetAgent);

    private MovementOrder(MovementOrder.MovementOrderEnum orderEnum, Agent targetAgent)
    {
      this.OrderEnum = orderEnum;
      WorldPosition targetAgentPos = targetAgent.GetWorldPosition();
      this._positionLambda = orderEnum != MovementOrder.MovementOrderEnum.Follow ? (Func<Formation, WorldPosition>) (f =>
      {
        WorldPosition worldPosition = targetAgentPos;
        worldPosition.SetVec2(worldPosition.AsVec2 - 4f * (f.QuerySystem.Team.MedianTargetFormationPosition.AsVec2 - targetAgentPos.AsVec2).Normalized());
        return (double) worldPosition.AsVec2.DistanceSquared(f.MovementOrder._lastPosition.AsVec2) > 6.25 ? worldPosition : f.MovementOrder._lastPosition;
      }) : (Func<Formation, WorldPosition>) (f =>
      {
        WorldPosition worldPosition = targetAgentPos;
        worldPosition.SetVec2(worldPosition.AsVec2 - f.GetMiddleFrontUnitPositionOffset());
        return worldPosition;
      });
      this._targetAgent = targetAgent;
      this.TargetFormation = (Formation) null;
      this.TargetEntity = (GameEntity) null;
      this._surroundEntity = false;
      this._scriptedAgents = (List<Agent>) null;
      this._tickTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.5f);
      this._attackEntityReassignTimer = (Timer) null;
      this._lastPosition = targetAgentPos;
      this._isFacingDirection = false;
      this._position = WorldPosition.Invalid;
      this._getPositionResultCache = WorldPosition.Invalid;
      this._getPositionFirstSectionCache = WorldPosition.Invalid;
      this._getPositionIsNavmeshlessCache = false;
      this._followState = MovementOrder.FollowState.Stop;
      this._departStartTime = -1f;
    }

    public static MovementOrder MovementOrderFollowEntity(GameEntity targetEntity) => new MovementOrder(MovementOrder.MovementOrderEnum.FollowEntity, targetEntity);

    private MovementOrder(MovementOrder.MovementOrderEnum orderEnum, GameEntity targetEntity)
    {
      this.OrderEnum = orderEnum;
      this._positionLambda = (Func<Formation, WorldPosition>) (f =>
      {
        WorldPosition worldPosition = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, targetEntity.GlobalPosition, false);
        worldPosition.SetVec2(worldPosition.AsVec2);
        return worldPosition;
      });
      this.TargetEntity = targetEntity;
      this.TargetFormation = (Formation) null;
      this._surroundEntity = false;
      this._targetAgent = (Agent) null;
      this._scriptedAgents = (List<Agent>) null;
      this._tickTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.5f);
      this._attackEntityReassignTimer = (Timer) null;
      this._lastPosition = WorldPosition.Invalid;
      this._isFacingDirection = false;
      this._position = WorldPosition.Invalid;
      this._getPositionResultCache = WorldPosition.Invalid;
      this._getPositionFirstSectionCache = WorldPosition.Invalid;
      this._getPositionIsNavmeshlessCache = false;
      this._followState = MovementOrder.FollowState.Stop;
      this._departStartTime = -1f;
    }

    public static MovementOrder MovementOrderMove(WorldPosition position) => new MovementOrder(MovementOrder.MovementOrderEnum.Move, position);

    private MovementOrder(MovementOrder.MovementOrderEnum orderEnum, WorldPosition position)
    {
      this.OrderEnum = orderEnum;
      this._positionLambda = (Func<Formation, WorldPosition>) null;
      this._isFacingDirection = false;
      this.TargetFormation = (Formation) null;
      this.TargetEntity = (GameEntity) null;
      this._surroundEntity = false;
      this._targetAgent = (Agent) null;
      this._scriptedAgents = (List<Agent>) null;
      this._tickTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.5f);
      this._attackEntityReassignTimer = (Timer) null;
      this._lastPosition = WorldPosition.Invalid;
      this._position = position;
      this._getPositionResultCache = WorldPosition.Invalid;
      this._getPositionFirstSectionCache = WorldPosition.Invalid;
      this._getPositionIsNavmeshlessCache = false;
      this._followState = MovementOrder.FollowState.Stop;
      this._departStartTime = -1f;
    }

    public static WorldPosition GetAttachPosition(
      Formation attached,
      Formation target,
      MovementOrder.Side side)
    {
      WorldPosition orderPosition = attached.OrderPosition;
      switch (side)
      {
        case MovementOrder.Side.Front:
          ref WorldPosition local1 = ref orderPosition;
          Vec2 asVec2_1 = orderPosition.AsVec2;
          WorldPosition worldPosition1 = target.FrontAttachmentPoint;
          Vec2 asVec2_2 = worldPosition1.AsVec2;
          Vec2 vec2_1 = asVec2_1 + asVec2_2;
          worldPosition1 = attached.RearAttachmentPoint;
          Vec2 asVec2_3 = worldPosition1.AsVec2;
          Vec2 vec2_2 = vec2_1 - asVec2_3;
          local1.SetVec2(vec2_2);
          break;
        case MovementOrder.Side.Rear:
          ref WorldPosition local2 = ref orderPosition;
          Vec2 asVec2_4 = orderPosition.AsVec2;
          WorldPosition worldPosition2 = target.RearAttachmentPoint;
          Vec2 asVec2_5 = worldPosition2.AsVec2;
          Vec2 vec2_3 = asVec2_4 + asVec2_5;
          worldPosition2 = attached.FrontAttachmentPoint;
          Vec2 asVec2_6 = worldPosition2.AsVec2;
          Vec2 vec2_4 = vec2_3 - asVec2_6;
          local2.SetVec2(vec2_4);
          break;
        case MovementOrder.Side.Left:
          ref WorldPosition local3 = ref orderPosition;
          Vec2 asVec2_7 = orderPosition.AsVec2;
          WorldPosition worldPosition3 = target.LeftAttachmentPoint;
          Vec2 asVec2_8 = worldPosition3.AsVec2;
          Vec2 vec2_5 = asVec2_7 + asVec2_8;
          worldPosition3 = attached.RightAttachmentPoint;
          Vec2 asVec2_9 = worldPosition3.AsVec2;
          Vec2 vec2_6 = vec2_5 - asVec2_9;
          local3.SetVec2(vec2_6);
          break;
        case MovementOrder.Side.Right:
          ref WorldPosition local4 = ref orderPosition;
          Vec2 asVec2_10 = orderPosition.AsVec2;
          WorldPosition worldPosition4 = target.RightAttachmentPoint;
          Vec2 asVec2_11 = worldPosition4.AsVec2;
          Vec2 vec2_7 = asVec2_10 + asVec2_11;
          worldPosition4 = attached.LeftAttachmentPoint;
          Vec2 asVec2_12 = worldPosition4.AsVec2;
          Vec2 vec2_8 = vec2_7 - asVec2_12;
          local4.SetVec2(vec2_8);
          break;
        default:
          return WorldPosition.Invalid;
      }
      return orderPosition;
    }

    public OrderType OrderType
    {
      get
      {
        switch (this.OrderEnum)
        {
          case MovementOrder.MovementOrderEnum.Attach:
            return OrderType.Attach;
          case MovementOrder.MovementOrderEnum.AttackEntity:
            return OrderType.AttackEntity;
          case MovementOrder.MovementOrderEnum.Charge:
            return OrderType.Charge;
          case MovementOrder.MovementOrderEnum.ChargeToTarget:
            return OrderType.ChargeWithTarget;
          case MovementOrder.MovementOrderEnum.Follow:
            return OrderType.FollowMe;
          case MovementOrder.MovementOrderEnum.FollowEntity:
            return OrderType.FollowEntity;
          case MovementOrder.MovementOrderEnum.Guard:
            return OrderType.GuardMe;
          case MovementOrder.MovementOrderEnum.Move:
            return OrderType.Move;
          case MovementOrder.MovementOrderEnum.Retreat:
            return OrderType.Retreat;
          case MovementOrder.MovementOrderEnum.Stop:
            return OrderType.StandYourGround;
          case MovementOrder.MovementOrderEnum.Advance:
            return OrderType.Advance;
          case MovementOrder.MovementOrderEnum.FallBack:
            return OrderType.FallBack;
          default:
            return OrderType.Move;
        }
      }
    }

    internal MovementOrder.MovementOrderEnum GetNativeEnum() => this.OrderEnum;

    internal bool IsDestinationSpeedMultiplier() => this.OrderEnum != MovementOrder.MovementOrderEnum.FollowEntity;

    internal float GetDestinationSpeed(Formation formation)
    {
      if (!this.IsApplicable(formation))
        return 0.0f;
      switch (this.OrderEnum)
      {
        case MovementOrder.MovementOrderEnum.Attach:
          return this.TargetFormation.GetMovementSpeedOfUnits();
        case MovementOrder.MovementOrderEnum.Follow:
          return this._followState == MovementOrder.FollowState.Move ? this._targetAgent.GetCurrentVelocity().Length : 0.0f;
        case MovementOrder.MovementOrderEnum.FollowEntity:
          GameEntity gameEntity = this.TargetEntity;
          do
          {
            ScriptComponentBehaviour componentBehaviour = gameEntity.GetScriptComponents((Func<ScriptComponentBehaviour, bool>) (sc => sc is IMoveableSiegeWeapon)).FirstOrDefault<ScriptComponentBehaviour>();
            if (componentBehaviour != null)
              return Math.Max(0.0f, ((IMoveableSiegeWeapon) componentBehaviour).MovementComponent.CurrentSpeed);
            gameEntity = gameEntity.Parent;
          }
          while (!((NativeObject) gameEntity == (NativeObject) null));
          return 0.0f;
        default:
          return 0.0f;
      }
    }

    internal MovementOrder.MovementStateEnum MovementState
    {
      get
      {
        switch (this.OrderEnum)
        {
          case MovementOrder.MovementOrderEnum.Charge:
          case MovementOrder.MovementOrderEnum.ChargeToTarget:
          case MovementOrder.MovementOrderEnum.Guard:
            return MovementOrder.MovementStateEnum.Charge;
          case MovementOrder.MovementOrderEnum.Retreat:
            return MovementOrder.MovementStateEnum.Retreat;
          case MovementOrder.MovementOrderEnum.Stop:
            return MovementOrder.MovementStateEnum.StandGround;
          default:
            return MovementOrder.MovementStateEnum.Hold;
        }
      }
    }

    private static WorldPosition GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(
      Formation f,
      WorldPosition originalPosition)
    {
      float positionPenalty = 1f;
      WorldPosition ofBoundsPosition = Mission.Current.GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(originalPosition.AsVec2 - f.QuerySystem.AveragePosition, originalPosition, ref positionPenalty);
      if (f.AI == null || f.AI.ActiveBehavior == null)
        return ofBoundsPosition;
      f.AI.ActiveBehavior.NavmeshlessTargetPositionPenalty = positionPenalty;
      return ofBoundsPosition;
    }

    private WorldPosition GetPositionAuxF(Formation f)
    {
      if (this.OrderEnum != MovementOrder.MovementOrderEnum.Follow)
        return WorldPosition.Invalid;
      Vec2 zero = Vec2.Zero;
      if (this._followState != MovementOrder.FollowState.Move && this._targetAgent.MountAgent != null)
        zero += f.Direction * -2f;
      if (this._followState == MovementOrder.FollowState.Move && f.IsMounted())
        zero += 2f * this._targetAgent.Velocity.AsVec2;
      else if (this._followState == MovementOrder.FollowState.Move)
        f.IsMounted();
      WorldPosition worldPosition1 = this._targetAgent.GetWorldPosition();
      worldPosition1.SetVec2(worldPosition1.AsVec2 - f.GetMiddleFrontUnitPositionOffset() + zero);
      WorldPosition worldPosition2;
      if (this._followState == MovementOrder.FollowState.Stop || this._followState == MovementOrder.FollowState.Depart)
      {
        float num = f.IsCavalry() ? 4f : 2.5f;
        if (Mission.Current.IsTeleportingAgents || (double) worldPosition1.AsVec2.DistanceSquared(this._lastPosition.AsVec2) > (double) num * (double) num)
          this._lastPosition = worldPosition1;
        worldPosition2 = this._lastPosition;
      }
      else
        worldPosition2 = worldPosition1;
      this._lastPosition = worldPosition2;
      return worldPosition2;
    }

    public WorldPosition GetPosition(Formation f)
    {
      if (!this.IsApplicable(f))
        return f.OrderPosition;
      WorldPosition originalPosition;
      switch (this.OrderEnum)
      {
        case MovementOrder.MovementOrderEnum.Follow:
          originalPosition = this.GetPositionAuxF(f);
          break;
        case MovementOrder.MovementOrderEnum.Advance:
        case MovementOrder.MovementOrderEnum.FallBack:
          originalPosition = this.GetPositionAux(f);
          break;
        default:
          Func<Formation, WorldPosition> positionLambda = this._positionLambda;
          originalPosition = positionLambda != null ? positionLambda(f) : this._position;
          break;
      }
      bool flag = false;
      if (this._getPositionFirstSectionCache.Position != originalPosition.Position)
      {
        this._getPositionIsNavmeshlessCache = false;
        if (originalPosition.IsValid)
        {
          this._getPositionFirstSectionCache = originalPosition;
          if (originalPosition.GetNavMesh() == UIntPtr.Zero || !Mission.Current.IsPositionInsideBoundaries(originalPosition.AsVec2))
          {
            originalPosition = MovementOrder.GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(f, originalPosition);
          }
          else
          {
            flag = true;
            this._getPositionIsNavmeshlessCache = true;
          }
          this._getPositionResultCache = originalPosition;
        }
      }
      else
        originalPosition = this._getPositionResultCache;
      if (this._getPositionIsNavmeshlessCache | flag && f.AI?.ActiveBehavior != null)
        f.AI.ActiveBehavior.NavmeshlessTargetPositionPenalty = 1f;
      return originalPosition;
    }

    public bool AreOrdersPracticallySame(MovementOrder m1, MovementOrder m2, bool isAIControlled)
    {
      if (m1.OrderEnum != m2.OrderEnum)
        return false;
      switch (m1.OrderEnum)
      {
        case MovementOrder.MovementOrderEnum.Attach:
          return m1.TargetFormation == m2.TargetFormation;
        case MovementOrder.MovementOrderEnum.AttackEntity:
          return (NativeObject) m1.TargetEntity == (NativeObject) m2.TargetEntity;
        case MovementOrder.MovementOrderEnum.Charge:
          return true;
        case MovementOrder.MovementOrderEnum.ChargeToTarget:
          return m1.TargetFormation == m2.TargetFormation;
        case MovementOrder.MovementOrderEnum.Follow:
          return m1._targetAgent == m2._targetAgent;
        case MovementOrder.MovementOrderEnum.FollowEntity:
          return (NativeObject) m1.TargetEntity == (NativeObject) m2.TargetEntity;
        case MovementOrder.MovementOrderEnum.Guard:
          return m1._targetAgent == m2._targetAgent;
        case MovementOrder.MovementOrderEnum.Move:
          return isAIControlled && (double) m1._position.AsVec2.DistanceSquared(m2._position.AsVec2) < 1.0;
        case MovementOrder.MovementOrderEnum.Retreat:
          return true;
        case MovementOrder.MovementOrderEnum.Stop:
          return true;
        case MovementOrder.MovementOrderEnum.Advance:
          return true;
        case MovementOrder.MovementOrderEnum.FallBack:
          return true;
        default:
          return true;
      }
    }

    internal void OnApply(Formation formation)
    {
      switch (this.OrderEnum)
      {
        case MovementOrder.MovementOrderEnum.Attach:
          if (this.DetectLoop(formation))
          {
            Formation formation1 = formation.MovementOrder.TargetFormation;
            while (true)
            {
              Formation targetFormation = formation1.MovementOrder.TargetFormation;
              if (targetFormation != formation)
                formation1 = targetFormation;
              else
                break;
            }
            Formation formation2 = formation1;
            formation2.MovementOrder = new MovementOrder(MovementOrder.MovementOrderEnum.Move, formation2.OrderPosition);
            break;
          }
          break;
        case MovementOrder.MovementOrderEnum.AttackEntity:
          formation.FormAttackEntityDetachment(this.TargetEntity);
          break;
        case MovementOrder.MovementOrderEnum.ChargeToTarget:
          formation.TargetFormation = this.TargetFormation;
          break;
        case MovementOrder.MovementOrderEnum.Follow:
          formation.arrangement.ReserveMiddleFrontUnitPosition((IFormationUnit) this._targetAgent);
          break;
        case MovementOrder.MovementOrderEnum.Guard:
          Agent localTargetAgent = this._targetAgent;
          formation.ApplyActionOnEachUnit((Action<Agent>) (agent => MovementOrder.OnUnitJoinOrLeaveAux(agent, localTargetAgent, true)));
          break;
        case MovementOrder.MovementOrderEnum.Move:
          formation.SetPositioning(new WorldPosition?(this.GetPosition(formation)));
          break;
        case MovementOrder.MovementOrderEnum.Retreat:
          MovementOrder.RetreatAux(formation);
          break;
      }
      if (this.OrderEnum == MovementOrder.MovementOrderEnum.Charge || this.OrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget)
        formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetChargeBehaviorValues));
      else if (this.OrderEnum == MovementOrder.MovementOrderEnum.Follow || formation.ArrangementOrder.OrderEnum == ArrangementOrder.ArrangementOrderEnum.Column)
        formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetFollowBehaviorValues));
      else if (formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.ShieldWall && formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Circle && formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Square)
        formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetDefaultMoveBehaviorValues));
      else
        formation.ApplyActionOnEachUnit(new Action<Agent>(MovementOrder.SetDefensiveArrangementMoveBehaviorValues));
    }

    internal static void SetDefensiveArrangementMoveBehaviorValues(Agent unit)
    {
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.GoToPos, 3f, 8f, 5f, 20f, 6f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.Melee, 4f, 5f, 0.0f, 20f, 0.0f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.Ranged, 0.0f, 7f, 0.0f, 20f, 0.0f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.ChargeHorseback, 0.0f, 7f, 0.0f, 30f, 0.0f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.RangedHorseback, 0.0f, 15f, 0.0f, 30f, 0.0f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 4f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
    }

    internal static void SetFollowBehaviorValues(Agent unit)
    {
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.Melee, 6f, 7f, 4f, 20f, 0.0f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.Ranged, 0.0f, 7f, 0.0f, 20f, 0.0f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.ChargeHorseback, 0.0f, 7f, 0.0f, 30f, 0.0f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.RangedHorseback, 0.0f, 15f, 0.0f, 30f, 0.0f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 4f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
    }

    internal static void SetDefaultMoveBehaviorValues(Agent unit)
    {
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.GoToPos, 3f, 7f, 5f, 20f, 6f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.Melee, 8f, 7f, 5f, 20f, 0.01f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.Ranged, 0.02f, 7f, 0.04f, 20f, 0.03f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.ChargeHorseback, 10f, 7f, 5f, 30f, 0.05f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.RangedHorseback, 0.02f, 15f, 0.065f, 30f, 0.055f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 4f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
    }

    private static void SetChargeBehaviorValues(Agent unit)
    {
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.GoToPos, 0.0f, 7f, 4f, 20f, 6f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.Melee, 8f, 7f, 4f, 20f, 1f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.Ranged, 2f, 7f, 4f, 20f, 5f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.ChargeHorseback, 2f, 25f, 5f, 30f, 5f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.RangedHorseback, 0.0f, 10f, 3f, 20f, 6f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityMelee, 5f, 12f, 7.5f, 30f, 4f);
      unit.SetAIBehaviorValues(AISimpleBehaviorKind.AttackEntityRanged, 0.55f, 12f, 0.8f, 30f, 0.45f);
    }

    private static void RetreatAux(Formation formation)
    {
      foreach (IDetachment detachment in formation.Detachments.ToList<IDetachment>())
        formation.LeaveDetachment(detachment);
      formation.ApplyActionOnEachUnitViaBackupList((Action<Agent>) (agent =>
      {
        if (!agent.IsAIControlled)
          return;
        agent.Retreat();
      }));
    }

    internal void OnCancel(Formation formation)
    {
      switch (this.OrderEnum)
      {
        case MovementOrder.MovementOrderEnum.AttackEntity:
          formation.DisbandAttackEntityDetachment();
          break;
        case MovementOrder.MovementOrderEnum.ChargeToTarget:
          formation.TargetFormation = (Formation) null;
          break;
        case MovementOrder.MovementOrderEnum.Follow:
          formation.arrangement.ReleaseMiddleFrontUnitPosition();
          break;
        case MovementOrder.MovementOrderEnum.Guard:
          Agent localTargetAgent = this._targetAgent;
          formation.ApplyActionOnEachUnit((Action<Agent>) (agent => MovementOrder.OnUnitJoinOrLeaveAux(agent, localTargetAgent, false)));
          break;
        case MovementOrder.MovementOrderEnum.Retreat:
          formation.ApplyActionOnEachUnitViaBackupList((Action<Agent>) (agent =>
          {
            if (!agent.IsAIControlled)
              return;
            agent.StopRetreatingMoraleComponent();
          }));
          break;
        case MovementOrder.MovementOrderEnum.FallBack:
          if (Mission.Current.IsPositionInsideBoundaries(this.GetPosition(formation).AsVec2))
            break;
          formation.ApplyActionOnEachUnitViaBackupList((Action<Agent>) (agent =>
          {
            if (!agent.IsAIControlled)
              return;
            agent.StopRetreatingMoraleComponent();
          }));
          break;
      }
    }

    internal void OnUnitJoinOrLeave(Formation formation, Agent unit, bool isJoining)
    {
      if (!this.IsApplicable(formation))
        return;
      if (this.OrderEnum == MovementOrder.MovementOrderEnum.Guard)
        MovementOrder.OnUnitJoinOrLeaveAux(unit, this._targetAgent, isJoining);
      if (!isJoining)
        return;
      if (this.OrderEnum == MovementOrder.MovementOrderEnum.Charge || this.OrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget)
        MovementOrder.SetChargeBehaviorValues(unit);
      else if (formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.ShieldWall && formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Circle && formation.ArrangementOrder.OrderEnum != ArrangementOrder.ArrangementOrderEnum.Square)
        MovementOrder.SetDefaultMoveBehaviorValues(unit);
      else
        MovementOrder.SetDefensiveArrangementMoveBehaviorValues(unit);
    }

    internal bool CancelsPreviousDirectionOrder
    {
      get
      {
        switch (this.OrderEnum)
        {
          case MovementOrder.MovementOrderEnum.Attach:
            return true;
          case MovementOrder.MovementOrderEnum.Move:
            return this._isFacingDirection;
          default:
            return false;
        }
      }
    }

    internal bool IsApplicable(Formation formation)
    {
      switch (this.OrderEnum)
      {
        case MovementOrder.MovementOrderEnum.Attach:
          if (!MovementOrder.IsFormationAttachable(this.TargetFormation))
            return false;
          FacingOrder facingOrder = formation.FacingOrder;
          return false;
        case MovementOrder.MovementOrderEnum.AttackEntity:
          UsableMachine firstScriptOfType1 = this.TargetEntity.GetFirstScriptOfType<UsableMachine>();
          if (firstScriptOfType1 != null)
            return !firstScriptOfType1.IsDestroyed;
          DestructableComponent firstScriptOfType2 = this.TargetEntity.GetFirstScriptOfType<DestructableComponent>();
          return firstScriptOfType2 != null && !firstScriptOfType2.IsDestroyed;
        case MovementOrder.MovementOrderEnum.Charge:
          for (int index = 0; index < Mission.Current.Teams.Count; ++index)
          {
            Team team = Mission.Current.Teams[index];
            if (team.IsEnemyOf(formation.Team) && team.ActiveAgents.Count > 0)
              return true;
          }
          return false;
        case MovementOrder.MovementOrderEnum.ChargeToTarget:
          return this.TargetFormation.CountOfUnits > 0;
        case MovementOrder.MovementOrderEnum.Follow:
          return this._targetAgent.IsActive();
        case MovementOrder.MovementOrderEnum.FollowEntity:
          UsableMachine firstScriptOfType3 = this.TargetEntity.GetFirstScriptOfType<UsableMachine>();
          return firstScriptOfType3 == null || !firstScriptOfType3.IsDestroyed;
        case MovementOrder.MovementOrderEnum.Guard:
          return this._targetAgent.IsActive();
        default:
          return true;
      }
    }

    internal bool Tick(Formation formation)
    {
      int num = this._tickTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)) ? 1 : 0;
      this.TickAux(formation);
      if (num == 0)
        return num != 0;
      this.TickOccasionally(formation, this._tickTimer.PreviousDeltaTime);
      return num != 0;
    }

    private void TickOccasionally(Formation formation, float dt)
    {
      if (this.OrderEnum != MovementOrder.MovementOrderEnum.FallBack || Mission.Current.IsPositionInsideBoundaries(this.GetPosition(formation).AsVec2))
        return;
      MovementOrder.RetreatAux(formation);
    }

    private void TickAux(Formation formation)
    {
      if (this.OrderEnum != MovementOrder.MovementOrderEnum.Follow)
        return;
      float length = this._targetAgent.GetCurrentVelocity().Length;
      if ((double) length < 0.00999999977648258)
        this._followState = MovementOrder.FollowState.Stop;
      else if ((double) length < (double) this._targetAgent.Monster.WalkingSpeedLimit * 0.699999988079071)
      {
        if (this._followState == MovementOrder.FollowState.Stop)
        {
          this._followState = MovementOrder.FollowState.Depart;
          this._departStartTime = MBCommon.GetTime(MBCommon.TimeType.Mission);
        }
        else
        {
          if (this._followState != MovementOrder.FollowState.Move)
            return;
          this._followState = MovementOrder.FollowState.Arrive;
        }
      }
      else if (this._followState == MovementOrder.FollowState.Depart)
      {
        if ((double) MBCommon.GetTime(MBCommon.TimeType.Mission) - (double) this._departStartTime <= 1.0)
          return;
        this._followState = MovementOrder.FollowState.Move;
      }
      else
        this._followState = MovementOrder.FollowState.Move;
    }

    internal void OnArrangementChanged(Formation formation)
    {
      if (!this.IsApplicable(formation) || this.OrderEnum != MovementOrder.MovementOrderEnum.Follow)
        return;
      formation.arrangement.ReserveMiddleFrontUnitPosition((IFormationUnit) this._targetAgent);
    }

    public void Advance(Formation formation, float distance)
    {
      WorldPosition currentPosition = this.GetPosition(formation);
      Vec2 direction = formation.Direction;
      currentPosition.SetVec2(currentPosition.AsVec2 + direction * distance);
      this._positionLambda = (Func<Formation, WorldPosition>) (f => currentPosition);
    }

    public void FallBack(Formation formation, float distance) => this.Advance(formation, -distance);

    private static void OnUnitJoinOrLeaveAux(Agent unit, Agent target, bool isJoining) => unit.SetGuardState(target, isJoining);

    private (Agent, float) GetBestAgent(List<Agent> candidateAgents)
    {
      if (candidateAgents.IsEmpty<Agent>())
        return ((Agent) null, float.MaxValue);
      Vec3 targetEntityPos = this.TargetEntity.GlobalPosition;
      Agent agent = candidateAgents.MinBy<Agent, float>((Func<Agent, float>) (ca => ca.Position.DistanceSquared(targetEntityPos)));
      return (agent, agent.Position.DistanceSquared(targetEntityPos));
    }

    private (Agent, float) GetWorstAgent(List<Agent> currentAgents, int requiredAgentCount)
    {
      if (requiredAgentCount <= 0 || currentAgents.Count < requiredAgentCount)
        return ((Agent) null, float.MaxValue);
      Vec3 targetEntityPos = this.TargetEntity.GlobalPosition;
      Agent agent = currentAgents.MaxBy<Agent, float>((Func<Agent, float>) (ca => ca.Position.DistanceSquared(targetEntityPos)));
      return (agent, agent.Position.DistanceSquared(targetEntityPos));
    }

    internal MovementOrder GetSubstituteOrder(Formation formation)
    {
      switch (this.OrderEnum)
      {
        case MovementOrder.MovementOrderEnum.Attach:
          if (this.TargetFormation.MovementOrder.OrderEnum == MovementOrder.MovementOrderEnum.Charge)
            return MovementOrder.MovementOrderCharge;
          if (this.TargetFormation.MovementOrder.OrderEnum == MovementOrder.MovementOrderEnum.Retreat)
            return MovementOrder.MovementOrderRetreat;
          return this.TargetFormation.CountOfUnitsWithoutDetachedOnes == 0 ? MovementOrder.MovementOrderCharge : new MovementOrder(MovementOrder.MovementOrderEnum.Move, formation.OrderPosition);
        case MovementOrder.MovementOrderEnum.Charge:
          return MovementOrder.MovementOrderStop;
        default:
          return MovementOrder.MovementOrderCharge;
      }
    }

    public static bool IsFormationAttachable(Formation formation) => formation.MovementOrder.MovementState == MovementOrder.MovementStateEnum.Hold && formation.CountOfUnitsWithoutDetachedOnes > 0;

    public bool DetectLoop(Formation first)
    {
      Formation formation1 = first;
      Formation formation2 = first;
      Func<Formation, Formation> func = (Func<Formation, Formation>) (f =>
      {
        if (f == null)
          return (Formation) null;
        MovementOrder movementOrder = f.MovementOrder;
        return !(movementOrder != (object) null) ? (Formation) null : movementOrder.TargetFormation;
      });
      while (func(formation2) != null)
      {
        formation1 = func(formation1);
        formation2 = func(func(formation2));
        if (formation1 == formation2)
          break;
      }
      return func(formation2) != null;
    }

    private Vec2 GetDirectionAux(Formation f)
    {
      switch (this.OrderEnum)
      {
        case MovementOrder.MovementOrderEnum.Advance:
        case MovementOrder.MovementOrderEnum.FallBack:
          FormationQuerySystem querySystem = f.QuerySystem;
          FormationQuerySystem closestEnemyFormation = querySystem.ClosestEnemyFormation;
          return closestEnemyFormation == null ? Vec2.One : (closestEnemyFormation.MedianPosition.AsVec2 - querySystem.AveragePosition).Normalized();
        default:
          return Vec2.One;
      }
    }

    private WorldPosition GetPositionAux(Formation f)
    {
      switch (this.OrderEnum)
      {
        case MovementOrder.MovementOrderEnum.Advance:
          FormationQuerySystem querySystem = f.QuerySystem;
          FormationQuerySystem closestEnemyFormation = querySystem.ClosestEnemyFormation;
          if (closestEnemyFormation == null)
            return f.OrderPosition;
          WorldPosition medianPosition1 = closestEnemyFormation.MedianPosition;
          if (querySystem.IsRangedFormation || querySystem.IsRangedCavalryFormation)
          {
            if ((double) medianPosition1.AsVec2.DistanceSquared(querySystem.AveragePosition) <= (double) querySystem.MissileRange * (double) querySystem.MissileRange)
            {
              Vec2 directionAux = this.GetDirectionAux(f);
              medianPosition1.SetVec2(medianPosition1.AsVec2 - directionAux * querySystem.MissileRange);
            }
          }
          else
          {
            Vec2 vec2 = (closestEnemyFormation.AveragePosition - f.QuerySystem.AveragePosition).Normalized();
            float num = 2f;
            if ((double) closestEnemyFormation.FormationPower < (double) f.QuerySystem.FormationPower * 0.200000002980232)
              num = 0.1f;
            medianPosition1.SetVec2(medianPosition1.AsVec2 - vec2 * num);
          }
          return medianPosition1;
        case MovementOrder.MovementOrderEnum.FallBack:
          Vec2 directionAux1 = this.GetDirectionAux(f);
          WorldPosition medianPosition2 = f.QuerySystem.MedianPosition;
          medianPosition2.SetVec2(f.QuerySystem.AveragePosition - directionAux1 * 7f);
          return medianPosition2;
        default:
          return WorldPosition.Invalid;
      }
    }

    internal static int GetMovementOrderDefensiveness(MovementOrder.MovementOrderEnum orderEnum) => orderEnum == MovementOrder.MovementOrderEnum.Charge || orderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget ? 0 : 1;

    internal static int GetMovementOrderDefensivenessChange(
      MovementOrder.MovementOrderEnum previousOrderEnum,
      MovementOrder.MovementOrderEnum nextOrderEnum)
    {
      return previousOrderEnum == MovementOrder.MovementOrderEnum.Charge || previousOrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget ? (nextOrderEnum != MovementOrder.MovementOrderEnum.Charge && nextOrderEnum != MovementOrder.MovementOrderEnum.ChargeToTarget ? 1 : 0) : (nextOrderEnum == MovementOrder.MovementOrderEnum.Charge || nextOrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget ? -1 : 0);
    }

    public override bool Equals(object obj) => obj is MovementOrder movementOrder && movementOrder == (object) this;

    public override int GetHashCode() => (int) this.OrderEnum;

    public static bool operator !=(MovementOrder m, object obj)
    {
      if (obj is MovementOrder movementOrder)
        return m.OrderEnum != movementOrder.OrderEnum;
      return obj != null || (uint) m.OrderEnum > 0U;
    }

    public static bool operator ==(MovementOrder m, object obj)
    {
      if (obj is MovementOrder movementOrder)
        return m.OrderEnum == movementOrder.OrderEnum;
      return obj == null && m.OrderEnum == MovementOrder.MovementOrderEnum.Invalid;
    }

    internal enum MovementOrderEnum
    {
      Invalid,
      Attach,
      AttackEntity,
      Charge,
      ChargeToTarget,
      Follow,
      FollowEntity,
      Guard,
      Move,
      Retreat,
      Stop,
      Advance,
      FallBack,
    }

    public enum Side
    {
      Front,
      Rear,
      Left,
      Right,
    }

    private enum FollowState
    {
      Stop,
      Depart,
      Move,
      Arrive,
    }

    internal enum MovementStateEnum
    {
      Charge,
      Hold,
      Retreat,
      StandGround,
    }
  }
}
