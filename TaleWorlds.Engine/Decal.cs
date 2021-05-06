// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Decal
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglDecal")]
  public sealed class Decal : GameEntityComponent
  {
    internal Decal(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static Decal CreateDecal(string name = null) => EngineApplicationInterface.IDecal.CreateDecal(name);

    public Decal CreateCopy() => EngineApplicationInterface.IDecal.CreateCopy(this.Pointer);

    public bool IsValid => this.Pointer != UIntPtr.Zero;

    public uint GetFactor1() => EngineApplicationInterface.IDecal.GetFactor1(this.Pointer);

    public void SetFactor1Linear(uint linearFactorColor1) => EngineApplicationInterface.IDecal.SetFactor1Linear(this.Pointer, linearFactorColor1);

    public void SetFactor1(uint factorColor1) => EngineApplicationInterface.IDecal.SetFactor1(this.Pointer, factorColor1);

    public void SetVectorArgument(
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3)
    {
      EngineApplicationInterface.IDecal.SetVectorArgument(this.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
    }

    public void SetVectorArgument2(
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3)
    {
      EngineApplicationInterface.IDecal.SetVectorArgument2(this.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
    }

    public Material GetMaterial() => EngineApplicationInterface.IDecal.GetMaterial(this.Pointer);

    public void SetMaterial(Material material) => EngineApplicationInterface.IDecal.SetMaterial(this.Pointer, material.Pointer);

    public void SetFrame(MatrixFrame Frame) => this.Frame = Frame;

    public MatrixFrame Frame
    {
      get
      {
        MatrixFrame outFrame = new MatrixFrame();
        EngineApplicationInterface.IDecal.GetFrame(this.Pointer, ref outFrame);
        return outFrame;
      }
      set => EngineApplicationInterface.IDecal.SetFrame(this.Pointer, ref value);
    }
  }
}
