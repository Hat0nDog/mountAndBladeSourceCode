// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BehaviorProtectGeneral
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class BehaviorProtectGeneral : BehaviorComponent
  {
    public BehaviorProtectGeneral(Formation formation)
      : base(formation)
    {
      this.CurrentOrder = MovementOrder.MovementOrderFollow(formation.Team.GeneralsFormation == null || formation.Team.GeneralsFormation.CountOfUnits <= 0 ? Mission.Current.MainAgent : formation.Team.GeneralsFormation.GetFirstUnit());
    }

    protected internal override void TickOccasionally() => this.formation.MovementOrder = this.CurrentOrder;

    protected internal override float NavmeshlessTargetPositionPenalty => 1f;

    protected override float GetAiWeight() => this.formation.Team.GeneralsFormation != null && this.formation.Team.GeneralsFormation.CountOfUnits > 0 || this.formation.Team.IsPlayerTeam && this.formation.Team.IsPlayerGeneral && Mission.Current.MainAgent != null ? 1f : 0.0f;
  }
}
