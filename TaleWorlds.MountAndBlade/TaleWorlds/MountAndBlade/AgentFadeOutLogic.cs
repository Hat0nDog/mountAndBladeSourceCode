// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentFadeOutLogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class AgentFadeOutLogic : MissionLogic
  {
    public override void OnAgentCreated(Agent agent)
    {
      base.OnAgentCreated(agent);
      if (!agent.IsAIControlled && !agent.IsMount)
        return;
      agent.AddComponent((AgentComponent) new FadeOutAgentComponent(agent));
    }

    protected internal override void OnAgentControllerChanged(Agent agent)
    {
      base.OnAgentControllerChanged(agent);
      if (agent.Controller == Agent.ControllerType.AI)
      {
        agent.AddComponent((AgentComponent) new FadeOutAgentComponent(agent));
      }
      else
      {
        if (agent.Controller != Agent.ControllerType.Player)
          return;
        agent.RemoveComponent((AgentComponent) agent.GetComponent<FadeOutAgentComponent>());
      }
    }
  }
}
