// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FormOrder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public struct FormOrder
  {
    private float _customWidth;
    internal readonly FormOrder.FormOrderEnum OrderEnum;
    public static readonly FormOrder FormOrderDeep = new FormOrder(FormOrder.FormOrderEnum.Deep);
    public static readonly FormOrder FormOrderWide = new FormOrder(FormOrder.FormOrderEnum.Wide);
    public static readonly FormOrder FormOrderWider = new FormOrder(FormOrder.FormOrderEnum.Wider);

    internal float CustomWidth
    {
      get => this._customWidth;
      private set => this._customWidth = value;
    }

    private FormOrder(FormOrder.FormOrderEnum orderEnum, float customWidth = -1f)
    {
      this.OrderEnum = orderEnum;
      this._customWidth = customWidth;
    }

    public static FormOrder FormOrderCustom(float customWidth) => new FormOrder(FormOrder.FormOrderEnum.Custom, customWidth);

    public OrderType OrderType
    {
      get
      {
        switch (this.OrderEnum)
        {
          case FormOrder.FormOrderEnum.Wide:
            return OrderType.FormWide;
          case FormOrder.FormOrderEnum.Wider:
            return OrderType.FormWider;
          case FormOrder.FormOrderEnum.Custom:
            return OrderType.FormCustom;
          default:
            return OrderType.FormDeep;
        }
      }
    }

    internal void OnApply(Formation formation) => this.OnApplyToArrangement(formation, formation.arrangement);

    internal static int GetUnitCountOf(Formation formation) => !formation.OverridenUnitCount.HasValue ? formation.CountOfUnitsWithoutDetachedOnes : formation.OverridenUnitCount.Value;

    internal bool OnApplyToCustomArrangement(Formation formation, IFormationArrangement arrangement) => false;

    private void OnApplyToArrangement(Formation formation, IFormationArrangement arrangement)
    {
      if (this.OnApplyToCustomArrangement(formation, arrangement))
        return;
      switch (arrangement)
      {
        case ColumnFormation _:
          ColumnFormation columnFormation = arrangement as ColumnFormation;
          if (FormOrder.GetUnitCountOf(formation) <= 0)
            break;
          columnFormation.FormFromWidth((float) this.GetRankVerticalFormFileCount());
          break;
        case RectilinearSchiltronFormation _:
          (arrangement as RectilinearSchiltronFormation).Form();
          break;
        case CircularSchiltronFormation _:
          (arrangement as CircularSchiltronFormation).Form();
          break;
        case CircularFormation _:
          CircularFormation circularFormation = arrangement as CircularFormation;
          int unitCountOf1 = FormOrder.GetUnitCountOf(formation);
          int? fileCount1 = this.GetFileCount(unitCountOf1);
          if (fileCount1.HasValue)
          {
            int depth = Math.Max(1, (int) Math.Ceiling((double) unitCountOf1 * 1.0 / (double) fileCount1.Value));
            circularFormation.FormFromDepth(depth);
            break;
          }
          circularFormation.FormFromCircumference(3.141593f * this.CustomWidth);
          break;
        case SquareFormation _:
          SquareFormation squareFormation = arrangement as SquareFormation;
          int unitCountOf2 = FormOrder.GetUnitCountOf(formation);
          int? fileCount2 = this.GetFileCount(unitCountOf2);
          if (fileCount2.HasValue)
          {
            int depth = Math.Max(1, (int) Math.Ceiling((double) unitCountOf2 * 1.0 / (double) fileCount2.Value));
            squareFormation.FormFromDepth(depth);
            break;
          }
          squareFormation.FormFromBorderWidth(this.CustomWidth);
          break;
        case SkeinFormation _:
          SkeinFormation skeinFormation = arrangement as SkeinFormation;
          int? fileCount3 = this.GetFileCount(FormOrder.GetUnitCountOf(formation));
          if (fileCount3.HasValue)
          {
            skeinFormation.FormFromWidth(fileCount3.Value);
            break;
          }
          skeinFormation.Width = this.CustomWidth;
          break;
        case WedgeFormation _:
          WedgeFormation wedgeFormation = arrangement as WedgeFormation;
          int? fileCount4 = this.GetFileCount(FormOrder.GetUnitCountOf(formation));
          if (fileCount4.HasValue)
          {
            wedgeFormation.FormFromWidth(fileCount4.Value);
            break;
          }
          wedgeFormation.Width = this.CustomWidth;
          break;
        case TransposedLineFormation _:
          TransposedLineFormation transposedLineFormation = arrangement as TransposedLineFormation;
          if (FormOrder.GetUnitCountOf(formation) <= 0)
            break;
          transposedLineFormation.FormFromWidth(this.GetRankVerticalFormFileCount());
          break;
        case LineFormation _:
          LineFormation lineFormation = arrangement as LineFormation;
          int unitCountOf3 = FormOrder.GetUnitCountOf(formation);
          int? fileCount5 = this.GetFileCount(unitCountOf3);
          if (fileCount5.HasValue)
          {
            lineFormation.FormFromWidth(fileCount5.Value, unitCountOf3 > 40);
            break;
          }
          lineFormation.Width = this.CustomWidth;
          break;
      }
    }

    private int? GetFileCount(int unitCount) => FormOrder.GetFileCountStatic(this.OrderEnum, unitCount);

    internal static int? GetFileCountStatic(FormOrder.FormOrderEnum order, int unitCount) => FormOrder.GetFileCountAux(order, unitCount);

    private int GetRankVerticalFormFileCount()
    {
      switch (this.OrderEnum)
      {
        case FormOrder.FormOrderEnum.Deep:
          return 1;
        case FormOrder.FormOrderEnum.Wide:
          return 3;
        case FormOrder.FormOrderEnum.Wider:
          return 5;
        case FormOrder.FormOrderEnum.Custom:
          return (int) Math.Ceiling((double) this._customWidth);
        default:
          return 1;
      }
    }

    private static int? GetFileCountAux(FormOrder.FormOrderEnum order, int unitCount)
    {
      switch (order)
      {
        case FormOrder.FormOrderEnum.Deep:
          return new int?(Math.Max(Math.Sqrt((double) unitCount / 4.0).Round(), 1) * 4);
        case FormOrder.FormOrderEnum.Wide:
          return new int?(Math.Max(Math.Sqrt((double) unitCount / 16.0).Round(), 1) * 16);
        case FormOrder.FormOrderEnum.Wider:
          return new int?(Math.Max(Math.Sqrt((double) unitCount / 64.0).Round(), 1) * 64);
        case FormOrder.FormOrderEnum.Custom:
          return new int?();
        default:
          return new int?();
      }
    }

    internal FormOrder.FormOrderEnum GetNativeEnum() => this.OrderEnum;

    public override bool Equals(object obj) => obj is FormOrder formOrder && formOrder == this;

    public override int GetHashCode() => (int) this.OrderEnum;

    public static bool operator !=(FormOrder f1, FormOrder f2) => f1.OrderEnum != f2.OrderEnum;

    public static bool operator ==(FormOrder f1, FormOrder f2) => f1.OrderEnum == f2.OrderEnum;

    internal enum FormOrderEnum
    {
      Deep,
      Wide,
      Wider,
      Custom,
    }
  }
}
