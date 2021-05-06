// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Mesh
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglMesh")]
  public sealed class Mesh : Resource
  {
    internal Mesh(UIntPtr meshPointer)
      : base(meshPointer)
    {
    }

    public static Mesh CreateMeshWithMaterial(Material material) => EngineApplicationInterface.IMesh.CreateMeshWithMaterial(material.Pointer);

    public static Mesh CreateMesh(bool editable = true) => EngineApplicationInterface.IMesh.CreateMesh(editable);

    public Mesh GetBaseMesh() => EngineApplicationInterface.IMesh.GetBaseMesh(this.Pointer);

    public static Mesh GetFromResource(string meshName) => EngineApplicationInterface.IMesh.GetMeshFromResource(meshName);

    public static Mesh GetRandomMeshWithVdecl(int inputLayout) => EngineApplicationInterface.IMesh.GetRandomMeshWithVdecl(inputLayout);

    public void SetColorAndStroke(uint color, uint strokeColor, bool drawStroke)
    {
      this.Color = color;
      this.Color2 = strokeColor;
      EngineApplicationInterface.IMesh.SetColorAndStroke(this.Pointer, drawStroke);
    }

    public void SetMeshRenderOrder(int renderOrder) => EngineApplicationInterface.IMesh.SetMeshRenderOrder(this.Pointer, renderOrder);

    public bool HasTag(string str) => EngineApplicationInterface.IMesh.HasTag(this.Pointer, str);

    public Mesh CreateCopy() => EngineApplicationInterface.IMesh.CreateMeshCopy(this.Pointer);

    public void SetMaterial(string newMaterialName) => EngineApplicationInterface.IMesh.SetMaterialByName(this.Pointer, newMaterialName);

    public void SetVectorArgument(
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3)
    {
      EngineApplicationInterface.IMesh.SetVectorArgument(this.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
    }

    public void SetVectorArgument2(
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3)
    {
      EngineApplicationInterface.IMesh.SetVectorArgument2(this.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
    }

    public void SetMaterial(Material material) => EngineApplicationInterface.IMesh.SetMaterial(this.Pointer, material.Pointer);

    public Material GetMaterial() => EngineApplicationInterface.IMesh.GetMaterial(this.Pointer);

    public int AddFaceCorner(
      Vec3 position,
      Vec3 normal,
      Vec2 uvCoord,
      uint color,
      UIntPtr lockHandle)
    {
      return this.IsValid ? EngineApplicationInterface.IMesh.AddFaceCorner(this.Pointer, position, normal, uvCoord, color, lockHandle) : -1;
    }

    public int AddFace(int patchNode0, int patchNode1, int patchNode2, UIntPtr lockHandle) => this.IsValid ? EngineApplicationInterface.IMesh.AddFace(this.Pointer, patchNode0, patchNode1, patchNode2, lockHandle) : -1;

    public void ClearMesh()
    {
      if (!this.IsValid)
        return;
      EngineApplicationInterface.IMesh.ClearMesh(this.Pointer);
    }

    public string Name
    {
      get => this.IsValid ? EngineApplicationInterface.IMesh.GetName(this.Pointer) : string.Empty;
      set => EngineApplicationInterface.IMesh.SetName(this.Pointer, value);
    }

    public MBMeshCullingMode CullingMode
    {
      set
      {
        if (!this.IsValid)
          return;
        EngineApplicationInterface.IMesh.SetCullingMode(this.Pointer, (uint) value);
      }
    }

    public float MorphTime
    {
      set
      {
        if (!this.IsValid)
          return;
        EngineApplicationInterface.IMesh.SetMorphTime(this.Pointer, value);
      }
    }

    public uint Color
    {
      set
      {
        if (!this.IsValid)
          return;
        EngineApplicationInterface.IMesh.SetColor(this.Pointer, value);
      }
      get => EngineApplicationInterface.IMesh.GetColor(this.Pointer);
    }

    public uint Color2
    {
      set
      {
        if (!this.IsValid)
          return;
        EngineApplicationInterface.IMesh.SetColor2(this.Pointer, value);
      }
      get => EngineApplicationInterface.IMesh.GetColor2(this.Pointer);
    }

    public void SetColorAlpha(uint newAlpha)
    {
      if (!this.IsValid)
        return;
      EngineApplicationInterface.IMesh.SetColorAlpha(this.Pointer, newAlpha);
    }

    public uint GetFaceCount() => !this.IsValid ? 0U : EngineApplicationInterface.IMesh.GetFaceCount(this.Pointer);

    public uint GetFaceCornerCount() => !this.IsValid ? 0U : EngineApplicationInterface.IMesh.GetFaceCornerCount(this.Pointer);

    public void ComputeNormals()
    {
      if (!this.IsValid)
        return;
      EngineApplicationInterface.IMesh.ComputeNormals(this.Pointer);
    }

    public void ComputeTangents()
    {
      if (!this.IsValid)
        return;
      EngineApplicationInterface.IMesh.ComputeTangents(this.Pointer);
    }

    public void AddMesh(string meshResourceName, MatrixFrame meshFrame)
    {
      if (!this.IsValid)
        return;
      Mesh fromResource = Mesh.GetFromResource(meshResourceName);
      EngineApplicationInterface.IMesh.AddMeshToMesh(this.Pointer, fromResource.Pointer, ref meshFrame);
    }

    public void AddMesh(Mesh mesh, MatrixFrame meshFrame)
    {
      if (!this.IsValid)
        return;
      EngineApplicationInterface.IMesh.AddMeshToMesh(this.Pointer, mesh.Pointer, ref meshFrame);
    }

    public MatrixFrame GetLocalFrame()
    {
      if (!this.IsValid)
        return new MatrixFrame();
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.IMesh.GetLocalFrame(this.Pointer, ref outFrame);
      return outFrame;
    }

    public void SetLocalFrame(MatrixFrame meshFrame)
    {
      if (!this.IsValid)
        return;
      EngineApplicationInterface.IMesh.SetLocalFrame(this.Pointer, ref meshFrame);
    }

    public void SetVisibilityMask(VisibilityMaskFlags visibilityMask) => EngineApplicationInterface.IMesh.SetVisibilityMask(this.Pointer, visibilityMask);

    public void UpdateBoundingBox()
    {
      if (!this.IsValid)
        return;
      EngineApplicationInterface.IMesh.UpdateBoundingBox(this.Pointer);
    }

    public void SetAsNotEffectedBySeason() => EngineApplicationInterface.IMesh.SetAsNotEffectedBySeason(this.Pointer);

    public float GetBoundingBoxWidth() => !this.IsValid ? 0.0f : EngineApplicationInterface.IMesh.GetBoundingBoxWidth(this.Pointer);

    public float GetBoundingBoxHeight() => !this.IsValid ? 0.0f : EngineApplicationInterface.IMesh.GetBoundingBoxHeight(this.Pointer);

    public Vec3 GetBoundingBoxMin() => EngineApplicationInterface.IMesh.GetBoundingBoxMin(this.Pointer);

    public Vec3 GetBoundingBoxMax() => EngineApplicationInterface.IMesh.GetBoundingBoxMax(this.Pointer);

    public void AddTriangle(
      Vec3 p1,
      Vec3 p2,
      Vec3 p3,
      Vec2 uv1,
      Vec2 uv2,
      Vec2 uv3,
      uint color,
      UIntPtr lockHandle)
    {
      EngineApplicationInterface.IMesh.AddTriangle(this.Pointer, p1, p2, p3, uv1, uv2, uv3, color, lockHandle);
    }

    public void AddTriangleWithVertexColors(
      Vec3 p1,
      Vec3 p2,
      Vec3 p3,
      Vec2 uv1,
      Vec2 uv2,
      Vec2 uv3,
      uint c1,
      uint c2,
      uint c3,
      UIntPtr lockHandle)
    {
      EngineApplicationInterface.IMesh.AddTriangleWithVertexColors(this.Pointer, p1, p2, p3, uv1, uv2, uv3, c1, c2, c3, lockHandle);
    }

    public void HintIndicesDynamic() => EngineApplicationInterface.IMesh.HintIndicesDynamic(this.Pointer);

    public void HintVerticesDynamic() => EngineApplicationInterface.IMesh.HintVerticesDynamic(this.Pointer);

    public void RecomputeBoundingBox() => EngineApplicationInterface.IMesh.RecomputeBoundingBox(this.Pointer);

    public BillboardType Billboard
    {
      get => EngineApplicationInterface.IMesh.GetBillboard(this.Pointer);
      set => EngineApplicationInterface.IMesh.SetBillboard(this.Pointer, value);
    }

    public VisibilityMaskFlags VisibilityMask
    {
      get => EngineApplicationInterface.IMesh.GetVisibilityMask(this.Pointer);
      set => EngineApplicationInterface.IMesh.SetVisibilityMask(this.Pointer, value);
    }

    public int EditDataFaceCornerCount => EngineApplicationInterface.IMesh.GetEditDataFaceCornerCount(this.Pointer);

    public void SetEditDataFaceCornerVertexColor(int index, uint color) => EngineApplicationInterface.IMesh.SetEditDataFaceCornerVertexColor(this.Pointer, index, color);

    public uint GetEditDataFaceCornerVertexColor(int index) => EngineApplicationInterface.IMesh.GetEditDataFaceCornerVertexColor(this.Pointer, index);

    public void PreloadForRendering() => EngineApplicationInterface.IMesh.PreloadForRendering(this.Pointer);

    public void SetContourColor(Vec3 color, bool alwaysVisible, bool maskMesh) => EngineApplicationInterface.IMesh.SetContourColor(this.Pointer, color, alwaysVisible, maskMesh);

    public void DisableContour() => EngineApplicationInterface.IMesh.DisableContour(this.Pointer);

    public void SetExternalBoundingBox(BoundingBox bbox) => EngineApplicationInterface.IMesh.SetExternalBoundingBox(this.Pointer, ref bbox);

    public void AddEditDataUser() => EngineApplicationInterface.IMesh.AddEditDataUser(this.Pointer);

    public void ReleaseEditDataUser() => EngineApplicationInterface.IMesh.ReleaseEditDataUser(this.Pointer);

    public void SetEditDataPolicy(EditDataPolicy policy) => EngineApplicationInterface.IMesh.SetEditDataPolicy(this.Pointer, policy);

    public UIntPtr LockEditDataWrite() => EngineApplicationInterface.IMesh.LockEditDataWrite(this.Pointer);

    public void UnlockEditDataWrite(UIntPtr handle) => EngineApplicationInterface.IMesh.UnlockEditDataWrite(this.Pointer, handle);
  }
}
