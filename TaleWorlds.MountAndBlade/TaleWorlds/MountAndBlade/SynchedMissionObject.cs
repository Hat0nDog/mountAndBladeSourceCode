// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SynchedMissionObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class SynchedMissionObject : MissionObject
  {
    private SynchedMissionObject.SynchFlags _initialSynchFlags;
    private SynchedMissionObject.SynchState _synchState;
    private MatrixFrame _lastSynchedFrame;
    private MatrixFrame _firstFrame;
    private float _timer;
    private float _duration;

    public float RemainingDuration => this._duration - this._timer;

    public uint Color { get; private set; }

    public uint Color2 { get; private set; }

    public bool SynchronizeCompleted => this._synchState == SynchedMissionObject.SynchState.SynchronizeCompleted;

    protected internal override void OnInit()
    {
      base.OnInit();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => !this.SynchronizeCompleted ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      if (this.SynchronizeCompleted)
        return;
      MatrixFrame frame1 = this.GameEntity.GetFrame();
      if (this._synchState == SynchedMissionObject.SynchState.SynchronizePosition && this._lastSynchedFrame.origin.NearlyEquals(frame1.origin) || this._lastSynchedFrame.NearlyEquals(frame1))
      {
        this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeCompleted);
      }
      else
      {
        MatrixFrame frame2;
        frame2.origin = this._synchState == SynchedMissionObject.SynchState.SynchronizeFrameOverTime ? MBMath.Lerp(this._firstFrame.origin, this._lastSynchedFrame.origin, this._timer / this._duration, 0.2f * dt) : MBMath.Lerp(frame1.origin, this._lastSynchedFrame.origin, 8f * dt, 0.2f * dt);
        if (this._synchState == SynchedMissionObject.SynchState.SynchronizeFrame || this._synchState == SynchedMissionObject.SynchState.SynchronizeFrameOverTime)
        {
          frame2.rotation.s = MBMath.Lerp(frame1.rotation.s, this._lastSynchedFrame.rotation.s, 8f * dt, 0.2f * dt);
          frame2.rotation.f = MBMath.Lerp(frame1.rotation.f, this._lastSynchedFrame.rotation.f, 8f * dt, 0.2f * dt);
          frame2.rotation.u = MBMath.Lerp(frame1.rotation.u, this._lastSynchedFrame.rotation.u, 8f * dt, 0.2f * dt);
          if (frame2.origin != this._lastSynchedFrame.origin || frame2.rotation.s != this._lastSynchedFrame.rotation.s || (frame2.rotation.f != this._lastSynchedFrame.rotation.f || frame2.rotation.u != this._lastSynchedFrame.rotation.u))
          {
            frame2.rotation.Orthonormalize();
            if (this._lastSynchedFrame.rotation.HasScale())
              frame2.rotation.ApplyScaleLocal(this._lastSynchedFrame.rotation.GetScaleVector());
          }
          this.GameEntity.SetFrame(ref frame2);
        }
        else
          this.GameEntity.SetLocalPosition(frame2.origin);
        this._timer = Math.Min(this._timer + dt, this._duration);
      }
    }

    private void SetSynchState(SynchedMissionObject.SynchState newState)
    {
      if (newState == this._synchState)
        return;
      this._synchState = newState;
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public void SetLocalPositionSmoothStep(ref Vec3 targetPosition)
    {
      this._lastSynchedFrame.origin = targetPosition;
      this.SetSynchState(SynchedMissionObject.SynchState.SynchronizePosition);
    }

    public virtual void SetVisibleSynched(bool value, bool forceChildrenVisible = false)
    {
      bool flag = this.GameEntity.IsVisibleIncludeParents() != value;
      List<GameEntity> children = new List<GameEntity>();
      if (!flag & forceChildrenVisible)
      {
        this.GameEntity.GetChildrenRecursive(ref children);
        foreach (GameEntity gameEntity in children)
        {
          if (gameEntity.GetPhysicsState() != value)
          {
            flag = true;
            break;
          }
        }
      }
      if (!((NativeObject) this.GameEntity != (NativeObject) null & flag))
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectVisibility((MissionObject) this, value));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.GameEntity.SetVisibilityExcludeParents(value);
      if (!forceChildrenVisible)
        return;
      foreach (GameEntity gameEntity in children)
        gameEntity.SetVisibilityExcludeParents(value);
    }

    public virtual void SetPhysicsStateSynched(bool value, bool setChildren = true)
    {
    }

    public virtual void SetDisabledSynched()
    {
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectDisabled((MissionObject) this));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.SetDisabledAndMakeInvisible();
    }

    public void SetFrameSynched(ref MatrixFrame frame, bool isClient = false)
    {
      if (!(this.GameEntity.GetFrame() != frame) && this._synchState == SynchedMissionObject.SynchState.SynchronizeCompleted)
        return;
      this._duration = 0.0f;
      this._timer = 0.0f;
      if (GameNetwork.IsClientOrReplay)
      {
        this._lastSynchedFrame = frame;
        this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrame);
      }
      else
      {
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectFrame((MissionObject) this, ref frame));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeCompleted);
        this.GameEntity.SetFrame(ref frame);
        this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
      }
    }

    public void SetGlobalFrameSynched(ref MatrixFrame frame, bool isClient = false)
    {
      this._duration = 0.0f;
      this._timer = 0.0f;
      if (!(this.GameEntity.GetGlobalFrame() != frame))
        return;
      if (GameNetwork.IsClientOrReplay)
      {
        this._lastSynchedFrame = (NativeObject) this.GameEntity.Parent != (NativeObject) null ? this.GameEntity.Parent.GetGlobalFrame().TransformToLocalNonOrthogonal(ref frame) : frame;
        this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrame);
      }
      else
      {
        if (GameNetwork.IsServerOrRecorder)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectGlobalFrame((MissionObject) this, ref frame));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
        }
        this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeCompleted);
        this.GameEntity.SetGlobalFrame(frame);
        this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
      }
    }

    public void SetFrameSynchedOverTime(ref MatrixFrame frame, float duration, bool isClient = false)
    {
      if (!(this.GameEntity.GetFrame() != frame) && !duration.ApproximatelyEqualsTo(0.0f))
        return;
      this._firstFrame = this.GameEntity.GetFrame();
      this._lastSynchedFrame = frame;
      this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrameOverTime);
      this._duration = duration.ApproximatelyEqualsTo(0.0f) ? 0.1f : duration;
      this._timer = 0.0f;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectFrameOverTime((MissionObject) this, ref frame, duration));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
    }

    public void SetGlobalFrameSynchedOverTime(ref MatrixFrame frame, float duration, bool isClient = false)
    {
      if (!(this.GameEntity.GetGlobalFrame() != frame) && !duration.ApproximatelyEqualsTo(0.0f))
        return;
      this._firstFrame = this.GameEntity.GetFrame();
      this._lastSynchedFrame = (NativeObject) this.GameEntity.Parent != (NativeObject) null ? this.GameEntity.Parent.GetGlobalFrame().TransformToLocalNonOrthogonal(ref frame) : frame;
      this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrameOverTime);
      this._duration = duration.ApproximatelyEqualsTo(0.0f) ? 0.1f : duration;
      this._timer = 0.0f;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectGlobalFrameOverTime((MissionObject) this, ref frame, duration));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
    }

    public void SetAnimationAtChannelSynched(
      string animationName,
      int channelNo,
      float animationSpeed = 1f)
    {
      int animationIndexWithName1 = MBAnimation.GetAnimationIndexWithName(animationName);
      if (GameNetwork.IsServerOrRecorder)
      {
        int animationIndexWithName2 = MBAnimation.GetAnimationIndexWithName(this.GameEntity.Skeleton.GetAnimationAtChannel(channelNo));
        bool flag = true;
        int num = animationIndexWithName1;
        if (animationIndexWithName2 == num && this.GameEntity.Skeleton.GetAnimationSpeedAtChannel(channelNo).ApproximatelyEqualsTo(animationSpeed) && (double) this.GameEntity.Skeleton.GetAnimationParameterAtChannel(channelNo) < 0.0199999995529652)
          flag = false;
        if (flag)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectAnimationAtChannel((MissionObject) this, channelNo, animationIndexWithName1, animationSpeed));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
          this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchAnimation;
        }
      }
      this.GameEntity.Skeleton.SetAnimationAtChannel(animationIndexWithName1, channelNo, animationSpeed);
    }

    public void SetAnimationChannelParameterSynched(int channelNo, float parameter)
    {
      if (this.GameEntity.Skeleton.GetAnimationParameterAtChannel(channelNo).ApproximatelyEqualsTo(parameter))
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectAnimationChannelParameter((MissionObject) this, channelNo, parameter));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.GameEntity.Skeleton.SetAnimationParameterAtChannel(channelNo, parameter);
      this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchAnimation;
    }

    public void PauseSkeletonAnimationSynched()
    {
      if (this.GameEntity.IsSkeletonAnimationPaused())
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectAnimationPaused((MissionObject) this, true));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.GameEntity.PauseSkeletonAnimation();
      this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchAnimation;
    }

    public void ResumeSkeletonAnimationSynched()
    {
      if (!this.GameEntity.IsSkeletonAnimationPaused())
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectAnimationPaused((MissionObject) this, false));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.GameEntity.ResumeSkeletonAnimation();
      this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchAnimation;
    }

    public void BurstParticlesSynched(bool doChildren = true)
    {
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new BurstMissionObjectParticles((MissionObject) this, false));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.GameEntity.BurstEntityParticle(doChildren);
    }

    public void ApplyImpulseSynched(Vec3 position, Vec3 impulse, bool localSpace)
    {
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectImpulse((MissionObject) this, position, impulse, localSpace));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      if (localSpace)
        position += this.GameEntity.GlobalPosition;
      this.GameEntity.ApplyImpulseToDynamicBody(position, impulse);
      this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchTransform;
    }

    public void AddBodyFlagsSynched(BodyFlags flags, bool applyToChildren = true)
    {
      if ((this.GameEntity.BodyFlag & flags) == flags)
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new AddMissionObjectBodyFlags((MissionObject) this, flags, applyToChildren));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.GameEntity.AddBodyFlags(flags, applyToChildren);
      this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchBodyFlags;
    }

    public void RemoveBodyFlagsSynched(BodyFlags flags, bool applyToChildren = true)
    {
      if ((this.GameEntity.BodyFlag & flags) == BodyFlags.None)
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new RemoveMissionObjectBodyFlags((MissionObject) this, flags, applyToChildren));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.GameEntity.RemoveBodyFlags(flags, applyToChildren);
      this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SynchBodyFlags;
    }

    public void SetTeamColors(uint color, uint color2)
    {
      this.Color = color;
      this.Color2 = color2;
      this.GameEntity.SetColor(color, color2, "use_team_color");
    }

    public virtual void SetTeamColorsSynched(uint color, uint color2)
    {
      if (!((NativeObject) this.GameEntity != (NativeObject) null))
        return;
      if (GameNetwork.IsServerOrRecorder)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetMissionObjectColors((MissionObject) this, color, color2));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
      }
      this.SetTeamColors(color, color2);
      this._initialSynchFlags |= SynchedMissionObject.SynchFlags.SyncColors;
    }

    public virtual bool ReadFromNetwork()
    {
      bool bufferReadValid = true;
      this.GameEntity.SetVisibilityExcludeParents(GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid));
      if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
      {
        MatrixFrame frame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref bufferReadValid);
        this.GameEntity.SetFrame(ref frame);
        if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
        {
          this._firstFrame = this.GameEntity.GetFrame();
          this._lastSynchedFrame = GameNetworkMessage.ReadMatrixFrameFromPacket(ref bufferReadValid);
          this.SetSynchState(SynchedMissionObject.SynchState.SynchronizeFrameOverTime);
          this._duration = GameNetworkMessage.ReadFloatFromPacket(CompressionMission.FlagCapturePointDurationCompressionInfo, ref bufferReadValid);
          this._timer = 0.0f;
          if (this._duration.ApproximatelyEqualsTo(0.0f))
            this._duration = 0.1f;
        }
      }
      if ((NativeObject) this.GameEntity.Skeleton != (NativeObject) null && GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
      {
        int animationIndex = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.AnimationIndexCompressionInfo, ref bufferReadValid);
        float animationSpeedMultiplier = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.AnimationSpeedCompressionInfo, ref bufferReadValid);
        float parameter = GameNetworkMessage.ReadFloatFromPacket(CompressionBasic.UnitVectorCompressionInfo, ref bufferReadValid);
        this.GameEntity.Skeleton.SetAnimationAtChannel(animationIndex, 0, animationSpeedMultiplier, 0.0f);
        this.GameEntity.Skeleton.SetAnimationParameterAtChannel(0, parameter);
        if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
        {
          this.GameEntity.Skeleton.TickAnimationsAndForceUpdate(1f / 1000f, this.GameEntity.GetGlobalFrame(), true);
          this.GameEntity.PauseSkeletonAnimation();
        }
        else
          this.GameEntity.ResumeSkeletonAnimation();
      }
      if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
      {
        uint color1 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref bufferReadValid);
        uint color2 = GameNetworkMessage.ReadUintFromPacket(CompressionGeneric.ColorCompressionInfo, ref bufferReadValid);
        if (bufferReadValid)
          this.GameEntity.SetColor(color1, color2, "use_team_color");
      }
      if (GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid))
        this.SetDisabledAndMakeInvisible();
      return bufferReadValid;
    }

    public virtual void WriteToNetwork()
    {
      GameNetworkMessage.WriteBoolToPacket(this.GameEntity.GetVisibilityExcludeParents());
      GameNetworkMessage.WriteBoolToPacket(this._initialSynchFlags.HasAnyFlag<SynchedMissionObject.SynchFlags>(SynchedMissionObject.SynchFlags.SynchTransform));
      if (this._initialSynchFlags.HasAnyFlag<SynchedMissionObject.SynchFlags>(SynchedMissionObject.SynchFlags.SynchTransform))
      {
        GameNetworkMessage.WriteMatrixFrameToPacket(this.GameEntity.GetFrame());
        GameNetworkMessage.WriteBoolToPacket(this._synchState == SynchedMissionObject.SynchState.SynchronizeFrameOverTime);
        if (this._synchState == SynchedMissionObject.SynchState.SynchronizeFrameOverTime)
        {
          GameNetworkMessage.WriteMatrixFrameToPacket(this._lastSynchedFrame);
          GameNetworkMessage.WriteFloatToPacket(this._duration - this._timer, CompressionMission.FlagCapturePointDurationCompressionInfo);
        }
      }
      if ((NativeObject) this.GameEntity.Skeleton != (NativeObject) null)
      {
        string animationAtChannel = this.GameEntity.Skeleton.GetAnimationAtChannel(0);
        int num = !string.IsNullOrEmpty(animationAtChannel) ? 1 : 0;
        GameNetworkMessage.WriteBoolToPacket(num != 0 && this._initialSynchFlags.HasAnyFlag<SynchedMissionObject.SynchFlags>(SynchedMissionObject.SynchFlags.SynchAnimation));
        if (num != 0 && this._initialSynchFlags.HasAnyFlag<SynchedMissionObject.SynchFlags>(SynchedMissionObject.SynchFlags.SynchAnimation))
        {
          int animationIndexWithName = MBAnimation.GetAnimationIndexWithName(animationAtChannel);
          double animationSpeedAtChannel = (double) this.GameEntity.Skeleton.GetAnimationSpeedAtChannel(0);
          GameNetworkMessage.WriteIntToPacket(animationIndexWithName, CompressionBasic.AnimationIndexCompressionInfo);
          CompressionInfo.Float speedCompressionInfo = CompressionBasic.AnimationSpeedCompressionInfo;
          GameNetworkMessage.WriteFloatToPacket((float) animationSpeedAtChannel, speedCompressionInfo);
          GameNetworkMessage.WriteFloatToPacket(this.GameEntity.Skeleton.GetAnimationParameterAtChannel(0), CompressionBasic.UnitVectorCompressionInfo);
          GameNetworkMessage.WriteBoolToPacket(this.GameEntity.IsSkeletonAnimationPaused());
        }
      }
      GameNetworkMessage.WriteBoolToPacket(this._initialSynchFlags.HasAnyFlag<SynchedMissionObject.SynchFlags>(SynchedMissionObject.SynchFlags.SyncColors));
      if (this._initialSynchFlags.HasAnyFlag<SynchedMissionObject.SynchFlags>(SynchedMissionObject.SynchFlags.SyncColors))
      {
        GameNetworkMessage.WriteUintToPacket(this.Color, CompressionGeneric.ColorCompressionInfo);
        GameNetworkMessage.WriteUintToPacket(this.Color2, CompressionGeneric.ColorCompressionInfo);
      }
      GameNetworkMessage.WriteBoolToPacket(this.IsDisabled);
    }

    private enum SynchState
    {
      SynchronizeCompleted,
      SynchronizePosition,
      SynchronizeFrame,
      SynchronizeFrameOverTime,
    }

    [Flags]
    public enum SynchFlags : uint
    {
      SynchNone = 0,
      SynchTransform = 1,
      SynchAnimation = 2,
      SynchBodyFlags = 4,
      SyncColors = 8,
      SynchAll = 4294967295, // 0xFFFFFFFF
    }
  }
}
