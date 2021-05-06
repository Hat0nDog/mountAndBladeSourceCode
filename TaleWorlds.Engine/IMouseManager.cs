// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IMouseManager
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IMouseManager
  {
    [EngineMethod("activate_mouse_cursor", false)]
    void ActivateMouseCursor(int id);

    [EngineMethod("set_mouse_cursor", false)]
    void SetMouseCursor(int id, string mousePath);

    [EngineMethod("show_cursor", false)]
    void ShowCursor(bool show);

    [EngineMethod("lock_cursor_at_current_pos", false)]
    void LockCursorAtCurrentPosition(bool lockCursor);

    [EngineMethod("lock_cursor_at_position", false)]
    void LockCursorAtPosition(float x, float y);

    [EngineMethod("unlock_cursor", false)]
    void UnlockCursor();
  }
}
