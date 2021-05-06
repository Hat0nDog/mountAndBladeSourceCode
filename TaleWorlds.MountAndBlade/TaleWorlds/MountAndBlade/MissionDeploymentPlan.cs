// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionDeploymentPlan
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class MissionDeploymentPlan
  {
    public const int NumFormationsWithUnset = 11;
    private const float StandardSpawnPathLength = 800f;
    private readonly Mission _mission;
    private readonly BattleSideDeploymentPlan[] _battleSideDeploymentPlans = new BattleSideDeploymentPlan[2];
    private FormationSceneEntry[,] _formationSceneEntries;

    public bool IsPlanMade { get; private set; }

    public int TroopCount
    {
      get
      {
        int num = 0;
        foreach (BattleSideDeploymentPlan sideDeploymentPlan in this._battleSideDeploymentPlans)
          num += sideDeploymentPlan.TroopCount;
        return num;
      }
    }

    public MissionDeploymentPlan(Mission mission)
    {
      this._mission = mission;
      for (int index = 0; index < 2; ++index)
        this._battleSideDeploymentPlans[index] = new BattleSideDeploymentPlan((BattleSideEnum) index);
      this.ClearTroops();
      this.ClearPlan();
    }

    public static (float, float) GetFormationSpawnWidthAndDepth(
      FormationClass formationNo,
      int troopCount,
      bool considerCavalryAsInfantry = false)
    {
      bool isMounted = !considerCavalryAsInfantry && formationNo.IsMounted();
      float defaultUnitDiameter = Formation.GetDefaultUnitDiameter(isMounted);
      int unitSpacing = 1;
      float num1 = isMounted ? Formation.CavalryInterval(unitSpacing) : Formation.InfantryInterval(unitSpacing);
      float num2 = isMounted ? Formation.CavalryDistance(unitSpacing) : Formation.InfantryDistance(unitSpacing);
      int num3 = Math.Max(1, (int) (((double) Math.Max(0, troopCount - 1) * ((double) num1 + (double) defaultUnitDiameter) + (double) defaultUnitDiameter) / Math.Sqrt((isMounted ? 24.0 : 12.0) * (double) troopCount + 1.0)));
      return (Math.Max(0.0f, (float) troopCount / (float) num3 - 1f) * (num1 + defaultUnitDiameter) + defaultUnitDiameter, (float) (num3 - 1) * (num2 + defaultUnitDiameter) + defaultUnitDiameter);
    }

    public static float GetBattleSizeFactor(int battleSize, float normalizationFactor)
    {
      float num = -1f;
      if (battleSize > 0)
        num = (float) (0.0399999991059303 + 0.0799999982118607 * Math.Pow((double) battleSize, 0.400000005960464)) * normalizationFactor;
      return MBMath.ClampFloat(num, 0.15f, 1f);
    }

    public static float GetSpawnPathRatioForBattleSide(
      BattleSideEnum side,
      float battleSizeFactor,
      float middlePoint)
    {
      float num = 0.0f;
      switch (side)
      {
        case BattleSideEnum.Defender:
          num = Math.Max(0.0f, middlePoint - 0.44f * battleSizeFactor);
          break;
        case BattleSideEnum.Attacker:
          num = Math.Min(1f, middlePoint + 0.44f * battleSizeFactor);
          break;
      }
      return num;
    }

    public static void GetSpawnPathPositionsForBattleSides(
      Path spawnPath,
      float spawnPathOffset,
      int battleSize,
      out Vec2 attackerPosition,
      out Vec2 attackerDirection,
      out Vec2 defenderPosition,
      out Vec2 defenderDirection)
    {
      if ((NativeObject) spawnPath == (NativeObject) null)
      {
        attackerPosition = Mission.Current.GetSceneMiddleFrame().Origin.AsVec2;
        attackerDirection = Vec2.Forward;
        defenderPosition = attackerPosition;
        defenderDirection = -attackerDirection;
      }
      else
      {
        float totalLength = spawnPath.GetTotalLength();
        float normalizationFactor = 800f / totalLength;
        float middlePoint = 0.5f + spawnPathOffset;
        float battleSizeFactor = MissionDeploymentPlan.GetBattleSizeFactor(battleSize, normalizationFactor);
        float ratioForBattleSide1 = MissionDeploymentPlan.GetSpawnPathRatioForBattleSide(BattleSideEnum.Attacker, battleSizeFactor, middlePoint);
        float ratioForBattleSide2 = MissionDeploymentPlan.GetSpawnPathRatioForBattleSide(BattleSideEnum.Defender, battleSizeFactor, middlePoint);
        attackerPosition = spawnPath.GetFrameForDistance(totalLength * ratioForBattleSide1 + spawnPathOffset).origin.AsVec2;
        defenderPosition = spawnPath.GetFrameForDistance(totalLength * ratioForBattleSide2 - spawnPathOffset).origin.AsVec2;
        attackerDirection = (defenderPosition - attackerPosition).Normalized();
        defenderDirection = -attackerDirection;
      }
    }

    public void ClearPlan()
    {
      for (int index = 0; index < 2; ++index)
        this._battleSideDeploymentPlans[index].ClearPlan();
      this.IsPlanMade = false;
    }

    public void ClearTroops()
    {
      for (int index = 0; index < 2; ++index)
        this._battleSideDeploymentPlans[index].ClearTroops();
    }

    public void AddTroops(BattleSideEnum battleSide, FormationClass formationClass, int troopCount)
    {
      if (troopCount <= 0 || battleSide == BattleSideEnum.None)
        return;
      this._battleSideDeploymentPlans[(int) battleSide].AddTroops(formationClass, troopCount);
    }

    public void SetSpawnWithHorsesForSide(BattleSideEnum battleSide, bool spawnWithHorses) => this._battleSideDeploymentPlans[(int) battleSide].SpawnWithHorses = spawnWithHorses;

    public void PlanBattleDeployment(
      Path spawnPath = null,
      float spawnPathOffset = 0.0f,
      int reinforcementBattleSize = 25)
    {
      if (this.IsPlanMade)
        this.ClearPlan();
      BattleSideDeploymentPlan sideDeploymentPlan1 = this._battleSideDeploymentPlans[1];
      BattleSideDeploymentPlan sideDeploymentPlan2 = this._battleSideDeploymentPlans[0];
      int battleSize1 = Math.Min(sideDeploymentPlan1.TroopCount, sideDeploymentPlan2.TroopCount);
      bool isFieldBattle = this._mission.IsFieldBattle;
      if ((NativeObject) spawnPath != (NativeObject) null)
      {
        Vec2 attackerPosition1;
        Vec2 attackerDirection1;
        Vec2 defenderPosition1;
        Vec2 defenderDirection1;
        MissionDeploymentPlan.GetSpawnPathPositionsForBattleSides(spawnPath, spawnPathOffset, battleSize1, out attackerPosition1, out attackerDirection1, out defenderPosition1, out defenderDirection1);
        sideDeploymentPlan1.PlanFieldBattleDeploymentAtPosition(attackerPosition1, attackerDirection1);
        sideDeploymentPlan2.PlanFieldBattleDeploymentAtPosition(defenderPosition1, defenderDirection1);
        if (reinforcementBattleSize > 0)
        {
          int battleSize2 = battleSize1 + reinforcementBattleSize;
          Vec2 attackerPosition2;
          Vec2 attackerDirection2;
          Vec2 defenderPosition2;
          Vec2 defenderDirection2;
          MissionDeploymentPlan.GetSpawnPathPositionsForBattleSides(spawnPath, spawnPathOffset, battleSize2, out attackerPosition2, out attackerDirection2, out defenderPosition2, out defenderDirection2);
          sideDeploymentPlan1.PlanFieldBattleDeploymentAtPosition(attackerPosition2, attackerDirection2, true);
          sideDeploymentPlan2.PlanFieldBattleDeploymentAtPosition(defenderPosition2, defenderDirection2, true);
        }
      }
      else if (isFieldBattle)
      {
        if (this._formationSceneEntries == null)
          this.ReadFormationSpawnEntitiesFromScene(true);
        sideDeploymentPlan1.PlanFieldBattleDeploymentFromSceneData(battleSize1, this._formationSceneEntries);
        sideDeploymentPlan2.PlanFieldBattleDeploymentFromSceneData(battleSize1, this._formationSceneEntries);
        if (reinforcementBattleSize > 0)
        {
          int battleSize2 = battleSize1 + reinforcementBattleSize;
          sideDeploymentPlan1.PlanFieldBattleDeploymentFromSceneData(battleSize2, this._formationSceneEntries, true);
          sideDeploymentPlan2.PlanFieldBattleDeploymentFromSceneData(battleSize2, this._formationSceneEntries, true);
        }
      }
      else
      {
        if (this._formationSceneEntries == null)
          this.ReadFormationSpawnEntitiesFromScene(false);
        sideDeploymentPlan1.PlanBattleDeploymentFromSceneData(this._formationSceneEntries);
        sideDeploymentPlan2.PlanBattleDeploymentFromSceneData(this._formationSceneEntries);
      }
      this.IsPlanMade = true;
    }

    public FormationDeploymentPlan GetFormationPlan(
      BattleSideEnum side,
      FormationClass fClass)
    {
      return this._battleSideDeploymentPlans[(int) side].GetFormationPlan(fClass);
    }

    private void ReadFormationSpawnEntitiesFromScene(bool isFieldBattle)
    {
      this._formationSceneEntries = new FormationSceneEntry[2, 11];
      Scene scene = Mission.Current.Scene;
      if (isFieldBattle)
      {
        for (int index1 = 0; index1 < 2; ++index1)
        {
          string str = index1 == 1 ? "attacker_" : "defender_";
          for (int index2 = 0; index2 < 11; ++index2)
          {
            FormationClass formationClass1 = (FormationClass) index2;
            GameEntity defaultEntity1 = scene.FindEntityWithTag(str + formationClass1.GetName().ToLower());
            if ((NativeObject) defaultEntity1 == (NativeObject) null)
            {
              FormationClass formationClass2 = formationClass1.FallbackClass();
              int index3 = (int) formationClass2;
              GameEntity defaultEntity2 = this._formationSceneEntries[index1, index3].DefaultEntity;
              defaultEntity1 = (NativeObject) defaultEntity2 != (NativeObject) null ? defaultEntity2 : scene.FindEntityWithTag(str + formationClass2.GetName().ToLower());
            }
            this._formationSceneEntries[index1, index2] = new FormationSceneEntry(defaultEntity1, (GameEntity) null);
          }
        }
      }
      else
      {
        for (int index1 = 0; index1 < 2; ++index1)
        {
          string str = ((BattleSideEnum) index1).ToString().ToLower() + "_";
          for (int index2 = 0; index2 < 11; ++index2)
          {
            FormationClass formationClass1 = (FormationClass) index2;
            FormationClass formationClass2 = formationClass1.FallbackClass();
            int index3 = (int) formationClass2;
            string tag1 = str + formationClass1.GetName().ToLower();
            GameEntity defaultEntity1 = scene.FindEntityWithTag(tag1);
            if ((NativeObject) defaultEntity1 == (NativeObject) null)
            {
              GameEntity defaultEntity2 = this._formationSceneEntries[index1, index3].DefaultEntity;
              if ((NativeObject) defaultEntity2 != (NativeObject) null)
              {
                defaultEntity1 = defaultEntity2;
              }
              else
              {
                string tag2 = str + formationClass2.GetName().ToLower();
                defaultEntity1 = scene.FindEntityWithTag(tag2);
              }
            }
            string tag3 = str + formationClass1.GetName().ToLower() + "_reinforcement";
            GameEntity reinforcementEntity1 = scene.FindEntityWithTag(tag3);
            if ((NativeObject) reinforcementEntity1 == (NativeObject) null)
            {
              GameEntity reinforcementEntity2 = this._formationSceneEntries[index1, index3].ReinforcementEntity;
              if ((NativeObject) reinforcementEntity2 != (NativeObject) null)
              {
                reinforcementEntity1 = reinforcementEntity2;
              }
              else
              {
                string tag2 = str + formationClass2.GetName().ToLower() + "_reinforcement";
                reinforcementEntity1 = scene.FindEntityWithTag(tag2);
              }
            }
            this._formationSceneEntries[index1, index2] = new FormationSceneEntry(defaultEntity1, reinforcementEntity1);
          }
        }
      }
    }
  }
}
