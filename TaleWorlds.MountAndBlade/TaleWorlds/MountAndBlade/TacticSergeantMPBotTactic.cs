// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticSergeantMPBotTactic
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class TacticSergeantMPBotTactic : TacticComponent
  {
    public TacticSergeantMPBotTactic(Team team)
      : base(team)
    {
    }

    protected internal override void TickOccasionally()
    {
      foreach (Formation formation in this.Formations)
      {
        formation.AI.ResetBehaviorWeights();
        formation.AI.SetBehaviorWeight<BehaviorCharge>(1f);
        formation.AI.SetBehaviorWeight<BehaviorTacticalCharge>(1f);
        formation.AI.SetBehaviorWeight<BehaviorSergeantMPInfantry>(1f);
        formation.AI.SetBehaviorWeight<BehaviorSergeantMPRanged>(1f);
        formation.AI.SetBehaviorWeight<BehaviorSergeantMPMounted>(1f);
        formation.AI.SetBehaviorWeight<BehaviorSergeantMPMountedRanged>(1f);
        formation.AI.SetBehaviorWeight<BehaviorSergeantMPLastFlagLastStand>(1f);
      }
    }
  }
}
