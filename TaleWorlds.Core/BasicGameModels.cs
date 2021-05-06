// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.BasicGameModels
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;

namespace TaleWorlds.Core
{
  public class BasicGameModels : GameModelsManager
  {
    public RidingModel RidingModel { get; private set; }

    public StrikeMagnitudeCalculationModel StrikeMagnitudeModel { get; private set; }

    public BattleSurvivalModel BattleSurvivalModel { get; private set; }

    public ItemCategorySelector ItemCategorySelector { get; private set; }

    public ItemValueModel ItemValueModel { get; private set; }

    public SkillList SkillList { get; private set; }

    public BasicGameModels(IEnumerable<GameModel> inputComponents)
      : base(inputComponents)
    {
      this.StrikeMagnitudeModel = this.GetGameModel<StrikeMagnitudeCalculationModel>();
      this.RidingModel = this.GetGameModel<RidingModel>();
      this.SkillList = this.GetGameModel<SkillList>();
      this.ItemCategorySelector = this.GetGameModel<ItemCategorySelector>();
      this.ItemValueModel = this.GetGameModel<ItemValueModel>();
    }
  }
}
