// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TeamAIGeneral
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class TeamAIGeneral : TeamAIComponent
  {
    private int _numberOfEnemiesInShootRange;
    private int _numberOfEnemiesCloseToAttack;

    public TeamAIGeneral(
      Mission currentMission,
      Team currentTeam,
      float thinkTimerTime = 10f,
      float applyTimerTime = 1f)
      : base(currentMission, currentTeam, thinkTimerTime, applyTimerTime)
    {
    }

    private void UpdateVariables()
    {
      TeamQuerySystem querySystem = this.Team.QuerySystem;
      this._numberOfEnemiesInShootRange = 0;
      this._numberOfEnemiesCloseToAttack = 0;
      Vec2 averagePosition = querySystem.AveragePosition;
      foreach (Agent agent in (IEnumerable<Agent>) this.Mission.Agents)
      {
        if (!agent.IsMount && agent.Team.IsValid && agent.Team.IsEnemyOf(this.Team))
        {
          double num = (double) agent.Position.DistanceSquared(new Vec3(averagePosition.x, averagePosition.y));
          if (num < 40000.0)
            ++this._numberOfEnemiesInShootRange;
          if (num < 1600.0)
            ++this._numberOfEnemiesCloseToAttack;
        }
      }
    }

    protected override void DebugTick(float dt)
    {
    }
  }
}
