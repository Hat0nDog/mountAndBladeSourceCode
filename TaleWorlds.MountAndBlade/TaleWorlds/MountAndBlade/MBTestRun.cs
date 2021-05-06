// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBTestRun
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class MBTestRun
  {
    public static bool EnterEditMode() => MBAPI.IMBTestRun.EnterEditMode();

    public static bool NewScene() => MBAPI.IMBTestRun.NewScene();

    public static bool LeaveEditMode() => MBAPI.IMBTestRun.LeaveEditMode();

    public static bool OpenScene(string sceneName) => MBAPI.IMBTestRun.OpenScene(sceneName);

    public static bool CloseScene() => MBAPI.IMBTestRun.CloseScene();

    public static int GetFPS() => MBAPI.IMBTestRun.GetFPS();

    public static void StartMission() => MBAPI.IMBTestRun.StartMission();
  }
}
