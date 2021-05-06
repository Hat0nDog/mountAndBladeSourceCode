// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.PhysicsShape
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglPhysics_shape")]
  public sealed class PhysicsShape : Resource
  {
    public static PhysicsShape GetFromResource(string bodyName, bool mayReturnNull = false) => EngineApplicationInterface.IPhysicsShape.GetFromResource(bodyName, mayReturnNull);

    public static void PreloadWithName(string bodyName) => EngineApplicationInterface.IPhysicsShape.PreloadWithName(bodyName);

    internal PhysicsShape(UIntPtr bodyPointer)
      : base(bodyPointer)
    {
    }

    public PhysicsShape CreateCopy() => EngineApplicationInterface.IPhysicsShape.CreateBodyCopy(this.Pointer);

    public int SphereCount() => EngineApplicationInterface.IPhysicsShape.SphereCount(this.Pointer);

    public void GetSphere(ref SphereData data, int index) => EngineApplicationInterface.IPhysicsShape.GetSphere(this.Pointer, ref data, index);

    public void GetSphere(ref SphereData data, out PhysicsMaterial material, int index)
    {
      int materialIndex = -1;
      EngineApplicationInterface.IPhysicsShape.GetSphereWithMaterial(this.Pointer, ref data, ref materialIndex, index);
      material = new PhysicsMaterial(materialIndex);
    }

    public PhysicsMaterial GetDominantMaterialForTriangleMesh(int meshIndex) => new PhysicsMaterial(EngineApplicationInterface.IPhysicsShape.GetDominantMaterialForTriangleMesh(this, meshIndex));

    public string GetName() => EngineApplicationInterface.IPhysicsShape.GetName(this);

    public int TriangleMeshCount() => EngineApplicationInterface.IPhysicsShape.TriangleMeshCount(this.Pointer);

    public int TriangleCountInTriangleMesh(int meshIndex) => EngineApplicationInterface.IPhysicsShape.TriangleCountInTriangleMesh(this.Pointer, meshIndex);

    public void GetTriangle(Vec3[] triangle, int meshIndex, int triangleIndex) => EngineApplicationInterface.IPhysicsShape.GetTriangle(this.Pointer, triangle, meshIndex, triangleIndex);

    public int CapsuleCount() => EngineApplicationInterface.IPhysicsShape.CapsuleCount(this.Pointer);

    public void AddCapsule(CapsuleData data) => EngineApplicationInterface.IPhysicsShape.AddCapsule(this.Pointer, ref data);

    public void AddSphere(SphereData data) => EngineApplicationInterface.IPhysicsShape.AddSphere(this.Pointer, ref data.Origin, data.Radius);

    public void SetCapsule(CapsuleData data, int index) => EngineApplicationInterface.IPhysicsShape.SetCapsule(this.Pointer, ref data, index);

    public void GetCapsule(ref CapsuleData data, int index) => EngineApplicationInterface.IPhysicsShape.GetCapsule(this.Pointer, ref data, index);

    public void GetCapsule(ref CapsuleData data, out PhysicsMaterial material, int index)
    {
      int materialIndex = -1;
      EngineApplicationInterface.IPhysicsShape.GetCapsuleWithMaterial(this.Pointer, ref data, ref materialIndex, index);
      material = new PhysicsMaterial(materialIndex);
    }

    public void Transform(ref MatrixFrame frame) => EngineApplicationInterface.IPhysicsShape.Transform(this.Pointer, ref frame);

    public void Clear() => EngineApplicationInterface.IPhysicsShape.clear(this.Pointer);
  }
}
