// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IGameNetworkHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public interface IGameNetworkHandler
  {
    void OnNewPlayerConnect(
      PlayerConnectionInfo playerConnectionInfo,
      NetworkCommunicator networkPeer);

    void OnInitialize();

    void OnPlayerDisconnectedFromServer(NetworkCommunicator peer);

    void OnDisconnectedFromServer();

    void OnStartMultiplayer();

    void OnEndMultiplayer();

    void OnHandleConsoleCommand(string command);
  }
}
