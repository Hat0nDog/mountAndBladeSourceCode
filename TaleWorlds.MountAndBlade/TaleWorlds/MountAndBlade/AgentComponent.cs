// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public abstract class AgentComponent
  {
    protected readonly Agent Agent;

    protected AgentComponent(Agent agent) => this.Agent = agent;

    protected internal virtual void Initialize()
    {
    }

    protected internal virtual void OnTickAsAI(float dt)
    {
    }

    protected internal virtual float GetMoraleAddition() => 0.0f;

    protected internal virtual float GetMoraleDecreaseConstant() => 1f;

    protected internal virtual void OnItemPickup(SpawnedItemEntity item)
    {
    }

    protected internal virtual void OnWeaponDrop(MissionWeapon droppedWeapon)
    {
    }

    protected internal virtual void OnStopUsingGameObject()
    {
    }

    protected internal virtual void OnWeaponHPChanged(ItemObject item, int hitPoints)
    {
    }

    protected internal virtual void OnRetreating()
    {
    }

    protected internal virtual void OnMount(Agent mount)
    {
    }

    protected internal virtual void OnDismount(Agent mount)
    {
    }

    protected internal virtual void OnHit(
      Agent affectorAgent,
      int damage,
      in MissionWeapon affectorWeapon)
    {
    }

    protected internal virtual void OnDisciplineChanged()
    {
    }

    protected internal virtual void OnAgentRemoved()
    {
    }
  }
}
