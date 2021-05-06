// Decompiled with JetBrains decompiler
// Type: TaleWorlds.DotNet.NativeArrayEnumerator`1
// Assembly: TaleWorlds.DotNet, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 207BAA99-20DA-4442-9622-4DB0CDEF3C0E
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.DotNet.dll

using System.Collections;
using System.Collections.Generic;

namespace TaleWorlds.DotNet
{
  public sealed class NativeArrayEnumerator<T> : 
    IReadOnlyList<T>,
    IEnumerable<T>,
    IEnumerable,
    IReadOnlyCollection<T>
    where T : struct
  {
    private NativeArray _nativeArray;

    public NativeArrayEnumerator(NativeArray nativeArray) => this._nativeArray = nativeArray;

    public T this[int index] => this._nativeArray.GetElementAt<T>(index);

    public int Count => this._nativeArray.GetLength<T>();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => this._nativeArray.GetEnumerator<T>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._nativeArray.GetEnumerator<T>().GetEnumerator();
  }
}
