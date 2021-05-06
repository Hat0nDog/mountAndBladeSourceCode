// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FacingOrder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public struct FacingOrder
  {
    internal readonly FacingOrder.FacingOrderEnum OrderEnum;
    private Vec2 lookAtDirection;
    public static readonly FacingOrder FacingOrderLookAtEnemy = new FacingOrder(FacingOrder.FacingOrderEnum.LookAtEnemy);

    public static FacingOrder FacingOrderLookAtDirection(Vec2 direction) => new FacingOrder(FacingOrder.FacingOrderEnum.LookAtDirection, direction);

    private FacingOrder(FacingOrder.FacingOrderEnum orderEnum, Vec2 direction)
    {
      this.OrderEnum = orderEnum;
      this.lookAtDirection = direction;
    }

    private FacingOrder(FacingOrder.FacingOrderEnum orderEnum)
    {
      this.OrderEnum = orderEnum;
      this.lookAtDirection = Vec2.Invalid;
    }

    private Vec2 GetDirectionAux(Formation f, Agent targetAgent)
    {
      if (f.IsMounted() && targetAgent != null && (double) targetAgent.Velocity.LengthSquared > (double) targetAgent.RunSpeedCached * (double) targetAgent.RunSpeedCached * 0.0900000035762787)
        return targetAgent.Velocity.AsVec2.Normalized();
      if (this.OrderEnum == FacingOrder.FacingOrderEnum.LookAtDirection)
        return this.lookAtDirection;
      Vec2 currentPosition = f.CurrentPosition;
      Vec2 averageOfEnemies = Mission.Current.GetWeightedAverageOfEnemies(f.Team, currentPosition, true);
      if (!averageOfEnemies.IsValid)
        return f.Direction;
      Vec2 vec2_1 = averageOfEnemies - currentPosition;
      Vec2 vec2_2 = vec2_1.Normalized();
      vec2_1 = averageOfEnemies - currentPosition;
      float length = vec2_1.Length;
      int enemyUnitCount = f.QuerySystem.Team.EnemyUnitCount;
      int countOfUnits = f.CountOfUnits;
      Vec2 vector2 = f.Direction;
      bool flag = (double) length >= (double) countOfUnits * 0.200000002980232;
      if (enemyUnitCount == 0 || countOfUnits == 0)
        flag = false;
      float num = !flag ? 1f : MBMath.ClampFloat((float) countOfUnits * 1f / (float) enemyUnitCount, 0.3333333f, 3f) * MBMath.ClampFloat(length / (float) countOfUnits, 0.3333333f, 3f);
      if (flag && (double) Math.Abs(vec2_2.AngleBetween(vector2)) > 0.174532920122147 * (double) num)
        vector2 = vec2_2;
      return vector2;
    }

    public OrderType OrderType => this.OrderEnum != FacingOrder.FacingOrderEnum.LookAtDirection ? OrderType.LookAtEnemy : OrderType.LookAtDirection;

    public Vec2 GetDirection(Formation f, Agent targetAgent = null) => this.GetDirectionAux(f, targetAgent);

    internal FacingOrder.FacingOrderEnum GetNativeEnum() => this.OrderEnum;

    public override bool Equals(object obj) => obj is FacingOrder facingOrder && facingOrder == this;

    public override int GetHashCode() => (int) this.OrderEnum;

    public static bool operator !=(FacingOrder f1, FacingOrder f2) => f1.OrderEnum != f2.OrderEnum;

    public static bool operator ==(FacingOrder f1, FacingOrder f2) => f1.OrderEnum == f2.OrderEnum;

    internal enum FacingOrderEnum
    {
      LookAtDirection,
      LookAtEnemy,
    }
  }
}
