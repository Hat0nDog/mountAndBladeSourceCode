﻿// Decompiled with JetBrains decompiler
// Type: ManagedCallbacks.ScriptingInterfaceOfIMBVoiceManager
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
  internal class ScriptingInterfaceOfIMBVoiceManager : IMBVoiceManager
  {
    private static readonly Encoding _utf8 = Encoding.UTF8;
    public static ScriptingInterfaceOfIMBVoiceManager.GetVoiceTypeIndexDelegate call_GetVoiceTypeIndexDelegate;
    public static ScriptingInterfaceOfIMBVoiceManager.GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassNameDelegate call_GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassNameDelegate;
    public static ScriptingInterfaceOfIMBVoiceManager.GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassNameDelegate call_GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassNameDelegate;

    public int GetVoiceTypeIndex(string voiceType)
    {
      byte[] numArray = (byte[]) null;
      if (voiceType != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBVoiceManager._utf8.GetBytes(voiceType, 0, voiceType.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      return ScriptingInterfaceOfIMBVoiceManager.call_GetVoiceTypeIndexDelegate(numArray);
    }

    public int GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(string className)
    {
      byte[] numArray = (byte[]) null;
      if (className != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBVoiceManager._utf8.GetBytes(className, 0, className.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      return ScriptingInterfaceOfIMBVoiceManager.call_GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassNameDelegate(numArray);
    }

    public void GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(
      string className,
      int[] definitionIndices)
    {
      byte[] numArray = (byte[]) null;
      if (className != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBVoiceManager._utf8.GetBytes(className, 0, className.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      PinnedArrayData<int> pinnedArrayData = new PinnedArrayData<int>(definitionIndices);
      IntPtr pointer = pinnedArrayData.Pointer;
      ScriptingInterfaceOfIMBVoiceManager.call_GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassNameDelegate(numArray, pointer);
      pinnedArrayData.Dispose();
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int GetVoiceTypeIndexDelegate(byte[] voiceType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassNameDelegate(
      byte[] className);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassNameDelegate(
      byte[] className,
      IntPtr definitionIndices);
  }
}