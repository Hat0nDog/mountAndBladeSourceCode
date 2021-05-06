// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerOptionsExtensions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.MountAndBlade
{
  public static class MultiplayerOptionsExtensions
  {
    public static string GetValueText(
      this MultiplayerOptions.OptionType optionType,
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      switch (optionType.GetOptionProperty().OptionValueType)
      {
        case MultiplayerOptions.OptionValueType.Bool:
          return optionType.GetBoolValue(mode).ToString();
        case MultiplayerOptions.OptionValueType.Integer:
        case MultiplayerOptions.OptionValueType.Enum:
          return optionType.GetIntValue(mode).ToString();
        case MultiplayerOptions.OptionValueType.String:
          return optionType.GetStrValue(mode);
        default:
          return (string) null;
      }
    }

    public static bool GetBoolValue(
      this MultiplayerOptions.OptionType optionType,
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      int num;
      MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).GetValue(out num);
      return num == 1;
    }

    public static int GetIntValue(
      this MultiplayerOptions.OptionType optionType,
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      int num;
      MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).GetValue(out num);
      return num;
    }

    public static string GetStrValue(
      this MultiplayerOptions.OptionType optionType,
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      string str;
      MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).GetValue(out str);
      return str;
    }

    public static void SetValue(
      this MultiplayerOptions.OptionType optionType,
      bool value,
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).UpdateValue(value ? 1 : 0);
    }

    public static void SetValue(
      this MultiplayerOptions.OptionType optionType,
      int value,
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).UpdateValue(value);
    }

    public static void SetValue(
      this MultiplayerOptions.OptionType optionType,
      string value,
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      MultiplayerOptions.Instance.GetOptionFromOptionType(optionType, mode).UpdateValue(value);
    }

    public static int GetMinimumValue(this MultiplayerOptions.OptionType optionType) => optionType.GetOptionProperty().BoundsMin;

    public static int GetMaximumValue(this MultiplayerOptions.OptionType optionType) => optionType.GetOptionProperty().BoundsMax;

    public static MultiplayerOptionsProperty GetOptionProperty(
      this MultiplayerOptions.OptionType optionType)
    {
      return (MultiplayerOptionsProperty) ((IEnumerable<object>) optionType.GetType().GetField(optionType.ToString()).GetCustomAttributes(typeof (MultiplayerOptionsProperty), false)).Single<object>();
    }
  }
}
