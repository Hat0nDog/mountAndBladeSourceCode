// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ItemPhysicsSoundContainer
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public static class ItemPhysicsSoundContainer
  {
    public static int SoundCodePhysicsBoulderDefault { get; private set; }

    public static int SoundCodePhysicsArrowlikeDefault { get; private set; }

    public static int SoundCodePhysicsBowlikeDefault { get; private set; }

    public static int SoundCodePhysicsDaggerlikeDefault { get; private set; }

    public static int SoundCodePhysicsGreatswordlikeDefault { get; private set; }

    public static int SoundCodePhysicsShieldlikeDefault { get; private set; }

    public static int SoundCodePhysicsSpearlikeDefault { get; private set; }

    public static int SoundCodePhysicsSwordlikeDefault { get; private set; }

    public static int SoundCodePhysicsBoulderWood { get; private set; }

    public static int SoundCodePhysicsArrowlikeWood { get; private set; }

    public static int SoundCodePhysicsBowlikeWood { get; private set; }

    public static int SoundCodePhysicsDaggerlikeWood { get; private set; }

    public static int SoundCodePhysicsGreatswordlikeWood { get; private set; }

    public static int SoundCodePhysicsShieldlikeWood { get; private set; }

    public static int SoundCodePhysicsSpearlikeWood { get; private set; }

    public static int SoundCodePhysicsSwordlikeWood { get; private set; }

    public static int SoundCodePhysicsBoulderStone { get; private set; }

    public static int SoundCodePhysicsArrowlikeStone { get; private set; }

    public static int SoundCodePhysicsBowlikeStone { get; private set; }

    public static int SoundCodePhysicsDaggerlikeStone { get; private set; }

    public static int SoundCodePhysicsGreatswordlikeStone { get; private set; }

    public static int SoundCodePhysicsShieldlikeStone { get; private set; }

    public static int SoundCodePhysicsSpearlikeStone { get; private set; }

    public static int SoundCodePhysicsSwordlikeStone { get; private set; }

    static ItemPhysicsSoundContainer() => ItemPhysicsSoundContainer.UpdateItemPhysicsSoundCodes();

    private static void UpdateItemPhysicsSoundCodes()
    {
      ItemPhysicsSoundContainer.SoundCodePhysicsBoulderDefault = SoundEvent.GetEventIdFromString("event:/physics/boulder/default");
      ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/arrowlike/default");
      ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/bowlike/default");
      ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/daggerlike/default");
      ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/greatswordlike/default");
      ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/shieldlike/default");
      ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/spearlike/default");
      ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeDefault = SoundEvent.GetEventIdFromString("event:/physics/swordlike/default");
      ItemPhysicsSoundContainer.SoundCodePhysicsBoulderWood = SoundEvent.GetEventIdFromString("event:/physics/boulder/wood");
      ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeWood = SoundEvent.GetEventIdFromString("event:/physics/arrowlike/wood");
      ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeWood = SoundEvent.GetEventIdFromString("event:/physics/bowlike/wood");
      ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeWood = SoundEvent.GetEventIdFromString("event:/physics/daggerlike/wood");
      ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeWood = SoundEvent.GetEventIdFromString("event:/physics/greatswordlike/wood");
      ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeWood = SoundEvent.GetEventIdFromString("event:/physics/shieldlike/wood");
      ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeWood = SoundEvent.GetEventIdFromString("event:/physics/spearlike/wood");
      ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeWood = SoundEvent.GetEventIdFromString("event:/physics/swordlike/wood");
      ItemPhysicsSoundContainer.SoundCodePhysicsBoulderStone = SoundEvent.GetEventIdFromString("event:/physics/boulder/stone");
      ItemPhysicsSoundContainer.SoundCodePhysicsArrowlikeStone = SoundEvent.GetEventIdFromString("event:/physics/arrowlike/stone");
      ItemPhysicsSoundContainer.SoundCodePhysicsBowlikeStone = SoundEvent.GetEventIdFromString("event:/physics/bowlike/stone");
      ItemPhysicsSoundContainer.SoundCodePhysicsDaggerlikeStone = SoundEvent.GetEventIdFromString("event:/physics/daggerlike/stone");
      ItemPhysicsSoundContainer.SoundCodePhysicsGreatswordlikeStone = SoundEvent.GetEventIdFromString("event:/physics/greatswordlike/stone");
      ItemPhysicsSoundContainer.SoundCodePhysicsShieldlikeStone = SoundEvent.GetEventIdFromString("event:/physics/shieldlike/stone");
      ItemPhysicsSoundContainer.SoundCodePhysicsSpearlikeStone = SoundEvent.GetEventIdFromString("event:/physics/spearlike/stone");
      ItemPhysicsSoundContainer.SoundCodePhysicsSwordlikeStone = SoundEvent.GetEventIdFromString("event:/physics/swordlike/stone");
    }
  }
}
