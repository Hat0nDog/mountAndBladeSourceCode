// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ManagedOptions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public class ManagedOptions
  {
    public static ManagedOptions.OnManagedOptionChangedDelegate OnManagedOptionChanged;

    public static float GetConfig(ManagedOptions.ManagedOptionsType type)
    {
      switch (type)
      {
        case ManagedOptions.ManagedOptionsType.Language:
          return (float) LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode).IndexOf(BannerlordConfig.Language);
        case ManagedOptions.ManagedOptionsType.ControlBlockDirection:
          return (float) BannerlordConfig.DefendDirectionControl;
        case ManagedOptions.ManagedOptionsType.ControlAttackDirection:
          return (float) BannerlordConfig.AttackDirectionControl;
        case ManagedOptions.ManagedOptionsType.NumberOfCorpses:
          return (float) BannerlordConfig.NumberOfCorpses;
        case ManagedOptions.ManagedOptionsType.BattleSize:
          return (float) BannerlordConfig.BattleSize;
        case ManagedOptions.ManagedOptionsType.TurnCameraWithHorseInFirstPerson:
          return (float) BannerlordConfig.TurnCameraWithHorseInFirstPerson;
        case ManagedOptions.ManagedOptionsType.ShowBlood:
          return BannerlordConfig.ShowBlood ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.ShowAttackDirection:
          return BannerlordConfig.DisplayAttackDirection ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.ShowTargetingReticle:
          return BannerlordConfig.DisplayTargetingReticule ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity:
          return BannerlordConfig.FriendlyTroopsBannerOpacity;
        case ManagedOptions.ManagedOptionsType.ReportDamage:
          return BannerlordConfig.ReportDamage ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.ReportCasualtiesType:
          return (float) BannerlordConfig.ReportCasualtiesType;
        case ManagedOptions.ManagedOptionsType.ReportExperience:
          return BannerlordConfig.ReportExperience ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.ReportPersonalDamage:
          return BannerlordConfig.ReportPersonalDamage ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.FirstPersonFov:
          return BannerlordConfig.FirstPersonFov;
        case ManagedOptions.ManagedOptionsType.CombatCameraDistance:
          return BannerlordConfig.CombatCameraDistance;
        case ManagedOptions.ManagedOptionsType.EnableDamageTakenVisuals:
          return BannerlordConfig.EnableDamageTakenVisuals ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.EnableDeathIcon:
          return BannerlordConfig.EnableDeathIcon ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.EnableNetworkAlertIcons:
          return BannerlordConfig.EnableNetworkAlertIcons ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.ForceVSyncInMenus:
          return BannerlordConfig.ForceVSyncInMenus ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.EnableVerticalAimCorrection:
          return BannerlordConfig.EnableVerticalAimCorrection ? 1f : 0.0f;
        case ManagedOptions.ManagedOptionsType.UIScale:
          return BannerlordConfig.UIScale;
        case ManagedOptions.ManagedOptionsType.CrosshairType:
          return (float) BannerlordConfig.CrosshairType;
        case ManagedOptions.ManagedOptionsType.OrderType:
          return (float) BannerlordConfig.OrderType;
        case ManagedOptions.ManagedOptionsType.AutoTrackAttackedSettlements:
          return (float) BannerlordConfig.AutoTrackAttackedSettlements;
        default:
          return 0.0f;
      }
    }

    public static float GetDefaultConfig(ManagedOptions.ManagedOptionsType type)
    {
      switch (type)
      {
        case ManagedOptions.ManagedOptionsType.Language:
          return (float) LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode).IndexOf("English");
        case ManagedOptions.ManagedOptionsType.ControlBlockDirection:
          return 0.0f;
        case ManagedOptions.ManagedOptionsType.ControlAttackDirection:
          return 1f;
        case ManagedOptions.ManagedOptionsType.NumberOfCorpses:
          return 3f;
        case ManagedOptions.ManagedOptionsType.BattleSize:
          return 2f;
        case ManagedOptions.ManagedOptionsType.TurnCameraWithHorseInFirstPerson:
          return 2f;
        case ManagedOptions.ManagedOptionsType.ShowBlood:
          return 1f;
        case ManagedOptions.ManagedOptionsType.ShowAttackDirection:
          return 1f;
        case ManagedOptions.ManagedOptionsType.ShowTargetingReticle:
          return 1f;
        case ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity:
          return 1f;
        case ManagedOptions.ManagedOptionsType.ReportDamage:
          return 1f;
        case ManagedOptions.ManagedOptionsType.ReportCasualtiesType:
          return 0.0f;
        case ManagedOptions.ManagedOptionsType.ReportExperience:
          return 1f;
        case ManagedOptions.ManagedOptionsType.ReportPersonalDamage:
          return 1f;
        case ManagedOptions.ManagedOptionsType.FirstPersonFov:
          return 65f;
        case ManagedOptions.ManagedOptionsType.CombatCameraDistance:
          return 1f;
        case ManagedOptions.ManagedOptionsType.EnableDamageTakenVisuals:
          return 1f;
        case ManagedOptions.ManagedOptionsType.EnableDeathIcon:
          return 1f;
        case ManagedOptions.ManagedOptionsType.EnableNetworkAlertIcons:
          return 1f;
        case ManagedOptions.ManagedOptionsType.ForceVSyncInMenus:
          return 1f;
        case ManagedOptions.ManagedOptionsType.EnableVerticalAimCorrection:
          return 1f;
        case ManagedOptions.ManagedOptionsType.UIScale:
          return 1f;
        case ManagedOptions.ManagedOptionsType.CrosshairType:
          return (float) BannerlordConfig.CrosshairType;
        case ManagedOptions.ManagedOptionsType.OrderType:
          return 0.0f;
        case ManagedOptions.ManagedOptionsType.AutoTrackAttackedSettlements:
          return (float) BannerlordConfig.AutoTrackAttackedSettlements;
        default:
          return 0.0f;
      }
    }

    [MBCallback]
    internal static int GetConfigCount() => 25;

    [MBCallback]
    internal static float GetConfigValue(int type) => ManagedOptions.GetConfig((ManagedOptions.ManagedOptionsType) type);

    public static void SetConfig(ManagedOptions.ManagedOptionsType type, float value)
    {
      switch (type)
      {
        case ManagedOptions.ManagedOptionsType.Language:
          List<string> languageIds = LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode);
          BannerlordConfig.Language = (double) value < 0.0 || (double) value >= (double) languageIds.Count ? languageIds[0] : languageIds[(int) value];
          break;
        case ManagedOptions.ManagedOptionsType.ControlBlockDirection:
          BannerlordConfig.DefendDirectionControl = (int) value;
          break;
        case ManagedOptions.ManagedOptionsType.ControlAttackDirection:
          BannerlordConfig.AttackDirectionControl = (int) value;
          break;
        case ManagedOptions.ManagedOptionsType.NumberOfCorpses:
          BannerlordConfig.NumberOfCorpses = (int) value;
          break;
        case ManagedOptions.ManagedOptionsType.BattleSize:
          BannerlordConfig.BattleSize = (int) value;
          break;
        case ManagedOptions.ManagedOptionsType.TurnCameraWithHorseInFirstPerson:
          BannerlordConfig.TurnCameraWithHorseInFirstPerson = (int) value;
          break;
        case ManagedOptions.ManagedOptionsType.ShowBlood:
          BannerlordConfig.ShowBlood = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.ShowAttackDirection:
          BannerlordConfig.DisplayAttackDirection = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.ShowTargetingReticle:
          BannerlordConfig.DisplayTargetingReticule = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.FriendlyTroopsBannerOpacity:
          BannerlordConfig.FriendlyTroopsBannerOpacity = value;
          break;
        case ManagedOptions.ManagedOptionsType.ReportDamage:
          BannerlordConfig.ReportDamage = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.ReportCasualtiesType:
          BannerlordConfig.ReportCasualtiesType = (int) value;
          break;
        case ManagedOptions.ManagedOptionsType.ReportExperience:
          BannerlordConfig.ReportExperience = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.ReportPersonalDamage:
          BannerlordConfig.ReportPersonalDamage = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.FirstPersonFov:
          BannerlordConfig.FirstPersonFov = value;
          break;
        case ManagedOptions.ManagedOptionsType.CombatCameraDistance:
          BannerlordConfig.CombatCameraDistance = value;
          break;
        case ManagedOptions.ManagedOptionsType.EnableDamageTakenVisuals:
          BannerlordConfig.EnableDamageTakenVisuals = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.EnableDeathIcon:
          BannerlordConfig.EnableDeathIcon = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.EnableNetworkAlertIcons:
          BannerlordConfig.EnableNetworkAlertIcons = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.ForceVSyncInMenus:
          BannerlordConfig.ForceVSyncInMenus = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.EnableVerticalAimCorrection:
          BannerlordConfig.EnableVerticalAimCorrection = (double) value != 0.0;
          break;
        case ManagedOptions.ManagedOptionsType.UIScale:
          BannerlordConfig.UIScale = value;
          break;
        case ManagedOptions.ManagedOptionsType.CrosshairType:
          BannerlordConfig.CrosshairType = (int) value;
          break;
        case ManagedOptions.ManagedOptionsType.OrderType:
          BannerlordConfig.OrderType = (int) value;
          break;
        case ManagedOptions.ManagedOptionsType.AutoTrackAttackedSettlements:
          BannerlordConfig.AutoTrackAttackedSettlements = (int) value;
          break;
      }
      if (ManagedOptions.OnManagedOptionChanged == null)
        return;
      ManagedOptions.OnManagedOptionChanged(type);
    }

    public static void SaveConfig() => BannerlordConfig.Save();

    public static void ApplyConfigChanges()
    {
    }

    public enum ManagedOptionsType
    {
      Language,
      ControlBlockDirection,
      ControlAttackDirection,
      NumberOfCorpses,
      BattleSize,
      TurnCameraWithHorseInFirstPerson,
      ShowBlood,
      ShowAttackDirection,
      ShowTargetingReticle,
      FriendlyTroopsBannerOpacity,
      ReportDamage,
      ReportCasualtiesType,
      ReportExperience,
      ReportPersonalDamage,
      FirstPersonFov,
      CombatCameraDistance,
      EnableDamageTakenVisuals,
      EnableDeathIcon,
      EnableNetworkAlertIcons,
      ForceVSyncInMenus,
      EnableVerticalAimCorrection,
      UIScale,
      CrosshairType,
      OrderType,
      AutoTrackAttackedSettlements,
      ManagedOptionTypeCount,
    }

    public delegate void OnManagedOptionChangedDelegate(
      ManagedOptions.ManagedOptionsType changedManagedOptionsType);
  }
}
