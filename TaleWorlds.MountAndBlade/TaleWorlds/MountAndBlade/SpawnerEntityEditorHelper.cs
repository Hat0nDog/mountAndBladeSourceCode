// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SpawnerEntityEditorHelper
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class SpawnerEntityEditorHelper
  {
    private List<Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>>> _stableChildrenPermissions = new List<Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>>>();
    private ScriptComponentBehaviour spawner_;
    private List<KeyValuePair<string, MatrixFrame>> stableChildrenFrames = new List<KeyValuePair<string, MatrixFrame>>();
    public bool LockGhostParent = true;
    private bool _ghostMovementMode;
    private PathTracker _tracker;
    private float _ghostObjectPosition;
    private string _pathName;
    private bool _enableAutoGhostMovement;
    private List<GameEntity> _wheels = new List<GameEntity>();

    public bool IsValid { get; private set; }

    public GameEntity SpawnedGhostEntity { get; private set; }

    public SpawnerEntityEditorHelper(ScriptComponentBehaviour spawner)
    {
      this.spawner_ = spawner;
      if ((NativeObject) this.AddGhostEntity(this.spawner_.GameEntity, this.GetGhostName()) != (NativeObject) null)
      {
        this.SyncMatrixFrames(true);
        this.IsValid = true;
      }
      else
        spawner.GameEntity.RemoveScriptComponent(this.spawner_.ScriptComponent.Pointer, 11);
    }

    public GameEntity GetGhostEntityOrChild(string name)
    {
      if (this.SpawnedGhostEntity.Name == name)
        return this.SpawnedGhostEntity;
      List<GameEntity> children = new List<GameEntity>();
      this.SpawnedGhostEntity.GetChildrenRecursive(ref children);
      GameEntity gameEntity = children.FirstOrDefault<GameEntity>((Func<GameEntity, bool>) (x => x.Name == name));
      return (NativeObject) gameEntity != (NativeObject) null ? gameEntity : (GameEntity) null;
    }

    public void Tick(float dt)
    {
      if ((NativeObject) this.SpawnedGhostEntity.Parent != (NativeObject) this.spawner_.GameEntity)
      {
        this.IsValid = false;
        this.spawner_.GameEntity.RemoveScriptComponent(this.spawner_.ScriptComponent.Pointer, 12);
      }
      if (!this.IsValid)
        return;
      if (this.LockGhostParent)
      {
        bool flag = false;
        if (this.SpawnedGhostEntity.GetFrame() != MatrixFrame.Identity)
          flag = true;
        MatrixFrame identity = MatrixFrame.Identity;
        this.SpawnedGhostEntity.SetFrame(ref identity);
        if (flag)
          this.SpawnedGhostEntity.UpdateTriadFrameForEditor();
      }
      this.SyncMatrixFrames(false);
      if (!this._ghostMovementMode)
        return;
      this.UpdateGhostMovement(dt);
    }

    public void GivePermission(
      string childName,
      SpawnerEntityEditorHelper.Permission permission,
      Action<float> onChangeFunction)
    {
      this._stableChildrenPermissions.Add(Tuple.Create<string, SpawnerEntityEditorHelper.Permission, Action<float>>(childName, permission, onChangeFunction));
    }

    private void ApplyPermissions()
    {
      foreach (Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>> childrenPermission in this._stableChildrenPermissions)
      {
        Tuple<string, SpawnerEntityEditorHelper.Permission, Action<float>> item = childrenPermission;
        KeyValuePair<string, MatrixFrame> keyValuePair = this.stableChildrenFrames.Find((Predicate<KeyValuePair<string, MatrixFrame>>) (x => x.Key == item.Item1));
        MatrixFrame frame = this.GetGhostEntityOrChild(item.Item1).GetFrame();
        if (!frame.NearlyEquals(keyValuePair.Value))
        {
          switch (item.Item2.TypeOfPermission)
          {
            case SpawnerEntityEditorHelper.PermissionType.scale:
              if (frame.origin.NearlyEquals(keyValuePair.Value.origin, 0.0001f) && frame.rotation.f.NormalizedCopy().NearlyEquals(keyValuePair.Value.rotation.f.NormalizedCopy(), 0.0001f) && (frame.rotation.u.NormalizedCopy().NearlyEquals(keyValuePair.Value.rotation.u.NormalizedCopy(), 0.0001f) && frame.rotation.s.NormalizedCopy().NearlyEquals(keyValuePair.Value.rotation.s.NormalizedCopy(), 0.0001f)))
              {
                switch (item.Item2.PermittedAxis)
                {
                  case SpawnerEntityEditorHelper.Axis.x:
                    if (!frame.rotation.f.NearlyEquals(keyValuePair.Value.rotation.f))
                    {
                      this.ChangeStableChildMatrixFrame(item.Item1, frame);
                      item.Item3(frame.rotation.f.Length);
                      continue;
                    }
                    continue;
                  case SpawnerEntityEditorHelper.Axis.y:
                    if (!frame.rotation.s.NearlyEquals(keyValuePair.Value.rotation.s))
                    {
                      this.ChangeStableChildMatrixFrame(item.Item1, frame);
                      item.Item3(frame.rotation.s.Length);
                      continue;
                    }
                    continue;
                  case SpawnerEntityEditorHelper.Axis.z:
                    if (!frame.rotation.u.NearlyEquals(keyValuePair.Value.rotation.u))
                    {
                      this.ChangeStableChildMatrixFrame(item.Item1, frame);
                      item.Item3(frame.rotation.u.Length);
                      continue;
                    }
                    continue;
                  default:
                    continue;
                }
              }
              else
                continue;
            case SpawnerEntityEditorHelper.PermissionType.rotation:
              switch (item.Item2.PermittedAxis)
              {
                case SpawnerEntityEditorHelper.Axis.x:
                  if (!frame.rotation.f.NearlyEquals(keyValuePair.Value.rotation.f) && !frame.rotation.u.NearlyEquals(keyValuePair.Value.rotation.u) && frame.rotation.s.NearlyEquals(keyValuePair.Value.rotation.s))
                  {
                    this.ChangeStableChildMatrixFrame(item.Item1, frame);
                    item.Item3(frame.rotation.GetEulerAngles().x);
                    continue;
                  }
                  continue;
                case SpawnerEntityEditorHelper.Axis.y:
                  if (!frame.rotation.s.NearlyEquals(keyValuePair.Value.rotation.s) && !frame.rotation.u.NearlyEquals(keyValuePair.Value.rotation.u) && frame.rotation.f.NearlyEquals(keyValuePair.Value.rotation.f))
                  {
                    this.ChangeStableChildMatrixFrame(item.Item1, frame);
                    item.Item3(frame.rotation.GetEulerAngles().y);
                    continue;
                  }
                  continue;
                case SpawnerEntityEditorHelper.Axis.z:
                  if (!frame.rotation.f.NearlyEquals(keyValuePair.Value.rotation.f) && !frame.rotation.s.NearlyEquals(keyValuePair.Value.rotation.s) && frame.rotation.u.NearlyEquals(keyValuePair.Value.rotation.u))
                  {
                    this.ChangeStableChildMatrixFrame(item.Item1, frame);
                    item.Item3(frame.rotation.GetEulerAngles().z);
                    continue;
                  }
                  continue;
                default:
                  continue;
              }
            default:
              continue;
          }
        }
      }
    }

    private void ChangeStableChildMatrixFrame(string childName, MatrixFrame matrixFrame)
    {
      this.stableChildrenFrames.RemoveAll((Predicate<KeyValuePair<string, MatrixFrame>>) (x => x.Key == childName));
      this.stableChildrenFrames.Add(new KeyValuePair<string, MatrixFrame>(childName, matrixFrame));
      if (!SpawnerEntityEditorHelper.HasField((object) this.spawner_, childName, true))
        return;
      SpawnerEntityEditorHelper.SetSpawnerMatrixFrame((object) this.spawner_, childName, matrixFrame);
    }

    public void ChangeStableChildMatrixFrameAndApply(
      string childName,
      MatrixFrame matrixFrame,
      bool updateTriad = true)
    {
      this.ChangeStableChildMatrixFrame(childName, matrixFrame);
      this.GetGhostEntityOrChild(childName).SetFrame(ref matrixFrame);
      if (!updateTriad)
        return;
      this.SpawnedGhostEntity.UpdateTriadFrameForEditorForAllChildren();
    }

    private GameEntity AddGhostEntity(GameEntity parent, string entityName)
    {
      this.spawner_.GameEntity.RemoveAllChildren();
      this.SpawnedGhostEntity = GameEntity.Instantiate(parent.Scene, entityName, true);
      if ((NativeObject) this.SpawnedGhostEntity == (NativeObject) null)
        return (GameEntity) null;
      this.SpawnedGhostEntity.SetMobility(GameEntity.Mobility.dynamic);
      this.SpawnedGhostEntity.EntityFlags |= EntityFlags.DontSaveToScene;
      parent.AddChild(this.SpawnedGhostEntity);
      MatrixFrame identity = MatrixFrame.Identity;
      this.SpawnedGhostEntity.SetFrame(ref identity);
      this.GetChildrenInitialFrames();
      this.SpawnedGhostEntity.UpdateTriadFrameForEditorForAllChildren();
      return this.SpawnedGhostEntity;
    }

    private void SyncMatrixFrames(bool first)
    {
      this.ApplyPermissions();
      List<GameEntity> children = new List<GameEntity>();
      this.SpawnedGhostEntity.GetChildrenRecursive(ref children);
      foreach (GameEntity gameEntity in children)
      {
        GameEntity item = gameEntity;
        if (SpawnerEntityEditorHelper.HasField((object) this.spawner_, item.Name, false))
        {
          if (first)
          {
            MatrixFrame fieldValue = (MatrixFrame) SpawnerEntityEditorHelper.GetFieldValue((object) this.spawner_, item.Name);
            if (!fieldValue.IsZero)
              item.SetFrame(ref fieldValue);
          }
          else
            SpawnerEntityEditorHelper.SetSpawnerMatrixFrame((object) this.spawner_, item.Name, item.GetFrame());
        }
        else
        {
          MatrixFrame frame = this.stableChildrenFrames.Find((Predicate<KeyValuePair<string, MatrixFrame>>) (x => x.Key == item.Name)).Value;
          if (!frame.NearlyEquals(item.GetFrame()))
          {
            item.SetFrame(ref frame);
            this.SpawnedGhostEntity.UpdateTriadFrameForEditorForAllChildren();
          }
        }
      }
    }

    private void GetChildrenInitialFrames()
    {
      List<GameEntity> children = new List<GameEntity>();
      this.SpawnedGhostEntity.GetChildrenRecursive(ref children);
      foreach (GameEntity gameEntity in children)
      {
        if (!SpawnerEntityEditorHelper.HasField((object) this.spawner_, gameEntity.Name, false))
          this.stableChildrenFrames.Add(new KeyValuePair<string, MatrixFrame>(gameEntity.Name, gameEntity.GetFrame()));
      }
    }

    private string GetGhostName() => this.GetPrefabName() + "_ghost";

    private string GetPrefabName() => this.spawner_.GameEntity.Name.Remove(this.spawner_.GameEntity.Name.Length - ((IEnumerable<string>) this.spawner_.GameEntity.Name.Split('_')).Last<string>().Length - 1);

    public void SetupGhostMovement(string pathName)
    {
      this._ghostMovementMode = true;
      this._pathName = pathName;
      Path pathWithName = this.SpawnedGhostEntity.Scene.GetPathWithName(pathName);
      Vec3 scaleVector = this.SpawnedGhostEntity.GetFrame().rotation.GetScaleVector();
      this._tracker = new PathTracker(pathWithName, scaleVector);
      this._ghostObjectPosition = !((NativeObject) pathWithName != (NativeObject) null) ? 0.0f : pathWithName.GetTotalLength();
      this.SpawnedGhostEntity.UpdateTriadFrameForEditor();
      List<GameEntity> children = new List<GameEntity>();
      this.SpawnedGhostEntity.GetChildrenRecursive(ref children);
      this._wheels = children.Where<GameEntity>((Func<GameEntity, bool>) (x => x.HasTag("wheel"))).ToList<GameEntity>();
    }

    public void SetEnableAutoGhostMovement(bool enableAutoGhostMovement)
    {
      this._enableAutoGhostMovement = enableAutoGhostMovement;
      if (this._enableAutoGhostMovement || !this._tracker.IsValid)
        return;
      this._ghostObjectPosition = this._tracker.GetPathLength();
    }

    private void UpdateGhostMovement(float dt)
    {
      if (this._tracker.HasChanged)
      {
        this.SetupGhostMovement(this._pathName);
        this._tracker.Advance(this._tracker.GetPathLength());
      }
      if (this.spawner_.GameEntity.IsSelectedOnEditor() || this.SpawnedGhostEntity.IsSelectedOnEditor())
      {
        if (this._tracker.IsValid)
        {
          float num = 10f;
          if (Input.DebugInput.IsShiftDown())
            num = 1f;
          if (Input.DebugInput.IsKeyDown(InputKey.MouseScrollUp))
            this._ghostObjectPosition += dt * num;
          else if (Input.DebugInput.IsKeyDown(InputKey.MouseScrollDown))
            this._ghostObjectPosition -= dt * num;
          if (this._enableAutoGhostMovement)
          {
            this._ghostObjectPosition += dt * num;
            if ((double) this._ghostObjectPosition >= (double) this._tracker.GetPathLength())
              this._ghostObjectPosition = 0.0f;
          }
          this._ghostObjectPosition = MBMath.ClampFloat(this._ghostObjectPosition, 0.0f, this._tracker.GetPathLength());
        }
        else
          this._ghostObjectPosition = 0.0f;
      }
      if (this._tracker.IsValid)
      {
        MatrixFrame globalFrame = this.spawner_.GameEntity.GetGlobalFrame();
        this._tracker.Advance(0.0f);
        MatrixFrame frame;
        Vec3 color;
        this._tracker.CurrentFrameAndColor(out frame, out color);
        if (globalFrame != frame)
        {
          this.spawner_.GameEntity.SetGlobalFrame(frame);
          this.spawner_.GameEntity.UpdateTriadFrameForEditor();
        }
        this._tracker.Advance(this._ghostObjectPosition);
        this._tracker.CurrentFrameAndColor(out frame, out color);
        if (this._wheels.Count == 2)
          frame = this.LinearInterpolatedIK(ref this._tracker);
        if (globalFrame != frame)
        {
          this.SpawnedGhostEntity.SetGlobalFrame(frame);
          this.SpawnedGhostEntity.UpdateTriadFrameForEditor();
          this.SpawnedGhostEntity.GetGlobalFrame();
        }
        this._tracker.Reset();
      }
      else
      {
        if (!(this.SpawnedGhostEntity.GetGlobalFrame() != this.spawner_.GameEntity.GetGlobalFrame()))
          return;
        this.SpawnedGhostEntity.SetGlobalFrame(this.spawner_.GameEntity.GetGlobalFrame());
        this.SpawnedGhostEntity.UpdateTriadFrameForEditor();
      }
    }

    private MatrixFrame LinearInterpolatedIK(ref PathTracker pathTracker)
    {
      MatrixFrame frame;
      Vec3 color;
      pathTracker.CurrentFrameAndColor(out frame, out color);
      MatrixFrame frameForWheelsStatic = SiegeWeaponMovementComponent.FindGroundFrameForWheelsStatic(ref frame, 2.45f, 1.3f, this.SpawnedGhostEntity, this._wheels);
      return MatrixFrame.Lerp(frame, frameForWheelsStatic, color.x);
    }

    private static object GetFieldValue(object src, string propName) => src.GetType().GetField(propName).GetValue(src);

    private static bool HasField(object obj, string propertyName, bool findRestricted)
    {
      if (!(obj.GetType().GetField(propertyName) != (FieldInfo) null))
        return false;
      return findRestricted || obj.GetType().GetField(propertyName).GetCustomAttribute<RestrictedAccess>() == null;
    }

    private static bool SetSpawnerMatrixFrame(
      object target,
      string propertyName,
      MatrixFrame value)
    {
      value.Fill();
      FieldInfo field = target.GetType().GetField(propertyName);
      if (!(field != (FieldInfo) null))
        return false;
      field.SetValue(target, (object) value);
      return true;
    }

    public enum Axis
    {
      x,
      y,
      z,
    }

    public enum PermissionType
    {
      scale,
      rotation,
    }

    public struct Permission
    {
      public SpawnerEntityEditorHelper.PermissionType TypeOfPermission;
      public SpawnerEntityEditorHelper.Axis PermittedAxis;

      public Permission(
        SpawnerEntityEditorHelper.PermissionType permission,
        SpawnerEntityEditorHelper.Axis axis)
      {
        this.TypeOfPermission = permission;
        this.PermittedAxis = axis;
      }
    }
  }
}
