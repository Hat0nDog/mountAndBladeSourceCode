// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.HyperlinkTexts
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
  public static class HyperlinkTexts
  {
    public const string GoldIcon = "{=!}<img src=\"Icons\\Coin@2x\" extend=\"8\">";
    public const string MoraleIcon = "{=!}<img src=\"Icons\\Morale@2x\" extend=\"8\">";
    public const string InfluenceIcon = "{=!}<img src=\"Icons\\Influence@2x\" extend=\"7\">";
    public const string IssueAvailableIcon = "{=!}<img src=\"Icons\\icon_issue_available_square\" extend=\"4\">";
    public const string IssueActiveIcon = "{=!}<img src=\"Icons\\icon_issue_active_square\" extend=\"4\">";
    public const string QuestAvailableIcon = "{=!}<img src=\"Icons\\icon_quest_available\" extend=\"4\">";
    public const string QuestActiveIcon = "{=!}<img src=\"Icons\\icon_quest_active\" extend=\"4\">";
    public const string InPrisonIcon = "{=!}<img src=\"Clan\\Status\\icon_inprison\">";
    public const string ChildIcon = "{=!}<img src=\"Clan\\Status\\icon_ischild\">";
    public const string PregnantIcon = "{=!}<img src=\"Clan\\Status\\icon_pregnant\">";
    public const string IllIcon = "{=!}<img src=\"Clan\\Status\\icon_terminallyill\">";
    public const string HeirIcon = "{=!}<img src=\"Clan\\Status\\icon_heir\">";
    public const string UnreadIcon = "{=!}<img src=\"MapMenuUnread2x\">";
    public const string UnselectedPerkIcon = "{=!}<img src=\"CharacterDeveloper\\UnselectedPerksIcon\">";

    public static TextObject GetSettlementHyperlinkText(
      string link,
      TextObject settlementName)
    {
      TextObject textObject = new TextObject("{=!}<a style=\"Link.Settlement\" href=\"event:{LINK}\"><b>{SETTLEMENT_NAME}</b></a>");
      textObject.SetTextVariable("LINK", link);
      textObject.SetTextVariable("SETTLEMENT_NAME", settlementName);
      return textObject;
    }

    public static TextObject GetKingdomHyperlinkText(string link, TextObject kingdomName)
    {
      TextObject textObject = new TextObject("{=!}<a style=\"Link.Kingdom\" href=\"event:{LINK}\"><b>{KINGDOM_NAME}</b></a>");
      textObject.SetTextVariable("LINK", link);
      textObject.SetTextVariable("KINGDOM_NAME", kingdomName);
      return textObject;
    }

    public static TextObject GetHeroHyperlinkText(string link, TextObject heroName)
    {
      TextObject textObject = new TextObject("{=!}<a style=\"Link.Hero\" href=\"event:{LINK}\"><b>{HERO_NAME}</b></a>");
      textObject.SetTextVariable("LINK", link);
      textObject.SetTextVariable("HERO_NAME", heroName);
      return textObject;
    }

    public static TextObject GetConceptHyperlinkText(string link, TextObject conceptName)
    {
      TextObject textObject = new TextObject("{=!}<a style=\"Link.Concept\" href=\"event:{LINK}\"><b>{CONCEPT_NAME}</b></a>");
      textObject.SetTextVariable("LINK", link);
      textObject.SetTextVariable("CONCEPT_NAME", conceptName);
      return textObject;
    }

    public static TextObject GetClanHyperlinkText(string link, TextObject clanName)
    {
      TextObject textObject = new TextObject("{=!}<a style=\"Link.Clan\" href=\"event:{LINK}\"><b>{CLAN_NAME}</b></a>");
      textObject.SetTextVariable("LINK", link);
      textObject.SetTextVariable("CLAN_NAME", clanName);
      return textObject;
    }

    public static TextObject GetUnitHyperlinkText(string link, TextObject unitName)
    {
      TextObject textObject = new TextObject("{=!}<a style=\"Link.Unit\" href=\"event:{LINK}\"><b>{UNIT_NAME}</b></a>");
      textObject.SetTextVariable("LINK", link);
      textObject.SetTextVariable("UNIT_NAME", unitName);
      return textObject;
    }

    public static string GetGenericHyperlinkText(string link, string name) => "<a style=\"Link\" href=\"event:" + link + "\"><b>" + name + "</b></a>";
  }
}
