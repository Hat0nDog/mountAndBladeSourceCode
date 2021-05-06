// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticRangedEncirclement
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
  internal class TacticRangedEncirclement : TacticComponent
  {
    private const int MinimumUnitCountForSplit = 10;

    public TacticRangedEncirclement(Team team)
      : base(team)
    {
    }

    protected internal override void TickOccasionally()
    {
      IEnumerable<Formation> formations = this.Formations.Where<Formation>((Func<Formation, bool>) (f => f.IsRanged()));
      FormationAI.BehaviorSide behaviorSide = FormationAI.BehaviorSide.Left;
      foreach (Formation formation in formations)
      {
        formation.AI.Side = behaviorSide;
        behaviorSide = behaviorSide == FormationAI.BehaviorSide.Left ? FormationAI.BehaviorSide.Right : FormationAI.BehaviorSide.Left;
      }
      foreach (Formation formation in this.Formations)
      {
        formation.AI.ResetBehaviorWeights();
        if (formation.IsRanged())
          formation.AI.SetBehaviorWeight<BehaviorFlank>(1f);
        else
          formation.AI.SetBehaviorWeight<BehaviorAdvance>(1f);
      }
    }
  }
}
