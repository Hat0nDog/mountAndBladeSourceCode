// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.WedgeFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class WedgeFormation : LineFormation
  {
    public WedgeFormation(IFormation owner)
      : base(owner)
    {
    }

    public override IFormationArrangement Clone(IFormation formation) => (IFormationArrangement) new WedgeFormation(formation);

    private int GetUnitCountOfRank(int rankIndex) => Math.Min(this.FileCount, rankIndex * 2 * 3 + 3);

    protected override bool IsUnitPositionRestrained(int fileIndex, int rankIndex)
    {
      if (base.IsUnitPositionRestrained(fileIndex, rankIndex))
        return true;
      int unitCountOfRank = this.GetUnitCountOfRank(rankIndex);
      int num = (this.FileCount - unitCountOfRank) / 2;
      return fileIndex < num || fileIndex >= num + unitCountOfRank;
    }

    protected override IEnumerable<Vec2i> GetRestrainedUnitPositions()
    {
      WedgeFormation wedgeFormation = this;
      for (int fileIndex = 0; fileIndex < wedgeFormation.FileCount; ++fileIndex)
      {
        // ISSUE: explicit non-virtual call
        for (int rankIndex = 0; rankIndex < __nonvirtual (wedgeFormation.RankCount); ++rankIndex)
        {
          if (wedgeFormation.IsUnitPositionRestrained(fileIndex, rankIndex))
            yield return new Vec2i(fileIndex, rankIndex);
        }
      }
    }
  }
}
