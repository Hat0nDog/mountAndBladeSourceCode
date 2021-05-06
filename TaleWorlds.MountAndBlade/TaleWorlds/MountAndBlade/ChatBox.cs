// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ChatBox
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.PlayerServices;

namespace TaleWorlds.MountAndBlade
{
  public class ChatBox : GameHandler
  {
    private static ChatBox _chatBox;
    private List<PlayerId> _mutedPlayers = new List<PlayerId>();

    public bool NetworkReady
    {
      get
      {
        if (GameNetwork.IsClient || GameNetwork.IsServer)
          return true;
        return NetworkMain.GameClient != null && NetworkMain.GameClient.Connected;
      }
    }

    protected override void OnGameStart() => ChatBox._chatBox = this;

    public override void OnBeforeSave()
    {
    }

    public override void OnAfterSave()
    {
    }

    protected override void OnGameEnd() => ChatBox._chatBox = (ChatBox) null;

    public void SendMessageToAll(string message)
    {
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.PlayerMessageAll(message));
        GameNetwork.EndModuleEventAsClient();
      }
      else
      {
        if (!GameNetwork.IsServer)
          return;
        this.ServerPrepareAndSendMessage(GameNetwork.MyPeer, false, message);
      }
    }

    public void SendMessageToTeam(string message)
    {
      if (GameNetwork.IsClient)
      {
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.PlayerMessageTeam(message));
        GameNetwork.EndModuleEventAsClient();
      }
      else
      {
        if (!GameNetwork.IsServer)
          return;
        this.ServerPrepareAndSendMessage(GameNetwork.MyPeer, true, message);
      }
    }

    public void SendMessageToWhisperTarget(
      string message,
      string platformName,
      string whisperTarget)
    {
      if (NetworkMain.GameClient == null || !NetworkMain.GameClient.Connected)
        return;
      NetworkMain.GameClient.SendWhisper(whisperTarget, message);
      if (this.WhisperMessageSent == null)
        return;
      this.WhisperMessageSent(message, whisperTarget);
    }

    private void OnServerMessage(string message)
    {
      if (this.ServerMessage == null)
        return;
      this.ServerMessage(message);
    }

    protected override void OnGameNetworkBegin() => this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);

    private void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
    {
      GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
      if (GameNetwork.IsClient)
      {
        handlerRegisterer.Register<NetworkMessages.FromServer.PlayerMessageTeam>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.PlayerMessageTeam>(this.HandleServerEventPlayerMessageTeam));
        handlerRegisterer.Register<NetworkMessages.FromServer.PlayerMessageAll>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.PlayerMessageAll>(this.HandleServerEventPlayerMessageAll));
        handlerRegisterer.Register<NetworkMessages.FromServer.ServerMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<NetworkMessages.FromServer.ServerMessage>(this.HandleServerEventServerMessage));
      }
      else
      {
        if (!GameNetwork.IsServer)
          return;
        handlerRegisterer.Register<NetworkMessages.FromClient.PlayerMessageAll>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.PlayerMessageAll>(this.HandleClientEventPlayerMessageAll));
        handlerRegisterer.Register<NetworkMessages.FromClient.PlayerMessageTeam>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.PlayerMessageTeam>(this.HandleClientEventPlayerMessageTeam));
      }
    }

    protected override void OnGameNetworkEnd()
    {
      base.OnGameNetworkEnd();
      this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
    }

    private void HandleServerEventPlayerMessageAll(NetworkMessages.FromServer.PlayerMessageAll message)
    {
      if (this._mutedPlayers.Contains(message.Player.VirtualPlayer.Id))
        return;
      this.OnPlayerMessageReceived(message.Player, message.Message, false);
    }

    private void HandleServerEventPlayerMessageTeam(NetworkMessages.FromServer.PlayerMessageTeam message)
    {
      if (this._mutedPlayers.Contains(message.Player.VirtualPlayer.Id))
        return;
      this.OnPlayerMessageReceived(message.Player, message.Message, true);
    }

    private void HandleServerEventServerMessage(NetworkMessages.FromServer.ServerMessage message) => this.OnServerMessage(message.IsMessageTextId ? GameTexts.FindText(message.Message).ToString() : message.Message);

    private bool HandleClientEventPlayerMessageAll(
      NetworkCommunicator networkPeer,
      NetworkMessages.FromClient.PlayerMessageAll message)
    {
      return this.ServerPrepareAndSendMessage(networkPeer, false, message.Message);
    }

    private bool HandleClientEventPlayerMessageTeam(
      NetworkCommunicator networkPeer,
      NetworkMessages.FromClient.PlayerMessageTeam message)
    {
      return this.ServerPrepareAndSendMessage(networkPeer, true, message.Message);
    }

    public static void ServerSendServerMessageToEveryone(string message)
    {
      ChatBox._chatBox.OnServerMessage(message);
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.ServerMessage(message));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    private bool ServerPrepareAndSendMessage(
      NetworkCommunicator fromPeer,
      bool toTeamOnly,
      string message)
    {
      if (fromPeer.IsMuted)
      {
        GameNetwork.BeginModuleEventAsServer(fromPeer);
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.ServerMessage("str_multiplayer_muted_message", true));
        GameNetwork.EndModuleEventAsServer();
        return true;
      }
      if (!GameNetwork.IsDedicatedServer && fromPeer != GameNetwork.MyPeer && !this._mutedPlayers.Contains(fromPeer.VirtualPlayer.Id))
      {
        MissionPeer component1 = GameNetwork.MyPeer.GetComponent<MissionPeer>();
        if (component1 == null)
          return false;
        bool flag;
        if (toTeamOnly)
        {
          if (component1 == null)
            return false;
          MissionPeer component2 = fromPeer.GetComponent<MissionPeer>();
          if (component2 == null)
            return false;
          flag = component1.Team == component2.Team;
        }
        else
          flag = true;
        if (flag)
          this.OnPlayerMessageReceived(fromPeer, message, toTeamOnly);
      }
      if (toTeamOnly)
        ChatBox.ServerSendMessageToTeam(fromPeer, message);
      else
        ChatBox.ServerSendMessageToEveryone(fromPeer, message);
      return true;
    }

    private static void ServerSendMessageToTeam(NetworkCommunicator networkPeer, string message)
    {
      MissionPeer missionPeer = networkPeer.GetComponent<MissionPeer>();
      if (missionPeer.Team == null)
        return;
      foreach (NetworkCommunicator communicator in GameNetwork.NetworkPeers.Where<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => !x.IsServerPeer && x.IsSynchronized && x != networkPeer && x.GetComponent<MissionPeer>().Team == missionPeer.Team)))
      {
        GameNetwork.BeginModuleEventAsServer(communicator);
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.PlayerMessageTeam(networkPeer, message));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    private static void ServerSendMessageToEveryone(NetworkCommunicator networkPeer, string message)
    {
      foreach (NetworkCommunicator communicator in GameNetwork.NetworkPeers.Where<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => !x.IsServerPeer && x.IsSynchronized && x != networkPeer)))
      {
        GameNetwork.BeginModuleEventAsServer(communicator);
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromServer.PlayerMessageAll(networkPeer, message));
        GameNetwork.EndModuleEventAsServer();
      }
    }

    internal static void AddWhisperMessage(string fromUserName, string messageBody) => ChatBox._chatBox.OnWhisperMessageReceived(fromUserName, messageBody);

    internal static void AddErrorWhisperMessage(string toUserName) => ChatBox._chatBox.OnErrorWhisperMessageReceived(toUserName);

    private void OnWhisperMessageReceived(string fromUserName, string messageBody)
    {
      if (this.WhisperMessageReceived == null)
        return;
      this.WhisperMessageReceived(fromUserName, messageBody);
    }

    private void OnErrorWhisperMessageReceived(string toUserName)
    {
      if (this.ErrorWhisperMessageReceived == null)
        return;
      this.ErrorWhisperMessageReceived(toUserName);
    }

    private void OnPlayerMessageReceived(
      NetworkCommunicator networkPeer,
      string message,
      bool toTeamOnly)
    {
      if (this.PlayerMessageReceived == null)
        return;
      this.PlayerMessageReceived(networkPeer, message, toTeamOnly);
    }

    public void OnPlayerMuted(PlayerId mutedPlayer)
    {
      this._mutedPlayers.Add(mutedPlayer);
      PlayerMutedDelegate playerMuteChanged = this.OnPlayerMuteChanged;
      if (playerMuteChanged == null)
        return;
      playerMuteChanged(mutedPlayer, true);
    }

    public void OnPlayerUnmuted(PlayerId unmutedPlayer)
    {
      this._mutedPlayers.Remove(unmutedPlayer);
      PlayerMutedDelegate playerMuteChanged = this.OnPlayerMuteChanged;
      if (playerMuteChanged == null)
        return;
      playerMuteChanged(unmutedPlayer, false);
    }

    public bool IsPlayerMuted(PlayerId player) => this._mutedPlayers.Contains(player);

    public event PlayerMessageReceivedDelegate PlayerMessageReceived;

    public event WhisperMessageSentDelegate WhisperMessageSent;

    public event WhisperMessageReceivedDelegate WhisperMessageReceived;

    public event ErrorWhisperMessageReceivedDelegate ErrorWhisperMessageReceived;

    public event ServerMessageDelegate ServerMessage;

    public event PlayerMutedDelegate OnPlayerMuteChanged;

    protected override void OnTick()
    {
    }
  }
}
