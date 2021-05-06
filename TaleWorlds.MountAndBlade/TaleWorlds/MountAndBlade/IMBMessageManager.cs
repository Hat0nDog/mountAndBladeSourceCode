// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBMessageManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBMessageManager
  {
    [EngineMethod("display_message", false)]
    void DisplayMessage(string message);

    [EngineMethod("display_message_with_color", false)]
    void DisplayMessageWithColor(string message, uint color);

    [EngineMethod("set_message_manager", false)]
    void SetMessageManager(MessageManagerBase messageManager);
  }
}
