// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CombatLogData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public struct CombatLogData
  {
    private const string DetailTagStart = "<Detail>";
    private const string DetailTagEnd = "</Detail>";
    private const uint DamageReceivedColor = 4292917946;
    private const uint DamageDealedColor = 4210351871;
    private static List<(string, uint)> _logStringCache = new List<(string, uint)>();
    public readonly bool IsVictimAgentSameAsAttackerAgent;
    public readonly bool IsVictimRiderAgentSameAsAttackerAgent;
    public readonly bool IsAttackerAgentHuman;
    public readonly bool IsAttackerAgentMine;
    public readonly bool DoesAttackerAgentHaveRiderAgent;
    public readonly bool IsAttackerAgentRiderAgentMine;
    public readonly bool IsAttackerAgentMount;
    public readonly bool IsVictimAgentHuman;
    public readonly bool IsVictimAgentMine;
    public readonly bool DoesVictimAgentHaveRiderAgent;
    public readonly bool IsVictimAgentRiderAgentMine;
    public readonly bool IsVictimAgentMount;
    public bool IsVictimEntity;
    public DamageTypes DamageType;
    public bool CrushedThrough;
    public bool Chamber;
    public bool IsRangedAttack;
    public bool IsFriendlyFire;
    public bool IsFatalDamage;
    public BoneBodyPartType BodyPartHit;
    public string VictimAgentName;
    public float HitSpeed;
    public int InflictedDamage;
    public int AbsorbedDamage;
    public int ModifiedDamage;
    public float Distance;

    private bool IsValidForPlayer
    {
      get
      {
        if (!this.IsImportant)
          return false;
        return this.IsAttackerPlayer || this.IsVictimPlayer;
      }
    }

    private bool IsImportant => this.TotalDamage > 0 || this.CrushedThrough || this.Chamber;

    private bool IsAttackerPlayer
    {
      get
      {
        if (this.IsAttackerAgentHuman)
          return this.IsAttackerAgentMine;
        return this.DoesAttackerAgentHaveRiderAgent && this.IsAttackerAgentRiderAgentMine;
      }
    }

    private bool IsVictimPlayer
    {
      get
      {
        if (this.IsVictimAgentHuman)
          return this.IsVictimAgentMine;
        return this.DoesVictimAgentHaveRiderAgent && this.IsVictimAgentRiderAgentMine;
      }
    }

    private bool IsAttackerMount => this.IsAttackerAgentMount;

    private bool IsVictimMount => this.IsVictimAgentMount;

    public int TotalDamage => this.InflictedDamage + this.ModifiedDamage;

    public float AttackProgress { get; internal set; }

    public List<(string, uint)> GetLogString()
    {
      CombatLogData._logStringCache.Clear();
      if (this.IsValidForPlayer && (double) ManagedOptions.GetConfig(ManagedOptions.ManagedOptionsType.ReportDamage) > 0.0)
      {
        if (this.IsRangedAttack && this.IsAttackerPlayer && this.BodyPartHit == BoneBodyPartType.Head)
          CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("ui_head_shot").ToString(), 4289612505U));
        if (this.IsFriendlyFire)
          CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("combat_log_friendly_fire").ToString(), 4289612505U));
        if (this.CrushedThrough && !this.IsFriendlyFire)
        {
          if (this.IsAttackerPlayer)
            CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("combat_log_crushed_through_attacker").ToString(), 4289612505U));
          else
            CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("combat_log_crushed_through_victim").ToString(), 4289612505U));
        }
        if (this.Chamber)
          CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(GameTexts.FindText("combat_log_chamber_blocked").ToString(), 4289612505U));
        uint num1 = 4290563554;
        GameTexts.SetVariable("DAMAGE", this.TotalDamage);
        int num2 = (int) this.DamageType;
        GameTexts.SetVariable("DAMAGE_TYPE", GameTexts.FindText("combat_log_damage_type", num2.ToString()));
        MBStringBuilder mbStringBuilder = new MBStringBuilder();
        mbStringBuilder.Initialize(callerMemberName: nameof (GetLogString));
        if (this.IsVictimAgentSameAsAttackerAgent)
        {
          mbStringBuilder.Append<TextObject>(GameTexts.FindText("ui_received_number_damage_fall"));
          num1 = 4292917946U;
        }
        else if (this.IsVictimMount)
        {
          if (this.IsVictimRiderAgentSameAsAttackerAgent)
          {
            mbStringBuilder.Append<TextObject>(GameTexts.FindText("ui_received_number_damage_fall_to_horse"));
            num1 = 4292917946U;
          }
          else
          {
            mbStringBuilder.Append<TextObject>(GameTexts.FindText(this.IsAttackerPlayer ? "ui_delivered_number_damage_to_horse" : "ui_horse_received_number_damage"));
            num1 = this.IsAttackerPlayer ? 4210351871U : 4292917946U;
          }
        }
        else if (this.IsVictimEntity)
          mbStringBuilder.Append<TextObject>(GameTexts.FindText("ui_delivered_number_damage_to_entity"));
        else if (this.IsAttackerMount)
        {
          mbStringBuilder.Append<TextObject>(GameTexts.FindText(this.IsAttackerPlayer ? "ui_horse_charged_for_number_damage" : "ui_received_number_damage"));
          num1 = this.IsAttackerPlayer ? 4210351871U : 4292917946U;
        }
        else if (this.TotalDamage > 0)
        {
          mbStringBuilder.Append<TextObject>(GameTexts.FindText(this.IsAttackerPlayer ? "ui_delivered_number_damage" : "ui_received_number_damage"));
          num1 = this.IsAttackerPlayer ? 4210351871U : 4292917946U;
        }
        if (this.BodyPartHit != BoneBodyPartType.None)
        {
          num2 = (int) this.BodyPartHit;
          GameTexts.SetVariable("BODY_PART", GameTexts.FindText("body_part_type", num2.ToString()));
          mbStringBuilder.Append<string>("<Detail>");
          mbStringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_body_part"));
          mbStringBuilder.Append<string>("</Detail>");
        }
        if ((double) this.HitSpeed > 9.99999974737875E-06)
        {
          GameTexts.SetVariable("SPEED", (float) Math.Round((double) this.HitSpeed, 2));
          mbStringBuilder.Append<string>("<Detail>");
          mbStringBuilder.Append<TextObject>(this.IsRangedAttack ? GameTexts.FindText("combat_log_detail_missile_speed") : GameTexts.FindText("combat_log_detail_move_speed"));
          mbStringBuilder.Append<string>("</Detail>");
        }
        if (this.IsRangedAttack)
        {
          GameTexts.SetVariable("DISTANCE", (float) Math.Round((double) this.Distance, 1));
          mbStringBuilder.Append<string>("<Detail>");
          mbStringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_distance"));
          mbStringBuilder.Append<string>("</Detail>");
        }
        if (this.AbsorbedDamage > 0)
        {
          GameTexts.SetVariable("ABSORBED_DAMAGE", this.AbsorbedDamage);
          mbStringBuilder.Append<string>("<Detail>");
          mbStringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_absorbed_damage"));
          mbStringBuilder.Append<string>("</Detail>");
        }
        if (this.ModifiedDamage != 0)
        {
          GameTexts.SetVariable("MODIFIED_DAMAGE", Math.Abs(this.ModifiedDamage));
          mbStringBuilder.Append<string>("<Detail>");
          if (this.ModifiedDamage > 0)
            mbStringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_extra_damage"));
          else if (this.ModifiedDamage < 0)
            mbStringBuilder.Append<TextObject>(GameTexts.FindText("combat_log_detail_reduced_damage"));
          mbStringBuilder.Append<string>("</Detail>");
        }
        CombatLogData._logStringCache.Add(ValueTuple.Create<string, uint>(mbStringBuilder.ToStringAndRelease(), num1));
      }
      return CombatLogData._logStringCache;
    }

    public CombatLogData(
      bool isVictimAgentSameAsAttackerAgent,
      bool isAttackerAgentHuman,
      bool isAttackerAgentMine,
      bool doesAttackerAgentHaveRiderAgent,
      bool isAttackerAgentRiderAgentMine,
      bool isAttackerAgentMount,
      bool isVictimAgentHuman,
      bool isVictimAgentMine,
      bool isVictimAgentDead,
      bool doesVictimAgentHaveRiderAgent,
      bool isVictimAgentRiderAgentIsMine,
      bool isVictimAgentMount,
      bool isVictimEntity,
      bool isVictimRiderAgentSameAsAttackerAgent,
      bool crushedThrough,
      bool chamber,
      float distance)
    {
      this.IsVictimAgentSameAsAttackerAgent = isVictimAgentSameAsAttackerAgent;
      this.IsAttackerAgentHuman = isAttackerAgentHuman;
      this.IsAttackerAgentMine = isAttackerAgentMine;
      this.DoesAttackerAgentHaveRiderAgent = doesAttackerAgentHaveRiderAgent;
      this.IsAttackerAgentRiderAgentMine = isAttackerAgentRiderAgentMine;
      this.IsAttackerAgentMount = isAttackerAgentMount;
      this.IsVictimAgentHuman = isVictimAgentHuman;
      this.IsVictimAgentMine = isVictimAgentMine;
      this.DoesVictimAgentHaveRiderAgent = doesVictimAgentHaveRiderAgent;
      this.IsVictimAgentRiderAgentMine = isVictimAgentRiderAgentIsMine;
      this.IsVictimAgentMount = isVictimAgentMount;
      this.IsVictimEntity = isVictimEntity;
      this.IsVictimRiderAgentSameAsAttackerAgent = isVictimRiderAgentSameAsAttackerAgent;
      this.IsFatalDamage = isVictimAgentDead;
      this.DamageType = DamageTypes.Blunt;
      this.CrushedThrough = crushedThrough;
      this.Chamber = chamber;
      this.IsRangedAttack = false;
      this.IsFriendlyFire = false;
      this.VictimAgentName = (string) null;
      this.HitSpeed = 0.0f;
      this.InflictedDamage = 0;
      this.AbsorbedDamage = 0;
      this.ModifiedDamage = 0;
      this.AttackProgress = 0.0f;
      this.BodyPartHit = BoneBodyPartType.None;
      this.Distance = distance;
    }

    public void SetVictimAgent(Agent victimAgent) => this.VictimAgentName = victimAgent?.Name;
  }
}
