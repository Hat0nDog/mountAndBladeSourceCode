// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Objects.FlagCapturePoint
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects
{
  public class FlagCapturePoint : SynchedMissionObject
  {
    public const float PointRadius = 4f;
    public const float RadiusMultiplierForContestedArea = 1.5f;
    private const float TimeToTravelBetweenBoundaries = 10f;
    public int FlagIndex;
    private SynchedMissionObject _theFlag;
    private SynchedMissionObject _flagHolder;
    private GameEntity _flagBottomBoundary;
    private GameEntity _flagTopBoundary;
    private List<SynchedMissionObject> _flagDependentObjects;
    private CaptureTheFlagFlagDirection _currentDirection = CaptureTheFlagFlagDirection.None;

    [EditableScriptComponentVariable(false)]
    public Vec3 Position => this.GameEntity.GlobalPosition;

    public int FlagChar => 65 + this.FlagIndex;

    public bool IsContested => this._currentDirection == CaptureTheFlagFlagDirection.Down;

    public bool IsFullyRaised => this._currentDirection == CaptureTheFlagFlagDirection.None;

    public bool IsDeactivated => !this.GameEntity.IsVisibleIncludeParents();

    protected internal override void OnMissionReset() => this._currentDirection = CaptureTheFlagFlagDirection.None;

    public void ResetPointAsServer(uint defaultColor, uint defaultColor2)
    {
      MatrixFrame globalFrame = this._flagTopBoundary.GetGlobalFrame();
      this._flagHolder.SetGlobalFrameSynched(ref globalFrame);
      this.SetTeamColorsWithAllSynched(defaultColor, defaultColor2);
      this.SetVisibleWithAllSynched(true);
    }

    public void RemovePointAsServer() => this.SetVisibleWithAllSynched(false);

    protected internal override void OnInit()
    {
      this._flagHolder = this.GameEntity.CollectChildrenEntitiesWithTag("score_stand").SingleOrDefault<GameEntity>().GetScriptComponents<SynchedMissionObject>().SingleOrDefault<SynchedMissionObject>();
      this._theFlag = this._flagHolder.GameEntity.CollectChildrenEntitiesWithTag("flag_white").SingleOrDefault<GameEntity>().GetScriptComponents<SynchedMissionObject>().SingleOrDefault<SynchedMissionObject>();
      this._flagBottomBoundary = this.GameEntity.GetChildren().Single<GameEntity>((Func<GameEntity, bool>) (q => ((IEnumerable<string>) q.Tags).Contains<string>("flag_raising_bottom")));
      this._flagTopBoundary = this.GameEntity.GetChildren().Single<GameEntity>((Func<GameEntity, bool>) (q => ((IEnumerable<string>) q.Tags).Contains<string>("flag_raising_top")));
      if (GameNetwork.IsServerOrRecorder)
      {
        MatrixFrame globalFrame = this._flagTopBoundary.GetGlobalFrame();
        this._flagHolder.SetGlobalFrameSynched(ref globalFrame);
      }
      this._flagDependentObjects = new List<SynchedMissionObject>();
      foreach (GameEntity gameEntity in Mission.Current.Scene.FindEntitiesWithTag("depends_flag_" + (object) this.FlagIndex).ToList<GameEntity>())
        this._flagDependentObjects.Add(gameEntity.GetScriptComponents<SynchedMissionObject>().SingleOrDefault<SynchedMissionObject>());
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      if (!MBEditor.IsEntitySelected(this.GameEntity))
        return;
      DebugExtensions.RenderDebugCircleOnTerrain(this.Scene, this.GameEntity.GetGlobalFrame(), 4f, 2852192000U);
      DebugExtensions.RenderDebugCircleOnTerrain(this.Scene, this.GameEntity.GetGlobalFrame(), 6f, 2868838400U);
    }

    public void OnAfterTick(bool canOwnershipChange, out bool ownerTeamChanged)
    {
      ownerTeamChanged = false;
      if (!this._flagHolder.SynchronizeCompleted)
        return;
      bool flag = this._flagHolder.GameEntity.GlobalPosition.DistanceSquared(this._flagTopBoundary.GlobalPosition).ApproximatelyEqualsTo(0.0f);
      if (canOwnershipChange)
      {
        if (!flag)
          ownerTeamChanged = true;
        else
          this._currentDirection = CaptureTheFlagFlagDirection.None;
      }
      else
      {
        if (!flag)
          return;
        this._currentDirection = CaptureTheFlagFlagDirection.None;
      }
    }

    public void SetMoveFlag(CaptureTheFlagFlagDirection directionTo)
    {
      float duration = 10f - this._flagHolder.RemainingDuration;
      Debug.Print("Flag[ " + (object) this.FlagIndex + "]: " + (object) directionTo, color: Debug.DebugColor.Green, debugFilter: 64UL);
      this._currentDirection = directionTo;
      MatrixFrame frame;
      if (directionTo != CaptureTheFlagFlagDirection.Up)
      {
        if (directionTo != CaptureTheFlagFlagDirection.Down)
          throw new ArgumentOutOfRangeException(nameof (directionTo), (object) directionTo, (string) null);
        frame = this._flagBottomBoundary.GetFrame();
      }
      else
        frame = this._flagTopBoundary.GetFrame();
      this._flagHolder.SetFrameSynchedOverTime(ref frame, duration);
    }

    public void SetMoveNone()
    {
      this._currentDirection = CaptureTheFlagFlagDirection.None;
      MatrixFrame frame = this._flagHolder.GameEntity.GetFrame();
      this._flagHolder.SetFrameSynched(ref frame);
    }

    public void SetVisibleWithAllSynched(bool value, bool forceChildrenVisible = false)
    {
      this.SetVisibleSynched(value, forceChildrenVisible);
      foreach (SynchedMissionObject flagDependentObject in this._flagDependentObjects)
        flagDependentObject.SetVisibleSynched(value);
    }

    public void SetTeamColorsWithAllSynched(uint color, uint color2)
    {
      this._theFlag.SetTeamColorsSynched(color, color2);
      foreach (SynchedMissionObject flagDependentObject in this._flagDependentObjects)
        flagDependentObject.SetTeamColorsSynched(color, color2);
    }

    public uint GetFlagColor() => this._theFlag.Color;

    public uint GetFlagColor2() => this._theFlag.Color2;

    public float GetFlagProgress() => MathF.Clamp((float) (((double) this._theFlag.GameEntity.GlobalPosition.z - (double) this._flagBottomBoundary.GlobalPosition.z) / ((double) this._flagTopBoundary.GlobalPosition.z - (double) this._flagBottomBoundary.GlobalPosition.z)), 0.0f, 1f);
  }
}
