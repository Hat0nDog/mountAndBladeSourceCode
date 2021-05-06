// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionCustomGameClientComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
  public class MissionCustomGameClientComponent : MissionLobbyComponent
  {
    private LobbyClient _lobbyClient;

    public override void OnBehaviourInitialize()
    {
      base.OnBehaviourInitialize();
      this._lobbyClient = NetworkMain.GameClient;
    }

    public override void QuitMission()
    {
      base.QuitMission();
      if (GameNetwork.IsServer)
      {
        if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending || !this._lobbyClient.LoggedIn || this._lobbyClient.CurrentState != LobbyClient.State.HostingCustomGame)
          return;
        this._lobbyClient.EndCustomGame();
      }
      else
      {
        if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending || !this._lobbyClient.LoggedIn || this._lobbyClient.CurrentState != LobbyClient.State.InCustomGame)
          return;
        this._lobbyClient.QuitFromCustomGame();
      }
    }
  }
}
