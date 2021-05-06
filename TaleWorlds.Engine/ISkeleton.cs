// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ISkeleton
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface ISkeleton
  {
    [EngineMethod("create_from_model", false)]
    Skeleton CreateFromModel(string skeletonModelName);

    [EngineMethod("create_from_model_with_null_anim_tree", false)]
    Skeleton CreateFromModelWithNullAnimTree(
      UIntPtr entityPointer,
      string skeletonModelName,
      float scale);

    [EngineMethod("freeze", false)]
    void Freeze(UIntPtr skeletonPointer, bool isFrozen);

    [EngineMethod("is_frozen", false)]
    bool IsFrozen(UIntPtr skeletonPointer);

    [EngineMethod("add_mesh_to_bone", false)]
    void AddMeshToBone(UIntPtr skeletonPointer, UIntPtr multiMeshPointer, sbyte bone_index);

    [EngineMethod("get_bone_child_count", false)]
    byte GetBoneChildCount(Skeleton skeleton, byte boneIndex);

    [EngineMethod("get_bone_child_at_index", false)]
    byte GetBoneChildAtIndex(Skeleton skeleton, byte boneIndex, byte childIndex);

    [EngineMethod("get_bone_name", false)]
    string GetBoneName(Skeleton skeleton, int boneIndex);

    [EngineMethod("get_name", false)]
    string GetName(Skeleton skeleton);

    [EngineMethod("get_parent_bone_index", false)]
    sbyte GetParentBoneIndex(Skeleton skeleton, int boneIndex);

    [EngineMethod("set_bone_local_frame", false)]
    void SetBoneLocalFrame(UIntPtr skeletonPointer, int boneIndex, ref MatrixFrame localFrame);

    [EngineMethod("get_bone_count", false)]
    byte GetBoneCount(UIntPtr skeletonPointer);

    [EngineMethod("force_update_bone_frames", false)]
    void ForceUpdateBoneFrames(UIntPtr skeletonPointer);

    [EngineMethod("get_bone_entitial_frame_with_index", false)]
    void GetBoneEntitialFrameWithIndex(
      UIntPtr skeletonPointer,
      byte boneIndex,
      ref MatrixFrame outEntitialFrame);

    [EngineMethod("get_bone_entitial_frame_with_name", false)]
    void GetBoneEntitialFrameWithName(
      UIntPtr skeletonPointer,
      string boneName,
      ref MatrixFrame outEntitialFrame);

    [EngineMethod("add_prefab_entity_to_bone", false)]
    void AddPrefabEntityToBone(UIntPtr skeletonPointer, string prefab_name, sbyte bone_id);

    [EngineMethod("get_skeleton_bone_mapping", false)]
    sbyte GetSkeletonBoneMapping(UIntPtr skeletonPointer, sbyte boneIndex);

    [EngineMethod("add_mesh", false)]
    void AddMesh(UIntPtr skeletonPointer, UIntPtr mesnPointer);

    [EngineMethod("clear_meshes", false)]
    void ClearMeshes(UIntPtr skeletonPointer, bool clearBoneComponents);

    [EngineMethod("get_bone_body", false)]
    void GetBoneBody(UIntPtr skeletonPointer, sbyte boneIndex, ref CapsuleData data);

    [EngineMethod("get_current_ragdoll_state", false)]
    RagdollState GetCurrentRagdollState(UIntPtr skeletonPointer);

    [EngineMethod("activate_ragdoll", false)]
    void ActivateRagdoll(UIntPtr skeletonPointer);

    [EngineMethod("skeleton_model_exist", false)]
    bool SkeletonModelExist(string skeletonModelName);

    [EngineMethod("get_component_at_index", false)]
    GameEntityComponent GetComponentAtIndex(
      UIntPtr skeletonPointer,
      GameEntity.ComponentType componentType,
      int index);

    [EngineMethod("get_bone_entitial_frame", false)]
    void GetBoneEntitialFrame(UIntPtr skeletonPointer, int boneIndex, ref MatrixFrame outFrame);

    [EngineMethod("get_bone_component_count", false)]
    int GetBoneComponentCount(UIntPtr skeletonPointer, int boneIndex);

    [EngineMethod("add_component_to_bone", false)]
    void AddComponentToBone(UIntPtr skeletonPointer, int boneIndex, GameEntityComponent component);

    [EngineMethod("get_bone_component_at_index", false)]
    GameEntityComponent GetBoneComponentAtIndex(
      UIntPtr skeletonPointer,
      int boneIndex,
      int componentIndex);

    [EngineMethod("has_bone_component", false)]
    bool HasBoneComponent(UIntPtr skeletonPointer, int boneIndex, GameEntityComponent component);

    [EngineMethod("remove_bone_component", false)]
    void RemoveBoneComponent(UIntPtr skeletonPointer, int boneIndex, GameEntityComponent component);

    [EngineMethod("clear_meshes_at_bone", false)]
    void ClearMeshesAtBone(UIntPtr skeletonPointer, int boneIndex);

    [EngineMethod("get_component_count", false)]
    int GetComponentCount(UIntPtr skeletonPointer, GameEntity.ComponentType componentType);

    [EngineMethod("set_use_precise_bounding_volume", false)]
    void SetUsePreciseBoundingVolume(UIntPtr skeletonPointer, bool value);

    [EngineMethod("tick_animations", false)]
    void TickAnimations(
      UIntPtr skeletonPointer,
      ref MatrixFrame globalFrame,
      float dt,
      bool tickAnimsForChildren);

    [EngineMethod("tick_animations_and_force_update", false)]
    void TickAnimationsAndForceUpdate(
      UIntPtr skeletonPointer,
      ref MatrixFrame globalFrame,
      float dt,
      bool tickAnimsForChildren);

    [EngineMethod("get_skeleton_animation_parameter_at_channel", false)]
    float GetSkeletonAnimationParameterAtChannel(UIntPtr skeletonPointer, int channelNo);

    [EngineMethod("set_skeleton_animation_parameter_at_channel", false)]
    void SetSkeletonAnimationParameterAtChannel(
      UIntPtr skeletonPointer,
      int channelNo,
      float parameter);

    [EngineMethod("get_skeleton_animation_speed_at_channel", false)]
    float GetSkeletonAnimationSpeedAtChannel(UIntPtr skeletonPointer, int channelNo);

    [EngineMethod("set_up_to_date", false)]
    void SetSkeletonUptoDate(UIntPtr skeletonPointer, bool value);

    [EngineMethod("get_bone_entitial_rest_frame", false)]
    void GetBoneEntitialRestFrame(
      UIntPtr skeletonPointer,
      sbyte bone,
      bool useBoneMapping,
      ref MatrixFrame outFrame);

    [EngineMethod("get_bone_local_rest_frame", false)]
    void GetBoneLocalRestFrame(
      UIntPtr skeletonPointer,
      sbyte bone,
      bool useBoneMapping,
      ref MatrixFrame outFrame);

    [EngineMethod("get_bone_entitial_frame_at_channel", false)]
    void GetBoneEntitialFrameAtChannel(
      UIntPtr skeletonPointer,
      int channelNo,
      sbyte bone,
      ref MatrixFrame outFrame);

    [EngineMethod("get_animation_at_channel", false)]
    string GetAnimationAtChannel(UIntPtr skeletonPointer, int channelNo);

    [EngineMethod("reset_cloths", false)]
    void ResetCloths(UIntPtr skeletonPointer);

    [EngineMethod("reset_frames", false)]
    void ResetFrames(UIntPtr skeletonPointer);

    [EngineMethod("clear_components", false)]
    void ClearComponents(UIntPtr skeletonPointer);

    [EngineMethod("add_component", false)]
    void AddComponent(UIntPtr skeletonPointer, UIntPtr componentPointer);

    [EngineMethod("has_component", false)]
    bool HasComponent(UIntPtr skeletonPointer, UIntPtr componentPointer);

    [EngineMethod("remove_component", false)]
    void RemoveComponent(UIntPtr SkeletonPointer, UIntPtr componentPointer);

    [EngineMethod("update_entitial_frames_from_local_frames", false)]
    void UpdateEntitialFramesFromLocalFrames(UIntPtr skeletonPointer);

    [EngineMethod("get_all_meshes", false)]
    void GetAllMeshes(Skeleton skeleton, NativeObjectArray nativeObjectArray);

    [EngineMethod("get_bone_index_from_name", false)]
    sbyte GetBoneIndexFromName(string skeletonModelName, string boneName);
  }
}
