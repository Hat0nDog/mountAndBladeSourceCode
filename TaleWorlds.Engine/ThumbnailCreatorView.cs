// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ThumbnailCreatorView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineClass("rglThumbnail_creator_view")]
  public sealed class ThumbnailCreatorView : View
  {
    public static ThumbnailCreatorView.OnThumbnailRenderCompleteDelegate renderCallback;

    internal ThumbnailCreatorView(UIntPtr pointer)
      : base(pointer)
    {
    }

    [EngineCallback]
    internal static void OnThumbnailRenderComplete(string renderId, Texture renderTarget) => ThumbnailCreatorView.renderCallback(renderId, renderTarget);

    public static ThumbnailCreatorView CreateThumbnailCreatorView() => EngineApplicationInterface.IThumbnailCreatorView.CreateThumbnailCreatorView();

    public void RegisterScene(Scene scene, bool usePostFx = true) => EngineApplicationInterface.IThumbnailCreatorView.RegisterScene(this.Pointer, scene.Pointer, usePostFx);

    public void RegisterEntity(
      Scene scene,
      Camera cam,
      Texture texture,
      GameEntity itemEntity,
      int allocationGroupIndex,
      string renderId = "")
    {
      EngineApplicationInterface.IThumbnailCreatorView.RegisterEntity(this.Pointer, scene.Pointer, cam.Pointer, texture.Pointer, itemEntity.Pointer, renderId, allocationGroupIndex);
    }

    public void RegisterEntityWithoutTexture(
      Scene scene,
      Camera camera,
      GameEntity entity,
      int width,
      int height,
      int allocationGroupIndex,
      string renderId = "",
      string debugName = "")
    {
      EngineApplicationInterface.IThumbnailCreatorView.RegisterEntityWithoutTexture(this.Pointer, scene.Pointer, camera.Pointer, entity.Pointer, width, height, renderId, debugName, allocationGroupIndex);
    }

    public delegate void OnThumbnailRenderCompleteDelegate(string renderId, Texture renderTarget);
  }
}
