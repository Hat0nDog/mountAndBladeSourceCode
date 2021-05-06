// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IPhysicsMaterial
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IPhysicsMaterial
  {
    [EngineMethod("get_index_with_name", false)]
    PhysicsMaterial GetIndexWithName(string materialName);

    [EngineMethod("get_material_name_at_index", false)]
    string GetMaterialNameAtIndex(int index);

    [EngineMethod("get_material_flags_at_index", false)]
    PhysicsMaterialFlags GetFlagsAtIndex(int index);

    [EngineMethod("get_restitution_at_index", false)]
    float GetRestitutionAtIndex(int index);

    [EngineMethod("get_dynamic_friction_at_index", false)]
    float GetDynamicFrictionAtIndex(int index);

    [EngineMethod("get_static_friction_at_index", false)]
    float GetStaticFrictionAtIndex(int index);

    [EngineMethod("get_softness_at_index", false)]
    float GetSoftnessAtIndex(int index);
  }
}
