// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.INativeArray
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.DotNet
{
  [LibraryInterfaceBase]
  internal interface INativeArray
  {
    [EngineMethod("get_data_pointer_offset", false)]
    int GetDataPointerOffset();

    [EngineMethod("create", false)]
    NativeArray Create();

    [EngineMethod("get_data_size", false)]
    int GetDataSize(UIntPtr pointer);

    [EngineMethod("get_data_pointer", false)]
    UIntPtr GetDataPointer(UIntPtr pointer);

    [EngineMethod("add_integer_element", false)]
    void AddIntegerElement(UIntPtr pointer, int value);

    [EngineMethod("add_float_element", false)]
    void AddFloatElement(UIntPtr pointer, float value);

    [EngineMethod("add_element", false)]
    void AddElement(UIntPtr pointer, IntPtr element, int elementSize);

    [EngineMethod("clear", false)]
    void Clear(UIntPtr pointer);
  }
}
