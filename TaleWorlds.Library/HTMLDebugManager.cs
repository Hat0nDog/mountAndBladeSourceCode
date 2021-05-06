// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.HTMLDebugManager
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public class HTMLDebugManager : IDebugManager
  {
    private static Logger _mainLogger = new Logger("__global", true, false, false);

    public static bool LogOnlyErrors
    {
      get => HTMLDebugManager._mainLogger.LogOnlyErrors;
      set => HTMLDebugManager._mainLogger.LogOnlyErrors = value;
    }

    void IDebugManager.SetCrashReportCustomString(string customString)
    {
    }

    void IDebugManager.SetCrashReportCustomStack(string customStack)
    {
    }

    void IDebugManager.ShowMessageBox(string lpText, string lpCaption, uint uType)
    {
    }

    void IDebugManager.ShowWarning(string message) => HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.Warning, false);

    void IDebugManager.Assert(
      bool condition,
      string message,
      string callerFile,
      string callerMethod,
      int callerLine)
    {
      if (condition)
        return;
      HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.Error, false);
    }

    void IDebugManager.SilentAssert(
      bool condition,
      string message,
      bool getDump,
      string callerFile,
      string callerMethod,
      int callerLine)
    {
      ((IDebugManager) this).Assert(condition, message, callerFile, callerMethod, callerLine);
    }

    void IDebugManager.Print(
      string message,
      int logLevel,
      Debug.DebugColor color,
      ulong debugFilter)
    {
      HTMLDebugManager._mainLogger.Print(message, HTMLDebugCategory.General, false);
    }

    void IDebugManager.PrintError(string error, string stackTrace, ulong debugFilter) => HTMLDebugManager._mainLogger.Print(error, HTMLDebugCategory.Error, false);

    void IDebugManager.PrintWarning(string warning, ulong debugFilter) => HTMLDebugManager._mainLogger.Print(warning, HTMLDebugCategory.Warning, false);

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

    Vec3 IDebugManager.GetDebugVector() => Vec3.Zero;
  }
}
