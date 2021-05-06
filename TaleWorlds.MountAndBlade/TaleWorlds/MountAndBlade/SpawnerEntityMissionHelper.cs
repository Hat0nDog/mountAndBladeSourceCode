// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SpawnerEntityMissionHelper
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects.Siege;

namespace TaleWorlds.MountAndBlade
{
  public class SpawnerEntityMissionHelper
  {
    private const string EnabledSuffix = "_enabled";
    public GameEntity SpawnedEntity;
    private GameEntity _ownerEntity;
    private SpawnerBase _spawner;
    private string _gameEntityName;
    private bool _fireVersion;

    public SpawnerEntityMissionHelper(SpawnerBase spawner, bool fireVersion = false)
    {
      this._spawner = spawner;
      this._fireVersion = fireVersion;
      this._ownerEntity = this._spawner.GameEntity;
      this._gameEntityName = this._ownerEntity.Name;
      if ((NativeObject) this.SpawnPrefab(this._ownerEntity, this.GetPrefabName()) != (NativeObject) null)
        this.SyncMatrixFrames();
      this._spawner.AssignParameters(this);
      this.CallSetSpawnedFromSpawnerOfScripts();
    }

    private GameEntity SpawnPrefab(GameEntity parent, string entityName)
    {
      this.SpawnedEntity = GameEntity.Instantiate(parent.Scene, entityName, false);
      this.SpawnedEntity.SetMobility(GameEntity.Mobility.dynamic);
      this.SpawnedEntity.EntityFlags |= EntityFlags.DontSaveToScene;
      parent.AddChild(this.SpawnedEntity);
      MatrixFrame identity = MatrixFrame.Identity;
      this.SpawnedEntity.SetFrame(ref identity);
      foreach (string tag in this._ownerEntity.Tags)
        this.SpawnedEntity.AddTag(tag);
      return this.SpawnedEntity;
    }

    private void RemoveChildEntity(GameEntity child)
    {
      child.CallScriptCallbacks();
      child.Remove(85);
    }

    private void SyncMatrixFrames()
    {
      List<GameEntity> children = new List<GameEntity>();
      this.SpawnedEntity.GetChildrenRecursive(ref children);
      foreach (GameEntity child in children)
      {
        if (SpawnerEntityMissionHelper.HasField((object) this._spawner, child.Name))
        {
          MatrixFrame fieldValue = (MatrixFrame) SpawnerEntityMissionHelper.GetFieldValue((object) this._spawner, child.Name);
          child.SetFrame(ref fieldValue);
        }
        if (SpawnerEntityMissionHelper.HasField((object) this._spawner, child.Name + "_enabled") && !(bool) SpawnerEntityMissionHelper.GetFieldValue((object) this._spawner, child.Name + "_enabled"))
          this.RemoveChildEntity(child);
      }
    }

    private void CallSetSpawnedFromSpawnerOfScripts()
    {
      foreach (GameEntity entityAndChild in this.SpawnedEntity.GetEntityAndChildren())
      {
        foreach (ScriptComponentBehaviour componentBehaviour in entityAndChild.GetScriptComponents().Where<ScriptComponentBehaviour>((Func<ScriptComponentBehaviour, bool>) (x => x is ISpawnable)))
          (componentBehaviour as ISpawnable).SetSpawnedFromSpawner();
      }
    }

    private string GetPrefabName()
    {
      string str;
      if (this._spawner.ToBeSpawnedOverrideName != "")
        str = this._spawner.ToBeSpawnedOverrideName;
      else
        str = this._gameEntityName.Remove(this._gameEntityName.Length - ((IEnumerable<string>) this._gameEntityName.Split('_')).Last<string>().Length - 1);
      if (this._fireVersion)
        str = !(this._spawner.ToBeSpawnedOverrideNameForFireVersion != "") ? str + "_fire" : this._spawner.ToBeSpawnedOverrideNameForFireVersion;
      return str;
    }

    private static object GetFieldValue(object src, string propName) => src.GetType().GetField(propName).GetValue(src);

    private static bool HasField(object obj, string propertyName) => obj.GetType().GetField(propertyName) != (FieldInfo) null;
  }
}
