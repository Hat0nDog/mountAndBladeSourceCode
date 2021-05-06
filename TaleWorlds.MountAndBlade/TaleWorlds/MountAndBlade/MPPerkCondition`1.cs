// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MPPerkCondition`1
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MPPerkCondition<T> : MPPerkCondition where T : MissionMultiplayerGameModeBase
  {
    protected T GameModeInstance
    {
      get
      {
        Mission current = Mission.Current;
        return current == null ? default (T) : current.GetMissionBehaviour<T>();
      }
    }

    protected override bool IsGameModesValid(List<string> gameModes)
    {
      if (typeof (MissionMultiplayerFlagDomination).IsAssignableFrom(typeof (T)))
      {
        string str1 = MissionLobbyComponent.MultiplayerGameType.Skirmish.ToString();
        string str2 = MissionLobbyComponent.MultiplayerGameType.Captain.ToString();
        foreach (string gameMode in gameModes)
        {
          if (!gameMode.Equals(str1, StringComparison.InvariantCultureIgnoreCase) && !gameMode.Equals(str2, StringComparison.InvariantCultureIgnoreCase))
            return false;
        }
        return true;
      }
      if (typeof (MissionMultiplayerTeamDeathmatch).IsAssignableFrom(typeof (T)))
      {
        string str = MissionLobbyComponent.MultiplayerGameType.TeamDeathmatch.ToString();
        foreach (string gameMode in gameModes)
        {
          if (!gameMode.Equals(str, StringComparison.InvariantCultureIgnoreCase))
            return false;
        }
        return true;
      }
      if (!typeof (MissionMultiplayerSiege).IsAssignableFrom(typeof (T)))
        return false;
      string str3 = MissionLobbyComponent.MultiplayerGameType.Siege.ToString();
      foreach (string gameMode in gameModes)
      {
        if (!gameMode.Equals(str3, StringComparison.InvariantCultureIgnoreCase))
          return false;
      }
      return true;
    }
  }
}
