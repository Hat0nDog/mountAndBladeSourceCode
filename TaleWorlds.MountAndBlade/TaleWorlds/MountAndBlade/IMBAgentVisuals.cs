// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBAgentVisuals
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
  internal interface IMBAgentVisuals
  {
    [EngineMethod("create_agent_renderer_scene_controller", false)]
    UIntPtr CreateAgentRendererSceneController(UIntPtr scenePointer, int maxRenderCount);

    [EngineMethod("destruct_agent_renderer_scene_controller", false)]
    void DestructAgentRendererSceneController(
      UIntPtr scenePointer,
      UIntPtr agentRendererSceneControllerPointer);

    [EngineMethod("create_agent_visuals", false)]
    MBAgentVisuals CreateAgentVisuals(
      UIntPtr scenePtr,
      string ownerName,
      Vec3 eyeOffset);

    [EngineMethod("tick", false)]
    void Tick(
      UIntPtr agentVisualsId,
      UIntPtr parentAgentVisualsId,
      float dt,
      bool entityMoving,
      float speed);

    [EngineMethod("set_entity", false)]
    void SetEntity(UIntPtr agentVisualsId, UIntPtr entityPtr);

    [EngineMethod("set_skeleton", false)]
    void SetSkeleton(UIntPtr agentVisualsId, UIntPtr skeletonPtr);

    [EngineMethod("add_skin_meshes_to_agent_visuals", false)]
    void AddSkinMeshesToAgentEntity(
      UIntPtr agentVisualsId,
      ref SkinGenerationParams skinParams,
      ref BodyProperties bodyProperties,
      bool useFaceCache);

    [EngineMethod("set_lod_atlas_shading_index", false)]
    void SetLodAtlasShadingIndex(
      UIntPtr agentVisualsId,
      int index,
      bool useTeamColor,
      uint teamColor1,
      uint teamColor2);

    [EngineMethod("set_face_generation_params", false)]
    void SetFaceGenerationParams(UIntPtr agentVisualsId, FaceGenerationParams faceGenerationParams);

    [EngineMethod("start_rhubarb_record", false)]
    void StartRhubarbRecord(UIntPtr agentVisualsId, string path, int soundId);

    [EngineMethod("clear_visual_components", false)]
    void ClearVisualComponents(UIntPtr agentVisualsId, bool removeSkeleton);

    [EngineMethod("lazy_update_agent_renderer_data", false)]
    void LazyUpdateAgentRendererData(UIntPtr agentVisualsId);

    [EngineMethod("add_mesh", false)]
    void AddMesh(UIntPtr agentVisualsId, UIntPtr meshPointer);

    [EngineMethod("remove_mesh", false)]
    void RemoveMesh(UIntPtr agentVisualsPtr, UIntPtr meshPointer);

    [EngineMethod("add_multi_mesh", false)]
    void AddMultiMesh(UIntPtr agentVisualsPtr, UIntPtr multiMeshPointer, int bodyMeshIndex);

    [EngineMethod("add_horse_reins_cloth_mesh", false)]
    void AddHorseReinsClothMesh(
      UIntPtr agentVisualsPtr,
      UIntPtr reinMeshPointer,
      UIntPtr ropeMeshPointer);

    [EngineMethod("update_skeleton_scale", false)]
    void UpdateSkeletonScale(UIntPtr agentVisualsId, int bodyDeformType);

    [EngineMethod("apply_skeleton_scale", false)]
    void ApplySkeletonScale(
      UIntPtr agentVisualsId,
      Vec3 mountSitBoneScale,
      float mountRadiusAdder,
      byte boneCount,
      sbyte[] boneIndices,
      Vec3[] boneScales);

    [EngineMethod("batch_last_lod_meshes", false)]
    void BatchLastLodMeshes(UIntPtr agentVisualsPtr);

    [EngineMethod("remove_multi_mesh", false)]
    void RemoveMultiMesh(UIntPtr agentVisualsPtr, UIntPtr multiMeshPointer, int bodyMeshIndex);

    [EngineMethod("add_weapon_to_agent_entity", false)]
    void AddWeaponToAgentEntity(
      UIntPtr agentVisualsPtr,
      int slotIndex,
      in WeaponData agentEntityData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      in WeaponData agentEntityAmmoData,
      WeaponStatsData[] ammoWeaponStatsData,
      int ammoWeaponStatsDataLength,
      GameEntity cachedEntity);

    [EngineMethod("update_quiver_mesh_of_weapon_in_slot", false)]
    void UpdateQuiverMeshesWithoutAgent(
      UIntPtr agentVisualsId,
      int weaponIndex,
      int ammoCountToShow);

    [EngineMethod("set_wielded_weapon_indices", false)]
    void SetWieldedWeaponIndices(
      UIntPtr agentVisualsId,
      int slotIndexRightHand,
      int slotIndexLeftHand);

    [EngineMethod("clear_all_weapon_meshes", false)]
    void ClearAllWeaponMeshes(UIntPtr agentVisualsPtr);

    [EngineMethod("clear_weapon_meshes", false)]
    void ClearWeaponMeshes(UIntPtr agentVisualsPtr, int weaponVisualIndex);

    [EngineMethod("make_voice", false)]
    void MakeVoice(UIntPtr agentVisualsPtr, int voiceId, ref Vec3 position);

    [EngineMethod("set_setup_morph_node", false)]
    void SetSetupMorphNode(UIntPtr agentVisualsPtr, bool value);

    [EngineMethod("use_scaled_weapons", false)]
    void UseScaledWeapons(UIntPtr agentVisualsPtr, bool value);

    [EngineMethod("set_cloth_component_keep_state_of_all_meshes", false)]
    void SetClothComponentKeepStateOfAllMeshes(UIntPtr agentVisualsPtr, bool keepState);

    [EngineMethod("get_current_helmet_scaling_factor", false)]
    Vec3 GetCurrentHelmetScalingFactor(UIntPtr agentVisualsPtr);

    [EngineMethod("set_voice_definition_index", false)]
    void SetVoiceDefinitionIndex(
      UIntPtr agentVisualsPtr,
      int voiceDefinitionIndex,
      float voicePitch);

    [EngineMethod("set_agent_lod_level_external", false)]
    void SetAgentLodLevelExternal(UIntPtr agentVisualsPtr, float level);

    [EngineMethod("set_agent_local_speed", false)]
    void SetAgentLocalSpeed(UIntPtr agentVisualsPtr, Vec2 speed);

    [EngineMethod("set_look_direction", false)]
    void SetLookDirection(UIntPtr agentVisualsPtr, Vec3 direction);

    [EngineMethod("reset", false)]
    void Reset(UIntPtr agentVisualsPtr);

    [EngineMethod("reset_next_frame", false)]
    void ResetNextFrame(UIntPtr agentVisualsPtr);

    [EngineMethod("set_frame", false)]
    void SetFrame(UIntPtr agentVisualsPtr, ref MatrixFrame frame);

    [EngineMethod("get_frame", false)]
    void GetFrame(UIntPtr agentVisualsPtr, ref MatrixFrame outFrame);

    [EngineMethod("get_global_frame", false)]
    void GetGlobalFrame(UIntPtr agentVisualsPtr, ref MatrixFrame outFrame);

    [EngineMethod("set_visible", false)]
    void SetVisible(UIntPtr agentVisualsPtr, bool value);

    [EngineMethod("get_visible", false)]
    bool GetVisible(UIntPtr agentVisualsPtr);

    [EngineMethod("get_skeleton", false)]
    Skeleton GetSkeleton(UIntPtr agentVisualsPtr);

    [EngineMethod("get_entity", false)]
    GameEntity GetEntity(UIntPtr agentVisualsPtr);

    [EngineMethod("get_global_stable_eye_point", false)]
    Vec3 GetGlobalStableEyePoint(UIntPtr agentVisualsPtr, bool isHumanoid);

    [EngineMethod("get_global_stable_neck_point", false)]
    Vec3 GetGlobalStableNeckPoint(UIntPtr agentVisualsPtr, bool isHumanoid);

    [EngineMethod("add_prefab_to_agent_visual_bone", false)]
    void AddPrefabToAgentVisualBone(UIntPtr agentVisualsPtr, string prefabName, sbyte boneIndex);

    [EngineMethod("get_attached_weapon_entity", false)]
    GameEntity GetAttachedWeaponEntity(UIntPtr agentVisualsPtr, int attachedWeaponIndex);

    [EngineMethod("create_particle_system_attached_to_bone", false)]
    void CreateParticleSystemAttachedToBone(
      UIntPtr agentVisualsPtr,
      int runtimeParticleindex,
      sbyte boneIndex,
      ref MatrixFrame boneLocalParticleFrame);

    [EngineMethod("check_resources", false)]
    bool CheckResources(UIntPtr agentVisualsPtr, bool addToQueue);

    [EngineMethod("add_child_entity", false)]
    bool AddChildEntity(UIntPtr agentVisualsPtr, UIntPtr EntityId);

    [EngineMethod("set_cloth_wind_to_weapon_at_index", false)]
    void SetClothWindToWeaponAtIndex(
      UIntPtr agentVisualsPtr,
      Vec3 windDirection,
      bool isLocal,
      int index);

    [EngineMethod("remove_child_entity", false)]
    void RemoveChildEntity(UIntPtr agentVisualsPtr, UIntPtr EntityId, int removeReason);

    [EngineMethod("disable_contour", false)]
    void DisableContour(UIntPtr agentVisualsPtr);

    [EngineMethod("set_as_contour_entity", false)]
    void SetAsContourEntity(UIntPtr agentVisualsPtr, uint color);

    [EngineMethod("set_contour_state", false)]
    void SetContourState(UIntPtr agentVisualsPtr, bool alwaysVisible);

    [EngineMethod("set_enable_occlusion_culling", false)]
    void SetEnableOcclusionCulling(UIntPtr agentVisualsPtr, bool enable);

    [EngineMethod("get_bone_type_data", false)]
    void GetBoneTypeData(UIntPtr pointer, sbyte boneIndex, ref BoneBodyTypeData boneBodyTypeData);
  }
}
