// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.MBDebug
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public static class MBDebug
  {
    public static bool DisableAllUI;
    public static bool TestModeEnabled;
    public static bool ShouldAssertThrowException;
    public static bool IsDisplayingHighLevelAI;
    [ThreadStatic]
    private static Stack<uint> _telemetryStack;
    public static bool DisableLogging;
    private static readonly Dictionary<string, int> ProcessedFrameList = new Dictionary<string, int>();

    public static uint TelemetryLevelMask => Managed.TelemetryLevelMask;

    [Conditional("NOT_SHIPPING")]
    [Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
    public static void StartTelemetryConnection(bool showErrors) => EngineApplicationInterface.IUtil.StartTelemetryConnection(showErrors);

    [Conditional("NOT_SHIPPING")]
    [Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
    public static void StopTelemetryConnection() => EngineApplicationInterface.IUtil.StopTelemetryConnection();

    [Conditional("NOT_SHIPPING")]
    [Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
    public static void BeginTelemetryScope(TaleWorlds.Library.TelemetryLevelMask levelMask, string scopeName)
    {
      if (MBDebug._telemetryStack == null)
        MBDebug._telemetryStack = new Stack<uint>(8);
      if (((TaleWorlds.Library.TelemetryLevelMask) MBDebug.TelemetryLevelMask & levelMask) != (TaleWorlds.Library.TelemetryLevelMask) 0)
        MBDebug._telemetryStack.Push(EngineApplicationInterface.IUtil.BeginTelemetryScope(levelMask, scopeName));
      else
        MBDebug._telemetryStack.Push(0U);
    }

    [Conditional("NOT_SHIPPING")]
    [Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
    public static void EndTelemetryScope()
    {
      uint scopeId = MBDebug._telemetryStack.Pop();
      if (scopeId <= 0U)
        return;
      EngineApplicationInterface.IUtil.EndTelemetryScope(scopeId);
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("toggle_ui", "ui")]
    public static string DisableUI(List<string> strings)
    {
      if (strings.Count != 0)
        return "Invalid input.";
      MBDebug.DisableAllUI = !MBDebug.DisableAllUI;
      return MBDebug.DisableAllUI ? "UI is now disabled." : "UI is now enabled.";
    }

    [Conditional("TRACE")]
    public static void AssertMemoryUsage(int memoryMB) => EngineApplicationInterface.IDebug.AssertMemoryUsage(memoryMB);

    public static void AbortGame(int ExitCode = 5) => EngineApplicationInterface.IDebug.AbortGame(ExitCode);

    public static void ShowWarning(string message)
    {
      if (!(Debugger.IsAttached & EngineApplicationInterface.IDebug.Warning(message)))
        return;
      Debugger.Break();
    }

    public static void ShowMessageBox(string lpText, string lpCaption, uint uType) => EngineApplicationInterface.IDebug.MessageBox(lpText, lpCaption, uType);

    [Conditional("TRACE")]
    public static void Assert(
      bool condition,
      string message = "",
      [CallerFilePath] string callerFile = "",
      [CallerMemberName] string callerMethod = "",
      [CallerLineNumber] int callerLine = 0)
    {
      if (condition || !(Debugger.IsAttached & EngineApplicationInterface.IDebug.FailedAssert(message, callerFile, callerMethod, callerLine)))
        return;
      Debugger.Break();
    }

    public static void SilentAssert(
      bool condition,
      string message = "",
      bool getDump = false,
      [CallerFilePath] string callerFile = "",
      [CallerMemberName] string callerMethod = "",
      [CallerLineNumber] int callerLine = 0)
    {
      if (condition || !(Debugger.IsAttached & EngineApplicationInterface.IDebug.SilentAssert(message, callerFile, callerMethod, callerLine, getDump)))
        return;
      Debugger.Break();
    }

    [Conditional("DEBUG_MORE")]
    public static void AssertConditionOrCallerClassName(bool condition, string name)
    {
      StackFrame frame = new StackTrace(2, true).GetFrame(0);
      if (condition)
        return;
      string name1 = frame.GetMethod().DeclaringType.Name;
    }

    [Conditional("DEBUG_MORE")]
    public static void AssertConditionOrCallerClassNameSearchAllCallstack(
      bool condition,
      string name)
    {
      StackTrace stackTrace = new StackTrace(true);
      if (condition)
        return;
      int index = 0;
      while (index < stackTrace.FrameCount && !(stackTrace.GetFrame(index).GetMethod().DeclaringType.Name == name))
        ++index;
    }

    public static void Print(
      string message,
      int logLevel = 0,
      TaleWorlds.Library.Debug.DebugColor color = TaleWorlds.Library.Debug.DebugColor.White,
      ulong debugFilter = 17592186044416)
    {
      if (MBDebug.DisableLogging)
        return;
      debugFilter &= 18446744069414584320UL;
      if (debugFilter == 0UL)
        return;
      try
      {
        if (EngineApplicationInterface.IDebug == null)
          return;
        EngineApplicationInterface.IDebug.WriteLine(logLevel, message, (int) color, debugFilter);
      }
      catch
      {
      }
    }

    [Conditional("TRACE")]
    public static void ConsolePrint(string message, TaleWorlds.Library.Debug.DebugColor color = TaleWorlds.Library.Debug.DebugColor.White, ulong debugFilter = 17592186044416)
    {
      try
      {
        EngineApplicationInterface.IDebug.WriteLine(0, message, (int) color, debugFilter);
      }
      catch
      {
      }
    }

    [Conditional("TRACE")]
    public static void WriteDebugLineOnScreen(string str) => EngineApplicationInterface.IDebug.WriteDebugLineOnScreen(str);

    [Conditional("TRACE")]
    public static void RenderDebugText(
      float screenX,
      float screenY,
      string text,
      uint color = 4294967295,
      float time = 0.0f)
    {
      EngineApplicationInterface.IDebug.RenderDebugText(screenX, screenY, text, color, time);
    }

    public static void RenderText(
      float screenX,
      float screenY,
      string text,
      uint color = 4294967295,
      float time = 0.0f)
    {
      EngineApplicationInterface.IDebug.RenderDebugText(screenX, screenY, text, color, time);
    }

    [Conditional("TRACE")]
    public static void RenderDebugRect(float left, float bottom, float right, float top) => EngineApplicationInterface.IDebug.RenderDebugRect(left, bottom, right, top);

    [Conditional("TRACE")]
    public static void RenderDebugRectWithColor(
      float left,
      float bottom,
      float right,
      float top,
      uint color = 4294967295)
    {
      EngineApplicationInterface.IDebug.RenderDebugRectWithColor(left, bottom, right, top, color);
    }

    [Conditional("TRACE")]
    public static void RenderDebugFrame(MatrixFrame frame, float lineLength, float time = 0.0f) => EngineApplicationInterface.IDebug.RenderDebugFrame(ref frame, lineLength, time);

    [Conditional("TRACE")]
    public static void RenderDebugText3D(
      Vec3 worldPosition,
      string str,
      uint color = 4294967295,
      int screenPosOffsetX = 0,
      int screenPosOffsetY = 0,
      float time = 0.0f)
    {
      EngineApplicationInterface.IDebug.RenderDebugText3d(worldPosition, str, color, screenPosOffsetX, screenPosOffsetY, time);
    }

    [Conditional("TRACE")]
    public static void RenderDebugDirectionArrow(
      Vec3 position,
      Vec3 direction,
      uint color = 4294967295,
      bool depthCheck = false)
    {
      EngineApplicationInterface.IDebug.RenderDebugDirectionArrow(position, direction, color, depthCheck);
    }

    [Conditional("TRACE")]
    public static void RenderDebugLine(
      Vec3 position,
      Vec3 direction,
      uint color = 4294967295,
      bool depthCheck = false,
      float time = 0.0f)
    {
      EngineApplicationInterface.IDebug.RenderDebugLine(position, direction, color, depthCheck, time);
    }

    [Conditional("TRACE")]
    public static void RenderDebugSphere(
      Vec3 position,
      float radius,
      uint color = 4294967295,
      bool depthCheck = false,
      float time = 0.0f)
    {
      EngineApplicationInterface.IDebug.RenderDebugSphere(position, radius, color, depthCheck, time);
    }

    [Conditional("TRACE")]
    public static void RenderDebugCapsule(
      Vec3 p0,
      Vec3 p1,
      float radius,
      uint color = 4294967295,
      bool depthCheck = false,
      float time = 0.0f)
    {
      EngineApplicationInterface.IDebug.RenderDebugCapsule(p0, p1, radius, color, depthCheck, time);
    }

    [Conditional("TRACE")]
    public static void ClearRenderObjects() => EngineApplicationInterface.IDebug.ClearAllDebugRenderObjects();

    public static Vec3 DebugVector => EngineApplicationInterface.IDebug.GetDebugVector();

    [Conditional("TRACE")]
    public static void RenderDebugBoxObject(
      Vec3 min,
      Vec3 max,
      uint color = 4294967295,
      bool depthCheck = false,
      float time = 0.0f)
    {
      EngineApplicationInterface.IDebug.RenderDebugBoxObject(min, max, color, depthCheck, time);
    }

    [Conditional("TRACE")]
    public static void RenderDebugBoxObject(
      Vec3 min,
      Vec3 max,
      MatrixFrame frame,
      uint color = 4294967295,
      bool depthCheck = false,
      float time = 0.0f)
    {
      EngineApplicationInterface.IDebug.RenderDebugBoxObjectWithFrame(min, max, ref frame, color, depthCheck, time);
    }

    [Conditional("TRACE")]
    public static void PostWarningLine(string line) => EngineApplicationInterface.IDebug.PostWarningLine(line);

    public static bool IsErrorReportModeActive() => EngineApplicationInterface.IDebug.IsErrorReportModeActive();

    public static void SetErrorReportScene(Scene scene)
    {
      UIntPtr scenePointer = (NativeObject) scene == (NativeObject) null ? UIntPtr.Zero : scene.Pointer;
      EngineApplicationInterface.IDebug.SetErrorReportScene(scenePointer);
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("clear", "console")]
    public static string ClearConsole(List<string> strings)
    {
      Console.Clear();
      return "Debug console cleared.";
    }

    public static int ShowDebugInfoState
    {
      get => EngineApplicationInterface.IDebug.GetShowDebugInfo();
      set => EngineApplicationInterface.IDebug.SetShowDebugInfo(value);
    }

    public static bool IsTestMode() => EngineApplicationInterface.IDebug.IsTestMode();

    [Flags]
    public enum MessageBoxTypeFlag
    {
      Ok = 1,
      Warning = 2,
      Error = 4,
      OkCancel = 8,
      RetryCancel = 16, // 0x00000010
      YesNo = 32, // 0x00000020
      YesNoCancel = 64, // 0x00000040
      Information = 128, // 0x00000080
      Exclamation = 256, // 0x00000100
      Question = 512, // 0x00000200
      AssertFailed = 1024, // 0x00000400
    }
  }
}
