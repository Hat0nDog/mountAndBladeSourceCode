// Decompiled with JetBrains decompiler
// Type: TaleWorlds.PlayerServices.Avatar.ApiAvatarServiceBase
// Assembly: TaleWorlds.PlayerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FC0579B2-CBA1-4D57-8C1B-BA25E3FC058C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.PlayerServices.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaleWorlds.PlayerServices.Avatar
{
  public abstract class ApiAvatarServiceBase : IAvatarService
  {
    protected Dictionary<ulong, AvatarData> AvatarImageCache { get; }

    protected List<(ulong accountId, AvatarData avatarData)> WaitingAccounts { get; set; }

    protected List<(ulong accountId, AvatarData avatarData)> InProgressAccounts { get; set; }

    protected Task FetchAvatarsTask { get; set; }

    protected ApiAvatarServiceBase()
    {
      this.AvatarImageCache = new Dictionary<ulong, AvatarData>();
      this.WaitingAccounts = new List<(ulong, AvatarData)>();
      this.InProgressAccounts = new List<(ulong, AvatarData)>();
      this.FetchAvatarsTask = (Task) null;
    }

    public AvatarData GetPlayerAvatar(PlayerId playerId)
    {
      ulong part4 = playerId.Part4;
      AvatarData avatarData;
      lock (this.AvatarImageCache)
      {
        if (this.AvatarImageCache.TryGetValue(part4, out avatarData) && avatarData.Status != AvatarData.DataStatus.Failed)
          return avatarData;
        if (this.AvatarImageCache.Count > 300)
          this.AvatarImageCache.Clear();
        avatarData = new AvatarData();
        this.AvatarImageCache[part4] = avatarData;
      }
      lock (this.WaitingAccounts)
        this.WaitingAccounts.Add((part4, avatarData));
      this.CheckWaitingAccounts();
      return avatarData;
    }

    private void CheckWaitingAccounts()
    {
      lock (this.WaitingAccounts)
      {
        if (this.FetchAvatarsTask != null && !this.FetchAvatarsTask.IsCompleted)
          return;
        Task fetchAvatarsTask = this.FetchAvatarsTask;
        if ((fetchAvatarsTask != null ? (fetchAvatarsTask.IsFaulted ? 1 : 0) : 0) != 0)
        {
          this.FetchAvatarsTask = (Task) null;
          foreach ((ulong _, AvatarData avatarData4) in this.InProgressAccounts)
          {
            if (avatarData4.Status == AvatarData.DataStatus.NotReady)
              avatarData4.SetFailed();
          }
        }
        if (this.WaitingAccounts.Count <= 0)
          return;
        this.FetchAvatarsTask = this.FetchAvatars();
        Task.Run((Func<Task>) (async () =>
        {
          await this.FetchAvatarsTask;
          this.CheckWaitingAccounts();
        }));
      }
    }

    protected abstract Task FetchAvatars();
  }
}
