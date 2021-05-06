// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBMission
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
  internal interface IMBMission
  {
    [EngineMethod("clear_resources", false)]
    void ClearResources(UIntPtr missionPointer);

    [EngineMethod("create_mission", false)]
    UIntPtr CreateMission(Mission mission);

    [EngineMethod("clear_agent_actions", false)]
    void ClearAgentActions(UIntPtr missionPointer);

    [EngineMethod("clear_missiles", false)]
    void ClearMissiles(UIntPtr missionPointer);

    [EngineMethod("clear_corpses", false)]
    void ClearCorpses(UIntPtr missionPointer, bool isMissionReset);

    [EngineMethod("get_time_speed", false)]
    float GetTimeSpeed(UIntPtr missionPointer);

    [EngineMethod("set_time_speed", false)]
    void SetTimeSpeed(UIntPtr missionPointer, float value);

    [EngineMethod("get_time_speed_end", false)]
    float GetTimeSpeedEnd(UIntPtr missionPointer);

    [EngineMethod("get_time_speed_period", false)]
    float GetTimeSpeedPeriod(UIntPtr missionPointer);

    [EngineMethod("set_time_speed_period", false)]
    void SetTimeSpeedPeriod(UIntPtr missionPointer, float value);

    [EngineMethod("get_time_speed_timer_elapsed_time", false)]
    float GetTimeSpeedTimerElapsedTime(UIntPtr missionPointer);

    [EngineMethod("get_pause_ai_tick", false)]
    bool GetPauseAITick(UIntPtr missionPointer);

    [EngineMethod("set_pause_ai_tick", false)]
    void SetPauseAITick(UIntPtr missionPointer, bool I);

    [EngineMethod("get_clear_scene_timer_elapsed_time", false)]
    float GetClearSceneTimerElapsedTime(UIntPtr missionPointer);

    [EngineMethod("reset_first_third_person_view", false)]
    void ResetFirstThirdPersonView(UIntPtr missionPointer);

    [EngineMethod("set_camera_is_first_person", false)]
    void SetCameraIsFirstPerson(bool value);

    [EngineMethod("set_camera_frame", false)]
    void SetCameraFrame(UIntPtr missionPointer, ref MatrixFrame cameraFrame, float zoomFactor);

    [EngineMethod("get_camera_frame", false)]
    MatrixFrame GetCameraFrame(UIntPtr missionPointer);

    [EngineMethod("get_is_loading_finished", false)]
    bool GetIsLoadingFinished(UIntPtr missionPointer);

    [EngineMethod("clear_scene", false)]
    void ClearScene(UIntPtr missionPointer);

    [EngineMethod("initialize_mission", false)]
    void InitializeMission(UIntPtr missionPointer, ref MissionInitializerRecord rec);

    [EngineMethod("finalize_mission", false)]
    void FinalizeMission(UIntPtr missionPointer);

    [EngineMethod("get_time", false)]
    float GetTime(UIntPtr missionPointer);

    [EngineMethod("get_average_fps", false)]
    float GetAverageFps(UIntPtr missionPointer);

    [EngineMethod("get_combat_type", false)]
    int GetCombatType(UIntPtr missionPointer);

    [EngineMethod("set_combat_type", false)]
    void SetCombatType(UIntPtr missionPointer, int combatType);

    [EngineMethod("ray_cast_for_closest_agent", false)]
    Agent RayCastForClosestAgent(
      UIntPtr missionPointer,
      Vec3 SourcePoint,
      Vec3 RayFinishPoint,
      int ExcludeAgentIndex,
      ref float CollisionDistance,
      float RayThickness);

    [EngineMethod("ray_cast_for_closest_agents_limbs", false)]
    bool RayCastForClosestAgentsLimbs(
      UIntPtr missionPointer,
      Vec3 SourcePoint,
      Vec3 RayFinishPoint,
      int ExcludeAgentIndex,
      ref float CollisionDistance,
      ref int AgentIndex,
      ref sbyte BoneIndex);

    [EngineMethod("ray_cast_for_given_agents_limbs", false)]
    bool RayCastForGivenAgentsLimbs(
      UIntPtr missionPointer,
      Vec3 SourcePoint,
      Vec3 RayFinishPoint,
      int GivenAgentIndex,
      ref float CollisionDistance,
      ref sbyte BoneIndex);

    [EngineMethod("get_number_of_teams", false)]
    int GetNumberOfTeams(UIntPtr missionPointer);

    [EngineMethod("reset_teams", false)]
    void ResetTeams(UIntPtr missionPointer);

    [EngineMethod("add_team", false)]
    int AddTeam(UIntPtr missionPointer);

    [EngineMethod("restart_record", false)]
    void RestartRecord(UIntPtr missionPointer);

    [EngineMethod("is_position_inside_boundaries", false)]
    bool IsPositionInsideBoundaries(UIntPtr missionPointer, Vec2 position);

    [EngineMethod("get_alternate_position_for_navmeshless_or_out_of_bounds_position", false)]
    WorldPosition GetAlternatePositionForNavmeshlessOrOutOfBoundsPosition(
      UIntPtr ptr,
      ref Vec2 directionTowards,
      ref WorldPosition originalPosition,
      ref float positionPenalty);

    [EngineMethod("add_missile", false)]
    int AddMissile(
      UIntPtr missionPointer,
      bool isPrediction,
      int shooterAgentIndex,
      in WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      float damageBonus,
      ref Vec3 position,
      ref Vec3 direction,
      ref Mat3 orientation,
      float baseSpeed,
      float speed,
      bool addRigidBody,
      UIntPtr entityPointer,
      int forcedMissileIndex,
      bool isPrimaryWeaponShot,
      out UIntPtr missileEntity);

    [EngineMethod("get_missile_collision_point", false)]
    Vec3 GetMissileCollisionPoint(
      UIntPtr missionPointer,
      Vec3 missileStartingPosition,
      Vec3 missileDirection,
      float missileStartingSpeed,
      in WeaponData weaponData);

    [EngineMethod("remove_missile", false)]
    void RemoveMissile(UIntPtr missionPointer, int missileIndex);

    [EngineMethod("get_missile_vertical_aim_correction", false)]
    float GetMissileVerticalAimCorrection(
      Vec3 vecToTarget,
      float missileStartingSpeed,
      ref WeaponStatsData weaponStatsData,
      float airFrictionConstant);

    [EngineMethod("get_missile_range", false)]
    float GetMissileRange(float missileStartingSpeed, float heightDifference);

    [EngineMethod("prepare_missile_weapon_for_drop", false)]
    void PrepareMissileWeaponForDrop(UIntPtr missionPointer, int missileIndex);

    [EngineMethod("add_particle_system_burst_by_name", false)]
    void AddParticleSystemBurstByName(
      UIntPtr missionPointer,
      string particleSystem,
      ref MatrixFrame frame,
      bool synchThroughNetwork);

    [EngineMethod("tick", false)]
    void Tick(UIntPtr missionPointer, float dt);

    [EngineMethod("make_sound", false)]
    void MakeSound(
      UIntPtr pointer,
      int nativeSoundCode,
      Vec3 position,
      bool soundCanBePredicted,
      bool isReliable,
      int relatedAgent1,
      int relatedAgent2);

    [EngineMethod("make_sound_with_parameter", false)]
    void MakeSoundWithParameter(
      UIntPtr pointer,
      int nativeSoundCode,
      Vec3 position,
      bool soundCanBePredicted,
      bool isReliable,
      int relatedAgent1,
      int relatedAgent2,
      SoundEventParameter parameter);

    [EngineMethod("make_sound_only_on_related_peer", false)]
    void MakeSoundOnlyOnRelatedPeer(
      UIntPtr pointer,
      int nativeSoundCode,
      Vec3 position,
      int relatedAgent);

    [EngineMethod("add_sound_alarm_factor_to_agents", false)]
    void AddSoundAlarmFactorToAgents(
      UIntPtr pointer,
      int ownerId,
      Vec3 position,
      float alarmFactor);

    [EngineMethod("get_enemy_alarm_state_indicator", false)]
    int GetEnemyAlarmStateIndicator(UIntPtr missionPointer);

    [EngineMethod("get_player_alarm_indicator", false)]
    float GetPlayerAlarmIndicator(UIntPtr missionPointer);

    [EngineMethod("create_agent", false)]
    Mission.AgentCreationResult CreateAgent(
      UIntPtr missionPointer,
      ulong monsterFlag,
      int forcedAgentIndex,
      bool isFemale,
      ref AgentSpawnData spawnData,
      ref CapsuleData bodyCapsule,
      ref CapsuleData crouchedBodyCapsule,
      ref AgentVisualsNativeData agentVisualsNativeData,
      ref AnimationSystemData animationSystemData,
      int instanceNo);

    [EngineMethod("get_position_of_missile", false)]
    Vec3 GetPositionOfMissile(UIntPtr missionPointer, int index);

    [EngineMethod("get_velocity_of_missile", false)]
    Vec3 GetVelocityOfMissile(UIntPtr missionPointer, int index);

    [EngineMethod("get_missile_has_rigid_body", false)]
    bool GetMissileHasRigidBody(UIntPtr missionPointer, int index);

    [EngineMethod("add_boundary", false)]
    bool AddBoundary(
      UIntPtr missionPointer,
      string name,
      Vec2[] boundaryPoints,
      int boundaryPointCount,
      bool isAllowanceInside);

    [EngineMethod("remove_boundary", false)]
    bool RemoveBoundary(UIntPtr missionPointer, string name);

    [EngineMethod("get_boundary_points", false)]
    void GetBoundaryPoints(
      UIntPtr missionPointer,
      string name,
      int boundaryPointOffset,
      Vec2[] boundaryPoints,
      int boundaryPointsSize,
      ref int retrievedPointCount);

    [EngineMethod("get_boundary_count", false)]
    int GetBoundaryCount(UIntPtr missionPointer);

    [EngineMethod("get_boundary_radius", false)]
    float GetBoundaryRadius(UIntPtr missionPointer, string name);

    [EngineMethod("get_boundary_name", false)]
    string GetBoundaryName(UIntPtr missionPointer, int boundaryIndex);

    [EngineMethod("get_closest_boundary_position", false)]
    Vec2 GetClosestBoundaryPosition(UIntPtr missionPointer, Vec2 position);

    [EngineMethod("get_navigation_points", false)]
    bool GetNavigationPoints(UIntPtr missionPointer, ref NavigationData navigationData);

    [EngineMethod("set_navigation_face_cost_with_id_around_position", false)]
    void SetNavigationFaceCostWithIdAroundPosition(
      UIntPtr missionPointer,
      int navigationFaceId,
      Vec3 position,
      float cost);

    [EngineMethod("pause_mission_scene_sounds", false)]
    void PauseMissionSceneSounds(UIntPtr missionPointer);

    [EngineMethod("resume_mission_scene_sounds", false)]
    void ResumeMissionSceneSounds(UIntPtr missionPointer);

    [EngineMethod("process_record_until_time", false)]
    void ProcessRecordUntilTime(UIntPtr missionPointer, float time);

    [EngineMethod("end_of_record", false)]
    bool EndOfRecord(UIntPtr missionPointer);

    [EngineMethod("start_recording", false)]
    void StartRecording();

    [EngineMethod("record_current_state", false)]
    void RecordCurrentState(UIntPtr missionPointer);

    [EngineMethod("backup_record_to_file", false)]
    void BackupRecordToFile(
      UIntPtr missionPointer,
      string fileName,
      string gameType,
      string sceneLevels);

    [EngineMethod("restore_record_from_file", false)]
    void RestoreRecordFromFile(UIntPtr missionPointer, string fileName);

    [EngineMethod("clear_record_buffers", false)]
    void ClearRecordBuffers(UIntPtr missionPointer);

    [EngineMethod("get_scene_name_for_replay", false)]
    string GetSceneNameForReplay(string replayName);

    [EngineMethod("get_game_type_for_replay", false)]
    string GetGameTypeForReplay(string replayName);

    [EngineMethod("get_scene_levels_for_replay", false)]
    string GetSceneLevelsForReplay(string replayName);

    [EngineMethod("get_atmosphere_name_for_replay", false)]
    string GetAtmosphereNameForReplay(string replayName);

    [EngineMethod("get_atmosphere_season_for_replay", false)]
    int GetAtmosphereSeasonForReplay(string replayName);

    [EngineMethod("get_closest_enemy", false)]
    Agent GetClosestEnemy(UIntPtr missionPointer, int teamIndex, Vec3 position, float radius);

    [EngineMethod("get_closest_ally", false)]
    Agent GetClosestAlly(UIntPtr missionPointer, int teamIndex, Vec3 position, float radius);

    [EngineMethod("get_agent_count_around_position", false)]
    void GetAgentCountAroundPosition(
      UIntPtr missionPointer,
      int teamIndex,
      Vec2 position,
      float radius,
      ref int allyCount,
      ref int enemyCount);

    [EngineMethod("find_agent_with_index", false)]
    Agent FindAgentWithIndex(UIntPtr missionPointer, int index);

    [EngineMethod("set_random_decide_time_of_agents", false)]
    void SetRandomDecideTimeOfAgents(
      UIntPtr missionPointer,
      int agentCount,
      int[] agentIndices,
      float minAIReactionTime,
      float maxAIReactionTime);

    [EngineMethod("get_average_morale_of_agents", false)]
    float GetAverageMoraleOfAgents(UIntPtr missionPointer, int agentCount, int[] agentIndices);

    [EngineMethod("get_best_slope_towards_direction", false)]
    WorldPosition GetBestSlopeTowardsDirection(
      UIntPtr missionPointer,
      ref WorldPosition centerPosition,
      float halfsize,
      ref WorldPosition referencePosition);

    [EngineMethod("get_best_slope_angle_height_pos_for_defending", false)]
    WorldPosition GetBestSlopeAngleHeightPosForDefending(
      UIntPtr missionPointer,
      WorldPosition enemyPosition,
      WorldPosition defendingPosition,
      int sampleSize,
      float distanceRatioAllowedFromDefendedPos,
      float distanceSqrdAllowedFromBoundary,
      float cosinusOfBestSlope,
      float cosinusOfMaxAcceptedSlope,
      float minSlopeScore,
      float maxSlopeScore,
      float excessiveSlopePenalty,
      float nearConeCenterRatio,
      float nearConeCenterBonus,
      float heightDifferenceCeiling,
      float maxDisplacementPenalty);

    [EngineMethod("get_nearby_agents_aux", false)]
    void GetNearbyAgentsAux(
      UIntPtr missionPointer,
      Vec2 center,
      float radius,
      int teamIndex,
      int friendOrEnemyOrAll,
      int agentsArrayOffset,
      int[] agentIds,
      int agentsArraySize,
      ref int retrievedAgentCount);

    [EngineMethod("get_weighted_point_of_enemies", false)]
    Vec2 GetWeightedPointOfEnemies(UIntPtr missionPointer, int agentIndex, Vec2 basePoint);

    [EngineMethod("is_formation_unit_position_available", false)]
    bool IsFormationUnitPositionAvailable(
      UIntPtr missionPointer,
      ref WorldPosition orderPosition,
      ref WorldPosition unitPosition,
      ref WorldPosition neareastAvailableUnitPosition,
      float manhattanDistance);

    [EngineMethod("get_straight_path_to_target", false)]
    WorldPosition GetStraightPathToTarget(
      UIntPtr scenePointer,
      Vec2 targetPosition,
      WorldPosition startingPosition,
      float samplingDistance,
      bool stopAtObstacle);

    [EngineMethod("set_bow_missile_speed_modifier", false)]
    void SetBowMissileSpeedModifier(UIntPtr missionPointer, float modifier);

    [EngineMethod("set_crossbow_missile_speed_modifier", false)]
    void SetCrossbowMissileSpeedModifier(UIntPtr missionPointer, float modifier);

    [EngineMethod("set_throwing_missile_speed_modifier", false)]
    void SetThrowingMissileSpeedModifier(UIntPtr missionPointer, float modifier);

    [EngineMethod("set_missile_range_modifier", false)]
    void SetMissileRangeModifier(UIntPtr missionPointer, float modifier);

    [EngineMethod("set_last_movement_key_pressed", false)]
    void SetLastMovementKeyPressed(
      UIntPtr missionPointer,
      Agent.MovementControlFlag lastMovementKeyPressed);

    [EngineMethod("fastforward_mission", false)]
    void FastForwardMission(UIntPtr missionPointer, float startTime, float endTime);

    [EngineMethod("get_debug_agent", false)]
    int GetDebugAgent(UIntPtr missionPointer);

    [EngineMethod("set_debug_agent", false)]
    void SetDebugAgent(UIntPtr missionPointer, int index);

    [EngineMethod("add_ai_debug_text", false)]
    void AddAiDebugText(UIntPtr missionPointer, string text);

    [EngineMethod("agent_proximity_map_begin_search", false)]
    AgentProximityMap.ProximityMapSearchStructInternal ProximityMapBeginSearch(
      UIntPtr missionPointer,
      Vec2 searchPos,
      float searchRadius);

    [EngineMethod("agent_proximity_map_find_next", false)]
    void ProximityMapFindNext(
      UIntPtr missionPointer,
      ref AgentProximityMap.ProximityMapSearchStructInternal searchStruct);

    [EngineMethod("agent_proximity_map_get_max_search_radius", false)]
    float ProximityMapMaxSearchRadius(UIntPtr missionPointer);

    [EngineMethod("get_biggest_agent_collision_padding", false)]
    float GetBiggestAgentCollisionPadding(UIntPtr missionPointer);

    [EngineMethod("set_report_stuck_agents_mode", false)]
    void SetReportStuckAgentsMode(UIntPtr missionPointer, bool value);

    [EngineMethod("batch_formation_unit_positions", false)]
    void BatchFormationUnitPositions(
      UIntPtr missionPointer,
      Vec2i[] orderedPositionIndices,
      Vec2[] orderedLocalPositions,
      int[] availabilityTable,
      WorldPosition[] globalPositionTable,
      WorldPosition orderPosition,
      Vec2 direction,
      int fileCount,
      int rankCount);

    [EngineMethod("toggle_disable_fall_avoid", false)]
    bool ToggleDisableFallAvoid();
  }
}
