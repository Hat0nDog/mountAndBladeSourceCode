// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ArcherPosition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  internal class ArcherPosition
  {
    private FormationAI.BehaviorSide _closestSide;
    private int _connectedSides;
    private SiegeQuerySystem _siegeQuerySystem;
    private Formation[] _lastAssignedFormations;

    internal GameEntity Entity { get; }

    internal TacticalPosition TacticalArcherPosition { get; }

    internal int ConnectedSides
    {
      get => this._connectedSides;
      private set => this._connectedSides = value;
    }

    internal Formation GetLastAssignedFormation(int teamIndex) => teamIndex >= 0 ? this._lastAssignedFormations[teamIndex] : (Formation) null;

    public ArcherPosition(GameEntity _entity, SiegeQuerySystem siegeQuerySystem)
    {
      this.Entity = _entity;
      this.TacticalArcherPosition = this.Entity.GetFirstScriptOfType<TacticalPosition>();
      this._siegeQuerySystem = siegeQuerySystem;
      this.DetermineArcherPositionSide(BattleSideEnum.Defender);
      this._lastAssignedFormations = new Formation[Mission.Current.Teams.Count];
    }

    private static int ConvertToBinaryPow(int pow) => 1 << pow;

    public bool IsArcherPositionRelatedToSide(FormationAI.BehaviorSide side) => (uint) (ArcherPosition.ConvertToBinaryPow((int) side) & this.ConnectedSides) > 0U;

    internal FormationAI.BehaviorSide GetArcherPositionClosestSide() => this._closestSide;

    internal void OnDeploymentFinished(SiegeQuerySystem siegeQuerySystem, BattleSideEnum battleSide)
    {
      this._siegeQuerySystem = siegeQuerySystem;
      this.DetermineArcherPositionSide(battleSide);
    }

    private void DetermineArcherPositionSide(BattleSideEnum battleSide)
    {
      this.ConnectedSides = 0;
      if (this.TacticalArcherPosition != null)
      {
        int tacticalPositionSide = (int) this.TacticalArcherPosition.TacticalPositionSide;
        if (tacticalPositionSide < 3)
        {
          this._closestSide = this.TacticalArcherPosition.TacticalPositionSide;
          this.ConnectedSides = ArcherPosition.ConvertToBinaryPow(tacticalPositionSide);
        }
      }
      if (this.ConnectedSides != 0)
        return;
      if (battleSide == BattleSideEnum.Defender)
        ArcherPosition.CalculateArcherPositionSideUsingDefenderLanes(this._siegeQuerySystem, this.Entity.GlobalPosition, out this._closestSide, out this._connectedSides);
      else
        ArcherPosition.CalculateArcherPositionSideUsingAttackerRegions(this._siegeQuerySystem, this.Entity.GlobalPosition, out this._closestSide, out this._connectedSides);
    }

    private static void CalculateArcherPositionSideUsingAttackerRegions(
      SiegeQuerySystem siegeQuerySystem,
      Vec3 position,
      out FormationAI.BehaviorSide _closestSide,
      out int ConnectedSides)
    {
      float num1 = position.AsVec2.DistanceSquared(siegeQuerySystem.LeftAttackerOrigin);
      float num2 = position.AsVec2.DistanceSquared(siegeQuerySystem.MiddleAttackerOrigin);
      float num3 = position.AsVec2.DistanceSquared(siegeQuerySystem.RightAttackerOrigin);
      FormationAI.BehaviorSide behaviorSide = (double) num1 >= (double) num2 || (double) num1 >= (double) num3 ? ((double) num3 >= (double) num2 ? FormationAI.BehaviorSide.Middle : FormationAI.BehaviorSide.Right) : FormationAI.BehaviorSide.Left;
      _closestSide = behaviorSide;
      ConnectedSides = ArcherPosition.ConvertToBinaryPow((int) behaviorSide);
      Vec2 vec2_1 = position.AsVec2 - siegeQuerySystem.LeftDefenderOrigin;
      if ((double) vec2_1.DotProduct(siegeQuerySystem.LeftToMidDir) >= 0.0 && (double) vec2_1.DotProduct(siegeQuerySystem.LeftToMidDir.RightVec()) >= 0.0)
      {
        ConnectedSides |= ArcherPosition.ConvertToBinaryPow(0);
      }
      else
      {
        Vec2 vec2_2 = position.AsVec2 - siegeQuerySystem.MidDefenderOrigin;
        if ((double) vec2_2.DotProduct(siegeQuerySystem.MidToLeftDir) >= 0.0 && (double) vec2_2.DotProduct(siegeQuerySystem.MidToLeftDir.RightVec()) >= 0.0)
          ConnectedSides |= ArcherPosition.ConvertToBinaryPow(0);
      }
      Vec2 vec2_3 = position.AsVec2 - siegeQuerySystem.MidDefenderOrigin;
      if ((double) vec2_3.DotProduct(siegeQuerySystem.LeftToMidDir) >= 0.0 && (double) vec2_3.DotProduct(siegeQuerySystem.LeftToMidDir.LeftVec()) >= 0.0)
      {
        ConnectedSides |= ArcherPosition.ConvertToBinaryPow(1);
      }
      else
      {
        Vec2 vec2_2 = position.AsVec2 - siegeQuerySystem.RightDefenderOrigin;
        if ((double) vec2_2.DotProduct(siegeQuerySystem.RightToMidDir) >= 0.0 && (double) vec2_2.DotProduct(siegeQuerySystem.RightToMidDir.RightVec()) >= 0.0)
          ConnectedSides |= ArcherPosition.ConvertToBinaryPow(1);
      }
      Vec2 vec2_4 = position.AsVec2 - siegeQuerySystem.RightDefenderOrigin;
      if ((double) vec2_4.DotProduct(siegeQuerySystem.MidToRightDir) >= 0.0 && (double) vec2_4.DotProduct(siegeQuerySystem.MidToRightDir.LeftVec()) >= 0.0)
      {
        ConnectedSides |= ArcherPosition.ConvertToBinaryPow(2);
      }
      else
      {
        Vec2 vec2_2 = position.AsVec2 - siegeQuerySystem.RightDefenderOrigin;
        if ((double) vec2_2.DotProduct(siegeQuerySystem.RightToMidDir) < 0.0 || (double) vec2_2.DotProduct(siegeQuerySystem.RightToMidDir.LeftVec()) < 0.0)
          return;
        ConnectedSides |= ArcherPosition.ConvertToBinaryPow(2);
      }
    }

    private static void CalculateArcherPositionSideUsingDefenderLanes(
      SiegeQuerySystem siegeQuerySystem,
      Vec3 position,
      out FormationAI.BehaviorSide _closestSide,
      out int ConnectedSides)
    {
      float num1 = position.AsVec2.DistanceSquared(siegeQuerySystem.LeftDefenderOrigin);
      float num2 = position.AsVec2.DistanceSquared(siegeQuerySystem.MidDefenderOrigin);
      float num3 = position.AsVec2.DistanceSquared(siegeQuerySystem.RightDefenderOrigin);
      FormationAI.BehaviorSide behaviorSide1 = (double) num1 >= (double) num2 || (double) num1 >= (double) num3 ? ((double) num3 >= (double) num2 ? FormationAI.BehaviorSide.Middle : FormationAI.BehaviorSide.Right) : FormationAI.BehaviorSide.Left;
      FormationAI.BehaviorSide behaviorSide2 = FormationAI.BehaviorSide.BehaviorSideNotSet;
      switch (behaviorSide1)
      {
        case FormationAI.BehaviorSide.Left:
          if ((double) (position.AsVec2 - siegeQuerySystem.LeftDefenderOrigin).Normalized().DotProduct(siegeQuerySystem.DefenderLeftToDefenderMidDir) > 0.0)
          {
            behaviorSide2 = FormationAI.BehaviorSide.Middle;
            break;
          }
          break;
        case FormationAI.BehaviorSide.Middle:
          behaviorSide2 = (double) (position.AsVec2 - siegeQuerySystem.MidDefenderOrigin).Normalized().DotProduct(siegeQuerySystem.DefenderMidToDefenderRightDir) <= 0.0 ? FormationAI.BehaviorSide.Left : FormationAI.BehaviorSide.Right;
          break;
        case FormationAI.BehaviorSide.Right:
          if ((double) (position.AsVec2 - siegeQuerySystem.RightDefenderOrigin).Normalized().DotProduct(siegeQuerySystem.DefenderMidToDefenderRightDir) < 0.0)
          {
            behaviorSide2 = FormationAI.BehaviorSide.Middle;
            break;
          }
          break;
      }
      _closestSide = behaviorSide1;
      ConnectedSides = ArcherPosition.ConvertToBinaryPow((int) behaviorSide1);
      if (behaviorSide2 == FormationAI.BehaviorSide.BehaviorSideNotSet)
        return;
      ConnectedSides |= ArcherPosition.ConvertToBinaryPow((int) behaviorSide2);
    }

    internal void SetLastAssignedFormation(int teamIndex, Formation formation)
    {
      if (teamIndex < 0)
        return;
      this._lastAssignedFormations[teamIndex] = formation;
    }
  }
}
