// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.InputSystem.EngineInputManager
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.Engine.InputSystem
{
  public class EngineInputManager : IInputManager
  {
    float IInputManager.GetMousePositionX() => EngineApplicationInterface.IInput.GetMousePositionX();

    float IInputManager.GetMousePositionY() => EngineApplicationInterface.IInput.GetMousePositionY();

    float IInputManager.GetMouseScrollValue() => EngineApplicationInterface.IInput.GetMouseScrollValue();

    bool IInputManager.IsMouseActive() => EngineApplicationInterface.IInput.IsMouseActive();

    bool IInputManager.IsControllerConnected() => EngineApplicationInterface.IInput.IsControllerConnected();

    void IInputManager.PressKey(InputKey key) => EngineApplicationInterface.IInput.PressKey(key);

    void IInputManager.ClearKeys() => EngineApplicationInterface.IInput.ClearKeys();

    int IInputManager.GetVirtualKeyCode(InputKey key) => EngineApplicationInterface.IInput.GetVirtualKeyCode(key);

    void IInputManager.SetClipboardText(string text) => EngineApplicationInterface.IInput.SetClipboardText(text);

    string IInputManager.GetClipboardText() => EngineApplicationInterface.IInput.GetClipboardText();

    float IInputManager.GetMouseMoveX() => EngineApplicationInterface.IInput.GetMouseMoveX();

    float IInputManager.GetMouseMoveY() => EngineApplicationInterface.IInput.GetMouseMoveY();

    float IInputManager.GetMouseSensitivity() => EngineApplicationInterface.IInput.GetMouseSensitivity();

    float IInputManager.GetMouseDeltaZ() => EngineApplicationInterface.IInput.GetMouseDeltaZ();

    void IInputManager.UpdateKeyData(byte[] keyData) => EngineApplicationInterface.IInput.UpdateKeyData(keyData);

    Vec2 IInputManager.GetKeyState(InputKey key) => EngineApplicationInterface.IInput.GetKeyState(key);

    bool IInputManager.IsKeyPressed(InputKey key) => EngineApplicationInterface.IInput.IsKeyPressed(key);

    bool IInputManager.IsKeyDown(InputKey key) => EngineApplicationInterface.IInput.IsKeyDown(key);

    bool IInputManager.IsKeyDownImmediate(InputKey key) => EngineApplicationInterface.IInput.IsKeyDownImmediate(key);

    bool IInputManager.IsKeyReleased(InputKey key) => EngineApplicationInterface.IInput.IsKeyReleased(key);

    Vec2 IInputManager.GetResolution() => Screen.RealScreenResolution;

    void IInputManager.SetCursorPosition(int x, int y) => EngineApplicationInterface.IInput.SetCursorPosition(x, y);
  }
}
