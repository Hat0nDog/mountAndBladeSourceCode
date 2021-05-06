// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SkeinFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class SkeinFormation : LineFormation
  {
    public SkeinFormation(IFormation owner)
      : base(owner)
    {
    }

    public override IFormationArrangement Clone(IFormation formation) => (IFormationArrangement) new SkeinFormation(formation);

    protected override Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
    {
      float num = (float) (this.FileCount - 1) * (this.Interval + this.UnitDiameter);
      Vec2 vec2 = new Vec2((float) ((double) fileIndex * ((double) this.Interval + (double) this.UnitDiameter) - (double) num / 2.0), (float) -rankIndex * (this.Distance + this.UnitDiameter));
      float offsetOfFile = this.GetOffsetOfFile(fileIndex);
      vec2.y -= offsetOfFile;
      return vec2;
    }

    private float GetOffsetOfFile(int fileIndex)
    {
      int num = this.FileCount / 2;
      return (float) ((double) Math.Abs(fileIndex - num) * ((double) this.Interval + (double) this.UnitDiameter) / 2.0);
    }

    protected override bool TryGetUnitPositionIndexFromLocalPosition(
      Vec2 localPosition,
      out int fileIndex,
      out int rankIndex)
    {
      float num = (float) (this.FileCount - 1) * (this.Interval + this.UnitDiameter);
      fileIndex = (int) Math.Round(((double) localPosition.x + (double) num / 2.0) / ((double) this.Interval + (double) this.UnitDiameter));
      if (fileIndex < 0 || fileIndex >= this.FileCount)
      {
        rankIndex = -1;
        return false;
      }
      float offsetOfFile = this.GetOffsetOfFile(fileIndex);
      localPosition.y += offsetOfFile;
      rankIndex = (int) Math.Round(-(double) localPosition.y / ((double) this.Distance + (double) this.UnitDiameter));
      if (rankIndex >= 0 && rankIndex < this.RankCount)
        return true;
      fileIndex = -1;
      return false;
    }
  }
}
