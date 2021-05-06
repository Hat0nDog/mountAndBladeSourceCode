// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Screens.SceneLayer
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine.Screens
{
  public class SceneLayer : ScreenLayer
  {
    private SceneView _sceneView;

    public SceneView SceneView => this._sceneView;

    public SceneLayer(string categoryId = "SceneLayer")
      : base(-100, categoryId)
    {
      this.Name = nameof (SceneLayer);
      this.InputRestrictions.SetInputRestrictions(false);
      this._sceneView = SceneView.CreateSceneView();
      this.IsFocusLayer = true;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      this._sceneView.SetEnable(true);
      ScreenManager.TrySetFocus((ScreenLayer) this);
    }

    protected override void OnDeactivate()
    {
      base.OnDeactivate();
      this._sceneView.SetEnable(false);
    }

    protected override void OnFinalize()
    {
      this._sceneView.ClearAll(true, true);
      base.OnFinalize();
    }

    public void SetScene(Scene scene) => this._sceneView.SetScene(scene);

    public void SetRenderWithPostfx(bool value) => this._sceneView.SetRenderWithPostfx(value);

    public void SetPostfxConfigParams(int value) => this._sceneView.SetPostfxConfigParams(value);

    public void SetCamera(Camera camera) => this._sceneView.SetCamera(camera);

    public void SetPostfxFromConfig() => this._sceneView.SetPostfxFromConfig();

    public Vec2 WorldPointToScreenPoint(Vec3 position) => this._sceneView.WorldPointToScreenPoint(position);

    public Vec2 ScreenPointToViewportPoint(Vec2 position) => this._sceneView.ScreenPointToViewportPoint(position);

    public bool ProjectedMousePositionOnGround(ref Vec3 groundPosition, bool mouseVisible) => this._sceneView.ProjectedMousePositionOnGround(ref groundPosition, mouseVisible);

    public void TranslateMouse(ref Vec3 worldMouseNear, ref Vec3 worldMouseFar, float maxDistance = -1f) => this._sceneView.TranslateMouse(ref worldMouseNear, ref worldMouseFar, maxDistance);

    public void SetSceneUsesSkybox(bool value) => this._sceneView.SetSceneUsesSkybox(value);

    public void SetSceneUsesShadows(bool value) => this._sceneView.SetSceneUsesShadows(value);

    public void SetSceneUsesContour(bool value) => this._sceneView.SetSceneUsesContour(value);

    public void SetShadowmapResolutionMultiplier(float value) => this._sceneView.SetShadowmapResolutionMultiplier(value);

    public void SetFocusedShadowmap(bool enable, ref Vec3 center, float radius) => this._sceneView.SetFocusedShadowmap(enable, ref center, radius);

    public void DoNotClear(bool value) => this._sceneView.DoNotClear(value);

    public bool ReadyToRender() => this._sceneView.ReadyToRender();

    public void SetCleanScreenUntilLoadingDone(bool value) => this._sceneView.SetCleanScreenUntilLoadingDone(value);

    public void ClearAll() => this._sceneView.ClearAll(true, true);

    public void ClearRuntimeGPUMemory(bool remove_terrain) => this._sceneView.ClearAll(false, remove_terrain);

    protected internal override void RefreshGlobalOrder(ref int currentOrder)
    {
      this._sceneView.SetRenderOrder(currentOrder);
      ++currentOrder;
    }

    public override bool HitTest() => true;

    public override bool FocusTest() => true;
  }
}
