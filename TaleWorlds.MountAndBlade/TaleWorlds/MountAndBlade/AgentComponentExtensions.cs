// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentComponentExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public static class AgentComponentExtensions
  {
    public static void SetGuardState(this Agent agent, Agent guardedAgent, bool isGuarding) => agent.GetComponent<AgentAIStateFlagComponent>()?.SetGuardState(guardedAgent, isGuarding);

    public static bool IsAIPaused(this Agent agent)
    {
      AgentAIStateFlagComponent component = agent.GetComponent<AgentAIStateFlagComponent>();
      return component != null && component.IsPaused;
    }

    public static void SetIsAIPaused(this Agent agent, bool isPaused)
    {
      AgentAIStateFlagComponent component = agent.GetComponent<AgentAIStateFlagComponent>();
      if (component == null)
        return;
      component.IsPaused = isPaused;
    }

    public static void SetWatchState(this Agent agent, AgentAIStateFlagComponent.WatchState state)
    {
      AgentAIStateFlagComponent component = agent.GetComponent<AgentAIStateFlagComponent>();
      if (component == null)
        return;
      component.CurrentWatchState = state;
    }

    public static bool IsAlarmed(this Agent agent)
    {
      AgentAIStateFlagComponent component = agent.GetComponent<AgentAIStateFlagComponent>();
      return component != null && component.CurrentWatchState == AgentAIStateFlagComponent.WatchState.Alarmed;
    }

    public static bool IsCautious(this Agent agent)
    {
      AgentAIStateFlagComponent component = agent.GetComponent<AgentAIStateFlagComponent>();
      return component != null && component.CurrentWatchState == AgentAIStateFlagComponent.WatchState.Cautious;
    }

    public static bool IsPatroling(this Agent agent)
    {
      AgentAIStateFlagComponent component = agent.GetComponent<AgentAIStateFlagComponent>();
      return component != null && component.CurrentWatchState == AgentAIStateFlagComponent.WatchState.Patroling;
    }

    public static void SetAIBehaviorValues(
      this Agent agent,
      AISimpleBehaviorKind behavior,
      float y1,
      float x2,
      float y2,
      float x3,
      float y3)
    {
      agent.GetComponent<AIBehaviorComponent>()?.Set(behavior, y1, x2, y2, x3, y3);
    }

    internal static void FormationCohesionMarkAsDirty(this Agent agent) => agent.GetComponent<FormationCohesionComponent>()?.MarkAsDirty();

    public static bool GetFormationFrame(
      this Agent agent,
      out WorldPosition formationPosition,
      out Vec2 formationDirection,
      out float speedLimit,
      out bool isSettingDestinationSpeed,
      out bool limitIsMultiplier,
      bool finalDestination = false)
    {
      FormationMovementComponent component = agent.GetComponent<FormationMovementComponent>();
      if (component != null)
        return component.GetFormationFrame(out formationPosition, out formationDirection, out speedLimit, out isSettingDestinationSpeed, out limitIsMultiplier, finalDestination);
      formationPosition = WorldPosition.Invalid;
      formationDirection = Vec2.Invalid;
      speedLimit = -1f;
      isSettingDestinationSpeed = false;
      limitIsMultiplier = false;
      return false;
    }

    public static void UpdateFormationMovement(this Agent agent) => agent.GetComponent<FormationMovementComponent>()?.Update();

    public static Agent GetLastFollowedUnitForMovement(this Agent agent) => agent.GetComponent<FormationMovementComponent>()?.LastFollowedUnitForMovement;

    public static Agent GetFollowedUnit(this Agent agent) => agent.GetComponent<FormationMovementComponent>()?.FollowedAgent;

    public static void SetFollowedUnit(this Agent agent, Agent followedUnit)
    {
      FormationMovementComponent component = agent.GetComponent<FormationMovementComponent>();
      if (component == null)
        return;
      component.FollowedAgent = followedUnit;
    }

    public static void UpdateFormationOrders(this Agent agent) => agent.GetComponent<FormationOrderComponent>()?.Update();

    public static void EnforceShieldUsage(this Agent agent, Agent.UsageDirection shieldDirection) => agent.GetComponent<FormationOrderComponent>()?.EnforceShieldUsage(shieldDirection);

    public static float GetMorale(this Agent agent)
    {
      MoraleAgentComponent component = agent.GetComponent<MoraleAgentComponent>();
      return component != null ? component.Morale : -1f;
    }

    public static void SetMorale(this Agent agent, float morale)
    {
      MoraleAgentComponent component = agent.GetComponent<MoraleAgentComponent>();
      if (component == null)
        return;
      component.Morale = morale;
    }

    public static void ChangeMorale(this Agent agent, float delta)
    {
      MoraleAgentComponent component = agent.GetComponent<MoraleAgentComponent>();
      if (component == null)
        return;
      component.Morale += delta;
    }

    public static bool IsRetreating(this Agent agent, bool isComponentAssured = true)
    {
      MoraleAgentComponent component = agent.GetComponent<MoraleAgentComponent>();
      return component != null && component.IsRetreating;
    }

    public static void Retreat(this Agent agent) => agent.GetComponent<MoraleAgentComponent>()?.Retreat();

    public static void StopRetreatingMoraleComponent(this Agent agent) => agent.GetComponent<MoraleAgentComponent>()?.StopRetreating();

    public static void AIMoveToGameObjectEnable(
      this Agent agent,
      UsableMissionObject usedObject,
      Agent.AIScriptedFrameFlags scriptedFrameFlags = Agent.AIScriptedFrameFlags.NoAttack)
    {
      agent.GetComponent<UseObjectAgentComponent>()?.MoveToUsableGameObject(usedObject, scriptedFrameFlags);
    }

    public static void AIMoveToGameObjectDisable(this Agent agent) => agent.GetComponent<UseObjectAgentComponent>()?.MoveToClear();

    public static bool AIMoveToGameObjectIsEnabled(this Agent agent)
    {
      UseObjectAgentComponent component = agent.GetComponent<UseObjectAgentComponent>();
      return component != null && component.IsMovingTo;
    }

    public static bool AIUseGameObjectIsEnabled(this Agent agent)
    {
      UseObjectAgentComponent component = agent.GetComponent<UseObjectAgentComponent>();
      return component != null && component.IsUsing;
    }

    public static void AIUseGameObjectEnable(this Agent agent, bool isUsing)
    {
      UseObjectAgentComponent component = agent.GetComponent<UseObjectAgentComponent>();
      if (component == null)
        return;
      component.IsUsing = isUsing;
    }

    public static void SyncHealthToClient(this Agent agent) => agent.GetComponent<HealthAgentComponent>()?.SyncHealthToClients();
  }
}
