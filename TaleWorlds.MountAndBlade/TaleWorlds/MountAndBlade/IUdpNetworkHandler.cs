// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IUdpNetworkHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public interface IUdpNetworkHandler
  {
    void OnUdpNetworkHandlerClose();

    void OnUdpNetworkHandlerTick(float dt);

    void HandleNewClientConnect(PlayerConnectionInfo clientConnectionInfo);

    void HandleEarlyNewClientAfterLoadingFinished(NetworkCommunicator networkPeer);

    void HandleNewClientAfterLoadingFinished(NetworkCommunicator networkPeer);

    void HandleLateNewClientAfterLoadingFinished(NetworkCommunicator networkPeer);

    void HandleNewClientAfterSynchronized(NetworkCommunicator networkPeer);

    void HandleLateNewClientAfterSynchronized(NetworkCommunicator networkPeer);

    void HandleEarlyPlayerDisconnect(NetworkCommunicator networkPeer);

    void HandlePlayerDisconnect(NetworkCommunicator networkPeer);

    void OnEveryoneUnSynchronized();
  }
}
