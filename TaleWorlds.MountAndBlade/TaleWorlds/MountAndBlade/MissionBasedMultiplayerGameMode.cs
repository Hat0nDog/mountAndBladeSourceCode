// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionBasedMultiplayerGameMode
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
  public class MissionBasedMultiplayerGameMode : MultiplayerGameMode
  {
    public MissionBasedMultiplayerGameMode(string name)
      : base(name)
    {
    }

    public override void JoinCustomGame(JoinGameData joinGameData)
    {
      LobbyGameStateCustomGameClient state = Game.Current.GameStateManager.CreateState<LobbyGameStateCustomGameClient>();
      state.SetStartingParameters(NetworkMain.GameClient, joinGameData.GameServerProperties.Address, joinGameData.GameServerProperties.Port, joinGameData.PeerIndex, joinGameData.SessionKey);
      Game.Current.GameStateManager.PushState((GameState) state);
    }

    public override void StartMultiplayerGame(string scene)
    {
      if (this.Name == "FreeForAll")
        MultiplayerMissions.OpenFreeForAllMission(scene);
      else if (this.Name == "TeamDeathmatch")
        MultiplayerMissions.OpenTeamDeathmatchMission(scene);
      else if (this.Name == "Duel")
        MultiplayerMissions.OpenDuelMission(scene);
      else if (this.Name == "Siege")
        MultiplayerMissions.OpenSiegeMission(scene);
      else if (this.Name == "Captain")
      {
        MultiplayerMissions.OpenCaptainMission(scene);
      }
      else
      {
        if (!(this.Name == "Skirmish"))
          return;
        MultiplayerMissions.OpenSkirmishMission(scene);
      }
    }
  }
}
