// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.ConversationTagList
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using System.Reflection;

namespace TaleWorlds.Core
{
  public static class ConversationTagList
  {
    private static List<string> _tagIdList;
    public const string DefaultTag = "DefaultTag";
    public const string PlayerIsAffiliatedTag = "PlayerIsAffiliatedTag";
    public const string PlayerIsFemaleTag = "PlayerIsFemaleTag";
    public const string PlayerIsMaleTag = "PlayerIsMaleTag";
    public const string PlayerIsFamousTag = "PlayerIsFamousTag";
    public const string PlayerIsNobleTag = "PlayerIsNobleTag";
    public const string AttractedToPlayerTag = "AttractedToPlayerTag";
    public const string EngagedToPlayerTag = "EngagedToPlayerTag";
    public const string FriendlyRelationshipTag = "FriendlyRelationshipTag";
    public const string HostileRelationshipTag = "HostileRelationshipTag";
    public const string NoConflictTag = "NoConflictTag";
    public const string PlayerIsRulerTag = "PlayerIsRulerTag";
    public const string NpcIsFemaleTag = "NpcIsFemaleTag";
    public const string NpcIsMaleTag = "NpcIsMaleTag";
    public const string PlayerIsEnemyTag = "PlayerIsEnemyTag";
    public const string PlayerIsAlliedTag = "PlayerIsAlliedTag";
    public const string WaryTag = "WaryTag";
    public const string DrinkingInTavernTag = "DrinkingInTavernTag";
    public const string OutlawSympathyTag = "OutlawSympathyTag";
    public const string HighRegisterTag = "HighRegisterTag";
    public const string LowRegisterTag = "LowRegisterTag";
    public const string TribalRegisterTag = "TribalRegisterTag";
    public const string AttackingTag = "AttackingTag";
    public const string UncharitableTag = "UncharitableTag";
    public const string AmoralTag = "AmoralTag";
    public const string ChivalrousTag = "ChivalrousTag";
    public const string SexistTag = "SexistTag";
    public const string UnderCommandTag = "UnderCommandTag";
    public const string NpcIsNobleTag = "NpcIsNobleTag";
    public const string NpcIsLiegeTag = "NpcIsLiegeTag";
    public const string PlayerIsLiegeTag = "PlayerIsLiegeTag";
    public const string ImpoliteTag = "ImpoliteTag";
    public const string RomanticallyInvolvedTag = "RomanticallyInvolvedTag";
    public const string OldTag = "OldTag";
    public const string FirstMeetingTag = "FirstMeetingTag";
    public const string MetBeforeTag = "MetBeforeTag";
    public const string PlayerIsFatherTag = "PlayerIsFatherTag";
    public const string PlayerIsMotherTag = "PlayerIsMotherTag";
    public const string PlayerIsSonTag = "PlayerIsSonTag";
    public const string PlayerIsDaughterTag = "PlayerIsDaughterTag";
    public const string PlayerIsBrotherTag = "PlayerIsBrotherTag";
    public const string PlayerIsSisterTag = "PlayerIsSisterTag";
    public const string PlayerIsKinTag = "PlayerIsKinTag";
    public const string PlayerIsSpouseTag = "PlayerIsSpouseTag";
    public const string NonCombatantTag = "NonCombatantTag";
    public const string CombatantTag = "CombatantTag";
    public const string CurrentConversationIsFirst = "CurrentConversationIsFirst";
    public const string OnTheRoadTag = "OnTheRoadTag";
    public const string PlayerBesiegingTag = "PlayerBesiegingTag";
    public const string EmpireTag = "EmpireTag";
    public const string BattanianTag = "BattanianTag";
    public const string VlandianTag = "VlandianTag";
    public const string KhuzaitTag = "KhuzaitTag";
    public const string AseraiTag = "AseraiTag";
    public const string SturgianTag = "SturgianTag";
    public const string MercyTag = "MercyTag";
    public const string CruelTag = "CruelTag";
    public const string GenerosityTag = "GenerosityTag";
    public const string UngratefulTag = "UngratefulTag";
    public const string HonorTag = "HonorTag";
    public const string DeviousTag = "DeviousTag";
    public const string CalculatingTag = "CalculatingTag";
    public const string ImpulsiveTag = "ImpulsiveTag";
    public const string ValorTag = "ValorTag";
    public const string CautiousTag = "CautiousTag";
    public const string NonviolentProfessionTag = "NonviolentProfessionTag";
    public const string PreacherNotableTypeTag = "PreacherNotableTypeTag";
    public const string MerchantNotableTypeTag = "MerchantNotableTypeTag";
    public const string ArtisanNotableTypeTag = "ArtisanNotableTypeTag";
    public const string OutlawNotableTypeTag = "OutlawNotableTypeTag";
    public const string GangLeaderNotableTypeTag = "GangLeaderNotableTypeTag";
    public const string HeadmanNotableTypeTag = "HeadmanNotableTypeTag";
    public const string AnyNotableTypeTag = "AnyNotableTypeTag";
    public const string WandererTag = "WandererTag";
    public const string InHomeSettlementTag = "InHomeSettlementTag";
    public const string RogueSkillsTag = "RogueSkillsTag";
    public const string PersonaEarnestTag = "PersonaEarnestTag";
    public const string PersonaCurtTag = "PersonaCurtTag";
    public const string PersonaIronicTag = "PersonaIronicTag";
    public const string PersonaSoftspokenTag = "PersonaSoftspokenTag";
    public const string VoiceGroupPersonaEarnestTribalTag = "VoiceGroupPersonaEarnestTribalTag";
    public const string VoiceGroupPersonaEarnestUpperTag = "VoiceGroupPersonaEarnestUpperTag";
    public const string VoiceGroupPersonaEarnestLowerTag = "VoiceGroupPersonaEarnestLowerTag";
    public const string VoiceGroupPersonaCurtTribalTag = "VoiceGroupPersonaCurtTribalTag";
    public const string VoiceGroupPersonaCurtUpperTag = "VoiceGroupPersonaCurtUpperTag";
    public const string VoiceGroupPersonaCurtLowerTag = "VoiceGroupPersonaCurtLowerTag";
    public const string VoiceGroupPersonaIronicTribalTag = "VoiceGroupPersonaIronicTribalTag";
    public const string VoiceGroupPersonaIronicUpperTag = "VoiceGroupPersonaIronicUpperTag";
    public const string VoiceGroupPersonaIronicLowerTag = "VoiceGroupPersonaIronicLowerTag";
    public const string VoiceGroupPersonaSoftspokenTribalTag = "VoiceGroupPersonaSoftspokenTribalTag";
    public const string VoiceGroupPersonaSoftspokenUpperTag = "VoiceGroupPersonaSoftspokenUpperTag";
    public const string VoiceGroupPersonaSoftspokenLowerTag = "VoiceGroupPersonaSoftspokenLowerTag";

    internal static bool Contains(string tagId)
    {
      if (ConversationTagList._tagIdList == null)
      {
        ConversationTagList._tagIdList = new List<string>();
        foreach (FieldInfo field in typeof (ConversationTagList).GetFields(BindingFlags.Static | BindingFlags.Public))
        {
          if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof (string))
            ConversationTagList._tagIdList.Add((string) field.GetValue((object) null));
        }
      }
      return ConversationTagList._tagIdList.Contains(tagId);
    }
  }
}
