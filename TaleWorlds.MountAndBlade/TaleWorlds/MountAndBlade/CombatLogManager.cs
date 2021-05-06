// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CombatLogManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public static class CombatLogManager
  {
    public static event CombatLogManager.OnPrintCombatLogHandler OnGenerateCombatLog;

    public static void PrintDebugLogForInfo(
      Agent attackerAgent,
      Agent victimAgent,
      DamageTypes damageType,
      int speedBonus,
      int armorAmount,
      int inflictedDamage,
      int absorbedByArmor,
      sbyte collisionBone,
      float lostHpPercentage)
    {
      TextObject message = TextObject.Empty;
      CombatLogColor logColor = CombatLogColor.White;
      bool isMine = attackerAgent.IsMine;
      int num = victimAgent.IsMine ? 1 : 0;
      GameTexts.SetVariable("AMOUNT", inflictedDamage);
      GameTexts.SetVariable("DAMAGE_TYPE", damageType.ToString().ToLower());
      GameTexts.SetVariable("LOST_HP_PERCENTAGE", lostHpPercentage);
      if (num != 0)
      {
        GameTexts.SetVariable("ATTACKER_NAME", attackerAgent.Name);
        message = GameTexts.FindText("combat_log_player_attacked");
        logColor = CombatLogColor.Red;
      }
      else if (isMine)
      {
        GameTexts.SetVariable("VICTIM_NAME", victimAgent.Name);
        message = GameTexts.FindText("combat_log_player_attacker");
        logColor = CombatLogColor.Green;
      }
      CombatLogManager.Print(message, logColor);
      MBStringBuilder mbStringBuilder = new MBStringBuilder();
      mbStringBuilder.Initialize(callerMemberName: nameof (PrintDebugLogForInfo));
      if (armorAmount > 0)
      {
        GameTexts.SetVariable("ABSORBED_AMOUNT", absorbedByArmor);
        GameTexts.SetVariable("ARMOR_AMOUNT", armorAmount);
        mbStringBuilder.AppendLine<string>(GameTexts.FindText("combat_log_damage_absorbed").ToString());
      }
      if (victimAgent.IsHuman)
      {
        Agent.AgentBoneMapArray boneMappingArray = victimAgent.BoneMappingArray;
        for (int index = 0; index < boneMappingArray.Length; ++index)
        {
          HumanBone i = (HumanBone) index;
          if ((int) collisionBone == (int) boneMappingArray[i])
          {
            GameTexts.SetVariable("BONE", i.ToString());
            mbStringBuilder.AppendLine<string>(GameTexts.FindText("combat_log_hit_bone").ToString());
            break;
          }
        }
      }
      if (speedBonus != 0)
      {
        GameTexts.SetVariable("SPEED_BONUS", speedBonus);
        mbStringBuilder.AppendLine<string>(GameTexts.FindText("combat_log_speed_bonus").ToString());
      }
      CombatLogManager.Print(new TextObject(mbStringBuilder.ToStringAndRelease()));
    }

    private static void Print(TextObject message, CombatLogColor logColor = CombatLogColor.White)
    {
      Debug.DebugColor color = (Debug.DebugColor) logColor;
      Debug.Print(message.ToString(), color: color, debugFilter: 562949953421312UL);
    }

    public static void GenerateCombatLog(CombatLogData logData)
    {
      CombatLogManager.OnPrintCombatLogHandler generateCombatLog = CombatLogManager.OnGenerateCombatLog;
      if (generateCombatLog != null)
        generateCombatLog(logData);
      foreach ((string, uint) tuple in logData.GetLogString())
        InformationManager.DisplayMessage(new InformationMessage(tuple.Item1, Color.FromUint(tuple.Item2), "Combat"));
    }

    public delegate void OnPrintCombatLogHandler(CombatLogData logData);
  }
}
