// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.SiegeTowerAI
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  internal sealed class SiegeTowerAI : UsableMachineAIBase
  {
    private SiegeTower SiegeTower => this.UsableMachine as SiegeTower;

    public SiegeTowerAI(SiegeTower siegeTower)
      : base((UsableMachine) siegeTower)
    {
    }

    public override bool HasActionCompleted => this.SiegeTower.MovementComponent.HasArrivedAtTarget && this.SiegeTower.State == SiegeTower.GateState.Open;

    protected override MovementOrder NextOrder => MovementOrder.MovementOrderCharge;
  }
}
