// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MBMusicManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using psai.net;
using System.Threading;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class MBMusicManager
  {
    private const float DefaultFadeOutDurationInSeconds = 3f;
    private MBMusicManager.BattleMusicMode _battleMode;
    private MBMusicManager.CampaignMusicMode _campaignMode;
    private IMusicHandler _campaignMusicHandler;
    private IMusicHandler _battleMusicHandler;
    private IMusicHandler _silencedMusicHandler;
    private IMusicHandler _activeMusicHandler;
    private static bool _initialized;
    private static bool _creationCompleted;

    public static MBMusicManager Current { get; private set; }

    public MusicMode CurrentMode { get; private set; }

    private MBMusicManager()
    {
      if (NativeConfig.DisableSound)
        return;
      int num = (int) PsaiCore.Instance.LoadSoundtrackFromProjectFile(BasePath.Name + "music/soundtrack.xml");
    }

    public static bool isCreationCompleted() => MBMusicManager._creationCompleted;

    private static void ProcessCreation(object callback)
    {
      MBMusicManager.Current = new MBMusicManager();
      MusicParameters.LoadFromXml();
      MBMusicManager._creationCompleted = true;
    }

    public static void Create() => ThreadPool.QueueUserWorkItem(new WaitCallback(MBMusicManager.ProcessCreation));

    public static void Initialize()
    {
      if (MBMusicManager._initialized)
        return;
      MBMusicManager.Current._battleMode = new MBMusicManager.BattleMusicMode();
      MBMusicManager.Current._campaignMode = new MBMusicManager.CampaignMusicMode();
      MBMusicManager.Current.CurrentMode = MusicMode.Paused;
      MBMusicManager.Current.ActivateMenuMode();
      MBMusicManager._initialized = true;
    }

    public void OnCampaignMusicHandlerInit(IMusicHandler campaignMusicHandler)
    {
      this._campaignMusicHandler = campaignMusicHandler;
      this._activeMusicHandler = this._campaignMusicHandler;
    }

    public void OnCampaignMusicHandlerFinalize()
    {
      this._campaignMusicHandler = (IMusicHandler) null;
      this.CheckActiveHandler();
    }

    public void OnBattleMusicHandlerInit(IMusicHandler battleMusicHandler)
    {
      this._battleMusicHandler = battleMusicHandler;
      this._activeMusicHandler = this._battleMusicHandler;
    }

    public void OnBattleMusicHandlerFinalize()
    {
      this._battleMusicHandler = (IMusicHandler) null;
      this.CheckActiveHandler();
    }

    public void OnSilencedMusicHandlerInit(IMusicHandler silencedMusicHandler)
    {
      this._silencedMusicHandler = silencedMusicHandler;
      this._activeMusicHandler = this._silencedMusicHandler;
    }

    public void OnSilencedMusicHandlerFinalize()
    {
      this._silencedMusicHandler = (IMusicHandler) null;
      this.CheckActiveHandler();
    }

    private void CheckActiveHandler() => this._activeMusicHandler = this._battleMusicHandler ?? this._silencedMusicHandler ?? this._campaignMusicHandler;

    private void ActivateMenuMode()
    {
      this.CurrentMode = MusicMode.Menu;
      int num = (int) PsaiCore.Instance.MenuModeEnter(5, 0.5f);
    }

    private void DeactivateMenuMode()
    {
      int num = (int) PsaiCore.Instance.MenuModeLeave();
      this.CurrentMode = MusicMode.Paused;
    }

    public void ActivateBattleMode() => this.CurrentMode = MusicMode.Battle;

    public void DeactivateBattleMode()
    {
      int num = (int) PsaiCore.Instance.StopMusic(true, 3f);
      this.CurrentMode = MusicMode.Paused;
    }

    public void ActivateCampaignMode() => this.CurrentMode = MusicMode.Campaign;

    public void DeactivateCampaignMode()
    {
      int num = (int) PsaiCore.Instance.StopMusic(true, 3f);
      this.CurrentMode = MusicMode.Paused;
    }

    public void DeactivateCurrentMode()
    {
      switch (this.CurrentMode)
      {
        case MusicMode.Campaign:
          this.DeactivateCampaignMode();
          break;
        case MusicMode.Battle:
          this.DeactivateBattleMode();
          break;
      }
    }

    public void StartTheme(MusicTheme theme, float startIntensity, bool queueEndSegment = false)
    {
      int num1 = (int) PsaiCore.Instance.TriggerMusicTheme((int) theme, startIntensity);
      if (!queueEndSegment)
        return;
      int num2 = (int) PsaiCore.Instance.StopMusic(false, 3f);
    }

    public void StartThemeWithConstantIntensity(MusicTheme theme, bool queueEndSegment = false)
    {
      int num = (int) PsaiCore.Instance.HoldCurrentIntensity(true);
      this.StartTheme(theme, 0.0f, queueEndSegment);
    }

    public void ForceStopThemeWithFadeOut()
    {
      int num = (int) PsaiCore.Instance.StopMusic(true, 3f);
    }

    public void ChangeCurrentThemeIntensity(float deltaIntensity)
    {
      int currentIntensity = (int) PsaiCore.Instance.AddToCurrentIntensity(deltaIntensity);
    }

    public void Update(float dt)
    {
      if (Input.DebugInput.IsHotKeyPressed("ResetMusicParameters"))
        MusicParameters.LoadFromXml();
      if (GameStateManager.Current != null && GameStateManager.Current.ActiveState != null)
      {
        GameState activeState = GameStateManager.Current.ActiveState;
        switch (this.CurrentMode)
        {
          case MusicMode.Paused:
            if (activeState.IsMusicMenuState)
            {
              this.ActivateMenuMode();
              break;
            }
            break;
          case MusicMode.Menu:
            if (!activeState.IsMusicMenuState)
            {
              this.DeactivateMenuMode();
              break;
            }
            break;
        }
      }
      if (this._activeMusicHandler != null)
        this._activeMusicHandler.OnUpdated(dt);
      int num = (int) PsaiCore.Instance.Update();
    }

    public MusicTheme GetSiegeTheme(CultureCode cultureCode) => this._battleMode.GetSiegeTheme(cultureCode);

    public MusicTheme GetBattleTheme(
      CultureCode cultureCode,
      int battleSize,
      out bool isPaganBattle)
    {
      return this._battleMode.GetBattleTheme(cultureCode, battleSize, out isPaganBattle);
    }

    public MusicTheme GetBattleEndTheme(CultureCode cultureCode, bool isVictory) => this._battleMode.GetBattleEndTheme(cultureCode, isVictory);

    public MusicTheme GetBattleTurnsOneSideTheme(
      CultureCode cultureCode,
      bool isPositive,
      bool isPaganBattle)
    {
      return isPaganBattle ? (!isPositive ? MusicTheme.PaganTurnsNegative : MusicTheme.PaganTurnsPositive) : (!isPositive ? MusicTheme.BattleTurnsNegative : MusicTheme.BattleTurnsPositive);
    }

    public MusicTheme GetCampaignMusicTheme(
      CultureCode cultureCode,
      bool isDark,
      bool isWarMode)
    {
      MusicTheme musicTheme = MusicTheme.None;
      if (!isDark & isWarMode)
        musicTheme = this._campaignMode.GetCampaignDramaticThemeWithCulture(cultureCode);
      return musicTheme != MusicTheme.None ? musicTheme : this._campaignMode.GetCampaignTheme(cultureCode, isDark);
    }

    private class CampaignMusicMode
    {
      private const float DefaultSelectionFactorForFactionSpecificCampaignTheme = 0.35f;
      private const float SelectionFactorDecayAmountForFactionSpecificCampaignTheme = 0.1f;
      private const float SelectionFactorGrowthAmountForFactionSpecificCampaignTheme = 0.1f;
      private float _factionSpecificCampaignThemeSelectionFactor;
      private float _factionSpecificCampaignDramaticThemeSelectionFactor;

      public CampaignMusicMode()
      {
        this._factionSpecificCampaignThemeSelectionFactor = 0.35f;
        this._factionSpecificCampaignDramaticThemeSelectionFactor = 0.35f;
      }

      public MusicTheme GetCampaignTheme(CultureCode cultureCode, bool isDark)
      {
        if (isDark)
          return MusicTheme.CampaignDark;
        MusicTheme themeWithCulture = this.GetCampaignThemeWithCulture(cultureCode);
        MusicTheme musicTheme;
        if (themeWithCulture == MusicTheme.None)
        {
          musicTheme = MusicTheme.CampaignStandard;
          this._factionSpecificCampaignThemeSelectionFactor += 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificCampaignThemeSelectionFactor);
        }
        else
        {
          musicTheme = themeWithCulture;
          this._factionSpecificCampaignThemeSelectionFactor -= 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificCampaignThemeSelectionFactor);
        }
        return musicTheme;
      }

      private MusicTheme GetCampaignThemeWithCulture(CultureCode cultureCode)
      {
        if ((double) MBRandom.RandomFloat <= (double) this._factionSpecificCampaignThemeSelectionFactor)
        {
          this._factionSpecificCampaignThemeSelectionFactor -= 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificCampaignThemeSelectionFactor);
          switch (cultureCode)
          {
            case CultureCode.Empire:
              return (double) MBRandom.RandomFloat >= 0.5 ? MusicTheme.EmpireCampaignB : MusicTheme.EmpireCampaignA;
            case CultureCode.Sturgia:
              return MusicTheme.SturgiaCampaignA;
            case CultureCode.Aserai:
              return MusicTheme.AseraiCampaignA;
            case CultureCode.Vlandia:
              return MusicTheme.VlandiaCampaignA;
            case CultureCode.Khuzait:
              return MusicTheme.KhuzaitCampaignA;
            case CultureCode.Battania:
              return MusicTheme.BattaniaCampaignA;
          }
        }
        return MusicTheme.None;
      }

      public MusicTheme GetCampaignDramaticThemeWithCulture(CultureCode cultureCode)
      {
        if ((double) MBRandom.RandomFloat <= (double) this._factionSpecificCampaignDramaticThemeSelectionFactor)
        {
          this._factionSpecificCampaignDramaticThemeSelectionFactor -= 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificCampaignDramaticThemeSelectionFactor);
          switch (cultureCode)
          {
            case CultureCode.Empire:
              return MusicTheme.EmpireCampaignDramatic;
            case CultureCode.Sturgia:
              return MusicTheme.SturgiaCampaignDramatic;
            case CultureCode.Aserai:
              return MusicTheme.AseraiCampaignDramatic;
            case CultureCode.Vlandia:
              return MusicTheme.VlandiaCampaignDramatic;
            case CultureCode.Khuzait:
              return MusicTheme.KhuzaitCampaignDramatic;
            case CultureCode.Battania:
              return MusicTheme.BattaniaCampaignDramatic;
          }
        }
        this._factionSpecificCampaignDramaticThemeSelectionFactor += 0.1f;
        MBMath.ClampUnit(ref this._factionSpecificCampaignDramaticThemeSelectionFactor);
        return MusicTheme.None;
      }
    }

    private class BattleMusicMode
    {
      private const float DefaultSelectionFactorForFactionSpecificBattleTheme = 0.35f;
      private const float SelectionFactorDecayAmountForFactionSpecificBattleTheme = 0.1f;
      private const float SelectionFactorGrowthAmountForFactionSpecificBattleTheme = 0.1f;
      private const float DefaultSelectionFactorForFactionSpecificVictoryTheme = 0.65f;
      private float _factionSpecificBattleThemeSelectionFactor;
      private float _factionSpecificSiegeThemeSelectionFactor;

      public BattleMusicMode()
      {
        this._factionSpecificBattleThemeSelectionFactor = 0.35f;
        this._factionSpecificSiegeThemeSelectionFactor = 0.35f;
      }

      private MusicTheme GetBattleThemeWithCulture(
        CultureCode cultureCode,
        out bool isPaganBattle)
      {
        isPaganBattle = false;
        MusicTheme musicTheme = MusicTheme.None;
        if ((double) MBRandom.RandomFloat <= (double) this._factionSpecificBattleThemeSelectionFactor)
        {
          this._factionSpecificBattleThemeSelectionFactor -= 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificBattleThemeSelectionFactor);
          switch (cultureCode)
          {
            case CultureCode.Sturgia:
            case CultureCode.Aserai:
            case CultureCode.Khuzait:
            case CultureCode.Battania:
              isPaganBattle = true;
              musicTheme = (double) MBRandom.RandomFloat < 0.5 ? MusicTheme.BattlePaganA : MusicTheme.BattlePaganB;
              break;
            default:
              musicTheme = (double) MBRandom.RandomFloat < 0.5 ? MusicTheme.CombatA : MusicTheme.CombatB;
              break;
          }
        }
        return musicTheme;
      }

      private MusicTheme GetSiegeThemeWithCulture(CultureCode cultureCode)
      {
        MusicTheme musicTheme = MusicTheme.None;
        if ((double) MBRandom.RandomFloat <= (double) this._factionSpecificSiegeThemeSelectionFactor)
        {
          this._factionSpecificSiegeThemeSelectionFactor -= 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificSiegeThemeSelectionFactor);
          switch (cultureCode)
          {
            case CultureCode.Sturgia:
            case CultureCode.Aserai:
            case CultureCode.Khuzait:
            case CultureCode.Battania:
              musicTheme = MusicTheme.PaganSiege;
              break;
          }
        }
        return musicTheme;
      }

      private MusicTheme GetVictoryThemeForCulture(CultureCode cultureCode)
      {
        if ((double) MBRandom.RandomFloat <= 0.649999976158142)
        {
          switch (cultureCode)
          {
            case CultureCode.Empire:
              return MusicTheme.EmpireVictory;
            case CultureCode.Sturgia:
              return MusicTheme.SturgiaVictory;
            case CultureCode.Aserai:
              return MusicTheme.AseraiVictory;
            case CultureCode.Vlandia:
              return MusicTheme.VlandiaVictory;
            case CultureCode.Khuzait:
              return MusicTheme.KhuzaitVictory;
            case CultureCode.Battania:
              return MusicTheme.BattaniaVictory;
          }
        }
        return MusicTheme.None;
      }

      public MusicTheme GetBattleTheme(
        CultureCode culture,
        int battleSize,
        out bool isPaganBattle)
      {
        MusicTheme themeWithCulture = this.GetBattleThemeWithCulture(culture, out isPaganBattle);
        MusicTheme musicTheme;
        if (themeWithCulture == MusicTheme.None)
        {
          musicTheme = (double) battleSize < (double) MusicParameters.SmallBattleTreshold - (double) MusicParameters.SmallBattleTreshold * 0.200000002980232 * (double) MBRandom.RandomFloat ? MusicTheme.BattleSmall : MusicTheme.BattleMedium;
          this._factionSpecificBattleThemeSelectionFactor += 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificBattleThemeSelectionFactor);
        }
        else
        {
          musicTheme = themeWithCulture;
          this._factionSpecificBattleThemeSelectionFactor -= 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificBattleThemeSelectionFactor);
        }
        return musicTheme;
      }

      public MusicTheme GetSiegeTheme(CultureCode culture)
      {
        MusicTheme themeWithCulture = this.GetSiegeThemeWithCulture(culture);
        MusicTheme musicTheme;
        if (themeWithCulture == MusicTheme.None)
        {
          musicTheme = MusicTheme.BattleSiege;
          this._factionSpecificSiegeThemeSelectionFactor += 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificSiegeThemeSelectionFactor);
        }
        else
        {
          musicTheme = themeWithCulture;
          this._factionSpecificSiegeThemeSelectionFactor -= 0.1f;
          MBMath.ClampUnit(ref this._factionSpecificSiegeThemeSelectionFactor);
        }
        return musicTheme;
      }

      public MusicTheme GetBattleEndTheme(CultureCode culture, bool isVictorious)
      {
        MusicTheme musicTheme;
        if (isVictorious)
        {
          MusicTheme victoryThemeForCulture = this.GetVictoryThemeForCulture(culture);
          musicTheme = victoryThemeForCulture != MusicTheme.None ? victoryThemeForCulture : MusicTheme.BattleVictory;
        }
        else
          musicTheme = MusicTheme.BattleDefeat;
        return musicTheme;
      }
    }
  }
}
