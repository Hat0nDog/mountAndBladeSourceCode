// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBInitialScreenBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class MBInitialScreenBase : ScreenBase, IGameStateListener
  {
    private Camera _camera;
    private SceneLayer _sceneLayer;
    private int _frameCountSinceReadyToRender;
    private const int _numOfFramesToWaitAfterReadyToRender = 8;
    private GameEntity _cameraAnimationEntity;
    private Scene _scene;
    private bool _buttonInvokeMessage;
    private string _buttonToInvoke;
    private InitialState _state;

    public MBInitialScreenBase(InitialState state) => this._state = state;

    void IGameStateListener.OnActivate()
    {
    }

    void IGameStateListener.OnDeactivate()
    {
    }

    void IGameStateListener.OnInitialize() => this._state.OnInitialMenuOptionInvoked += new OnInitialMenuOptionInvokedDelegate(this.OnExecutedInitialStateOption);

    void IGameStateListener.OnFinalize() => this._state.OnInitialMenuOptionInvoked -= new OnInitialMenuOptionInvokedDelegate(this.OnExecutedInitialStateOption);

    private void OnExecutedInitialStateOption(InitialStateOption target)
    {
    }

    protected override void OnInitialize()
    {
      base.OnInitialize();
      this._sceneLayer = new SceneLayer();
      this.AddLayer((ScreenLayer) this._sceneLayer);
      this._sceneLayer.SceneView.SetResolutionScaling(true);
      this._camera = Camera.CreateCamera();
      Common.MemoryCleanup();
      if (Game.Current != null)
        Game.Current.Destroy();
      MBMusicManager.Initialize();
    }

    protected override void OnFinalize()
    {
      this._camera = (Camera) null;
      this._sceneLayer = (SceneLayer) null;
      this._cameraAnimationEntity = (GameEntity) null;
      this._scene = (Scene) null;
      base.OnFinalize();
    }

    protected override void OnFrameTick(float dt)
    {
      base.OnFrameTick(dt);
      if (this._buttonInvokeMessage)
      {
        this._buttonInvokeMessage = false;
        Module.CurrentModule.ExecuteInitialStateOptionWithId(this._buttonToInvoke);
      }
      if (NativeConfig.DeterministicMode)
        Module.CurrentModule.ExecuteInitialStateOptionWithId("CustomBattle");
      if (this._sceneLayer == null)
        Console.WriteLine("InitialScreen::OnFrameTick scene view null");
      if ((NativeObject) this._scene == (NativeObject) null)
        return;
      if (this._sceneLayer != null && this._sceneLayer.SceneView.ReadyToRender())
      {
        if (this._frameCountSinceReadyToRender > 8)
        {
          Utilities.DisableGlobalLoadingWindow();
          LoadingWindow.DisableGlobalLoadingWindow();
        }
        else
          ++this._frameCountSinceReadyToRender;
      }
      if (this._sceneLayer != null)
        this._sceneLayer.SetCamera(this._camera);
      SoundManager.SetListenerFrame(this._camera.Frame);
      this._scene.Tick(dt);
      if (!Input.IsKeyDown(InputKey.LeftControl) || !Input.IsKeyReleased(InputKey.E))
        return;
      MBInitialScreenBase.OnEditModeEnterPress();
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      if (Utilities.renderingActive)
        this.RefreshScene();
      this._frameCountSinceReadyToRender = 0;
      if (!NativeConfig.DoLocalizationCheckAtStartup)
        return;
      LocalizedTextManager.CheckValidity(new List<string>());
    }

    private void RefreshScene()
    {
      if ((NativeObject) this._scene == (NativeObject) null)
      {
        this._scene = Scene.CreateNewScene();
        this._scene.SetName(nameof (MBInitialScreenBase));
        this._scene.SetPlaySoundEventsAfterReadyToRender(true);
        this._scene.Read("main_menu_a");
        for (int index = 0; index < 40; ++index)
          this._scene.Tick(0.1f);
        Vec3 dof_params = new Vec3();
        this._scene.FindEntityWithTag("camera_instance").GetCameraParamsFromCameraScript(this._camera, ref dof_params);
      }
      SoundManager.SetListenerFrame(this._camera.Frame);
      if (this._sceneLayer != null)
      {
        this._sceneLayer.SetScene(this._scene);
        this._sceneLayer.SceneView.SetEnable(true);
        this._sceneLayer.SceneView.SetSceneUsesShadows(true);
      }
      this._cameraAnimationEntity = GameEntity.CreateEmpty(this._scene);
    }

    private void OnSceneEditorWindowOpen() => GameStateManager.Current.CleanAndPushState((GameState) GameStateManager.Current.CreateState<EditorState>());

    protected override void OnDeactivate()
    {
      this._sceneLayer.SceneView.SetEnable(false);
      this._sceneLayer.SceneView.ClearAll(true, true);
      this._scene = (Scene) null;
      base.OnDeactivate();
    }

    protected override void OnPause()
    {
      LoadingWindow.DisableGlobalLoadingWindow();
      base.OnPause();
      if (!((NativeObject) this._scene != (NativeObject) null))
        return;
      this._scene.FinishSceneSounds();
    }

    protected override void OnResume()
    {
      base.OnResume();
      if (!((NativeObject) this._scene != (NativeObject) null))
        return;
      int sinceReadyToRender = this._frameCountSinceReadyToRender;
    }

    public static void DoExitButtonAction() => MBAPI.IMBScreen.OnExitButtonClick();

    public bool StartedRendering() => this._sceneLayer.SceneView.ReadyToRender();

    public static void OnEditModeEnterPress() => MBAPI.IMBScreen.OnEditModeEnterPress();

    public static void OnEditModeEnterRelease() => MBAPI.IMBScreen.OnEditModeEnterRelease();
  }
}
