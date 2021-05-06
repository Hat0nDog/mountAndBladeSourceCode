// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MPPerksAgentComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class MPPerksAgentComponent : AgentComponent
  {
    public MPPerksAgentComponent(Agent agent)
      : base(agent)
    {
      this.Agent.OnAgentHealthChanged += new Agent.OnAgentHealthChangedDelegate(this.OnHealthChanged);
      if (!this.Agent.HasMount)
        return;
      this.Agent.MountAgent.OnAgentHealthChanged += new Agent.OnAgentHealthChangedDelegate(this.OnMountHealthChanged);
    }

    protected internal override void OnMount(Agent mount)
    {
      mount.OnAgentHealthChanged += new Agent.OnAgentHealthChangedDelegate(this.OnMountHealthChanged);
      mount.UpdateAgentProperties();
      MPPerkObject.GetPerkHandler(this.Agent)?.OnEvent(this.Agent, MPPerkCondition.PerkEventFlags.MountChange);
    }

    protected internal override void OnDismount(Agent mount)
    {
      mount.OnAgentHealthChanged -= new Agent.OnAgentHealthChangedDelegate(this.OnMountHealthChanged);
      mount.UpdateAgentProperties();
      MPPerkObject.GetPerkHandler(this.Agent)?.OnEvent(this.Agent, MPPerkCondition.PerkEventFlags.MountChange);
    }

    protected internal override void OnItemPickup(SpawnedItemEntity item)
    {
      if (item.WeaponCopy.IsEmpty || item.WeaponCopy.Item.ItemType != ItemObject.ItemTypeEnum.Banner)
        return;
      MPPerkObject.GetPerkHandler(this.Agent)?.OnEvent(MPPerkCondition.PerkEventFlags.BannerPickUp);
    }

    protected internal override void OnWeaponDrop(MissionWeapon droppedWeapon)
    {
      if (droppedWeapon.IsEmpty || droppedWeapon.Item.ItemType != ItemObject.ItemTypeEnum.Banner)
        return;
      MPPerkObject.GetPerkHandler(this.Agent)?.OnEvent(MPPerkCondition.PerkEventFlags.BannerDrop);
    }

    protected internal override void OnAgentRemoved()
    {
      if (!this.Agent.HasMount)
        return;
      this.Agent.MountAgent.OnAgentHealthChanged -= new Agent.OnAgentHealthChangedDelegate(this.OnMountHealthChanged);
      this.Agent.MountAgent.UpdateAgentProperties();
    }

    private void OnHealthChanged(Agent agent, float oldHealth, float newHealth) => MPPerkObject.GetPerkHandler(this.Agent)?.OnEvent(agent, MPPerkCondition.PerkEventFlags.HealthChange);

    private void OnMountHealthChanged(Agent agent, float oldHealth, float newHealth)
    {
      if (this.Agent.IsActive() && this.Agent.MountAgent == agent)
        MPPerkObject.GetPerkHandler(this.Agent)?.OnEvent(this.Agent, MPPerkCondition.PerkEventFlags.MountHealthChange);
      else
        agent.OnAgentHealthChanged -= new Agent.OnAgentHealthChangedDelegate(this.OnMountHealthChanged);
    }
  }
}
