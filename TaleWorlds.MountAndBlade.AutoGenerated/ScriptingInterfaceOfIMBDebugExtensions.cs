// Decompiled with JetBrains decompiler
// Type: ManagedCallbacks.ScriptingInterfaceOfIMBDebugExtensions
// Assembly: TaleWorlds.MountAndBlade.AutoGenerated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D02C25D1-9727-49C6-A24A-EE3800F0364C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.AutoGenerated.dll

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using TaleWorlds.DotNet;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace ManagedCallbacks
{
  internal class ScriptingInterfaceOfIMBDebugExtensions : IMBDebugExtensions
  {
    private static readonly Encoding _utf8 = Encoding.UTF8;
    public static ScriptingInterfaceOfIMBDebugExtensions.RenderDebugCircleOnTerrainDelegate call_RenderDebugCircleOnTerrainDelegate;
    public static ScriptingInterfaceOfIMBDebugExtensions.RenderDebugArcOnTerrainDelegate call_RenderDebugArcOnTerrainDelegate;
    public static ScriptingInterfaceOfIMBDebugExtensions.RenderDebugLineOnTerrainDelegate call_RenderDebugLineOnTerrainDelegate;
    public static ScriptingInterfaceOfIMBDebugExtensions.OverrideNativeParameterDelegate call_OverrideNativeParameterDelegate;
    public static ScriptingInterfaceOfIMBDebugExtensions.ReloadNativeParametersDelegate call_ReloadNativeParametersDelegate;

    public void RenderDebugCircleOnTerrain(
      UIntPtr scenePointer,
      ref MatrixFrame frame,
      float radius,
      uint color,
      bool depthCheck,
      bool isDotted)
    {
      ScriptingInterfaceOfIMBDebugExtensions.call_RenderDebugCircleOnTerrainDelegate(scenePointer, ref frame, radius, color, depthCheck, isDotted);
    }

    public void RenderDebugArcOnTerrain(
      UIntPtr scenePointer,
      ref MatrixFrame frame,
      float radius,
      float beginAngle,
      float endAngle,
      uint color,
      bool depthCheck,
      bool isDotted)
    {
      ScriptingInterfaceOfIMBDebugExtensions.call_RenderDebugArcOnTerrainDelegate(scenePointer, ref frame, radius, beginAngle, endAngle, color, depthCheck, isDotted);
    }

    public void RenderDebugLineOnTerrain(
      UIntPtr scenePointer,
      Vec3 position,
      Vec3 direction,
      uint color,
      bool depthCheck,
      float time,
      bool isDotted,
      float pointDensity)
    {
      ScriptingInterfaceOfIMBDebugExtensions.call_RenderDebugLineOnTerrainDelegate(scenePointer, position, direction, color, depthCheck, time, isDotted, pointDensity);
    }

    public void OverrideNativeParameter(string paramName, float value)
    {
      byte[] numArray = (byte[]) null;
      if (paramName != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBDebugExtensions._utf8.GetBytes(paramName, 0, paramName.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      ScriptingInterfaceOfIMBDebugExtensions.call_OverrideNativeParameterDelegate(numArray, value);
    }

    public void ReloadNativeParameters() => ScriptingInterfaceOfIMBDebugExtensions.call_ReloadNativeParametersDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void RenderDebugCircleOnTerrainDelegate(
      UIntPtr scenePointer,
      ref MatrixFrame frame,
      float radius,
      uint color,
      [MarshalAs(UnmanagedType.U1)] bool depthCheck,
      [MarshalAs(UnmanagedType.U1)] bool isDotted);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void RenderDebugArcOnTerrainDelegate(
      UIntPtr scenePointer,
      ref MatrixFrame frame,
      float radius,
      float beginAngle,
      float endAngle,
      uint color,
      [MarshalAs(UnmanagedType.U1)] bool depthCheck,
      [MarshalAs(UnmanagedType.U1)] bool isDotted);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void RenderDebugLineOnTerrainDelegate(
      UIntPtr scenePointer,
      Vec3 position,
      Vec3 direction,
      uint color,
      [MarshalAs(UnmanagedType.U1)] bool depthCheck,
      float time,
      [MarshalAs(UnmanagedType.U1)] bool isDotted,
      float pointDensity);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void OverrideNativeParameterDelegate(byte[] paramName, float value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ReloadNativeParametersDelegate();
  }
}
