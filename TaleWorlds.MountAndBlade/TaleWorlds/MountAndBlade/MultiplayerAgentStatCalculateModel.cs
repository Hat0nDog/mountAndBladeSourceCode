// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerAgentStatCalculateModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerAgentStatCalculateModel : AgentStatCalculateModel
  {
    public override float GetDifficultyModifier() => 0.5f;

    public override void InitializeAgentStats(
      Agent agent,
      Equipment spawnEquipment,
      AgentDrivenProperties agentDrivenProperties,
      AgentBuildData agentBuildData)
    {
      agentDrivenProperties.ArmorEncumbrance = spawnEquipment.GetTotalWeightOfArmor(agent.IsHuman);
      if (!agent.IsHuman)
        MultiplayerAgentStatCalculateModel.InitializeHorseAgentStats(agent, spawnEquipment, agentDrivenProperties);
      else
        agentDrivenProperties = this.InitializeHumanAgentStats(agent, agentDrivenProperties, agentBuildData);
      foreach (DrivenPropertyBonusAgentComponent bonusAgentComponent in agent.Components.OfType<DrivenPropertyBonusAgentComponent>())
      {
        if (MBMath.IsBetween((int) bonusAgentComponent.DrivenProperty, 0, 56))
        {
          float num = agentDrivenProperties.GetStat(bonusAgentComponent.DrivenProperty) + bonusAgentComponent.DrivenPropertyBonus;
          agentDrivenProperties.SetStat(bonusAgentComponent.DrivenProperty, num);
        }
      }
    }

    private AgentDrivenProperties InitializeHumanAgentStats(
      Agent agent,
      AgentDrivenProperties agentDrivenProperties,
      AgentBuildData agentBuildData)
    {
      MultiplayerClassDivisions.MPHeroClass classForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character);
      if (classForCharacter != null)
      {
        this.FillAgentStatsFromData(ref agentDrivenProperties, classForCharacter, agentBuildData?.AgentMissionPeer, agentBuildData?.OwningAgentMissionPeer);
        agentDrivenProperties.SetStat(DrivenProperty.UseRealisticBlocking, MultiplayerOptions.OptionType.UseRealisticBlocking.GetBoolValue() ? 1f : 0.0f);
      }
      agent.BaseHealthLimit = classForCharacter == null ? 100f : (float) classForCharacter.Health;
      agent.HealthLimit = agent.BaseHealthLimit;
      agent.Health = agent.HealthLimit;
      return agentDrivenProperties;
    }

    private static void InitializeHorseAgentStats(
      Agent agent,
      Equipment spawnEquipment,
      AgentDrivenProperties agentDrivenProperties)
    {
      AgentDrivenProperties drivenProperties1 = agentDrivenProperties;
      EquipmentElement equipmentElement1 = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
      HorseComponent horseComponent1 = equipmentElement1.Item.HorseComponent;
      int num1 = horseComponent1 != null ? horseComponent1.Monster.FamilyType : 0;
      drivenProperties1.AiSpeciesIndex = num1;
      AgentDrivenProperties drivenProperties2 = agentDrivenProperties;
      equipmentElement1 = spawnEquipment[EquipmentIndex.HorseHarness];
      double num2 = 0.800000011920929 + (equipmentElement1.Item != null ? 0.200000002980232 : 0.0);
      drivenProperties2.AttributeRiding = (float) num2;
      float num3 = 0.0f;
      for (int index = 1; index < 12; ++index)
      {
        equipmentElement1 = spawnEquipment[index];
        if (equipmentElement1.Item != null)
        {
          double num4 = (double) num3;
          equipmentElement1 = spawnEquipment[index];
          double modifiedMountBodyArmor = (double) equipmentElement1.GetModifiedMountBodyArmor();
          num3 = (float) (num4 + modifiedMountBodyArmor);
        }
      }
      agentDrivenProperties.ArmorTorso = num3;
      equipmentElement1 = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
      ItemObject itemObject = equipmentElement1.Item;
      if (itemObject == null)
        return;
      HorseComponent horseComponent2 = itemObject.HorseComponent;
      EquipmentElement mountElement = spawnEquipment[EquipmentIndex.ArmorItemEndSlot];
      EquipmentElement equipmentElement2 = spawnEquipment[EquipmentIndex.HorseHarness];
      agentDrivenProperties.MountManeuver = (float) mountElement.GetModifiedMountManeuver(in equipmentElement2) * 0.8f;
      agentDrivenProperties.MountSpeed = (float) (mountElement.GetModifiedMountSpeed(in equipmentElement2) + 1) * 0.2f;
      agentDrivenProperties.MountChargeDamage = (float) mountElement.GetModifiedMountCharge(in equipmentElement2) * 0.01f;
      agentDrivenProperties.MountDifficulty = (float) mountElement.Item.Difficulty;
      Agent riderAgent = agent.RiderAgent;
      int ridingSkill = riderAgent != null ? riderAgent.Character.GetSkillValue(DefaultSkills.Riding) : 0;
      agentDrivenProperties.TopSpeedReachDuration = Game.Current.BasicModels.RidingModel.CalculateAcceleration(in mountElement, in equipmentElement2, ridingSkill);
      if (agent.RiderAgent == null)
        return;
      agentDrivenProperties.MountSpeed *= (float) (1.0 + (double) ridingSkill * (2.0 / 625.0));
      agentDrivenProperties.MountManeuver *= (float) (1.0 + (double) ridingSkill * 0.00350000010803342);
    }

    public override void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
    {
      if (agent.IsHuman)
      {
        this.UpdateHumanAgentStats(agent, agentDrivenProperties);
      }
      else
      {
        if (!agent.IsMount)
          return;
        this.UpdateMountAgentStats(agent, agentDrivenProperties);
      }
    }

    private void UpdateMountAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
    {
      MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(agent.RiderAgent);
      EquipmentElement mountElement = agent.SpawnEquipment[EquipmentIndex.ArmorItemEndSlot];
      EquipmentElement equipmentElement = agent.SpawnEquipment[EquipmentIndex.HorseHarness];
      agentDrivenProperties.MountManeuver = (float) mountElement.GetModifiedMountManeuver(in equipmentElement) * (float) (1.0 + (perkHandler != null ? (double) perkHandler.GetMountManeuver() : 0.0));
      agentDrivenProperties.MountSpeed = (float) ((double) (mountElement.GetModifiedMountSpeed(in equipmentElement) + 1) * 0.219999998807907 * (1.0 + (perkHandler != null ? (double) perkHandler.GetMountSpeed() : 0.0)));
      Agent riderAgent = agent.RiderAgent;
      int ridingSkill = riderAgent != null ? riderAgent.Character.GetSkillValue(DefaultSkills.Riding) : 0;
      agentDrivenProperties.TopSpeedReachDuration = Game.Current.BasicModels.RidingModel.CalculateAcceleration(in mountElement, in equipmentElement, ridingSkill);
      if (agent.RiderAgent == null)
        return;
      agentDrivenProperties.MountSpeed *= (float) (1.0 + (double) ridingSkill * (2.0 / 625.0));
      agentDrivenProperties.MountManeuver *= (float) (1.0 + (double) ridingSkill * 0.00350000010803342);
    }

    private void UpdateHumanAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties)
    {
      MPPerkObject.MPPerkHandler perkHandler = MPPerkObject.GetPerkHandler(agent);
      BasicCharacterObject character = agent.Character;
      MissionEquipment equipment = agent.Equipment;
      float num1 = equipment.GetTotalWeightOfWeapons() * (float) (1.0 + (perkHandler != null ? (double) perkHandler.GetEncumbrance(true) : 0.0));
      EquipmentIndex wieldedItemIndex1 = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      EquipmentIndex wieldedItemIndex2 = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
      if (wieldedItemIndex1 != EquipmentIndex.None)
      {
        ItemObject itemObject = equipment[wieldedItemIndex1].Item;
        WeaponComponent weaponComponent = itemObject.WeaponComponent;
        float realWeaponLength = weaponComponent.PrimaryWeapon.GetRealWeaponLength();
        float num2 = (weaponComponent.GetItemType() == ItemObject.ItemTypeEnum.Bow ? 4f : 1.5f) * itemObject.Weight * MathF.Sqrt(realWeaponLength) * (float) (1.0 + (perkHandler != null ? (double) perkHandler.GetEncumbrance(false) : 0.0));
        num1 += num2;
      }
      if (wieldedItemIndex2 != EquipmentIndex.None)
      {
        float num2 = 1.5f * equipment[wieldedItemIndex2].Item.Weight * (float) (1.0 + (perkHandler != null ? (double) perkHandler.GetEncumbrance(false) : 0.0));
        num1 += num2;
      }
      agentDrivenProperties.WeaponsEncumbrance = num1;
      EquipmentIndex wieldedItemIndex3 = agent.GetWieldedItemIndex(Agent.HandIndex.MainHand);
      WeaponComponentData weaponComponentData = wieldedItemIndex3 != EquipmentIndex.None ? equipment[wieldedItemIndex3].CurrentUsageItem : (WeaponComponentData) null;
      ItemObject primaryItem = wieldedItemIndex3 != EquipmentIndex.None ? equipment[wieldedItemIndex3].Item : (ItemObject) null;
      EquipmentIndex wieldedItemIndex4 = agent.GetWieldedItemIndex(Agent.HandIndex.OffHand);
      WeaponComponentData secondaryItem = wieldedItemIndex4 != EquipmentIndex.None ? equipment[wieldedItemIndex4].CurrentUsageItem : (WeaponComponentData) null;
      float inaccuracy;
      agentDrivenProperties.LongestRangedWeaponSlotIndex = (float) equipment.GetLongestRangedWeaponWithAimingError(out inaccuracy, agent);
      agentDrivenProperties.LongestRangedWeaponInaccuracy = inaccuracy;
      agentDrivenProperties.SwingSpeedMultiplier = (float) (0.930000007152557 + 0.000699999975040555 * (double) this.GetSkillValueForItem(character, primaryItem));
      agentDrivenProperties.ThrustOrRangedReadySpeedMultiplier = agentDrivenProperties.SwingSpeedMultiplier;
      agentDrivenProperties.HandlingMultiplier = 1f;
      agentDrivenProperties.ShieldBashStunDurationMultiplier = 1f;
      agentDrivenProperties.KickStunDurationMultiplier = 1f;
      agentDrivenProperties.ReloadSpeed = (float) (0.930000007152557 + 0.000699999975040555 * (double) this.GetSkillValueForItem(character, primaryItem));
      agentDrivenProperties.ReloadMovementPenaltyFactor = 1f;
      agentDrivenProperties.WeaponInaccuracy = 0.0f;
      MultiplayerClassDivisions.MPHeroClass classForCharacter = MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character);
      agentDrivenProperties.MaxSpeedMultiplier = (float) (1.04999995231628 * ((double) classForCharacter.MovementSpeedMultiplier * (100.0 / (100.0 + (double) num1))));
      int skillValue = character.GetSkillValue(DefaultSkills.Riding);
      bool flag1 = false;
      bool flag2 = false;
      if (weaponComponentData != null)
      {
        int weaponSkill = character.GetSkillValue(weaponComponentData.RelevantSkill);
        if (weaponSkill > 0 && weaponComponentData.IsRangedWeapon && perkHandler != null)
          weaponSkill = MathF.Ceiling((float) weaponSkill * (perkHandler.GetRangedAccuracy() + 1f));
        int thrustSpeed = weaponComponentData.ThrustSpeed;
        agentDrivenProperties.WeaponInaccuracy = this.GetWeaponInaccuracy(agent, weaponComponentData, weaponSkill);
        if (weaponComponentData.IsRangedWeapon)
        {
          agentDrivenProperties.WeaponMaxMovementAccuracyPenalty = (float) (500 - weaponSkill) * 0.00025f;
          agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty = (float) (500 - weaponSkill) * 0.0002f;
          if (agent.HasMount)
          {
            agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= Math.Max(1f, (float) (700 - weaponSkill - skillValue) * (3f / 1000f));
            agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= Math.Max(1f, (float) (700 - weaponSkill - skillValue) * 0.0033f);
          }
          else if (weaponComponentData.RelevantSkill == DefaultSkills.Bow)
          {
            agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 4.5f / MBMath.Lerp(0.75f, 2f, (float) (((double) thrustSpeed - 60.0) / 75.0));
            agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 6f;
          }
          else if (weaponComponentData.RelevantSkill == DefaultSkills.Crossbow)
          {
            agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 1.2f;
            agentDrivenProperties.WeaponMaxMovementAccuracyPenalty *= 2.5f;
          }
          else if (weaponComponentData.RelevantSkill == DefaultSkills.Throwing)
            agentDrivenProperties.WeaponMaxUnsteadyAccuracyPenalty *= 3.5f * MBMath.Lerp(1.5f, 0.8f, (float) (((double) thrustSpeed - 89.0) / 13.0));
          if (weaponComponentData.WeaponClass == WeaponClass.Bow)
          {
            flag1 = true;
            agentDrivenProperties.WeaponBestAccuracyWaitTime = (float) (0.300000011920929 + (95.75 - (double) thrustSpeed) * 0.00499999988824129);
            agentDrivenProperties.WeaponUnsteadyBeginTime = (float) (0.100000001490116 + (double) weaponSkill * 0.00999999977648258 * (double) MBMath.Lerp(1f, 2f, (float) (((double) thrustSpeed - 60.0) / 75.0)));
            if (agent.IsAIControlled)
              agentDrivenProperties.WeaponUnsteadyBeginTime *= 4f;
            agentDrivenProperties.WeaponUnsteadyEndTime = 2f + agentDrivenProperties.WeaponUnsteadyBeginTime;
            agentDrivenProperties.WeaponRotationalAccuracyPenaltyInRadians = 0.1f;
          }
          else if (weaponComponentData.WeaponClass == WeaponClass.Javelin || weaponComponentData.WeaponClass == WeaponClass.ThrowingAxe || weaponComponentData.WeaponClass == WeaponClass.ThrowingKnife)
          {
            agentDrivenProperties.WeaponBestAccuracyWaitTime = (float) (0.400000005960464 + (89.0 - (double) thrustSpeed) * 0.0299999993294477);
            agentDrivenProperties.WeaponUnsteadyBeginTime = (float) (2.5 + (double) weaponSkill * 0.00999999977648258);
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
        else if (weaponComponentData.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.WideGrip))
        {
          flag2 = true;
          agentDrivenProperties.WeaponUnsteadyBeginTime = (float) (1.0 + (double) weaponSkill * 0.00499999988824129);
          agentDrivenProperties.WeaponUnsteadyEndTime = (float) (3.0 + (double) weaponSkill * 0.00999999977648258);
        }
      }
      agentDrivenProperties.AttributeShieldMissileCollisionBodySizeAdder = 0.3f;
      Agent mountAgent = agent.MountAgent;
      float num3 = mountAgent != null ? mountAgent.GetAgentDrivenPropertyValue(DrivenProperty.AttributeRiding) : 1f;
      agentDrivenProperties.AttributeRiding = (float) skillValue * num3;
      agentDrivenProperties.AttributeHorseArchery = Game.Current.BasicModels.StrikeMagnitudeModel.CalculateHorseArcheryFactor(character);
      agentDrivenProperties.BipedalRangedReadySpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRangedReadySpeedMultiplier);
      agentDrivenProperties.BipedalRangedReloadSpeedMultiplier = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalRangedReloadSpeedMultiplier);
      foreach (DrivenPropertyBonusAgentComponent bonusAgentComponent in agent.Components.OfType<DrivenPropertyBonusAgentComponent>())
      {
        if (!MBMath.IsBetween((int) bonusAgentComponent.DrivenProperty, 0, 56))
        {
          float num2 = agentDrivenProperties.GetStat(bonusAgentComponent.DrivenProperty) + bonusAgentComponent.DrivenPropertyBonus;
          agentDrivenProperties.SetStat(bonusAgentComponent.DrivenProperty, num2);
        }
      }
      if (perkHandler != null)
      {
        for (int index = 56; index < 85; ++index)
        {
          DrivenProperty drivenProperty = (DrivenProperty) index;
          if (((drivenProperty == DrivenProperty.WeaponUnsteadyBeginTime ? 0 : (drivenProperty != DrivenProperty.WeaponUnsteadyEndTime ? 1 : 0)) | (flag1 ? 1 : 0) | (flag2 ? 1 : 0)) != 0 && drivenProperty != DrivenProperty.WeaponRotationalAccuracyPenaltyInRadians | flag1)
          {
            float stat = agentDrivenProperties.GetStat(drivenProperty);
            agentDrivenProperties.SetStat(drivenProperty, stat + perkHandler.GetDrivenPropertyBonus(drivenProperty, stat));
          }
        }
      }
      this.SetAiRelatedProperties(agent, agentDrivenProperties, weaponComponentData, secondaryItem);
    }

    private void FillAgentStatsFromData(
      ref AgentDrivenProperties agentDrivenProperties,
      MultiplayerClassDivisions.MPHeroClass heroClass,
      MissionPeer missionPeer,
      MissionPeer owningMissionPeer)
    {
      MissionPeer peer = missionPeer ?? owningMissionPeer;
      if (peer != null)
      {
        MPPerkObject.MPOnSpawnPerkHandler spawnPerkHandler = MPPerkObject.GetOnSpawnPerkHandler(peer);
        bool isPlayer = missionPeer != null;
        for (int index = 0; index < 56; ++index)
        {
          DrivenProperty drivenProperty = (DrivenProperty) index;
          float stat = agentDrivenProperties.GetStat(drivenProperty);
          if (drivenProperty == DrivenProperty.ArmorHead || drivenProperty == DrivenProperty.ArmorTorso || (drivenProperty == DrivenProperty.ArmorLegs || drivenProperty == DrivenProperty.ArmorArms))
            agentDrivenProperties.SetStat(drivenProperty, stat + (float) heroClass.ArmorValue + spawnPerkHandler.GetDrivenPropertyBonusOnSpawn(isPlayer, drivenProperty, stat));
          else
            agentDrivenProperties.SetStat(drivenProperty, stat + spawnPerkHandler.GetDrivenPropertyBonusOnSpawn(isPlayer, drivenProperty, stat));
        }
      }
      agentDrivenProperties.TopSpeedReachDuration = heroClass.TopSpeedReachDuration;
      float managedParameter1 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalCombatSpeedMinMultiplier);
      float managedParameter2 = ManagedParameters.Instance.GetManagedParameter(ManagedParametersEnum.BipedalCombatSpeedMaxMultiplier);
      agentDrivenProperties.CombatMaxSpeedMultiplier = managedParameter1 + (managedParameter2 - managedParameter1) * heroClass.CombatMovementSpeedMultiplier;
    }

    private int GetSkillValueForItem(BasicCharacterObject characterObject, ItemObject primaryItem) => characterObject.GetSkillValue(primaryItem != null ? primaryItem.RelevantSkill : DefaultSkills.Athletics);

    public static float CalculateMaximumSpeedMultiplier(Agent agent) => MultiplayerClassDivisions.GetMPHeroClassForCharacter(agent.Character).MovementSpeedMultiplier;
  }
}
