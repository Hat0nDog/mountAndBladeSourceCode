// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MPPerkObject
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.Network.Gameplay.Perks.Conditions;

namespace TaleWorlds.MountAndBlade
{
  public class MPPerkObject : IReadOnlyPerkObject
  {
    private readonly MissionPeer _peer;
    private readonly MPConditionalEffect.ConditionalEffectContainer _conditionalEffects;
    private readonly MPPerkCondition.PerkEventFlags _perkEventFlags;
    private readonly string _name;
    private readonly string _description;
    private readonly List<MPPerkEffectBase> _effects;

    public TextObject Name => new TextObject(this._name);

    public TextObject Description => new TextObject(this._description);

    public bool HasBannerBearer { get; }

    public List<string> GameModes { get; }

    public int PerkListIndex { get; }

    public string IconId { get; }

    public string HeroIdleAnimOverride { get; }

    public string HeroMountIdleAnimOverride { get; }

    public string TroopIdleAnimOverride { get; }

    public string TroopMountIdleAnimOverride { get; }

    public MPPerkObject(
      MissionPeer peer,
      string name,
      string description,
      List<string> gameModes,
      int perkListIndex,
      string iconId,
      IEnumerable<MPConditionalEffect> conditionalEffects,
      IEnumerable<MPPerkEffectBase> effects,
      string heroIdleAnimOverride,
      string heroMountIdleAnimOverride,
      string troopIdleAnimOverride,
      string troopMountIdleAnimOverride)
    {
      this._peer = peer;
      this._name = name;
      this._description = description;
      this.GameModes = gameModes;
      this.PerkListIndex = perkListIndex;
      this.IconId = iconId;
      this._conditionalEffects = new MPConditionalEffect.ConditionalEffectContainer(conditionalEffects);
      this._effects = new List<MPPerkEffectBase>(effects);
      this.HeroIdleAnimOverride = heroIdleAnimOverride;
      this.HeroMountIdleAnimOverride = heroMountIdleAnimOverride;
      this.TroopIdleAnimOverride = troopIdleAnimOverride;
      this.TroopMountIdleAnimOverride = troopMountIdleAnimOverride;
      this._perkEventFlags = MPPerkCondition.PerkEventFlags.None;
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) conditionalEffect.Conditions)
          this._perkEventFlags |= condition.EventFlags;
      }
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) conditionalEffect.Conditions)
        {
          if (condition is BannerBearerCondition)
            this.HasBannerBearer = true;
        }
      }
    }

    private MPPerkObject(XmlNode node)
    {
      this._peer = (MissionPeer) null;
      this._conditionalEffects = new MPConditionalEffect.ConditionalEffectContainer();
      this._effects = new List<MPPerkEffectBase>();
      this._name = node.Attributes["name"].Value;
      this._description = node.Attributes["description"].Value;
      this.GameModes = new List<string>((IEnumerable<string>) node.Attributes["game_mode"].Value.Split(','));
      for (int index = 0; index < this.GameModes.Count; ++index)
        this.GameModes[index] = this.GameModes[index].Trim();
      this.IconId = node.Attributes["icon"].Value;
      this.PerkListIndex = 0;
      XmlNode attribute = (XmlNode) node.Attributes["perk_list"];
      if (attribute != null)
      {
        this.PerkListIndex = Convert.ToInt32(attribute.Value);
        this.PerkListIndex = this.PerkListIndex - 1;
      }
      foreach (XmlNode childNode in node.ChildNodes)
      {
        if (childNode.NodeType != XmlNodeType.Comment && childNode.NodeType != XmlNodeType.SignificantWhitespace)
        {
          if (childNode.Name == "ConditionalEffect")
            this._conditionalEffects.Add(new MPConditionalEffect(this.GameModes, childNode));
          else if (childNode.Name == "Effect")
            this._effects.Add((MPPerkEffectBase) MPPerkEffect.CreateFrom(childNode));
          else if (childNode.Name == "OnSpawnEffect")
            this._effects.Add((MPPerkEffectBase) MPOnSpawnPerkEffect.CreateFrom(childNode));
          else if (childNode.Name == "RandomOnSpawnEffect")
            this._effects.Add((MPPerkEffectBase) MPRandomOnSpawnPerkEffect.CreateFrom(childNode));
        }
      }
      this.HeroIdleAnimOverride = node.Attributes["hero_idle_anim"]?.Value;
      this.HeroMountIdleAnimOverride = node.Attributes["hero_mount_idle_anim"]?.Value;
      this.TroopIdleAnimOverride = node.Attributes["troop_idle_anim"]?.Value;
      this.TroopMountIdleAnimOverride = node.Attributes["troop_mount_idle_anim"]?.Value;
      this._perkEventFlags = MPPerkCondition.PerkEventFlags.None;
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) conditionalEffect.Conditions)
          this._perkEventFlags |= condition.EventFlags;
      }
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        foreach (MPPerkCondition condition in (IEnumerable<MPPerkCondition>) conditionalEffect.Conditions)
        {
          if (condition is BannerBearerCondition)
            this.HasBannerBearer = true;
        }
      }
    }

    public MPPerkObject Clone(MissionPeer peer) => new MPPerkObject(peer, this._name, this._description, this.GameModes, this.PerkListIndex, this.IconId, (IEnumerable<MPConditionalEffect>) this._conditionalEffects, (IEnumerable<MPPerkEffectBase>) this._effects, this.HeroIdleAnimOverride, this.HeroMountIdleAnimOverride, this.TroopIdleAnimOverride, this.TroopMountIdleAnimOverride);

    public void Reset() => this._conditionalEffects.ResetStates();

    private void OnEvent(MPPerkCondition.PerkEventFlags flags)
    {
      if ((flags & this._perkEventFlags) == MPPerkCondition.PerkEventFlags.None)
        return;
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if ((flags & conditionalEffect.EventFlags) != MPPerkCondition.PerkEventFlags.None)
          conditionalEffect.OnEvent(this._peer, this._conditionalEffects);
      }
    }

    private void OnEvent(Agent agent, MPPerkCondition.PerkEventFlags flags)
    {
      if (agent?.MissionPeer == null && agent != null)
      {
        MissionPeer agentMissionPeer = agent.OwningAgentMissionPeer;
      }
      if ((flags & this._perkEventFlags) == MPPerkCondition.PerkEventFlags.None)
        return;
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if ((flags & conditionalEffect.EventFlags) != MPPerkCondition.PerkEventFlags.None)
          conditionalEffect.OnEvent(agent, this._conditionalEffects);
      }
    }

    private void OnTick(int tickCount)
    {
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.IsTickRequired)
          conditionalEffect.OnTick(this._peer, tickCount);
      }
      foreach (MPPerkEffectBase effect in this._effects)
      {
        if (effect.IsTickRequired)
          effect.OnTick(this._peer, tickCount);
      }
    }

    private float GetDamage(
      Agent agent,
      WeaponComponentData attackerWeapon,
      DamageTypes damageType,
      bool isAlternativeAttack)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetDamage(attackerWeapon, damageType, isAlternativeAttack);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetDamage(attackerWeapon, damageType, isAlternativeAttack);
        }
      }
      return num;
    }

    private float GetMountDamage(
      Agent agent,
      WeaponComponentData attackerWeapon,
      DamageTypes damageType,
      bool isAlternativeAttack)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetMountDamage(attackerWeapon, damageType, isAlternativeAttack);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetMountDamage(attackerWeapon, damageType, isAlternativeAttack);
        }
      }
      return num;
    }

    private float GetDamageTaken(
      Agent agent,
      WeaponComponentData attackerWeapon,
      DamageTypes damageType)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetDamageTaken(attackerWeapon, damageType);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetDamageTaken(attackerWeapon, damageType);
        }
      }
      return num;
    }

    private float GetMountDamageTaken(
      Agent agent,
      WeaponComponentData attackerWeapon,
      DamageTypes damageType)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetMountDamageTaken(attackerWeapon, damageType);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetMountDamageTaken(attackerWeapon, damageType);
        }
      }
      return num;
    }

    private float GetSpeedBonusEffectiveness(Agent agent)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetSpeedBonusEffectiveness(agent);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetSpeedBonusEffectiveness(agent);
        }
      }
      return num;
    }

    private float GetShieldDamage(Agent attacker, Agent defender, bool isCorrectSideBlock)
    {
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetShieldDamage(isCorrectSideBlock);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(attacker))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetShieldDamage(isCorrectSideBlock);
        }
      }
      return num;
    }

    private float GetShieldDamageTaken(Agent attacker, Agent defender, bool isCorrectSideBlock)
    {
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetShieldDamageTaken(isCorrectSideBlock);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(defender))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetShieldDamageTaken(isCorrectSideBlock);
        }
      }
      return num;
    }

    private float GetRangedAccuracy(Agent agent)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetRangedAccuracy();
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetRangedAccuracy();
        }
      }
      return num;
    }

    private float GetThrowingWeaponSpeed(Agent agent, WeaponComponentData attackerWeapon)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetThrowingWeaponSpeed(attackerWeapon);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetThrowingWeaponSpeed(attackerWeapon);
        }
      }
      return num;
    }

    private float GetDamageInterruptionThreshold(Agent agent)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetDamageInterruptionThreshold();
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetDamageInterruptionThreshold();
        }
      }
      return num;
    }

    private float GetMountManeuver(Agent agent)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetMountManeuver();
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetMountManeuver();
        }
      }
      return num;
    }

    private float GetMountSpeed(Agent agent)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetMountSpeed();
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetMountSpeed();
        }
      }
      return num;
    }

    private float GetRangedHeadShotDamage(Agent agent)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetRangedHeadShotDamage();
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetRangedHeadShotDamage();
        }
      }
      return num;
    }

    public float GetTroopCountMultiplier()
    {
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
      {
        if (effect is IOnSpawnPerkEffect onSpawnPerkEffect1)
          num += onSpawnPerkEffect1.GetTroopCountMultiplier();
      }
      return num;
    }

    public float GetExtraTroopCount()
    {
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
      {
        if (effect is IOnSpawnPerkEffect onSpawnPerkEffect1)
          num += onSpawnPerkEffect1.GetExtraTroopCount();
      }
      return num;
    }

    public List<(EquipmentIndex, EquipmentElement)> GetAlternativeEquipments(
      bool isPlayer,
      List<(EquipmentIndex, EquipmentElement)> alternativeEquipments)
    {
      foreach (MPPerkEffectBase effect in this._effects)
      {
        if (effect is IOnSpawnPerkEffect onSpawnPerkEffect1)
          alternativeEquipments = onSpawnPerkEffect1.GetAlternativeEquipments(isPlayer, alternativeEquipments);
      }
      return alternativeEquipments;
    }

    private float GetDrivenPropertyBonus(
      Agent agent,
      DrivenProperty drivenProperty,
      float baseValue)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num1 = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num1 += effect.GetDrivenPropertyBonus(drivenProperty, baseValue);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        float num2 = 0.0f;
        foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
          num2 += effect.GetDrivenPropertyBonus(drivenProperty, baseValue);
        if ((double) num2 != 0.0 && conditionalEffect.Check(agent))
          num1 += num2;
      }
      return num1;
    }

    public float GetDrivenPropertyBonusOnSpawn(
      bool isPlayer,
      DrivenProperty drivenProperty,
      float baseValue)
    {
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
      {
        if (effect is IOnSpawnPerkEffect onSpawnPerkEffect1)
          num += onSpawnPerkEffect1.GetDrivenPropertyBonusOnSpawn(isPlayer, drivenProperty, baseValue);
      }
      return num;
    }

    public float GetHitpoints(bool isPlayer)
    {
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
      {
        if (effect is IOnSpawnPerkEffect onSpawnPerkEffect1)
          num += onSpawnPerkEffect1.GetHitpoints(isPlayer);
      }
      return num;
    }

    private float GetEncumbrance(Agent agent, bool isOnBody)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      float num = 0.0f;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetEncumbrance(isOnBody);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetEncumbrance(isOnBody);
        }
      }
      return num;
    }

    private int GetGoldOnKill(Agent agent, float attackerValue, float victimValue)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      int num = 0;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetGoldOnKill(attackerValue, victimValue);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetGoldOnKill(attackerValue, victimValue);
        }
      }
      return num;
    }

    private int GetGoldOnAssist(Agent agent)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      int num = 0;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetGoldOnAssist();
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetGoldOnAssist();
        }
      }
      return num;
    }

    private int GetRewardedGoldOnAssist(Agent agent)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      int num = 0;
      foreach (MPPerkEffectBase effect in this._effects)
        num += effect.GetRewardedGoldOnAssist();
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            num += effect.GetRewardedGoldOnAssist();
        }
      }
      return num;
    }

    private bool GetIsTeamRewardedOnDeath(Agent agent)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      foreach (MPPerkEffectBase effect in this._effects)
      {
        if (effect.GetIsTeamRewardedOnDeath())
          return true;
      }
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
          {
            if (effect.GetIsTeamRewardedOnDeath())
              return true;
          }
        }
      }
      return false;
    }

    private void CalculateRewardedGoldOnDeath(Agent agent, List<(MissionPeer, int)> teamMembers)
    {
      agent = agent ?? this._peer?.ControlledAgent;
      teamMembers.Shuffle<(MissionPeer, int)>();
      foreach (MPPerkEffectBase effect in this._effects)
        effect.CalculateRewardedGoldOnDeath(agent, teamMembers);
      foreach (MPConditionalEffect conditionalEffect in (List<MPConditionalEffect>) this._conditionalEffects)
      {
        if (conditionalEffect.Check(agent))
        {
          foreach (MPPerkEffectBase effect in (IEnumerable<MPPerkEffectBase>) conditionalEffect.Effects)
            effect.CalculateRewardedGoldOnDeath(agent, teamMembers);
        }
      }
    }

    public static int GetTroopCount(
      MultiplayerClassDivisions.MPHeroClass heroClass,
      MPPerkObject.MPOnSpawnPerkHandler onSpawnPerkHandler)
    {
      float num = (float) (int) Math.Ceiling((double) MultiplayerOptions.OptionType.NumberOfBotsPerFormation.GetIntValue() * (double) heroClass.TroopMultiplier);
      if (onSpawnPerkHandler != null)
        num = num * (1f + onSpawnPerkHandler.GetTroopCountMultiplier()) + onSpawnPerkHandler.GetExtraTroopCount();
      return Math.Max((int) Math.Ceiling((double) num), 1);
    }

    public static IReadOnlyPerkObject Deserialize(XmlNode node) => (IReadOnlyPerkObject) new MPPerkObject(node);

    public static MPPerkObject.MPPerkHandler GetPerkHandler(Agent agent)
    {
      IReadOnlyList<MPPerkObject> mpPerkObjectList = agent?.MissionPeer?.SelectedPerks ?? agent?.OwningAgentMissionPeer?.SelectedPerks;
      return mpPerkObjectList != null && mpPerkObjectList.Count > 0 && !agent.IsMount ? (MPPerkObject.MPPerkHandler) new MPPerkObject.MPPerkHandlerInstance(agent) : (MPPerkObject.MPPerkHandler) null;
    }

    public static MPPerkObject.MPPerkHandler GetPerkHandler(MissionPeer peer)
    {
      IReadOnlyList<MPPerkObject> mpPerkObjectList = peer?.SelectedPerks ?? peer?.SelectedPerks;
      return mpPerkObjectList != null && mpPerkObjectList.Count > 0 ? (MPPerkObject.MPPerkHandler) new MPPerkObject.MPPerkHandlerInstance(peer) : (MPPerkObject.MPPerkHandler) null;
    }

    public static MPPerkObject.MPCombatPerkHandler GetCombatPerkHandler(
      Agent attacker,
      Agent defender)
    {
      Agent agent1 = attacker == null || !attacker.IsMount ? attacker : attacker.RiderAgent;
      IReadOnlyList<MPPerkObject> mpPerkObjectList1 = agent1?.MissionPeer?.SelectedPerks ?? agent1?.OwningAgentMissionPeer?.SelectedPerks;
      Agent agent2 = defender == null || !defender.IsMount ? defender : defender.RiderAgent;
      IReadOnlyList<MPPerkObject> mpPerkObjectList2 = agent2?.MissionPeer?.SelectedPerks ?? agent2?.OwningAgentMissionPeer?.SelectedPerks;
      return attacker != defender && (mpPerkObjectList1 != null && mpPerkObjectList1.Count > 0 || mpPerkObjectList2 != null && mpPerkObjectList2.Count > 0) ? (MPPerkObject.MPCombatPerkHandler) new MPPerkObject.MPCombatPerkHandlerInstance(attacker, defender) : (MPPerkObject.MPCombatPerkHandler) null;
    }

    public static MPPerkObject.MPOnSpawnPerkHandler GetOnSpawnPerkHandler(
      MissionPeer peer)
    {
      return (peer?.SelectedPerks ?? peer?.SelectedPerks) != null ? (MPPerkObject.MPOnSpawnPerkHandler) new MPPerkObject.MPOnSpawnPerkHandlerInstance(peer) : (MPPerkObject.MPOnSpawnPerkHandler) null;
    }

    public static MPPerkObject.MPOnSpawnPerkHandler GetOnSpawnPerkHandler(
      IEnumerable<IReadOnlyPerkObject> perks)
    {
      return perks != null ? (MPPerkObject.MPOnSpawnPerkHandler) new MPPerkObject.MPOnSpawnPerkHandlerInstance(perks) : (MPPerkObject.MPOnSpawnPerkHandler) null;
    }

    public static void RaiseEventForAllPeers(MPPerkCondition.PerkEventFlags flags)
    {
      if (!GameNetwork.IsServerOrRecorder)
        return;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        MPPerkObject.GetPerkHandler(networkPeer.GetComponent<MissionPeer>())?.OnEvent(flags);
    }

    public static void RaiseEventForAllPeersOnTeam(Team side, MPPerkCondition.PerkEventFlags flags)
    {
      if (!GameNetwork.IsServerOrRecorder)
        return;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (component != null && component.Team == side)
          MPPerkObject.GetPerkHandler(component)?.OnEvent(flags);
      }
    }

    public static void TickAllPeerPerks(int tickCount)
    {
      if (!GameNetwork.IsServerOrRecorder)
        return;
      foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
      {
        MissionPeer component = networkPeer.GetComponent<MissionPeer>();
        if (component != null && component.Team != null && component.Team.Side != BattleSideEnum.None)
          MPPerkObject.GetPerkHandler(component)?.OnTick(tickCount);
      }
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("raise_event", "mp_perks")]
    public static string RaiseEventForAllPeersCommand(List<string> strings)
    {
      if (!GameNetwork.IsServerOrRecorder)
        return "Can't run this command on clients";
      MPPerkCondition.PerkEventFlags flags = MPPerkCondition.PerkEventFlags.None;
      foreach (string str in strings)
      {
        MPPerkCondition.PerkEventFlags result;
        if (Enum.TryParse<MPPerkCondition.PerkEventFlags>(str, true, out result))
          flags |= result;
      }
      MPPerkObject.RaiseEventForAllPeers(flags);
      return "Raised event with flags " + (object) flags;
    }

    [CommandLineFunctionality.CommandLineArgumentFunction("tick_perks", "mp_perks")]
    public static string TickAllPeerPerksCommand(List<string> strings)
    {
      if (!GameNetwork.IsServerOrRecorder)
        return "Can't run this command on clients";
      int result;
      if (strings.Count == 0 || !int.TryParse(strings[0], out result))
        result = 1;
      MPPerkObject.TickAllPeerPerks(result);
      return "Peer perks on tick with tick count " + (object) result;
    }

    private class MPOnSpawnPerkHandlerInstance : MPPerkObject.MPOnSpawnPerkHandler
    {
      public MPOnSpawnPerkHandlerInstance(IEnumerable<IReadOnlyPerkObject> perks)
        : base(perks)
      {
      }

      public MPOnSpawnPerkHandlerInstance(MissionPeer peer)
        : base(peer)
      {
      }
    }

    private class MPPerkHandlerInstance : MPPerkObject.MPPerkHandler
    {
      public MPPerkHandlerInstance(Agent agent)
        : base(agent)
      {
      }

      public MPPerkHandlerInstance(MissionPeer peer)
        : base(peer)
      {
      }
    }

    private class MPCombatPerkHandlerInstance : MPPerkObject.MPCombatPerkHandler
    {
      public MPCombatPerkHandlerInstance(Agent attacker, Agent defender)
        : base(attacker, defender)
      {
      }
    }

    public class MPOnSpawnPerkHandler
    {
      private IEnumerable<IReadOnlyPerkObject> _perks;

      protected MPOnSpawnPerkHandler(IEnumerable<IReadOnlyPerkObject> perks) => this._perks = perks;

      protected MPOnSpawnPerkHandler(MissionPeer peer) => this._perks = (IEnumerable<IReadOnlyPerkObject>) peer.SelectedPerks;

      public float GetTroopCountMultiplier()
      {
        float num = 0.0f;
        foreach (IReadOnlyPerkObject perk in this._perks)
          num += perk.GetTroopCountMultiplier();
        return num;
      }

      public float GetExtraTroopCount()
      {
        float num = 0.0f;
        foreach (IReadOnlyPerkObject perk in this._perks)
          num += perk.GetExtraTroopCount();
        return num;
      }

      public IEnumerable<(EquipmentIndex, EquipmentElement)> GetAlternativeEquipments(
        bool isPlayer)
      {
        List<(EquipmentIndex, EquipmentElement)> alternativeEquipments = (List<(EquipmentIndex, EquipmentElement)>) null;
        foreach (IReadOnlyPerkObject perk in this._perks)
          alternativeEquipments = perk.GetAlternativeEquipments(isPlayer, alternativeEquipments);
        return (IEnumerable<(EquipmentIndex, EquipmentElement)>) alternativeEquipments;
      }

      public float GetDrivenPropertyBonusOnSpawn(
        bool isPlayer,
        DrivenProperty drivenProperty,
        float baseValue)
      {
        float num = 0.0f;
        foreach (IReadOnlyPerkObject perk in this._perks)
          num += perk.GetDrivenPropertyBonusOnSpawn(isPlayer, drivenProperty, baseValue);
        return num;
      }

      public float GetHitpoints(bool isPlayer)
      {
        float num = 0.0f;
        foreach (IReadOnlyPerkObject perk in this._perks)
          num += perk.GetHitpoints(isPlayer);
        return num;
      }
    }

    public class MPPerkHandler
    {
      private readonly Agent _agent;
      private readonly IReadOnlyList<MPPerkObject> _perks;

      protected MPPerkHandler(Agent agent)
      {
        this._agent = agent;
        this._perks = (this._agent?.MissionPeer?.SelectedPerks ?? this._agent?.OwningAgentMissionPeer?.SelectedPerks) ?? (IReadOnlyList<MPPerkObject>) new List<MPPerkObject>();
      }

      protected MPPerkHandler(MissionPeer peer)
      {
        this._agent = peer?.ControlledAgent;
        this._perks = peer?.SelectedPerks ?? (IReadOnlyList<MPPerkObject>) new List<MPPerkObject>();
      }

      public void OnEvent(MPPerkCondition.PerkEventFlags flags)
      {
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          perk.OnEvent(flags);
      }

      public void OnEvent(Agent agent, MPPerkCondition.PerkEventFlags flags)
      {
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          perk.OnEvent(agent, flags);
      }

      public void OnTick(int tickCount)
      {
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          perk.OnTick(tickCount);
      }

      public float GetDrivenPropertyBonus(DrivenProperty drivenProperty, float baseValue)
      {
        float num = 0.0f;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetDrivenPropertyBonus(this._agent, drivenProperty, baseValue);
        return num;
      }

      public float GetRangedAccuracy()
      {
        float num = 0.0f;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetRangedAccuracy(this._agent);
        return num;
      }

      public float GetThrowingWeaponSpeed(WeaponComponentData attackerWeapon)
      {
        float num = 0.0f;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetThrowingWeaponSpeed(this._agent, attackerWeapon);
        return num;
      }

      public float GetDamageInterruptionThreshold()
      {
        float num = 0.0f;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetDamageInterruptionThreshold(this._agent);
        return num;
      }

      public float GetMountManeuver()
      {
        float num = 0.0f;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetMountManeuver(this._agent);
        return num;
      }

      public float GetMountSpeed()
      {
        float num = 0.0f;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetMountSpeed(this._agent);
        return num;
      }

      public int GetGoldOnKill(float attackerValue, float victimValue)
      {
        int num = 0;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetGoldOnKill(this._agent, attackerValue, victimValue);
        return num;
      }

      public int GetGoldOnAssist()
      {
        int num = 0;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetGoldOnAssist(this._agent);
        return num;
      }

      public int GetRewardedGoldOnAssist()
      {
        int num = 0;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetRewardedGoldOnAssist(this._agent);
        return num;
      }

      public bool GetIsTeamRewardedOnDeath()
      {
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
        {
          if (perk.GetIsTeamRewardedOnDeath(this._agent))
            return true;
        }
        return false;
      }

      public IEnumerable<(MissionPeer, int)> GetTeamGoldRewardsOnDeath()
      {
        if (!this.GetIsTeamRewardedOnDeath())
          return (IEnumerable<(MissionPeer, int)>) null;
        MissionPeer missionPeer = this._agent?.MissionPeer ?? this._agent?.OwningAgentMissionPeer;
        List<(MissionPeer, int)> teamMembers = new List<(MissionPeer, int)>();
        foreach (NetworkCommunicator networkPeer in GameNetwork.NetworkPeers)
        {
          MissionPeer component = networkPeer.GetComponent<MissionPeer>();
          if (component != missionPeer && component.Team == missionPeer.Team)
            teamMembers.Add((component, 0));
        }
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          perk.CalculateRewardedGoldOnDeath(this._agent, teamMembers);
        return (IEnumerable<(MissionPeer, int)>) teamMembers;
      }

      public float GetEncumbrance(bool isOnBody)
      {
        float num = 0.0f;
        foreach (MPPerkObject perk in (IEnumerable<MPPerkObject>) this._perks)
          num += perk.GetEncumbrance(this._agent, isOnBody);
        return num;
      }
    }

    public class MPCombatPerkHandler
    {
      private readonly Agent _attacker;
      private readonly Agent _defender;
      private readonly IReadOnlyList<MPPerkObject> _attackerPerks;
      private readonly IReadOnlyList<MPPerkObject> _defenderPerks;

      protected MPCombatPerkHandler(Agent attacker, Agent defender)
      {
        this._attacker = attacker;
        this._defender = defender;
        attacker = attacker == null || !attacker.IsMount ? attacker : attacker.RiderAgent;
        defender = defender == null || !defender.IsMount ? defender : defender.RiderAgent;
        this._attackerPerks = attacker?.MissionPeer?.SelectedPerks ?? attacker?.OwningAgentMissionPeer?.SelectedPerks ?? (IReadOnlyList<MPPerkObject>) new List<MPPerkObject>();
        this._defenderPerks = defender?.MissionPeer?.SelectedPerks ?? defender?.OwningAgentMissionPeer?.SelectedPerks ?? (IReadOnlyList<MPPerkObject>) new List<MPPerkObject>();
      }

      public float GetDamage(
        WeaponComponentData attackerWeapon,
        DamageTypes damageType,
        bool isAlternativeAttack)
      {
        float num = 0.0f;
        if (this._attackerPerks.Count > 0 && this._defender != null)
        {
          if (this._defender.IsMount)
          {
            foreach (MPPerkObject attackerPerk in (IEnumerable<MPPerkObject>) this._attackerPerks)
              num += attackerPerk.GetMountDamage(this._attacker, attackerWeapon, damageType, isAlternativeAttack);
          }
          foreach (MPPerkObject attackerPerk in (IEnumerable<MPPerkObject>) this._attackerPerks)
            num += attackerPerk.GetDamage(this._attacker, attackerWeapon, damageType, isAlternativeAttack);
        }
        return num;
      }

      public float GetDamageTaken(WeaponComponentData attackerWeapon, DamageTypes damageType)
      {
        float num = 0.0f;
        if (this._defenderPerks.Count > 0)
        {
          if (this._defender.IsMount)
          {
            foreach (MPPerkObject defenderPerk in (IEnumerable<MPPerkObject>) this._defenderPerks)
              num += defenderPerk.GetMountDamageTaken(this._defender, attackerWeapon, damageType);
          }
          else
          {
            foreach (MPPerkObject defenderPerk in (IEnumerable<MPPerkObject>) this._defenderPerks)
              num += defenderPerk.GetDamageTaken(this._defender, attackerWeapon, damageType);
          }
        }
        return num;
      }

      public float GetSpeedBonusEffectiveness()
      {
        float num = 0.0f;
        foreach (MPPerkObject attackerPerk in (IEnumerable<MPPerkObject>) this._attackerPerks)
          num += attackerPerk.GetSpeedBonusEffectiveness(this._attacker);
        return num;
      }

      public float GetShieldDamage(bool isCorrectSideBlock)
      {
        float num = 0.0f;
        if (this._defender != null)
        {
          foreach (MPPerkObject attackerPerk in (IEnumerable<MPPerkObject>) this._attackerPerks)
            num += attackerPerk.GetShieldDamage(this._attacker, this._defender, isCorrectSideBlock);
        }
        return num;
      }

      public float GetShieldDamageTaken(bool isCorrectSideBlock)
      {
        float num = 0.0f;
        foreach (MPPerkObject defenderPerk in (IEnumerable<MPPerkObject>) this._defenderPerks)
          num += defenderPerk.GetShieldDamageTaken(this._attacker, this._defender, isCorrectSideBlock);
        return num;
      }

      public float GetRangedHeadShotDamage()
      {
        float num = 0.0f;
        if (this._attacker != null)
        {
          foreach (MPPerkObject attackerPerk in (IEnumerable<MPPerkObject>) this._attackerPerks)
            num += attackerPerk.GetRangedHeadShotDamage(this._attacker);
        }
        return num;
      }
    }
  }
}
