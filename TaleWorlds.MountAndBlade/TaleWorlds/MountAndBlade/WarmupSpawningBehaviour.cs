// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.WarmupSpawningBehaviour
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  internal class WarmupSpawningBehaviour : SpawningBehaviourBase
  {
    private Timer _spawnCheckTimer;

    public WarmupSpawningBehaviour()
    {
      this._spawnCheckTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 0.2f);
      this.IsSpawningEnabled = true;
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
        if (peer.GetNetworkPeer().IsSynchronized && peer.ControlledAgent == null && (!peer.HasSpawnedAgentVisuals && peer.Team != null) && (peer.Team != this.Mission.SpectatorTeam && peer.SpawnTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission))))
        {
          IAgentVisual agentVisualForPeer = peer.GetAgentVisualForPeer(0);
          BasicCultureObject basicCultureObject = peer.Team.Side == BattleSideEnum.Attacker ? cultureLimit1 : cultureLimit2;
          int num = peer.SelectedTroopIndex;
          IEnumerable<MultiplayerClassDivisions.MPHeroClass> mpHeroClasses = MultiplayerClassDivisions.GetMPHeroClasses(basicCultureObject);
          MultiplayerClassDivisions.MPHeroClass mpHeroClass = num < 0 ? (MultiplayerClassDivisions.MPHeroClass) null : mpHeroClasses.ElementAt<MultiplayerClassDivisions.MPHeroClass>(num);
          if (mpHeroClass == null && num < 0)
          {
            mpHeroClass = mpHeroClasses.First<MultiplayerClassDivisions.MPHeroClass>();
            num = 0;
          }
          BasicCharacterObject heroCharacter = mpHeroClass.HeroCharacter;
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
          MatrixFrame frame;
          if (agentVisualForPeer == null)
          {
            frame = this.SpawnComponent.GetSpawnFrame(peer.Team, heroCharacter.Equipment.Horse.Item != null);
          }
          else
          {
            frame = agentVisualForPeer.GetFrame();
            frame.rotation.MakeUnit();
          }
          agentBuildData.InitialFrame(frame);
          agentBuildData.IsFemale(peer.Peer.IsFemale);
          BodyProperties bodyProperties = this.GetBodyProperties(peer, basicCultureObject);
          agentBuildData.BodyProperties(bodyProperties);
          agentBuildData.VisualsIndex(0);
          agentBuildData.ClothingColor1(peer.Team == this.Mission.AttackerTeam ? basicCultureObject.Color : basicCultureObject.ClothAlternativeColor);
          agentBuildData.ClothingColor2(peer.Team == this.Mission.AttackerTeam ? basicCultureObject.Color2 : basicCultureObject.ClothAlternativeColor2);
          agentBuildData.TroopOrigin((IAgentOriginBase) new BasicBattleAgentOrigin(heroCharacter));
          NetworkCommunicator networkPeer = peer.GetNetworkPeer();
          if (this.GameMode.ShouldSpawnVisualsForServer(networkPeer))
            this.AgentVisualSpawnComponent.SpawnAgentVisualsForPeer(peer, agentBuildData, num);
          this.GameMode.HandleAgentVisualSpawning(networkPeer, agentBuildData);
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

    public override int GetMaximumReSpawnPeriodForPeer(MissionPeer peer) => 3;

    protected override bool IsRoundInProgress() => Mission.Current.CurrentState == Mission.State.Continuing;

    public override void Clear()
    {
      base.Clear();
      this.RequestStopSpawnSession();
    }
  }
}
