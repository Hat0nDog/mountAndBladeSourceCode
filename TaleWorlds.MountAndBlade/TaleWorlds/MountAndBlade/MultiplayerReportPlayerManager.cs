// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerReportPlayerManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public static class MultiplayerReportPlayerManager
  {
    private static Dictionary<PlayerId, int> _reportsPerPlayer = new Dictionary<PlayerId, int>();
    private const int _maxReportsPerPlayer = 3;

    public static event Action<string, PlayerId, bool> ReportHandlers;

    public static void RequestReportPlayer(
      string gameId,
      PlayerId playerId,
      bool isRequestedFromMission)
    {
      MultiplayerReportPlayerManager.IncrementReportOfPlayer(playerId);
      Action<string, PlayerId, bool> reportHandlers = MultiplayerReportPlayerManager.ReportHandlers;
      if (reportHandlers == null)
        return;
      reportHandlers(gameId, playerId, isRequestedFromMission);
    }

    public static bool IsPlayerReportedOverLimit(PlayerId player) => MultiplayerReportPlayerManager._reportsPerPlayer.ContainsKey(player) && MultiplayerReportPlayerManager._reportsPerPlayer[player] == 3;

    private static void IncrementReportOfPlayer(PlayerId player)
    {
      if (MultiplayerReportPlayerManager._reportsPerPlayer.ContainsKey(player))
        MultiplayerReportPlayerManager._reportsPerPlayer[player]++;
      else
        MultiplayerReportPlayerManager._reportsPerPlayer.Add(player, 1);
    }
  }
}
