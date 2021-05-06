// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ObjectInstanceTracker
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.Library
{
  public class ObjectInstanceTracker
  {
    private static Dictionary<string, List<WeakReference>> TrackedInstances = new Dictionary<string, List<WeakReference>>();

    public static void RegisterTrackedInstance(string name, WeakReference instance)
    {
    }

    public static bool CheckBlacklistedTypeCounts(
      Dictionary<string, int> typeNameCounts,
      ref string outputLog)
    {
      bool flag = false;
      foreach (string key in typeNameCounts.Keys)
      {
        int num = 0;
        int typeNameCount = typeNameCounts[key];
        List<WeakReference> weakReferenceList;
        if (ObjectInstanceTracker.TrackedInstances.TryGetValue(key, out weakReferenceList))
        {
          foreach (WeakReference weakReference in weakReferenceList)
          {
            if (weakReference.Target != null)
              ++num;
          }
        }
        if (num != typeNameCount)
        {
          flag = true;
          outputLog = outputLog + "Type(" + key + ") has " + (object) num + " alive instance, but its should be " + (object) typeNameCount + "\n";
        }
      }
      return flag;
    }
  }
}
