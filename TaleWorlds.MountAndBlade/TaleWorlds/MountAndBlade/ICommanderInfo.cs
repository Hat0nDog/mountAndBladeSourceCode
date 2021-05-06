// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ICommanderInfo
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Objects;

namespace TaleWorlds.MountAndBlade
{
  public interface ICommanderInfo : IMissionBehavior
  {
    event Action<BattleSideEnum, float> OnMoraleChangedEvent;

    event Action OnFlagNumberChangedEvent;

    event Action<FlagCapturePoint, Team> OnCapturePointOwnerChangedEvent;

    IEnumerable<FlagCapturePoint> AllCapturePoints { get; }

    Team GetFlagOwner(FlagCapturePoint flag);

    bool AreMoralesIndependent { get; }
  }
}
