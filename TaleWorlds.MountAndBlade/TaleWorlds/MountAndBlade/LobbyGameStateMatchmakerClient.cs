// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LobbyGameStateMatchmakerClient
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
  public sealed class LobbyGameStateMatchmakerClient : LobbyGameState
  {
    private LobbyClient _gameClient;
    private int _playerIndex;
    private int _sessionKey;
    private string _address;
    private int _assignedPort;
    private string _multiplayerGameType;
    private string _scene;
    private LobbyGameClientHandler _lobbyGameClientHandler;

    public void SetStartingParameters(
      LobbyGameClientHandler lobbyGameClientHandler,
      int playerIndex,
      int sessionKey,
      string address,
      int assignedPort,
      string multiplayerGameType,
      string scene)
    {
      this._lobbyGameClientHandler = lobbyGameClientHandler;
      this._gameClient = lobbyGameClientHandler.GameClient;
      this._playerIndex = playerIndex;
      this._sessionKey = sessionKey;
      this._address = address;
      this._assignedPort = assignedPort;
      this._multiplayerGameType = multiplayerGameType;
      this._scene = scene;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      if (this._gameClient == null || this._gameClient.CurrentState != LobbyClient.State.AtLobby && this._gameClient.CurrentState != LobbyClient.State.QuittingFromBattle && this._gameClient.Connected)
        return;
      this.GameStateManager.PopState();
    }

    protected override void StartMultiplayer()
    {
      GameNetwork.StartMultiplayerOnClient(this._address, this._assignedPort, this._sessionKey, this._playerIndex);
      BannerlordNetwork.StartMultiplayerLobbyMission(LobbyMissionType.Matchmaker);
      Module.CurrentModule.StartMultiplayerGame(this._multiplayerGameType, this._scene);
    }
  }
}
