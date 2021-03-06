// Decompiled with JetBrains decompiler
// Type: ManagedCallbacks.ScriptingInterfaceOfIMBNetwork
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
  internal class ScriptingInterfaceOfIMBNetwork : IMBNetwork
  {
    private static readonly Encoding _utf8 = Encoding.UTF8;
    public static ScriptingInterfaceOfIMBNetwork.GetMultiplayerDisabledDelegate call_GetMultiplayerDisabledDelegate;
    public static ScriptingInterfaceOfIMBNetwork.IsDedicatedServerDelegate call_IsDedicatedServerDelegate;
    public static ScriptingInterfaceOfIMBNetwork.InitializeServerSideDelegate call_InitializeServerSideDelegate;
    public static ScriptingInterfaceOfIMBNetwork.InitializeClientSideDelegate call_InitializeClientSideDelegate;
    public static ScriptingInterfaceOfIMBNetwork.TerminateServerSideDelegate call_TerminateServerSideDelegate;
    public static ScriptingInterfaceOfIMBNetwork.TerminateClientSideDelegate call_TerminateClientSideDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ServerPingDelegate call_ServerPingDelegate;
    public static ScriptingInterfaceOfIMBNetwork.AddPeerToDisconnectDelegate call_AddPeerToDisconnectDelegate;
    public static ScriptingInterfaceOfIMBNetwork.PrepareNewUdpSessionDelegate call_PrepareNewUdpSessionDelegate;
    public static ScriptingInterfaceOfIMBNetwork.CanAddNewPlayersOnServerDelegate call_CanAddNewPlayersOnServerDelegate;
    public static ScriptingInterfaceOfIMBNetwork.AddNewPlayerOnServerDelegate call_AddNewPlayerOnServerDelegate;
    public static ScriptingInterfaceOfIMBNetwork.AddNewBotOnServerDelegate call_AddNewBotOnServerDelegate;
    public static ScriptingInterfaceOfIMBNetwork.RemoveBotOnServerDelegate call_RemoveBotOnServerDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ResetMissionDataDelegate call_ResetMissionDataDelegate;
    public static ScriptingInterfaceOfIMBNetwork.BeginBroadcastModuleEventDelegate call_BeginBroadcastModuleEventDelegate;
    public static ScriptingInterfaceOfIMBNetwork.EndBroadcastModuleEventDelegate call_EndBroadcastModuleEventDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ElapsedTimeSinceLastUdpPacketArrivedDelegate call_ElapsedTimeSinceLastUdpPacketArrivedDelegate;
    public static ScriptingInterfaceOfIMBNetwork.BeginModuleEventAsClientDelegate call_BeginModuleEventAsClientDelegate;
    public static ScriptingInterfaceOfIMBNetwork.EndModuleEventAsClientDelegate call_EndModuleEventAsClientDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ReadIntFromPacketDelegate call_ReadIntFromPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ReadUintFromPacketDelegate call_ReadUintFromPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ReadLongFromPacketDelegate call_ReadLongFromPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ReadUlongFromPacketDelegate call_ReadUlongFromPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ReadFloatFromPacketDelegate call_ReadFloatFromPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ReadStringFromPacketDelegate call_ReadStringFromPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.WriteIntToPacketDelegate call_WriteIntToPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.WriteUintToPacketDelegate call_WriteUintToPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.WriteLongToPacketDelegate call_WriteLongToPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.WriteUlongToPacketDelegate call_WriteUlongToPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.WriteFloatToPacketDelegate call_WriteFloatToPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.WriteStringToPacketDelegate call_WriteStringToPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ReadByteArrayFromPacketDelegate call_ReadByteArrayFromPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.WriteByteArrayToPacketDelegate call_WriteByteArrayToPacketDelegate;
    public static ScriptingInterfaceOfIMBNetwork.IncreaseTotalUploadLimitDelegate call_IncreaseTotalUploadLimitDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ResetDebugVariablesDelegate call_ResetDebugVariablesDelegate;
    public static ScriptingInterfaceOfIMBNetwork.PrintDebugStatsDelegate call_PrintDebugStatsDelegate;
    public static ScriptingInterfaceOfIMBNetwork.GetAveragePacketLossRatioDelegate call_GetAveragePacketLossRatioDelegate;
    public static ScriptingInterfaceOfIMBNetwork.GetDebugUploadsInBitsDelegate call_GetDebugUploadsInBitsDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ResetDebugUploadsDelegate call_ResetDebugUploadsDelegate;
    public static ScriptingInterfaceOfIMBNetwork.PrintReplicationTableStatisticsDelegate call_PrintReplicationTableStatisticsDelegate;
    public static ScriptingInterfaceOfIMBNetwork.ClearReplicationTableStatisticsDelegate call_ClearReplicationTableStatisticsDelegate;

    public bool GetMultiplayerDisabled() => ScriptingInterfaceOfIMBNetwork.call_GetMultiplayerDisabledDelegate();

    public bool IsDedicatedServer() => ScriptingInterfaceOfIMBNetwork.call_IsDedicatedServerDelegate();

    public void InitializeServerSide(int port) => ScriptingInterfaceOfIMBNetwork.call_InitializeServerSideDelegate(port);

    public void InitializeClientSide(
      string serverAddress,
      int port,
      int sessionKey,
      int playerIndex)
    {
      byte[] numArray = (byte[]) null;
      if (serverAddress != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBNetwork._utf8.GetBytes(serverAddress, 0, serverAddress.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      ScriptingInterfaceOfIMBNetwork.call_InitializeClientSideDelegate(numArray, port, sessionKey, playerIndex);
    }

    public void TerminateServerSide() => ScriptingInterfaceOfIMBNetwork.call_TerminateServerSideDelegate();

    public void TerminateClientSide() => ScriptingInterfaceOfIMBNetwork.call_TerminateClientSideDelegate();

    public void ServerPing(string serverAddress, int port)
    {
      byte[] numArray = (byte[]) null;
      if (serverAddress != null)
      {
        numArray = CallbackStringBufferManager.StringBuffer0;
        int bytes = ScriptingInterfaceOfIMBNetwork._utf8.GetBytes(serverAddress, 0, serverAddress.Length, numArray, 0);
        numArray[bytes] = (byte) 0;
      }
      ScriptingInterfaceOfIMBNetwork.call_ServerPingDelegate(numArray, port);
    }

    public void AddPeerToDisconnect(int peer) => ScriptingInterfaceOfIMBNetwork.call_AddPeerToDisconnectDelegate(peer);

    public void PrepareNewUdpSession(int player, int sessionKey) => ScriptingInterfaceOfIMBNetwork.call_PrepareNewUdpSessionDelegate(player, sessionKey);

    public bool CanAddNewPlayersOnServer(int numPlayers) => ScriptingInterfaceOfIMBNetwork.call_CanAddNewPlayersOnServerDelegate(numPlayers);

    public int AddNewPlayerOnServer(bool serverPlayer) => ScriptingInterfaceOfIMBNetwork.call_AddNewPlayerOnServerDelegate(serverPlayer);

    public int AddNewBotOnServer() => ScriptingInterfaceOfIMBNetwork.call_AddNewBotOnServerDelegate();

    public void RemoveBotOnServer(int botPlayerIndex) => ScriptingInterfaceOfIMBNetwork.call_RemoveBotOnServerDelegate(botPlayerIndex);

    public void ResetMissionData() => ScriptingInterfaceOfIMBNetwork.call_ResetMissionDataDelegate();

    public void BeginBroadcastModuleEvent() => ScriptingInterfaceOfIMBNetwork.call_BeginBroadcastModuleEventDelegate();

    public void EndBroadcastModuleEvent(int broadcastFlags, int targetPlayer, bool isReliable) => ScriptingInterfaceOfIMBNetwork.call_EndBroadcastModuleEventDelegate(broadcastFlags, targetPlayer, isReliable);

    public double ElapsedTimeSinceLastUdpPacketArrived() => ScriptingInterfaceOfIMBNetwork.call_ElapsedTimeSinceLastUdpPacketArrivedDelegate();

    public void BeginModuleEventAsClient(bool isReliable) => ScriptingInterfaceOfIMBNetwork.call_BeginModuleEventAsClientDelegate(isReliable);

    public void EndModuleEventAsClient(bool isReliable) => ScriptingInterfaceOfIMBNetwork.call_EndModuleEventAsClientDelegate(isReliable);

    public bool ReadIntFromPacket(ref CompressionInfo.Integer compressionInfo, out int output) => ScriptingInterfaceOfIMBNetwork.call_ReadIntFromPacketDelegate(ref compressionInfo, out output);

    public bool ReadUintFromPacket(
      ref CompressionInfo.UnsignedInteger compressionInfo,
      out uint output)
    {
      return ScriptingInterfaceOfIMBNetwork.call_ReadUintFromPacketDelegate(ref compressionInfo, out output);
    }

    public bool ReadLongFromPacket(ref CompressionInfo.LongInteger compressionInfo, out long output) => ScriptingInterfaceOfIMBNetwork.call_ReadLongFromPacketDelegate(ref compressionInfo, out output);

    public bool ReadUlongFromPacket(
      ref CompressionInfo.UnsignedLongInteger compressionInfo,
      out ulong output)
    {
      return ScriptingInterfaceOfIMBNetwork.call_ReadUlongFromPacketDelegate(ref compressionInfo, out output);
    }

    public bool ReadFloatFromPacket(ref CompressionInfo.Float compressionInfo, out float output) => ScriptingInterfaceOfIMBNetwork.call_ReadFloatFromPacketDelegate(ref compressionInfo, out output);

    public string ReadStringFromPacket(ref bool bufferReadValid) => ScriptingInterfaceOfIMBNetwork.call_ReadStringFromPacketDelegate(ref bufferReadValid) != 1 ? (string) null : Managed.ReturnValueFromEngine;

    public void WriteIntToPacket(int value, ref CompressionInfo.Integer compressionInfo) => ScriptingInterfaceOfIMBNetwork.call_WriteIntToPacketDelegate(value, ref compressionInfo);

    public void WriteUintToPacket(
      uint value,
      ref CompressionInfo.UnsignedInteger compressionInfo)
    {
      ScriptingInterfaceOfIMBNetwork.call_WriteUintToPacketDelegate(value, ref compressionInfo);
    }

    public void WriteLongToPacket(long value, ref CompressionInfo.LongInteger compressionInfo) => ScriptingInterfaceOfIMBNetwork.call_WriteLongToPacketDelegate(value, ref compressionInfo);

    public void WriteUlongToPacket(
      ulong value,
      ref CompressionInfo.UnsignedLongInteger compressionInfo)
    {
      ScriptingInterfaceOfIMBNetwork.call_WriteUlongToPacketDelegate(value, ref compressionInfo);
    }

    public void WriteFloatToPacket(float value, ref CompressionInfo.Float compressionInfo) => ScriptingInterfaceOfIMBNetwork.call_WriteFloatToPacketDelegate(value, ref compressionInfo);

    public void WriteStringToPacket(string value)
    {
      byte[] bytes1 = (byte[]) null;
      if (value != null)
      {
        bytes1 = CallbackStringBufferManager.StringBuffer0;
        int bytes2 = ScriptingInterfaceOfIMBNetwork._utf8.GetBytes(value, 0, value.Length, bytes1, 0);
        bytes1[bytes2] = (byte) 0;
      }
      ScriptingInterfaceOfIMBNetwork.call_WriteStringToPacketDelegate(bytes1);
    }

    public int ReadByteArrayFromPacket(
      byte[] buffer,
      int offset,
      int bufferCapacity,
      ref bool bufferReadValid)
    {
      PinnedArrayData<byte> pinnedArrayData = new PinnedArrayData<byte>(buffer);
      ManagedArray buffer1 = new ManagedArray(pinnedArrayData.Pointer, buffer != null ? buffer.Length : 0);
      int num = ScriptingInterfaceOfIMBNetwork.call_ReadByteArrayFromPacketDelegate(buffer1, offset, bufferCapacity, ref bufferReadValid);
      pinnedArrayData.Dispose();
      return num;
    }

    public void WriteByteArrayToPacket(byte[] value, int offset, int size)
    {
      PinnedArrayData<byte> pinnedArrayData = new PinnedArrayData<byte>(value);
      ManagedArray managedArray = new ManagedArray(pinnedArrayData.Pointer, value != null ? value.Length : 0);
      ScriptingInterfaceOfIMBNetwork.call_WriteByteArrayToPacketDelegate(managedArray, offset, size);
      pinnedArrayData.Dispose();
    }

    public void IncreaseTotalUploadLimit(int value) => ScriptingInterfaceOfIMBNetwork.call_IncreaseTotalUploadLimitDelegate(value);

    public void ResetDebugVariables() => ScriptingInterfaceOfIMBNetwork.call_ResetDebugVariablesDelegate();

    public void PrintDebugStats() => ScriptingInterfaceOfIMBNetwork.call_PrintDebugStatsDelegate();

    public float GetAveragePacketLossRatio() => ScriptingInterfaceOfIMBNetwork.call_GetAveragePacketLossRatioDelegate();

    public void GetDebugUploadsInBits(
      ref GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct,
      ref GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct)
    {
      ScriptingInterfaceOfIMBNetwork.call_GetDebugUploadsInBitsDelegate(ref networkStatisticsStruct, ref posStatisticsStruct);
    }

    public void ResetDebugUploads() => ScriptingInterfaceOfIMBNetwork.call_ResetDebugUploadsDelegate();

    public void PrintReplicationTableStatistics() => ScriptingInterfaceOfIMBNetwork.call_PrintReplicationTableStatisticsDelegate();

    public void ClearReplicationTableStatistics() => ScriptingInterfaceOfIMBNetwork.call_ClearReplicationTableStatisticsDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool GetMultiplayerDisabledDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool IsDedicatedServerDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void InitializeServerSideDelegate(int port);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void InitializeClientSideDelegate(
      byte[] serverAddress,
      int port,
      int sessionKey,
      int playerIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void TerminateServerSideDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void TerminateClientSideDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ServerPingDelegate(byte[] serverAddress, int port);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void AddPeerToDisconnectDelegate(int peer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void PrepareNewUdpSessionDelegate(int player, int sessionKey);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool CanAddNewPlayersOnServerDelegate(int numPlayers);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int AddNewPlayerOnServerDelegate([MarshalAs(UnmanagedType.U1)] bool serverPlayer);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int AddNewBotOnServerDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void RemoveBotOnServerDelegate(int botPlayerIndex);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ResetMissionDataDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void BeginBroadcastModuleEventDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void EndBroadcastModuleEventDelegate(
      int broadcastFlags,
      int targetPlayer,
      [MarshalAs(UnmanagedType.U1)] bool isReliable);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate double ElapsedTimeSinceLastUdpPacketArrivedDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void BeginModuleEventAsClientDelegate([MarshalAs(UnmanagedType.U1)] bool isReliable);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void EndModuleEventAsClientDelegate([MarshalAs(UnmanagedType.U1)] bool isReliable);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool ReadIntFromPacketDelegate(
      ref CompressionInfo.Integer compressionInfo,
      out int output);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool ReadUintFromPacketDelegate(
      ref CompressionInfo.UnsignedInteger compressionInfo,
      out uint output);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool ReadLongFromPacketDelegate(
      ref CompressionInfo.LongInteger compressionInfo,
      out long output);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool ReadUlongFromPacketDelegate(
      ref CompressionInfo.UnsignedLongInteger compressionInfo,
      out ulong output);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    [return: MarshalAs(UnmanagedType.U1)]
    public delegate bool ReadFloatFromPacketDelegate(
      ref CompressionInfo.Float compressionInfo,
      out float output);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int ReadStringFromPacketDelegate([MarshalAs(UnmanagedType.U1)] ref bool bufferReadValid);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void WriteIntToPacketDelegate(
      int value,
      ref CompressionInfo.Integer compressionInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void WriteUintToPacketDelegate(
      uint value,
      ref CompressionInfo.UnsignedInteger compressionInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void WriteLongToPacketDelegate(
      long value,
      ref CompressionInfo.LongInteger compressionInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void WriteUlongToPacketDelegate(
      ulong value,
      ref CompressionInfo.UnsignedLongInteger compressionInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void WriteFloatToPacketDelegate(
      float value,
      ref CompressionInfo.Float compressionInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void WriteStringToPacketDelegate(byte[] value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate int ReadByteArrayFromPacketDelegate(
      ManagedArray buffer,
      int offset,
      int bufferCapacity,
      [MarshalAs(UnmanagedType.U1)] ref bool bufferReadValid);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void WriteByteArrayToPacketDelegate(ManagedArray value, int offset, int size);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void IncreaseTotalUploadLimitDelegate(int value);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ResetDebugVariablesDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void PrintDebugStatsDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate float GetAveragePacketLossRatioDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void GetDebugUploadsInBitsDelegate(
      ref GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct,
      ref GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ResetDebugUploadsDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void PrintReplicationTableStatisticsDelegate();

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Ansi)]
    [SuppressUnmanagedCodeSecurity]
    [MonoNativeFunctionWrapper]
    public delegate void ClearReplicationTableStatisticsDelegate();
  }
}
