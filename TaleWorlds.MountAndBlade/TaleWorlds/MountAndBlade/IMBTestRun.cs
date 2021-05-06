// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBTestRun
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBTestRun
  {
    [EngineMethod("auto_continue", false)]
    int AutoContinue(int type);

    [EngineMethod("get_fps", false)]
    int GetFPS();

    [EngineMethod("enter_edit_mode", false)]
    bool EnterEditMode();

    [EngineMethod("open_scene", false)]
    bool OpenScene(string sceneName);

    [EngineMethod("close_scene", false)]
    bool CloseScene();

    [EngineMethod("leave_edit_mode", false)]
    bool LeaveEditMode();

    [EngineMethod("new_scene", false)]
    bool NewScene();

    [EngineMethod("start_mission", false)]
    void StartMission();
  }
}
