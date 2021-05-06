// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBEditor
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBEditor
  {
    [EngineMethod("is_edit_mode", false)]
    bool IsEditMode();

    [EngineMethod("is_edit_mode_enabled", false)]
    bool IsEditModeEnabled();

    [EngineMethod("update_scene_tree", false)]
    void UpdateSceneTree();

    [EngineMethod("is_entity_selected", false)]
    bool IsEntitySelected(UIntPtr entityId);

    [EngineMethod("add_editor_warning", false)]
    void AddEditorWarning(UIntPtr entityId, string warningText);

    [EngineMethod("render_editor_mesh", false)]
    void RenderEditorMesh(UIntPtr metaMeshId, ref MatrixFrame frame);

    [EngineMethod("enter_edit_mode", false)]
    void EnterEditMode(
      UIntPtr sceneWidgetPointer,
      ref MatrixFrame initialCameraFrame,
      float initialCameraElevation,
      float initialCameraBearing);

    [EngineMethod("tick_edit_mode", false)]
    void TickEditMode(float dt);

    [EngineMethod("leave_edit_mode", false)]
    void LeaveEditMode();

    [EngineMethod("enter_edit_mission_mode", false)]
    void EnterEditMissionMode(UIntPtr missionPointer);

    [EngineMethod("leave_edit_mission_mode", false)]
    void LeaveEditMissionMode();

    [EngineMethod("activate_scene_editor_presentation", false)]
    void ActivateSceneEditorPresentation();

    [EngineMethod("deactivate_scene_editor_presentation", false)]
    void DeactivateSceneEditorPresentation();

    [EngineMethod("tick_scene_editor_presentation", false)]
    void TickSceneEditorPresentation(float dt);

    [EngineMethod("get_editor_scene_view", false)]
    SceneView GetEditorSceneView();

    [EngineMethod("helpers_enabled", false)]
    bool HelpersEnabled();

    [EngineMethod("border_helpers_enabled", false)]
    bool BorderHelpersEnabled();

    [EngineMethod("zoom_to_position", false)]
    void ZoomToPosition(Vec3 pos);

    [EngineMethod("add_entity_warning", false)]
    void AddEntityWarning(UIntPtr entityId, string msg);

    [EngineMethod("get_all_prefabs_and_child_with_tag", false)]
    string GetAllPrefabsAndChildWithTag(string tag);

    [EngineMethod("set_upgrade_level_visibility", false)]
    void SetUpgradeLevelVisibility(string cumulated_string);

    [EngineMethod("exit_edit_mode", false)]
    void ExitEditMode();

    [EngineMethod("is_replay_manager_recording", false)]
    bool IsReplayManagerRecording();

    [EngineMethod("is_replay_manager_rendering", false)]
    bool IsReplayManagerRendering();

    [EngineMethod("is_replay_manager_replaying", false)]
    bool IsReplayManagerReplaying();
  }
}
