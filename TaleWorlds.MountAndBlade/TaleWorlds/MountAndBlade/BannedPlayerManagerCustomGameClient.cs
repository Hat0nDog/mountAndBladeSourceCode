// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BannedPlayerManagerCustomGameClient
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public static class BannedPlayerManagerCustomGameClient
  {
    private static Dictionary<PlayerId, BannedPlayerManagerCustomGameClient.BannedPlayer> _bannedPlayers = new Dictionary<PlayerId, BannedPlayerManagerCustomGameClient.BannedPlayer>();

    public static void AddBannedPlayer(PlayerId playerId, int banDueTime) => BannedPlayerManagerCustomGameClient._bannedPlayers[playerId] = new BannedPlayerManagerCustomGameClient.BannedPlayer()
    {
      PlayerId = playerId,
      BanDueTime = banDueTime
    };

    public static bool IsUserBanned(PlayerId playerId) => BannedPlayerManagerCustomGameClient._bannedPlayers.ContainsKey(playerId) && BannedPlayerManagerCustomGameClient._bannedPlayers[playerId].BanDueTime > Environment.TickCount;

    private struct BannedPlayer
    {
      public PlayerId PlayerId { get; set; }

      public int BanDueTime { get; set; }
    }
  }
}
