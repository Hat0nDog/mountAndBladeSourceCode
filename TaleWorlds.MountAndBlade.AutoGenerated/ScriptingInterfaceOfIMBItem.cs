﻿// Decompiled with JetBrains decompiler
// Type: ManagedCallbacks.ScriptingInterfaceOfIMBItem
// Assembly: TaleWorlds.MountAndBlade.AutoGenerated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D02C25D1-9727-49C6-A24A-EE3800F0364C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.AutoGenerated.dll

using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using TaleWorlds.DotNet;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace ManagedCallbacks
{
  internal class ScriptingInterfaceOfIMBItem : IMBItem
  {
    private static readonly Encoding _utf8 = Encoding.UTF8;
    public static ScriptingInterfaceOfIMBItem.GetItemUsageIndexDelegate call_GetItemUsageIndexDelegate;
    public static ScriptingInterfaceOfIMBItem.GetItemHolsterIndexDelegate call_GetItemHolsterIndexDelegate;
    public static ScriptingInterfaceOfIMBItem.GetItemIsPassiveUsageDelegate call_GetItemIsPassiveUsageDelegate;
    public static ScriptingInterfaceOfIMBItem.GetHolsterFrameByIndexDelegate call_GetHolsterFrameByIndexDelegate;
    public static ScriptingInterfaceOfIMBItem.GetItemUsageSetFlagsDelegate call_GetItemUsageSetFlagsDelegate;
    public static ScriptingInterfaceOfIMBItem.GetItemUsageReloadActionCodeDelegate call_GetItemUsageReloadActionCodeDelegate;
    public static ScriptingInterfaceOfIMBItem.GetItemUsageStrikeTypeDelegate call_GetItemUsageStrikeTypeDelegate;
    public static ScriptingInterfaceOfIMBItem.GetMissileRangeDelegate call_GetMissileRangeDelegate;

    public int GetItemUsageIndex(string itemusagename)
    {
      byte[] numArray = (byte[]) null;
      if (itemusagename != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBItem._utf8.GetBytes(itemusagename, 0, itemusagename.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      return ScriptingInterfaceOfIMBItem.call_GetItemUsageIndexDelegate(numArray);
    }

    public int GetItemHolsterIndex(string itemholstername)
    {
      byte[] numArray = (byte[]) null;
      if (itemholstername != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBItem._utf8.GetBytes(itemholstername, 0, itemholstername.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      return ScriptingInterfaceOfIMBItem.call_GetItemHolsterIndexDelegate(numArray);
    }

    public bool GetItemIsPassiveUsage(string itemUsageName)
    {
      byte[] numArray = (byte[]) null;
      if (itemUsageName != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBItem._utf8.GetBytes(itemUsageName, 0, itemUsageName.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      return ScriptingInterfaceOfIMBItem.call_GetItemIsPassiveUsageDelegate(numArray);
    }

    public void GetHolsterFrameByIndex(int index, ref MatrixFrame outFrame) => ScriptingInterfaceOfIMBItem.call_GetHolsterFrameByIndexDelegate(index, ref outFrame);

    public int GetItemUsageSetFlags(string ItemUsageName)
    {
      byte[] numArray = (byte[]) null;
      if (ItemUsageName != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBItem._utf8.GetBytes(ItemUsageName, 0, ItemUsageName.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      return ScriptingInterfaceOfIMBItem.call_GetItemUsageSetFlagsDelegate(numArray);
    }

    public int GetItemUsageReloadActionCode(
      string itemUsageName,
      int usageDirection,
      bool isMounted,
      int leftHandUsageSetIndex,
      bool isLeftStance)
    {
      byte[] numArray = (byte[]) null;
      if (itemUsageName != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBItem._utf8.GetBytes(itemUsageName, 0, itemUsageName.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      return ScriptingInterfaceOfIMBItem.call_GetItemUsageReloadActionCodeDelegate(numArray, usageDirection, isMounted, leftHandUsageSetIndex, isLeftStance);
    }

    public int GetItemUsageStrikeType(
      string itemUsageName,
      int usageDirection,
      bool isMounted,
      int leftHandUsageSetIndex,
      bool isLeftStance)
    {
      byte[] numArray = (byte[]) null;
      if (itemUsageName != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBItem._utf8.GetBytes(itemUsageName, 0, itemUsageName.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      return ScriptingInterfaceOfIMBItem.call_GetItemUsageStrikeTypeDelegate(numArray, usageDirection, isMounted, leftHandUsageSetIndex, isLeftStance);
    }

    public float GetMissileRange(float shot_speed, float z_diff) => ScriptingInterfaceOfIMBItem.call_GetMissileRangeDelegate(shot_speed, z_diff);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int GetItemUsageIndexDelegate(byte[] itemusagename);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int GetItemHolsterIndexDelegate(byte[] itemholstername);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool GetItemIsPassiveUsageDelegate(byte[] itemUsageName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void GetHolsterFrameByIndexDelegate(int index, ref MatrixFrame outFrame);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int GetItemUsageSetFlagsDelegate(byte[] ItemUsageName);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int GetItemUsageReloadActionCodeDelegate(
      byte[] itemUsageName,
      int usageDirection,
      [MarshalAs(UnmanagedType.U1)] bool isMounted,
      int leftHandUsageSetIndex,
      [MarshalAs(UnmanagedType.U1)] bool isLeftStance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int GetItemUsageStrikeTypeDelegate(
      byte[] itemUsageName,
      int usageDirection,
      [MarshalAs(UnmanagedType.U1)] bool isMounted,
      int leftHandUsageSetIndex,
      [MarshalAs(UnmanagedType.U1)] bool isLeftStance);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate float GetMissileRangeDelegate(float shot_speed, float z_diff);
  }
}