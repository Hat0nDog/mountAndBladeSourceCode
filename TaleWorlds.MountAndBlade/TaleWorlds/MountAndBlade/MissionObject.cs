// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MissionObject : ScriptComponentBehaviour
  {
    protected const int InsideNavMeshIdLocal = 1;
    protected const int EnterNavMeshIdLocal = 2;
    protected const int ExitNavMeshIdLocal = 3;
    protected const int BlockerNavMeshIdLocal = 4;
    [EditableScriptComponentVariable(true)]
    protected string NavMeshPrefabName = "";
    protected int DynamicNavmeshIdStart;

    private Mission Mission => Mission.Current;

    public MissionObjectId Id { get; set; }

    public bool IsDisabled { get; private set; }

    public MissionObject() => this.Id = new MissionObjectId(-1);

    public virtual void SetAbilityOfFaces(bool enabled)
    {
      if (this.DynamicNavmeshIdStart <= 0)
        return;
      this.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 1, enabled);
      this.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 2, enabled);
      this.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 3, enabled);
      this.GameEntity.Scene.SetAbilityOfFacesWithId(this.DynamicNavmeshIdStart + 4, enabled);
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      if (GameNetwork.IsClientOrReplay)
        return;
      this.AttachDynamicNavmeshToEntity();
      this.SetAbilityOfFaces((NativeObject) this.GameEntity != (NativeObject) null && this.GameEntity.IsVisibleIncludeParents());
    }

    protected virtual void AttachDynamicNavmeshToEntity()
    {
      if (this.NavMeshPrefabName.Length <= 0)
        return;
      this.DynamicNavmeshIdStart = Mission.Current.GetNextDynamicNavMeshIdStart();
      this.GameEntity.Scene.ImportNavigationMeshPrefab(this.NavMeshPrefabName, this.DynamicNavmeshIdStart);
      this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 1, false);
      this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 2, true);
      this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 3, true);
      this.GetEntityToAttachNavMeshFaces().AttachNavigationMeshFaces(this.DynamicNavmeshIdStart + 4, false, true);
      this.SetAbilityOfFaces((NativeObject) this.GameEntity != (NativeObject) null && this.GameEntity.GetPhysicsState());
    }

    protected virtual GameEntity GetEntityToAttachNavMeshFaces() => this.GameEntity;

    protected internal override bool OnCheckForProblems()
    {
      base.OnCheckForProblems();
      bool flag1 = false;
      List<GameEntity> children = new List<GameEntity>();
      children.Add(this.GameEntity);
      this.GameEntity.GetChildrenRecursive(ref children);
      bool flag2 = false;
      foreach (GameEntity gameEntity in children)
        flag2 = flag2 || gameEntity.HasPhysicsDefinitionWithoutFlags(1) && !gameEntity.PhysicsDescBodyFlag.HasAnyFlag<BodyFlags>(BodyFlags.CommonCollisionExcludeFlagsForMissile);
      Vec3 scaleVector = this.GameEntity.GetGlobalFrame().rotation.GetScaleVector();
      bool flag3 = (double) MBMath.Absf(scaleVector.x - scaleVector.y) >= 0.01 || (double) MBMath.Absf(scaleVector.x - scaleVector.z) >= 0.01;
      if (flag2 & flag3)
      {
        MBEditor.AddEntityWarning(this.GameEntity, "Mission object has non-uniform scale and physics object. This is not supported because any attached focusable item to this mesh will not work within this configuration.");
        flag1 = true;
      }
      return flag1;
    }

    protected internal override void OnPreInit()
    {
      base.OnPreInit();
      if (this.Mission != null)
      {
        int id = -1;
        bool createdAtRuntime;
        if (this.Mission.IsLoadingFinished)
        {
          createdAtRuntime = true;
          if (!GameNetwork.IsClientOrReplay)
            id = this.Mission.GetFreeRuntimeMissionObjectId();
        }
        else
        {
          createdAtRuntime = false;
          id = this.Mission.GetFreeSceneMissionObjectId();
        }
        this.Id = new MissionObjectId(id, createdAtRuntime);
        this.Mission.AddActiveMissionObject(this);
      }
      this.GameEntity.SetAsReplayEntity();
    }

    public override int GetHashCode() => this.Id.GetHashCode();

    protected internal virtual void OnMissionReset()
    {
    }

    public virtual void AfterMissionStart()
    {
    }

    protected internal virtual bool OnHit(
      Agent attackerAgent,
      int damage,
      Vec3 impactPosition,
      Vec3 impactDirection,
      in MissionWeapon weapon,
      ScriptComponentBehaviour attackerScriptComponentBehaviour,
      out bool reportDamage)
    {
      reportDamage = false;
      return false;
    }

    public void SetDisabled(bool isParentObject = false)
    {
      if (!GameNetwork.IsClientOrReplay)
        this.SetAbilityOfFaces(false);
      if (isParentObject && (NativeObject) this.GameEntity != (NativeObject) null)
      {
        List<GameEntity> children = new List<GameEntity>();
        this.GameEntity.GetChildrenRecursive(ref children);
        foreach (MissionObject missionObject in children.SelectMany<GameEntity, ScriptComponentBehaviour>((Func<GameEntity, IEnumerable<ScriptComponentBehaviour>>) (ac => ac.GetScriptComponents())).Where<ScriptComponentBehaviour>((Func<ScriptComponentBehaviour, bool>) (sc => sc is MissionObject)).Select<ScriptComponentBehaviour, MissionObject>((Func<ScriptComponentBehaviour, MissionObject>) (sc => sc as MissionObject)))
          missionObject.SetDisabled();
      }
      Mission.Current.DeactivateMissionObject(this);
      this.IsDisabled = true;
    }

    public void SetDisabledAndMakeInvisible(bool isParentObject = false)
    {
      if (isParentObject && (NativeObject) this.GameEntity != (NativeObject) null)
      {
        List<GameEntity> children = new List<GameEntity>();
        this.GameEntity.GetChildrenRecursive(ref children);
        foreach (MissionObject missionObject in children.SelectMany<GameEntity, ScriptComponentBehaviour>((Func<GameEntity, IEnumerable<ScriptComponentBehaviour>>) (ac => ac.GetScriptComponents())).Where<ScriptComponentBehaviour>((Func<ScriptComponentBehaviour, bool>) (sc => sc is MissionObject)).Select<ScriptComponentBehaviour, MissionObject>((Func<ScriptComponentBehaviour, MissionObject>) (sc => sc as MissionObject)))
          missionObject.SetDisabledAndMakeInvisible();
      }
      Mission.Current.DeactivateMissionObject(this);
      this.IsDisabled = true;
      if (!((NativeObject) this.GameEntity != (NativeObject) null))
        return;
      this.GameEntity.SetVisibilityExcludeParents(false);
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      if (!GameNetwork.IsClientOrReplay)
        this.SetAbilityOfFaces(false);
      if (this.Mission == null)
        return;
      this.Mission.OnMissionObjectRemoved(this, removeReason);
    }

    public virtual void OnEndMission()
    {
    }

    public bool CreatedAtRuntime => this.Id.CreatedAtRuntime;

    protected internal override bool MovesEntity() => true;

    public virtual void AddStuckMissile(GameEntity missileEntity) => this.GameEntity.AddChild(missileEntity);
  }
}
