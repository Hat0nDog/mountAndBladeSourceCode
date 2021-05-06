// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BatteringRamAI
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  internal sealed class BatteringRamAI : UsableMachineAIBase
  {
    public BatteringRamAI(BatteringRam batteringRam)
      : base((UsableMachine) batteringRam)
    {
    }

    private BatteringRam BatteringRam => this.UsableMachine as BatteringRam;

    public override bool HasActionCompleted => this.BatteringRam.IsDeactivated;

    protected override MovementOrder NextOrder => Mission.Current.Teams[0].TeamAI is TeamAISiegeComponent teamAi && teamAi.InnerGate != null && !teamAi.InnerGate.IsDestroyed ? MovementOrder.MovementOrderAttackEntity(teamAi.InnerGate.GameEntity, false) : MovementOrder.MovementOrderCharge;
  }
}
