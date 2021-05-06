// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerOptions
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.ModuleManager;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerOptions
  {
    private const int PlayerCountLimitMin = 1;
    private const int PlayerCountLimitMax = 120;
    private const int PlayerCountLimitForMatchStartMin = 0;
    private const int PlayerCountLimitForMatchStartMax = 20;
    private const int MapTimeLimitMin = 1;
    private const int MapTimeLimitMax = 60;
    private const int RoundLimitMin = 1;
    private const int RoundLimitMax = 20;
    private const int RoundTimeLimitMin = 120;
    private const int RoundTimeLimitMax = 960;
    private const int RoundPreparationTimeLimitMin = 2;
    private const int RoundPreparationTimeLimitMax = 60;
    private const int RespawnPeriodMin = 1;
    private const int RespawnPeriodMax = 60;
    private const int GoldGainChangePercentageMin = -100;
    private const int GoldGainChangePercentageMax = 100;
    private const int PollAcceptThresholdMin = 0;
    private const int PollAcceptThresholdMax = 10;
    private const int BotsPerTeamLimitMin = 0;
    private const int BotsPerTeamLimitMax = 100;
    private const int BotsPerFormationLimitMin = 0;
    private const int BotsPerFormationLimitMax = 100;
    private const int FriendlyFireDamagePercentMin = 0;
    private const int FriendlyFireDamagePercentMax = 100;
    private const int GameDefinitionIdMin = -2147483648;
    private const int GameDefinitionIdMax = 2147483647;
    private static MultiplayerOptions _instance;
    private readonly MultiplayerOptions.MultiplayerOptionsContainer _current;
    private readonly MultiplayerOptions.MultiplayerOptionsContainer _next;
    private readonly MultiplayerOptions.MultiplayerOptionsContainer _ui;
    public MultiplayerOptions.OptionsCategory CurrentOptionsCategory;

    public static MultiplayerOptions Instance => MultiplayerOptions._instance ?? (MultiplayerOptions._instance = new MultiplayerOptions());

    public MultiplayerOptions()
    {
      this._current = new MultiplayerOptions.MultiplayerOptionsContainer();
      this._next = new MultiplayerOptions.MultiplayerOptionsContainer();
      this._ui = new MultiplayerOptions.MultiplayerOptionsContainer();
      MultiplayerOptions.MultiplayerOptionsContainer container = this.GetContainer();
      for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; ++optionType)
        container.CreateOption(optionType);
      List<MultiplayerGameTypeInfo> multiplayerGameTypes = Module.CurrentModule.GetMultiplayerGameTypes();
      if (multiplayerGameTypes.Count > 0)
      {
        MultiplayerGameTypeInfo multiplayerGameTypeInfo = multiplayerGameTypes[0];
        container.UpdateOptionValue(MultiplayerOptions.OptionType.GameType, multiplayerGameTypeInfo.GameType);
        container.UpdateOptionValue(MultiplayerOptions.OptionType.ClanMatchType, multiplayerGameTypes.First<MultiplayerGameTypeInfo>((Func<MultiplayerGameTypeInfo, bool>) (info => info.GameType == "Skirmish")).GameType);
        container.UpdateOptionValue(MultiplayerOptions.OptionType.Map, multiplayerGameTypeInfo.Scenes.First<string>());
      }
      container.UpdateOptionValue(MultiplayerOptions.OptionType.CultureTeam1, MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>()[0].StringId);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.CultureTeam2, MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>()[2].StringId);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.MaxNumberOfPlayers, 120);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart, 1);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.WarmupTimeLimit, 5);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.MapTimeLimit, 30);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.RoundTimeLimit, 120);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.RoundPreparationTimeLimit, 10);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.RoundTotal, 20);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.RespawnPeriodTeam1, 3);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.RespawnPeriodTeam2, 3);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.MinScoreToWinMatch, 120000);
      container.UpdateOptionValue(MultiplayerOptions.OptionType.AutoTeamBalanceThreshold, 0);
      this._current.CopyAllValuesTo(this._next);
      this._current.CopyAllValuesTo(this._ui);
    }

    public MultiplayerOptions.MultiplayerOption GetOptionFromOptionType(
      MultiplayerOptions.OptionType optionType,
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      return this.GetContainer(mode).GetOptionFromOptionType(optionType);
    }

    public void OnGameTypeChanged(
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      string str = "";
      if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.Default)
        str = MultiplayerOptions.OptionType.GameType.GetStrValue(mode);
      else if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.ClanMatch)
        str = MultiplayerOptions.OptionType.ClanMatchType.GetStrValue(mode);
      if (!(str == "FreeForAll"))
      {
        if (!(str == "TeamDeathmatch"))
        {
          if (!(str == "Duel"))
          {
            if (!(str == "Siege"))
            {
              if (!(str == "Captain"))
              {
                if (str == "Skirmish")
                  this.InitializeForSkirmish(mode);
              }
              else
                this.InitializeForCaptain(mode);
            }
            else
              this.InitializeForSiege(mode);
          }
          else
            this.InitializeForDuel(mode);
        }
        else
          this.InitializeForTeamDeathmatch(mode);
      }
      else
        this.InitializeForFreeForAll(mode);
      MultiplayerOptions.OptionType.Map.SetValue(this.GetMapList()[0]);
    }

    private void InitializeForFreeForAll(
      MultiplayerOptions.MultiplayerOptionsAccessMode mode)
    {
      MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(120, mode);
      MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.SpectatorCamera.SetValue(0, mode);
      MultiplayerOptions.OptionType.MapTimeLimit.SetValue(30, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
      MultiplayerOptions.OptionType.MinScoreToWinMatch.SetValue(120000, mode);
      MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(0, mode);
    }

    private void InitializeForTeamDeathmatch(
      MultiplayerOptions.MultiplayerOptionsAccessMode mode)
    {
      MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(120, mode);
      MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.SpectatorCamera.SetValue(0, mode);
      MultiplayerOptions.OptionType.MapTimeLimit.SetValue(30, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
      MultiplayerOptions.OptionType.MinScoreToWinMatch.SetValue(120000, mode);
      MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(1, mode);
    }

    private void InitializeForDuel(
      MultiplayerOptions.MultiplayerOptionsAccessMode mode)
    {
      MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(120, mode);
      MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.SpectatorCamera.SetValue(0, mode);
      MultiplayerOptions.OptionType.MapTimeLimit.SetValue(MultiplayerOptions.OptionType.MapTimeLimit.GetMaximumValue(), mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
      MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(0, mode);
    }

    private void InitializeForSiege(
      MultiplayerOptions.MultiplayerOptionsAccessMode mode)
    {
      MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(120, mode);
      MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(50, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(50, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.SpectatorCamera.SetValue(0, mode);
      MultiplayerOptions.OptionType.WarmupTimeLimit.SetValue(3, mode);
      MultiplayerOptions.OptionType.MapTimeLimit.SetValue(30, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(12, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(30, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
      MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(1, mode);
    }

    private void InitializeForCaptain(
      MultiplayerOptions.MultiplayerOptionsAccessMode mode)
    {
      MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(12, mode);
      MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(25, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(50, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(50, mode);
      MultiplayerOptions.OptionType.SpectatorCamera.SetValue(6, mode);
      MultiplayerOptions.OptionType.WarmupTimeLimit.SetValue(5, mode);
      MultiplayerOptions.OptionType.MapTimeLimit.SetValue(5, mode);
      MultiplayerOptions.OptionType.RoundTimeLimit.SetValue(600, mode);
      MultiplayerOptions.OptionType.RoundPreparationTimeLimit.SetValue(20, mode);
      MultiplayerOptions.OptionType.RoundTotal.SetValue(5, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
      MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(1, mode);
    }

    private void InitializeForSkirmish(
      MultiplayerOptions.MultiplayerOptionsAccessMode mode)
    {
      MultiplayerOptions.OptionType.MaxNumberOfPlayers.SetValue(12, mode);
      MultiplayerOptions.OptionType.NumberOfBotsPerFormation.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent.SetValue(50, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent.SetValue(0, mode);
      MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent.SetValue(50, mode);
      MultiplayerOptions.OptionType.SpectatorCamera.SetValue(6, mode);
      MultiplayerOptions.OptionType.WarmupTimeLimit.SetValue(5, mode);
      MultiplayerOptions.OptionType.MapTimeLimit.SetValue(5, mode);
      MultiplayerOptions.OptionType.RoundTimeLimit.SetValue(420, mode);
      MultiplayerOptions.OptionType.RoundPreparationTimeLimit.SetValue(20, mode);
      MultiplayerOptions.OptionType.RoundTotal.SetValue(5, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam1.SetValue(3, mode);
      MultiplayerOptions.OptionType.RespawnPeriodTeam2.SetValue(3, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1.SetValue(0, mode);
      MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2.SetValue(0, mode);
      MultiplayerOptions.OptionType.AutoTeamBalanceThreshold.SetValue(1, mode);
    }

    public static void InitializeFromConfigFile(string fileName)
    {
      if (fileName.IsStringNoneOrEmpty())
        return;
      foreach (string readAllLine in File.ReadAllLines(ModuleHelper.GetModuleFullPath("Native") + fileName))
        GameNetwork.HandleConsoleCommand(readAllLine);
    }

    public List<string> GetMultiplayerOptionsList(MultiplayerOptions.OptionType optionType)
    {
      List<string> stringList1 = new List<string>();
      switch (optionType)
      {
        case MultiplayerOptions.OptionType.ClanMatchType:
          stringList1 = Module.CurrentModule.GetMultiplayerGameTypes().Select<MultiplayerGameTypeInfo, string>((Func<MultiplayerGameTypeInfo, string>) (q => q.GameType)).ToList<string>();
          stringList1.Remove("FreeForAll");
          stringList1.Remove("TeamDeathmatch");
          stringList1.Remove("Duel");
          stringList1.Remove("Siege");
          break;
        case MultiplayerOptions.OptionType.GameType:
          stringList1 = Module.CurrentModule.GetMultiplayerGameTypes().Select<MultiplayerGameTypeInfo, string>((Func<MultiplayerGameTypeInfo, string>) (q => q.GameType)).ToList<string>();
          break;
        case MultiplayerOptions.OptionType.Map:
          if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.Default)
          {
            stringList1 = MultiplayerGameTypes.GetGameTypeInfo(MultiplayerOptions.OptionType.GameType.GetStrValue()).Scenes.ToList<string>();
            break;
          }
          if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.ClanMatch)
          {
            MultiplayerOptions.OptionType.ClanMatchType.GetStrValue();
            stringList1 = this.GetAvailableClanMatchScenes();
            break;
          }
          break;
        case MultiplayerOptions.OptionType.CultureTeam1:
        case MultiplayerOptions.OptionType.CultureTeam2:
          stringList1 = MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>().Where<BasicCultureObject>((Func<BasicCultureObject, bool>) (c => c.IsMainCulture)).Select<BasicCultureObject, string>((Func<BasicCultureObject, string>) (x => x.StringId)).ToList<string>();
          break;
        case MultiplayerOptions.OptionType.SpectatorCamera:
          List<string> stringList2 = new List<string>();
          stringList2.Add(SpectatorCameraTypes.LockToAnyAgent.ToString());
          SpectatorCameraTypes spectatorCameraTypes = SpectatorCameraTypes.LockToAnyPlayer;
          stringList2.Add(spectatorCameraTypes.ToString());
          spectatorCameraTypes = SpectatorCameraTypes.LockToTeamMembers;
          stringList2.Add(spectatorCameraTypes.ToString());
          spectatorCameraTypes = SpectatorCameraTypes.LockToTeamMembersView;
          stringList2.Add(spectatorCameraTypes.ToString());
          stringList1 = stringList2;
          break;
        case MultiplayerOptions.OptionType.AutoTeamBalanceThreshold:
          List<string> stringList3 = new List<string>();
          for (int index = 0; index < 6; ++index)
            stringList3.Add(((AutoTeamBalanceLimits) index).ToString());
          stringList1 = stringList3;
          break;
      }
      return stringList1;
    }

    private List<string> GetAvailableClanMatchScenes()
    {
      string[] strArray1 = new string[0];
      string[] strArray2;
      if (NetworkMain.GameClient.AvailableScenes.ScenesByGameTypes.TryGetValue(MultiplayerOptions.OptionType.ClanMatchType.GetStrValue(), out strArray2))
        strArray1 = strArray2;
      return ((IEnumerable<string>) strArray1).ToList<string>();
    }

    private MultiplayerOptions.MultiplayerOptionsContainer GetContainer(
      MultiplayerOptions.MultiplayerOptionsAccessMode mode = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
    {
      switch (mode)
      {
        case MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions:
          return this._current;
        case MultiplayerOptions.MultiplayerOptionsAccessMode.NextMapOptions:
          return this._next;
        case MultiplayerOptions.MultiplayerOptionsAccessMode.MissionUIOptions:
          return this._ui;
        default:
          return (MultiplayerOptions.MultiplayerOptionsContainer) null;
      }
    }

    public void InitializeAllOptionsFromCurrent()
    {
      this._current.CopyAllValuesTo(this._ui);
      this._current.CopyAllValuesTo(this._next);
    }

    public void InitializeAllOptionsFromNext()
    {
      this._next.CopyAllValuesTo(this._ui);
      this._next.CopyAllValuesTo(this._current);
      this.UpdateMbMultiplayerData();
    }

    public void InitializeOptionsFromUi()
    {
      this._ui.CopyAllValuesTo(this._next);
      if (Mission.Current == null)
        this._ui.CopyAllValuesTo(this._current);
      else
        this._ui.CopyImmediateEffectValuesTo(this._current);
      this.UpdateMbMultiplayerData();
    }

    private void UpdateMbMultiplayerData()
    {
      MultiplayerOptions.MultiplayerOptionsContainer container = this.GetContainer();
      container.GetOptionFromOptionType(MultiplayerOptions.OptionType.ServerName).GetValue(out MBMultiplayerData.ServerName);
      if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.Default)
        container.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType).GetValue(out MBMultiplayerData.GameType);
      else if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.ClanMatch)
        container.GetOptionFromOptionType(MultiplayerOptions.OptionType.ClanMatchType).GetValue(out MBMultiplayerData.GameType);
      container.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map).GetValue(out MBMultiplayerData.Map);
      container.GetOptionFromOptionType(MultiplayerOptions.OptionType.MaxNumberOfPlayers).GetValue(out MBMultiplayerData.PlayerCountLimit);
    }

    public List<string> GetMapList()
    {
      MultiplayerGameTypeInfo multiplayerGameTypeInfo = (MultiplayerGameTypeInfo) null;
      if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.Default)
        multiplayerGameTypeInfo = MultiplayerGameTypes.GetGameTypeInfo(MultiplayerOptions.OptionType.GameType.GetStrValue());
      else if (this.CurrentOptionsCategory == MultiplayerOptions.OptionsCategory.ClanMatch)
        multiplayerGameTypeInfo = MultiplayerGameTypes.GetGameTypeInfo(MultiplayerOptions.OptionType.ClanMatchType.GetStrValue());
      List<string> stringList = new List<string>();
      stringList.Clear();
      if (multiplayerGameTypeInfo.Scenes.Count > 0)
      {
        stringList.Add(multiplayerGameTypeInfo.Scenes[0]);
        MultiplayerOptions.OptionType.Map.SetValue(stringList[0]);
      }
      return stringList;
    }

    public string GetValueTextForOptionWithMultipleSelection(
      MultiplayerOptions.OptionType optionType)
    {
      MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
      switch (optionProperty.OptionValueType)
      {
        case MultiplayerOptions.OptionValueType.Enum:
          return Enum.ToObject(optionProperty.EnumType, optionType.GetIntValue()).ToString();
        case MultiplayerOptions.OptionValueType.String:
          return optionType.GetStrValue();
        default:
          return (string) null;
      }
    }

    public void SetValueForOptionWithMultipleSelectionFromText(
      MultiplayerOptions.OptionType optionType,
      string value)
    {
      MultiplayerOptionsProperty optionProperty = optionType.GetOptionProperty();
      switch (optionProperty.OptionValueType)
      {
        case MultiplayerOptions.OptionValueType.Enum:
          optionType.SetValue((int) Enum.Parse(optionProperty.EnumType, value));
          break;
        case MultiplayerOptions.OptionValueType.String:
          optionType.SetValue(value);
          break;
      }
      if (optionType != MultiplayerOptions.OptionType.GameType && optionType != MultiplayerOptions.OptionType.ClanMatchType)
        return;
      this.OnGameTypeChanged();
    }

    public enum MultiplayerOptionsAccessMode
    {
      CurrentMapOptions,
      NextMapOptions,
      MissionUIOptions,
    }

    public enum OptionValueType
    {
      Bool,
      Integer,
      Enum,
      String,
    }

    public enum OptionType
    {
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Changes the name of the server in the server list", 0, 0, null, false, null)] ServerName,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Welcome messages which is shown to all players when they enter the server.", 0, 0, null, false, null)] WelcomeMessage,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Never, "Sets a password that clients have to enter before connecting to the server.", 0, 0, null, false, null)] GamePassword,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Never, "Sets a password that allows players access to admin tools during the game.", 0, 0, null, false, null)] AdminPassword,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Never, "Sets ID of the private game definition.", -2147483648, 2147483647, null, false, null)] GameDefinitionId,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to start polls to kick other players.", 0, 0, null, false, null)] AllowPollsToKickPlayers,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to start polls to ban other players.", 0, 0, null, false, null)] AllowPollsToBanPlayers,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to start polls to change the current map.", 0, 0, null, false, null)] AllowPollsToChangeMaps,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Allow players to use their custom banner.", 0, 0, null, false, null)] AllowIndividualBanners,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Use animation progress dependent blocking.", 0, 0, null, false, null)] UseRealisticBlocking,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Changes the game type.", 0, 0, null, true, null)] ClanMatchType,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Changes the game type.", 0, 0, null, true, null)] GameType,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Maximum player imbalance between team 1 and team 2.", 0, 0, null, true, null)] Map,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Sets culture for team 1", 0, 0, null, true, null)] CultureTeam1,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.String, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Sets culture for team 2", 0, 0, null, true, null)] CultureTeam2,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Set the maximum amount of player allowed on the server.", 1, 120, null, false, null)] MaxNumberOfPlayers,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Set the amount of players that are needed to start the first round. If not met, players will just wait.", 0, 20, null, false, null)] MinNumberOfPlayersForMatchStart,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Amount of bots on team 1", 0, 100, null, false, null)] NumberOfBotsTeam1,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Amount of bots on team 2", 0, 100, new string[] {"Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch"}, false, null)] NumberOfBotsTeam2,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Amount of bots per formation", 0, 100, new string[] {"Captain"}, false, null)] NumberOfBotsPerFormation,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much melee damage inflicted upon a friend is dealt back to the inflictor.", 0, 100, new string[] {"Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch"}, false, null)] FriendlyFireDamageMeleeSelfPercent,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much melee damage inflicted upon a friend is actually dealt.", 0, 100, new string[] {"Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch"}, false, null)] FriendlyFireDamageMeleeFriendPercent,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much ranged damage inflicted upon a friend is dealt back to the inflictor.", 0, 100, new string[] {"Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch"}, false, null)] FriendlyFireDamageRangedSelfPercent,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "A percentage of how much ranged damage inflicted upon a friend is actually dealt.", 0, 100, new string[] {"Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege", "TeamDeathmatch"}, false, null)] FriendlyFireDamageRangedFriendPercent,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Enum, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Who can spectators look at, and how.", 0, 7, null, true, typeof (SpectatorCameraTypes))] SpectatorCamera,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Maximum duration for the warmup. In minutes.", 1, 60, null, false, null)] WarmupTimeLimit,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum duration for the map. In minutes.", 1, 60, null, false, null)] MapTimeLimit,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum duration for each round. In seconds.", 120, 960, new string[] {"Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege"}, false, null)] RoundTimeLimit,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Time available to select class/equipment. In seconds.", 2, 60, new string[] {"Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege"}, false, null)] RoundPreparationTimeLimit,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum amount of rounds before the game ends.", 1, 20, new string[] {"Battle", "NewBattle", "ClassicBattle", "Captain", "Skirmish", "Siege"}, false, null)] RoundTotal,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Wait time after death, before respawning again. In seconds.", 1, 60, new string[] {"Siege"}, false, null)] RespawnPeriodTeam1,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Wait time after death, before respawning again. In seconds.", 1, 60, new string[] {"Siege"}, false, null)] RespawnPeriodTeam2,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Gold gain multiplier from agent deaths.", -100, 100, new string[] {"Siege", "TeamDeathmatch"}, false, null)] GoldGainChangePercentageTeam1,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Gold gain multiplier from agent deaths.", -100, 100, new string[] {"Siege", "TeamDeathmatch"}, false, null)] GoldGainChangePercentageTeam2,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Min score to win match.", 0, 120000, new string[] {"TeamDeathmatch"}, false, null)] MinScoreToWinMatch,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Integer, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Minimum needed difference in poll results before it is accepted.", 0, 10, null, false, null)] PollAcceptThreshold,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Enum, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Maximum player imbalance between team 1 and team 2.", 0, 5, null, true, typeof (AutoTeamBalanceLimits))] AutoTeamBalanceThreshold,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.Immediately, "Enables anti-cheat.", 0, 0, null, false, null)] EnableAntiCheat,
      [MultiplayerOptionsProperty(MultiplayerOptions.OptionValueType.Bool, MultiplayerOptionsProperty.ReplicationOccurrence.AtMapLoad, "Enables mission recording.", 0, 0, null, false, null)] EnableMissionRecording,
      NumOfSlots,
    }

    public enum OptionsCategory
    {
      Default,
      ClanMatch,
    }

    public class MultiplayerOption
    {
      public readonly MultiplayerOptions.OptionType OptionType;
      private MultiplayerOptions.MultiplayerOption.IntegerValue _intValue;
      private MultiplayerOptions.MultiplayerOption.StringValue _stringValue;

      public static MultiplayerOptions.MultiplayerOption CreateMultiplayerOption(
        MultiplayerOptions.OptionType optionType)
      {
        return new MultiplayerOptions.MultiplayerOption(optionType);
      }

      private MultiplayerOption(MultiplayerOptions.OptionType optionType)
      {
        this.OptionType = optionType;
        if (optionType.GetOptionProperty().OptionValueType == MultiplayerOptions.OptionValueType.String)
        {
          this._intValue = MultiplayerOptions.MultiplayerOption.IntegerValue.Invalid;
          this._stringValue = MultiplayerOptions.MultiplayerOption.StringValue.Create();
        }
        else
        {
          this._intValue = MultiplayerOptions.MultiplayerOption.IntegerValue.Create();
          this._stringValue = MultiplayerOptions.MultiplayerOption.StringValue.Invalid;
        }
      }

      public MultiplayerOptions.MultiplayerOption UpdateValue(bool value)
      {
        this.UpdateValue(value ? 1 : 0);
        return this;
      }

      public MultiplayerOptions.MultiplayerOption UpdateValue(int value)
      {
        this._intValue.UpdateValue(value);
        return this;
      }

      public MultiplayerOptions.MultiplayerOption UpdateValue(string value)
      {
        this._stringValue.UpdateValue(value);
        return this;
      }

      public void GetValue(out bool value) => value = this._intValue.Value == 1;

      public void GetValue(out int value) => value = this._intValue.Value;

      public void GetValue(out string value) => value = this._stringValue.Value;

      private struct IntegerValue
      {
        public static MultiplayerOptions.MultiplayerOption.IntegerValue Invalid => new MultiplayerOptions.MultiplayerOption.IntegerValue();

        public bool IsValid { get; private set; }

        public int Value { get; private set; }

        public static MultiplayerOptions.MultiplayerOption.IntegerValue Create() => new MultiplayerOptions.MultiplayerOption.IntegerValue()
        {
          IsValid = true
        };

        public void UpdateValue(int value) => this.Value = value;
      }

      private struct StringValue
      {
        public static MultiplayerOptions.MultiplayerOption.StringValue Invalid => new MultiplayerOptions.MultiplayerOption.StringValue();

        public bool IsValid { get; private set; }

        public string Value { get; private set; }

        public static MultiplayerOptions.MultiplayerOption.StringValue Create() => new MultiplayerOptions.MultiplayerOption.StringValue()
        {
          IsValid = true
        };

        public void UpdateValue(string value) => this.Value = value;
      }
    }

    private class MultiplayerOptionsContainer
    {
      private readonly MultiplayerOptions.MultiplayerOption[] _multiplayerOptions;

      public MultiplayerOptionsContainer() => this._multiplayerOptions = new MultiplayerOptions.MultiplayerOption[39];

      public MultiplayerOptions.MultiplayerOption GetOptionFromOptionType(
        MultiplayerOptions.OptionType optionType)
      {
        return this._multiplayerOptions[(int) optionType];
      }

      private void CopyOptionFromOther(
        MultiplayerOptions.OptionType optionType,
        MultiplayerOptions.MultiplayerOption option)
      {
        this._multiplayerOptions[(int) optionType] = option;
      }

      public void CreateOption(MultiplayerOptions.OptionType optionType) => this._multiplayerOptions[(int) optionType] = MultiplayerOptions.MultiplayerOption.CreateMultiplayerOption(optionType);

      public void UpdateOptionValue(MultiplayerOptions.OptionType optionType, int value) => this._multiplayerOptions[(int) optionType].UpdateValue(value);

      public void UpdateOptionValue(MultiplayerOptions.OptionType optionType, string value) => this._multiplayerOptions[(int) optionType].UpdateValue(value);

      public void UpdateOptionValue(MultiplayerOptions.OptionType optionType, bool value) => this._multiplayerOptions[(int) optionType].UpdateValue(value ? 1 : 0);

      public void CopyAllValuesTo(
        MultiplayerOptions.MultiplayerOptionsContainer other)
      {
        this.CopyImmediateEffectValuesTo(other);
        this.CopyNewRoundValuesTo(other);
        this.CopyNewMapValuesTo(other);
      }

      public void CopyImmediateEffectValuesTo(
        MultiplayerOptions.MultiplayerOptionsContainer other)
      {
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.AllowPollsToKickPlayers, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AllowPollsToKickPlayers));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.AllowPollsToBanPlayers, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AllowPollsToBanPlayers));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.AllowPollsToChangeMaps, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AllowPollsToChangeMaps));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.AllowIndividualBanners, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AllowIndividualBanners));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.UseRealisticBlocking, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.UseRealisticBlocking));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.MaxNumberOfPlayers, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.MaxNumberOfPlayers));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.MinNumberOfPlayersForMatchStart));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.WarmupTimeLimit, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.WarmupTimeLimit));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.MapTimeLimit, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.MapTimeLimit));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.RoundTotal, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RoundTotal));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.RoundTimeLimit, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RoundTimeLimit));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.RoundPreparationTimeLimit, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RoundPreparationTimeLimit));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.RespawnPeriodTeam1, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RespawnPeriodTeam1));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.RespawnPeriodTeam2, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.RespawnPeriodTeam2));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam1));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.GoldGainChangePercentageTeam2));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.MinScoreToWinMatch, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.MinScoreToWinMatch));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.PollAcceptThreshold, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.PollAcceptThreshold));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.SpectatorCamera, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.SpectatorCamera));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.NumberOfBotsTeam1, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.NumberOfBotsTeam1));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.NumberOfBotsTeam2, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.NumberOfBotsTeam2));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeSelfPercent));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.FriendlyFireDamageMeleeFriendPercent));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.FriendlyFireDamageRangedSelfPercent));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.FriendlyFireDamageRangedFriendPercent));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.AutoTeamBalanceThreshold, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AutoTeamBalanceThreshold));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.WelcomeMessage, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.WelcomeMessage));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.GamePassword, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.GamePassword));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.AdminPassword, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.AdminPassword));
      }

      public void CopyNewRoundValuesTo(
        MultiplayerOptions.MultiplayerOptionsContainer other)
      {
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.NumberOfBotsPerFormation, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.NumberOfBotsPerFormation));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.CultureTeam1, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.CultureTeam2, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam2));
      }

      public void CopyNewMapValuesTo(
        MultiplayerOptions.MultiplayerOptionsContainer other)
      {
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.Map, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.GameType, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType));
        other.CopyOptionFromOther(MultiplayerOptions.OptionType.ClanMatchType, this.GetOptionFromOptionType(MultiplayerOptions.OptionType.ClanMatchType));
      }
    }
  }
}
