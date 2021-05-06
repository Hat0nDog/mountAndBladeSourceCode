// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TacticStop
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class TacticStop : TacticComponent
  {
    public TacticStop(Team team)
      : base(team)
    {
    }

    protected internal override void OnApply()
    {
    }

    protected internal override void TickOccasionally()
    {
      foreach (Formation formation in this.Formations)
      {
        formation.AI.ResetBehaviorWeights();
        formation.AI.SetBehaviorWeight<BehaviorStop>(1f);
      }
      base.TickOccasionally();
    }
  }
}
