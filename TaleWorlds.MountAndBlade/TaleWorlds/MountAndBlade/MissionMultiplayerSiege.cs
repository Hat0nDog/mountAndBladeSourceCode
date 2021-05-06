// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerSiege
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

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
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MissionMultiplayerSiege : 
    MissionMultiplayerGameModeBase,
    IAnalyticsFlagInfo,
    IMissionBehavior
  {
    public const int NumberOfFlagsInGame = 7;
    public const int NumberOfFlagsAffectingMoraleInGame = 6;
    public const int MaxMorale = 1440;
    public const int StartingMorale = 360;
    private const int FirstSpawnGold = 120;
    private const int FirstSpawnGoldForEarlyJoin = 160;
    private const int RespawnGold = 100;
    private const float ObjectiveCheckPeriod = 0.25f;
    private const float MoraleTickTimeInSeconds = 1f;
    public const int MaxMoraleGainPerFlag = 90;
    private const int MoraleBoostOnFlagRemoval = 90;
    private const int MoraleDecayInTick = -1;
    private const int MoraleDecayOnDefenderInTick = -6;
    public const int MoraleGainPerFlag = 1;
    public const int GoldBonusOnFlagRemoval = 35;
    public const string MasterFlagTag = "keep_capture_point";
    private int[] _morales;
    private Agent _masterFlagBestAgent;
    private FlagCapturePoint _masterFlag;
    private Team[] _capturePointOwners;
    private int[] _capturePointRemainingMoraleGains;
    private float _dtSumCheckMorales;
    private float _dtSumObjectiveCheck;
    private MissionMultiplayerSiege.ObjectiveSystem _objectiveSystem;
    private (IMoveableSiegeWeapon, Vec3)[] _movingObjectives;
    private (RangedSiegeWeapon, Agent)[] _lastReloadingAgentPerRangedSiegeMachine;
    private MissionMultiplayerSiegeClient _gameModeSiegeClient;
    private MultiplayerWarmupComponent _warmupComponent;
    private Dictionary<GameEntity, List<DestructableComponent>> _childDestructableComponents;
    private bool _firstTickDone;

    public override bool IsGameModeHidingAllAgentVisuals => true;

    public MBReadOnlyList<FlagCapturePoint> AllCapturePoints { get; private set; }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._objectiveSystem = new MissionMultiplayerSiege.ObjectiveSystem();
      this._childDestructableComponents = new Dictionary<GameEntity, List<DestructableComponent>>();
      this._gameModeSiegeClient = Mission.Current.GetMissionBehaviour<MissionMultiplayerSiegeClient>();
      this._warmupComponent = Mission.Current.GetMissionBehaviour<MultiplayerWarmupComponent>();
      this._capturePointOwners = new Team[7];
      this._capturePointRemainingMoraleGains = new int[7];
      this._morales = new int[2];
      this._morales[1] = 360;
      this._morales[0] = 360;
      this.AllCapturePoints = new MBReadOnlyList<FlagCapturePoint>(Mission.Current.MissionObjects.FindAllWithType<FlagCapturePoint>().ToListQ<FlagCapturePoint>());
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        allCapturePoint.SetTeamColorsSynched(4284111450U, uint.MaxValue);
        this._capturePointOwners[allCapturePoint.FlagIndex] = (Team) null;
        this._capturePointRemainingMoraleGains[allCapturePoint.FlagIndex] = 90;
        if (allCapturePoint.GameEntity.HasTag("keep_capture_point"))
          this._masterFlag = allCapturePoint;
      }
      foreach (DestructableComponent destructableComponent in Mission.Current.MissionObjects.FindAllWithType<DestructableComponent>())
      {
        if (destructableComponent.BattleSide != BattleSideEnum.None)
        {
          GameEntity mostParent = destructableComponent.GameEntity.GetMostParent();
          if (this._objectiveSystem.RegisterObjective(mostParent))
          {
            this._childDestructableComponents.Add(mostParent, new List<DestructableComponent>());
            MissionMultiplayerSiege.GetDestructableCompoenentClosestToTheRoot(mostParent).OnDestroyed += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnDestroyed);
          }
          this._childDestructableComponents[mostParent].Add(destructableComponent);
          destructableComponent.OnHitTaken += new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnHitTaken);
        }
      }
      List<RangedSiegeWeapon> rangedSiegeWeaponList = new List<RangedSiegeWeapon>();
      List<IMoveableSiegeWeapon> moveableSiegeWeaponList = new List<IMoveableSiegeWeapon>();
      foreach (UsableMachine usableMachine in Mission.Current.MissionObjects.FindAllWithType<UsableMachine>())
      {
        if (usableMachine is RangedSiegeWeapon rangedSiegeWeapon1)
        {
          rangedSiegeWeaponList.Add(rangedSiegeWeapon1);
          rangedSiegeWeapon1.OnAgentLoadsMachine += new Action<RangedSiegeWeapon, Agent>(this.RangedSiegeMachineOnAgentLoadsMachine);
        }
        else if (usableMachine is IMoveableSiegeWeapon moveableSiegeWeapon)
        {
          moveableSiegeWeaponList.Add(moveableSiegeWeapon);
          this._objectiveSystem.RegisterObjective(usableMachine.GameEntity.GetMostParent());
        }
      }
      this._lastReloadingAgentPerRangedSiegeMachine = new (RangedSiegeWeapon, Agent)[rangedSiegeWeaponList.Count];
      for (int index = 0; index < this._lastReloadingAgentPerRangedSiegeMachine.Length; ++index)
        this._lastReloadingAgentPerRangedSiegeMachine[index] = ValueTuple.Create<RangedSiegeWeapon, Agent>(rangedSiegeWeaponList[index], (Agent) null);
      this._movingObjectives = new (IMoveableSiegeWeapon, Vec3)[moveableSiegeWeaponList.Count];
      for (int index = 0; index < this._movingObjectives.Length; ++index)
      {
        SiegeWeapon siegeWeapon = moveableSiegeWeaponList[index] as SiegeWeapon;
        this._movingObjectives[index] = ValueTuple.Create<IMoveableSiegeWeapon, Vec3>(moveableSiegeWeaponList[index], siegeWeapon.GameEntity.GlobalPosition);
      }
    }

    private static DestructableComponent GetDestructableCompoenentClosestToTheRoot(
      GameEntity entity)
    {
      DestructableComponent destructableComponent = entity.GetFirstScriptOfType<DestructableComponent>();
      while (destructableComponent == null && entity.ChildCount != 0)
      {
        for (int index = 0; index < entity.ChildCount; ++index)
        {
          destructableComponent = MissionMultiplayerSiege.GetDestructableCompoenentClosestToTheRoot(entity.GetChild(index));
          if (destructableComponent != null)
            break;
        }
      }
      return destructableComponent;
    }

    private void RangedSiegeMachineOnAgentLoadsMachine(
      RangedSiegeWeapon siegeWeapon,
      Agent reloadingAgent)
    {
      for (int index = 0; index < this._lastReloadingAgentPerRangedSiegeMachine.Length; ++index)
      {
        if (this._lastReloadingAgentPerRangedSiegeMachine[index].Item1 == siegeWeapon)
          this._lastReloadingAgentPerRangedSiegeMachine[index].Item2 = reloadingAgent;
      }
    }

    private void DestructableComponentOnHitTaken(
      DestructableComponent destructableComponent,
      Agent attackerAgent,
      in MissionWeapon weapon,
      ScriptComponentBehaviour attackerScriptComponentBehaviour,
      int inflictedDamage)
    {
      if (this.WarmupComponent.IsInWarmup)
        return;
      GameEntity mostParent = destructableComponent.GameEntity.GetMostParent();
      if (attackerScriptComponentBehaviour is BatteringRam batteringRam)
      {
        float contribution = (float) inflictedDamage / (float) batteringRam.UserCount;
        foreach (Agent user in batteringRam.Users)
        {
          if (user.MissionPeer != null && user.MissionPeer.Team.Side == destructableComponent.BattleSide.GetOppositeSide())
            this._objectiveSystem.AddContributionForObjective(mostParent, user.MissionPeer, contribution);
        }
      }
      else if (attackerAgent?.MissionPeer?.Team != null && attackerAgent.MissionPeer.Team.Side == destructableComponent.BattleSide.GetOppositeSide())
      {
        if (attackerAgent.CurrentlyUsedGameObject != null && attackerAgent.CurrentlyUsedGameObject is StandingPoint currentlyUsedGameObject3)
        {
          RangedSiegeWeapon scriptOfTypeInFamily = currentlyUsedGameObject3.GameEntity.GetFirstScriptOfTypeInFamily<RangedSiegeWeapon>();
          if (scriptOfTypeInFamily != null)
          {
            for (int index = 0; index < this._lastReloadingAgentPerRangedSiegeMachine.Length; ++index)
            {
              if (this._lastReloadingAgentPerRangedSiegeMachine[index].Item1 == scriptOfTypeInFamily && this._lastReloadingAgentPerRangedSiegeMachine[index].Item2?.MissionPeer != null)
              {
                BattleSideEnum? side = this._lastReloadingAgentPerRangedSiegeMachine[index].Item2?.MissionPeer.Team.Side;
                BattleSideEnum oppositeSide = destructableComponent.BattleSide.GetOppositeSide();
                if (side.GetValueOrDefault() == oppositeSide & side.HasValue)
                  this._objectiveSystem.AddContributionForObjective(mostParent, this._lastReloadingAgentPerRangedSiegeMachine[index].Item2.MissionPeer, (float) inflictedDamage * 0.33f);
              }
            }
          }
        }
        this._objectiveSystem.AddContributionForObjective(mostParent, attackerAgent.MissionPeer, (float) inflictedDamage);
      }
      if (!destructableComponent.IsDestroyed)
        return;
      destructableComponent.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnHitTaken);
      this._childDestructableComponents[mostParent].Remove(destructableComponent);
    }

    private void DestructableComponentOnDestroyed(
      DestructableComponent destructableComponent,
      Agent attackerAgent,
      in MissionWeapon weapon,
      ScriptComponentBehaviour attackerScriptComponentBehaviour,
      int inflictedDamage)
    {
      GameEntity mostParent = destructableComponent.GameEntity.GetMostParent();
      List<KeyValuePair<MissionPeer, float>> contributorsForSideAndClear = this._objectiveSystem.GetAllContributorsForSideAndClear(mostParent, destructableComponent.BattleSide.GetOppositeSide());
      float num = contributorsForSideAndClear.Sum<KeyValuePair<MissionPeer, float>>((Func<KeyValuePair<MissionPeer, float>, float>) (ac => ac.Value));
      foreach (KeyValuePair<MissionPeer, float> keyValuePair in contributorsForSideAndClear)
      {
        int fromObjectiveAssist = (keyValuePair.Key.Representative as SiegeMissionRepresentative).GetGoldGainsFromObjectiveAssist(mostParent, keyValuePair.Value / num, false);
        if (fromObjectiveAssist > 0)
          this.ChangeCurrentGoldForPeer(keyValuePair.Key, keyValuePair.Key.Representative.Gold + fromObjectiveAssist);
      }
      destructableComponent.OnDestroyed -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnDestroyed);
      foreach (DestructableComponent destructableComponent1 in this._childDestructableComponents[mostParent])
        destructableComponent1.OnHitTaken -= new DestructableComponent.OnHitTakenAndDestroyedDelegate(this.DestructableComponentOnHitTaken);
      this._childDestructableComponents.Remove(mostParent);
    }

    public override MissionLobbyComponent.MultiplayerGameType GetMissionType() => MissionLobbyComponent.MultiplayerGameType.Siege;

    public override bool UseRoundController() => false;

    public override void AfterStart()
    {
      BasicCultureObject basicCultureObject1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue());
      BasicCultureObject basicCultureObject2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue());
      Banner banner1 = new Banner(basicCultureObject1.BannerKey, basicCultureObject1.BackgroundColor1, basicCultureObject1.ForegroundColor1);
      Banner banner2 = new Banner(basicCultureObject2.BannerKey, basicCultureObject2.BackgroundColor2, basicCultureObject2.ForegroundColor2);
      this.Mission.Teams.Add(BattleSideEnum.Attacker, basicCultureObject1.BackgroundColor1, basicCultureObject1.ForegroundColor1, banner1);
      this.Mission.Teams.Add(BattleSideEnum.Defender, basicCultureObject2.BackgroundColor2, basicCultureObject2.ForegroundColor2, banner2);
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        this._capturePointOwners[allCapturePoint.FlagIndex] = this.Mission.Teams.Defender;
        allCapturePoint.SetTeamColors(this.Mission.Teams.Defender.Color, this.Mission.Teams.Defender.Color2);
        this._gameModeSiegeClient?.OnCapturePointOwnerChanged(allCapturePoint, this.Mission.Teams.Defender);
      }
      this._warmupComponent.OnWarmupEnding += new Action(this.OnWarmupEnding);
    }

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if (!this._firstTickDone)
      {
        foreach (CastleGate castleGate in Mission.Current.MissionObjects.FindAllWithType<CastleGate>())
        {
          castleGate.OpenDoor();
          foreach (UsableMissionObject standingPoint in castleGate.StandingPoints)
            standingPoint.SetIsDeactivatedSynched(true);
        }
        this._firstTickDone = true;
      }
      if (this.MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Playing || this.WarmupComponent.IsInWarmup)
        return;
      this.CheckMorales(dt);
      if (!this.CheckObjectives(dt))
        return;
      this.TickFlags(dt);
      this.TickObjectives(dt);
    }

    private void CheckMorales(float dt)
    {
      this._dtSumCheckMorales += dt;
      if ((double) this._dtSumCheckMorales < 1.0)
        return;
      --this._dtSumCheckMorales;
      int attackerMorale = Math.Max(this._morales[1] + this.GetMoraleGain(BattleSideEnum.Attacker), 0);
      int defenderMorale = MBMath.ClampInt(this._morales[0] + this.GetMoraleGain(BattleSideEnum.Defender), 0, 360);
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new SiegeMoraleChangeMessage(attackerMorale, defenderMorale, this._capturePointRemainingMoraleGains));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      this._gameModeSiegeClient?.OnMoraleChanged(attackerMorale, defenderMorale, this._capturePointRemainingMoraleGains);
      this._morales[1] = attackerMorale;
      this._morales[0] = defenderMorale;
    }

    public override bool CheckForMatchEnd() => ((IEnumerable<int>) this._morales).Any<int>((Func<int, bool>) (morale => morale == 0));

    public override Team GetWinnerTeam()
    {
      Team team1 = (Team) null;
      if (this._morales[1] <= 0 && this._morales[0] > 0)
        team1 = this.Mission.Teams.Defender;
      if (this._morales[0] <= 0 && this._morales[1] > 0)
        team1 = this.Mission.Teams.Attacker;
      Team team2 = team1 ?? this.Mission.Teams.Defender;
      this.Mission.GetMissionBehaviour<MissionScoreboardComponent>().ChangeTeamScore(team2, 1);
      return team2;
    }

    private int GetMoraleGain(BattleSideEnum side)
    {
      int num1 = 0;
      bool flag1 = this._masterFlagBestAgent != null && this._masterFlagBestAgent.Team.Side == side;
      if (side == BattleSideEnum.Attacker)
      {
        if (!flag1)
          num1 += -1;
        foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (flag => flag != this._masterFlag && !flag.IsDeactivated && flag.IsFullyRaised && this.GetFlagOwnerTeam(flag).Side == BattleSideEnum.Attacker)))
        {
          --this._capturePointRemainingMoraleGains[flagCapturePoint.FlagIndex];
          ++num1;
          if (this._capturePointRemainingMoraleGains[flagCapturePoint.FlagIndex] == 0)
          {
            num1 += 90;
            foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
            {
              MissionPeer component = networkPeer.GetComponent<MissionPeer>();
              if (component != null)
              {
                BattleSideEnum? side1 = component.Team?.Side;
                BattleSideEnum battleSideEnum = side;
                if (side1.GetValueOrDefault() == battleSideEnum & side1.HasValue)
                  this.ChangeCurrentGoldForPeer(component, this.GetCurrentGoldForPeer(component) + 35);
              }
            }
            flagCapturePoint.RemovePointAsServer();
            (this.SpawnComponent.SpawnFrameBehaviour as SiegeSpawnFrameBehaviour).OnFlagDeactivated(flagCapturePoint);
            this._gameModeSiegeClient.OnNumberOfFlagsChanged();
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationFlagsRemovedMessage());
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
            this.NotificationsComponent.FlagsXRemoved(flagCapturePoint);
          }
        }
      }
      else if (this._masterFlag.IsFullyRaised)
      {
        if (this.GetFlagOwnerTeam(this._masterFlag).Side == BattleSideEnum.Attacker)
        {
          if (!flag1)
          {
            int num2 = 0;
            for (int index = 0; index < this.AllCapturePoints.Count; ++index)
            {
              if (this.AllCapturePoints[index] != this._masterFlag && !this.AllCapturePoints[index].IsDeactivated)
                ++num2;
            }
            num1 += num2 - 6;
          }
        }
        else
          ++num1;
      }
      return num1;
    }

    public Team GetFlagOwnerTeam(FlagCapturePoint flag) => this._capturePointOwners[flag.FlagIndex];

    private bool CheckObjectives(float dt)
    {
      this._dtSumObjectiveCheck += dt;
      if ((double) this._dtSumObjectiveCheck < 0.25)
        return false;
      this._dtSumObjectiveCheck -= 0.25f;
      return true;
    }

    private void TickFlags(float dt)
    {
      foreach (FlagCapturePoint allCapturePoint in this.AllCapturePoints)
      {
        if (!allCapturePoint.IsDeactivated)
        {
          Team flagOwnerTeam = this.GetFlagOwnerTeam(allCapturePoint);
          Agent agent1 = (Agent) null;
          float num1 = float.MaxValue;
          foreach (Agent agent2 in Mission.Current.GetAgentsInRange(allCapturePoint.Position.AsVec2, 4f).Where<Agent>((Func<Agent, bool>) (a => !a.IsMount && a.IsActive())))
          {
            float num2 = agent2.Position.DistanceSquared(allCapturePoint.Position);
            if ((double) num2 <= 16.0 && (double) num2 < (double) num1)
            {
              agent1 = agent2;
              num1 = num2;
            }
          }
          if (allCapturePoint == this._masterFlag)
            this._masterFlagBestAgent = agent1;
          CaptureTheFlagFlagDirection directionTo = CaptureTheFlagFlagDirection.None;
          bool isContested = allCapturePoint.IsContested;
          if (flagOwnerTeam == null)
          {
            if (!isContested && agent1 != null)
              directionTo = CaptureTheFlagFlagDirection.Down;
            else if (agent1 == null & isContested)
              directionTo = CaptureTheFlagFlagDirection.Up;
          }
          else if (agent1 != null)
          {
            if (agent1.Team != flagOwnerTeam && !isContested)
              directionTo = CaptureTheFlagFlagDirection.Down;
            else if (agent1.Team == flagOwnerTeam & isContested)
              directionTo = CaptureTheFlagFlagDirection.Up;
          }
          else if (isContested)
            directionTo = CaptureTheFlagFlagDirection.Up;
          if (directionTo != CaptureTheFlagFlagDirection.None)
            allCapturePoint.SetMoveFlag(directionTo);
          bool ownerTeamChanged;
          allCapturePoint.OnAfterTick(agent1 != null, out ownerTeamChanged);
          if (ownerTeamChanged)
          {
            Team team = agent1.Team;
            uint color = team != null ? team.Color : 4284111450U;
            uint color2 = team != null ? team.Color2 : uint.MaxValue;
            allCapturePoint.SetTeamColorsSynched(color, color2);
            this._capturePointOwners[allCapturePoint.FlagIndex] = team;
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationCapturePointMessage(allCapturePoint.FlagIndex, team));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
            this._gameModeSiegeClient?.OnCapturePointOwnerChanged(allCapturePoint, team);
            this.NotificationsComponent.FlagXCapturedByTeamX((SynchedMissionObject) allCapturePoint, agent1.Team);
          }
        }
      }
    }

    private void TickObjectives(float dt)
    {
      for (int index = this._movingObjectives.Length - 1; index >= 0; --index)
      {
        IMoveableSiegeWeapon moveableSiegeWeapon = this._movingObjectives[index].Item1;
        if (moveableSiegeWeapon != null)
        {
          SiegeWeapon siegeWeapon = moveableSiegeWeapon as SiegeWeapon;
          if (siegeWeapon.IsDeactivated || siegeWeapon.IsDestroyed || siegeWeapon.IsDisabled)
            this._movingObjectives[index].Item1 = (IMoveableSiegeWeapon) null;
          else if (moveableSiegeWeapon.MovementComponent.HasArrivedAtTarget)
          {
            this._movingObjectives[index].Item1 = (IMoveableSiegeWeapon) null;
            GameEntity mostParent = siegeWeapon.GameEntity.GetMostParent();
            List<KeyValuePair<MissionPeer, float>> contributorsForSideAndClear = this._objectiveSystem.GetAllContributorsForSideAndClear(mostParent, BattleSideEnum.Attacker);
            float num = contributorsForSideAndClear.Sum<KeyValuePair<MissionPeer, float>>((Func<KeyValuePair<MissionPeer, float>, float>) (ac => ac.Value));
            foreach (KeyValuePair<MissionPeer, float> keyValuePair in contributorsForSideAndClear)
            {
              int fromObjectiveAssist = (keyValuePair.Key.Representative as SiegeMissionRepresentative).GetGoldGainsFromObjectiveAssist(mostParent, keyValuePair.Value / num, true);
              if (fromObjectiveAssist > 0)
                this.ChangeCurrentGoldForPeer(keyValuePair.Key, keyValuePair.Key.Representative.Gold + fromObjectiveAssist);
            }
          }
          else
          {
            GameEntity gameEntity = siegeWeapon.GameEntity;
            Vec3 vec3 = this._movingObjectives[index].Item2;
            Vec3 globalPosition = gameEntity.GlobalPosition;
            float lengthSquared = (globalPosition - vec3).LengthSquared;
            if ((double) lengthSquared > 1.0)
            {
              this._movingObjectives[index].Item2 = globalPosition;
              foreach (Agent user in siegeWeapon.Users)
              {
                if (user.MissionPeer != null && user.MissionPeer.Team.Side == siegeWeapon.Side)
                  this._objectiveSystem.AddContributionForObjective(gameEntity.GetMostParent(), user.MissionPeer, lengthSquared);
              }
            }
          }
        }
      }
    }

    private void OnWarmupEnding() => this.NotificationsComponent.WarmupEnding();

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

    protected override void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
      networkPeer.AddComponent<SiegeMissionRepresentative>();
      int num = 120;
      if (this._warmupComponent != null && this._warmupComponent.IsInWarmup)
        num = 160;
      this.ChangeCurrentGoldForPeer(networkPeer.GetComponent<MissionPeer>(), num);
      this._gameModeSiegeClient?.OnGoldAmountChangedForRepresentative((MissionRepresentativeBase) networkPeer.GetComponent<SiegeMissionRepresentative>(), num);
      if (this.AllCapturePoints == null || networkPeer.IsServerPeer)
        return;
      foreach (FlagCapturePoint flagCapturePoint in this.AllCapturePoints.Where<FlagCapturePoint>((Func<FlagCapturePoint, bool>) (cp => !cp.IsDeactivated)))
      {
        GameNetwork.BeginModuleEventAsServer(networkPeer);
        GameNetwork.WriteMessage((GameNetworkMessage) new FlagDominationCapturePointMessage(flagCapturePoint.FlagIndex, this._capturePointOwners[flagCapturePoint.FlagIndex]));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    protected override void OnEndMission()
    {
      base.OnEndMission();
      if (!GameNetwork.IsServer)
        return;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        VirtualPlayer virtualPlayer = networkPeer.VirtualPlayer;
        SiegeMissionRepresentative component = virtualPlayer.GetComponent<SiegeMissionRepresentative>();
        if (component != null)
          virtualPlayer.RemoveComponent((PeerComponent) component);
      }
    }

    public override void OnPeerChangedTeam(NetworkCommunicator peer, Team oldTeam, Team newTeam)
    {
      if (this.MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Playing || oldTeam == null || oldTeam == newTeam)
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
      if (this.MissionLobbyComponent.CurrentMultiplayerState != MissionLobbyComponent.MultiplayerGameState.Playing || blow.DamageType == DamageTypes.Invalid || agentState != AgentState.Unconscious && agentState != AgentState.Killed || !affectedAgent.IsHuman)
        return;
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
      if (affectorAgent?.MissionPeer != null && affectorAgent != affectedAgent && affectedAgent.Team != affectorAgent.Team)
      {
        SiegeMissionRepresentative representative = affectorAgent.MissionPeer.Representative as SiegeMissionRepresentative;
        int dataAndUpdateFlags = representative.GetGoldGainsFromKillDataAndUpdateFlags(MPPerkObject.GetPerkHandler(affectorAgent.MissionPeer), MPPerkObject.GetPerkHandler(assistingHitter?.HitterPeer), classForCharacter, false, blow.IsMissile);
        this.ChangeCurrentGoldForPeer(affectorAgent.MissionPeer, representative.Gold + dataAndUpdateFlags);
      }
      if (assistingHitter?.HitterPeer != null && !assistingHitter.IsFriendlyHit)
      {
        SiegeMissionRepresentative representative = assistingHitter.HitterPeer.Representative as SiegeMissionRepresentative;
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
        if (baseAmount > 0 && peer?.Representative is SiegeMissionRepresentative representative1)
        {
          int local_21 = representative1.GetGoldGainsFromAllyDeathReward(baseAmount);
          if (local_21 > 0)
            this.ChangeCurrentGoldForPeer(peer, representative1.Gold + local_21);
        }
      }
    }

    protected override void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new SiegeMoraleChangeMessage(this._morales[1], this._morales[0], this._capturePointRemainingMoraleGains));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    public override void OnRemoveBehaviour()
    {
      base.OnRemoveBehaviour();
      this._warmupComponent.OnWarmupEnding -= new Action(this.OnWarmupEnding);
    }

    public override void OnClearScene()
    {
      base.OnClearScene();
      this.ClearPeerCounts();
      foreach (UsableMachine usableMachine in Mission.Current.MissionObjects.FindAllWithType<CastleGate>())
      {
        foreach (UsableMissionObject standingPoint in usableMachine.StandingPoints)
          standingPoint.SetIsDeactivatedSynched(false);
      }
    }

    private class ObjectiveSystem
    {
      private readonly Dictionary<GameEntity, List<MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor>[]> _objectiveContributorMap;

      public ObjectiveSystem() => this._objectiveContributorMap = new Dictionary<GameEntity, List<MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor>[]>();

      public bool RegisterObjective(GameEntity entity)
      {
        if (this._objectiveContributorMap.ContainsKey(entity))
          return false;
        this._objectiveContributorMap.Add(entity, new List<MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor>[2]);
        for (int index = 0; index < 2; ++index)
          this._objectiveContributorMap[entity][index] = new List<MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor>();
        return true;
      }

      public void AddContributionForObjective(
        GameEntity objectiveEntity,
        MissionPeer contributorPeer,
        float contribution)
      {
        string str = ((IEnumerable<string>) objectiveEntity.Tags).FirstOrDefault<string>((Func<string, bool>) (x => x.StartsWith("mp_siege_objective_"))) ?? "";
        bool flag = false;
        for (int index = 0; index < 2; ++index)
        {
          foreach (MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor objectiveContributor in this._objectiveContributorMap[objectiveEntity][index])
          {
            if (objectiveContributor.Peer == contributorPeer)
            {
              Debug.Print(string.Format("[CONT > {0}] Increased contribution for {1}({2}) by {3}.", (object) str, (object) contributorPeer.Name, (object) contributorPeer.Team.Side.ToString(), (object) contribution), debugFilter: 17179869184UL);
              objectiveContributor.IncreaseAmount(contribution);
              flag = true;
              break;
            }
          }
          if (flag)
            break;
        }
        if (flag)
          return;
        Debug.Print(string.Format("[CONT > {0}] Adding {1} contribution for {2}({3}).", (object) str, (object) contribution, (object) contributorPeer.Name, (object) contributorPeer.Team.Side.ToString()), debugFilter: 17179869184UL);
        this._objectiveContributorMap[objectiveEntity][(int) contributorPeer.Team.Side].Add(new MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor(contributorPeer, contribution));
      }

      public List<KeyValuePair<MissionPeer, float>> GetAllContributorsForSideAndClear(
        GameEntity objectiveEntity,
        BattleSideEnum side)
      {
        List<KeyValuePair<MissionPeer, float>> keyValuePairList = new List<KeyValuePair<MissionPeer, float>>();
        string str = ((IEnumerable<string>) objectiveEntity.Tags).FirstOrDefault<string>((Func<string, bool>) (x => x.StartsWith("mp_siege_objective_"))) ?? "";
        foreach (MissionMultiplayerSiege.ObjectiveSystem.ObjectiveContributor objectiveContributor in this._objectiveContributorMap[objectiveEntity][(int) side])
        {
          Debug.Print(string.Format("[CONT > {0}] Rewarding {1} contribution for {2}({3}).", (object) str, (object) objectiveContributor.Contribution, (object) objectiveContributor.Peer.Name, (object) side.ToString()), debugFilter: 17179869184UL);
          keyValuePairList.Add(new KeyValuePair<MissionPeer, float>(objectiveContributor.Peer, objectiveContributor.Contribution));
        }
        this._objectiveContributorMap[objectiveEntity][(int) side].Clear();
        return keyValuePairList;
      }

      private class ObjectiveContributor
      {
        public readonly MissionPeer Peer;

        public float Contribution { get; private set; }

        public ObjectiveContributor(MissionPeer peer, float initialContribution)
        {
          this.Peer = peer;
          this.Contribution = initialContribution;
        }

        public void IncreaseAmount(float deltaContribution) => this.Contribution += deltaContribution;
      }
    }
  }
}
