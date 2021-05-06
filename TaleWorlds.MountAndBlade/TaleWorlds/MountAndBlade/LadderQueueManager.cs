// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.LadderQueueManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class LadderQueueManager : MissionObject
  {
    public int ManagedNavigationFaceId;
    private MatrixFrame _managedFrame;
    private Vec3 _managedDirection;
    private BattleSideEnum _managedSide;
    private bool _blockUsage;
    private readonly List<Agent> _userAgents = new List<Agent>();
    private int _maxUserCount;
    private int _queuedAgentCount;
    private readonly List<Agent> _queuedAgents = new List<Agent>();
    private float _arcAngle = 2.356194f;
    private float _queueBeginDistance = 1f;
    private float _queueRowSize = 0.8f;
    private float _agentSpacing = 1f;
    private float _timeSinceLastUpdate;
    private float _updatePeriod;
    private float _usingAgentResetTime;
    private float _costPerRow;
    private float _baseCost;
    private float _zDifferenceToStopUsing = 2f;
    private float _distanceToStopUsing2d = 5f;

    public bool IsDeactivated { get; set; }

    protected internal override void OnInit()
    {
      base.OnInit();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public void Initialize(
      int managedNavigationFaceId,
      MatrixFrame managedFrame,
      Vec3 managedDirection,
      BattleSideEnum managedSide,
      int maxUserCount,
      float arcAngle,
      float queueBeginDistance,
      float queueRowSize,
      float costPerRow,
      float baseCost,
      bool blockUsage,
      float agentSpacing,
      float zDifferenceToStopUsing,
      float distanceToStopUsing2d)
    {
      this.ManagedNavigationFaceId = managedNavigationFaceId;
      this._managedFrame = managedFrame;
      this._managedDirection = managedDirection;
      this._managedSide = managedSide;
      this._maxUserCount = maxUserCount;
      this._arcAngle = arcAngle;
      this._queueBeginDistance = queueBeginDistance;
      this._queueRowSize = queueRowSize;
      this._costPerRow = costPerRow;
      this._baseCost = baseCost;
      this._blockUsage = blockUsage;
      this._agentSpacing = agentSpacing;
      this._zDifferenceToStopUsing = zDifferenceToStopUsing;
      this._distanceToStopUsing2d = distanceToStopUsing2d;
      this.UpdateNavigationFaceCost();
    }

    private MatrixFrame GetManagedGlobalFrame() => this.GameEntity.GetGlobalFrame().TransformToParent(this._managedFrame);

    private Vec3 GetManagedGlobalDirection() => this.GameEntity.GetGlobalFrame().rotation.TransformToParent(this._managedDirection);

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      if (this.IsDeactivated)
      {
        this._userAgents.Clear();
        for (int index = 0; index < this._queuedAgents.Count; ++index)
        {
          if (this._queuedAgents[index] != null && this._queuedAgents[index].IsActive())
            this.RemoveAgentFromQueueAtIndex(index);
        }
        this._queuedAgents.Clear();
      }
      else
      {
        MatrixFrame managedGlobalFrame = this.GetManagedGlobalFrame();
        this._timeSinceLastUpdate += dt;
        if ((double) this._timeSinceLastUpdate < (double) this._updatePeriod)
          return;
        this._usingAgentResetTime -= this._timeSinceLastUpdate;
        this._timeSinceLastUpdate = 0.0f;
        this._updatePeriod = (float) (0.200000002980232 + (double) MBRandom.RandomFloat * 0.100000001490116);
        for (int index = this._userAgents.Count - 1; index >= 0; --index)
        {
          Agent userAgent = this._userAgents[index];
          if (!userAgent.IsActive() || (double) this._usingAgentResetTime < 0.0 && (!userAgent.HasPathThroughNavigationFaceIdFromDirection(this.ManagedNavigationFaceId, this.GetManagedGlobalDirection().AsVec2) && ((double) userAgent.Position.z - (double) managedGlobalFrame.origin.z > (double) this._zDifferenceToStopUsing || (double) (userAgent.Position.AsVec2 - managedGlobalFrame.origin.AsVec2).LengthSquared > (double) this._distanceToStopUsing2d * (double) this._distanceToStopUsing2d) || userAgent.GetCurrentNavigationFaceId() == this.ManagedNavigationFaceId && (double) Vec3.DotProduct(userAgent.Position - managedGlobalFrame.origin, -managedGlobalFrame.rotation.u) > (double) ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRadius)))
            this._userAgents.RemoveAt(index);
        }
        for (int index = this._queuedAgents.Count - 1; index >= 0; --index)
        {
          if (this._queuedAgents[index] != null)
          {
            if (!this.ConditionsAreMet(this._queuedAgents[index], Agent.AIScriptedFrameFlags.GoToPosition))
            {
              this.RemoveAgentFromQueueAtIndex(index);
            }
            else
            {
              float num = MBRandom.RandomFloat * (float) this._maxUserCount;
              if ((double) num > 0.699999988079071)
              {
                int parentIndex1;
                int parentIndex2;
                this.GetParentIndicesForQueueIndex(index, out parentIndex1, out parentIndex2);
                if (parentIndex1 >= 0 && this._queuedAgents[parentIndex1] == null && (double) num > (parentIndex2 >= 0 ? 0.850000023841858 : 0.699999988079071))
                  this.MoveAgentFromQueueIndexToQueueIndex(index, parentIndex1);
                else if (parentIndex2 >= 0 && this._queuedAgents[parentIndex2] == null)
                  this.MoveAgentFromQueueIndexToQueueIndex(index, parentIndex2);
              }
            }
          }
        }
        foreach (Agent agent in Mission.Current.GetAgentsInRange(managedGlobalFrame.origin.AsVec2, 30f))
        {
          if (this.ConditionsAreMet(agent, Agent.AIScriptedFrameFlags.None) && ((double) agent.Position.DistanceSquared(managedGlobalFrame.origin) < 900.0 && !this._queuedAgents.Contains(agent) && agent.HasPathThroughNavigationFaceIdFromDirection(this.ManagedNavigationFaceId, Vec2.Zero)))
            this.AddAgentToQueue(agent);
        }
        if (this._blockUsage || this._userAgents.Count >= this._maxUserCount)
          return;
        float num1 = float.MaxValue;
        int num2 = -1;
        for (int index = 0; index < this._queuedAgents.Count; ++index)
        {
          if (this._queuedAgents[index] != null)
          {
            float lengthSquared = (this._queuedAgents[index].Position - managedGlobalFrame.origin).LengthSquared;
            if ((double) lengthSquared < (double) num1)
            {
              num1 = lengthSquared;
              num2 = index;
            }
          }
        }
        if (num2 < 0)
          return;
        this._userAgents.Add(this._queuedAgents[num2]);
        this._usingAgentResetTime = 2f;
        this.RemoveAgentFromQueueAtIndex(num2);
      }
    }

    private bool ConditionsAreMet(Agent agent, Agent.AIScriptedFrameFlags flags) => agent.IsAIControlled && agent.IsActive() && (agent.Team != null && agent.Team.Side == this._managedSide) && (agent.MovementLockedState == AgentMovementLockedState.None && !agent.IsUsingGameObject && (!agent.AIMoveToGameObjectIsEnabled() && (agent.AIStateFlags & (Agent.AIStateFlag.UseObjectMoving | Agent.AIStateFlag.UseObjectUsing)) == Agent.AIStateFlag.None)) && (!agent.IsDetachedFromFormation && !this._userAgents.Contains(agent) && agent.GetScriptedFlags() == flags) && agent.GetCurrentNavigationFaceId() != this.ManagedNavigationFaceId;

    protected internal override void OnMissionReset()
    {
      base.OnMissionReset();
      this._userAgents.Clear();
      this.IsDeactivated = true;
      this._queuedAgents.Clear();
      this._queuedAgentCount = 0;
      this.UpdateNavigationFaceCost();
    }

    private void GetParentIndicesForQueueIndex(
      int queueIndex,
      out int parentIndex1,
      out int parentIndex2)
    {
      parentIndex1 = -1;
      parentIndex2 = -1;
      Vec2i coordinatesForQueueIndex = this.GetCoordinatesForQueueIndex(queueIndex);
      int num1 = coordinatesForQueueIndex.Y - 1;
      if (num1 < 0)
        return;
      int num2 = Math.Max(this.GetRowSize(num1) - 1, 1);
      int num3 = Math.Max(this.GetRowSize(coordinatesForQueueIndex.Y) - 1, 1);
      float num4 = (float) coordinatesForQueueIndex.X * (float) num2 / (float) num3;
      parentIndex1 = (int) num4;
      float num5 = Math.Abs(num4 - (float) parentIndex1);
      if ((double) num5 > 0.200000002980232)
      {
        if ((double) num5 > 0.800000011920929)
          ++parentIndex1;
        else
          parentIndex2 = parentIndex1 + 1;
      }
      parentIndex1 = this.GetQueueIndexForCoordinates(new Vec2i(parentIndex1, num1));
      if (parentIndex2 < 0)
        return;
      parentIndex2 = this.GetQueueIndexForCoordinates(new Vec2i(parentIndex2, num1));
    }

    private float GetScoreForAddingAgentToQueueIndex(
      Vec3 agentPosition,
      int queueIndex,
      out int scoreOfQueueIndex)
    {
      scoreOfQueueIndex = queueIndex;
      float num = float.MinValue;
      if (this._queuedAgents.Count <= queueIndex || this._queuedAgents[queueIndex] == null)
      {
        int parentIndex1;
        int parentIndex2;
        this.GetParentIndicesForQueueIndex(queueIndex, out parentIndex1, out parentIndex2);
        if (parentIndex1 < 0 || this._queuedAgents.Count > parentIndex1 && this._queuedAgents[parentIndex1] != null || parentIndex2 >= 0 && this._queuedAgents.Count > parentIndex2 && this._queuedAgents[parentIndex2] != null)
        {
          Vec2i coordinatesForQueueIndex = this.GetCoordinatesForQueueIndex(queueIndex);
          num = (float) ((double) coordinatesForQueueIndex.Y * (double) this._queueRowSize * -3.0) - (agentPosition.AsVec2 - this.GetQueuePositionForCoordinates(coordinatesForQueueIndex, -1).AsVec2).Length;
        }
        if (parentIndex1 >= 0 && (this._queuedAgents.Count <= parentIndex1 || this._queuedAgents[parentIndex1] == null))
        {
          int scoreOfQueueIndex1;
          float agentToQueueIndex = this.GetScoreForAddingAgentToQueueIndex(agentPosition, parentIndex1, out scoreOfQueueIndex1);
          if ((double) num < (double) agentToQueueIndex)
          {
            scoreOfQueueIndex = scoreOfQueueIndex1;
            num = agentToQueueIndex;
          }
        }
        if (parentIndex2 >= 0 && (this._queuedAgents.Count <= parentIndex2 || this._queuedAgents[parentIndex2] == null))
        {
          int scoreOfQueueIndex1;
          float agentToQueueIndex = this.GetScoreForAddingAgentToQueueIndex(agentPosition, parentIndex2, out scoreOfQueueIndex1);
          if ((double) num < (double) agentToQueueIndex)
          {
            scoreOfQueueIndex = scoreOfQueueIndex1;
            num = agentToQueueIndex;
          }
        }
      }
      return num;
    }

    private void AddAgentToQueue(Agent agent)
    {
      int y = this.GetCoordinatesForQueueIndex(this._queuedAgents.Count).Y;
      int rowSize = this.GetRowSize(y);
      Vec3 position = agent.Position;
      int num1 = -1;
      float num2 = float.MinValue;
      for (int x = 0; x < rowSize; ++x)
      {
        int scoreOfQueueIndex;
        float agentToQueueIndex = this.GetScoreForAddingAgentToQueueIndex(position, this.GetQueueIndexForCoordinates(new Vec2i(x, y)), out scoreOfQueueIndex);
        if ((double) agentToQueueIndex > (double) num2)
        {
          num2 = agentToQueueIndex;
          num1 = scoreOfQueueIndex;
        }
      }
      while (this._queuedAgents.Count <= num1)
        this._queuedAgents.Add((Agent) null);
      this._queuedAgents[num1] = agent;
      WorldPosition positionForIndex = this.GetQueuePositionForIndex(num1, agent.Index);
      agent.SetScriptedPosition(ref positionForIndex, false);
      ++this._queuedAgentCount;
      if (this._blockUsage)
        return;
      this.UpdateNavigationFaceCost();
    }

    private void RemoveAgentFromQueueAtIndex(int queueIndex)
    {
      --this._queuedAgentCount;
      if (!this._queuedAgents[queueIndex].IsUsingGameObject && !this._queuedAgents[queueIndex].AIMoveToGameObjectIsEnabled())
        this._queuedAgents[queueIndex].DisableScriptedMovement();
      this._queuedAgents[queueIndex] = (Agent) null;
      if (this._blockUsage)
        return;
      this.UpdateNavigationFaceCost();
    }

    private float GetNavigationFaceCost(int rowIndex) => this._baseCost + (float) Math.Max(rowIndex - 1, 0) * this._costPerRow;

    private void UpdateNavigationFaceCost() => Mission.Current.SetNavigationFaceCostWithIdAroundPosition(this.ManagedNavigationFaceId, this.GetManagedGlobalFrame().origin, this.GetNavigationFaceCost(this.GetCoordinatesForQueueIndex(this._queuedAgentCount).Y));

    private void MoveAgentFromQueueIndexToQueueIndex(int fromQueueIndex, int toQueueIndex)
    {
      this._queuedAgents[toQueueIndex] = this._queuedAgents[fromQueueIndex];
      this._queuedAgents[fromQueueIndex] = (Agent) null;
      WorldPosition positionForIndex = this.GetQueuePositionForIndex(toQueueIndex, this._queuedAgents[toQueueIndex].Index);
      this._queuedAgents[toQueueIndex].SetScriptedPosition(ref positionForIndex, false);
    }

    private int GetRowSize(int rowIndex) => 1 + (int) ((double) (this._arcAngle * (this._queueBeginDistance + this._queueRowSize * (float) rowIndex)) / (double) this._agentSpacing);

    private int GetQueueIndexForCoordinates(Vec2i coordinates)
    {
      int x = coordinates.X;
      for (int rowIndex = 0; rowIndex < coordinates.Y; ++rowIndex)
        x += this.GetRowSize(rowIndex);
      return x;
    }

    private Vec2i GetCoordinatesForQueueIndex(int queueIndex)
    {
      Vec2i vec2i = new Vec2i();
      while (true)
      {
        int rowSize = this.GetRowSize(vec2i.Y);
        if (rowSize <= queueIndex)
        {
          queueIndex -= rowSize;
          ++vec2i.Y;
        }
        else
          break;
      }
      vec2i.X = queueIndex;
      return vec2i;
    }

    private WorldPosition GetQueuePositionForCoordinates(
      Vec2i coordinates,
      int randomSeed)
    {
      MatrixFrame managedGlobalFrame = this.GetManagedGlobalFrame();
      WorldPosition worldPosition = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, managedGlobalFrame.origin, false);
      float a = 0.0f;
      int rowSize = this.GetRowSize(coordinates.Y);
      if (rowSize > 1)
        a = this._arcAngle * (float) ((double) coordinates.X / (double) (rowSize - 1) - 0.5);
      managedGlobalFrame.rotation.RotateAboutForward(a);
      managedGlobalFrame.origin += managedGlobalFrame.rotation.u * (this._queueBeginDistance + this._queueRowSize * (float) coordinates.Y);
      if (randomSeed >= 0)
      {
        Random random = new Random(coordinates.X * 100000 + coordinates.Y * 10000000 + randomSeed);
        managedGlobalFrame.rotation.RotateAboutForward((float) ((double) random.NextFloat() * 3.14159274101257 * 2.0));
        managedGlobalFrame.origin += managedGlobalFrame.rotation.u * random.NextFloat() * 0.3f;
      }
      worldPosition.SetVec2(managedGlobalFrame.origin.AsVec2);
      return worldPosition;
    }

    private WorldPosition GetQueuePositionForIndex(int queueIndex, int randomSeed) => this.GetQueuePositionForCoordinates(this.GetCoordinatesForQueueIndex(queueIndex), randomSeed);
  }
}
