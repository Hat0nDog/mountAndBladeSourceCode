// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.IDebugManager
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Runtime.CompilerServices;

namespace TaleWorlds.Library
{
  public interface IDebugManager
  {
    void ShowWarning(string message);

    void Assert(
      bool condition,
      string message,
      [CallerFilePath] string callerFile = "",
      [CallerMemberName] string callerMethod = "",
      [CallerLineNumber] int callerLine = 0);

    void SilentAssert(
      bool condition,
      string message = "",
      bool getDump = false,
      [CallerFilePath] string callerFile = "",
      [CallerMemberName] string callerMethod = "",
      [CallerLineNumber] int callerLine = 0);

    void Print(string message, int logLevel = 0, Debug.DebugColor color = Debug.DebugColor.White, ulong debugFilter = 17592186044416);

    void PrintError(string error, string stackTrace, ulong debugFilter = 17592186044416);

    void PrintWarning(string warning, ulong debugFilter = 17592186044416);

    void ShowMessageBox(string lpText, string lpCaption, uint uType);

    void DisplayDebugMessage(string message);

    void WatchVariable(string name, object value);

    void BeginTelemetryScope(TelemetryLevelMask levelMask, string scopeName);

    void EndTelemetryScope();

    void WriteDebugLineOnScreen(string message);

    void RenderDebugLine(Vec3 position, Vec3 direction, uint color = 4294967295, bool depthCheck = false, float time = 0.0f);

    void RenderDebugSphere(Vec3 position, float radius, uint color = 4294967295, bool depthCheck = false, float time = 0.0f);

    void RenderDebugFrame(MatrixFrame frame, float lineLength, float time = 0.0f);

    void RenderDebugText(float screenX, float screenY, string text, uint color = 4294967295, float time = 0.0f);

    Vec3 GetDebugVector();

    void SetCrashReportCustomString(string customString);

    void SetCrashReportCustomStack(string customStack);
  }
}
