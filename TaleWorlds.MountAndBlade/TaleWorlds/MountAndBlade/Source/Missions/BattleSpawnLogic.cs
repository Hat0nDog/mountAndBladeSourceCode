// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Missions.BattleSpawnLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Source.Missions
{
  public class BattleSpawnLogic : MissionLogic
  {
    public const string BattleTag = "battle_set";
    public const string SallyOutTag = "sally_out_set";
    public const string ReliefForceAttackTag = "relief_force_attack_set";
    private const string SpawnPointSetCommonTag = "spawnpoint_set";
    private readonly string _selectedSpawnPointSetTag;
    private bool _isScenePrepared;

    public BattleSpawnLogic(string selectedSpawnPointSetTag) => this._selectedSpawnPointSetTag = selectedSpawnPointSetTag;

    public override void OnPreMissionTick(float dt)
    {
      if (this._isScenePrepared)
        return;
      GameEntity entityWithTag = this.Mission.Scene.FindEntityWithTag(this._selectedSpawnPointSetTag);
      if ((NativeObject) entityWithTag != (NativeObject) null)
      {
        List<GameEntity> list = this.Mission.Scene.FindEntitiesWithTag("spawnpoint_set").ToList<GameEntity>();
        list.Remove(entityWithTag);
        foreach (GameEntity gameEntity in list)
          gameEntity.Remove(76);
      }
      this._isScenePrepared = true;
    }
  }
}
