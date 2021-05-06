// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TransposedLineFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace TaleWorlds.MountAndBlade
{
  public class TransposedLineFormation : LineFormation
  {
    public TransposedLineFormation(IFormation owner)
      : base(owner)
    {
      this.IsStaggered = false;
      this.IsTransforming = true;
    }

    public override IFormationArrangement Clone(IFormation formation) => (IFormationArrangement) new TransposedLineFormation(formation);

    public override void RearrangeFrom(IFormationArrangement arrangement)
    {
      if (arrangement is ColumnFormation)
      {
        int unitCount = arrangement.UnitCount;
        if (unitCount > 0)
        {
          int? fileCountStatic = FormOrder.GetFileCountStatic(((Formation) this.owner).FormOrder.OrderEnum, unitCount);
          if (fileCountStatic.HasValue)
            this.FormFromWidth((int) Math.Ceiling((double) unitCount * 1.0 / (double) fileCountStatic.Value));
        }
      }
      base.RearrangeFrom(arrangement);
    }
  }
}
