// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BatteringRam
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
  public class BatteringRam : 
    SiegeWeapon,
    IPathHolder,
    IPrimarySiegeWeapon,
    IMoveableSiegeWeapon,
    ISpawnable
  {
    private static readonly ActionIndexCache act_usage_batteringram_left = ActionIndexCache.Create(nameof (act_usage_batteringram_left));
    private static readonly ActionIndexCache act_usage_batteringram_left_slower = ActionIndexCache.Create(nameof (act_usage_batteringram_left_slower));
    private static readonly ActionIndexCache act_usage_batteringram_left_slowest = ActionIndexCache.Create(nameof (act_usage_batteringram_left_slowest));
    private static readonly ActionIndexCache act_usage_batteringram_right = ActionIndexCache.Create(nameof (act_usage_batteringram_right));
    private static readonly ActionIndexCache act_usage_batteringram_right_slower = ActionIndexCache.Create(nameof (act_usage_batteringram_right_slower));
    private static readonly ActionIndexCache act_usage_batteringram_right_slowest = ActionIndexCache.Create(nameof (act_usage_batteringram_right_slowest));
    private string _pathEntityName = "Path";
    private const string PullStandingPointTag = "pull";
    private const string RightStandingPointTag = "right";
    private const string LeftStandingPointTag = "left";
    public string IdleAnimation = "batteringram_idle";
    public string KnockAnimation = "batteringram_fire";
    public string KnockSlowerAnimation = "batteringram_fire_weak";
    public string KnockSlowestAnimation = "batteringram_fire_weakest";
    public float KnockAnimationHitProgress = 0.53f;
    public float KnockSlowerAnimationHitProgress = 0.6f;
    public float KnockSlowestAnimationHitProgress = 0.61f;
    private string _gateTag = "gate";
    public string RoofTag = "roof";
    public bool GhostEntityMove = true;
    public float GhostEntitySpeedMultiplier = 1f;
    private string _sideTag;
    private FormationAI.BehaviorSide _weaponSide;
    public float WheelDiameter = 1.3f;
    public int GateNavMeshId = 7;
    public int DisabledNavMeshID = 8;
    private int _bridgeNavMeshID_1 = 8;
    private int _bridgeNavMeshID_2 = 8;
    private int _ditchNavMeshID_1 = 9;
    private int _ditchNavMeshID_2 = 10;
    private int _groundToBridgeNavMeshID_1 = 12;
    private int _groundToBridgeNavMeshID_2 = 13;
    public int NavMeshIdToDisableOnDestination = -1;
    public float MinSpeed = 0.5f;
    public float MaxSpeed = 1f;
    public float DamageMultiplier = 10f;
    private int _usedPower;
    private float _storedPower;
    private List<StandingPoint> _pullStandingPoints;
    private List<MatrixFrame> _pullStandingPointLocalIKFrames;
    private GameEntity _ditchFillDebris;
    private GameEntity _batteringRamBody;
    private bool _isGhostMovementOn;
    private bool _isAllStandingPointsDisabled;
    private BatteringRam.RamState _state;
    private CastleGate _gate;
    private bool _hasArrivedAtTarget;

    public SiegeWeaponMovementComponent MovementComponent { get; private set; }

    public FormationAI.BehaviorSide WeaponSide => this._weaponSide;

    public string PathEntity => this._pathEntityName;

    public bool EditorGhostEntityMove => this.GhostEntityMove;

    public BatteringRam.RamState State
    {
      get => this._state;
      set
      {
        if (this._state == value)
          return;
        this._state = value;
      }
    }

    public MissionObject TargetCastlePosition => (MissionObject) this._gate;

    public bool HasCompletedAction()
    {
      if (this._gate == null || this._gate.IsDestroyed)
        return true;
      return this._gate.State == CastleGate.GateState.Open && this.HasArrivedAtTarget;
    }

    public float SiegeWeaponPriority => 25f;

    public int OverTheWallNavMeshID => this.GateNavMeshId;

    public bool HoldLadders => !this.MovementComponent.HasArrivedAtTarget;

    public bool SendLadders => this.MovementComponent.HasArrivedAtTarget;

    public bool HasArrivedAtTarget
    {
      get => this._hasArrivedAtTarget;
      set
      {
        if (!GameNetwork.IsClientOrReplay)
          this.MovementComponent.SetDestinationNavMeshIdState(!value);
        if (this._hasArrivedAtTarget == value)
          return;
        this._hasArrivedAtTarget = value;
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetBatteringRamHasArrivedAtTarget(this));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        else
        {
          if (!GameNetwork.IsClientOrReplay)
            return;
          this.MovementComponent.MoveToTargetAsClient();
        }
      }
    }

    public override void Disable()
    {
      base.Disable();
      if (GameNetwork.IsClientOrReplay)
        return;
      if (this.DisabledNavMeshID != 0)
        this.Scene.SetAbilityOfFacesWithId(this.DisabledNavMeshID, true);
      this.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 4, false);
    }

    public override SiegeEngineType GetSiegeEngineType() => DefaultSiegeEngineTypes.Ram;

    protected internal override void OnInit()
    {
      base.OnInit();
      DestructableComponent destructableComponent = this.GameEntity.GetScriptComponents<DestructableComponent>().FirstOrDefault<DestructableComponent>();
      if (destructableComponent != null)
        destructableComponent.BattleSide = BattleSideEnum.Attacker;
      this._state = BatteringRam.RamState.Stable;
      IEnumerable<GameEntity> source = this.Scene.FindEntitiesWithTag(this._gateTag).ToList<GameEntity>().Where<GameEntity>((Func<GameEntity, bool>) (ewgt => ewgt.HasScriptOfType<CastleGate>()));
      if (!source.IsEmpty<GameEntity>())
      {
        this._gate = source.First<GameEntity>().GetFirstScriptOfType<CastleGate>();
        this._gate.AttackerSiegeWeapon = (IPrimarySiegeWeapon) this;
      }
      this.AddRegularMovementComponent();
      this._batteringRamBody = this.GameEntity.GetChildren().FirstOrDefault<GameEntity>((Func<GameEntity, bool>) (x => x.HasTag("body")));
      this._batteringRamBody.Skeleton.SetAnimationAtChannel(this.IdleAnimation, 0, blendInPeriod: 0.0f);
      this._pullStandingPoints = new List<StandingPoint>();
      this._pullStandingPointLocalIKFrames = new List<MatrixFrame>();
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      if (this.StandingPoints != null)
      {
        foreach (StandingPoint standingPoint in this.StandingPoints)
        {
          standingPoint.AddComponent((UsableMissionObjectComponent) new ResetAnimationOnStopUsageComponent(ActionIndexCache.act_none));
          if (standingPoint.GameEntity.HasTag("pull"))
          {
            standingPoint.IsDeactivated = true;
            this._pullStandingPoints.Add(standingPoint);
            this._pullStandingPointLocalIKFrames.Add(standingPoint.GameEntity.GetGlobalFrame().TransformToLocal(globalFrame));
            standingPoint.AddComponent((UsableMissionObjectComponent) new ClearHandInverseKinematicsOnStopUsageComponent());
          }
        }
      }
      string sideTag = this._sideTag;
      this._weaponSide = sideTag == "left" ? FormationAI.BehaviorSide.Left : (sideTag == "middle" ? FormationAI.BehaviorSide.Middle : (sideTag == "right" ? FormationAI.BehaviorSide.Right : FormationAI.BehaviorSide.Middle));
      this._ditchFillDebris = this.Scene.FindEntitiesWithTag("ditch_filler").FirstOrDefault<GameEntity>((Func<GameEntity, bool>) (df => df.HasTag(this._sideTag)));
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    private void AddRegularMovementComponent()
    {
      this.MovementComponent = new SiegeWeaponMovementComponent()
      {
        PathEntityName = this.PathEntity,
        MinSpeed = this.MinSpeed,
        MaxSpeed = this.MaxSpeed,
        MainObject = (SynchedMissionObject) this,
        WheelDiameter = this.WheelDiameter,
        NavMeshIdToDisableOnDestination = this.NavMeshIdToDisableOnDestination,
        MovementSoundCodeID = SoundEvent.GetEventIdFromString("event:/mission/siege/batteringram/move"),
        GhostEntitySpeedMultiplier = this.GhostEntitySpeedMultiplier
      };
      this.AddComponent((UsableMissionObjectComponent) this.MovementComponent);
    }

    internal override void OnDeploymentStateChanged(bool isDeployed)
    {
      base.OnDeploymentStateChanged(isDeployed);
      if (!((NativeObject) this._ditchFillDebris != (NativeObject) null))
        return;
      this._ditchFillDebris.SetVisibilityExcludeParents(isDeployed);
      if (GameNetwork.IsClientOrReplay)
        return;
      if (isDeployed)
      {
        Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavMeshID_1, true);
        Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavMeshID_2, true);
        Mission.Current.Scene.SeparateFacesWithId(this._ditchNavMeshID_1, this._groundToBridgeNavMeshID_1);
        Mission.Current.Scene.SeparateFacesWithId(this._ditchNavMeshID_2, this._groundToBridgeNavMeshID_2);
        Mission.Current.Scene.MergeFacesWithId(this._bridgeNavMeshID_1, this._groundToBridgeNavMeshID_1, 0);
        Mission.Current.Scene.MergeFacesWithId(this._bridgeNavMeshID_2, this._groundToBridgeNavMeshID_2, 0);
      }
      else
      {
        Mission.Current.Scene.SeparateFacesWithId(this._bridgeNavMeshID_1, this._groundToBridgeNavMeshID_1);
        Mission.Current.Scene.SeparateFacesWithId(this._bridgeNavMeshID_2, this._groundToBridgeNavMeshID_2);
        Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavMeshID_1, false);
        Mission.Current.Scene.SetAbilityOfFacesWithId(this._bridgeNavMeshID_2, false);
        Mission.Current.Scene.MergeFacesWithId(this._ditchNavMeshID_1, this._groundToBridgeNavMeshID_1, 0);
        Mission.Current.Scene.MergeFacesWithId(this._ditchNavMeshID_2, this._groundToBridgeNavMeshID_2, 0);
      }
    }

    public MatrixFrame GetInitialFrame() => this.MovementComponent != null ? this.MovementComponent.GetInitialFrame() : this.GameEntity.GetGlobalFrame();

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.GameEntity.IsVisibleIncludeParents() ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this.GameEntity.IsVisibleIncludeParents())
        return;
      MatrixFrame globalFrame1 = this.GameEntity.GetGlobalFrame();
      for (int index = 0; index < this._pullStandingPoints.Count; ++index)
      {
        if (this._pullStandingPoints[index].HasUser)
          this._pullStandingPoints[index].UserAgent.SetHandInverseKinematicsFrameFromActionChannel(1, this._pullStandingPointLocalIKFrames[index], globalFrame1);
      }
      if (this.MovementComponent.HasArrivedAtTarget && !this.IsDeactivated)
      {
        int userCount = this.UserCount;
        if (userCount > 0)
        {
          float parameterAtChannel = this._batteringRamBody.Skeleton.GetAnimationParameterAtChannel(0);
          this.UpdateHitAnimationWithProgress((userCount - 1) / 2, parameterAtChannel);
        }
      }
      if (GameNetwork.IsClientOrReplay)
        return;
      if (this.MovementComponent.HasArrivedAtTarget && !this.HasArrivedAtTarget)
      {
        this.HasArrivedAtTarget = true;
        foreach (StandingPoint standingPoint in this.StandingPoints)
          standingPoint.SetIsDeactivatedSynched(standingPoint.GameEntity.HasTag("move"));
        if (this.DisabledNavMeshID != 0)
          this.GameEntity.Scene.SetAbilityOfFacesWithId(this.DisabledNavMeshID, false);
      }
      if (!this.MovementComponent.HasArrivedAtTarget)
        return;
      if (this._gate == null || this._gate.IsDestroyed || this._gate.IsGateOpen)
      {
        if (GameNetwork.IsClientOrReplay || this._isAllStandingPointsDisabled)
          return;
        foreach (UsableMissionObject standingPoint in this.StandingPoints)
          standingPoint.SetIsDeactivatedSynched(true);
        this._isAllStandingPointsDisabled = true;
      }
      else
      {
        if (this._isAllStandingPointsDisabled && !this.IsDeactivated)
        {
          foreach (UsableMissionObject standingPoint in this.StandingPoints)
            standingPoint.SetIsDeactivatedSynched(false);
          this._isAllStandingPointsDisabled = false;
        }
        int userCount = this.UserCount;
        switch (this.State)
        {
          case BatteringRam.RamState.Stable:
            if (userCount <= 0)
              break;
            this.State = BatteringRam.RamState.Hitting;
            this._usedPower = userCount;
            this._storedPower = 0.0f;
            this.StartHitAnimationWithProgress((userCount - 1) / 2, 0.0f);
            break;
          case BatteringRam.RamState.Hitting:
            if (userCount > 0 && this._gate != null && !this._gate.IsGateOpen)
            {
              int powerStage = (userCount - 1) / 2;
              float parameterAtChannel = this._batteringRamBody.Skeleton.GetAnimationParameterAtChannel(0);
              if ((this._usedPower - 1) / 2 != powerStage)
                this.StartHitAnimationWithProgress(powerStage, parameterAtChannel);
              this._usedPower = userCount;
              this._storedPower += (float) this._usedPower * dt;
              double animationHitProgress;
              switch (powerStage)
              {
                case 2:
                  animationHitProgress = (double) this.KnockSlowerAnimationHitProgress;
                  break;
                case 3:
                  animationHitProgress = (double) this.KnockAnimationHitProgress;
                  break;
                default:
                  animationHitProgress = (double) this.KnockSlowestAnimationHitProgress;
                  break;
              }
              float num = (float) animationHitProgress;
              string str;
              switch (powerStage)
              {
                case 2:
                  str = this.KnockSlowerAnimation;
                  break;
                case 3:
                  str = this.KnockAnimation;
                  break;
                default:
                  str = this.KnockSlowestAnimation;
                  break;
              }
              string animationName = str;
              if ((double) parameterAtChannel < (double) num)
                break;
              MatrixFrame globalFrame2 = this.GameEntity.GetGlobalFrame();
              this._gate.DestructionComponent.TriggerOnHit(this.PilotAgent, (int) (this._storedPower * this.DamageMultiplier / (parameterAtChannel * MBAnimation.GetAnimationDuration(animationName))), globalFrame2.origin, globalFrame2.rotation.f, in MissionWeapon.Invalid, (ScriptComponentBehaviour) this);
              this.State = BatteringRam.RamState.AfterHit;
              break;
            }
            this._batteringRamBody.GetFirstScriptOfType<SynchedMissionObject>().SetAnimationAtChannelSynched(this.IdleAnimation, 0);
            this.State = BatteringRam.RamState.Stable;
            break;
          case BatteringRam.RamState.AfterHit:
            if ((double) this._batteringRamBody.Skeleton.GetAnimationParameterAtChannel(0) <= 0.999000012874603)
              break;
            this.State = BatteringRam.RamState.Stable;
            break;
        }
      }
    }

    private void StartHitAnimationWithProgress(int powerStage, float progress)
    {
      string str;
      switch (powerStage)
      {
        case 1:
          str = this.KnockSlowerAnimation;
          break;
        case 2:
          str = this.KnockAnimation;
          break;
        default:
          str = this.KnockSlowestAnimation;
          break;
      }
      string animationName = str;
      this._batteringRamBody.GetFirstScriptOfType<SynchedMissionObject>().SetAnimationAtChannelSynched(animationName, 0);
      if ((double) progress > 0.0)
        this._batteringRamBody.GetFirstScriptOfType<SynchedMissionObject>().SetAnimationChannelParameterSynched(0, progress);
      foreach (StandingPoint standingPoint in this.StandingPoints.Where<StandingPoint>((Func<StandingPoint, bool>) (standingPoint => standingPoint.HasUser && standingPoint.GameEntity.HasTag("pull"))))
      {
        ActionIndexCache forStandingPoint = this.GetActionCodeForStandingPoint(standingPoint, powerStage);
        if (!standingPoint.UserAgent.SetActionChannel(1, forStandingPoint, startProgress: progress))
          standingPoint.UserAgent.StopUsingGameObject(false);
      }
    }

    private void UpdateHitAnimationWithProgress(int powerStage, float progress)
    {
      foreach (StandingPoint standingPoint in this.StandingPoints.Where<StandingPoint>((Func<StandingPoint, bool>) (standingPoint => standingPoint.HasUser && standingPoint.GameEntity.HasTag("pull"))))
      {
        ActionIndexCache forStandingPoint = this.GetActionCodeForStandingPoint(standingPoint, powerStage);
        if (standingPoint.UserAgent.GetCurrentAction(1) == forStandingPoint)
          standingPoint.UserAgent.SetCurrentActionProgress(1, progress);
      }
    }

    private ActionIndexCache GetActionCodeForStandingPoint(
      StandingPoint standingPoint,
      int powerStage)
    {
      bool flag = standingPoint.GameEntity.HasTag("right");
      ActionIndexCache actionIndexCache = ActionIndexCache.act_none;
      switch (powerStage)
      {
        case 0:
          actionIndexCache = flag ? BatteringRam.act_usage_batteringram_left_slowest : BatteringRam.act_usage_batteringram_right_slowest;
          break;
        case 1:
          actionIndexCache = flag ? BatteringRam.act_usage_batteringram_left_slower : BatteringRam.act_usage_batteringram_right_slower;
          break;
        case 2:
          actionIndexCache = flag ? BatteringRam.act_usage_batteringram_left : BatteringRam.act_usage_batteringram_right;
          break;
      }
      return actionIndexCache;
    }

    public override UsableMachineAIBase CreateAIBehaviourObject() => (UsableMachineAIBase) new BatteringRamAI(this);

    protected internal override void OnMissionReset()
    {
      base.OnMissionReset();
      this._state = BatteringRam.RamState.Stable;
      this._hasArrivedAtTarget = false;
      this._batteringRamBody.Skeleton.SetAnimationAtChannel(this.IdleAnimation, 0, blendInPeriod: 0.0f);
      foreach (StandingPoint standingPoint in this.StandingPoints)
        standingPoint.IsDeactivated = !standingPoint.GameEntity.HasTag("move");
    }

    public override bool ReadFromNetwork()
    {
      bool bufferReadValid = base.ReadFromNetwork();
      if (bufferReadValid)
      {
        bool flag = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);
        int num = GameNetworkMessage.ReadIntFromPacket(CompressionMission.BatteringRamStateCompressionInfo, ref bufferReadValid);
        if (bufferReadValid)
        {
          this.HasArrivedAtTarget = flag;
          this._state = (BatteringRam.RamState) num;
        }
      }
      return bufferReadValid;
    }

    public override void WriteToNetwork()
    {
      base.WriteToNetwork();
      GameNetworkMessage.WriteBoolToPacket(this.HasArrivedAtTarget);
      GameNetworkMessage.WriteIntToPacket((int) this.State, CompressionMission.BatteringRamStateCompressionInfo);
    }

    public override bool IsDeactivated => this._gate == null || this._gate.IsDestroyed || this._gate.State == CastleGate.GateState.Open && this.HasArrivedAtTarget || base.IsDeactivated;

    public void HighlightPath() => this.MovementComponent.HighlightPath();

    public void SwitchGhostEntityMovementMode(bool isGhostEnabled)
    {
      if (isGhostEnabled)
      {
        if (!this._isGhostMovementOn)
        {
          this.RemoveComponent((UsableMissionObjectComponent) this.MovementComponent);
          this.SetUpGhostEntity();
          this.GhostEntityMove = true;
          SiegeWeaponMovementComponent component = this.GetComponent<SiegeWeaponMovementComponent>();
          component.GhostEntitySpeedMultiplier *= 3f;
          component.SetGhostVisibility(true);
        }
        this._isGhostMovementOn = true;
      }
      else
      {
        if (this._isGhostMovementOn)
        {
          this.RemoveComponent((UsableMissionObjectComponent) this.MovementComponent);
          this.RemoveComponent((UsableMissionObjectComponent) this.GetComponent<PathLastNodeFixer>());
          this.AddRegularMovementComponent();
          this.MovementComponent.SetGhostVisibility(false);
        }
        this._isGhostMovementOn = false;
      }
    }

    internal void SetUpGhostEntity()
    {
      this.AddComponent((UsableMissionObjectComponent) new PathLastNodeFixer()
      {
        PathHolder = (IPathHolder) this
      });
      this.MovementComponent = new SiegeWeaponMovementComponent()
      {
        PathEntityName = this.PathEntity,
        MainObject = (SynchedMissionObject) this,
        GhostEntitySpeedMultiplier = this.GhostEntitySpeedMultiplier
      };
      this.AddComponent((UsableMissionObjectComponent) this.MovementComponent);
      this.MovementComponent.SetupGhostEntity();
    }

    public override string GetDescriptionText(GameEntity gameEntity = null) => new TextObject("{=MaBSSg7I}Battering Ram").ToString();

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      TextObject textObject = !usableGameObject.GameEntity.HasTag("pull") ? new TextObject("{=rwZAZSvX}({KEY}) Move") : new TextObject("{=1cnJtNTt}({KEY}) Pull");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override OrderType GetOrder(BattleSideEnum side)
    {
      if (side != BattleSideEnum.Attacker)
        return OrderType.AttackEntity;
      return this.HasCompletedAction() ? OrderType.Use : OrderType.FollowEntity;
    }

    public override TargetFlags GetTargetFlags()
    {
      TargetFlags targetFlags1 = TargetFlags.None;
      if (this.UserCount > 0)
        targetFlags1 |= TargetFlags.IsMoving;
      TargetFlags targetFlags2 = targetFlags1 | TargetFlags.IsFlammable | TargetFlags.IsSiegeEngine | TargetFlags.IsAttacker;
      if (this.Side == BattleSideEnum.Attacker && DebugSiegeBehaviour.DebugDefendState == DebugSiegeBehaviour.DebugStateDefender.DebugDefendersToRam)
        targetFlags2 |= TargetFlags.DebugThreat;
      if (this.HasCompletedAction() || this.IsDestroyed || this.IsDeactivated)
        targetFlags2 |= TargetFlags.NotAThreat;
      return targetFlags2;
    }

    public override float GetTargetValue(List<Vec3> weaponPos) => 300f * this.GetUserMultiplierOfWeapon() * this.GetDistanceMultiplierOfWeapon(weaponPos[0]) * this.GetHitpointMultiplierofWeapon();

    protected override float GetDistanceMultiplierOfWeapon(Vec3 weaponPos)
    {
      float betweenPositions = this.GetMinimumDistanceBetweenPositions(weaponPos);
      if ((double) betweenPositions < 100.0)
        return 1f;
      return (double) betweenPositions < 625.0 ? 0.8f : 0.6f;
    }

    public void SetSpawnedFromSpawner() => this._spawnedFromSpawner = true;

    public void AssignParametersFromSpawner(
      string gateTag,
      string sideTag,
      int bridgeNavMeshID_1,
      int bridgeNavMeshID_2,
      int ditchNavMeshID_1,
      int ditchNavMeshID_2,
      int groundToBridgeNavMeshID_1,
      int groundToBridgeNavMeshID_2,
      string pathEntityName)
    {
      this._gateTag = gateTag;
      this._sideTag = sideTag;
      this._bridgeNavMeshID_1 = bridgeNavMeshID_1;
      this._bridgeNavMeshID_2 = bridgeNavMeshID_2;
      this._ditchNavMeshID_1 = ditchNavMeshID_1;
      this._ditchNavMeshID_2 = ditchNavMeshID_2;
      this._groundToBridgeNavMeshID_1 = groundToBridgeNavMeshID_1;
      this._groundToBridgeNavMeshID_2 = groundToBridgeNavMeshID_2;
      this._pathEntityName = pathEntityName;
    }

    public enum RamState
    {
      Stable,
      Hitting,
      AfterHit,
      NumberOfStates,
    }
  }
}
