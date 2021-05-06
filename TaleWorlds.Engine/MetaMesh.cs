// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.MetaMesh
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglMeta_mesh")]
  public sealed class MetaMesh : GameEntityComponent
  {
    internal MetaMesh(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static MetaMesh CreateMetaMesh(string name = null) => EngineApplicationInterface.IMetaMesh.CreateMetaMesh(name);

    public bool IsValid => this.Pointer != UIntPtr.Zero;

    public int GetLodMaskForMeshAtIndex(int index) => EngineApplicationInterface.IMetaMesh.GetLodMaskForMeshAtIndex(this.Pointer, index);

    public int RemoveMeshesWithTag(string tag) => EngineApplicationInterface.IMetaMesh.RemoveMeshesWithTag(this.Pointer, tag);

    public int RemoveMeshesWithoutTag(string tag) => EngineApplicationInterface.IMetaMesh.RemoveMeshesWithoutTag(this.Pointer, tag);

    public int GetMeshCountWithTag(string tag) => EngineApplicationInterface.IMetaMesh.GetMeshCountWithTag(this.Pointer, tag);

    public bool HasAnyGeneratedLods() => EngineApplicationInterface.IMetaMesh.HasAnyGeneratedLods(this.Pointer);

    public bool HasAnyLods() => EngineApplicationInterface.IMetaMesh.HasAnyLods(this.Pointer);

    public static MetaMesh GetCopy(
      string metaMeshName,
      bool showErrors = true,
      bool mayReturnNull = false)
    {
      return EngineApplicationInterface.IMetaMesh.CreateCopyFromName(metaMeshName, showErrors, mayReturnNull);
    }

    public void CopyTo(MetaMesh res, bool copyMeshes = true) => EngineApplicationInterface.IMetaMesh.CopyTo(this.Pointer, res.Pointer, copyMeshes);

    public void ClearMeshesForOtherLods(int lodToKeep) => EngineApplicationInterface.IMetaMesh.ClearMeshesForOtherLods(this.Pointer, lodToKeep);

    public void ClearMeshesForLod(int lodToClear) => EngineApplicationInterface.IMetaMesh.ClearMeshesForLod(this.Pointer, lodToClear);

    public void ClearMeshesForLowerLods(int lodToClear) => EngineApplicationInterface.IMetaMesh.ClearMeshesForLowerLods(this.Pointer, lodToClear);

    public void ClearMeshes() => EngineApplicationInterface.IMetaMesh.ClearMeshes(this.Pointer);

    public void SetNumLods(int lodToClear) => EngineApplicationInterface.IMetaMesh.SetNumLods(this.Pointer, lodToClear);

    public static void CheckMetaMeshExistence(string metaMeshName, int lod_count_check) => EngineApplicationInterface.IMetaMesh.CheckMetaMeshExistence(metaMeshName, lod_count_check);

    public static MetaMesh GetMorphedCopy(
      string metaMeshName,
      float morphTarget,
      bool showErrors)
    {
      return EngineApplicationInterface.IMetaMesh.GetMorphedCopy(metaMeshName, morphTarget, showErrors);
    }

    public MetaMesh CreateCopy() => EngineApplicationInterface.IMetaMesh.CreateCopy(this.Pointer);

    public void AddMesh(Mesh mesh) => EngineApplicationInterface.IMetaMesh.AddMesh(this.Pointer, mesh.Pointer, 0U);

    public void AddMesh(Mesh mesh, uint lodLevel) => EngineApplicationInterface.IMetaMesh.AddMesh(this.Pointer, mesh.Pointer, lodLevel);

    public void AddMetaMesh(MetaMesh metaMesh) => EngineApplicationInterface.IMetaMesh.AddMetaMesh(this.Pointer, metaMesh.Pointer);

    public void SetCullMode(MBMeshCullingMode cullMode) => EngineApplicationInterface.IMetaMesh.SetCullMode(this.Pointer, cullMode);

    public void AddMaterialShaderFlag(string materialShaderFlag)
    {
      for (int meshIndex = 0; meshIndex < this.MeshCount; ++meshIndex)
      {
        Mesh meshAtIndex = this.GetMeshAtIndex(meshIndex);
        Material copy = meshAtIndex.GetMaterial().CreateCopy();
        copy.AddMaterialShaderFlag(materialShaderFlag, false);
        meshAtIndex.SetMaterial(copy);
      }
    }

    public void MergeMultiMeshes(MetaMesh metaMesh) => EngineApplicationInterface.IMetaMesh.MergeMultiMeshes(this.Pointer, metaMesh.Pointer);

    public void BatchMultiMeshes(MetaMesh metaMesh) => EngineApplicationInterface.IMetaMesh.BatchMultiMeshes(this.Pointer, metaMesh.Pointer);

    public void ClearEditData() => EngineApplicationInterface.IMetaMesh.ClearEditData(this.Pointer);

    public int MeshCount => EngineApplicationInterface.IMetaMesh.GetMeshCount(this.Pointer);

    public Mesh GetMeshAtIndex(int meshIndex) => EngineApplicationInterface.IMetaMesh.GetMeshAtIndex(this.Pointer, meshIndex);

    public Mesh GetFirstMeshWithTag(string tag)
    {
      for (int meshIndex = 0; meshIndex < this.MeshCount; ++meshIndex)
      {
        Mesh meshAtIndex = this.GetMeshAtIndex(meshIndex);
        if (meshAtIndex.HasTag(tag))
          return meshAtIndex;
      }
      return (Mesh) null;
    }

    private void Release() => EngineApplicationInterface.IMetaMesh.Release(this.Pointer);

    public uint GetFactor1() => EngineApplicationInterface.IMetaMesh.GetFactor1(this.Pointer);

    public void SetGlossMultiplier(float value) => EngineApplicationInterface.IMetaMesh.SetGlossMultiplier(this.Pointer, value);

    public uint GetFactor2() => EngineApplicationInterface.IMetaMesh.GetFactor2(this.Pointer);

    public void SetFactor1Linear(uint linearFactorColor1) => EngineApplicationInterface.IMetaMesh.SetFactor1Linear(this.Pointer, linearFactorColor1);

    public void SetFactor2Linear(uint linearFactorColor2) => EngineApplicationInterface.IMetaMesh.SetFactor2Linear(this.Pointer, linearFactorColor2);

    public void SetFactor1(uint factorColor1) => EngineApplicationInterface.IMetaMesh.SetFactor1(this.Pointer, factorColor1);

    public void SetFactor2(uint factorColor2) => EngineApplicationInterface.IMetaMesh.SetFactor2(this.Pointer, factorColor2);

    public void SetVectorArgument(
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3)
    {
      EngineApplicationInterface.IMetaMesh.SetVectorArgument(this.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
    }

    public void SetVectorArgument2(
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3)
    {
      EngineApplicationInterface.IMetaMesh.SetVectorArgument2(this.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
    }

    public Vec3 GetVectorArgument2() => EngineApplicationInterface.IMetaMesh.GetVectorArgument2(this.Pointer);

    public void SetMaterial(Material material) => EngineApplicationInterface.IMetaMesh.SetMaterial(this.Pointer, material.Pointer);

    public void SetLodBias(int lodBias) => EngineApplicationInterface.IMetaMesh.SetLodBias(this.Pointer, lodBias);

    public void SetBillboarding(BillboardType billboard) => EngineApplicationInterface.IMetaMesh.SetBillboarding(this.Pointer, billboard);

    public void UseHeadBoneFaceGenScaling(
      Skeleton skeleton,
      sbyte headLookDirectionBoneIndex,
      MatrixFrame frame)
    {
      EngineApplicationInterface.IMetaMesh.UseHeadBoneFaceGenScaling(this.Pointer, skeleton.Pointer, headLookDirectionBoneIndex, ref frame);
    }

    public void DrawTextWithDefaultFont(
      string text,
      Vec2 textPositionMin,
      Vec2 textPositionMax,
      Vec2 size,
      uint color,
      TextFlags flags)
    {
      EngineApplicationInterface.IMetaMesh.DrawTextWithDefaultFont(this.Pointer, text, textPositionMin, textPositionMax, size, color, flags);
    }

    public MatrixFrame Frame
    {
      get
      {
        MatrixFrame outFrame = new MatrixFrame();
        EngineApplicationInterface.IMetaMesh.GetFrame(this.Pointer, ref outFrame);
        return outFrame;
      }
      set => EngineApplicationInterface.IMetaMesh.SetFrame(this.Pointer, ref value);
    }

    public Vec3 VectorUserData
    {
      get => EngineApplicationInterface.IMetaMesh.GetVectorUserData(this.Pointer);
      set => EngineApplicationInterface.IMetaMesh.SetVectorUserData(this.Pointer, ref value);
    }

    public void PreloadForRendering() => EngineApplicationInterface.IMetaMesh.PreloadForRendering(this.Pointer);

    public void PreloadShaders(bool useTableau, bool useTeamColor) => EngineApplicationInterface.IMetaMesh.PreloadShaders(this.Pointer, useTableau, useTeamColor);

    public void RecomputeBoundingBox(bool recomputeMeshes) => EngineApplicationInterface.IMetaMesh.RecomputeBoundingBox(this.Pointer, recomputeMeshes);

    public void AddEditDataUser() => EngineApplicationInterface.IMetaMesh.AddEditDataUser(this.Pointer);

    public void ReleaseEditDataUser() => EngineApplicationInterface.IMetaMesh.ReleaseEditDataUser(this.Pointer);

    public void SetEditDataPolicy(EditDataPolicy policy) => EngineApplicationInterface.IMetaMesh.SetEditDataPolicy(this.Pointer, policy);

    public MatrixFrame Fit()
    {
      MatrixFrame identity = MatrixFrame.Identity;
      Vec3 v1_1 = new Vec3(1000000f, 1000000f, 1000000f);
      Vec3 v1_2 = new Vec3(-1000000f, -1000000f, -1000000f);
      for (int meshIndex = 0; meshIndex != this.MeshCount; ++meshIndex)
      {
        Vec3 boundingBoxMin = this.GetMeshAtIndex(meshIndex).GetBoundingBoxMin();
        Vec3 boundingBoxMax = this.GetMeshAtIndex(meshIndex).GetBoundingBoxMax();
        v1_1 = Vec3.Vec3Min(v1_1, boundingBoxMin);
        v1_2 = Vec3.Vec3Max(v1_2, boundingBoxMax);
      }
      Vec3 vec3 = (v1_1 + v1_2) * 0.5f;
      float scaleAmount = 0.95f / Math.Max(v1_2.x - v1_1.x, v1_2.y - v1_1.y);
      identity.origin -= vec3 * scaleAmount;
      identity.rotation.ApplyScaleLocal(scaleAmount);
      return identity;
    }

    public BoundingBox GetBoundingBox()
    {
      BoundingBox outBoundingBox = new BoundingBox();
      EngineApplicationInterface.IMetaMesh.GetBoundingBox(this.Pointer, ref outBoundingBox);
      return outBoundingBox;
    }

    public VisibilityMaskFlags GetVisibilityMask() => EngineApplicationInterface.IMetaMesh.GetVisibilityMask(this.Pointer);

    public void SetVisibilityMask(VisibilityMaskFlags visibilityMask) => EngineApplicationInterface.IMetaMesh.SetVisibilityMask(this.Pointer, visibilityMask);

    public string GetName() => EngineApplicationInterface.IMetaMesh.GetName(this.Pointer);

    public static void GetAllMultiMeshes(ref List<MetaMesh> multiMeshList)
    {
      int multiMeshCount = EngineApplicationInterface.IMetaMesh.GetMultiMeshCount();
      UIntPtr[] gameEntitiesTemp = new UIntPtr[multiMeshCount];
      EngineApplicationInterface.IMetaMesh.GetAllMultiMeshes(gameEntitiesTemp);
      for (int index = 0; index < multiMeshCount; ++index)
        multiMeshList.Add(new MetaMesh(gameEntitiesTemp[index]));
    }

    public static void GetMultiMesh(string name, ref MetaMesh multiMesh)
    {
      UIntPtr[] gameEntity = new UIntPtr[1];
      EngineApplicationInterface.IMetaMesh.GetMultiMesh(name, gameEntity);
      multiMesh = new MetaMesh(gameEntity[0]);
    }

    public void SetContourState(bool alwaysVisible) => EngineApplicationInterface.IMetaMesh.SetContourState(this.Pointer, alwaysVisible);

    public void SetContourColor(uint color) => EngineApplicationInterface.IMetaMesh.SetContourColor(this.Pointer, color);

    public void SetMaterialToSubMeshesWithTag(Material bodyMaterial, string tag) => EngineApplicationInterface.IMetaMesh.SetMaterialToSubMeshesWithTag(this.Pointer, bodyMaterial.Pointer, tag);

    public void SetFactorColorToSubMeshesWithTag(uint color, string tag) => EngineApplicationInterface.IMetaMesh.SetFactorColorToSubMeshesWithTag(this.Pointer, color, tag);
  }
}
