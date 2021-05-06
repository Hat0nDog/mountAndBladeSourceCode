// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MatchHistory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public static class MatchHistory
  {
    private const int MaxMatchCountPerMatchType = 10;
    private const string HistoryDirectoryName = "Data";
    private const string HistoryFileName = "History.json";
    private static bool IsHistoryCacheDirty = true;
    private static List<MatchInfo> _matches = new List<MatchInfo>();

    private static string HistoryFilePath => Common.PlatformFileHelper.DocumentsPath + "\\Mount and Blade II Bannerlord\\Data\\Native\\History.json";

    public static MBReadOnlyList<MatchInfo> Matches { get; private set; }

    static MatchHistory() => MatchHistory.Matches = new MBReadOnlyList<MatchInfo>(MatchHistory._matches);

    public static async Task LoadMatchHistory()
    {
      if (!MatchHistory.IsHistoryCacheDirty)
        return;
      if (Common.PlatformFileHelper.FileExists(MatchHistory.HistoryFilePath))
      {
        try
        {
          MatchHistory._matches = JsonConvert.DeserializeObject<List<MatchInfo>>(await Common.PlatformFileHelper.GetFileContentAsync(MatchHistory.HistoryFilePath));
          if (MatchHistory._matches != null)
          {
            MatchHistory.Matches = new MBReadOnlyList<MatchInfo>(MatchHistory._matches);
          }
          else
          {
            MatchHistory._matches = new List<MatchInfo>();
            MatchHistory.Matches = new MBReadOnlyList<MatchInfo>(MatchHistory._matches);
            throw new Exception("_matches were null.");
          }
        }
        catch (Exception ex1)
        {
          try
          {
            Common.PlatformFileHelper.DeleteFile(MatchHistory.HistoryFilePath);
          }
          catch (Exception ex2)
          {
          }
        }
      }
      MatchHistory.IsHistoryCacheDirty = false;
    }

    public static async Task<MBReadOnlyList<MatchInfo>> GetMatches()
    {
      await MatchHistory.LoadMatchHistory();
      return MatchHistory.Matches;
    }

    public static void AddMatch(MatchInfo match)
    {
      if (MatchHistory.TryGetMatchInfo(match.MatchId, out MatchInfo _))
      {
        for (int index = 0; index < MatchHistory._matches.Count; ++index)
        {
          if (MatchHistory._matches[index].MatchId == match.MatchId)
            MatchHistory._matches[index] = match;
        }
      }
      else
      {
        int matchTypeCount = MatchHistory.GetMatchTypeCount(match.MatchType);
        if (matchTypeCount >= 10)
          MatchHistory.RemoveMatches(match.MatchType, matchTypeCount - 10 + 1);
        MatchHistory._matches.Add(match);
      }
    }

    public static bool TryGetMatchInfo(string matchId, out MatchInfo matchInfo)
    {
      matchInfo = (MatchInfo) null;
      foreach (MatchInfo match in MatchHistory._matches)
      {
        if (match.MatchId == matchId)
        {
          matchInfo = match;
          return true;
        }
      }
      return false;
    }

    private static void RemoveMatches(string matchType, int numMatchToRemove)
    {
      for (int index = 0; index < numMatchToRemove; ++index)
      {
        MatchInfo oldestMatch = MatchHistory.GetOldestMatch(matchType);
        MatchHistory._matches.Remove(oldestMatch);
      }
    }

    private static MatchInfo GetOldestMatch(string matchType)
    {
      DateTime dateTime = DateTime.MaxValue;
      MatchInfo matchInfo = (MatchInfo) null;
      foreach (MatchInfo match in MatchHistory._matches)
      {
        if (match.MatchDate < dateTime)
        {
          dateTime = match.MatchDate;
          matchInfo = match;
        }
      }
      return matchInfo;
    }

    public static void Serialize()
    {
      try
      {
        Common.PlatformFileHelper.Serialize(MatchHistory.HistoryFilePath, (object) MatchHistory._matches);
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
      }
    }

    private static int GetMatchTypeCount(string category)
    {
      int num = 0;
      foreach (MatchInfo match in MatchHistory._matches)
      {
        if (match.MatchType == category)
          ++num;
      }
      return num;
    }
  }
}
