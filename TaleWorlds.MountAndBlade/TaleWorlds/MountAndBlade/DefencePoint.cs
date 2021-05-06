// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DefencePoint
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class DefencePoint : ScriptComponentBehaviour
  {
    private List<Agent> defenders = new List<Agent>();
    public BattleSideEnum Side;

    public void AddDefender(Agent defender) => this.defenders.Add(defender);

    public bool RemoveDefender(Agent defender) => this.defenders.Remove(defender);

    public IEnumerable<Agent> Defenders => (IEnumerable<Agent>) this.defenders;

    public void PurgeInactiveDefenders()
    {
      foreach (Agent defender in this.defenders.Where<Agent>((Func<Agent, bool>) (d => !d.IsActive())).ToList<Agent>())
        this.RemoveDefender(defender);
    }

    private MatrixFrame GetPosition(int index)
    {
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      Vec3 f = globalFrame.rotation.f;
      double num = (double) f.Normalize();
      globalFrame.origin -= f * (float) index * ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius) * 2f * 1.5f;
      return globalFrame;
    }

    public MatrixFrame GetVacantPosition(Agent a)
    {
      Mission current = Mission.Current;
      Team team = current.Teams.First<Team>((Func<Team, bool>) (t => t.Side == this.Side));
      for (int index = 0; index < 100; ++index)
      {
        MatrixFrame position = this.GetPosition(index);
        Agent closestAllyAgent = current.GetClosestAllyAgent(team, position.origin, ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius));
        if (closestAllyAgent == null || closestAllyAgent == a)
          return position;
      }
      return MatrixFrame.Identity;
    }

    public int CountOccupiedDefenderPositions()
    {
      Mission current = Mission.Current;
      Team team = current.Teams.First<Team>((Func<Team, bool>) (t => t.Side == this.Side));
      for (int index = 0; index < 100; ++index)
      {
        MatrixFrame position = this.GetPosition(index);
        if (current.GetClosestAllyAgent(team, position.origin, ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius)) == null)
          return index;
      }
      return 100;
    }
  }
}
