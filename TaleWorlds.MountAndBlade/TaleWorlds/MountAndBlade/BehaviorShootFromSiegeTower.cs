// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorShootFromSiegeTower
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorShootFromSiegeTower : BehaviorComponent
  {
    private FormationAI.BehaviorSide behaviourSide;
    private SiegeTower _siegeTower;

    public BehaviorShootFromSiegeTower(Formation formation)
      : base(formation)
    {
      this.behaviourSide = formation.AI.Side;
      this._siegeTower = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeTower>().FirstOrDefault<SiegeTower>((Func<SiegeTower, bool>) (st => st.WeaponSide == this.behaviourSide));
    }

    protected internal override void TickOccasionally()
    {
      base.TickOccasionally();
      if (this.formation.AI.Side != this.behaviourSide)
      {
        this.behaviourSide = this.formation.AI.Side;
        this._siegeTower = Mission.Current.ActiveMissionObjects.FindAllWithType<SiegeTower>().FirstOrDefault<SiegeTower>((Func<SiegeTower, bool>) (st => st.WeaponSide == this.behaviourSide));
      }
      if (this._siegeTower == null || this._siegeTower.IsDestroyed)
        return;
      this.formation.MovementOrder = this.CurrentOrder;
    }

    protected override float GetAiWeight() => 0.0f;
  }
}
