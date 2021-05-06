// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.RecentPlayersFriendListService
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public class RecentPlayersFriendListService : BannerlordFriendListService, IFriendListService
  {
    string IFriendListService.GetServiceName() => new TextObject("{=*}Recently Played Players").ToString();

    IEnumerable<PlayerId> IFriendListService.GetAllFriends() => RecentPlayersManager.GetPlayersOrdered();
  }
}
