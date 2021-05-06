// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBGarbageDeleteQueue
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  internal static class MBGarbageDeleteQueue
  {
    internal static List<UIntPtr> DeletedRenderScenes { get; private set; }

    static MBGarbageDeleteQueue() => MBGarbageDeleteQueue.DeletedRenderScenes = new List<UIntPtr>();

    internal static void AddGarbageRenderScene(UIntPtr pointer)
    {
      lock (MBGarbageDeleteQueue.DeletedRenderScenes)
      {
        if (MBGarbageDeleteQueue.DeletedRenderScenes.Contains(pointer))
          return;
        MBGarbageDeleteQueue.DeletedRenderScenes.Add(pointer);
      }
    }

    internal static void Tick()
    {
    }
  }
}
