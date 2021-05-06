// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentBattleAILogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class AgentBattleAILogic : MissionLogic
  {
    private void AddComponents(Agent agent)
    {
      agent.AddComponent((AgentComponent) new AgentAIStateFlagComponent(agent));
      agent.AddComponent((AgentComponent) new AIBehaviorComponent(agent));
      if (agent.IsHuman)
      {
        agent.AddComponent((AgentComponent) new UseObjectAgentComponent(agent));
        agent.AddComponent((AgentComponent) new ItemPickupAgentComponent(agent));
      }
      agent.AddComponent((AgentComponent) new FormationCohesionComponent(agent));
      agent.AddComponent((AgentComponent) new FormationMovementComponent(agent));
      agent.AddComponent((AgentComponent) new FormationOrderComponent(agent));
    }

    private void RemoveComponents(Agent agent)
    {
      agent.RemoveComponent((AgentComponent) agent.GetComponent<AgentAIStateFlagComponent>());
      agent.RemoveComponent((AgentComponent) agent.GetComponent<AIBehaviorComponent>());
      agent.RemoveComponent((AgentComponent) agent.GetComponent<ItemPickupAgentComponent>());
      agent.RemoveComponent((AgentComponent) agent.GetComponent<UseObjectAgentComponent>());
      agent.RemoveComponent((AgentComponent) agent.GetComponent<FormationCohesionComponent>());
      agent.RemoveComponent((AgentComponent) agent.GetComponent<FormationMovementComponent>());
      agent.RemoveComponent((AgentComponent) agent.GetComponent<FormationOrderComponent>());
    }

    public override void OnAgentCreated(Agent agent)
    {
      base.OnAgentCreated(agent);
      if (!agent.IsAIControlled)
        return;
      this.AddComponents(agent);
    }

    protected internal override void OnAgentControllerChanged(Agent agent)
    {
      base.OnAgentControllerChanged(agent);
      if (agent.Controller == Agent.ControllerType.AI)
      {
        this.AddComponents(agent);
      }
      else
      {
        if (agent.Controller != Agent.ControllerType.Player)
          return;
        this.RemoveComponents(agent);
      }
    }
  }
}
