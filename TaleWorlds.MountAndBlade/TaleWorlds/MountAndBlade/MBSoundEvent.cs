// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBSoundEvent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public static class MBSoundEvent
  {
    public static bool PlaySound(int soundCodeId, ref Vec3 position) => MBAPI.IMBSoundEvent.PlaySound(soundCodeId, ref position);

    public static bool PlaySound(int soundCodeId, Vec3 position)
    {
      Vec3 position1 = position;
      return MBAPI.IMBSoundEvent.PlaySound(soundCodeId, ref position1);
    }

    public static bool PlaySound(int soundCodeId, ref SoundEventParameter parameter, Vec3 position)
    {
      Vec3 position1 = position;
      return MBSoundEvent.PlaySound(soundCodeId, ref parameter, ref position1);
    }

    public static bool PlaySound(
      string soundPath,
      ref SoundEventParameter parameter,
      Vec3 position)
    {
      int eventIdFromString = SoundEvent.GetEventIdFromString(soundPath);
      Vec3 vec3 = position;
      ref SoundEventParameter local1 = ref parameter;
      ref Vec3 local2 = ref vec3;
      return MBSoundEvent.PlaySound(eventIdFromString, ref local1, ref local2);
    }

    public static bool PlaySound(
      int soundCodeId,
      ref SoundEventParameter parameter,
      ref Vec3 position)
    {
      return MBAPI.IMBSoundEvent.PlaySoundWithParam(soundCodeId, parameter, ref position);
    }
  }
}
