// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BannerlordConfig
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Engine;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade
{
  public static class BannerlordConfig
  {
    private static int[] _battleSizes = new int[6]
    {
      200,
      300,
      400,
      500,
      600,
      1000
    };
    public const string DefaultLanguage = "English";
    public const int DefaultAttackDirectionControl = 1;
    public const int DefaultDefendDirectionControl = 0;
    public const int DefaultNumberOfCorpses = 3;
    public const bool DefaultShowBlood = true;
    public const bool DefaultDisplayAttackDirection = true;
    public const bool DefaultDisplayTargetingReticule = true;
    public const bool DefaultForceVSyncInMenus = true;
    public const int DefaultBattleSize = 2;
    public const float DefaultBattleSizeMultiplier = 0.5f;
    public const float DefaultFirstPersonFov = 65f;
    public const float DefaultUIScale = 1f;
    public const float DefaultCombatCameraDistance = 1f;
    public const int DefaultCombatAI = 0;
    public const int DefaultTurnCameraWithHorseInFirstPerson = 2;
    public const float DefaultFriendlyTroopsBannerOpacity = 1f;
    public const bool DefaultReportDamage = true;
    public const int DefaultReportCasualtiesType = 0;
    public const int DefaultAutoTrackAttackedSettlements = 0;
    public const bool DefaultReportPersonalDamage = true;
    public const bool DefaultReportExperience = true;
    public const bool DefaultEnableDamageTakenVisuals = true;
    public const bool DefaultEnableDeathIcon = true;
    public const bool DefaultEnableNetworkAlertIcons = true;
    public const bool DefaultEnableVerticalAimCorrection = true;
    public const int DefaultCrosshairType = 0;
    public const int DefaultOrderType = 0;
    private static string _language = "English";

    public static int MinBattleSize => BannerlordConfig._battleSizes[0];

    public static int MaxBattleSize => BannerlordConfig._battleSizes[BannerlordConfig._battleSizes.Length - 1];

    public static void Initialize()
    {
      string str1 = Utilities.LoadConfigFile(nameof (BannerlordConfig));
      if (string.IsNullOrEmpty(str1))
      {
        BannerlordConfig.Save();
      }
      else
      {
        bool flag = false;
        string str2 = str1;
        char[] chArray1 = new char[1]{ '\n' };
        foreach (string str3 in str2.Split(chArray1))
        {
          char[] chArray2 = new char[1]{ '=' };
          string[] strArray = str3.Split(chArray2);
          PropertyInfo property = typeof (BannerlordConfig).GetProperty(strArray[0]);
          if (property == (PropertyInfo) null)
          {
            flag = true;
          }
          else
          {
            string s = strArray[1];
            try
            {
              if (property.PropertyType == typeof (string))
                property.SetValue((object) null, (object) s);
              else if (property.PropertyType == typeof (float))
              {
                float result;
                if (float.TryParse(s, out result))
                  property.SetValue((object) null, (object) result);
                else
                  flag = true;
              }
              else if (property.PropertyType == typeof (int))
              {
                int result;
                if (int.TryParse(s, out result))
                {
                  BannerlordConfig.ConfigPropertyInt customAttribute = property.GetCustomAttribute<BannerlordConfig.ConfigPropertyInt>();
                  if (customAttribute == null || customAttribute.IsValidValue(result))
                    property.SetValue((object) null, (object) result);
                  else
                    flag = true;
                }
                else
                  flag = true;
              }
              else if (property.PropertyType == typeof (bool))
              {
                bool result;
                if (bool.TryParse(s, out result))
                  property.SetValue((object) null, (object) result);
                else
                  flag = true;
              }
              else
                flag = true;
            }
            catch
            {
              flag = true;
            }
          }
        }
        if (flag)
          BannerlordConfig.Save();
        MBAPI.IMBBannerlordConfig.ValidateOptions();
      }
      MBTextManager.ChangeLanguage(BannerlordConfig.Language);
    }

    public static void Save()
    {
      Dictionary<PropertyInfo, object> dictionary = new Dictionary<PropertyInfo, object>();
      foreach (PropertyInfo property in typeof (BannerlordConfig).GetProperties())
      {
        if (property.GetCustomAttribute<BannerlordConfig.ConfigProperty>() != null)
          dictionary.Add(property, property.GetValue((object) null, (object[]) null));
      }
      string configProperties = "";
      foreach (KeyValuePair<PropertyInfo, object> keyValuePair in dictionary)
        configProperties = configProperties + keyValuePair.Key.Name + "=" + keyValuePair.Value.ToString() + "\n";
      Utilities.SaveConfigFile(nameof (BannerlordConfig), configProperties);
      MBAPI.IMBBannerlordConfig.ValidateOptions();
    }

    public static int GetRealBattleSize() => BannerlordConfig._battleSizes[BannerlordConfig.BattleSize];

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static string Language
    {
      get => BannerlordConfig._language;
      set
      {
        if (!(BannerlordConfig._language != value))
          return;
        if (MBTextManager.LanguageExistsInCurrentConfiguration(value, NativeConfig.IsDevelopmentMode) && MBTextManager.ChangeLanguage(value))
        {
          BannerlordConfig._language = value;
        }
        else
        {
          if (!MBTextManager.ChangeLanguage("English"))
            return;
          BannerlordConfig._language = "English";
        }
      }
    }

    [BannerlordConfig.ConfigPropertyInt(new int[] {0, 1, 2}, false)]
    public static int AttackDirectionControl { get; set; } = 1;

    [BannerlordConfig.ConfigPropertyInt(new int[] {0, 1, 2}, false)]
    public static int DefendDirectionControl { get; set; } = 0;

    [BannerlordConfig.ConfigPropertyInt(new int[] {0, 1, 2, 3, 4, 5}, false)]
    public static int NumberOfCorpses { get; set; } = 3;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool ShowBlood { get; set; } = true;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool DisplayAttackDirection { get; set; } = true;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool DisplayTargetingReticule { get; set; } = true;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool ForceVSyncInMenus { get; set; } = true;

    [BannerlordConfig.ConfigPropertyInt(new int[] {0, 1, 2, 3, 4, 5}, false)]
    public static int BattleSize { get; set; } = 2;

    public static float CivilianAgentCount => (float) BannerlordConfig.GetRealBattleSize() * 0.5f;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static float FirstPersonFov { get; set; } = 65f;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static float UIScale { get; set; } = 1f;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static float CombatCameraDistance { get; set; } = 1f;

    [BannerlordConfig.ConfigPropertyInt(new int[] {0, 1, 2, 3}, false)]
    public static int TurnCameraWithHorseInFirstPerson { get; set; } = 2;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool ReportDamage { get; set; } = true;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static float FriendlyTroopsBannerOpacity { get; set; } = 1f;

    [BannerlordConfig.ConfigPropertyInt(new int[] {0, 1, 2}, false)]
    public static int ReportCasualtiesType { get; set; } = 0;

    [BannerlordConfig.ConfigPropertyInt(new int[] {0, 1, 2}, false)]
    public static int AutoTrackAttackedSettlements { get; set; } = 0;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool ReportPersonalDamage { get; set; } = true;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool ReportExperience { get; set; } = true;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool EnableDamageTakenVisuals { get; set; } = true;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool EnableVerticalAimCorrection { get; set; } = true;

    [BannerlordConfig.ConfigPropertyInt(new int[] {0, 1}, false)]
    public static int CrosshairType { get; set; } = 0;

    [BannerlordConfig.ConfigPropertyInt(new int[] {0, 1}, false)]
    public static int OrderType { get; set; } = 0;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool EnableDeathIcon { get; set; } = true;

    [BannerlordConfig.ConfigPropertyUnbounded]
    public static bool EnableNetworkAlertIcons { get; set; } = true;

    private interface IConfigPropertyBoundChecker<T>
    {
    }

    private abstract class ConfigProperty : Attribute
    {
    }

    private sealed class ConfigPropertyInt : BannerlordConfig.ConfigProperty
    {
      private int[] _possibleValues;
      private bool _isRange;

      public ConfigPropertyInt(int[] possibleValues, bool isRange = false)
      {
        this._possibleValues = possibleValues;
        this._isRange = isRange;
        int num = this._isRange ? 1 : 0;
      }

      public bool IsValidValue(int value)
      {
        if (this._isRange)
          return value >= this._possibleValues[0] && value <= this._possibleValues[1];
        foreach (int possibleValue in this._possibleValues)
        {
          if (possibleValue == value)
            return true;
        }
        return false;
      }
    }

    private sealed class ConfigPropertyUnbounded : BannerlordConfig.ConfigProperty
    {
    }
  }
}
