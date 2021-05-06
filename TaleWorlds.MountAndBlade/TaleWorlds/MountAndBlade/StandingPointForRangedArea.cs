// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.StandingPointForRangedArea
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class StandingPointForRangedArea : StandingPoint
  {
    public float ThrowingValueMultiplier = 5f;
    public float RangedWeaponValueMultiplier = 2f;

    public override Agent.AIScriptedFrameFlags DisableScriptedFrameFlags => Agent.AIScriptedFrameFlags.NoAttack | Agent.AIScriptedFrameFlags.ConsiderRotation;

    protected internal override void OnInit()
    {
      base.OnInit();
      this.AutoSheathWeapons = false;
      this.LockUserFrames = false;
      this.LockUserPositions = true;
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    public override bool IsDisabledForAgent(Agent agent)
    {
      EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      if (wieldedItemIndex == EquipmentIndex.None)
        return true;
      WeaponComponentData currentUsageItem = agent.Equipment[wieldedItemIndex].CurrentUsageItem;
      if (currentUsageItem == null || !currentUsageItem.IsRangedWeapon)
        return true;
      return wieldedItemIndex == EquipmentIndex.Weapon4 ? (double) this.ThrowingValueMultiplier <= 0.0 || base.IsDisabledForAgent(agent) : (double) this.RangedWeaponValueMultiplier <= 0.0 || base.IsDisabledForAgent(agent);
    }

    public override float GetUsageScoreForAgent(Agent agent)
    {
      EquipmentIndex wieldedItemIndex = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      float num = 0.0f;
      if (wieldedItemIndex != EquipmentIndex.None && agent.Equipment[wieldedItemIndex].CurrentUsageItem.IsRangedWeapon)
        num = wieldedItemIndex == EquipmentIndex.Weapon4 ? this.ThrowingValueMultiplier : this.RangedWeaponValueMultiplier;
      return base.GetUsageScoreForAgent(agent) + num;
    }

    public override bool HasAlternative() => true;

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => this.HasUser ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this.HasUser || !this.IsDisabledForAgent(this.UserAgent))
        return;
      this.UserAgent.StopUsingGameObject(false);
    }
  }
}
