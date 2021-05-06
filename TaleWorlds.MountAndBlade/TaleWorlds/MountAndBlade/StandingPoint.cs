// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.StandingPoint
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class StandingPoint : UsableMissionObject
  {
    public string WaypointTag = "aiwaypoint";
    public bool AutoSheathWeapons;
    public readonly bool TranslateUser;
    internal bool HasRecentlyBeenRechecked;
    private Dictionary<Agent, StandingPoint.AgentDistanceCache> _cachedAgentDistances;
    protected BattleSideEnum StandingPointSide = BattleSideEnum.None;

    public virtual Agent.AIScriptedFrameFlags DisableScriptedFrameFlags => Agent.AIScriptedFrameFlags.None;

    protected List<GameEntityWithWorldPosition> WaypointEntities { get; private set; }

    internal void ClearWaypointEntities()
    {
      this.WaypointTag = "Not used";
      this.WaypointEntities?.Clear();
    }

    public override bool DisableCombatActionsOnUse => false;

    [EditableScriptComponentVariable(false)]
    internal Agent FavoredUser { get; set; }

    public virtual bool PlayerStopsUsingWhenInteractsWithOther => true;

    public StandingPoint()
      : base()
    {
      this.AutoSheathWeapons = true;
      this.TranslateUser = true;
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      List<GameEntity> gameEntityList = this.GameEntity.CollectChildrenEntitiesWithTag(this.WaypointTag);
      this.WaypointEntities = new List<GameEntityWithWorldPosition>(gameEntityList.Count);
      foreach (GameEntity gameEntity in gameEntityList)
        this.WaypointEntities.Add(new GameEntityWithWorldPosition(gameEntity));
      this._cachedAgentDistances = new Dictionary<Agent, StandingPoint.AgentDistanceCache>();
      bool flag1 = this.GameEntity.HasTag("attacker");
      bool flag2 = this.GameEntity.HasTag("defender");
      if (flag1 && !flag2)
        this.StandingPointSide = BattleSideEnum.Attacker;
      else if (!flag1 & flag2)
        this.StandingPointSide = BattleSideEnum.Defender;
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    internal void OnParentMachinePhysicsStateChanged() => this.GameEntityWithWorldPosition.InvalidateWorldPosition();

    public override bool IsDisabledForAgent(Agent agent)
    {
      if (base.IsDisabledForAgent(agent))
        return true;
      return this.StandingPointSide != BattleSideEnum.None && agent.IsAIControlled && agent.Team != null && agent.Team.Side != this.StandingPointSide;
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => !GameNetwork.IsClientOrReplay && (this.HasAIMovingTo || this.HasUser) ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (GameNetwork.IsClientOrReplay)
        return;
      if (this.HasAIMovingTo)
      {
        foreach (KeyValuePair<Agent, UsableMissionObject.MoveInfo> movingAgent in this.MovingAgents)
          this.UpdateAgent(movingAgent.Key, movingAgent.Value);
      }
      if (!this.HasUser)
        return;
      if (this.DoesActionTypeStopUsingGameObject(MBAnimation.GetActionType(this.UserAgent.GetCurrentAction(0))))
      {
        this.UserAgent.StopUsingGameObject(false);
      }
      else
      {
        if (!this.AutoSheathWeapons)
          return;
        if (this.UserAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand) != EquipmentIndex.None)
          this.UserAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.Instant);
        if (this.UserAgent.GetWieldedItemIndex(Agent.HandIndex.OffHand) == EquipmentIndex.None)
          return;
        this.UserAgent.TryToSheathWeaponInHand(Agent.HandIndex.OffHand, Agent.WeaponWieldActionType.Instant);
      }
    }

    protected virtual bool DoesActionTypeStopUsingGameObject(Agent.ActionCodeType actionType) => actionType == Agent.ActionCodeType.Jump || actionType == Agent.ActionCodeType.Kick || actionType == Agent.ActionCodeType.WeaponBash;

    public override void OnUse(Agent userAgent)
    {
      base.OnUse(userAgent);
      if (this.LockUserFrames)
      {
        WorldFrame userFrameForAgent = this.GetUserFrameForAgent(userAgent);
        userAgent.SetTargetPositionAndDirection(userFrameForAgent.Origin.AsVec2, userFrameForAgent.Rotation.f);
      }
      else
      {
        if (!this.LockUserPositions)
          return;
        userAgent.SetTargetPosition(this.GetUserFrameForAgent(userAgent).Origin.AsVec2);
      }
    }

    public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
    {
      base.OnUseStopped(userAgent, isSuccessful, preferenceIndex);
      if (!this.LockUserFrames && !this.LockUserPositions)
        return;
      userAgent.ClearTargetFrame();
    }

    private void UpdateAgent(Agent movingAgent, UsableMissionObject.MoveInfo moveInfo = null)
    {
      if (moveInfo == null)
        moveInfo = this.MovingAgents[movingAgent];
      Vec3 position = movingAgent.Position;
      if ((double) position.DistanceSquared(this.GameEntity.GlobalPosition) < 1.0 && (double) position.DistanceSquared(this.GetUserFrameForAgent(movingAgent).Origin.GetGroundVec3()) < 0.100000001490116)
        moveInfo.CurrentWaypoint = this.WaypointEntities.Count;
      for (int index = this.WaypointEntities.Count - 1; index > moveInfo.CurrentWaypoint; --index)
      {
        if ((double) position.DistanceSquared(this.WaypointEntities[index].WorldPosition.GetGroundVec3()) < 0.100000001490116)
        {
          moveInfo.CurrentWaypoint = index;
          break;
        }
      }
      while (moveInfo.CurrentWaypoint < this.WaypointEntities.Count && (double) (position - this.WaypointEntities[moveInfo.CurrentWaypoint].WorldPosition.GetGroundVec3()).LengthSquared < 1.0)
        ++moveInfo.CurrentWaypoint;
    }

    private bool HasFinishedWay(Agent userAgent)
    {
      this.UpdateAgent(userAgent);
      return this.MovingAgents[userAgent].CurrentWaypoint >= this.WaypointEntities.Count - 1;
    }

    public override bool IsUsableByAgent(Agent userAgent) => !userAgent.IsAIControlled || this.HasFinishedWay(userAgent);

    public override WorldFrame GetUserFrameForAgent(Agent agent)
    {
      if (!this.HasUser && agent.IsAIControlled && (this.MovingAgents.ContainsKey(agent) && this.MovingAgents[agent].CurrentWaypoint < this.WaypointEntities.Count) && !Mission.Current.IsTeleportingAgents)
        return this.WaypointEntities[this.MovingAgents[agent].CurrentWaypoint].WorldFrame;
      if (!Mission.Current.IsTeleportingAgents && !this.TranslateUser)
        return agent.GetWorldFrame();
      if (!Mission.Current.IsTeleportingAgents && (this.LockUserFrames || this.LockUserPositions))
        return base.GetUserFrameForAgent(agent);
      WorldFrame userFrameForAgent = base.GetUserFrameForAgent(agent);
      MatrixFrame lookFrame = agent.LookFrame;
      Vec2 vec2_1 = (lookFrame.origin.AsVec2 - userFrameForAgent.Origin.AsVec2).Normalized();
      Vec2 vec2_2 = userFrameForAgent.Origin.AsVec2 + agent.GetInteractionDistanceToUsable((IUsable) this) * 0.5f * vec2_1;
      Mat3 rotation = lookFrame.rotation;
      userFrameForAgent.Origin.SetVec2(vec2_2);
      userFrameForAgent.Rotation = rotation;
      return userFrameForAgent;
    }

    public override bool HasUserPositionsChanged(Agent agent)
    {
      UsableMissionObject.MoveInfo moveInfo;
      return this.MovingAgents.TryGetValue(agent, out moveInfo) && moveInfo.HasPositionsChanged || base.HasUserPositionsChanged(agent);
    }

    public virtual bool HasAlternative() => false;

    public virtual float GetUsageScoreForAgent(Agent agent)
    {
      WorldPosition origin = this.GetUserFrameForAgent(agent).Origin;
      WorldPosition worldPosition = agent.GetWorldPosition();
      float pathDistance = this.GetPathDistance(agent, ref origin, ref worldPosition);
      float num = (double) pathDistance < 0.0 ? float.MinValue : -pathDistance;
      if (agent == this.FavoredUser)
        num *= 0.5f;
      return num;
    }

    public virtual float GetUsageScoreForAgent(AgentValuePair<float> agentPair)
    {
      float num1 = agentPair.Value;
      float num2 = (double) num1 < 0.0 ? float.MinValue : -num1;
      if (agentPair.Agent == this.FavoredUser)
        num2 *= 0.5f;
      return num2;
    }

    private float GetPathDistance(
      Agent agent,
      ref WorldPosition userPosition,
      ref WorldPosition agentPosition)
    {
      StandingPoint.AgentDistanceCache agentDistanceCache1;
      float pathDistance;
      if (this._cachedAgentDistances.TryGetValue(agent, out agentDistanceCache1))
      {
        if ((double) agentDistanceCache1.AgentPosition.DistanceSquared(agentPosition.AsVec2) < 1.0 && (double) agentDistanceCache1.StandingPointPosition.DistanceSquared(userPosition.AsVec2) < 1.0)
        {
          pathDistance = agentDistanceCache1.PathDistance;
        }
        else
        {
          if (!Mission.Current.Scene.GetPathDistanceBetweenPositions(ref userPosition, ref agentPosition, agent.Monster.BodyCapsuleRadius, out pathDistance))
            pathDistance = float.MaxValue;
          StandingPoint.AgentDistanceCache agentDistanceCache2 = new StandingPoint.AgentDistanceCache()
          {
            AgentPosition = agentPosition.AsVec2,
            StandingPointPosition = userPosition.AsVec2,
            PathDistance = pathDistance
          };
          this._cachedAgentDistances[agent] = agentDistanceCache2;
        }
      }
      else
      {
        if (!Mission.Current.Scene.GetPathDistanceBetweenPositions(ref userPosition, ref agentPosition, agent.Monster.BodyCapsuleRadius, out pathDistance))
          pathDistance = float.MaxValue;
        StandingPoint.AgentDistanceCache agentDistanceCache2 = new StandingPoint.AgentDistanceCache()
        {
          AgentPosition = agentPosition.AsVec2,
          StandingPointPosition = userPosition.AsVec2,
          PathDistance = pathDistance
        };
        this._cachedAgentDistances[agent] = agentDistanceCache2;
      }
      return pathDistance;
    }

    public override void OnEndMission()
    {
      base.OnEndMission();
      this.FavoredUser = (Agent) null;
    }

    internal virtual bool IsUsableBySide(BattleSideEnum side)
    {
      if (this.IsDeactivated || !this.IsInstantUse && this.HasUser)
        return false;
      return this.StandingPointSide == BattleSideEnum.None || side == this.StandingPointSide;
    }

    public override string GetDescriptionText(GameEntity gameEntity = null) => string.Empty;

    private struct AgentDistanceCache
    {
      public Vec2 AgentPosition;
      public Vec2 StandingPointPosition;
      public float PathDistance;
    }
  }
}
