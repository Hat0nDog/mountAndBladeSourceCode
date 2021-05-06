// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AgentStatCalculateModel
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class AgentStatCalculateModel : GameModel
  {
    protected const float MaxHorizontalErrorRadian = 0.03490658f;

    public abstract void InitializeAgentStats(
      Agent agent,
      Equipment spawnEquipment,
      AgentDrivenProperties agentDrivenProperties,
      AgentBuildData agentBuildData);

    public virtual void InitializeMissionEquipment(Agent agent)
    {
    }

    public abstract void UpdateAgentStats(Agent agent, AgentDrivenProperties agentDrivenProperties);

    public abstract float GetDifficultyModifier();

    public virtual float GetEffectiveMaxHealth(Agent agent) => agent.BaseHealthLimit;

    public float CalculateAIAttackOnDecideMaxValue() => (double) this.GetDifficultyModifier() < 0.5 ? 0.32f : 0.96f;

    public virtual float GetWeaponInaccuracy(
      Agent agent,
      WeaponComponentData weapon,
      int weaponSkill)
    {
      float val1 = 0.0f;
      if (weapon.IsRangedWeapon)
        val1 = (float) ((100.0 - (double) weapon.Accuracy) * (1.0 - 1.0 / 500.0 * (double) weaponSkill) * (1.0 / 1000.0));
      else if (weapon.WeaponFlags.HasAllFlags<WeaponFlags>(WeaponFlags.WideGrip))
        val1 = (float) (1.0 - (double) weaponSkill * 0.00999999977648258);
      return Math.Max(val1, 0.0f);
    }

    public virtual float GetInteractionDistance(Agent agent) => 1.5f;

    public virtual float GetMaxCameraZoom(Agent agent) => 1f;

    public virtual int GetEffectiveSkill(
      BasicCharacterObject agentCharacter,
      IAgentOriginBase agentOrigin,
      Formation agentFormation,
      SkillObject skill)
    {
      return agentCharacter.GetSkillValue(skill);
    }

    public virtual string GetMissionDebugInfoForAgent(Agent agent) => "Debug info not supported in this model";

    protected int GetMeleeSkill(
      Agent agent,
      WeaponComponentData equippedItem,
      WeaponComponentData secondaryItem)
    {
      SkillObject skill = DefaultSkills.Athletics;
      if (equippedItem != null)
      {
        SkillObject relevantSkill = equippedItem.RelevantSkill;
        skill = relevantSkill == DefaultSkills.OneHanded || relevantSkill == DefaultSkills.Polearm ? relevantSkill : (relevantSkill != DefaultSkills.TwoHanded ? DefaultSkills.OneHanded : (secondaryItem == null ? DefaultSkills.TwoHanded : DefaultSkills.OneHanded));
      }
      return this.GetEffectiveSkill(agent.Character, agent.Origin, agent.Formation, skill);
    }

    protected float CalculateAILevel(Agent agent, int relevantSkillLevel)
    {
      float difficultyModifier = this.GetDifficultyModifier();
      return MBMath.ClampFloat((float) relevantSkillLevel / 350f * difficultyModifier, 0.0f, 1f);
    }

    protected void SetAiRelatedProperties(
      Agent agent,
      AgentDrivenProperties agentDrivenProperties,
      WeaponComponentData equippedItem,
      WeaponComponentData secondaryItem)
    {
      int meleeSkill = this.GetMeleeSkill(agent, equippedItem, secondaryItem);
      SkillObject skill = equippedItem == null ? DefaultSkills.Athletics : equippedItem.RelevantSkill;
      int effectiveSkill = this.GetEffectiveSkill(agent.Character, agent.Origin, agent.Formation, skill);
      float aiLevel1 = this.CalculateAILevel(agent, meleeSkill);
      float aiLevel2 = this.CalculateAILevel(agent, effectiveSkill);
      float num1 = aiLevel1 + agent.Defensiveness;
      agentDrivenProperties.AiRangedHorsebackMissileRange = (float) (0.300000011920929 + 0.400000005960464 * (double) aiLevel2);
      agentDrivenProperties.AiFacingMissileWatch = (float) ((double) aiLevel1 * 0.0599999986588955 - 0.959999978542328);
      agentDrivenProperties.AiFlyingMissileCheckRadius = (float) (8.0 - 6.0 * (double) aiLevel1);
      agentDrivenProperties.AiShootFreq = (float) (0.300000011920929 + 0.699999988079071 * (double) aiLevel2);
      agentDrivenProperties.AiWaitBeforeShootFactor = agent._propertyModifiers.resetAiWaitBeforeShootFactor ? 0.0f : (float) (1.0 - 0.5 * (double) aiLevel2);
      int num2 = secondaryItem != null ? 1 : 0;
      agentDrivenProperties.AIBlockOnDecideAbility = MBMath.Lerp(0.25f, 0.99f, MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 1.0), 0.0f, 1f));
      agentDrivenProperties.AIParryOnDecideAbility = MBMath.Lerp(0.01f, 0.95f, MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 1.5), 0.0f, 1f));
      agentDrivenProperties.AiTryChamberAttackOnDecide = (float) (((double) aiLevel1 - 0.150000005960464) * 0.100000001490116);
      agentDrivenProperties.AIAttackOnParryChance = (float) (0.300000011920929 - 0.100000001490116 * (double) agent.Defensiveness);
      agentDrivenProperties.AiAttackOnParryTiming = (float) (0.300000011920929 * (double) aiLevel1 - 0.200000002980232);
      agentDrivenProperties.AIDecideOnAttackChance = 0.15f * agent.Defensiveness;
      agentDrivenProperties.AIParryOnAttackAbility = MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 3.0), 0.0f, 1f);
      agentDrivenProperties.AiKick = (float) (((double) aiLevel1 > 0.400000005960464 ? 0.400000005960464 : (double) aiLevel1) - 0.100000001490116);
      agentDrivenProperties.AiAttackCalculationMaxTimeFactor = aiLevel1;
      agentDrivenProperties.AiDecideOnAttackWhenReceiveHitTiming = (float) (-0.25 * (1.0 - (double) aiLevel1));
      agentDrivenProperties.AiDecideOnAttackContinueAction = (float) (-0.5 * (1.0 - (double) aiLevel1));
      agentDrivenProperties.AiDecideOnAttackingContinue = 0.1f * aiLevel1;
      agentDrivenProperties.AIParryOnAttackingContinueAbility = MBMath.Lerp(0.05f, 0.95f, MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 3.0), 0.0f, 1f));
      agentDrivenProperties.AIDecideOnRealizeEnemyBlockingAttackAbility = 0.5f * MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 2.5) - 0.1f, 0.0f, 1f);
      agentDrivenProperties.AIRealizeBlockingFromIncorrectSideAbility = 0.5f * MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 2.5) - 0.1f, 0.0f, 1f);
      agentDrivenProperties.AiAttackingShieldDefenseChance = (float) (0.200000002980232 + 0.300000011920929 * (double) aiLevel1);
      agentDrivenProperties.AiAttackingShieldDefenseTimer = (float) (0.300000011920929 * (double) aiLevel1 - 0.300000011920929);
      agentDrivenProperties.AiRandomizedDefendDirectionChance = (float) (1.0 - Math.Log((double) aiLevel1 * 7.0 + 1.0, 2.0) * 0.333330005407333);
      agentDrivenProperties.AISetNoAttackTimerAfterBeingHitAbility = MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 2.0), 0.05f, 0.95f);
      agentDrivenProperties.AISetNoAttackTimerAfterBeingParriedAbility = MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 2.0), 0.05f, 0.95f);
      agentDrivenProperties.AISetNoDefendTimerAfterHittingAbility = MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 2.0), 0.05f, 0.95f);
      agentDrivenProperties.AISetNoDefendTimerAfterParryingAbility = MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 2.0), 0.05f, 0.95f);
      agentDrivenProperties.AIEstimateStunDurationPrecision = 1f - MBMath.ClampFloat((float) Math.Pow((double) aiLevel1, 2.0), 0.05f, 0.95f);
      agentDrivenProperties.AIHoldingReadyMaxDuration = MBMath.Lerp(0.25f, 0.0f, Math.Min(1f, aiLevel1 * 1.2f));
      agentDrivenProperties.AIHoldingReadyVariationPercentage = aiLevel1;
      agentDrivenProperties.AiRaiseShieldDelayTimeBase = (float) (0.5 * (double) aiLevel1 - 0.75);
      agentDrivenProperties.AiUseShieldAgainstEnemyMissileProbability = (float) (0.100000001490116 + (double) aiLevel1 * 0.600000023841858 + (double) num1 * 0.200000002980232);
      agentDrivenProperties.AiCheckMovementIntervalFactor = (float) (0.00499999988824129 * (1.10000002384186 - (double) aiLevel1));
      agentDrivenProperties.AiMovemetDelayFactor = (float) (4.0 / (3.0 + (double) aiLevel2));
      agentDrivenProperties.AiParryDecisionChangeValue = (float) (0.0500000007450581 + 0.699999988079071 * (double) aiLevel1);
      agentDrivenProperties.AiDefendWithShieldDecisionChanceValue = Math.Min(1f, (float) (0.200000002980232 + 0.5 * (double) aiLevel1 + 0.200000002980232 * (double) num1));
      agentDrivenProperties.AiMoveEnemySideTimeValue = (float) (0.5 * (double) aiLevel1 - 2.5);
      agentDrivenProperties.AiMinimumDistanceToContinueFactor = (float) (2.0 + 0.300000011920929 * (3.0 - (double) aiLevel1));
      agentDrivenProperties.AiStandGroundTimerValue = (float) (0.5 * ((double) aiLevel1 - 1.0));
      agentDrivenProperties.AiStandGroundTimerMoveAlongValue = (float) (0.5 * (double) aiLevel1 - 1.0);
      agentDrivenProperties.AiHearingDistanceFactor = 1f + aiLevel1;
      agentDrivenProperties.AiChargeHorsebackTargetDistFactor = (float) (1.5 * (3.0 - (double) aiLevel1));
      agentDrivenProperties.AiWaitBeforeShootFactor = agent._propertyModifiers.resetAiWaitBeforeShootFactor ? 0.0f : (float) (1.0 - 0.5 * (double) aiLevel2);
      float num3 = 1f - aiLevel2;
      agentDrivenProperties.AiRangerLeadErrorMin = (float) (-(double) num3 * 0.349999994039536);
      agentDrivenProperties.AiRangerLeadErrorMax = num3 * 0.2f;
      agentDrivenProperties.AiRangerVerticalErrorMultiplier = num3 * 0.1f;
      agentDrivenProperties.AiRangerHorizontalErrorMultiplier = num3 * ((float) Math.PI / 90f);
      agentDrivenProperties.AIAttackOnDecideChance = MathF.Clamp((float) (0.230000004172325 * (double) this.CalculateAIAttackOnDecideMaxValue() * (3.0 - (double) agent.Defensiveness)), 0.05f, 1f);
      agentDrivenProperties.SetStat(DrivenProperty.UseRealisticBlocking, agent.Controller != Agent.ControllerType.Player ? 1f : 0.0f);
    }
  }
}
