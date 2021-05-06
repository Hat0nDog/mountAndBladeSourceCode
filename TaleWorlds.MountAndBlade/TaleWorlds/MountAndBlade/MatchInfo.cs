// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MatchInfo
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  public class MatchInfo
  {
    public string MatchId { get; set; }

    public string MatchType { get; set; }

    public string GameType { get; set; }

    public DateTime MatchDate { get; set; }

    public int WinnerTeam { get; set; }

    public string Faction1 { get; set; }

    public string Faction2 { get; set; }

    public int DefenderScore { get; set; }

    public int AttackerScore { get; set; }

    public List<PlayerInfo> Players { get; set; }

    public MatchInfo() => this.Players = new List<PlayerInfo>();

    private PlayerInfo TryGetPlayer(string id)
    {
      foreach (PlayerInfo player in this.Players)
      {
        if (player.PlayerId == id)
          return player;
      }
      return (PlayerInfo) null;
    }

    public void AddOrUpdatePlayer(string id, string username, int forcedIndex, int teamNo)
    {
      PlayerInfo player = this.TryGetPlayer(id);
      if (player == null)
        this.Players.Add(new PlayerInfo()
        {
          PlayerId = id,
          Username = username,
          ForcedIndex = forcedIndex,
          TeamNo = teamNo
        });
      else
        player.TeamNo = teamNo;
    }
  }
}
