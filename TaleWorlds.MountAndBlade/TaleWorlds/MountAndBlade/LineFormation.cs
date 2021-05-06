// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LineFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class LineFormation : IFormationArrangement
  {
    private const int UnitPositionAvailabilityValueOfUnprocessed = 0;
    private const int UnitPositionAvailabilityValueOfUnavailable = 1;
    private const int UnitPositionAvailabilityValueOfAvailable = 2;
    private static readonly Vec2i InvalidPositionIndex = new Vec2i(-1, -1);
    protected readonly IFormation owner;
    private MBList2D<IFormationUnit> _units2D;
    private MBList2D<IFormationUnit> _units2DWorkspace;
    private List<IFormationUnit> _allUnits;
    private bool _isBatchRemovingUnits;
    private readonly List<int> _gapFillMinRanksPerFileForBatchRemove = new List<int>();
    private bool _batchRemoveInvolvesUnavailablePositions;
    private List<IFormationUnit> _unpositionedUnits;
    private MBList2D<int> _unitPositionAvailabilities;
    private MBList2D<int> _unitPositionAvailabilitiesWorkspace;
    private MBList2D<WorldPosition> _globalPositions;
    private MBList2D<WorldPosition> _globalPositionsWorkspace;
    private readonly MBWorkspace<MBQueue<(IFormationUnit, int, int)>> _displacedUnitsWorkspace;
    private readonly MBWorkspace<MBList<Vec2i>> _finalOccupationsWorkspace;
    private readonly MBWorkspace<MBList<Vec2i>> _filledInUnitPositionsWorkspace;
    private readonly MBWorkspace<MBQueue<Vec2i>> _toBeFilledInGapsWorkspace;
    private readonly MBWorkspace<MBList<Vec2i>> _finalVacanciesWorkspace;
    private readonly MBWorkspace<MBList<Vec2i>> _filledInGapsWorkspace;
    private MBList<Vec2i> _cachedOrderedUnitPositionIndices;
    private MBList<Vec2i> _cachedOrderedAndAvailableUnitPositionIndices;
    private MBList<Vec2> _cachedOrderedLocalPositions;
    private Func<LineFormation, int, int, bool> _shiftUnitsBackwardsPredicateDelegate;
    private Func<LineFormation, int, int, bool> _shiftUnitsForwardsPredicateDelegate;
    private bool _isCavalry;
    private bool _isStaggered = true;
    private readonly bool IsDeformingOnWidthChange;
    private bool IsMiddleFrontUnitPositionReserved;
    protected bool IsTransforming;

    protected int FileCount => this._units2D.Count1;

    public int RankCount => this._units2D.Count2;

    public bool AreLocalPositionsDirty { protected get; set; }

    protected float Interval => this.owner.Interval;

    protected float Distance => this.owner.Distance;

    protected float UnitDiameter => this.owner.UnitDiameter;

    internal int GetFileCountFromWidth(float width) => Math.Max(Math.Max(0, (int) (((double) width - (double) this.UnitDiameter) / ((double) this.Interval + (double) this.UnitDiameter) + 9.99999974737875E-06)) + 1, this.MinimumFileCount);

    public float Width
    {
      get => (float) (this.FileCount - 1) * (this.Interval + this.UnitDiameter) + this.UnitDiameter;
      set
      {
        int fileCountFromWidth = this.GetFileCountFromWidth(value);
        if (fileCountFromWidth > this.FileCount)
          LineFormation.WidenFormation(this, fileCountFromWidth - this.FileCount);
        else if (fileCountFromWidth < this.FileCount)
          LineFormation.NarrowFormation(this, this.FileCount - fileCountFromWidth);
        Action onWidthChanged = this.OnWidthChanged;
        if (onWidthChanged != null)
          onWidthChanged();
        Action onShapeChanged = this.OnShapeChanged;
        if (onShapeChanged == null)
          return;
        onShapeChanged();
      }
    }

    private int MinimumFileCount => this.IsTransforming ? 1 : Math.Max(1, (int) Math.Sqrt((double) this.GetUnitCountWithOverride()));

    protected int GetUnitCountWithOverride() => !this.owner.OverridenUnitCount.HasValue ? this.UnitCount : this.owner.OverridenUnitCount.Value;

    public float MinimumWidth => (float) (this.MinimumFileCount - 1) * (this.MinimumInterval + this.UnitDiameter) + this.UnitDiameter;

    private float MinimumInterval => this.owner.MinimumInterval;

    public float Depth => (float) (this.RankCount - 1) * (this.Distance + this.UnitDiameter) + this.UnitDiameter;

    public bool IsStaggered
    {
      get => this._isStaggered;
      set
      {
        if (this._isStaggered == value)
          return;
        this._isStaggered = value;
        Action onShapeChanged = this.OnShapeChanged;
        if (onShapeChanged == null)
          return;
        onShapeChanged();
      }
    }

    public virtual bool? IsLoose => new bool?();

    public event Action OnWidthChanged;

    public event Action OnShapeChanged;

    public LineFormation(IFormation ownerFormation, bool isStaggered = true)
    {
      this.owner = ownerFormation;
      this.IsStaggered = isStaggered;
      this._units2D = new MBList2D<IFormationUnit>(1, 1);
      this._unitPositionAvailabilities = new MBList2D<int>(1, 1);
      this._globalPositions = new MBList2D<WorldPosition>(1, 1);
      this._units2DWorkspace = new MBList2D<IFormationUnit>(1, 1);
      this._unitPositionAvailabilitiesWorkspace = new MBList2D<int>(1, 1);
      this._globalPositionsWorkspace = new MBList2D<WorldPosition>(1, 1);
      this._cachedOrderedUnitPositionIndices = new MBList<Vec2i>();
      this._cachedOrderedAndAvailableUnitPositionIndices = new MBList<Vec2i>();
      this._cachedOrderedLocalPositions = new MBList<Vec2>();
      this._unpositionedUnits = new List<IFormationUnit>();
      this._displacedUnitsWorkspace = new MBWorkspace<MBQueue<(IFormationUnit, int, int)>>();
      this._finalOccupationsWorkspace = new MBWorkspace<MBList<Vec2i>>();
      this._filledInUnitPositionsWorkspace = new MBWorkspace<MBList<Vec2i>>();
      this._toBeFilledInGapsWorkspace = new MBWorkspace<MBQueue<Vec2i>>();
      this._finalVacanciesWorkspace = new MBWorkspace<MBList<Vec2i>>();
      this._filledInGapsWorkspace = new MBWorkspace<MBList<Vec2i>>();
      this.ReconstructUnitsFromUnits2D();
      Action onShapeChanged = this.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    protected LineFormation(
      IFormation ownerFormation,
      bool isDeformingOnWidthChange,
      bool isStaggered = true)
      : this(ownerFormation, isStaggered)
    {
      this.IsDeformingOnWidthChange = isDeformingOnWidthChange;
    }

    public virtual IFormationArrangement Clone(IFormation formation) => (IFormationArrangement) new LineFormation(formation, this.IsDeformingOnWidthChange, this.IsStaggered);

    public virtual void DeepCopyFrom(IFormationArrangement arrangement)
    {
      LineFormation lineFormation = arrangement as LineFormation;
      this.IsStaggered = lineFormation.IsStaggered;
      this.IsTransforming = lineFormation.IsTransforming;
    }

    public void Reset()
    {
      this._units2D = new MBList2D<IFormationUnit>(1, 1);
      this._unitPositionAvailabilities = new MBList2D<int>(1, 1);
      this._globalPositions = new MBList2D<WorldPosition>(1, 1);
      this._units2DWorkspace = new MBList2D<IFormationUnit>(1, 1);
      this._unitPositionAvailabilitiesWorkspace = new MBList2D<int>(1, 1);
      this._globalPositionsWorkspace = new MBList2D<WorldPosition>(1, 1);
      this._cachedOrderedUnitPositionIndices = new MBList<Vec2i>();
      this._cachedOrderedAndAvailableUnitPositionIndices = new MBList<Vec2i>();
      this._cachedOrderedLocalPositions = new MBList<Vec2>();
      this._unpositionedUnits.Clear();
      this.ReconstructUnitsFromUnits2D();
      Action onShapeChanged = this.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    protected virtual bool IsUnitPositionRestrained(int fileIndex, int rankIndex)
    {
      if (!this.IsMiddleFrontUnitPositionReserved)
        return false;
      Vec2i frontUnitPosition = this.GetMiddleFrontUnitPosition();
      return fileIndex == frontUnitPosition.Item1 && rankIndex == frontUnitPosition.Item2;
    }

    protected virtual IEnumerable<Vec2i> GetRestrainedUnitPositions()
    {
      if (this.IsMiddleFrontUnitPositionReserved)
        yield return this.GetMiddleFrontUnitPosition();
    }

    private bool IsUnitPositionAvailable(int fileIndex, int rankIndex) => this._unitPositionAvailabilities[fileIndex, rankIndex] == 2;

    private Vec2i GetNearestAvailableNeighbourPositionIndex(int fileIndex, int rankIndex)
    {
      for (int index1 = 1; index1 < this.FileCount + this.RankCount; ++index1)
      {
        bool flag1 = true;
        bool flag2 = true;
        bool flag3 = true;
        bool flag4 = true;
        int num1 = 0;
        for (int index2 = 0; index2 <= index1 && flag1 | flag2 | flag3 | flag4; ++index2)
        {
          int num2 = index1 - index2;
          num1 = index2;
          int num3 = fileIndex - index2;
          int num4 = fileIndex + index2;
          int num5 = rankIndex - num2;
          int num6 = rankIndex + num2;
          if (flag1 && (num3 < 0 || num5 < 0))
            flag1 = false;
          if (flag3 && (num3 < 0 || num6 >= this.RankCount))
            flag3 = false;
          if (flag2 && (num4 >= this.FileCount || num5 < 0))
            flag2 = false;
          if (flag4 && (num4 >= this.FileCount || num6 >= this.RankCount))
            flag4 = false;
          if (flag1 && this._unitPositionAvailabilities[num3, num5] == 2)
            return new Vec2i(num3, num5);
          if (flag3 && this._unitPositionAvailabilities[num3, num6] == 2)
            return new Vec2i(num3, num6);
          if (flag2 && this._unitPositionAvailabilities[num4, num5] == 2)
            return new Vec2i(num4, num5);
          if (flag4 && this._unitPositionAvailabilities[num4, num6] == 2)
            return new Vec2i(num4, num6);
        }
        int num7;
        bool flag5 = (num7 = 1) != 0;
        bool flag6 = num7 != 0;
        bool flag7 = num7 != 0;
        bool flag8 = num7 != 0;
        for (int index2 = 0; index2 < index1 - num1 && flag8 | flag7 | flag6 | flag5; ++index2)
        {
          int num2 = index1 - index2;
          int num3 = fileIndex - num2;
          int num4 = fileIndex + num2;
          int num5 = rankIndex - index2;
          int num6 = rankIndex + index2;
          if (flag8 && (num3 < 0 || num5 < 0))
            flag8 = false;
          if (flag6 && (num3 < 0 || num6 >= this.RankCount))
            flag6 = false;
          if (flag7 && (num4 >= this.FileCount || num5 < 0))
            flag7 = false;
          if (flag5 && (num4 >= this.FileCount || num6 >= this.RankCount))
            flag5 = false;
          if (flag8 && this._unitPositionAvailabilities[num3, num5] == 2)
            return new Vec2i(num3, num5);
          if (flag6 && this._unitPositionAvailabilities[num3, num6] == 2)
            return new Vec2i(num3, num6);
          if (flag7 && this._unitPositionAvailabilities[num4, num5] == 2)
            return new Vec2i(num4, num5);
          if (flag5 && this._unitPositionAvailabilities[num4, num6] == 2)
            return new Vec2i(num4, num6);
        }
      }
      return LineFormation.InvalidPositionIndex;
    }

    private bool GetNextVacancy(out int fileIndex, out int rankIndex)
    {
      int num = this.FileCount * this.RankCount;
      for (int unitIndex = 0; unitIndex < num; ++unitIndex)
      {
        Vec2i unitPositionIndex = this.GetOrderedUnitPositionIndex(unitIndex);
        fileIndex = unitPositionIndex.Item1;
        rankIndex = unitPositionIndex.Item2;
        if (this._units2D[fileIndex, rankIndex] == null && this.IsUnitPositionAvailable(fileIndex, rankIndex))
          return true;
      }
      fileIndex = -1;
      rankIndex = -1;
      return false;
    }

    private static Vec2i GetOrderedUnitPositionIndexAux(
      int fileIndexBegin,
      int fileIndexEnd,
      int rankIndexBegin,
      int rankIndexEnd,
      int unitIndex)
    {
      int num1 = fileIndexEnd - fileIndexBegin + 1;
      int num2 = unitIndex / num1;
      int num3 = unitIndex - num2 * num1;
      return new Vec2i((num1 % 2 != 1 ? num1 / 2 - 1 + (num3 % 2 == 0 ? -1 : 1) * (num3 + 1) / 2 : num1 / 2 + (num3 % 2 == 0 ? 1 : -1) * (num3 + 1) / 2) + fileIndexBegin, num2 + rankIndexBegin);
    }

    private Vec2i GetOrderedUnitPositionIndex(int unitIndex) => LineFormation.GetOrderedUnitPositionIndexAux(0, this.FileCount - 1, 0, this.RankCount - 1, unitIndex);

    private static IEnumerable<Vec2i> GetOrderedUnitPositionIndicesAux(
      int fileIndexBegin,
      int fileIndexEnd,
      int rankIndexBegin,
      int rankIndexEnd)
    {
      int fileCount = fileIndexEnd - fileIndexBegin + 1;
      int centerFileIndex;
      int rankIndex;
      int fileIndexOffset;
      if (fileCount % 2 == 1)
      {
        centerFileIndex = fileCount / 2;
        for (rankIndex = rankIndexBegin; rankIndex <= rankIndexEnd; ++rankIndex)
        {
          yield return new Vec2i(fileIndexBegin + centerFileIndex, rankIndex);
          for (fileIndexOffset = 1; fileIndexOffset <= centerFileIndex; ++fileIndexOffset)
          {
            yield return new Vec2i(fileIndexBegin + centerFileIndex - fileIndexOffset, rankIndex);
            if (centerFileIndex + fileIndexOffset < fileCount)
              yield return new Vec2i(fileIndexBegin + centerFileIndex + fileIndexOffset, rankIndex);
          }
        }
      }
      else
      {
        centerFileIndex = fileCount / 2 - 1;
        for (rankIndex = rankIndexBegin; rankIndex <= rankIndexEnd; ++rankIndex)
        {
          yield return new Vec2i(fileIndexBegin + centerFileIndex, rankIndex);
          for (fileIndexOffset = 1; fileIndexOffset <= centerFileIndex + 1; ++fileIndexOffset)
          {
            yield return new Vec2i(fileIndexBegin + centerFileIndex + fileIndexOffset, rankIndex);
            if (centerFileIndex - fileIndexOffset >= 0)
              yield return new Vec2i(fileIndexBegin + centerFileIndex - fileIndexOffset, rankIndex);
          }
        }
      }
    }

    private IEnumerable<Vec2i> GetOrderedUnitPositionIndices() => LineFormation.GetOrderedUnitPositionIndicesAux(0, this.FileCount - 1, 0, this.RankCount - 1);

    public Vec2? GetLocalPositionOfUnitOrDefault(int unitIndex)
    {
      Vec2i vec2i = unitIndex < this._cachedOrderedAndAvailableUnitPositionIndices.Count ? this._cachedOrderedAndAvailableUnitPositionIndices.ElementAt<Vec2i>(unitIndex) : LineFormation.InvalidPositionIndex;
      return !(vec2i != LineFormation.InvalidPositionIndex) ? new Vec2?() : new Vec2?(this.GetLocalPositionOfUnit(vec2i.Item1, vec2i.Item2));
    }

    public Vec2? GetLocalDirectionOfUnitOrDefault(int unitIndex)
    {
      Vec2i vec2i = unitIndex < this._cachedOrderedAndAvailableUnitPositionIndices.Count ? this._cachedOrderedAndAvailableUnitPositionIndices.ElementAt<Vec2i>(unitIndex) : LineFormation.InvalidPositionIndex;
      return !(vec2i != LineFormation.InvalidPositionIndex) ? new Vec2?() : new Vec2?(this.GetLocalDirectionOfUnit(vec2i.Item1, vec2i.Item2));
    }

    public WorldPosition? GetWorldPositionOfUnitOrDefault(int unitIndex)
    {
      Vec2i vec2i = unitIndex < this._cachedOrderedAndAvailableUnitPositionIndices.Count ? this._cachedOrderedAndAvailableUnitPositionIndices.ElementAt<Vec2i>(unitIndex) : LineFormation.InvalidPositionIndex;
      return !(vec2i != LineFormation.InvalidPositionIndex) ? new WorldPosition?() : new WorldPosition?(this._globalPositions[vec2i.Item1, vec2i.Item2]);
    }

    public IEnumerable<Vec2> GetUnavailableUnitPositions()
    {
      for (int fileIndex = 0; fileIndex < this.FileCount; ++fileIndex)
      {
        for (int rankIndex = 0; rankIndex < this.RankCount; ++rankIndex)
        {
          if (this._unitPositionAvailabilities[fileIndex, rankIndex] == 1 && !this.IsUnitPositionRestrained(fileIndex, rankIndex))
            yield return this.GetLocalPositionOfUnit(fileIndex, rankIndex);
        }
      }
    }

    private void InsertUnit(IFormationUnit unit, int fileIndex, int rankIndex)
    {
      unit.FormationFileIndex = fileIndex;
      unit.FormationRankIndex = rankIndex;
      this._units2D[fileIndex, rankIndex] = unit;
      this.ReconstructUnitsFromUnits2D();
      Action onShapeChanged = this.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    public bool AddUnit(IFormationUnit unit)
    {
      bool flag = false;
      while (!flag && !this.AreLastRanksCompletelyUnavailable())
      {
        int fileIndex;
        int rankIndex;
        if (this.GetNextVacancy(out fileIndex, out rankIndex))
        {
          unit.FormationFileIndex = fileIndex;
          unit.FormationRankIndex = rankIndex;
          this._units2D[fileIndex, rankIndex] = unit;
          this.ReconstructUnitsFromUnits2D();
          flag = true;
        }
        else if (this.IsDeepenApplicable())
          this.Deepen();
        else
          break;
      }
      if (!flag)
      {
        this._unpositionedUnits.Add(unit);
        this.ReconstructUnitsFromUnits2D();
      }
      if (flag)
      {
        if (this.FileCount < this.MinimumFileCount)
          LineFormation.WidenFormation(this, this.MinimumFileCount - this.FileCount);
        Action onShapeChanged = this.OnShapeChanged;
        if (onShapeChanged != null)
          onShapeChanged();
        if (unit is Agent)
        {
          bool hasMount = (unit as Agent).HasMount;
          if ((!(this.owner is Formation) ? 0 : ((this.owner as Formation).CalculateHasSignificantNumberOfMounted ? 1 : 0)) != (this._isCavalry ? 1 : 0))
            this.BatchUnitPositionAvailabilities();
          else if (this._isCavalry != hasMount && this.owner is Formation)
          {
            (this.owner as Formation).QuerySystem.ForceExpireCavalryUnitRatio();
            if ((this.owner as Formation).CalculateHasSignificantNumberOfMounted != this._isCavalry)
              this.BatchUnitPositionAvailabilities();
          }
        }
      }
      return flag;
    }

    public void RemoveUnit(IFormationUnit unit)
    {
      if (this._unpositionedUnits.Remove(unit))
        this.ReconstructUnitsFromUnits2D();
      else
        this.RemoveUnit(unit, true);
    }

    public void OnBatchRemoveStart()
    {
      if (this._isBatchRemovingUnits)
        return;
      this._isBatchRemovingUnits = true;
      this._gapFillMinRanksPerFileForBatchRemove.Clear();
      this._batchRemoveInvolvesUnavailablePositions = false;
    }

    public void OnBatchRemoveEnd()
    {
      if (!this._isBatchRemovingUnits)
        return;
      if (this._gapFillMinRanksPerFileForBatchRemove.Count > 0)
      {
        for (int index = 0; index < this._gapFillMinRanksPerFileForBatchRemove.Count; ++index)
        {
          int rankIndex = this._gapFillMinRanksPerFileForBatchRemove[index];
          if (index < this.FileCount && rankIndex < this.RankCount)
            LineFormation.FillInTheGapsOfFile(this, index, rankIndex);
        }
        this.FillInTheGapsOfFormationAfterRemove(this._batchRemoveInvolvesUnavailablePositions);
        this._gapFillMinRanksPerFileForBatchRemove.Clear();
      }
      this._isBatchRemovingUnits = false;
    }

    public IEnumerable<IFormationUnit> GetUnitsToPop(int count)
    {
      foreach (IFormationUnit unpositionedUnit in this._unpositionedUnits)
      {
        yield return unpositionedUnit;
        --count;
        if (count == 0)
          yield break;
      }
      for (int i = this.FileCount * this.RankCount - 1; i >= 0; --i)
      {
        Vec2i unitPositionIndex = this.GetOrderedUnitPositionIndex(i);
        int index1 = unitPositionIndex.Item1;
        int index2 = unitPositionIndex.Item2;
        if (this._units2D[index1, index2] != null)
        {
          yield return this._units2D[index1, index2];
          --count;
          if (count == 0)
            break;
        }
      }
    }

    private void TryToKeepDepth()
    {
      if (this.FileCount <= this.MinimumFileCount || this.GetUnitsAtRank(this.RankCount - 1).Count<IFormationUnit>() + (this.RankCount - 1) > this.FileCount || MBRandom.RandomInt(this.RankCount * 2) != 0 || !this.IsNarrowApplicable(this.FileCount > 2 ? 2 : 1))
        return;
      LineFormation.NarrowFormation(this, this.FileCount > 2 ? 2 : 1);
    }

    private void RemoveUnit(
      IFormationUnit unit,
      bool fillInTheGap,
      bool isRemovingFromAnUnavailablePosition = false)
    {
      if (fillInTheGap)
      {
        int num1 = isRemovingFromAnUnavailablePosition ? 1 : 0;
      }
      int formationFileIndex = unit.FormationFileIndex;
      int formationRankIndex = unit.FormationRankIndex;
      if (unit.FormationFileIndex < 0 || unit.FormationRankIndex < 0 || (unit.FormationFileIndex >= this.FileCount || unit.FormationRankIndex >= this.RankCount))
        MBDebug.Print("Unit removed has file-rank indices: " + (object) unit.FormationFileIndex + " " + (object) unit.FormationRankIndex + " while line formation has file-rank counts of " + (object) this.FileCount + " " + (object) this.RankCount + " agent state is " + (object) (unit is Agent agent3 ? new AgentState?(agent3.State) : new AgentState?()) + " unit detachment is " + (unit is Agent agent4 ? (object) agent4.Detachment : (object) (IDetachment) null));
      this._units2D[unit.FormationFileIndex, unit.FormationRankIndex] = (IFormationUnit) null;
      this.ReconstructUnitsFromUnits2D();
      unit.FormationFileIndex = -1;
      unit.FormationRankIndex = -1;
      if (fillInTheGap)
      {
        if (this._isBatchRemovingUnits)
        {
          int num2 = formationFileIndex - this._gapFillMinRanksPerFileForBatchRemove.Count + 1;
          for (int index = 0; index < num2; ++index)
            this._gapFillMinRanksPerFileForBatchRemove.Add(int.MaxValue);
          this._gapFillMinRanksPerFileForBatchRemove[formationFileIndex] = Math.Min(formationRankIndex, this._gapFillMinRanksPerFileForBatchRemove[formationFileIndex]);
          this._batchRemoveInvolvesUnavailablePositions |= isRemovingFromAnUnavailablePosition;
        }
        else
        {
          LineFormation.FillInTheGapsOfFile(this, formationFileIndex, formationRankIndex);
          this.FillInTheGapsOfFormationAfterRemove(isRemovingFromAnUnavailablePosition);
        }
      }
      Action onShapeChanged = this.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    protected virtual bool TryGetUnitPositionIndexFromLocalPosition(
      Vec2 localPosition,
      out int fileIndex,
      out int rankIndex)
    {
      rankIndex = (int) Math.Round(-(double) localPosition.y / ((double) this.Distance + (double) this.UnitDiameter));
      if (rankIndex >= this.RankCount)
      {
        fileIndex = -1;
        return false;
      }
      if (this.IsStaggered && rankIndex % 2 == 1)
        localPosition.x -= (float) (((double) this.Interval + (double) this.UnitDiameter) * 0.5);
      float num = (float) (this.FileCount - 1) * (this.Interval + this.UnitDiameter);
      fileIndex = (int) Math.Round(((double) localPosition.x + (double) num / 2.0) / ((double) this.Interval + (double) this.UnitDiameter));
      return fileIndex >= 0 && fileIndex < this.FileCount;
    }

    protected virtual Vec2 GetLocalPositionOfUnit(int fileIndex, int rankIndex)
    {
      float num = (float) (this.FileCount - 1) * (this.Interval + this.UnitDiameter);
      Vec2 vec2 = new Vec2((float) ((double) fileIndex * ((double) this.Interval + (double) this.UnitDiameter) - (double) num / 2.0), (float) -rankIndex * (this.Distance + this.UnitDiameter));
      if (this.IsStaggered && rankIndex % 2 == 1)
        vec2.x += (float) (((double) this.Interval + (double) this.UnitDiameter) * 0.5);
      return vec2;
    }

    protected virtual Vec2 GetLocalDirectionOfUnit(int fileIndex, int rankIndex) => Vec2.Forward;

    public Vec2? GetLocalPositionOfUnitOrDefault(IFormationUnit unit) => this._unpositionedUnits.Contains(unit) ? new Vec2?() : new Vec2?(this.GetLocalPositionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));

    public Vec2? GetLocalDirectionOfUnitOrDefault(IFormationUnit unit) => this._unpositionedUnits.Contains(unit) ? new Vec2?() : new Vec2?(this.GetLocalDirectionOfUnit(unit.FormationFileIndex, unit.FormationRankIndex));

    public WorldPosition? GetWorldPositionOfUnitOrDefault(IFormationUnit unit) => this._unpositionedUnits.Contains(unit) ? new WorldPosition?() : new WorldPosition?(this._globalPositions[unit.FormationFileIndex, unit.FormationRankIndex]);

    private void ReconstructUnitsFromUnits2D()
    {
      if (this._allUnits == null)
        this._allUnits = new List<IFormationUnit>();
      this._allUnits.Clear();
      for (int index1 = 0; index1 < this._units2D.Count1; ++index1)
      {
        for (int index2 = 0; index2 < this._units2D.Count2; ++index2)
        {
          if (this._units2D[index1, index2] != null)
            this._allUnits.Add(this._units2D[index1, index2]);
        }
      }
      for (int index = 0; index < this._unpositionedUnits.Count; ++index)
        this._allUnits.Add(this._unpositionedUnits[index]);
    }

    private void FillInTheGapsOfFormationAfterRemove(bool hasUnavailablePositions)
    {
      this.TryReaddingUnpositionedUnits();
      LineFormation.FillInTheGapsOfMiddleRanks(this);
      int num = hasUnavailablePositions ? 1 : 0;
      this.TryToKeepDepth();
    }

    private static void WidenFormation(LineFormation formation, int fileCountFromBothFlanks)
    {
      if (fileCountFromBothFlanks % 2 == 0)
        LineFormation.WidenFormation(formation, fileCountFromBothFlanks / 2, fileCountFromBothFlanks / 2);
      else if (formation.FileCount % 2 == 0)
        LineFormation.WidenFormation(formation, fileCountFromBothFlanks / 2 + 1, fileCountFromBothFlanks / 2);
      else
        LineFormation.WidenFormation(formation, fileCountFromBothFlanks / 2, fileCountFromBothFlanks / 2 + 1);
    }

    private static void WidenFormation(
      LineFormation formation,
      int fileCountFromLeftFlank,
      int fileCountFromRightFlank)
    {
      formation._units2DWorkspace.ResetWithNewCount(formation.FileCount + fileCountFromLeftFlank + fileCountFromRightFlank, formation.RankCount);
      formation._unitPositionAvailabilitiesWorkspace.ResetWithNewCount(formation.FileCount + fileCountFromLeftFlank + fileCountFromRightFlank, formation.RankCount);
      formation._globalPositionsWorkspace.ResetWithNewCount(formation.FileCount + fileCountFromLeftFlank + fileCountFromRightFlank, formation.RankCount);
      for (int index = 0; index < formation.FileCount; ++index)
      {
        int destinationIndex1 = index + fileCountFromLeftFlank;
        formation._units2D.CopyRowTo(index, 0, formation._units2DWorkspace, destinationIndex1, 0, formation.RankCount);
        formation._unitPositionAvailabilities.CopyRowTo(index, 0, formation._unitPositionAvailabilitiesWorkspace, destinationIndex1, 0, formation.RankCount);
        formation._globalPositions.CopyRowTo(index, 0, formation._globalPositionsWorkspace, destinationIndex1, 0, formation.RankCount);
        if (fileCountFromLeftFlank > 0)
        {
          for (int index2 = 0; index2 < formation.RankCount; ++index2)
          {
            if (formation._units2D[index, index2] != null)
              formation._units2D[index, index2].FormationFileIndex += fileCountFromLeftFlank;
          }
        }
      }
      MBList2D<IFormationUnit> units2D = formation._units2D;
      formation._units2D = formation._units2DWorkspace;
      formation._units2DWorkspace = units2D;
      formation.ReconstructUnitsFromUnits2D();
      MBList2D<int> positionAvailabilities = formation._unitPositionAvailabilities;
      formation._unitPositionAvailabilities = formation._unitPositionAvailabilitiesWorkspace;
      formation._unitPositionAvailabilitiesWorkspace = positionAvailabilities;
      MBList2D<WorldPosition> globalPositions = formation._globalPositions;
      formation._globalPositions = formation._globalPositionsWorkspace;
      formation._globalPositionsWorkspace = globalPositions;
      formation.BatchUnitPositionAvailabilities();
      if (formation.IsDeformingOnWidthChange || (fileCountFromLeftFlank + fileCountFromRightFlank) % 2 == 1)
      {
        formation.OnFormationFrameChanged();
      }
      else
      {
        LineFormation.ShiftUnitsForwardsForWideningFormation(formation);
        formation.TryReaddingUnpositionedUnits();
        while (formation.RankCount > 1 && formation.GetUnitsAtRank(formation.RankCount - 1).IsEmpty<IFormationUnit>())
          formation.Shorten();
      }
      Action onShapeChanged = formation.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    private static void GetToBeFilledInAndToBeEmptiedOutUnitPositions(
      LineFormation formation,
      MBQueue<Vec2i> toBeFilledInUnitPositions,
      MBList<Vec2i> toBeEmptiedOutUnitPositions)
    {
      int unitIndex1 = 0;
      int unitIndex2 = formation.FileCount * formation.RankCount - 1;
      while (true)
      {
        Vec2i unitPositionIndex1 = formation.GetOrderedUnitPositionIndex(unitIndex1);
        int num1 = unitPositionIndex1.Item1;
        int num2 = unitPositionIndex1.Item2;
        Vec2i unitPositionIndex2 = formation.GetOrderedUnitPositionIndex(unitIndex2);
        int num3 = unitPositionIndex2.Item1;
        int num4 = unitPositionIndex2.Item2;
        if (num2 < num4)
        {
          if (formation._units2D[num1, num2] != null || !formation.IsUnitPositionAvailable(num1, num2))
            ++unitIndex1;
          else if (formation._units2D[num3, num4] == null)
          {
            --unitIndex2;
          }
          else
          {
            toBeFilledInUnitPositions.Enqueue(new Vec2i(num1, num2));
            toBeEmptiedOutUnitPositions.Add(new Vec2i(num3, num4));
            ++unitIndex1;
            --unitIndex2;
          }
        }
        else
          break;
      }
    }

    private static Vec2i GetUnitPositionForFillInFromNearby(
      LineFormation formation,
      int relocationFileIndex,
      int relocationRankIndex,
      Func<LineFormation, int, int, bool> predicate,
      bool isRelocationUnavailable = false)
    {
      return LineFormation.GetUnitPositionForFillInFromNearby(formation, relocationFileIndex, relocationRankIndex, predicate, LineFormation.InvalidPositionIndex, isRelocationUnavailable);
    }

    private static Vec2i GetUnitPositionForFillInFromNearby(
      LineFormation formation,
      int relocationFileIndex,
      int relocationRankIndex,
      Func<LineFormation, int, int, bool> predicate,
      Vec2i lastFinalOccupation,
      bool isRelocationUnavailable = false)
    {
      int fileCount = formation.FileCount;
      int rankCount = formation.RankCount;
      bool flag = relocationFileIndex >= fileCount / 2;
      if (lastFinalOccupation != LineFormation.InvalidPositionIndex)
        flag = lastFinalOccupation.Item1 <= relocationFileIndex;
      for (int val1 = 1; val1 <= fileCount + rankCount; ++val1)
      {
        for (int index = Math.Min(val1, rankCount - 1 - relocationRankIndex); index >= 0; --index)
        {
          int num = val1 - index;
          if (flag && relocationFileIndex - num >= 0 && predicate(formation, relocationFileIndex - num, relocationRankIndex + index))
            return new Vec2i(relocationFileIndex - num, relocationRankIndex + index);
          if (relocationFileIndex + num < fileCount && predicate(formation, relocationFileIndex + num, relocationRankIndex + index))
            return new Vec2i(relocationFileIndex + num, relocationRankIndex + index);
          if (!flag && relocationFileIndex - num >= 0 && predicate(formation, relocationFileIndex - num, relocationRankIndex + index))
            return new Vec2i(relocationFileIndex - num, relocationRankIndex + index);
        }
      }
      return LineFormation.InvalidPositionIndex;
    }

    private static void ShiftUnitsForwardsForWideningFormation(LineFormation formation)
    {
      MBQueue<Vec2i> toBeFilledInUnitPositions = formation._toBeFilledInGapsWorkspace.StartUsingWorkspace();
      MBList<Vec2i> toBeEmptiedOutUnitPositions = formation._finalVacanciesWorkspace.StartUsingWorkspace();
      MBList<Vec2i> mbList = formation._filledInGapsWorkspace.StartUsingWorkspace();
      LineFormation.GetToBeFilledInAndToBeEmptiedOutUnitPositions(formation, toBeFilledInUnitPositions, toBeEmptiedOutUnitPositions);
      if (formation._shiftUnitsForwardsPredicateDelegate == null)
        formation._shiftUnitsForwardsPredicateDelegate = new Func<LineFormation, int, int, bool>(ShiftUnitForwardsPredicate);
      while (toBeFilledInUnitPositions.Count > 0)
      {
        Vec2i vec2i1 = toBeFilledInUnitPositions.Dequeue();
        Vec2i fillInFromNearby = LineFormation.GetUnitPositionForFillInFromNearby(formation, vec2i1.Item1, vec2i1.Item2, formation._shiftUnitsForwardsPredicateDelegate);
        if (fillInFromNearby != LineFormation.InvalidPositionIndex)
        {
          int num1 = fillInFromNearby.Item1;
          int num2 = fillInFromNearby.Item2;
          IFormationUnit unit = formation._units2D[num1, num2];
          formation.RelocateUnit(unit, vec2i1.Item1, vec2i1.Item2);
          mbList.Add(vec2i1);
          Vec2i vec2i2 = new Vec2i(num1, num2);
          if (!toBeEmptiedOutUnitPositions.Contains(vec2i2))
            toBeFilledInUnitPositions.Enqueue(vec2i2);
        }
      }
      formation._toBeFilledInGapsWorkspace.StopUsingWorkspace();
      formation._finalVacanciesWorkspace.StopUsingWorkspace();
      formation._filledInGapsWorkspace.StopUsingWorkspace();

      bool ShiftUnitForwardsPredicate(LineFormation localFormation, int fileIndex, int rankIndex) => localFormation._units2D[fileIndex, rankIndex] != null && !localFormation._filledInGapsWorkspace.GetWorkspace().Contains(new Vec2i(fileIndex, rankIndex));
    }

    private static void DeepenFormation(
      LineFormation formation,
      int rankCountFromFront,
      int rankCountFromRear)
    {
      formation._units2DWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount + rankCountFromFront + rankCountFromRear);
      formation._unitPositionAvailabilitiesWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount + rankCountFromFront + rankCountFromRear);
      formation._globalPositionsWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount + rankCountFromFront + rankCountFromRear);
      for (int index = 0; index < formation.FileCount; ++index)
      {
        formation._units2D.CopyRowTo(index, 0, formation._units2DWorkspace, index, rankCountFromFront, formation.RankCount);
        formation._unitPositionAvailabilities.CopyRowTo(index, 0, formation._unitPositionAvailabilitiesWorkspace, index, rankCountFromFront, formation.RankCount);
        formation._globalPositions.CopyRowTo(index, 0, formation._globalPositionsWorkspace, index, rankCountFromFront, formation.RankCount);
        if (rankCountFromFront > 0)
        {
          for (int index2 = 0; index2 < formation.RankCount; ++index2)
          {
            if (formation._units2D[index, index2] != null)
              formation._units2D[index, index2].FormationRankIndex += rankCountFromFront;
          }
        }
      }
      MBList2D<IFormationUnit> units2D = formation._units2D;
      formation._units2D = formation._units2DWorkspace;
      formation._units2DWorkspace = units2D;
      formation.ReconstructUnitsFromUnits2D();
      MBList2D<int> positionAvailabilities = formation._unitPositionAvailabilities;
      formation._unitPositionAvailabilities = formation._unitPositionAvailabilitiesWorkspace;
      formation._unitPositionAvailabilitiesWorkspace = positionAvailabilities;
      MBList2D<WorldPosition> globalPositions = formation._globalPositions;
      formation._globalPositions = formation._globalPositionsWorkspace;
      formation._globalPositionsWorkspace = globalPositions;
      formation.BatchUnitPositionAvailabilities();
      Action onShapeChanged = formation.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    protected virtual bool IsDeepenApplicable() => true;

    private void Deepen() => LineFormation.DeepenFormation(this, 0, 1);

    private static bool DeepenForVacancy(
      LineFormation formation,
      int requestedVacancyCount,
      int fileOffsetFromLeftFlank,
      int fileOffsetFromRightFlank)
    {
      int num1 = 0;
      bool? nullable = new bool?();
      while (!nullable.HasValue)
      {
        int num2 = formation.RankCount - 1;
        for (int index = fileOffsetFromLeftFlank; index < formation.FileCount - fileOffsetFromRightFlank; ++index)
        {
          if (formation._units2D[index, num2] == null && formation.IsUnitPositionAvailable(index, num2))
            ++num1;
        }
        if (num1 >= requestedVacancyCount)
          nullable = new bool?(true);
        else if (!formation.AreLastRanksCompletelyUnavailable())
        {
          if (formation.IsDeepenApplicable())
            formation.Deepen();
          else
            nullable = new bool?(false);
        }
        else
          nullable = new bool?(false);
      }
      return nullable.Value;
    }

    protected virtual bool IsNarrowApplicable(int amount) => true;

    private static void NarrowFormation(LineFormation formation, int fileCountFromBothFlanks)
    {
      int fileCountFromLeftFlank = fileCountFromBothFlanks / 2;
      int fileCountFromRightFlank = fileCountFromBothFlanks / 2;
      if (fileCountFromBothFlanks % 2 != 0)
      {
        if (formation.FileCount % 2 == 0)
          ++fileCountFromRightFlank;
        else
          ++fileCountFromLeftFlank;
      }
      if (!formation.IsNarrowApplicable(fileCountFromLeftFlank + fileCountFromRightFlank))
        return;
      LineFormation.NarrowFormation(formation, fileCountFromLeftFlank, fileCountFromRightFlank);
    }

    private static bool ShiftUnitsBackwardsForNewUnavailableUnitPositions(LineFormation formation)
    {
      List<Vec2i> list1 = formation.GetOrderedUnitPositionIndices().Where<Vec2i>((Func<Vec2i, bool>) (p => formation._units2D[p.Item1, p.Item2] != null && !formation.IsUnitPositionAvailable(p.Item1, p.Item2))).ToList<Vec2i>();
      bool flag = list1.Count > 0;
      if (flag)
      {
        MBQueue<(IFormationUnit, int, int)> displacedUnits = formation._displacedUnitsWorkspace.StartUsingWorkspace();
        for (int index = list1.Count - 1; index >= 0; --index)
        {
          Vec2i vec2i = list1[index];
          IFormationUnit unit = formation._units2D[vec2i.Item1, vec2i.Item2];
          if (unit != null)
          {
            formation.RemoveUnit(unit, false, true);
            displacedUnits.Enqueue(ValueTuple.Create<IFormationUnit, int, int>(unit, vec2i.Item1, vec2i.Item2));
          }
        }
        LineFormation.DeepenForVacancy(formation, displacedUnits.Count, 0, 0);
        IEnumerable<Vec2i> list2 = formation.GetOrderedUnitPositionIndices().Where<Vec2i>((Func<Vec2i, bool>) (p => formation._units2D[p.Item1, p.Item2] == null && formation.IsUnitPositionAvailable(p.Item1, p.Item2))).Take<Vec2i>(displacedUnits.Count);
        MBList<Vec2i> finalOccupations = formation._finalOccupationsWorkspace.StartUsingWorkspace();
        finalOccupations.AddRange(list2);
        LineFormation.ShiftUnitsBackwardsAux(formation, displacedUnits, finalOccupations);
        formation._displacedUnitsWorkspace.StopUsingWorkspace();
        formation._finalOccupationsWorkspace.StopUsingWorkspace();
      }
      return flag;
    }

    private static void ShiftUnitsBackwardsForNarrowingFormation(
      LineFormation formation,
      int fileCountFromLeftFlank,
      int fileCountFromRightFlank)
    {
      MBQueue<(IFormationUnit, int, int)> displacedUnits = formation._displacedUnitsWorkspace.StartUsingWorkspace();
      foreach (Vec2i vec2i in formation.GetOrderedUnitPositionIndices().Where<Vec2i>((Func<Vec2i, bool>) (p => p.Item1 < fileCountFromLeftFlank || p.Item1 >= formation.FileCount - fileCountFromRightFlank)).Reverse<Vec2i>())
      {
        IFormationUnit unit = formation._units2D[vec2i.Item1, vec2i.Item2];
        if (unit != null)
        {
          formation.RemoveUnit(unit, false);
          displacedUnits.Enqueue(ValueTuple.Create<IFormationUnit, int, int>(unit, vec2i.Item1, vec2i.Item2));
        }
      }
      LineFormation.DeepenForVacancy(formation, displacedUnits.Count, fileCountFromLeftFlank, fileCountFromRightFlank);
      IEnumerable<Vec2i> list = LineFormation.GetOrderedUnitPositionIndicesAux(fileCountFromLeftFlank, formation.FileCount - 1 - fileCountFromRightFlank, 0, formation.RankCount - 1).Where<Vec2i>((Func<Vec2i, bool>) (p => formation._units2D[p.Item1, p.Item2] == null && formation.IsUnitPositionAvailable(p.Item1, p.Item2))).Take<Vec2i>(displacedUnits.Count);
      MBList<Vec2i> finalOccupations = formation._finalOccupationsWorkspace.StartUsingWorkspace();
      finalOccupations.AddRange(list);
      LineFormation.ShiftUnitsBackwardsAux(formation, displacedUnits, finalOccupations);
      formation._displacedUnitsWorkspace.StopUsingWorkspace();
      formation._finalOccupationsWorkspace.StopUsingWorkspace();
    }

    private static void ShiftUnitsBackwardsAux(
      LineFormation formation,
      MBQueue<(IFormationUnit, int, int)> displacedUnits,
      MBList<Vec2i> finalOccupations)
    {
      MBList<Vec2i> mbList = formation._filledInUnitPositionsWorkspace.StartUsingWorkspace();
      if (formation._shiftUnitsBackwardsPredicateDelegate == null)
        formation._shiftUnitsBackwardsPredicateDelegate = new Func<LineFormation, int, int, bool>(ShiftUnitsBackwardsPredicate);
      while (!displacedUnits.IsEmpty<(IFormationUnit, int, int)>())
      {
        (IFormationUnit unit4, int relocationFileIndex2, int relocationRankIndex2) = displacedUnits.Dequeue();
        Vec2i fillInFromNearby = LineFormation.GetUnitPositionForFillInFromNearby(formation, relocationFileIndex2, relocationRankIndex2, formation._shiftUnitsBackwardsPredicateDelegate, finalOccupations.Count == 1 ? finalOccupations[0] : LineFormation.InvalidPositionIndex, true);
        if (fillInFromNearby != LineFormation.InvalidPositionIndex)
        {
          IFormationUnit unit2 = formation._units2D[fillInFromNearby.Item1, fillInFromNearby.Item2];
          if (unit2 != null)
          {
            formation.RemoveUnit(unit2, false);
            displacedUnits.Enqueue(ValueTuple.Create<IFormationUnit, int, int>(unit2, fillInFromNearby.Item1, fillInFromNearby.Item2));
          }
          mbList.Add(fillInFromNearby);
          formation.InsertUnit(unit4, fillInFromNearby.Item1, fillInFromNearby.Item2);
        }
        else
        {
          float num1 = float.MaxValue;
          Vec2i vec2i = LineFormation.InvalidPositionIndex;
          for (int index = 0; index < finalOccupations.Count; ++index)
          {
            if (mbList.IndexOf(finalOccupations[index]) < 0)
            {
              float num2 = (float) (Math.Abs(finalOccupations[index].Item1 - relocationFileIndex2) + Math.Abs(finalOccupations[index].Item2 - relocationRankIndex2));
              if ((double) num2 < (double) num1)
              {
                num1 = num2;
                vec2i = finalOccupations[index];
              }
            }
          }
          if (vec2i != LineFormation.InvalidPositionIndex)
          {
            mbList.Add(vec2i);
            formation.InsertUnit(unit4, vec2i.Item1, vec2i.Item2);
          }
          else
          {
            formation._unpositionedUnits.Add(unit4);
            formation.ReconstructUnitsFromUnits2D();
          }
        }
      }
      formation._filledInUnitPositionsWorkspace.StopUsingWorkspace();

      bool ShiftUnitsBackwardsPredicate(LineFormation localFormation, int fileIndex, int rankIndex)
      {
        Vec2i vec2i = new Vec2i(fileIndex, rankIndex);
        return (localFormation._units2D[fileIndex, rankIndex] != null || localFormation._finalOccupationsWorkspace.GetWorkspace().IndexOf(vec2i) >= 0) && !localFormation._filledInUnitPositionsWorkspace.GetWorkspace().Contains(vec2i);
      }
    }

    private static void NarrowFormation(
      LineFormation formation,
      int fileCountFromLeftFlank,
      int fileCountFromRightFlank)
    {
      LineFormation.ShiftUnitsBackwardsForNarrowingFormation(formation, fileCountFromLeftFlank, fileCountFromRightFlank);
      LineFormation.NarrowFormationAux(formation, fileCountFromLeftFlank, fileCountFromRightFlank);
    }

    private static void NarrowFormationAux(
      LineFormation formation,
      int fileCountFromLeftFlank,
      int fileCountFromRightFlank)
    {
      formation._units2DWorkspace.ResetWithNewCount(formation.FileCount - fileCountFromLeftFlank - fileCountFromRightFlank, formation.RankCount);
      formation._unitPositionAvailabilitiesWorkspace.ResetWithNewCount(formation.FileCount - fileCountFromLeftFlank - fileCountFromRightFlank, formation.RankCount);
      formation._globalPositionsWorkspace.ResetWithNewCount(formation.FileCount - fileCountFromLeftFlank - fileCountFromRightFlank, formation.RankCount);
      for (int index = fileCountFromLeftFlank; index < formation.FileCount - fileCountFromRightFlank; ++index)
      {
        int destinationIndex1 = index - fileCountFromLeftFlank;
        formation._units2D.CopyRowTo(index, 0, formation._units2DWorkspace, destinationIndex1, 0, formation.RankCount);
        formation._unitPositionAvailabilities.CopyRowTo(index, 0, formation._unitPositionAvailabilitiesWorkspace, destinationIndex1, 0, formation.RankCount);
        formation._globalPositions.CopyRowTo(index, 0, formation._globalPositionsWorkspace, destinationIndex1, 0, formation.RankCount);
        if (fileCountFromLeftFlank > 0)
        {
          for (int index2 = 0; index2 < formation.RankCount; ++index2)
          {
            if (formation._units2D[index, index2] != null)
              formation._units2D[index, index2].FormationFileIndex -= fileCountFromLeftFlank;
          }
        }
      }
      MBList2D<IFormationUnit> units2D = formation._units2D;
      formation._units2D = formation._units2DWorkspace;
      formation._units2DWorkspace = units2D;
      formation.ReconstructUnitsFromUnits2D();
      MBList2D<int> positionAvailabilities = formation._unitPositionAvailabilities;
      formation._unitPositionAvailabilities = formation._unitPositionAvailabilitiesWorkspace;
      formation._unitPositionAvailabilitiesWorkspace = positionAvailabilities;
      MBList2D<WorldPosition> globalPositions = formation._globalPositions;
      formation._globalPositions = formation._globalPositionsWorkspace;
      formation._globalPositionsWorkspace = globalPositions;
      formation.BatchUnitPositionAvailabilities();
      Action onShapeChanged = formation.OnShapeChanged;
      if (onShapeChanged != null)
        onShapeChanged();
      if (!formation.IsDeformingOnWidthChange && (fileCountFromLeftFlank + fileCountFromRightFlank) % 2 != 1)
        return;
      formation.OnFormationFrameChanged();
    }

    private static void ShortenFormation(LineFormation formation, int front, int rear)
    {
      formation._units2DWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount - front - rear);
      formation._unitPositionAvailabilitiesWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount - front - rear);
      formation._globalPositionsWorkspace.ResetWithNewCount(formation.FileCount, formation.RankCount - front - rear);
      for (int index = 0; index < formation.FileCount; ++index)
      {
        formation._units2D.CopyRowTo(index, front, formation._units2DWorkspace, index, 0, formation.RankCount - rear - front);
        formation._unitPositionAvailabilities.CopyRowTo(index, front, formation._unitPositionAvailabilitiesWorkspace, index, 0, formation.RankCount - rear - front);
        formation._globalPositions.CopyRowTo(index, front, formation._globalPositionsWorkspace, index, 0, formation.RankCount - rear - front);
        if (front > 0)
        {
          for (int index2 = front; index2 < formation.RankCount - rear; ++index2)
          {
            if (formation._units2D[index, index2] != null)
              formation._units2D[index, index2].FormationRankIndex -= front;
          }
        }
      }
      MBList2D<IFormationUnit> units2D = formation._units2D;
      formation._units2D = formation._units2DWorkspace;
      formation._units2DWorkspace = units2D;
      formation.ReconstructUnitsFromUnits2D();
      MBList2D<int> positionAvailabilities = formation._unitPositionAvailabilities;
      formation._unitPositionAvailabilities = formation._unitPositionAvailabilitiesWorkspace;
      formation._unitPositionAvailabilitiesWorkspace = positionAvailabilities;
      MBList2D<WorldPosition> globalPositions = formation._globalPositions;
      formation._globalPositions = formation._globalPositionsWorkspace;
      formation._globalPositionsWorkspace = globalPositions;
      formation.BatchUnitPositionAvailabilities();
      Action onShapeChanged = formation.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    private void Shorten() => LineFormation.ShortenFormation(this, 0, 1);

    private void GetFrontAndRearOfFile(
      int fileIndex,
      out bool isFileEmtpy,
      out int rankIndexOfFront,
      out int rankIndexOfRear,
      bool includeUnavailablePositions = false)
    {
      rankIndexOfFront = -1;
      rankIndexOfRear = this.RankCount;
      for (int index2 = 0; index2 < this.RankCount; ++index2)
      {
        if (this._units2D[fileIndex, index2] != null)
        {
          rankIndexOfFront = index2;
          break;
        }
      }
      if (includeUnavailablePositions)
      {
        if (rankIndexOfFront != -1)
        {
          for (int rankIndex = rankIndexOfFront - 1; rankIndex >= 0 && !this.IsUnitPositionAvailable(fileIndex, rankIndex); --rankIndex)
            rankIndexOfFront = rankIndex;
        }
        else
        {
          bool flag = true;
          for (int rankIndex = 0; rankIndex < this.RankCount; ++rankIndex)
          {
            if (this.IsUnitPositionAvailable(fileIndex, rankIndex))
            {
              flag = false;
              break;
            }
          }
          if (flag)
            rankIndexOfFront = 0;
        }
      }
      for (int index2 = this.RankCount - 1; index2 >= 0; --index2)
      {
        if (this._units2D[fileIndex, index2] != null)
        {
          rankIndexOfRear = index2;
          break;
        }
      }
      if (includeUnavailablePositions)
      {
        if (rankIndexOfRear != this.RankCount)
        {
          for (int rankIndex = rankIndexOfRear + 1; rankIndex < this.RankCount && !this.IsUnitPositionAvailable(fileIndex, rankIndex); ++rankIndex)
            rankIndexOfRear = rankIndex;
        }
        else
        {
          bool flag = true;
          for (int rankIndex = 0; rankIndex < this.RankCount; ++rankIndex)
          {
            if (this.IsUnitPositionAvailable(fileIndex, rankIndex))
            {
              flag = false;
              break;
            }
          }
          if (flag)
            rankIndexOfRear = this.RankCount - 1;
        }
      }
      if (rankIndexOfFront == -1 && rankIndexOfRear == this.RankCount)
        isFileEmtpy = true;
      else
        isFileEmtpy = false;
    }

    private void GetFlanksOfRank(
      int rankIndex,
      out bool isRankEmpty,
      out int fileIndexOfLeftFlank,
      out int fileIndexOfRightFlank,
      bool includeUnavailablePositions = false)
    {
      fileIndexOfLeftFlank = -1;
      fileIndexOfRightFlank = this.FileCount;
      for (int index1 = 0; index1 < this.FileCount; ++index1)
      {
        if (this._units2D[index1, rankIndex] != null)
        {
          fileIndexOfLeftFlank = index1;
          break;
        }
      }
      if (includeUnavailablePositions)
      {
        if (fileIndexOfLeftFlank != -1)
        {
          for (int fileIndex = fileIndexOfLeftFlank - 1; fileIndex >= 0 && !this.IsUnitPositionAvailable(fileIndex, rankIndex); --fileIndex)
            fileIndexOfLeftFlank = fileIndex;
        }
        else
        {
          bool flag = true;
          for (int fileIndex = 0; fileIndex < this.FileCount; ++fileIndex)
          {
            if (this.IsUnitPositionAvailable(fileIndex, rankIndex))
            {
              flag = false;
              break;
            }
          }
          if (flag)
            fileIndexOfLeftFlank = 0;
        }
      }
      for (int index1 = this.FileCount - 1; index1 >= 0; --index1)
      {
        if (this._units2D[index1, rankIndex] != null)
        {
          fileIndexOfRightFlank = index1;
          break;
        }
      }
      if (includeUnavailablePositions)
      {
        if (fileIndexOfRightFlank != this.FileCount)
        {
          for (int fileIndex = fileIndexOfRightFlank + 1; fileIndex < this.FileCount && !this.IsUnitPositionAvailable(fileIndex, rankIndex); ++fileIndex)
            fileIndexOfRightFlank = fileIndex;
        }
        else
        {
          bool flag = true;
          for (int fileIndex = 0; fileIndex < this.FileCount; ++fileIndex)
          {
            if (this.IsUnitPositionAvailable(fileIndex, rankIndex))
            {
              flag = false;
              break;
            }
          }
          if (flag)
            fileIndexOfRightFlank = this.FileCount - 1;
        }
      }
      if (fileIndexOfLeftFlank == -1 && fileIndexOfRightFlank == this.FileCount)
        isRankEmpty = true;
      else
        isRankEmpty = false;
    }

    private static void FillInTheGapsOfFile(
      LineFormation formation,
      int fileIndex,
      int rankIndex = 0,
      bool isCheckingLastRankForEmptiness = true)
    {
      LineFormation.FillInTheGapsOfFileAux(formation, fileIndex, rankIndex);
      while (isCheckingLastRankForEmptiness && formation.RankCount > 1 && formation.GetUnitsAtRank(formation.RankCount - 1).IsEmpty<IFormationUnit>())
        formation.Shorten();
    }

    private static void FillInTheGapsOfFileAux(
      LineFormation formation,
      int fileIndex,
      int rankIndex)
    {
      while (true)
      {
        int rankIndex1 = -1;
        for (; rankIndex < formation.RankCount - 1; ++rankIndex)
        {
          if (formation._units2D[fileIndex, rankIndex] == null && formation.IsUnitPositionAvailable(fileIndex, rankIndex))
          {
            rankIndex1 = rankIndex;
            break;
          }
        }
        int index2 = -1;
        for (; rankIndex < formation.RankCount; ++rankIndex)
        {
          if (formation._units2D[fileIndex, rankIndex] != null)
          {
            index2 = rankIndex;
            break;
          }
        }
        if (rankIndex1 != -1 && index2 != -1)
        {
          formation.RelocateUnit(formation._units2D[fileIndex, index2], fileIndex, rankIndex1);
          rankIndex = rankIndex1 + 1;
        }
        else
          break;
      }
    }

    private static void FillInTheGapsOfMiddleRanks(
      LineFormation formation,
      List<IFormationUnit> relocatedUnits = null)
    {
      int num1 = formation.RankCount - 1;
label_18:
      for (int index = 0; index < formation.FileCount; ++index)
      {
        if (formation._units2D[index, num1] == null && !formation.IsFileFullyOccupied(index))
        {
          while (true)
          {
            do
            {
              bool isFileEmtpy;
              int rankIndexOfRear;
              formation.GetFrontAndRearOfFile(index, out isFileEmtpy, out int _, out rankIndexOfRear, true);
              if (rankIndexOfRear != num1)
              {
                int num2 = rankIndexOfRear + 1;
                if (isFileEmtpy)
                {
                  num2 = -1;
                  for (int rankIndex = 0; rankIndex < formation.RankCount; ++rankIndex)
                  {
                    if (formation.IsUnitPositionAvailable(index, rankIndex))
                    {
                      num2 = rankIndex;
                      break;
                    }
                  }
                }
                IFormationUnit unitToFillIn = LineFormation.GetUnitToFillIn(formation, index, num2);
                if (unitToFillIn != null)
                {
                  formation.RelocateUnit(unitToFillIn, index, num2);
                  relocatedUnits?.Add(unitToFillIn);
                }
                else
                {
                  int num3 = num2 + 1;
                  while (num3 < formation.RankCount)
                    ++num3;
                  goto label_18;
                }
              }
              else
                goto label_18;
            }
            while (!formation.GetUnitsAtRank(num1).IsEmpty<IFormationUnit>());
            formation.Shorten();
            num1 = formation.RankCount - 1;
          }
        }
      }
      while (formation.RankCount > 1 && formation.GetUnitsAtRank(formation.RankCount - 1).IsEmpty<IFormationUnit>())
        formation.Shorten();
      LineFormation.AlignLastRank(formation);
    }

    private static void AlignRankToLeft(LineFormation formation, int fileIndex, int rankIndex)
    {
      int fileIndex1 = -1;
      for (; fileIndex < formation.FileCount - 1; ++fileIndex)
      {
        if (formation._units2D[fileIndex, rankIndex] == null && formation.IsUnitPositionAvailable(fileIndex, rankIndex))
        {
          fileIndex1 = fileIndex;
          break;
        }
      }
      int index1 = -1;
      for (; fileIndex < formation.FileCount; ++fileIndex)
      {
        if (formation._units2D[fileIndex, rankIndex] != null)
        {
          index1 = fileIndex;
          break;
        }
      }
      if (fileIndex1 == -1 || index1 == -1)
        return;
      formation.RelocateUnit(formation._units2D[index1, rankIndex], fileIndex1, rankIndex);
      LineFormation.AlignRankToLeft(formation, fileIndex1 + 1, rankIndex);
    }

    private static void AlignRankToRight(LineFormation formation, int fileIndex, int rankIndex)
    {
      int fileIndex1 = -1;
      for (; fileIndex > 0; --fileIndex)
      {
        if (formation._units2D[fileIndex, rankIndex] == null && formation.IsUnitPositionAvailable(fileIndex, rankIndex))
        {
          fileIndex1 = fileIndex;
          break;
        }
      }
      int index1 = -1;
      for (; fileIndex >= 0; --fileIndex)
      {
        if (formation._units2D[fileIndex, rankIndex] != null)
        {
          index1 = fileIndex;
          break;
        }
      }
      if (fileIndex1 == -1 || index1 == -1)
        return;
      formation.RelocateUnit(formation._units2D[index1, rankIndex], fileIndex1, rankIndex);
      LineFormation.AlignRankToRight(formation, fileIndex1 - 1, rankIndex);
    }

    private static void AlignLastRank(LineFormation formation)
    {
      int num = formation.RankCount - 1;
      bool isRankEmpty;
      int fileIndexOfLeftFlank1;
      int fileIndexOfRightFlank1;
      formation.GetFlanksOfRank(num, out isRankEmpty, out fileIndexOfLeftFlank1, out fileIndexOfRightFlank1, true);
      if (num == 0 & isRankEmpty)
        return;
      LineFormation.AlignRankToLeft(formation, fileIndexOfLeftFlank1, num);
      bool flag1 = false;
      bool flag2 = false;
      while (true)
      {
        formation.GetFlanksOfRank(num, out isRankEmpty, out fileIndexOfLeftFlank1, out fileIndexOfRightFlank1, true);
        if (!flag1 && fileIndexOfLeftFlank1 < formation.FileCount - fileIndexOfRightFlank1 - 1 - 1)
        {
          int fileIndexOfRightFlank2;
          formation.GetFlanksOfRank(num, out isRankEmpty, out int _, out fileIndexOfRightFlank2);
          formation.RelocateUnit(formation._units2D[fileIndexOfRightFlank2, num], fileIndexOfRightFlank1 + 1, num);
          LineFormation.AlignRankToRight(formation, fileIndexOfRightFlank1 + 1, num);
          flag2 = true;
        }
        else if (!flag2 && fileIndexOfLeftFlank1 - 1 > formation.FileCount - fileIndexOfRightFlank1 - 1)
        {
          int fileIndexOfLeftFlank2;
          formation.GetFlanksOfRank(num, out isRankEmpty, out fileIndexOfLeftFlank2, out int _);
          formation.RelocateUnit(formation._units2D[fileIndexOfLeftFlank2, num], fileIndexOfLeftFlank1 - 1, num);
          LineFormation.AlignRankToLeft(formation, fileIndexOfLeftFlank1 - 1, num);
          flag1 = true;
        }
        else
          break;
      }
    }

    private IEnumerable<IFormationUnit> GetUnitsAtFile(int fileIndex)
    {
      for (int rankIndex = 0; rankIndex < this.RankCount; ++rankIndex)
      {
        if (this._units2D[fileIndex, rankIndex] != null)
          yield return this._units2D[fileIndex, rankIndex];
      }
    }

    private IEnumerable<IFormationUnit> GetUnitsAtRank(int rankIndex)
    {
      for (int fileIndex = 0; fileIndex < this.FileCount; ++fileIndex)
      {
        if (this._units2D[fileIndex, rankIndex] != null)
          yield return this._units2D[fileIndex, rankIndex];
      }
    }

    private bool IsFileFullyOccupied(int fileIndex)
    {
      bool flag = true;
      for (int index = 0; index < this.RankCount; ++index)
      {
        if (this._units2D[fileIndex, index] == null && this.IsUnitPositionAvailable(fileIndex, index))
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    private bool IsRankFullyOccupied(int rankIndex)
    {
      bool flag = true;
      for (int index = 0; index < this.FileCount; ++index)
      {
        if (this._units2D[index, rankIndex] == null && this.IsUnitPositionAvailable(index, rankIndex))
        {
          flag = false;
          break;
        }
      }
      return flag;
    }

    private static IFormationUnit GetUnitToFillIn(
      LineFormation formation,
      int relocationFileIndex,
      int relocationRankIndex)
    {
      for (int index = formation.RankCount - 1; index >= 0; --index)
      {
        if (relocationRankIndex == index)
          return (IFormationUnit) null;
        bool isRankEmpty;
        int fileIndexOfLeftFlank;
        int fileIndexOfRightFlank;
        formation.GetFlanksOfRank(index, out isRankEmpty, out fileIndexOfLeftFlank, out fileIndexOfRightFlank);
        if (!isRankEmpty)
          return relocationFileIndex > fileIndexOfRightFlank || relocationFileIndex >= fileIndexOfLeftFlank && fileIndexOfRightFlank - relocationFileIndex <= relocationFileIndex - fileIndexOfLeftFlank ? formation._units2D[fileIndexOfRightFlank, index] : formation._units2D[fileIndexOfLeftFlank, index];
      }
      return (IFormationUnit) null;
    }

    private void RelocateUnit(IFormationUnit unit, int fileIndex, int rankIndex)
    {
      this._units2D[unit.FormationFileIndex, unit.FormationRankIndex] = (IFormationUnit) null;
      this._units2D[fileIndex, rankIndex] = unit;
      this.ReconstructUnitsFromUnits2D();
      unit.FormationFileIndex = fileIndex;
      unit.FormationRankIndex = rankIndex;
      Action onShapeChanged = this.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    public int UnitCount => this.GetAllUnits().Count;

    public int PositionedUnitCount => this.UnitCount - this._unpositionedUnits.Count;

    public List<IFormationUnit> GetAllUnits() => this._allUnits;

    public List<IFormationUnit> GetUnpositionedUnits() => this._unpositionedUnits;

    public Vec2? GetLocalDirectionOfRelativeFormationLocation(IFormationUnit unit)
    {
      if (this._unpositionedUnits.Contains(unit))
        return new Vec2?();
      Vec2 vec2 = new Vec2((float) unit.FormationFileIndex, (float) -unit.FormationRankIndex) - new Vec2((float) (this.FileCount - 1) * 0.5f, (float) (this.RankCount - 1) * -0.5f);
      double num = (double) vec2.Normalize();
      return new Vec2?(vec2);
    }

    public Vec2? GetLocalWallDirectionOfRelativeFormationLocation(IFormationUnit unit)
    {
      if (this._unpositionedUnits.Contains(unit))
        return new Vec2?();
      Vec2 vec2 = new Vec2((float) unit.FormationFileIndex, (float) -unit.FormationRankIndex) - new Vec2((float) (this.FileCount - 1) * 0.5f, (float) -this.RankCount);
      double num = (double) vec2.Normalize();
      return new Vec2?(vec2);
    }

    internal void GetFormationInfo(out int fileCount, out int rankCount)
    {
      fileCount = this.FileCount;
      rankCount = this.RankCount;
    }

    [Conditional("DEBUG")]
    private void AssertUnit(IFormationUnit unit, bool isAssertingUnitPositionAvailability = true)
    {
      if (!isAssertingUnitPositionAvailability)
        return;
      this.IsUnitPositionRestrained(unit.FormationFileIndex, unit.FormationRankIndex);
      int num = !this.IsMiddleFrontUnitPositionReserved || this.GetMiddleFrontUnitPosition().Item1 != unit.FormationFileIndex ? 0 : (this.GetMiddleFrontUnitPosition().Item2 == unit.FormationRankIndex ? 1 : 0);
      this.IsUnitPositionAvailable(unit.FormationFileIndex, unit.FormationRankIndex);
    }

    [Conditional("DEBUG")]
    private void AssertUnpositionedUnit(IFormationUnit unit)
    {
    }

    public float GetUnitsDistanceToFrontLine(IFormationUnit unit) => this._unpositionedUnits.Contains(unit) ? -1f : (float) ((double) unit.FormationRankIndex * ((double) this.Distance + (double) this.UnitDiameter) + (double) this.UnitDiameter * 0.5);

    public IFormationUnit GetNeighbourUnit(
      IFormationUnit unit,
      int fileOffset,
      int rankOffset)
    {
      if (this._unpositionedUnits.Contains(unit))
        return (IFormationUnit) null;
      int index1 = unit.FormationFileIndex + fileOffset;
      int index2 = unit.FormationRankIndex + rankOffset;
      return index1 >= 0 && index1 < this.FileCount && (index2 >= 0 && index2 < this.RankCount) ? this._units2D[index1, index2] : (IFormationUnit) null;
    }

    public IFormationUnit GetNeighbourUnitOfLeftSide(IFormationUnit unit)
    {
      if (this._unpositionedUnits.Contains(unit))
        return (IFormationUnit) null;
      int formationRankIndex = unit.FormationRankIndex;
      for (int index1 = unit.FormationFileIndex - 1; index1 >= 0; --index1)
      {
        if (this._units2D[index1, formationRankIndex] != null)
          return this._units2D[index1, formationRankIndex];
      }
      return (IFormationUnit) null;
    }

    public IFormationUnit GetNeighbourUnitOfRightSide(IFormationUnit unit)
    {
      if (this._unpositionedUnits.Contains(unit))
        return (IFormationUnit) null;
      int formationRankIndex = unit.FormationRankIndex;
      for (int index1 = unit.FormationFileIndex + 1; index1 < this.FileCount; ++index1)
      {
        if (this._units2D[index1, formationRankIndex] != null)
          return this._units2D[index1, formationRankIndex];
      }
      return (IFormationUnit) null;
    }

    public void SwitchUnitLocationsWithUnpositionedUnit(
      IFormationUnit firstUnit,
      IFormationUnit secondUnit)
    {
      int formationFileIndex = firstUnit.FormationFileIndex;
      int formationRankIndex = firstUnit.FormationRankIndex;
      this._unpositionedUnits.Remove(secondUnit);
      this._units2D[formationFileIndex, formationRankIndex] = secondUnit;
      secondUnit.FormationFileIndex = formationFileIndex;
      secondUnit.FormationRankIndex = formationRankIndex;
      firstUnit.FormationFileIndex = -1;
      firstUnit.FormationRankIndex = -1;
      this._unpositionedUnits.Add(firstUnit);
      this.ReconstructUnitsFromUnits2D();
      Action onShapeChanged = this.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    public void SwitchUnitLocations(IFormationUnit firstUnit, IFormationUnit secondUnit)
    {
      int formationFileIndex1 = firstUnit.FormationFileIndex;
      int formationRankIndex1 = firstUnit.FormationRankIndex;
      int formationFileIndex2 = secondUnit.FormationFileIndex;
      int formationRankIndex2 = secondUnit.FormationRankIndex;
      this._units2D[formationFileIndex1, formationRankIndex1] = secondUnit;
      this._units2D[formationFileIndex2, formationRankIndex2] = firstUnit;
      this.ReconstructUnitsFromUnits2D();
      firstUnit.FormationFileIndex = formationFileIndex2;
      firstUnit.FormationRankIndex = formationRankIndex2;
      secondUnit.FormationFileIndex = formationFileIndex1;
      secondUnit.FormationRankIndex = formationRankIndex1;
      Action onShapeChanged = this.OnShapeChanged;
      if (onShapeChanged == null)
        return;
      onShapeChanged();
    }

    public void BeforeFormationFrameChange()
    {
    }

    internal void BatchUnitPositionAvailabilities(bool isUpdatingCachedOrderedLocalPositions = true)
    {
      if (isUpdatingCachedOrderedLocalPositions)
        this.AreLocalPositionsDirty = true;
      bool localPositionsDirty = this.AreLocalPositionsDirty;
      this.AreLocalPositionsDirty = false;
      if (localPositionsDirty)
      {
        this._cachedOrderedUnitPositionIndices.Clear();
        for (int unitIndex = 0; unitIndex < this.FileCount * this.RankCount; ++unitIndex)
          this._cachedOrderedUnitPositionIndices.Add(this.GetOrderedUnitPositionIndex(unitIndex));
        this._cachedOrderedLocalPositions.Clear();
        for (int index = 0; index < this._cachedOrderedUnitPositionIndices.Count; ++index)
        {
          MBList<Vec2> orderedLocalPositions = this._cachedOrderedLocalPositions;
          Vec2i unitPositionIndex = this._cachedOrderedUnitPositionIndices[index];
          int fileIndex = unitPositionIndex.Item1;
          unitPositionIndex = this._cachedOrderedUnitPositionIndices[index];
          int rankIndex = unitPositionIndex.Item2;
          Vec2 localPositionOfUnit = this.GetLocalPositionOfUnit(fileIndex, rankIndex);
          orderedLocalPositions.Add(localPositionOfUnit);
        }
      }
      foreach (Vec2i restrainedUnitPosition in this.GetRestrainedUnitPositions())
        this._unitPositionAvailabilities[restrainedUnitPosition.Item1, restrainedUnitPosition.Item2] = 1;
      if (!this.owner.BatchUnitPositions(this._cachedOrderedUnitPositionIndices, this._cachedOrderedLocalPositions, this._unitPositionAvailabilities, this._globalPositions, this.FileCount, this.RankCount))
      {
        for (int index1 = 0; index1 < this.FileCount; ++index1)
        {
          for (int index2 = 0; index2 < this.RankCount; ++index2)
            this._unitPositionAvailabilities[index1, index2] = 1;
        }
      }
      if (localPositionsDirty)
      {
        this._cachedOrderedAndAvailableUnitPositionIndices.Clear();
        for (int index = 0; index < this._cachedOrderedUnitPositionIndices.Count; ++index)
        {
          Vec2i unitPositionIndex = this._cachedOrderedUnitPositionIndices[index];
          if (this._unitPositionAvailabilities[unitPositionIndex.Item1, unitPositionIndex.Item2] == 2)
            this._cachedOrderedAndAvailableUnitPositionIndices.Add(unitPositionIndex);
        }
      }
      this._isCavalry = this.owner is Formation && (this.owner as Formation).CalculateHasSignificantNumberOfMounted;
    }

    public void OnFormationFrameChanged()
    {
      this._unitPositionAvailabilities.Clear();
      this.BatchUnitPositionAvailabilities(false);
      bool flag1 = LineFormation.ShiftUnitsBackwardsForNewUnavailableUnitPositions(this);
      for (int fileIndex = 0; fileIndex < this.FileCount; ++fileIndex)
        LineFormation.FillInTheGapsOfFile(this, fileIndex, isCheckingLastRankForEmptiness: (fileIndex == this.FileCount - 1));
      int num = flag1 ? 1 : 0;
      bool flag2 = this.TryReaddingUnpositionedUnits();
      if (num != 0 && !flag2)
        this.owner.OnUnitAddedOrRemoved();
      LineFormation.FillInTheGapsOfMiddleRanks(this);
    }

    private bool TryReaddingUnpositionedUnits()
    {
      bool flag = this._unpositionedUnits.Count > 0;
      int index;
      for (int val1 = this._unpositionedUnits.Count - 1; val1 >= 0; val1 = index - 1)
      {
        index = Math.Min(val1, this._unpositionedUnits.Count - 1);
        if (index >= 0)
        {
          IFormationUnit unpositionedUnit = this._unpositionedUnits[index];
          this.RemoveUnit(unpositionedUnit);
          if (!this.AddUnit(unpositionedUnit))
            break;
        }
        else
          break;
      }
      if (flag)
        this.owner.OnUnitAddedOrRemoved();
      return flag;
    }

    private bool AreLastRanksCompletelyUnavailable(int numberOfRanksToCheck = 3)
    {
      bool flag = true;
      if (this.RankCount < numberOfRanksToCheck)
      {
        flag = false;
      }
      else
      {
        for (int fileIndex = 0; fileIndex < this.FileCount; ++fileIndex)
        {
          for (int rankIndex = this.RankCount - numberOfRanksToCheck; rankIndex < this.RankCount; ++rankIndex)
          {
            if (this.IsUnitPositionAvailable(fileIndex, rankIndex))
            {
              fileIndex = 2147483646;
              flag = false;
              break;
            }
          }
        }
      }
      return flag;
    }

    [Conditional("DEBUG")]
    private void AssertUnitPositions()
    {
      for (int index1 = 0; index1 < this._units2D.Count1; ++index1)
      {
        for (int index2 = 0; index2 < this._units2D.Count2; ++index2)
        {
          IFormationUnit formationUnit = this._units2D[index1, index2];
        }
      }
      foreach (IFormationUnit unpositionedUnit in this._unpositionedUnits)
        ;
    }

    [Conditional("DEBUG")]
    private void AssertFilePositions(int fileIndex)
    {
      bool isFileEmtpy;
      int rankIndexOfFront;
      int rankIndexOfRear;
      this.GetFrontAndRearOfFile(fileIndex, out isFileEmtpy, out rankIndexOfFront, out rankIndexOfRear, true);
      if (isFileEmtpy)
        return;
      int num = rankIndexOfFront;
      while (num <= rankIndexOfRear)
        ++num;
    }

    [Conditional("DEBUG")]
    private void AssertRankPositions(int rankIndex)
    {
      bool isRankEmpty;
      int fileIndexOfLeftFlank;
      int fileIndexOfRightFlank;
      this.GetFlanksOfRank(rankIndex, out isRankEmpty, out fileIndexOfLeftFlank, out fileIndexOfRightFlank, true);
      if (isRankEmpty)
        return;
      int num = fileIndexOfLeftFlank;
      while (num <= fileIndexOfRightFlank)
        ++num;
    }

    public void OnFormationDispersed()
    {
      IEnumerable<Vec2i> vec2is = this.GetOrderedUnitPositionIndices().Where<Vec2i>((Func<Vec2i, bool>) (i => this.IsUnitPositionAvailable(i.Item1, i.Item2)));
      List<IFormationUnit> list = this.GetAllUnits().ToList<IFormationUnit>();
      foreach (Vec2i vec2i in vec2is)
      {
        int num1 = vec2i.Item1;
        int num2 = vec2i.Item2;
        IFormationUnit firstUnit = this._units2D[num1, num2];
        if (firstUnit != null)
        {
          IFormationUnit closestUnitTo = this.owner.GetClosestUnitTo(this.GetLocalPositionOfUnit(num1, num2), list);
          list[list.IndexOf(closestUnitTo)] = (IFormationUnit) null;
          if (firstUnit != closestUnitTo)
          {
            if (closestUnitTo.FormationFileIndex == -1)
              this.SwitchUnitLocationsWithUnpositionedUnit(firstUnit, closestUnitTo);
            else
              this.SwitchUnitLocations(firstUnit, closestUnitTo);
          }
        }
      }
    }

    public void OnUnitLostMount(IFormationUnit unit)
    {
    }

    public bool IsTurnBackwardsNecessary(
      WorldPosition previousPosition,
      WorldPosition? newPosition,
      Vec2 previousDirection,
      Vec2? newDirection)
    {
      return newDirection.HasValue && (double) Math.Abs(MBMath.GetSmallestDifferenceBetweenTwoAngles(newDirection.Value.RotationInRadians, previousDirection.RotationInRadians)) >= 2.35619449615479;
    }

    public void TurnBackwards()
    {
      for (int index1 = 0; index1 <= this.FileCount / 2; ++index1)
      {
        int num1 = index1;
        int num2 = this.FileCount - 1 - index1;
        for (int index2 = 0; index2 < this.RankCount; ++index2)
        {
          int num3 = index2;
          int num4 = this.RankCount - 1 - index2;
          IFormationUnit formationUnit1 = this._units2D[num1, num3];
          IFormationUnit formationUnit2 = this._units2D[num2, num4];
          if (formationUnit1 != formationUnit2)
          {
            if (formationUnit1 != null && formationUnit2 != null)
              this.SwitchUnitLocations(formationUnit1, formationUnit2);
            else if (formationUnit1 != null)
            {
              if (this.IsUnitPositionAvailable(num2, num4))
                this.RelocateUnit(formationUnit1, num2, num4);
            }
            else if (formationUnit2 != null && this.IsUnitPositionAvailable(num1, num3))
              this.RelocateUnit(formationUnit2, num1, num3);
          }
        }
      }
    }

    public float GetOccupationWidth(int unitCount)
    {
      if (unitCount < 1)
        return 0.0f;
      int num1 = this.FileCount - 1;
      int num2 = 0;
      for (int unitIndex = 0; unitIndex < this.FileCount * this.RankCount; ++unitIndex)
      {
        Vec2i unitPositionIndex = this.GetOrderedUnitPositionIndex(unitIndex);
        if (unitPositionIndex.Item1 < num1)
          num1 = unitPositionIndex.Item1;
        if (unitPositionIndex.Item1 > num2)
          num2 = unitPositionIndex.Item1;
        if (this.IsUnitPositionAvailable(unitPositionIndex.Item1, unitPositionIndex.Item2))
        {
          --unitCount;
          if (unitCount == 0)
            break;
        }
      }
      return (float) (num2 - num1) * (this.Interval + this.UnitDiameter) + this.UnitDiameter;
    }

    public void InvalidateCacheOfUnitAux(Vec2 roundedLocalPosition)
    {
      int fileIndex;
      int rankIndex;
      if (!this.TryGetUnitPositionIndexFromLocalPosition(roundedLocalPosition, out fileIndex, out rankIndex))
        return;
      this._unitPositionAvailabilities[fileIndex, rankIndex] = 0;
    }

    public Vec2? CreateNewPosition(int unitIndex)
    {
      Vec2? nullable = new Vec2?();
      for (int index = 100; !nullable.HasValue && index > 0 && (!this.AreLastRanksCompletelyUnavailable() && this.IsDeepenApplicable()); --index)
      {
        this.Deepen();
        nullable = this.GetLocalPositionOfUnitOrDefault(unitIndex);
      }
      return nullable;
    }

    public virtual void RearrangeFrom(IFormationArrangement arrangement) => this.BatchUnitPositionAvailabilities();

    public virtual void RearrangeTo(IFormationArrangement arrangement)
    {
      if (!(arrangement is ColumnFormation))
        return;
      this.IsTransforming = true;
      this.ReleaseMiddleFrontUnitPosition();
    }

    public virtual void RearrangeTransferUnits(IFormationArrangement arrangement)
    {
      if (arrangement is LineFormation lineFormation)
      {
        lineFormation._units2D = this._units2D;
        lineFormation._allUnits = this._allUnits;
        lineFormation._unitPositionAvailabilities = this._unitPositionAvailabilities;
        lineFormation._globalPositions = this._globalPositions;
        lineFormation._unpositionedUnits = this._unpositionedUnits;
        lineFormation.AreLocalPositionsDirty = true;
        lineFormation.OnFormationFrameChanged();
      }
      else
      {
        for (int unitIndex = 0; unitIndex < this.FileCount * this.RankCount; ++unitIndex)
        {
          Vec2i unitPositionIndex = this.GetOrderedUnitPositionIndex(unitIndex);
          IFormationUnit unit = this._units2D[unitPositionIndex.Item1, unitPositionIndex.Item2];
          if (unit != null)
          {
            unit.FormationFileIndex = -1;
            unit.FormationRankIndex = -1;
            arrangement.AddUnit(unit);
          }
        }
        foreach (IFormationUnit unpositionedUnit in this._unpositionedUnits)
          arrangement.AddUnit(unpositionedUnit);
      }
    }

    internal static float CalculateWidth(float interval, float unitDiameter, int unitCountOnLine) => (float) Math.Max(0, unitCountOnLine - 1) * (interval + unitDiameter) + unitDiameter;

    public void FormFromWidth(int unitCountOnLine, bool skipSingleFileChangesForPerformance = false)
    {
      if (skipSingleFileChangesForPerformance && Math.Abs(this.FileCount - unitCountOnLine) <= 1)
        return;
      this.Width = LineFormation.CalculateWidth(this.Interval, this.UnitDiameter, unitCountOnLine);
    }

    public void ReserveMiddleFrontUnitPosition(IFormationUnit vanguard)
    {
      this.IsMiddleFrontUnitPositionReserved = true;
      this.OnFormationFrameChanged();
    }

    public void ReleaseMiddleFrontUnitPosition()
    {
      this.IsMiddleFrontUnitPositionReserved = false;
      this.OnFormationFrameChanged();
    }

    private Vec2i GetMiddleFrontUnitPosition() => this.GetOrderedUnitPositionIndex(0);

    public Vec2 GetLocalPositionOfReservedUnitPosition()
    {
      Vec2i frontUnitPosition = this.GetMiddleFrontUnitPosition();
      return this.GetLocalPositionOfUnit(frontUnitPosition.Item1, frontUnitPosition.Item2);
    }

    public virtual void OnTickOccasionallyOfUnit(IFormationUnit unit, bool arrangementChangeAllowed)
    {
      if (!arrangementChangeAllowed || unit.FormationRankIndex <= 0 || (!(unit is Agent) || !(unit as Agent).HasShieldCached) || !(this._units2D[unit.FormationFileIndex, unit.FormationRankIndex - 1] is Agent agent1))
        return;
      if (!agent1.HasShieldCached)
      {
        this.SwitchUnitLocations(unit, (IFormationUnit) agent1);
      }
      else
      {
        for (int index = 1; unit.FormationFileIndex - index >= 0 || unit.FormationFileIndex + index < this.FileCount; ++index)
        {
          if (unit.FormationFileIndex - index >= 0 && this._units2D[unit.FormationFileIndex - index, unit.FormationRankIndex - 1] is Agent agent8 && !agent8.HasShieldCached)
          {
            this.SwitchUnitLocations(unit, (IFormationUnit) agent8);
            break;
          }
          if (unit.FormationFileIndex + index < this.FileCount && this._units2D[unit.FormationFileIndex + index, unit.FormationRankIndex - 1] is Agent agent9 && !agent9.HasShieldCached)
          {
            this.SwitchUnitLocations(unit, (IFormationUnit) agent9);
            break;
          }
        }
      }
    }

    public virtual float GetDirectionChangeTendencyOfUnit(IFormationUnit unit) => this.RankCount == 1 || unit.FormationRankIndex == -1 ? 0.0f : (float) unit.FormationRankIndex * 1f / (float) (this.RankCount - 1);

    internal int GetCachedOrderedAndAvailableUnitPositionIndicesCount() => this._cachedOrderedAndAvailableUnitPositionIndices.Count;

    internal Vec2i GetCachedOrderedAndAvailableUnitPositionIndexAt(int i) => this._cachedOrderedAndAvailableUnitPositionIndices[i];

    internal WorldPosition GetGlobalPositionAtIndex(int indexX, int indexY) => this._globalPositions[indexX, indexY];
  }
}
