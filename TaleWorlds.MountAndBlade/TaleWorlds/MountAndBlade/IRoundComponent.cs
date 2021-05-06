// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IRoundComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public interface IRoundComponent : IMissionBehavior
  {
    event Action OnRoundStarted;

    event Action OnPreparationEnded;

    event Action OnPreRoundEnding;

    event Action OnRoundEnding;

    event Action OnPostRoundEnded;

    event Action OnCurrentRoundStateChanged;

    float LastRoundEndRemainingTime { get; }

    float RemainingRoundTime { get; }

    MultiplayerRoundState CurrentRoundState { get; }

    int RoundCount { get; }

    BattleSideEnum RoundWinner { get; }

    RoundEndReason RoundEndReason { get; }
  }
}
