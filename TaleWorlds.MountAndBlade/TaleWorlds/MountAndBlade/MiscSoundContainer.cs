// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MiscSoundContainer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public static class MiscSoundContainer
  {
    public static int SoundCodeSiegeSiegetowerDoorland { get; private set; }

    public static int SoundCodeMovementFoleyDoorOpen { get; private set; }

    public static int SoundCodeMovementFoleyDoorClose { get; private set; }

    public static int SoundCodeAmbientNodeSiegeBallistaFire { get; private set; }

    public static int SoundCodeAmbientNodeSiegeMangonelFire { get; private set; }

    public static int SoundCodeAmbientNodeSiegeTrebuchetFire { get; private set; }

    public static int SoundCodeAmbientNodeSiegeBallistaHit { get; private set; }

    public static int SoundCodeAmbientNodeSiegeBoulderHit { get; private set; }

    static MiscSoundContainer() => MiscSoundContainer.UpdateMiscSoundCodes();

    private static void UpdateMiscSoundCodes()
    {
      MiscSoundContainer.SoundCodeSiegeSiegetowerDoorland = SoundEvent.GetEventIdFromString("event:/mission/siege/siegetower/doorland");
      MiscSoundContainer.SoundCodeMovementFoleyDoorOpen = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/door_open");
      MiscSoundContainer.SoundCodeMovementFoleyDoorClose = SoundEvent.GetEventIdFromString("event:/mission/movement/foley/door_close");
      MiscSoundContainer.SoundCodeAmbientNodeSiegeBallistaFire = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/ballista_fire");
      MiscSoundContainer.SoundCodeAmbientNodeSiegeMangonelFire = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/mangonel_fire");
      MiscSoundContainer.SoundCodeAmbientNodeSiegeTrebuchetFire = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/trebuchet_fire");
      MiscSoundContainer.SoundCodeAmbientNodeSiegeBallistaHit = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/ballista_hit");
      MiscSoundContainer.SoundCodeAmbientNodeSiegeBoulderHit = SoundEvent.GetEventIdFromString("event:/map/ambient/node/siege/boulder_hit");
    }
  }
}
