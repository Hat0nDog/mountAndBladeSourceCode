// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GameNetwork
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public static class GameNetwork
  {
    private static IGameNetworkHandler _handler;
    public static int ClientPeerIndex;
    private const MultiplayerMessageFilter MultiplayerLogging = MultiplayerMessageFilter.All;
    private static List<UdpNetworkComponent> _destroyedComponents;
    private static List<IUdpNetworkHandler> _destroyedHandlers;
    private static Dictionary<System.Type, int> _gameNetworkMessageTypesAll;
    private static Dictionary<System.Type, int> _gameNetworkMessageTypesFromClient;
    private static List<System.Type> _gameNetworkMessageIdsFromClient;
    private static Dictionary<System.Type, int> _gameNetworkMessageTypesFromServer;
    private static List<System.Type> _gameNetworkMessageIdsFromServer;
    private static Dictionary<int, List<object>> _fromClientMessageHandlers;
    private static Dictionary<int, List<object>> _fromServerMessageHandlers;

    public static bool IsServer => MBCommon.CurrentGameType == MBCommon.GameType.MultiServer || MBCommon.CurrentGameType == MBCommon.GameType.MultiClientServer;

    public static bool IsServerOrRecorder => GameNetwork.IsServer || MBCommon.CurrentGameType == MBCommon.GameType.SingleRecord;

    public static bool IsClient => MBCommon.CurrentGameType == MBCommon.GameType.MultiClient;

    public static bool IsReplay => MBCommon.CurrentGameType == MBCommon.GameType.SingleReplay;

    public static bool IsClientOrReplay => GameNetwork.IsClient || GameNetwork.IsReplay;

    public static bool IsDedicatedServer => false;

    public static bool MultiplayerDisabled => false;

    public static bool IsMultiplayer => GameNetwork.IsServer || GameNetwork.IsClient;

    public static bool IsSessionActive => GameNetwork.IsServerOrRecorder || GameNetwork.IsClientOrReplay;

    public static IEnumerable<NetworkCommunicator> NetworkPeers => MBNetwork.NetworkPeers.OfType<NetworkCommunicator>();

    public static int NetworkPeerCount => MBNetwork.NetworkPeers.Count;

    public static bool NetworkPeersValid => MBNetwork.NetworkPeers != null;

    private static void AddNetworkPeer(NetworkCommunicator networkPeer) => MBNetwork.NetworkPeers.Add((ICommunicator) networkPeer);

    private static void RemoveNetworkPeer(NetworkCommunicator networkPeer) => MBNetwork.NetworkPeers.Remove((ICommunicator) networkPeer);

    public static void ClearAllPeers()
    {
      for (int index = 0; index < MBNetwork.VirtualPlayers.Length; ++index)
        MBNetwork.VirtualPlayers[index] = (VirtualPlayer) null;
      MBNetwork.NetworkPeers.Clear();
    }

    public static NetworkCommunicator FindNetworkPeer(int index)
    {
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        if (networkPeer.Index == index)
          return networkPeer;
      }
      return (NetworkCommunicator) null;
    }

    public static void Initialize(IGameNetworkHandler handler)
    {
      GameNetwork._handler = handler;
      MBNetwork.Initialize((INetworkCommunication) new NetworkCommunication());
      GameNetwork.NetworkComponents = new List<UdpNetworkComponent>();
      GameNetwork.NetworkHandlers = new List<IUdpNetworkHandler>();
      GameNetwork._destroyedComponents = new List<UdpNetworkComponent>();
      GameNetwork._destroyedHandlers = new List<IUdpNetworkHandler>();
      GameNetwork._handler.OnInitialize();
    }

    internal static void Tick(float dt)
    {
      foreach (IUdpNetworkHandler destroyedHandler in GameNetwork._destroyedHandlers)
      {
        destroyedHandler.OnUdpNetworkHandlerClose();
        GameNetwork.NetworkHandlers.Remove(destroyedHandler);
      }
      foreach (UdpNetworkComponent destroyedComponent in GameNetwork._destroyedComponents)
        GameNetwork.NetworkComponents.Remove(destroyedComponent);
      GameNetwork._destroyedComponents.Clear();
      GameNetwork._destroyedHandlers.Clear();
      int index = 0;
      try
      {
        for (index = 0; index < GameNetwork.NetworkHandlers.Count; ++index)
          GameNetwork.NetworkHandlers[index].OnUdpNetworkHandlerTick(dt);
      }
      catch (Exception ex)
      {
        if (GameNetwork.NetworkHandlers.Count > 0 && index < GameNetwork.NetworkHandlers.Count && GameNetwork.NetworkHandlers[index] != null)
          Debug.Print("Exception On Network Component: " + GameNetwork.NetworkHandlers[index].ToString());
        Debug.Print(ex.StackTrace);
        Debug.Print(ex.Message);
      }
    }

    private static void StartMultiplayer()
    {
      VirtualPlayer.Reset();
      GameNetwork._handler.OnStartMultiplayer();
    }

    public static void EndMultiplayer()
    {
      GameNetwork._handler.OnEndMultiplayer();
      foreach (UdpNetworkComponent networkComponent in GameNetwork.NetworkComponents)
        GameNetwork.DestroyComponent(networkComponent);
      foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
        GameNetwork.RemoveNetworkHandler(networkHandler);
      if (GameNetwork.IsServer)
        GameNetwork.TerminateServerSide();
      if (GameNetwork.IsClientOrReplay)
        GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
      if (GameNetwork.IsClient)
        GameNetwork.TerminateClientSide();
      if (GameNetwork.NetworkPeerCount > 0)
      {
        MBDebug.Print("Clearing peers list with count " + (object) GameNetwork.NetworkPeerCount);
        GameNetwork.ClearAllPeers();
      }
      VirtualPlayer.Reset();
      GameNetwork.MyPeer = (NetworkCommunicator) null;
      MBDebug.Print("NetworkManager::HandleMultiplayerEnd");
    }

    [MBCallback]
    internal static void HandleRemovePlayer(MBNetworkPeer peer, bool isTimedOut)
    {
      DisconnectInfo disconnectInfo1 = peer.NetworkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo");
      if (disconnectInfo1 == null)
        disconnectInfo1 = new DisconnectInfo()
        {
          Type = DisconnectType.QuitFromGame
        };
      DisconnectInfo disconnectInfo2 = disconnectInfo1;
      disconnectInfo2.Type = isTimedOut ? DisconnectType.TimedOut : disconnectInfo2.Type;
      peer.NetworkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", (object) disconnectInfo2);
      GameNetwork.HandleRemovePlayer(peer.NetworkPeer);
    }

    internal static void HandleRemovePlayer(NetworkCommunicator networkPeer)
    {
      if (GameNetwork.IsClient && networkPeer.IsMine)
      {
        GameNetwork.HandleDisconnect();
      }
      else
      {
        GameNetwork._handler.OnPlayerDisconnectedFromServer(networkPeer);
        if (GameNetwork.IsServer)
        {
          foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
            networkHandler.HandleEarlyPlayerDisconnect(networkPeer);
          foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
            networkHandler.HandlePlayerDisconnect(networkPeer);
        }
        networkPeer.VirtualPlayer.OnDisconnect();
        GameNetwork.RemoveNetworkPeer(networkPeer);
        MBNetwork.VirtualPlayers[networkPeer.VirtualPlayer.Index] = (VirtualPlayer) null;
        if (!GameNetwork.IsServer)
          return;
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new DeletePlayer(networkPeer.Index));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
      }
    }

    [MBCallback]
    internal static void HandleDisconnect()
    {
      GameNetwork._handler.OnDisconnectedFromServer();
      GameNetwork.MyPeer = (NetworkCommunicator) null;
    }

    public static void PreStartMultiplayerOnServer()
    {
      MBCommon.CurrentGameType = GameNetwork.IsDedicatedServer ? MBCommon.GameType.MultiServer : MBCommon.GameType.MultiClientServer;
      GameNetwork.ClientPeerIndex = -1;
    }

    public static void StartMultiplayerOnServer(int port)
    {
      MBDebug.Print(nameof (StartMultiplayerOnServer));
      GameNetwork.PreStartMultiplayerOnServer();
      GameNetwork.InitializeServerSide(port);
      GameNetwork.StartMultiplayer();
    }

    [MBCallback]
    internal static bool HandleNetworkPacketAsServer(MBNetworkPeer networkPeer) => GameNetwork.HandleNetworkPacketAsServer(networkPeer.NetworkPeer);

    internal static bool HandleNetworkPacketAsServer(NetworkCommunicator networkPeer)
    {
      if (networkPeer == null)
      {
        MBDebug.Print("networkPeer == null");
        return false;
      }
      bool bufferReadValid = true;
      try
      {
        int num1 = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.NetworkComponentEventTypeFromClientCompressionInfo, ref bufferReadValid);
        if (bufferReadValid)
        {
          if (num1 >= 0 && num1 < GameNetwork._gameNetworkMessageIdsFromClient.Count)
          {
            GameNetworkMessage instance = Activator.CreateInstance(GameNetwork._gameNetworkMessageIdsFromClient[num1]) as GameNetworkMessage;
            instance.MessageId = num1;
            bufferReadValid = instance.Read();
            if (bufferReadValid)
            {
              List<object> objectList;
              if (GameNetwork._fromClientMessageHandlers.TryGetValue(num1, out objectList))
              {
                foreach (object obj in objectList)
                {
                  Delegate method = obj as Delegate;
                  int num2;
                  if (bufferReadValid)
                    num2 = (bool) method.DynamicInvokeWithLog((object) networkPeer, (object) instance) ? 1 : 0;
                  else
                    num2 = 0;
                  bufferReadValid = num2 != 0;
                  if (!bufferReadValid)
                    break;
                }
                if (objectList.Count == 0)
                  bufferReadValid = false;
              }
              else
                bufferReadValid = false;
            }
          }
          else
            bufferReadValid = false;
        }
      }
      catch (Exception ex)
      {
        MBDebug.Print("error " + ex.Message);
        return false;
      }
      return bufferReadValid;
    }

    [MBCallback]
    public static void HandleConsoleCommand(string command)
    {
      if (GameNetwork._handler == null)
        return;
      GameNetwork._handler.OnHandleConsoleCommand(command);
    }

    private static void InitializeServerSide(int port) => MBAPI.IMBNetwork.InitializeServerSide(port);

    private static void TerminateServerSide()
    {
      MBAPI.IMBNetwork.TerminateServerSide();
      if (GameNetwork.IsDedicatedServer)
        return;
      MBCommon.CurrentGameType = MBCommon.GameType.Single;
    }

    private static void PrepareNewUdpSession(int peerIndex, int sessionKey) => MBAPI.IMBNetwork.PrepareNewUdpSession(peerIndex, sessionKey);

    public static ICommunicator AddNewPlayerOnServer(
      PlayerConnectionInfo playerConnectionInfo,
      bool serverPeer,
      bool isAdmin)
    {
      bool flag = playerConnectionInfo == null;
      int num = flag ? MBAPI.IMBNetwork.AddNewBotOnServer() : MBAPI.IMBNetwork.AddNewPlayerOnServer(serverPeer);
      if (num < 0)
        return (ICommunicator) null;
      int sessionKey = 0;
      if (!serverPeer)
        sessionKey = GameNetwork.GetSessionKeyForPlayer();
      ICommunicator communicator = !flag ? (ICommunicator) NetworkCommunicator.CreateAsServer(playerConnectionInfo, num, isAdmin) : (ICommunicator) DummyCommunicator.CreateAsServer(num, "");
      MBNetwork.VirtualPlayers[communicator.VirtualPlayer.Index] = communicator.VirtualPlayer;
      if (!flag)
      {
        NetworkCommunicator networkCommunicator = communicator as NetworkCommunicator;
        if (serverPeer && GameNetwork.IsServer)
        {
          GameNetwork.ClientPeerIndex = num;
          GameNetwork.MyPeer = networkCommunicator;
        }
        networkCommunicator.SessionKey = sessionKey;
        networkCommunicator.SetServerPeer(serverPeer);
        GameNetwork.AddNetworkPeer(networkCommunicator);
        playerConnectionInfo.NetworkPeer = networkCommunicator;
        if (!serverPeer)
          GameNetwork.PrepareNewUdpSession(num, sessionKey);
        GameNetwork.BeginBroadcastModuleEvent();
        GameNetwork.WriteMessage((GameNetworkMessage) new CreatePlayer(networkCommunicator.Index, playerConnectionInfo.Name));
        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord | GameNetwork.EventBroadcastFlags.DontSendToPeers);
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        {
          if (networkPeer != networkCommunicator && networkPeer != GameNetwork.MyPeer)
          {
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new CreatePlayer(networkCommunicator.Index, playerConnectionInfo.Name));
            GameNetwork.EndModuleEventAsServer();
          }
          if (!serverPeer)
          {
            GameNetwork.BeginModuleEventAsServer(networkCommunicator);
            GameNetwork.WriteMessage((GameNetworkMessage) new CreatePlayer(networkPeer.Index, networkPeer.UserName));
            GameNetwork.EndModuleEventAsServer();
          }
        }
        foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
          networkHandler.HandleNewClientConnect(playerConnectionInfo);
      }
      return communicator;
    }

    public static GameNetwork.AddPlayersResult AddNewPlayersOnServer(
      PlayerConnectionInfo[] playerConnectionInfos,
      bool serverPeer)
    {
      bool flag = MBAPI.IMBNetwork.CanAddNewPlayersOnServer(playerConnectionInfos.Length);
      NetworkCommunicator[] networkCommunicatorArray = new NetworkCommunicator[playerConnectionInfos.Length];
      if (flag)
      {
        for (int index = 0; index < networkCommunicatorArray.Length; ++index)
        {
          ICommunicator communicator = GameNetwork.AddNewPlayerOnServer(playerConnectionInfos[index], serverPeer, false);
          networkCommunicatorArray[index] = communicator as NetworkCommunicator;
        }
      }
      return new GameNetwork.AddPlayersResult()
      {
        NetworkPeers = networkCommunicatorArray,
        Success = flag
      };
    }

    public static void ClientFinishedLoading(NetworkCommunicator networkPeer)
    {
      foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
        networkHandler.HandleEarlyNewClientAfterLoadingFinished(networkPeer);
      foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
        networkHandler.HandleNewClientAfterLoadingFinished(networkPeer);
      foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
        networkHandler.HandleLateNewClientAfterLoadingFinished(networkPeer);
      networkPeer.IsSynchronized = true;
      foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
        networkHandler.HandleNewClientAfterSynchronized(networkPeer);
      foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
        networkHandler.HandleLateNewClientAfterSynchronized(networkPeer);
    }

    public static void BeginModuleEventAsClient() => MBAPI.IMBNetwork.BeginModuleEventAsClient(true);

    public static void EndModuleEventAsClient() => MBAPI.IMBNetwork.EndModuleEventAsClient(true);

    public static void BeginModuleEventAsClientUnreliable() => MBAPI.IMBNetwork.BeginModuleEventAsClient(false);

    public static void EndModuleEventAsClientUnreliable() => MBAPI.IMBNetwork.EndModuleEventAsClient(false);

    public static void BeginModuleEventAsServer(NetworkCommunicator communicator) => GameNetwork.BeginModuleEventAsServer(communicator.VirtualPlayer);

    public static void BeginModuleEventAsServerUnreliable(NetworkCommunicator communicator) => GameNetwork.BeginModuleEventAsServerUnreliable(communicator.VirtualPlayer);

    public static void BeginModuleEventAsServer(VirtualPlayer peer) => MBAPI.IMBPeer.BeginModuleEvent(peer.Index, true);

    public static void EndModuleEventAsServer() => MBAPI.IMBPeer.EndModuleEvent(true);

    public static void BeginModuleEventAsServerUnreliable(VirtualPlayer peer) => MBAPI.IMBPeer.BeginModuleEvent(peer.Index, false);

    public static void EndModuleEventAsServerUnreliable() => MBAPI.IMBPeer.EndModuleEvent(false);

    public static void BeginBroadcastModuleEvent() => MBAPI.IMBNetwork.BeginBroadcastModuleEvent();

    public static void EndBroadcastModuleEvent(
      GameNetwork.EventBroadcastFlags broadcastFlags,
      NetworkCommunicator targetPlayer = null)
    {
      int targetPlayer1 = targetPlayer != null ? targetPlayer.Index : -1;
      MBAPI.IMBNetwork.EndBroadcastModuleEvent((int) broadcastFlags, targetPlayer1, true);
    }

    public static void EndBroadcastModuleEventUnreliable(
      GameNetwork.EventBroadcastFlags broadcastFlags,
      NetworkCommunicator targetPlayer = null)
    {
      int targetPlayer1 = targetPlayer != null ? targetPlayer.Index : -1;
      MBAPI.IMBNetwork.EndBroadcastModuleEvent((int) broadcastFlags, targetPlayer1, false);
    }

    public static void UnSynchronizeEveryone()
    {
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        networkPeer.IsSynchronized = false;
      foreach (IUdpNetworkHandler networkHandler in GameNetwork.NetworkHandlers)
        networkHandler.OnEveryoneUnSynchronized();
    }

    public static void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
      GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
      handlerRegisterer.Register<CreatePlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreatePlayer>(GameNetwork.HandleServerEventCreatePlayer));
      handlerRegisterer.Register<DeletePlayer>(new GameNetworkMessage.ServerMessageHandlerDelegate<DeletePlayer>(GameNetwork.HandleServerEventDeletePlayer));
    }

    public static void StartMultiplayerOnClient(
      string serverAddress,
      int port,
      int sessionKey,
      int playerIndex)
    {
      MBDebug.Print(nameof (StartMultiplayerOnClient));
      MBCommon.CurrentGameType = MBCommon.GameType.MultiClient;
      GameNetwork.ClientPeerIndex = playerIndex;
      GameNetwork.InitializeClientSide(serverAddress, port, sessionKey, playerIndex);
      GameNetwork.StartMultiplayer();
      GameNetwork.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
    }

    [MBCallback]
    internal static bool HandleNetworkPacketAsClient()
    {
      bool bufferReadValid = true;
      try
      {
        int num = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo, ref bufferReadValid);
        if (bufferReadValid && num >= 0 && num < GameNetwork._gameNetworkMessageIdsFromServer.Count)
        {
          GameNetworkMessage instance = Activator.CreateInstance(GameNetwork._gameNetworkMessageIdsFromServer[num]) as GameNetworkMessage;
          instance.MessageId = num;
          MBDebug.Print("Starting to read message: " + (object) instance.GetType(), debugFilter: 17179869184UL);
          bufferReadValid = instance.Read();
          if (bufferReadValid)
          {
            if (!NetworkMain.GameClient.IsInGame && !GameNetwork.IsReplay)
            {
              MBDebug.Print("ignoring post mission message: " + (object) instance.GetType(), debugFilter: 17179869184UL);
            }
            else
            {
              if ((instance.GetLogFilter() & MultiplayerMessageFilter.All) != MultiplayerMessageFilter.None)
              {
                if (GameNetworkMessage.IsClientMissionOver)
                  MBDebug.Print("WARNING: Entering message processing while client mission is over");
                MBDebug.Print("processing message " + (object) instance.GetType() + ": " + instance.GetLogFormat(), debugFilter: 17179869184UL);
              }
              List<object> objectList;
              if (GameNetwork._fromServerMessageHandlers.TryGetValue(num, out objectList))
              {
                foreach (object obj in objectList)
                {
                  try
                  {
                    (obj as Delegate).DynamicInvokeWithLog((object) instance);
                  }
                  catch
                  {
                    MBDebug.Print("Exception in handler of " + num.ToString(), debugFilter: 17179869184UL);
                    MBDebug.Print("Exception in handler of " + (object) instance.GetType(), color: Debug.DebugColor.Red, debugFilter: 17179869184UL);
                    throw;
                  }
                }
                if (objectList.Count == 0)
                  MBDebug.Print("No message handler found for " + (object) instance.GetType(), color: Debug.DebugColor.Red, debugFilter: 17179869184UL);
              }
              else
              {
                MBDebug.Print("Invalid messageId " + num.ToString(), debugFilter: 17179869184UL);
                MBDebug.Print("Invalid messageId " + (object) instance.GetType(), debugFilter: 17179869184UL);
              }
            }
          }
          else
            MBDebug.Print("Invalid message read for: " + (object) instance.GetType(), debugFilter: 17179869184UL);
        }
        else
          MBDebug.Print("Invalid message id read: " + (object) num, debugFilter: 17179869184UL);
      }
      catch (Exception ex)
      {
        MBDebug.Print("error " + ex.Message);
        MBDebug.Print(ex.StackTrace);
        throw;
      }
      return bufferReadValid;
    }

    private static int GetSessionKeyForPlayer() => new Random(DateTime.Now.Millisecond).Next(1, 4001);

    public static NetworkCommunicator HandleNewClientConnect(
      PlayerConnectionInfo playerConnectionInfo,
      bool isAdmin)
    {
      NetworkCommunicator networkPeer = GameNetwork.AddNewPlayerOnServer(playerConnectionInfo, false, isAdmin) as NetworkCommunicator;
      GameNetwork._handler.OnNewPlayerConnect(playerConnectionInfo, networkPeer);
      return networkPeer;
    }

    public static GameNetwork.AddPlayersResult HandleNewClientsConnect(
      PlayerConnectionInfo[] playerConnectionInfos,
      bool isAdmin)
    {
      GameNetwork.AddPlayersResult addPlayersResult = GameNetwork.AddNewPlayersOnServer(playerConnectionInfos, isAdmin);
      if (addPlayersResult.Success)
      {
        for (int index = 0; index < playerConnectionInfos.Length; ++index)
          GameNetwork._handler.OnNewPlayerConnect(playerConnectionInfos[index], addPlayersResult.NetworkPeers[index]);
      }
      return addPlayersResult;
    }

    public static void AddNetworkPeerToDisconnectAsServer(NetworkCommunicator networkPeer)
    {
      MBDebug.Print("adding peer to disconnect index:" + (object) networkPeer.Index, debugFilter: 17179869184UL);
      GameNetwork.AddPeerToDisconnect(networkPeer);
      GameNetwork.BeginModuleEventAsServer(networkPeer);
      GameNetwork.WriteMessage((GameNetworkMessage) new DeletePlayer(networkPeer.Index));
      GameNetwork.EndModuleEventAsServer();
    }

    private static void HandleServerEventCreatePlayer(CreatePlayer message)
    {
      int playerIndex = message.PlayerIndex;
      NetworkCommunicator asClient = NetworkCommunicator.CreateAsClient(message.PlayerName, playerIndex);
      if (playerIndex == GameNetwork.ClientPeerIndex)
        GameNetwork.MyPeer = asClient;
      MBNetwork.VirtualPlayers[asClient.VirtualPlayer.Index] = asClient.VirtualPlayer;
      GameNetwork.AddNetworkPeer(asClient);
    }

    private static void HandleServerEventDeletePlayer(DeletePlayer message)
    {
      NetworkCommunicator networkPeer1 = GameNetwork.NetworkPeers.FirstOrDefault<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (networkPeer => networkPeer.Index == message.PlayerIndex));
      if (networkPeer1 == null)
        return;
      GameNetwork.HandleRemovePlayer(networkPeer1);
    }

    public static void InitializeClientSide(
      string serverAddress,
      int port,
      int sessionKey,
      int playerIndex)
    {
      MBAPI.IMBNetwork.InitializeClientSide(serverAddress, port, sessionKey, playerIndex);
    }

    public static void TerminateClientSide()
    {
      MBAPI.IMBNetwork.TerminateClientSide();
      MBCommon.CurrentGameType = MBCommon.GameType.Single;
    }

    public static void DestroyComponent(UdpNetworkComponent udpNetworkComponent)
    {
      if (GameNetwork._destroyedComponents.Contains(udpNetworkComponent))
        return;
      GameNetwork._destroyedComponents.Add(udpNetworkComponent);
      GameNetwork._destroyedHandlers.Add((IUdpNetworkHandler) udpNetworkComponent);
    }

    public static T AddNetworkComponent<T>() where T : UdpNetworkComponent
    {
      T instance = (T) Activator.CreateInstance(typeof (T), new object[0]);
      GameNetwork.NetworkComponents.Add((UdpNetworkComponent) instance);
      GameNetwork.NetworkHandlers.Add((IUdpNetworkHandler) instance);
      return instance;
    }

    public static void AddNetworkHandler(IUdpNetworkHandler handler) => GameNetwork.NetworkHandlers.Add(handler);

    public static void RemoveNetworkHandler(IUdpNetworkHandler handler)
    {
      if (GameNetwork._destroyedHandlers.Contains(handler))
        return;
      GameNetwork._destroyedHandlers.Add(handler);
    }

    public static T GetNetworkComponent<T>() where T : UdpNetworkComponent
    {
      foreach (UdpNetworkComponent networkComponent in GameNetwork.NetworkComponents)
      {
        if (networkComponent is T obj1)
          return obj1;
      }
      return default (T);
    }

    public static UdpNetworkComponent GetNetworkComponentWithID(int id)
    {
      foreach (UdpNetworkComponent networkComponent in GameNetwork.NetworkComponents)
      {
        if (networkComponent.UniqueComponentID == id)
          return networkComponent;
      }
      return (UdpNetworkComponent) null;
    }

    public static List<UdpNetworkComponent> NetworkComponents { get; private set; }

    public static List<IUdpNetworkHandler> NetworkHandlers { get; private set; }

    public static void WriteMessage(GameNetworkMessage message)
    {
      System.Type type = message.GetType();
      message.MessageId = GameNetwork._gameNetworkMessageTypesAll[type];
      message.Write();
    }

    private static void AddServerMessageHandler<T>(
      GameNetworkMessage.ServerMessageHandlerDelegate<T> handler)
      where T : GameNetworkMessage
    {
      int key = GameNetwork._gameNetworkMessageTypesFromServer[typeof (T)];
      GameNetwork._fromServerMessageHandlers[key].Add((object) handler);
    }

    private static void AddClientMessageHandler<T>(
      GameNetworkMessage.ClientMessageHandlerDelegate<T> handler)
      where T : GameNetworkMessage
    {
      int key = GameNetwork._gameNetworkMessageTypesFromClient[typeof (T)];
      GameNetwork._fromClientMessageHandlers[key].Add((object) handler);
    }

    private static void RemoveServerMessageHandler<T>(
      GameNetworkMessage.ServerMessageHandlerDelegate<T> handler)
      where T : GameNetworkMessage
    {
      int key = GameNetwork._gameNetworkMessageTypesFromServer[typeof (T)];
      GameNetwork._fromServerMessageHandlers[key].Remove((object) handler);
    }

    private static void RemoveClientMessageHandler<T>(
      GameNetworkMessage.ClientMessageHandlerDelegate<T> handler)
      where T : GameNetworkMessage
    {
      int key = GameNetwork._gameNetworkMessageTypesFromClient[typeof (T)];
      GameNetwork._fromClientMessageHandlers[key].Remove((object) handler);
    }

    internal static void FindGameNetworkMessages()
    {
      MBDebug.Print("Searching Game NetworkMessages Methods", debugFilter: 17179869184UL);
      GameNetwork._fromClientMessageHandlers = new Dictionary<int, List<object>>();
      GameNetwork._fromServerMessageHandlers = new Dictionary<int, List<object>>();
      GameNetwork._gameNetworkMessageTypesAll = new Dictionary<System.Type, int>();
      GameNetwork._gameNetworkMessageTypesFromClient = new Dictionary<System.Type, int>();
      GameNetwork._gameNetworkMessageTypesFromServer = new Dictionary<System.Type, int>();
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
      List<System.Type> gameNetworkMessagesFromClient = new List<System.Type>();
      List<System.Type> gameNetworkMessagesFromServer = new List<System.Type>();
      foreach (Assembly assembly in assemblies)
      {
        if (GameNetwork.CheckAssemblyForNetworkMessage(assembly))
          GameNetwork.CollectGameNetworkMessagesFromAssembly(assembly, gameNetworkMessagesFromClient, gameNetworkMessagesFromServer);
      }
      gameNetworkMessagesFromClient.Sort((Comparison<System.Type>) ((s1, s2) => s1.FullName.CompareTo(s2.FullName)));
      gameNetworkMessagesFromServer.Sort((Comparison<System.Type>) ((s1, s2) => s1.FullName.CompareTo(s2.FullName)));
      GameNetwork._gameNetworkMessageIdsFromClient = new List<System.Type>(gameNetworkMessagesFromClient.Count);
      for (int index = 0; index < gameNetworkMessagesFromClient.Count; ++index)
      {
        System.Type key = gameNetworkMessagesFromClient[index];
        GameNetwork._gameNetworkMessageIdsFromClient.Add(key);
        GameNetwork._gameNetworkMessageTypesFromClient.Add(key, index);
        GameNetwork._gameNetworkMessageTypesAll.Add(key, index);
        GameNetwork._fromClientMessageHandlers.Add(index, new List<object>());
      }
      GameNetwork._gameNetworkMessageIdsFromServer = new List<System.Type>(gameNetworkMessagesFromServer.Count);
      for (int index = 0; index < gameNetworkMessagesFromServer.Count; ++index)
      {
        System.Type key = gameNetworkMessagesFromServer[index];
        GameNetwork._gameNetworkMessageIdsFromServer.Add(key);
        GameNetwork._gameNetworkMessageTypesFromServer.Add(key, index);
        GameNetwork._gameNetworkMessageTypesAll.Add(key, index);
        GameNetwork._fromServerMessageHandlers.Add(index, new List<object>());
      }
      CompressionBasic.NetworkComponentEventTypeFromClientCompressionInfo = new CompressionInfo.Integer(0, gameNetworkMessagesFromClient.Count - 1, true);
      CompressionBasic.NetworkComponentEventTypeFromServerCompressionInfo = new CompressionInfo.Integer(0, gameNetworkMessagesFromServer.Count - 1, true);
      MBDebug.Print("Found " + (object) gameNetworkMessagesFromClient.Count + " Client Game Network Messages", debugFilter: 17179869184UL);
      MBDebug.Print("Found " + (object) gameNetworkMessagesFromServer.Count + " Server Game Network Messages", debugFilter: 17179869184UL);
    }

    private static bool CheckAssemblyForNetworkMessage(Assembly assembly)
    {
      Assembly assembly1 = Assembly.GetAssembly(typeof (GameNetworkMessage));
      if (assembly == assembly1)
        return true;
      foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies())
      {
        if (referencedAssembly.FullName == assembly1.FullName)
          return true;
      }
      return false;
    }

    public static void IncreaseTotalUploadLimit(int value) => MBAPI.IMBNetwork.IncreaseTotalUploadLimit(value);

    public static void ResetDebugVariables() => MBAPI.IMBNetwork.ResetDebugVariables();

    public static void PrintDebugStats() => MBAPI.IMBNetwork.PrintDebugStats();

    public static float GetAveragePacketLossRatio() => MBAPI.IMBNetwork.GetAveragePacketLossRatio();

    public static void GetDebugUploadsInBits(
      ref GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct,
      ref GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct)
    {
      MBAPI.IMBNetwork.GetDebugUploadsInBits(ref networkStatisticsStruct, ref posStatisticsStruct);
    }

    public static void PrintReplicationTableStatistics() => MBAPI.IMBNetwork.PrintReplicationTableStatistics();

    public static void ClearReplicationTableStatistics() => MBAPI.IMBNetwork.ClearReplicationTableStatistics();

    public static void ResetDebugUploads() => MBAPI.IMBNetwork.ResetDebugUploads();

    public static void ResetMissionData() => MBAPI.IMBNetwork.ResetMissionData();

    private static void AddPeerToDisconnect(NetworkCommunicator networkPeer) => MBAPI.IMBNetwork.AddPeerToDisconnect(networkPeer.Index);

    public static void InitializeCompressionInfos()
    {
      CompressionBasic.ActionCodeCompressionInfo = new CompressionInfo.Integer(ActionIndexCache.act_none.Index, MBAnimation.GetNumActionCodes() - 1, true);
      CompressionBasic.AnimationIndexCompressionInfo = new CompressionInfo.Integer(0, MBAnimation.GetNumAnimations() - 1, true);
      CompressionBasic.CultureIndexCompressionInfo = new CompressionInfo.Integer(-1, MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>().Count - 1, true);
      CompressionBasic.SoundEventsCompressionInfo = new CompressionInfo.Integer(0, SoundEvent.GetTotalEventCount() - 1, true);
      CompressionMission.ActionSetCompressionInfo = new CompressionInfo.Integer(0, MBActionSet.GetNumberOfActionSets(), true);
      CompressionMission.MonsterUsageSetCompressionInfo = new CompressionInfo.Integer(0, MBActionSet.GetNumberOfMonsterUsageSets(), true);
    }

    [MBCallback]
    internal static void SyncRelevantGameOptionsToServer()
    {
      NetworkMessages.FromClient.SyncRelevantGameOptionsToServer gameOptionsToServer = new NetworkMessages.FromClient.SyncRelevantGameOptionsToServer();
      gameOptionsToServer.InitializeOptions();
      GameNetwork.BeginModuleEventAsClient();
      GameNetwork.WriteMessage((GameNetworkMessage) gameOptionsToServer);
      GameNetwork.EndModuleEventAsClient();
    }

    private static void CollectGameNetworkMessagesFromAssembly(
      Assembly assembly,
      List<System.Type> gameNetworkMessagesFromClient,
      List<System.Type> gameNetworkMessagesFromServer)
    {
      System.Type type1 = typeof (GameNetworkMessage);
      bool? nullable = new bool?();
      foreach (System.Type type2 in assembly.GetTypes())
      {
        if (type1.IsAssignableFrom(type2) && type2 != type1 && (type2.IsSealed && !(type2.GetConstructor(System.Type.EmptyTypes) == (ConstructorInfo) null)))
        {
          DefineGameNetworkMessageType customAttribute = type2.GetCustomAttribute<DefineGameNetworkMessageType>();
          if (customAttribute != null)
          {
            if ((!nullable.HasValue ? 1 : (!nullable.Value ? 1 : 0)) != 0)
            {
              nullable = new bool?(false);
              switch (customAttribute.SendType)
              {
                case GameNetworkMessageSendType.FromClient:
                  gameNetworkMessagesFromClient.Add(type2);
                  continue;
                case GameNetworkMessageSendType.FromServer:
                case GameNetworkMessageSendType.DebugFromServer:
                  gameNetworkMessagesFromServer.Add(type2);
                  continue;
                default:
                  continue;
              }
            }
          }
          else if (type2.GetCustomAttribute<DefineGameNetworkMessageTypeForMod>() != null && (!nullable.HasValue ? 1 : (nullable.Value ? 1 : 0)) != 0)
          {
            nullable = new bool?(true);
            switch (customAttribute.SendType)
            {
              case GameNetworkMessageSendType.FromClient:
                gameNetworkMessagesFromClient.Add(type2);
                continue;
              case GameNetworkMessageSendType.FromServer:
              case GameNetworkMessageSendType.DebugFromServer:
                gameNetworkMessagesFromServer.Add(type2);
                continue;
              default:
                continue;
            }
          }
        }
      }
    }

    public static NetworkCommunicator MyPeer { get; private set; }

    public static bool IsMyPeerReady => GameNetwork.MyPeer != null && GameNetwork.MyPeer.IsSynchronized;

    public class NetworkMessageHandlerRegisterer
    {
      private readonly GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode _registerMode;

      public NetworkMessageHandlerRegisterer(
        GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode definitionMode)
      {
        this._registerMode = definitionMode;
      }

      public void Register<T>(
        GameNetworkMessage.ServerMessageHandlerDelegate<T> handler)
        where T : GameNetworkMessage
      {
        if (this._registerMode == GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add)
          GameNetwork.AddServerMessageHandler<T>(handler);
        else
          GameNetwork.RemoveServerMessageHandler<T>(handler);
      }

      public void Register<T>(
        GameNetworkMessage.ClientMessageHandlerDelegate<T> handler)
        where T : GameNetworkMessage
      {
        if (this._registerMode == GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add)
          GameNetwork.AddClientMessageHandler<T>(handler);
        else
          GameNetwork.RemoveClientMessageHandler<T>(handler);
      }

      public enum RegisterMode
      {
        Add,
        Remove,
      }
    }

    public class NetworkMessageHandlerRegistererContainer
    {
      private List<Delegate> _fromClientHandlers;
      private List<Delegate> _fromServerHandlers;

      public NetworkMessageHandlerRegistererContainer()
      {
        this._fromClientHandlers = new List<Delegate>();
        this._fromServerHandlers = new List<Delegate>();
      }

      public void Register<T>(
        GameNetworkMessage.ServerMessageHandlerDelegate<T> handler)
        where T : GameNetworkMessage
      {
        this._fromServerHandlers.Add((Delegate) handler);
      }

      public void Register<T>(
        GameNetworkMessage.ClientMessageHandlerDelegate<T> handler)
        where T : GameNetworkMessage
      {
        this._fromClientHandlers.Add((Delegate) handler);
      }

      public void RegisterMessages()
      {
        if (this._fromServerHandlers.Count > 0)
        {
          foreach (Delegate fromServerHandler in this._fromServerHandlers)
          {
            System.Type genericTypeArgument = fromServerHandler.GetType().GenericTypeArguments[0];
            int key = GameNetwork._gameNetworkMessageTypesFromServer[genericTypeArgument];
            GameNetwork._fromServerMessageHandlers[key].Add((object) fromServerHandler);
          }
        }
        else
        {
          foreach (Delegate fromClientHandler in this._fromClientHandlers)
          {
            System.Type genericTypeArgument = fromClientHandler.GetType().GenericTypeArguments[0];
            int key = GameNetwork._gameNetworkMessageTypesFromClient[genericTypeArgument];
            GameNetwork._fromClientMessageHandlers[key].Add((object) fromClientHandler);
          }
        }
      }

      public void UnregisterMessages()
      {
        if (this._fromServerHandlers.Count > 0)
        {
          foreach (Delegate fromServerHandler in this._fromServerHandlers)
          {
            System.Type genericTypeArgument = fromServerHandler.GetType().GenericTypeArguments[0];
            int key = GameNetwork._gameNetworkMessageTypesFromServer[genericTypeArgument];
            GameNetwork._fromServerMessageHandlers[key].Remove((object) fromServerHandler);
          }
        }
        else
        {
          foreach (Delegate fromClientHandler in this._fromClientHandlers)
          {
            System.Type genericTypeArgument = fromClientHandler.GetType().GenericTypeArguments[0];
            int key = GameNetwork._gameNetworkMessageTypesFromClient[genericTypeArgument];
            GameNetwork._fromClientMessageHandlers[key].Remove((object) fromClientHandler);
          }
        }
      }
    }

    [Flags]
    public enum EventBroadcastFlags
    {
      None = 0,
      ExcludeTargetPlayer = 1,
      ExcludeNoBloodStainsOption = 2,
      ExcludeNoParticlesOption = 4,
      ExcludeNoSoundOption = 8,
      AddToMissionRecord = 16, // 0x00000010
      IncludeUnsynchronizedClients = 32, // 0x00000020
      ExcludeOtherTeamPlayers = 64, // 0x00000040
      ExcludePeerTeamPlayers = 128, // 0x00000080
      DontSendToPeers = 256, // 0x00000100
    }

    [EngineStruct("Debug_network_position_compression_statistics_struct")]
    public struct DebugNetworkPositionCompressionStatisticsStruct
    {
      public int totalPositionUpload;
      public int totalPositionPrecisionBitCount;
      public int totalPositionCoarseBitCountX;
      public int totalPositionCoarseBitCountY;
      public int totalPositionCoarseBitCountZ;
    }

    [EngineStruct("Debug_network_packet_statistics_struct")]
    public struct DebugNetworkPacketStatisticsStruct
    {
      public int TotalPackets;
      public int TotalUpload;
      public int TotalConstantsUpload;
      public int TotalReliableEventUpload;
      public int TotalReplicationUpload;
      public int TotalUnreliableEventUpload;
      public int TotalReplicationTableAdderCount;
      public int debug_total_replication_table_adder_bit_count;
      public int debug_total_replication_table_adder;
      public double debug_total_cell_priority;
      public double debug_total_cell_agent_priority;
      public double debug_total_cell_cell_priority;
      public int debug_total_cell_priority_checks;
      public int debug_total_sent_cell_count;
      public int debug_total_not_sent_cell_count;
      public int debug_total_replication_write_count;
      public int debug_cur_max_packet_size_in_bytes;
      public double average_ping_time;
      public double debug_average_dt_to_send_packet;
      public double time_out_period;
      public double pacing_rate;
      public double delivery_rate;
      public double round_trip_time;
      public int inflight_bit_count;
      public int is_congested;
      public int probe_bw_phase_index;
      public double lost_percent;
      public int lost_count;
      public int total_count_on_lost_check;
    }

    public struct AddPlayersResult
    {
      public bool Success;
      public NetworkCommunicator[] NetworkPeers;
    }
  }
}
