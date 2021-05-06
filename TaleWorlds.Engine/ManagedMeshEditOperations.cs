// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ManagedMeshEditOperations
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglManaged_mesh_edit_operations")]
  public sealed class ManagedMeshEditOperations : NativeObject
  {
    internal ManagedMeshEditOperations(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static ManagedMeshEditOperations Create(Mesh meshToEdit) => EngineApplicationInterface.IManagedMeshEditOperations.Create(meshToEdit.Pointer);

    public void Weld() => EngineApplicationInterface.IManagedMeshEditOperations.Weld(this.Pointer);

    public int AddVertex(Vec3 vertexPos) => EngineApplicationInterface.IManagedMeshEditOperations.AddVertex(this.Pointer, ref vertexPos);

    public int AddFaceCorner(int vertexIndex, Vec2 uv0, Vec3 color, Vec3 normal) => EngineApplicationInterface.IManagedMeshEditOperations.AddFaceCorner1(this.Pointer, vertexIndex, ref uv0, ref color, ref normal);

    public int AddFaceCorner(int vertexIndex, Vec2 uv0, Vec2 uv1, Vec3 color, Vec3 normal) => EngineApplicationInterface.IManagedMeshEditOperations.AddFaceCorner2(this.Pointer, vertexIndex, ref uv0, ref uv1, ref color, ref normal);

    public int AddFace(int patchNode0, int patchNode1, int patchNode2) => EngineApplicationInterface.IManagedMeshEditOperations.AddFace(this.Pointer, patchNode0, patchNode1, patchNode2);

    public void AddTriangle(
      Vec3 p1,
      Vec3 p2,
      Vec3 p3,
      Vec2 uv1,
      Vec2 uv2,
      Vec2 uv3,
      Vec3 color)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.AddTriangle1(this.Pointer, ref p1, ref p2, ref p3, ref uv1, ref uv2, ref uv3, ref color);
    }

    public void AddTriangle(
      Vec3 p1,
      Vec3 p2,
      Vec3 p3,
      Vec3 n1,
      Vec3 n2,
      Vec3 n3,
      Vec2 uv1,
      Vec2 uv2,
      Vec2 uv3,
      Vec3 c1,
      Vec3 c2,
      Vec3 c3)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.AddTriangle2(this.Pointer, ref p1, ref p2, ref p3, ref n1, ref n2, ref n3, ref uv1, ref uv2, ref uv3, ref c1, ref c2, ref c3);
    }

    public void AddRectangle3(Vec3 o, Vec2 size, Vec2 uv_origin, Vec2 uvSize, Vec3 color) => EngineApplicationInterface.IManagedMeshEditOperations.AddRectangle3(this.Pointer, ref o, ref size, ref uv_origin, ref uvSize, ref color);

    public void AddRectangleWithInverseUV(
      Vec3 o,
      Vec2 size,
      Vec2 uv_origin,
      Vec2 uvSize,
      Vec3 color)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.AddRectangleWithInverseUV(this.Pointer, ref o, ref size, ref uv_origin, ref uvSize, ref color);
    }

    public void AddRect(Vec3 originBegin, Vec3 originEnd, Vec2 uvBegin, Vec2 uvEnd, Vec3 color) => EngineApplicationInterface.IManagedMeshEditOperations.AddRect(this.Pointer, ref originBegin, ref originEnd, ref uvBegin, ref uvEnd, ref color);

    public void AddRectWithZUp(
      Vec3 originBegin,
      Vec3 originEnd,
      Vec2 uvBegin,
      Vec2 uvEnd,
      Vec3 color)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.AddRectWithZUp(this.Pointer, ref originBegin, ref originEnd, ref uvBegin, ref uvEnd, ref color);
    }

    public void InvertFacesWindingOrder() => EngineApplicationInterface.IManagedMeshEditOperations.InvertFacesWindingOrder(this.Pointer);

    public void ScaleVertices(float newScale) => EngineApplicationInterface.IManagedMeshEditOperations.ScaleVertices1(this.Pointer, newScale);

    public void MoveVerticesAlongNormal(float moveAmount) => EngineApplicationInterface.IManagedMeshEditOperations.MoveVerticesAlongNormal(this.Pointer, moveAmount);

    public void ScaleVertices(Vec3 newScale, bool keepUvX = false, float maxUvSize = 1f) => EngineApplicationInterface.IManagedMeshEditOperations.ScaleVertices2(this.Pointer, ref newScale, keepUvX, maxUvSize);

    public void TranslateVertices(Vec3 newOrigin) => EngineApplicationInterface.IManagedMeshEditOperations.TranslateVertices(this.Pointer, ref newOrigin);

    public void AddMeshAux(
      Mesh mesh,
      MatrixFrame frame,
      int boneNo,
      Vec3 color,
      bool transformNormal,
      bool heightGradient,
      bool addSkinData,
      bool useDoublePrecision = true)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.AddMeshAux(this.Pointer, mesh.Pointer, ref frame, boneNo, ref color, transformNormal, heightGradient, addSkinData, useDoublePrecision);
    }

    public int ComputeTangents(bool checkFixedNormals) => EngineApplicationInterface.IManagedMeshEditOperations.ComputeTangents(this.Pointer, checkFixedNormals);

    public void GenerateGrid(Vec2i numEdges, Vec2 edgeScale) => EngineApplicationInterface.IManagedMeshEditOperations.GenerateGrid(this.Pointer, ref numEdges, ref edgeScale);

    public void RescaleMesh2d(Vec2 scaleSizeMin, Vec2 scaleSizeMax) => EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2d(this.Pointer, ref scaleSizeMin, ref scaleSizeMax);

    public void RescaleMesh2dRepeatX(
      Vec2 scaleSizeMin,
      Vec2 scaleSizeMax,
      float frameThickness = 0.0f,
      int frameSide = 0)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatX(this.Pointer, ref scaleSizeMin, ref scaleSizeMax, frameThickness, frameSide);
    }

    public void RescaleMesh2dRepeatY(
      Vec2 scaleSizeMin,
      Vec2 scaleSizeMax,
      float frameThickness = 0.0f,
      int frameSide = 0)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatY(this.Pointer, ref scaleSizeMin, ref scaleSizeMax, frameThickness, frameSide);
    }

    public void RescaleMesh2dRepeatXWithTiling(
      Vec2 scaleSizeMin,
      Vec2 scaleSizeMax,
      float frameThickness = 0.0f,
      int frameSide = 0,
      float xyRatio = 0.0f)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatXWithTiling(this.Pointer, ref scaleSizeMin, ref scaleSizeMax, frameThickness, frameSide, xyRatio);
    }

    public void RescaleMesh2dRepeatYWithTiling(
      Vec2 scaleSizeMin,
      Vec2 scaleSizeMax,
      float frameThickness = 0.0f,
      int frameSide = 0,
      float xyRatio = 0.0f)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatYWithTiling(this.Pointer, ref scaleSizeMin, ref scaleSizeMax, frameThickness, frameSide, xyRatio);
    }

    public void RescaleMesh2dWithoutChangingUV(
      Vec2 scaleSizeMin,
      Vec2 scaleSizeMax,
      float remaining)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.RescaleMesh2dRepeatYWithTiling(this.Pointer, ref scaleSizeMin, ref scaleSizeMax, remaining);
    }

    public void AddLine(Vec3 start, Vec3 end, Vec3 color, float lineWidth = 0.004f) => EngineApplicationInterface.IManagedMeshEditOperations.AddLine(this.Pointer, ref start, ref end, ref color, lineWidth);

    public void ComputeCornerNormals(bool checkFixedNormals = false, bool smoothCornerNormals = true) => EngineApplicationInterface.IManagedMeshEditOperations.ComputeCornerNormals(this.Pointer, checkFixedNormals, smoothCornerNormals);

    public void ComputeCornerNormalsWithSmoothingData() => EngineApplicationInterface.IManagedMeshEditOperations.ComputeCornerNormalsWithSmoothingData(this.Pointer);

    public void AddMesh(Mesh mesh, MatrixFrame frame) => EngineApplicationInterface.IManagedMeshEditOperations.AddMesh(this.Pointer, mesh.Pointer, ref frame);

    public void AddMeshWithSkinData(Mesh mesh, MatrixFrame frame, int boneIndex) => EngineApplicationInterface.IManagedMeshEditOperations.AddMeshWithSkinData(this.Pointer, mesh.Pointer, ref frame, boneIndex);

    public void AddMeshWithColor(
      Mesh mesh,
      MatrixFrame frame,
      Vec3 vertexColor,
      bool useDoublePrecision = true)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.AddMeshWithColor(this.Pointer, mesh.Pointer, ref frame, ref vertexColor, useDoublePrecision);
    }

    public void AddMeshToBone(Mesh mesh, MatrixFrame frame, int boneIndex) => EngineApplicationInterface.IManagedMeshEditOperations.AddMeshToBone(this.Pointer, mesh.Pointer, ref frame, boneIndex);

    public void AddMeshWithFixedNormals(Mesh mesh, MatrixFrame frame) => EngineApplicationInterface.IManagedMeshEditOperations.AddMeshWithFixedNormals(this.Pointer, mesh.Pointer, ref frame);

    public void AddMeshWithFixedNormalsWithHeightGradientColor(Mesh mesh, MatrixFrame frame) => EngineApplicationInterface.IManagedMeshEditOperations.AddMeshWithFixedNormalsWithHeightGradientColor(this.Pointer, mesh.Pointer, ref frame);

    public void AddSkinnedMeshWithColor(
      Mesh mesh,
      MatrixFrame frame,
      Vec3 vertexColor,
      bool useDoublePrecision = true)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.AddSkinnedMeshWithColor(this.Pointer, mesh.Pointer, ref frame, ref vertexColor, useDoublePrecision);
    }

    public void SetCornerVertexColor(int cornerNo, Vec3 vertexColor) => EngineApplicationInterface.IManagedMeshEditOperations.SetCornerVertexColor(this.Pointer, cornerNo, ref vertexColor);

    public void SetCornerUV(int cornerNo, Vec2 newUV, int uvNumber = 0) => EngineApplicationInterface.IManagedMeshEditOperations.SetCornerUV(this.Pointer, cornerNo, ref newUV, uvNumber);

    public void ReserveVertices(int count) => EngineApplicationInterface.IManagedMeshEditOperations.ReserveVertices(this.Pointer, count);

    public void ReserveFaceCorners(int count) => EngineApplicationInterface.IManagedMeshEditOperations.ReserveFaceCorners(this.Pointer, count);

    public void ReserveFaces(int count) => EngineApplicationInterface.IManagedMeshEditOperations.ReserveFaces(this.Pointer, count);

    public int RemoveDuplicatedCorners() => EngineApplicationInterface.IManagedMeshEditOperations.RemoveDuplicatedCorners(this.Pointer);

    public void TransformVerticesToParent(MatrixFrame frame) => EngineApplicationInterface.IManagedMeshEditOperations.TransformVerticesToParent(this.Pointer, ref frame);

    public void TransformVerticesToLocal(MatrixFrame frame) => EngineApplicationInterface.IManagedMeshEditOperations.TransformVerticesToLocal(this.Pointer, ref frame);

    public void SetVertexColor(Vec3 color) => EngineApplicationInterface.IManagedMeshEditOperations.SetVertexColor(this.Pointer, ref color);

    public Vec3 GetVertexColor(int faceCornerIndex) => EngineApplicationInterface.IManagedMeshEditOperations.GetVertexColor(this.Pointer, faceCornerIndex);

    public void SetVertexColorAlpha(float newAlpha) => EngineApplicationInterface.IManagedMeshEditOperations.SetVertexColorAlpha(this.Pointer, newAlpha);

    public float GetVertexColorAlpha() => EngineApplicationInterface.IManagedMeshEditOperations.GetVertexColorAlpha(this.Pointer);

    public void EnsureTransformedVertices() => EngineApplicationInterface.IManagedMeshEditOperations.EnsureTransformedVertices(this.Pointer);

    public void ApplyCPUSkinning(Skeleton skeleton) => EngineApplicationInterface.IManagedMeshEditOperations.ApplyCPUSkinning(this.Pointer, skeleton.Pointer);

    public void UpdateOverlappedVertexNormals(
      Mesh attachedToMesh,
      MatrixFrame attachFrame,
      float mergeRadiusSQ = 0.0025f)
    {
      EngineApplicationInterface.IManagedMeshEditOperations.UpdateOverlappedVertexNormals(this.Pointer, attachedToMesh.Pointer, ref attachFrame, mergeRadiusSQ);
    }

    public void ClearAll() => EngineApplicationInterface.IManagedMeshEditOperations.ClearAll(this.Pointer);

    public void SetTangentsOfFaceCorner(int faceCornerIndex, Vec3 tangent, Vec3 binormal) => EngineApplicationInterface.IManagedMeshEditOperations.SetTangentsOfFaceCorner(this.Pointer, faceCornerIndex, ref tangent, ref binormal);

    public void SetPositionOfVertex(int vertexIndex, Vec3 position) => EngineApplicationInterface.IManagedMeshEditOperations.SetPositionOfVertex(this.Pointer, vertexIndex, ref position);

    public Vec3 GetPositionOfVertex(int vertexIndex) => EngineApplicationInterface.IManagedMeshEditOperations.GetPositionOfVertex(this.Pointer, vertexIndex);

    public void RemoveFace(int faceIndex) => EngineApplicationInterface.IManagedMeshEditOperations.RemoveFace(this.Pointer, faceIndex);

    public void FinalizeEditing() => EngineApplicationInterface.IManagedMeshEditOperations.FinalizeEditing(this.Pointer);
  }
}
