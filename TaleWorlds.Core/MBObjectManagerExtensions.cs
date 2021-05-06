// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MBObjectManagerExtensions
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.Core
{
  public static class MBObjectManagerExtensions
  {
    public static void LoadXML(
      this MBObjectManager objectManager,
      string id,
      Type typeOfGameMenusCallbacks = null,
      bool skipXmlFilterForEditor = false)
    {
      Game current = Game.Current;
      bool isDevelopment = false;
      string gameType = "";
      if (current != null)
      {
        isDevelopment = current.GameType.IsDevelopment;
        gameType = current.GameType.GetType().Name;
      }
      objectManager.LoadXML(id, isDevelopment, gameType, typeOfGameMenusCallbacks, skipXmlFilterForEditor);
    }
  }
}
