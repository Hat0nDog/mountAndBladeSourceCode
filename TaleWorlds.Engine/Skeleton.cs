// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Skeleton
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglSkeleton")]
  public sealed class Skeleton : NativeObject
  {
    public const sbyte MaxBoneCount = 64;

    internal Skeleton(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static Skeleton CreateFromModel(string modelName) => EngineApplicationInterface.ISkeleton.CreateFromModel(modelName);

    public static Skeleton CreateFromModelWithNullAnimTree(
      GameEntity entity,
      string modelName,
      float boneScale = 1f)
    {
      return EngineApplicationInterface.ISkeleton.CreateFromModelWithNullAnimTree(entity.Pointer, modelName, boneScale);
    }

    public bool IsValid => this.Pointer != UIntPtr.Zero;

    public string GetName() => EngineApplicationInterface.ISkeleton.GetName(this);

    public string GetBoneName(byte boneIndex) => EngineApplicationInterface.ISkeleton.GetBoneName(this, (int) boneIndex);

    public byte GetBoneChildAtIndex(byte boneIndex, byte childIndex) => EngineApplicationInterface.ISkeleton.GetBoneChildAtIndex(this, boneIndex, childIndex);

    public byte GetBoneChildCount(byte boneIndex) => EngineApplicationInterface.ISkeleton.GetBoneChildCount(this, boneIndex);

    public sbyte GetParentBoneIndex(byte boneIndex) => EngineApplicationInterface.ISkeleton.GetParentBoneIndex(this, (int) boneIndex);

    public void AddMeshToBone(UIntPtr mesh, sbyte boneIndex) => EngineApplicationInterface.ISkeleton.AddMeshToBone(this.Pointer, mesh, boneIndex);

    public void Freeze(bool p) => EngineApplicationInterface.ISkeleton.Freeze(this.Pointer, p);

    public bool IsFrozen() => EngineApplicationInterface.ISkeleton.IsFrozen(this.Pointer);

    public void SetBoneLocalFrame(int boneIndex, MatrixFrame localFrame) => EngineApplicationInterface.ISkeleton.SetBoneLocalFrame(this.Pointer, boneIndex, ref localFrame);

    public byte GetBoneCount() => EngineApplicationInterface.ISkeleton.GetBoneCount(this.Pointer);

    public void GetBoneBody(sbyte boneIndex, ref CapsuleData data) => EngineApplicationInterface.ISkeleton.GetBoneBody(this.Pointer, boneIndex, ref data);

    public static bool SkeletonModelExist(string skeletonModelName) => EngineApplicationInterface.ISkeleton.SkeletonModelExist(skeletonModelName);

    public void ForceUpdateBoneFrames() => EngineApplicationInterface.ISkeleton.ForceUpdateBoneFrames(this.Pointer);

    public MatrixFrame GetBoneEntitialFrameWithIndex(byte boneIndex)
    {
      MatrixFrame outEntitialFrame = new MatrixFrame();
      EngineApplicationInterface.ISkeleton.GetBoneEntitialFrameWithIndex(this.Pointer, boneIndex, ref outEntitialFrame);
      return outEntitialFrame;
    }

    public MatrixFrame GetBoneEntitialFrameWithName(string boneName)
    {
      MatrixFrame outEntitialFrame = new MatrixFrame();
      EngineApplicationInterface.ISkeleton.GetBoneEntitialFrameWithName(this.Pointer, boneName, ref outEntitialFrame);
      return outEntitialFrame;
    }

    public RagdollState GetCurrentRagdollState() => EngineApplicationInterface.ISkeleton.GetCurrentRagdollState(this.Pointer);

    public void ActivateRagdoll() => EngineApplicationInterface.ISkeleton.ActivateRagdoll(this.Pointer);

    public sbyte GetSkeletonBoneMapping(sbyte boneIndex) => EngineApplicationInterface.ISkeleton.GetSkeletonBoneMapping(this.Pointer, boneIndex);

    public void AddMesh(Mesh mesh) => EngineApplicationInterface.ISkeleton.AddMesh(this.Pointer, mesh.Pointer);

    public void ClearComponents() => EngineApplicationInterface.ISkeleton.ClearComponents(this.Pointer);

    public void AddComponent(GameEntityComponent component) => EngineApplicationInterface.ISkeleton.AddComponent(this.Pointer, component.Pointer);

    public bool HasComponent(GameEntityComponent component) => EngineApplicationInterface.ISkeleton.HasComponent(this.Pointer, component.Pointer);

    public void RemoveComponent(GameEntityComponent component) => EngineApplicationInterface.ISkeleton.RemoveComponent(this.Pointer, component.Pointer);

    public void ClearMeshes(bool clearBoneComponents = true) => EngineApplicationInterface.ISkeleton.ClearMeshes(this.Pointer, clearBoneComponents);

    public int GetComponentCount(GameEntity.ComponentType componentType) => EngineApplicationInterface.ISkeleton.GetComponentCount(this.Pointer, componentType);

    public void UpdateEntitialFramesFromLocalFrames() => EngineApplicationInterface.ISkeleton.UpdateEntitialFramesFromLocalFrames(this.Pointer);

    public void ResetFrames() => EngineApplicationInterface.ISkeleton.ResetFrames(this.Pointer);

    public GameEntityComponent GetComponentAtIndex(
      GameEntity.ComponentType componentType,
      int index)
    {
      return EngineApplicationInterface.ISkeleton.GetComponentAtIndex(this.Pointer, componentType, index);
    }

    public void SetUsePreciseBoundingVolume(bool value) => EngineApplicationInterface.ISkeleton.SetUsePreciseBoundingVolume(this.Pointer, value);

    public MatrixFrame GetBoneEntitialRestFrame(sbyte boneNumber, bool useBoneMapping)
    {
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.ISkeleton.GetBoneEntitialRestFrame(this.Pointer, boneNumber, useBoneMapping, ref outFrame);
      return outFrame;
    }

    public MatrixFrame GetBoneLocalRestFrame(sbyte boneNumber, bool useBoneMapping = true)
    {
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.ISkeleton.GetBoneLocalRestFrame(this.Pointer, boneNumber, useBoneMapping, ref outFrame);
      return outFrame;
    }

    public MatrixFrame GetBoneEntitialRestFrame(sbyte boneNumber)
    {
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.ISkeleton.GetBoneEntitialRestFrame(this.Pointer, boneNumber, true, ref outFrame);
      return outFrame;
    }

    public MatrixFrame GetBoneEntitialFrameAtChannel(int channelNo, sbyte boneNumber)
    {
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.ISkeleton.GetBoneEntitialFrameAtChannel(this.Pointer, channelNo, boneNumber, ref outFrame);
      return outFrame;
    }

    public MatrixFrame GetBoneEntitialFrame(int boneIndex)
    {
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.ISkeleton.GetBoneEntitialFrame(this.Pointer, boneIndex, ref outFrame);
      return outFrame;
    }

    public int GetBoneComponentCount(int boneIndex) => EngineApplicationInterface.ISkeleton.GetBoneComponentCount(this.Pointer, boneIndex);

    public GameEntityComponent GetBoneComponentAtIndex(
      int boneIndex,
      int componentIndex)
    {
      return EngineApplicationInterface.ISkeleton.GetBoneComponentAtIndex(this.Pointer, boneIndex, componentIndex);
    }

    public bool HasBoneComponent(int boneIndex, GameEntityComponent component) => EngineApplicationInterface.ISkeleton.HasBoneComponent(this.Pointer, boneIndex, component);

    public void AddComponentToBone(int boneIndex, GameEntityComponent component) => EngineApplicationInterface.ISkeleton.AddComponentToBone(this.Pointer, boneIndex, component);

    public void RemoveBoneComponent(int boneIndex, GameEntityComponent component) => EngineApplicationInterface.ISkeleton.RemoveBoneComponent(this.Pointer, boneIndex, component);

    public void ClearMeshesAtBone(int boneIndex) => EngineApplicationInterface.ISkeleton.ClearMeshesAtBone(this.Pointer, boneIndex);

    public void TickAnimations(float dt, MatrixFrame globalFrame, bool tickAnimsForChildren) => EngineApplicationInterface.ISkeleton.TickAnimations(this.Pointer, ref globalFrame, dt, tickAnimsForChildren);

    public void TickAnimationsAndForceUpdate(
      float dt,
      MatrixFrame globalFrame,
      bool tickAnimsForChildren)
    {
      EngineApplicationInterface.ISkeleton.TickAnimationsAndForceUpdate(this.Pointer, ref globalFrame, dt, tickAnimsForChildren);
    }

    public float GetAnimationParameterAtChannel(int channelNo) => EngineApplicationInterface.ISkeleton.GetSkeletonAnimationParameterAtChannel(this.Pointer, channelNo);

    public void SetAnimationParameterAtChannel(int channelNo, float parameter) => EngineApplicationInterface.ISkeleton.SetSkeletonAnimationParameterAtChannel(this.Pointer, channelNo, parameter);

    public float GetAnimationSpeedAtChannel(int channelNo) => EngineApplicationInterface.ISkeleton.GetSkeletonAnimationSpeedAtChannel(this.Pointer, channelNo);

    public void SetUptoDate(bool value) => EngineApplicationInterface.ISkeleton.SetSkeletonUptoDate(this.Pointer, value);

    public string GetAnimationAtChannel(int channelNo) => EngineApplicationInterface.ISkeleton.GetAnimationAtChannel(this.Pointer, channelNo);

    public void ResetCloths() => EngineApplicationInterface.ISkeleton.ResetCloths(this.Pointer);

    public IEnumerable<Mesh> GetAllMeshes()
    {
      Skeleton skeleton = this;
      NativeObjectArray nativeObjectArray = NativeObjectArray.Create();
      EngineApplicationInterface.ISkeleton.GetAllMeshes(skeleton, nativeObjectArray);
      foreach (Mesh mesh in (IEnumerable<NativeObject>) nativeObjectArray)
        yield return mesh;
    }

    public static sbyte GetBoneIndexFromName(string skeletonModelName, string boneName) => EngineApplicationInterface.ISkeleton.GetBoneIndexFromName(skeletonModelName, boneName);
  }
}
