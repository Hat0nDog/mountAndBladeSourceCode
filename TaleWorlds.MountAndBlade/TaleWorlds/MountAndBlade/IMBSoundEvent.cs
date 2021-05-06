// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBSoundEvent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBSoundEvent
  {
    [EngineMethod("play_sound", false)]
    bool PlaySound(int fmodEventIndex, ref Vec3 position);

    [EngineMethod("play_sound_with_int_param", false)]
    bool PlaySoundWithIntParam(
      int fmodEventIndex,
      int paramIndex,
      float paramVal,
      ref Vec3 position);

    [EngineMethod("play_sound_with_str_param", false)]
    bool PlaySoundWithStrParam(
      int fmodEventIndex,
      string paramName,
      float paramVal,
      ref Vec3 position);

    [EngineMethod("play_sound_with_param", false)]
    bool PlaySoundWithParam(int soundCodeId, SoundEventParameter parameter, ref Vec3 position);
  }
}
