// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBVoiceManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBVoiceManager
  {
    [EngineMethod("get_voice_type_index", false)]
    int GetVoiceTypeIndex(string voiceType);

    [EngineMethod("get_voice_definition_count_with_monster_sound_and_collision_info_class_name", false)]
    int GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(string className);

    [EngineMethod("get_voice_definitions_with_monster_sound_and_collision_info_class_name", false)]
    void GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(
      string className,
      int[] definitionIndices);
  }
}
