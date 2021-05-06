// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FlagDominationSpawnFrameBehaviour
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class FlagDominationSpawnFrameBehaviour : SpawnFrameBehaviourBase
  {
    private IEnumerable<GameEntity>[] _spawnPointsByTeam;
    private IEnumerable<GameEntity>[] _spawnZonesByTeam;

    public override void Initialize()
    {
      base.Initialize();
      this._spawnPointsByTeam = new IEnumerable<GameEntity>[2];
      this._spawnZonesByTeam = new IEnumerable<GameEntity>[2];
      this._spawnPointsByTeam[1] = (IEnumerable<GameEntity>) this.SpawnPoints.Where<GameEntity>((Func<GameEntity, bool>) (x => x.HasTag("attacker"))).ToList<GameEntity>();
      this._spawnPointsByTeam[0] = (IEnumerable<GameEntity>) this.SpawnPoints.Where<GameEntity>((Func<GameEntity, bool>) (x => x.HasTag("defender"))).ToList<GameEntity>();
      this._spawnZonesByTeam[1] = (IEnumerable<GameEntity>) this._spawnPointsByTeam[1].Select<GameEntity, GameEntity>((Func<GameEntity, GameEntity>) (sp => sp.Parent)).Distinct<GameEntity>().Where<GameEntity>((Func<GameEntity, bool>) (sz => (NativeObject) sz != (NativeObject) null)).ToList<GameEntity>();
      this._spawnZonesByTeam[0] = (IEnumerable<GameEntity>) this._spawnPointsByTeam[0].Select<GameEntity, GameEntity>((Func<GameEntity, GameEntity>) (sp => sp.Parent)).Distinct<GameEntity>().Where<GameEntity>((Func<GameEntity, bool>) (sz => (NativeObject) sz != (NativeObject) null)).ToList<GameEntity>();
    }

    public override MatrixFrame GetSpawnFrame(
      Team team,
      bool hasMount,
      bool isInitialSpawn)
    {
      GameEntity bestZone = this.GetBestZone(team, isInitialSpawn);
      return this.GetBestSpawnPoint(!((NativeObject) bestZone != (NativeObject) null) ? this._spawnPointsByTeam[(int) team.Side].ToList<GameEntity>() : this._spawnPointsByTeam[(int) team.Side].Where<GameEntity>((Func<GameEntity, bool>) (sp => (NativeObject) sp.Parent == (NativeObject) bestZone)).ToList<GameEntity>(), hasMount);
    }

    private GameEntity GetBestZone(Team team, bool isInitialSpawn)
    {
      if (!this._spawnZonesByTeam[(int) team.Side].Any<GameEntity>())
        return (GameEntity) null;
      if (isInitialSpawn)
        return this._spawnZonesByTeam[(int) team.Side].Single<GameEntity>((Func<GameEntity, bool>) (sz => sz.HasTag("starting")));
      List<GameEntity> list = this._spawnZonesByTeam[(int) team.Side].Where<GameEntity>((Func<GameEntity, bool>) (sz => !sz.HasTag("starting"))).ToList<GameEntity>();
      if (!list.Any<GameEntity>())
        return (GameEntity) null;
      float[] numArray = new float[list.Count];
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (component?.Team != null && component.Team.Side != BattleSideEnum.None && (component.ControlledAgent != null && component.ControlledAgent.IsActive()))
        {
          for (int index = 0; index < list.Count; ++index)
          {
            Vec3 globalPosition = list[index].GlobalPosition;
            Vec3 position;
            if (component.Team != team)
            {
              ref float local = ref numArray[index];
              double num1 = (double) local;
              position = component.ControlledAgent.Position;
              double num2 = 1.0 / (9.99999974737875E-05 + (double) position.Distance(globalPosition)) * 1.0;
              local = (float) (num1 - num2);
            }
            else
            {
              ref float local = ref numArray[index];
              double num1 = (double) local;
              position = component.ControlledAgent.Position;
              double num2 = 1.0 / (9.99999974737875E-05 + (double) position.Distance(globalPosition)) * 1.5;
              local = (float) (num1 + num2);
            }
          }
        }
      }
      int index1 = -1;
      for (int index2 = 0; index2 < numArray.Length; ++index2)
      {
        if (index1 < 0 || (double) numArray[index2] > (double) numArray[index1])
          index1 = index2;
      }
      return list[index1];
    }

    private MatrixFrame GetBestSpawnPoint(List<GameEntity> spawnPointList, bool hasMount)
    {
      float num1 = float.MinValue;
      int index1 = -1;
      for (int index2 = 0; index2 < spawnPointList.Count; ++index2)
      {
        float num2 = MBRandom.RandomFloat * 0.2f;
        Mission current = Mission.Current;
        Vec3 vec3 = spawnPointList[index2].GlobalPosition;
        Vec2 asVec2 = vec3.AsVec2;
        foreach (Agent agent in current.GetAgentsInRange(asVec2, 2f))
        {
          vec3 = agent.Position - spawnPointList[index2].GlobalPosition;
          float lengthSquared = vec3.LengthSquared;
          if ((double) lengthSquared < 4.0)
          {
            float num3 = MathF.Sqrt(lengthSquared);
            num2 -= (float) ((2.0 - (double) num3) * 5.0);
          }
        }
        if (hasMount && spawnPointList[index2].HasTag("exclude_mounted"))
          num2 -= 100f;
        if ((double) num2 > (double) num1)
        {
          num1 = num2;
          index1 = index2;
        }
      }
      MatrixFrame globalFrame = spawnPointList[index1].GetGlobalFrame();
      globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
      return globalFrame;
    }
  }
}
