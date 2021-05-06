// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SpawnFrameBehaviourBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class SpawnFrameBehaviourBase
  {
    private const string ExcludeMountedTag = "exclude_mounted";
    private const string ExcludeFootmenTag = "exclude_footmen";
    protected const string SpawnPointTag = "spawnpoint";
    public IEnumerable<GameEntity> SpawnPoints;

    public virtual void Initialize() => this.SpawnPoints = Mission.Current.Scene.FindEntitiesWithTag("spawnpoint");

    public abstract MatrixFrame GetSpawnFrame(
      Team team,
      bool hasMount,
      bool isInitialSpawn);

    protected MatrixFrame GetSpawnFrameFromSpawnPoints(
      IList<GameEntity> spawnPointsList,
      Team team,
      bool hasMount)
    {
      float num1 = float.MinValue;
      int index1 = -1;
      for (int index2 = 0; index2 < spawnPointsList.Count; ++index2)
      {
        float num2 = MBRandom.RandomFloat * 0.2f;
        float num3 = 0.0f;
        if (hasMount && spawnPointsList[index2].HasTag("exclude_mounted"))
          num2 -= 1000f;
        if (!hasMount && spawnPointsList[index2].HasTag("exclude_footmen"))
          num2 -= 1000f;
        SpawnFrameBehaviourBase.WeightCache decreasingCache1 = SpawnFrameBehaviourBase.WeightCache.CreateDecreasingCache();
        SpawnFrameBehaviourBase.WeightCache decreasingCache2 = SpawnFrameBehaviourBase.WeightCache.CreateDecreasingCache();
        foreach (Agent agent in (IEnumerable<Agent>) Mission.Current.Agents)
        {
          if (!agent.IsMount)
          {
            float length = (agent.Position - spawnPointsList[index2].GlobalPosition).Length;
            float num4 = team == null || agent.Team.IsEnemyOf(team) ? (float) (-Math.Tanh(Math.Pow(3.75 - (double) length * 0.125, 2.0)) * 2.0 + 3.09999990463257 - (double) length * 0.0125000001862645 - 1.0 / (((double) length + 9.99999974737875E-05) * 0.0500000007450581)) : (float) (-Math.Tanh(Math.Pow(1.79999995231628 - (double) length * 0.100000001490116, 2.0)) + 1.70000004768372 - (double) length * 0.00999999977648258 - 1.0 / (((double) length + 9.99999974737875E-05) * 0.100000001490116));
            if ((double) num4 >= 0.0)
            {
              float valueDifference;
              if (decreasingCache1.CheckAndInsertNewValueIfLower(num4, out valueDifference))
                num3 -= valueDifference;
            }
            else
            {
              float valueDifference;
              if (decreasingCache2.CheckAndInsertNewValueIfLower(num4, out valueDifference))
                num3 -= valueDifference;
            }
          }
        }
        if ((double) num3 > 0.0)
          num3 /= (float) Mission.Current.Agents.Count;
        float num5 = num2 + num3;
        if ((double) num5 > (double) num1)
        {
          num1 = num5;
          index1 = index2;
        }
      }
      MatrixFrame globalFrame = spawnPointsList[index1].GetGlobalFrame();
      globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
      return globalFrame;
    }

    private struct WeightCache
    {
      private const int Length = 3;
      private float _value1;
      private float _value2;
      private float _value3;

      private float this[int index]
      {
        get
        {
          switch (index)
          {
            case 0:
              return this._value1;
            case 1:
              return this._value2;
            case 2:
              return this._value3;
            default:
              throw new ArgumentOutOfRangeException();
          }
        }
        set
        {
          switch (index)
          {
            case 0:
              this._value1 = value;
              break;
            case 1:
              this._value2 = value;
              break;
            case 2:
              this._value3 = value;
              break;
          }
        }
      }

      private WeightCache(float value1, float value2, float value3)
      {
        this._value1 = value1;
        this._value2 = value2;
        this._value3 = value3;
      }

      public static SpawnFrameBehaviourBase.WeightCache CreateDecreasingCache() => new SpawnFrameBehaviourBase.WeightCache(float.NaN, float.NaN, float.NaN);

      public bool CheckAndInsertNewValueIfLower(float value, out float valueDifference)
      {
        int index1 = 0;
        for (int index2 = 1; index2 < 3; ++index2)
        {
          if ((double) this[index2] > (double) this[index1])
            index1 = index2;
        }
        if (float.IsNaN(this[index1]) || (double) value < (double) this[index1])
        {
          valueDifference = float.IsNaN(this[index1]) ? Math.Abs(value) : this[index1] - value;
          this[index1] = value;
          return true;
        }
        valueDifference = float.NaN;
        return false;
      }
    }
  }
}
