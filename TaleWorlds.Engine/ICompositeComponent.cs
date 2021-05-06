// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ICompositeComponent
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface ICompositeComponent
  {
    [EngineMethod("create_composite_component", false)]
    CompositeComponent CreateCompositeComponent();

    [EngineMethod("set_material", false)]
    void SetMaterial(UIntPtr compositeComponentPointer, UIntPtr materialPointer);

    [EngineMethod("create_copy", false)]
    CompositeComponent CreateCopy(UIntPtr pointer);

    [EngineMethod("add_component", false)]
    void AddComponent(UIntPtr pointer, UIntPtr componentPointer);

    [EngineMethod("add_prefab_entity", false)]
    void AddPrefabEntity(UIntPtr pointer, UIntPtr scenePointer, string prefabName);

    [EngineMethod("release", false)]
    void Release(UIntPtr compositeComponentPointer);

    [EngineMethod("get_factor_1", false)]
    uint GetFactor1(UIntPtr compositeComponentPointer);

    [EngineMethod("get_factor_2", false)]
    uint GetFactor2(UIntPtr compositeComponentPointer);

    [EngineMethod("set_factor_1", false)]
    void SetFactor1(UIntPtr compositeComponentPointer, uint factorColor1);

    [EngineMethod("set_factor_2", false)]
    void SetFactor2(UIntPtr compositeComponentPointer, uint factorColor2);

    [EngineMethod("set_vector_argument", false)]
    void SetVectorArgument(
      UIntPtr compositeComponentPointer,
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3);

    [EngineMethod("get_frame", false)]
    void GetFrame(UIntPtr compositeComponentPointer, ref MatrixFrame outFrame);

    [EngineMethod("set_frame", false)]
    void SetFrame(UIntPtr compositeComponentPointer, ref MatrixFrame meshFrame);

    [EngineMethod("get_vector_user_data", false)]
    Vec3 GetVectorUserData(UIntPtr compositeComponentPointer);

    [EngineMethod("set_vector_user_data", false)]
    void SetVectorUserData(UIntPtr compositeComponentPointer, ref Vec3 vectorArg);

    [EngineMethod("get_bounding_box", false)]
    void GetBoundingBox(UIntPtr compositeComponentPointer, ref BoundingBox outBoundingBox);

    [EngineMethod("set_visibility_mask", false)]
    void SetVisibilityMask(UIntPtr compositeComponentPointer, VisibilityMaskFlags visibilityMask);

    [EngineMethod("get_first_meta_mesh", false)]
    MetaMesh GetFirstMetaMesh(UIntPtr compositeComponentPointer);

    [EngineMethod("add_multi_mesh", false)]
    void AddMultiMesh(UIntPtr compositeComponentPointer, string multiMeshName);

    [EngineMethod("is_visible", false)]
    bool IsVisible(UIntPtr compositeComponentPointer);

    [EngineMethod("set_visible", false)]
    void SetVisible(UIntPtr compositeComponentPointer, bool visible);
  }
}
