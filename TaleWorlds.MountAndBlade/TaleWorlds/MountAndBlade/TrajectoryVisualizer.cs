// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TrajectoryVisualizer
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
  public class TrajectoryVisualizer
  {
    private List<GameEntity> trajectoryPointList;
    protected MatrixFrame initialFrame;
    private GameEntity[] collisionEntity;
    private const int vectorCount = 0;
    private const string trajectoryPointPrefabName = "trajectory_entity";
    private Scene _scene;

    public bool IsVisible { get; private set; }

    public TrajectoryVisualizer(Scene scene)
    {
      this.trajectoryPointList = new List<GameEntity>();
      this._scene = scene;
      this.IsVisible = true;
    }

    public void Init(
      Vec3 startingPosition,
      Vec3 startingVelocity,
      float simulationTime,
      float pointCount)
    {
      this.trajectoryPointList.Clear();
      this.collisionEntity = new GameEntity[0];
      for (int index = 0; index < this.collisionEntity.Length; ++index)
      {
        this.collisionEntity[index] = GameEntity.Instantiate(this._scene, "trajectory_entity", true);
        this.collisionEntity[index].SetMobility(GameEntity.Mobility.dynamic);
        MatrixFrame frame = this.collisionEntity[index].GetFrame();
        frame.Scale(new Vec3(2f, 2f, 2f));
        this.collisionEntity[index].SetFrame(ref frame);
        this.collisionEntity[index].EntityFlags |= EntityFlags.NonModifiableFromEditor;
        this.collisionEntity[index].EntityFlags |= EntityFlags.DontSaveToScene;
        this.collisionEntity[index].EntityFlags |= EntityFlags.DoesNotAffectParentsLocalBb;
        this.collisionEntity[index].EntityFlags |= EntityFlags.NotAffectedBySeason;
      }
      float num1 = simulationTime / pointCount;
      for (float num2 = 0.0f; (double) num2 < (double) simulationTime && (double) this.trajectoryPointList.Count < (double) pointCount; num2 += num1)
      {
        Vec3 vec3_1 = startingVelocity + MBGlobals.GravityVec3 * num2;
        Vec3 vec3_2 = startingPosition + vec3_1 * num2;
        GameEntity gameEntity = GameEntity.Instantiate(this._scene, "trajectory_entity", true);
        gameEntity.SetMobility(GameEntity.Mobility.dynamic);
        gameEntity.EntityFlags |= EntityFlags.NonModifiableFromEditor;
        gameEntity.EntityFlags |= EntityFlags.DontSaveToScene;
        gameEntity.EntityFlags |= EntityFlags.DoesNotAffectParentsLocalBb;
        gameEntity.EntityFlags |= EntityFlags.NotAffectedBySeason;
        MatrixFrame frame = gameEntity.GetFrame();
        frame.origin = vec3_2;
        gameEntity.SetFrame(ref frame);
        this.trajectoryPointList.Add(gameEntity);
      }
    }

    public void Clear()
    {
      foreach (GameEntity trajectoryPoint in this.trajectoryPointList)
        trajectoryPoint.Remove(86);
      this.trajectoryPointList.Clear();
      for (int index = 0; index < this.collisionEntity.Length; ++index)
        this.collisionEntity[index].Remove(87);
    }

    public void Update(
      Vec3 position,
      Vec3 velocity,
      float simulationTime,
      float pointCount,
      ItemObject missileItem)
    {
      if (missileItem != null)
        this.UpdateCollisionSpheres(position, velocity, missileItem);
      double num1 = (double) simulationTime / (double) pointCount;
      int num2 = 0;
      int num3 = 0;
      float num4 = (float) (num1 / 10.0);
      for (float num5 = 0.0f; (double) num5 < (double) simulationTime - 9.99999974737875E-06 && (double) num2 < (double) pointCount; num5 += num4)
      {
        velocity += MBGlobals.GravityVec3 * num4;
        position += velocity * num4;
        if (num3 % 10 == 0)
        {
          GameEntity trajectoryPoint = this.trajectoryPointList[num2++];
          MatrixFrame frame = trajectoryPoint.GetFrame();
          frame.origin = position;
          trajectoryPoint.SetFrame(ref frame);
        }
        ++num3;
      }
    }

    private void UpdateCollisionSpheres(Vec3 position, Vec3 velocity, ItemObject missileItem)
    {
      MissionWeapon missionWeapon = new MissionWeapon(missileItem, (ItemModifier) null, (Banner) null);
      Vec3 vec3_1 = velocity;
      double num1 = (double) vec3_1.Normalize();
      Vec3 va = vec3_1;
      double num2 = (double) va.Normalize();
      float x = va.x;
      float y = va.y;
      float z = va.z;
      float num3 = (float) ((double) x * (double) position.x + (double) y * (double) position.y + (double) z * (double) position.z);
      Vec3 vb = new Vec3(1f, 1f);
      float num4 = (double) Math.Abs(z - 0.0f) < 1.40129846432482E-45 ? 1f : z;
      vb.z = (float) (-(double) num3 - (double) x * (1.0 - (double) position.x) - (double) y * (1.0 - (double) position.y) + (double) num4 * (double) position.z) / num4;
      double num5 = (double) vb.Normalize();
      Vec3 vec3_2 = Vec3.CrossProduct(va, vb);
      double num6 = (double) vec3_2.Normalize();
      Vec3[] vec3Array = new Vec3[0];
      for (int index = 0; index < 0; ++index)
      {
        float num7 = (float) ((double) index * 360.0 / 0.0 * 3.14159274101257 / 180.0);
        vec3Array[index] = va + vb * (float) Math.Sin((double) num7) * 0.1f + vec3_2 * (float) Math.Cos((double) num7) * 0.1f;
      }
      for (int index = 0; index < vec3Array.Length; ++index)
      {
        Vec3 missileDirection = vec3Array[index];
        WeaponData weaponData = missionWeapon.GetWeaponData(false);
        Vec3 missileCollisionPoint = Mission.Current.GetMissileCollisionPoint(position, missileDirection, velocity.Length, in weaponData);
        MatrixFrame frame = this.collisionEntity[index].GetFrame();
        frame.origin = missileCollisionPoint;
        this.collisionEntity[index].SetFrame(ref frame);
      }
    }

    public void SetVisible(bool isVisible)
    {
      if (this.IsVisible == isVisible)
        return;
      this.IsVisible = isVisible;
      foreach (GameEntity trajectoryPoint in this.trajectoryPointList)
        trajectoryPoint.SetVisibilityExcludeParents(isVisible);
      foreach (GameEntity gameEntity in this.collisionEntity)
        gameEntity.SetVisibilityExcludeParents(isVisible);
    }
  }
}
