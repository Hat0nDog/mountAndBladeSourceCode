// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattleAgentStatCalculateModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class CustomBattleAgentStatCalculateModel : AgentStatCalculateModel
  {
    public override float GetDifficultyModifier() => 1f;

    public override void InitializeAgentStats(
      Agent agent,
      Equipment spawnEquipment,
      AgentDrivenProperties agentDrivenProperties,
      AgentBuildData agentBuildData)
    {
      agentDrivenProperties.ArmorEncumbrance = spawnEquipment.GetTotalWeightOfArmor(agent.IsHuman);
      if (!agent.IsHuman)
      {
        AgentDrivenProperties drivenProperties1 = agentDrivenProperties;
        EquipmentElement equipmentElement1 = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
        int internalValue = (int) equipmentElement1.Item.Id.InternalValue;
        drivenProperties1.AiSpeciesIndex = internalValue;
        AgentDrivenProperties drivenProperties2 = agentDrivenProperties;
        equipmentElement1 = spawnEquipment[EquipmentIndex.HorseHarness];
        double num1 = 0.800000011920929 + (equipmentElement1.Item != null ? 0.200000002980232 : 0.0);
        drivenProperties2.AttributeRiding = (float) num1;
        float num2 = 0.0f;
        for (int index = 1; index < 12; ++index)
        {
          equipmentElement1 = spawnEquipment[index];
          if (equipmentElement1.Item != null)
          {
            double num3 = (double) num2;
            equipmentElement1 = spawnEquipment[index];
            double modifiedMountBodyArmor = (double) equipmentElement1.GetModifiedMountBodyArmor();
            num2 = (float) (num3 + modifiedMountBodyArmor);
          }
        }
        agentDrivenProperties.ArmorTorso = num2;
        equipmentElement1 = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
        ItemObject itemObject = equipmentElement1.Item;
        if (itemObject != null)
        {
          float num3 = 1f;
          if (!agent.Mission.Scene.IsAtmosphereIndoor)
          {
            if ((double) agent.Mission.Scene.GetRainDensity() > 0.0)
              num3 *= 0.9f;
            if (!MBMath.IsBetween(agent.Mission.Scene.TimeOfDay, 4f, 20.01f))
              num3 *= 0.9f;
          }
          HorseComponent horseComponent = itemObject.HorseComponent;
          EquipmentElement mountElement = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
          EquipmentElement equipmentElement2 = spawnEquipment[EquipmentIndex.HorseHarness];
          agentDrivenProperties.MountManeuver = (float) mountElement.GetModifiedMountManeuver(in equipmentElement2);
          agentDrivenProperties.MountSpeed = (float) ((double) num3 * (double) (mountElement.GetModifiedMountSpeed(in equipmentElement2) + 1) * 0.219999998807907);
          agentDrivenProperties.MountChargeDamage = (float) mountElement.GetModifiedMountCharge(in equipmentElement2) * 0.01f;
          agentDrivenProperties.MountDifficulty = (float) mountElement.Item.Difficulty;
          int effectiveSkill = this.GetEffectiveSkill(agent.RiderAgent.Character, agent.RiderAgent.Origin, agent.RiderAgent.Formation, DefaultSkills.Riding);
          agentDrivenProperties.TopSpeedReachDuration = Game.Current.BasicModels.RidingModel.CalculateAcceleration(in mountElement, in equipmentElement2, effectiveSkill);
          if (agent.RiderAgent != null)
          {
            agentDrivenProperties.MountSpeed *= (float) (1.0 + (double) effectiveSkill * (1.0 / 1000.0));
            agentDrivenProperties.MountManeuver *= (float) (1.0 + (double) effectiveSkill * 0.00039999998989515);
          }
        }
      }
      else
      {
        agentDrivenProperties.ArmorHead = spawnEquipment.GetHeadArmorSum();
        agentDrivenProperties.ArmorTorso = spawnEquipment.GetHumanBodyArmorSum();
        agentDrivenProperties.ArmorLegs = spawnEquipment.GetLegArmorSum();
        agentDrivenProperties.ArmorArms = spawnEquipment.GetArmArmorSum();
      }
      foreach (DrivenPropertyBonusAgentComponent bonusAgentComponent in agent.Components.OfType<DrivenPropertyBonusAgentComponent>())
      {
        if (MBMath.IsBetween((int) bonusAgentComponent.DrivenProperty, 0, 56))
        {
          float num = agentDrivenProperties.GetStat(bonusAgentComponent.DrivenProperty) + bonusAgentComponent.DrivenPropertyBonus;
          agentDrivenProperties.SetStat(bonusAgentComponent.DrivenProperty, num);
        }
      }
    }

    public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
    {
      if (!agent.IsHuman)
        return;
      BasicCharacterObject character1 = agent.Character;
      MissionEquipment equipment = agent.Equipment;
      float totalWeightOfWeapons = equipment.GetTotalWeightOfWeapons();
      int weight = agent.Monster.Weight;
      float num1 = agentDrivenProperties.ArmorEncumbrance + totalWeightOfWeapons;
      EquipmentIndex wieldedItemIndex1 = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      EquipmentIndex wieldedItemIndex2 = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
      if (wieldedItemIndex1 != EquipmentIndex.None)
      {
        ItemObject itemObject = equipment[wieldedItemIndex1].Item;
        float realWeaponLength = itemObject.WeaponComponent.PrimaryWeapon.GetRealWeaponLength();
        totalWeightOfWeapons += 1.5f * itemObject.Weight * MathF.Sqrt(realWeaponLength);
      }
      if (wieldedItemIndex2 != EquipmentIndex.None)
      {
        ItemObject itemObject = equipment[wieldedItemIndex2].Item;
        totalWeightOfWeapons += 1.5f * itemObject.Weight;
      }
      agentDrivenProperties.WeaponsEncumbrance = totalWeightOfWeapons;
      EquipmentIndex wieldedItemIndex3 = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      MissionWeapon missionWeapon;
      WeaponComponentData weaponComponentData1;
      if (wieldedItemIndex3 == EquipmentIndex.None)
      {
        weaponComponentData1 = (WeaponComponentData) null;
      }
      else
      {
        missionWeapon = equipment[wieldedItemIndex3];
        weaponComponentData1 = missionWeapon.CurrentUsageItem;
      }
      WeaponComponentData equippedItem = weaponComponentData1;
      ItemObject itemObject1;
      if (wieldedItemIndex3 == EquipmentIndex.None)
      {
        itemObject1 = (ItemObject) null;
      }
      else
      {
        missionWeapon = equipment[wieldedItemIndex3];
        itemObject1 = missionWeapon.Item;
      }
      ItemObject primaryItem = itemObject1;
      EquipmentIndex wieldedItemIndex4 = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
      WeaponComponentData weaponComponentData2;
      if (wieldedItemIndex4 == EquipmentIndex.None)
      {
        weaponComponentData2 = (WeaponComponentData) null;
      }
      else
      {
        missionWeapon = equipment[wieldedItemIndex4];
        weaponComponentData2 = missionWeapon.CurrentUsageItem;
      }
      WeaponComponentData secondaryItem = weaponComponentData2;
      float inaccuracy;
      agentDrivenProperties.LongestRangedWeaponSlotIndex = (float) equipment.GetLongestRangedWeaponWithAimingError(out inaccuracy, agent);
      agentDrivenProperties.LongestRangedWeaponInaccuracy = inaccuracy;
      agentDrivenProperties.SwingSpeedMultiplier = (float) (0.930000007152557 + 0.000699999975040555 * (double) this.GetSkillValueForItem(agent, primaryItem));
      agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier = agentDrivenProperties.SwingSpeedMultiplier;
      agentDrivenProperties.HandlingMultiplier = 1f;
      agentDrivenProperties.ShieldBashStunDurationMultiplier = 1f;
      agentDrivenProperties.KickStunDurationMultiplier = 1f;
      agentDrivenProperties.ReloadSpeed = (float) (0.930000007152557 + 0.000699999975040555 * (double) this.GetSkillValueForItem(agent, primaryItem));
      agentDrivenProperties.ReloadMovementPenaltyFactor = 1f;
      agentDrivenProperties.WeaponInaccuracy = 0.0f;
      IAgentOriginBase origin = agent.Origin;
      BasicCharacterObject character2 = agent.Character;
      Formation formation = agent.Formation;
      int effectiveSkill1 = this.GetEffectiveSkill(character2, origin, agent.Formation, DefaultSkills.Athletics);
      int effectiveSkill2 = this.GetEffectiveSkill(character2, origin, formation, DefaultSkills.Riding);
      if (equippedItem != null)
      {
        int thrustSpeed = equippedItem.ThrustSpeed;
        WeaponComponentData weapon = equippedItem;
        int effectiveSkill3 = this.GetEffectiveSkill(character2, origin, formation, equippedItem.RelevantSkill);
        agentDrivenProperties.WeaponInaccuracy = this.GetWeaponInaccuracy(agent, weapon, effectiveSkill3);
        if (equippedItem.IsRangedWeapon)
        {
          agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = (float) (500 - effectiveSkill3) * 0.00025f;
          agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = (float) (500 - effectiveSkill3) * 0.0002f;
          if (agent.HasMount)
          {
            agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= Math.Max(1f, (float) (700 - effectiveSkill3 - effectiveSkill2) * (3f / 1000f));
            agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= Math.Max(1f, (float) (700 - effectiveSkill3 - effectiveSkill2) * 0.0033f);
          }
          else if (weapon.RelevantSkill == DefaultSkills.Bow)
          {
            agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 4.5f / MBMath.Lerp(0.75f, 2f, (float) (((double) thrustSpeed - 45.0) / 90.0));
            agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 6f;
          }
          else if (weapon.RelevantSkill == DefaultSkills.Crossbow)
          {
            agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 1.2f;
            agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 2.5f;
          }
          else if (weapon.RelevantSkill == DefaultSkills.Throwing)
            agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 3.5f * MBMath.Lerp(1.5f, 0.8f, (float) (((double) thrustSpeed - 89.0) / 13.0));
          if (weapon.WeaponClass == WeaponClass.Bow)
          {
            agentDrivenProperties.WeaponBestAccuracyWaitTime = (float) (0.300000011920929 + (95.75 - (double) thrustSpeed) * 0.00499999988824129);
            agentDrivenProperties.WeaponUnsteadyBeginTime = (float) (0.600000023841858 + (double) effectiveSkill3 * 0.00999999977648258 * (double) MBMath.Lerp(2f, 4f, (float) (((double) thrustSpeed - 45.0) / 90.0)));
            if (agent.IsAIControlled)
              agentDrivenProperties.WeaponUnsteadyBeginTime *= 4f;
            agentDrivenProperties.WeaponUnsteadyEndTime = 2f + agentDrivenProperties.WeaponUnsteadyBeginTime;
            agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.1f;
          }
          else if (weapon.WeaponClass == WeaponClass.Javelin || weapon.WeaponClass == WeaponClass.ThrowingAxe || weapon.WeaponClass == WeaponClass.ThrowingKnife)
          {
            agentDrivenProperties.WeaponBestAccuracyWaitTime = (float) (0.400000005960464 + (89.0 - (double) thrustSpeed) * 0.0299999993294477);
            agentDrivenProperties.WeaponUnsteadyBeginTime = (float) (2.5 + (double) character1.GetSkillValue(equippedItem.RelevantSkill) * 0.00999999977648258);
            agentDrivenProperties.WeaponUnsteadyEndTime = 10f + agentDrivenProperties.WeaponUnsteadyBeginTime;
            agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.025f;
          }
          else
          {
            agentDrivenProperties.WeaponBestAccuracyWaitTime = 0.1f;
            agentDrivenProperties.WeaponUnsteadyBeginTime = 0.0f;
            agentDrivenProperties.WeaponUnsteadyEndTime = 0.0f;
            agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.1f;
          }
        }
        else if (equippedItem.RelevantSkill == DefaultSkills.Polearm && equippedItem.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.WideGrip))
        {
          agentDrivenProperties.WeaponUnsteadyBeginTime = (float) (1.0 + (double) effectiveSkill3 * 0.00499999988824129);
          agentDrivenProperties.WeaponUnsteadyEndTime = (float) (3.0 + (double) effectiveSkill3 * 0.00999999977648258);
        }
        if (agent.HasMount)
        {
          float num2 = 1f - Math.Max(0.0f, (float) (0.200000002980232 - (double) effectiveSkill2 * (1.0 / 500.0)));
          agentDrivenProperties.SwingSpeedMultiplier *= num2;
          agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier *= num2;
          agentDrivenProperties.ReloadSpeed *= num2;
        }
      }
      agentDrivenProperties.TopSpeedReachDuration = 2f / Math.Max((float) ((200.0 + (double) effectiveSkill1) / 300.0 * ((double) weight / ((double) weight + (double) num1))), 0.3f);
      float num3 = 1f;
      if (!agent.Mission.Scene.IsAtmosphereIndoor && (double) agent.Mission.Scene.GetRainDensity() > 0.0)
        num3 *= 0.9f;
      agentDrivenProperties.MaxSpeedMultiplier = num3 * Math.Min((float) ((200.0 + (double) effectiveSkill1) / 300.0 * ((double) weight * 2.0 / ((double) weight * 2.0 + (double) num1))), 1f);
      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalCombatSpeedMinMultiplier);
      float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalCombatSpeedMaxMultiplier);
      float amount = Math.Min(num1 / (float) weight, 1f);
      agentDrivenProperties.CombatMaxSpeedMultiplier = Math.Min(MBMath.Lerp(managedParameter2, managedParameter1, amount), 1f);
      agentDrivenProperties.AttributeShieldMissileCollisionBodySizeAdder = 0.3f;
      Agent mountAgent = agent.MountAgent;
      float num4 = mountAgent != null ? mountAgent.GetAgentDrivenPropertyValue(DrivenProperty.AttributeRiding) : 1f;
      agentDrivenProperties.AttributeRiding = (float) effectiveSkill2 * num4;
      agentDrivenProperties.AttributeHorseArchery = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateHorseArcheryFactor(character1);
      agentDrivenProperties.BipedalRangedReadySpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRangedReadySpeedMultiplier);
      agentDrivenProperties.BipedalRangedReloadSpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRangedReloadSpeedMultiplier);
      this.SetAiRelatedProperties(agent, agentDrivenProperties, equippedItem, secondaryItem);
    }

    private int GetSkillValueForItem(Agent agent, ItemObject primaryItem) => this.GetEffectiveSkill(agent.Character, agent.Origin, agent.Formation, primaryItem != null ? primaryItem.RelevantSkill : DefaultSkills.Athletics);
  }
}
