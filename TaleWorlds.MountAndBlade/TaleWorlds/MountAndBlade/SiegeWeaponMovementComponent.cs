// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeWeaponMovementComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class SiegeWeaponMovementComponent : UsableMissionObjectComponent
  {
    private static readonly ActionIndexCache act_usage_siege_machine_push = ActionIndexCache.Create(nameof (act_usage_siege_machine_push));
    private static readonly ActionIndexCache act_strike_bent_over = ActionIndexCache.Create(nameof (act_strike_bent_over));
    private const string WheelTag = "wheel";
    public const string GhostObjectTag = "ghost_object";
    public const string MoveStandingPointTag = "move";
    private float _wheelCircumference;
    private float _wheelDiameter;
    public float AxleLength = 2.45f;
    public int NavMeshIdToDisableOnDestination = -1;
    private float _ghostObjectPos;
    private List<GameEntity> _wheels;
    private List<StandingPoint> _standingPoints;
    private MatrixFrame[] _standingPointLocalIKFrames;
    private SoundEvent _movementSound;
    private bool _isMoveSoundPlaying;
    private Path _path;
    private PathTracker _pathTracker;
    private PathTracker _ghostEntityPathTracker;
    private float _advancementError;
    private float _lastSynchronizedDistance;

    public float CurrentSpeed { get; private set; }

    public float MinSpeed { get; set; }

    public float MaxSpeed { get; set; }

    public string PathEntityName { get; set; }

    public float GhostEntitySpeedMultiplier { get; set; }

    public float WheelDiameter
    {
      set
      {
        this._wheelDiameter = value;
        this._wheelCircumference = this._wheelDiameter * 3.141593f;
      }
    }

    public SynchedMissionObject MainObject { get; set; }

    public int MovementSoundCodeID { get; set; }

    protected internal override void OnAdded(Scene scene)
    {
      base.OnAdded(scene);
      this._path = scene.GetPathWithName(this.PathEntityName);
      Vec3 scaleVector = this.MainObject.GameEntity.GetFrame().rotation.GetScaleVector();
      this._wheels = this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("wheel");
      this._standingPoints = this.MainObject.GameEntity.CollectObjectsWithTag<StandingPoint>("move");
      this._pathTracker = new PathTracker(this._path, scaleVector);
      this._pathTracker.Reset();
      this.SetTargetFrame();
      MatrixFrame globalFrame = this.MainObject.GameEntity.GetGlobalFrame();
      this._standingPointLocalIKFrames = new MatrixFrame[this._standingPoints.Count];
      for (int index = 0; index < this._standingPoints.Count; ++index)
      {
        this._standingPointLocalIKFrames[index] = this._standingPoints[index].GameEntity.GetGlobalFrame().TransformToLocal(globalFrame);
        this._standingPoints[index].AddComponent((UsableMissionObjectComponent) new ClearHandInverseKinematicsOnStopUsageComponent());
      }
    }

    internal void HighlightPath()
    {
      MatrixFrame[] points = new MatrixFrame[this._path.NumberOfPoints];
      this._path.GetPoints(points);
      ref MatrixFrame local = ref points[0];
      for (int index = 1; index < this._path.NumberOfPoints; ++index)
      {
        MatrixFrame matrixFrame = points[index];
      }
    }

    internal void SetupGhostEntity()
    {
      Path pathWithName = this.MainObject.Scene.GetPathWithName(this.PathEntityName);
      Vec3 scaleVector = this.MainObject.GameEntity.GetFrame().rotation.GetScaleVector();
      this._pathTracker = new PathTracker(pathWithName, scaleVector);
      this._ghostEntityPathTracker = new PathTracker(pathWithName, scaleVector);
      this._ghostObjectPos = !((NativeObject) pathWithName != (NativeObject) null) ? 0.0f : pathWithName.GetTotalLength();
      this._wheels = this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("wheel");
    }

    private static bool HasEntityTreesFrameChanged(GameEntity entity) => entity.HasFrameChanged || entity.GetChildren().Any<GameEntity>((Func<GameEntity, bool>) (c => SiegeWeaponMovementComponent.HasEntityTreesFrameChanged(c)));

    private void SetPath()
    {
      Path pathWithName = this.MainObject.Scene.GetPathWithName(this.PathEntityName);
      Vec3 scaleVector = this.MainObject.GameEntity.GetFrame().rotation.GetScaleVector();
      this._pathTracker = new PathTracker(pathWithName, scaleVector);
      this._ghostEntityPathTracker = new PathTracker(pathWithName, scaleVector);
      this._ghostObjectPos = !((NativeObject) pathWithName != (NativeObject) null) ? 0.0f : pathWithName.GetTotalLength();
      this.UpdateGhostObject(0.0f);
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this.UpdateGhostObject(dt);
    }

    public void SetGhostVisibility(bool isVisible) => this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("ghost_object").FirstOrDefault<GameEntity>().SetVisibilityExcludeParents(isVisible);

    public void OnEditorInit()
    {
      this.SetPath();
      this._wheels = this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("wheel");
    }

    private void UpdateGhostObject(float dt)
    {
      if (this._pathTracker.HasChanged)
      {
        this.SetPath();
        this._pathTracker.Advance(this._pathTracker.GetPathLength());
        this._ghostEntityPathTracker.Advance(this._ghostEntityPathTracker.GetPathLength());
      }
      List<GameEntity> source = this.MainObject.GameEntity.CollectChildrenEntitiesWithTag("ghost_object");
      if (this.MainObject.GameEntity.IsSelectedOnEditor())
      {
        if (this._pathTracker.IsValid)
        {
          float num = 10f;
          if (Input.DebugInput.IsShiftDown())
            num = 1f;
          if (Input.DebugInput.IsKeyDown(InputKey.MouseScrollUp))
            this._ghostObjectPos += dt * num;
          else if (Input.DebugInput.IsKeyDown(InputKey.MouseScrollDown))
            this._ghostObjectPos -= dt * num;
          this._ghostObjectPos = MBMath.ClampFloat(this._ghostObjectPos, 0.0f, this._pathTracker.GetPathLength());
        }
        else
          this._ghostObjectPos = 0.0f;
      }
      if (source.IsEmpty<GameEntity>())
        return;
      GameEntity gameEntity = source.First<GameEntity>();
      if (this.MainObject is IPathHolder mainObject && mainObject.EditorGhostEntityMove)
      {
        if (!this._ghostEntityPathTracker.IsValid)
          return;
        this._ghostEntityPathTracker.Advance(0.05f * this.GhostEntitySpeedMultiplier);
        MatrixFrame identity = MatrixFrame.Identity;
        Vec3 zero = Vec3.Zero;
        MatrixFrame frame = this.LinearInterpolatedIK(ref this._ghostEntityPathTracker);
        gameEntity.SetGlobalFrame(frame);
        if (!this._ghostEntityPathTracker.HasReachedEnd)
          return;
        this._ghostEntityPathTracker.Reset();
      }
      else
      {
        if (!this._pathTracker.IsValid)
          return;
        this._pathTracker.Advance(this._ghostObjectPos);
        MatrixFrame frame = this.LinearInterpolatedIK(ref this._pathTracker);
        gameEntity.SetGlobalFrame(this.FindGroundFrameForWheels(ref frame));
        this._pathTracker.Reset();
      }
    }

    private void RotateWheels(float angleInRadian)
    {
      foreach (GameEntity wheel in this._wheels)
      {
        MatrixFrame frame = wheel.GetFrame();
        frame.rotation.RotateAboutSide(angleInRadian);
        wheel.SetFrame(ref frame);
      }
    }

    private MatrixFrame LinearInterpolatedIK(ref PathTracker pathTracker)
    {
      MatrixFrame frame;
      Vec3 color;
      pathTracker.CurrentFrameAndColor(out frame, out color);
      MatrixFrame groundFrameForWheels = this.FindGroundFrameForWheels(ref frame);
      return MatrixFrame.Lerp(frame, groundFrameForWheels, color.x);
    }

    public void SetDistanceTravelledAsClient(float distance) => this._advancementError = distance - this._pathTracker.TotalDistanceTravelled;

    public override bool IsOnTickRequired() => true;

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (this._ghostEntityPathTracker != null)
        this.UpdateGhostObject(dt);
      if (!this._pathTracker.PathExists() || this._pathTracker.HasReachedEnd)
      {
        this.CurrentSpeed = 0.0f;
        if (!GameNetwork.IsClientOrReplay)
        {
          foreach (UsableMissionObject standingPoint in this._standingPoints)
            standingPoint.SetIsDeactivatedSynched(true);
        }
      }
      else
      {
        int num1 = this._standingPoints.Count<StandingPoint>((Func<StandingPoint, bool>) (usableGameObject => usableGameObject.HasUser));
        if (num1 > 0)
        {
          int count = this._standingPoints.Count;
          this.CurrentSpeed = MBMath.Lerp(this.MinSpeed, this.MaxSpeed, (float) (num1 - 1) / (float) (count - 1));
          float animationBlendInPeriod = MBAnimation.GetAnimationBlendInPeriod(MBAnimation.GetAnimationIndexOfActionCode(MBGlobals.HumanWarriorActionSet, SiegeWeaponMovementComponent.act_usage_siege_machine_push));
          MatrixFrame globalFrame = this.MainObject.GameEntity.GetGlobalFrame();
          for (int index = 0; index < this._standingPoints.Count; ++index)
          {
            StandingPoint standingPoint = this._standingPoints[index];
            if (standingPoint.HasUser)
            {
              Agent userAgent = standingPoint.UserAgent;
              ActionIndexCache currentAction1 = userAgent.GetCurrentAction(0);
              ActionIndexCache currentAction2 = userAgent.GetCurrentAction(1);
              if (currentAction1 != SiegeWeaponMovementComponent.act_usage_siege_machine_push && !userAgent.SetActionChannel(0, SiegeWeaponMovementComponent.act_usage_siege_machine_push, actionSpeed: this.CurrentSpeed, blendInPeriod: (animationBlendInPeriod * this.CurrentSpeed)))
              {
                if (MBMath.IsBetween((int) userAgent.GetCurrentActionType(0), 44, 48))
                {
                  if (currentAction1 != SiegeWeaponMovementComponent.act_strike_bent_over)
                    userAgent.SetActionChannel(0, SiegeWeaponMovementComponent.act_strike_bent_over);
                }
                else if (!GameNetwork.IsClientOrReplay)
                  userAgent.StopUsingGameObject(false);
              }
              else if (currentAction2 != SiegeWeaponMovementComponent.act_usage_siege_machine_push && !userAgent.SetActionChannel(1, SiegeWeaponMovementComponent.act_usage_siege_machine_push, actionSpeed: this.CurrentSpeed, blendInPeriod: (animationBlendInPeriod * this.CurrentSpeed)))
              {
                if (MBMath.IsBetween((int) userAgent.GetCurrentActionType(1), 44, 48))
                {
                  if (currentAction2 != SiegeWeaponMovementComponent.act_strike_bent_over)
                    userAgent.SetActionChannel(1, SiegeWeaponMovementComponent.act_strike_bent_over);
                }
                else if (!GameNetwork.IsClientOrReplay)
                  userAgent.StopUsingGameObject(false);
              }
              else
              {
                userAgent.SetCurrentActionSpeed(0, this.CurrentSpeed);
                userAgent.SetCurrentActionSpeed(1, this.CurrentSpeed);
                if (currentAction1 == SiegeWeaponMovementComponent.act_usage_siege_machine_push)
                  standingPoint.UserAgent.SetHandInverseKinematicsFrameFromActionChannel(1, this._standingPointLocalIKFrames[index], globalFrame);
              }
            }
          }
        }
        else
          this.CurrentSpeed = this._advancementError;
        if (!this.CurrentSpeed.ApproximatelyEqualsTo(0.0f))
        {
          float deltaDistance = this.CurrentSpeed * dt;
          if (!this._advancementError.ApproximatelyEqualsTo(0.0f))
          {
            float num2 = 3f * this.CurrentSpeed * dt * (float) Math.Sign(this._advancementError);
            if ((double) Math.Abs(num2) >= (double) Math.Abs(this._advancementError))
            {
              num2 = this._advancementError;
              this._advancementError = 0.0f;
            }
            else
              this._advancementError -= num2;
            deltaDistance += num2;
          }
          this._pathTracker.Advance(deltaDistance);
          this.SetTargetFrame();
          this.RotateWheels((float) ((double) deltaDistance / (double) this._wheelCircumference * 2.0 * 3.14159274101257));
          if (GameNetwork.IsServerOrRecorder && (double) this._pathTracker.TotalDistanceTravelled - (double) this._lastSynchronizedDistance > 1.0)
          {
            this._lastSynchronizedDistance = this._pathTracker.TotalDistanceTravelled;
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage) new SetSiegeMachineMovementDistance(this.MainObject as UsableMachine, this._lastSynchronizedDistance));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
          }
        }
      }
      this.TickSound();
    }

    internal MatrixFrame GetInitialFrame()
    {
      PathTracker pathTracker = new PathTracker(this._path, Vec3.One);
      pathTracker.Reset();
      return this.LinearInterpolatedIK(ref pathTracker);
    }

    private void SetTargetFrame()
    {
      if (!this._pathTracker.PathExists())
        return;
      this.MainObject.GameEntity.SetGlobalFrame(this.LinearInterpolatedIK(ref this._pathTracker));
    }

    public MatrixFrame GetTargetFrame()
    {
      float distanceTravelled = this._pathTracker.TotalDistanceTravelled;
      this._pathTracker.Advance(1000000f);
      MatrixFrame currentFrame = this._pathTracker.CurrentFrame;
      this._pathTracker.Reset();
      this._pathTracker.Advance(distanceTravelled);
      return currentFrame;
    }

    public bool HasArrivedAtTarget => !this._pathTracker.PathExists() || this._pathTracker.HasReachedEnd;

    public void SetDestinationNavMeshIdState(bool enabled)
    {
      if (this.NavMeshIdToDisableOnDestination == -1)
        return;
      Mission.Current.Scene.SetAbilityOfFacesWithId(this.NavMeshIdToDisableOnDestination, enabled);
    }

    public void MoveToTargetAsClient()
    {
      if (!this._pathTracker.IsValid)
        return;
      float distanceTravelled = this._pathTracker.TotalDistanceTravelled;
      this._pathTracker.Advance(1000000f);
      this.SetTargetFrame();
      this.RotateWheels((float) (((double) this._pathTracker.TotalDistanceTravelled - (double) distanceTravelled) / (double) this._wheelCircumference * 2.0 * 3.14159274101257));
    }

    private void TickSound()
    {
      if ((double) this.CurrentSpeed > 0.0)
        this.PlayMovementSound();
      else
        this.StopMovementSound();
    }

    private void PlayMovementSound()
    {
      if (!this._isMoveSoundPlaying)
      {
        this._movementSound = SoundEvent.CreateEvent(this.MovementSoundCodeID, this.MainObject.GameEntity.Scene);
        this._movementSound.Play();
        this._isMoveSoundPlaying = true;
      }
      this._movementSound.SetPosition(this.MainObject.GameEntity.GlobalPosition);
    }

    private void StopMovementSound()
    {
      if (!this._isMoveSoundPlaying)
        return;
      this._movementSound.Stop();
      this._isMoveSoundPlaying = false;
    }

    protected internal override void OnMissionReset()
    {
      base.OnMissionReset();
      this.CurrentSpeed = 0.0f;
      this._lastSynchronizedDistance = 0.0f;
      this._advancementError = 0.0f;
      this._pathTracker.Reset();
      this.SetTargetFrame();
    }

    protected internal override bool ReadFromNetwork()
    {
      bool bufferReadValid = base.ReadFromNetwork();
      float num = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.PositionCompressionInfo, ref bufferReadValid);
      if (bufferReadValid)
      {
        this._pathTracker.TotalDistanceTravelled = num;
        this._pathTracker.TotalDistanceTravelled += 0.05f;
        this.SetTargetFrame();
      }
      return bufferReadValid;
    }

    protected internal override void WriteToNetwork() => GameNetworkMessage.WriteFloatToPacket(this._pathTracker.TotalDistanceTravelled, CompressionBasic.PositionCompressionInfo);

    private MatrixFrame FindGroundFrameForWheels(ref MatrixFrame frame) => SiegeWeaponMovementComponent.FindGroundFrameForWheelsStatic(ref frame, this.AxleLength, this._wheelDiameter, this.MainObject.GameEntity, this._wheels);

    public static MatrixFrame FindGroundFrameForWheelsStatic(
      ref MatrixFrame frame,
      float axleLength,
      float wheelDiameter,
      GameEntity gameEntity,
      List<GameEntity> wheels)
    {
      List<Vec3> vec3List = new List<Vec3>(wheels.Count * 2);
      bool visibilityExcludeParents = gameEntity.GetVisibilityExcludeParents();
      if (visibilityExcludeParents)
        gameEntity.SetVisibilityExcludeParents(false);
      Scene scene = gameEntity.Scene;
      foreach (GameEntity wheel in wheels)
      {
        Vec3 parent = frame.TransformToParent(wheel.GetFrame().origin);
        Vec3 position1 = parent + frame.rotation.s * axleLength + (float) ((double) wheelDiameter * 0.5 + 0.5) * frame.rotation.u;
        Vec3 position2 = parent - frame.rotation.s * axleLength + (float) ((double) wheelDiameter * 0.5 + 0.5) * frame.rotation.u;
        position1.z = scene.GetGroundHeightAtPosition(position1);
        position2.z = scene.GetGroundHeightAtPosition(position2);
        vec3List.Add(position1);
        vec3List.Add(position2);
      }
      if (visibilityExcludeParents)
        gameEntity.SetVisibilityExcludeParents(true);
      float num1 = 0.0f;
      float num2 = 0.0f;
      float num3 = 0.0f;
      float num4 = 0.0f;
      float num5 = 0.0f;
      Vec3 vec3_1 = new Vec3();
      foreach (Vec3 vec3_2 in vec3List)
        vec3_1 += vec3_2;
      Vec3 vec3_3 = vec3_1 / (float) vec3List.Count;
      foreach (Vec3 vec3_2 in vec3List)
      {
        Vec3 vec3_4 = vec3_2 - vec3_3;
        num1 += vec3_4.x * vec3_4.x;
        num2 += vec3_4.x * vec3_4.y;
        num3 += vec3_4.y * vec3_4.y;
        num4 += vec3_4.x * vec3_4.z;
        num5 += vec3_4.y * vec3_4.z;
      }
      float num6 = (float) ((double) num1 * (double) num3 - (double) num2 * (double) num2);
      float x = (float) ((double) num5 * (double) num2 - (double) num4 * (double) num3) / num6;
      float y = (float) ((double) num2 * (double) num4 - (double) num1 * (double) num5) / num6;
      MatrixFrame matrixFrame;
      matrixFrame.origin = vec3_3;
      matrixFrame.rotation.u = new Vec3(x, y, 1f);
      double num7 = (double) matrixFrame.rotation.u.Normalize();
      matrixFrame.rotation.f = frame.rotation.f;
      matrixFrame.rotation.f -= Vec3.DotProduct(matrixFrame.rotation.f, matrixFrame.rotation.u) * matrixFrame.rotation.u;
      double num8 = (double) matrixFrame.rotation.f.Normalize();
      matrixFrame.rotation.s = Vec3.CrossProduct(matrixFrame.rotation.f, matrixFrame.rotation.u);
      double num9 = (double) matrixFrame.rotation.s.Normalize();
      return matrixFrame;
    }
  }
}
