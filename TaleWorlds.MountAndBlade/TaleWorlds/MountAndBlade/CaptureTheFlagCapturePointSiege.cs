// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CaptureTheFlagCapturePointSiege
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  internal class CaptureTheFlagCapturePointSiege : 
    SynchedMissionObject,
    IOrderableWithInteractionArea,
    IOrderable
  {
    public float InteractionDistance = 4f;
    protected float InteractionDistanceSquared;
    public float FlagRaiseTime = 30f;
    public float FlagLowerTime = 70f;
    private CaptureTheFlagCapturePointSiege.FlagStatusEnum _prevStatus;
    private CaptureTheFlagCapturePointSiege.FlagStatusEnum _status;
    private GameEntity _distanceIndicator;
    private Timer _flagSyncTimer;
    private bool _stopUpdating;

    private MatrixFrame CaptureFrame
    {
      get
      {
        MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
        globalFrame.Advance(this.InteractionDistance);
        return globalFrame;
      }
    }

    public bool CaptureTimeOutMode { get; set; }

    public event CaptureTheFlagCapturePointSiege.CapturedDelegate Captured;

    public CaptureTheFlagCapturePoint Flag { get; private set; }

    protected internal override void OnInit()
    {
      base.OnInit();
      if (Mission.Current == null)
        return;
      this.InteractionDistanceSquared = this.InteractionDistance * this.InteractionDistance;
      this.GameEntity.EntityFlags |= EntityFlags.NoOcclusionCulling;
      this.Flag = new CaptureTheFlagCapturePoint(this.GameEntity, BattleSideEnum.Defender, 0);
      this.UpdateIndicatorDecal();
      this.OnMissionReset();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected internal override void OnMissionReset()
    {
      base.OnMissionReset();
      this.CaptureTimeOutMode = false;
      this._status = CaptureTheFlagCapturePointSiege.FlagStatusEnum.None;
      this._stopUpdating = false;
      this.Flag.Reset();
      this._flagSyncTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 4f);
    }

    private void UpdateIndicatorDecal()
    {
      if (GameNetwork.IsDedicatedServer)
        return;
      this._distanceIndicator = this.GameEntity.GetChildren().SingleOrDefault<GameEntity>((Func<GameEntity, bool>) (q => q.HasTag("interaction_decal_mesh")));
      if (!((NativeObject) this._distanceIndicator != (NativeObject) null))
        return;
      MatrixFrame localFrame = this._distanceIndicator.GetFirstMesh().GetLocalFrame();
      localFrame.Scale(new Vec3(this.InteractionDistance, this.InteractionDistance, 1f));
      localFrame.origin += new Vec3(y: this.InteractionDistance);
      this._distanceIndicator.GetFirstMesh().SetLocalFrame(localFrame);
      this._distanceIndicator.SetBoundingboxDirty();
      this.GameEntity.SetBoundingboxDirty();
    }

    public int AttackingSideCount => Mission.Current.GetNearbyEnemyAgentCount(Mission.Current.DefenderTeam, this.CaptureFrame.origin.AsVec2, this.InteractionDistance);

    public int DefendingSideCount => Mission.Current.GetNearbyEnemyAgentCount(Mission.Current.AttackerTeam, this.CaptureFrame.origin.AsVec2, this.InteractionDistance);

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (GameNetwork.IsServer && !this._stopUpdating)
      {
        if (Mission.Current == null || Mission.Current.AttackerTeam == null || Mission.Current.DefenderTeam == null)
          return;
        int defendingSideCount = this.DefendingSideCount;
        int attackingSideCount = this.AttackingSideCount;
        this._status = attackingSideCount <= 0 || defendingSideCount != 0 ? (attackingSideCount <= 0 || defendingSideCount <= 0 ? (attackingSideCount != 0 || defendingSideCount <= 0 ? CaptureTheFlagCapturePointSiege.FlagStatusEnum.NoAttackersAndDefenders : CaptureTheFlagCapturePointSiege.FlagStatusEnum.DefendersOnly) : CaptureTheFlagCapturePointSiege.FlagStatusEnum.AttackersAndDefenders) : CaptureTheFlagCapturePointSiege.FlagStatusEnum.AttackersOnly;
        if (this._status != CaptureTheFlagCapturePointSiege.FlagStatusEnum.None)
        {
          float num = 0.0f;
          bool flag1 = false;
          bool flag2 = this._prevStatus != this._status;
          if (flag2)
            this._flagSyncTimer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission));
          switch (this._status)
          {
            case CaptureTheFlagCapturePointSiege.FlagStatusEnum.AttackersOnly:
              num = dt / this.FlagRaiseTime;
              this.Flag.Direction = CaptureTheFlagFlagDirection.Up;
              this.Flag.Speed = this.FlagRaiseTime;
              break;
            case CaptureTheFlagCapturePointSiege.FlagStatusEnum.DefendersOnly:
              num = -dt / this.FlagLowerTime;
              this.Flag.Direction = CaptureTheFlagFlagDirection.Down;
              this.Flag.Speed = this.FlagLowerTime;
              break;
            case CaptureTheFlagCapturePointSiege.FlagStatusEnum.AttackersAndDefenders:
              num = 0.0f;
              this.Flag.Direction = CaptureTheFlagFlagDirection.Static;
              break;
            case CaptureTheFlagCapturePointSiege.FlagStatusEnum.NoAttackersAndDefenders:
              num = -dt / this.FlagLowerTime;
              this.Flag.Direction = CaptureTheFlagFlagDirection.Down;
              this.Flag.Speed = this.FlagLowerTime;
              break;
          }
          this.Flag.Progress += num;
          if (flag1 || (double) this.Flag.Progress >= 1.0)
          {
            this.OnRaised();
            flag2 = true;
            this._stopUpdating = true;
            this.Flag.Direction = CaptureTheFlagFlagDirection.Static;
          }
          this.Flag.Progress = MBMath.ClampFloat(this.Flag.Progress, 0.0f, 1f);
          if (this.Flag.Direction == CaptureTheFlagFlagDirection.Up || this.Flag.Direction == CaptureTheFlagFlagDirection.Down)
            flag2 = flag2 || this._flagSyncTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission));
          if (flag2)
          {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage) new FlagRaisingStatus(this.Flag.Progress, this.Flag.Direction, this.Flag.Speed));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
          }
          MatrixFrame globalFrame = this.Flag.FlagHolder.GameEntity.GetGlobalFrame();
          globalFrame.origin.z = MBMath.Lerp(this.Flag.FlagBottomBoundary.GetGlobalFrame().origin.z, this.Flag.FlagTopBoundary.GetGlobalFrame().origin.z, 1f - this.Flag.Progress);
          this.Flag.FlagHolder.GameEntity.SetGlobalFrame(globalFrame);
        }
        this._prevStatus = this._status;
      }
      if (!GameNetwork.IsClientOrReplay || !this.Flag.UpdateFlag)
        return;
      MatrixFrame globalFrame1 = this.Flag.FlagHolder.GameEntity.GetGlobalFrame();
      globalFrame1.origin.z = MBMath.Lerp(this.Flag.FlagBottomBoundary.GetGlobalFrame().origin.z, this.Flag.FlagTopBoundary.GetGlobalFrame().origin.z, 1f - this.Flag.Progress);
      this.Flag.FlagHolder.GameEntity.SetGlobalFrame(globalFrame1);
      if (!MBMath.ApproximatelyEquals(this.Flag.Speed, 0.0f) && this.Flag.Direction != CaptureTheFlagFlagDirection.None && this.Flag.Direction != CaptureTheFlagFlagDirection.Static)
        this.Flag.Progress += (float) ((this.Flag.Direction == CaptureTheFlagFlagDirection.Up ? 1.0 : (this.Flag.Direction == CaptureTheFlagFlagDirection.Down ? -1.0 : 0.0)) * ((double) dt / (double) this.Flag.Speed));
      else
        this.Flag.UpdateFlag = false;
      this.Flag.Progress = MBMath.ClampFloat(this.Flag.Progress, 0.0f, 1f);
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      if (!MBEditor.IsEntitySelected(this.GameEntity))
        return;
      DebugExtensions.RenderDebugCircleOnTerrain(this.Scene, this.CaptureFrame, this.InteractionDistance, 2852192000U);
    }

    public void SetFlagMotion(float progress, CaptureTheFlagFlagDirection direction, float speed)
    {
      this.Flag.Progress = progress;
      this.Flag.Direction = direction;
      this.Flag.Speed = speed;
      this.Flag.UpdateFlag = true;
    }

    protected void OnRaised()
    {
      CaptureTheFlagCaptureResultEnum result = CaptureTheFlagCaptureResultEnum.AttackersWin;
      if (this.Captured != null)
        this.Captured(result);
      this._status = CaptureTheFlagCapturePointSiege.FlagStatusEnum.None;
    }

    public OrderType GetOrder(BattleSideEnum side) => OrderType.Move;

    public bool IsPointInsideInteractionArea(Vec3 point) => (double) this.CaptureFrame.origin.DistanceSquared(point) < (double) this.InteractionDistanceSquared;

    public bool IsOvertimeAllowed() => this._status != CaptureTheFlagCapturePointSiege.FlagStatusEnum.None && (this._status == CaptureTheFlagCapturePointSiege.FlagStatusEnum.AttackersAndDefenders || this._status == CaptureTheFlagCapturePointSiege.FlagStatusEnum.AttackersOnly || (this._status == CaptureTheFlagCapturePointSiege.FlagStatusEnum.OvertimeAttackersOnly || this._status == CaptureTheFlagCapturePointSiege.FlagStatusEnum.OvertimeAttackersAndDefenders));

    public void SynchronizeToPeer(NetworkCommunicator peer)
    {
      GameNetwork.BeginModuleEventAsServer(peer);
      GameNetwork.WriteMessage((GameNetworkMessage) new FlagRaisingStatus(this.Flag.Progress, this.Flag.Direction, this.Flag.Speed));
      GameNetwork.EndModuleEventAsServer();
    }

    private enum FlagStatusEnum
    {
      None = -1, // 0xFFFFFFFF
      AttackersOnly = 0,
      DefendersOnly = 1,
      AttackersAndDefenders = 2,
      NoAttackersAndDefenders = 3,
      OvertimeAttackersOnly = 4,
      OvertimeDefendersOnly = 5,
      OvertimeAttackersAndDefenders = 6,
      OvertimeNoAttackersAndDefenders = 7,
      Count = 8,
    }

    public delegate void CapturedDelegate(CaptureTheFlagCaptureResultEnum result);
  }
}
