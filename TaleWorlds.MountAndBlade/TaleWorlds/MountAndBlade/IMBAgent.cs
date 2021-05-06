// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBAgent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBAgent
  {
    [EngineMethod("get_movement_flags", false)]
    uint GetMovementFlags(UIntPtr agentPointer);

    [EngineMethod("set_movement_flags", false)]
    void SetMovementFlags(UIntPtr agentPointer, Agent.MovementControlFlag value);

    [EngineMethod("get_movement_input_vector", false)]
    Vec2 GetMovementInputVector(UIntPtr agentPointer);

    [EngineMethod("set_movement_input_vector", false)]
    void SetMovementInputVector(UIntPtr agentPointer, Vec2 value);

    [EngineMethod("get_collision_capsule", false)]
    void GetCollisionCapsule(UIntPtr agentPointer, ref CapsuleData value);

    [EngineMethod("set_attack_state", false)]
    void SetAttackState(UIntPtr agentPointer, int attackState);

    [EngineMethod("get_agent_visuals", false)]
    MBAgentVisuals GetAgentVisuals(UIntPtr agentPointer);

    [EngineMethod("get_event_control_flags", false)]
    uint GetEventControlFlags(UIntPtr agentPointer);

    [EngineMethod("set_event_control_flags", false)]
    void SetEventControlFlags(UIntPtr agentPointer, Agent.EventControlFlag eventflag);

    [EngineMethod("set_average_ping_in_milliseconds", false)]
    void SetAveragePingInMilliseconds(UIntPtr agentPointer, double averagePingInMilliseconds);

    [EngineMethod("set_look_agent", false)]
    void SetLookAgent(UIntPtr agentPointer, UIntPtr lookAtAgentPointer);

    [EngineMethod("get_look_agent", false)]
    Agent GetLookAgent(UIntPtr agentPointer);

    [EngineMethod("get_target_agent", false)]
    Agent GetTargetAgent(UIntPtr agentPointer);

    [EngineMethod("set_interaction_agent", false)]
    void SetInteractionAgent(UIntPtr agentPointer, UIntPtr interactionAgentPointer);

    [EngineMethod("set_look_to_point_of_interest", false)]
    void SetLookToPointOfInterest(UIntPtr agentPointer, Vec3 point);

    [EngineMethod("disable_look_to_point_of_interest", false)]
    void DisableLookToPointOfInterest(UIntPtr agentPointer);

    [EngineMethod("is_enemy", false)]
    bool IsEnemy(UIntPtr agentPointer1, UIntPtr agentPointer2);

    [EngineMethod("is_friend", false)]
    bool IsFriend(UIntPtr agentPointer1, UIntPtr agentPointer2);

    [EngineMethod("get_agent_flags", false)]
    uint GetAgentFlags(UIntPtr agentPointer);

    [EngineMethod("set_agent_flags", false)]
    void SetAgentFlags(UIntPtr agentPointer, uint agentFlags);

    [EngineMethod("get_stepped_entity_id", false)]
    UIntPtr GetSteppedEntityId(UIntPtr agentPointer);

    [EngineMethod("set_network_peer", false)]
    void SetNetworkPeer(UIntPtr agentPointer, int networkPeerIndex);

    [EngineMethod("die", false)]
    void Die(UIntPtr agentPointer, ref Blow b, sbyte overrideKillInfo);

    [EngineMethod("make_dead", false)]
    void MakeDead(UIntPtr agentPointer, bool isKilled, int actionIndex);

    [EngineMethod("set_formation_frame_disabled", false)]
    void SetFormationFrameDisabled(UIntPtr agentPointer);

    [EngineMethod("set_formation_frame_enabled", false)]
    bool SetFormationFrameEnabled(
      UIntPtr agentPointer,
      WorldPosition position,
      Vec2 direction,
      float formationDirectionEnforcingFactor);

    [EngineMethod("set_should_catch_up_with_formation", false)]
    void SetShouldCatchUpWithFormation(UIntPtr agentPointer, bool value);

    [EngineMethod("set_formation_integrity_data", false)]
    void SetFormationIntegrityData(
      UIntPtr agentPointer,
      Vec2 position,
      Vec2 currentFormationDirection,
      Vec2 averageVelocityOfCloseAgents,
      float averageMaxUnlimitedSpeedOfCloseAgents,
      float deviationOfPositions);

    [EngineMethod("set_formation_neighborhood_data", false)]
    void SetFormationNeighborhoodData(
      UIntPtr agentPointer,
      int[] neighborAgentIndices,
      Vec2[] neighborAgentLocalDifferences);

    [EngineMethod("set_formation_info", false)]
    void SetFormationInfo(
      UIntPtr agentPointer,
      int fileIndex,
      int rankIndex,
      int fileCount,
      int rankCount,
      Vec2 wallDir,
      int unitSpacing);

    [EngineMethod("set_retreat_mode", false)]
    void SetRetreatMode(UIntPtr agentPointer, WorldPosition retreatPos, bool retreat);

    [EngineMethod("is_retreating", false)]
    bool IsRetreating(UIntPtr agentPointer);

    [EngineMethod("is_fading_out", false)]
    bool IsFadingOut(UIntPtr agentPointer);

    [EngineMethod("start_fading_out", false)]
    void StartFadingOut(UIntPtr agentPointer);

    [EngineMethod("get_retreat_pos", false)]
    WorldPosition GetRetreatPos(UIntPtr agentPointer);

    [EngineMethod("get_team", false)]
    int GetTeam(UIntPtr agentPointer);

    [EngineMethod("set_team", false)]
    void SetTeam(UIntPtr agentPointer, int teamIndex);

    [EngineMethod("set_courage", false)]
    void SetCourage(UIntPtr agentPointer, float courage);

    [EngineMethod("update_driven_properties", false)]
    void UpdateDrivenProperties(UIntPtr agentPointer, float[] values);

    [EngineMethod("get_look_direction", false)]
    Vec3 GetLookDirection(UIntPtr agentPointer);

    [EngineMethod("set_look_direction", false)]
    void SetLookDirection(UIntPtr agentPointer, Vec3 lookDirection);

    [EngineMethod("get_look_down_limit", false)]
    float GetLookDownLimit(UIntPtr agentPointer);

    [EngineMethod("get_position", false)]
    Vec3 GetPosition(UIntPtr agentPointer);

    [EngineMethod("set_position", false)]
    void SetPosition(UIntPtr agentPointer, ref Vec3 position);

    [EngineMethod("get_rotation_frame", false)]
    void GetRotationFrame(UIntPtr agentPointer, ref MatrixFrame outFrame);

    [EngineMethod("get_eye_global_height", false)]
    float GetEyeGlobalHeight(UIntPtr agentPointer);

    [EngineMethod("get_movement_velocity", false)]
    Vec2 GetMovementVelocity(UIntPtr agentPointer);

    [EngineMethod("get_average_velocity", false)]
    Vec3 GetAverageVelocity(UIntPtr agentPointer);

    [EngineMethod("get_is_left_stance", false)]
    bool GetIsLeftStance(UIntPtr agentPointer);

    [EngineMethod("reset_ai", false)]
    void ResetAI(UIntPtr agentPointer);

    [EngineMethod("get_ai_state_flags", false)]
    Agent.AIStateFlag GetAIStateFlags(UIntPtr agentPointer);

    [EngineMethod("set_ai_state_flags", false)]
    void SetAIStateFlags(UIntPtr agentPointer, Agent.AIStateFlag aiStateFlags);

    [EngineMethod("get_state_flags", false)]
    AgentState GetStateFlags(UIntPtr agentPointer);

    [EngineMethod("set_state_flags", false)]
    void SetStateFlags(UIntPtr agentPointer, AgentState StateFlags);

    [EngineMethod("get_mount_agent", false)]
    Agent GetMountAgent(UIntPtr agentPointer);

    [EngineMethod("set_mount_agent", false)]
    void SetMountAgent(UIntPtr agentPointer, int mountAgentIndex);

    [EngineMethod("get_agent_current_morale", false)]
    float GetAgentCurrentMorale(UIntPtr agentPointer);

    [EngineMethod("set_agent_current_morale", false)]
    void SetAgentCurrentMorale(UIntPtr agentPointer, float morale);

    [EngineMethod("set_always_attack_in_melee", false)]
    void SetAlwaysAttackInMelee(UIntPtr agentPointer, bool attack);

    [EngineMethod("get_rider_agent", false)]
    Agent GetRiderAgent(UIntPtr agentPointer);

    [EngineMethod("set_controller", false)]
    void SetController(UIntPtr agentPointer, Agent.ControllerType controller);

    [EngineMethod("get_controller", false)]
    Agent.ControllerType GetController(UIntPtr agentPointer);

    [EngineMethod("set_initial_frame", false)]
    void SetInitialFrame(UIntPtr agentPointer, ref MatrixFrame initialFrame);

    [EngineMethod("weapon_equipped", false)]
    void WeaponEquipped(
      UIntPtr agentPointer,
      int equipmentSlot,
      in WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      in WeaponData ammoWeaponData,
      WeaponStatsData[] ammoWeaponStatsData,
      int ammoWeaponStatsDataLength,
      UIntPtr weaponEntity,
      bool removeOldWeaponFromScene,
      bool isWieldedOnSpawn);

    [EngineMethod("drop_item", false)]
    void DropItem(UIntPtr agentPointer, int itemIndex, int pickedUpItemType);

    [EngineMethod("set_weapon_amount_in_slot", false)]
    void SetWeaponAmountInSlot(
      UIntPtr agentPointer,
      int equipmentSlot,
      short amount,
      bool enforcePrimaryItem);

    [EngineMethod("clear_equipment", false)]
    void ClearEquipment(UIntPtr agentPointer);

    [EngineMethod("get_wielded_item_index", false)]
    EquipmentIndex GetWieldedItemIndex(UIntPtr agentPointer, int handIndex);

    [EngineMethod("set_wielded_item_index_as_client", false)]
    void SetWieldedItemIndexAsClient(
      UIntPtr agentPointer,
      int handIndex,
      int wieldedItemIndex,
      bool isWieldedInstantly,
      bool isWieldedOnSpawn,
      int mainHandCurrentUsageIndex);

    [EngineMethod("set_usage_index_of_weapon_in_slot_as_client", false)]
    void SetUsageIndexOfWeaponInSlotAsClient(UIntPtr agentPointer, int slotIndex, int usageIndex);

    [EngineMethod("set_weapon_hit_points_in_slot", false)]
    void SetWeaponHitPointsInSlot(UIntPtr agentPointer, int wieldedItemIndex, short hitPoints);

    [EngineMethod("set_weapon_ammo_as_client", false)]
    void SetWeaponAmmoAsClient(
      UIntPtr agentPointer,
      int equipmentIndex,
      int ammoEquipmentIndex,
      short ammo);

    [EngineMethod("set_weapon_reload_phase_as_client", false)]
    void SetWeaponReloadPhaseAsClient(
      UIntPtr agentPointer,
      int wieldedItemIndex,
      short reloadPhase);

    [EngineMethod("set_reload_ammo_in_slot", false)]
    void SetReloadAmmoInSlot(
      UIntPtr agentPointer,
      int slotIndex,
      int ammoSlotIndex,
      short reloadedAmmo);

    [EngineMethod("start_switching_weapon_usage_index_as_client", false)]
    void StartSwitchingWeaponUsageIndexAsClient(
      UIntPtr agentPointer,
      int wieldedItemIndex,
      int usageIndex,
      Agent.UsageDirection currentMovementFlagUsageDirection);

    [EngineMethod("try_to_wield_weapon_in_slot", false)]
    void TryToWieldWeaponInSlot(
      UIntPtr agentPointer,
      int equipmentSlot,
      int type,
      bool isWieldedOnSpawn);

    [EngineMethod("get_weapon_entity_from_equipment_slot", false)]
    UIntPtr GetWeaponEntityFromEquipmentSlot(UIntPtr agentPointer, int equipmentSlot);

    [EngineMethod("prepare_weapon_for_drop_in_equipment_slot", false)]
    void PrepareWeaponForDropInEquipmentSlot(
      UIntPtr agentPointer,
      int equipmentSlot,
      bool showHolsterWithWeapon);

    [EngineMethod("try_to_sheath_weapon_in_hand", false)]
    void TryToSheathWeaponInHand(UIntPtr agentPointer, int handIndex, int type);

    [EngineMethod("update_weapons", false)]
    void UpdateWeapons(UIntPtr agentPointer);

    [EngineMethod("attach_weapon_to_bone", false)]
    void AttachWeaponToBone(
      UIntPtr agentPointer,
      in WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      UIntPtr weaponEntity,
      sbyte boneIndex,
      ref MatrixFrame attachLocalFrame);

    [EngineMethod("attach_weapon_to_weapon_in_slot", false)]
    void AttachWeaponToWeaponInSlot(
      UIntPtr agentPointer,
      in WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      UIntPtr weaponEntity,
      int slotIndex,
      ref MatrixFrame attachLocalFrame);

    [EngineMethod("build", false)]
    void Build(UIntPtr agentPointer, Vec3 eyeOffsetWrtHead);

    [EngineMethod("lock_agent_replication_table_with_current_reliable_sequence_no", false)]
    void LockAgentReplicationTableDataWithCurrentReliableSequenceNo(
      UIntPtr agentPointer,
      int peerIndex);

    [EngineMethod("set_agent_exclude_state_for_face_group_id", false)]
    void SetAgentExcludeStateForFaceGroupId(UIntPtr agentPointer, int faceGroupId, bool isExcluded);

    [EngineMethod("set_agent_scale", false)]
    void SetAgentScale(UIntPtr agentPointer, float scale);

    [EngineMethod("get_current_velocity", false)]
    Vec2 GetCurrentVelocity(UIntPtr agentPointer);

    [EngineMethod("get_turn_speed", false)]
    float GetTurnSpeed(UIntPtr agentPointer);

    [EngineMethod("get_movement_direction_as_angle", false)]
    float GetMovementDirectionAsAngle(UIntPtr agentPointer);

    [EngineMethod("get_movement_direction", false)]
    Vec3 GetMovementDirection(UIntPtr agentPointer);

    [EngineMethod("set_movement_direction", false)]
    void SetMovementDirection(UIntPtr agentPointer, ref Vec3 direction);

    [EngineMethod("get_current_speed_limit", false)]
    float GetCurrentSpeedLimit(UIntPtr agentPointer);

    [EngineMethod("set_minimum_speed", false)]
    void SetMinimumSpeed(UIntPtr agentPointer, float speed);

    [EngineMethod("set_maximum_speed_limit", false)]
    void SetMaximumSpeedLimit(UIntPtr agentPointer, float maximumSpeedLimit, bool isMultiplier);

    [EngineMethod("get_maximum_speed_limit", false)]
    float GetMaximumSpeedLimit(UIntPtr agentPointer);

    [EngineMethod("get_maximum_forward_unlimited_speed", false)]
    float GetMaximumForwardUnlimitedSpeed(UIntPtr agentPointer);

    [EngineMethod("set_destination_speed", false)]
    void SetDestinationSpeed(UIntPtr agentPointer, float speed);

    [EngineMethod("fade_out", false)]
    void FadeOut(UIntPtr agentPointer, bool hideInstantly);

    [EngineMethod("fade_in", false)]
    void FadeIn(UIntPtr agentPointer);

    [EngineMethod("get_scripted_flags", false)]
    int GetScriptedFlags(UIntPtr agentPointer);

    [EngineMethod("set_scripted_flags", false)]
    void SetScriptedFlags(UIntPtr agentPointer, int flags);

    [EngineMethod("get_scripted_combat_flags", false)]
    int GetScriptedCombatFlags(UIntPtr agentPointer);

    [EngineMethod("set_scripted_combat_flags", false)]
    void SetScriptedCombatFlags(UIntPtr agentPointer, int flags);

    [EngineMethod("set_scripted_position_and_direction", false)]
    bool SetScriptedPositionAndDirection(
      UIntPtr agentPointer,
      ref WorldPosition targetPosition,
      float targetDirection,
      bool addHumanLikeDelay,
      int additionalFlags,
      string debugString);

    [EngineMethod("set_scripted_position", false)]
    bool SetScriptedPosition(
      UIntPtr agentPointer,
      ref WorldPosition targetPosition,
      bool addHumanLikeDelay,
      int additionalFlags,
      string debugString);

    [EngineMethod("set_scripted_target_entity", false)]
    void SetScriptedTargetEntity(
      UIntPtr agentPointer,
      UIntPtr entityId,
      ref WorldPosition specialPosition,
      int additionalFlags);

    [EngineMethod("disable_scripted_movement", false)]
    void DisableScriptedMovement(UIntPtr agentPointer);

    [EngineMethod("disable_scripted_combat_movement", false)]
    void DisableScriptedCombatMovement(UIntPtr agentPointer);

    [EngineMethod("force_ai_behaviour_selection", false)]
    void ForceAiBehaviourSelection(UIntPtr agentPointer);

    [EngineMethod("has_path_through_navigation_face_id_from_direction", false)]
    bool HasPathThroughNavigationFaceIdFromDirection(
      UIntPtr agentPointer,
      int navigationFaceId,
      ref Vec2 direction);

    [EngineMethod("can_move_directly_to_position", false)]
    bool CanMoveDirectlyToPosition(UIntPtr agentPointer, in WorldPosition position);

    [EngineMethod("check_path_to_ai_target_agent_passes_through_navigation_face_id_from_direction", false)]
    bool CheckPathToAITargetAgentPassesThroughNavigationFaceIdFromDirection(
      UIntPtr agentPointer,
      int navigationFaceId,
      ref Vec3 direction,
      float overridenCostForFaceId);

    [EngineMethod("get_path_distance_to_point", false)]
    float GetPathDistanceToPoint(UIntPtr agentPointer, ref Vec3 direction);

    [EngineMethod("get_current_navigation_face_id", false)]
    int GetCurrentNavigationFaceId(UIntPtr agentPointer);

    [EngineMethod("get_world_position", false)]
    WorldPosition GetWorldPosition(UIntPtr agentPointer);

    [EngineMethod("set_agent_facial_animation", false)]
    void SetAgentFacialAnimation(
      UIntPtr agentPointer,
      int channel,
      string animationName,
      bool loop);

    [EngineMethod("get_agent_facial_animation", false)]
    string GetAgentFacialAnimation(UIntPtr agentPointer);

    [EngineMethod("get_agent_voice_definiton", false)]
    string GetAgentVoiceDefinition(UIntPtr agentPointer);

    [EngineMethod("allow_first_person_wide_rotation", false)]
    bool AllowFirstPersonWideRotation(UIntPtr agentPointer);

    [EngineMethod("get_current_animation_flags", false)]
    ulong GetCurrentAnimationFlags(UIntPtr agentPointer, int channelNo);

    [EngineMethod("get_current_action", false)]
    int GetCurrentAction(UIntPtr agentPointer, int channelNo);

    [EngineMethod("get_current_action_type", false)]
    int GetCurrentActionType(UIntPtr agentPointer, int channelNo);

    [EngineMethod("get_current_action_stage", false)]
    int GetCurrentActionStage(UIntPtr agentPointer, int channelNo);

    [EngineMethod("get_current_action_direction", false)]
    int GetCurrentActionDirection(UIntPtr agentPointer, int channelNo);

    [EngineMethod("compute_animation_displacement", false)]
    Vec3 ComputeAnimationDisplacement(UIntPtr agentPointer, float dt);

    [EngineMethod("get_current_action_priority", false)]
    int GetCurrentActionPriority(UIntPtr agentPointer, int channelNo);

    [EngineMethod("get_current_action_progress", false)]
    float GetCurrentActionProgress(UIntPtr agentPointer, int channelNo);

    [EngineMethod("set_current_action_progress", false)]
    void SetCurrentActionProgress(UIntPtr agentPointer, int channelNo, float progress);

    [EngineMethod("set_action_channel", false)]
    bool SetActionChannel(
      UIntPtr agentPointer,
      int channelNo,
      int actionNo,
      ulong additionalFlags,
      bool ignorePriority,
      float blendWithNextActionFactor,
      float actionSpeed,
      float blendInPeriod,
      float blendOutPeriodToNoAnim,
      float startProgress,
      bool useLinearSmoothing,
      float blendOutPeriod,
      bool forceFaceMorphRestart);

    [EngineMethod("set_current_action_speed", false)]
    void SetCurrentActionSpeed(UIntPtr agentPointer, int channelNo, float actionSpeed);

    [EngineMethod("tick_action_channels", false)]
    void TickActionChannels(UIntPtr agentPointer, float dt);

    [EngineMethod("get_action_channel_weight", false)]
    float GetActionChannelWeight(UIntPtr agentPointer, int channelNo);

    [EngineMethod("get_action_channel_current_action_weight", false)]
    float GetActionChannelCurrentActionWeight(UIntPtr agentPointer, int channelNo);

    [EngineMethod("set_action_set", false)]
    void SetActionSet(
      UIntPtr agentPointer,
      ref AgentVisualsNativeData agentVisualsNativeData,
      ref AnimationSystemData animationSystemData);

    [EngineMethod("get_action_set_no", false)]
    int GetActionSetNo(UIntPtr agentPointer);

    [EngineMethod("get_movement_locked_state", false)]
    AgentMovementLockedState GetMovementLockedState(UIntPtr agentPointer);

    [EngineMethod("get_target_position", false)]
    Vec2 GetTargetPosition(UIntPtr agentPointer);

    [EngineMethod("set_target_position", false)]
    void SetTargetPosition(UIntPtr agentPointer, ref Vec2 targetPosition);

    [EngineMethod("get_target_direction", false)]
    Vec3 GetTargetDirection(UIntPtr agentPointer);

    [EngineMethod("set_target_position_and_direction", false)]
    void SetTargetPositionAndDirection(
      UIntPtr agentPointer,
      ref Vec2 targetPosition,
      ref Vec3 targetDirection);

    [EngineMethod("clear_target_frame", false)]
    void ClearTargetFrame(UIntPtr agentPointer);

    [EngineMethod("get_is_look_direction_locked", false)]
    bool GetIsLookDirectionLocked(UIntPtr agentPointer);

    [EngineMethod("set_is_look_direction_locked", false)]
    void SetIsLookDirectionLocked(UIntPtr agentPointer, bool isLocked);

    [EngineMethod("set_mono_object", false)]
    void SetMonoObject(UIntPtr agentPointer, Agent monoObject);

    [EngineMethod("get_eye_global_position", false)]
    Vec3 GetEyeGlobalPosition(UIntPtr agentPointer);

    [EngineMethod("get_chest_global_position", false)]
    Vec3 GetChestGlobalPosition(UIntPtr agentPointer);

    [EngineMethod("add_mesh_to_bone", false)]
    void AddMeshToBone(UIntPtr agentPointer, UIntPtr meshPointer, sbyte boneIndex);

    [EngineMethod("remove_mesh_from_bone", false)]
    void RemoveMeshFromBone(UIntPtr agentPointer, UIntPtr meshPointer, sbyte boneIndex);

    [EngineMethod("add_prefab_to_agent_bone", false)]
    CompositeComponent AddPrefabToAgentBone(
      UIntPtr agentPointer,
      string prefabName,
      sbyte boneIndex);

    [EngineMethod("wield_next_weapon", false)]
    void WieldNextWeapon(UIntPtr agentPointer, int item_index);

    [EngineMethod("preload_for_rendering", false)]
    void PreloadForRendering(UIntPtr agentPointer);

    [EngineMethod("get_agent_scale", false)]
    float GetAgentScale(UIntPtr agentPointer);

    [EngineMethod("get_crouch_mode", false)]
    bool GetCrouchMode(UIntPtr agentPointer);

    [EngineMethod("get_walk_mode", false)]
    bool GetWalkMode(UIntPtr agentPointer);

    [EngineMethod("get_visual_position", false)]
    Vec3 GetVisualPosition(UIntPtr agentPointer);

    [EngineMethod("is_look_rotation_in_slow_motion", false)]
    bool IsLookRotationInSlowMotion(UIntPtr agentPointer);

    [EngineMethod("get_look_direction_as_angle", false)]
    float GetLookDirectionAsAngle(UIntPtr agentPointer);

    [EngineMethod("set_look_direction_as_angle", false)]
    void SetLookDirectionAsAngle(UIntPtr agentPointer, float value);

    [EngineMethod("attack_direction_to_movement_flag", false)]
    Agent.MovementControlFlag AttackDirectionToMovementFlag(
      UIntPtr agentPointer,
      Agent.UsageDirection direction);

    [EngineMethod("defend_direction_to_movement_flag", false)]
    Agent.MovementControlFlag DefendDirectionToMovementFlag(
      UIntPtr agentPointer,
      Agent.UsageDirection direction);

    [EngineMethod("get_head_camera_mode", false)]
    bool GetHeadCameraMode(UIntPtr agentPointer);

    [EngineMethod("set_head_camera_mode", false)]
    void SetHeadCameraMode(UIntPtr agentPointer, bool value);

    [EngineMethod("kick_clear", false)]
    bool KickClear(UIntPtr agentPointer);

    [EngineMethod("reset_guard", false)]
    void ResetGuard(UIntPtr agentPointer);

    [EngineMethod("get_current_guard_mode", false)]
    Agent.GuardMode GetCurrentGuardMode(UIntPtr agentPointer);

    [EngineMethod("get_defend_movement_flag", false)]
    Agent.MovementControlFlag GetDefendMovementFlag(UIntPtr agentPointer);

    [EngineMethod("get_attack_direction", false)]
    Agent.UsageDirection GetAttackDirection(UIntPtr agentPointer, bool doAiCheck);

    [EngineMethod("player_attack_direction", false)]
    Agent.UsageDirection PlayerAttackDirection(UIntPtr agentPointer);

    [EngineMethod("get_wielded_weapon_info", false)]
    bool GetWieldedWeaponInfo(
      UIntPtr agentPointer,
      int handIndex,
      ref bool isMeleeWeapon,
      ref bool isRangedWeapon);

    [EngineMethod("get_immediate_enemy", false)]
    Agent GetImmediateEnemy(UIntPtr agentPointer);

    [EngineMethod("get_is_doing_passive_attack", false)]
    bool GetIsDoingPassiveAttack(UIntPtr agentPointer);

    [EngineMethod("get_is_passive_usage_conditions_are_met", false)]
    bool GetIsPassiveUsageConditionsAreMet(UIntPtr agentPointer);

    [EngineMethod("get_current_aiming_turbulance", false)]
    float GetCurrentAimingTurbulance(UIntPtr agentPointer);

    [EngineMethod("get_current_aiming_error", false)]
    float GetCurrentAimingError(UIntPtr agentPointer);

    [EngineMethod("get_body_rotation_constraint", false)]
    Vec3 GetBodyRotationConstraint(UIntPtr agentPointer, int channelIndex);

    [EngineMethod("get_action_direction", false)]
    Agent.UsageDirection GetActionDirection(int actionIndex);

    [EngineMethod("get_attack_direction_usage", false)]
    Agent.UsageDirection GetAttackDirectionUsage(UIntPtr agentPointer);

    [EngineMethod("handle_blow_aux", false)]
    void HandleBlowAux(UIntPtr agentPointer, ref Blow blow);

    [EngineMethod("set_current_discipline", false)]
    void SetCurrentDiscipline(UIntPtr agentPointer, float value);

    [EngineMethod("make_voice", false)]
    void MakeVoice(UIntPtr agentPointer, int voiceType, int predictionType);

    [EngineMethod("set_hand_inverse_kinematics_frame", false)]
    bool SetHandInverseKinematicsFrame(
      UIntPtr agentPointer,
      ref MatrixFrame leftGlobalFrame,
      ref MatrixFrame rightGlobalFrame);

    [EngineMethod("clear_hand_inverse_kinematics", false)]
    void ClearHandInverseKinematics(UIntPtr agentPointer);

    [EngineMethod("debug_more", false)]
    void DebugMore(UIntPtr agentPointer);

    [EngineMethod("is_on_land", false)]
    bool IsOnLand(UIntPtr agentPointer);

    [EngineMethod("is_sliding", false)]
    bool IsSliding(UIntPtr agentPointer);

    [EngineMethod("is_running_away", false)]
    bool IsRunningAway(UIntPtr agentPointer);

    [EngineMethod("get_cur_weapon_offset", false)]
    Vec3 GetCurWeaponOffset(UIntPtr agentPointer);

    [EngineMethod("get_walking_speed_limit_of_mountable", false)]
    float GetWalkSpeedLimitOfMountable(UIntPtr agentPointer);

    [EngineMethod("create_blood_burst_at_limb", false)]
    void CreateBloodBurstAtLimb(
      UIntPtr agentPointer,
      sbyte iBone,
      ref Vec3 impactPosition,
      float scale);

    [EngineMethod("get_native_action_index", false)]
    int GetNativeActionIndex(string actionName);

    [EngineMethod("set_guarded_agent_index", false)]
    void SetGuardedAgentIndex(UIntPtr agentPointer, int guardedAgentIndex);

    [EngineMethod("set_columnwise_follow_agent", false)]
    void SetColumnwiseFollowAgent(
      UIntPtr agentPointer,
      int followAgentIndex,
      ref Vec2 followPosition);

    [EngineMethod("get_monster_usage_index", false)]
    int GetMonsterUsageIndex(string monsterUsage);

    [EngineMethod("get_missile_range_with_height_difference", false)]
    float GetMissileRangeWithHeightDifference(UIntPtr agentPointer, float targetZ);

    [EngineMethod("set_formation_no", false)]
    void SetFormationNo(UIntPtr agentPointer, int formationNo);

    [EngineMethod("enforce_shield_usage", false)]
    void EnforceShieldUsage(UIntPtr agentPointer, Agent.UsageDirection direction);

    [EngineMethod("set_firing_order", false)]
    void SetFiringOrder(UIntPtr agentPointer, int order);

    [EngineMethod("set_riding_order", false)]
    void SetRidingOrder(UIntPtr agentPointer, int order);

    [EngineMethod("set_direction_change_tendency", false)]
    void SetDirectionChangeTendency(UIntPtr agentPointer, float tendency);

    [EngineMethod("set_ai_behavior_params", false)]
    void SetAiBehaviorParams(
      UIntPtr agentPointer,
      int behavior,
      float y1,
      float x2,
      float y2,
      float x3,
      float y3);

    [EngineMethod("set_all_ai_behavior_params", false)]
    void SetAllAIBehaviorParams(UIntPtr agentPointer, BehaviorValues[] behaviorParams);

    [EngineMethod("set_body_armor_material_type", false)]
    void SetBodyArmorMaterialType(
      UIntPtr agentPointer,
      ArmorComponent.ArmorMaterialTypes bodyArmorMaterialType);

    [EngineMethod("get_maximum_number_of_agents", false)]
    int GetMaximumNumberOfAgents();

    [EngineMethod("get_running_simulation_data_until_maximum_speed_reached", false)]
    void GetRunningSimulationDataUntilMaximumSpeedReached(
      UIntPtr agentPointer,
      ref float combatAccelerationTime,
      ref float maxSpeed,
      float[] speedValues);

    [EngineMethod("get_last_target_visibility_state", false)]
    int GetLastTargetVisibilityState(UIntPtr agentPointer);

    [EngineMethod("get_target_range", false)]
    float GetTargetRange(UIntPtr agentPointer);

    [EngineMethod("get_missile_range", false)]
    float GetMissileRange(UIntPtr agentPointer);

    [EngineMethod("set_sound_occlusion", false)]
    void SetSoundOcclusion(UIntPtr agentPointer, float value);
  }
}
