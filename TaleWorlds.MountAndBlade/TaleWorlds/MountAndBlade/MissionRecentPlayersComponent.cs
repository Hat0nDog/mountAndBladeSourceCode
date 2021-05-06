// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionRecentPlayersComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public class MissionRecentPlayersComponent : MissionNetwork
  {
    private PlayerId _myId;

    public override void AfterStart()
    {
      base.AfterStart();
      MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
      MissionPeer.OnPlayerKilled += new MissionPeer.OnPlayerKilledDelegate(this.OnPlayerKilled);
      this._myId = NetworkMain.GameClient.PlayerID;
    }

    private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam)
    {
      if (!(player.VirtualPlayer.Id != this._myId))
        return;
      RecentPlayersManager.AddOrUpdatePlayerEntry(player.VirtualPlayer.Id, player.UserName, InteractionType.InGameTogether, player.ForcedAvatarIndex);
    }

    private void OnPlayerKilled(MissionPeer killerPeer, MissionPeer killedPeer)
    {
      if (killerPeer == null || killedPeer == null || (killerPeer.Peer == null || killedPeer.Peer == null))
        return;
      PlayerId id1 = killerPeer.Peer.Id;
      PlayerId id2 = killedPeer.Peer.Id;
      if (id1 == this._myId && id2 != this._myId)
      {
        RecentPlayersManager.AddOrUpdatePlayerEntry(id2, killedPeer.Name, InteractionType.Killed, killedPeer.GetNetworkPeer().ForcedAvatarIndex);
      }
      else
      {
        if (!(id2 == this._myId) || !(id1 != this._myId))
          return;
        RecentPlayersManager.AddOrUpdatePlayerEntry(id1, killerPeer.Name, InteractionType.KilledBy, killerPeer.GetNetworkPeer().ForcedAvatarIndex);
      }
    }

    public override void OnRemoveBehaviour()
    {
      MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
      MissionPeer.OnPlayerKilled -= new MissionPeer.OnPlayerKilledDelegate(this.OnPlayerKilled);
      base.OnRemoveBehaviour();
    }
  }
}
