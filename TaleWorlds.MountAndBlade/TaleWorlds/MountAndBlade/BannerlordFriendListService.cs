// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BannerlordFriendListService
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public class BannerlordFriendListService : IFriendListService
  {
    protected List<FriendInfo> Friends;

    public event Action<PlayerId> OnUserStatusChanged;

    public event Action<PlayerId> OnFriendRemoved;

    public event Action OnFriendListChanged;

    bool IFriendListService.InGameStatusFetchable => false;

    bool IFriendListService.AllowsFriendOperations => true;

    bool IFriendListService.CanInvitePlayersToGame => PlatformServices.InvitationServices != null;

    public BannerlordFriendListService() => this.Friends = new List<FriendInfo>();

    string IFriendListService.GetServiceName() => "TaleWorlds";

    IEnumerable<PlayerId> IFriendListService.GetPendingRequests() => this.Friends.Where<FriendInfo>((Func<FriendInfo, bool>) (f => f.Status == FriendStatus.Pending)).Select<FriendInfo, PlayerId>((Func<FriendInfo, PlayerId>) (f => f.Id));

    IEnumerable<PlayerId> IFriendListService.GetReceivedRequests() => this.Friends.Where<FriendInfo>((Func<FriendInfo, bool>) (f => f.Status == FriendStatus.Received)).Select<FriendInfo, PlayerId>((Func<FriendInfo, PlayerId>) (f => f.Id));

    IEnumerable<PlayerId> IFriendListService.GetAllFriends() => this.Friends.Where<FriendInfo>((Func<FriendInfo, bool>) (f => f.Status == FriendStatus.Accepted)).Select<FriendInfo, PlayerId>((Func<FriendInfo, PlayerId>) (f => f.Id));

    Task<bool> IFriendListService.GetUserOnlineStatus(PlayerId providedId)
    {
      foreach (FriendInfo friend in this.Friends)
      {
        if (friend.Id.Equals((object) providedId))
          return Task.FromResult<bool>(friend.IsOnline);
      }
      return Task.FromResult<bool>(false);
    }

    Task<bool> IFriendListService.IsPlayingThisGame(PlayerId providedId) => ((IFriendListService) this).GetUserOnlineStatus(providedId);

    Task<string> IFriendListService.GetUserName(PlayerId providedId)
    {
      foreach (FriendInfo friend in this.Friends)
      {
        if (friend.Id.Equals((object) providedId))
          return Task.FromResult<string>(friend.Name);
      }
      return Task.FromResult<string>((string) null);
    }

    Task<PlayerId> IFriendListService.GetUserWithName(string name)
    {
      foreach (FriendInfo friend in this.Friends)
      {
        if (friend.Name == name)
          return Task.FromResult<PlayerId>(friend.Id);
      }
      return Task.FromResult<PlayerId>(new PlayerId());
    }

    public void OnFriendListReceived(FriendInfo[] friends)
    {
      List<FriendInfo> friends1 = this.Friends;
      this.Friends = new List<FriendInfo>((IEnumerable<FriendInfo>) friends);
      List<PlayerId> playerIdList = (List<PlayerId>) null;
      bool flag = false;
      foreach (FriendInfo friend1 in this.Friends)
      {
        FriendInfo friend = friend1;
        int index = friends1.FindIndex((Predicate<FriendInfo>) (o => o.Id.Equals((object) friend.Id)));
        if (index < 0)
        {
          flag = true;
        }
        else
        {
          FriendInfo friendInfo = friends1[index];
          friends1.RemoveAt(index);
          if (friendInfo.Status != friend.Status)
            flag = true;
          else if (friendInfo.IsOnline != friend.IsOnline)
          {
            if (playerIdList == null)
              playerIdList = new List<PlayerId>();
            playerIdList.Add(friendInfo.Id);
          }
        }
        if (flag)
          break;
      }
      if (flag)
      {
        Action friendListChanged = this.OnFriendListChanged;
        if (friendListChanged == null)
          return;
        friendListChanged();
      }
      else
      {
        if (friends1.Count > 0)
        {
          foreach (FriendInfo friendInfo in friends1)
          {
            Action<PlayerId> onFriendRemoved = this.OnFriendRemoved;
            if (onFriendRemoved != null)
              onFriendRemoved(friendInfo.Id);
          }
        }
        if (playerIdList == null)
          return;
        foreach (PlayerId playerId in playerIdList)
        {
          Action<PlayerId> userStatusChanged = this.OnUserStatusChanged;
          if (userStatusChanged != null)
            userStatusChanged(playerId);
        }
      }
    }
  }
}
