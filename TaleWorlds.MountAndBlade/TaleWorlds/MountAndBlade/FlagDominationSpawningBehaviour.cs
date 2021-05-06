// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FlagDominationSpawningBehaviour
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  internal class FlagDominationSpawningBehaviour : SpawningBehaviourBase
  {
    private const int EnforcedSpawnTimeInSeconds = 15;
    private float _spawningTimer;
    private bool _spawningTimerTicking;
    private bool _haveBotsBeenSpawned;
    private bool _roundInitialSpawnOver;
    private MissionMultiplayerFlagDomination _flagDominationMissionController;
    private MultiplayerRoundController _roundController;
    private List<KeyValuePair<MissionPeer, Timer>> _enforcedSpawnTimers;

    public FlagDominationSpawningBehaviour() => this._enforcedSpawnTimers = new List<KeyValuePair<MissionPeer, Timer>>();

    public override void Initialize(SpawnComponent spawnComponent)
    {
      base.Initialize(spawnComponent);
      this._flagDominationMissionController = this.Mission.GetMissionBehaviour<MissionMultiplayerFlagDomination>();
      this._roundController = this.Mission.GetMissionBehaviour<MultiplayerRoundController>();
      this._roundController.OnRoundStarted += new Action(((SpawningBehaviourBase) this).RequestStartSpawnSession);
      this._roundController.OnRoundEnding += new Action(((SpawningBehaviourBase) this).RequestStopSpawnSession);
      if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() == 0)
        this._roundController.EnableEquipmentUpdate();
      this.OnAllAgentsFromPeerSpawnedFromVisuals += new Action<MissionPeer>(this.OnAllAgentsFromPeerSpawnedFromVisuals);
      this.OnPeerSpawnedFromVisuals += new Action<MissionPeer>(this.OnPeerSpawnedFromVisuals);
    }

    public override void Clear()
    {
      base.Clear();
      this._roundController.OnRoundStarted -= new Action(((SpawningBehaviourBase) this).RequestStartSpawnSession);
      this._roundController.OnRoundEnding -= new Action(((SpawningBehaviourBase) this).RequestStopSpawnSession);
      this.OnAllAgentsFromPeerSpawnedFromVisuals -= new Action<MissionPeer>(this.OnAllAgentsFromPeerSpawnedFromVisuals);
      this.OnPeerSpawnedFromVisuals -= new Action<MissionPeer>(this.OnPeerSpawnedFromVisuals);
    }

    public override void OnTick(float dt)
    {
      if (this._spawningTimerTicking)
        this._spawningTimer += dt;
      if (this.IsSpawningEnabled)
      {
        if (!this._roundInitialSpawnOver && this.IsRoundInProgress())
        {
          foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
          {
            MissionPeer component = networkPeer.GetComponent<MissionPeer>();
            if (component?.Team != null && component.Team.Side != BattleSideEnum.None)
              this.SpawnComponent.SetEarlyAgentVisualsDespawning(component);
          }
          this._roundInitialSpawnOver = true;
          this.Mission.AllowAiTicking = true;
        }
        this.SpawnAgents();
        if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0 && (double) this._spawningTimer > (double) MultiplayerOptions.OptionType.RoundPreparationTimeLimit.GetIntValue())
        {
          this.IsSpawningEnabled = false;
          this._spawningTimer = 0.0f;
          this._spawningTimerTicking = false;
        }
      }
      base.OnTick(dt);
    }

    public override void RequestStartSpawnSession()
    {
      if (this.IsSpawningEnabled)
        return;
      Mission.Current.SetBattleAgentCount(-1);
      this.IsSpawningEnabled = true;
      this._haveBotsBeenSpawned = false;
      this._spawningTimerTicking = true;
      this.ResetSpawnCounts();
      this.ResetSpawnTimers();
    }

    protected override void SpawnAgents()
    {
      BasicCultureObject basicCultureObject1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
      BasicCultureObject basicCultureObject2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());
      if (!this._haveBotsBeenSpawned && (MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue() > 0 || MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue() > 0))
      {
        Mission.Current.AllowAiTicking = false;
        List<string> stringList = new List<string>()
        {
          "11.8.1.4345.4345.770.774.1.0.0.133.7.5.512.512.784.769.1.0.0",
          "11.8.1.4345.4345.770.774.1.0.0.156.7.5.512.512.784.769.1.0.0",
          "11.8.1.4345.4345.770.774.1.0.0.155.7.5.512.512.784.769.1.0.0",
          "11.8.1.4345.4345.770.774.1.0.0.158.7.5.512.512.784.769.1.0.0",
          "11.8.1.4345.4345.770.774.1.0.0.118.7.5.512.512.784.769.1.0.0",
          "11.8.1.4345.4345.770.774.1.0.0.149.7.5.512.512.784.769.1.0.0"
        };
        foreach (Team team in (ReadOnlyCollection<Team>) this.Mission.Teams)
        {
          if (this.Mission.AttackerTeam == team || this.Mission.DefenderTeam == team)
          {
            BasicCultureObject teamCulture = team == this.Mission.AttackerTeam ? basicCultureObject1 : basicCultureObject2;
            int num1 = this.Mission.AttackerTeam == team ? MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue() : MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue();
            int num2 = 0;
            for (int index1 = 0; index1 < num1; ++index1)
            {
              Formation formation = (Formation) null;
              if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
              {
                while (formation == null || formation.PlayerOwner != null)
                {
                  FormationClass formationClass = (FormationClass) num2;
                  formation = team.GetFormation(formationClass);
                  ++num2;
                }
              }
              if (formation != null)
                formation.BannerCode = stringList[num2 - 1];
              MultiplayerClassDivisions.MPHeroClass elementWithPredicate = MultiplayerClassDivisions.GetMPHeroClasses().GetRandomElementWithPredicate<MultiplayerClassDivisions.MPHeroClass>((Func<MultiplayerClassDivisions.MPHeroClass, bool>) (x => x.Culture == teamCulture));
              BasicCharacterObject heroCharacter = elementWithPredicate.HeroCharacter;
              AgentBuildData agentBuildData = new AgentBuildData(heroCharacter);
              agentBuildData.Equipment(elementWithPredicate.HeroCharacter.Equipment);
              agentBuildData.TroopOrigin((IAgentOriginBase) new BasicBattleAgentOrigin(heroCharacter));
              agentBuildData.EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(heroCharacter));
              agentBuildData.Team(team);
              agentBuildData.VisualsIndex(0);
              if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() == 0)
                agentBuildData.InitialFrame(this.SpawnComponent.GetSpawnFrame(team, elementWithPredicate.HeroCharacter.Equipment[EquipmentIndex.ArmorItemEndSlot].Item != null, true));
              agentBuildData.Formation(formation);
              agentBuildData.SpawnOnInitialPoint(true);
              agentBuildData.IsFemale(heroCharacter.IsFemale);
              agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentIsFemale, heroCharacter.GetBodyPropertiesMin(), heroCharacter.GetBodyPropertiesMax(), (int) agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, heroCharacter.HairTags, heroCharacter.BeardTags, heroCharacter.TattooTags));
              agentBuildData.ClothingColor1(team.Side == BattleSideEnum.Attacker ? teamCulture.Color : teamCulture.ClothAlternativeColor);
              agentBuildData.ClothingColor2(team.Side == BattleSideEnum.Attacker ? teamCulture.Color2 : teamCulture.ClothAlternativeColor2);
              Agent agent = this.Mission.SpawnAgent(agentBuildData);
              agent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
              agent.WieldInitialWeapons();
              if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
              {
                int num3 = (int) Math.Ceiling((double) MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() * (double) elementWithPredicate.TroopMultiplier);
                for (int index2 = 0; index2 < num3; ++index2)
                  this.SpawnBotInBotFormation(index2 + 1, team, teamCulture, elementWithPredicate.TroopCharacter.StringId, formation);
                this.BotFormationSpawned(team);
                formation.IsAIControlled = true;
              }
            }
            if (num1 > 0 && team.Formations.Any<Formation>())
            {
              TeamAIGeneral teamAiGeneral = new TeamAIGeneral(Mission.Current, team);
              teamAiGeneral.AddTacticOption((TacticComponent) new TacticSergeantMPBotTactic(team));
              team.AddTeamAI((TeamAIComponent) teamAiGeneral);
            }
          }
        }
        this.AllBotFormationsSpawned();
        this._haveBotsBeenSpawned = true;
      }
      foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
      {
        NetworkCommunicator networkPeer = peer.GetNetworkPeer();
        if (networkPeer.IsSynchronized && peer.Team != null && peer.Team.Side != BattleSideEnum.None && (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() != 0 || !this.CheckIfEnforcedSpawnTimerExpiredForPeer(peer)))
        {
          Team team = peer.Team;
          int num = team == this.Mission.AttackerTeam ? 1 : 0;
          Team defenderTeam = this.Mission.DefenderTeam;
          BasicCultureObject basicCultureObject3 = num != 0 ? basicCultureObject1 : basicCultureObject2;
          MultiplayerClassDivisions.MPHeroClass heroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(peer);
          if (peer.ControlledAgent == null && !peer.HasSpawnedAgentVisuals && (peer.Team != null && peer.Team != this.Mission.SpectatorTeam) && peer.SpawnTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
          {
            int currentGoldForPeer = this._flagDominationMissionController.GetCurrentGoldForPeer(peer);
            if (heroClassForPeer == null || this._flagDominationMissionController.UseGold() && heroClassForPeer.TroopCost > currentGoldForPeer)
            {
              if (currentGoldForPeer >= MultiplayerClassDivisions.GetMinimumTroopCost(basicCultureObject3) && peer.SelectedTroopIndex != 0)
              {
                peer.SelectedTroopIndex = 0;
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage((GameNetworkMessage) new UpdateSelectedTroopIndex(networkPeer, 0));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, networkPeer);
              }
            }
            else
            {
              if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() == 0)
                this.CreateEnforcedSpawnTimerForPeer(peer, 15);
              Formation formation = peer.ControlledFormation;
              if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0 && formation == null)
              {
                FormationClass formationIndex = peer.Team.FormationsIncludingEmpty.First<Formation>((Func<Formation, bool>) (x => x.PlayerOwner == null && !x.ContainsAgentVisuals && x.CountOfUnits == 0)).FormationIndex;
                formation = team.GetFormation(formationIndex);
                formation.ContainsAgentVisuals = true;
                if (formation.BannerCode.IsStringNoneOrEmpty())
                  formation.BannerCode = peer.Peer.BannerCode;
              }
              BasicCharacterObject heroCharacter = heroClassForPeer.HeroCharacter;
              AgentBuildData agentBuildData = new AgentBuildData(heroCharacter);
              agentBuildData.MissionPeer(peer);
              MPPerkObject.MPOnSpawnPerkHandler spawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(peer);
              Equipment equipment = heroCharacter.Equipment.Clone();
              IEnumerable<(EquipmentIndex, EquipmentElement)> alternativeEquipments1 = spawnPerkHandler?.GetAlternativeEquipments(true);
              if (alternativeEquipments1 != null)
              {
                foreach ((EquipmentIndex, EquipmentElement) tuple in alternativeEquipments1)
                  equipment[tuple.Item1] = tuple.Item2;
              }
              int agentVisualsForPeer = peer.GetAmountOfAgentVisualsForPeer();
              bool updateExistingAgentVisuals = agentVisualsForPeer > 0;
              agentBuildData.Equipment(equipment);
              agentBuildData.Team(peer.Team);
              agentBuildData.VisualsIndex(0);
              if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() == 0)
              {
                if (!updateExistingAgentVisuals)
                {
                  agentBuildData.InitialFrame(this.SpawnComponent.GetSpawnFrame(peer.Team, equipment[EquipmentIndex.ArmorItemEndSlot].Item != null, true));
                }
                else
                {
                  MatrixFrame frame = peer.GetAgentVisualForPeer(0).GetFrame();
                  frame.rotation.MakeUnit();
                  agentBuildData.InitialFrame(frame);
                }
              }
              agentBuildData.Formation(formation);
              agentBuildData.SpawnOnInitialPoint(true);
              agentBuildData.MakeUnitStandOutOfFormationDistance(7f);
              agentBuildData.IsFemale(peer.Peer.IsFemale);
              BodyProperties bodyProperties = this.GetBodyProperties(peer, peer.Team == this.Mission.AttackerTeam ? basicCultureObject1 : basicCultureObject2);
              agentBuildData.BodyProperties(bodyProperties);
              agentBuildData.ClothingColor1(team == this.Mission.AttackerTeam ? basicCultureObject3.Color : basicCultureObject3.ClothAlternativeColor);
              agentBuildData.ClothingColor2(team == this.Mission.AttackerTeam ? basicCultureObject3.Color2 : basicCultureObject3.ClothAlternativeColor2);
              if (this.GameMode.ShouldSpawnVisualsForServer(networkPeer))
                this.AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(peer, agentBuildData, peer.SelectedTroopIndex);
              this.GameMode.HandleAgentVisualSpawning(networkPeer, agentBuildData);
              peer.ControlledFormation = formation;
              if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() > 0)
              {
                int troopCount = MPPerkObject.GetTroopCount(heroClassForPeer, spawnPerkHandler);
                IEnumerable<(EquipmentIndex, EquipmentElement)> alternativeEquipments2 = spawnPerkHandler?.GetAlternativeEquipments(false);
                for (int index = 0; index < troopCount; ++index)
                {
                  if (index + 1 >= agentVisualsForPeer)
                    updateExistingAgentVisuals = false;
                  this.SpawnBotVisualsInPlayerFormation(peer, index + 1, team, basicCultureObject3, heroClassForPeer.TroopCharacter.StringId, formation, updateExistingAgentVisuals, troopCount, alternativeEquipments2);
                }
              }
            }
          }
        }
      }
    }

    private void OnPeerSpawnedFromVisuals(MissionPeer peer)
    {
      if (peer.ControlledFormation == null)
        return;
      peer.ControlledAgent.Team.AssignPlayerAsSergeantOfFormation(peer, peer.ControlledFormation.FormationIndex);
    }

    private void OnAllAgentsFromPeerSpawnedFromVisuals(MissionPeer peer)
    {
      if (peer.ControlledFormation != null)
      {
        peer.ControlledFormation.OnFormationDispersed();
        peer.ControlledFormation.MovementOrder = MovementOrder.MovementOrderFollow(peer.ControlledAgent);
        NetworkCommunicator networkPeer = peer.GetNetworkPeer();
        if (peer.BotsUnderControlAlive != 0 || peer.BotsUnderControlTotal != 0)
        {
          GameNetwork.BeginBroadcastModuleEvent();
          GameNetwork.WriteMessage((GameNetworkMessage) new BotsControlledChange(networkPeer, peer.BotsUnderControlAlive, peer.BotsUnderControlTotal));
          GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
          this.Mission.GetMissionBehaviour<MissionMultiplayerGameModeFlagDominationClient>().OnBotsControlledChanged(peer, peer.BotsUnderControlAlive, peer.BotsUnderControlTotal);
        }
        if (peer.Team == this.Mission.AttackerTeam)
          ++this.Mission.NumOfFormationsSpawnedTeamOne;
        else
          ++this.Mission.NumOfFormationsSpawnedTeamTwo;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetSpawnedFormationCount(this.Mission.NumOfFormationsSpawnedTeamOne, this.Mission.NumOfFormationsSpawnedTeamTwo));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
      if (!this._flagDominationMissionController.UseGold())
        return;
      bool flag = peer.Team == this.Mission.AttackerTeam;
      Team defenderTeam = this.Mission.DefenderTeam;
      MultiplayerClassDivisions.MPHeroClass mpHeroClass = MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(flag ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue() : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue())).ElementAt<MultiplayerClassDivisions.MPHeroClass>(peer.SelectedTroopIndex);
      this._flagDominationMissionController.ChangeCurrentGoldForPeer(peer, this._flagDominationMissionController.GetCurrentGoldForPeer(peer) - mpHeroClass.TroopCost);
    }

    private void BotFormationSpawned(Team team)
    {
      if (team == this.Mission.AttackerTeam)
      {
        ++this.Mission.NumOfFormationsSpawnedTeamOne;
      }
      else
      {
        if (team != this.Mission.DefenderTeam)
          return;
        ++this.Mission.NumOfFormationsSpawnedTeamTwo;
      }
    }

    private void AllBotFormationsSpawned()
    {
      if (this.Mission.NumOfFormationsSpawnedTeamOne == 0 && this.Mission.NumOfFormationsSpawnedTeamTwo == 0)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new SetSpawnedFormationCount(this.Mission.NumOfFormationsSpawnedTeamOne, this.Mission.NumOfFormationsSpawnedTeamTwo));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    public override bool AllowEarlyAgentVisualsDespawning(MissionPeer lobbyPeer)
    {
      if (MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() != 0 || !this._roundController.IsRoundInProgress)
        return false;
      if (!lobbyPeer.HasSpawnTimerExpired && lobbyPeer.SpawnTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        lobbyPeer.HasSpawnTimerExpired = true;
      return lobbyPeer.HasSpawnTimerExpired;
    }

    protected override bool IsRoundInProgress() => this._roundController.IsRoundInProgress;

    private void CreateEnforcedSpawnTimerForPeer(MissionPeer peer, int durationInSeconds)
    {
      if (this._enforcedSpawnTimers.Any<KeyValuePair<MissionPeer, Timer>>((Func<KeyValuePair<MissionPeer, Timer>, bool>) (pair => pair.Key == peer)))
        return;
      this._enforcedSpawnTimers.Add(new KeyValuePair<MissionPeer, Timer>(peer, new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), (float) durationInSeconds)));
      Debug.Print("EST for " + peer.Name + " set to " + (object) durationInSeconds + " seconds.", color: Debug.DebugColor.Yellow, debugFilter: 64UL);
    }

    private bool CheckIfEnforcedSpawnTimerExpiredForPeer(MissionPeer peer)
    {
      KeyValuePair<MissionPeer, Timer> keyValuePair = this._enforcedSpawnTimers.FirstOrDefault<KeyValuePair<MissionPeer, Timer>>((Func<KeyValuePair<MissionPeer, Timer>, bool>) (pr => pr.Key == peer));
      if (keyValuePair.Key == null)
        return false;
      if (peer.ControlledAgent != null)
      {
        this._enforcedSpawnTimers.RemoveAll((Predicate<KeyValuePair<MissionPeer, Timer>>) (p => p.Key == peer));
        Debug.Print("EST for " + peer.Name + " is no longer valid (spawned already).", color: Debug.DebugColor.Yellow, debugFilter: 64UL);
        return false;
      }
      Timer timer = keyValuePair.Value;
      if (!peer.HasSpawnedAgentVisuals || !timer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        return false;
      this.SpawnComponent.SetEarlyAgentVisualsDespawning(peer);
      this._enforcedSpawnTimers.RemoveAll((Predicate<KeyValuePair<MissionPeer, Timer>>) (p => p.Key == peer));
      Debug.Print("EST for " + peer.Name + " has expired.", color: Debug.DebugColor.Yellow, debugFilter: 64UL);
      return true;
    }

    public override void OnClearScene()
    {
      base.OnClearScene();
      this._enforcedSpawnTimers.Clear();
      this._roundInitialSpawnOver = false;
    }

    protected void SpawnBotInBotFormation(
      int visualsIndex,
      Team agentTeam,
      BasicCultureObject cultureLimit,
      string troopName,
      Formation formation)
    {
      BasicCharacterObject basicCharacterObject = MBObjectManager.Instance.GetObject<BasicCharacterObject>(troopName);
      AgentBuildData agentBuildData = new AgentBuildData(basicCharacterObject);
      agentBuildData.Team(agentTeam);
      agentBuildData.TroopOrigin((IAgentOriginBase) new BasicBattleAgentOrigin(basicCharacterObject));
      agentBuildData.VisualsIndex(visualsIndex);
      agentBuildData.EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(basicCharacterObject, visualsIndex));
      agentBuildData.Equipment(Equipment.GetRandomEquipmentElements(basicCharacterObject, !(Game.Current.GameType is MultiplayerGame), seed: agentBuildData.AgentEquipmentSeed));
      agentBuildData.SpawnOnInitialPoint(true);
      agentBuildData.Formation(formation);
      agentBuildData.ClothingColor1(agentTeam.Side == BattleSideEnum.Attacker ? cultureLimit.Color : cultureLimit.ClothAlternativeColor);
      agentBuildData.ClothingColor2(agentTeam.Side == BattleSideEnum.Attacker ? cultureLimit.Color2 : cultureLimit.ClothAlternativeColor2);
      agentBuildData.IsFemale(basicCharacterObject.IsFemale);
      agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentIsFemale, basicCharacterObject.GetBodyPropertiesMin(), basicCharacterObject.GetBodyPropertiesMax(), (int) agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, basicCharacterObject.HairTags, basicCharacterObject.BeardTags, basicCharacterObject.TattooTags));
      Agent agent = this.Mission.SpawnAgent(agentBuildData);
      agent.AddComponent((AgentComponent) new AgentAIStateFlagComponent(agent));
      agent.SetWatchState(AgentAIStateFlagComponent.WatchState.Alarmed);
    }

    protected void SpawnBotVisualsInPlayerFormation(
      MissionPeer missionPeer,
      int visualsIndex,
      Team agentTeam,
      BasicCultureObject cultureLimit,
      string troopName,
      Formation formation,
      bool updateExistingAgentVisuals,
      int totalCount,
      IEnumerable<(EquipmentIndex, EquipmentElement)> alternativeEquipments)
    {
      BasicCharacterObject basicCharacterObject = MBObjectManager.Instance.GetObject<BasicCharacterObject>(troopName);
      AgentBuildData agentBuildData = new AgentBuildData(basicCharacterObject);
      agentBuildData.Team(agentTeam);
      agentBuildData.OwningMissionPeer(missionPeer);
      agentBuildData.VisualsIndex(visualsIndex);
      Equipment equipmentElements = Equipment.GetRandomEquipmentElements(basicCharacterObject, !(Game.Current.GameType is MultiplayerGame), seed: MBRandom.RandomInt());
      if (alternativeEquipments != null)
      {
        foreach ((EquipmentIndex, EquipmentElement) alternativeEquipment in alternativeEquipments)
          equipmentElements[alternativeEquipment.Item1] = alternativeEquipment.Item2;
      }
      agentBuildData.Equipment(equipmentElements);
      agentBuildData.SpawnOnInitialPoint(true);
      agentBuildData.Formation(formation);
      agentBuildData.ClothingColor1(agentTeam.Side == BattleSideEnum.Attacker ? cultureLimit.Color : cultureLimit.ClothAlternativeColor);
      agentBuildData.ClothingColor2(agentTeam.Side == BattleSideEnum.Attacker ? cultureLimit.Color2 : cultureLimit.ClothAlternativeColor2);
      agentBuildData.TroopOrigin((IAgentOriginBase) new BasicBattleAgentOrigin(basicCharacterObject));
      agentBuildData.EquipmentSeed(this.MissionLobbyComponent.GetRandomFaceSeedForCharacter(basicCharacterObject, visualsIndex));
      agentBuildData.IsFemale(basicCharacterObject.IsFemale);
      agentBuildData.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData.AgentIsFemale, basicCharacterObject.GetBodyPropertiesMin(), basicCharacterObject.GetBodyPropertiesMax(), (int) agentBuildData.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData.AgentEquipmentSeed, basicCharacterObject.HairTags, basicCharacterObject.BeardTags, basicCharacterObject.TattooTags));
      NetworkCommunicator networkPeer = missionPeer.GetNetworkPeer();
      if (this.GameMode.ShouldSpawnVisualsForServer(networkPeer))
        this.AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(missionPeer, agentBuildData, isBot: true, totalTroopCount: totalCount);
      this.GameMode.HandleAgentVisualSpawning(networkPeer, agentBuildData, totalCount);
    }
  }
}
