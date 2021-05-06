// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ItemCollectionElementMissionExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public static class ItemCollectionElementMissionExtensions
  {
    public static StackArray.StackArray4Int GetItemHolsterIndices(this ItemObject item)
    {
      StackArray.StackArray4Int stackArray4Int = new StackArray.StackArray4Int();
      for (int index = 0; index < item.ItemHolsters.Length; ++index)
        stackArray4Int[index] = item.ItemHolsters[index].Length > 0 ? MBItem.GetItemHolsterIndex(item.ItemHolsters[index]) : -1;
      for (int length = item.ItemHolsters.Length; length < 4; ++length)
        stackArray4Int[length] = -1;
      return stackArray4Int;
    }
  }
}
