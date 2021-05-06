// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticArchersOnTheHill
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
  public class TacticArchersOnTheHill : TacticComponent
  {
    public TacticArchersOnTheHill(Team team)
      : base(team)
    {
    }

    protected internal override void TickOccasionally()
    {
      IEnumerable<Formation> formations1 = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.IsRanged()));
      IEnumerable<Formation> formations2 = this.Formations.Where<Formation>((Func<Formation, bool>) (f => !f.IsRanged()));
      foreach (Formation formation in formations1)
      {
        formation.AI.ResetBehaviorWeights();
        formation.AI.SetBehaviorWeight<BehaviorHoldHighGround>(1f);
      }
      foreach (Formation formation in formations2)
      {
        formation.AI.ResetBehaviorWeights();
        formation.AI.SetBehaviorWeight<BehaviorAdvance>(1f);
      }
    }
  }
}
