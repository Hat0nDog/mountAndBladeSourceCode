// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeQuerySystem
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class SiegeQuerySystem
  {
    private const float LaneProximityDistance = 10f;
    private Vec2 _leftDefenderOrigin;
    private Vec2 _middleDefenderOrigin;
    private Vec2 _rightDefenderOrigin;
    private Vec2 _leftAttackerOrigin;
    private Vec2 _middleAttackerOrigin;
    private Vec2 _rightAttackerOrigin;
    private Vec2 _leftToMidDir;
    private Vec2 _midToLeftDir;
    private Vec2 _midToRightDir;
    private Vec2 _rightToMidDir;
    private readonly Team _attackerTeam;
    internal Vec2 DefenderLeftToDefenderMidDir;
    internal Vec2 DefenderMidToDefenderRightDir;
    private readonly QueryData<int> _leftRegionMemberCount;
    private readonly QueryData<int> _leftCloseAttackerCount;
    private readonly QueryData<int> _middleRegionMemberCount;
    private readonly QueryData<int> _middleCloseAttackerCount;
    private readonly QueryData<int> _rightRegionMemberCount;
    private readonly QueryData<int> _rightCloseAttackerCount;
    private readonly QueryData<int> _insideAttackerCount;
    private readonly QueryData<int> _leftDefenderCount;
    private readonly QueryData<int> _middleDefenderCount;
    private readonly QueryData<int> _rightDefenderCount;

    internal Vec2 LeftDefenderOrigin => this._leftDefenderOrigin;

    internal Vec2 MidDefenderOrigin => this._middleDefenderOrigin;

    internal Vec2 RightDefenderOrigin => this._rightDefenderOrigin;

    internal Vec2 LeftAttackerOrigin => this._leftAttackerOrigin;

    internal Vec2 MiddleAttackerOrigin => this._middleAttackerOrigin;

    internal Vec2 RightAttackerOrigin => this._rightAttackerOrigin;

    internal Vec2 LeftToMidDir => this._leftToMidDir;

    internal Vec2 MidToLeftDir => this._midToLeftDir;

    internal Vec2 MidToRightDir => this._midToRightDir;

    internal Vec2 RightToMidDir => this._rightToMidDir;

    public int LeftRegionMemberCount => this._leftRegionMemberCount.Value;

    public int LeftCloseAttackerCount => this._leftCloseAttackerCount.Value;

    public int MiddleRegionMemberCount => this._middleRegionMemberCount.Value;

    public int MiddleCloseAttackerCount => this._middleCloseAttackerCount.Value;

    public int RightRegionMemberCount => this._rightRegionMemberCount.Value;

    public int RightCloseAttackerCount => this._rightCloseAttackerCount.Value;

    public int InsideAttackerCount => this._insideAttackerCount.Value;

    public int LeftDefenderCount => this._leftDefenderCount.Value;

    public int MiddleDefenderCount => this._middleDefenderCount.Value;

    public int RightDefenderCount => this._rightDefenderCount.Value;

    internal SiegeQuerySystem(Team team, IEnumerable<SiegeLane> lanes)
    {
      Mission mission = Mission.Current;
      this._attackerTeam = mission.AttackerTeam;
      Team defenderTeam = mission.DefenderTeam;
      SiegeLane siegeLane1 = lanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (l => l.LaneSide == FormationAI.BehaviorSide.Left));
      WorldPosition worldPosition = siegeLane1.DefenderOrigin;
      this._leftDefenderOrigin = worldPosition.AsVec2;
      worldPosition = siegeLane1.AttackerOrigin;
      this._leftAttackerOrigin = worldPosition.AsVec2;
      SiegeLane siegeLane2 = lanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (l => l.LaneSide == FormationAI.BehaviorSide.Middle));
      worldPosition = siegeLane2.DefenderOrigin;
      this._middleDefenderOrigin = worldPosition.AsVec2;
      worldPosition = siegeLane2.AttackerOrigin;
      this._middleAttackerOrigin = worldPosition.AsVec2;
      SiegeLane siegeLane3 = lanes.FirstOrDefault<SiegeLane>((Func<SiegeLane, bool>) (l => l.LaneSide == FormationAI.BehaviorSide.Right));
      worldPosition = siegeLane3.DefenderOrigin;
      this._rightDefenderOrigin = worldPosition.AsVec2;
      worldPosition = siegeLane3.AttackerOrigin;
      this._rightAttackerOrigin = worldPosition.AsVec2;
      this._leftToMidDir = (this._middleAttackerOrigin - this._leftDefenderOrigin).Normalized();
      this._midToLeftDir = (this._leftAttackerOrigin - this._middleDefenderOrigin).Normalized();
      this._midToRightDir = (this._rightAttackerOrigin - this._middleDefenderOrigin).Normalized();
      this._rightToMidDir = (this._middleAttackerOrigin - this._rightDefenderOrigin).Normalized();
      this._leftRegionMemberCount = new QueryData<int>((Func<int>) (() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.Left)), 5f);
      this._leftCloseAttackerCount = new QueryData<int>((Func<int>) (() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.LeftClose)), 5f);
      this._middleRegionMemberCount = new QueryData<int>((Func<int>) (() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.Middle)), 5f);
      this._middleCloseAttackerCount = new QueryData<int>((Func<int>) (() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.MiddleClose)), 5f);
      this._rightRegionMemberCount = new QueryData<int>((Func<int>) (() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.Right)), 5f);
      this._rightCloseAttackerCount = new QueryData<int>((Func<int>) (() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.RightClose)), 5f);
      this._insideAttackerCount = new QueryData<int>((Func<int>) (() => this.LocateAttackers(SiegeQuerySystem.RegionEnum.Inside)), 5f);
      this._leftDefenderCount = new QueryData<int>((Func<int>) (() => mission.GetNearbyAllyAgents(this._leftDefenderOrigin, 10f, defenderTeam).Count<Agent>()), 5f);
      this._middleDefenderCount = new QueryData<int>((Func<int>) (() => mission.GetNearbyAllyAgents(this._middleDefenderOrigin, 10f, defenderTeam).Count<Agent>()), 5f);
      this._rightDefenderCount = new QueryData<int>((Func<int>) (() => mission.GetNearbyAllyAgents(this._rightDefenderOrigin, 10f, defenderTeam).Count<Agent>()), 5f);
      this.DefenderLeftToDefenderMidDir = (this._middleDefenderOrigin - this._leftDefenderOrigin).Normalized();
      this.DefenderMidToDefenderRightDir = (this._rightDefenderOrigin - this._middleDefenderOrigin).Normalized();
      this.InitializeTelemetryScopeNames();
    }

    private int LocateAttackers(SiegeQuerySystem.RegionEnum region)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      int num6 = 0;
      int num7 = 0;
      foreach (Agent activeAgent in this._attackerTeam.ActiveAgents)
      {
        Vec3 position = activeAgent.Position;
        Vec2 vec2 = position.AsVec2 - this._leftDefenderOrigin;
        if ((double) vec2.Normalize() < 10.0)
        {
          ++num2;
          ++num1;
        }
        else
        {
          if ((double) vec2.DotProduct(this._leftToMidDir) >= 0.0 && (double) vec2.DotProduct(this._leftToMidDir.RightVec()) >= 0.0)
          {
            ++num1;
          }
          else
          {
            position = activeAgent.Position;
            vec2 = position.AsVec2 - this._middleDefenderOrigin;
            if ((double) vec2.DotProduct(this._midToLeftDir) >= 0.0 && (double) vec2.DotProduct(this._midToLeftDir.RightVec()) >= 0.0)
              ++num1;
          }
          position = activeAgent.Position;
          vec2 = position.AsVec2 - this._middleDefenderOrigin;
          if ((double) vec2.Normalize() < 10.0)
          {
            ++num4;
            ++num3;
          }
          else
          {
            if ((double) vec2.DotProduct(this._leftToMidDir) >= 0.0 && (double) vec2.DotProduct(this._leftToMidDir.LeftVec()) >= 0.0)
            {
              ++num3;
            }
            else
            {
              position = activeAgent.Position;
              vec2 = position.AsVec2 - this._rightDefenderOrigin;
              if ((double) vec2.DotProduct(this._rightToMidDir) >= 0.0 && (double) vec2.DotProduct(this._rightToMidDir.RightVec()) >= 0.0)
                ++num3;
            }
            position = activeAgent.Position;
            vec2 = position.AsVec2 - this._rightDefenderOrigin;
            if ((double) vec2.Normalize() < 10.0)
            {
              ++num6;
              ++num5;
            }
            else
            {
              if ((double) vec2.DotProduct(this._midToRightDir) >= 0.0 && (double) vec2.DotProduct(this._midToRightDir.LeftVec()) >= 0.0)
              {
                ++num5;
              }
              else
              {
                position = activeAgent.Position;
                vec2 = position.AsVec2 - this._rightDefenderOrigin;
                if ((double) vec2.DotProduct(this._rightToMidDir) >= 0.0 && (double) vec2.DotProduct(this._rightToMidDir.LeftVec()) >= 0.0)
                  ++num5;
              }
              if (activeAgent.GetCurrentNavigationFaceId() % 10 == 1)
                ++num7;
            }
          }
        }
      }
      float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
      this._leftRegionMemberCount.SetValue(num1, time);
      this._leftCloseAttackerCount.SetValue(num2, time);
      this._middleRegionMemberCount.SetValue(num3, time);
      this._middleCloseAttackerCount.SetValue(num4, time);
      this._rightRegionMemberCount.SetValue(num5, time);
      this._rightCloseAttackerCount.SetValue(num6, time);
      this._insideAttackerCount.SetValue(num7, time);
      switch (region)
      {
        case SiegeQuerySystem.RegionEnum.Left:
          return num1;
        case SiegeQuerySystem.RegionEnum.LeftClose:
          return num2;
        case SiegeQuerySystem.RegionEnum.Middle:
          return num3;
        case SiegeQuerySystem.RegionEnum.MiddleClose:
          return num4;
        case SiegeQuerySystem.RegionEnum.Right:
          return num5;
        case SiegeQuerySystem.RegionEnum.RightClose:
          return num6;
        case SiegeQuerySystem.RegionEnum.Inside:
          return num7;
        default:
          return 0;
      }
    }

    internal void Expire()
    {
      foreach (FieldInfo field in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
      {
        if (field.GetValue((object) this) is IQueryData queryData1)
        {
          queryData1.Expire();
          field.SetValue((object) this, (object) queryData1);
        }
      }
    }

    private void InitializeTelemetryScopeNames()
    {
    }

    public int DeterminePositionAssociatedSide(Vec3 position)
    {
      float num1 = position.AsVec2.DistanceSquared(this.LeftDefenderOrigin);
      float num2 = position.AsVec2.DistanceSquared(this.MidDefenderOrigin);
      float num3 = position.AsVec2.DistanceSquared(this.RightDefenderOrigin);
      FormationAI.BehaviorSide behaviorSide1 = (double) num1 >= (double) num2 || (double) num1 >= (double) num3 ? ((double) num3 >= (double) num2 ? FormationAI.BehaviorSide.Middle : FormationAI.BehaviorSide.Right) : FormationAI.BehaviorSide.Left;
      FormationAI.BehaviorSide behaviorSide2 = FormationAI.BehaviorSide.BehaviorSideNotSet;
      switch (behaviorSide1)
      {
        case FormationAI.BehaviorSide.Left:
          if ((double) (position.AsVec2 - this.LeftDefenderOrigin).Normalized().DotProduct(this.DefenderLeftToDefenderMidDir) > 0.0)
          {
            behaviorSide2 = FormationAI.BehaviorSide.Middle;
            break;
          }
          break;
        case FormationAI.BehaviorSide.Middle:
          behaviorSide2 = (double) (position.AsVec2 - this.MidDefenderOrigin).Normalized().DotProduct(this.DefenderMidToDefenderRightDir) <= 0.0 ? FormationAI.BehaviorSide.Left : FormationAI.BehaviorSide.Right;
          break;
        case FormationAI.BehaviorSide.Right:
          if ((double) (position.AsVec2 - this.RightDefenderOrigin).Normalized().DotProduct(this.DefenderMidToDefenderRightDir) < 0.0)
          {
            behaviorSide2 = FormationAI.BehaviorSide.Middle;
            break;
          }
          break;
      }
      int num4 = 1 << (int) (behaviorSide1 & (FormationAI.BehaviorSide) 31);
      if (behaviorSide2 != FormationAI.BehaviorSide.BehaviorSideNotSet)
        num4 |= 1 << (int) (behaviorSide2 & (FormationAI.BehaviorSide) 31);
      return num4;
    }

    public static bool AreSidesRelated(FormationAI.BehaviorSide side, int connectedSides) => (uint) (1 << (int) (side & (FormationAI.BehaviorSide) 31) & connectedSides) > 0U;

    public static int SideDistance(int connectedSides, int side)
    {
      for (; connectedSides != 0 && side != 0; side >>= 1)
        connectedSides >>= 1;
      int num1 = connectedSides != 0 ? connectedSides : side;
      int num2 = 0;
      for (; num1 > 0; num1 >>= 1)
      {
        ++num2;
        if ((num1 & 1) == 1)
          break;
      }
      return num2;
    }

    private enum RegionEnum
    {
      Left,
      LeftClose,
      Middle,
      MiddleClose,
      Right,
      RightClose,
      Inside,
    }
  }
}
