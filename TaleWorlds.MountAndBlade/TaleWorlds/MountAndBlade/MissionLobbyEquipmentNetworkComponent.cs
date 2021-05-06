// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionLobbyEquipmentNetworkComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MissionLobbyEquipmentNetworkComponent : MissionNetwork
  {
    private Dictionary<MissionPeer, Equipment> _playerEquipments;
    private Dictionary<MissionPeer, MissionLobbyEquipmentChest> _playerChests;
    private bool _myEquipmentInitialized;

    public Equipment MyEquipment { get; private set; }

    public MissionLobbyEquipmentChest MyChest { get; private set; }

    public event MissionLobbyEquipmentNetworkComponent.OnToggleLoadoutDelegate OnToggleLoadout;

    public event MissionLobbyEquipmentNetworkComponent.OnRefreshEquipmentEventDelegate OnEquipmentRefreshed;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._playerEquipments = new Dictionary<MissionPeer, Equipment>();
      this._playerChests = new Dictionary<MissionPeer, MissionLobbyEquipmentChest>();
      if (GameNetwork.IsDedicatedServer)
        return;
      MultiplayerMissionAgentVisualSpawnComponent missionBehaviour = Mission.Current.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>();
      missionBehaviour.OnMyAgentVisualSpawned += new Action(this.OpenLoadout);
      missionBehaviour.OnMyAgentSpawnedFromVisual += new Action(this.CloseLoadout);
      missionBehaviour.OnMyAgentVisualRemoved += new Action(this.CloseLoadout);
    }

    public override void OnRemoveBehaviour()
    {
      if (!GameNetwork.IsDedicatedServer)
      {
        MultiplayerMissionAgentVisualSpawnComponent missionBehaviour = Mission.Current.GetMissionBehaviour<MultiplayerMissionAgentVisualSpawnComponent>();
        missionBehaviour.OnMyAgentVisualSpawned -= new Action(this.OpenLoadout);
        missionBehaviour.OnMyAgentSpawnedFromVisual -= new Action(this.CloseLoadout);
        missionBehaviour.OnMyAgentVisualRemoved -= new Action(this.CloseLoadout);
      }
      base.OnRemoveBehaviour();
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (GameNetwork.IsServer)
      {
        registerer.Register<RequestTroopIndexChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestTroopIndexChange>(this.HandleClientEventLobbyEquipmentUpdated));
        registerer.Register<RequestPerkChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestPerkChange>(this.HandleClientEventRequestPerkChange));
      }
      else
      {
        if (!GameNetwork.IsClientOrReplay)
          return;
        registerer.Register<UpdateSelectedTroopIndex>(new GameNetworkMessage.ServerMessageHandlerDelegate<UpdateSelectedTroopIndex>(this.HandleServerEventEquipmentIndexUpdated));
        registerer.Register<SyncPerk>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncPerk>(this.HandleServerEventSyncPerk));
      }
    }

    private void HandleServerEventEquipmentIndexUpdated(UpdateSelectedTroopIndex message) => message.Peer.GetComponent<MissionPeer>().SelectedTroopIndex = message.SelectedTroopIndex;

    private void HandleServerEventSyncPerk(SyncPerk message) => message.Peer.GetComponent<MissionPeer>().SelectPerk(message.PerkListIndex, message.PerkIndex, message.SelectedTroopIndex);

    private bool HandleClientEventLobbyEquipmentUpdated(
      NetworkCommunicator peer,
      RequestTroopIndexChange message)
    {
      MissionPeer component = peer.GetComponent<MissionPeer>();
      if (component == null)
        return false;
      SpawnComponent missionBehaviour = this.Mission.GetMissionBehaviour<SpawnComponent>();
      if (missionBehaviour == null)
        return false;
      if (missionBehaviour.AreAgentsSpawning() && component.SelectedTroopIndex != message.SelectedTroopIndex)
      {
        component.SelectedTroopIndex = message.SelectedTroopIndex;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new UpdateSelectedTroopIndex(peer, message.SelectedTroopIndex));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, peer);
        if (this.OnEquipmentRefreshed != null)
          this.OnEquipmentRefreshed(component);
      }
      return true;
    }

    private bool HandleClientEventRequestPerkChange(
      NetworkCommunicator peer,
      RequestPerkChange message)
    {
      MissionPeer component = peer.GetComponent<MissionPeer>();
      if (component == null)
        return false;
      SpawnComponent missionBehaviour = this.Mission.GetMissionBehaviour<SpawnComponent>();
      if (missionBehaviour == null)
        return false;
      if (component.SelectPerk(message.PerkListIndex, message.PerkIndex) && missionBehaviour.AreAgentsSpawning() && this.OnEquipmentRefreshed != null)
        this.OnEquipmentRefreshed(component);
      return true;
    }

    protected override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer) => this.AddPeerEquipment(networkPeer.GetComponent<MissionPeer>());

    private void AddPeerEquipment(MissionPeer peer)
    {
    }

    public Equipment GetEquipmentOf(MissionPeer peer) => (Equipment) null;

    private void UnequipItem(EquipmentIndex itemIndex, Equipment equipment) => equipment[itemIndex] = new EquipmentElement();

    private void UnequipItem(EquipmentIndex itemIndex, MissionPeer peer)
    {
      Equipment playerEquipment = this._playerEquipments[peer];
      if (playerEquipment[itemIndex].Item == null)
        return;
      this.UnequipItem(itemIndex, playerEquipment);
    }

    private bool EquipItem(Equipment equipment, ItemObject item, EquipmentIndex itemIndex)
    {
      if (!ItemData.CanItemToEquipmentDragPossible(item.StringId, (int) itemIndex))
        return false;
      equipment[itemIndex] = new EquipmentElement(item);
      return true;
    }

    private bool EquipItem(MissionPeer peer, int itemChestIndex, EquipmentIndex itemIndex) => this.EquipItem(this._playerEquipments[peer], this._playerChests[peer].GetItem(itemChestIndex), itemIndex);

    public override void OnMissionTick(float dt)
    {
      base.OnMissionTick(dt);
      if (this._myEquipmentInitialized || GameNetwork.IsServer || !GameNetwork.IsMyPeerReady)
        return;
      MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
      if (component == null)
        return;
      this.AddPeerEquipment(component);
    }

    public void PerkUpdated(int perkList, int perkIndex)
    {
      if (GameNetwork.IsServer)
      {
        MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
        if (this.OnEquipmentRefreshed == null)
          return;
        this.OnEquipmentRefreshed(component);
      }
      else
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new RequestPerkChange(perkList, perkIndex));
        GameNetwork.EndModuleEventAsClient();
      }
    }

    public void EquipmentUpdated()
    {
      if (GameNetwork.IsServer)
      {
        MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
        if (component.SelectedTroopIndex == component.NextSelectedTroopIndex)
          return;
        component.SelectedTroopIndex = component.NextSelectedTroopIndex;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new UpdateSelectedTroopIndex(GameNetwork.MyPeer, component.SelectedTroopIndex));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, GameNetwork.MyPeer);
        if (this.OnEquipmentRefreshed == null)
          return;
        this.OnEquipmentRefreshed(component);
      }
      else
      {
        MissionPeer component = GameNetwork.MyPeer.GetComponent<MissionPeer>();
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new RequestTroopIndexChange(component.NextSelectedTroopIndex));
        GameNetwork.EndModuleEventAsClient();
      }
    }

    public bool TransferFromEquipmentSlotToEquipmentSlot(
      int draggedEquipmentIndex,
      int droppedEquipmentIndex)
    {
      return false;
    }

    public int CharacterCount => 1;

    public int CurrentCharacterIndex => 0;

    public void SetNextCharacter()
    {
    }

    public void SetPreviousCharacter()
    {
    }

    public void OnEditSelectedCharacter(BodyProperties bodyProperties, bool isFemale)
    {
    }

    public bool SingleCharacterMode => true;

    public void ToggleLoadout(bool isActive)
    {
      if (this.OnToggleLoadout == null)
        return;
      this.OnToggleLoadout(isActive);
    }

    private void OpenLoadout() => this.ToggleLoadout(true);

    private void CloseLoadout() => this.ToggleLoadout(false);

    public delegate void OnEquipmentSetLoadedDelegate();

    public delegate void OnToggleLoadoutDelegate(bool isActive);

    public delegate void OnRefreshEquipmentEventDelegate(MissionPeer lobbyPeer);
  }
}
