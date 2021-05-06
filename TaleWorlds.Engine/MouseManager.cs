// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.MouseManager
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

namespace TaleWorlds.Engine
{
  public static class MouseManager
  {
    public static void ActivateMouseCursor(CursorType mouseId) => EngineApplicationInterface.IMouseManager.ActivateMouseCursor((int) mouseId);

    public static void SetMouseCursor(CursorType mouseId, string mousePath) => EngineApplicationInterface.IMouseManager.SetMouseCursor((int) mouseId, mousePath);

    public static void ShowCursor(bool show) => EngineApplicationInterface.IMouseManager.ShowCursor(show);

    public static void LockCursorAtCurrentPosition(bool lockCursor) => EngineApplicationInterface.IMouseManager.LockCursorAtCurrentPosition(lockCursor);

    public static void LockCursorAtPosition(float x, float y) => EngineApplicationInterface.IMouseManager.LockCursorAtPosition(x, y);

    public static void UnlockCursor() => EngineApplicationInterface.IMouseManager.UnlockCursor();
  }
}
