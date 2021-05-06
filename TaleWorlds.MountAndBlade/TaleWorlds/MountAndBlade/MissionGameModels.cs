// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionGameModels
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TaleWorlds.MountAndBlade
{
  public sealed class MissionGameModels : GameModelsManager
  {
    public static MissionGameModels Current { get; private set; }

    public AgentStatCalculateModel AgentStatCalculateModel { get; private set; }

    public ApplyWeatherEffectsModel ApplyWeatherEffectsModel { get; private set; }

    public AgentApplyDamageModel AgentApplyDamageModel { get; private set; }

    public AgentDecideKilledOrUnconsciousModel AgentDecideKilledOrUnconsciousModel { get; private set; }

    public BattleMoraleModel BattleMoraleModel { get; private set; }

    public BattleInitializationModel BattleInitializationModel { get; private set; }

    public AutoBlockModel AutoBlockModel { get; private set; }

    private void GetSpecificGameBehaviors()
    {
      this.AgentStatCalculateModel = this.GetGameModel<AgentStatCalculateModel>();
      this.ApplyWeatherEffectsModel = this.GetGameModel<ApplyWeatherEffectsModel>();
      this.AgentApplyDamageModel = this.GetGameModel<AgentApplyDamageModel>();
      this.AgentDecideKilledOrUnconsciousModel = this.GetGameModel<AgentDecideKilledOrUnconsciousModel>();
      this.BattleMoraleModel = this.GetGameModel<BattleMoraleModel>();
      this.BattleInitializationModel = this.GetGameModel<BattleInitializationModel>();
      this.AutoBlockModel = this.GetGameModel<AutoBlockModel>();
    }

    private void MakeGameComponentBindings()
    {
    }

    public MissionGameModels(IEnumerable<GameModel> inputComponents)
      : base(inputComponents)
    {
      MissionGameModels.Current = this;
      this.GetSpecificGameBehaviors();
      this.MakeGameComponentBindings();
    }

    public static void Clear() => MissionGameModels.Current = (MissionGameModels) null;
  }
}
