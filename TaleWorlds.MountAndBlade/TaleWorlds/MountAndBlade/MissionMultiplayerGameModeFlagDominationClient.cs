// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerGameModeFlagDominationClient
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
  public class MissionMultiplayerGameModeFlagDominationClient : 
    MissionMultiplayerGameModeBaseClient,
    ICommanderInfo,
    IMissionBehavior
  {
    private const float MySideMoraleDropThreshold = 0.4f;
    private float _remainingTimeForBellSoundToStop = float.MinValue;
    private SoundEvent _bellSoundEvent;
    private FlagDominationMissionRepresentative _myRepresentative;
    private MissionScoreboardComponent _scoreboardComponent;
    private MissionLobbyComponent.MultiplayerGameType _currentGameType;
    private Team[] _capturePointOwners;
    private bool _informedAboutFlagRemoval;

    public override bool IsGameModeUsingGold => this.GameType == MissionLobbyComponent.MultiplayerGameType.Skirmish;

    public override bool IsGameModeTactical => true;

    public override bool IsGameModeUsingRoundCountdown => true;

    public override MissionLobbyComponent.MultiplayerGameType GameType => this._currentGameType;

    public event Action<NetworkCommunicator> OnBotsControlledChangedEvent;

    public event Action<BattleSideEnum, float> OnTeamPowerChangedEvent;

    public event Action<BattleSideEnum, float> OnMoraleChangedEvent;

    public event Action OnFlagNumberChangedEvent;

    public event Action<FlagCapturePoint, Team> OnCapturePointOwnerChangedEvent;

    public IEnumerable<FlagCapturePoint> AllCapturePoints { get; private set; }

    public bool AreMoralesIndependent => false;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._scoreboardComponent = Mission.Current.GetMissionBehaviour<MissionScoreboardComponent>();
      this._currentGameType = MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0 ? MissionLobbyComponent.MultiplayerGameType.Captain : MissionLobbyComponent.MultiplayerGameType.Skirmish;
      this.ResetTeamPowers();
      this._capturePointOwners = new Team[3];
      this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>();
      this.RoundComponent.OnPreparationEnded += new Action(this.OnPreparationEnded);
      NetworkCommunicator.OnPeerComponentAdded += new Action<PeerComponent>(this.OnPeerComponentAdded);
    }

    public override void OnRemoveBehaviour()
    {
      this.RoundComponent.OnPreparationEnded -= new Action(this.OnPreparationEnded);
      NetworkCommunicator.OnPeerComponentAdded -= new Action<PeerComponent>(this.OnPeerComponentAdded);
    }

    private void OnPeerComponentAdded(PeerComponent component)
    {
      if (!component.IsMine || !(component is MissionRepresentativeBase))
        return;
      this._myRepresentative = component as FlagDominationMissionRepresentative;
    }

    public override void AfterStart() => Mission.Current.SetMissionMode(MissionMode.Battle, true);

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (!GameNetwork.IsClient)
        return;
      registerer.Register<BotsControlledChange>(new GameNetworkMessage.ServerMessageHandlerDelegate<BotsControlledChange>(this.HandleServerEventBotsControlledChangeEvent));
      registerer.Register<FlagDominationMoraleChangeMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationMoraleChangeMessage>(this.HandleMoraleChangedMessage));
      registerer.Register<SyncGoldsForSkirmish>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncGoldsForSkirmish>(this.HandleServerEventUpdateGold));
      registerer.Register<FlagDominationFlagsRemovedMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationFlagsRemovedMessage>(this.HandleFlagsRemovedMessage));
      registerer.Register<FlagDominationCapturePointMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FlagDominationCapturePointMessage>(this.HandleServerEventPointCapturedMessage));
      registerer.Register<FormationWipedMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<FormationWipedMessage>(this.HandleServerEventFormationWipedMessage));
    }

    public void OnPreparationEnded()
    {
      this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>();
      Action numberChangedEvent = this.OnFlagNumberChangedEvent;
      if (numberChangedEvent != null)
        numberChangedEvent();
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        Action<FlagCapturePoint, Team> ownerChangedEvent = this.OnCapturePointOwnerChangedEvent;
        if (ownerChangedEvent != null)
          ownerChangedEvent(allCapturePoint, (Team) null);
      }
    }

    public override SpectatorCameraTypes GetMissionCameraLockMode(
      bool lockedToMainPlayer)
    {
      SpectatorCameraTypes spectatorCameraTypes = SpectatorCameraTypes.Invalid;
      MissionPeer missionPeer = GameNetwork.IsMyPeerReady ? GameNetwork.MyPeer.GetComponent<MissionPeer>() : (MissionPeer) null;
      if (!lockedToMainPlayer && missionPeer != null)
      {
        if (missionPeer.Team != this.Mission.SpectatorTeam)
        {
          if (this.GameType == MissionLobbyComponent.MultiplayerGameType.Captain && this.IsRoundInProgress)
          {
            Formation controlledFormation = missionPeer.ControlledFormation;
            if (controlledFormation != null && controlledFormation.HasUnitsWithCondition((Func<Agent, bool>) (agent => !agent.IsPlayerControlled && agent.IsActive())))
              spectatorCameraTypes = SpectatorCameraTypes.LockToPlayerFormation;
          }
        }
        else
          spectatorCameraTypes = SpectatorCameraTypes.Free;
      }
      return spectatorCameraTypes;
    }

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
      if (!this.IsRoundInProgress || affectedAgent.IsMount)
        return;
      Team team = affectedAgent.Team;
      if (this.IsGameModeUsingGold)
        this.UpdateTeamPowerBasedOnGold(team);
      else
        this.UpdateTeamPowerBasedOnTroopCount(team);
    }

    public override void OnClearScene()
    {
      this._informedAboutFlagRemoval = false;
      this.AllCapturePoints = Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>();
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
        this._capturePointOwners[allCapturePoint.FlagIndex] = (Team) null;
      this.ResetTeamPowers();
      if (this._bellSoundEvent == null)
        return;
      this._remainingTimeForBellSoundToStop = float.MinValue;
      this._bellSoundEvent.Stop();
      this._bellSoundEvent = (SoundEvent) null;
    }

    protected override int GetWarningTimer()
    {
      int num1 = 0;
      if (this.IsRoundInProgress)
      {
        float num2 = 180f;
        if (this.GameType == MissionLobbyComponent.MultiplayerGameType.Skirmish)
          num2 = 120f;
        float num3 = (float) MultiplayerOptions.OptionType.RoundTimeLimit.GetIntValue() - num2;
        float num4 = num3 + 30f;
        if ((double) this.RoundComponent.RemainingRoundTime <= (double) num4 && (double) this.RoundComponent.RemainingRoundTime > (double) num3)
        {
          num1 = MBMath.Ceiling((float) (30.0 - ((double) num4 - (double) this.RoundComponent.RemainingRoundTime)));
          if (!this._informedAboutFlagRemoval)
          {
            this._informedAboutFlagRemoval = true;
            this.NotificationsComponent.FlagsWillBeRemovedInXSeconds(30);
          }
        }
      }
      return num1;
    }

    public Team GetFlagOwner(FlagCapturePoint flag) => this._capturePointOwners[flag.FlagIndex];

    private void HandleServerEventBotsControlledChangeEvent(BotsControlledChange message) => this.OnBotsControlledChanged(message.Peer.GetComponent<MissionPeer>(), message.AliveCount, message.TotalCount);

    private void HandleMoraleChangedMessage(FlagDominationMoraleChangeMessage message) => this.OnMoraleChanged(message.Morale);

    private void HandleServerEventUpdateGold(SyncGoldsForSkirmish message) => this.OnGoldAmountChangedForRepresentative((MissionRepresentativeBase) message.VirtualPlayer.GetComponent<FlagDominationMissionRepresentative>(), message.GoldAmount);

    private void HandleFlagsRemovedMessage(FlagDominationFlagsRemovedMessage message) => this.OnNumberOfFlagsChanged();

    private void HandleServerEventPointCapturedMessage(FlagDominationCapturePointMessage message)
    {
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        if (allCapturePoint.FlagIndex == message.FlagIndex)
        {
          this.OnCapturePointOwnerChanged(allCapturePoint, message.OwnerTeam);
          break;
        }
      }
    }

    private void HandleServerEventFormationWipedMessage(FormationWipedMessage message)
    {
      MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
      Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
      MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/squad_wiped"), position);
    }

    public void OnTeamPowerChanged(BattleSideEnum teamSide, float power)
    {
      Action<BattleSideEnum, float> powerChangedEvent = this.OnTeamPowerChangedEvent;
      if (powerChangedEvent == null)
        return;
      powerChangedEvent(teamSide, power);
    }

    public void OnMoraleChanged(float morale)
    {
      for (int index = 0; index < 2; ++index)
      {
        float num = (float) (((double) morale + 1.0) / 2.0);
        if (index == 0)
        {
          Action<BattleSideEnum, float> moraleChangedEvent = this.OnMoraleChangedEvent;
          if (moraleChangedEvent != null)
            moraleChangedEvent(BattleSideEnum.Defender, 1f - num);
        }
        else if (index == 1)
        {
          Action<BattleSideEnum, float> moraleChangedEvent = this.OnMoraleChangedEvent;
          if (moraleChangedEvent != null)
            moraleChangedEvent(BattleSideEnum.Attacker, num);
        }
      }
      if (this._myRepresentative?.MissionPeer.Team == null || this._myRepresentative.MissionPeer.Team.Side == BattleSideEnum.None)
        return;
      float num1 = Math.Abs(morale);
      if ((double) this._remainingTimeForBellSoundToStop < 0.0)
      {
        this._remainingTimeForBellSoundToStop = (double) num1 < 0.600000023841858 || (double) num1 >= 1.0 ? float.MinValue : float.MaxValue;
        if ((double) this._remainingTimeForBellSoundToStop <= 0.0)
          return;
        BattleSideEnum side = this._myRepresentative.MissionPeer.Team.Side;
        this._bellSoundEvent = side == BattleSideEnum.Defender && (double) morale >= 0.600000023841858 || side == BattleSideEnum.Attacker && (double) morale <= -0.600000023841858 ? SoundEvent.CreateEventFromString("event:/multiplayer/warning_bells_defender", this.Mission.Scene) : SoundEvent.CreateEventFromString("event:/multiplayer/warning_bells_attacker", this.Mission.Scene);
        MatrixFrame globalFrame = this.AllCapturePoints.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => !cp.IsDeactivated)).GetRandomElementInefficiently<FlagCapturePoint>().GameEntity.GetGlobalFrame();
        this._bellSoundEvent.PlayInPosition(globalFrame.origin + globalFrame.rotation.u * 3f);
      }
      else
      {
        if ((double) num1 < 1.0 && (double) num1 >= 0.600000023841858)
          return;
        this._remainingTimeForBellSoundToStop = float.MinValue;
      }
    }

    public override void OnGoldAmountChangedForRepresentative(
      MissionRepresentativeBase representative,
      int goldAmount)
    {
      if (representative == null)
        return;
      MissionPeer component = representative.GetComponent<MissionPeer>();
      if (component == null)
        return;
      representative.UpdateGold(goldAmount);
      this._scoreboardComponent.PlayerPropertiesChanged(component);
      if (!this.IsGameModeUsingGold || !this.IsRoundInProgress || (component.Team == null || component.Team.Side == BattleSideEnum.None))
        return;
      this.UpdateTeamPowerBasedOnGold(component.Team);
    }

    public void OnNumberOfFlagsChanged()
    {
      Action numberChangedEvent = this.OnFlagNumberChangedEvent;
      if (numberChangedEvent == null)
        return;
      numberChangedEvent();
    }

    public void OnBotsControlledChanged(
      MissionPeer missionPeer,
      int botAliveCount,
      int botTotalCount)
    {
      missionPeer.BotsUnderControlAlive = botAliveCount;
      missionPeer.BotsUnderControlTotal = botTotalCount;
      Action<NetworkCommunicator> controlledChangedEvent = this.OnBotsControlledChangedEvent;
      if (controlledChangedEvent == null)
        return;
      controlledChangedEvent(missionPeer.GetNetworkPeer());
    }

    public void OnCapturePointOwnerChanged(FlagCapturePoint flagCapturePoint, Team ownerTeam)
    {
      this._capturePointOwners[flagCapturePoint.FlagIndex] = ownerTeam;
      Action<FlagCapturePoint, Team> ownerChangedEvent = this.OnCapturePointOwnerChangedEvent;
      if (ownerChangedEvent != null)
        ownerChangedEvent(flagCapturePoint, ownerTeam);
      if (this._myRepresentative == null || this._myRepresentative.MissionPeer.Team == null)
        return;
      MatrixFrame cameraFrame = Mission.Current.GetCameraFrame();
      Vec3 position = cameraFrame.origin + cameraFrame.rotation.u;
      if (this._myRepresentative.MissionPeer.Team == ownerTeam)
        MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/flag_captured"), position);
      else
        MBSoundEvent.PlaySound(SoundEvent.GetEventIdFromString("event:/alerts/report/flag_lost"), position);
    }

    public void OnRequestForfeitSpawn()
    {
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new RequestForfeitSpawn());
        GameNetwork.EndModuleEventAsClient();
      }
      else
        Mission.Current.GetMissionBehaviour<MissionMultiplayerFlagDomination>().ForfeitSpawning(GameNetwork.MyPeer);
    }

    private void ResetTeamPowers(float value = 1f)
    {
      Action<BattleSideEnum, float> powerChangedEvent1 = this.OnTeamPowerChangedEvent;
      if (powerChangedEvent1 != null)
        powerChangedEvent1(BattleSideEnum.Attacker, value);
      Action<BattleSideEnum, float> powerChangedEvent2 = this.OnTeamPowerChangedEvent;
      if (powerChangedEvent2 == null)
        return;
      powerChangedEvent2(BattleSideEnum.Defender, value);
    }

    private void UpdateTeamPowerBasedOnGold(Team team)
    {
      int num1 = 0;
      int num2 = 0;
      foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
      {
        if (peer.Team != null && peer.Team.Side == team.Side)
        {
          int gold = peer.GetComponent<FlagDominationMissionRepresentative>().Gold;
          if (gold >= 100)
            num2 += gold;
          if (peer.ControlledAgent != null && peer.ControlledAgent.IsActive())
          {
            MultiplayerClassDivisions.MPHeroClass classForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(peer.ControlledAgent.Character);
            num2 += classForCharacter.TroopCost;
          }
          ++num1;
        }
      }
      int num3 = num1 + (team.Side == BattleSideEnum.Attacker ? MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue() : MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue());
      foreach (Agent activeAgent in team.ActiveAgents)
      {
        if (activeAgent.MissionPeer == null)
          num2 += 300;
      }
      int num4 = num3 * 300;
      float num5 = Math.Min(1f, num4 == 0 ? 0.0f : (float) num2 / (float) num4);
      Action<BattleSideEnum, float> powerChangedEvent = this.OnTeamPowerChangedEvent;
      if (powerChangedEvent == null)
        return;
      powerChangedEvent(team.Side, num5);
    }

    private void UpdateTeamPowerBasedOnTroopCount(Team team)
    {
      int count = team.ActiveAgents.Count;
      float num = (float) count / (float) (count + team.QuerySystem.DeathCount);
      Action<BattleSideEnum, float> powerChangedEvent = this.OnTeamPowerChangedEvent;
      if (powerChangedEvent == null)
        return;
      powerChangedEvent(team.Side, num);
    }

    public override List<CompassItemUpdateParams> GetCompassTargets()
    {
      List<CompassItemUpdateParams> itemUpdateParamsList = new List<CompassItemUpdateParams>();
      if (!GameNetwork.IsMyPeerReady || !this.IsRoundInProgress)
        return itemUpdateParamsList;
      MissionPeer component1 = GameNetwork.MyPeer.GetComponent<MissionPeer>();
      if (component1 == null || component1.Team == null || component1.Team.Side == BattleSideEnum.None)
        return itemUpdateParamsList;
      foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => !cp.IsDeactivated)))
      {
        int num = 17 + flagCapturePoint.FlagIndex;
        itemUpdateParamsList.Add(new CompassItemUpdateParams((object) flagCapturePoint, (TargetIconType) num, flagCapturePoint.Position, flagCapturePoint.GetFlagColor(), flagCapturePoint.GetFlagColor2()));
      }
      bool flag1 = true;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component2 = networkPeer.GetComponent<MissionPeer>();
        if (component2?.Team != null && component2.Team.Side != BattleSideEnum.None)
        {
          bool flag2 = component2.ControlledFormation != null;
          if (!flag2)
            flag1 = false;
          if (flag1 || component2.Team == component1.Team)
          {
            MultiplayerClassDivisions.MPHeroClass heroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(component2);
            if (flag2)
            {
              Formation controlledFormation = component2.ControlledFormation;
              if (controlledFormation.CountOfUnits != 0)
              {
                WorldPosition medianPosition = controlledFormation.QuerySystem.MedianPosition;
                Vec2 vec2 = controlledFormation.SmoothedAverageUnitPosition;
                if (!vec2.IsValid)
                  vec2 = controlledFormation.QuerySystem.AveragePosition;
                medianPosition.SetVec2(vec2);
                BannerCode bannerCode = (BannerCode) null;
                bool isAttacker = false;
                bool isAlly = false;
                if (controlledFormation.Team != null)
                {
                  if (controlledFormation.Banner == null)
                    controlledFormation.Banner = new Banner(controlledFormation.BannerCode, controlledFormation.Team.Color, controlledFormation.Team.Color2);
                  isAttacker = controlledFormation.Team.IsAttacker;
                  isAlly = controlledFormation.Team.IsPlayerAlly;
                  bannerCode = BannerCode.CreateFrom(controlledFormation.Banner);
                }
                TargetIconType targetType = heroClassForPeer != null ? heroClassForPeer.IconType : TargetIconType.None;
                itemUpdateParamsList.Add(new CompassItemUpdateParams((object) controlledFormation, targetType, medianPosition.GetNavMeshVec3(), bannerCode, isAttacker, isAlly));
              }
            }
            else
            {
              Agent controlledAgent = component2.ControlledAgent;
              if (controlledAgent != null && controlledAgent.IsActive() && controlledAgent.Controller != Agent.ControllerType.Player)
              {
                BannerCode from = BannerCode.CreateFrom(new Banner(component2.Peer.BannerCode, component2.Team.Color, component2.Team.Color2));
                itemUpdateParamsList.Add(new CompassItemUpdateParams((object) controlledAgent, heroClassForPeer.IconType, controlledAgent.Position, from, component2.Team.IsAttacker, component2.Team.IsPlayerAlly));
              }
            }
          }
        }
      }
      return itemUpdateParamsList;
    }

    public override int GetGoldAmount() => this._myRepresentative.Gold;

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if ((double) this._remainingTimeForBellSoundToStop > 0.0)
        this._remainingTimeForBellSoundToStop -= dt;
      if (this._bellSoundEvent == null || (double) this._remainingTimeForBellSoundToStop > 0.0 && this.MissionLobbyComponent.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Playing)
        return;
      this._remainingTimeForBellSoundToStop = float.MinValue;
      this._bellSoundEvent.Stop();
      this._bellSoundEvent = (SoundEvent) null;
    }
  }
}
