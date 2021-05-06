// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.Debug
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library
{
  public static class Debug
  {
    public static event Action<string, ulong> OnPrint;

    public static IDebugManager DebugManager { get; set; }

    public static void SetCrashReportCustomString(string customString)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.SetCrashReportCustomString(customString);
    }

    public static void SetCrashReportCustomStack(string customStack)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.SetCrashReportCustomStack(customStack);
    }

    [Conditional("TRACE")]
    public static void Assert(
      bool condition,
      string message = "",
      [CallerFilePath] string callerFile = "",
      [CallerMemberName] string callerMethod = "",
      [CallerLineNumber] int callerLine = 0)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.Assert(condition, message, callerFile, callerMethod, callerLine);
    }

    public static void SilentAssert(
      bool condition,
      string message = "",
      bool getDump = false,
      [CallerFilePath] string callerFile = "",
      [CallerMemberName] string callerMethod = "",
      [CallerLineNumber] int callerLine = 0)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.SilentAssert(condition, message, getDump, callerFile, callerMethod, callerLine);
    }

    [Conditional("TRACE")]
    public static void ShowWarning(string message)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.ShowWarning(message);
    }

    public static void Print(
      string message,
      int logLevel = 0,
      Debug.DebugColor color = Debug.DebugColor.White,
      ulong debugFilter = 17592186044416)
    {
      if (Debug.DebugManager == null)
        return;
      debugFilter &= 18446744069414584320UL;
      if (debugFilter == 0UL)
        return;
      Debug.DebugManager.Print(message, logLevel, color, debugFilter);
      Action<string, ulong> onPrint = Debug.OnPrint;
      if (onPrint == null)
        return;
      onPrint(message, debugFilter);
    }

    public static void ShowMessageBox(string lpText, string lpCaption, uint uType)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.ShowMessageBox(lpText, lpCaption, uType);
    }

    [Conditional("TRACE")]
    public static void PrintWarning(string warning, ulong debugFilter = 17592186044416)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.PrintWarning(warning, debugFilter);
    }

    [Conditional("TRACE")]
    public static void PrintError(string error, string stackTrace = null, ulong debugFilter = 17592186044416)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.PrintError(error, stackTrace, debugFilter);
    }

    [Conditional("TRACE")]
    public static void DisplayDebugMessage(string message)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.DisplayDebugMessage(message);
    }

    [Conditional("TRACE")]
    public static void WatchVariable(string name, object value)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.WatchVariable(name, value);
    }

    [Conditional("TRACE")]
    public static void Print(bool p1, int p2, ConsoleColor consoleColor, ulong p3) => throw new NotImplementedException();

    [Conditional("NOT_SHIPPING")]
    [Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
    public static void BeginTelemetryScope(TelemetryLevelMask levelMask, [CallerMemberName] string scopeName = null)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.BeginTelemetryScope(levelMask, scopeName);
    }

    [Conditional("NOT_SHIPPING")]
    [Conditional("ENABLE_PROFILING_APIS_IN_SHIPPING")]
    public static void EndTelemetryScope()
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.EndTelemetryScope();
    }

    [Conditional("TRACE")]
    public static void WriteDebugLineOnScreen(string message)
    {
      if (Debug.DebugManager == null)
        return;
      Debug.DebugManager.WriteDebugLineOnScreen(message);
    }

    [Conditional("TRACE")]
    public static void RenderDebugLine(
      Vec3 position,
      Vec3 direction,
      uint color = 4294967295,
      bool depthCheck = false,
      float time = 0.0f)
    {
      Debug.DebugManager?.RenderDebugLine(position, direction, color, depthCheck, time);
    }

    [Conditional("TRACE")]
    public static void RenderDebugSphere(
      Vec3 position,
      float radius,
      uint color = 4294967295,
      bool depthCheck = false,
      float time = 0.0f)
    {
      Debug.DebugManager?.RenderDebugSphere(position, radius, color, depthCheck, time);
    }

    [Conditional("TRACE")]
    public static void RenderDebugFrame(MatrixFrame frame, float lineLength, float time = 0.0f) => Debug.DebugManager?.RenderDebugFrame(frame, lineLength, time);

    [Conditional("TRACE")]
    public static void RenderDebugText(
      float screenX,
      float screenY,
      string text,
      uint color = 4294967295,
      float time = 0.0f)
    {
      Debug.DebugManager?.RenderDebugText(screenX, screenY, text, color, time);
    }

    public static Vec3 GetDebugVector()
    {
      IDebugManager debugManager = Debug.DebugManager;
      return debugManager == null ? Vec3.Zero : debugManager.GetDebugVector();
    }

    public enum DebugColor
    {
      DarkRed,
      DarkGreen,
      DarkBlue,
      Red,
      Green,
      Blue,
      DarkCyan,
      Cyan,
      DarkYellow,
      Yellow,
      Purple,
      Magenta,
      White,
      BrightWhite,
    }

    public enum DebugUserFilter : ulong
    {
      None = 0,
      Default = 1,
      Serdar = 2,
      Koray = 4,
      Armagan = 8,
      Intern = 16, // 0x0000000000000010
      Mustafa = 32, // 0x0000000000000020
      Oguzhan = 64, // 0x0000000000000040
      DamageDebug = 72, // 0x0000000000000048
      Omer = 128, // 0x0000000000000080
      Ozan = 256, // 0x0000000000000100
      Arhan = 512, // 0x0000000000000200
      Basak = 1024, // 0x0000000000000400
      Can = 2048, // 0x0000000000000800
      Alper = 4096, // 0x0000000000001000
      Cem = 8192, // 0x0000000000002000
      Bahar = 16384, // 0x0000000000004000
      Ceren = 32768, // 0x0000000000008000
      CampaignDebug = 49472, // 0x000000000000C140
      Emircan = 65536, // 0x0000000000010000
      Korneel = 131072, // 0x0000000000020000
      All = 4294967295, // 0x00000000FFFFFFFF
    }

    public enum DebugSystemFilter : ulong
    {
      None = 0,
      Graphics = 4294967296, // 0x0000000100000000
      ArtificialIntelligence = 8589934592, // 0x0000000200000000
      MultiPlayer = 17179869184, // 0x0000000400000000
      IO = 34359738368, // 0x0000000800000000
      Network = 68719476736, // 0x0000001000000000
      CampaignEvents = 137438953472, // 0x0000002000000000
      MemoryManager = 274877906944, // 0x0000004000000000
      TCP = 549755813888, // 0x0000008000000000
      FileManager = 1099511627776, // 0x0000010000000000
      NaturalInteractionDevice = 2199023255552, // 0x0000020000000000
      UDP = 4398046511104, // 0x0000040000000000
      ResourceManager = 8796093022208, // 0x0000080000000000
      Mono = 17592186044416, // 0x0000100000000000
      ONO = 35184372088832, // 0x0000200000000000
      Old = 70368744177664, // 0x0000400000000000
      Sound = 281474976710656, // 0x0001000000000000
      CombatLog = 562949953421312, // 0x0002000000000000
      Notifications = 1125899906842624, // 0x0004000000000000
      Quest = 2251799813685248, // 0x0008000000000000
      Dialog = 4503599627370496, // 0x0010000000000000
      Steam = 9007199254740992, // 0x0020000000000000
      All = 18446744069414584320, // 0xFFFFFFFF00000000
      DefaultMask = 18446744069414584320, // 0xFFFFFFFF00000000
    }
  }
}
