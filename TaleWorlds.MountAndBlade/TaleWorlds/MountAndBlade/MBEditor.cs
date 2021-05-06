// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBEditor
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MBEditor
  {
    public static Scene _editorScene;
    private static MBAgentRendererSceneController _agentRendererSceneController;
    public static bool _isEditorMissionOn;

    [MBCallback]
    internal static void SetEditorScene(Scene scene)
    {
      if ((NativeObject) MBEditor._editorScene != (NativeObject) null)
      {
        if (MBEditor._agentRendererSceneController != null)
          MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController);
        MBEditor._editorScene.ClearAll();
      }
      MBEditor._editorScene = scene;
      MBEditor._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(MBEditor._editorScene, 32);
    }

    [MBCallback]
    internal static void CloseEditorScene()
    {
      if (MBEditor._agentRendererSceneController != null)
        MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController);
      MBEditor._agentRendererSceneController = (MBAgentRendererSceneController) null;
      MBEditor._editorScene = (Scene) null;
    }

    [MBCallback]
    internal static void DestroyEditor(Scene scene)
    {
      MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController);
      MBEditor._editorScene.ClearAll();
      MBEditor._editorScene = (Scene) null;
      MBEditor._agentRendererSceneController = (MBAgentRendererSceneController) null;
    }

    public static bool IsEditModeOn => MBAPI.IMBEditor.IsEditMode();

    public static bool EditModeEnabled => MBAPI.IMBEditor.IsEditModeEnabled();

    public static void UpdateSceneTree() => MBAPI.IMBEditor.UpdateSceneTree();

    public static bool IsEntitySelected(GameEntity entity) => MBAPI.IMBEditor.IsEntitySelected(entity.Pointer);

    public static void RenderEditorMesh(MetaMesh mesh, MatrixFrame frame) => MBAPI.IMBEditor.RenderEditorMesh(mesh.Pointer, ref frame);

    public static void EnterEditMode(
      SceneView sceneView,
      MatrixFrame initialCameraFrame,
      float initialCameraElevation,
      float initialCameraBearing)
    {
      MBAPI.IMBEditor.EnterEditMode(sceneView.Pointer, ref initialCameraFrame, initialCameraElevation, initialCameraBearing);
    }

    public static void TickEditMode(float dt) => MBAPI.IMBEditor.TickEditMode(dt);

    public static void LeaveEditMode()
    {
      MBAPI.IMBEditor.LeaveEditMode();
      MBAgentRendererSceneController.DestructAgentRendererSceneController(MBEditor._editorScene, MBEditor._agentRendererSceneController);
      MBEditor._agentRendererSceneController = (MBAgentRendererSceneController) null;
      MBEditor._editorScene = (Scene) null;
    }

    public static void EnterEditMissionMode(Mission mission)
    {
      MBAPI.IMBEditor.EnterEditMissionMode(mission.Pointer);
      MBEditor._isEditorMissionOn = true;
    }

    public static void LeaveEditMissionMode()
    {
      MBAPI.IMBEditor.LeaveEditMissionMode();
      MBEditor._isEditorMissionOn = false;
    }

    public static bool IsEditorMissionOn() => MBEditor._isEditorMissionOn && MBEditor.IsEditModeOn;

    public static void ActivateSceneEditorPresentation()
    {
      Monster.GetBoneIndexWithId = new Func<string, string, sbyte>(MBActionSet.GetBoneIndexWithId);
      MBObjectManager.Init();
      MBObjectManager.Instance.RegisterType<Monster>("Monster", "Monsters", 2U);
      MBObjectManager.Instance.LoadXML("Monsters", skipXmlFilterForEditor: true);
      MBAPI.IMBEditor.ActivateSceneEditorPresentation();
    }

    public static void DeactivateSceneEditorPresentation()
    {
      MBAPI.IMBEditor.DeactivateSceneEditorPresentation();
      MBObjectManager.Instance.Destroy();
    }

    public static void TickSceneEditorPresentation(float dt)
    {
      MBAPI.IMBEditor.TickSceneEditorPresentation(dt);
      LoadingWindow.DisableGlobalLoadingWindow();
    }

    public static SceneView GetEditorSceneView() => MBAPI.IMBEditor.GetEditorSceneView();

    public static bool HelpersEnabled() => MBAPI.IMBEditor.HelpersEnabled();

    public static bool BorderHelpersEnabled() => MBAPI.IMBEditor.BorderHelpersEnabled();

    public static void ZoomToPosition(Vec3 pos) => MBAPI.IMBEditor.ZoomToPosition(pos);

    public static bool IsReplayManagerReplaying() => MBAPI.IMBEditor.IsReplayManagerReplaying();

    public static bool IsReplayManagerRendering() => MBAPI.IMBEditor.IsReplayManagerRendering();

    public static bool IsReplayManagerRecording() => MBAPI.IMBEditor.IsReplayManagerRecording();

    public static void AddEntityWarning(GameEntity entityId, string msg) => MBAPI.IMBEditor.AddEntityWarning(entityId.Pointer, msg);

    public static string GetAllPrefabsAndChildWithTag(string tag) => MBAPI.IMBEditor.GetAllPrefabsAndChildWithTag(tag);

    public static void ExitEditMode() => MBAPI.IMBEditor.ExitEditMode();

    public static void SetUpgradeLevelVisibility(List<string> levels)
    {
      string str = "";
      for (int index = 0; index < levels.Count - 1; ++index)
        str = str + levels[index] + "|";
      string cumulated_string = str + levels[levels.Count - 1];
      MBAPI.IMBEditor.SetUpgradeLevelVisibility(cumulated_string);
    }
  }
}
