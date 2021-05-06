// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IPhysicsShape
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IPhysicsShape
  {
    [EngineMethod("get_from_resource", false)]
    PhysicsShape GetFromResource(string bodyName, bool mayReturnNull);

    [EngineMethod("preload_with_name", false)]
    void PreloadWithName(string bodyName);

    [EngineMethod("create_body_copy", false)]
    PhysicsShape CreateBodyCopy(UIntPtr bodyPointer);

    [EngineMethod("get_name", false)]
    string GetName(PhysicsShape shape);

    [EngineMethod("triangle_mesh_count", false)]
    int TriangleMeshCount(UIntPtr pointer);

    [EngineMethod("triangle_count_in_triangle_mesh", false)]
    int TriangleCountInTriangleMesh(UIntPtr pointer, int meshIndex);

    [EngineMethod("get_dominant_material_index_for_mesh_at_index", false)]
    int GetDominantMaterialForTriangleMesh(PhysicsShape shape, int meshIndex);

    [EngineMethod("get_triangle", false)]
    void GetTriangle(UIntPtr pointer, Vec3[] data, int meshIndex, int triangleIndex);

    [EngineMethod("sphere_count", false)]
    int SphereCount(UIntPtr pointer);

    [EngineMethod("get_sphere", false)]
    void GetSphere(UIntPtr shapePointer, ref SphereData data, int sphereIndex);

    [EngineMethod("get_sphere_with_material", false)]
    void GetSphereWithMaterial(
      UIntPtr shapePointer,
      ref SphereData data,
      ref int materialIndex,
      int sphereIndex);

    [EngineMethod("capsule_count", false)]
    int CapsuleCount(UIntPtr shapePointer);

    [EngineMethod("add_capsule", false)]
    void AddCapsule(UIntPtr shapePointer, ref CapsuleData data);

    [EngineMethod("add_sphere", false)]
    void AddSphere(UIntPtr shapePointer, ref Vec3 origin, float radius);

    [EngineMethod("set_capsule", false)]
    void SetCapsule(UIntPtr shapePointer, ref CapsuleData data, int index);

    [EngineMethod("get_capsule", false)]
    void GetCapsule(UIntPtr shapePointer, ref CapsuleData data, int index);

    [EngineMethod("get_capsule_with_material", false)]
    void GetCapsuleWithMaterial(
      UIntPtr shapePointer,
      ref CapsuleData data,
      ref int materialIndex,
      int index);

    [EngineMethod("clear", false)]
    void clear(UIntPtr shapePointer);

    [EngineMethod("transform", false)]
    void Transform(UIntPtr shapePointer, ref MatrixFrame frame);
  }
}
