// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.INativeObjectArray
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
  [LibraryInterfaceBase]
  internal interface INativeObjectArray
  {
    [EngineMethod("create", false)]
    NativeObjectArray Create();

    [EngineMethod("get_count", false)]
    int GetCount(UIntPtr pointer);

    [EngineMethod("add_element", false)]
    void AddElement(UIntPtr pointer, UIntPtr nativeObject);

    [EngineMethod("get_element_at_index", false)]
    NativeObject GetElementAtIndex(UIntPtr pointer, int index);

    [EngineMethod("clear", false)]
    void Clear(UIntPtr pointer);
  }
}
