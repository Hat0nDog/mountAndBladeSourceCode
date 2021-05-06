// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IFormationArrangement
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public interface IFormationArrangement
  {
    float Width { get; set; }

    float Depth { get; }

    float MinimumWidth { get; }

    bool? IsLoose { get; }

    List<IFormationUnit> GetAllUnits();

    List<IFormationUnit> GetUnpositionedUnits();

    int UnitCount { get; }

    int RankCount { get; }

    int PositionedUnitCount { get; }

    bool AddUnit(IFormationUnit unit);

    void RemoveUnit(IFormationUnit unit);

    void OnBatchRemoveStart();

    void OnBatchRemoveEnd();

    Vec2? GetLocalPositionOfUnitOrDefault(int unitIndex);

    Vec2? GetLocalPositionOfUnitOrDefault(IFormationUnit unit);

    Vec2? GetLocalDirectionOfUnitOrDefault(int unitIndex);

    Vec2? GetLocalDirectionOfUnitOrDefault(IFormationUnit unit);

    WorldPosition? GetWorldPositionOfUnitOrDefault(int unitIndex);

    WorldPosition? GetWorldPositionOfUnitOrDefault(IFormationUnit unit);

    IEnumerable<IFormationUnit> GetUnitsToPop(int count);

    void SwitchUnitLocations(IFormationUnit firstUnit, IFormationUnit secondUnit);

    void SwitchUnitLocationsWithUnpositionedUnit(
      IFormationUnit firstUnit,
      IFormationUnit secondUnit);

    IFormationUnit GetNeighbourUnit(
      IFormationUnit unit,
      int fileOffset,
      int rankOffset);

    IFormationUnit GetNeighbourUnitOfLeftSide(IFormationUnit unit);

    IFormationUnit GetNeighbourUnitOfRightSide(IFormationUnit unit);

    Vec2? GetLocalWallDirectionOfRelativeFormationLocation(IFormationUnit unit);

    IEnumerable<Vec2> GetUnavailableUnitPositions();

    float GetOccupationWidth(int unitCount);

    Vec2? CreateNewPosition(int unitIndex);

    void BeforeFormationFrameChange();

    void OnFormationFrameChanged();

    bool IsTurnBackwardsNecessary(
      WorldPosition previousPosition,
      WorldPosition? newPosition,
      Vec2 previousDirection,
      Vec2? newDirection);

    void TurnBackwards();

    void OnFormationDispersed();

    void Reset();

    IFormationArrangement Clone(IFormation formation);

    void DeepCopyFrom(IFormationArrangement arrangement);

    void RearrangeTo(IFormationArrangement arrangement);

    void RearrangeFrom(IFormationArrangement arrangement);

    void RearrangeTransferUnits(IFormationArrangement arrangement);

    event Action OnWidthChanged;

    event Action OnShapeChanged;

    void ReserveMiddleFrontUnitPosition(IFormationUnit vanguard);

    void ReleaseMiddleFrontUnitPosition();

    Vec2 GetLocalPositionOfReservedUnitPosition();

    void OnTickOccasionallyOfUnit(IFormationUnit unit, bool arrangementChangeAllowed);

    void OnUnitLostMount(IFormationUnit unit);

    float GetDirectionChangeTendencyOfUnit(IFormationUnit unit);

    bool AreLocalPositionsDirty { set; }
  }
}
