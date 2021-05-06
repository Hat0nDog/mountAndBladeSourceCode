// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IScene
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IScene
  {
    [EngineMethod("create_new_scene", false)]
    Scene CreateNewScene(bool initialize_physics);

    [EngineMethod("get_path_between_ai_face_pointers", false)]
    bool GetPathBetweenAIFacePointers(
      UIntPtr scenePointer,
      UIntPtr startingAiFace,
      UIntPtr endingAiFace,
      Vec2 startingPosition,
      Vec2 endingPosition,
      float agentRadius,
      Vec2[] result,
      ref int pathSize);

    [EngineMethod("get_path_between_ai_face_indices", false)]
    bool GetPathBetweenAIFaceIndices(
      UIntPtr scenePointer,
      int startingAiFace,
      int endingAiFace,
      Vec2 startingPosition,
      Vec2 endingPosition,
      float agentRadius,
      Vec2[] result,
      ref int pathSize);

    [EngineMethod("get_path_distance_between_ai_faces", false)]
    bool GetPathDistanceBetweenAIFaces(
      UIntPtr scenePointer,
      int startingAiFace,
      int endingAiFace,
      Vec2 startingPosition,
      Vec2 endingPosition,
      float agentRadius,
      float distanceLimit,
      out float distance);

    [EngineMethod("get_nav_mesh_face_index", false)]
    void GetNavMeshFaceIndex(
      UIntPtr scenePointer,
      ref PathFaceRecord record,
      Vec2 position,
      bool checkIfDisabled,
      bool ignoreHeight);

    [EngineMethod("take_photo_mode_picture", false)]
    string TakePhotoModePicture(Scene scene);

    [EngineMethod("get_all_color_grade_names", false)]
    string GetAllColorGradeNames(Scene scene);

    [EngineMethod("get_all_filter_names", false)]
    string GetAllFilterNames(Scene scene);

    [EngineMethod("get_photo_mode_roll", false)]
    float GetPhotoModeRoll(Scene scene);

    [EngineMethod("get_photo_mode_orbit", false)]
    bool GetPhotoModeOrbit(Scene scene);

    [EngineMethod("get_photo_mode_on", false)]
    bool GetPhotoModeOn(Scene scene);

    [EngineMethod("get_photo_mode_focus", false)]
    void GetPhotoModeFocus(
      Scene scene,
      ref float focus,
      ref float focusStart,
      ref float focusEnd,
      ref float exposure,
      ref bool vignetteOn);

    [EngineMethod("get_scene_color_grade_index", false)]
    int GetSceneColorGradeIndex(Scene scene);

    [EngineMethod("get_scene_filter_index", false)]
    int GetSceneFilterIndex(Scene scene);

    [EngineMethod("set_photo_mode_roll", false)]
    void SetPhotoModeRoll(Scene scene, float roll);

    [EngineMethod("set_photo_mode_orbit", false)]
    void SetPhotoModeOrbit(Scene scene, bool orbit);

    [EngineMethod("set_photo_mode_on", false)]
    void SetPhotoModeOn(Scene scene, bool on);

    [EngineMethod("set_photo_mode_focus", false)]
    void SetPhotoModeFocus(
      Scene scene,
      float focusStart,
      float focusEnd,
      float focus,
      float exposure);

    [EngineMethod("set_photo_mode_vignette", false)]
    void SetPhotoModeVignette(Scene scene, bool vignetteOn);

    [EngineMethod("set_scene_color_grade_index", false)]
    void SetSceneColorGradeIndex(Scene scene, int index);

    [EngineMethod("set_scene_filter_index", false)]
    int SetSceneFilterIndex(Scene scene, int index);

    [EngineMethod("set_scene_color_grade", false)]
    void SetSceneColorGrade(Scene scene, string textureName);

    [EngineMethod("get_water_level", false)]
    float GetWaterLevel(Scene scene);

    [EngineMethod("get_terrain_material_index_at_layer", false)]
    int GetTerrainPhysicsMaterialIndexAtLayer(Scene scene, int layerIndex);

    [EngineMethod("create_burst_particle", false)]
    void CreateBurstParticle(Scene scene, int particleId, ref MatrixFrame frame);

    [EngineMethod("get_nav_mesh_face_index3", false)]
    void GetNavMeshFaceIndex3(
      UIntPtr scenePointer,
      ref PathFaceRecord record,
      Vec3 position,
      bool checkIfDisabled);

    [EngineMethod("set_upgrade_level", false)]
    void SetUpgradeLevel(UIntPtr scenePointer, int level);

    [EngineMethod("create_path_mesh", false)]
    MetaMesh CreatePathMesh(UIntPtr scenePointer, string baseEntityName, bool isWaterPath);

    [EngineMethod("set_active_visibility_levels", false)]
    void SetActiveVisibilityLevels(UIntPtr scenePointer, string levelsAppended);

    [EngineMethod("set_terrain_dynamic_params", false)]
    void SetTerrainDynamicParams(UIntPtr scenePointer, Vec3 dynamic_params);

    [EngineMethod("create_path_mesh2", false)]
    MetaMesh CreatePathMesh2(
      UIntPtr scenePointer,
      UIntPtr[] pathNodes,
      int pathNodeCount,
      bool isWaterPath);

    [EngineMethod("clear_all", false)]
    void ClearAll(UIntPtr scenePointer);

    [EngineMethod("check_resources", false)]
    void CheckResources(UIntPtr scenePointer);

    [EngineMethod("force_load_resources", false)]
    void ForceLoadResources(UIntPtr scenePointer);

    [EngineMethod("check_path_entities_frame_changed", false)]
    bool CheckPathEntitiesFrameChanged(UIntPtr scenePointer, string containsName);

    [EngineMethod("tick", false)]
    void Tick(UIntPtr scenePointer, float deltaTime);

    [EngineMethod("add_entity_with_mesh", false)]
    void AddEntityWithMesh(UIntPtr scenePointer, UIntPtr meshPointer, ref MatrixFrame frame);

    [EngineMethod("add_entity_with_multi_mesh", false)]
    void AddEntityWithMultiMesh(
      UIntPtr scenePointer,
      UIntPtr multiMeshPointer,
      ref MatrixFrame frame);

    [EngineMethod("add_item_entity", false)]
    GameEntity AddItemEntity(
      UIntPtr scenePointer,
      ref MatrixFrame frame,
      UIntPtr meshPointer);

    [EngineMethod("remove_entity", false)]
    void RemoveEntity(UIntPtr scenePointer, UIntPtr entityId, int removeReason);

    [EngineMethod("attach_entity", false)]
    bool AttachEntity(UIntPtr scenePointer, UIntPtr entity, bool showWarnings);

    [EngineMethod("get_terrain_height_and_normal", false)]
    void GetTerrainHeightAndNormal(
      UIntPtr scenePointer,
      Vec2 position,
      out float height,
      out Vec3 normal);

    [EngineMethod("resume_loading_renderings", false)]
    void ResumeLoadingRenderings(UIntPtr scenePointer);

    [EngineMethod("get_upgrade_level_mask", false)]
    uint GetUpgradeLevelMask(UIntPtr scenePointer);

    [EngineMethod("set_upgrade_level_visibility", false)]
    void SetUpgradeLevelVisibility(UIntPtr scenePointer, string concatLevels);

    [EngineMethod("set_upgrade_level_visibility_with_mask", false)]
    void SetUpgradeLevelVisibilityWithMask(UIntPtr scenePointer, uint mask);

    [EngineMethod("stall_loading_renderings", false)]
    void StallLoadingRenderingsUntilFurtherNotice(UIntPtr scenePointer);

    [EngineMethod("get_nav_mesh_face_center_position", false)]
    void GetNavMeshFaceCenterPosition(UIntPtr scenePointer, int navMeshFace, ref Vec3 centerPos);

    [EngineMethod("get_id_of_nav_mesh_face", false)]
    int GetIdOfNavMeshFace(UIntPtr scenePointer, int navMeshFace);

    [EngineMethod("set_cloth_simulation_state", false)]
    void SetClothSimulationState(UIntPtr scenePointer, bool state);

    [EngineMethod("get_first_entity_with_name", false)]
    GameEntity GetFirstEntityWithName(UIntPtr scenePointer, string entityName);

    [EngineMethod("get_first_entity_with_script_component", false)]
    GameEntity GetFirstEntityWithScriptComponent(
      UIntPtr scenePointer,
      string scriptComponentName);

    [EngineMethod("get_upgrade_level_mask_of_level_name", false)]
    uint GetUpgradeLevelMaskOfLevelName(UIntPtr scenePointer, string levelName);

    [EngineMethod("get_winter_time_factor", false)]
    float GetWinterTimeFactor(UIntPtr scenePointer);

    [EngineMethod("get_nav_mesh_face_first_vertex_z", false)]
    float GetNavMeshFaceFirstVertexZ(UIntPtr scenePointer, int navMeshFaceIndex);

    [EngineMethod("set_winter_time_factor", false)]
    void SetWinterTimeFactor(UIntPtr scenePointer, float winterTimeFactor);

    [EngineMethod("set_dryness_factor", false)]
    void SetDrynessFactor(UIntPtr scenePointer, float drynessFactor);

    [EngineMethod("get_fog", false)]
    float GetFog(UIntPtr scenePointer);

    [EngineMethod("set_fog", false)]
    void SetFog(UIntPtr scenePointer, float fogDensity, ref Vec3 fogColor, float fogFalloff);

    [EngineMethod("set_fog_advanced", false)]
    void SetFogAdvanced(
      UIntPtr scenePointer,
      float fogFalloffOffset,
      float fogFalloffMinFog,
      float fogFalloffStartDist);

    [EngineMethod("set_fog_ambient_color", false)]
    void SetFogAmbientColor(UIntPtr scenePointer, ref Vec3 fogAmbientColor);

    [EngineMethod("set_temperature", false)]
    void SetTemperature(UIntPtr scenePointer, float temperature);

    [EngineMethod("set_humidity", false)]
    void SetHumidity(UIntPtr scenePointer, float humidity);

    [EngineMethod("set_dynamic_shadowmap_cascades_radius_multiplier", false)]
    void SetDynamicShadowmapCascadesRadiusMultiplier(UIntPtr scenePointer, float extraRadius);

    [EngineMethod("set_env_map_multiplier", false)]
    void SetEnvironmentMultiplier(UIntPtr scenePointer, bool useMultiplier, float multiplier);

    [EngineMethod("set_sky_rotation", false)]
    void SetSkyRotation(UIntPtr scenePointer, float rotation);

    [EngineMethod("set_sky_brightness", false)]
    void SetSkyBrigthness(UIntPtr scenePointer, float brightness);

    [EngineMethod("set_forced_snow", false)]
    void SetForcedSnow(UIntPtr scenePointer, bool value);

    [EngineMethod("set_sun", false)]
    void SetSun(UIntPtr scenePointer, Vec3 color, float altitude, float angle, float intensity);

    [EngineMethod("set_sun_angle_altitude", false)]
    void SetSunAngleAltidude(UIntPtr scenePointer, float angle, float altitude);

    [EngineMethod("set_sun_light", false)]
    void SetSunLight(UIntPtr scenePointer, Vec3 color, Vec3 direction);

    [EngineMethod("set_sun_direction", false)]
    void SetSunDirection(UIntPtr scenePointer, Vec3 direction);

    [EngineMethod("set_sun_size", false)]
    void SetSunSize(UIntPtr scenePointer, float size);

    [EngineMethod("set_sunshafts_strength", false)]
    void SetSunShaftStrength(UIntPtr scenePointer, float strength);

    [EngineMethod("get_rain_density", false)]
    float GetRainDensity(UIntPtr scenePointer);

    [EngineMethod("set_rain_density", false)]
    void SetRainDensity(UIntPtr scenePointer, float density);

    [EngineMethod("get_snow_density", false)]
    float GetSnowDensity(UIntPtr scenePointer);

    [EngineMethod("set_snow_density", false)]
    void SetSnowDensity(UIntPtr scenePointer, float density);

    [EngineMethod("add_decal_instance", false)]
    void AddDecalInstance(
      UIntPtr scenePointer,
      UIntPtr decalMeshPointer,
      string decalSetID,
      bool deletable);

    [EngineMethod("set_shadow", false)]
    void SetShadow(UIntPtr scenePointer, bool shadowEnabled);

    [EngineMethod("add_point_light", false)]
    int AddPointLight(UIntPtr scenePointer, Vec3 position, float radius);

    [EngineMethod("add_directional_light", false)]
    int AddDirectionalLight(UIntPtr scenePointer, Vec3 position, Vec3 direction, float radius);

    [EngineMethod("set_light_position", false)]
    void SetLightPosition(UIntPtr scenePointer, int lightIndex, Vec3 position);

    [EngineMethod("set_light_diffuse_color", false)]
    void SetLightDiffuseColor(UIntPtr scenePointer, int lightIndex, Vec3 diffuseColor);

    [EngineMethod("set_light_direction", false)]
    void SetLightDirection(UIntPtr scenePointer, int lightIndex, Vec3 direction);

    [EngineMethod("calculate_effective_lighting", false)]
    bool CalculateEffectiveLighting(UIntPtr scenePointer);

    [EngineMethod("set_rayleigh_constant", false)]
    void SetMieScatterStrength(UIntPtr scenePointer, float strength);

    [EngineMethod("set_mie_scatter_particle_size", false)]
    void SetMieScatterFocus(UIntPtr scenePointer, float strength);

    [EngineMethod("set_brightpass_threshold", false)]
    void SetBrightpassTreshold(UIntPtr scenePointer, float threshold);

    [EngineMethod("set_min_exposure", false)]
    void SetMinExposure(UIntPtr scenePointer, float minExposure);

    [EngineMethod("set_max_exposure", false)]
    void SetMaxExposure(UIntPtr scenePointer, float maxExposure);

    [EngineMethod("set_target_exposure", false)]
    void SetTargetExposure(UIntPtr scenePointer, float targetExposure);

    [EngineMethod("set_middle_gray", false)]
    void SetMiddleGray(UIntPtr scenePointer, float middleGray);

    [EngineMethod("set_bloom_strength", false)]
    void SetBloomStrength(UIntPtr scenePointer, float bloomStrength);

    [EngineMethod("set_bloom_amount", false)]
    void SetBloomAmount(UIntPtr scenePointer, float bloomAmount);

    [EngineMethod("set_grain_amount", false)]
    void SetGrainAmount(UIntPtr scenePointer, float grainAmount);

    [EngineMethod("set_lens_flare_amount", false)]
    void SetLensFlareAmount(UIntPtr scenePointer, float lensFlareAmount);

    [EngineMethod("set_lens_flare_threshold", false)]
    void SetLensFlareThreshold(UIntPtr scenePointer, float lensFlareThreshold);

    [EngineMethod("set_lens_flare_strength", false)]
    void SetLensFlareStrength(UIntPtr scenePointer, float lensFlareStrength);

    [EngineMethod("set_lens_flare_dirt_weight", false)]
    void SetLensFlareDirtWeight(UIntPtr scenePointer, float lensFlareDirtWeight);

    [EngineMethod("set_lens_flare_diffraction_weight", false)]
    void SetLensFlareDiffractionWeight(UIntPtr scenePointer, float lensFlareDiffractionWeight);

    [EngineMethod("set_lens_flare_halo_weight", false)]
    void SetLensFlareHaloWeight(UIntPtr scenePointer, float lensFlareHaloWeight);

    [EngineMethod("set_lens_flare_ghost_weight", false)]
    void SetLensFlareGhostWeight(UIntPtr scenePointer, float lensFlareGhostWeight);

    [EngineMethod("set_lens_flare_halo_width", false)]
    void SetLensFlareHaloWidth(UIntPtr scenePointer, float lensFlareHaloWidth);

    [EngineMethod("set_lens_flare_ghost_samples", false)]
    void SetLensFlareGhostSamples(UIntPtr scenePointer, int lensFlareGhostSamples);

    [EngineMethod("set_lens_flare_aberration_offset", false)]
    void SetLensFlareAberrationOffset(UIntPtr scenePointer, float lensFlareAberrationOffset);

    [EngineMethod("set_lens_flare_blur_size", false)]
    void SetLensFlareBlurSize(UIntPtr scenePointer, int lensFlareBlurSize);

    [EngineMethod("set_lens_flare_blur_sigma", false)]
    void SetLensFlareBlurSigma(UIntPtr scenePointer, float lensFlareBlurSigma);

    [EngineMethod("set_streak_amount", false)]
    void SetStreakAmount(UIntPtr scenePointer, float streakAmount);

    [EngineMethod("set_streak_threshold", false)]
    void SetStreakThreshold(UIntPtr scenePointer, float streakThreshold);

    [EngineMethod("set_streak_strength", false)]
    void SetStreakStrength(UIntPtr scenePointer, float strengthAmount);

    [EngineMethod("set_streak_stretch", false)]
    void SetStreakStretch(UIntPtr scenePointer, float stretchAmount);

    [EngineMethod("set_streak_intensity", false)]
    void SetStreakIntensity(UIntPtr scenePointer, float stretchAmount);

    [EngineMethod("set_streak_tint", false)]
    void SetStreakTint(UIntPtr scenePointer, ref Vec3 p_streak_tint_color);

    [EngineMethod("set_hexagon_vignette_color", false)]
    void SetHexagonVignetteColor(UIntPtr scenePointer, ref Vec3 p_hexagon_vignette_color);

    [EngineMethod("set_hexagon_vignette_alpha", false)]
    void SetHexagonVignetteAlpha(UIntPtr scenePointer, float Alpha);

    [EngineMethod("set_vignette_inner_radius", false)]
    void SetVignetteInnerRadius(UIntPtr scenePointer, float vignetteInnerRadius);

    [EngineMethod("set_vignette_outer_radius", false)]
    void SetVignetteOuterRadius(UIntPtr scenePointer, float vignetteOuterRadius);

    [EngineMethod("set_vignette_opacity", false)]
    void SetVignetteOpacity(UIntPtr scenePointer, float vignetteOpacity);

    [EngineMethod("set_aberration_offset", false)]
    void SetAberrationOffset(UIntPtr scenePointer, float aberrationOffset);

    [EngineMethod("set_aberration_size", false)]
    void SetAberrationSize(UIntPtr scenePointer, float aberrationSize);

    [EngineMethod("set_aberration_smooth", false)]
    void SetAberrationSmooth(UIntPtr scenePointer, float aberrationSmooth);

    [EngineMethod("set_lens_distortion", false)]
    void SetLensDistortion(UIntPtr scenePointer, float lensDistortion);

    [EngineMethod("get_height_at_point", false)]
    bool GetHeightAtPoint(
      UIntPtr scenePointer,
      Vec2 point,
      BodyFlags excludeBodyFlags,
      ref float height);

    [EngineMethod("get_entity_count", false)]
    int GetEntityCount(UIntPtr scenePointer);

    [EngineMethod("get_entities", false)]
    void GetEntities(UIntPtr scenePointer, UIntPtr entityObjectsArrayPointer);

    [EngineMethod("get_root_entity_count", false)]
    int GetRootEntityCount(UIntPtr scenePointer);

    [EngineMethod("get_root_entities", false)]
    void GetRootEntities(Scene scene, NativeObjectArray output);

    [EngineMethod("get_entity_with_guid", false)]
    GameEntity GetEntityWithGuid(UIntPtr scenePointer, string guid);

    [EngineMethod("select_entities_in_box_with_script_component", false)]
    int SelectEntitiesInBoxWithScriptComponent(
      UIntPtr scenePointer,
      ref Vec3 boundingBoxMin,
      ref Vec3 boundingBoxMax,
      UIntPtr[] entitiesOutput,
      int maxCount,
      string scriptComponentName);

    [EngineMethod("select_entities_collided_with", false)]
    int SelectEntitiesCollidedWith(
      UIntPtr scenePointer,
      ref Ray ray,
      UIntPtr[] entityIds,
      Intersection[] intersections);

    [EngineMethod("generate_contacts_with_capsule", false)]
    int GenerateContactsWithCapsule(
      UIntPtr scenePointer,
      ref CapsuleData cap,
      BodyFlags exclude_flags,
      Intersection[] intersections);

    [EngineMethod("invalidate_terrain_physics_materials", false)]
    void InvalidateTerrainPhysicsMaterials(UIntPtr scenePointer);

    [EngineMethod("read", false)]
    void Read(UIntPtr scenePointer, string sceneName, ref SceneInitializationData initData);

    [EngineMethod("read_and_calculate_initial_camera", false)]
    void ReadAndCalculateInitialCamera(UIntPtr scenePointer, ref MatrixFrame outFrame);

    [EngineMethod("optimize_scene", false)]
    void OptimizeScene(UIntPtr scenePointer, bool optimizeFlora, bool optimizeOro);

    [EngineMethod("get_terrain_height", false)]
    float GetTerrainHeight(UIntPtr scenePointer, Vec2 position, bool checkHoles);

    [EngineMethod("get_normal_at", false)]
    Vec3 GetNormalAt(UIntPtr scenePointer, Vec2 position);

    [EngineMethod("has_terrain_heightmap", false)]
    bool HasTerrainHeightmap(UIntPtr scenePointer);

    [EngineMethod("contains_terrain", false)]
    bool ContainsTerrain(UIntPtr scenePointer);

    [EngineMethod("set_dof_focus", false)]
    void SetDofFocus(UIntPtr scenePointer, float dofFocus);

    [EngineMethod("set_dof_params", false)]
    void SetDofParams(
      UIntPtr scenePointer,
      float dofFocusStart,
      float dofFocusEnd,
      bool isVignetteOn);

    [EngineMethod("get_last_final_render_camera_position", false)]
    Vec3 GetLastFinalRenderCameraPosition(UIntPtr scenePointer);

    [EngineMethod("get_last_final_render_camera_frame", false)]
    void GetLastFinalRenderCameraFrame(UIntPtr scenePointer, ref MatrixFrame outFrame);

    [EngineMethod("get_time_of_day", false)]
    float GetTimeOfDay(UIntPtr scenePointer);

    [EngineMethod("set_time_of_day", false)]
    void SetTimeOfDay(UIntPtr scenePointer, float value);

    [EngineMethod("is_atmosphere_indoor", false)]
    bool IsAtmosphereIndoor(UIntPtr scenePointer);

    [EngineMethod("set_color_grade_blend", false)]
    void SetColorGradeBlend(UIntPtr scenePointer, string texture1, string texture2, float alpha);

    [EngineMethod("preload_for_rendering", false)]
    void PreloadForRendering(UIntPtr scenePointer);

    [EngineMethod("resume_scene_sounds", false)]
    void ResumeSceneSounds(UIntPtr scenePointer);

    [EngineMethod("finish_scene_sounds", false)]
    void FinishSceneSounds(UIntPtr scenePointer);

    [EngineMethod("pause_scene_sounds", false)]
    void PauseSceneSounds(UIntPtr scenePointer);

    [EngineMethod("get_ground_height_at_position", false)]
    float GetGroundHeightAtPosition(
      UIntPtr scenePointer,
      Vec3 position,
      uint excludeFlags,
      bool excludeInvisibleEntities);

    [EngineMethod("get_ground_height_and_normal_at_position", false)]
    float GetGroundHeightAndNormalAtPosition(
      UIntPtr scenePointer,
      Vec3 position,
      ref Vec3 normal,
      uint excludeFlags,
      bool excludeInvisibleEntities);

    [EngineMethod("check_point_can_see_point", false)]
    bool CheckPointCanSeePoint(
      UIntPtr scenePointer,
      Vec3 sourcePoint,
      Vec3 targetPoint,
      float distanceToCheck);

    [EngineMethod("ray_cast_for_closest_entity_or_terrain", false)]
    bool RayCastForClosestEntityOrTerrain(
      UIntPtr scenePointer,
      ref Vec3 sourcePoint,
      ref Vec3 targetPoint,
      float rayThickness,
      ref float collisionDistance,
      ref Vec3 closestPoint,
      ref UIntPtr entityIndex,
      BodyFlags bodyExcludeFlags);

    [EngineMethod("box_cast_only_for_camera", false)]
    bool BoxCastOnlyForCamera(
      UIntPtr scenePointer,
      Vec3[] boxPoints,
      ref Vec3 centerPoint,
      ref Vec3 dir,
      float distance,
      ref float collisionDistance,
      ref Vec3 closestPoint,
      ref UIntPtr entityIndex,
      BodyFlags bodyExcludeFlags,
      bool preFilter,
      bool postFilter);

    [EngineMethod("set_ability_of_faces_with_id", false)]
    void SetAbilityOfFacesWithId(UIntPtr scenePointer, int faceGroupId, bool isEnabled);

    [EngineMethod("swap_face_connections_with_id", false)]
    void SwapFaceConnectionsWithId(
      UIntPtr scenePointer,
      int hubFaceGroupID,
      int toBeSeparatedFaceGroupId,
      int toBeMergedFaceGroupId);

    [EngineMethod("merge_faces_with_id", false)]
    void MergeFacesWithId(
      UIntPtr scenePointer,
      int faceGroupId0,
      int faceGroupId1,
      int newFaceGroupId);

    [EngineMethod("separate_faces_with_id", false)]
    void SeparateFacesWithId(UIntPtr scenePointer, int faceGroupId0, int faceGroupId1);

    [EngineMethod("is_any_face_with_id", false)]
    bool IsAnyFaceWithId(UIntPtr scenePointer, int faceGroupId);

    [EngineMethod("load_nav_mesh_prefab", false)]
    void LoadNavMeshPrefab(UIntPtr scenePointer, string navMeshPrefabName, int navMeshGroupIdShift);

    [EngineMethod("get_navigation_mesh_face_for_position", false)]
    bool GetNavigationMeshFaceForPosition(
      UIntPtr scenePointer,
      ref Vec3 position,
      ref int faceGroupId,
      float heightDifferenceLimit);

    [EngineMethod("get_path_distance_between_positions", false)]
    bool GetPathDistanceBetweenPositions(
      UIntPtr scenePointer,
      ref WorldPosition position,
      ref WorldPosition destination,
      float agentRadius,
      ref float pathLength);

    [EngineMethod("is_line_to_point_clear", false)]
    bool IsLineToPointClear(
      UIntPtr scenePointer,
      int startingFace,
      Vec2 position,
      Vec2 destination,
      float agentRadius);

    [EngineMethod("is_line_to_point_clear2", false)]
    bool IsLineToPointClear2(
      UIntPtr scenePointer,
      UIntPtr startingFace,
      Vec2 position,
      Vec2 destination,
      float agentRadius);

    [EngineMethod("get_last_point_within_navigation_mesh_for_line_to_point", false)]
    Vec2 GetLastPointWithinNavigationMeshForLineToPoint(
      UIntPtr scenePointer,
      int startingFace,
      Vec2 position,
      Vec2 destination);

    [EngineMethod("does_path_exist_between_positions", false)]
    bool DoesPathExistBetweenPositions(
      UIntPtr scenePointer,
      WorldPosition position,
      WorldPosition destination);

    [EngineMethod("does_path_exist_between_faces", false)]
    bool DoesPathExistBetweenFaces(
      UIntPtr scenePointer,
      int firstNavMeshFace,
      int secondNavMeshFace,
      bool ignoreDisabled);

    [EngineMethod("ensure_postfx_system", false)]
    void EnsurePostfxSystem(UIntPtr scenePointer);

    [EngineMethod("set_bloom", false)]
    void SetBloom(UIntPtr scenePointer, bool mode);

    [EngineMethod("set_dof_mode", false)]
    void SetDofMode(UIntPtr scenePointer, bool mode);

    [EngineMethod("set_occlusion_mode", false)]
    void SetOcclusionMode(UIntPtr scenePointer, bool mode);

    [EngineMethod("set_external_injection_texture", false)]
    void SetExternalInjectionTexture(UIntPtr scenePointer, UIntPtr texturePointer);

    [EngineMethod("set_sunshaft_mode", false)]
    void SetSunshaftMode(UIntPtr scenePointer, bool mode);

    [EngineMethod("get_sun_direction", false)]
    Vec3 GetSunDirection(UIntPtr scenePointer);

    [EngineMethod("get_north_angle", false)]
    float GetNorthAngle(UIntPtr scenePointer);

    [EngineMethod("get_terrain_min_max_height", false)]
    bool GetTerrainMinMaxHeight(Scene scene, ref float min, ref float max);

    [EngineMethod("get_physics_min_max", false)]
    void GetPhysicsMinMax(UIntPtr scenePointer, ref Vec3 min_max);

    [EngineMethod("is_editor_scene", false)]
    bool IsEditorScene(UIntPtr scenePointer);

    [EngineMethod("set_motionblur_mode", false)]
    void SetMotionBlurMode(UIntPtr scenePointer, bool mode);

    [EngineMethod("set_antialiasing_mode", false)]
    void SetAntialiasingMode(UIntPtr scenePointer, bool mode);

    [EngineMethod("set_dlss_mode", false)]
    void SetDLSSMode(UIntPtr scenePointer, bool mode);

    [EngineMethod("get_path_with_name", false)]
    Path GetPathWithName(UIntPtr scenePointer, string name);

    [EngineMethod("delete_path_with_name", false)]
    void DeletePathWithName(UIntPtr scenePointer, string name);

    [EngineMethod("add_path", false)]
    void AddPath(UIntPtr scenePointer, string name);

    [EngineMethod("add_path_point", false)]
    void AddPathPoint(UIntPtr scenePointer, string name, ref MatrixFrame frame);

    [EngineMethod("get_bounding_box", false)]
    void GetBoundingBox(UIntPtr scenePointer, ref Vec3 min, ref Vec3 max);

    [EngineMethod("get_slow_motion_mode", false)]
    bool GetSlowMotionMode(UIntPtr scenePointer);

    [EngineMethod("set_slow_motion_mode", false)]
    void SetSlowMotionMode(UIntPtr scenePointer, bool value);

    [EngineMethod("set_name", false)]
    void SetName(UIntPtr scenePointer, string name);

    [EngineMethod("get_name", false)]
    string GetName(UIntPtr scenePointer);

    [EngineMethod("get_slow_motion_factor", false)]
    float GetSlowMotionFactor(UIntPtr scenePointer);

    [EngineMethod("set_slow_motion_factor", false)]
    void SetSlowMotionFactor(UIntPtr scenePointer, float value);

    [EngineMethod("set_owner_thread", false)]
    void SetOwnerThread(UIntPtr scenePointer);

    [EngineMethod("get_number_of_path_with_name_prefix", false)]
    int GetNumberOfPathsWithNamePrefix(UIntPtr ptr, string prefix);

    [EngineMethod("get_paths_with_name_prefix", false)]
    void GetPathsWithNamePrefix(UIntPtr ptr, UIntPtr[] points, string prefix);

    [EngineMethod("set_use_constant_time", false)]
    void SetUseConstantTime(UIntPtr ptr, bool value);

    [EngineMethod("set_play_sound_events_after_render_ready", false)]
    void SetPlaySoundEventsAfterReadyToRender(UIntPtr ptr, bool value);

    [EngineMethod("disable_static_shadows", false)]
    void DisableStaticShadows(UIntPtr ptr, bool value);

    [EngineMethod("get_skybox_mesh", false)]
    Mesh GetSkyboxMesh(UIntPtr ptr);

    [EngineMethod("set_atmosphere_with_name", false)]
    void SetAtmosphereWithName(UIntPtr ptr, string name);

    [EngineMethod("fill_entity_with_hard_border_physics_barrier", false)]
    void FillEntityWithHardBorderPhysicsBarrier(UIntPtr scenePointer, UIntPtr entityPointer);

    [EngineMethod("clear_decals", false)]
    void ClearDecals(UIntPtr scenePointer);

    [EngineMethod("get_scripted_entity_count", false)]
    int GetScriptedEntityCount(UIntPtr scenePointer);

    [EngineMethod("get_scripted_entity", false)]
    GameEntity GetScriptedEntity(UIntPtr scenePointer, int index);

    [EngineMethod("world_position_validate_z", false)]
    void WorldPositionValidateZ(ref WorldPosition position, int minimumValidityState);

    [EngineMethod("get_node_data_count", false)]
    int GetNodeDataCount(Scene scene, int xIndex, int yIndex);

    [EngineMethod("fill_terrain_height_data", false)]
    void FillTerrainHeightData(Scene scene, int xIndex, int yIndex, float[] heightArray);

    [EngineMethod("fill_terrain_physics_material_index_data", false)]
    void FillTerrainPhysicsMaterialIndexData(
      Scene scene,
      int xIndex,
      int yIndex,
      short[] materialIndexArray);

    [EngineMethod("get_terrain_data", false)]
    void GetTerrainData(
      Scene scene,
      out Vec2i nodeDimension,
      out float nodeSize,
      out int layerCount,
      out int layerVersion);

    [EngineMethod("get_terrain_node_data", false)]
    void GetTerrainNodeData(
      Scene scene,
      int xIndex,
      int yIndex,
      out int vertexCountAlongAxis,
      out float quadLength,
      out float minHeight,
      out float maxHeight);
  }
}
