// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerRoundController
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerRoundController : MissionNetwork, IRoundComponent, IMissionBehavior
  {
    private MissionMultiplayerGameModeBase _gameModeServer;
    private int _roundCount;
    private BattleSideEnum _roundWinner;
    private RoundEndReason _roundEndReason;
    private MissionLobbyComponent _missionLobbyComponent;
    private bool _roundTimeOver;
    private MissionTime _currentRoundStateStartTime;
    private bool _equipmentUpdateDisabled = true;

    public event Action OnRoundStarted;

    public event Action OnPreparationEnded;

    public event Action OnPreRoundEnding;

    public event Action OnRoundEnding;

    public event Action OnPostRoundEnded;

    public event Action OnCurrentRoundStateChanged;

    public int RoundCount
    {
      get => this._roundCount;
      set
      {
        if (this._roundCount == value)
          return;
        this._roundCount = value;
        if (!GameNetwork.IsServer)
          return;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new RoundCountChange(this._roundCount));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
    }

    public BattleSideEnum RoundWinner
    {
      get => this._roundWinner;
      set
      {
        if (this._roundWinner == value)
          return;
        this._roundWinner = value;
        if (!GameNetwork.IsServer)
          return;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new RoundWinnerChange(value));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
    }

    public RoundEndReason RoundEndReason
    {
      get => this._roundEndReason;
      set
      {
        if (this._roundEndReason == value)
          return;
        this._roundEndReason = value;
        if (!GameNetwork.IsServer)
          return;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new RoundEndReasonChange(value));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
    }

    public bool IsMatchEnding { get; private set; }

    public float LastRoundEndRemainingTime { get; private set; }

    public float RemainingRoundTime => this._gameModeServer.TimerComponent.GetRemainingTime(false);

    public MultiplayerRoundState CurrentRoundState { get; private set; }

    public bool IsRoundInProgress => this.CurrentRoundState == MultiplayerRoundState.InProgress;

    public void EnableEquipmentUpdate() => this._equipmentUpdateDisabled = false;

    public override void AfterStart()
    {
      GameNetwork.AddNetworkHandler((IUdpNetworkHandler) this);
      this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
      if (GameNetwork.IsServerOrRecorder)
        this._gameModeServer = Mission.Current.GetMissionBehaviour<MissionMultiplayerGameModeBase>();
      this._missionLobbyComponent = Mission.Current.GetMissionBehaviour<MissionLobbyComponent>();
      this._roundCount = 0;
      this._gameModeServer.TimerComponent.StartTimerAsServer(8f);
    }

    private void EndRound()
    {
      if (this.OnPreRoundEnding != null)
        this.OnPreRoundEnding();
      this.ChangeRoundState(MultiplayerRoundState.Ending);
      this._gameModeServer.TimerComponent.StartTimerAsServer(3f);
      this._roundTimeOver = false;
      if (this.OnRoundEnding == null)
        return;
      this.OnRoundEnding();
    }

    private bool CheckPostEndRound() => this._gameModeServer.TimerComponent.CheckIfTimerPassed();

    private bool CheckPostMatchEnd() => this._gameModeServer.TimerComponent.CheckIfTimerPassed();

    private void PostRoundEnd()
    {
      this._gameModeServer.TimerComponent.StartTimerAsServer(5f);
      this.ChangeRoundState(MultiplayerRoundState.Ended);
      if (this._roundCount == MultiplayerOptions.OptionType.RoundTotal.GetIntValue() || this.CheckForMatchEndEarly() || !this.HasEnoughCharactersOnBothSides())
        this.IsMatchEnding = true;
      if (this.OnPostRoundEnded == null)
        return;
      this.OnPostRoundEnded();
    }

    private void PostMatchEnd()
    {
      this._gameModeServer.TimerComponent.StartTimerAsServer(5f);
      this.ChangeRoundState(MultiplayerRoundState.MatchEnded);
      this._missionLobbyComponent.SetStateEndingAsServer();
    }

    public override void OnRemoveBehaviour() => GameNetwork.RemoveNetworkHandler((IUdpNetworkHandler) this);

    private void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
      GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
      if (GameNetwork.IsClient || !GameNetwork.IsServer)
        return;
      handlerRegisterer.Register<CultureVoteClient>(new GameNetworkMessage.ClientMessageHandlerDelegate<CultureVoteClient>(this.HandleClientEventCultureSelect));
    }

    protected override void OnUdpNetworkHandlerClose() => this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);

    public override void OnPreDisplayMissionTick(float dt)
    {
      if (GameNetwork.IsServer)
      {
        if (this._missionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.WaitingFirstPlayers)
          return;
        if (!this.IsMatchEnding && this._missionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Ending && (this.CurrentRoundState == MultiplayerRoundState.WaitingForPlayers || this.CurrentRoundState == MultiplayerRoundState.Ended))
        {
          if (this.CheckForNewRound())
          {
            this.BeginNewRound();
          }
          else
          {
            if (!this.IsMatchEnding)
              return;
            this.PostMatchEnd();
          }
        }
        else if (this.CurrentRoundState == MultiplayerRoundState.Preparation)
        {
          if (!this.CheckForPreparationEnd())
            return;
          this.EndPreparation();
          this.StartSpawning(this._equipmentUpdateDisabled);
        }
        else if (this.CurrentRoundState == MultiplayerRoundState.InProgress)
        {
          if (!this.CheckForRoundEnd())
            return;
          this.EndRound();
        }
        else if (this.CurrentRoundState == MultiplayerRoundState.Ending)
        {
          if (!this.CheckPostEndRound())
            return;
          this.PostRoundEnd();
        }
        else
        {
          if (this.CurrentRoundState != MultiplayerRoundState.Ended || !this.IsMatchEnding || !this.CheckPostMatchEnd())
            return;
          this.PostMatchEnd();
        }
      }
      else
        this._gameModeServer.TimerComponent.CheckIfTimerPassed();
    }

    private void ChangeRoundState(MultiplayerRoundState newRoundState)
    {
      if (this.CurrentRoundState == newRoundState)
        return;
      if (this.CurrentRoundState == MultiplayerRoundState.InProgress)
        this.LastRoundEndRemainingTime = this.RemainingRoundTime;
      this.CurrentRoundState = newRoundState;
      this._currentRoundStateStartTime = MissionTime.Now;
      Action roundStateChanged = this.OnCurrentRoundStateChanged;
      if (roundStateChanged != null)
        roundStateChanged();
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new RoundStateChange(newRoundState, this._currentRoundStateStartTime.NumberOfTicks, MBMath.Ceiling(this.LastRoundEndRemainingTime)));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
    }

    public bool HandleClientEventCultureSelect(NetworkCommunicator peer, CultureVoteClient message)
    {
      peer.GetComponent<MissionPeer>().HandleVoteChange(message.VotedType, message.VotedCulture);
      return true;
    }

    private bool CheckForRoundEnd()
    {
      if (!this._roundTimeOver)
        this._roundTimeOver = this._gameModeServer.TimerComponent.CheckIfTimerPassed();
      return !this._gameModeServer.CheckIfOvertime() && this._roundTimeOver || this._gameModeServer.CheckForRoundEnd();
    }

    private bool CheckForNewRound()
    {
      if (this.CurrentRoundState != MultiplayerRoundState.WaitingForPlayers && !this._gameModeServer.TimerComponent.CheckIfTimerPassed())
        return false;
      int[] numArray = new int[2];
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (networkPeer.IsSynchronized && (component?.Team != null && component.Team.Side != BattleSideEnum.None))
          ++numArray[(int) component.Team.Side];
      }
      if (((IEnumerable<int>) numArray).Sum() >= MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart.GetIntValue() || this.RoundCount != 0)
        return true;
      this.IsMatchEnding = true;
      return false;
    }

    private bool HasEnoughCharactersOnBothSides() => ((MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue() > 0 ? 1 : (GameNetwork.NetworkPeers.Count<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (q => q.GetComponent<MissionPeer>() != null && q.GetComponent<MissionPeer>().Team == Mission.Current.AttackerTeam)) > 0 ? 1 : 0)) & (MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue() > 0 ? (true ? 1 : 0) : (GameNetwork.NetworkPeers.Count<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (q => q.GetComponent<MissionPeer>() != null && q.GetComponent<MissionPeer>().Team == Mission.Current.DefenderTeam)) > 0 ? 1 : 0))) != 0;

    private void BeginNewRound()
    {
      if (this.CurrentRoundState == MultiplayerRoundState.WaitingForPlayers)
        this._gameModeServer.ClearPeerCounts();
      this.ChangeRoundState(MultiplayerRoundState.Preparation);
      ++this.RoundCount;
      Mission.Current.ResetMission();
      Mission.Current.BalanceTeams();
      this._gameModeServer.TimerComponent.StartTimerAsServer((float) MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetIntValue());
      if (this.OnRoundStarted != null)
        this.OnRoundStarted();
      this._gameModeServer.SpawnComponent.ToggleUpdatingSpawnEquipment(true);
    }

    private bool CheckForPreparationEnd() => this.CurrentRoundState == MultiplayerRoundState.Preparation && this._gameModeServer.TimerComponent.CheckIfTimerPassed();

    private void EndPreparation()
    {
      if (this.OnPreparationEnded == null)
        return;
      this.OnPreparationEnded();
    }

    private void StartSpawning(bool disableEquipmentUpdate = true)
    {
      this._gameModeServer.TimerComponent.StartTimerAsServer((float) MultiplayerOptions.OptionType.RoundTimeLimit.GetIntValue());
      if (disableEquipmentUpdate)
        this._gameModeServer.SpawnComponent.ToggleUpdatingSpawnEquipment(false);
      this.ChangeRoundState(MultiplayerRoundState.InProgress);
    }

    private bool CheckForMatchEndEarly()
    {
      bool flag = false;
      MissionScoreboardComponent missionBehaviour = Mission.Current.GetMissionBehaviour<MissionScoreboardComponent>();
      if (missionBehaviour != null)
      {
        for (int index = 0; index < 2; ++index)
        {
          if (missionBehaviour.GetRoundScore((BattleSideEnum) index) > MultiplayerOptions.OptionType.RoundTotal.GetIntValue() / 2)
          {
            flag = true;
            break;
          }
        }
      }
      return flag;
    }

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
      if (networkPeer.IsServerPeer)
        return;
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new RoundStateChange(this.CurrentRoundState, this._currentRoundStateStartTime.NumberOfTicks, MBMath.Ceiling(this.LastRoundEndRemainingTime)));
      GameNetwork.EndModuleEventAsServer();
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new RoundWinnerChange(this.RoundWinner));
      GameNetwork.EndModuleEventAsServer();
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new RoundCountChange(this.RoundCount));
      GameNetwork.EndModuleEventAsServer();
    }
  }
}
