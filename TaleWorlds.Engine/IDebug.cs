// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IDebug
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IDebug
  {
    [EngineMethod("write_debug_line_on_screen", false)]
    void WriteDebugLineOnScreen(string line);

    [EngineMethod("abort_game", false)]
    void AbortGame(int ExitCode);

    [EngineMethod("assert_memory_usage", false)]
    void AssertMemoryUsage(int memoryMB);

    [EngineMethod("write_line", false)]
    void WriteLine(int logLevel, string line, int color, ulong filter);

    [EngineMethod("render_debug_direction_arrow", false)]
    void RenderDebugDirectionArrow(Vec3 position, Vec3 direction, uint color, bool depthCheck);

    [EngineMethod("render_debug_line", false)]
    void RenderDebugLine(Vec3 position, Vec3 direction, uint color, bool depthCheck, float time);

    [EngineMethod("render_debug_sphere", false)]
    void RenderDebugSphere(Vec3 position, float radius, uint color, bool depthCheck, float time);

    [EngineMethod("render_debug_capsule", false)]
    void RenderDebugCapsule(
      Vec3 p0,
      Vec3 p1,
      float radius,
      uint color,
      bool depthCheck,
      float time);

    [EngineMethod("render_debug_frame", false)]
    void RenderDebugFrame(ref MatrixFrame frame, float lineLength, float time);

    [EngineMethod("render_debug_text3d", false)]
    void RenderDebugText3d(
      Vec3 worldPosition,
      string str,
      uint color,
      int screenPosOffsetX,
      int screenPosOffsetY,
      float time);

    [EngineMethod("render_debug_text", false)]
    void RenderDebugText(float screenX, float screenY, string str, uint color, float time);

    [EngineMethod("render_debug_rect", false)]
    void RenderDebugRect(float left, float bottom, float right, float top);

    [EngineMethod("render_debug_rect_with_color", false)]
    void RenderDebugRectWithColor(float left, float bottom, float right, float top, uint color);

    [EngineMethod("clear_all_debug_render_objects", false)]
    void ClearAllDebugRenderObjects();

    [EngineMethod("get_debug_vector", false)]
    Vec3 GetDebugVector();

    [EngineMethod("render_debug_box_object", false)]
    void RenderDebugBoxObject(Vec3 min, Vec3 max, uint color, bool depthCheck, float time);

    [EngineMethod("render_debug_box_object_with_frame", false)]
    void RenderDebugBoxObjectWithFrame(
      Vec3 min,
      Vec3 max,
      ref MatrixFrame frame,
      uint color,
      bool depthCheck,
      float time);

    [EngineMethod("post_warning_line", false)]
    void PostWarningLine(string line);

    [EngineMethod("is_error_report_mode_active", false)]
    bool IsErrorReportModeActive();

    [EngineMethod("set_error_report_scene", false)]
    void SetErrorReportScene(UIntPtr scenePointer);

    [EngineMethod("message_box", false)]
    int MessageBox(string lpText, string lpCaption, uint uType);

    [EngineMethod("get_show_debug_info", false)]
    int GetShowDebugInfo();

    [EngineMethod("set_show_debug_info", false)]
    void SetShowDebugInfo(int value);

    [EngineMethod("error", false)]
    bool Error(string MessageString);

    [EngineMethod("warning", false)]
    bool Warning(string MessageString);

    [EngineMethod("content_warning", false)]
    bool ContentWarning(string MessageString);

    [EngineMethod("failed_assert", false)]
    bool FailedAssert(
      string messageString,
      string callerFile,
      string callerMethod,
      int callerLine);

    [EngineMethod("silent_assert", false)]
    bool SilentAssert(
      string messageString,
      string callerFile,
      string callerMethod,
      int callerLine,
      bool getDump);

    [EngineMethod("is_test_mode", false)]
    bool IsTestMode();
  }
}
