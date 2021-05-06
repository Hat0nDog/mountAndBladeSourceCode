// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.FormationOrderComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class FormationOrderComponent : AgentComponent
  {
    public FormationOrderComponent(Agent agent)
      : base(agent)
    {
    }

    public void Update()
    {
      if (this.Agent.Formation == null)
        return;
      this.Agent.SetFiringOrder((int) this.Agent.Formation.FiringOrder.GetNativeEnum());
      this.Agent.EnforceShieldUsage = ArrangementOrder.GetShieldDirectionOfUnit(this.Agent.Formation, this.Agent, this.Agent.Formation.ArrangementOrder.OrderEnum);
    }

    public void EnforceShieldUsage(Agent.UsageDirection shieldDirection) => this.Agent.EnforceShieldUsage = shieldDirection;
  }
}
