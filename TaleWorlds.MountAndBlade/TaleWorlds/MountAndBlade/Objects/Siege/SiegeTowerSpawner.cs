// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Objects.Siege.SiegeTowerSpawner
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
  public class SiegeTowerSpawner : SpawnerBase
  {
    private const float _modifierFactorUpperLimit = 1.2f;
    private const float _modifierFactorLowerLimit = 0.8f;
    [EditorVisibleScriptComponentVariable(false)]
    public MatrixFrame wait_pos_ground = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(true)]
    public string SideTag;
    [EditorVisibleScriptComponentVariable(true)]
    public string TargetWallSegmentTag = "";
    [EditorVisibleScriptComponentVariable(true)]
    public string PathEntityName = "Path";
    [EditorVisibleScriptComponentVariable(true)]
    public int SoilNavMeshID1 = -1;
    [EditorVisibleScriptComponentVariable(true)]
    public int SoilNavMeshID2 = -1;
    [EditorVisibleScriptComponentVariable(true)]
    public int DitchNavMeshID1 = -1;
    [EditorVisibleScriptComponentVariable(true)]
    public int DitchNavMeshID2 = -1;
    [EditorVisibleScriptComponentVariable(true)]
    public int GroundToSoilNavMeshID1 = -1;
    [EditorVisibleScriptComponentVariable(true)]
    public int GroundToSoilNavMeshID2 = -1;
    [EditorVisibleScriptComponentVariable(true)]
    public int SoilGenericNavMeshID = -1;
    [EditorVisibleScriptComponentVariable(true)]
    public int GroundGenericNavMeshID = -1;
    [EditorVisibleScriptComponentVariable(true)]
    public string AddOnDeployTag = "";
    [EditorVisibleScriptComponentVariable(true)]
    public string RemoveOnDeployTag = "";
    [EditorVisibleScriptComponentVariable(true)]
    public float RampRotationDegree;
    [EditorVisibleScriptComponentVariable(true)]
    public float BarrierLength = 1f;
    [EditorVisibleScriptComponentVariable(true)]
    public float SpeedModifierFactor = 1f;
    public bool EnableAutoGhostMovement;
    [EditorVisibleScriptComponentVariable(false)]
    [RestrictedAccess]
    public MatrixFrame ai_barrier_l = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(false)]
    [RestrictedAccess]
    public MatrixFrame ai_barrier_r = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(true)]
    public string BarrierTagToRemove = string.Empty;

    public float RampRotationRadian => this.RampRotationDegree * ((float) Math.PI / 180f);

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      this._spawnerEditorHelper = new SpawnerEntityEditorHelper((ScriptComponentBehaviour) this);
      this._spawnerEditorHelper.LockGhostParent = false;
      if (this._spawnerEditorHelper.IsValid)
      {
        this._spawnerEditorHelper.SetupGhostMovement(this.PathEntityName);
        this._spawnerEditorHelper.GivePermission("ramp", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.rotation, SpawnerEntityEditorHelper.Axis.x), new Action<float>(this.SetRampRotation));
        this._spawnerEditorHelper.GivePermission("ai_barrier_r", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.scale, SpawnerEntityEditorHelper.Axis.z), new Action<float>(this.SetAIBarrierRight));
        this._spawnerEditorHelper.GivePermission("ai_barrier_l", new SpawnerEntityEditorHelper.Permission(SpawnerEntityEditorHelper.PermissionType.scale, SpawnerEntityEditorHelper.Axis.z), new Action<float>(this.SetAIBarrierLeft));
      }
      this.OnEditorVariableChanged("RampRotationDegree");
      this.OnEditorVariableChanged("BarrierLength");
    }

    private void SetRampRotation(float unusedArgument)
    {
      MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ramp").GetFrame();
      Vec3 vec3 = new Vec3(-frame.rotation.u.y, frame.rotation.u.x);
      float z = frame.rotation.u.z;
      float num = (float) Math.Atan2((double) vec3.Length, (double) z);
      if ((double) vec3.x < 0.0)
        num = -num + 6.283185f;
      this.RampRotationDegree = num * 57.29578f;
    }

    private void SetAIBarrierRight(float barrierScale)
    {
      this.BarrierLength = barrierScale;
      MatrixFrame frame1 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_l").GetFrame();
      MatrixFrame frame2 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_r").GetFrame();
      frame1.rotation.u = frame2.rotation.u;
      this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_l", frame1, false);
    }

    private void SetAIBarrierLeft(float barrierScale)
    {
      this.BarrierLength = barrierScale;
      MatrixFrame frame1 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_l").GetFrame();
      MatrixFrame frame2 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_r").GetFrame();
      frame2.rotation.u = frame1.rotation.u;
      this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_r", frame2, false);
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this._spawnerEditorHelper.Tick(dt);
    }

    protected internal override void OnEditorVariableChanged(string variableName)
    {
      base.OnEditorVariableChanged(variableName);
      if (variableName == "PathEntityName")
        this._spawnerEditorHelper.SetupGhostMovement(this.PathEntityName);
      else if (variableName == "EnableAutoGhostMovement")
        this._spawnerEditorHelper.SetEnableAutoGhostMovement(this.EnableAutoGhostMovement);
      else if (variableName == "RampRotationDegree")
      {
        MatrixFrame frame = this._spawnerEditorHelper.GetGhostEntityOrChild("ramp").GetFrame();
        frame.rotation = Mat3.Identity;
        frame.rotation.RotateAboutSide(this.RampRotationRadian);
        this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ramp", frame);
      }
      else if (variableName == "BarrierLength")
      {
        MatrixFrame frame1 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_l").GetFrame();
        double num = (double) frame1.rotation.u.Normalize();
        frame1.rotation.u *= Math.Max(0.01f, MathF.Abs(this.BarrierLength));
        MatrixFrame frame2 = this._spawnerEditorHelper.GetGhostEntityOrChild("ai_barrier_r").GetFrame();
        frame2.rotation.u = frame1.rotation.u;
        this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_l", frame1);
        this._spawnerEditorHelper.ChangeStableChildMatrixFrameAndApply("ai_barrier_r", frame2);
      }
      else
      {
        if (!(variableName == "SpeedModifierFactor"))
          return;
        this.SpeedModifierFactor = MathF.Clamp(this.SpeedModifierFactor, 0.8f, 1.2f);
      }
    }

    protected internal override void OnPreInit()
    {
      base.OnPreInit();
      this._spawnerMissionHelper = new SpawnerEntityMissionHelper((SpawnerBase) this);
    }

    public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
    {
      SiegeTower firstScriptOfType = _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<SiegeTower>();
      firstScriptOfType.AddOnDeployTag = this.AddOnDeployTag;
      firstScriptOfType.RemoveOnDeployTag = this.RemoveOnDeployTag;
      firstScriptOfType.MaxSpeed *= this.SpeedModifierFactor;
      firstScriptOfType.MinSpeed *= this.SpeedModifierFactor;
      Mat3 identity = Mat3.Identity;
      identity.RotateAboutSide(this.RampRotationRadian);
      firstScriptOfType.AssignParametersFromSpawner(this.PathEntityName, this.TargetWallSegmentTag, this.SideTag, this.SoilNavMeshID1, this.SoilNavMeshID2, this.DitchNavMeshID1, this.DitchNavMeshID2, this.GroundToSoilNavMeshID1, this.GroundToSoilNavMeshID2, this.SoilGenericNavMeshID, this.GroundGenericNavMeshID, identity, this.BarrierTagToRemove);
    }
  }
}
