// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GameNetworkHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class GameNetworkHandler : IGameNetworkHandler
  {
    void IGameNetworkHandler.OnNewPlayerConnect(
      PlayerConnectionInfo playerConnectionInfo,
      NetworkCommunicator networkPeer)
    {
      if (networkPeer == null)
        return;
      GameManagerBase.Current.OnPlayerConnect(networkPeer.VirtualPlayer);
    }

    void IGameNetworkHandler.OnInitialize() => MultiplayerGameTypes.Initialize();

    void IGameNetworkHandler.OnDisconnectedFromServer()
    {
      if (Mission.Current == null)
        return;
      Mission.Current.EndMission();
    }

    void IGameNetworkHandler.OnPlayerDisconnectedFromServer(
      NetworkCommunicator networkPeer)
    {
      GameManagerBase.Current.OnPlayerDisconnect(networkPeer.VirtualPlayer);
    }

    void IGameNetworkHandler.OnStartMultiplayer()
    {
      GameNetwork.AddNetworkComponent<BaseNetworkComponent>();
      GameNetwork.AddNetworkComponent<LobbyNetworkComponent>();
      GameNetwork.AddNetworkComponent<NetworkStatusReplicationComponent>();
      GameManagerBase.Current.OnGameNetworkBegin();
    }

    void IGameNetworkHandler.OnEndMultiplayer()
    {
      GameManagerBase.Current.OnGameNetworkEnd();
      GameNetwork.DestroyComponent((UdpNetworkComponent) GameNetwork.GetNetworkComponent<LobbyNetworkComponent>());
      GameNetwork.DestroyComponent((UdpNetworkComponent) GameNetwork.GetNetworkComponent<NetworkStatusReplicationComponent>());
      GameNetwork.DestroyComponent((UdpNetworkComponent) GameNetwork.GetNetworkComponent<BaseNetworkComponent>());
    }

    void IGameNetworkHandler.OnHandleConsoleCommand(string command) => DedicatedServerConsoleCommandManager.HandleConsoleCommand(command);
  }
}
