// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IGameEntity
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IGameEntity
  {
    [EngineMethod("get_scene", false)]
    Scene GetScene(UIntPtr entityId);

    [EngineMethod("get_first_mesh", false)]
    Mesh GetFirstMesh(UIntPtr entityId);

    [EngineMethod("create_from_prefab", false)]
    GameEntity CreateFromPrefab(
      UIntPtr scenePointer,
      string prefabid,
      bool callScriptCallbacks);

    [EngineMethod("call_script_callbacks", false)]
    void CallScriptCallbacks(UIntPtr entityPointer);

    [EngineMethod("create_from_prefab_with_initial_frame", false)]
    GameEntity CreateFromPrefabWithInitialFrame(
      UIntPtr scenePointer,
      string prefabid,
      ref MatrixFrame frame);

    [EngineMethod("add_component", false)]
    void AddComponent(UIntPtr pointer, UIntPtr componentPointer);

    [EngineMethod("remove_component", false)]
    bool RemoveComponent(UIntPtr pointer, UIntPtr componentPointer);

    [EngineMethod("has_component", false)]
    bool HasComponent(UIntPtr pointer, UIntPtr componentPointer);

    [EngineMethod("update_global_bounds", false)]
    void UpdateGlobalBounds(UIntPtr entityPointer);

    [EngineMethod("validate_bounding_box", false)]
    void ValidateBoundingBox(UIntPtr entityPointer);

    [EngineMethod("clear_components", false)]
    void ClearComponents(UIntPtr entityId);

    [EngineMethod("clear_only_own_components", false)]
    void ClearOnlyOwnComponents(UIntPtr entityId);

    [EngineMethod("clear_entity_components", false)]
    void ClearEntityComponents(
      UIntPtr entityId,
      bool resetAll,
      bool removeScripts,
      bool deleteChildEntities);

    [EngineMethod("update_visibility_mask", false)]
    void UpdateVisibilityMask(UIntPtr entityPtr);

    [EngineMethod("check_resources", false)]
    bool CheckResources(UIntPtr entityId, bool addToQueue);

    [EngineMethod("set_mobility", false)]
    void SetMobility(UIntPtr entityId, int mobility);

    [EngineMethod("add_mesh", false)]
    void AddMesh(UIntPtr entityId, UIntPtr mesh, bool recomputeBoundingBox);

    [EngineMethod("add_multi_mesh_to_skeleton", false)]
    void AddMultiMeshToSkeleton(UIntPtr gameEntity, UIntPtr multiMesh);

    [EngineMethod("add_multi_mesh_to_skeleton_bone", false)]
    void AddMultiMeshToSkeletonBone(UIntPtr gameEntity, UIntPtr multiMesh, sbyte boneIndex);

    [EngineMethod("set_as_replay_entity", false)]
    void SetAsReplayEntity(UIntPtr gameEntity);

    [EngineMethod("set_cloth_max_distance_multiplier", false)]
    void SetClothMaxDistanceMultiplier(UIntPtr gameEntity, float multiplier);

    [EngineMethod("set_previous_frame_invalid", false)]
    void SetPreviousFrameInvalid(UIntPtr gameEntity);

    [EngineMethod("remove_multi_mesh_from_skeleton", false)]
    void RemoveMultiMeshFromSkeleton(UIntPtr gameEntity, UIntPtr multiMesh);

    [EngineMethod("remove_multi_mesh_from_skeleton_bone", false)]
    void RemoveMultiMeshFromSkeletonBone(UIntPtr gameEntity, UIntPtr multiMesh, sbyte boneIndex);

    [EngineMethod("remove_component_with_mesh", false)]
    bool RemoveComponentWithMesh(UIntPtr entityId, UIntPtr mesh);

    [EngineMethod("get_guid", false)]
    string GetGuid(UIntPtr entityId);

    [EngineMethod("is_guid_valid", false)]
    bool IsGuidValid(UIntPtr entityId);

    [EngineMethod("add_sphere_as_body", false)]
    void AddSphereAsBody(UIntPtr entityId, Vec3 center, float radius, uint bodyFlags);

    [EngineMethod("get_quick_bone_entitial_frame", false)]
    void GetQuickBoneEntitialFrame(UIntPtr entityId, sbyte index, ref MatrixFrame frame);

    [EngineMethod("create_empty", false)]
    GameEntity CreateEmpty(
      UIntPtr scenePointer,
      bool isModifiableFromEditor,
      UIntPtr entityId);

    [EngineMethod("create_empty_without_scene", false)]
    GameEntity CreateEmptyWithoutScene();

    [EngineMethod("remove", false)]
    void Remove(UIntPtr entityId, int removeReason);

    [EngineMethod("find_with_name", false)]
    GameEntity FindWithName(UIntPtr scenePointer, string name);

    [EngineMethod("get_frame", false)]
    void GetFrame(UIntPtr entityId, ref MatrixFrame outFrame);

    [EngineMethod("set_frame", false)]
    void SetFrame(UIntPtr entityId, ref MatrixFrame frame);

    [EngineMethod("set_cloth_component_keep_state", false)]
    void SetClothComponentKeepState(UIntPtr entityId, UIntPtr metaMesh, bool keepState);

    [EngineMethod("set_cloth_component_keep_state_of_all_meshes", false)]
    void SetClothComponentKeepStateOfAllMeshes(UIntPtr entityId, bool keepState);

    [EngineMethod("update_triad_frame_for_editor", false)]
    void UpdateTriadFrameForEditor(UIntPtr meshPointer);

    [EngineMethod("get_global_frame", false)]
    void GetGlobalFrame(UIntPtr meshPointer, ref MatrixFrame outFrame);

    [EngineMethod("set_global_frame", false)]
    void SetGlobalFrame(UIntPtr entityId, ref MatrixFrame frame);

    [EngineMethod("set_local_position", false)]
    void SetLocalPosition(UIntPtr entityId, Vec3 position);

    [EngineMethod("get_entity_flags", false)]
    uint GetEntityFlags(UIntPtr entityId);

    [EngineMethod("set_entity_flags", false)]
    void SetEntityFlags(UIntPtr entityId, uint entityFlags);

    [EngineMethod("get_entity_visibility_flags", false)]
    uint GetEntityVisibilityFlags(UIntPtr entityId);

    [EngineMethod("set_entity_visibility_flags", false)]
    void SetEntityVisibilityFlags(UIntPtr entityId, uint entityVisibilityFlags);

    [EngineMethod("get_body_flags", false)]
    uint GetBodyFlags(UIntPtr entityId);

    [EngineMethod("set_body_flags", false)]
    void SetBodyFlags(UIntPtr entityId, uint bodyFlags);

    [EngineMethod("get_physics_desc_body_flags", false)]
    uint GetPhysicsDescBodyFlags(UIntPtr entityId);

    [EngineMethod("get_center_of_mass", false)]
    Vec3 GetCenterOfMass(UIntPtr entityId);

    [EngineMethod("get_mass", false)]
    float GetMass(UIntPtr entityId);

    [EngineMethod("set_body_flags_recursive", false)]
    void SetBodyFlagsRecursive(UIntPtr entityId, uint bodyFlags);

    [EngineMethod("get_global_scale", false)]
    Vec3 GetGlobalScale(GameEntity entity);

    [EngineMethod("get_body_shape", false)]
    PhysicsShape GetBodyShape(GameEntity entity);

    [EngineMethod("set_body_shape", false)]
    void SetBodyShape(UIntPtr entityId, UIntPtr shape);

    [EngineMethod("add_physics", false)]
    void AddPhysics(
      UIntPtr entityId,
      UIntPtr body,
      float mass,
      ref Vec3 localCenterOfMass,
      ref Vec3 initialVelocity,
      ref Vec3 initialAngularVelocity,
      int physicsMaterial,
      bool isStatic,
      int collisionGroupID);

    [EngineMethod("remove_physics", false)]
    void RemovePhysics(UIntPtr entityId, bool clearingTheScene);

    [EngineMethod("set_physics_state", false)]
    void SetPhysicsState(UIntPtr entityId, bool isEnabled, bool setChildren);

    [EngineMethod("get_physics_state", false)]
    bool GetPhysicsState(UIntPtr entityId);

    [EngineMethod("has_physics_definition", false)]
    bool HasPhysicsDefinition(UIntPtr entityId, int excludeFlags);

    [EngineMethod("remove_engine_physics", false)]
    void RemoveEnginePhysics(UIntPtr entityId);

    [EngineMethod("is_engine_body_sleeping", false)]
    bool IsEngineBodySleeping(UIntPtr entityId);

    [EngineMethod("enable_dynamic_body", false)]
    void EnableDynamicBody(UIntPtr entityId);

    [EngineMethod("disable_dynamic_body_simulation", false)]
    void DisableDynamicBodySimulation(UIntPtr entityId);

    [EngineMethod("apply_impulse_to_dynamic_body", false)]
    void ApplyImpulseToDynamicBody(UIntPtr entityId, ref Vec3 position, ref Vec3 impulse);

    [EngineMethod("add_child", false)]
    void AddChild(UIntPtr parententity, UIntPtr childentity, bool autoLocalizeFrame);

    [EngineMethod("remove_child", false)]
    void RemoveChild(
      UIntPtr parentEntity,
      UIntPtr childEntity,
      bool keepPhysics,
      bool keepScenePointer,
      bool callScriptCallbacks,
      int removeReason);

    [EngineMethod("get_child_count", false)]
    int GetChildCount(UIntPtr entityId);

    [EngineMethod("get_child", false)]
    GameEntity GetChild(UIntPtr entityId, int childIndex);

    [EngineMethod("get_parent", false)]
    GameEntity GetParent(UIntPtr entityId);

    [EngineMethod("has_complex_anim_tree", false)]
    bool HasComplexAnimTree(UIntPtr entityId);

    [EngineMethod("get_script_component", false)]
    ScriptComponentBehaviour GetScriptComponent(UIntPtr entityId);

    [EngineMethod("get_script_component_count", false)]
    int GetScriptComponentCount(UIntPtr entityId);

    [EngineMethod("get_script_component_at_index", false)]
    ScriptComponentBehaviour GetScriptComponentAtIndex(
      UIntPtr entityId,
      int index);

    [EngineMethod("set_entity_env_map_visibility", false)]
    void SetEntityEnvMapVisibility(UIntPtr entityId, bool value);

    [EngineMethod("create_and_add_script_component", false)]
    void CreateAndAddScriptComponent(UIntPtr entityId, string name);

    [EngineMethod("remove_script_component", false)]
    void RemoveScriptComponent(UIntPtr entityId, UIntPtr scriptComponentPtr, int removeReason);

    [EngineMethod("prefab_exists", false)]
    bool PrefabExists(string prefabName);

    [EngineMethod("is_ghost_object", false)]
    bool IsGhostObject(UIntPtr entityId);

    [EngineMethod("has_script_component", false)]
    bool HasScriptComponent(UIntPtr entityId, string scName);

    [EngineMethod("get_name", false)]
    string GetName(UIntPtr entityId);

    [EngineMethod("get_first_entity_with_tag", false)]
    GameEntity GetFirstEntityWithTag(UIntPtr scenePointer, string tag);

    [EngineMethod("get_next_entity_with_tag", false)]
    GameEntity GetNextEntityWithTag(UIntPtr currententityId, string tag);

    [EngineMethod("get_first_entity_with_tag_expression", false)]
    GameEntity GetFirstEntityWithTagExpression(UIntPtr scenePointer, string tagExpression);

    [EngineMethod("get_next_entity_with_tag_expression", false)]
    GameEntity GetNextEntityWithTagExpression(
      UIntPtr currententityId,
      string tagExpression);

    [EngineMethod("get_next_prefab", false)]
    GameEntity GetNextPrefab(UIntPtr currentPrefab);

    [EngineMethod("copy_from_prefab", false)]
    GameEntity CopyFromPrefab(UIntPtr prefab);

    [EngineMethod("set_upgrade_level_mask", false)]
    void SetUpgradeLevelMask(UIntPtr prefab, uint mask);

    [EngineMethod("get_upgrade_level_mask", false)]
    uint GetUpgradeLevelMask(UIntPtr prefab);

    [EngineMethod("get_old_prefab_name", false)]
    string GetOldPrefabName(UIntPtr prefab);

    [EngineMethod("get_prefab_name", false)]
    string GetPrefabName(UIntPtr prefab);

    [EngineMethod("copy_script_component_from_another_entity", false)]
    void CopyScriptComponentFromAnotherEntity(
      UIntPtr prefab,
      UIntPtr other_prefab,
      string script_name);

    [EngineMethod("add_multi_mesh", false)]
    void AddMultiMesh(UIntPtr entityId, UIntPtr multiMeshPtr, bool updateVisMask);

    [EngineMethod("remove_multi_mesh", false)]
    bool RemoveMultiMesh(UIntPtr entityId, UIntPtr multiMeshPtr);

    [EngineMethod("get_component_count", false)]
    int GetComponentCount(UIntPtr entityId, GameEntity.ComponentType componentType);

    [EngineMethod("get_component_at_index", false)]
    GameEntityComponent GetComponentAtIndex(
      UIntPtr entityId,
      GameEntity.ComponentType componentType,
      int index);

    [EngineMethod("add_all_meshes_of_game_entity", false)]
    void AddAllMeshesOfGameEntity(UIntPtr entityId, UIntPtr copiedEntityId);

    [EngineMethod("is_visible_include_parents", false)]
    bool IsVisibleIncludeParents(UIntPtr entityId);

    [EngineMethod("get_visibility_level_mask_including_parents", false)]
    uint GetVisibilityLevelMaskIncludingParents(UIntPtr entityId);

    [EngineMethod("get_edit_mode_level_visibility", false)]
    bool GetEditModeLevelVisibility(UIntPtr entityId);

    [EngineMethod("get_visibility_exclude_parents", false)]
    bool GetVisibilityExcludeParents(UIntPtr entityId);

    [EngineMethod("set_visibility_exclude_parents", false)]
    void SetVisibilityExcludeParents(UIntPtr entityId, bool visibility);

    [EngineMethod("set_alpha", false)]
    void SetAlpha(UIntPtr entityId, float alpha);

    [EngineMethod("set_ready_to_render", false)]
    void SetReadyToRender(UIntPtr entityId, bool ready);

    [EngineMethod("add_particle_system_component", false)]
    void AddParticleSystemComponent(UIntPtr entityId, string particleid);

    [EngineMethod("remove_all_particle_systems", false)]
    void RemoveAllParticleSystems(UIntPtr entityId);

    [EngineMethod("get_tags", false)]
    string GetTags(UIntPtr entityId);

    [EngineMethod("has_tag", false)]
    bool HasTag(UIntPtr entityId, string tag);

    [EngineMethod("add_tag", false)]
    void AddTag(UIntPtr entityId, string tag);

    [EngineMethod("remove_tag", false)]
    void RemoveTag(UIntPtr entityId, string tag);

    [EngineMethod("add_light", false)]
    bool AddLight(UIntPtr entityId, UIntPtr lightPointer);

    [EngineMethod("get_light", false)]
    Light GetLight(UIntPtr entityId);

    [EngineMethod("set_material_for_all_meshes", false)]
    void SetMaterialForAllMeshes(UIntPtr entityId, UIntPtr materialPointer);

    [EngineMethod("set_name", false)]
    void SetName(UIntPtr entityId, string name);

    [EngineMethod("set_vector_argument", false)]
    void SetVectorArgument(
      UIntPtr entityId,
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3);

    [EngineMethod("set_factor2_color", false)]
    void SetFactor2Color(UIntPtr entityId, uint factor2Color);

    [EngineMethod("set_factor_color", false)]
    void SetFactorColor(UIntPtr entityId, uint factorColor);

    [EngineMethod("get_factor_color", false)]
    uint GetFactorColor(UIntPtr entityId);

    [EngineMethod("set_animation_sound_activation", false)]
    void SetAnimationSoundActivation(UIntPtr entityId, bool activate);

    [EngineMethod("copy_components_to_skeleton", false)]
    void CopyComponentsToSkeleton(UIntPtr entityId);

    [EngineMethod("get_bounding_box_min", false)]
    Vec3 GetBoundingBoxMin(UIntPtr entityId);

    [EngineMethod("get_bounding_box_max", false)]
    Vec3 GetBoundingBoxMax(UIntPtr entityId);

    [EngineMethod("has_frame_changed", false)]
    bool HasFrameChanged(UIntPtr entityId);

    [EngineMethod("set_external_references_usage", false)]
    void SetExternalReferencesUsage(UIntPtr entityId, bool value);

    [EngineMethod("set_morph_frame_of_components", false)]
    void SetMorphFrameOfComponents(UIntPtr entityId, float value);

    [EngineMethod("add_edit_data_user_to_all_meshes", false)]
    void AddEditDataUserToAllMeshes(
      UIntPtr entityId,
      bool entity_components,
      bool skeleton_components);

    [EngineMethod("release_edit_data_user_to_all_meshes", false)]
    void ReleaseEditDataUserToAllMeshes(
      UIntPtr entityId,
      bool entity_components,
      bool skeleton_components);

    [EngineMethod("get_camera_params_from_camera_script", false)]
    void GetCameraParamsFromCameraScript(UIntPtr entityId, UIntPtr camPtr, ref Vec3 dof_params);

    [EngineMethod("get_mesh_bended_position", false)]
    void GetMeshBendedPosition(
      UIntPtr entityId,
      ref MatrixFrame worldSpacePosition,
      ref MatrixFrame output);

    [EngineMethod("break_prefab", false)]
    void BreakPrefab(UIntPtr entityId);

    [EngineMethod("set_anim_tree_channel_parameter", false)]
    void SetAnimTreeChannelParameter(UIntPtr entityId, float phase, int channel_no);

    [EngineMethod("disable_contour", false)]
    void DisableContour(UIntPtr entityId);

    [EngineMethod("set_as_contour_entity", false)]
    void SetAsContourEntity(UIntPtr entityId, uint color);

    [EngineMethod("set_contour_state", false)]
    void SetContourState(UIntPtr entityId, bool alwaysVisible);

    [EngineMethod("recompute_bounding_box", false)]
    void RecomputeBoundingBox(GameEntity entity);

    [EngineMethod("set_boundingbox_dirty", false)]
    void SetBoundingboxDirty(UIntPtr entityId);

    [EngineMethod("get_global_box_max", false)]
    Vec3 GetGlobalBoxMax(UIntPtr entityId);

    [EngineMethod("get_global_box_min", false)]
    Vec3 GetGlobalBoxMin(UIntPtr entityId);

    [EngineMethod("get_radius", false)]
    float GetRadius(UIntPtr entityId);

    [EngineMethod("change_meta_mesh_or_remove_it_if_not_exists", false)]
    void ChangeMetaMeshOrRemoveItIfNotExists(
      UIntPtr entityId,
      UIntPtr entityMetaMeshPointer,
      UIntPtr newMetaMeshPointer);

    [EngineMethod("set_skeleton", false)]
    void SetSkeleton(UIntPtr entityId, UIntPtr skeletonPointer);

    [EngineMethod("get_skeleton", false)]
    Skeleton GetSkeleton(UIntPtr entityId);

    [EngineMethod("delete_all_children", false)]
    void RemoveAllChildren(UIntPtr entityId);

    [EngineMethod("check_point_with_oriented_bounding_box", false)]
    bool CheckPointWithOrientedBoundingBox(UIntPtr entityId, Vec3 point);

    [EngineMethod("resume_particle_system", false)]
    void ResumeParticleSystem(UIntPtr entityId, bool doChildren);

    [EngineMethod("pause_particle_system", false)]
    void PauseParticleSystem(UIntPtr entityId, bool doChildren);

    [EngineMethod("burst_entity_particle", false)]
    void BurstEntityParticle(UIntPtr entityId, bool doChildren);

    [EngineMethod("set_runtime_emission_rate_multiplier", false)]
    void SetRuntimeEmissionRateMultiplier(UIntPtr entityId, float emission_rate_multiplier);

    [EngineMethod("has_body", false)]
    bool HasBody(UIntPtr entityId);

    [EngineMethod("attach_nav_mesh_faces_to_entity", false)]
    void AttachNavigationMeshFaces(
      UIntPtr entityId,
      int faceGroupId,
      bool isConnected,
      bool isBlocker,
      bool autoLocalize);

    [EngineMethod("get_lod_level_for_distance_sq", false)]
    float GetLodLevelForDistanceSq(UIntPtr entityId, float dist_sq);

    [EngineMethod("is_entity_selected_on_editor", false)]
    bool IsEntitySelectedOnEditor(UIntPtr entityId);

    [EngineMethod("select_entity_on_editor", false)]
    void SelectEntityOnEditor(UIntPtr entityId);

    [EngineMethod("deselect_entity_on_editor", false)]
    void DeselectEntityOnEditor(UIntPtr entityId);

    [EngineMethod("is_dynamic_body_stationary", false)]
    bool IsDynamicBodyStationary(UIntPtr entityId);

    [EngineMethod("set_cull_mode", false)]
    void SetCullMode(UIntPtr entityPtr, MBMeshCullingMode cullMode);
  }
}
