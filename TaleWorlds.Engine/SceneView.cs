// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.SceneView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglScene_view")]
  public class SceneView : View
  {
    internal SceneView(UIntPtr meshPointer)
      : base(meshPointer)
    {
    }

    public static SceneView CreateSceneView() => EngineApplicationInterface.ISceneView.CreateSceneView();

    public void SetScene(Scene scene) => EngineApplicationInterface.ISceneView.SetScene(this.Pointer, scene.Pointer);

    public void SetAcceptGlobalDebugRenderObjects(bool value) => EngineApplicationInterface.ISceneView.SetAcceptGlobalDebugRenderObjects(this.Pointer, value);

    public void SetRenderWithPostfx(bool value) => EngineApplicationInterface.ISceneView.SetRenderWithPostfx(this.Pointer, value);

    public void SetPostfxConfigParams(int value) => EngineApplicationInterface.ISceneView.SetPostfxConfigParams(this.Pointer, value);

    public void SetForceShaderCompilation(bool value) => EngineApplicationInterface.ISceneView.SetForceShaderCompilation(this.Pointer, value);

    public bool CheckSceneReadyToRender() => EngineApplicationInterface.ISceneView.CheckSceneReadyToRender(this.Pointer);

    public void SetCamera(Camera camera) => EngineApplicationInterface.ISceneView.SetCamera(this.Pointer, camera.Pointer);

    public void SetResolutionScaling(bool value) => EngineApplicationInterface.ISceneView.SetResolutionScaling(this.Pointer, value);

    public void SetPostfxFromConfig() => EngineApplicationInterface.ISceneView.SetPostfxFromConfig(this.Pointer);

    public Vec2 WorldPointToScreenPoint(Vec3 position) => EngineApplicationInterface.ISceneView.WorldPointToScreenPoint(this.Pointer, position);

    public Vec2 ScreenPointToViewportPoint(Vec2 position) => EngineApplicationInterface.ISceneView.ScreenPointToViewportPoint(this.Pointer, position.x, position.y);

    public bool ProjectedMousePositionOnGround(
      ref Vec3 groundPosition,
      bool mouseVisible,
      bool checkOccludedSurface = false)
    {
      return EngineApplicationInterface.ISceneView.ProjectedMousePositionOnGround(this.Pointer, ref groundPosition, mouseVisible, checkOccludedSurface);
    }

    public void TranslateMouse(ref Vec3 worldMouseNear, ref Vec3 worldMouseFar, float maxDistance = -1f) => EngineApplicationInterface.ISceneView.TranslateMouse(this.Pointer, ref worldMouseNear, ref worldMouseFar, maxDistance);

    public void SetSceneUsesSkybox(bool value) => EngineApplicationInterface.ISceneView.SetSceneUsesSkybox(this.Pointer, value);

    public void SetSceneUsesShadows(bool value) => EngineApplicationInterface.ISceneView.SetSceneUsesShadows(this.Pointer, value);

    public void SetSceneUsesContour(bool value) => EngineApplicationInterface.ISceneView.SetSceneUsesContour(this.Pointer, value);

    public void DoNotClear(bool value) => EngineApplicationInterface.ISceneView.DoNotClear(this.Pointer, value);

    public bool ReadyToRender() => EngineApplicationInterface.ISceneView.ReadyToRender(this.Pointer);

    public void SetClearAndDisableAfterSucessfullRender(bool value) => EngineApplicationInterface.ISceneView.SetClearAndDisableAfterSucessfullRender(this.Pointer, value);

    public void SetClearGbuffer(bool value) => EngineApplicationInterface.ISceneView.SetClearGbuffer(this.Pointer, value);

    public void SetShadowmapResolutionMultiplier(float value) => EngineApplicationInterface.ISceneView.SetShadowmapResolutionMultiplier(this.Pointer, value);

    public void SetCleanScreenUntilLoadingDone(bool value) => EngineApplicationInterface.ISceneView.SetCleanScreenUntilLoadingDone(this.Pointer, value);

    public void ClearAll(bool clearScene, bool removeTerrain) => EngineApplicationInterface.ISceneView.ClearAll(this.Pointer, clearScene, removeTerrain);

    public void SetFocusedShadowmap(bool enable, ref Vec3 center, float radius) => EngineApplicationInterface.ISceneView.SetFocusedShadowmap(this.Pointer, enable, ref center, radius);

    public Scene GetScene() => EngineApplicationInterface.ISceneView.GetScene(this.Pointer);
  }
}
