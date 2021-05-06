// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BattleSideDeploymentPlan
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BattleSideDeploymentPlan
  {
    public const float VerticalFormationGap = 3f;
    public const float HorizontalFormationGap = 2f;
    private readonly int[] _formationTroopCounts = new int[11];
    private readonly FormationDeploymentPlan[] _formationDeploymentPlans = new FormationDeploymentPlan[11];
    private readonly SortedList<FormationDeploymentOrder, FormationDeploymentPlan>[] _deploymentFlanks = new SortedList<FormationDeploymentOrder, FormationDeploymentPlan>[4];
    public readonly BattleSideEnum Side;
    public bool SpawnWithHorses;

    public int TroopCount
    {
      get
      {
        int num = 0;
        foreach (int formationTroopCount in this._formationTroopCounts)
          num += formationTroopCount;
        return num;
      }
    }

    public BattleSideDeploymentPlan(BattleSideEnum side)
    {
      this.Side = side;
      this.SpawnWithHorses = false;
      for (int index = 0; index < this._formationDeploymentPlans.Length; ++index)
      {
        FormationClass fClass = (FormationClass) index;
        this._formationDeploymentPlans[index] = new FormationDeploymentPlan(fClass);
      }
      for (int index = 0; index < 4; ++index)
        this._deploymentFlanks[index] = new SortedList<FormationDeploymentOrder, FormationDeploymentPlan>((IComparer<FormationDeploymentOrder>) FormationDeploymentOrder.GetComparer());
      this.ClearTroops();
      this.ClearPlan();
    }

    public static WorldFrame GetFrameFromFormationSpawnEntity(
      GameEntity formationSpawnEntity)
    {
      MatrixFrame globalFrame = formationSpawnEntity.GetGlobalFrame();
      globalFrame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();
      WorldPosition origin = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, globalFrame.origin, false);
      return new WorldFrame(globalFrame.rotation, origin);
    }

    public void AddTroops(FormationClass formationClass, int troopCount)
    {
      int index = (int) formationClass;
      if (troopCount <= 0 || index >= this._formationTroopCounts.Length)
        return;
      this._formationTroopCounts[index] += troopCount;
    }

    public void ClearPlan()
    {
      foreach (FormationDeploymentPlan formationDeploymentPlan in this._formationDeploymentPlans)
        formationDeploymentPlan.Clear();
      foreach (SortedList<FormationDeploymentOrder, FormationDeploymentPlan> deploymentFlank in this._deploymentFlanks)
        deploymentFlank.Clear();
    }

    public void ClearTroops()
    {
      for (int index = 0; index < this._formationTroopCounts.Length; ++index)
        this._formationTroopCounts[index] = 0;
    }

    public void PlanFieldBattleDeploymentAtPosition(
      Vec2 deployPosition,
      Vec2 deployDirection,
      bool isReinforcement = false)
    {
      for (int index = 0; index < this._formationDeploymentPlans.Length; ++index)
      {
        int formationTroopCount = this._formationTroopCounts[index];
        FormationDeploymentPlan formationDeploymentPlan = this._formationDeploymentPlans[index];
        FormationDeploymentFlank defaultFlank = formationDeploymentPlan.GetDefaultFlank(this.SpawnWithHorses);
        (float width2, float depth2) = MissionDeploymentPlan.GetFormationSpawnWidthAndDepth(formationDeploymentPlan.Class, formationTroopCount, !this.SpawnWithHorses);
        formationDeploymentPlan.SetPlannedDimensions(width2, depth2);
        formationDeploymentPlan.SetPlannedTroopCount(formationTroopCount);
        int offset = formationTroopCount > 0 ? 0 : 1;
        FormationDeploymentOrder deploymentOrder = formationDeploymentPlan.GetDeploymentOrder(offset);
        this._deploymentFlanks[(int) defaultFlank].Add(deploymentOrder, formationDeploymentPlan);
      }
      (float flankWidth, float flankDepth) tuple = this.PlanFlankDeployment(FormationDeploymentFlank.Front, deployPosition, deployDirection, isReinforcement: isReinforcement);
      float flankWidth1 = tuple.flankWidth;
      float verticalOffset1 = tuple.flankDepth + 3f;
      float flankWidth2 = this.PlanFlankDeployment(FormationDeploymentFlank.Rear, deployPosition, deployDirection, verticalOffset1, isReinforcement: isReinforcement).flankWidth;
      float num = Math.Max(flankWidth1, flankWidth2);
      float verticalOffset2 = this.ComputeFlankDepth(FormationDeploymentFlank.Front, true) + 3f;
      float flankWidth3 = this.ComputeFlankWidth(FormationDeploymentFlank.Left);
      float horizontalOffset1 = (float) (2.0 + 0.5 * ((double) num + (double) flankWidth3));
      this.PlanFlankDeployment(FormationDeploymentFlank.Left, deployPosition, deployDirection, verticalOffset2, horizontalOffset1, isReinforcement);
      float flankWidth4 = this.ComputeFlankWidth(FormationDeploymentFlank.Right);
      float horizontalOffset2 = (float) (-1.0 * (2.0 + 0.5 * ((double) num + (double) flankWidth4)));
      this.PlanFlankDeployment(FormationDeploymentFlank.Right, deployPosition, deployDirection, verticalOffset2, horizontalOffset2, isReinforcement);
      for (int index = 0; index < this._deploymentFlanks.Length; ++index)
        this._deploymentFlanks[index].Clear();
    }

    public void PlanFieldBattleDeploymentFromSceneData(
      int battleSize,
      FormationSceneEntry[,] formationSceneEntries,
      bool isReinforcement = false)
    {
      if (formationSceneEntries == null || formationSceneEntries.GetLength(0) != 2 || formationSceneEntries.GetLength(1) != this._formationDeploymentPlans.Length)
        return;
      int side = (int) this.Side;
      int index1 = this.Side == BattleSideEnum.Attacker ? 0 : 1;
      Scene scene = Mission.Current.Scene;
      for (int index2 = 0; index2 < this._formationDeploymentPlans.Length; ++index2)
      {
        GameEntity defaultEntity1 = formationSceneEntries[side, index2].DefaultEntity;
        GameEntity defaultEntity2 = formationSceneEntries[index1, index2].DefaultEntity;
        WorldFrame? frame = new WorldFrame?();
        if ((NativeObject) defaultEntity1 != (NativeObject) null && (NativeObject) defaultEntity2 != (NativeObject) null)
        {
          Vec3 globalPosition = defaultEntity1.GlobalPosition;
          Vec3 vec3_1 = defaultEntity2.GlobalPosition - globalPosition;
          Vec3 vec3_2 = globalPosition;
          float num1 = vec3_1.Normalize();
          float num2 = 1000f / num1;
          float num3 = (float) (0.0799999982118607 + 0.0799999982118607 * Math.Pow((double) battleSize, 0.300000011920929)) * num2;
          float num4 = Math.Max(0.0f, (float) (0.5 - 0.5 * (double) num3));
          float num5 = Math.Min(1f, (float) (0.5 + 0.5 * (double) num3));
          Vec3 position = vec3_2 + vec3_1 * (num1 * num4);
          Vec3 vec3_3 = vec3_2 + vec3_1 * (num1 * num5) - position;
          double num6 = (double) vec3_3.Normalize();
          Mat3 identity = Mat3.Identity;
          identity.RotateAboutUp(vec3_3.AsVec2.RotationInRadians);
          WorldPosition origin = new WorldPosition(scene, UIntPtr.Zero, position, false);
          frame = new WorldFrame?(new WorldFrame(identity, origin));
        }
        this._formationDeploymentPlans[index2].SetFrame(frame, isReinforcement);
      }
    }

    public void PlanBattleDeploymentFromSceneData(FormationSceneEntry[,] formationSceneEntries)
    {
      if (formationSceneEntries == null || formationSceneEntries.GetLength(0) != 2 || formationSceneEntries.GetLength(1) != this._formationDeploymentPlans.Length)
        return;
      int side = (int) this.Side;
      for (int index = 0; index < this._formationDeploymentPlans.Length; ++index)
      {
        GameEntity defaultEntity = formationSceneEntries[side, index].DefaultEntity;
        WorldFrame? frame1 = (NativeObject) defaultEntity != (NativeObject) null ? new WorldFrame?(BattleSideDeploymentPlan.GetFrameFromFormationSpawnEntity(defaultEntity)) : new WorldFrame?();
        this._formationDeploymentPlans[index].SetFrame(frame1, false);
        GameEntity reinforcementEntity = formationSceneEntries[side, index].ReinforcementEntity;
        WorldFrame? frame2 = (NativeObject) reinforcementEntity != (NativeObject) null ? new WorldFrame?(BattleSideDeploymentPlan.GetFrameFromFormationSpawnEntity(reinforcementEntity)) : frame1;
        this._formationDeploymentPlans[index].SetFrame(frame2, true);
      }
    }

    public FormationDeploymentPlan GetFormationPlan(FormationClass fClass) => this._formationDeploymentPlans[(int) fClass];

    private (float flankWidth, float flankDepth) PlanFlankDeployment(
      FormationDeploymentFlank flankFlank,
      Vec2 deployPosition,
      Vec2 deployDirection,
      float verticalOffset = 0.0f,
      float horizontalOffset = 0.0f,
      bool isReinforcement = false)
    {
      Mat3 identity = Mat3.Identity;
      identity.RotateAboutUp(deployDirection.RotationInRadians);
      float val1 = 0.0f;
      float num1 = 0.0f;
      Vec2 vec2_1 = deployDirection.LeftVec();
      foreach (KeyValuePair<FormationDeploymentOrder, FormationDeploymentPlan> keyValuePair in this._deploymentFlanks[(int) flankFlank])
      {
        FormationDeploymentPlan formationDeploymentPlan = keyValuePair.Value;
        Vec2 vec2_2 = deployPosition - (num1 + verticalOffset) * deployDirection + horizontalOffset * vec2_1;
        WorldPosition origin = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, vec2_2.ToVec3(), false);
        IntPtr navMesh = (IntPtr) origin.GetNavMesh();
        WorldFrame worldFrame = new WorldFrame(identity, origin);
        formationDeploymentPlan.SetFrame(new WorldFrame?(worldFrame), isReinforcement);
        float num2 = formationDeploymentPlan.PlannedDepth + 3f;
        num1 += num2;
        val1 = Math.Max(val1, formationDeploymentPlan.PlannedWidth);
      }
      float num3 = Math.Max(num1 - 3f, 0.0f);
      return (val1, num3);
    }

    private float ComputeFlankWidth(FormationDeploymentFlank flank)
    {
      float val1 = 0.0f;
      foreach (KeyValuePair<FormationDeploymentOrder, FormationDeploymentPlan> keyValuePair in this._deploymentFlanks[(int) flank])
        val1 = Math.Max(val1, keyValuePair.Value.PlannedWidth);
      return val1;
    }

    private float ComputeFlankDepth(FormationDeploymentFlank flank, bool countPositiveTroops = false)
    {
      float num = 0.0f;
      foreach (KeyValuePair<FormationDeploymentOrder, FormationDeploymentPlan> keyValuePair in this._deploymentFlanks[(int) flank])
      {
        if (!countPositiveTroops)
          num += keyValuePair.Value.PlannedDepth + 3f;
        else if (keyValuePair.Value.PlannedTroopCount > 0)
          num += keyValuePair.Value.PlannedDepth + 3f;
      }
      return num - 3f;
    }
  }
}
