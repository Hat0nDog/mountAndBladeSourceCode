// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CustomBattle.CustomBattleScreen
// Assembly: TaleWorlds.MountAndBlade.CustomBattle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8065FEE3-AE77-4E53-B13C-3890101BA431
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\Modules\CustomBattle\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.CustomBattle.dll

using System;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Screen;

namespace TaleWorlds.MountAndBlade.CustomBattle
{
  [GameStateScreen(typeof (CustomBattleState))]
  public class CustomBattleScreen : ScreenBase, IGameStateListener
  {
    private CustomBattleState _customBattleState;
    private GauntletLayer _gauntletLayer;
    private IGauntletMovie _gauntletMovie;
    private CustomBattleMenuVM _dataSource;
    private bool _isMovieLoaded;

    public CustomBattleScreen(CustomBattleState customBattleState) => this._customBattleState = customBattleState;

    void IGameStateListener.OnActivate()
    {
    }

    void IGameStateListener.OnDeactivate()
    {
    }

    void IGameStateListener.OnInitialize()
    {
    }

    void IGameStateListener.OnFinalize() => this._dataSource.OnFinalize();

    protected override void OnInitialize()
    {
      base.OnInitialize();
      this._dataSource = new CustomBattleMenuVM(this._customBattleState);
      this._dataSource.SetStartInputKey(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>) (g => g?.Id == "Start")));
      this._dataSource.SetCancelInputKey(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>) (g => g?.Id == "Exit")));
      this._dataSource.SetRandomizeInputKey(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory").RegisteredHotKeys.FirstOrDefault<HotKey>((Func<HotKey, bool>) (g => g?.Id == "Randomize")));
      this._gauntletLayer = new GauntletLayer(1, "GauntletLayer");
      ((ScreenLayer) this._gauntletLayer).Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
      this.LoadMovie();
      ((ScreenLayer) this._gauntletLayer).InputRestrictions.SetInputRestrictions();
      this._dataSource.SetActiveState(true);
      this.AddLayer((ScreenLayer) this._gauntletLayer);
    }

    protected override void OnFrameTick(float dt)
    {
      base.OnFrameTick(dt);
      if (((ScreenLayer) this._gauntletLayer).Input.IsHotKeyDownAndReleased("Exit") && !((ScreenLayer) this._gauntletLayer).IsFocusedOnInput())
        this._dataSource.ExecuteBack();
      else if (((ScreenLayer) this._gauntletLayer).Input.IsHotKeyDownAndReleased("Randomize") && !((ScreenLayer) this._gauntletLayer).IsFocusedOnInput())
      {
        this._dataSource.ExecuteRandomize();
      }
      else
      {
        if (!((ScreenLayer) this._gauntletLayer).Input.IsHotKeyDownAndReleased("Start") || ((ScreenLayer) this._gauntletLayer).IsFocusedOnInput())
          return;
        this._dataSource.ExecuteStart();
      }
    }

    protected override void OnFinalize()
    {
      this.UnloadMovie();
      this.RemoveLayer((ScreenLayer) this._gauntletLayer);
      this._dataSource = (CustomBattleMenuVM) null;
      this._gauntletLayer = (GauntletLayer) null;
      base.OnFinalize();
    }

    protected override void OnActivate()
    {
      this.LoadMovie();
      this._dataSource?.SetActiveState(true);
      ((ScreenLayer) this._gauntletLayer).IsFocusLayer = true;
      ScreenManager.TrySetFocus((ScreenLayer) this._gauntletLayer);
      LoadingWindow.DisableGlobalLoadingWindow();
      base.OnActivate();
    }

    protected override void OnDeactivate()
    {
      base.OnDeactivate();
      this.UnloadMovie();
      this._dataSource?.SetActiveState(false);
    }

    private void LoadMovie()
    {
      if (this._isMovieLoaded)
        return;
      this._gauntletMovie = this._gauntletLayer.LoadMovie(nameof (CustomBattleScreen), (ViewModel) this._dataSource);
      this._isMovieLoaded = true;
    }

    private void UnloadMovie()
    {
      if (!this._isMovieLoaded)
        return;
      this._gauntletLayer.ReleaseMovie(this._gauntletMovie);
      this._gauntletMovie = (IGauntletMovie) null;
      this._isMovieLoaded = false;
      ((ScreenLayer) this._gauntletLayer).IsFocusLayer = false;
      ScreenManager.TryLoseFocus((ScreenLayer) this._gauntletLayer);
    }
  }
}
