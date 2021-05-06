// Decompiled with JetBrains decompiler
// Type: TaleWorlds.PlayerServices.Avatar.SteamAvatarService
// Assembly: TaleWorlds.PlayerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FC0579B2-CBA1-4D57-8C1B-BA25E3FC058C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.PlayerServices.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaleWorlds.PlayerServices.Avatar
{
  public class SteamAvatarService : ApiAvatarServiceBase
  {
    private const int FetchTaskWaitTime = 3000;
    private const string SteamWebApiKey = "820D6EC50E6AAE61E460EA207D8966F7";
    private const int MaxAccountsPerRequest = 100;

    protected override async Task FetchAvatars()
    {
      SteamAvatarService steamAvatarService = this;
      await Task.Delay(3000);
      lock (steamAvatarService.WaitingAccounts)
      {
        if (steamAvatarService.WaitingAccounts.Count < 1)
          return;
        if (steamAvatarService.WaitingAccounts.Count <= 100)
        {
          steamAvatarService.InProgressAccounts = steamAvatarService.WaitingAccounts;
          steamAvatarService.WaitingAccounts = new List<(ulong, AvatarData)>();
        }
        else
        {
          steamAvatarService.InProgressAccounts = steamAvatarService.WaitingAccounts.GetRange(0, 100);
          steamAvatarService.WaitingAccounts.RemoveRange(0, 100);
        }
      }
      string address = "http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=820D6EC50E6AAE61E460EA207D8966F7&steamids=" + string.Join<ulong>(",", steamAvatarService.InProgressAccounts.Select<(ulong, AvatarData), ulong>((Func<(ulong, AvatarData), ulong>) (a => a.accountId)));
      SteamAvatarService.SteamPlayers steamPlayers = (SteamAvatarService.SteamPlayers) null;
      try
      {
        SteamAvatarService.GetPlayerSummariesResult playerSummariesResult = JsonConvert.DeserializeObject<SteamAvatarService.GetPlayerSummariesResult>(await new TimeoutWebClient().DownloadStringTaskAsync(address));
        if (playerSummariesResult?.response?.players != null)
        {
          if (playerSummariesResult.response.players.Length != 0)
            steamPlayers = playerSummariesResult.response;
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
      }
      if (steamPlayers == null || steamPlayers.players.Length < 1)
      {
        foreach ((ulong, AvatarData) inProgressAccount in steamAvatarService.InProgressAccounts)
          inProgressAccount.Item2.SetFailed();
      }
      else
      {
        List<Task> taskList = new List<Task>();
        foreach ((ulong accountId2, AvatarData avatarData2) in steamAvatarService.InProgressAccounts)
        {
          string local_12 = string.Concat((object) accountId2);
          string local_13 = (string) null;
          foreach (SteamAvatarService.SteamPlayerSummary item_0 in steamPlayers.players)
          {
            if (item_0.steamid == local_12)
            {
              local_13 = item_0.avatarfull;
              break;
            }
          }
          if (!string.IsNullOrWhiteSpace(local_13))
            taskList.Add(steamAvatarService.UpdateAvatarImageData(accountId2, local_13, avatarData2));
          else
            avatarData2.SetFailed();
        }
        if (taskList.Count <= 0)
          return;
        await Task.WhenAll((IEnumerable<Task>) taskList);
      }
    }

    private async Task UpdateAvatarImageData(
      ulong accountId,
      string avatarUrl,
      AvatarData avatarData)
    {
      SteamAvatarService steamAvatarService = this;
      if (string.IsNullOrWhiteSpace(avatarUrl))
        return;
      byte[] image = await new TimeoutWebClient().DownloadDataTaskAsync(avatarUrl);
      if (image == null || image.Length == 0)
        return;
      avatarData.SetImageData(image);
      lock (steamAvatarService.AvatarImageCache)
        steamAvatarService.AvatarImageCache[accountId] = avatarData;
    }

    private class GetPlayerSummariesResult
    {
      public SteamAvatarService.SteamPlayers response { get; set; }
    }

    private class SteamPlayers
    {
      public SteamAvatarService.SteamPlayerSummary[] players { get; set; }
    }

    private class SteamPlayerSummary
    {
      public string avatar { get; set; }

      public string avatarfull { get; set; }

      public string avatarmedium { get; set; }

      public int communityvisibilitystate { get; set; }

      public int lastlogoff { get; set; }

      public string personaname { get; set; }

      public int personastate { get; set; }

      public int personastateflags { get; set; }

      public string primaryclanid { get; set; }

      public int profilestate { get; set; }

      public string profileurl { get; set; }

      public string realname { get; set; }

      public string steamid { get; set; }

      public int timecreated { get; set; }
    }
  }
}
