// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BaseNetworkComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class BaseNetworkComponent : UdpNetworkComponent
  {
    public const int ComponentUniqueID = 2;
    public const float MaxIntermissionStateTime = 240f;

    public override int UniqueComponentID => 2;

    public MultiplayerIntermissionState ClientIntermissionState { get; private set; }

    public float CurrentIntermissionTimer { get; private set; }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      base.AddRemoveMessageHandlers(registerer);
      if (GameNetwork.IsClientOrReplay)
      {
        registerer.Register<AddPeerComponent>(new GameNetworkMessage.ServerMessageHandlerDelegate<AddPeerComponent>(this.HandleServerEventAddPeerComponent));
        registerer.Register<RemovePeerComponent>(new GameNetworkMessage.ServerMessageHandlerDelegate<RemovePeerComponent>(this.HandleServerEventRemovePeerComponent));
        registerer.Register<SynchronizingDone>(new GameNetworkMessage.ServerMessageHandlerDelegate<SynchronizingDone>(this.HandleServerEventSynchronizingDone));
        registerer.Register<LoadMission>(new GameNetworkMessage.ServerMessageHandlerDelegate<LoadMission>(this.HandleServerEventLoadMission));
        registerer.Register<UnloadMission>(new GameNetworkMessage.ServerMessageHandlerDelegate<UnloadMission>(this.HandleServerEventUnloadMission));
        registerer.Register<InitializeCustomGameMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<InitializeCustomGameMessage>(this.HandleServerEventInitializeCustomGame));
        registerer.Register<MultiplayerOptionsInitial>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerOptionsInitial>(this.HandleServerEventMultiplayerOptionsInitial));
        registerer.Register<MultiplayerOptionsImmediate>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerOptionsImmediate>(this.HandleServerEventMultiplayerOptionsImmediate));
        registerer.Register<MultiplayerIntermissionUpdate>(new GameNetworkMessage.ServerMessageHandlerDelegate<MultiplayerIntermissionUpdate>(this.HandleServerEventMultiplayerIntermissionUpdate));
      }
      else
      {
        if (!GameNetwork.IsServer)
          return;
        registerer.Register<FinishedLoading>(new GameNetworkMessage.ClientMessageHandlerDelegate<FinishedLoading>(this.HandleClientEventFinishedLoading));
        registerer.Register<SyncRelevantGameOptionsToServer>(new GameNetworkMessage.ClientMessageHandlerDelegate<SyncRelevantGameOptionsToServer>(this.HandleSyncRelevantGameOptionsToServer));
      }
    }

    public override void OnUdpNetworkHandlerTick(float dt)
    {
      base.OnUdpNetworkHandlerTick(dt);
      if (!GameNetwork.IsClientOrReplay || this.ClientIntermissionState != MultiplayerIntermissionState.CountingForMission && this.ClientIntermissionState != MultiplayerIntermissionState.CountingForEnd)
        return;
      this.CurrentIntermissionTimer -= dt;
      if ((double) this.CurrentIntermissionTimer > 0.0)
        return;
      this.CurrentIntermissionTimer = 0.0f;
    }

    public override void HandleNewClientConnect(PlayerConnectionInfo playerConnectionInfo)
    {
      NetworkCommunicator networkPeer = playerConnectionInfo.NetworkPeer;
      if (networkPeer.IsServerPeer)
        return;
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new MultiplayerOptionsInitial());
      GameNetwork.EndModuleEventAsServer();
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new MultiplayerOptionsImmediate());
      GameNetwork.EndModuleEventAsServer();
      if (BannerlordNetwork.LobbyMissionType != LobbyMissionType.Custom)
        return;
      bool inMission = false;
      string map = "";
      string gameType = "";
      if (GameNetwork.IsDedicatedServer && Mission.Current != null || !GameNetwork.IsDedicatedServer)
      {
        inMission = true;
        MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map).GetValue(out map);
        MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType).GetValue(out gameType);
      }
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new InitializeCustomGameMessage(inMission, gameType, map));
      GameNetwork.EndModuleEventAsServer();
    }

    public bool DisplayingWelcomeMessage { get; private set; }

    public event BaseNetworkComponent.WelcomeMessageReceivedDelegate WelcomeMessageReceived;

    public void SetDisplayingWelcomeMessage(bool displaying) => this.DisplayingWelcomeMessage = displaying;

    private void HandleServerEventMultiplayerOptionsInitial(MultiplayerOptionsInitial message)
    {
      for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; ++optionType)
      {
        MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
        if (optionProperty.Replication == MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad)
        {
          switch (optionProperty.OptionValueType)
          {
            case MultiplayerOptions.OptionValueType.Bool:
              bool flag;
              message.GetOption(optionType).GetValue(out flag);
              optionType.SetValue(flag);
              continue;
            case MultiplayerOptions.OptionValueType.Integer:
            case MultiplayerOptions.OptionValueType.Enum:
              int num;
              message.GetOption(optionType).GetValue(out num);
              optionType.SetValue(num);
              continue;
            case MultiplayerOptions.OptionValueType.String:
              string str;
              message.GetOption(optionType).GetValue(out str);
              optionType.SetValue(str);
              continue;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
      string strValue = MultiplayerOptions.OptionType.WelcomeMessage.GetStrValue();
      if (string.IsNullOrEmpty(strValue))
        return;
      BaseNetworkComponent.WelcomeMessageReceivedDelegate welcomeMessageReceived = this.WelcomeMessageReceived;
      if (welcomeMessageReceived == null)
        return;
      welcomeMessageReceived(strValue);
    }

    private void HandleServerEventMultiplayerOptionsImmediate(MultiplayerOptionsImmediate message)
    {
      for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; ++optionType)
      {
        MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
        if (optionProperty.Replication == MultiplayerOptionsProperty.ReplicationOccurrence.Immediately)
        {
          switch (optionProperty.OptionValueType)
          {
            case MultiplayerOptions.OptionValueType.Bool:
              bool flag;
              message.GetOption(optionType).GetValue(out flag);
              optionType.SetValue(flag);
              continue;
            case MultiplayerOptions.OptionValueType.Integer:
            case MultiplayerOptions.OptionValueType.Enum:
              int num;
              message.GetOption(optionType).GetValue(out num);
              optionType.SetValue(num);
              continue;
            case MultiplayerOptions.OptionValueType.String:
              string str;
              message.GetOption(optionType).GetValue(out str);
              optionType.SetValue(str);
              continue;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
      }
    }

    private void HandleServerEventMultiplayerIntermissionUpdate(
      MultiplayerIntermissionUpdate message)
    {
      this.CurrentIntermissionTimer = message.IntermissionTimer;
      this.ClientIntermissionState = message.IntermissionState;
    }

    private void HandleServerEventAddPeerComponent(AddPeerComponent message)
    {
      NetworkCommunicator peer = message.Peer;
      uint componentId = message.ComponentId;
      if (peer.GetComponent(componentId) != null)
        return;
      peer.AddComponent(componentId);
    }

    private void HandleServerEventRemovePeerComponent(RemovePeerComponent message)
    {
      NetworkCommunicator peer = message.Peer;
      peer.RemoveComponent(peer.GetComponent(message.ComponentId));
    }

    private void HandleServerEventSynchronizingDone(SynchronizingDone message) => message.Peer.IsSynchronized = message.Synchronized;

    private void HandleServerEventLoadMission(LoadMission message)
    {
      GameNetwork.MyPeer.IsSynchronized = false;
      this.CurrentIntermissionTimer = 0.0f;
      this.ClientIntermissionState = MultiplayerIntermissionState.Idle;
      Module.CurrentModule.StartMultiplayerGame(message.GameType, message.Map);
    }

    private void HandleServerEventUnloadMission(UnloadMission message) => this.HandleServerEventUnloadMissionAux();

    private void HandleServerEventInitializeCustomGame(InitializeCustomGameMessage message) => this.InitializeCustomGameAux(message);

    private async void InitializeCustomGameAux(InitializeCustomGameMessage message)
    {
      if (message.InMission)
      {
        MBDebug.Print("Client: I have received InitializeCustomGameMessage with mission " + message.GameType + " " + message.Map + ". Loading it...", debugFilter: 17179869184UL);
        if (Module.CurrentModule.StartMultiplayerGame(message.GameType, message.Map))
          ;
      }
      else
      {
        await Task.Delay(200);
        while (!(GameStateManager.Current.ActiveState is LobbyGameStateCustomGameClient))
          await Task.Delay(1);
        LoadingWindow.DisableGlobalLoadingWindow();
        MBDebug.Print("Client: I have received InitializeCustomGameMessage with no mission. Sending confirmation to server.", debugFilter: 17179869184UL);
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new FinishedLoading());
        GameNetwork.EndModuleEventAsClient();
        GameNetwork.SyncRelevantGameOptionsToServer();
      }
    }

    private async void HandleServerEventUnloadMissionAux()
    {
      GameNetwork.MyPeer.IsSynchronized = false;
      this.CurrentIntermissionTimer = 0.0f;
      this.ClientIntermissionState = MultiplayerIntermissionState.Idle;
      BannerlordNetwork.EndMultiplayerLobbyMission();
      while (Mission.Current != null)
        await Task.Delay(1);
      LoadingWindow.DisableGlobalLoadingWindow();
      MBDebug.Print("Client: I finished HandleServerEventUnloadMissionAux. Sending confirmation to server.", debugFilter: 17179869184UL);
      GameNetwork.BeginModuleEventAsClient();
      GameNetwork.WriteMessage((GameNetworkMessage) new FinishedLoading());
      GameNetwork.EndModuleEventAsClient();
    }

    private bool HandleClientEventFinishedLoading(
      NetworkCommunicator networkPeer,
      FinishedLoading message)
    {
      this.HandleClientEventFinishedLoadingAux(networkPeer, message);
      return true;
    }

    private async void HandleClientEventFinishedLoadingAux(
      NetworkCommunicator networkPeer,
      FinishedLoading message)
    {
      while (Mission.Current != null && Mission.Current.CurrentState != Mission.State.Continuing)
        await Task.Delay(1);
      if (networkPeer.IsServerPeer)
        return;
      MBDebug.Print("Server: " + networkPeer.UserName + " has finished loading. From now on, I will include him in the broadcasted messages", debugFilter: 17179869184UL);
      GameNetwork.ClientFinishedLoading(networkPeer);
    }

    private bool HandleSyncRelevantGameOptionsToServer(
      NetworkCommunicator networkPeer,
      SyncRelevantGameOptionsToServer message)
    {
      networkPeer.SetRelevantGameOptions(message.SendMeBloodEvents, message.SendMeSoundEvents);
      return true;
    }

    public delegate void WelcomeMessageReceivedDelegate(string messageText);
  }
}
