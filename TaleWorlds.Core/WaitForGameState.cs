// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.WaitForGameState
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using TaleWorlds.Network;

namespace TaleWorlds.Core
{
  public class WaitForGameState : CoroutineState
  {
    private Type _stateType;

    public WaitForGameState(Type stateType) => this._stateType = stateType;

    protected override bool IsFinished
    {
      get
      {
        GameState gameState = GameStateManager.Current != null ? GameStateManager.Current.ActiveState : (GameState) null;
        return gameState != null && this._stateType.IsInstanceOfType((object) gameState);
      }
    }
  }
}
