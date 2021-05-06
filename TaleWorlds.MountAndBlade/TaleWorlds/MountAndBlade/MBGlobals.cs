// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBGlobals
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public static class MBGlobals
  {
    public const float Gravity = 9.806f;
    public static readonly Vec3 GravityVec3 = new Vec3(z: -9.806f);
    private static bool _initialized;
    private static Dictionary<string, MBActionSet> _actionSets;

    public static MBActionSet HumanWarriorActionSet { get; private set; }

    public static MBActionSet HumanVillager1ActionSet { get; private set; }

    public static MBActionSet HumanVillager2ActionSet { get; private set; }

    public static MBActionSet HumanVillager3ActionSet { get; private set; }

    public static MBActionSet HumanVillager4ActionSet { get; private set; }

    public static MBActionSet HumanFemaleVillager1ActionSet { get; private set; }

    public static MBActionSet HumanFemaleVillager2ActionSet { get; private set; }

    public static MBActionSet HumanVillainActionSet { get; private set; }

    public static MBActionSet HumanPrisonerActionSet { get; private set; }

    public static MBActionSet HumanLordActionSet { get; private set; }

    public static MBActionSet HumanLadyActionSet { get; private set; }

    public static MBActionSet HumanVillagerDrinkerWithMugActionSet { get; private set; }

    public static MBActionSet HumanVillagerDrinkerWithBowlActionSet { get; private set; }

    public static MBActionSet HumanBarmaidActionSet { get; private set; }

    public static MBActionSet HumanTavernKeeperActionSet { get; private set; }

    public static MBActionSet HumanMusicianActionSet { get; private set; }

    public static MBActionSet HumanGameHostActionSet { get; private set; }

    public static MBActionSet HumanGuardActionSet { get; private set; }

    public static MBActionSet HumanMaleFaceGenActionSet { get; private set; }

    public static MBActionSet HumanFemaleFaceGenActionSet { get; private set; }

    public static MBActionSet HumanMapActionSet { get; private set; }

    public static MBActionSet HumanMapWithBannerActionSet { get; private set; }

    public static MBActionSet PlayerMaleActionSet { get; private set; }

    public static void InitializeReferences()
    {
      if (MBGlobals._initialized)
        return;
      MBGlobals._actionSets = new Dictionary<string, MBActionSet>();
      MBGlobals.HumanWarriorActionSet = MBGlobals.GetActionSet("as_human_warrior");
      MBGlobals.HumanVillager1ActionSet = MBGlobals.GetActionSet("as_human_villager");
      MBGlobals.HumanVillager2ActionSet = MBGlobals.GetActionSet("as_human_villager_2");
      MBGlobals.HumanVillager3ActionSet = MBGlobals.GetActionSet("as_human_villager_3");
      MBGlobals.HumanVillager4ActionSet = MBGlobals.GetActionSet("as_human_villager_4");
      MBGlobals.HumanFemaleVillager1ActionSet = MBGlobals.GetActionSet("as_human_female_villager");
      MBGlobals.HumanFemaleVillager2ActionSet = MBGlobals.GetActionSet("as_human_female_villager_2");
      MBGlobals.HumanVillainActionSet = MBGlobals.GetActionSet("as_human_villain");
      MBGlobals.HumanPrisonerActionSet = MBGlobals.GetActionSet("as_human_prisoner");
      MBGlobals.HumanLordActionSet = MBGlobals.GetActionSet("as_human_lord");
      MBGlobals.HumanLadyActionSet = MBGlobals.GetActionSet("as_human_lady");
      MBGlobals.HumanVillagerDrinkerWithMugActionSet = MBGlobals.GetActionSet("as_human_villager_drinker_with_mug");
      MBGlobals.HumanVillagerDrinkerWithBowlActionSet = MBGlobals.GetActionSet("as_human_villager_drinker_with_bowl");
      MBGlobals.HumanBarmaidActionSet = MBGlobals.GetActionSet("as_human_barmaid");
      MBGlobals.HumanTavernKeeperActionSet = MBGlobals.GetActionSet("as_human_tavern_keeper");
      MBGlobals.HumanMusicianActionSet = MBGlobals.GetActionSet("as_human_musician");
      MBGlobals.HumanGameHostActionSet = MBGlobals.GetActionSet("as_human_game_host");
      MBGlobals.HumanGuardActionSet = MBGlobals.GetActionSet("as_human_guard");
      MBGlobals.HumanMaleFaceGenActionSet = MBGlobals.GetActionSet("as_human_male_facegen");
      MBGlobals.HumanFemaleFaceGenActionSet = MBGlobals.GetActionSet("as_human_female_facegen");
      MBGlobals.HumanMapActionSet = MBGlobals.GetActionSet("as_human_map");
      MBGlobals.HumanMapWithBannerActionSet = MBGlobals.GetActionSet("as_human_map_with_banner");
      MBGlobals.PlayerMaleActionSet = MBGlobals.GetActionSet("as_player_male");
      MBGlobals._initialized = true;
    }

    public static MBActionSet GetActionSet(string actionSetCode)
    {
      MBActionSet mbActionSet = MBActionSet.InvalidActionSet;
      if (!MBGlobals._actionSets.TryGetValue(actionSetCode, out mbActionSet))
      {
        mbActionSet = MBActionSet.GetActionSet(actionSetCode);
        if (!mbActionSet.IsValid)
          throw new MBNotFoundException("Unable to find action set : " + actionSetCode);
      }
      return mbActionSet;
    }

    public static string GetMemberName<T>(Expression<Func<T>> memberExpression) => ((MemberExpression) memberExpression.Body).Member.Name;

    public static string GetMethodName<T>(Expression<Func<T>> memberExpression) => ((MethodCallExpression) memberExpression.Body).Method.Name;
  }
}
