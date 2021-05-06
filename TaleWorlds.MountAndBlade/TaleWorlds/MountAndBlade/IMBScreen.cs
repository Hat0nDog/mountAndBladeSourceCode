// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBScreen
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBScreen
  {
    [EngineMethod("on_exit_button_click", false)]
    void OnExitButtonClick();

    [EngineMethod("on_edit_mode_enter_press", false)]
    void OnEditModeEnterPress();

    [EngineMethod("on_edit_mode_enter_release", false)]
    void OnEditModeEnterRelease();
  }
}
