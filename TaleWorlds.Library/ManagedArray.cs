﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ManagedArray
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  [Serializable]
  public struct ManagedArray
  {
    internal IntPtr Array;
    internal int Length;

    public ManagedArray(IntPtr array, int length)
    {
      this.Array = array;
      this.Length = length;
    }
  }
}
