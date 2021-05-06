// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ICastleKeyPosition
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public interface ICastleKeyPosition
  {
    IPrimarySiegeWeapon AttackerSiegeWeapon { get; set; }

    TacticalPosition MiddlePosition { get; }

    TacticalPosition WaitPosition { get; }

    WorldFrame MiddleFrame { get; }

    WorldFrame DefenseWaitFrame { get; }

    FormationAI.BehaviorSide DefenseSide { get; }

    Vec3 GetPosition();
  }
}
