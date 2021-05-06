// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SquareFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class SquareFormation : LineFormation
  {
    private bool _disableRearOfLastRank;

    private int UnitCountOfOuterSide => MBMath.Ceiling((float) this.FileCount / 4f) + 1;

    private int MaxRank => (this.UnitCountOfOuterSide + 1) / 2;

    private new float Distance => this.Interval;

    protected bool DisableRearOfLastRank
    {
      get => this._disableRearOfLastRank;
      set
      {
        if (this._disableRearOfLastRank == value)
          return;
        this._disableRearOfLastRank = value;
        this.OnFormationFrameChanged();
      }
    }

    public SquareFormation(IFormation owner)
      : base(owner, true, true)
    {
    }

    public override IFormationArrangement Clone(IFormation formation) => (IFormationArrangement) new SquareFormation(formation);

    public override void DeepCopyFrom(IFormationArrangement arrangement)
    {
      base.DeepCopyFrom(arrangement);
      this.DisableRearOfLastRank = (arrangement as SquareFormation).DisableRearOfLastRank;
    }

    public void FormFromBorderWidth(float borderLineWidth) => this.FormFromBorderWidth(Math.Max(1, (int) (((double) borderLineWidth - (double) this.UnitDiameter) / ((double) this.Interval + (double) this.UnitDiameter) + 9.99999974737875E-06)) + 1);

    public void FormFromBorderWidth(int unitCountOfBorderLine)
    {
      if (unitCountOfBorderLine == 1)
        this.Width = this.UnitDiameter;
      else
        this.Width = (float) (4 * (unitCountOfBorderLine - 1) - 1) * (this.Interval + this.UnitDiameter) + this.UnitDiameter;
    }

    public void FormFromDepth(int depth)
    {
      int countWithOverride = this.GetUnitCountWithOverride();
      depth = Math.Min(SquareFormation.GetMaximumDepth(countWithOverride, out int _), depth);
      double num1 = (double) countWithOverride / (4.0 * (double) depth) + (double) depth;
      int unitCountOfBorderLine = MBMath.Ceiling((float) num1);
      int num2 = MBMath.Round((float) num1);
      if (num2 < unitCountOfBorderLine && num2 * num2 == countWithOverride)
        unitCountOfBorderLine = num2;
      if (unitCountOfBorderLine == 0)
        unitCountOfBorderLine = 1;
      this.FormFromBorderWidth(unitCountOfBorderLine);
    }

    protected static int GetMaximumDepth(int unitCount, out int minimumWidth)
    {
      int num = (int) Math.Sqrt((double) unitCount);
      if (num * num != unitCount)
        ++num;
      minimumWidth = num;
      return Math.Max(1, (num + 1) / 2);
    }

    private SquareFormation.Side GetSideOfUnitPosition(int fileIndex) => (SquareFormation.Side) (fileIndex / (this.UnitCountOfOuterSide - 1));

    private SquareFormation.Side? GetSideOfUnitPosition(int fileIndex, int rankIndex)
    {
      SquareFormation.Side sideOfUnitPosition = this.GetSideOfUnitPosition(fileIndex);
      if (rankIndex == 0)
        return new SquareFormation.Side?(sideOfUnitPosition);
      int num1 = this.UnitCountOfOuterSide - 2 * rankIndex;
      if (num1 == 1 && sideOfUnitPosition != SquareFormation.Side.Front)
        return new SquareFormation.Side?();
      int num2 = fileIndex % (this.UnitCountOfOuterSide - 1);
      int num3 = (this.UnitCountOfOuterSide - num1) / 2;
      return num2 >= num3 && this.UnitCountOfOuterSide - num2 - 1 > num3 ? new SquareFormation.Side?(sideOfUnitPosition) : new SquareFormation.Side?();
    }

    private Vec2 GetLocalPositionOfUnitAux(int fileIndex, int rankIndex)
    {
      if (this.UnitCountOfOuterSide == 1)
        return Vec2.Zero;
      SquareFormation.Side sideOfUnitPosition = this.GetSideOfUnitPosition(fileIndex);
      float num1 = (float) (this.UnitCountOfOuterSide - 1) * (this.Interval + this.UnitDiameter);
      float num2 = (float) (fileIndex % (this.UnitCountOfOuterSide - 1)) * (this.Interval + this.UnitDiameter);
      float num3 = (float) rankIndex * (this.Distance + this.UnitDiameter);
      Vec2 vec2;
      switch (sideOfUnitPosition)
      {
        case SquareFormation.Side.Front:
          vec2 = new Vec2((float) (-(double) num1 / 2.0), 0.0f) + new Vec2(num2, -num3);
          break;
        case SquareFormation.Side.Right:
          vec2 = new Vec2(num1 / 2f, 0.0f) + new Vec2(-num3, -num2);
          break;
        case SquareFormation.Side.Rear:
          vec2 = new Vec2(num1 / 2f, -num1) + new Vec2(-num2, num3);
          break;
        case SquareFormation.Side.Left:
          vec2 = new Vec2((float) (-(double) num1 / 2.0), -num1) + new Vec2(num3, num2);
          break;
        default:
          vec2 = Vec2.Zero;
          break;
      }
      return vec2;
    }

    protected override Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex) => this.GetLocalPositionOfUnitAux(this.ShiftFileIndex(fileIndex), rankIndex);

    protected override Vec2 GetLocalDirectionOfUnit(int fileIndex, int rankIndex)
    {
      switch (this.GetSideOfUnitPosition(this.ShiftFileIndex(fileIndex)))
      {
        case SquareFormation.Side.Front:
          return Vec2.Forward;
        case SquareFormation.Side.Right:
          return Vec2.Side;
        case SquareFormation.Side.Rear:
          return -Vec2.Forward;
        case SquareFormation.Side.Left:
          return -Vec2.Side;
        default:
          return Vec2.Forward;
      }
    }

    protected override bool IsUnitPositionRestrained(int fileIndex, int rankIndex)
    {
      if (base.IsUnitPositionRestrained(fileIndex, rankIndex) || rankIndex >= this.MaxRank)
        return true;
      int fileIndex1 = this.ShiftFileIndex(fileIndex);
      SquareFormation.Side? sideOfUnitPosition = this.GetSideOfUnitPosition(fileIndex1, rankIndex);
      return this.DisableRearOfLastRank && rankIndex == 0 && (sideOfUnitPosition.HasValue && fileIndex1 >= (this.UnitCountOfOuterSide - 1) * 2) && fileIndex1 <= (this.UnitCountOfOuterSide - 1) * 3 || !sideOfUnitPosition.HasValue;
    }

    protected override IEnumerable<Vec2i> GetRestrainedUnitPositions()
    {
      SquareFormation squareFormation = this;
      for (int fileIndex = 0; fileIndex < squareFormation.FileCount; ++fileIndex)
      {
        // ISSUE: explicit non-virtual call
        for (int rankIndex = 0; rankIndex < __nonvirtual (squareFormation.RankCount); ++rankIndex)
        {
          if (squareFormation.IsUnitPositionRestrained(fileIndex, rankIndex))
            yield return new Vec2i(fileIndex, rankIndex);
        }
      }
    }

    private SquareFormation.Side GetSideOfLocalPosition(Vec2 localPosition)
    {
      Vec2 vec2_1 = new Vec2(0.0f, (float) (-(double) ((float) (this.UnitCountOfOuterSide - 1) * (this.Interval + this.UnitDiameter)) / 2.0));
      Vec2 vec2_2 = localPosition - vec2_1;
      vec2_2.y *= (float) (((double) this.Interval + (double) this.UnitDiameter) / ((double) this.Distance + (double) this.UnitDiameter));
      float rotationInRadians = vec2_2.RotationInRadians;
      if ((double) rotationInRadians < 0.0)
        rotationInRadians += 6.283185f;
      if ((double) rotationInRadians <= 0.78639817237854 || (double) rotationInRadians > 5.49878740310669)
        return SquareFormation.Side.Front;
      if ((double) rotationInRadians <= 2.35719442367554)
        return SquareFormation.Side.Left;
      return (double) rotationInRadians <= 3.92799091339111 ? SquareFormation.Side.Rear : SquareFormation.Side.Right;
    }

    protected override bool TryGetUnitPositionIndexFromLocalPosition(
      Vec2 localPosition,
      out int fileIndex,
      out int rankIndex)
    {
      SquareFormation.Side sideOfLocalPosition = this.GetSideOfLocalPosition(localPosition);
      float num1 = (float) (this.UnitCountOfOuterSide - 1) * (this.Interval + this.UnitDiameter);
      float num2;
      float num3;
      switch (sideOfLocalPosition)
      {
        case SquareFormation.Side.Front:
          Vec2 vec2_1 = localPosition - new Vec2((float) (-(double) num1 / 2.0), 0.0f);
          num2 = vec2_1.x;
          num3 = -vec2_1.y;
          break;
        case SquareFormation.Side.Right:
          Vec2 vec2_2 = localPosition - new Vec2(num1 / 2f, 0.0f);
          num2 = -vec2_2.y;
          num3 = -vec2_2.x;
          break;
        case SquareFormation.Side.Rear:
          Vec2 vec2_3 = localPosition - new Vec2(num1 / 2f, -num1);
          num2 = -vec2_3.x;
          num3 = vec2_3.y;
          break;
        case SquareFormation.Side.Left:
          Vec2 vec2_4 = localPosition - new Vec2((float) (-(double) num1 / 2.0), -num1);
          num2 = vec2_4.y;
          num3 = vec2_4.x;
          break;
        default:
          Vec2 zero = Vec2.Zero;
          num2 = 0.0f;
          num3 = 0.0f;
          break;
      }
      rankIndex = MBMath.Round(num3 / (this.Distance + this.UnitDiameter));
      if (rankIndex < 0 || rankIndex >= this.RankCount || rankIndex >= this.MaxRank)
      {
        fileIndex = -1;
        return false;
      }
      int num4 = MBMath.Round(num2 / (this.Interval + this.UnitDiameter));
      if (num4 >= this.UnitCountOfOuterSide - 1)
      {
        fileIndex = 1;
        return false;
      }
      int shiftedFileIndex = num4 + (this.UnitCountOfOuterSide - 1) * (int) sideOfLocalPosition;
      fileIndex = this.UnshiftFileIndex(shiftedFileIndex);
      return fileIndex >= 0 && fileIndex < this.FileCount;
    }

    private int ShiftFileIndex(int fileIndex)
    {
      int num1 = this.UnitCountOfOuterSide + this.UnitCountOfOuterSide / 2 - 2;
      int num2 = fileIndex - num1;
      if (num2 < 0)
        num2 += (this.UnitCountOfOuterSide - 1) * 4;
      return num2;
    }

    private int UnshiftFileIndex(int shiftedFileIndex)
    {
      int num1 = this.UnitCountOfOuterSide + this.UnitCountOfOuterSide / 2 - 2;
      int num2 = shiftedFileIndex + num1;
      if (num2 >= (this.UnitCountOfOuterSide - 1) * 4)
        num2 -= (this.UnitCountOfOuterSide - 1) * 4;
      return num2;
    }

    private enum Side
    {
      Front,
      Right,
      Rear,
      Left,
    }
  }
}
