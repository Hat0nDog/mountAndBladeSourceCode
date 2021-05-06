// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorUseMurderHole
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorUseMurderHole : BehaviorComponent
  {
    private CastleGate _outerGate;
    private CastleGate _innerGate;
    private BatteringRam _batteringRam;

    public BehaviorUseMurderHole(Formation formation)
      : base(formation)
    {
      this.behaviorSide = formation.AI.Side;
      WorldPosition position = new WorldPosition(Mission.Current.Scene, UIntPtr.Zero, (formation.Team.TeamAI as TeamAISiegeDefender).MurderHolePosition, false);
      this._outerGate = (formation.Team.TeamAI as TeamAISiegeDefender).OuterGate;
      this._innerGate = (formation.Team.TeamAI as TeamAISiegeDefender).InnerGate;
      this._batteringRam = Mission.Current.ActiveMissionObjects.FindAllWithType<BatteringRam>().FirstOrDefault<BatteringRam>();
      this.CurrentOrder = MovementOrder.MovementOrderMove(position);
      this.BehaviorCoherence = 0.0f;
    }

    protected internal override void TickOccasionally() => this.formation.MovementOrder = this.CurrentOrder;

    public bool IsMurderHoleActive()
    {
      if (this._batteringRam != null && this._batteringRam.HasArrivedAtTarget && !this._innerGate.IsDestroyed)
        return true;
      return this._outerGate.IsDestroyed && !this._innerGate.IsDestroyed;
    }

    protected override float GetAiWeight() => (float) (10.0 * (this.IsMurderHoleActive() ? 1.0 : 0.0));
  }
}
