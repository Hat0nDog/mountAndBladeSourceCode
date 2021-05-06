// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Objects.Siege.SiegeLadderSpawner
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
  public class SiegeLadderSpawner : SpawnerBase
  {
    [EditorVisibleScriptComponentVariable(false)]
    public MatrixFrame fork_holder = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(false)]
    public MatrixFrame initial_wait_pos = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(false)]
    public MatrixFrame use_push = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(false)]
    public MatrixFrame stand_position_wall_push = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(false)]
    public MatrixFrame distance_holder = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(false)]
    public MatrixFrame stand_position_ground_wait = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(true)]
    public string SideTag;
    [EditorVisibleScriptComponentVariable(true)]
    public string TargetWallSegmentTag = "";
    [EditorVisibleScriptComponentVariable(true)]
    public int OnWallNavMeshId = -1;
    [EditorVisibleScriptComponentVariable(true)]
    public string AddOnDeployTag = "";
    [EditorVisibleScriptComponentVariable(true)]
    public string RemoveOnDeployTag = "";
    [EditorVisibleScriptComponentVariable(true)]
    public float UpperStateRotationDegree;
    [EditorVisibleScriptComponentVariable(true)]
    public float DownStateRotationDegree = 90f;
    public float TacticalPositionWidth = 1f;
    [EditorVisibleScriptComponentVariable(true)]
    public string BarrierTagToRemove = string.Empty;
    [EditorVisibleScriptComponentVariable(true)]
    public string IndestructibleMerlonsTag = string.Empty;

    public float UpperStateRotationRadian => this.UpperStateRotationDegree * ((float) Math.PI / 180f);

    public float DownStateRotationRadian => this.DownStateRotationDegree * ((float) Math.PI / 180f);

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      this._spawnerEditorHelper = new SpawnerEntityEditorHelper((ScriptComponentBehaviour) this);
      if (this._spawnerEditorHelper.IsValid)
      {
        this._spawnerEditorHelper.GivePermission("ladder_up_state", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.rotation, SpawnerEntityEditorHelper.Axis.x), new Action<float>(this.OnLadderUpStateChange));
        this._spawnerEditorHelper.GivePermission("ladder_down_state", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.rotation, SpawnerEntityEditorHelper.Axis.x), new Action<float>(this.OnLadderDownStateChange));
      }
      this.OnEditorVariableChanged("UpperStateRotationDegree");
      this.OnEditorVariableChanged("DownStateRotationDegree");
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this._spawnerEditorHelper.Tick(dt);
    }

    private void OnLadderUpStateChange(float rotation)
    {
      if ((double) rotation > -0.201358318328857)
      {
        rotation = -0.2013583f;
        this.UpperStateRotationDegree = rotation * 57.29578f;
        this.OnEditorVariableChanged("UpperStateRotationDegree");
      }
      else
        this.UpperStateRotationDegree = rotation * 57.29578f;
    }

    private void OnLadderDownStateChange(float unusedArgument)
    {
      GameEntity ghostEntityOrChild = this._spawnerEditorHelper.GetGhostEntityOrChild("ladder_down_state");
      this.DownStateRotationDegree = Vec3.AngleBetweenTwoVectors(Vec3.Up, ghostEntityOrChild.GetFrame().rotation.u) * 57.29578f;
    }

    protected internal override void OnEditorVariableChanged(string variableName)
    {
      base.OnEditorVariableChanged(variableName);
      if (variableName == "UpperStateRotationDegree")
      {
        if ((double) this.UpperStateRotationDegree > -11.5369815826416)
          this.UpperStateRotationDegree = -11.53698f;
        MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ladder_up_state").GetFrame();
        frame.rotation = Mat3.Identity;
        frame.rotation.RotateAboutSide(this.UpperStateRotationRadian);
        this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ladder_up_state", frame);
      }
      else
      {
        if (!(variableName == "DownStateRotationDegree"))
          return;
        MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ladder_down_state").GetFrame();
        frame.rotation = Mat3.Identity;
        frame.rotation.RotateAboutUp(1.570796f);
        frame.rotation.RotateAboutSide(this.DownStateRotationRadian);
        this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ladder_down_state", frame);
      }
    }

    protected internal override void OnPreInit()
    {
      base.OnPreInit();
      this._spawnerMissionHelper = new SpawnerEntityMissionHelper((SpawnerBase) this);
    }

    public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
    {
      SiegeLadder firstScriptOfType = _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<SiegeLadder>();
      firstScriptOfType.AddOnDeployTag = this.AddOnDeployTag;
      firstScriptOfType.RemoveOnDeployTag = this.RemoveOnDeployTag;
      firstScriptOfType.AssignParametersFromSpawner(this.SideTag, this.TargetWallSegmentTag, this.OnWallNavMeshId, this.DownStateRotationRadian, this.UpperStateRotationRadian, this.BarrierTagToRemove, this.IndestructibleMerlonsTag);
      List<GameEntity> children = new List<GameEntity>();
      _spawnerMissionHelper.SpawnedEntity.GetChildrenRecursive(ref children);
      children.Find((Predicate<GameEntity>) (x => x.Name == "initial_wait_pos")).GetFirstScriptOfType<TacticalPosition>().SetWidthFromSpawner(this.TacticalPositionWidth);
    }
  }
}
