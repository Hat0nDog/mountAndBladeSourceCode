// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SpawnComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class SpawnComponent : MissionLogic
  {
    public SpawnFrameBehaviourBase SpawnFrameBehaviour { get; private set; }

    public SpawningBehaviourBase SpawningBehaviour { get; private set; }

    public SpawnComponent(
      SpawnFrameBehaviourBase spawnFrameBehaviour,
      SpawningBehaviourBase spawningBehaviour)
    {
      this.SpawnFrameBehaviour = spawnFrameBehaviour;
      this.SpawningBehaviour = spawningBehaviour;
    }

    public bool AreAgentsSpawning() => this.SpawningBehaviour.AreAgentsSpawning();

    public void SetNewSpawnFrameBehaviour(SpawnFrameBehaviourBase spawnFrameBehaviour)
    {
      this.SpawnFrameBehaviour = spawnFrameBehaviour;
      if (this.SpawnFrameBehaviour == null)
        return;
      this.SpawnFrameBehaviour.Initialize();
    }

    public void SetNewSpawningBehaviour(SpawningBehaviourBase spawningBehaviour)
    {
      this.SpawningBehaviour = spawningBehaviour;
      if (this.SpawningBehaviour == null)
        return;
      this.SpawningBehaviour.Initialize(this);
    }

    protected override void OnEndMission()
    {
      base.OnEndMission();
      this.SpawningBehaviour.Clear();
    }

    public static void SetSiegeSpawningBehaviour()
    {
      Mission.Current.GetMissionBehaviour<SpawnComponent>().SetNewSpawnFrameBehaviour((SpawnFrameBehaviourBase) new SiegeSpawnFrameBehaviour());
      Mission.Current.GetMissionBehaviour<SpawnComponent>().SetNewSpawningBehaviour((SpawningBehaviourBase) new SiegeSpawningBehaviour());
    }

    public static void SetFlagDominationSpawningBehaviour()
    {
      Mission.Current.GetMissionBehaviour<SpawnComponent>().SetNewSpawnFrameBehaviour((SpawnFrameBehaviourBase) new FlagDominationSpawnFrameBehaviour());
      Mission.Current.GetMissionBehaviour<SpawnComponent>().SetNewSpawningBehaviour((SpawningBehaviourBase) new FlagDominationSpawningBehaviour());
    }

    public static void SetWarmupSpawningBehaviour()
    {
      Mission.Current.GetMissionBehaviour<SpawnComponent>().SetNewSpawnFrameBehaviour((SpawnFrameBehaviourBase) new FFASpawnFrameBehaviour());
      Mission.Current.GetMissionBehaviour<SpawnComponent>().SetNewSpawningBehaviour((SpawningBehaviourBase) new WarmupSpawningBehaviour());
    }

    public static void SetSpawningBehaviorForCurrentGameType(
      MissionLobbyComponent.MultiplayerGameType currentGameType)
    {
      switch (currentGameType)
      {
        case MissionLobbyComponent.MultiplayerGameType.Siege:
          SpawnComponent.SetSiegeSpawningBehaviour();
          break;
        case MissionLobbyComponent.MultiplayerGameType.Captain:
        case MissionLobbyComponent.MultiplayerGameType.Skirmish:
          SpawnComponent.SetFlagDominationSpawningBehaviour();
          break;
      }
    }

    public override void AfterStart()
    {
      base.AfterStart();
      this.SetNewSpawnFrameBehaviour(this.SpawnFrameBehaviour);
      this.SetNewSpawningBehaviour(this.SpawningBehaviour);
    }

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      this.SpawningBehaviour.OnTick(dt);
    }

    protected void StartSpawnSession() => this.SpawningBehaviour.RequestStartSpawnSession();

    public MatrixFrame GetSpawnFrame(Team team, bool hasMount, bool isInitialSpawn = false) => this.SpawnFrameBehaviour == null ? MatrixFrame.Identity : this.SpawnFrameBehaviour.GetSpawnFrame(team, hasMount, isInitialSpawn);

    protected void SpawnEquipmentUpdated(MissionPeer lobbyPeer, Equipment equipment)
    {
      if (!GameNetwork.IsServer || lobbyPeer == null || (!this.SpawningBehaviour.CanUpdateSpawnEquipment(lobbyPeer) || !lobbyPeer.HasSpawnedAgentVisuals))
        return;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new EquipEquipmentToPeer(lobbyPeer.GetNetworkPeer(), equipment));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    public void SetEarlyAgentVisualsDespawning(MissionPeer missionPeer, bool canDespawnEarly = true)
    {
      if (missionPeer == null || !this.AllowEarlyAgentVisualsDespawning(missionPeer))
        return;
      missionPeer.EquipmentUpdatingExpired = canDespawnEarly;
    }

    public void ToggleUpdatingSpawnEquipment(bool canUpdate) => this.SpawningBehaviour.ToggleUpdatingSpawnEquipment(canUpdate);

    public bool AllowEarlyAgentVisualsDespawning(MissionPeer lobbyPeer) => this.SpawningBehaviour.AllowEarlyAgentVisualsDespawning(lobbyPeer);

    public int GetMaximumReSpawnPeriodForPeer(MissionPeer lobbyPeer) => this.SpawningBehaviour.GetMaximumReSpawnPeriodForPeer(lobbyPeer);

    public override void OnClearScene()
    {
      base.OnClearScene();
      this.SpawningBehaviour.OnClearScene();
    }
  }
}
