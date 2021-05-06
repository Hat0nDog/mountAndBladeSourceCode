// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Options.ManagedOptions.ManagedSelectionOptionData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Options;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Options.ManagedOptions
{
  public class ManagedSelectionOptionData : ManagedOptionData, ISelectionOptionData, IOptionData
  {
    private readonly int _selectableOptionsLimit;
    private readonly IEnumerable<SelectionData> _selectableOptionNames;

    public ManagedSelectionOptionData(TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType type)
      : base(type)
    {
      this._selectableOptionsLimit = ManagedSelectionOptionData.GetOptionsLimit(type);
      this._selectableOptionNames = ManagedSelectionOptionData.GetOptionNames(type);
    }

    public int GetSelectableOptionsLimit() => this._selectableOptionsLimit;

    public IEnumerable<SelectionData> GetSelectableOptionNames() => this._selectableOptionNames;

    public static int GetOptionsLimit(TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType optionType)
    {
      switch (optionType)
      {
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.Language:
          return LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode).Count;
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.ControlBlockDirection:
          return 3;
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.ControlAttackDirection:
          return 3;
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.NumberOfCorpses:
          return 6;
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.BattleSize:
          return 6;
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.TurnCameraWithHorseInFirstPerson:
          return 4;
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.ReportCasualtiesType:
          return 3;
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.CrosshairType:
          return 2;
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.OrderType:
          return 2;
        case TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.AutoTrackAttackedSettlements:
          return 3;
        default:
          return 0;
      }
    }

    private static IEnumerable<SelectionData> GetOptionNames(
      TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType type)
    {
      int i1;
      if (type == TaleWorlds.MountAndBlade.ManagedOptions.ManagedOptionsType.Language)
      {
        List<string> languageIds = LocalizedTextManager.GetLanguageIds(NativeConfig.IsDevelopmentMode);
        for (i1 = 0; i1 < languageIds.Count; ++i1)
          yield return new SelectionData(false, languageIds[i1]);
        languageIds = (List<string>) null;
      }
      else
      {
        i1 = ManagedSelectionOptionData.GetOptionsLimit(type);
        string typeName = type.ToString();
        for (int i2 = 0; i2 < i1; ++i2)
          yield return new SelectionData(true, "str_options_type_" + typeName + "_" + i2.ToString());
        typeName = (string) null;
      }
    }
  }
}
