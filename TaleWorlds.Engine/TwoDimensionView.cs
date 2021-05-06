// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.TwoDimensionView
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineClass("rglTwo_dimension_view")]
  public sealed class TwoDimensionView : View
  {
    internal TwoDimensionView(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static TwoDimensionView CreateTwoDimension() => EngineApplicationInterface.ITwoDimensionView.CreateTwoDimensionView();

    public void BeginFrame() => EngineApplicationInterface.ITwoDimensionView.BeginFrame(this.Pointer);

    public void EndFrame() => EngineApplicationInterface.ITwoDimensionView.EndFrame(this.Pointer);

    public void CreateMeshFromDescription(
      float[] vertices,
      float[] uvs,
      uint[] indices,
      int indexCount,
      Material material,
      TwoDimensionMeshDrawData meshDrawData)
    {
      EngineApplicationInterface.ITwoDimensionView.AddNewMesh(this.Pointer, vertices, uvs, indices, vertices.Length / 2, indexCount, material.Pointer, ref meshDrawData);
    }

    public void CreateMeshFromDescription(Material material, TwoDimensionMeshDrawData meshDrawData) => EngineApplicationInterface.ITwoDimensionView.AddNewQuadMesh(this.Pointer, material.Pointer, ref meshDrawData);

    public bool CreateTextMeshFromCache(
      Material material,
      TwoDimensionTextMeshDrawData meshDrawData)
    {
      return EngineApplicationInterface.ITwoDimensionView.AddCachedTextMesh(this.Pointer, material.Pointer, ref meshDrawData);
    }

    public void CreateTextMeshFromDescription(
      float[] vertices,
      float[] uvs,
      uint[] indices,
      int indexCount,
      Material material,
      TwoDimensionTextMeshDrawData meshDrawData)
    {
      EngineApplicationInterface.ITwoDimensionView.AddNewTextMesh(this.Pointer, vertices, uvs, indices, vertices.Length / 2, indexCount, material.Pointer, ref meshDrawData);
    }
  }
}
