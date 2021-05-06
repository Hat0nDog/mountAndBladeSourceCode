// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Objects.Siege.AgentPathNavMeshChecker
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Source.Objects.Siege
{
  public class AgentPathNavMeshChecker
  {
    private BattleSideEnum _teamToCollect;
    private AgentPathNavMeshChecker.Direction _directionToCollect;
    private MatrixFrame _pathFrameToCheck;
    private float _radiusToCheck;
    private Mission _mission;
    private int _navMeshId;
    private Timer _tickOccasionallyTimer;
    private List<Agent> _nearbyAgents = new List<Agent>();
    private bool _isBeingUsed;
    private Timer _setBeingUsedToFalseTimer;
    private float _maxDistanceCheck;
    private float _agentMoveTime;

    public AgentPathNavMeshChecker(
      Mission mission,
      MatrixFrame pathFrameToCheck,
      float radiusToCheck,
      int navMeshId,
      BattleSideEnum teamToCollect,
      AgentPathNavMeshChecker.Direction directionToCollect,
      float maxDistanceCheck,
      float agentMoveTime)
    {
      this._mission = mission;
      this._pathFrameToCheck = pathFrameToCheck;
      this._radiusToCheck = radiusToCheck;
      this._navMeshId = navMeshId;
      this._teamToCollect = teamToCollect;
      this._directionToCollect = directionToCollect;
      this._maxDistanceCheck = maxDistanceCheck;
      this._agentMoveTime = agentMoveTime;
    }

    public void Tick(float dt)
    {
      float time = MBCommon.GetTime(MBCommon.TimeType.Mission);
      if (this._tickOccasionallyTimer == null || this._tickOccasionallyTimer.Check(time))
      {
        float dt1 = dt;
        if (this._tickOccasionallyTimer != null)
          dt1 = this._tickOccasionallyTimer.ElapsedTime();
        this._tickOccasionallyTimer = new Timer(time, (float) (0.100000001490116 + (double) MBRandom.RandomFloat * 0.100000001490116));
        this.TickOccasionally(dt1);
      }
      bool flag = false;
      foreach (Agent nearbyAgent in this._nearbyAgents)
      {
        Vec3 position = nearbyAgent.Position;
        if ((this._teamToCollect == BattleSideEnum.None || nearbyAgent.Team != null && nearbyAgent.Team.Side == this._teamToCollect) && nearbyAgent.IsAIControlled)
        {
          if (nearbyAgent.GetCurrentNavigationFaceId() == this._navMeshId)
          {
            flag = true;
            break;
          }
          if (this._isBeingUsed && (double) position.DistanceSquared(this._pathFrameToCheck.origin) < (double) this._radiusToCheck * (double) this._radiusToCheck)
          {
            flag = true;
            break;
          }
          if ((double) nearbyAgent.MovementVelocity.LengthSquared > 0.00999999977648258)
          {
            Vec2 direction = this._directionToCollect != AgentPathNavMeshChecker.Direction.ForwardOnly ? (this._directionToCollect != AgentPathNavMeshChecker.Direction.BackwardOnly ? Vec2.Zero : -this._pathFrameToCheck.rotation.f.AsVec2) : this._pathFrameToCheck.rotation.f.AsVec2;
            if (nearbyAgent.HasPathThroughNavigationFaceIdFromDirection(this._navMeshId, direction))
            {
              float num = nearbyAgent.GetPathDistanceToPoint(ref this._pathFrameToCheck.origin);
              if ((double) num >= 100000.0)
                num = nearbyAgent.Position.Distance(this._pathFrameToCheck.origin);
              float forwardUnlimitedSpeed = nearbyAgent.MaximumForwardUnlimitedSpeed;
              if ((double) num < (double) this._radiusToCheck * 2.0 || (double) num / (double) forwardUnlimitedSpeed < (double) this._agentMoveTime)
                flag = true;
            }
          }
        }
      }
      if (flag)
      {
        this._isBeingUsed = true;
        this._setBeingUsedToFalseTimer = (Timer) null;
      }
      else if (this._setBeingUsedToFalseTimer == null)
        this._setBeingUsedToFalseTimer = new Timer(time, 1f);
      if (this._setBeingUsedToFalseTimer == null || !this._setBeingUsedToFalseTimer.Check(time))
        return;
      this._setBeingUsedToFalseTimer = (Timer) null;
      this._isBeingUsed = false;
    }

    public void TickOccasionally(float dt)
    {
      this._nearbyAgents.Clear();
      this._nearbyAgents.AddRange(this._mission.GetNearbyAgents(this._pathFrameToCheck.origin.AsVec2, this._maxDistanceCheck));
    }

    public bool HasAgentsUsingPath() => this._isBeingUsed;

    public enum Direction
    {
      ForwardOnly,
      BackwardOnly,
      BothDirections,
    }
  }
}
