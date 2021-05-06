// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionNetwork
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public abstract class MissionNetwork : MissionLogic, IUdpNetworkHandler
  {
    private GameNetwork.NetworkMessageHandlerRegistererContainer _missionNetworkMessageHandlerRegisterer;

    public override void OnAfterMissionCreated()
    {
      this._missionNetworkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegistererContainer();
      this.AddRemoveMessageHandlers(this._missionNetworkMessageHandlerRegisterer);
      this._missionNetworkMessageHandlerRegisterer.RegisterMessages();
    }

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      GameNetwork.AddNetworkHandler((IUdpNetworkHandler) this);
    }

    public override void OnRemoveBehaviour()
    {
      GameNetwork.RemoveNetworkHandler((IUdpNetworkHandler) this);
      base.OnRemoveBehaviour();
    }

    protected virtual void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
    }

    void IUdpNetworkHandler.OnUdpNetworkHandlerTick(float dt) => this.OnUdpNetworkHandlerTick();

    void IUdpNetworkHandler.OnUdpNetworkHandlerClose()
    {
      this.OnUdpNetworkHandlerClose();
      this._missionNetworkMessageHandlerRegisterer?.UnregisterMessages();
    }

    void IUdpNetworkHandler.HandleNewClientConnect(
      PlayerConnectionInfo clientConnectionInfo)
    {
      this.HandleNewClientConnect(clientConnectionInfo);
    }

    void IUdpNetworkHandler.HandleEarlyNewClientAfterLoadingFinished(
      NetworkCommunicator networkPeer)
    {
      this.HandleEarlyNewClientAfterLoadingFinished(networkPeer);
    }

    void IUdpNetworkHandler.HandleNewClientAfterLoadingFinished(
      NetworkCommunicator networkPeer)
    {
      this.HandleNewClientAfterLoadingFinished(networkPeer);
    }

    void IUdpNetworkHandler.HandleLateNewClientAfterLoadingFinished(
      NetworkCommunicator networkPeer)
    {
      this.HandleLateNewClientAfterLoadingFinished(networkPeer);
    }

    void IUdpNetworkHandler.HandleNewClientAfterSynchronized(
      NetworkCommunicator networkPeer)
    {
      this.HandleNewClientAfterSynchronized(networkPeer);
    }

    void IUdpNetworkHandler.HandleLateNewClientAfterSynchronized(
      NetworkCommunicator networkPeer)
    {
      this.HandleLateNewClientAfterSynchronized(networkPeer);
    }

    void IUdpNetworkHandler.HandleEarlyPlayerDisconnect(
      NetworkCommunicator networkPeer)
    {
      this.HandleEarlyPlayerDisconnect(networkPeer);
    }

    void IUdpNetworkHandler.HandlePlayerDisconnect(NetworkCommunicator networkPeer) => this.HandlePlayerDisconnect(networkPeer);

    void IUdpNetworkHandler.OnEveryoneUnSynchronized()
    {
    }

    protected virtual void OnUdpNetworkHandlerTick()
    {
    }

    protected virtual void OnUdpNetworkHandlerClose()
    {
    }

    protected virtual void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
    {
    }

    protected virtual void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
    }

    protected virtual void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
    }

    protected virtual void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
    }

    protected virtual void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
    }

    protected virtual void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
    }

    protected virtual void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
    {
    }

    protected virtual void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
    {
    }
  }
}
