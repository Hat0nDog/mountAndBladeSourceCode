// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Texture
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;

namespace TaleWorlds.Engine
{
  public sealed class Texture : Resource
  {
    internal Texture(UIntPtr ptr)
      : base(ptr)
    {
    }

    public int Width => EngineApplicationInterface.ITexture.GetWidth(this.Pointer);

    public int Height => EngineApplicationInterface.ITexture.GetHeight(this.Pointer);

    public int MemorySize => EngineApplicationInterface.ITexture.GetMemorySize(this.Pointer);

    public bool IsRenderTarget => EngineApplicationInterface.ITexture.IsRenderTarget(this.Pointer);

    public static Texture CreateTextureFromPath(string path, string fileName) => EngineApplicationInterface.ITexture.CreateTextureFromPath(path, fileName);

    public string Name
    {
      get => EngineApplicationInterface.ITexture.GetName(this.Pointer);
      set => EngineApplicationInterface.ITexture.SetName(this.Pointer, value);
    }

    public void TransformRenderTargetToResource(string name) => EngineApplicationInterface.ITexture.TransformRenderTargetToResourceTexture(this.Pointer, name);

    public static Texture GetFromResource(string resourceName) => EngineApplicationInterface.ITexture.GetFromResource(resourceName);

    public static Texture CheckAndGetFromResource(string resourceName) => EngineApplicationInterface.ITexture.CheckAndGetFromResource(resourceName);

    public static void ScaleTextureWithRatio(ref int tableauSizeX, ref int tableauSizeY)
    {
      float num1 = (float) tableauSizeX;
      float num2 = (float) tableauSizeY;
      float num3 = (float) Math.Pow(2.0, (double) ((int) Math.Log((double) num1, 2.0) + 2)) / num1;
      tableauSizeX = (int) ((double) num1 * (double) num3);
      tableauSizeY = (int) ((double) num2 * (double) num3);
    }

    public void PreloadTexture() => EngineApplicationInterface.ITexture.GetCurObject(this.Pointer);

    public void Release() => EngineApplicationInterface.ITexture.Release(this.Pointer);

    public static Texture LoadTextureFromPath(string fileName, string folder) => EngineApplicationInterface.ITexture.LoadTextureFromPath(fileName, folder);

    public static Texture CreateDepthTarget(string name, int width, int height) => EngineApplicationInterface.ITexture.CreateDepthTarget(name, width, height);

    public static Texture CreateFromByteArray(byte[] data, int width, int height) => EngineApplicationInterface.ITexture.CreateFromByteArray(data, width, height);

    public void SaveToFile(string path) => EngineApplicationInterface.ITexture.SaveToFile(this.Pointer, path);

    public static Texture CreateFromMemory(byte[] data) => EngineApplicationInterface.ITexture.CreateFromMemory(data);

    public static void ReleaseGpuMemories() => EngineApplicationInterface.ITexture.ReleaseGpuMemories();

    public RenderTargetComponent RenderTargetComponent => EngineApplicationInterface.ITexture.GetRenderTargetComponent(this.Pointer);

    public TableauView TableauView => EngineApplicationInterface.ITexture.GetTableauView(this.Pointer);

    public object UserData => this.RenderTargetComponent.UserData;

    private void SetTableauView(TableauView tableauView) => EngineApplicationInterface.ITexture.SetTableauView(this.Pointer, tableauView.Pointer);

    public static Texture CreateTableauTexture(
      string name,
      RenderTargetComponent.TextureUpdateEventHandler eventHandler,
      object objectRef,
      int tableauSizeX,
      int tableauSizeY)
    {
      Texture renderTarget = Texture.CreateRenderTarget(name, tableauSizeX, tableauSizeY, true, true, true);
      RenderTargetComponent renderTargetComponent = renderTarget.RenderTargetComponent;
      renderTargetComponent.PaintNeeded += eventHandler;
      renderTargetComponent.UserData = objectRef;
      TableauView tableauView = TableauView.CreateTableauView();
      tableauView.SetRenderTarget(renderTarget);
      tableauView.SetAutoDepthTargetCreation(true);
      tableauView.SetSceneUsesSkybox(false);
      tableauView.SetClearColor(4294902015U);
      renderTarget.SetTableauView(tableauView);
      return renderTarget;
    }

    public static Texture CreateRenderTarget(
      string name,
      int width,
      int height,
      bool autoMipmaps,
      bool isTableau,
      bool createUninitialized = false,
      bool always_valid = false)
    {
      return EngineApplicationInterface.ITexture.CreateRenderTarget(name, width, height, autoMipmaps, isTableau, createUninitialized, always_valid);
    }
  }
}
