// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BannerlordNetwork
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
  public static class BannerlordNetwork
  {
    public const int DefaultPort = 9999;

    private static PlayerConnectionInfo CreateServerPeerConnectionInfo()
    {
      PlayerConnectionInfo playerConnectionInfo = new PlayerConnectionInfo();
      LobbyClient gameClient = NetworkMain.GameClient;
      playerConnectionInfo.AddParameter("PlayerData", (object) gameClient.PlayerData);
      playerConnectionInfo.Name = gameClient.Name;
      return playerConnectionInfo;
    }

    public static void CreateServerPeer()
    {
      if (MBCommon.CurrentGameType != MBCommon.GameType.MultiClientServer)
        return;
      GameNetwork.AddNewPlayerOnServer(BannerlordNetwork.CreateServerPeerConnectionInfo(), true, true);
    }

    public static void StartMultiplayerLobbyMission(LobbyMissionType lobbyMissionType) => BannerlordNetwork.LobbyMissionType = lobbyMissionType;

    public static void EndMultiplayerLobbyMission()
    {
      if (!(Game.Current.GameStateManager.ActiveState is MissionState activeState) || activeState.CurrentMission == null || activeState.CurrentMission.MissionEnded())
        return;
      MBDebug.Print("Starting to clean up the current mission now.", debugFilter: 17179869184UL);
      activeState.CurrentMission.EndMission();
    }

    public static LobbyMissionType LobbyMissionType { get; private set; }
  }
}
