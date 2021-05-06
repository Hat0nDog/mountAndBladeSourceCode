// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerTeamDeathmatch
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.MissionRepresentatives;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MissionMultiplayerTeamDeathmatch : MissionMultiplayerGameModeBase
  {
    public const int MaxScoreToEndMatch = 120000;
    private const int FirstSpawnGold = 120;
    private const int RespawnGold = 100;
    private MissionScoreboardComponent _missionScoreboardComponent;

    public override bool IsGameModeHidingAllAgentVisuals => true;

    public override MissionLobbyComponent.MultiplayerGameType GetMissionType() => MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._missionScoreboardComponent = this.Mission.GetMissionBehaviour<MissionScoreboardComponent>();
    }

    public override void AfterStart()
    {
      string strValue1 = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue();
      string strValue2 = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue();
      BasicCultureObject basicCultureObject1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(strValue1);
      BasicCultureObject basicCultureObject2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(strValue2);
      Banner banner1 = new Banner(basicCultureObject1.BannerKey, basicCultureObject1.BackgroundColor1, basicCultureObject1.ForegroundColor1);
      Banner banner2 = new Banner(basicCultureObject2.BannerKey, basicCultureObject2.BackgroundColor2, basicCultureObject2.ForegroundColor2);
      this.Mission.Teams.Add(BattleSideEnum.Attacker, basicCultureObject1.BackgroundColor1, basicCultureObject1.ForegroundColor1, banner1);
      this.Mission.Teams.Add(BattleSideEnum.Defender, basicCultureObject2.BackgroundColor2, basicCultureObject2.ForegroundColor2, banner2);
    }

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
      networkPeer.AddComponent<TeamDeathmatchMissionRepresentative>();
      this.ChangeCurrentGoldForPeer(networkPeer.GetComponent<MissionPeer>(), 120);
      this.GameModeBaseClient.OnGoldAmountChangedForRepresentative((MissionRepresentativeBase) networkPeer.GetComponent<TeamDeathmatchMissionRepresentative>(), 120);
    }

    protected override void OnEndMission()
    {
      base.OnEndMission();
      if (!GameNetwork.IsServer)
        return;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        VirtualPlayer virtualPlayer = networkPeer.VirtualPlayer;
        TeamDeathmatchMissionRepresentative component = virtualPlayer.GetComponent<TeamDeathmatchMissionRepresentative>();
        if (component != null)
          virtualPlayer.RemoveComponent((PeerComponent) component);
      }
    }

    public override void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
    {
      if (oldTeam == null || oldTeam == newTeam || oldTeam.Side == BattleSideEnum.None)
        return;
      this.ChangeCurrentGoldForPeer(peer.GetComponent<MissionPeer>(), 100);
    }

    public override int GetScoreForKill(Agent killedAgent) => MultiplayerClassDivisions.GetMPHeroClassForCharacter(killedAgent.Character).TroopCost;

    public override int GetScoreForAssist(Agent killedAgent) => (int) ((double) MultiplayerClassDivisions.GetMPHeroClassForCharacter(killedAgent.Character).TroopCost * 0.5);

    public override void OnAgentRemoved(
      Agent affectedAgent,
      Agent affectorAgent,
      AgentState agentState,
      KillingBlow blow)
    {
      if (blow.DamageType == DamageTypes.Invalid || agentState != AgentState.Unconscious && agentState != AgentState.Killed || !affectedAgent.IsHuman)
        return;
      if (affectorAgent != null && affectorAgent.IsEnemyOf(affectedAgent))
        this._missionScoreboardComponent.ChangeTeamScore(affectorAgent.Team, this.GetScoreForKill(affectedAgent));
      else
        this._missionScoreboardComponent.ChangeTeamScore(affectedAgent.Team, -this.GetScoreForKill(affectedAgent));
      MissionPeer missionPeer = affectedAgent.MissionPeer;
      if (missionPeer != null)
      {
        int num1 = 100;
        if (affectorAgent != affectedAgent)
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
          int num2 = missionPeerListArray[1].Count - missionPeerListArray[0].Count;
          BattleSideEnum battleSideEnum = num2 == 0 ? BattleSideEnum.None : (num2 < 0 ? BattleSideEnum.Attacker : BattleSideEnum.Defender);
          if (battleSideEnum != BattleSideEnum.None && battleSideEnum == missionPeer.Team.Side)
          {
            int num3 = Math.Abs(num2);
            int count = missionPeerListArray[(int) battleSideEnum].Count;
            if (count > 0)
            {
              int num4 = num1 * num3 / 10 / count * 10;
              num1 += num4;
            }
          }
        }
        this.ChangeCurrentGoldForPeer(missionPeer, missionPeer.Representative.Gold + num1);
      }
      MultiplayerClassDivisions.MPHeroClass classForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(affectedAgent.Character);
      Agent.Hitter assistingHitter = affectedAgent.GetAssistingHitter(affectorAgent?.MissionPeer);
      if (affectorAgent?.MissionPeer != null && affectorAgent != affectedAgent && !affectorAgent.IsFriendOf(affectedAgent))
      {
        TeamDeathmatchMissionRepresentative representative = affectorAgent.MissionPeer.Representative as TeamDeathmatchMissionRepresentative;
        int dataAndUpdateFlags = representative.GetGoldGainsFromKillDataAndUpdateFlags(MPPerkObject.GetPerkHandler(affectorAgent.MissionPeer), MPPerkObject.GetPerkHandler(assistingHitter?.HitterPeer), classForCharacter, false, blow.IsMissile);
        this.ChangeCurrentGoldForPeer(affectorAgent.MissionPeer, representative.Gold + dataAndUpdateFlags);
      }
      if (assistingHitter?.HitterPeer != null && !assistingHitter.IsFriendlyHit)
      {
        TeamDeathmatchMissionRepresentative representative = assistingHitter.HitterPeer.Representative as TeamDeathmatchMissionRepresentative;
        int dataAndUpdateFlags = representative.GetGoldGainsFromKillDataAndUpdateFlags(MPPerkObject.GetPerkHandler(affectorAgent?.MissionPeer), MPPerkObject.GetPerkHandler(assistingHitter.HitterPeer), classForCharacter, true, blow.IsMissile);
        this.ChangeCurrentGoldForPeer(assistingHitter.HitterPeer, representative.Gold + dataAndUpdateFlags);
      }
      if (missionPeer?.Team == null)
        return;
      IEnumerable<(MissionPeer, int)> goldRewardsOnDeath = MPPerkObject.GetPerkHandler(missionPeer)?.GetTeamGoldRewardsOnDeath();
      if (goldRewardsOnDeath == null)
        return;
      foreach ((MissionPeer peer, int baseAmount) in goldRewardsOnDeath)
      {
        if (peer?.Representative is TeamDeathmatchMissionRepresentative representative1)
        {
          int local_21 = representative1.GetGoldGainsFromAllyDeathReward(baseAmount);
          if (local_21 > 0)
            this.ChangeCurrentGoldForPeer(peer, representative1.Gold + local_21);
        }
      }
    }

    public override bool CheckForMatchEnd()
    {
      int minScoreToWinMatch = MultiplayerOptions.OptionType.MinScoreToWinMatch.GetIntValue();
      return ((IEnumerable<MissionScoreboardComponent.MissionScoreboardSide>) this._missionScoreboardComponent.Sides).Any<MissionScoreboardComponent.MissionScoreboardSide>((Func<MissionScoreboardComponent.MissionScoreboardSide, bool>) (side => side.SideScore >= minScoreToWinMatch));
    }

    public override Team GetWinnerTeam()
    {
      int intValue = MultiplayerOptions.OptionType.MinScoreToWinMatch.GetIntValue();
      Team team = (Team) null;
      MissionScoreboardComponent.MissionScoreboardSide[] sides = this._missionScoreboardComponent.Sides;
      if (sides[1].SideScore < intValue && sides[0].SideScore >= intValue)
        team = this.Mission.Teams.Defender;
      if (sides[0].SideScore < intValue && sides[1].SideScore >= intValue)
        team = this.Mission.Teams.Attacker;
      return team;
    }
  }
}
