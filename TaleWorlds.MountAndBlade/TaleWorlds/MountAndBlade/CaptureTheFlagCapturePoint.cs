// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CaptureTheFlagCapturePoint
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class CaptureTheFlagCapturePoint
  {
    public float Progress { get; set; }

    public CaptureTheFlagFlagDirection Direction { get; set; }

    public float Speed { get; set; }

    public MatrixFrame InitialFlagFrame { get; private set; }

    public GameEntity FlagEntity { get; private set; }

    public SynchedMissionObject FlagHolder { get; private set; }

    public GameEntity FlagBottomBoundary { get; private set; }

    public GameEntity FlagTopBoundary { get; private set; }

    public BattleSideEnum BattleSide { get; }

    public int Index { get; }

    public bool UpdateFlag { get; set; }

    public CaptureTheFlagCapturePoint(GameEntity flagPole, BattleSideEnum battleSide, int index)
    {
      this.Reset();
      this.BattleSide = battleSide;
      this.Index = index;
      this.FlagHolder = flagPole.CollectChildrenEntitiesWithTag("score_stand").SingleOrDefault<GameEntity>().GetFirstScriptOfType<SynchedMissionObject>();
      this.FlagEntity = this.FlagHolder.GameEntity.GetChildren().Single<GameEntity>((Func<GameEntity, bool>) (q => ((IEnumerable<string>) q.Tags).Contains<string>("flag")));
      this.FlagHolder.GameEntity.EntityFlags |= EntityFlags.NoOcclusionCulling;
      this.FlagEntity.EntityFlags |= EntityFlags.NoOcclusionCulling;
      this.FlagBottomBoundary = flagPole.GetChildren().Single<GameEntity>((Func<GameEntity, bool>) (q => ((IEnumerable<string>) q.Tags).Contains<string>("flag_raising_bottom")));
      this.FlagTopBoundary = flagPole.GetChildren().Single<GameEntity>((Func<GameEntity, bool>) (q => ((IEnumerable<string>) q.Tags).Contains<string>("flag_raising_top")));
      MatrixFrame globalFrame = this.FlagHolder.GameEntity.GetGlobalFrame();
      globalFrame.origin.z = this.FlagBottomBoundary.GetGlobalFrame().origin.z;
      this.InitialFlagFrame = globalFrame;
    }

    public void Reset()
    {
      this.Progress = 0.0f;
      this.Direction = CaptureTheFlagFlagDirection.None;
      this.Speed = 0.0f;
      this.UpdateFlag = false;
    }
  }
}
