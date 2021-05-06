// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeSpawningBehaviour
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  internal class SiegeSpawningBehaviour : SpawningBehaviourBase
  {
    private Timer _spawnCheckTimer;

    public SiegeSpawningBehaviour() => this._spawnCheckTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.2f);

    public override void Initialize(SpawnComponent spawnComponent)
    {
      base.Initialize(spawnComponent);
      this.OnAllAgentsFromPeerSpawnedFromVisuals += new Action<MissionPeer>(this.OnAllAgentsFromPeerSpawnedFromVisuals);
    }

    public override void Clear()
    {
      base.Clear();
      this.OnAllAgentsFromPeerSpawnedFromVisuals -= new Action<MissionPeer>(this.OnAllAgentsFromPeerSpawnedFromVisuals);
    }

    public override void OnTick(float dt)
    {
      if (this.IsSpawningEnabled && this._spawnCheckTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)))
        this.SpawnAgents();
      base.OnTick(dt);
    }

    protected override void SpawnAgents()
    {
      BasicCultureObject cultureLimit1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
      BasicCultureObject cultureLimit2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());
      foreach (MissionPeer peer in VirtualPlayer.Peers<MissionPeer>())
      {
        NetworkCommunicator networkPeer = peer.GetNetworkPeer();
        if (networkPeer.IsSynchronized && peer.ControlledAgent == null && (!peer.HasSpawnedAgentVisuals && peer.Team != null) && (peer.Team != this.Mission.SpectatorTeam && peer.SpawnTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission))))
        {
          BasicCultureObject basicCultureObject = peer.Team.Side == BattleSideEnum.Attacker ? cultureLimit1 : cultureLimit2;
          MultiplayerClassDivisions.MPHeroClass heroClassForPeer = MultiplayerClassDivisions.GetMPHeroClassForPeer(peer);
          if (heroClassForPeer == null || heroClassForPeer.TroopCost > this.GameMode.GetCurrentGoldForPeer(peer))
          {
            if (peer.SelectedTroopIndex != 0)
            {
              peer.SelectedTroopIndex = 0;
              GameNetwork.BeginBroadcastModuleEvent();
              GameNetwork.WriteMessage((GameNetworkMessage) new UpdateSelectedTroopIndex(networkPeer, 0));
              GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, networkPeer);
            }
          }
          else
          {
            BasicCharacterObject heroCharacter = heroClassForPeer.HeroCharacter;
            Equipment equipment = heroCharacter.Equipment.Clone();
            IEnumerable<(EquipmentIndex, EquipmentElement)> alternativeEquipments = MPPerkObject.GetOnSpawnPerkHandler(peer)?.GetAlternativeEquipments(true);
            if (alternativeEquipments != null)
            {
              foreach ((EquipmentIndex, EquipmentElement) tuple in alternativeEquipments)
                equipment[tuple.Item1] = tuple.Item2;
            }
            AgentBuildData agentBuildData = new AgentBuildData(heroCharacter);
            agentBuildData.MissionPeer(peer);
            agentBuildData.Equipment(equipment);
            agentBuildData.Team(peer.Team);
            agentBuildData.IsFemale(peer.Peer.IsFemale);
            agentBuildData.BodyProperties(this.GetBodyProperties(peer, peer.Team == this.Mission.AttackerTeam ? cultureLimit1 : cultureLimit2));
            agentBuildData.VisualsIndex(0);
            agentBuildData.ClothingColor1(peer.Team == this.Mission.AttackerTeam ? basicCultureObject.Color : basicCultureObject.ClothAlternativeColor);
            agentBuildData.ClothingColor2(peer.Team == this.Mission.AttackerTeam ? basicCultureObject.Color2 : basicCultureObject.ClothAlternativeColor2);
            agentBuildData.TroopOrigin((IAgentOriginBase) new BasicBattleAgentOrigin(heroCharacter));
            if (this.GameMode.ShouldSpawnVisualsForServer(networkPeer))
              this.AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(peer, agentBuildData, peer.SelectedTroopIndex);
            this.GameMode.HandleAgentVisualSpawning(networkPeer, agentBuildData);
          }
        }
      }
      if (this.Mission.AttackerTeam != null)
      {
        int num = 0;
        foreach (Agent activeAgent in this.Mission.AttackerTeam.ActiveAgents)
        {
          if (activeAgent.Character != null && activeAgent.MissionPeer == null)
            ++num;
        }
        if (num < MultiplayerOptions.OptionType.NumberOfBotsTeam1.GetIntValue())
          this.SpawnBot(this.Mission.AttackerTeam, cultureLimit1);
      }
      if (this.Mission.DefenderTeam == null)
        return;
      int num1 = 0;
      foreach (Agent activeAgent in this.Mission.DefenderTeam.ActiveAgents)
      {
        if (activeAgent.Character != null && activeAgent.MissionPeer == null)
          ++num1;
      }
      if (num1 >= MultiplayerOptions.OptionType.NumberOfBotsTeam2.GetIntValue())
        return;
      this.SpawnBot(this.Mission.DefenderTeam, cultureLimit2);
    }

    public override bool AllowEarlyAgentVisualsDespawning(MissionPeer lobbyPeer) => true;

    public override int GetMaximumReSpawnPeriodForPeer(MissionPeer peer)
    {
      if (this.GameMode.WarmupComponent != null && this.GameMode.WarmupComponent.IsInWarmup)
        return 3;
      if (peer.Team != null)
      {
        if (peer.Team.Side == BattleSideEnum.Attacker)
          return MultiplayerOptions.OptionType.RespawnPeriodTeam1.GetIntValue();
        if (peer.Team.Side == BattleSideEnum.Defender)
          return MultiplayerOptions.OptionType.RespawnPeriodTeam2.GetIntValue();
      }
      return -1;
    }

    protected override bool IsRoundInProgress() => Mission.Current.CurrentState == Mission.State.Continuing;

    private void OnAllAgentsFromPeerSpawnedFromVisuals(MissionPeer peer)
    {
      bool flag = peer.Team == this.Mission.AttackerTeam;
      Team defenderTeam = this.Mission.DefenderTeam;
      MultiplayerClassDivisions.MPHeroClass mpHeroClass = MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(flag ? MultiplayerOptions.OptionType.CultureTeam1.GetStrValue() : MultiplayerOptions.OptionType.CultureTeam2.GetStrValue())).ElementAt<MultiplayerClassDivisions.MPHeroClass>(peer.SelectedTroopIndex);
      this.GameMode.ChangeCurrentGoldForPeer(peer, this.GameMode.GetCurrentGoldForPeer(peer) - mpHeroClass.TroopCost);
    }
  }
}
