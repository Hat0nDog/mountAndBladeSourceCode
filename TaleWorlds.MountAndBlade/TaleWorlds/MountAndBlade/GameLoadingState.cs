// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.GameLoadingState
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class GameLoadingState : GameState
  {
    private bool _loadingFinished;
    private MBGameManager _gameLoader;

    public override bool IsMusicMenuState => true;

    public void SetLoadingParameters(MBGameManager gameLoader) => this._gameLoader = gameLoader;

    protected override void OnTick(float dt)
    {
      base.OnTick(dt);
      if (!this._loadingFinished)
      {
        this._loadingFinished = this._gameLoader.DoLoadingForGameManager();
      }
      else
      {
        GameStateManager.Current = Game.Current.GameStateManager;
        this._gameLoader.OnLoadFinished();
      }
    }
  }
}
