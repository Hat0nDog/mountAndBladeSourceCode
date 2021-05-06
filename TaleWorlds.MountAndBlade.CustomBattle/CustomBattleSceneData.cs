// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattleSceneData
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  public struct CustomBattleSceneData
  {
    public string SceneID { get; private set; }

    public TextObject Name { get; private set; }

    public TerrainType Terrain { get; private set; }

    public List<TerrainType> TerrainTypes { get; private set; }

    public ForestDensity ForestDensity { get; private set; }

    public bool IsSiegeMap { get; private set; }

    public bool IsVillageMap { get; private set; }

    public CustomBattleSceneData(
      string sceneID,
      TextObject name,
      TerrainType terrain,
      List<TerrainType> terrainTypes,
      ForestDensity forestDensity,
      bool isSiegeMap,
      bool isVillageMap)
    {
      this.SceneID = sceneID;
      this.Name = name;
      this.Terrain = terrain;
      this.TerrainTypes = terrainTypes;
      this.ForestDensity = forestDensity;
      this.IsSiegeMap = isSiegeMap;
      this.IsVillageMap = isVillageMap;
    }
  }
}
