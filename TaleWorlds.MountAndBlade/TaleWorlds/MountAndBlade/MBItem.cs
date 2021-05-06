// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBItem
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public static class MBItem
  {
    public static int GetItemUsageIndex(string itemUsageName) => MBAPI.IMBItem.GetItemUsageIndex(itemUsageName);

    public static int GetItemHolsterIndex(string itemHolsterName) => MBAPI.IMBItem.GetItemHolsterIndex(itemHolsterName);

    public static bool GetItemIsPassiveUsage(string itemUsageName) => MBAPI.IMBItem.GetItemIsPassiveUsage(itemUsageName);

    public static MatrixFrame GetHolsterFrameByIndex(int index)
    {
      MatrixFrame outFrame = new MatrixFrame();
      MBAPI.IMBItem.GetHolsterFrameByIndex(index, ref outFrame);
      return outFrame;
    }

    public static ItemObject.ItemUsageSetFlags GetItemUsageSetFlags(string ItemUsageName) => (ItemObject.ItemUsageSetFlags) MBAPI.IMBItem.GetItemUsageSetFlags(ItemUsageName);

    public static ActionIndexCache GetItemUsageReloadActionCode(
      string itemUsageName,
      int usageDirection,
      bool isMounted,
      int leftHandUsageSetIndex,
      bool isLeftStance)
    {
      return new ActionIndexCache(MBAPI.IMBItem.GetItemUsageReloadActionCode(itemUsageName, usageDirection, isMounted, leftHandUsageSetIndex, isLeftStance));
    }

    public static int GetItemUsageStrikeType(
      string itemUsageName,
      int usageDirection,
      bool isMounted,
      int leftHandUsageSetIndex,
      bool isLeftStance)
    {
      return MBAPI.IMBItem.GetItemUsageStrikeType(itemUsageName, usageDirection, isMounted, leftHandUsageSetIndex, isLeftStance);
    }

    public static float GetMissileRange(float shotSpeed, float zDiff) => MBAPI.IMBItem.GetMissileRange(shotSpeed, zDiff);
  }
}
