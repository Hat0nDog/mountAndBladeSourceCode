// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ScoreboardFactory
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;

namespace TaleWorlds.MountAndBlade
{
  public static class ScoreboardFactory
  {
    private static Dictionary<string, IScoreboardData> _registeredScoreboards;

    public static void Register(string gameMode, IScoreboardData scoreboardData)
    {
      if (ScoreboardFactory._registeredScoreboards == null)
        ScoreboardFactory._registeredScoreboards = new Dictionary<string, IScoreboardData>();
      ScoreboardFactory._registeredScoreboards.Add(gameMode, scoreboardData);
    }

    public static IScoreboardData Get(string gameMode) => ScoreboardFactory._registeredScoreboards[gameMode];

    public static void Clear()
    {
      ScoreboardFactory._registeredScoreboards.Clear();
      ScoreboardFactory._registeredScoreboards = (Dictionary<string, IScoreboardData>) null;
    }
  }
}
