// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.IManaged
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
  [LibraryInterfaceBase]
  internal interface IManaged
  {
    [EngineMethod("increase_reference_count", false)]
    void IncreaseReferenceCount(UIntPtr ptr);

    [EngineMethod("decrease_reference_count", false)]
    void DecreaseReferenceCount(UIntPtr ptr);

    [EngineMethod("get_class_type_definition_count", false)]
    int GetClassTypeDefinitionCount();

    [EngineMethod("get_class_type_definition", false)]
    void GetClassTypeDefinition(
      int index,
      ref EngineClassTypeDefinition engineClassTypeDefinition);

    [EngineMethod("release_managed_object", false)]
    void ReleaseManagedObject(UIntPtr ptr);
  }
}
