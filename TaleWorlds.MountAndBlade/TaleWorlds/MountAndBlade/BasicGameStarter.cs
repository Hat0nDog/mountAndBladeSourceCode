// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BasicGameStarter
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;

namespace TaleWorlds.MountAndBlade
{
  public class BasicGameStarter : IGameStarter
  {
    private List<GameModel> _models;

    IEnumerable<GameModel> IGameStarter.Models => (IEnumerable<GameModel>) this._models;

    public BasicGameStarter() => this._models = new List<GameModel>();

    void IGameStarter.AddModel(GameModel gameModel) => this._models.Add(gameModel);
  }
}
