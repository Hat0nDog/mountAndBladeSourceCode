// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CameraDisplay
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class CameraDisplay : ScriptComponentBehaviour
  {
    private Camera _myCamera;
    private SceneView _sceneView;
    public int renderOrder;

    private void BuildView()
    {
      this._sceneView = SceneView.CreateSceneView();
      this._myCamera = Camera.CreateCamera();
      this._sceneView.SetScene(this.GameEntity.Scene);
      this._sceneView.SetPostfxFromConfig();
      this._sceneView.SetRenderOption(View.ViewRenderOptions.ClearColor, false);
      this._sceneView.SetRenderOption(View.ViewRenderOptions.ClearDepth, true);
      this._sceneView.SetScale(new Vec2(0.2f, 0.2f));
    }

    private void SetCamera()
    {
      Vec2 screenResolution = Screen.RealScreenResolution;
      float aspectRatioXY = screenResolution.x / screenResolution.y;
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      this._myCamera.SetFovVertical(0.7853982f, aspectRatioXY, 0.2f, 200f);
      this._myCamera.Frame = globalFrame;
      this._sceneView.SetCamera(this._myCamera);
    }

    private void RenderCameraFrustrum() => this._myCamera.RenderFrustrum();

    protected internal override void OnEditorInit() => this.BuildView();

    protected internal override void OnInit() => this.BuildView();

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      if (MBEditor.IsEntitySelected(this.GameEntity))
      {
        this.RenderCameraFrustrum();
        this._sceneView.SetEnable(true);
      }
      else
        this._sceneView.SetEnable(false);
    }

    protected override void OnRemoved(int removeReason)
    {
      base.OnRemoved(removeReason);
      this._sceneView = (SceneView) null;
      this._myCamera = (Camera) null;
    }
  }
}
