// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.UdpNetworkComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public abstract class UdpNetworkComponent : IUdpNetworkHandler
  {
    private GameNetwork.NetworkMessageHandlerRegistererContainer _missionNetworkMessageHandlerRegisterer;

    protected UdpNetworkComponent()
    {
      this._missionNetworkMessageHandlerRegisterer = new GameNetwork.NetworkMessageHandlerRegistererContainer();
      this.AddRemoveMessageHandlers(this._missionNetworkMessageHandlerRegisterer);
      this._missionNetworkMessageHandlerRegisterer.RegisterMessages();
    }

    protected virtual void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
    {
    }

    public virtual void OnUdpNetworkHandlerClose()
    {
      this._missionNetworkMessageHandlerRegisterer?.UnregisterMessages();
      GameNetwork.NetworkComponents.Remove(this);
    }

    public virtual void OnUdpNetworkHandlerTick(float dt)
    {
    }

    public virtual void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo)
    {
    }

    public virtual void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
    }

    public virtual void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
    }

    public virtual void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer)
    {
    }

    public virtual void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
    }

    public virtual void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer)
    {
    }

    public virtual void OnEveryoneUnSynchronized()
    {
    }

    public void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer)
    {
    }

    public virtual void HandlePlayerDisconnect(NetworkCommunicator networkPeer)
    {
    }

    public abstract int UniqueComponentID { get; }
  }
}
