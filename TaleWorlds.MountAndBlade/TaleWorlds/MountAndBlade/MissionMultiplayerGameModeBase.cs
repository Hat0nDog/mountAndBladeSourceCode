// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerGameModeBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MissionMultiplayerGameModeBase : MissionNetwork
  {
    public const int GoldCap = 2000;
    public const float PerkTickPeriod = 1f;
    private float _lastPerkTickTime;
    protected MissionLobbyComponent MissionLobbyComponent;
    protected MultiplayerGameNotificationsComponent NotificationsComponent;
    public MultiplayerRoundController RoundController;
    public MultiplayerWarmupComponent WarmupComponent;
    public MultiplayerTimerComponent TimerComponent;
    protected MissionMultiplayerGameModeBaseClient GameModeBaseClient;

    public abstract bool IsGameModeHidingAllAgentVisuals { get; }

    public SpawnComponent SpawnComponent { get; private set; }

    public abstract MissionLobbyComponent.MultiplayerGameType GetMissionType();

    public virtual bool CheckIfOvertime() => false;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this.MissionLobbyComponent = this.Mission.GetMissionBehaviour<MissionLobbyComponent>();
      this.GameModeBaseClient = this.Mission.GetMissionBehaviour<MissionMultiplayerGameModeBaseClient>();
      this.NotificationsComponent = this.Mission.GetMissionBehaviour<MultiplayerGameNotificationsComponent>();
      this.RoundController = this.Mission.GetMissionBehaviour<MultiplayerRoundController>();
      this.WarmupComponent = this.Mission.GetMissionBehaviour<MultiplayerWarmupComponent>();
      this.TimerComponent = this.Mission.GetMissionBehaviour<MultiplayerTimerComponent>();
      this.SpawnComponent = Mission.Current.GetMissionBehaviour<SpawnComponent>();
      this._lastPerkTickTime = Mission.Current.Time;
    }

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if ((double) Mission.Current.Time - (double) this._lastPerkTickTime < 1.0)
        return;
      this._lastPerkTickTime = Mission.Current.Time;
      MPPerkObject.TickAllPeerPerks((int) ((double) this._lastPerkTickTime / 1.0));
    }

    public virtual bool CheckForWarmupEnd() => false;

    public virtual bool CheckForRoundEnd() => false;

    public virtual bool CheckForMatchEnd() => false;

    public virtual bool UseCultureSelection() => false;

    public virtual bool UseRoundController() => false;

    public virtual Team GetWinnerTeam() => (Team) null;

    public virtual void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
    {
    }

    public override void OnMissionRestart()
    {
      base.OnMissionRestart();
      this.ClearPeerCounts();
      this._lastPerkTickTime = Mission.Current.Time;
    }

    public void ClearPeerCounts()
    {
      foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
      {
        peer.AssistCount = 0;
        peer.DeathCount = 0;
        peer.KillCount = 0;
        peer.Score = 0;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new KillDeathCountChange(peer.GetNetworkPeer(), (NetworkCommunicator) null, peer.KillCount, peer.AssistCount, peer.DeathCount, peer.Score));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
    }

    public bool ShouldSpawnVisualsForServer(NetworkCommunicator spawningNetworkPeer)
    {
      if (GameNetwork.IsDedicatedServer)
        return false;
      NetworkCommunicator myPeer = GameNetwork.MyPeer;
      MissionPeer missionPeer = myPeer != null ? myPeer.GetComponent<MissionPeer>() : (MissionPeer) null;
      if (missionPeer == null)
        return false;
      MissionPeer component = spawningNetworkPeer.GetComponent<MissionPeer>();
      return !this.IsGameModeHidingAllAgentVisuals && component.Team == missionPeer.Team || spawningNetworkPeer.IsServerPeer;
    }

    public void HandleAgentVisualSpawning(
      NetworkCommunicator spawningNetworkPeer,
      AgentBuildData spawningAgentBuildData,
      int troopCountInFormation = 0)
    {
      MissionPeer component = spawningNetworkPeer.GetComponent<MissionPeer>();
      component.HasSpawnedAgentVisuals = true;
      component.EquipmentUpdatingExpired = false;
      if (!this.IsGameModeHidingAllAgentVisuals)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new CreateAgentVisuals(spawningNetworkPeer, spawningAgentBuildData, component.SelectedTroopIndex, troopCountInFormation));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, spawningNetworkPeer);
      }
      else
      {
        if (spawningNetworkPeer.IsServerPeer)
          return;
        GameNetwork.BeginModuleEventAsServer(spawningNetworkPeer);
        GameNetwork.WriteMessage((GameNetworkMessage) new CreateAgentVisuals(spawningNetworkPeer, spawningAgentBuildData, component.SelectedTroopIndex, troopCountInFormation));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    public virtual bool AllowCustomPlayerBanners() => true;

    public virtual int GetScoreForKill(Agent killedAgent) => 3;

    public virtual int GetScoreForAssist(Agent killedAgent) => 1;

    public virtual float GetTroopNumberMultiplierForMissingPlayer(MissionPeer spawningPeer) => 1f;

    public int GetCurrentGoldForPeer(MissionPeer peer) => peer.Representative.Gold;

    public void ChangeCurrentGoldForPeer(MissionPeer peer, int newAmount)
    {
      if (newAmount >= 0)
        newAmount = MBMath.ClampInt(newAmount, 0, 2000);
      if (peer.Peer.Communicator.IsConnectionActive)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SyncGoldsForSkirmish(peer.Peer, newAmount));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      if (this.GameModeBaseClient == null)
        return;
      this.GameModeBaseClient.OnGoldAmountChangedForRepresentative(peer.Representative, newAmount);
    }

    protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
      if (!this.GameModeBaseClient.IsGameModeUsingGold)
        return;
      foreach (NetworkCommunicator networkPeer1 in GameNetwork.NetworkPeers)
      {
        if (networkPeer1 != networkPeer)
        {
          MissionRepresentativeBase component = networkPeer1.GetComponent<MissionRepresentativeBase>();
          if (component != null)
          {
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new SyncGoldsForSkirmish(component.Peer, component.Gold));
            GameNetwork.EndModuleEventAsServer();
          }
        }
      }
    }
  }
}
