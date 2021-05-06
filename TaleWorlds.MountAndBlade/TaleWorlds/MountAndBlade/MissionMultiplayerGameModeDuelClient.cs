// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionMultiplayerGameModeDuelClient
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  internal class MissionMultiplayerGameModeDuelClient : MissionMultiplayerGameModeBaseClient
  {
    public override bool IsGameModeUsingGold => false;

    public override bool IsGameModeTactical => false;

    public override bool IsGameModeUsingRoundCountdown => false;

    public override MissionLobbyComponent.MultiplayerGameType GameType => MissionLobbyComponent.MultiplayerGameType.Duel;

    public override int GetGoldAmount() => 0;

    public override void OnGoldAmountChangedForRepresentative(
      MissionRepresentativeBase representative,
      int goldAmount)
    {
    }
  }
}
