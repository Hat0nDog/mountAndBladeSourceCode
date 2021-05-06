// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBNetwork
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBNetwork
  {
    [EngineMethod("get_multiplayer_disabled", false)]
    bool GetMultiplayerDisabled();

    [EngineMethod("is_dedicated_server", false)]
    bool IsDedicatedServer();

    [EngineMethod("initialize_server_side", false)]
    void InitializeServerSide(int port);

    [EngineMethod("initialize_client_side", false)]
    void InitializeClientSide(string serverAddress, int port, int sessionKey, int playerIndex);

    [EngineMethod("terminate_server_side", false)]
    void TerminateServerSide();

    [EngineMethod("terminate_client_side", false)]
    void TerminateClientSide();

    [EngineMethod("server_ping", false)]
    void ServerPing(string serverAddress, int port);

    [EngineMethod("add_peer_to_disconnect", false)]
    void AddPeerToDisconnect(int peer);

    [EngineMethod("prepare_new_udp_session", false)]
    void PrepareNewUdpSession(int player, int sessionKey);

    [EngineMethod("can_add_new_players_on_server", false)]
    bool CanAddNewPlayersOnServer(int numPlayers);

    [EngineMethod("add_new_player_on_server", false)]
    int AddNewPlayerOnServer(bool serverPlayer);

    [EngineMethod("add_new_bot_on_server", false)]
    int AddNewBotOnServer();

    [EngineMethod("remove_bot_on_server", false)]
    void RemoveBotOnServer(int botPlayerIndex);

    [EngineMethod("reset_mission_data", false)]
    void ResetMissionData();

    [EngineMethod("begin_broadcast_module_event", false)]
    void BeginBroadcastModuleEvent();

    [EngineMethod("end_broadcast_module_event", false)]
    void EndBroadcastModuleEvent(int broadcastFlags, int targetPlayer, bool isReliable);

    [EngineMethod("elapsed_time_since_last_udp_packet_arrived", false)]
    double ElapsedTimeSinceLastUdpPacketArrived();

    [EngineMethod("begin_module_event_as_client", false)]
    void BeginModuleEventAsClient(bool isReliable);

    [EngineMethod("end_module_event_as_client", false)]
    void EndModuleEventAsClient(bool isReliable);

    [EngineMethod("read_int_from_packet", false)]
    bool ReadIntFromPacket(ref CompressionInfo.Integer compressionInfo, out int output);

    [EngineMethod("read_uint_from_packet", false)]
    bool ReadUintFromPacket(
      ref CompressionInfo.UnsignedInteger compressionInfo,
      out uint output);

    [EngineMethod("read_long_from_packet", false)]
    bool ReadLongFromPacket(ref CompressionInfo.LongInteger compressionInfo, out long output);

    [EngineMethod("read_ulong_from_packet", false)]
    bool ReadUlongFromPacket(
      ref CompressionInfo.UnsignedLongInteger compressionInfo,
      out ulong output);

    [EngineMethod("read_float_from_packet", false)]
    bool ReadFloatFromPacket(ref CompressionInfo.Float compressionInfo, out float output);

    [EngineMethod("read_string_from_packet", false)]
    string ReadStringFromPacket(ref bool bufferReadValid);

    [EngineMethod("write_int_to_packet", false)]
    void WriteIntToPacket(int value, ref CompressionInfo.Integer compressionInfo);

    [EngineMethod("write_uint_to_packet", false)]
    void WriteUintToPacket(
      uint value,
      ref CompressionInfo.UnsignedInteger compressionInfo);

    [EngineMethod("write_long_to_packet", false)]
    void WriteLongToPacket(long value, ref CompressionInfo.LongInteger compressionInfo);

    [EngineMethod("write_ulong_to_packet", false)]
    void WriteUlongToPacket(
      ulong value,
      ref CompressionInfo.UnsignedLongInteger compressionInfo);

    [EngineMethod("write_float_to_packet", false)]
    void WriteFloatToPacket(float value, ref CompressionInfo.Float compressionInfo);

    [EngineMethod("write_string_to_packet", false)]
    void WriteStringToPacket(string value);

    [EngineMethod("read_byte_array_from_packet", false)]
    int ReadByteArrayFromPacket(
      byte[] buffer,
      int offset,
      int bufferCapacity,
      ref bool bufferReadValid);

    [EngineMethod("write_byte_array_to_packet", false)]
    void WriteByteArrayToPacket(byte[] value, int offset, int size);

    [EngineMethod("increase_total_upload_limit", false)]
    void IncreaseTotalUploadLimit(int value);

    [EngineMethod("reset_debug_variables", false)]
    void ResetDebugVariables();

    [EngineMethod("print_debug_stats", false)]
    void PrintDebugStats();

    [EngineMethod("get_average_packet_loss_ratio", false)]
    float GetAveragePacketLossRatio();

    [EngineMethod("get_debug_uploads_in_bits", false)]
    void GetDebugUploadsInBits(
      ref GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct,
      ref GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct);

    [EngineMethod("reset_debug_uploads", false)]
    void ResetDebugUploads();

    [EngineMethod("print_replication_table_statistics", false)]
    void PrintReplicationTableStatistics();

    [EngineMethod("clear_replication_table_statistics", false)]
    void ClearReplicationTableStatistics();
  }
}
