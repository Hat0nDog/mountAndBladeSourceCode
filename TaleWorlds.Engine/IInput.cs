// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IInput
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IInput
  {
    [EngineMethod("clear_keys", false)]
    void ClearKeys();

    [EngineMethod("get_mouse_sensitivity", false)]
    float GetMouseSensitivity();

    [EngineMethod("get_mouse_delta_z", false)]
    float GetMouseDeltaZ();

    [EngineMethod("is_mouse_active", false)]
    bool IsMouseActive();

    [EngineMethod("is_controller_connected", false)]
    bool IsControllerConnected();

    [EngineMethod("vibrate_controller_motors", false)]
    void VibrateControllerMotors(float leftMotor, float rightMotor, float duration);

    [EngineMethod("vibrate_controller_triggers", false)]
    void VibrateControllerTriggers(float leftTrigger, float rightTrigger, float duration);

    [EngineMethod("press_key", false)]
    void PressKey(InputKey key);

    [EngineMethod("get_virtual_key_code", false)]
    int GetVirtualKeyCode(InputKey key);

    [EngineMethod("set_clipboard_text", false)]
    void SetClipboardText(string text);

    [EngineMethod("get_clipboard_text", false)]
    string GetClipboardText();

    [EngineMethod("update_key_data", false)]
    void UpdateKeyData(byte[] keyData);

    [EngineMethod("get_mouse_move_x", false)]
    float GetMouseMoveX();

    [EngineMethod("get_mouse_move_y", false)]
    float GetMouseMoveY();

    [EngineMethod("get_mouse_position_x", false)]
    float GetMousePositionX();

    [EngineMethod("get_mouse_position_y", false)]
    float GetMousePositionY();

    [EngineMethod("get_mouse_scroll_value", false)]
    float GetMouseScrollValue();

    [EngineMethod("get_key_state", false)]
    Vec2 GetKeyState(InputKey key);

    [EngineMethod("is_key_down", false)]
    bool IsKeyDown(InputKey key);

    [EngineMethod("is_key_down_immediate", false)]
    bool IsKeyDownImmediate(InputKey key);

    [EngineMethod("is_key_pressed", false)]
    bool IsKeyPressed(InputKey key);

    [EngineMethod("is_key_released", false)]
    bool IsKeyReleased(InputKey key);

    [EngineMethod("set_cursor_position", false)]
    void SetCursorPosition(int x, int y);
  }
}
