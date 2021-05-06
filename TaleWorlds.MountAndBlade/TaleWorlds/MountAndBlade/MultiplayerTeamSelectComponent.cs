// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerTeamSelectComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.PlatformService;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerTeamSelectComponent : MissionNetwork
  {
    private bool _isInitialized = true;
    private MissionNetworkComponent _missionNetworkComponent;
    private MissionMultiplayerGameModeBase _gameModeServer;
    private Dictionary<Team, IEnumerable<VirtualPlayer>> _friendsPerTeam;
    private Timer _friendsCachingTimer;

    public event MultiplayerTeamSelectComponent.OnSelectingTeamDelegate OnSelectingTeam;

    public event MultiplayerTeamSelectComponent.OnMyTeamChangeDelegate OnMyTeamChange;

    public event MultiplayerTeamSelectComponent.OnUpdateTeamsDelegate OnUpdateTeams;

    public event MultiplayerTeamSelectComponent.OnUpdateFriendsDelegate OnUpdateFriendsPerTeam;

    public bool IsOnTeamSelect => this._isInitialized;

    public bool TeamSelectionEnabled { get; private set; }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._missionNetworkComponent = this.Mission.GetMissionBehaviour<MissionNetworkComponent>();
      this._gameModeServer = this.Mission.GetMissionBehaviour<MissionMultiplayerGameModeBase>();
      if (BannerlordNetwork.LobbyMissionType == LobbyMissionType.Matchmaker)
        this.TeamSelectionEnabled = false;
      else
        this.TeamSelectionEnabled = true;
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (!GameNetwork.IsServer)
        return;
      registerer.Register<TeamChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<TeamChange>(this.HandleClientEventTeamChange));
    }

    public override void AfterStart()
    {
      this._isInitialized = false;
      this._friendsPerTeam = new Dictionary<Team, IEnumerable<VirtualPlayer>>();
      this._friendsCachingTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Application), 1f);
      MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.UpdateTeams);
    }

    public override void OnRemoveBehaviour()
    {
      MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.UpdateTeams);
      this.OnMyTeamChange = (MultiplayerTeamSelectComponent.OnMyTeamChangeDelegate) null;
      base.OnRemoveBehaviour();
    }

    private bool HandleClientEventTeamChange(NetworkCommunicator peer, TeamChange message)
    {
      if (this.TeamSelectionEnabled)
      {
        if (message.AutoAssign)
          this.AutoAssignTeam(peer);
        else
          this.ChangeTeamServer(peer, message.Team);
      }
      return true;
    }

    public void SelectTeam()
    {
      if (this.OnSelectingTeam == null)
        return;
      this.OnSelectingTeam(this.GetDisabledTeams());
    }

    public void UpdateTeams(NetworkCommunicator peer, Team oldTeam, Team newTeam)
    {
      if (this.OnUpdateTeams != null)
        this.OnUpdateTeams();
      if (!GameNetwork.IsMyPeerReady)
        return;
      this.CacheFriendsForTeams();
    }

    public static int GetAutoTeamBalanceDifference(AutoTeamBalanceLimits limit)
    {
      switch (limit)
      {
        case AutoTeamBalanceLimits.Off:
          return 0;
        case AutoTeamBalanceLimits.LimitTo2:
          return 2;
        case AutoTeamBalanceLimits.LimitTo3:
          return 3;
        case AutoTeamBalanceLimits.LimitTo5:
          return 5;
        case AutoTeamBalanceLimits.LimitTo10:
          return 10;
        case AutoTeamBalanceLimits.LimitTo20:
          return 20;
        default:
          return 0;
      }
    }

    public List<Team> GetDisabledTeams()
    {
      List<Team> teamList = new List<Team>();
      if (MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue() == 0)
        return teamList;
      Team myTeam = GameNetwork.IsMyPeerReady ? GameNetwork.MyPeer.GetComponent<MissionPeer>().Team : (Team) null;
      Team[] array = this.Mission.Teams.Where<Team>((Func<Team, bool>) (q => q != this.Mission.SpectatorTeam)).OrderBy<Team, int>((Func<Team, int>) (q =>
      {
        if (myTeam == null)
          return this.Mission.CountTeamPeers(q);
        return q != myTeam ? this.Mission.CountTeamPeers(q) : this.Mission.CountTeamPeers(q) - 1;
      })).ToArray<Team>();
      foreach (Team team in array)
      {
        int num1 = this.Mission.CountTeamPeers(team);
        int num2 = this.Mission.CountTeamPeers(array[0]);
        if (myTeam == team)
          --num1;
        if (myTeam == array[0])
          --num2;
        if (num1 - num2 >= MultiplayerTeamSelectComponent.GetAutoTeamBalanceDifference((AutoTeamBalanceLimits) MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.GetIntValue()))
          teamList.Add(team);
      }
      return teamList;
    }

    private void ChangeTeamServer(NetworkCommunicator networkPeer, Team team)
    {
      MissionPeer component1 = networkPeer.GetComponent<MissionPeer>();
      MissionPeer component2 = networkPeer.GetComponent<MissionPeer>();
      Team team1 = component1.Team;
      if (team1 != null && team1 != this.Mission.SpectatorTeam && (team1 != team && component1.ControlledAgent != null))
        component1.ControlledAgent.Die(new Blow(component1.ControlledAgent.Index)
        {
          DamageType = DamageTypes.Invalid,
          BaseMagnitude = 10000f,
          Position = component1.ControlledAgent.Position
        }, Agent.KillInfo.Stone);
      component1.Team = team;
      if (team != team1)
      {
        if (component2.HasSpawnedAgentVisuals)
        {
          component2.HasSpawnedAgentVisuals = false;
          MBDebug.Print("HasSpawnedAgentVisuals = false for peer: " + component2.Name + " because he just changed his team");
          component2.SpawnCountThisRound = 0;
          Mission.Current.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component2, true);
        }
        if (!this._gameModeServer.IsGameModeHidingAllAgentVisuals && !networkPeer.IsServerPeer)
          this._missionNetworkComponent?.OnPeerSelectedTeam(component1);
        this._gameModeServer.OnPeerChangedTeam(networkPeer, team1, team);
        component2.SpawnTimer.Reset(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.1f);
        component2.WantsToSpawnAsBot = false;
        component2.HasSpawnTimerExpired = false;
      }
      this.UpdateTeams(networkPeer, team1, team);
    }

    public void ChangeTeam(Team team)
    {
      if (team == GameNetwork.MyPeer.GetComponent<MissionPeer>().Team)
        return;
      if (GameNetwork.IsServer)
      {
        Mission.Current.PlayerTeam = team;
        this.ChangeTeamServer(GameNetwork.MyPeer, team);
      }
      else
      {
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
          networkPeer.GetComponent<MissionPeer>()?.ClearAllVisuals();
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new TeamChange(false, team));
        GameNetwork.EndModuleEventAsClient();
      }
      if (this.OnMyTeamChange == null)
        return;
      this.OnMyTeamChange();
    }

    public int GetPlayerCountForTeam(Team team) => VirtualPlayer.Peers<MissionPeer>().Count<MissionPeer>((Func<MissionPeer, bool>) (x => x.Team != null && x.Team == team));

    private void CacheFriendsForTeams()
    {
      this._friendsPerTeam.Clear();
      IEnumerable<PlayerId> friends = FriendListService.GetAllFriendsInAllPlatforms();
      if (friends == null)
        return;
      IEnumerable<MissionPeer> source = VirtualPlayer.Peers<MissionPeer>().Where<MissionPeer>((Func<MissionPeer, bool>) (x => friends.Contains<PlayerId>(x.Peer.Id)));
      foreach (Team team1 in (ReadOnlyCollection<Team>) this.Mission.Teams)
      {
        Team team = team1;
        if (team != null)
          this._friendsPerTeam.Add(team, source.Where<MissionPeer>((Func<MissionPeer, bool>) (x => x.Team == team)).Select<MissionPeer, VirtualPlayer>((Func<MissionPeer, VirtualPlayer>) (x => x.Peer)));
      }
      if (this.OnUpdateFriendsPerTeam == null)
        return;
      this.OnUpdateFriendsPerTeam();
    }

    public IEnumerable<VirtualPlayer> GetFriendsForTeam(Team team) => this._friendsPerTeam.ContainsKey(team) ? this._friendsPerTeam[team] : (IEnumerable<VirtualPlayer>) new List<VirtualPlayer>();

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if (this._isInitialized || !GameNetwork.IsMyPeerReady)
        return;
      this._isInitialized = true;
      if (GameNetwork.MyPeer.GetComponent<MissionPeer>().Team != null)
        return;
      this.SelectTeam();
    }

    public void AutoAssignTeam(NetworkCommunicator peer)
    {
      if (GameNetwork.IsServer)
      {
        List<Team> disabledTeams = this.GetDisabledTeams();
        List<Team> list = this.Mission.Teams.Where<Team>((Func<Team, bool>) (x => !disabledTeams.Contains(x) && x.Side != BattleSideEnum.None)).ToList<Team>();
        Team team;
        if (list.Count > 1)
        {
          int[] numArray = new int[list.Count];
          foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
          {
            MissionPeer component = networkPeer.GetComponent<MissionPeer>();
            if (component?.Team != null)
            {
              for (int index = 0; index < list.Count; ++index)
              {
                if (component.Team == list[index])
                  ++numArray[index];
              }
            }
          }
          int num = -1;
          int index1 = -1;
          for (int index2 = 0; index2 < numArray.Length; ++index2)
          {
            if (index1 < 0 || numArray[index2] < num)
            {
              index1 = index2;
              num = numArray[index2];
            }
          }
          team = list[index1];
        }
        else
          team = list[0];
        if (!peer.IsMine)
          this.ChangeTeamServer(peer, team);
        else
          this.ChangeTeam(team);
      }
      else
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new TeamChange(true, (Team) null));
        GameNetwork.EndModuleEventAsClient();
        if (this.OnMyTeamChange == null)
          return;
        this.OnMyTeamChange();
      }
    }

    public delegate void OnSelectingTeamDelegate(List<Team> disableTeams);

    public delegate void OnMyTeamChangeDelegate();

    public delegate void OnUpdateTeamsDelegate();

    public delegate void OnUpdateFriendsDelegate();
  }
}
