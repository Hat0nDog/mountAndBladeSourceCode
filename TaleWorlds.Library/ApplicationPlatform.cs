// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ApplicationPlatform
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public static class ApplicationPlatform
  {
    public static Platform CurrentPlatform { get; private set; }

    public static Runtime CurrentRuntimeLibrary { get; private set; }

    public static void Initialize(Platform currentPlatform, Runtime currentRuntimeLibrary)
    {
      ApplicationPlatform.CurrentPlatform = currentPlatform;
      ApplicationPlatform.CurrentRuntimeLibrary = currentRuntimeLibrary;
    }

    public static bool IsPlatformWindows() => ApplicationPlatform.CurrentPlatform == Platform.WindowsEpic || ApplicationPlatform.CurrentPlatform == Platform.WindowsNoPlatform || (ApplicationPlatform.CurrentPlatform == Platform.WindowsSteam || ApplicationPlatform.CurrentPlatform == Platform.WindowsGOG) || ApplicationPlatform.CurrentPlatform == Platform.GDKDesktop;
  }
}
