// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBDebugManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class MBDebugManager : IDebugManager
  {
    void IDebugManager.SetCrashReportCustomString(string customString) => Utilities.SetCrashReportCustomString(customString);

    void IDebugManager.SetCrashReportCustomStack(string customStack) => Utilities.SetCrashReportCustomStack(customStack);

    void IDebugManager.ShowWarning(string message) => MBDebug.ShowWarning(message);

    void IDebugManager.ShowMessageBox(string lpText, string lpCaption, uint uType) => MBDebug.ShowMessageBox(lpText, lpCaption, uType);

    void IDebugManager.Assert(
      bool condition,
      string message,
      string callerFile,
      string callerMethod,
      int callerLine)
    {
    }

    void IDebugManager.SilentAssert(
      bool condition,
      string message,
      bool getDump,
      string callerFile,
      string callerMethod,
      int callerLine)
    {
      MBDebug.SilentAssert(condition, message, getDump, callerFile, callerMethod, callerLine);
    }

    void IDebugManager.Print(
      string message,
      int logLevel,
      Debug.DebugColor color,
      ulong debugFilter)
    {
      MBDebug.Print(message, logLevel, color, debugFilter);
    }

    void IDebugManager.PrintError(string error, string stackTrace, ulong debugFilter) => MBDebug.Print(error, debugFilter: debugFilter);

    void IDebugManager.PrintWarning(string warning, ulong debugFilter) => MBDebug.Print(warning, debugFilter: debugFilter);

    void IDebugManager.DisplayDebugMessage(string message)
    {
    }

    void IDebugManager.WatchVariable(string name, object value)
    {
    }

    void IDebugManager.BeginTelemetryScope(
      TelemetryLevelMask levelMask,
      string scopeName)
    {
    }

    void IDebugManager.EndTelemetryScope()
    {
    }

    void IDebugManager.WriteDebugLineOnScreen(string message)
    {
    }

    void IDebugManager.RenderDebugLine(
      Vec3 position,
      Vec3 direction,
      uint color,
      bool depthCheck,
      float time)
    {
    }

    void IDebugManager.RenderDebugSphere(
      Vec3 position,
      float radius,
      uint color,
      bool depthCheck,
      float time)
    {
    }

    void IDebugManager.RenderDebugFrame(
      MatrixFrame frame,
      float lineLength,
      float time)
    {
    }

    void IDebugManager.RenderDebugText(
      float screenX,
      float screenY,
      string text,
      uint color,
      float time)
    {
    }

    Vec3 IDebugManager.GetDebugVector() => MBDebug.DebugVector;
  }
}
