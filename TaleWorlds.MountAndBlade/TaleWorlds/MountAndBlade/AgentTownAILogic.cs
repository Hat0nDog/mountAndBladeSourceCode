// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentTownAILogic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class AgentTownAILogic : MissionLogic
  {
    public override void OnAgentCreated(Agent agent)
    {
      base.OnAgentCreated(agent);
      if (!agent.IsAIControlled)
        return;
      agent.AddComponent((AgentComponent) new UseObjectAgentComponent(agent));
      agent.AddComponent((AgentComponent) new AgentAIStateFlagComponent(agent));
      agent.AddComponent((AgentComponent) new AIBehaviorComponent(agent));
    }
  }
}
