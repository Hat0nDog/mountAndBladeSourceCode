// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LobbyNetworkComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace TaleWorlds.MountAndBlade
{
  public class LobbyNetworkComponent : UdpNetworkComponent
  {
    public const int MaxForcedAvatarIndex = 100;
    public const int ComponentUniqueID = 4;

    protected override void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
      if (GameNetwork.IsClient)
      {
        registerer.Register<InitializeLobbyPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<InitializeLobbyPeer>(this.HandleServerEventInitializeLobbyPeer));
      }
      else
      {
        if (!GameNetwork.IsReplay)
          return;
        registerer.Register<InitializeLobbyPeer>(new GameNetworkMessage.ServerMessageHandlerDelegate<InitializeLobbyPeer>(this.HandleServerEventInitializeLobbyPeer));
      }
    }

    private void HandleServerEventInitializeLobbyPeer(InitializeLobbyPeer message)
    {
      NetworkCommunicator peer = message.Peer;
      VirtualPlayer virtualPlayer = peer.VirtualPlayer;
      virtualPlayer.Id = message.ProvidedId;
      virtualPlayer.BannerCode = message.BannerCode;
      virtualPlayer.BodyProperties = message.BodyProperties;
      virtualPlayer.ChosenBadgeIndex = message.ChosenBadgeIndex;
      peer.ForcedAvatarIndex = message.ForcedAvatarIndex;
    }

    public override void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
      PlayerData parameter = networkPeer.PlayerConnectionInfo.GetParameter<PlayerData>("PlayerData");
      VirtualPlayer virtualPlayer = networkPeer.VirtualPlayer;
      virtualPlayer.Id = parameter.PlayerId;
      virtualPlayer.BannerCode = parameter.Bannercode;
      virtualPlayer.BodyProperties = parameter.BodyProperties;
      virtualPlayer.IsFemale = parameter.IsFemale;
      virtualPlayer.ChosenBadgeIndex = parameter.ShownBadgeIndex;
      networkPeer.IsMuted = parameter.IsMuted;
    }

    public override void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
      VirtualPlayer virtualPlayer1 = networkPeer.VirtualPlayer;
      GameNetwork.BeginBroadcastModuleEvent();
      GameNetwork.WriteMessage((GameNetworkMessage) new InitializeLobbyPeer(networkPeer, virtualPlayer1.Id, virtualPlayer1.BannerCode, virtualPlayer1.BodyProperties, virtualPlayer1.ChosenBadgeIndex, -1));
      GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.AddToMissionRecord | GameNetwork.EventBroadcastFlags.DontSendToPeers);
      foreach (NetworkCommunicator networkPeer1 in GameNetwork.NetworkPeers)
      {
        if (networkPeer1.IsSynchronized || networkPeer1 == networkPeer)
        {
          if (networkPeer1 != networkPeer && networkPeer1 != GameNetwork.MyPeer)
          {
            GameNetwork.BeginModuleEventAsServer(networkPeer1);
            GameNetwork.WriteMessage((GameNetworkMessage) new InitializeLobbyPeer(networkPeer, virtualPlayer1.Id, virtualPlayer1.BannerCode, virtualPlayer1.BodyProperties, virtualPlayer1.ChosenBadgeIndex, -1));
            GameNetwork.EndModuleEventAsServer();
          }
          VirtualPlayer virtualPlayer2 = networkPeer1.VirtualPlayer;
          if (!networkPeer.IsServerPeer)
          {
            GameNetwork.BeginModuleEventAsServer(networkPeer);
            GameNetwork.WriteMessage((GameNetworkMessage) new InitializeLobbyPeer(networkPeer1, virtualPlayer2.Id, virtualPlayer2.BannerCode, virtualPlayer2.BodyProperties, virtualPlayer2.ChosenBadgeIndex, -1));
            GameNetwork.EndModuleEventAsServer();
          }
        }
      }
    }

    public override void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
    }

    public override void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
    {
    }

    public override void OnUdpNetworkHandlerTick(float dt)
    {
    }

    public override int UniqueComponentID => 4;
  }
}
