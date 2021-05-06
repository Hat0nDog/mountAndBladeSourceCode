// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BannedPlayerManagerCustomGameServer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public static class BannedPlayerManagerCustomGameServer
  {
    private static object _bannedPlayersWriteLock = new object();
    private static Dictionary<PlayerId, BannedPlayerManagerCustomGameServer.BannedPlayer> _bannedPlayers = new Dictionary<PlayerId, BannedPlayerManagerCustomGameServer.BannedPlayer>();

    public static void AddBannedPlayer(PlayerId playerId, string playerName, int banDueTime)
    {
      if (BannedPlayerManagerCustomGameServer._bannedPlayers.ContainsKey(playerId))
      {
        lock (BannedPlayerManagerCustomGameServer._bannedPlayersWriteLock)
        {
          StreamWriter streamWriter = new StreamWriter("bannedUsers.txt", true);
          streamWriter.WriteLine(playerId.ToString() + "\t" + playerName);
          streamWriter.Close();
        }
      }
      BannedPlayerManagerCustomGameServer._bannedPlayers[playerId] = new BannedPlayerManagerCustomGameServer.BannedPlayer()
      {
        PlayerId = playerId,
        BanDueTime = banDueTime,
        PlayerName = playerName
      };
    }

    public static bool IsUserBanned(PlayerId playerId)
    {
      if (!BannedPlayerManagerCustomGameServer._bannedPlayers.ContainsKey(playerId))
        return false;
      BannedPlayerManagerCustomGameServer.BannedPlayer bannedPlayer = BannedPlayerManagerCustomGameServer._bannedPlayers[playerId];
      if (bannedPlayer.BanDueTime == 0)
        return true;
      bannedPlayer = BannedPlayerManagerCustomGameServer._bannedPlayers[playerId];
      return bannedPlayer.BanDueTime > Environment.TickCount;
    }

    public static void LoadPlayers()
    {
      lock (BannedPlayerManagerCustomGameServer._bannedPlayersWriteLock)
      {
        StreamReader streamReader = new StreamReader("bannedUsers.txt");
        while (streamReader.Peek() > 0)
        {
          string[] strArray = streamReader.ReadLine().Split('\t');
          PlayerId key = PlayerId.FromString(strArray[0]);
          string str = strArray[1];
          BannedPlayerManagerCustomGameServer._bannedPlayers[key] = new BannedPlayerManagerCustomGameServer.BannedPlayer()
          {
            PlayerId = key,
            PlayerName = str,
            BanDueTime = 0
          };
        }
        streamReader.Close();
      }
    }

    private struct BannedPlayer
    {
      public PlayerId PlayerId { get; set; }

      public string PlayerName { get; set; }

      public int BanDueTime { get; set; }
    }
  }
}
