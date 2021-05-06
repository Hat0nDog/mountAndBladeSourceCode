// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentAIStateFlagComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class AgentAIStateFlagComponent : AgentComponent
  {
    public AgentAIStateFlagComponent(Agent agent)
      : base(agent)
    {
    }

    public void SetGuardState(Agent guardedAgent, bool isGuarding)
    {
      if (isGuarding)
        this.Agent.AIStateFlags |= Agent.AIStateFlag.Guard;
      else
        this.Agent.AIStateFlags &= ~Agent.AIStateFlag.Guard;
      this.Agent.SetGuardedAgent(guardedAgent);
    }

    public bool IsPaused
    {
      get => this.Agent.AIStateFlags.HasAnyFlag<Agent.AIStateFlag>(Agent.AIStateFlag.Paused);
      set
      {
        if (value)
          this.Agent.AIStateFlags |= Agent.AIStateFlag.Paused;
        else
          this.Agent.AIStateFlags &= ~Agent.AIStateFlag.Paused;
      }
    }

    public AgentAIStateFlagComponent.WatchState CurrentWatchState
    {
      get
      {
        Agent.AIStateFlag aiStateFlags = this.Agent.AIStateFlags;
        if ((aiStateFlags & Agent.AIStateFlag.Alarmed) == Agent.AIStateFlag.Alarmed)
          return AgentAIStateFlagComponent.WatchState.Alarmed;
        return (aiStateFlags & Agent.AIStateFlag.Cautious) == Agent.AIStateFlag.Cautious ? AgentAIStateFlagComponent.WatchState.Cautious : AgentAIStateFlagComponent.WatchState.Patroling;
      }
      set
      {
        Agent.AIStateFlag aiStateFlag = this.Agent.AIStateFlags;
        switch (value)
        {
          case AgentAIStateFlagComponent.WatchState.Patroling:
            aiStateFlag &= ~(Agent.AIStateFlag.Cautious | Agent.AIStateFlag.Alarmed);
            break;
          case AgentAIStateFlagComponent.WatchState.Cautious:
            aiStateFlag = (aiStateFlag | Agent.AIStateFlag.Cautious) & ~Agent.AIStateFlag.Alarmed;
            break;
          case AgentAIStateFlagComponent.WatchState.Alarmed:
            aiStateFlag = (aiStateFlag | Agent.AIStateFlag.Alarmed) & ~Agent.AIStateFlag.Cautious;
            break;
        }
        this.Agent.AIStateFlags = aiStateFlag;
      }
    }

    public enum WatchState
    {
      Patroling,
      Cautious,
      Alarmed,
    }
  }
}
