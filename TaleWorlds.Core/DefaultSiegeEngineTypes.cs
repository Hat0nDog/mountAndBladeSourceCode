﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.DefaultSiegeEngineTypes
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.ObjectSystem;
using TaleWorlds.SaveSystem;

namespace TaleWorlds.Core
{
  [SaveableClass(10038)]
  public class DefaultSiegeEngineTypes
  {
    internal SiegeEngineType SiegeEngineTypePreparations;
    internal SiegeEngineType SiegeEngineTypeLadder;
    internal SiegeEngineType SiegeEngineTypeBallista;
    internal SiegeEngineType SiegeEngineTypeFireBallista;
    internal SiegeEngineType SiegeEngineTypeRam;
    internal SiegeEngineType SiegeEngineTypeImprovedRam;
    internal SiegeEngineType SiegeEngineTypeSiegeTower;
    internal SiegeEngineType SiegeEngineTypeHeavySiegeTower;
    internal SiegeEngineType SiegeEngineTypeCatapult;
    internal SiegeEngineType SiegeEngineTypeFireCatapult;
    internal SiegeEngineType SiegeEngineTypeOnager;
    internal SiegeEngineType SiegeEngineTypeFireOnager;
    internal SiegeEngineType SiegeEngineTypeBricole;
    internal SiegeEngineType SiegeEngineTypeTrebuchet;
    internal SiegeEngineType SiegeEngineTypeFireTrebuchet;

    internal static void AutoGeneratedStaticCollectObjectsDefaultSiegeEngineTypes(
      object o,
      List<object> collectedObjects)
    {
      ((DefaultSiegeEngineTypes) o).AutoGeneratedInstanceCollectObjects(collectedObjects);
    }

    protected virtual void AutoGeneratedInstanceCollectObjects(List<object> collectedObjects)
    {
    }

    public static DefaultSiegeEngineTypes Instance => Game.Current.DefaultSiegeEngineTypes;

    public static SiegeEngineType Preparations => DefaultSiegeEngineTypes.Instance.SiegeEngineTypePreparations;

    public static SiegeEngineType Ladder => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeLadder;

    public static SiegeEngineType Ballista => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeBallista;

    public static SiegeEngineType FireBallista => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeFireBallista;

    public static SiegeEngineType Ram => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeRam;

    public static SiegeEngineType ImprovedRam => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeImprovedRam;

    public static SiegeEngineType SiegeTower => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeSiegeTower;

    public static SiegeEngineType HeavySiegeTower => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeHeavySiegeTower;

    public static SiegeEngineType Catapult => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeCatapult;

    public static SiegeEngineType FireCatapult => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeFireCatapult;

    public static SiegeEngineType Onager => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeOnager;

    public static SiegeEngineType FireOnager => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeFireOnager;

    public static SiegeEngineType Bricole => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeBricole;

    public static SiegeEngineType Trebuchet => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeTrebuchet;

    public static SiegeEngineType FireTrebuchet => DefaultSiegeEngineTypes.Instance.SiegeEngineTypeTrebuchet;

    public DefaultSiegeEngineTypes() => this.RegisterAll();

    private void RegisterAll()
    {
      MBObjectManager.Instance.LoadXML("SiegeEngines");
      this.SiegeEngineTypePreparations = this.GetSiegeEngine("preparations");
      this.SiegeEngineTypeLadder = this.GetSiegeEngine("ladder");
      this.SiegeEngineTypeSiegeTower = this.GetSiegeEngine("siege_tower_level1");
      this.SiegeEngineTypeHeavySiegeTower = this.GetSiegeEngine("siege_tower_level2");
      this.SiegeEngineTypeBallista = this.GetSiegeEngine("ballista");
      this.SiegeEngineTypeFireBallista = this.GetSiegeEngine("fire_ballista");
      this.SiegeEngineTypeCatapult = this.GetSiegeEngine("catapult");
      this.SiegeEngineTypeFireCatapult = this.GetSiegeEngine("fire_catapult");
      this.SiegeEngineTypeOnager = this.GetSiegeEngine("onager");
      this.SiegeEngineTypeFireOnager = this.GetSiegeEngine("fire_onager");
      this.SiegeEngineTypeBricole = this.GetSiegeEngine("bricole");
      this.SiegeEngineTypeTrebuchet = this.GetSiegeEngine("trebuchet");
      this.SiegeEngineTypeFireTrebuchet = this.GetSiegeEngine("fire_trebuchet");
      this.SiegeEngineTypeRam = this.GetSiegeEngine("ram");
      this.SiegeEngineTypeImprovedRam = this.GetSiegeEngine("improved_ram");
    }

    private SiegeEngineType GetSiegeEngine(string id) => MBObjectManager.Instance.GetObject<SiegeEngineType>(id);
  }
}
