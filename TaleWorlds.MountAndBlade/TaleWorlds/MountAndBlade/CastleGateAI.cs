// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CastleGateAI
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class CastleGateAI : UsableMachineAIBase
  {
    private CastleGate.GateState _initialState;

    internal void ResetInitialGateState(CastleGate.GateState newInitialState) => this._initialState = newInitialState;

    public CastleGateAI(CastleGate gate)
      : base((UsableMachine) gate)
    {
      this._initialState = gate.State;
    }

    public override bool HasActionCompleted => (this.UsableMachine as CastleGate).State != this._initialState;
  }
}
