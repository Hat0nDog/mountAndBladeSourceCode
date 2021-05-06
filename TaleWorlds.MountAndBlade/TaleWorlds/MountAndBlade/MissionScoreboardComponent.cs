// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionScoreboardComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MissionScoreboardComponent : MissionNetwork
  {
    private MissionLobbyComponent _missionLobbyComponent;
    private MissionMultiplayerGameModeBaseClient _mpGameModeBase;
    private IScoreboardData _scoreboardData;
    private List<MissionPeer> _spectators;
    private const int _totalSideCount = 2;
    private MissionScoreboardComponent.MissionScoreboardSide[] _sides;
    private bool _isInitialized;
    private List<BattleSideEnum> _roundWinnerList;
    private MissionScoreboardComponent.ScoreboardSides _scoreboardSides;

    public event Action OnRoundPropertiesChanged;

    public event Action<BattleSideEnum> OnBotPropertiesChanged;

    public event Action<Team, Team, MissionPeer> OnPlayerSideChanged;

    public event Action<BattleSideEnum, MissionPeer> OnPlayerPropertiesChanged;

    public event Action OnScoreboardInitialized;

    public MissionScoreboardComponent.ScoreboardHeader[] Headers => this._scoreboardData.GetScoreboardHeaders();

    public MissionScoreboardComponent(string gameMode)
    {
      this._spectators = new List<MissionPeer>();
      this._sides = new MissionScoreboardComponent.MissionScoreboardSide[2];
      this._roundWinnerList = new List<BattleSideEnum>();
      ScoreboardFactory.Register("Duel", (IScoreboardData) new DuelScoreboardData());
      ScoreboardFactory.Register("Skirmish", (IScoreboardData) new SkirmishScoreboardData());
      ScoreboardFactory.Register("Captain", (IScoreboardData) new CaptainScoreboardData());
      ScoreboardFactory.Register("Battle", (IScoreboardData) new BattleScoreboardData());
      ScoreboardFactory.Register("FreeForAll", (IScoreboardData) new FFAScoreboardData());
      ScoreboardFactory.Register("TeamDeathmatch", (IScoreboardData) new TDMScoreboardData());
      ScoreboardFactory.Register("Siege", (IScoreboardData) new SiegeScoreboardData());
      this._scoreboardData = ScoreboardFactory.Get(gameMode);
    }

    public IEnumerable<BattleSideEnum> RoundWinnerList => (IEnumerable<BattleSideEnum>) this._roundWinnerList.AsReadOnly();

    public MissionScoreboardComponent.MissionScoreboardSide[] Sides => this._sides;

    public List<MissionPeer> Spectators => this._spectators;

    public override void AfterStart()
    {
      this.Clear();
      this._missionLobbyComponent = this.Mission.GetMissionBehaviour<MissionLobbyComponent>();
      this._mpGameModeBase = this.Mission.GetMissionBehaviour<MissionMultiplayerGameModeBaseClient>();
      this._scoreboardSides = this._missionLobbyComponent.MissionType == MissionLobbyComponent.MultiplayerGameType.FreeForAll || this._missionLobbyComponent.MissionType == MissionLobbyComponent.MultiplayerGameType.Duel ? MissionScoreboardComponent.ScoreboardSides.OneSide : MissionScoreboardComponent.ScoreboardSides.TwoSides;
      MissionPeer.OnTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
      VirtualPlayer.OnPeerComponentPreRemoved += new Action<VirtualPlayer, PeerComponent>(this.OnPeerComponentPreRemoved);
      NetworkCommunicator.OnPeerComponentAdded += new Action<PeerComponent>(this.OnPeerComponentAdded);
      if (GameNetwork.IsServerOrRecorder && this._mpGameModeBase.RoundComponent != null)
        this._mpGameModeBase.RoundComponent.OnRoundEnding += new Action(this.OnRoundEnding);
      this.LateInitScoreboard();
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (!GameNetwork.IsClient)
        return;
      registerer.Register<NetworkMessages.FromServer.UpdateRoundScores>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.UpdateRoundScores>(this.HandleServerUpdateRoundScoresMessage));
      registerer.Register<NetworkMessages.FromServer.BotData>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.BotData>(this.HandleServerEventBotDataMessage));
    }

    public override void OnRemoveBehaviour()
    {
      this._spectators.Clear();
      for (int index = 0; index < 2; ++index)
      {
        if (this._sides[index] != null)
          this._sides[index].Clear();
      }
      MissionPeer.OnTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.TeamChange);
      VirtualPlayer.OnPeerComponentPreRemoved -= new Action<VirtualPlayer, PeerComponent>(this.OnPeerComponentPreRemoved);
      NetworkCommunicator.OnPeerComponentAdded -= new Action<PeerComponent>(this.OnPeerComponentAdded);
      if (GameNetwork.IsServerOrRecorder && this._mpGameModeBase.RoundComponent != null)
        this._mpGameModeBase.RoundComponent.OnRoundEnding -= new Action(this.OnRoundEnding);
      ScoreboardFactory.Clear();
      base.OnRemoveBehaviour();
    }

    public void Clear() => this._spectators.Clear();

    public void ResetBotScores()
    {
      foreach (MissionScoreboardComponent.MissionScoreboardSide side in this._sides)
        side.BotScores?.ResetKillDeathAssist();
    }

    public void ChangeTeamScore(Team team, int scoreChange)
    {
      this._sides[(int) team.Side].SideScore += scoreChange;
      this._sides[(int) team.Side].SideScore = MBMath.ClampInt(this._sides[(int) team.Side].SideScore, -120000, 120000);
      if (GameNetwork.IsServer)
      {
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.UpdateRoundScores(this.GetRoundScore(BattleSideEnum.Attacker), this.GetRoundScore(BattleSideEnum.Defender)));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      if (this.OnRoundPropertiesChanged == null)
        return;
      this.OnRoundPropertiesChanged();
    }

    public void ClearScores()
    {
      this._sides[1].SideScore = 0;
      this._sides[0].SideScore = 0;
      if (this.OnRoundPropertiesChanged == null)
        return;
      this.OnRoundPropertiesChanged();
    }

    private void UpdateRoundScores()
    {
      foreach (MissionScoreboardComponent.MissionScoreboardSide side in this._sides)
      {
        if (side != null && side.Side == this.RoundWinner)
        {
          this._roundWinnerList.Add(this.RoundWinner);
          if (this.RoundWinner != BattleSideEnum.None)
            ++this._sides[(int) this.RoundWinner].SideScore;
        }
      }
      if (this.OnRoundPropertiesChanged != null)
        this.OnRoundPropertiesChanged();
      if (!GameNetwork.IsServer)
        return;
      int defenderTeamScore = this._scoreboardSides != MissionScoreboardComponent.ScoreboardSides.OneSide ? this._sides[0].SideScore : 0;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.UpdateRoundScores(this._sides[1].SideScore, defenderTeamScore));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    public MissionScoreboardComponent.MissionScoreboardSide GetSideSafe(
      BattleSideEnum battleSide)
    {
      return this._scoreboardSides == MissionScoreboardComponent.ScoreboardSides.OneSide ? this._sides[1] : this._sides[(int) battleSide];
    }

    public int GetRoundScore(BattleSideEnum side) => side > (BattleSideEnum) this._sides.Length || side < BattleSideEnum.Defender ? 0 : this._sides[(int) side].SideScore;

    public void HandleServerUpdateRoundScoresMessage(NetworkMessages.FromServer.UpdateRoundScores message)
    {
      this._sides[1].SideScore = message.AttackerTeamScore;
      if (this._scoreboardSides != MissionScoreboardComponent.ScoreboardSides.OneSide)
        this._sides[0].SideScore = message.DefenderTeamScore;
      if (this.OnRoundPropertiesChanged == null)
        return;
      this.OnRoundPropertiesChanged();
    }

    public void CalculateTotalNumbers()
    {
      foreach (MissionScoreboardComponent.MissionScoreboardSide side in this._sides)
      {
        if (side != null)
        {
          int deathCount = side.BotScores.DeathCount;
          int assistCount = side.BotScores.AssistCount;
          int killCount = side.BotScores.KillCount;
          foreach (MissionPeer player in side.Players)
          {
            assistCount += player.AssistCount;
            deathCount += player.DeathCount;
            killCount += player.KillCount;
          }
        }
      }
    }

    private void TeamChange(NetworkCommunicator player, Team oldTeam, Team nextTeam)
    {
      MissionPeer component = player.GetComponent<MissionPeer>();
      if (oldTeam != null)
      {
        if (oldTeam == this.Mission.SpectatorTeam)
          this._spectators.Remove(component);
        else
          this._sides[(int) oldTeam.Side].RemovePlayer(component);
      }
      if (nextTeam != null)
      {
        if (nextTeam == this.Mission.SpectatorTeam)
          this._spectators.Add(component);
        else
          this._sides[(int) nextTeam.Side].AddPlayer(component);
      }
      if (this.OnPlayerSideChanged == null)
        return;
      this.OnPlayerSideChanged(oldTeam, nextTeam, component);
    }

    public override void OnClearScene()
    {
      foreach (MissionScoreboardComponent.MissionScoreboardSide side in this.Sides)
      {
        if (side != null)
          side.BotScores.AliveCount = 0;
      }
    }

    private void OnPeerComponentPreRemoved(VirtualPlayer peer, PeerComponent component)
    {
      if (!(component is MissionPeer missionPeer))
        return;
      MissionPeer lobbyComponent = missionPeer.GetComponent<MissionPeer>();
      int num = this._spectators.Contains(lobbyComponent) ? 1 : 0;
      bool flag = ((IEnumerable<MissionScoreboardComponent.MissionScoreboardSide>) this._sides).Any<MissionScoreboardComponent.MissionScoreboardSide>((Func<MissionScoreboardComponent.MissionScoreboardSide, bool>) (x => x != null && x.Players.Contains<MissionPeer>(lobbyComponent)));
      if (num != 0)
      {
        this._spectators.Remove(lobbyComponent);
      }
      else
      {
        if (!flag)
          return;
        this._sides[(int) lobbyComponent.Team.Side].RemovePlayer(lobbyComponent);
        Formation controlledFormation = missionPeer.ControlledFormation;
        if (controlledFormation != null)
        {
          Team team = missionPeer.Team;
          this.Sides[(int) team.Side].BotScores.AliveCount += controlledFormation.GetCountOfUnitsWithCondition((Func<Agent, bool>) (agent => agent.IsActive()));
          this.BotPropertiesChanged(team.Side);
        }
        Action<Team, Team, MissionPeer> playerSideChanged = this.OnPlayerSideChanged;
        if (playerSideChanged == null)
          return;
        playerSideChanged(lobbyComponent.Team, (Team) null, lobbyComponent);
      }
    }

    private void BotsControlledChanged(NetworkCommunicator peer) => this.PlayerPropertiesChanged(peer);

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
      if (!agent.IsActive() || agent.IsMount)
        return;
      if (agent.MissionPeer == null)
      {
        this.BotPropertiesChanged(agent.Team.Side);
      }
      else
      {
        if (agent.MissionPeer == null)
          return;
        this.PlayerPropertiesChanged(agent.MissionPeer.GetNetworkPeer());
      }
    }

    public override void OnAssignPlayerAsSergeantOfFormation(Agent agent)
    {
      if (agent.MissionPeer == null)
        return;
      this.PlayerPropertiesChanged(agent.MissionPeer.GetNetworkPeer());
    }

    public void BotPropertiesChanged(BattleSideEnum side)
    {
      if (this.OnBotPropertiesChanged == null)
        return;
      this.OnBotPropertiesChanged(side);
    }

    public void PlayerPropertiesChanged(NetworkCommunicator player)
    {
      if (GameNetwork.IsDedicatedServer)
        return;
      MissionPeer component = player.GetComponent<MissionPeer>();
      if (component == null)
        return;
      this.PlayerPropertiesChanged(component);
    }

    public void PlayerPropertiesChanged(MissionPeer player)
    {
      if (GameNetwork.IsDedicatedServer)
        return;
      this.CalculateTotalNumbers();
      if (this.OnPlayerPropertiesChanged == null || player.Team == null || player.Team == Mission.Current.SpectatorTeam)
        return;
      this.OnPlayerPropertiesChanged(player.Team.Side, player);
    }

    protected override void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
      networkPeer.GetComponent<MissionPeer>();
      foreach (MissionScoreboardComponent.MissionScoreboardSide side in this._sides)
      {
        if (side != null && !networkPeer.IsServerPeer)
        {
          if (side.BotScores.IsAnyValid)
          {
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.BotData(side.Side, side.BotScores.KillCount, side.BotScores.AssistCount, side.BotScores.DeathCount, side.BotScores.AliveCount));
            GameNetwork.EndModuleEventAsServer();
          }
          if (this._mpGameModeBase != null)
          {
            int defenderTeamScore = this._scoreboardSides != MissionScoreboardComponent.ScoreboardSides.OneSide ? this._sides[0].SideScore : 0;
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.UpdateRoundScores(this._sides[1].SideScore, defenderTeamScore));
            GameNetwork.EndModuleEventAsServer();
          }
        }
      }
    }

    public void HandleServerEventBotDataMessage(NetworkMessages.FromServer.BotData message)
    {
      this._sides[(int) message.Side].BotScores.KillCount = message.KillCount;
      this._sides[(int) message.Side].BotScores.AssistCount = message.AssistCount;
      this._sides[(int) message.Side].BotScores.DeathCount = message.DeathCount;
      this._sides[(int) message.Side].BotScores.AliveCount = message.AliveBotCount;
      this.BotPropertiesChanged(message.Side);
    }

    public BattleSideEnum RoundWinner
    {
      get
      {
        IRoundComponent roundComponent = this._mpGameModeBase.RoundComponent;
        return roundComponent == null ? BattleSideEnum.None : roundComponent.RoundWinner;
      }
    }

    public void OnRoundEnding()
    {
      if (!GameNetwork.IsServerOrRecorder)
        return;
      this.UpdateRoundScores();
    }

    public override void OnMissionRestart()
    {
      this.Clear();
      this.ClearScores();
    }

    private void OnPeerComponentAdded(PeerComponent component)
    {
      if (!(component is MissionRepresentativeBase) || !component.IsMine)
        return;
      this.LateInitializeHeaders();
    }

    private void LateInitScoreboard()
    {
      this._sides[1] = new MissionScoreboardComponent.MissionScoreboardSide(BattleSideEnum.Attacker);
      this._sides[1].BotScores = new BotData();
      if (this._scoreboardSides != MissionScoreboardComponent.ScoreboardSides.TwoSides)
        return;
      this._sides[0] = new MissionScoreboardComponent.MissionScoreboardSide(BattleSideEnum.Defender);
      this._sides[0].BotScores = new BotData();
    }

    private void LateInitializeHeaders()
    {
      if (this._isInitialized)
        return;
      this._isInitialized = true;
      foreach (MissionScoreboardComponent.MissionScoreboardSide side in this._sides)
        side?.UpdateHeader(this.Headers);
      if (this.OnScoreboardInitialized == null)
        return;
      this.OnScoreboardInitialized();
    }

    public void OnMultiplayerGameClientBehaviorInitialized(
      ref Action<NetworkCommunicator> onBotsControlledChanged)
    {
      onBotsControlledChanged += new Action<NetworkCommunicator>(this.BotsControlledChanged);
    }

    private enum ScoreboardSides
    {
      OneSide,
      TwoSides,
    }

    public struct ScoreboardHeader
    {
      private readonly Func<MissionPeer, string> _playerGetterFunc;
      private readonly Func<BotData, string> _botGetterFunc;
      public readonly string Id;
      public readonly TextObject Name;

      public ScoreboardHeader(
        string id,
        Func<MissionPeer, string> playerGetterFunc,
        Func<BotData, string> botGetterFunc)
      {
        this.Id = id;
        this.Name = GameTexts.FindText("str_scoreboard_header", id);
        this._playerGetterFunc = playerGetterFunc;
        this._botGetterFunc = botGetterFunc;
      }

      public string GetValueOf(MissionPeer missionPeer) => this._playerGetterFunc == null ? "" : this._playerGetterFunc(missionPeer);

      public string GetValueOf(BotData botData) => this._botGetterFunc == null ? "" : this._botGetterFunc(botData);
    }

    public class MissionScoreboardSide
    {
      public readonly BattleSideEnum Side;
      private MissionScoreboardComponent.ScoreboardHeader[] _properties;
      public BotData BotScores;
      public int SideScore;
      protected List<MissionPeer> _players;

      public IEnumerable<MissionPeer> Players => (IEnumerable<MissionPeer>) this._players;

      public MissionScoreboardSide(BattleSideEnum side)
      {
        this.Side = side;
        this._players = new List<MissionPeer>();
      }

      public void AddPlayer(MissionPeer peer) => this._players.Add(peer);

      public void RemovePlayer(MissionPeer peer) => this._players.Remove(peer);

      public string[] GetValuesOf(MissionPeer peer)
      {
        if (this._properties == null)
          return new string[0];
        string[] strArray = new string[this._properties.Length];
        if (peer == null)
        {
          for (int index = 0; index < this._properties.Length; ++index)
            strArray[index] = this._properties[index].GetValueOf(this.BotScores);
          return strArray;
        }
        for (int index = 0; index < this._properties.Length; ++index)
          strArray[index] = this._properties[index].GetValueOf(peer);
        return strArray;
      }

      public string[] GetHeaderNames()
      {
        if (this._properties == null)
          return new string[0];
        string[] strArray = new string[this._properties.Length];
        for (int index = 0; index < this._properties.Length; ++index)
          strArray[index] = this._properties[index].Name.ToString();
        return strArray;
      }

      public string[] GetHeaderIds()
      {
        if (this._properties == null)
          return new string[0];
        string[] strArray = new string[this._properties.Length];
        for (int index = 0; index < this._properties.Length; ++index)
          strArray[index] = this._properties[index].Id;
        return strArray;
      }

      public int GetScore(MissionPeer peer)
      {
        if (this._properties == null)
          return 0;
        string s = peer != null ? ((IEnumerable<MissionScoreboardComponent.ScoreboardHeader>) this._properties).Single<MissionScoreboardComponent.ScoreboardHeader>((Func<MissionScoreboardComponent.ScoreboardHeader, bool>) (x => x.Id == "score")).GetValueOf(peer) : ((IEnumerable<MissionScoreboardComponent.ScoreboardHeader>) this._properties).Single<MissionScoreboardComponent.ScoreboardHeader>((Func<MissionScoreboardComponent.ScoreboardHeader, bool>) (x => x.Id == "score")).GetValueOf(this.BotScores);
        int result = 0;
        int.TryParse(s, out result);
        return result;
      }

      public void UpdateHeader(
        MissionScoreboardComponent.ScoreboardHeader[] headers)
      {
        this._properties = headers;
      }

      public void Clear() => this._players.Clear();
    }
  }
}
