// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.GenericComparer`1
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
  public class GenericComparer<T> : Comparer<T> where T : IComparable<T>
  {
    public override int Compare(T x, T y) => (object) x != null ? ((object) y != null ? x.CompareTo(y) : 1) : ((object) y != null ? -1 : 0);

    public override bool Equals(object obj) => obj is GenericComparer<T>;

    public override int GetHashCode() => this.GetType().Name.GetHashCode();
  }
}
