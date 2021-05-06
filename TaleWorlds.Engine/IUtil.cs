// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IUtil
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IUtil
  {
    [EngineMethod("output_benchmark_values_to_performance_reporter", false)]
    void OutputBenchmarkValuesToPerformanceReporter();

    [EngineMethod("set_benchmark_status", false)]
    bool SetBenchmarkStatus(int status, string def);

    [EngineMethod("get_native_memory_statistics", false)]
    string GetNativeMemoryStatistics();

    [EngineMethod("command_line_argument_exits", false)]
    bool CommandLineArgumentExists(string str);

    [EngineMethod("export_nav_mesh_face_marks", false)]
    string ExportNavMeshFaceMarks(string file_name);

    [EngineMethod("take_ss_from_top", false)]
    string TakeSSFromTop(string file_name);

    [EngineMethod("check_if_assets_and_sources_are_same", false)]
    void CheckIfAssetsAndSourcesAreSame();

    [EngineMethod("get_snow_amount_data", false)]
    void GetSnowAmountData(byte[] snowData);

    [EngineMethod("find_and_clear_unused_resources", false)]
    void FindAndClearUnusedResources(
      string scene_names,
      int length_of_scene_names_string,
      uint operation_type);

    [EngineMethod("find_meshes_without_lods", false)]
    void FindMeshesWithoutLods(string module_name);

    [EngineMethod("set_print_callstack_at_crashes", false)]
    void SetPrintCallstackAtCrahses(bool value);

    [EngineMethod("set_disable_dump_generation", false)]
    void SetDisableDumpGeneration(bool value);

    [EngineMethod("get_modules_code", false)]
    string GetModulesCode();

    [EngineMethod("get_full_module_path", false)]
    string GetFullModulePath(string moduleName);

    [EngineMethod("get_full_module_paths", false)]
    string GetFullModulePaths();

    [EngineMethod("get_executable_working_directory", false)]
    string GetExecutableWorkingDirectory();

    [EngineMethod("add_main_thread_performance_query", false)]
    void AddMainThreadPerformanceQuery(string parent, string name, float seconds);

    [EngineMethod("set_dump_folder_path", false)]
    void SetDumpFolderPath(string path);

    [EngineMethod("check_scene_for_problems", false)]
    void CheckSceneForProblems(string path);

    [EngineMethod("set_screen_text_rendering_state", false)]
    void SetScreenTextRenderingState(bool value);

    [EngineMethod("set_message_line_rendering_state", false)]
    void SetMessageLineRenderingState(bool value);

    [EngineMethod("start_telemetry_connection", false)]
    void StartTelemetryConnection(bool showErrors);

    [EngineMethod("check_shader_compilation", false)]
    bool CheckShaderCompilation();

    [EngineMethod("compile_terrain_shaders", false)]
    void CompileTerrainShaders();

    [EngineMethod("set_crash_on_asserts", false)]
    void SetCrashOnAsserts(bool val);

    [EngineMethod("set_crash_on_warnings", false)]
    void SetCrashOnWarnings(bool val);

    [EngineMethod("compile_single_terrain_shader", false)]
    void CompileSingleTerrainShader(
      string scene_name,
      string module_name,
      string compile_configuration);

    [EngineMethod("compile_all_shaders", false)]
    void CompileAllShaders(string targetPlatform);

    [EngineMethod("toggle_render", false)]
    void ToggleRender();

    [EngineMethod("is_terrain_shader_compile_finished", false)]
    bool IsTerrainShaderCompileFinished();

    [EngineMethod("stop_telemetry_connection", false)]
    void StopTelemetryConnection();

    [EngineMethod("begin_telemetry_scope", false)]
    uint BeginTelemetryScope(TelemetryLevelMask levelMask, string scopeName);

    [EngineMethod("end_telemetry_scope", false)]
    void EndTelemetryScope(uint scopeId);

    [EngineMethod("has_telemetry_connection", false)]
    bool HasTelemetryConnection();

    [EngineMethod("execute_command_line_command", false)]
    void ExecuteCommandLineCommand(string command);

    [EngineMethod("quit_game", false)]
    void QuitGame();

    [EngineMethod("start_scene_performance_report", false)]
    void StartScenePerformanceReport(string folderPath);

    [EngineMethod("get_base_directory", false)]
    string GetBaseDirectory();

    [EngineMethod("get_visual_tests_test_files_path", false)]
    string GetVisualTestsTestFilesPath();

    [EngineMethod("get_visual_tests_validate_path", false)]
    string GetVisualTestsValidatePath();

    [EngineMethod("get_attachments_path", false)]
    string GetAttachmentsPath();

    [EngineMethod("set_thumbnail_creator_view_to_release_resources", false)]
    void SetThumbnailCreatorViewForResourceCleanup(UIntPtr pointer);

    [EngineMethod("is_scene_performance_report_finished", false)]
    bool IsSceneReportFinished();

    [EngineMethod("flush_managed_objects_memory", false)]
    void FlushManagedObjectsMemory();

    [EngineMethod("set_render_mode", false)]
    void SetRenderMode(int mode);

    [EngineMethod("add_performance_report_token", false)]
    void AddPerformanceReportToken(string performance_type, string name, float loading_time);

    [EngineMethod("add_scene_object_report", false)]
    void AddSceneObjectReport(string scene_name, string report_name, float report_value);

    [EngineMethod("output_performance_reports", false)]
    void OutputPerformanceReports();

    [EngineMethod("add_command_line_function", false)]
    void AddCommandLineFunction(string concatName);

    [EngineMethod("get_number_of_shader_compilations_in_progress", false)]
    int GetNumberOfShaderCompilationsInProgress();

    [EngineMethod("get_editor_selected_entity_count", false)]
    int GetEditorSelectedEntityCount();

    [EngineMethod("get_entity_count_of_selection_set", false)]
    int GetEntityCountOfSelectionSet(string name);

    [EngineMethod("get_entities_of_selection_set", false)]
    void GetEntitiesOfSelectionSet(string name, UIntPtr[] gameEntitiesTemp);

    [EngineMethod("get_editor_selected_entities", false)]
    void GetEditorSelectedEntities(UIntPtr[] gameEntitiesTemp);

    [EngineMethod("delete_entities_in_editor_scene", false)]
    void DeleteEntitiesInEditorScene(UIntPtr[] gameEntities, int entityCount);

    [EngineMethod("create_selection_set_in_editor", false)]
    void CreateSelectionInEditor(UIntPtr[] gameEntities, int entityCount, string name);

    [EngineMethod("select_entities_in_editor", false)]
    void SelectEntities(UIntPtr[] gameEntities, int entityCount);

    [EngineMethod("get_current_cpu_memory_usage", false)]
    ulong GetCurrentCpuMemoryUsage();

    [EngineMethod("get_gpu_memory_stats", false)]
    void GetGPUMemoryStats(
      ref float totalMemory,
      ref float renderTargetMemory,
      ref float depthTargetMemory,
      ref float srvMemory,
      ref float bufferMemory);

    [EngineMethod("get_gpu_memory_of_allocation_group", false)]
    ulong GetGpuMemoryOfAllocationGroup(string allocationName);

    [EngineMethod("get_detailed_gpu_buffer_memory_stats", false)]
    void GetDetailedGPUBufferMemoryStats(
      ref int totalMemoryAllocated,
      ref int totalMemoryUsed,
      ref int emptyChunkCount);

    [EngineMethod("is_detailed_soung_log_on", false)]
    int IsDetailedSoundLogOn();

    [EngineMethod("get_main_fps", false)]
    float GetMainFps();

    [EngineMethod("disable_global_loading_window", false)]
    void DisableGlobalLoadingWindow();

    [EngineMethod("enable_global_loading_window", false)]
    void EnableGlobalLoadingWindow();

    [EngineMethod("enable_global_edit_data_cacher", false)]
    void EnableGlobalEditDataCacher();

    [EngineMethod("get_renderer_fps", false)]
    float GetRendererFps();

    [EngineMethod("disable_global_edit_data_cacher", false)]
    void DisableGlobalEditDataCacher();

    [EngineMethod("enable_single_gpu_query_per_frame", false)]
    void EnableSingleGPUQueryPerFrame();

    [EngineMethod("get_fps", false)]
    float GetFps();

    [EngineMethod("get_full_command_line_string", false)]
    string GetFullCommandLineString();

    [EngineMethod("take_screenshot", false)]
    void TakeScreenshot(string path);

    [EngineMethod("check_resource_modifications", false)]
    void CheckResourceModifications();

    [EngineMethod("set_graphics_preset", false)]
    void SetGraphicsPreset(int preset);

    [EngineMethod("clear_old_resources_and_objects", false)]
    void ClearOldResourcesAndObjects();

    [EngineMethod("get_delta_time", false)]
    float GetDeltaTime(int timerId);

    [EngineMethod("load_sky_boxes", false)]
    void LoadSkyBoxes();

    [EngineMethod("get_engine_frame_no", false)]
    int GetEngineFrameNo();

    [EngineMethod("set_allocation_always_valid_scene", false)]
    void SetAllocationAlwaysValidScene(UIntPtr scene);

    [EngineMethod("get_console_host_machine", false)]
    string GetConsoleHostMachine();

    [EngineMethod("is_edit_mode_enabled", false)]
    bool IsEditModeEnabled();

    [EngineMethod("get_pc_info", false)]
    string GetPCInfo();

    [EngineMethod("get_gpu_memory_mb", false)]
    int GetGPUMemoryMB();

    [EngineMethod("get_current_estimated_gpu_memory_cost_mb", false)]
    int GetCurrentEstimatedGPUMemoryCostMB();

    [EngineMethod("dump_gpu_memory_statistics", false)]
    void DumpGPUMemoryStatistics(string filePath);

    [EngineMethod("save_data_as_texture", false)]
    int SaveDataAsTexture(string path, int width, int height, float[] data);

    [EngineMethod("get_application_name", false)]
    string GetApplicationName();

    [EngineMethod("set_window_title", false)]
    void SetWindowTitle(string title);

    [EngineMethod("process_window_title", false)]
    string ProcessWindowTitle(string title);

    [EngineMethod("get_current_process_id", false)]
    uint GetCurrentProcessID();

    [EngineMethod("do_delayed_exit", false)]
    void DoDelayedexit(int returnCode);

    [EngineMethod("set_report_mode", false)]
    void SetReportMode(bool reportMode);

    [EngineMethod("set_assertions_and_warnings_set_exit_code", false)]
    void SetAssertionsAndWarningsSetExitCode(bool value);

    [EngineMethod("set_assertion_at_shader_compile", false)]
    void SetAssertionAtShaderCompile(bool value);

    [EngineMethod("did_automated_gi_bake_finished", false)]
    bool DidAutomatedGIBakeFinished();

    [EngineMethod("do_full_bake_all_levels_automated", false)]
    void DoFullBakeAllLevelsAutomated(string module, string sceneName);

    [EngineMethod("do_full_bake_single_level_automated", false)]
    void DoFullBakeSingleLevelAutomated(string module, string sceneName);

    [EngineMethod("get_return_code", false)]
    int GetReturnCode();

    [EngineMethod("do_light_only_bake_single_level_automated", false)]
    void DoLightOnlyBakeSingleLevelAutomated(string module, string sceneName);

    [EngineMethod("do_light_only_bake_all_levels_automated", false)]
    void DoLightOnlyBakeAllLevelsAutomated(string module, string sceneName);

    [EngineMethod("get_local_output_dir", false)]
    string GetLocalOutputPath();

    [EngineMethod("get_configs_path", false)]
    string GetConfigsPath();

    [EngineMethod("get_game_saves_path", false)]
    string GetGameSavesPath();

    [EngineMethod("get_recordings_path", false)]
    string GetRecordingsPath();

    [EngineMethod("get_temp_path", false)]
    string GetTempPath();

    [EngineMethod("set_crash_report_custom_string", false)]
    void SetCrashReportCustomString(string customString);

    [EngineMethod("set_crash_report_custom_managed_stack", false)]
    void SetCrashReportCustomStack(string customStack);

    [EngineMethod("get_steam_appid", false)]
    int GetSteamAppId();

    [EngineMethod("set_force_vsync", false)]
    void SetForceVsync(bool value);

    [EngineMethod("save_game_file", false)]
    void SaveGameFile(string fileName, byte[] data);

    [EngineMethod("load_game_file", false)]
    void LoadGameFile(string fileName, byte[] data);

    [EngineMethod("load_game_file_meta_data", false)]
    void LoadGameFileMetadata(string fileName, byte[] data);

    [EngineMethod("get_all_save_file_names", false)]
    string GetAllSaveFileNames();

    [EngineMethod("get_game_metadata_length", false)]
    ulong GetGameMetadataLength(string fileName);

    [EngineMethod("get_game_file_length", false)]
    ulong GetGameFileLength(string fileName);

    [EngineMethod("delete_save_game_file", false)]
    void DeleteSaveGameFile(string fileName);

    [EngineMethod("save_config_file", false)]
    void SaveConfigFile(string fileName, string configProperties);

    [EngineMethod("load_config_file", false)]
    string LoadConfigFile(string fileName);

    [EngineMethod("open_onscreen_keyboard", false)]
    void OpenOnscreenKeyboard(string initialText, int maxLength, int keyboardTypeEnum);

    [EngineMethod("register_gpu_allocation_group", false)]
    int RegisterGPUAllocationGroup(string name);
  }
}
