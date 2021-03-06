// Decompiled with JetBrains decompiler
// Type: ManagedCallbacks.ScriptingInterfaceOfIMBGame
// Assembly: TaleWorlds.MountAndBlade.AutoGenerated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D02C25D1-9727-49C6-A24A-EE3800F0364C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.AutoGenerated.dll

using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using TaleWorlds.MountAndBlade;

namespace ManagedCallbacks
{
  internal class ScriptingInterfaceOfIMBGame : IMBGame
  {
    private static readonly Encoding _utf8 = Encoding.UTF8;
    public static ScriptingInterfaceOfIMBGame.StartNewDelegate call_StartNewDelegate;
    public static ScriptingInterfaceOfIMBGame.LoadModuleDataDelegate call_LoadModuleDataDelegate;

    public void StartNew() => ScriptingInterfaceOfIMBGame.call_StartNewDelegate();

    public void LoadModuleData(bool isLoadGame) => ScriptingInterfaceOfIMBGame.call_LoadModuleDataDelegate(isLoadGame);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void StartNewDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void LoadModuleDataDelegate([MarshalAs(UnmanagedType.U1)] bool isLoadGame);
  }
}
