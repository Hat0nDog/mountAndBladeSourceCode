// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBActionSet
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBActionSet
  {
    [EngineMethod("get_index_with_id", false)]
    int GetIndexWithID(string id);

    [EngineMethod("get_name_with_index", false)]
    string GetNameWithIndex(int index);

    [EngineMethod("get_number_of_action_sets", false)]
    int GetNumberOfActionSets();

    [EngineMethod("get_number_of_monster_usage_sets", false)]
    int GetNumberOfMonsterUsageSets();

    [EngineMethod("are_actions_alternatives", false)]
    bool AreActionsAlternatives(int index, int actionNo1, int actionNo2);

    [EngineMethod("get_bone_index_with_id", false)]
    sbyte GetBoneIndexWithId(string actionSetId, string boneId);
  }
}
