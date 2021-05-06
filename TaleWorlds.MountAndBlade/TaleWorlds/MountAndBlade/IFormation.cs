// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IFormation
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public interface IFormation
  {
    float Interval { get; }

    float Distance { get; }

    float UnitDiameter { get; }

    float MinimumInterval { get; }

    int? OverridenUnitCount { get; }

    bool GetIsLocalPositionAvailable(Vec2 localPosition, Vec2? nearestAvailableUnitPositionLocal);

    bool BatchUnitPositions(
      MBList<Vec2i> orderedPositionIndices,
      MBList<Vec2> orderedLocalPositions,
      MBList2D<int> availabilityTable,
      MBList2D<WorldPosition> globalPositionTable,
      int fileCount,
      int rankCount);

    IFormationUnit GetClosestUnitTo(
      Vec2 localPosition,
      List<IFormationUnit> unitsWithSpaces = null,
      float? maxDistance = null);

    IFormationUnit GetClosestUnitTo(
      IFormationUnit targetUnit,
      List<IFormationUnit> unitsWithSpaces = null,
      float? maxDistance = null);

    void OnUnitAddedOrRemoved();

    void SetUnitToFollow(IFormationUnit unit, IFormationUnit toFollow, Vec2 vector);

    void GetClosestAttachmentPointToTheOrderPosition(
      out bool left,
      out bool right,
      out bool front,
      out bool rear);
  }
}
