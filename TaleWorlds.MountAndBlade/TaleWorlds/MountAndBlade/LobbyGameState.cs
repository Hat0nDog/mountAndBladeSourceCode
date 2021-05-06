// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LobbyGameState
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public abstract class LobbyGameState : GameState, IUdpNetworkHandler
  {
    public override bool IsMusicMenuState => true;

    protected override void OnInitialize()
    {
      base.OnInitialize();
      this.StartMultiplayer();
      GameNetwork.AddNetworkHandler((IUdpNetworkHandler) this);
    }

    protected override void OnActivate() => base.OnActivate();

    protected override void OnFinalize()
    {
      base.OnFinalize();
      GameNetwork.RemoveNetworkHandler((IUdpNetworkHandler) this);
      GameNetwork.EndMultiplayer();
    }

    void IUdpNetworkHandler.OnUdpNetworkHandlerClose()
    {
    }

    void IUdpNetworkHandler.OnUdpNetworkHandlerTick(float dt)
    {
    }

    void IUdpNetworkHandler.HandleNewClientConnect(
      PlayerConnectionInfo clientConnectionInfo)
    {
    }

    void IUdpNetworkHandler.HandleEarlyNewClientAfterLoadingFinished(
      NetworkCommunicator networkPeer)
    {
    }

    void IUdpNetworkHandler.HandleNewClientAfterLoadingFinished(
      NetworkCommunicator networkPeer)
    {
    }

    void IUdpNetworkHandler.HandleLateNewClientAfterLoadingFinished(
      NetworkCommunicator networkPeer)
    {
    }

    void IUdpNetworkHandler.HandleNewClientAfterSynchronized(
      NetworkCommunicator networkPeer)
    {
    }

    void IUdpNetworkHandler.HandleLateNewClientAfterSynchronized(
      NetworkCommunicator networkPeer)
    {
    }

    void IUdpNetworkHandler.HandleEarlyPlayerDisconnect(
      NetworkCommunicator networkPeer)
    {
    }

    void IUdpNetworkHandler.HandlePlayerDisconnect(NetworkCommunicator networkPeer)
    {
    }

    void IUdpNetworkHandler.OnEveryoneUnSynchronized()
    {
    }

    protected abstract void StartMultiplayer();
  }
}
