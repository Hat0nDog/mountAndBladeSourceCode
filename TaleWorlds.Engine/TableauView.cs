// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.TableauView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineClass("rglTableau_view")]
  public sealed class TableauView : SceneView
  {
    internal TableauView(UIntPtr meshPointer)
      : base(meshPointer)
    {
    }

    public static TableauView CreateTableauView() => EngineApplicationInterface.ITableauView.CreateTableauView();

    public void SetSortingEnabled(bool value) => EngineApplicationInterface.ITableauView.SetSortingEnabled(this.Pointer, value);

    public void SetContinuousRendering(bool value) => EngineApplicationInterface.ITableauView.SetContinousRendering(this.Pointer, value);

    public void SetDoNotRenderThisFrame(bool value) => EngineApplicationInterface.ITableauView.SetDoNotRenderThisFrame(this.Pointer, value);

    public void SetDeleteAfterRendering(bool value) => EngineApplicationInterface.ITableauView.SetDeleteAfterRendering(this.Pointer, value);

    public void AddClearTask(bool clearOnlySceneview = false) => EngineApplicationInterface.ITableauView.AddClearTask(this.Pointer, clearOnlySceneview);

    public static Texture AddTableau(
      RenderTargetComponent.TextureUpdateEventHandler eventHandler,
      object objectRef,
      int tableauSizeX,
      int tableauSizeY)
    {
      Texture tableauTexture = Texture.CreateTableauTexture(nameof (AddTableau), eventHandler, objectRef, tableauSizeX, tableauSizeY);
      TableauView tableauView = tableauTexture.TableauView;
      tableauView.SetRenderOnDemand(false);
      tableauView.SetResolutionScaling(true);
      return tableauTexture;
    }
  }
}
