// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerAdminComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerAdminComponent : MissionNetwork
  {
    private static MultiplayerAdminComponent _multiplayerAdminComponent;

    public event MultiplayerAdminComponent.OnSelectPlayerToKickDelegate OnSelectPlayerToKick;

    public event MultiplayerAdminComponent.OnShowAdminMenuDelegate OnShowAdminMenu;

    public void OnApplySettings()
    {
      MultiplayerOptions.Instance.InitializeOptionsFromUi();
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new MultiplayerOptionsImmediate());
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
    }

    public void KickPlayer(bool banPlayer)
    {
      if (this.OnSelectPlayerToKick == null)
        return;
      this.OnSelectPlayerToKick(false);
    }

    public void ShowAdminMenu()
    {
      if (this.OnShowAdminMenu == null)
        return;
      this.OnShowAdminMenu();
    }

    public void KickPlayer(NetworkCommunicator peerToKick, bool banPlayer)
    {
      if (GameNetwork.IsServer || GameNetwork.IsDedicatedServer)
      {
        if (peerToKick.IsMine)
          return;
        MissionPeer component = peerToKick.GetComponent<MissionPeer>();
        if (component == null)
          return;
        if (banPlayer)
        {
          if (GameNetwork.IsClient)
            BannedPlayerManagerCustomGameClient.AddBannedPlayer(component.Peer.Id, GameNetwork.IsDedicatedServer ? -1 : Environment.TickCount + 600000);
          else if (GameNetwork.IsDedicatedServer)
            BannedPlayerManagerCustomGameServer.AddBannedPlayer(component.Peer.Id, component.GetNetworkPeer().UserName, GameNetwork.IsDedicatedServer ? -1 : Environment.TickCount + 600000);
        }
        if (GameNetwork.IsDedicatedServer)
          throw new NotImplementedException();
        NetworkMain.GameClient.KickPlayer(component.Peer.Id, banPlayer);
      }
      else
      {
        if (!GameNetwork.IsClient)
          return;
        GameNetwork.BeginModuleEventAsClient();
        GameNetwork.WriteMessage((GameNetworkMessage) new NetworkMessages.FromClient.KickPlayer(peerToKick, banPlayer));
        GameNetwork.EndModuleEventAsClient();
      }
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      MultiplayerAdminComponent._multiplayerAdminComponent = this;
    }

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (!GameNetwork.IsServer)
        return;
      registerer.Register<NetworkMessages.FromClient.KickPlayer>(new GameNetworkMessage.ClientMessageHandlerDelegate<NetworkMessages.FromClient.KickPlayer>(this.HandleClientEventKickPlayer));
    }

    private bool HandleClientEventKickPlayer(NetworkCommunicator peer, NetworkMessages.FromClient.KickPlayer message)
    {
      if (peer.IsAdmin)
        this.KickPlayer(message.PlayerPeer, message.BanPlayer);
      return true;
    }

    public override void OnRemoveBehaviour()
    {
      MultiplayerAdminComponent._multiplayerAdminComponent = (MultiplayerAdminComponent) null;
      base.OnRemoveBehaviour();
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("help", "mp_host")]
    public static string MPHostHelp(List<string> strings)
    {
      if (MultiplayerAdminComponent._multiplayerAdminComponent == null)
        return "Failed: MultiplayerAdminComponent has not been created.";
      return !GameNetwork.IsServerOrRecorder ? "Failed: Only the host can use mp_host commands." : "" + "mp_host.restart_game : Restarts the game.\n" + "mp_host.kick_player : Kicks the given player.\n";
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("kick_player", "mp_host")]
    public static string MPHostKickPlayer(List<string> strings)
    {
      if (MultiplayerAdminComponent._multiplayerAdminComponent == null)
        return "Failed: MultiplayerAdminComponent has not been created.";
      if (!GameNetwork.IsServerOrRecorder)
        return "Failed: Only the host can use mp_host commands.";
      if (strings.Count != 1)
        return "Failed: Input is incorrect.";
      string str = strings[0];
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        if (networkPeer.UserName == str)
        {
          DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
          disconnectInfo.Type = DisconnectType.KickedByHost;
          networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", (object) disconnectInfo);
          GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
          return "Player " + str + " has been kicked from the server.";
        }
      }
      return str + " could not be found.";
    }

    public delegate void OnSelectPlayerToKickDelegate(bool banPlayer);

    public delegate void OnShowAdminMenuDelegate();
  }
}
