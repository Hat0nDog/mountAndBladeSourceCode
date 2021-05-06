// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionBattleSchedulerClientComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
  public class MissionBattleSchedulerClientComponent : MissionLobbyComponent
  {
    public override void QuitMission()
    {
      base.QuitMission();
      if (this.CurrentMultiplayerState == MissionLobbyComponent.MultiplayerGameState.Ending || !NetworkMain.GameClient.LoggedIn || NetworkMain.GameClient.CurrentState != LobbyClient.State.AtBattle)
        return;
      NetworkMain.GameClient.QuitFromMatchmakerGame();
    }
  }
}
