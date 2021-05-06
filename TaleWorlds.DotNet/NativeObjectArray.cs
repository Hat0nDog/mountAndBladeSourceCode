// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.NativeObjectArray
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
  [EngineClass("ftdnNative_object_array")]
  public sealed class NativeObjectArray : NativeObject, IEnumerable<NativeObject>, IEnumerable
  {
    internal NativeObjectArray(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static NativeObjectArray Create() => LibraryApplicationInterface.INativeObjectArray.Create();

    public int Count => LibraryApplicationInterface.INativeObjectArray.GetCount(this.Pointer);

    public NativeObject GetElementAt(int index) => LibraryApplicationInterface.INativeObjectArray.GetElementAtIndex(this.Pointer, index);

    public void AddElement(NativeObject nativeObject) => LibraryApplicationInterface.INativeObjectArray.AddElement(this.Pointer, nativeObject != (NativeObject) null ? nativeObject.Pointer : UIntPtr.Zero);

    public void Clear() => LibraryApplicationInterface.INativeObjectArray.Clear(this.Pointer);

    IEnumerator<NativeObject> IEnumerable<NativeObject>.GetEnumerator()
    {
      int count = this.Count;
      for (int i = 0; i < count; ++i)
        yield return this.GetElementAt(i);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      int count = this.Count;
      for (int i = 0; i < count; ++i)
        yield return (object) this.GetElementAt(i);
    }
  }
}
