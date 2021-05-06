// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.InitialState
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class InitialState : GameState
  {
    public override bool IsMusicMenuState => true;

    public event OnInitialMenuOptionInvokedDelegate OnInitialMenuOptionInvoked;

    protected override void OnActivate()
    {
      base.OnActivate();
      if (!Utilities.CommandLineArgumentExists("+connect_lobby"))
        return;
      MBGameManager.StartNewGame((MBGameManager) new MultiplayerGameManager());
    }

    protected override void OnTick(float dt) => base.OnTick(dt);

    public void OnExecutedInitialStateOption(InitialStateOption target)
    {
      if (this.OnInitialMenuOptionInvoked == null)
        return;
      this.OnInitialMenuOptionInvoked(target);
    }
  }
}
