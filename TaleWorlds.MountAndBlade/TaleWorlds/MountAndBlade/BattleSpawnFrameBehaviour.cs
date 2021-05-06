// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BattleSpawnFrameBehaviour
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
  public class BattleSpawnFrameBehaviour : SpawnFrameBehaviourBase
  {
    private IEnumerable<GameEntity> _spawnPointsOfAttackers;
    private IEnumerable<GameEntity> _spawnPointsOfDefenders;

    public override void Initialize()
    {
      base.Initialize();
      this._spawnPointsOfAttackers = (IEnumerable<GameEntity>) this.SpawnPoints.Where<GameEntity>((Func<GameEntity, bool>) (x => x.HasTag("attacker"))).ToList<GameEntity>();
      this._spawnPointsOfDefenders = (IEnumerable<GameEntity>) this.SpawnPoints.Where<GameEntity>((Func<GameEntity, bool>) (x => x.HasTag("defender"))).ToList<GameEntity>();
    }

    public override MatrixFrame GetSpawnFrame(
      Team team,
      bool hasMount,
      bool isInitialSpawn)
    {
      List<GameEntity> list = (team == Mission.Current.AttackerTeam ? this._spawnPointsOfAttackers : this._spawnPointsOfDefenders).ToList<GameEntity>();
      float num1 = float.MinValue;
      int index1 = -1;
      for (int index2 = 0; index2 < list.Count; ++index2)
      {
        float num2 = MBRandom.RandomFloat * 0.2f;
        Mission current = Mission.Current;
        Vec3 vec3 = list[index2].GlobalPosition;
        Vec2 asVec2 = vec3.AsVec2;
        foreach (Agent agent in current.GetAgentsInRange(asVec2, 2f))
        {
          vec3 = agent.Position - list[index2].GlobalPosition;
          float lengthSquared = vec3.LengthSquared;
          if ((double) lengthSquared < 4.0)
          {
            float num3 = MathF.Sqrt(lengthSquared);
            num2 -= (float) ((2.0 - (double) num3) * 5.0);
          }
        }
        if (hasMount && list[index2].HasTag("exclude_mounted"))
          num2 -= 100f;
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          index1 = index2;
        }
      }
      return list[index1].GetGlobalFrame();
    }
  }
}
