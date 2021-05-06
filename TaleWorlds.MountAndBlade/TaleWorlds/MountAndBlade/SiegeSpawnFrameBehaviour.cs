// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeSpawnFrameBehaviour
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
  public class SiegeSpawnFrameBehaviour : SpawnFrameBehaviourBase
  {
    public const string SpawnZoneTagAffix = "sp_zone_";
    public const string SpawnZoneEnableTagAffix = "enable_";
    public const string SpawnZoneDisableTagAffix = "disable_";
    public const int StartingActiveSpawnZoneIndex = 0;
    private IEnumerable<GameEntity>[] _spawnPointsByTeam;
    private IEnumerable<GameEntity>[] _spawnZonesByTeam;
    private int _activeSpawnZoneIndex;

    public override void Initialize()
    {
      base.Initialize();
      this._spawnPointsByTeam = new IEnumerable<GameEntity>[2];
      this._spawnZonesByTeam = new IEnumerable<GameEntity>[2];
      this._spawnPointsByTeam[1] = (IEnumerable<GameEntity>) this.SpawnPoints.Where<GameEntity>((Func<GameEntity, bool>) (x => x.HasTag("attacker"))).ToList<GameEntity>();
      this._spawnPointsByTeam[0] = (IEnumerable<GameEntity>) this.SpawnPoints.Where<GameEntity>((Func<GameEntity, bool>) (x => x.HasTag("defender"))).ToList<GameEntity>();
      this._spawnZonesByTeam[1] = (IEnumerable<GameEntity>) this._spawnPointsByTeam[1].Select<GameEntity, GameEntity>((Func<GameEntity, GameEntity>) (sp => sp.Parent)).Distinct<GameEntity>().Where<GameEntity>((Func<GameEntity, bool>) (sz => (NativeObject) sz != (NativeObject) null)).ToList<GameEntity>();
      this._spawnZonesByTeam[0] = (IEnumerable<GameEntity>) this._spawnPointsByTeam[0].Select<GameEntity, GameEntity>((Func<GameEntity, GameEntity>) (sp => sp.Parent)).Distinct<GameEntity>().Where<GameEntity>((Func<GameEntity, bool>) (sz => (NativeObject) sz != (NativeObject) null)).ToList<GameEntity>();
      this._activeSpawnZoneIndex = 0;
    }

    public override MatrixFrame GetSpawnFrame(
      Team team,
      bool hasMount,
      bool isInitialSpawn)
    {
      List<GameEntity> gameEntityList = new List<GameEntity>();
      GameEntity gameEntity = this._spawnZonesByTeam[(int) team.Side].First<GameEntity>((Func<GameEntity, bool>) (sz => sz.HasTag(string.Format("{0}{1}", (object) "sp_zone_", (object) this._activeSpawnZoneIndex))));
      gameEntityList.AddRange(gameEntity.GetChildren().Where<GameEntity>((Func<GameEntity, bool>) (sp => sp.HasTag("spawnpoint"))));
      return this.GetSpawnFrameFromSpawnPoints((IList<GameEntity>) gameEntityList, team, hasMount);
    }

    public void OnFlagDeactivated(FlagCapturePoint flag) => ++this._activeSpawnZoneIndex;
  }
}
