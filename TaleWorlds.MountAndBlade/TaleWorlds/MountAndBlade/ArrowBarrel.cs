// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ArrowBarrel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class ArrowBarrel : UsableMachine
  {
    private static readonly ActionIndexCache act_pickup_down_begin = ActionIndexCache.Create(nameof (act_pickup_down_begin));
    private static readonly ActionIndexCache act_pickup_down_begin_left_stance = ActionIndexCache.Create(nameof (act_pickup_down_begin_left_stance));
    private static readonly ActionIndexCache act_pickup_down_end = ActionIndexCache.Create(nameof (act_pickup_down_end));
    private static readonly ActionIndexCache act_pickup_down_end_left_stance = ActionIndexCache.Create(nameof (act_pickup_down_end_left_stance));
    private static int _pickupArrowSoundFromBarrel = -1;
    private bool _isVisible = true;

    private static int _pickupArrowSoundFromBarrelCache
    {
      get
      {
        if (ArrowBarrel._pickupArrowSoundFromBarrel == -1)
          ArrowBarrel._pickupArrowSoundFromBarrel = SoundEvent.GetEventIdFromString("event:/mission/combat/pickup_arrows");
        return ArrowBarrel._pickupArrowSoundFromBarrel;
      }
    }

    protected ArrowBarrel()
    {
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      foreach (StandingPoint standingPoint in this.StandingPoints)
        (standingPoint as StandingPointWithWeaponRequirement).InitRequiredWeaponClasses(WeaponClass.Arrow, WeaponClass.Bolt);
      this.SetScriptComponentToTick(this.GetTickRequirement());
      this.MakeVisibilityCheck = false;
      this._isVisible = this.GameEntity.IsVisibleIncludeParents();
    }

    public override void AfterMissionStart()
    {
      if (this.StandingPoints == null)
        return;
      foreach (UsableMissionObject standingPoint in this.StandingPoints)
        standingPoint.LockUserFrames = true;
    }

    public override TextObject GetActionTextForStandingPoint(
      UsableMissionObject usableGameObject)
    {
      TextObject textObject = new TextObject("{=bNYm3K6b}({KEY}) Pick Up");
      textObject.SetTextVariable("KEY", Game.Current.GameTextManager.GetHotKeyGameText("CombatHotKeyCategory", 13));
      return textObject;
    }

    public override string GetDescriptionText(GameEntity gameEntity = null) => new TextObject("{=bWi4aMO9}Arrow Barrel").ToString();

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => !GameNetwork.IsClientOrReplay ? ScriptComponentBehaviour.TickRequirement.Tick : base.GetTickRequirement();

    protected internal override void OnTick(float dt)
    {
      if (!this._isVisible)
        return;
      base.OnTick(dt);
      if (GameNetwork.IsClientOrReplay)
        return;
      foreach (StandingPoint standingPoint in this.StandingPoints)
      {
        if (standingPoint.HasUser)
        {
          Agent userAgent = standingPoint.UserAgent;
          ActionIndexCache currentAction1 = userAgent.GetCurrentAction(0);
          ActionIndexCache currentAction2 = userAgent.GetCurrentAction(1);
          if (!(currentAction2 == ActionIndexCache.act_none) || !(currentAction1 == ArrowBarrel.act_pickup_down_begin) && !(currentAction1 == ArrowBarrel.act_pickup_down_begin_left_stance))
          {
            if (currentAction2 == ActionIndexCache.act_none && (currentAction1 == ArrowBarrel.act_pickup_down_end || currentAction1 == ArrowBarrel.act_pickup_down_end_left_stance))
            {
              for (EquipmentIndex index = EquipmentIndex.WeaponItemBeginSlot; index < EquipmentIndex.NumAllWeaponSlots; ++index)
              {
                MissionWeapon missionWeapon = userAgent.Equipment[index];
                if (!missionWeapon.IsEmpty)
                {
                  missionWeapon = userAgent.Equipment[index];
                  if (missionWeapon.CurrentUsageItem.WeaponClass != WeaponClass.Arrow)
                  {
                    missionWeapon = userAgent.Equipment[index];
                    if (missionWeapon.CurrentUsageItem.WeaponClass != WeaponClass.Bolt)
                      continue;
                  }
                  missionWeapon = userAgent.Equipment[index];
                  int amount = (int) missionWeapon.Amount;
                  missionWeapon = userAgent.Equipment[index];
                  int modifiedMaxAmount1 = (int) missionWeapon.ModifiedMaxAmount;
                  if (amount < modifiedMaxAmount1)
                  {
                    Agent agent = userAgent;
                    int num = (int) index;
                    missionWeapon = userAgent.Equipment[index];
                    int modifiedMaxAmount2 = (int) missionWeapon.ModifiedMaxAmount;
                    agent.SetWeaponAmountInSlot((EquipmentIndex) num, (short) modifiedMaxAmount2, true);
                    Mission.Current.MakeSoundOnlyOnRelatedPeer(ArrowBarrel._pickupArrowSoundFromBarrelCache, userAgent.Position, userAgent.Index);
                  }
                }
              }
              userAgent.StopUsingGameObject();
            }
            else if (currentAction2 != ActionIndexCache.act_none || !userAgent.SetActionChannel(0, userAgent.GetIsLeftStance() ? ArrowBarrel.act_pickup_down_begin_left_stance : ArrowBarrel.act_pickup_down_begin))
              userAgent.StopUsingGameObject();
          }
        }
      }
    }

    public override OrderType GetOrder(BattleSideEnum side) => OrderType.None;
  }
}
