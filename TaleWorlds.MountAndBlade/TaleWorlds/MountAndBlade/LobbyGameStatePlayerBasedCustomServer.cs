// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LobbyGameStatePlayerBasedCustomServer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Threading.Tasks;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
  public sealed class LobbyGameStatePlayerBasedCustomServer : LobbyGameState
  {
    private LobbyClient _gameClient;

    public void SetStartingParameters(LobbyGameClientHandler lobbyGameClientHandler) => this._gameClient = lobbyGameClientHandler.GameClient;

    protected override void OnActivate()
    {
      base.OnActivate();
      if (this._gameClient == null || !this._gameClient.AtLobby && this._gameClient.Connected)
        return;
      this.GameStateManager.PopState();
    }

    protected override void StartMultiplayer() => this.HandleServerStartMultiplayer();

    private async void HandleServerStartMultiplayer()
    {
      GameNetwork.PreStartMultiplayerOnServer();
      BannerlordNetwork.StartMultiplayerLobbyMission(LobbyMissionType.Custom);
      if (Module.CurrentModule.StartMultiplayerGame(this._gameClient.CustomGameType, this._gameClient.CustomGameScene))
        ;
      while (Mission.Current == null || Mission.Current.CurrentState != Mission.State.Continuing)
        await Task.Delay(1);
      GameNetwork.StartMultiplayerOnServer(9999);
      if (!this._gameClient.IsInGame)
        return;
      BannerlordNetwork.CreateServerPeer();
      MBDebug.Print("Server: I finished loading and I am now visible to clients in the server list.", debugFilter: 17179869184UL);
      if (GameNetwork.IsDedicatedServer)
        return;
      GameNetwork.ClientFinishedLoading(GameNetwork.MyPeer);
    }
  }
}
