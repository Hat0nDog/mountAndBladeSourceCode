// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.OrderComponent
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Diagnostics;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public abstract class OrderComponent
  {
    private readonly Timer tickTimer;
    protected Func<Formation, Vec3> Position;
    protected Func<Formation, Vec2> Direction;
    private Vec2 _previousDirection = Vec2.Invalid;

    public Vec2 GetDirection(Formation f)
    {
      Vec2 vec2 = this.Direction(f);
      if (f.IsAIControlled && (double) vec2.DotProduct(this._previousDirection) > 0.870000004768372)
        vec2 = this._previousDirection;
      else
        this._previousDirection = vec2;
      return vec2;
    }

    protected void CopyPositionAndDirectionFrom(OrderComponent order)
    {
      this.Position = order.Position;
      this.Direction = order.Direction;
    }

    protected OrderComponent(float tickTimerDuration = 0.5f) => this.tickTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), tickTimerDuration);

    public abstract OrderType OrderType { get; }

    internal bool Tick(Formation formation)
    {
      int num = this.tickTimer.Check(MBCommon.GetTime(MBCommon.TimeType.Mission)) ? 1 : 0;
      if (num == 0)
        return num != 0;
      this.TickOccasionally(formation, this.tickTimer.PreviousDeltaTime);
      return num != 0;
    }

    [Conditional("DEBUG")]
    protected virtual void TickDebug(Formation formation)
    {
    }

    protected internal virtual void TickOccasionally(Formation formation, float dt)
    {
    }

    protected internal virtual void OnApply(Formation formation)
    {
    }

    protected internal virtual void OnCancel(Formation formation)
    {
    }

    protected internal virtual void OnUnitJoinOrLeave(Agent unit, bool isJoining)
    {
    }

    protected internal virtual bool IsApplicable(Formation formation) => true;

    protected internal virtual bool CanStack => false;

    protected internal virtual bool CancelsPreviousDirectionOrder => false;

    protected internal virtual bool CancelsPreviousArrangementOrder => false;

    protected internal virtual MovementOrder GetSubstituteOrder(Formation formation) => MovementOrder.MovementOrderCharge;

    protected internal virtual void OnArrangementChanged(Formation formation)
    {
    }
  }
}
