﻿// Decompiled with JetBrains decompiler
// Type: ManagedCallbacks.ScriptingInterfaceOfIMBAgentVisuals
// Assembly: TaleWorlds.MountAndBlade.AutoGenerated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D02C25D1-9727-49C6-A24A-EE3800F0364C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.AutoGenerated.dll

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.AutoGenerated;

namespace ManagedCallbacks
{
  internal class ScriptingInterfaceOfIMBAgentVisuals : IMBAgentVisuals
  {
    private static readonly Encoding _utf8 = Encoding.UTF8;
    public static ScriptingInterfaceOfIMBAgentVisuals.CreateAgentRendererSceneControllerDelegate call_CreateAgentRendererSceneControllerDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.DestructAgentRendererSceneControllerDelegate call_DestructAgentRendererSceneControllerDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.CreateAgentVisualsDelegate call_CreateAgentVisualsDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.TickDelegate call_TickDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetEntityDelegate call_SetEntityDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetSkeletonDelegate call_SetSkeletonDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.AddSkinMeshesToAgentEntityDelegate call_AddSkinMeshesToAgentEntityDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetLodAtlasShadingIndexDelegate call_SetLodAtlasShadingIndexDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetFaceGenerationParamsDelegate call_SetFaceGenerationParamsDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.StartRhubarbRecordDelegate call_StartRhubarbRecordDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.ClearVisualComponentsDelegate call_ClearVisualComponentsDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.LazyUpdateAgentRendererDataDelegate call_LazyUpdateAgentRendererDataDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.AddMeshDelegate call_AddMeshDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.RemoveMeshDelegate call_RemoveMeshDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.AddMultiMeshDelegate call_AddMultiMeshDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.AddHorseReinsClothMeshDelegate call_AddHorseReinsClothMeshDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.UpdateSkeletonScaleDelegate call_UpdateSkeletonScaleDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.ApplySkeletonScaleDelegate call_ApplySkeletonScaleDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.BatchLastLodMeshesDelegate call_BatchLastLodMeshesDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.RemoveMultiMeshDelegate call_RemoveMultiMeshDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.AddWeaponToAgentEntityDelegate call_AddWeaponToAgentEntityDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.UpdateQuiverMeshesWithoutAgentDelegate call_UpdateQuiverMeshesWithoutAgentDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetWieldedWeaponIndicesDelegate call_SetWieldedWeaponIndicesDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.ClearAllWeaponMeshesDelegate call_ClearAllWeaponMeshesDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.ClearWeaponMeshesDelegate call_ClearWeaponMeshesDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.MakeVoiceDelegate call_MakeVoiceDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetSetupMorphNodeDelegate call_SetSetupMorphNodeDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.UseScaledWeaponsDelegate call_UseScaledWeaponsDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetClothComponentKeepStateOfAllMeshesDelegate call_SetClothComponentKeepStateOfAllMeshesDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetCurrentHelmetScalingFactorDelegate call_GetCurrentHelmetScalingFactorDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetVoiceDefinitionIndexDelegate call_SetVoiceDefinitionIndexDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetAgentLodLevelExternalDelegate call_SetAgentLodLevelExternalDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetAgentLocalSpeedDelegate call_SetAgentLocalSpeedDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetLookDirectionDelegate call_SetLookDirectionDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.ResetDelegate call_ResetDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.ResetNextFrameDelegate call_ResetNextFrameDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetFrameDelegate call_SetFrameDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetFrameDelegate call_GetFrameDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetGlobalFrameDelegate call_GetGlobalFrameDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetVisibleDelegate call_SetVisibleDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetVisibleDelegate call_GetVisibleDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetSkeletonDelegate call_GetSkeletonDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetEntityDelegate call_GetEntityDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetGlobalStableEyePointDelegate call_GetGlobalStableEyePointDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetGlobalStableNeckPointDelegate call_GetGlobalStableNeckPointDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.AddPrefabToAgentVisualBoneDelegate call_AddPrefabToAgentVisualBoneDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetAttachedWeaponEntityDelegate call_GetAttachedWeaponEntityDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.CreateParticleSystemAttachedToBoneDelegate call_CreateParticleSystemAttachedToBoneDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.CheckResourcesDelegate call_CheckResourcesDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.AddChildEntityDelegate call_AddChildEntityDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetClothWindToWeaponAtIndexDelegate call_SetClothWindToWeaponAtIndexDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.RemoveChildEntityDelegate call_RemoveChildEntityDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.DisableContourDelegate call_DisableContourDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetAsContourEntityDelegate call_SetAsContourEntityDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetContourStateDelegate call_SetContourStateDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.SetEnableOcclusionCullingDelegate call_SetEnableOcclusionCullingDelegate;
    public static ScriptingInterfaceOfIMBAgentVisuals.GetBoneTypeDataDelegate call_GetBoneTypeDataDelegate;

    public UIntPtr CreateAgentRendererSceneController(
      UIntPtr scenePointer,
      int maxRenderCount)
    {
      return ScriptingInterfaceOfIMBAgentVisuals.call_CreateAgentRendererSceneControllerDelegate(scenePointer, maxRenderCount);
    }

    public void DestructAgentRendererSceneController(
      UIntPtr scenePointer,
      UIntPtr agentRendererSceneControllerPointer)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_DestructAgentRendererSceneControllerDelegate(scenePointer, agentRendererSceneControllerPointer);
    }

    public MBAgentVisuals CreateAgentVisuals(
      UIntPtr scenePtr,
      string ownerName,
      Vec3 eyeOffset)
    {
      byte[] numArray = (byte[]) null;
      if (ownerName != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBAgentVisuals._utf8.GetBytes(ownerName, 0, ownerName.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      NativeObjectPointer nativeObjectPointer = ScriptingInterfaceOfIMBAgentVisuals.call_CreateAgentVisualsDelegate(scenePtr, numArray, eyeOffset);
      MBAgentVisuals mbAgentVisuals = (MBAgentVisuals) null;
      if (nativeObjectPointer.Pointer != UIntPtr.Zero)
      {
        mbAgentVisuals = new MBAgentVisuals(nativeObjectPointer.Pointer);
        LibraryApplicationInterface.IManaged.DecreaseReferenceCount(nativeObjectPointer.Pointer);
      }
      return mbAgentVisuals;
    }

    public void Tick(
      UIntPtr agentVisualsId,
      UIntPtr parentAgentVisualsId,
      float dt,
      bool entityMoving,
      float speed)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_TickDelegate(agentVisualsId, parentAgentVisualsId, dt, entityMoving, speed);
    }

    public void SetEntity(UIntPtr agentVisualsId, UIntPtr entityPtr) => ScriptingInterfaceOfIMBAgentVisuals.call_SetEntityDelegate(agentVisualsId, entityPtr);

    public void SetSkeleton(UIntPtr agentVisualsId, UIntPtr skeletonPtr) => ScriptingInterfaceOfIMBAgentVisuals.call_SetSkeletonDelegate(agentVisualsId, skeletonPtr);

    public void AddSkinMeshesToAgentEntity(
      UIntPtr agentVisualsId,
      ref SkinGenerationParams skinParams,
      ref BodyProperties bodyProperties,
      bool useFaceCache)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_AddSkinMeshesToAgentEntityDelegate(agentVisualsId, ref skinParams, ref bodyProperties, useFaceCache);
    }

    public void SetLodAtlasShadingIndex(
      UIntPtr agentVisualsId,
      int index,
      bool useTeamColor,
      uint teamColor1,
      uint teamColor2)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_SetLodAtlasShadingIndexDelegate(agentVisualsId, index, useTeamColor, teamColor1, teamColor2);
    }

    public void SetFaceGenerationParams(
      UIntPtr agentVisualsId,
      FaceGenerationParams faceGenerationParams)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_SetFaceGenerationParamsDelegate(agentVisualsId, faceGenerationParams);
    }

    public void StartRhubarbRecord(UIntPtr agentVisualsId, string path, int soundId)
    {
      byte[] numArray = (byte[]) null;
      if (path != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBAgentVisuals._utf8.GetBytes(path, 0, path.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      ScriptingInterfaceOfIMBAgentVisuals.call_StartRhubarbRecordDelegate(agentVisualsId, numArray, soundId);
    }

    public void ClearVisualComponents(UIntPtr agentVisualsId, bool removeSkeleton) => ScriptingInterfaceOfIMBAgentVisuals.call_ClearVisualComponentsDelegate(agentVisualsId, removeSkeleton);

    public void LazyUpdateAgentRendererData(UIntPtr agentVisualsId) => ScriptingInterfaceOfIMBAgentVisuals.call_LazyUpdateAgentRendererDataDelegate(agentVisualsId);

    public void AddMesh(UIntPtr agentVisualsId, UIntPtr meshPointer) => ScriptingInterfaceOfIMBAgentVisuals.call_AddMeshDelegate(agentVisualsId, meshPointer);

    public void RemoveMesh(UIntPtr agentVisualsPtr, UIntPtr meshPointer) => ScriptingInterfaceOfIMBAgentVisuals.call_RemoveMeshDelegate(agentVisualsPtr, meshPointer);

    public void AddMultiMesh(UIntPtr agentVisualsPtr, UIntPtr multiMeshPointer, int bodyMeshIndex) => ScriptingInterfaceOfIMBAgentVisuals.call_AddMultiMeshDelegate(agentVisualsPtr, multiMeshPointer, bodyMeshIndex);

    public void AddHorseReinsClothMesh(
      UIntPtr agentVisualsPtr,
      UIntPtr reinMeshPointer,
      UIntPtr ropeMeshPointer)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_AddHorseReinsClothMeshDelegate(agentVisualsPtr, reinMeshPointer, ropeMeshPointer);
    }

    public void UpdateSkeletonScale(UIntPtr agentVisualsId, int bodyDeformType) => ScriptingInterfaceOfIMBAgentVisuals.call_UpdateSkeletonScaleDelegate(agentVisualsId, bodyDeformType);

    public void ApplySkeletonScale(
      UIntPtr agentVisualsId,
      Vec3 mountSitBoneScale,
      float mountRadiusAdder,
      byte boneCount,
      sbyte[] boneIndices,
      Vec3[] boneScales)
    {
      PinnedArrayData<sbyte> pinnedArrayData1 = new PinnedArrayData<sbyte>(boneIndices);
      IntPtr pointer1 = pinnedArrayData1.Pointer;
      PinnedArrayData<Vec3> pinnedArrayData2 = new PinnedArrayData<Vec3>(boneScales);
      IntPtr pointer2 = pinnedArrayData2.Pointer;
      ScriptingInterfaceOfIMBAgentVisuals.call_ApplySkeletonScaleDelegate(agentVisualsId, mountSitBoneScale, mountRadiusAdder, boneCount, pointer1, pointer2);
      pinnedArrayData1.Dispose();
      pinnedArrayData2.Dispose();
    }

    public void BatchLastLodMeshes(UIntPtr agentVisualsPtr) => ScriptingInterfaceOfIMBAgentVisuals.call_BatchLastLodMeshesDelegate(agentVisualsPtr);

    public void RemoveMultiMesh(
      UIntPtr agentVisualsPtr,
      UIntPtr multiMeshPointer,
      int bodyMeshIndex)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_RemoveMultiMeshDelegate(agentVisualsPtr, multiMeshPointer, bodyMeshIndex);
    }

    public void AddWeaponToAgentEntity(
      UIntPtr agentVisualsPtr,
      int slotIndex,
      in WeaponData agentEntityData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      in WeaponData agentEntityAmmoData,
      WeaponStatsData[] ammoWeaponStatsData,
      int ammoWeaponStatsDataLength,
      GameEntity cachedEntity)
    {
      WeaponDataAsNative agentEntityData1 = new WeaponDataAsNative(agentEntityData);
      PinnedArrayData<WeaponStatsData> pinnedArrayData1 = new PinnedArrayData<WeaponStatsData>(weaponStatsData);
      IntPtr pointer1 = pinnedArrayData1.Pointer;
      WeaponDataAsNative agentEntityAmmoData1 = new WeaponDataAsNative(agentEntityAmmoData);
      PinnedArrayData<WeaponStatsData> pinnedArrayData2 = new PinnedArrayData<WeaponStatsData>(ammoWeaponStatsData);
      IntPtr pointer2 = pinnedArrayData2.Pointer;
      UIntPtr cachedEntity1 = (NativeObject) cachedEntity != (NativeObject) null ? cachedEntity.Pointer : UIntPtr.Zero;
      ScriptingInterfaceOfIMBAgentVisuals.call_AddWeaponToAgentEntityDelegate(agentVisualsPtr, slotIndex, in agentEntityData1, pointer1, weaponStatsDataLength, in agentEntityAmmoData1, pointer2, ammoWeaponStatsDataLength, cachedEntity1);
      pinnedArrayData1.Dispose();
      pinnedArrayData2.Dispose();
    }

    public void UpdateQuiverMeshesWithoutAgent(
      UIntPtr agentVisualsId,
      int weaponIndex,
      int ammoCountToShow)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_UpdateQuiverMeshesWithoutAgentDelegate(agentVisualsId, weaponIndex, ammoCountToShow);
    }

    public void SetWieldedWeaponIndices(
      UIntPtr agentVisualsId,
      int slotIndexRightHand,
      int slotIndexLeftHand)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_SetWieldedWeaponIndicesDelegate(agentVisualsId, slotIndexRightHand, slotIndexLeftHand);
    }

    public void ClearAllWeaponMeshes(UIntPtr agentVisualsPtr) => ScriptingInterfaceOfIMBAgentVisuals.call_ClearAllWeaponMeshesDelegate(agentVisualsPtr);

    public void ClearWeaponMeshes(UIntPtr agentVisualsPtr, int weaponVisualIndex) => ScriptingInterfaceOfIMBAgentVisuals.call_ClearWeaponMeshesDelegate(agentVisualsPtr, weaponVisualIndex);

    public void MakeVoice(UIntPtr agentVisualsPtr, int voiceId, ref Vec3 position) => ScriptingInterfaceOfIMBAgentVisuals.call_MakeVoiceDelegate(agentVisualsPtr, voiceId, ref position);

    public void SetSetupMorphNode(UIntPtr agentVisualsPtr, bool value) => ScriptingInterfaceOfIMBAgentVisuals.call_SetSetupMorphNodeDelegate(agentVisualsPtr, value);

    public void UseScaledWeapons(UIntPtr agentVisualsPtr, bool value) => ScriptingInterfaceOfIMBAgentVisuals.call_UseScaledWeaponsDelegate(agentVisualsPtr, value);

    public void SetClothComponentKeepStateOfAllMeshes(UIntPtr agentVisualsPtr, bool keepState) => ScriptingInterfaceOfIMBAgentVisuals.call_SetClothComponentKeepStateOfAllMeshesDelegate(agentVisualsPtr, keepState);

    public Vec3 GetCurrentHelmetScalingFactor(UIntPtr agentVisualsPtr) => ScriptingInterfaceOfIMBAgentVisuals.call_GetCurrentHelmetScalingFactorDelegate(agentVisualsPtr);

    public void SetVoiceDefinitionIndex(
      UIntPtr agentVisualsPtr,
      int voiceDefinitionIndex,
      float voicePitch)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_SetVoiceDefinitionIndexDelegate(agentVisualsPtr, voiceDefinitionIndex, voicePitch);
    }

    public void SetAgentLodLevelExternal(UIntPtr agentVisualsPtr, float level) => ScriptingInterfaceOfIMBAgentVisuals.call_SetAgentLodLevelExternalDelegate(agentVisualsPtr, level);

    public void SetAgentLocalSpeed(UIntPtr agentVisualsPtr, Vec2 speed) => ScriptingInterfaceOfIMBAgentVisuals.call_SetAgentLocalSpeedDelegate(agentVisualsPtr, speed);

    public void SetLookDirection(UIntPtr agentVisualsPtr, Vec3 direction) => ScriptingInterfaceOfIMBAgentVisuals.call_SetLookDirectionDelegate(agentVisualsPtr, direction);

    public void Reset(UIntPtr agentVisualsPtr) => ScriptingInterfaceOfIMBAgentVisuals.call_ResetDelegate(agentVisualsPtr);

    public void ResetNextFrame(UIntPtr agentVisualsPtr) => ScriptingInterfaceOfIMBAgentVisuals.call_ResetNextFrameDelegate(agentVisualsPtr);

    public void SetFrame(UIntPtr agentVisualsPtr, ref MatrixFrame frame) => ScriptingInterfaceOfIMBAgentVisuals.call_SetFrameDelegate(agentVisualsPtr, ref frame);

    public void GetFrame(UIntPtr agentVisualsPtr, ref MatrixFrame outFrame) => ScriptingInterfaceOfIMBAgentVisuals.call_GetFrameDelegate(agentVisualsPtr, ref outFrame);

    public void GetGlobalFrame(UIntPtr agentVisualsPtr, ref MatrixFrame outFrame) => ScriptingInterfaceOfIMBAgentVisuals.call_GetGlobalFrameDelegate(agentVisualsPtr, ref outFrame);

    public void SetVisible(UIntPtr agentVisualsPtr, bool value) => ScriptingInterfaceOfIMBAgentVisuals.call_SetVisibleDelegate(agentVisualsPtr, value);

    public bool GetVisible(UIntPtr agentVisualsPtr) => ScriptingInterfaceOfIMBAgentVisuals.call_GetVisibleDelegate(agentVisualsPtr);

    public Skeleton GetSkeleton(UIntPtr agentVisualsPtr)
    {
      NativeObjectPointer nativeObjectPointer = ScriptingInterfaceOfIMBAgentVisuals.call_GetSkeletonDelegate(agentVisualsPtr);
      Skeleton skeleton = (Skeleton) null;
      if (nativeObjectPointer.Pointer != UIntPtr.Zero)
      {
        skeleton = new Skeleton(nativeObjectPointer.Pointer);
        LibraryApplicationInterface.IManaged.DecreaseReferenceCount(nativeObjectPointer.Pointer);
      }
      return skeleton;
    }

    public GameEntity GetEntity(UIntPtr agentVisualsPtr)
    {
      NativeObjectPointer nativeObjectPointer = ScriptingInterfaceOfIMBAgentVisuals.call_GetEntityDelegate(agentVisualsPtr);
      GameEntity gameEntity = (GameEntity) null;
      if (nativeObjectPointer.Pointer != UIntPtr.Zero)
      {
        gameEntity = new GameEntity(nativeObjectPointer.Pointer);
        LibraryApplicationInterface.IManaged.DecreaseReferenceCount(nativeObjectPointer.Pointer);
      }
      return gameEntity;
    }

    public Vec3 GetGlobalStableEyePoint(UIntPtr agentVisualsPtr, bool isHumanoid) => ScriptingInterfaceOfIMBAgentVisuals.call_GetGlobalStableEyePointDelegate(agentVisualsPtr, isHumanoid);

    public Vec3 GetGlobalStableNeckPoint(UIntPtr agentVisualsPtr, bool isHumanoid) => ScriptingInterfaceOfIMBAgentVisuals.call_GetGlobalStableNeckPointDelegate(agentVisualsPtr, isHumanoid);

    public void AddPrefabToAgentVisualBone(
      UIntPtr agentVisualsPtr,
      string prefabName,
      sbyte boneIndex)
    {
      byte[] numArray = (byte[]) null;
      if (prefabName != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBAgentVisuals._utf8.GetBytes(prefabName, 0, prefabName.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      ScriptingInterfaceOfIMBAgentVisuals.call_AddPrefabToAgentVisualBoneDelegate(agentVisualsPtr, numArray, boneIndex);
    }

    public GameEntity GetAttachedWeaponEntity(
      UIntPtr agentVisualsPtr,
      int attachedWeaponIndex)
    {
      NativeObjectPointer nativeObjectPointer = ScriptingInterfaceOfIMBAgentVisuals.call_GetAttachedWeaponEntityDelegate(agentVisualsPtr, attachedWeaponIndex);
      GameEntity gameEntity = (GameEntity) null;
      if (nativeObjectPointer.Pointer != UIntPtr.Zero)
      {
        gameEntity = new GameEntity(nativeObjectPointer.Pointer);
        LibraryApplicationInterface.IManaged.DecreaseReferenceCount(nativeObjectPointer.Pointer);
      }
      return gameEntity;
    }

    public void CreateParticleSystemAttachedToBone(
      UIntPtr agentVisualsPtr,
      int runtimeParticleindex,
      sbyte boneIndex,
      ref MatrixFrame boneLocalParticleFrame)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_CreateParticleSystemAttachedToBoneDelegate(agentVisualsPtr, runtimeParticleindex, boneIndex, ref boneLocalParticleFrame);
    }

    public bool CheckResources(UIntPtr agentVisualsPtr, bool addToQueue) => ScriptingInterfaceOfIMBAgentVisuals.call_CheckResourcesDelegate(agentVisualsPtr, addToQueue);

    public bool AddChildEntity(UIntPtr agentVisualsPtr, UIntPtr EntityId) => ScriptingInterfaceOfIMBAgentVisuals.call_AddChildEntityDelegate(agentVisualsPtr, EntityId);

    public void SetClothWindToWeaponAtIndex(
      UIntPtr agentVisualsPtr,
      Vec3 windDirection,
      bool isLocal,
      int index)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_SetClothWindToWeaponAtIndexDelegate(agentVisualsPtr, windDirection, isLocal, index);
    }

    public void RemoveChildEntity(UIntPtr agentVisualsPtr, UIntPtr EntityId, int removeReason) => ScriptingInterfaceOfIMBAgentVisuals.call_RemoveChildEntityDelegate(agentVisualsPtr, EntityId, removeReason);

    public void DisableContour(UIntPtr agentVisualsPtr) => ScriptingInterfaceOfIMBAgentVisuals.call_DisableContourDelegate(agentVisualsPtr);

    public void SetAsContourEntity(UIntPtr agentVisualsPtr, uint color) => ScriptingInterfaceOfIMBAgentVisuals.call_SetAsContourEntityDelegate(agentVisualsPtr, color);

    public void SetContourState(UIntPtr agentVisualsPtr, bool alwaysVisible) => ScriptingInterfaceOfIMBAgentVisuals.call_SetContourStateDelegate(agentVisualsPtr, alwaysVisible);

    public void SetEnableOcclusionCulling(UIntPtr agentVisualsPtr, bool enable) => ScriptingInterfaceOfIMBAgentVisuals.call_SetEnableOcclusionCullingDelegate(agentVisualsPtr, enable);

    public void GetBoneTypeData(
      UIntPtr pointer,
      sbyte boneIndex,
      ref BoneBodyTypeData boneBodyTypeData)
    {
      ScriptingInterfaceOfIMBAgentVisuals.call_GetBoneTypeDataDelegate(pointer, boneIndex, ref boneBodyTypeData);
    }

    void IMBAgentVisuals.AddWeaponToAgentEntity(
      UIntPtr agentVisualsPtr,
      int slotIndex,
      in WeaponData agentEntityData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      in WeaponData agentEntityAmmoData,
      WeaponStatsData[] ammoWeaponStatsData,
      int ammoWeaponStatsDataLength,
      GameEntity cachedEntity)
    {
      this.AddWeaponToAgentEntity(agentVisualsPtr, slotIndex, in agentEntityData, weaponStatsData, weaponStatsDataLength, in agentEntityAmmoData, ammoWeaponStatsData, ammoWeaponStatsDataLength, cachedEntity);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate UIntPtr CreateAgentRendererSceneControllerDelegate(
      UIntPtr scenePointer,
      int maxRenderCount);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void DestructAgentRendererSceneControllerDelegate(
      UIntPtr scenePointer,
      UIntPtr agentRendererSceneControllerPointer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate NativeObjectPointer CreateAgentVisualsDelegate(
      UIntPtr scenePtr,
      byte[] ownerName,
      Vec3 eyeOffset);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void TickDelegate(
      UIntPtr agentVisualsId,
      UIntPtr parentAgentVisualsId,
      float dt,
      [MarshalAs(UnmanagedType.U1)] bool entityMoving,
      float speed);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetEntityDelegate(UIntPtr agentVisualsId, UIntPtr entityPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetSkeletonDelegate(UIntPtr agentVisualsId, UIntPtr skeletonPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void AddSkinMeshesToAgentEntityDelegate(
      UIntPtr agentVisualsId,
      ref SkinGenerationParams skinParams,
      ref BodyProperties bodyProperties,
      [MarshalAs(UnmanagedType.U1)] bool useFaceCache);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetLodAtlasShadingIndexDelegate(
      UIntPtr agentVisualsId,
      int index,
      [MarshalAs(UnmanagedType.U1)] bool useTeamColor,
      uint teamColor1,
      uint teamColor2);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetFaceGenerationParamsDelegate(
      UIntPtr agentVisualsId,
      FaceGenerationParams faceGenerationParams);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void StartRhubarbRecordDelegate(
      UIntPtr agentVisualsId,
      byte[] path,
      int soundId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ClearVisualComponentsDelegate(UIntPtr agentVisualsId, [MarshalAs(UnmanagedType.U1)] bool removeSkeleton);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void LazyUpdateAgentRendererDataDelegate(UIntPtr agentVisualsId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void AddMeshDelegate(UIntPtr agentVisualsId, UIntPtr meshPointer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void RemoveMeshDelegate(UIntPtr agentVisualsPtr, UIntPtr meshPointer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void AddMultiMeshDelegate(
      UIntPtr agentVisualsPtr,
      UIntPtr multiMeshPointer,
      int bodyMeshIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void AddHorseReinsClothMeshDelegate(
      UIntPtr agentVisualsPtr,
      UIntPtr reinMeshPointer,
      UIntPtr ropeMeshPointer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void UpdateSkeletonScaleDelegate(UIntPtr agentVisualsId, int bodyDeformType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ApplySkeletonScaleDelegate(
      UIntPtr agentVisualsId,
      Vec3 mountSitBoneScale,
      float mountRadiusAdder,
      byte boneCount,
      IntPtr boneIndices,
      IntPtr boneScales);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void BatchLastLodMeshesDelegate(UIntPtr agentVisualsPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void RemoveMultiMeshDelegate(
      UIntPtr agentVisualsPtr,
      UIntPtr multiMeshPointer,
      int bodyMeshIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void AddWeaponToAgentEntityDelegate(
      UIntPtr agentVisualsPtr,
      int slotIndex,
      in WeaponDataAsNative agentEntityData,
      IntPtr weaponStatsData,
      int weaponStatsDataLength,
      in WeaponDataAsNative agentEntityAmmoData,
      IntPtr ammoWeaponStatsData,
      int ammoWeaponStatsDataLength,
      UIntPtr cachedEntity);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void UpdateQuiverMeshesWithoutAgentDelegate(
      UIntPtr agentVisualsId,
      int weaponIndex,
      int ammoCountToShow);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetWieldedWeaponIndicesDelegate(
      UIntPtr agentVisualsId,
      int slotIndexRightHand,
      int slotIndexLeftHand);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ClearAllWeaponMeshesDelegate(UIntPtr agentVisualsPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ClearWeaponMeshesDelegate(UIntPtr agentVisualsPtr, int weaponVisualIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void MakeVoiceDelegate(UIntPtr agentVisualsPtr, int voiceId, ref Vec3 position);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetSetupMorphNodeDelegate(UIntPtr agentVisualsPtr, [MarshalAs(UnmanagedType.U1)] bool value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void UseScaledWeaponsDelegate(UIntPtr agentVisualsPtr, [MarshalAs(UnmanagedType.U1)] bool value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetClothComponentKeepStateOfAllMeshesDelegate(
      UIntPtr agentVisualsPtr,
      [MarshalAs(UnmanagedType.U1)] bool keepState);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate Vec3 GetCurrentHelmetScalingFactorDelegate(UIntPtr agentVisualsPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetVoiceDefinitionIndexDelegate(
      UIntPtr agentVisualsPtr,
      int voiceDefinitionIndex,
      float voicePitch);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetAgentLodLevelExternalDelegate(UIntPtr agentVisualsPtr, float level);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetAgentLocalSpeedDelegate(UIntPtr agentVisualsPtr, Vec2 speed);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetLookDirectionDelegate(UIntPtr agentVisualsPtr, Vec3 direction);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ResetDelegate(UIntPtr agentVisualsPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ResetNextFrameDelegate(UIntPtr agentVisualsPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetFrameDelegate(UIntPtr agentVisualsPtr, ref MatrixFrame frame);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void GetFrameDelegate(UIntPtr agentVisualsPtr, ref MatrixFrame outFrame);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void GetGlobalFrameDelegate(UIntPtr agentVisualsPtr, ref MatrixFrame outFrame);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetVisibleDelegate(UIntPtr agentVisualsPtr, [MarshalAs(UnmanagedType.U1)] bool value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool GetVisibleDelegate(UIntPtr agentVisualsPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate NativeObjectPointer GetSkeletonDelegate(
      UIntPtr agentVisualsPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate NativeObjectPointer GetEntityDelegate(UIntPtr agentVisualsPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate Vec3 GetGlobalStableEyePointDelegate(
      UIntPtr agentVisualsPtr,
      [MarshalAs(UnmanagedType.U1)] bool isHumanoid);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate Vec3 GetGlobalStableNeckPointDelegate(
      UIntPtr agentVisualsPtr,
      [MarshalAs(UnmanagedType.U1)] bool isHumanoid);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void AddPrefabToAgentVisualBoneDelegate(
      UIntPtr agentVisualsPtr,
      byte[] prefabName,
      sbyte boneIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate NativeObjectPointer GetAttachedWeaponEntityDelegate(
      UIntPtr agentVisualsPtr,
      int attachedWeaponIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void CreateParticleSystemAttachedToBoneDelegate(
      UIntPtr agentVisualsPtr,
      int runtimeParticleindex,
      sbyte boneIndex,
      ref MatrixFrame boneLocalParticleFrame);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool CheckResourcesDelegate(UIntPtr agentVisualsPtr, [MarshalAs(UnmanagedType.U1)] bool addToQueue);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool AddChildEntityDelegate(UIntPtr agentVisualsPtr, UIntPtr EntityId);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetClothWindToWeaponAtIndexDelegate(
      UIntPtr agentVisualsPtr,
      Vec3 windDirection,
      [MarshalAs(UnmanagedType.U1)] bool isLocal,
      int index);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void RemoveChildEntityDelegate(
      UIntPtr agentVisualsPtr,
      UIntPtr EntityId,
      int removeReason);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void DisableContourDelegate(UIntPtr agentVisualsPtr);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetAsContourEntityDelegate(UIntPtr agentVisualsPtr, uint color);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetContourStateDelegate(UIntPtr agentVisualsPtr, [MarshalAs(UnmanagedType.U1)] bool alwaysVisible);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetEnableOcclusionCullingDelegate(UIntPtr agentVisualsPtr, [MarshalAs(UnmanagedType.U1)] bool enable);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void GetBoneTypeDataDelegate(
      UIntPtr pointer,
      sbyte boneIndex,
      ref BoneBodyTypeData boneBodyTypeData);
  }
}
