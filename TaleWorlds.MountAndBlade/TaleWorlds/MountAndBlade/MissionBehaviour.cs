// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionBehaviour
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MissionBehaviour : IMissionBehavior
  {
    public Mission Mission { get; internal set; }

    public IInputContext DebugInput => Input.DebugInput;

    public abstract MissionBehaviourType BehaviourType { get; }

    public virtual void OnAfterMissionCreated()
    {
    }

    public virtual void OnBehaviourInitialize()
    {
    }

    public virtual void OnCreated()
    {
    }

    public virtual void EarlyStart()
    {
    }

    public virtual void AfterStart()
    {
    }

    public virtual void OnMissileHit(Agent attacker, Agent victim, bool isCanceled)
    {
    }

    public virtual void OnMissileCollisionReaction(
      Mission.MissileCollisionReaction collisionReaction,
      Agent attackerAgent,
      Agent attachedAgent,
      sbyte attachedBoneIndex)
    {
    }

    public virtual void OnAgentCreated(Agent agent)
    {
    }

    public virtual void OnAgentBuild(Agent agent, Banner banner)
    {
    }

    public virtual void OnAgentHit(
      Agent affectedAgent,
      Agent affectorAgent,
      int damage,
      in MissionWeapon affectorWeapon)
    {
    }

    public virtual void OnScoreHit(
      Agent affectedAgent,
      Agent affectorAgent,
      WeaponComponentData attackerWeapon,
      bool isBlocked,
      float damage,
      float movementSpeedDamageModifier,
      float hitDistance,
      AgentAttackType attackType,
      float shotDifficulty,
      BoneBodyPartType victimHitBodyPart)
    {
    }

    public virtual void OnEarlyAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
    }

    public virtual void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
    }

    public virtual void OnAgentDeleted(Agent affectedAgent)
    {
    }

    public virtual void OnAgentFleeing(Agent affectedAgent)
    {
    }

    public virtual void OnAgentPanicked(Agent affectedAgent)
    {
    }

    public virtual void OnFocusGained(Agent agent, IFocusable focusableObject, bool isInteractable)
    {
    }

    public virtual void OnFocusLost(Agent agent, IFocusable focusableObject)
    {
    }

    public virtual void OnAddTeam(Team team)
    {
    }

    public virtual void AfterAddTeam(Team team)
    {
    }

    public virtual void OnAgentInteraction(Agent userAgent, Agent agent)
    {
    }

    public virtual void OnClearScene()
    {
    }

    public virtual void HandleOnCloseMission() => this.OnEndMission();

    protected virtual void OnEndMission()
    {
    }

    public virtual void OnRemoveBehaviour()
    {
    }

    public virtual void OnPreMissionTick(float dt)
    {
    }

    public virtual void OnPreDisplayMissionTick(float dt)
    {
    }

    public virtual void OnMissionTick(float dt)
    {
    }

    public virtual void OnAgentMount(Agent agent)
    {
    }

    public virtual void OnAgentDismount(Agent agent)
    {
    }

    public virtual bool IsThereAgentAction(Agent userAgent, Agent otherAgent) => false;

    public virtual void OnEntityRemoved(GameEntity entity)
    {
    }

    public virtual void OnObjectUsed(Agent userAgent, UsableMissionObject usedObject)
    {
    }

    public virtual void OnObjectStoppedBeingUsed(Agent userAgent, UsableMissionObject usedObject)
    {
    }

    public virtual void OnRenderingStarted()
    {
    }

    public virtual void OnMissionActivate()
    {
    }

    public virtual void OnMissionDeactivate()
    {
    }

    public virtual void OnMissionRestart()
    {
    }

    public virtual List<CompassItemUpdateParams> GetCompassTargets() => (List<CompassItemUpdateParams>) null;

    public virtual void OnAssignPlayerAsSergeantOfFormation(Agent agent)
    {
    }

    public virtual void OnFormationUnitsSpawned(Team team)
    {
    }

    protected internal virtual void OnGetAgentState(Agent agent, bool usedSurgery)
    {
    }

    public virtual void OnAgentAlarmedStateChanged(Agent agent, Agent.AIStateFlag flag)
    {
    }

    protected internal virtual void OnObjectDisabled(DestructableComponent destructionComponent)
    {
    }

    public virtual void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
    {
    }

    protected internal virtual void OnAgentControllerChanged(Agent agent)
    {
    }

    public virtual void OnItemPickup(Agent agent, SpawnedItemEntity item)
    {
    }

    public virtual void OnRegisterBlow(
      Agent attacker,
      Agent victim,
      GameEntity realHitEntity,
      Blow b,
      ref AttackCollisionData collisionData,
      in MissionWeapon attackerWeapon)
    {
    }

    public virtual void OnAgentShootMissile(
      Agent shooterAgent,
      EquipmentIndex weaponIndex,
      Vec3 position,
      Vec3 velocity,
      Mat3 orientation,
      bool hasRigidBody,
      int forcedMissileIndex)
    {
    }
  }
}
