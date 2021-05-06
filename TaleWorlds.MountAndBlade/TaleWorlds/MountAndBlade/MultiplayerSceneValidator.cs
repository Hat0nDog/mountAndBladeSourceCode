// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerSceneValidator
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class MultiplayerSceneValidator : ScriptComponentBehaviour
  {
    public SimpleButton SelectFaultyEntities;

    protected internal override void OnEditorVariableChanged(string variableName)
    {
      base.OnEditorVariableChanged(variableName);
      if (!(variableName == "SelectFaultyEntities"))
        return;
      this.SelectInvalidEntities();
    }

    protected internal override void OnSceneSave(string saveFolder)
    {
      base.OnSceneSave(saveFolder);
      foreach (GameEntity invalidEntity in this.GetInvalidEntities())
        ;
    }

    private List<GameEntity> GetInvalidEntities()
    {
      List<GameEntity> gameEntityList = new List<GameEntity>();
      List<GameEntity> entities = new List<GameEntity>();
      this.Scene.GetEntities(ref entities);
      foreach (GameEntity gameEntity in entities)
      {
        foreach (ScriptComponentBehaviour scriptComponent in gameEntity.GetScriptComponents())
        {
          if (scriptComponent != null && (scriptComponent.GetType().IsSubclassOf(typeof (MissionObject)) || scriptComponent.GetType() == typeof (MissionObject) && scriptComponent.IsOnlyVisual()))
          {
            gameEntityList.Add(gameEntity);
            break;
          }
        }
      }
      return gameEntityList;
    }

    private void SelectInvalidEntities()
    {
      this.GameEntity.DeselectEntityOnEditor();
      foreach (GameEntity invalidEntity in this.GetInvalidEntities())
        invalidEntity.SelectEntityOnEditor();
    }
  }
}
