// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LobbyGameStateCustomGameClient
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
  public sealed class LobbyGameStateCustomGameClient : LobbyGameState
  {
    private LobbyClient _gameClient;
    private string _address;
    private int _port;
    private int _peerIndex;
    private int _sessionKey;
    private Timer _inactivityTimer;
    private static readonly float InactivityThreshold = 2f;

    public void SetStartingParameters(
      LobbyClient gameClient,
      string address,
      int port,
      int peerIndex,
      int sessionKey)
    {
      this._gameClient = gameClient;
      this._address = address;
      this._port = port;
      this._peerIndex = peerIndex;
      this._sessionKey = sessionKey;
    }

    protected override void OnActivate()
    {
      base.OnActivate();
      this._inactivityTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Application), LobbyGameStateCustomGameClient.InactivityThreshold);
      if (this._gameClient == null || !this._gameClient.AtLobby && this._gameClient.Connected)
        return;
      this.GameStateManager.PopState();
    }

    protected override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!GameNetwork.IsClient || !this._inactivityTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Application)))
        return;
      this._gameClient.IsInCriticalState = MBAPI.IMBNetwork.ElapsedTimeSinceLastUdpPacketArrived() > (double) LobbyGameStateCustomGameClient.InactivityThreshold;
    }

    protected override void StartMultiplayer()
    {
      MBDebug.Print("CUSTOM GAME SERVER ADDRESS: " + this._address);
      GameNetwork.StartMultiplayerOnClient(this._address, this._port, this._sessionKey, this._peerIndex);
      BannerlordNetwork.StartMultiplayerLobbyMission(LobbyMissionType.Custom);
    }
  }
}
