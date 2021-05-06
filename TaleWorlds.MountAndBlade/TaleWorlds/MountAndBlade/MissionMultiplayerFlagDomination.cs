// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerFlagDomination
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MissionMultiplayerFlagDomination : 
    MissionMultiplayerGameModeBase,
    IAnalyticsFlagInfo,
    IMissionBehavior
  {
    private const float FlagStateCheckPeriod = 0.25f;
    public const int NumberOfFlagsInGame = 3;
    public const float MoraleRoundPrecision = 0.01f;
    public const int DefaultGoldAmountForTroopSelection = 300;
    public const int MaxGoldAmountToCarryOver = 80;
    public const int MaxGoldAmountToCarryOverForSurvival = 30;
    private const float MoraleGainOnSecond = 0.0025f;
    private const float MoralePenaltyPercentageIfNoPointsCaptured = 0.1f;
    private const float MoraleTickTimeInSeconds = 1f;
    public const float TimeTillFlagRemovalForPriorInfoInSeconds = 30f;
    public const float PointRemovalTimeInSecondsForCaptain = 180f;
    public const float PointRemovalTimeInSecondsForSkirmish = 120f;
    public const float MoraleMultiplierForEachFlagForCaptain = 1f;
    public const float MoraleMultiplierForEachFlagForSkirmish = 2f;
    private readonly float _pointRemovalTimeInSeconds = 180f;
    private readonly float _moraleMultiplierForEachFlag = 1f;
    private readonly float _moraleMultiplierOnLastFlag = 2f;
    private float _dtSumFlagStateCheck;
    private Team[] _capturePointOwners;
    private bool _flagRemovalOccured;
    private float _nextTimeToCheckForMorales = float.MinValue;
    private float _nextTimeToCheckForPointRemoval = float.MinValue;
    private MissionMultiplayerGameModeFlagDominationClient _gameModeFlagDominationClient;
    private float _morale;
    private readonly MissionLobbyComponent.MultiplayerGameType _gameType;

    public override bool IsGameModeHidingAllAgentVisuals => this._gameType == MissionLobbyComponent.MultiplayerGameType.Captain;

    public MBReadOnlyList<FlagCapturePoint> AllCapturePoints { get; private set; }

    public float MoraleRounded => (float) (int) ((double) this._morale / 0.00999999977648258) * 0.01f;

    public bool UseGold() => this._gameModeFlagDominationClient.IsGameModeUsingGold;

    public override bool AllowCustomPlayerBanners() => false;

    public override bool UseRoundController() => true;

    public MissionMultiplayerFlagDomination(bool isSkirmish)
    {
      this._gameType = isSkirmish ? MissionLobbyComponent.MultiplayerGameType.Skirmish : MissionLobbyComponent.MultiplayerGameType.Captain;
      if (this._gameType != MissionLobbyComponent.MultiplayerGameType.Skirmish)
        return;
      this._moraleMultiplierForEachFlag = 2f;
      this._pointRemovalTimeInSeconds = 120f;
    }

    public override MissionLobbyComponent.MultiplayerGameType GetMissionType() => this._gameType;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._gameModeFlagDominationClient = Mission.Current.GetMissionBehaviour<MissionMultiplayerGameModeFlagDominationClient>();
      this._morale = 0.0f;
      this._capturePointOwners = new Team[3];
      this.AllCapturePoints = new MBReadOnlyList<FlagCapturePoint>(Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>().ToListQ<FlagCapturePoint>());
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        allCapturePoint.SetTeamColorsWithAllSynched(4284111450U, uint.MaxValue);
        this._capturePointOwners[allCapturePoint.FlagIndex] = (Team) null;
      }
    }

    public override void AfterStart()
    {
      base.AfterStart();
      this.RoundController.OnRoundStarted += new Action(this.OnPreparationStart);
      MissionPeer.OnPreTeamChanged += new MissionPeer.OnTeamChangedDelegate(this.OnPreTeamChanged);
      this.RoundController.OnPreparationEnded += new Action(this.OnPreparationEnded);
      this.WarmupComponent.OnWarmupEnding += new Action(this.OnWarmupEnding);
      this.RoundController.OnPreRoundEnding += new Action(this.OnRoundEnd);
      this.RoundController.OnPostRoundEnded += new Action(this.OnPostRoundEnd);
      BasicCultureObject basicCultureObject1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
      BasicCultureObject basicCultureObject2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());
      Banner banner1 = new Banner(basicCultureObject1.BannerKey, basicCultureObject1.BackgroundColor1, basicCultureObject1.ForegroundColor1);
      Banner banner2 = new Banner(basicCultureObject2.BannerKey, basicCultureObject2.BackgroundColor2, basicCultureObject2.ForegroundColor2);
      this.Mission.Teams.Add(BattleSideEnum.Attacker, basicCultureObject1.BackgroundColor1, basicCultureObject1.ForegroundColor1, banner1, false, true);
      this.Mission.Teams.Add(BattleSideEnum.Defender, basicCultureObject2.BackgroundColor2, basicCultureObject2.ForegroundColor2, banner2, false, true);
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      registerer.Register<RequestForfeitSpawn>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestForfeitSpawn>(this.HandleClientEventRequestForfeitSpawn));
    }

    public override void OnRemoveBehaviour()
    {
      this.RoundController.OnRoundStarted -= new Action(this.OnPreparationStart);
      MissionPeer.OnPreTeamChanged -= new MissionPeer.OnTeamChangedDelegate(this.OnPreTeamChanged);
      this.RoundController.OnPreparationEnded -= new Action(this.OnPreparationEnded);
      this.WarmupComponent.OnWarmupEnding -= new Action(this.OnWarmupEnding);
      this.RoundController.OnPreRoundEnding -= new Action(this.OnRoundEnd);
      this.RoundController.OnPostRoundEnded -= new Action(this.OnPostRoundEnd);
      base.OnRemoveBehaviour();
    }

    public override void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
    {
      if (oldTeam == null || oldTeam == newTeam || !this.UseGold())
        return;
      this.ChangeCurrentGoldForPeer(peer.GetComponent<MissionPeer>(), 0);
    }

    private void OnPreparationStart() => this.NotificationsComponent.PreparationStarted();

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if (this.MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Playing)
        return;
      if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
        this.CheckForPlayersSpawningAsBots();
      if (!this.RoundController.IsRoundInProgress)
        return;
      if (!this._flagRemovalOccured)
        this.CheckRemovingOfPoints();
      this.CheckMorales();
      this.TickFlags(dt);
    }

    private void CheckForPlayersSpawningAsBots()
    {
      foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
      {
        if (peer.GetNetworkPeer().IsSynchronized && peer.ControlledAgent == null && (peer.Team != null && peer.ControlledFormation != null) && peer.SpawnCountThisRound > 0)
        {
          if (!peer.HasSpawnTimerExpired && peer.SpawnTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
            peer.HasSpawnTimerExpired = true;
          if (peer.HasSpawnTimerExpired && peer.WantsToSpawnAsBot && peer.ControlledFormation.HasUnitsWithCondition((Func<Agent, bool>) (agent => agent.IsActive() && agent.IsAIControlled)))
          {
            Agent newAgent = (Agent) null;
            Agent followingAgent = peer.FollowedAgent;
            if (followingAgent != null && followingAgent.IsActive() && (followingAgent.IsAIControlled && peer.ControlledFormation.HasUnitsWithCondition((Func<Agent, bool>) (agent => agent == followingAgent))))
            {
              newAgent = followingAgent;
            }
            else
            {
              float maxHealth = 0.0f;
              peer.ControlledFormation.ApplyActionOnEachUnit((Action<Agent>) (agent =>
              {
                if ((double) agent.Health <= (double) maxHealth)
                  return;
                maxHealth = agent.Health;
                newAgent = agent;
              }));
            }
            Mission.Current.ReplaceBotWithPlayer(newAgent, peer);
            peer.WantsToSpawnAsBot = false;
            peer.HasSpawnTimerExpired = false;
          }
        }
      }
    }

    private bool GetMoraleGain(out float moraleGain)
    {
      List<FlagCapturePoint> list = this.AllCapturePoints.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (flag => !flag.IsDeactivated && this.GetFlagOwnerTeam(flag) != null && flag.IsFullyRaised)).ToList<FlagCapturePoint>();
      int num1 = list.Count<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (flag => this.GetFlagOwnerTeam(flag).Side == BattleSideEnum.Attacker)) - list.Count<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (flag => this.GetFlagOwnerTeam(flag).Side == BattleSideEnum.Defender));
      int num2 = Math.Sign(num1);
      moraleGain = 0.0f;
      if (num2 == 0)
        return false;
      float num3 = 1f / 400f * this._moraleMultiplierForEachFlag * (float) Math.Abs(num1);
      moraleGain = num2 <= 0 ? MBMath.ClampFloat((float) num2 - this._morale, -2f, -1f) * num3 : MBMath.ClampFloat((float) num2 - this._morale, 1f, 2f) * num3;
      if (this._flagRemovalOccured)
        moraleGain *= this._moraleMultiplierOnLastFlag;
      return true;
    }

    public float GetTimeUntilBattleSideVictory(BattleSideEnum side)
    {
      float val1 = float.MaxValue;
      if (side == BattleSideEnum.Attacker && (double) this._morale > 0.0 || side == BattleSideEnum.Defender && (double) this._morale < 0.0)
        val1 = this.RoundController.RemainingRoundTime;
      float val2 = float.MaxValue;
      float moraleGain;
      this.GetMoraleGain(out moraleGain);
      if (side == BattleSideEnum.Attacker && (double) moraleGain > 0.0)
        val2 = (1f - this._morale) / moraleGain;
      else if (side == BattleSideEnum.Defender && (double) moraleGain < 0.0)
        val2 = (float) ((-1.0 - (double) this._morale) / ((double) moraleGain / 1.0));
      return Math.Min(val1, val2);
    }

    private void CheckMorales()
    {
      if ((double) this._nextTimeToCheckForMorales < 0.0)
        this._nextTimeToCheckForMorales = this.Mission.Time + 1f;
      if ((double) this.Mission.Time < (double) this._nextTimeToCheckForMorales)
        return;
      ++this._nextTimeToCheckForMorales;
      float moraleGain;
      if (!this.GetMoraleGain(out moraleGain))
        return;
      this._morale += moraleGain;
      this._morale = MBMath.ClampFloat(this._morale, -1f, 1f);
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationMoraleChangeMessage(this.MoraleRounded));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      this._gameModeFlagDominationClient?.OnMoraleChanged(this.MoraleRounded);
      MPPerkObject.RaiseEventForAllPeers(MPPerkCondition.PerkEventFlags.MoraleChange);
    }

    private void CheckRemovingOfPoints()
    {
      if ((double) this._nextTimeToCheckForPointRemoval < 0.0)
        this._nextTimeToCheckForPointRemoval = this.Mission.Time + this._pointRemovalTimeInSeconds;
      if ((double) this.Mission.Time < (double) this._nextTimeToCheckForPointRemoval)
        return;
      this._nextTimeToCheckForPointRemoval += this._pointRemovalTimeInSeconds;
      List<BattleSideEnum> battleSideEnumList = new List<BattleSideEnum>();
      foreach (Team team1 in (ReadOnlyCollection<Team>) this.Mission.Teams)
      {
        Team team = team1;
        if (team.Side != BattleSideEnum.None)
        {
          int num = (int) team.Side * 2 - 1;
          if (this.AllCapturePoints.All<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => this.GetFlagOwnerTeam(cp) != team)))
          {
            if (this.AllCapturePoints.FirstOrDefault<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => this.GetFlagOwnerTeam(cp) == null)) != null)
            {
              this._morale -= 0.1f * (float) num;
              battleSideEnumList.Add(BattleSideEnum.None);
            }
            else
            {
              this._morale -= (float) (0.100000001490116 * (double) num * 2.0);
              battleSideEnumList.Add(team.Side.GetOppositeSide());
            }
            this._morale = MBMath.ClampFloat(this._morale, -1f, 1f);
          }
          else
            battleSideEnumList.Add(team.Side);
        }
      }
      List<int> removedCapIndexList = new List<int>();
      List<FlagCapturePoint> list1 = this.AllCapturePoints.ToList<FlagCapturePoint>();
      foreach (BattleSideEnum battleSideEnum in battleSideEnumList)
      {
        BattleSideEnum side = battleSideEnum;
        if (side == BattleSideEnum.None)
        {
          removedCapIndexList.Add(this.RemoveCapturePoint(list1.GetRandomElementWithPredicate<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => this.GetFlagOwnerTeam(cp) == null))));
        }
        else
        {
          List<FlagCapturePoint> list2 = list1.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => this.GetFlagOwnerTeam(cp) != null && this.GetFlagOwnerTeam(cp).Side == side)).ToList<FlagCapturePoint>();
          List<FlagCapturePoint> list3 = list2.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => this.GetNumberOfAttackersAroundFlag(cp) == 0)).ToList<FlagCapturePoint>();
          if (list3.Any<FlagCapturePoint>())
          {
            removedCapIndexList.Add(this.RemoveCapturePoint(list3.GetRandomElement<FlagCapturePoint>()));
          }
          else
          {
            List<KeyValuePair<FlagCapturePoint, int>> keyValuePairList = new List<KeyValuePair<FlagCapturePoint, int>>();
            foreach (FlagCapturePoint flagCapturePoint in list2)
            {
              if (!keyValuePairList.Any<KeyValuePair<FlagCapturePoint, int>>())
              {
                keyValuePairList.Add(new KeyValuePair<FlagCapturePoint, int>(flagCapturePoint, this.GetNumberOfAttackersAroundFlag(flagCapturePoint)));
              }
              else
              {
                int count = this.GetNumberOfAttackersAroundFlag(flagCapturePoint);
                if (keyValuePairList.Any<KeyValuePair<FlagCapturePoint, int>>((Func<KeyValuePair<FlagCapturePoint, int>, bool>) (cc => cc.Value > count)))
                {
                  keyValuePairList.Clear();
                  keyValuePairList.Add(new KeyValuePair<FlagCapturePoint, int>(flagCapturePoint, count));
                }
                else if (keyValuePairList.Any<KeyValuePair<FlagCapturePoint, int>>((Func<KeyValuePair<FlagCapturePoint, int>, bool>) (cc => cc.Value == count)))
                  keyValuePairList.Add(new KeyValuePair<FlagCapturePoint, int>(flagCapturePoint, count));
              }
            }
            removedCapIndexList.Add(this.RemoveCapturePoint(keyValuePairList.GetRandomElement<KeyValuePair<FlagCapturePoint, int>>().Key));
          }
        }
        FlagCapturePoint flagCapturePoint1 = list1.First<FlagCapturePoint>(closure_0 ?? (closure_0 = (Func<FlagCapturePoint, bool>) (fl => fl.FlagIndex == removedCapIndexList[removedCapIndexList.Count - 1])));
        list1.Remove(flagCapturePoint1);
      }
      removedCapIndexList.Sort();
      int first = removedCapIndexList[0];
      int second = removedCapIndexList[1];
      FlagCapturePoint remainingFlag = this.AllCapturePoints.First<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => cp.FlagIndex != first && cp.FlagIndex != second));
      this.NotificationsComponent.FlagXRemaining(remainingFlag);
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationMoraleChangeMessage(this.MoraleRounded));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationFlagsRemovedMessage());
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      this._flagRemovalOccured = true;
      this._gameModeFlagDominationClient?.OnNumberOfFlagsChanged();
      foreach (MissionBehaviour missionBehaviour in this.Mission.MissionBehaviours)
      {
        if (missionBehaviour is IFlagRemoved flagRemoved1)
          flagRemoved1.OnFlagsRemoved(remainingFlag.FlagIndex);
      }
      MPPerkObject.RaiseEventForAllPeers(MPPerkCondition.PerkEventFlags.FlagRemoval);
    }

    private int RemoveCapturePoint(FlagCapturePoint capToRemove)
    {
      int flagIndex = capToRemove.FlagIndex;
      capToRemove.RemovePointAsServer();
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationCapturePointMessage(flagIndex, (Team) null));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      return flagIndex;
    }

    public override void OnClearScene()
    {
      this.AllCapturePoints = new MBReadOnlyList<FlagCapturePoint>(Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>().ToListQ<FlagCapturePoint>());
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        allCapturePoint.ResetPointAsServer(4284111450U, uint.MaxValue);
        this._capturePointOwners[allCapturePoint.FlagIndex] = (Team) null;
      }
      this._morale = 0.0f;
      this._nextTimeToCheckForMorales = float.MinValue;
      this._nextTimeToCheckForPointRemoval = float.MinValue;
      this._flagRemovalOccured = false;
    }

    public override bool CheckIfOvertime()
    {
      if (!this._flagRemovalOccured)
        return false;
      Team flagOwnerTeam = this.GetFlagOwnerTeam(this.AllCapturePoints.FirstOrDefault<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (flag => !flag.IsDeactivated)));
      return flagOwnerTeam != null && (double) ((int) flagOwnerTeam.Side * 2 - 1) * (double) this._morale < 0.0;
    }

    public override bool CheckForWarmupEnd()
    {
      int[] numArray = new int[2];
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (networkPeer.IsSynchronized && (component?.Team != null && component.Team.Side != BattleSideEnum.None))
          ++numArray[(int) component.Team.Side];
      }
      return ((IEnumerable<int>) numArray).Sum() >= MultiplayerOptions.OptionType.MaxNumberOfPlayers.GetIntValue();
    }

    public override bool CheckForRoundEnd()
    {
      if ((double) Math.Abs(this._morale) >= 1.0)
        return true;
      bool flag1 = this.Mission.AttackerTeam.ActiveAgents.Count > 0;
      bool flag2 = this.Mission.DefenderTeam.ActiveAgents.Count > 0;
      if (flag1 & flag2)
        return false;
      if (!this.SpawnComponent.AreAgentsSpawning())
        return true;
      bool[] flagArray = new bool[2];
      if (this.UseGold())
      {
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        {
          MissionPeer component = networkPeer.GetComponent<MissionPeer>();
          if (component?.Team != null && component.Team.Side != BattleSideEnum.None && !flagArray[(int) component.Team.Side])
          {
            string strValue = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue();
            if (component.Team.Side != BattleSideEnum.Attacker)
              strValue = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue();
            if (this.GetCurrentGoldForPeer(component) >= MultiplayerClassDivisions.GetMinimumTroopCost(MBObjectManager.Instance.GetObject<BasicCultureObject>(strValue)))
              flagArray[(int) component.Team.Side] = true;
          }
        }
      }
      return !flag1 && !flagArray[1] || !flag2 && !flagArray[0];
    }

    public override bool UseCultureSelection() => false;

    private void OnWarmupEnding() => this.NotificationsComponent.WarmupEnding();

    private void OnRoundEnd()
    {
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        if (!allCapturePoint.IsDeactivated)
          allCapturePoint.SetMoveNone();
      }
      RoundEndReason roundEndReason = RoundEndReason.Invalid;
      bool flag1 = (double) this.RoundController.RemainingRoundTime <= 0.0 && !this.CheckIfOvertime();
      int num1 = -1;
      for (int index = 0; index < 2; ++index)
      {
        int num2 = index * 2 - 1;
        if (flag1 && (double) num2 * (double) this._morale > 0.0 || !flag1 && (double) num2 * (double) this._morale >= 1.0)
        {
          num1 = index;
          break;
        }
      }
      CaptureTheFlagCaptureResultEnum roundResult = CaptureTheFlagCaptureResultEnum.NotCaptured;
      if (num1 >= 0)
      {
        roundResult = num1 == 0 ? CaptureTheFlagCaptureResultEnum.DefendersWin : CaptureTheFlagCaptureResultEnum.AttackersWin;
        this.RoundController.RoundWinner = num1 == 0 ? BattleSideEnum.Defender : BattleSideEnum.Attacker;
        roundEndReason = flag1 ? RoundEndReason.RoundTimeEnded : RoundEndReason.GameModeSpecificEnded;
      }
      else
      {
        bool flag2 = this.Mission.AttackerTeam.ActiveAgents.Count > 0;
        bool flag3 = this.Mission.DefenderTeam.ActiveAgents.Count > 0;
        if (flag2 & flag3)
        {
          if ((double) this._morale > 0.0)
          {
            roundResult = CaptureTheFlagCaptureResultEnum.AttackersWin;
            this.RoundController.RoundWinner = BattleSideEnum.Attacker;
          }
          else if ((double) this._morale < 0.0)
          {
            roundResult = CaptureTheFlagCaptureResultEnum.DefendersWin;
            this.RoundController.RoundWinner = BattleSideEnum.Defender;
          }
          else
          {
            roundResult = CaptureTheFlagCaptureResultEnum.Draw;
            this.RoundController.RoundWinner = BattleSideEnum.None;
          }
          roundEndReason = RoundEndReason.RoundTimeEnded;
        }
        else if (flag2)
        {
          roundResult = CaptureTheFlagCaptureResultEnum.AttackersWin;
          this.RoundController.RoundWinner = BattleSideEnum.Attacker;
          roundEndReason = RoundEndReason.SideDepleted;
        }
        else if (flag3)
        {
          roundResult = CaptureTheFlagCaptureResultEnum.DefendersWin;
          this.RoundController.RoundWinner = BattleSideEnum.Defender;
          roundEndReason = RoundEndReason.SideDepleted;
        }
        else
        {
          foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
          {
            MissionPeer component = networkPeer.GetComponent<MissionPeer>();
            if (component?.Team != null && component.Team.Side != BattleSideEnum.None)
            {
              string strValue = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue();
              if (component.Team.Side != BattleSideEnum.Attacker)
                strValue = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue();
              if (this.GetCurrentGoldForPeer(component) >= MultiplayerClassDivisions.GetMinimumTroopCost(MBObjectManager.Instance.GetObject<BasicCultureObject>(strValue)))
              {
                this.RoundController.RoundWinner = component.Team.Side;
                roundEndReason = RoundEndReason.SideDepleted;
                roundResult = component.Team.Side == BattleSideEnum.Attacker ? CaptureTheFlagCaptureResultEnum.AttackersWin : CaptureTheFlagCaptureResultEnum.DefendersWin;
                break;
              }
            }
          }
        }
      }
      if (roundResult == CaptureTheFlagCaptureResultEnum.NotCaptured)
        return;
      this.RoundController.RoundEndReason = roundEndReason;
      this.HandleRoundEnd(roundResult);
    }

    public override void OnAgentBuild(Agent agent, Banner banner)
    {
      agent.GetComponent<HealthAgentComponent>()?.UpdateSyncToAllClients(true);
      if (!agent.IsPlayerControlled)
        return;
      agent.MissionPeer.GetComponent<FlagDominationMissionRepresentative>().UpdateSelectedClassServer(agent);
    }

    private void HandleRoundEnd(CaptureTheFlagCaptureResultEnum roundResult)
    {
      AgentVictoryLogic missionBehaviour = this.Mission.GetMissionBehaviour<AgentVictoryLogic>();
      if (missionBehaviour == null)
        return;
      if (roundResult != CaptureTheFlagCaptureResultEnum.AttackersWin)
      {
        if (roundResult != CaptureTheFlagCaptureResultEnum.DefendersWin)
          return;
        missionBehaviour.SetTimersOfVictoryReactions(BattleSideEnum.Defender);
      }
      else
        missionBehaviour.SetTimersOfVictoryReactions(BattleSideEnum.Attacker);
    }

    private void OnPostRoundEnd()
    {
      if (!this.UseGold() || this.RoundController.IsMatchEnding)
        return;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (component != null && this.RoundController.RoundCount > 0)
        {
          int num1 = 300;
          int num2 = this.GetCurrentGoldForPeer(component);
          if (num2 < 0)
            num2 = 80;
          else if (component.Team != null && component.Team.Side != BattleSideEnum.None && (this.RoundController.RoundWinner == component.Team.Side && component.GetComponent<FlagDominationMissionRepresentative>().CheckIfSurvivedLastRoundAndReset()))
            num2 += 30;
          int newAmount = num1 + MBMath.ClampInt(num2, 0, 80);
          if (newAmount > 300)
            this.NotificationsComponent.GoldCarriedFromPreviousRound(newAmount - 300, component.GetNetworkPeer());
          this.ChangeCurrentGoldForPeer(component, newAmount);
        }
      }
    }

    protected override void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
    {
      if (!this.RoundController.IsRoundInProgress || MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() <= 0)
        return;
      this.MakePlayerFormationCharge(networkPeer);
    }

    private void OnPreTeamChanged(NetworkCommunicator peer, Team currentTeam, Team newTeam)
    {
      if (!peer.IsSynchronized || peer.GetComponent<MissionPeer>().ControlledAgent == null)
        return;
      this.MakePlayerFormationCharge(peer);
    }

    private void OnPreparationEnded()
    {
      if (!this.UseGold())
        return;
      List<MissionPeer>[] missionPeerListArray = new List<MissionPeer>[2];
      for (int index = 0; index < missionPeerListArray.Length; ++index)
        missionPeerListArray[index] = new List<MissionPeer>();
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (component != null && component.Team != null && component.Team.Side != BattleSideEnum.None)
          missionPeerListArray[(int) component.Team.Side].Add(component);
      }
      int num1 = missionPeerListArray[1].Count - missionPeerListArray[0].Count;
      BattleSideEnum battleSideEnum = num1 == 0 ? BattleSideEnum.None : (num1 < 0 ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
      if (battleSideEnum == BattleSideEnum.None)
        return;
      int num2 = Math.Abs(num1);
      int count = missionPeerListArray[(int) battleSideEnum].Count;
      if (count <= 0)
        return;
      int num3 = 300 * num2 / 10 / count * 10;
      foreach (MissionPeer peer in missionPeerListArray[(int) battleSideEnum])
        this.ChangeCurrentGoldForPeer(peer, this.GetCurrentGoldForPeer(peer) + num3);
    }

    private void MakePlayerFormationCharge(NetworkCommunicator peer)
    {
      if (!peer.IsSynchronized)
        return;
      MissionPeer component = peer.GetComponent<MissionPeer>();
      if (component.ControlledFormation == null || component.ControlledAgent == null)
        return;
      component.Team.GetOrderControllerOf(component.ControlledAgent).SetOrder(OrderType.Charge);
    }

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
      networkPeer.AddComponent<FlagDominationMissionRepresentative>();
      if (this.UseGold() && !this.RoundController.IsRoundInProgress)
      {
        this.ChangeCurrentGoldForPeer(networkPeer.GetComponent<MissionPeer>(), 300);
        this._gameModeFlagDominationClient?.OnGoldAmountChangedForRepresentative((MissionRepresentativeBase) networkPeer.GetComponent<FlagDominationMissionRepresentative>(), 300);
      }
      if (this.AllCapturePoints == null || networkPeer.IsServerPeer)
        return;
      foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => !cp.IsDeactivated)))
      {
        GameNetwork.BeginModuleEventAsServer(networkPeer);
        GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationCapturePointMessage(flagCapturePoint.FlagIndex, this._capturePointOwners[flagCapturePoint.FlagIndex]));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    private bool HandleClientEventRequestForfeitSpawn(
      NetworkCommunicator peer,
      RequestForfeitSpawn message)
    {
      this.ForfeitSpawning(peer);
      return true;
    }

    public void ForfeitSpawning(NetworkCommunicator peer)
    {
      MissionPeer component = peer.GetComponent<MissionPeer>();
      if (component == null || !component.HasSpawnedAgentVisuals || (!this.UseGold() || !this.RoundController.IsRoundInProgress))
        return;
      Mission.Current.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>().RemoveAgentVisuals(component, true);
      this.ChangeCurrentGoldForPeer(component, -1);
    }

    public static void SetWinnerTeam(int winnerTeamNo)
    {
      Mission current = Mission.Current;
      MissionMultiplayerFlagDomination missionBehaviour = current.GetMissionBehaviour<MissionMultiplayerFlagDomination>();
      if (missionBehaviour == null)
        return;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        missionBehaviour.ChangeCurrentGoldForPeer(component, 0);
      }
      for (int index = current.Agents.Count - 1; index >= 0; --index)
      {
        Agent agent = current.Agents[index];
        if (agent.IsHuman && agent.Team.MBTeam.Index != winnerTeamNo + 1)
          Mission.Current.KillAgentCheat(agent);
      }
    }

    protected override void OnEndMission()
    {
      if (!this.UseGold())
        return;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (component != null)
          this.ChangeCurrentGoldForPeer(component, -1);
      }
    }

    private void TickFlags(float dt)
    {
      this._dtSumFlagStateCheck += dt;
      if ((double) this._dtSumFlagStateCheck < 0.25)
        return;
      this._dtSumFlagStateCheck -= 0.25f;
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        if (!allCapturePoint.IsDeactivated)
        {
          Team capturePointOwner = this._capturePointOwners[allCapturePoint.FlagIndex];
          Agent agent1 = (Agent) null;
          float num1 = float.MaxValue;
          foreach (Agent agent2 in Mission.Current.GetAgentsInRange(allCapturePoint.Position.AsVec2, 4f).Where<Agent>((Func<Agent, bool>) (a => a.IsHuman && a.IsActive())))
          {
            float num2 = agent2.Position.DistanceSquared(allCapturePoint.Position);
            if ((double) num2 <= 16.0 && (double) num2 < (double) num1)
            {
              agent1 = agent2;
              num1 = num2;
            }
          }
          bool isContested = allCapturePoint.IsContested;
          if (capturePointOwner == null)
          {
            if (!isContested && agent1 != null)
              allCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Down);
            else if (agent1 == null & isContested)
              allCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Up);
          }
          else if (agent1 != null)
          {
            if (agent1.Team != capturePointOwner && !isContested)
              allCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Down);
            else if (agent1.Team == capturePointOwner & isContested)
              allCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Up);
          }
          else if (isContested)
            allCapturePoint.SetMoveFlag(CaptureTheFlagFlagDirection.Up);
          bool ownerTeamChanged;
          allCapturePoint.OnAfterTick(agent1 != null, out ownerTeamChanged);
          if (ownerTeamChanged)
          {
            Team team = agent1.Team;
            uint color = team != null ? team.Color : 4284111450U;
            uint color2 = team != null ? team.Color2 : uint.MaxValue;
            allCapturePoint.SetTeamColorsWithAllSynched(color, color2);
            this._capturePointOwners[allCapturePoint.FlagIndex] = team;
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationCapturePointMessage(allCapturePoint.FlagIndex, team));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
            this._gameModeFlagDominationClient?.OnCapturePointOwnerChanged(allCapturePoint, team);
            this.NotificationsComponent.FlagXCapturedByTeamX((SynchedMissionObject) allCapturePoint, agent1.Team);
            MPPerkObject.RaiseEventForAllPeers(MPPerkCondition.PerkEventFlags.FlagCapture);
          }
        }
      }
    }

    public int GetNumberOfAttackersAroundFlag(FlagCapturePoint capturePoint)
    {
      Team ownerTeam = this.GetFlagOwnerTeam(capturePoint);
      return ownerTeam == null ? 0 : Mission.Current.GetAgentsInRange(capturePoint.Position.AsVec2, 6f).Count<Agent>((Func<Agent, bool>) (a => a.IsHuman && a.IsActive() && (double) a.Position.DistanceSquared(capturePoint.Position) <= 36.0 && a.Team.Side != ownerTeam.Side));
    }

    public Team GetFlagOwnerTeam(FlagCapturePoint flag) => flag == null ? (Team) null : this._capturePointOwners[flag.FlagIndex];

    public override int GetScoreForKill(Agent killedAgent) => MultiplayerClassDivisions.GetMPHeroClassForCharacter(killedAgent.Character).TroopCost;

    public override int GetScoreForAssist(Agent killedAgent) => (int) ((double) MultiplayerClassDivisions.GetMPHeroClassForCharacter(killedAgent.Character).TroopCost * 0.5);

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
      base.OnAgentRemoved(affectedAgent, affectorAgent, agentState, blow);
      if (this.UseGold() && affectorAgent != null && (affectedAgent != null && affectedAgent.IsHuman) && (blow.DamageType != DamageTypes.Invalid && (agentState == AgentState.Unconscious || agentState == AgentState.Killed)))
      {
        Agent.Hitter assistingHitter = affectedAgent.GetAssistingHitter(affectorAgent.MissionPeer);
        MultiplayerClassDivisions.MPHeroClass classForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(affectedAgent.Character);
        if (affectorAgent.MissionPeer != null && affectorAgent.MissionPeer.Representative is FlagDominationMissionRepresentative representative10)
        {
          int gainsFromKillData = representative10.GetGoldGainsFromKillData(MPPerkObject.GetPerkHandler(affectorAgent.MissionPeer), MPPerkObject.GetPerkHandler(assistingHitter?.HitterPeer), classForCharacter, false);
          if (gainsFromKillData > 0)
            this.ChangeCurrentGoldForPeer(affectorAgent.MissionPeer, representative10.Gold + gainsFromKillData);
        }
        if (assistingHitter?.HitterPeer != null && !assistingHitter.IsFriendlyHit && assistingHitter.HitterPeer.Representative is FlagDominationMissionRepresentative representative11)
        {
          int gainsFromKillData = representative11.GetGoldGainsFromKillData(MPPerkObject.GetPerkHandler(affectorAgent.MissionPeer), MPPerkObject.GetPerkHandler(assistingHitter.HitterPeer), classForCharacter, false);
          if (gainsFromKillData > 0)
            this.ChangeCurrentGoldForPeer(assistingHitter.HitterPeer, representative11.Gold + gainsFromKillData);
        }
        if (affectedAgent.MissionPeer?.Team != null)
        {
          IEnumerable<(MissionPeer, int)> goldRewardsOnDeath = MPPerkObject.GetPerkHandler(affectedAgent.MissionPeer)?.GetTeamGoldRewardsOnDeath();
          if (goldRewardsOnDeath != null)
          {
            foreach ((MissionPeer peer6, int baseAmount6) in goldRewardsOnDeath)
            {
              if (baseAmount6 > 0 && peer6?.Representative is FlagDominationMissionRepresentative representative12)
              {
                int local_11 = representative12.GetGoldGainsFromAllyDeathReward(baseAmount6);
                if (local_11 > 0)
                  this.ChangeCurrentGoldForPeer(peer6, representative12.Gold + local_11);
              }
            }
          }
        }
      }
      if (affectedAgent.IsPlayerControlled)
      {
        affectedAgent.MissionPeer.GetComponent<FlagDominationMissionRepresentative>().UpdateSelectedClassServer((Agent) null);
      }
      else
      {
        if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() <= 0 || this.WarmupComponent != null && this.WarmupComponent.IsInWarmup || (affectedAgent.IsMount || affectedAgent.OwningAgentMissionPeer == null || (affectedAgent.Formation == null || affectedAgent.Formation.CountOfUnits != 1)))
          return;
        if (!GameNetwork.IsDedicatedServer)
        {
          MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
          Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
          MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/squad_wiped"), position);
        }
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new FormationWipedMessage());
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, affectedAgent.OwningAgentMissionPeer.GetNetworkPeer());
      }
    }

    public override float GetTroopNumberMultiplierForMissingPlayer(MissionPeer spawningPeer)
    {
      if (this._gameType == MissionLobbyComponent.MultiplayerGameType.Captain)
      {
        List<MissionPeer>[] missionPeerListArray = new List<MissionPeer>[2];
        for (int index = 0; index < missionPeerListArray.Length; ++index)
          missionPeerListArray[index] = new List<MissionPeer>();
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        {
          MissionPeer component = networkPeer.GetComponent<MissionPeer>();
          if (component != null && component.Team != null && component.Team.Side != BattleSideEnum.None)
            missionPeerListArray[(int) component.Team.Side].Add(component);
        }
        int[] numArray = new int[2]
        {
          0,
          MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue()
        };
        numArray[0] = MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue();
        int num = missionPeerListArray[1].Count + numArray[1] - (missionPeerListArray[0].Count + numArray[0]);
        BattleSideEnum battleSideEnum = num == 0 ? BattleSideEnum.None : (num < 0 ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
        if (battleSideEnum == spawningPeer.Team.Side)
          return (float) (1.0 + (double) Math.Abs(num) / (double) (missionPeerListArray[(int) battleSideEnum].Count + numArray[(int) battleSideEnum]));
      }
      return 1f;
    }

    protected override void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationMoraleChangeMessage(this.MoraleRounded));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }
  }
}
