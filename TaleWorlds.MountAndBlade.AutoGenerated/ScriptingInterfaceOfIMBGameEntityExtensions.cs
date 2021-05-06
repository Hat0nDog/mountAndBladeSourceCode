﻿// Decompiled with JetBrains decompiler
// Type: ManagedCallbacks.ScriptingInterfaceOfIMBGameEntityExtensions
// Assembly: TaleWorlds.MountAndBlade.AutoGenerated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D02C25D1-9727-49C6-A24A-EE3800F0364C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.AutoGenerated.dll

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.AutoGenerated;

namespace ManagedCallbacks
{
  internal class ScriptingInterfaceOfIMBGameEntityExtensions : IMBGameEntityExtensions
  {
    private static readonly Encoding _utf8 = Encoding.UTF8;
    public static ScriptingInterfaceOfIMBGameEntityExtensions.CreateFromWeaponDelegate call_CreateFromWeaponDelegate;
    public static ScriptingInterfaceOfIMBGameEntityExtensions.FadeOutDelegate call_FadeOutDelegate;
    public static ScriptingInterfaceOfIMBGameEntityExtensions.FadeInDelegate call_FadeInDelegate;
    public static ScriptingInterfaceOfIMBGameEntityExtensions.HideIfNotFadingOutDelegate call_HideIfNotFadingOutDelegate;

    public GameEntity CreateFromWeapon(
      UIntPtr scenePointer,
      in WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      in WeaponData ammoWeaponData,
      WeaponStatsData[] ammoWeaponStatsData,
      int ammoWeaponStatsDataLength,
      bool showHolsterWithWeapon)
    {
      WeaponDataAsNative weaponData1 = new WeaponDataAsNative(weaponData);
      PinnedArrayData<WeaponStatsData> pinnedArrayData1 = new PinnedArrayData<WeaponStatsData>(weaponStatsData);
      IntPtr pointer1 = pinnedArrayData1.Pointer;
      WeaponDataAsNative ammoWeaponData1 = new WeaponDataAsNative(ammoWeaponData);
      PinnedArrayData<WeaponStatsData> pinnedArrayData2 = new PinnedArrayData<WeaponStatsData>(ammoWeaponStatsData);
      IntPtr pointer2 = pinnedArrayData2.Pointer;
      NativeObjectPointer nativeObjectPointer = ScriptingInterfaceOfIMBGameEntityExtensions.call_CreateFromWeaponDelegate(scenePointer, in weaponData1, pointer1, weaponStatsDataLength, in ammoWeaponData1, pointer2, ammoWeaponStatsDataLength, showHolsterWithWeapon);
      pinnedArrayData1.Dispose();
      pinnedArrayData2.Dispose();
      GameEntity gameEntity = (GameEntity) null;
      if (nativeObjectPointer.Pointer != UIntPtr.Zero)
      {
        gameEntity = new GameEntity(nativeObjectPointer.Pointer);
        LibraryApplicationInterface.IManaged.DecreaseReferenceCount(nativeObjectPointer.Pointer);
      }
      return gameEntity;
    }

    public void FadeOut(UIntPtr entityPointer, float interval, bool isRemovingFromScene) => ScriptingInterfaceOfIMBGameEntityExtensions.call_FadeOutDelegate(entityPointer, interval, isRemovingFromScene);

    public void FadeIn(UIntPtr entityPointer, bool resetAlpha) => ScriptingInterfaceOfIMBGameEntityExtensions.call_FadeInDelegate(entityPointer, resetAlpha);

    public void HideIfNotFadingOut(UIntPtr entityPointer) => ScriptingInterfaceOfIMBGameEntityExtensions.call_HideIfNotFadingOutDelegate(entityPointer);

    GameEntity IMBGameEntityExtensions.CreateFromWeapon(
      UIntPtr scenePointer,
      in WeaponData weaponData,
      WeaponStatsData[] weaponStatsData,
      int weaponStatsDataLength,
      in WeaponData ammoWeaponData,
      WeaponStatsData[] ammoWeaponStatsData,
      int ammoWeaponStatsDataLength,
      bool showHolsterWithWeapon)
    {
      return this.CreateFromWeapon(scenePointer, in weaponData, weaponStatsData, weaponStatsDataLength, in ammoWeaponData, ammoWeaponStatsData, ammoWeaponStatsDataLength, showHolsterWithWeapon);
    }

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate NativeObjectPointer CreateFromWeaponDelegate(
      UIntPtr scenePointer,
      in WeaponDataAsNative weaponData,
      IntPtr weaponStatsData,
      int weaponStatsDataLength,
      in WeaponDataAsNative ammoWeaponData,
      IntPtr ammoWeaponStatsData,
      int ammoWeaponStatsDataLength,
      [MarshalAs(UnmanagedType.U1)] bool showHolsterWithWeapon);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void FadeOutDelegate(
      UIntPtr entityPointer,
      float interval,
      [MarshalAs(UnmanagedType.U1)] bool isRemovingFromScene);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void FadeInDelegate(UIntPtr entityPointer, [MarshalAs(UnmanagedType.U1)] bool resetAlpha);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void HideIfNotFadingOutDelegate(UIntPtr entityPointer);
  }
}