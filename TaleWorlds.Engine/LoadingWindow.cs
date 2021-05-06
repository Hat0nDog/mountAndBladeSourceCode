// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.LoadingWindow
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

namespace TaleWorlds.Engine
{
  public static class LoadingWindow
  {
    public static bool _loadingWindowState;
    private static ILoadingWindowManager _loadingWindowManager;

    public static bool DisableLoadingScreenAutoEnd { get; private set; }

    public static void Initialize(ILoadingWindowManager loadingWindowManager) => LoadingWindow._loadingWindowManager = loadingWindowManager;

    public static void Destroy()
    {
      if (LoadingWindow._loadingWindowState)
        LoadingWindow.DisableGlobalLoadingWindow();
      LoadingWindow._loadingWindowManager = (ILoadingWindowManager) null;
    }

    public static void DisableGlobalLoadingWindow()
    {
      if (LoadingWindow._loadingWindowManager == null)
        return;
      if (LoadingWindow._loadingWindowState)
      {
        LoadingWindow._loadingWindowManager.DisableLoadingWindow();
        Utilities.DisableGlobalLoadingWindow();
      }
      LoadingWindow.DisableLoadingScreenAutoEnd = false;
      LoadingWindow._loadingWindowState = false;
    }

    public static bool GetGlobalLoadingWindowState() => LoadingWindow._loadingWindowState;

    public static void EnableGlobalLoadingWindow(bool disableLoadingScreenAutoEnd = false)
    {
      if (LoadingWindow._loadingWindowManager == null)
        return;
      LoadingWindow.DisableLoadingScreenAutoEnd = disableLoadingScreenAutoEnd;
      LoadingWindow._loadingWindowState = true;
      if (!LoadingWindow._loadingWindowState)
        return;
      LoadingWindow._loadingWindowManager.EnableLoadingWindow();
      Utilities.EnableGlobalLoadingWindow();
    }
  }
}
