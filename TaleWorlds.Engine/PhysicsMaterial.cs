// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.PhysicsMaterial
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineStruct("int")]
  public struct PhysicsMaterial
  {
    public readonly int Index;
    public static readonly PhysicsMaterial InvalidPhysicsMaterial = new PhysicsMaterial(-1);

    internal PhysicsMaterial(int index)
      : this()
    {
      this.Index = index;
    }

    public bool IsValid => this.Index >= 0;

    public PhysicsMaterialFlags GetFlags() => PhysicsMaterial.GetFlagsAtIndex(this.Index);

    public float GetDynamicFriction() => PhysicsMaterial.GetDynamicFrictionAtIndex(this.Index);

    public float GetStaticFriction() => PhysicsMaterial.GetStaticFrictionAtIndex(this.Index);

    public float GetSoftness() => PhysicsMaterial.GetSoftnessAtIndex(this.Index);

    public float GetRestitution() => PhysicsMaterial.GetRestitutionAtIndex(this.Index);

    public string Name => PhysicsMaterial.GetNameAtIndex(this.Index);

    public bool Equals(PhysicsMaterial m) => this.Index == m.Index;

    public static PhysicsMaterial GetFromName(string id) => EngineApplicationInterface.IPhysicsMaterial.GetIndexWithName(id);

    public static string GetNameAtIndex(int index) => EngineApplicationInterface.IPhysicsMaterial.GetMaterialNameAtIndex(index);

    public static PhysicsMaterialFlags GetFlagsAtIndex(int index) => EngineApplicationInterface.IPhysicsMaterial.GetFlagsAtIndex(index);

    public static float GetRestitutionAtIndex(int index) => EngineApplicationInterface.IPhysicsMaterial.GetRestitutionAtIndex(index);

    public static float GetSoftnessAtIndex(int index) => EngineApplicationInterface.IPhysicsMaterial.GetSoftnessAtIndex(index);

    public static float GetDynamicFrictionAtIndex(int index) => EngineApplicationInterface.IPhysicsMaterial.GetDynamicFrictionAtIndex(index);

    public static float GetStaticFrictionAtIndex(int index) => EngineApplicationInterface.IPhysicsMaterial.GetStaticFrictionAtIndex(index);

    public static PhysicsMaterial GetFromIndex(int index) => new PhysicsMaterial(index);
  }
}
