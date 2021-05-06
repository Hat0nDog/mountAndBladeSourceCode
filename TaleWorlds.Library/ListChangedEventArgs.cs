// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ListChangedEventArgs
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;

namespace TaleWorlds.Library
{
  public class ListChangedEventArgs : EventArgs
  {
    public ListChangedEventArgs(ListChangedType listChangedType, int newIndex)
    {
      this.ListChangedType = listChangedType;
      this.NewIndex = newIndex;
      this.OldIndex = -1;
    }

    public ListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex)
    {
      this.ListChangedType = listChangedType;
      this.NewIndex = newIndex;
      this.OldIndex = oldIndex;
    }

    public ListChangedType ListChangedType { get; private set; }

    public int NewIndex { get; private set; }

    public int OldIndex { get; private set; }
  }
}
