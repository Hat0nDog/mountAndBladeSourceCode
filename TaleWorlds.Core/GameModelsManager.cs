// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.GameModelsManager
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using System.Linq;

namespace TaleWorlds.Core
{
  public abstract class GameModelsManager
  {
    private readonly GameModel[] _gameModels;

    protected GameModelsManager(IEnumerable<GameModel> inputComponents) => this._gameModels = inputComponents.ToArray<GameModel>();

    protected T GetGameModel<T>() where T : GameModel
    {
      for (int index = this._gameModels.Length - 1; index >= 0; --index)
      {
        if (this._gameModels[index] is T gameModel1)
          return gameModel1;
      }
      return default (T);
    }

    public IReadOnlyList<GameModel> GetGameModels() => (IReadOnlyList<GameModel>) this._gameModels;
  }
}
