// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FormationQuerySystem
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class FormationQuerySystem
  {
    public readonly Formation Formation;
    private readonly QueryData<float> _formationPower;
    private readonly QueryData<float> _formationMeleeFightingPower;
    private readonly QueryData<Vec2> _averagePosition;
    private readonly QueryData<Vec2> _currentVelocity;
    private float _lastAveragePositionCalculateTime;
    private readonly QueryData<Vec2> _estimatedDirection;
    private readonly QueryData<WorldPosition> _medianPosition;
    private readonly QueryData<Vec2> _averageAllyPosition;
    private readonly QueryData<float> _idealAverageDisplacement;
    private readonly QueryData<float> _formationDispersedness;
    private readonly QueryData<FormationQuerySystem.FormationIntegrityDataGroup> _formationIntegrityData;
    private readonly QueryData<IEnumerable<Agent>> _localAllyUnits;
    private readonly QueryData<IEnumerable<Agent>> _localEnemyUnits;
    private readonly QueryData<FormationClass> _mainClass;
    private readonly QueryData<float> _infantryUnitRatio;
    private readonly QueryData<float> _hasShieldUnitRatio;
    private readonly QueryData<float> _rangedUnitRatio;
    private readonly QueryData<float> _cavalryUnitRatio;
    private readonly QueryData<float> _rangedCavalryUnitRatio;
    private readonly QueryData<bool> _isMeleeFormation;
    private readonly QueryData<bool> _isInfantryFormation;
    private readonly QueryData<bool> _hasShield;
    private readonly QueryData<bool> _isRangedFormation;
    private readonly QueryData<bool> _isCavalryFormation;
    private readonly QueryData<bool> _isRangedCavalryFormation;
    private readonly QueryData<float> _movementSpeedMaximum;
    private readonly QueryData<float> _movementSpeed;
    private readonly QueryData<float> _missileRange;
    private readonly QueryData<float> _localInfantryUnitRatio;
    private readonly QueryData<float> _localRangedUnitRatio;
    private readonly QueryData<float> _localCavalryUnitRatio;
    private readonly QueryData<float> _localRangedCavalryUnitRatio;
    private readonly QueryData<float> _localAllyPower;
    private readonly QueryData<float> _localEnemyPower;
    private readonly QueryData<float> _localPowerRatio;
    private readonly QueryData<float> _casualtyRatio;
    private readonly QueryData<bool> _isUnderRangedAttack;
    private readonly QueryData<float> _underRangedAttackRatio;
    private readonly QueryData<float> _makingRangedAttackRatio;
    private readonly QueryData<Formation> _mainFormation;
    private readonly QueryData<float> _mainFormationReliabilityFactor;
    private readonly QueryData<Formation> _closestEnemyFormation;
    private readonly QueryData<Formation> _closestSignificantlyLargeEnemyFormation;
    private readonly QueryData<Formation> _fastestSignificantlyLargeEnemyFormation;
    private readonly QueryData<Vec2> _highGroundCloseToForeseenBattleGround;

    public TeamQuerySystem Team => this.Formation.Team.QuerySystem;

    public float FormationPower => this._formationPower.Value;

    public float FormationMeleeFightingPower => this._formationMeleeFightingPower.Value;

    public Vec2 AveragePosition => this._averagePosition.Value;

    public Vec2 CurrentVelocity => this._currentVelocity.Value;

    public Vec2 EstimatedDirection => this._estimatedDirection.Value;

    public WorldPosition MedianPosition => this._medianPosition.Value;

    public Vec2 AverageAllyPosition => this._averageAllyPosition.Value;

    public float IdealAverageDisplacement => this._idealAverageDisplacement.Value;

    public float FormationDispersedness => this._formationDispersedness.Value;

    public FormationQuerySystem.FormationIntegrityDataGroup FormationIntegrityData => this._formationIntegrityData.Value;

    public IEnumerable<Agent> LocalAllyUnits => this._localAllyUnits.Value;

    public IEnumerable<Agent> LocalEnemyUnits => this._localEnemyUnits.Value;

    public FormationClass MainClass => this._mainClass.Value;

    public float InfantryUnitRatio => this._infantryUnitRatio.Value;

    public float HasShieldUnitRatio => this._hasShieldUnitRatio.Value;

    public float RangedUnitRatio => this._rangedUnitRatio.Value;

    public void ForceExpireCavalryUnitRatio() => this._cavalryUnitRatio.Expire();

    public float CavalryUnitRatio => this._cavalryUnitRatio.Value;

    public float GetCavalryUnitRatioWithoutExpiration => this._cavalryUnitRatio.GetCachedValue();

    public float RangedCavalryUnitRatio => this._rangedCavalryUnitRatio.Value;

    public float GetRangedCavalryUnitRatioWithoutExpiration => this._rangedCavalryUnitRatio.GetCachedValue();

    public bool IsMeleeFormation => this._isMeleeFormation.Value;

    public bool IsInfantryFormation => this._isInfantryFormation.Value;

    public bool HasShield => this._hasShield.Value;

    public bool IsRangedFormation => this._isRangedFormation.Value;

    public bool IsCavalryFormation => this._isCavalryFormation.Value;

    public bool IsRangedCavalryFormation => this._isRangedCavalryFormation.Value;

    public float MovementSpeedMaximum => this._movementSpeedMaximum.Value;

    public float MovementSpeed => this._movementSpeed.Value;

    public float MissileRange => this._missileRange.Value;

    public float LocalInfantryUnitRatio => this._localInfantryUnitRatio.Value;

    public float LocalRangedUnitRatio => this._localRangedUnitRatio.Value;

    public float LocalCavalryUnitRatio => this._localCavalryUnitRatio.Value;

    public float LocalRangedCavalryUnitRatio => this._localRangedCavalryUnitRatio.Value;

    public float LocalAllyPower => this._localAllyPower.Value;

    public float LocalEnemyPower => this._localEnemyPower.Value;

    public float LocalPowerRatio => this._localPowerRatio.Value;

    public float CasualtyRatio => this._casualtyRatio.Value;

    public bool IsUnderRangedAttack => this._isUnderRangedAttack.Value;

    public float UnderRangedAttackRatio => this._underRangedAttackRatio.Value;

    public float MakingRangedAttackRatio => this._makingRangedAttackRatio.Value;

    public Formation MainFormation => this._mainFormation.Value;

    public float MainFormationReliabilityFactor => this._mainFormationReliabilityFactor.Value;

    public FormationQuerySystem ClosestEnemyFormation
    {
      get
      {
        if (this._closestEnemyFormation.Value == null || this._closestEnemyFormation.Value.CountOfUnits == 0)
          this._closestEnemyFormation.Expire();
        return this._closestEnemyFormation.Value?.QuerySystem;
      }
    }

    public FormationQuerySystem ClosestSignificantlyLargeEnemyFormation
    {
      get
      {
        if (this._closestSignificantlyLargeEnemyFormation.Value == null || this._closestSignificantlyLargeEnemyFormation.Value.CountOfUnits == 0)
          this._closestSignificantlyLargeEnemyFormation.Expire();
        return this._closestSignificantlyLargeEnemyFormation.Value?.QuerySystem;
      }
    }

    public FormationQuerySystem FastestSignificantlyLargeEnemyFormation
    {
      get
      {
        if (this._fastestSignificantlyLargeEnemyFormation.Value == null || this._fastestSignificantlyLargeEnemyFormation.Value.CountOfUnits == 0)
          this._fastestSignificantlyLargeEnemyFormation.Expire();
        return this._fastestSignificantlyLargeEnemyFormation.Value?.QuerySystem;
      }
    }

    public Vec2 HighGroundCloseToForeseenBattleGround => this._highGroundCloseToForeseenBattleGround.Value;

    public FormationQuerySystem(Formation formation)
    {
      FormationQuerySystem formationQuerySystem = this;
      this.Formation = formation;
      Mission mission = Mission.Current;
      this._formationPower = new QueryData<float>(new Func<float>(formation.GetFormationPower), 2.5f);
      this._formationMeleeFightingPower = new QueryData<float>(new Func<float>(formation.GetFormationMeleeFightingPower), 2.5f);
      this._averagePosition = new QueryData<Vec2>((Func<Vec2>) (() =>
      {
        Vec2 vec2 = formation.CountOfUnitsWithoutDetachedOnes > 1 ? formation.GetAveragePositionOfUnits(true, true) : (formation.CountOfUnitsWithoutDetachedOnes > 0 ? formation.GetAveragePositionOfUnits(true, false) : formation.OrderPosition.AsVec2);
        float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
        float num = time - formationQuerySystem._lastAveragePositionCalculateTime;
        if ((double) num > 0.0)
          formationQuerySystem._currentVelocity.SetValue((vec2 - formationQuerySystem._averagePosition.GetCachedValue()) * (1f / num), time);
        formationQuerySystem._lastAveragePositionCalculateTime = time;
        return vec2;
      }), 0.05f);
      this._currentVelocity = new QueryData<Vec2>((Func<Vec2>) (() =>
      {
        formationQuerySystem._averagePosition.Evaluate(MBCommon.GetTime(MBCommon.TimeType.Mission));
        return formationQuerySystem._currentVelocity.GetCachedValue();
      }), 1f);
      this._estimatedDirection = new QueryData<Vec2>((Func<Vec2>) (() =>
      {
        if (formation.CountOfUnitsWithoutDetachedOnes > 0)
        {
          Vec2 averagePositionOfUnits = formation.GetAveragePositionOfUnits(true, false);
          float num1 = 0.0f;
          float num2 = 0.0f;
          Vec2 localAveragePosition = formation.OrderLocalAveragePosition;
          foreach (Agent looseDetachedOne in formation.UnitsWithoutLooseDetachedOnes)
          {
            Vec2? positionOfUnitOrDefault = formation.arrangement.GetLocalPositionOfUnitOrDefault((IFormationUnit) looseDetachedOne);
            if (positionOfUnitOrDefault.HasValue)
            {
              Vec2 vec2 = positionOfUnitOrDefault.Value;
              Vec2 asVec2 = looseDetachedOne.Position.AsVec2;
              num1 += (float) (((double) vec2.x - (double) localAveragePosition.x) * ((double) asVec2.x - (double) averagePositionOfUnits.x) + ((double) vec2.y - (double) localAveragePosition.y) * ((double) asVec2.y - (double) averagePositionOfUnits.y));
              num2 += (float) (((double) vec2.x - (double) localAveragePosition.x) * ((double) asVec2.y - (double) averagePositionOfUnits.y) - ((double) vec2.y - (double) localAveragePosition.y) * ((double) asVec2.x - (double) averagePositionOfUnits.x));
            }
          }
          float num3 = 1f / (float) formation.CountOfUnitsWithoutDetachedOnes;
          float num4 = num1 * num3;
          float num5 = num2 * num3;
          float num6 = MathF.Sqrt((float) ((double) num4 * (double) num4 + (double) num5 * (double) num5));
          if ((double) num6 > 0.0)
          {
            double num7 = Math.Acos((double) MBMath.ClampFloat(num4 / num6, -1f, 1f));
            Vec2 vec2_1 = Vec2.FromRotation((float) num7);
            Vec2 vec2_2 = Vec2.FromRotation((float) -num7);
            float num8 = 0.0f;
            float num9 = 0.0f;
            foreach (Agent looseDetachedOne in formation.UnitsWithoutLooseDetachedOnes)
            {
              Vec2? positionOfUnitOrDefault = formation.arrangement.GetLocalPositionOfUnitOrDefault((IFormationUnit) looseDetachedOne);
              if (positionOfUnitOrDefault.HasValue)
              {
                Vec2 parentUnitF1 = vec2_1.TransformToParentUnitF(positionOfUnitOrDefault.Value - localAveragePosition);
                Vec2 parentUnitF2 = vec2_2.TransformToParentUnitF(positionOfUnitOrDefault.Value - localAveragePosition);
                Vec2 asVec2 = looseDetachedOne.Position.AsVec2;
                num8 += (parentUnitF1 - asVec2 + averagePositionOfUnits).LengthSquared;
                num9 += (parentUnitF2 - asVec2 + averagePositionOfUnits).LengthSquared;
              }
            }
            return (double) num8 >= (double) num9 ? vec2_2 : vec2_1;
          }
        }
        return new Vec2(0.0f, 1f);
      }), 0.2f);
      this._medianPosition = new QueryData<WorldPosition>((Func<WorldPosition>) (() =>
      {
        if (formation.CountOfUnitsWithoutDetachedOnes != 0)
          return formation.CountOfUnitsWithoutDetachedOnes != 1 ? formation.GetMedianAgent(true, true, formationQuerySystem.AveragePosition).GetWorldPosition() : formation.GetMedianAgent(true, false, formationQuerySystem.AveragePosition).GetWorldPosition();
        if (formation.CountOfUnits == 0)
          return formation.OrderPosition;
        return formation.CountOfUnits != 1 ? formation.GetMedianAgent(false, true, formationQuerySystem.AveragePosition).GetWorldPosition() : formation.GetFirstUnit().GetWorldPosition();
      }), 0.05f);
      this._averageAllyPosition = new QueryData<Vec2>((Func<Vec2>) (() =>
      {
        IEnumerable<Formation> source = mission.Teams.GetAlliesOf(formation.Team, true).SelectMany<TaleWorlds.MountAndBlade.Team, Formation>((Func<TaleWorlds.MountAndBlade.Team, IEnumerable<Formation>>) (t => t.FormationsIncludingSpecial)).Where<Formation>(closure_1 ?? (closure_1 = (Func<Formation, bool>) (f => f != formation)));
        if (source.IsEmpty<Formation>())
          return closure_0.AveragePosition;
        Vec2 zero = Vec2.Zero;
        int num = 0;
        foreach (Formation formation1 in source)
        {
          zero += formation1.GetAveragePositionOfUnits(false, false) * (float) formation1.CountOfUnits;
          num += formation1.CountOfUnits;
        }
        return num == 0 ? Vec2.Invalid : zero * (1f / (float) num);
      }), 5f);
      this._idealAverageDisplacement = new QueryData<float>((Func<float>) (() => (float) Math.Sqrt(Math.Pow((double) formation.Width / 2.0, 2.0) + Math.Pow((double) formation.Depth / 2.0, 2.0)) / 2f), 5f);
      this._formationIntegrityData = new QueryData<FormationQuerySystem.FormationIntegrityDataGroup>((Func<FormationQuerySystem.FormationIntegrityDataGroup>) (() =>
      {
        if (formation.CountOfUnitsWithoutDetachedOnes > 0)
        {
          float num1 = 0.0f;
          List<IFormationUnit> allUnits = formation.arrangement.GetAllUnits();
          Vec3 vec3;
          for (int index = 0; index < allUnits.Count; ++index)
          {
            Agent unit = allUnits[index] as Agent;
            double num2 = (double) num1;
            Vec2 globalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(unit, false);
            vec3 = unit.Position;
            Vec2 asVec2 = vec3.AsVec2;
            double lengthSquared = (double) (globalPositionOfUnit - asVec2).LengthSquared;
            num1 = (float) (num2 + lengthSquared);
          }
          Vec2 vec2_1;
          for (int index = 0; index < formation.LooseDetachedUnits.Count; ++index)
          {
            Agent looseDetachedUnit = formation.LooseDetachedUnits[index];
            double num2 = (double) num1;
            Vec2 globalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(looseDetachedUnit, false);
            vec3 = looseDetachedUnit.Position;
            Vec2 asVec2 = vec3.AsVec2;
            vec2_1 = globalPositionOfUnit - asVec2;
            double lengthSquared = (double) vec2_1.LengthSquared;
            num1 = (float) (num2 + lengthSquared);
          }
          float num3 = (float) ((double) num1 / (double) formation.CountOfUnitsWithoutDetachedOnes * 4.0);
          float num4 = 0.0f;
          Vec2 vec2_2 = Vec2.Zero;
          float num5 = 0.0f;
          int num6 = 0;
          for (int index = 0; index < allUnits.Count; ++index)
          {
            Agent unit = allUnits[index] as Agent;
            Vec2 globalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(unit, false);
            vec3 = unit.Position;
            Vec2 asVec2_1 = vec3.AsVec2;
            vec2_1 = globalPositionOfUnit - asVec2_1;
            float lengthSquared = vec2_1.LengthSquared;
            if ((double) lengthSquared < (double) num3)
            {
              num4 += lengthSquared;
              Vec2 vec2_3 = vec2_2;
              vec3 = unit.AverageVelocity;
              Vec2 asVec2_2 = vec3.AsVec2;
              vec2_2 = vec2_3 + asVec2_2;
              num5 += unit.MaximumForwardUnlimitedSpeed;
              ++num6;
            }
          }
          for (int index = 0; index < formation.LooseDetachedUnits.Count; ++index)
          {
            Agent looseDetachedUnit = formation.LooseDetachedUnits[index];
            Vec2 globalPositionOfUnit = formation.GetCurrentGlobalPositionOfUnit(looseDetachedUnit, false);
            vec3 = looseDetachedUnit.Position;
            Vec2 asVec2_1 = vec3.AsVec2;
            vec2_1 = globalPositionOfUnit - asVec2_1;
            float lengthSquared = vec2_1.LengthSquared;
            if ((double) lengthSquared < (double) num3)
            {
              num4 += lengthSquared;
              Vec2 vec2_3 = vec2_2;
              vec3 = looseDetachedUnit.AverageVelocity;
              Vec2 asVec2_2 = vec3.AsVec2;
              vec2_2 = vec2_3 + asVec2_2;
              num5 += looseDetachedUnit.MaximumForwardUnlimitedSpeed;
              ++num6;
            }
          }
          if (num6 > 0)
          {
            Vec2 vec2_3 = vec2_2 * (1f / (float) num6);
            float x = num4 / (float) num6;
            float num2 = num5 / (float) num6;
            FormationQuerySystem.FormationIntegrityDataGroup integrityDataGroup;
            integrityDataGroup.AverageVelocityExcludeFarAgents = vec2_3;
            integrityDataGroup.DeviationOfPositionsExcludeFarAgents = MathF.Sqrt(x);
            integrityDataGroup.AverageMaxUnlimitedSpeedExcludeFarAgents = num2;
            return integrityDataGroup;
          }
        }
        FormationQuerySystem.FormationIntegrityDataGroup integrityDataGroup1;
        integrityDataGroup1.AverageVelocityExcludeFarAgents = Vec2.Zero;
        integrityDataGroup1.DeviationOfPositionsExcludeFarAgents = 0.0f;
        integrityDataGroup1.AverageMaxUnlimitedSpeedExcludeFarAgents = 0.0f;
        return integrityDataGroup1;
      }), 1f);
      this._formationDispersedness = new QueryData<float>((Func<float>) (() =>
      {
        if (formation.CountOfUnits == 0)
          return 0.0f;
        float num1 = 0.0f;
        int num2 = 0;
        foreach (IFormationUnit allUnit in formation.arrangement.GetAllUnits())
        {
          Vec2? positionOfUnitOrDefault = formation.arrangement.GetLocalPositionOfUnitOrDefault(allUnit);
          if (positionOfUnitOrDefault.HasValue)
          {
            MatrixFrame m = new MatrixFrame(Mat3.Identity, new Vec3(positionOfUnitOrDefault.Value));
            MatrixFrame matrixFrame;
            ref MatrixFrame local = ref matrixFrame;
            Mat3 rot = new Mat3(new Vec3(formation.Direction.RightVec()), new Vec3(formation.Direction), new Vec3(z: 1f));
            WorldPosition worldPosition = formation.QuerySystem.MedianPosition;
            Vec3 navMeshVec3 = worldPosition.GetNavMeshVec3();
            local = new MatrixFrame(rot, navMeshVec3);
            MatrixFrame parent = matrixFrame.TransformToParent(m);
            double num3 = (double) num1;
            worldPosition = (allUnit as Agent).GetWorldPosition();
            double num4 = (double) worldPosition.GetGroundVec3().Distance(parent.origin);
            num1 = (float) (num3 + num4);
          }
          else
            ++num2;
        }
        return formation.CountOfUnits - num2 > 0 ? num1 / (float) (formation.CountOfUnits - num2) : 0.0f;
      }), 2f);
      this._localAllyUnits = new QueryData<IEnumerable<Agent>>((Func<IEnumerable<Agent>>) (() => mission.GetNearbyAllyAgents(closure_0.AveragePosition, 30f, formation.Team)), 5f);
      this._localEnemyUnits = new QueryData<IEnumerable<Agent>>((Func<IEnumerable<Agent>>) (() => mission.GetNearbyEnemyAgents(closure_0.AveragePosition, 30f, formation.Team)), 5f);
      this._infantryUnitRatio = new QueryData<float>((Func<float>) (() => (float) formation.GetCountOfUnitsWithCondition(new Func<Agent, bool>(QueryLibrary.IsInfantry)) / (float) formation.CountOfUnits), 2.5f);
      this._hasShieldUnitRatio = new QueryData<float>((Func<float>) (() => (float) formation.GetCountOfUnitsWithCondition(new Func<Agent, bool>(QueryLibrary.HasShield)) / (float) formation.CountOfUnits), 2.5f);
      this._rangedUnitRatio = new QueryData<float>((Func<float>) (() => (float) formation.GetCountOfUnitsWithCondition(new Func<Agent, bool>(QueryLibrary.IsRanged)) / (float) formation.CountOfUnits), 2.5f);
      this._cavalryUnitRatio = new QueryData<float>((Func<float>) (() => (float) formation.GetCountOfUnitsWithCondition(new Func<Agent, bool>(QueryLibrary.IsCavalry)) / (float) formation.CountOfUnits), 2.5f);
      this._rangedCavalryUnitRatio = new QueryData<float>((Func<float>) (() => (float) formation.GetCountOfUnitsWithCondition(new Func<Agent, bool>(QueryLibrary.IsRangedCavalry)) / (float) formation.CountOfUnits), 2.5f);
      this._isMeleeFormation = new QueryData<bool>((Func<bool>) (() => (double) formationQuerySystem.InfantryUnitRatio + (double) formationQuerySystem.CavalryUnitRatio > (double) formationQuerySystem.RangedUnitRatio + (double) formationQuerySystem.RangedCavalryUnitRatio), 5f);
      this._isInfantryFormation = new QueryData<bool>((Func<bool>) (() => (double) formationQuerySystem.InfantryUnitRatio >= (double) formationQuerySystem.RangedUnitRatio && (double) formationQuerySystem.InfantryUnitRatio >= (double) formationQuerySystem.CavalryUnitRatio && (double) formationQuerySystem.InfantryUnitRatio >= (double) formationQuerySystem.RangedCavalryUnitRatio), 5f);
      this._hasShield = new QueryData<bool>((Func<bool>) (() => (double) formationQuerySystem.HasShieldUnitRatio >= 0.400000005960464), 5f);
      this._isRangedFormation = new QueryData<bool>((Func<bool>) (() => (double) formationQuerySystem.RangedUnitRatio > (double) formationQuerySystem.InfantryUnitRatio && (double) formationQuerySystem.RangedUnitRatio >= (double) formationQuerySystem.CavalryUnitRatio && (double) formationQuerySystem.RangedUnitRatio >= (double) formationQuerySystem.RangedCavalryUnitRatio), 5f);
      this._isCavalryFormation = new QueryData<bool>((Func<bool>) (() => (double) formationQuerySystem.CavalryUnitRatio > (double) formationQuerySystem.InfantryUnitRatio && (double) formationQuerySystem.CavalryUnitRatio > (double) formationQuerySystem.RangedUnitRatio && (double) formationQuerySystem.CavalryUnitRatio >= (double) formationQuerySystem.RangedCavalryUnitRatio), 5f);
      this._isRangedCavalryFormation = new QueryData<bool>((Func<bool>) (() => (double) formationQuerySystem.RangedCavalryUnitRatio > (double) formationQuerySystem.InfantryUnitRatio && (double) formationQuerySystem.RangedCavalryUnitRatio > (double) formationQuerySystem.RangedUnitRatio && (double) formationQuerySystem.RangedCavalryUnitRatio > (double) formationQuerySystem.CavalryUnitRatio), 5f);
      QueryData<float>.SetupSyncGroup((IQueryData) this._infantryUnitRatio, (IQueryData) this._hasShieldUnitRatio, (IQueryData) this._rangedUnitRatio, (IQueryData) this._cavalryUnitRatio, (IQueryData) this._rangedCavalryUnitRatio, (IQueryData) this._isMeleeFormation, (IQueryData) this._isInfantryFormation, (IQueryData) this._hasShield, (IQueryData) this._isRangedFormation, (IQueryData) this._isCavalryFormation, (IQueryData) this._isRangedCavalryFormation);
      this._movementSpeedMaximum = new QueryData<float>((Func<float>) (() => formation.GetAverageMaximumMovementSpeedOfUnits()), 10f);
      this._movementSpeed = new QueryData<float>((Func<float>) (() => formation.GetMovementSpeedOfUnits()), 2f);
      this._missileRange = new QueryData<float>((Func<float>) (() =>
      {
        if (formation.CountOfUnits == 0)
          return 0.0f;
        float sum = 0.0f;
        formation.ApplyActionOnEachUnit((Action<Agent>) (agent => sum += agent.MaximumMissileRange));
        return sum / (float) formation.CountOfUnits;
      }), 10f);
      this._localInfantryUnitRatio = new QueryData<float>((Func<float>) (() => formationQuerySystem.LocalAllyUnits.Any<Agent>() ? 1f * (float) formationQuerySystem.LocalAllyUnits.Count<Agent>((Func<Agent, bool>) (u => QueryLibrary.IsInfantry(u))) / (float) formationQuerySystem.LocalAllyUnits.Count<Agent>() : 0.0f), 15f);
      this._localRangedUnitRatio = new QueryData<float>((Func<float>) (() => formationQuerySystem.LocalAllyUnits.Any<Agent>() ? 1f * (float) formationQuerySystem.LocalAllyUnits.Count<Agent>((Func<Agent, bool>) (u => QueryLibrary.IsRanged(u))) / (float) formationQuerySystem.LocalAllyUnits.Count<Agent>() : 0.0f), 15f);
      this._localCavalryUnitRatio = new QueryData<float>((Func<float>) (() => formationQuerySystem.LocalAllyUnits.Any<Agent>() ? 1f * (float) formationQuerySystem.LocalAllyUnits.Count<Agent>((Func<Agent, bool>) (u => QueryLibrary.IsCavalry(u))) / (float) formationQuerySystem.LocalAllyUnits.Count<Agent>() : 0.0f), 15f);
      this._localRangedCavalryUnitRatio = new QueryData<float>((Func<float>) (() => formationQuerySystem.LocalAllyUnits.Any<Agent>() ? 1f * (float) formationQuerySystem.LocalAllyUnits.Count<Agent>((Func<Agent, bool>) (u => QueryLibrary.IsRangedCavalry(u))) / (float) formationQuerySystem.LocalAllyUnits.Count<Agent>() : 0.0f), 15f);
      QueryData<float>.SetupSyncGroup((IQueryData) this._localInfantryUnitRatio, (IQueryData) this._localRangedUnitRatio, (IQueryData) this._localCavalryUnitRatio, (IQueryData) this._localRangedCavalryUnitRatio);
      this._localAllyPower = new QueryData<float>((Func<float>) (() => formationQuerySystem.LocalAllyUnits.Sum<Agent>((Func<Agent, float>) (lau => lau.CharPowerCached))), 5f);
      this._localEnemyPower = new QueryData<float>((Func<float>) (() => formationQuerySystem.LocalEnemyUnits.Sum<Agent>((Func<Agent, float>) (leu => leu.CharPowerCached))), 5f);
      this._localPowerRatio = new QueryData<float>((Func<float>) (() => MBMath.ClampFloat((float) Math.Sqrt(((double) formationQuerySystem.LocalAllyUnits.Sum<Agent>((Func<Agent, float>) (lau => lau.CharPowerCached)) + 1.0) * 1.0 / ((double) formationQuerySystem.LocalEnemyUnits.Sum<Agent>((Func<Agent, float>) (leu => leu.CharPowerCached)) + 1.0)), 0.5f, 1.75f)), 5f);
      this._casualtyRatio = new QueryData<float>((Func<float>) (() =>
      {
        if (formation.CountOfUnits == 0)
          return 0.0f;
        CasualtyHandler missionBehaviour = mission.GetMissionBehaviour<CasualtyHandler>();
        int num = missionBehaviour != null ? missionBehaviour.GetCasualtyCountOfFormation(formation) : 0;
        return (float) (1.0 - (double) num * 1.0 / (double) (num + formation.CountOfUnits));
      }), 10f);
      this._isUnderRangedAttack = new QueryData<bool>((Func<bool>) (() => formation.GetUnderAttackTypeOfUnits(10f) == Agent.UnderAttackType.UnderRangedAttack), 3f);
      this._underRangedAttackRatio = new QueryData<float>((Func<float>) (() =>
      {
        float currentTime = MBCommon.TimeType.Mission.GetTime();
        int unitsWithCondition = formation.GetCountOfUnitsWithCondition((Func<Agent, bool>) (agent => (double) currentTime - (double) agent.LastRangedHitTime < 10.0));
        return formation.CountOfUnits <= 0 ? 0.0f : (float) unitsWithCondition / (float) formation.CountOfUnits;
      }), 3f);
      this._makingRangedAttackRatio = new QueryData<float>((Func<float>) (() =>
      {
        float currentTime = MBCommon.TimeType.Mission.GetTime();
        int unitsWithCondition = formation.GetCountOfUnitsWithCondition((Func<Agent, bool>) (agent => (double) currentTime - (double) agent.LastRangedAttackTime < 10.0));
        return formation.CountOfUnits <= 0 ? 0.0f : (float) unitsWithCondition / (float) formation.CountOfUnits;
      }), 3f);
      this._closestEnemyFormation = new QueryData<Formation>((Func<Formation>) (() =>
      {
        float num1 = float.MaxValue;
        Formation formation1 = (Formation) null;
        foreach (TaleWorlds.MountAndBlade.Team team in (ReadOnlyCollection<TaleWorlds.MountAndBlade.Team>) mission.Teams)
        {
          if (team.IsEnemyOf(formation.Team))
          {
            foreach (Formation formation2 in team.FormationsIncludingSpecialAndEmpty)
            {
              if (formation2.CountOfUnits > 0)
              {
                float num2 = formation2.QuerySystem.MedianPosition.GetNavMeshVec3().DistanceSquared(new Vec3(closure_0.AveragePosition, closure_0.MedianPosition.GetNavMeshZ()));
                if ((double) num2 < (double) num1)
                {
                  num1 = num2;
                  formation1 = formation2;
                }
              }
            }
          }
        }
        return formation1;
      }), 1.5f);
      this._closestSignificantlyLargeEnemyFormation = new QueryData<Formation>((Func<Formation>) (() =>
      {
        float num1 = float.MaxValue;
        Formation formation1 = (Formation) null;
        float num2 = float.MaxValue;
        Formation formation2 = (Formation) null;
        double num3 = (double) closure_0.FormationPower / (double) closure_0.Team.TeamPower;
        for (int index1 = 0; index1 < mission.Teams.Count; ++index1)
        {
          TaleWorlds.MountAndBlade.Team team = mission.Teams[index1];
          if (team.IsEnemyOf(formation.Team))
          {
            for (int index2 = 0; index2 < team.FormationsIncludingSpecialAndEmpty.Count; ++index2)
            {
              Formation formation3 = team.FormationsIncludingSpecialAndEmpty[index2];
              Vec3 navMeshVec3;
              if (formation3.CountOfUnits > 0)
              {
                if ((double) formation3.QuerySystem.FormationPower / (double) closure_0.FormationPower > 0.200000002980232 || (double) formation3.QuerySystem.FormationPower * (double) closure_0.Team.TeamPower / ((double) formation3.Team.QuerySystem.TeamPower * (double) closure_0.FormationPower) > 0.200000002980232)
                {
                  navMeshVec3 = formation3.QuerySystem.MedianPosition.GetNavMeshVec3();
                  float num4 = navMeshVec3.DistanceSquared(new Vec3(closure_0.AveragePosition, closure_0.MedianPosition.GetNavMeshZ()));
                  if ((double) num4 < (double) num1)
                  {
                    num1 = num4;
                    formation1 = formation3;
                  }
                }
                else if (formation1 == null)
                {
                  navMeshVec3 = formation3.QuerySystem.MedianPosition.GetNavMeshVec3();
                  float num4 = navMeshVec3.DistanceSquared(new Vec3(closure_0.AveragePosition, closure_0.MedianPosition.GetNavMeshZ()));
                  if ((double) num4 < (double) num2)
                  {
                    num2 = num4;
                    formation2 = formation3;
                  }
                }
              }
            }
          }
        }
        return formation1 ?? formation2;
      }), 1.5f);
      this._fastestSignificantlyLargeEnemyFormation = new QueryData<Formation>((Func<Formation>) (() =>
      {
        float num1 = float.MaxValue;
        Formation formation1 = (Formation) null;
        float num2 = float.MaxValue;
        Formation formation2 = (Formation) null;
        double num3 = (double) closure_0.FormationPower / (double) closure_0.Team.TeamPower;
        for (int index1 = 0; index1 < mission.Teams.Count; ++index1)
        {
          TaleWorlds.MountAndBlade.Team team = mission.Teams[index1];
          if (team.IsEnemyOf(formation.Team))
          {
            for (int index2 = 0; index2 < team.FormationsIncludingSpecialAndEmpty.Count; ++index2)
            {
              Formation formation3 = team.FormationsIncludingSpecialAndEmpty[index2];
              Vec3 navMeshVec3;
              if (formation3.CountOfUnits > 0)
              {
                if ((double) formation3.QuerySystem.FormationPower / (double) closure_0.FormationPower > 0.200000002980232 || (double) formation3.QuerySystem.FormationPower * (double) closure_0.Team.TeamPower / ((double) formation3.Team.QuerySystem.TeamPower * (double) closure_0.FormationPower) > 0.200000002980232)
                {
                  navMeshVec3 = formation3.QuerySystem.MedianPosition.GetNavMeshVec3();
                  float num4 = navMeshVec3.DistanceSquared(new Vec3(closure_0.AveragePosition, closure_0.MedianPosition.GetNavMeshZ())) / (formation3.QuerySystem.MovementSpeed * formation3.QuerySystem.MovementSpeed);
                  if ((double) num4 < (double) num1)
                  {
                    num1 = num4;
                    formation1 = formation3;
                  }
                }
                else if (formation1 == null)
                {
                  navMeshVec3 = formation3.QuerySystem.MedianPosition.GetNavMeshVec3();
                  float num4 = navMeshVec3.DistanceSquared(new Vec3(closure_0.AveragePosition, closure_0.MedianPosition.GetNavMeshZ())) / (formation3.QuerySystem.MovementSpeed * formation3.QuerySystem.MovementSpeed);
                  if ((double) num4 < (double) num2)
                  {
                    num2 = num4;
                    formation2 = formation3;
                  }
                }
              }
            }
          }
        }
        return formation1 ?? formation2;
      }), 1.5f);
      this._mainClass = new QueryData<FormationClass>((Func<FormationClass>) (() =>
      {
        FormationClass formationClass = FormationClass.Infantry;
        float num = formationQuerySystem.InfantryUnitRatio;
        if ((double) formationQuerySystem.RangedUnitRatio > (double) num)
        {
          formationClass = FormationClass.Ranged;
          num = formationQuerySystem.RangedUnitRatio;
        }
        if ((double) formationQuerySystem.CavalryUnitRatio > (double) num)
        {
          formationClass = FormationClass.Cavalry;
          num = formationQuerySystem.CavalryUnitRatio;
        }
        if ((double) formationQuerySystem.RangedCavalryUnitRatio > (double) num)
          formationClass = FormationClass.HorseArcher;
        return formationClass;
      }), 15f);
      this._mainFormation = new QueryData<Formation>((Func<Formation>) (() => formation.Team.FormationsIncludingSpecial.FirstOrDefault<Formation>((Func<Formation, bool>) (f => f.AI.IsMainFormation && f != formation))), 15f);
      this._mainFormationReliabilityFactor = new QueryData<float>((Func<float>) (() => formationQuerySystem.MainFormation == null ? 0.0f : (float) ((formationQuerySystem.MainFormation.MovementOrder.OrderEnum == MovementOrder.MovementOrderEnum.Charge || formationQuerySystem.MainFormation.MovementOrder.OrderEnum == MovementOrder.MovementOrderEnum.ChargeToTarget || formationQuerySystem.MainFormation.MovementOrder == (object) MovementOrder.MovementOrderRetreat ? 0.5 : 1.0) * (formationQuerySystem.MainFormation.GetUnderAttackTypeOfUnits(10f) == Agent.UnderAttackType.UnderMeleeAttack ? 0.800000011920929 : 1.0))), 5f);
      this._highGroundCloseToForeseenBattleGround = new QueryData<Vec2>((Func<Vec2>) (() =>
      {
        WorldPosition medianPosition = closure_0.MedianPosition;
        medianPosition.SetVec2(closure_0.AveragePosition);
        WorldPosition formationPosition = closure_0.Team.MedianTargetFormationPosition;
        return mission.FindPositionWithBiggestSlopeTowardsDirectionInSquare(ref medianPosition, closure_0.AveragePosition.Distance(closure_0.Team.MedianTargetFormationPosition.AsVec2) * 0.5f, ref formationPosition).AsVec2;
      }), 10f);
      this.InitializeTelemetryScopeNames();
    }

    public void EvaluateAllPreliminaryQueryData()
    {
      float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
      this._infantryUnitRatio.Evaluate(time);
      this._hasShieldUnitRatio.Evaluate(time);
      this._rangedUnitRatio.Evaluate(time);
      this._cavalryUnitRatio.Evaluate(time);
      this._rangedCavalryUnitRatio.Evaluate(time);
      this._isInfantryFormation.Evaluate(time);
      this._hasShield.Evaluate(time);
      this._isRangedFormation.Evaluate(time);
      this._isCavalryFormation.Evaluate(time);
      this._isRangedCavalryFormation.Evaluate(time);
      this._isMeleeFormation.Evaluate(time);
    }

    public void Expire()
    {
      foreach (FieldInfo field in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
      {
        if (field.GetValue((object) this) is IQueryData queryData1)
        {
          queryData1.Expire();
          field.SetValue((object) this, (object) queryData1);
        }
      }
    }

    public void ExpireAfterUnitAddRemove()
    {
      float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
      this._infantryUnitRatio.Evaluate(time);
      this._hasShieldUnitRatio.Evaluate(time);
      this._rangedUnitRatio.Evaluate(time);
      this._cavalryUnitRatio.Evaluate(time);
      this._rangedCavalryUnitRatio.Evaluate(time);
      this._isMeleeFormation.Evaluate(time);
      this._isInfantryFormation.Evaluate(time);
      this._hasShield.Evaluate(time);
      this._isRangedFormation.Evaluate(time);
      this._isCavalryFormation.Evaluate(time);
      this._isRangedCavalryFormation.Evaluate(time);
      this._mainClass.Evaluate(time);
      if (this.Formation.CountOfUnits != 0)
        return;
      this._infantryUnitRatio.SetValue(0.0f, time);
      this._hasShieldUnitRatio.SetValue(0.0f, time);
      this._rangedUnitRatio.SetValue(0.0f, time);
      this._cavalryUnitRatio.SetValue(0.0f, time);
      this._rangedCavalryUnitRatio.SetValue(0.0f, time);
      this._isMeleeFormation.SetValue(false, time);
      this._isInfantryFormation.SetValue(true, time);
      this._hasShield.SetValue(false, time);
      this._isRangedFormation.SetValue(false, time);
      this._isCavalryFormation.SetValue(false, time);
      this._isRangedCavalryFormation.SetValue(false, time);
    }

    private void InitializeTelemetryScopeNames()
    {
    }

    public Vec2 GetAveragePositionWithMaxAge(float age) => this._averagePosition.GetCachedValueWithMaxAge(age);

    public float GetClassWeightedFactor(
      float infantryWeight,
      float rangedWeight,
      float cavalryWeight,
      float rangedCavalryWeight)
    {
      return (float) ((double) this.InfantryUnitRatio * (double) infantryWeight + (double) this.RangedUnitRatio * (double) rangedWeight + (double) this.CavalryUnitRatio * (double) cavalryWeight + (double) this.RangedCavalryUnitRatio * (double) rangedCavalryWeight);
    }

    public float GetLocalAllyPower(Vec2 target) => this.Formation.Team.FormationsIncludingSpecial.Where<Formation>((Func<Formation, bool>) (f => f != this.Formation)).Sum<Formation>((Func<Formation, float>) (f => f.QuerySystem.FormationPower / f.QuerySystem.AveragePosition.Distance(target)));

    public struct FormationIntegrityDataGroup
    {
      public Vec2 AverageVelocityExcludeFarAgents;
      public float DeviationOfPositionsExcludeFarAgents;
      public float AverageMaxUnlimitedSpeedExcludeFarAgents;
    }
  }
}
