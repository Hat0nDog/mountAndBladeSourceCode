﻿// Decompiled with JetBrains decompiler
// Type: ManagedCallbacks.ScriptingInterfaceOfIMBTeam
// Assembly: TaleWorlds.MountAndBlade.AutoGenerated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D02C25D1-9727-49C6-A24A-EE3800F0364C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.AutoGenerated.dll

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using TaleWorlds.MountAndBlade;

namespace ManagedCallbacks
{
  internal class ScriptingInterfaceOfIMBTeam : IMBTeam
  {
    private static readonly Encoding _utf8 = Encoding.UTF8;
    public static ScriptingInterfaceOfIMBTeam.IsEnemyDelegate call_IsEnemyDelegate;
    public static ScriptingInterfaceOfIMBTeam.SetIsEnemyDelegate call_SetIsEnemyDelegate;

    public bool IsEnemy(UIntPtr missionPointer, int teamIndex, int otherTeamIndex) => ScriptingInterfaceOfIMBTeam.call_IsEnemyDelegate(missionPointer, teamIndex, otherTeamIndex);

    public void SetIsEnemy(
      UIntPtr missionPointer,
      int teamIndex,
      int otherTeamIndex,
      bool isEnemy)
    {
      ScriptingInterfaceOfIMBTeam.call_SetIsEnemyDelegate(missionPointer, teamIndex, otherTeamIndex, isEnemy);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool IsEnemyDelegate(UIntPtr missionPointer, int teamIndex, int otherTeamIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetIsEnemyDelegate(
      UIntPtr missionPointer,
      int teamIndex,
      int otherTeamIndex,
      [MarshalAs(UnmanagedType.U1)] bool isEnemy);
  }
}