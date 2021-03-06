// Decompiled with JetBrains decompiler
// Type: ManagedCallbacks.ScriptingInterfaceOfIMBMapScene
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
  internal class ScriptingInterfaceOfIMBMapScene : IMBMapScene
  {
    private static readonly Encoding _utf8 = Encoding.UTF8;
    public static ScriptingInterfaceOfIMBMapScene.GetAccessiblePointNearPositionDelegate call_GetAccessiblePointNearPositionDelegate;
    public static ScriptingInterfaceOfIMBMapScene.RemoveZeroCornerBodiesDelegate call_RemoveZeroCornerBodiesDelegate;
    public static ScriptingInterfaceOfIMBMapScene.LoadAtmosphereDataDelegate call_LoadAtmosphereDataDelegate;
    public static ScriptingInterfaceOfIMBMapScene.GetFaceIndexForMultiplePositionsDelegate call_GetFaceIndexForMultiplePositionsDelegate;
    public static ScriptingInterfaceOfIMBMapScene.SetSoundParametersDelegate call_SetSoundParametersDelegate;
    public static ScriptingInterfaceOfIMBMapScene.TickStepSoundDelegate call_TickStepSoundDelegate;
    public static ScriptingInterfaceOfIMBMapScene.TickAmbientSoundsDelegate call_TickAmbientSoundsDelegate;
    public static ScriptingInterfaceOfIMBMapScene.TickVisualsDelegate call_TickVisualsDelegate;
    public static ScriptingInterfaceOfIMBMapScene.ValidateTerrainSoundIdsDelegate call_ValidateTerrainSoundIdsDelegate;
    public static ScriptingInterfaceOfIMBMapScene.SetPoliticalColorDelegate call_SetPoliticalColorDelegate;
    public static ScriptingInterfaceOfIMBMapScene.SetFrameForAtmosphereDelegate call_SetFrameForAtmosphereDelegate;
    public static ScriptingInterfaceOfIMBMapScene.GetColorGradeGridDataDelegate call_GetColorGradeGridDataDelegate;
    public static ScriptingInterfaceOfIMBMapScene.SetTerrainDynamicParamsDelegate call_SetTerrainDynamicParamsDelegate;
    public static ScriptingInterfaceOfIMBMapScene.GetMouseVisibleDelegate call_GetMouseVisibleDelegate;
    public static ScriptingInterfaceOfIMBMapScene.SendMouseKeyEventDelegate call_SendMouseKeyEventDelegate;
    public static ScriptingInterfaceOfIMBMapScene.SetMouseVisibleDelegate call_SetMouseVisibleDelegate;
    public static ScriptingInterfaceOfIMBMapScene.SetMousePosDelegate call_SetMousePosDelegate;

    public Vec3 GetAccessiblePointNearPosition(
      UIntPtr scenePointer,
      Vec2 position,
      float radius)
    {
      return ScriptingInterfaceOfIMBMapScene.call_GetAccessiblePointNearPositionDelegate(scenePointer, position, radius);
    }

    public void RemoveZeroCornerBodies(UIntPtr scenePointer) => ScriptingInterfaceOfIMBMapScene.call_RemoveZeroCornerBodiesDelegate(scenePointer);

    public void LoadAtmosphereData(UIntPtr scenePointer) => ScriptingInterfaceOfIMBMapScene.call_LoadAtmosphereDataDelegate(scenePointer);

    public void GetFaceIndexForMultiplePositions(
      UIntPtr scenePointer,
      int movedPartyCount,
      float[] positionArray,
      PathFaceRecord[] resultArray,
      bool check_if_disabled,
      bool check_height)
    {
      PinnedArrayData<float> pinnedArrayData1 = new PinnedArrayData<float>(positionArray);
      IntPtr pointer1 = pinnedArrayData1.Pointer;
      PinnedArrayData<PathFaceRecord> pinnedArrayData2 = new PinnedArrayData<PathFaceRecord>(resultArray);
      IntPtr pointer2 = pinnedArrayData2.Pointer;
      ScriptingInterfaceOfIMBMapScene.call_GetFaceIndexForMultiplePositionsDelegate(scenePointer, movedPartyCount, pointer1, pointer2, check_if_disabled, check_height);
      pinnedArrayData1.Dispose();
      pinnedArrayData2.Dispose();
    }

    public void SetSoundParameters(
      UIntPtr scenePointer,
      float tod,
      int season,
      float cameraHeight)
    {
      ScriptingInterfaceOfIMBMapScene.call_SetSoundParametersDelegate(scenePointer, tod, season, cameraHeight);
    }

    public void TickStepSound(
      UIntPtr scenePointer,
      UIntPtr strategicEntityId,
      int faceIndexterrainType,
      int soundType)
    {
      ScriptingInterfaceOfIMBMapScene.call_TickStepSoundDelegate(scenePointer, strategicEntityId, faceIndexterrainType, soundType);
    }

    public void TickAmbientSounds(UIntPtr scenePointer, int terrainType) => ScriptingInterfaceOfIMBMapScene.call_TickAmbientSoundsDelegate(scenePointer, terrainType);

    public void TickVisuals(
      UIntPtr scenePointer,
      float tod,
      UIntPtr[] ticked_map_meshes,
      int tickedMapMeshesCount)
    {
      PinnedArrayData<UIntPtr> pinnedArrayData = new PinnedArrayData<UIntPtr>(ticked_map_meshes);
      IntPtr pointer = pinnedArrayData.Pointer;
      ScriptingInterfaceOfIMBMapScene.call_TickVisualsDelegate(scenePointer, tod, pointer, tickedMapMeshesCount);
      pinnedArrayData.Dispose();
    }

    public void ValidateTerrainSoundIds() => ScriptingInterfaceOfIMBMapScene.call_ValidateTerrainSoundIdsDelegate();

    public void SetPoliticalColor(UIntPtr scenePointer, string value)
    {
      byte[] bytes1 = (byte[]) null;
      if (value != null)
      {
        bytes1 = CallbackStringBufferManager.StringBuffer0;
        int bytes2 = ScriptingInterfaceOfIMBMapScene._utf8.GetBytes(value, 0, value.Length, bytes1, 0);
        bytes1[bytes2] = (byte) 0;
      }
      ScriptingInterfaceOfIMBMapScene.call_SetPoliticalColorDelegate(scenePointer, bytes1);
    }

    public void SetFrameForAtmosphere(UIntPtr scenePointer, float tod, float cameraElevation) => ScriptingInterfaceOfIMBMapScene.call_SetFrameForAtmosphereDelegate(scenePointer, tod, cameraElevation);

    public void GetColorGradeGridData(UIntPtr scenePointer, byte[] snowData)
    {
      PinnedArrayData<byte> pinnedArrayData = new PinnedArrayData<byte>(snowData);
      ManagedArray snowData1 = new ManagedArray(pinnedArrayData.Pointer, snowData != null ? snowData.Length : 0);
      ScriptingInterfaceOfIMBMapScene.call_GetColorGradeGridDataDelegate(scenePointer, snowData1);
      pinnedArrayData.Dispose();
    }

    public void SetTerrainDynamicParams(UIntPtr scenePointer, Vec3 dynamic_params) => ScriptingInterfaceOfIMBMapScene.call_SetTerrainDynamicParamsDelegate(scenePointer, dynamic_params);

    public bool GetMouseVisible() => ScriptingInterfaceOfIMBMapScene.call_GetMouseVisibleDelegate();

    public void SendMouseKeyEvent(int keyId, bool isDown) => ScriptingInterfaceOfIMBMapScene.call_SendMouseKeyEventDelegate(keyId, isDown);

    public void SetMouseVisible(bool value) => ScriptingInterfaceOfIMBMapScene.call_SetMouseVisibleDelegate(value);

    public void SetMousePos(int posX, int posY) => ScriptingInterfaceOfIMBMapScene.call_SetMousePosDelegate(posX, posY);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate Vec3 GetAccessiblePointNearPositionDelegate(
      UIntPtr scenePointer,
      Vec2 position,
      float radius);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void RemoveZeroCornerBodiesDelegate(UIntPtr scenePointer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void LoadAtmosphereDataDelegate(UIntPtr scenePointer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void GetFaceIndexForMultiplePositionsDelegate(
      UIntPtr scenePointer,
      int movedPartyCount,
      IntPtr positionArray,
      IntPtr resultArray,
      [MarshalAs(UnmanagedType.U1)] bool check_if_disabled,
      [MarshalAs(UnmanagedType.U1)] bool check_height);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetSoundParametersDelegate(
      UIntPtr scenePointer,
      float tod,
      int season,
      float cameraHeight);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void TickStepSoundDelegate(
      UIntPtr scenePointer,
      UIntPtr strategicEntityId,
      int faceIndexterrainType,
      int soundType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void TickAmbientSoundsDelegate(UIntPtr scenePointer, int terrainType);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void TickVisualsDelegate(
      UIntPtr scenePointer,
      float tod,
      IntPtr ticked_map_meshes,
      int tickedMapMeshesCount);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ValidateTerrainSoundIdsDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetPoliticalColorDelegate(UIntPtr scenePointer, byte[] value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetFrameForAtmosphereDelegate(
      UIntPtr scenePointer,
      float tod,
      float cameraElevation);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void GetColorGradeGridDataDelegate(UIntPtr scenePointer, ManagedArray snowData);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetTerrainDynamicParamsDelegate(UIntPtr scenePointer, Vec3 dynamic_params);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool GetMouseVisibleDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SendMouseKeyEventDelegate(int keyId, [MarshalAs(UnmanagedType.U1)] bool isDown);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetMouseVisibleDelegate([MarshalAs(UnmanagedType.U1)] bool value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void SetMousePosDelegate(int posX, int posY);
  }
}
