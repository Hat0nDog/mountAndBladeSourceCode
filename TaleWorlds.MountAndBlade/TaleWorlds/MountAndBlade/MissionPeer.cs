// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionPeer
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
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MissionPeer : PeerComponent
  {
    public const int NumberOfPerkLists = 3;
    public const int MaxNumberOfTroopTypesPerCulture = 16;
    private const float InactivityKickInSeconds = 180f;
    private const float InactivityWarnInSeconds = 120f;
    public const int MinKDACount = -1000;
    public const int MaxKDACount = 100000;
    public const int MinSpawnTimer = 3;
    private List<PeerVisualsHolder> _visuals = new List<PeerVisualsHolder>();
    private Dictionary<MissionPeer, int> _numberOfTimesPeerKilledPerPeer = new Dictionary<MissionPeer, int>();
    private MissionTime _lastActiveTime = MissionTime.Zero;
    private (Agent.MovementControlFlag, Vec2, Vec3) _previousActivityStatus;
    private bool _inactiveWarningGiven;
    private int _selectedTroopIndex;
    private Agent _followedAgent;
    private Team _team;
    private Formation _controlledFormation;
    private MissionRepresentativeBase _representative;
    private readonly List<List<int>> _perks;
    private int _killCount;
    private int _assistCount;
    private int _deathCount;
    private (int, List<MPPerkObject>) _selectedPerks;
    private int _botsUnderControlAlive;

    public static event MissionPeer.OnUpdateEquipmentSetIndexEventDelegate OnEquipmentIndexRefreshed;

    public static event MissionPeer.OnTeamChangedDelegate OnPreTeamChanged;

    public static event MissionPeer.OnTeamChangedDelegate OnTeamChanged;

    public static event MissionPeer.OnPlayerKilledDelegate OnPlayerKilled;

    public DateTime JoinTime { get; internal set; }

    public bool EquipmentUpdatingExpired { get; set; }

    public bool HasSpawnedAgentVisuals { get; set; }

    public int SelectedTroopIndex
    {
      get => this._selectedTroopIndex;
      set
      {
        if (this._selectedTroopIndex == value)
          return;
        this._selectedTroopIndex = value;
        this.ResetSelectedPerks();
        MissionPeer.OnUpdateEquipmentSetIndexEventDelegate equipmentIndexRefreshed = MissionPeer.OnEquipmentIndexRefreshed;
        if (equipmentIndexRefreshed == null)
          return;
        equipmentIndexRefreshed(this, value);
      }
    }

    public int NextSelectedTroopIndex { get; set; }

    public MissionRepresentativeBase Representative
    {
      get
      {
        if (this._representative == null)
          this._representative = this.Peer.GetComponent<MissionRepresentativeBase>();
        return this._representative;
      }
    }

    public IReadOnlyList<List<int>> Perks => (IReadOnlyList<List<int>>) this._perks;

    public IReadOnlyList<MPPerkObject> SelectedPerks
    {
      get
      {
        if (this.SelectedTroopIndex < 0)
          return (IReadOnlyList<MPPerkObject>) new List<MPPerkObject>();
        if (this._selectedPerks.Item2 == null || this.SelectedTroopIndex != this._selectedPerks.Item1 || this._selectedPerks.Item2.Count < 3)
        {
          List<MPPerkObject> mpPerkObjectList = new List<MPPerkObject>();
          List<List<IReadOnlyPerkObject>> availablePerksForPeer = MultiplayerClassDivisions.GetAvailablePerksForPeer(this);
          if (availablePerksForPeer.Count != 3)
            return (IReadOnlyList<MPPerkObject>) mpPerkObjectList;
          for (int index = 0; index < 3; ++index)
          {
            int num = this._perks[this.SelectedTroopIndex][index];
            if (availablePerksForPeer[index].Count > 0)
              mpPerkObjectList.Add(availablePerksForPeer[index][num < 0 || num >= availablePerksForPeer[index].Count ? 0 : num].Clone(this));
          }
          this._selectedPerks = (this.SelectedTroopIndex, mpPerkObjectList);
        }
        return (IReadOnlyList<MPPerkObject>) this._selectedPerks.Item2;
      }
    }

    public MissionPeer()
    {
      this.SpawnTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), 3f, false);
      this._selectedPerks = (0, (List<MPPerkObject>) null);
      this._perks = new List<List<int>>();
      for (int index1 = 0; index1 < 16; ++index1)
      {
        List<int> intList = new List<int>();
        for (int index2 = 0; index2 < 3; ++index2)
          intList.Add(0);
        this._perks.Add(intList);
      }
    }

    public Timer SpawnTimer { get; internal set; }

    public bool HasSpawnTimerExpired { get; set; }

    public BasicCultureObject VotedForBan { get; private set; }

    public BasicCultureObject VotedForSelection { get; private set; }

    public bool WantsToSpawnAsBot { get; set; }

    public int SpawnCountThisRound { get; set; }

    public int KillCount
    {
      get => this._killCount;
      internal set => this._killCount = MBMath.ClampInt(value, -1000, 100000);
    }

    public int AssistCount
    {
      get => this._assistCount;
      internal set => this._assistCount = MBMath.ClampInt(value, -1000, 100000);
    }

    public int DeathCount
    {
      get => this._deathCount;
      internal set => this._deathCount = MBMath.ClampInt(value, -1000, 100000);
    }

    public int Score { get; internal set; }

    public int BotsUnderControlAlive
    {
      get => this._botsUnderControlAlive;
      set
      {
        if (this._botsUnderControlAlive == value)
          return;
        this._botsUnderControlAlive = value;
        MPPerkObject.GetPerkHandler(this)?.OnEvent(MPPerkCondition.PerkEventFlags.AliveBotCountChange);
      }
    }

    public int BotsUnderControlTotal { get; internal set; }

    public bool IsControlledAgentActive => this.ControlledAgent != null && this.ControlledAgent.IsActive();

    public Agent ControlledAgent
    {
      get => this.GetNetworkPeer().ControlledAgent;
      set
      {
        NetworkCommunicator networkPeer = this.GetNetworkPeer();
        if (networkPeer.ControlledAgent == value)
          return;
        this.ResetSelectedPerks();
        Agent controlledAgent = networkPeer.ControlledAgent;
        networkPeer.ControlledAgent = value;
        if (controlledAgent != null && controlledAgent.MissionPeer == this && controlledAgent.IsActive())
          controlledAgent.MissionPeer = (MissionPeer) null;
        if (networkPeer.ControlledAgent != null && networkPeer.ControlledAgent.MissionPeer != this)
          networkPeer.ControlledAgent.MissionPeer = this;
        networkPeer.VirtualPlayer.GetComponent<MissionRepresentativeBase>()?.SetAgent(value);
        if (value == null)
          return;
        MPPerkObject.GetPerkHandler(this)?.OnEvent(value, MPPerkCondition.PerkEventFlags.PeerControlledAgentChange);
      }
    }

    public Agent FollowedAgent
    {
      get => this._followedAgent;
      set
      {
        if (this._followedAgent == value)
          return;
        this._followedAgent = value;
        if (!GameNetwork.IsClient)
          return;
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new SetFollowedAgent(this._followedAgent));
        GameNetwork.EndModuleEventAsClient();
      }
    }

    public Team Team
    {
      get => this._team;
      set
      {
        if (this._team == value)
          return;
        if (MissionPeer.OnPreTeamChanged != null)
          MissionPeer.OnPreTeamChanged(this.GetNetworkPeer(), this._team, value);
        Team team = this._team;
        this._team = value;
        Debug.Print("Setting team to: " + (object) value.Side + ", for peer: " + this.Name);
        this._controlledFormation = (Formation) null;
        if (this._team != null)
        {
          MissionPeer component = this.GetComponent<MissionPeer>();
          if (this._team.IsAttacker || this._team.IsDefender)
          {
            component.SelectedTroopIndex = 0;
            component.NextSelectedTroopIndex = 0;
          }
          if (GameNetwork.IsServer)
          {
            MBAPI.IMBPeer.SetTeam(this.Peer.Index, this._team.MBTeam.Index);
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage) new SetPeerTeam(this.GetNetworkPeer(), this._team));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord);
          }
          if (MissionPeer.OnTeamChanged == null)
            return;
          MissionPeer.OnTeamChanged(this.GetNetworkPeer(), team, this._team);
        }
        else
        {
          if (!GameNetwork.IsServer)
            return;
          MBAPI.IMBPeer.SetTeam(this.Peer.Index, -1);
        }
      }
    }

    public Formation ControlledFormation
    {
      get => this._controlledFormation;
      set
      {
        if (this._controlledFormation == value)
          return;
        this._controlledFormation = value;
      }
    }

    public bool IsAgentAliveForChatting
    {
      get
      {
        MissionPeer component = this.GetComponent<MissionPeer>();
        if (component == null)
          return false;
        return this.IsControlledAgentActive || component.HasSpawnedAgentVisuals;
      }
    }

    public int GetSelectedPerkIndexWithPerkListIndex(int troopIndex, int perkListIndex) => this._perks[troopIndex][perkListIndex];

    public bool SelectPerk(int perkListIndex, int perkIndex, int enforcedSelectedTroopIndex = -1)
    {
      if (this.SelectedTroopIndex >= 0 && enforcedSelectedTroopIndex >= 0 && this.SelectedTroopIndex != enforcedSelectedTroopIndex)
      {
        Debug.Print("SelectedTroopIndex < 0 || enforcedSelectedTroopIndex < 0 || SelectedTroopIndex == enforcedSelectedTroopIndex", debugFilter: 17179869184UL);
        Debug.Print(string.Format("SelectedTroopIndex: {0} enforcedSelectedTroopIndex: {1}", (object) this.SelectedTroopIndex, (object) enforcedSelectedTroopIndex), debugFilter: 17179869184UL);
      }
      int num = enforcedSelectedTroopIndex >= 0 ? enforcedSelectedTroopIndex : this.SelectedTroopIndex;
      if (perkIndex == this._perks[num][perkListIndex])
        return false;
      this._perks[num][perkListIndex] = perkIndex;
      if (num == this.SelectedTroopIndex)
        this.ResetSelectedPerks();
      if (GameNetwork.IsServerOrRecorder)
      {
        NetworkCommunicator networkPeer = this.GetNetworkPeer();
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new SyncPerk(networkPeer, perkListIndex, perkIndex, num));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, networkPeer);
      }
      return true;
    }

    public void HandleVoteChange(CultureVoteTypes voteType, BasicCultureObject culture)
    {
      switch (voteType)
      {
        case CultureVoteTypes.Ban:
          this.VotedForBan = culture;
          break;
        case CultureVoteTypes.Select:
          this.VotedForSelection = culture;
          break;
      }
      if (!GameNetwork.IsServer)
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new CultureVoteServer(this.GetNetworkPeer(), voteType, voteType == CultureVoteTypes.Ban ? this.VotedForBan : this.VotedForSelection));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    public override void OnFinalize()
    {
      base.OnFinalize();
      this.ResetKillRegistry();
      if (!this.HasSpawnedAgentVisuals || Mission.Current == null)
        return;
      Mission.Current.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>()?.RemoveAgentVisuals(this);
    }

    public int GetAmountOfAgentVisualsForPeer() => this._visuals.Count<PeerVisualsHolder>((Func<PeerVisualsHolder, bool>) (v => v != null));

    public PeerVisualsHolder GetVisuals(int visualIndex) => !this._visuals.Any<PeerVisualsHolder>() ? (PeerVisualsHolder) null : this._visuals[visualIndex];

    public void ClearVisuals(int visualIndex)
    {
      if (visualIndex >= this._visuals.Count || this._visuals[visualIndex] == null)
        return;
      if (!GameNetwork.IsDedicatedServer)
      {
        MBAgentVisuals visuals1 = this._visuals[visualIndex].AgentVisuals.GetVisuals();
        visuals1.ClearVisualComponents(true);
        visuals1.ClearAllWeaponMeshes();
        visuals1.Reset();
        if (this._visuals[visualIndex].MountAgentVisuals != null)
        {
          MBAgentVisuals visuals2 = this._visuals[visualIndex].MountAgentVisuals.GetVisuals();
          visuals2.ClearVisualComponents(true);
          visuals2.ClearAllWeaponMeshes();
          visuals2.Reset();
        }
      }
      this._visuals[visualIndex] = (PeerVisualsHolder) null;
    }

    public void ClearAllVisuals(bool freeResources = false)
    {
      for (int index = this._visuals.Count - 1; index >= 0; --index)
      {
        if (this._visuals[index] != null)
          this.ClearVisuals(index);
      }
      if (!freeResources)
        return;
      this._visuals = (List<PeerVisualsHolder>) null;
    }

    public void OnVisualsSpawned(PeerVisualsHolder visualsHolder, int visualIndex)
    {
      if (visualIndex >= this._visuals.Count)
      {
        int num = visualIndex - this._visuals.Count;
        for (int index = 0; index < num + 1; ++index)
          this._visuals.Add((PeerVisualsHolder) null);
      }
      this._visuals[visualIndex] = visualsHolder;
    }

    public IEnumerable<IAgentVisual> GetAllAgentVisualsForPeer()
    {
      int count = this.GetAmountOfAgentVisualsForPeer();
      for (int i = 0; i < count; ++i)
        yield return this.GetVisuals(i).AgentVisuals;
    }

    public IAgentVisual GetAgentVisualForPeer(int visualsIndex) => this.GetAgentVisualForPeer(visualsIndex, out IAgentVisual _);

    public IAgentVisual GetAgentVisualForPeer(
      int visualsIndex,
      out IAgentVisual mountAgentVisuals)
    {
      PeerVisualsHolder visuals = this.GetVisuals(visualsIndex);
      mountAgentVisuals = visuals?.MountAgentVisuals;
      return visuals?.AgentVisuals;
    }

    public void TickInactivityStatus()
    {
      NetworkCommunicator networkPeer = this.GetNetworkPeer();
      if (networkPeer.IsMine)
        return;
      if (this.ControlledAgent != null && this.ControlledAgent.IsActive())
      {
        if (this._lastActiveTime == MissionTime.Zero)
        {
          this._lastActiveTime = MissionTime.Now;
          this._previousActivityStatus = ValueTuple.Create<Agent.MovementControlFlag, Vec2, Vec3>(this.ControlledAgent.MovementFlags, this.ControlledAgent.MovementInputVector, this.ControlledAgent.LookDirection);
          this._inactiveWarningGiven = false;
        }
        else
        {
          (Agent.MovementControlFlag, Vec2, Vec3) tuple = ValueTuple.Create<Agent.MovementControlFlag, Vec2, Vec3>(this.ControlledAgent.MovementFlags, this.ControlledAgent.MovementInputVector, this.ControlledAgent.LookDirection);
          if (this._previousActivityStatus.Item1 != tuple.Item1 || (double) this._previousActivityStatus.Item2.DistanceSquared(tuple.Item2) > 9.99999974737875E-06 || (double) this._previousActivityStatus.Item3.DistanceSquared(tuple.Item3) > 9.99999974737875E-06)
          {
            this._lastActiveTime = MissionTime.Now;
            this._previousActivityStatus = tuple;
            this._inactiveWarningGiven = false;
          }
          if ((double) this._lastActiveTime.ElapsedSeconds > 180.0)
          {
            DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
            disconnectInfo.Type = DisconnectType.Inactivity;
            networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", (object) disconnectInfo);
            GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
          }
          else
          {
            if ((double) this._lastActiveTime.ElapsedSeconds <= 120.0 || this._inactiveWarningGiven)
              return;
            Mission.Current.GetMissionBehaviour<MultiplayerGameNotificationsComponent>()?.PlayerIsInactive(this.GetNetworkPeer());
            this._inactiveWarningGiven = true;
          }
        }
      }
      else
      {
        this._lastActiveTime = MissionTime.Now;
        this._inactiveWarningGiven = false;
      }
    }

    internal void OnKillAnotherPeer(MissionPeer killedPeer)
    {
      if (killedPeer == null)
        return;
      if (!this._numberOfTimesPeerKilledPerPeer.ContainsKey(killedPeer))
        this._numberOfTimesPeerKilledPerPeer.Add(killedPeer, 1);
      else
        this._numberOfTimesPeerKilledPerPeer[killedPeer]++;
      MissionPeer.OnPlayerKilledDelegate onPlayerKilled = MissionPeer.OnPlayerKilled;
      if (onPlayerKilled == null)
        return;
      onPlayerKilled(this, killedPeer);
    }

    public int GetNumberOfTimesPeerKilledPeer(MissionPeer killedPeer) => this._numberOfTimesPeerKilledPerPeer.ContainsKey(killedPeer) ? this._numberOfTimesPeerKilledPerPeer[killedPeer] : 0;

    public void ResetKillRegistry() => this._numberOfTimesPeerKilledPerPeer.Clear();

    private void ResetSelectedPerks()
    {
      if (this._selectedPerks.Item2 == null)
        return;
      foreach (MPPerkObject mpPerkObject in this._selectedPerks.Item2)
        mpPerkObject.Reset();
    }

    public delegate void OnUpdateEquipmentSetIndexEventDelegate(
      MissionPeer lobbyPeer,
      int equipmentSetIndex);

    public delegate void OnTeamChangedDelegate(
      NetworkCommunicator peer,
      Team previousTeam,
      Team newTeam);

    public delegate void OnPlayerKilledDelegate(MissionPeer killerPeer, MissionPeer killedPeer);
  }
}
