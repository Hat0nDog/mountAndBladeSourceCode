// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeLadder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
  public class SiegeLadder : 
    SiegeWeapon,
    IPrimarySiegeWeapon,
    IOrderableWithInteractionArea,
    IOrderable,
    ISpawnable
  {
    private static readonly ActionIndexCache act_usage_ladder_lift_from_left_1_start = ActionIndexCache.Create(nameof (act_usage_ladder_lift_from_left_1_start));
    private static readonly ActionIndexCache act_usage_ladder_lift_from_left_2_start = ActionIndexCache.Create(nameof (act_usage_ladder_lift_from_left_2_start));
    private static readonly ActionIndexCache act_usage_ladder_lift_from_right_1_start = ActionIndexCache.Create(nameof (act_usage_ladder_lift_from_right_1_start));
    private static readonly ActionIndexCache act_usage_ladder_lift_from_right_2_start = ActionIndexCache.Create(nameof (act_usage_ladder_lift_from_right_2_start));
    private static readonly ActionIndexCache act_usage_ladder_pick_up_fork_begin = ActionIndexCache.Create(nameof (act_usage_ladder_pick_up_fork_begin));
    private static readonly ActionIndexCache act_usage_ladder_pick_up_fork_end = ActionIndexCache.Create(nameof (act_usage_ladder_pick_up_fork_end));
    private static readonly ActionIndexCache act_usage_ladder_push_back = ActionIndexCache.Create(nameof (act_usage_ladder_push_back));
    private static readonly ActionIndexCache act_usage_ladder_push_back_stopped = ActionIndexCache.Create(nameof (act_usage_ladder_push_back_stopped));
    public const float ClimbingLimitRadian = -0.2013583f;
    public const float ClimbingLimitDegree = -11.53698f;
    public string AttackerTag = "attacker";
    public string DefenderTag = "defender";
    public string downStateEntityTag = "ladderDown";
    public string IdleAnimation = "siege_ladder_idle";
    public string RaiseAnimation = "siege_ladder_rise";
    public string RaiseAnimationWithoutRootBone = "siege_ladder_rise_wo_rootbone";
    public string PushBackAnimation = "siege_ladder_push_back";
    public string PushBackAnimationWithoutRootBone = "siege_ladder_push_back_wo_rootbone";
    public string TrembleWallHeavyAnimation = "siege_ladder_stop_wall_heavy";
    public string TrembleWallLightAnimation = "siege_ladder_stop_wall_light";
    public string TrembleGroundAnimation = "siege_ladder_stop_ground_heavy";
    public string RightStandingPointTag = "right";
    public string LeftStandingPointTag = "left";
    public string FrontStandingPointTag = "front";
    public string PushForkItemID = "push_fork";
    public string upStateEntityTag = "ladderUp";
    public string BodyTag = "ladder_body";
    public string CollisionBodyTag = "ladder_collision_body";
    public string InitialWaitPositionTag = "initialwaitposition";
    private string _targetWallSegmentTag = "";
    private string _sideTag;
    private FormationAI.BehaviorSide _weaponSide;
    private WallSegment _targetWallSegment;
    public const float AutomaticUseActivationRange = 20f;
    private int _onWallNavMeshId;
    private bool _isNavigationMeshDisabled;
    private bool _isLadderPhysicsDisabled;
    private bool _isLadderCollisionPhysicsDisabled;
    private Timer _tickOccasionallyTimer;
    private float _upStateRotationRadian;
    private float _downStateRotationRadian;
    public SiegeLadder.LadderState initialState;
    private float _fallAngularSpeed;
    private MatrixFrame _ladderDownFrame;
    private MatrixFrame _ladderUpFrame;
    private SiegeLadder.LadderAnimationState _animationState;
    private int _currentActionAgentCount;
    private SiegeLadder.LadderState _state;
    public string BarrierTagToRemove = "barrier";
    public string IndestructibleMerlonsTag = string.Empty;
    private List<GameEntity> _aiBarriers;
    private List<StandingPoint> _attackerStandingPoints;
    private StandingPointWithWeaponRequirement _pushingWithForkStandingPoint;
    private StandingPointWithWeaponRequirement _forkPickUpStandingPoint;
    private ItemObject _forkItem;
    private MatrixFrame[] _attackerStandingPointLocalIKFrames;
    private MatrixFrame _ladderInitialGlobalFrame;
    private SynchedMissionObject _ladderParticleObject;
    private SynchedMissionObject _ladderBodyObject;
    private SynchedMissionObject _ladderCollisionBodyObject;
    private SynchedMissionObject _ladderObject;
    private float _lastDotProductOfAnimationAndTargetRotation;
    private LadderQueueManager _queueManagerForAttackers;
    private LadderQueueManager _queueManagerForDefenders;
    private Timer _forkReappearingTimer;
    private float _forkReappearingDelay = 10f;
    private SynchedMissionObject _forkEntity;

    public MissionObject TargetCastlePosition => (MissionObject) this._targetWallSegment;

    public SiegeLadder.LadderState State
    {
      get => this._state;
      set
      {
        if (this._state == value)
          return;
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetSiegeLadderState(this, value));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        this._state = value;
        this.OnLadderStateChange();
      }
    }

    public bool HasCompletedAction() => this.State == SiegeLadder.LadderState.OnWall;

    public float SiegeWeaponPriority => 3f;

    public FormationAI.BehaviorSide WeaponSide => this._weaponSide;

    public int OverTheWallNavMeshID => 13;

    public bool HoldLadders => false;

    public bool SendLadders => (uint) this.State > 0U;

    public override bool IsDisabledForBattleSide(BattleSideEnum sideEnum) => sideEnum == BattleSideEnum.Attacker ? this.State == SiegeLadder.LadderState.FallToLand || this.State == SiegeLadder.LadderState.FallToWall || (this.State == SiegeLadder.LadderState.OnWall || this.State == SiegeLadder.LadderState.BeingPushedBack) || this.State == SiegeLadder.LadderState.BeingPushedBackStartFromWall || this.State == SiegeLadder.LadderState.BeingPushedBackStopped : this.State == SiegeLadder.LadderState.OnLand || this.State == SiegeLadder.LadderState.FallToLand || (this.State == SiegeLadder.LadderState.BeingRaised || this.State == SiegeLadder.LadderState.BeingRaisedStartFromGround) || this.State == SiegeLadder.LadderState.BeingRaisedStopped || this.State == SiegeLadder.LadderState.FallToWall;

    public GameEntity InitialWaitPosition { get; private set; }

    public override SiegeEngineType GetSiegeEngineType() => DefaultSiegeEngineTypes.Ladder;

    protected internal override void OnInit()
    {
      base.OnInit();
      this._tickOccasionallyTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), (float) (0.200000002980232 + (double) MBRandom.RandomFloat * 0.0500000007450581));
      this._aiBarriers = this.Scene.FindEntitiesWithTag(this.BarrierTagToRemove).ToList<GameEntity>();
      if (this.IndestructibleMerlonsTag != string.Empty)
      {
        foreach (GameEntity gameEntity in this.Scene.FindEntitiesWithTag(this.IndestructibleMerlonsTag))
        {
          DestructableComponent firstScriptOfType = gameEntity.GetFirstScriptOfType<DestructableComponent>();
          firstScriptOfType.SetDisabled();
          firstScriptOfType.CanBeDestroyedInitially = false;
        }
      }
      this._attackerStandingPoints = this.GameEntity.CollectObjectsWithTag<StandingPoint>(this.AttackerTag);
      this._pushingWithForkStandingPoint = this.GameEntity.CollectObjectsWithTag<StandingPointWithWeaponRequirement>(this.DefenderTag).FirstOrDefault<StandingPointWithWeaponRequirement>();
      this._forkPickUpStandingPoint = this.GameEntity.CollectObjectsWithTag<StandingPointWithWeaponRequirement>(this.AmmoPickUpTag).FirstOrDefault<StandingPointWithWeaponRequirement>();
      if (this._forkPickUpStandingPoint != null)
        this._forkPickUpStandingPoint.SetUsingBattleSide(BattleSideEnum.Defender);
      this._ladderParticleObject = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>("particles").FirstOrDefault<SynchedMissionObject>();
      this._forkEntity = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>("push_fork").FirstOrDefault<SynchedMissionObject>();
      if (this.StandingPoints != null)
      {
        foreach (StandingPoint standingPoint in this.StandingPoints)
        {
          if (!standingPoint.GameEntity.HasTag(this.AmmoPickUpTag))
          {
            standingPoint.AddComponent((UsableMissionObjectComponent) new ResetAnimationOnStopUsageComponent(standingPoint.GameEntity.HasTag(this.DefenderTag) ? SiegeLadder.act_usage_ladder_push_back_stopped : ActionIndexCache.act_none));
            standingPoint.IsDeactivated = true;
          }
        }
      }
      this._forkItem = Game.Current.ObjectManager.GetObject<ItemObject>(this.PushForkItemID);
      this._pushingWithForkStandingPoint.InitRequiredWeapon(this._forkItem);
      this._forkPickUpStandingPoint.InitGivenWeapon(this._forkItem);
      GameEntity gameEntity1 = this.GameEntity.CollectChildrenEntitiesWithTag(this.upStateEntityTag)[0];
      this._ladderObject = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>(this.downStateEntityTag)[0];
      this._ladderBodyObject = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>(this.BodyTag)[0];
      this._ladderCollisionBodyObject = this.GameEntity.CollectObjectsWithTag<SynchedMissionObject>(this.CollisionBodyTag)[0];
      this._ladderDownFrame = this._ladderObject.GameEntity.GetFrame();
      this._ladderDownFrame.rotation.RotateAboutSide(this._downStateRotationRadian - this._ladderDownFrame.rotation.GetEulerAngles().x);
      this._ladderObject.GameEntity.SetFrame(ref this._ladderDownFrame);
      MatrixFrame frame = gameEntity1.GetFrame();
      frame.rotation = Mat3.Identity;
      frame.rotation.RotateAboutSide(this._upStateRotationRadian);
      this._ladderUpFrame = frame;
      MatrixFrame matrixFrame = this._ladderObject.GameEntity.Parent.GetFrame();
      this._ladderUpFrame = matrixFrame.TransformToLocal(this._ladderUpFrame);
      this._ladderInitialGlobalFrame = this._ladderObject.GameEntity.GetGlobalFrame();
      this._attackerStandingPointLocalIKFrames = new MatrixFrame[this._attackerStandingPoints.Count];
      this.State = this.initialState;
      for (int index1 = 0; index1 < this._attackerStandingPoints.Count; ++index1)
      {
        MatrixFrame[] pointLocalIkFrames = this._attackerStandingPointLocalIKFrames;
        int index2 = index1;
        matrixFrame = this._attackerStandingPoints[index1].GameEntity.GetGlobalFrame();
        MatrixFrame local = matrixFrame.TransformToLocal(this._ladderInitialGlobalFrame);
        pointLocalIkFrames[index2] = local;
        this._attackerStandingPoints[index1].AddComponent((UsableMissionObjectComponent) new ClearHandInverseKinematicsOnStopUsageComponent());
      }
      this.CalculateNavigationAndPhysics();
      this.InitialWaitPosition = this.GameEntity.CollectChildrenEntitiesWithTag(this.InitialWaitPositionTag).FirstOrDefault<GameEntity>();
      foreach (GameEntity gameEntity2 in this.Scene.FindEntitiesWithTag(this._targetWallSegmentTag))
      {
        WallSegment firstScriptOfType = gameEntity2.GetFirstScriptOfType<WallSegment>();
        if (firstScriptOfType != null)
        {
          this._targetWallSegment = firstScriptOfType;
          this._targetWallSegment.AttackerSiegeWeapon = (IPrimarySiegeWeapon) this;
          break;
        }
      }
      string sideTag = this._sideTag;
      this._weaponSide = sideTag == "left" ? FormationAI.BehaviorSide.Left : (sideTag == "middle" ? FormationAI.BehaviorSide.Middle : (sideTag == "right" ? FormationAI.BehaviorSide.Right : FormationAI.BehaviorSide.Middle));
      this.ForcedUse = false;
      LadderQueueManager[] array = this.GameEntity.GetScriptComponents<LadderQueueManager>().ToArray<LadderQueueManager>();
      MatrixFrame globalFrame1 = this.GameEntity.GetGlobalFrame();
      MatrixFrame local1 = globalFrame1.TransformToLocal(this._ladderObject.GameEntity.GetGlobalFrame());
      if (array.Length != 0)
      {
        this._queueManagerForAttackers = array[0];
        this._queueManagerForAttackers.Initialize(this._onWallNavMeshId, local1, -local1.rotation.f, BattleSideEnum.Attacker, 3, 2.356194f, 0.8f, 0.8f, 30f, 50f, false, 0.8f, 4f, 5f);
      }
      if (array.Length > 1 && this._pushingWithForkStandingPoint != null)
      {
        this._queueManagerForDefenders = array[1];
        MatrixFrame globalFrame2 = this._pushingWithForkStandingPoint.GameEntity.GetGlobalFrame();
        globalFrame2.rotation.RotateAboutSide(1.570796f);
        globalFrame2.origin -= globalFrame2.rotation.u;
        globalFrame1 = this.GameEntity.GetGlobalFrame();
        this._queueManagerForDefenders.Initialize(this._onWallNavMeshId, globalFrame1.TransformToLocal(globalFrame2), local1.rotation.f, BattleSideEnum.Defender, 1, 2.827433f, 0.5f, 0.8f, 30f, 50f, true, 0.8f, 0.0f, 5f);
      }
      this.EnemyRangeToStopUsing = 0.0f;
      this.SetUpStateVisibility(false);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    private void CalculateNavigationAndPhysics()
    {
      if (!GameNetwork.IsClientOrReplay)
      {
        bool flag = this.State != SiegeLadder.LadderState.OnWall;
        if (this._isNavigationMeshDisabled != flag)
        {
          this._isNavigationMeshDisabled = flag;
          this.GameEntity.Scene.SetAbilityOfFacesWithId(this._onWallNavMeshId, !this._isNavigationMeshDisabled);
        }
      }
      bool flag1 = (this.State == SiegeLadder.LadderState.BeingRaisedStartFromGround || this.State == SiegeLadder.LadderState.BeingRaised) && this._animationState != SiegeLadder.LadderAnimationState.PhysicallyDynamic;
      bool flag2 = true;
      if (this._isLadderPhysicsDisabled != flag1)
      {
        this._isLadderPhysicsDisabled = flag1;
        this._ladderBodyObject.GameEntity.SetPhysicsState(!this._isLadderPhysicsDisabled, true);
      }
      if (!flag1)
      {
        MatrixFrame parent = this._ladderObject.GameEntity.GetGlobalFrame().TransformToParent(this._ladderObject.GameEntity.Skeleton.GetBoneEntitialFrameWithIndex((byte) 0));
        parent.rotation.RotateAboutForward(1.570796f);
        this._ladderBodyObject.GameEntity.SetGlobalFrame(parent);
        flag2 = this.State != SiegeLadder.LadderState.BeingPushedBack || (double) parent.rotation.f.z < 0.0;
        if (!flag2)
        {
          float y = Math.Min((float) (2.00999999046326 - (double) parent.rotation.u.z * 2.0), 1f);
          parent.rotation.ApplyScaleLocal(new Vec3(1f, y, 1f));
          this._ladderCollisionBodyObject.GameEntity.SetGlobalFrame(parent);
        }
      }
      if (this._isLadderCollisionPhysicsDisabled == flag2)
        return;
      this._isLadderCollisionPhysicsDisabled = flag2;
      this._ladderCollisionBodyObject.GameEntity.SetPhysicsState(!this._isLadderCollisionPhysicsDisabled, true);
    }

    private float GetCurrentLadderAngularSpeed(string animationName)
    {
      GameEntity gameEntity = this._ladderObject.GameEntity;
      float parameterAtChannel = gameEntity.Skeleton.GetAnimationParameterAtChannel(0);
      MatrixFrame entitialFrameWithIndex1 = gameEntity.Skeleton.GetBoneEntitialFrameWithIndex((byte) 0);
      if ((double) parameterAtChannel <= 0.00999999977648258)
        return 0.0f;
      gameEntity.Skeleton.SetAnimationParameterAtChannel(0, parameterAtChannel - 0.01f);
      gameEntity.Skeleton.TickAnimationsAndForceUpdate(0.0001f, gameEntity.GetGlobalFrame(), false);
      MatrixFrame entitialFrameWithIndex2 = gameEntity.Skeleton.GetBoneEntitialFrameWithIndex((byte) 0);
      return (float) (((double) new Vec2(entitialFrameWithIndex1.rotation.f.y, entitialFrameWithIndex1.rotation.f.z).RotationInRadians - (double) new Vec2(entitialFrameWithIndex2.rotation.f.y, entitialFrameWithIndex2.rotation.f.z).RotationInRadians) / ((double) MBAnimation.GetAnimationDuration(animationName) * 0.00999999977648258));
    }

    private void OnLadderStateChange()
    {
      GameEntity gameEntity = this._ladderObject.GameEntity;
      if (this.State != SiegeLadder.LadderState.OnWall)
        this.SetVisibilityOfAIBarriers(true);
      switch (this.State)
      {
        case SiegeLadder.LadderState.OnLand:
          this._animationState = SiegeLadder.LadderAnimationState.Static;
          break;
        case SiegeLadder.LadderState.FallToLand:
          if (gameEntity.Skeleton.GetAnimationAtChannel(0) != this.TrembleGroundAnimation)
          {
            gameEntity.SetFrame(ref this._ladderDownFrame);
            gameEntity.Skeleton.SetAnimationAtChannel(this.TrembleGroundAnimation, 0);
            this._animationState = SiegeLadder.LadderAnimationState.Static;
          }
          if (GameNetwork.IsClientOrReplay)
            break;
          this.State = SiegeLadder.LadderState.OnLand;
          break;
        case SiegeLadder.LadderState.BeingRaisedStartFromGround:
          this._animationState = SiegeLadder.LadderAnimationState.Animated;
          MatrixFrame frame = gameEntity.GetFrame();
          frame.rotation.RotateAboutSide(-1.570796f);
          gameEntity.SetFrame(ref frame);
          gameEntity.Skeleton.SetAnimationAtChannel(this.RaiseAnimation, 0);
          gameEntity.Skeleton.ForceUpdateBoneFrames();
          this._lastDotProductOfAnimationAndTargetRotation = -1000f;
          if (GameNetwork.IsClientOrReplay)
            break;
          this._currentActionAgentCount = 1;
          this.State = SiegeLadder.LadderState.BeingRaised;
          break;
        case SiegeLadder.LadderState.BeingRaisedStopped:
          this._animationState = SiegeLadder.LadderAnimationState.PhysicallyDynamic;
          MatrixFrame parent1 = gameEntity.GetGlobalFrame().TransformToParent(gameEntity.Skeleton.GetBoneEntitialFrameWithIndex((byte) 0));
          parent1.rotation.RotateAboutForward(1.570796f);
          this._fallAngularSpeed = this.GetCurrentLadderAngularSpeed(this.RaiseAnimation);
          float parameterAtChannel1 = gameEntity.Skeleton.GetAnimationParameterAtChannel(0);
          gameEntity.SetGlobalFrame(parent1);
          gameEntity.Skeleton.SetAnimationAtChannel(this.RaiseAnimationWithoutRootBone, 0);
          gameEntity.Skeleton.SetAnimationParameterAtChannel(0, parameterAtChannel1);
          gameEntity.Skeleton.TickAnimationsAndForceUpdate(0.0001f, gameEntity.GetGlobalFrame(), false);
          gameEntity.Skeleton.SetAnimationAtChannel(this.IdleAnimation, 0);
          this._ladderObject.SetLocalPositionSmoothStep(ref this._ladderDownFrame.origin);
          if (GameNetwork.IsClientOrReplay)
            break;
          this.State = SiegeLadder.LadderState.BeingPushedBack;
          break;
        case SiegeLadder.LadderState.OnWall:
          this._animationState = SiegeLadder.LadderAnimationState.Static;
          this.SetVisibilityOfAIBarriers(false);
          break;
        case SiegeLadder.LadderState.FallToWall:
          if (GameNetwork.IsClientOrReplay)
          {
            string animationAtChannel = gameEntity.Skeleton.GetAnimationAtChannel(0);
            if (!(animationAtChannel != this.TrembleWallHeavyAnimation) || !(animationAtChannel != this.TrembleWallLightAnimation))
              break;
            gameEntity.SetFrame(ref this._ladderUpFrame);
            gameEntity.Skeleton.SetAnimationAtChannel((double) this._fallAngularSpeed < -0.5 ? this.TrembleWallHeavyAnimation : this.TrembleWallLightAnimation, 0);
            this._animationState = SiegeLadder.LadderAnimationState.Static;
            break;
          }
          this.State = SiegeLadder.LadderState.OnWall;
          this._ladderParticleObject?.BurstParticlesSynched(false);
          break;
        case SiegeLadder.LadderState.BeingPushedBackStartFromWall:
          this._animationState = SiegeLadder.LadderAnimationState.Animated;
          gameEntity.Skeleton.SetAnimationAtChannel(this.PushBackAnimation, 0);
          gameEntity.Skeleton.TickAnimationsAndForceUpdate(0.0001f, gameEntity.GetGlobalFrame(), false);
          this._lastDotProductOfAnimationAndTargetRotation = -1000f;
          if (GameNetwork.IsClientOrReplay)
            break;
          this._currentActionAgentCount = 1;
          this.State = SiegeLadder.LadderState.BeingPushedBack;
          break;
        case SiegeLadder.LadderState.BeingPushedBackStopped:
          Skeleton skeleton = gameEntity.Skeleton;
          this._animationState = SiegeLadder.LadderAnimationState.PhysicallyDynamic;
          MatrixFrame parent2 = gameEntity.GetGlobalFrame().TransformToParent(gameEntity.Skeleton.GetBoneEntitialFrameWithIndex((byte) 0));
          parent2.rotation.RotateAboutForward(1.570796f);
          this._fallAngularSpeed = this.GetCurrentLadderAngularSpeed(this.PushBackAnimation);
          float parameterAtChannel2 = skeleton.GetAnimationParameterAtChannel(0);
          gameEntity.SetGlobalFrame(parent2);
          skeleton.SetAnimationAtChannel(this.PushBackAnimationWithoutRootBone, 0);
          skeleton.SetAnimationParameterAtChannel(0, parameterAtChannel2);
          skeleton.TickAnimationsAndForceUpdate(0.0001f, gameEntity.GetGlobalFrame(), false);
          skeleton.SetAnimationAtChannel(this.IdleAnimation, 0);
          this._ladderObject.SetLocalPositionSmoothStep(ref this._ladderUpFrame.origin);
          if (!GameNetwork.IsClientOrReplay)
            this.State = SiegeLadder.LadderState.BeingRaised;
          skeleton.ForceUpdateBoneFrames();
          break;
      }
    }

    private void SetVisibilityOfAIBarriers(bool visibility)
    {
      foreach (GameEntity aiBarrier in this._aiBarriers)
        aiBarrier.SetVisibilityExcludeParents(visibility);
    }

    public override OrderType GetOrder(BattleSideEnum side) => side == BattleSideEnum.Attacker ? base.GetOrder(side) : OrderType.Move;

    private ActionIndexCache GetActionCodeToUseForStandingPoint(
      StandingPoint standingPoint)
    {
      GameEntity gameEntity = standingPoint.GameEntity;
      return !gameEntity.HasTag(this.RightStandingPointTag) ? (!gameEntity.HasTag(this.FrontStandingPointTag) ? SiegeLadder.act_usage_ladder_lift_from_left_2_start : SiegeLadder.act_usage_ladder_lift_from_left_1_start) : (!gameEntity.HasTag(this.FrontStandingPointTag) ? SiegeLadder.act_usage_ladder_lift_from_right_2_start : SiegeLadder.act_usage_ladder_lift_from_right_1_start);
    }

    protected override float GetDetachmentWeightAux(BattleSideEnum side)
    {
      if (side == BattleSideEnum.Attacker)
        return base.GetDetachmentWeightAux(side);
      if (this.IsDisabledForBattleSideAI(side))
        return float.MinValue;
      this._usableStandingPoints.Clear();
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < this.StandingPoints.Count; ++index)
      {
        StandingPoint standingPoint = this.StandingPoints[index];
        if (standingPoint.IsUsableBySide(side) && (standingPoint != this._forkPickUpStandingPoint || this._pushingWithForkStandingPoint.IsUsableBySide(side)))
        {
          if (!standingPoint.HasAIMovingTo)
          {
            if (!flag2)
              this._usableStandingPoints.Clear();
            flag2 = true;
          }
          else if (flag2 || standingPoint.MovingAgents.FirstOrDefault<KeyValuePair<Agent, UsableMissionObject.MoveInfo>>().Key.Formation.Team.Side != side)
            continue;
          flag1 = true;
          this._usableStandingPoints.Add((index, standingPoint));
        }
      }
      this._areUsableStandingPointsVacant = flag2;
      if (!flag1)
        return float.MinValue;
      if (flag2)
        return 1f;
      return !this._isDetachmentRecentlyEvaluated ? 0.1f : 0.01f;
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (this._tickOccasionallyTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        this.TickRare();
      if (!GameNetwork.IsClientOrReplay && this._forkReappearingTimer != null && this._forkReappearingTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
      {
        this._forkPickUpStandingPoint.SetIsDeactivatedSynched(false);
        this._forkEntity.SetVisibleSynched(true);
      }
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < this._attackerStandingPoints.Count; ++index)
      {
        if (this._attackerStandingPoints[index].HasUser)
          this._attackerStandingPoints[index].UserAgent.SetHandInverseKinematicsFrameFromActionChannel(1, this._attackerStandingPointLocalIKFrames[index], this._ladderInitialGlobalFrame);
      }
      if (this._ladderObject == null)
        return;
      GameEntity ladderObjectEntity = this._ladderObject.GameEntity;
      if (!GameNetwork.IsClientOrReplay)
      {
        if (this._queueManagerForAttackers != null)
          this._queueManagerForAttackers.IsDeactivated = this.State != SiegeLadder.LadderState.OnWall;
        if (this._queueManagerForDefenders != null)
          this._queueManagerForDefenders.IsDeactivated = this.State != SiegeLadder.LadderState.OnWall;
        string animationAtChannel = ladderObjectEntity.Skeleton.GetAnimationAtChannel(0);
        bool flag1 = false;
        if (!string.IsNullOrEmpty(animationAtChannel))
        {
          flag1 = animationAtChannel == this.TrembleGroundAnimation || animationAtChannel == this.TrembleWallHeavyAnimation || animationAtChannel == this.TrembleWallLightAnimation;
          if (flag1)
            flag1 = (double) ladderObjectEntity.Skeleton.GetAnimationParameterAtChannel(0) < 1.0;
        }
        num1 += this._pushingWithForkStandingPoint.HasUser ? 1 : 0;
        num2 += this._attackerStandingPoints.Count<StandingPoint>((Func<StandingPoint, bool>) (standingPoint => standingPoint.HasUser));
        bool? someoneOver = new bool?();
        Func<bool> func = (Func<bool>) (() =>
        {
          if (!someoneOver.HasValue)
          {
            someoneOver = new bool?(false);
            foreach (Agent agent in Mission.Current.GetAgentsInRange(ladderObjectEntity.GlobalPosition.AsVec2, (ladderObjectEntity.GlobalBoxMax - ladderObjectEntity.GlobalBoxMin).AsVec2.Length * 0.5f))
            {
              if (agent.IsActive() && agent.GetSteppedMachine() == this)
              {
                someoneOver = new bool?(true);
                break;
              }
            }
          }
          return someoneOver.Value;
        });
        foreach (StandingPoint standingPoint in this.StandingPoints)
        {
          GameEntity gameEntity = standingPoint.GameEntity;
          if (!gameEntity.HasTag(this.AmmoPickUpTag))
          {
            bool flag2 = false;
            if (!standingPoint.HasUser && this.State == SiegeLadder.LadderState.BeingRaised && gameEntity.HasTag(this.AttackerTag))
              flag2 = (double) ladderObjectEntity.Skeleton.GetAnimationParameterAtChannel(0) * (double) MBAnimation.GetAnimationDuration(ladderObjectEntity.Skeleton.GetAnimationAtChannel(0)) / (double) Math.Max(MBAnimation.GetAnimationDuration(MBAnimation.GetAnimationIndexOfActionCode(MBGlobals.HumanWarriorActionSet, this.GetActionCodeToUseForStandingPoint(standingPoint))), 0.01f) > 0.980000019073486;
            standingPoint.SetIsDeactivatedSynched(((flag2 || this._animationState == SiegeLadder.LadderAnimationState.PhysicallyDynamic || this.State == SiegeLadder.LadderState.BeingRaisedStopped ? 1 : (this.State == SiegeLadder.LadderState.BeingPushedBackStopped ? 1 : 0)) | (flag1 ? 1 : 0)) != 0 || gameEntity.HasTag(this.AttackerTag) && (this.State == SiegeLadder.LadderState.OnWall || this.State == SiegeLadder.LadderState.FallToWall || (this.State == SiegeLadder.LadderState.BeingPushedBack || this.State == SiegeLadder.LadderState.BeingPushedBackStartFromWall)) || gameEntity.HasTag(this.DefenderTag) && (this.State == SiegeLadder.LadderState.OnLand || this.State == SiegeLadder.LadderState.FallToLand || (this.State == SiegeLadder.LadderState.BeingRaised || this.State == SiegeLadder.LadderState.BeingRaisedStartFromGround) || func()));
          }
        }
        if (this._forkPickUpStandingPoint.HasUser)
        {
          Agent userAgent = this._forkPickUpStandingPoint.UserAgent;
          ActionIndexCache currentAction = userAgent.GetCurrentAction(1);
          if (!(currentAction == SiegeLadder.act_usage_ladder_pick_up_fork_begin))
          {
            if (currentAction == SiegeLadder.act_usage_ladder_pick_up_fork_end)
            {
              MissionWeapon weapon = new MissionWeapon(this._forkItem, (ItemModifier) null, (Banner) null);
              userAgent.EquipWeaponToExtraSlotAndWield(ref weapon);
              this._forkPickUpStandingPoint.UserAgent.StopUsingGameObject();
              this._forkPickUpStandingPoint.SetIsDeactivatedSynched(true);
              this._forkEntity.SetVisibleSynched(false);
              this._forkReappearingTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), this._forkReappearingDelay);
              if (userAgent.IsAIControlled)
              {
                StandingPoint standingPointFor = this.GetSuitableStandingPointFor(userAgent.Team.Side, userAgent, (IEnumerable<Agent>) null, (IEnumerable<AgentValuePair<float>>) null);
                if (standingPointFor != null)
                {
                  ((IDetachment) this).AddAgent(userAgent);
                  if (userAgent.Formation != null)
                  {
                    userAgent.Formation.DetachUnit(userAgent, this.IsLoose);
                    userAgent.Detachment = (IDetachment) this;
                    userAgent.DetachmentWeight = this.GetWeightOfStandingPoint(standingPointFor);
                  }
                }
              }
            }
            else if (!this._forkPickUpStandingPoint.UserAgent.SetActionChannel(1, SiegeLadder.act_usage_ladder_pick_up_fork_begin))
              this._forkPickUpStandingPoint.UserAgent.StopUsingGameObject();
          }
        }
        else if (this._forkPickUpStandingPoint.HasAIMovingTo)
        {
          List<Agent> agentList = new List<Agent>();
          foreach (KeyValuePair<Agent, UsableMissionObject.MoveInfo> movingAgent in this._forkPickUpStandingPoint.MovingAgents)
          {
            if (movingAgent.Key != null && movingAgent.Key.Team != null && !this._pushingWithForkStandingPoint.IsUsableBySide(movingAgent.Key.Team.Side))
              agentList.Add(movingAgent.Key);
          }
          foreach (Agent agent in agentList)
            agent.StopUsingGameObject();
        }
      }
      switch (this.State)
      {
        case SiegeLadder.LadderState.OnLand:
        case SiegeLadder.LadderState.FallToLand:
          if (!GameNetwork.IsClientOrReplay && num2 > 0)
          {
            this.State = SiegeLadder.LadderState.BeingRaisedStartFromGround;
            break;
          }
          break;
        case SiegeLadder.LadderState.BeingRaised:
        case SiegeLadder.LadderState.BeingRaisedStartFromGround:
        case SiegeLadder.LadderState.BeingPushedBackStopped:
          if (this._animationState == SiegeLadder.LadderAnimationState.Animated)
          {
            float parameterAtChannel1 = ladderObjectEntity.Skeleton.GetAnimationParameterAtChannel(0);
            float animationDuration1 = MBAnimation.GetAnimationDuration(ladderObjectEntity.Skeleton.GetAnimationAtChannel(0));
            foreach (StandingPoint standingPoint in this._attackerStandingPoints.Where<StandingPoint>((Func<StandingPoint, bool>) (standingPoint => standingPoint.HasUser)))
            {
              ActionIndexCache forStandingPoint = this.GetActionCodeToUseForStandingPoint(standingPoint);
              ActionIndexCache currentAction = standingPoint.UserAgent.GetCurrentAction(1);
              if (currentAction == forStandingPoint)
              {
                int indexOfActionCode = MBAnimation.GetAnimationIndexOfActionCode(MBGlobals.HumanWarriorActionSet, forStandingPoint);
                float progress = MBMath.ClampFloat(parameterAtChannel1 * animationDuration1 / Math.Max(MBAnimation.GetAnimationDuration(indexOfActionCode), 0.01f), 0.0f, 1f);
                standingPoint.UserAgent.SetCurrentActionProgress(1, progress);
              }
              else if (MBAnimation.GetActionType(currentAction) == Agent.ActionCodeType.LadderRaiseEnd)
              {
                float animationDuration2 = MBAnimation.GetAnimationDuration(MBAnimation.GetAnimationIndexOfActionCode(MBGlobals.HumanWarriorActionSet, currentAction));
                float num3 = animationDuration1 - animationDuration2;
                float progress = MBMath.ClampFloat((parameterAtChannel1 * animationDuration1 - num3) / Math.Max(animationDuration2, 0.01f), 0.0f, 1f);
                standingPoint.UserAgent.SetCurrentActionProgress(1, progress);
              }
            }
            bool flag = false;
            if (!GameNetwork.IsClientOrReplay)
            {
              if (num2 > 0)
              {
                if (num2 != this._currentActionAgentCount)
                {
                  this._currentActionAgentCount = num2;
                  float animationSpeed = (float) Math.Sqrt((double) this._currentActionAgentCount);
                  float parameterAtChannel2 = ladderObjectEntity.Skeleton.GetAnimationParameterAtChannel(0);
                  this._ladderObject.SetAnimationAtChannelSynched(this.RaiseAnimation, 0, animationSpeed);
                  if ((double) parameterAtChannel2 > 0.0)
                    this._ladderObject.SetAnimationChannelParameterSynched(0, parameterAtChannel2);
                }
                foreach (StandingPoint standingPoint in this._attackerStandingPoints.Where<StandingPoint>((Func<StandingPoint, bool>) (standingPoint => standingPoint.HasUser)))
                {
                  ActionIndexCache forStandingPoint = this.GetActionCodeToUseForStandingPoint(standingPoint);
                  ActionIndexCache currentAction = standingPoint.UserAgent.GetCurrentAction(1);
                  if (currentAction != forStandingPoint && MBAnimation.GetActionType(currentAction) != Agent.ActionCodeType.LadderRaiseEnd)
                  {
                    if (!standingPoint.UserAgent.SetActionChannel(1, forStandingPoint))
                      standingPoint.UserAgent.StopUsingGameObject(false);
                  }
                  else if (MBAnimation.GetActionType(currentAction) == Agent.ActionCodeType.LadderRaiseEnd)
                    standingPoint.UserAgent.ClearTargetFrame();
                }
              }
              else
              {
                this.State = SiegeLadder.LadderState.BeingRaisedStopped;
                flag = true;
              }
            }
            if (!flag)
            {
              Skeleton skeleton = ladderObjectEntity.Skeleton;
              MatrixFrame parent = ladderObjectEntity.GetGlobalFrame().TransformToParent(skeleton.GetBoneEntitialFrameWithIndex((byte) 0));
              parent.rotation.RotateAboutForward(1.570796f);
              if ((double) parameterAtChannel1 > 0.899999976158142 || (double) parent.rotation.f.z <= 0.0)
              {
                this._animationState = SiegeLadder.LadderAnimationState.PhysicallyDynamic;
                this._fallAngularSpeed = this.GetCurrentLadderAngularSpeed(this.RaiseAnimation);
                ladderObjectEntity.SetGlobalFrame(parent);
                skeleton.SetAnimationAtChannel(this.RaiseAnimationWithoutRootBone, 0);
                skeleton.SetAnimationParameterAtChannel(0, parameterAtChannel1);
                skeleton.TickAnimationsAndForceUpdate(0.0001f, ladderObjectEntity.GetGlobalFrame(), false);
                skeleton.SetAnimationAtChannel(this.IdleAnimation, 0);
                this._ladderObject.SetLocalPositionSmoothStep(ref this._ladderUpFrame.origin);
                break;
              }
              break;
            }
            break;
          }
          if (this._animationState == SiegeLadder.LadderAnimationState.PhysicallyDynamic)
          {
            MatrixFrame frame = ladderObjectEntity.GetFrame();
            frame.rotation.RotateAboutSide(this._fallAngularSpeed * dt);
            ladderObjectEntity.SetFrame(ref frame);
            MatrixFrame parent = ladderObjectEntity.GetFrame().TransformToParent(ladderObjectEntity.Skeleton.GetBoneEntitialFrameWithIndex((byte) 0));
            float num3 = Vec3.DotProduct(parent.rotation.f, this._ladderUpFrame.rotation.f);
            if ((double) this._fallAngularSpeed < 0.0 && (double) num3 > 0.949999988079071 && (double) num3 < (double) this._lastDotProductOfAnimationAndTargetRotation)
            {
              ladderObjectEntity.SetFrame(ref this._ladderUpFrame);
              ladderObjectEntity.Skeleton.SetAnimationParameterAtChannel(0, 0.0f);
              ladderObjectEntity.Skeleton.TickAnimationsAndForceUpdate(0.0001f, ladderObjectEntity.GetGlobalFrame(), false);
              this._animationState = SiegeLadder.LadderAnimationState.Static;
              ladderObjectEntity.Skeleton.SetAnimationAtChannel((double) this._fallAngularSpeed < -0.5 ? this.TrembleWallHeavyAnimation : this.TrembleWallLightAnimation, 0);
              if (!GameNetwork.IsClientOrReplay)
                this.State = SiegeLadder.LadderState.FallToWall;
            }
            this._fallAngularSpeed -= dt * 2f * Math.Max(0.3f, 1f - parent.rotation.u.z);
            this._lastDotProductOfAnimationAndTargetRotation = num3;
            break;
          }
          break;
        case SiegeLadder.LadderState.BeingRaisedStopped:
        case SiegeLadder.LadderState.BeingPushedBack:
        case SiegeLadder.LadderState.BeingPushedBackStartFromWall:
          if (this._animationState == SiegeLadder.LadderAnimationState.Animated)
          {
            float parameterAtChannel1 = ladderObjectEntity.Skeleton.GetAnimationParameterAtChannel(0);
            if (this._pushingWithForkStandingPoint.HasUser)
            {
              ActionIndexCache usageLadderPushBack = SiegeLadder.act_usage_ladder_push_back;
              if (this._pushingWithForkStandingPoint.UserAgent.GetCurrentAction(1) == usageLadderPushBack)
                this._pushingWithForkStandingPoint.UserAgent.SetCurrentActionProgress(1, parameterAtChannel1);
            }
            bool flag = false;
            if (!GameNetwork.IsClientOrReplay)
            {
              if (num1 > 0)
              {
                if (num1 != this._currentActionAgentCount)
                {
                  this._currentActionAgentCount = num1;
                  float animationSpeed = (float) Math.Sqrt((double) this._currentActionAgentCount);
                  float parameterAtChannel2 = ladderObjectEntity.Skeleton.GetAnimationParameterAtChannel(0);
                  this._ladderObject.SetAnimationAtChannelSynched(this.PushBackAnimation, 0, animationSpeed);
                  if ((double) parameterAtChannel2 > 0.0)
                    this._ladderObject.SetAnimationChannelParameterSynched(0, parameterAtChannel2);
                }
                if (this._pushingWithForkStandingPoint.HasUser)
                {
                  ActionIndexCache usageLadderPushBack = SiegeLadder.act_usage_ladder_push_back;
                  if (this._pushingWithForkStandingPoint.UserAgent.GetCurrentAction(1) != usageLadderPushBack && (double) parameterAtChannel1 < 1.0 && !this._pushingWithForkStandingPoint.UserAgent.SetActionChannel(1, usageLadderPushBack))
                    this._pushingWithForkStandingPoint.UserAgent.StopUsingGameObject(false);
                }
              }
              else
              {
                this.State = SiegeLadder.LadderState.BeingPushedBackStopped;
                flag = true;
              }
            }
            if (!flag)
            {
              MatrixFrame parent = ladderObjectEntity.GetGlobalFrame().TransformToParent(ladderObjectEntity.Skeleton.GetBoneEntitialFrameWithIndex((byte) 0));
              parent.rotation.RotateAboutForward(1.570796f);
              if ((double) parameterAtChannel1 > 0.999899983406067 || (double) parent.rotation.f.z >= 0.0)
              {
                this._animationState = SiegeLadder.LadderAnimationState.PhysicallyDynamic;
                this._fallAngularSpeed = this.GetCurrentLadderAngularSpeed(this.PushBackAnimation);
                ladderObjectEntity.SetGlobalFrame(parent);
                ladderObjectEntity.Skeleton.SetAnimationAtChannel(this.PushBackAnimationWithoutRootBone, 0);
                ladderObjectEntity.Skeleton.SetAnimationParameterAtChannel(0, parameterAtChannel1);
                ladderObjectEntity.Skeleton.TickAnimationsAndForceUpdate(0.0001f, ladderObjectEntity.GetGlobalFrame(), false);
                ladderObjectEntity.Skeleton.SetAnimationAtChannel(this.IdleAnimation, 0);
                this._ladderObject.SetLocalPositionSmoothStep(ref this._ladderDownFrame.origin);
                break;
              }
              break;
            }
            break;
          }
          if (this._animationState == SiegeLadder.LadderAnimationState.PhysicallyDynamic)
          {
            MatrixFrame frame = ladderObjectEntity.GetFrame();
            frame.rotation.RotateAboutSide(this._fallAngularSpeed * dt);
            ladderObjectEntity.SetFrame(ref frame);
            MatrixFrame parent = ladderObjectEntity.GetFrame().TransformToParent(ladderObjectEntity.Skeleton.GetBoneEntitialFrameWithIndex((byte) 0));
            parent.rotation.RotateAboutForward(1.570796f);
            float num3 = Vec3.DotProduct(parent.rotation.f, this._ladderDownFrame.rotation.f);
            if ((double) this._fallAngularSpeed > 0.0 && (double) num3 > 0.949999988079071 && (double) num3 < (double) this._lastDotProductOfAnimationAndTargetRotation)
            {
              this._animationState = SiegeLadder.LadderAnimationState.Static;
              ladderObjectEntity.SetFrame(ref this._ladderDownFrame);
              ladderObjectEntity.Skeleton.SetAnimationParameterAtChannel(0, 0.0f);
              ladderObjectEntity.Skeleton.TickAnimationsAndForceUpdate(0.0001f, ladderObjectEntity.GetGlobalFrame(), false);
              ladderObjectEntity.Skeleton.SetAnimationAtChannel(this.TrembleGroundAnimation, 0);
              this._animationState = SiegeLadder.LadderAnimationState.Static;
              if (!GameNetwork.IsClientOrReplay)
                this.State = SiegeLadder.LadderState.FallToLand;
            }
            this._fallAngularSpeed += dt * 2f * Math.Max(0.3f, 1f - parent.rotation.u.z);
            this._lastDotProductOfAnimationAndTargetRotation = num3;
            break;
          }
          break;
        case SiegeLadder.LadderState.OnWall:
        case SiegeLadder.LadderState.FallToWall:
          if (num1 > 0 && !GameNetwork.IsClientOrReplay)
          {
            this.State = SiegeLadder.LadderState.BeingPushedBackStartFromWall;
            break;
          }
          break;
      }
      this.CalculateNavigationAndPhysics();
    }

    private void TickRare()
    {
      if (GameNetwork.IsReplay)
        return;
      float num1 = (float) (20.0 + (this.ForcedUse ? 3.0 : 0.0));
      float num2 = num1 * num1;
      GameEntity gameEntity = this.GameEntity;
      Mission.TeamCollection teams = Mission.Current.Teams;
      int count = teams.Count;
      Vec3 globalPosition = gameEntity.GlobalPosition;
      for (int index = 0; index < count; ++index)
      {
        Team team = teams[index];
        if (team.Side == BattleSideEnum.Attacker)
        {
          this.ForcedUse = false;
          foreach (Formation formation in team.FormationsIncludingSpecial)
          {
            if ((double) formation.QuerySystem.MedianPosition.AsVec2.DistanceSquared(globalPosition.AsVec2) < (double) num2 && (double) formation.QuerySystem.MedianPosition.GetNavMeshZ() - (double) globalPosition.z < 4.0)
            {
              this.ForcedUse = true;
              break;
            }
          }
        }
      }
    }

    public override UsableMachineAIBase CreateAIBehaviourObject() => (UsableMachineAIBase) new SiegeLadderAI(this);

    public void SetUpStateVisibility(bool isVisible) => this.GameEntity.CollectChildrenEntitiesWithTag(this.upStateEntityTag)[0].SetVisibilityExcludeParents(isVisible);

    public override void SetAbilityOfFaces(bool enabled)
    {
      base.SetAbilityOfFaces(enabled);
      if (enabled)
        return;
      this.GameEntity.Scene.SetAbilityOfFacesWithId(this._onWallNavMeshId, false);
    }

    protected internal override void OnMissionReset()
    {
      this._ladderObject.GameEntity.Skeleton.SetAnimationAtChannel(-1, 0);
      if (this.initialState == SiegeLadder.LadderState.OnLand)
      {
        if (!GameNetwork.IsClientOrReplay)
          this.State = SiegeLadder.LadderState.OnLand;
        this._ladderObject.GameEntity.SetFrame(ref this._ladderDownFrame);
      }
      else
      {
        if (!GameNetwork.IsClientOrReplay)
          this.State = SiegeLadder.LadderState.OnWall;
        this._ladderObject.GameEntity.SetFrame(ref this._ladderUpFrame);
      }
    }

    public override string GetDescriptionText(GameEntity gameEntity = null) => gameEntity.HasTag(this.AmmoPickUpTag) ? new TextObject("{=F9AQxCax}Fork").ToString() : new TextObject("{=G0AWk1rX}Ladder").ToString();

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      TextObject textObject = !usableGameObject.GameEntity.HasTag(this.AmmoPickUpTag) ? (!usableGameObject.GameEntity.HasTag(this.AttackerTag) ? new TextObject("{=MdQJxiGz}({KEY}) Push") : new TextObject("{=kbNcm68J}({KEY}) Lift")) : new TextObject("{=bNYm3K6b}({KEY}) Pick Up");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override bool ReadFromNetwork()
    {
      bool bufferReadValid = base.ReadFromNetwork();
      bool flag = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
      int num1 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeLadderStateCompressionInfo, ref bufferReadValid);
      int num2 = GameNetworkMessage.ReadIntFromPacket(CompressionMission.SiegeLadderAnimationStateCompressionInfo, ref bufferReadValid);
      float num3 = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo, ref bufferReadValid);
      MatrixFrame frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref bufferReadValid);
      string animationName = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
      float num4 = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationProgressCompressionInfo, ref bufferReadValid);
      if (bufferReadValid)
      {
        this.initialState = flag ? SiegeLadder.LadderState.OnLand : SiegeLadder.LadderState.OnWall;
        this._state = (SiegeLadder.LadderState) num1;
        this._animationState = (SiegeLadder.LadderAnimationState) num2;
        this._fallAngularSpeed = num3;
        this._lastDotProductOfAnimationAndTargetRotation = -1000f;
        frame.rotation.Orthonormalize();
        this._ladderObject.GameEntity.SetGlobalFrame(frame);
        if (!string.IsNullOrEmpty(animationName))
        {
          this._ladderObject.GameEntity.Skeleton.SetAnimationAtChannel(animationName, 0);
          this._ladderObject.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, MBMath.ClampFloat(num4, 0.0f, 1f));
          this._ladderObject.GameEntity.Skeleton.ForceUpdateBoneFrames();
        }
      }
      return bufferReadValid;
    }

    public override void WriteToNetwork()
    {
      base.WriteToNetwork();
      GameNetworkMessage.WriteBoolToPacket(this.initialState == SiegeLadder.LadderState.OnLand);
      GameNetworkMessage.WriteIntToPacket((int) this.State, CompressionMission.SiegeLadderStateCompressionInfo);
      GameNetworkMessage.WriteIntToPacket((int) this._animationState, CompressionMission.SiegeLadderAnimationStateCompressionInfo);
      GameNetworkMessage.WriteFloatToPacket(this._fallAngularSpeed, CompressionMission.SiegeMachineComponentAngularSpeedCompressionInfo);
      GameNetworkMessage.WriteMatrixFrameToPacket(this._ladderObject.GameEntity.GetGlobalFrame());
      GameNetworkMessage.WriteStringToPacket(this._ladderObject.GameEntity.Skeleton.GetAnimationAtChannel(0));
      GameNetworkMessage.WriteFloatToPacket(this._ladderObject.GameEntity.Skeleton.GetAnimationParameterAtChannel(0), CompressionBasic.AnimationProgressCompressionInfo);
    }

    bool IOrderableWithInteractionArea.IsPointInsideInteractionArea(
      Vec3 point)
    {
      GameEntity gameEntity = this.GameEntity.CollectChildrenEntitiesWithTag("ui_interaction").FirstOrDefault<GameEntity>();
      return !((NativeObject) gameEntity == (NativeObject) null) && (double) gameEntity.GlobalPosition.AsVec2.DistanceSquared(point.AsVec2) < 25.0;
    }

    public override TargetFlags GetTargetFlags()
    {
      TargetFlags targetFlags = (TargetFlags) (0 | 2 | 8 | 16);
      if (this.HasCompletedAction() || this.IsDeactivated)
        targetFlags |= TargetFlags.NotAThreat;
      return targetFlags;
    }

    public override float GetTargetValue(List<Vec3> weaponPos) => 10f * this.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]);

    protected override float GetDistanceMultiplierOfWeapon(Vec3 weaponPos) => (double) this.GetMinimumDistanceBetweenPositions(weaponPos) < 10.0 ? 1f : 0.9f;

    protected override StandingPoint GetSuitableStandingPointFor(
      BattleSideEnum side,
      Agent agent = null,
      IEnumerable<Agent> agents = null,
      IEnumerable<AgentValuePair<float>> agentValuePairs = null)
    {
      if (side == BattleSideEnum.Attacker)
        return this._attackerStandingPoints.FirstOrDefault<StandingPoint>((Func<StandingPoint, bool>) (sp =>
        {
          if (sp.IsDeactivated)
            return false;
          if (sp.IsInstantUse)
            return true;
          return !sp.HasUser && !sp.HasAIMovingTo;
        }));
      return this._pushingWithForkStandingPoint.IsDeactivated || !this._pushingWithForkStandingPoint.IsInstantUse && (this._pushingWithForkStandingPoint.HasUser || this._pushingWithForkStandingPoint.HasAIMovingTo) ? (StandingPoint) null : (StandingPoint) this._pushingWithForkStandingPoint;
    }

    public void SetSpawnedFromSpawner() => this._spawnedFromSpawner = true;

    public void AssignParametersFromSpawner(
      string sideTag,
      string targetWallSegment,
      int onWallNavMeshId,
      float downStateRotationRadian,
      float upperStateRotationRadian,
      string barrierTagToRemove,
      string indestructibleMerlonsTag)
    {
      this._sideTag = sideTag;
      this._targetWallSegmentTag = targetWallSegment;
      this._onWallNavMeshId = onWallNavMeshId;
      this._downStateRotationRadian = downStateRotationRadian;
      this._upStateRotationRadian = upperStateRotationRadian;
      this.BarrierTagToRemove = barrierTagToRemove;
      this.IndestructibleMerlonsTag = indestructibleMerlonsTag;
    }

    public enum LadderState
    {
      OnLand,
      FallToLand,
      BeingRaised,
      BeingRaisedStartFromGround,
      BeingRaisedStopped,
      OnWall,
      FallToWall,
      BeingPushedBack,
      BeingPushedBackStartFromWall,
      BeingPushedBackStopped,
      NumberOfStates,
    }

    public enum LadderAnimationState
    {
      Static,
      Animated,
      PhysicallyDynamic,
      NumberOfStates,
    }
  }
}
