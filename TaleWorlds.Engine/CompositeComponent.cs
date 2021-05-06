// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.CompositeComponent
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglComposite_component")]
  public sealed class CompositeComponent : GameEntityComponent
  {
    internal CompositeComponent(UIntPtr pointer)
      : base(pointer)
    {
    }

    public bool IsValid => this.Pointer != UIntPtr.Zero;

    public static bool IsNull(CompositeComponent component) => (NativeObject) component == (NativeObject) null || component.Pointer == UIntPtr.Zero;

    public static CompositeComponent CreateCompositeComponent() => EngineApplicationInterface.ICompositeComponent.CreateCompositeComponent();

    public CompositeComponent CreateCopy() => EngineApplicationInterface.ICompositeComponent.CreateCopy(this.Pointer);

    public void AddComponent(GameEntityComponent component) => EngineApplicationInterface.ICompositeComponent.AddComponent(this.Pointer, component.Pointer);

    public void AddPrefabEntity(string prefabName, Scene scene) => EngineApplicationInterface.ICompositeComponent.AddPrefabEntity(this.Pointer, scene.Pointer, prefabName);

    public void Dispose()
    {
      if (!this.IsValid)
        return;
      this.Release();
      GC.SuppressFinalize((object) this);
    }

    private void Release() => EngineApplicationInterface.ICompositeComponent.Release(this.Pointer);

    ~CompositeComponent() => this.Dispose();

    public uint GetFactor1() => EngineApplicationInterface.ICompositeComponent.GetFactor1(this.Pointer);

    public uint GetFactor2() => EngineApplicationInterface.ICompositeComponent.GetFactor2(this.Pointer);

    public void SetFactor1(uint factorColor1) => EngineApplicationInterface.ICompositeComponent.SetFactor1(this.Pointer, factorColor1);

    public void SetFactor2(uint factorColor2) => EngineApplicationInterface.ICompositeComponent.SetFactor2(this.Pointer, factorColor2);

    public void SetVectorArgument(
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3)
    {
      EngineApplicationInterface.ICompositeComponent.SetVectorArgument(this.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
    }

    public void SetMaterial(Material material) => EngineApplicationInterface.ICompositeComponent.SetMaterial(this.Pointer, material.Pointer);

    public MatrixFrame Frame
    {
      get
      {
        MatrixFrame outFrame = new MatrixFrame();
        EngineApplicationInterface.ICompositeComponent.GetFrame(this.Pointer, ref outFrame);
        return outFrame;
      }
      set => EngineApplicationInterface.ICompositeComponent.SetFrame(this.Pointer, ref value);
    }

    public Vec3 VectorUserData
    {
      get => EngineApplicationInterface.ICompositeComponent.GetVectorUserData(this.Pointer);
      set => EngineApplicationInterface.ICompositeComponent.SetVectorUserData(this.Pointer, ref value);
    }

    public void SetVisibilityMask(VisibilityMaskFlags visibilityMask) => EngineApplicationInterface.ICompositeComponent.SetVisibilityMask(this.Pointer, visibilityMask);

    public override MetaMesh GetFirstMetaMesh() => EngineApplicationInterface.ICompositeComponent.GetFirstMetaMesh(this.Pointer);

    public void AddMultiMesh(string MultiMeshName) => EngineApplicationInterface.ICompositeComponent.AddMultiMesh(this.Pointer, MultiMeshName);

    public void SetVisible(bool visible) => EngineApplicationInterface.ICompositeComponent.SetVisible(this.Pointer, visible);

    public bool GetVisible() => EngineApplicationInterface.ICompositeComponent.IsVisible(this.Pointer);
  }
}
