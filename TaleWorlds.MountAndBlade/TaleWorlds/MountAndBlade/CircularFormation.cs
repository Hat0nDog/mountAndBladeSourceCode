// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CircularFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class CircularFormation : LineFormation
  {
    public CircularFormation(IFormation owner)
      : base(owner, true, true)
    {
    }

    public override IFormationArrangement Clone(IFormation formation) => (IFormationArrangement) new CircularFormation(formation);

    private float GetDistanceFromCenterOfRank(int rankIndex)
    {
      float num = this.Radius - (float) rankIndex * (this.Distance + this.UnitDiameter);
      return (double) num >= 0.0 ? num : 0.0f;
    }

    protected override bool IsDeepenApplicable() => (double) this.Radius - (double) this.RankCount * ((double) this.Distance + (double) this.UnitDiameter) >= 0.0;

    protected override bool IsNarrowApplicable(int amount) => ((double) (this.FileCount - 1 - amount) * ((double) this.Interval + (double) this.UnitDiameter) + (double) this.UnitDiameter) / 6.28318548202515 - (double) this.RankCount * ((double) this.Distance + (double) this.UnitDiameter) >= 0.0;

    private int GetUnitCountOfRank(int rankIndex) => rankIndex == 0 ? this.FileCount : Math.Max(1, MBMath.Floor((float) (6.28318548202515 * (double) this.GetDistanceFromCenterOfRank(rankIndex) / ((double) this.Interval + (double) this.UnitDiameter))));

    private float Radius => this.Width / 6.283185f;

    private int MaxRank => MBMath.Floor(this.Radius / (this.Distance + this.UnitDiameter));

    protected override bool IsUnitPositionRestrained(int fileIndex, int rankIndex)
    {
      if (base.IsUnitPositionRestrained(fileIndex, rankIndex) || rankIndex > this.MaxRank)
        return true;
      int unitCountOfRank = this.GetUnitCountOfRank(rankIndex);
      int num = (this.FileCount - unitCountOfRank) / 2;
      return fileIndex < num || fileIndex >= num + unitCountOfRank;
    }

    protected override IEnumerable<Vec2i> GetRestrainedUnitPositions()
    {
      CircularFormation circularFormation = this;
      for (int fileIndex = 0; fileIndex < circularFormation.FileCount; ++fileIndex)
      {
        // ISSUE: explicit non-virtual call
        for (int rankIndex = 0; rankIndex < __nonvirtual (circularFormation.RankCount); ++rankIndex)
        {
          if (circularFormation.IsUnitPositionRestrained(fileIndex, rankIndex))
            yield return new Vec2i(fileIndex, rankIndex);
        }
      }
    }

    protected override Vec2 GetLocalDirectionOfUnit(int fileIndex, int rankIndex)
    {
      int unitCountOfRank = this.GetUnitCountOfRank(rankIndex);
      int num = (this.FileCount - unitCountOfRank) / 2;
      Vec2 vec2 = Vec2.FromRotation((float) ((double) ((fileIndex - num) * 2) * 3.14159274101257 / (double) unitCountOfRank + 3.14159274101257));
      vec2.x *= -1f;
      return vec2;
    }

    protected override Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex) => new Vec2(0.0f, -this.Radius) + this.GetLocalDirectionOfUnit(fileIndex, rankIndex) * this.GetDistanceFromCenterOfRank(rankIndex);

    protected override bool TryGetUnitPositionIndexFromLocalPosition(
      Vec2 localPosition,
      out int fileIndex,
      out int rankIndex)
    {
      Vec2 vec2_1 = new Vec2(0.0f, -this.Radius);
      Vec2 vec2_2 = localPosition - vec2_1;
      float length = vec2_2.Length;
      rankIndex = MBMath.Round((float) (((double) length - (double) this.Radius) / ((double) this.Distance + (double) this.UnitDiameter) * -1.0));
      if (rankIndex < 0 || rankIndex >= this.RankCount)
      {
        fileIndex = -1;
        return false;
      }
      if ((double) this.Radius - (double) rankIndex * ((double) this.Distance + (double) this.UnitDiameter) < 0.0)
      {
        fileIndex = -1;
        return false;
      }
      int unitCountOfRank = this.GetUnitCountOfRank(rankIndex);
      int num1 = (this.FileCount - unitCountOfRank) / 2;
      vec2_2.x *= -1f;
      float num2 = vec2_2.RotationInRadians - 3.141593f;
      if ((double) num2 < 0.0)
        num2 += 6.283185f;
      int num3 = MBMath.Round((float) ((double) num2 / 2.0 / 3.14159274101257) * (float) unitCountOfRank);
      fileIndex = num3 + num1;
      return fileIndex >= 0 && fileIndex < this.FileCount;
    }

    protected int GetMaximumDepth(int unitCount)
    {
      int val1 = 0;
      int num = 0;
      while (num < unitCount)
      {
        int val2 = MBMath.Floor((float) (6.28318548202515 * (double) ((float) val1 * (this.Distance + this.UnitDiameter)) / ((double) this.Interval + (double) this.UnitDiameter)));
        num += Math.Max(1, val2);
        ++val1;
      }
      return Math.Max(val1, 1);
    }

    public void FormFromDepth(int depth)
    {
      int countWithOverride = this.GetUnitCountWithOverride();
      depth = Math.Min(this.GetMaximumDepth(countWithOverride), depth);
      float num1 = (float) (6.28318548202515 * ((double) this.Distance + (double) this.UnitDiameter) / ((double) this.Interval + (double) this.UnitDiameter));
      int num2 = MBMath.Round((float) (depth * (depth - 1) / 2) * num1);
      this.FormFromCircumference((float) MBMath.Round((float) ((countWithOverride + num2) / depth)) * (this.Interval + this.UnitDiameter));
    }

    public void FormFromCircumference(float circumference)
    {
      int countWithOverride = this.GetUnitCountWithOverride();
      int maximumDepth = this.GetMaximumDepth(countWithOverride);
      float num1 = (float) (6.28318548202515 * ((double) this.Distance + (double) this.UnitDiameter) / ((double) this.Interval + (double) this.UnitDiameter));
      int num2 = MBMath.Round((float) (maximumDepth * (maximumDepth - 1) / 2) * num1);
      float minValue = (float) Math.Max(0, Math.Min(MBMath.Round((float) ((countWithOverride + num2) / maximumDepth)), countWithOverride) - 1) * (this.Interval + this.UnitDiameter);
      float maxValue = (float) Math.Max(0, countWithOverride - 1) * (this.Interval + this.UnitDiameter);
      circumference = MBMath.ClampFloat(circumference, minValue, maxValue);
      this.Width = circumference + this.UnitDiameter;
    }
  }
}
