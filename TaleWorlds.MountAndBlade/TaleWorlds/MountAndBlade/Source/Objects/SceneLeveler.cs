// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Source.Objects.SceneLeveler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TaleWorlds.MountAndBlade.Source.Objects
{
  public class SceneLeveler : ScriptComponentBehaviour
  {
    public string SourceSelectionSetName = "";
    public string TargetSelectionSetName = "";
    public SimpleButton CreateLevel1;
    public SimpleButton CreateLevel2;
    public SimpleButton CreateLevel3;
    public SimpleButton DeleteLevel1;
    public SimpleButton DeleteLevel2;
    public SimpleButton DeleteLevel3;
    public SimpleButton SelectEntitiesWithoutLevel;

    protected internal override void OnEditorVariableChanged(string variableName)
    {
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(variableName))
      {
        case 40127036:
          if (!(variableName == "CreateLevel1"))
            break;
          this.OnLevelizeButtonPressed(1);
          break;
        case 73682274:
          if (!(variableName == "CreateLevel3"))
            break;
          this.OnLevelizeButtonPressed(3);
          break;
        case 90459893:
          if (!(variableName == "CreateLevel2"))
            break;
          this.OnLevelizeButtonPressed(2);
          break;
        case 804927328:
          if (!(variableName == "SelectEntitiesWithoutLevel"))
            break;
          this.OnSelectEntitiesWithoutLevelButtonPressed();
          break;
        case 1310461563:
          if (!(variableName == "DeleteLevel1"))
            break;
          this.OnDeleteButtonPressed(1);
          break;
        case 1327239182:
          if (!(variableName == "DeleteLevel2"))
            break;
          this.OnDeleteButtonPressed(2);
          break;
        case 1344016801:
          if (!(variableName == "DeleteLevel3"))
            break;
          this.OnDeleteButtonPressed(3);
          break;
      }
    }

    private void OnLevelizeButtonPressed(int level)
    {
      if (this.SourceSelectionSetName.IsEmpty<char>())
        MessageManager.DisplayMessage("ApplyToSelectionSet is empty!");
      else if (this.TargetSelectionSetName.IsEmpty<char>())
      {
        MessageManager.DisplayMessage("NewSelectionSetName is empty!");
      }
      else
      {
        int num1 = 0;
        int num2 = 0;
        int num3 = 0;
        int num4 = 0;
        GameEntity.UpgradeLevelMask levelMask = this.GetLevelMask(level);
        List<GameEntity> gameEntityList = this.CollectEntitiesWithLevel();
        List<GameEntity> gameEntities = new List<GameEntity>();
        foreach (GameEntity gameEntity in gameEntityList)
        {
          string possiblePrefabName = this.FindPossiblePrefabName(gameEntity);
          if (possiblePrefabName.IsEmpty<char>())
          {
            ++num1;
          }
          else
          {
            GameEntity.UpgradeLevelMask upgradeLevelMask = gameEntity.GetUpgradeLevelMask();
            if ((upgradeLevelMask & levelMask) != GameEntity.UpgradeLevelMask.None)
            {
              ++num2;
              gameEntities.Add(gameEntity);
            }
            else
            {
              GameEntity entity = GameEntity.Instantiate(this.Scene, this.ConvertPrefabName(possiblePrefabName, levelMask), gameEntity.GetGlobalFrame());
              if ((NativeObject) entity == (NativeObject) null)
              {
                ++num3;
              }
              else
              {
                ++num4;
                GameEntity.UpgradeLevelMask mask = upgradeLevelMask & ~GameEntity.UpgradeLevelMask.Level1 & ~GameEntity.UpgradeLevelMask.Level2 & ~GameEntity.UpgradeLevelMask.Level3 | levelMask;
                entity.SetUpgradeLevelMask(mask);
                this.CopyScriptParameters(entity, gameEntity);
                gameEntities.Add(entity);
              }
            }
          }
        }
        Debug.Print("Created Entities : " + (object) num4 + "\nAlready Visible In Desired Level : " + (object) num2 + "\nWithout Prefab For Level : " + (object) num3 + "\nWithout Prefab Info : " + (object) num1, color: Debug.DebugColor.Magenta);
        Utilities.CreateSelectionInEditor(gameEntities, this.TargetSelectionSetName);
      }
    }

    private void CopyScriptParameters(GameEntity entity, GameEntity copyFromEntity)
    {
      if (copyFromEntity.HasScriptComponent("WallSegment") && !entity.HasScriptComponent("WallSegment"))
        entity.CopyScriptComponentFromAnotherEntity(copyFromEntity, "WallSegment");
      if (copyFromEntity.HasScriptComponent("mesh_bender") && !entity.HasScriptComponent("mesh_bender"))
        entity.CopyScriptComponentFromAnotherEntity(copyFromEntity, "mesh_bender");
      for (int index = 0; index < entity.ChildCount && index < copyFromEntity.ChildCount; ++index)
        this.CopyScriptParameters(entity.GetChild(index), copyFromEntity.GetChild(index));
    }

    private GameEntity.UpgradeLevelMask GetLevelMask(int level)
    {
      if (level == 1)
        return GameEntity.UpgradeLevelMask.Level1;
      return level != 2 ? GameEntity.UpgradeLevelMask.Level3 : GameEntity.UpgradeLevelMask.Level2;
    }

    private string GetLevelSubString(GameEntity.UpgradeLevelMask levelMask)
    {
      switch (levelMask)
      {
        case GameEntity.UpgradeLevelMask.Level1:
          return "_l1";
        case GameEntity.UpgradeLevelMask.Level2:
          return "_l2";
        case GameEntity.UpgradeLevelMask.Level3:
          return "_l3";
        default:
          return "";
      }
    }

    private string ConvertPrefabName(string prefabName, GameEntity.UpgradeLevelMask newLevelMask)
    {
      string str = prefabName;
      string levelSubString = this.GetLevelSubString(newLevelMask);
      if (newLevelMask != GameEntity.UpgradeLevelMask.Level1)
        str = str.Replace(this.GetLevelSubString(GameEntity.UpgradeLevelMask.Level1), levelSubString);
      if (newLevelMask != GameEntity.UpgradeLevelMask.Level2)
        str = str.Replace(this.GetLevelSubString(GameEntity.UpgradeLevelMask.Level2), levelSubString);
      if (newLevelMask != GameEntity.UpgradeLevelMask.Level3)
        str = str.Replace(this.GetLevelSubString(GameEntity.UpgradeLevelMask.Level3), levelSubString);
      return str.Equals(prefabName) ? "" : str;
    }

    private string FindPossiblePrefabName(GameEntity gameEntity)
    {
      string prefabName = gameEntity.GetPrefabName();
      return prefabName.IsEmpty<char>() ? gameEntity.GetOldPrefabName() : prefabName;
    }

    private void OnDeleteButtonPressed(int level)
    {
      if (this.SourceSelectionSetName.IsEmpty<char>())
      {
        MessageManager.DisplayMessage("ApplyToSelectionSet is empty!");
      }
      else
      {
        List<GameEntity> gameEntityList = this.CollectEntitiesWithLevel();
        GameEntity.UpgradeLevelMask levelMask = this.GetLevelMask(level);
        List<GameEntity> gameEntities = new List<GameEntity>();
        int variable1 = 0;
        int variable2 = 0;
        foreach (GameEntity gameEntity in gameEntityList)
        {
          GameEntity.UpgradeLevelMask upgradeLevelMask = gameEntity.GetUpgradeLevelMask();
          if (upgradeLevelMask == levelMask)
          {
            gameEntities.Add(gameEntity);
            ++variable1;
          }
          else if ((upgradeLevelMask & levelMask) != GameEntity.UpgradeLevelMask.None)
          {
            gameEntity.SetUpgradeLevelMask(upgradeLevelMask & ~levelMask);
            ++variable2;
          }
        }
        Utilities.DeleteEntitiesInEditorScene(gameEntities);
        TextObject textObject1 = new TextObject("{=!}Deleted entity count : {DELETED_ENTRY_COUNT}");
        TextObject textObject2 = new TextObject("{=!}Removed level mask count : {REMOVED_LEVEL_MASK}");
        textObject1.SetTextVariable("DELETED_ENTRY_COUNT", variable1);
        textObject2.SetTextVariable("REMOVED_LEVEL_MASK", variable2);
        MessageManager.DisplayMessage(textObject1.ToString());
        MessageManager.DisplayMessage(textObject2.ToString());
      }
    }

    private void OnSelectEntitiesWithoutLevelButtonPressed()
    {
      List<GameEntity> entities = new List<GameEntity>();
      this.Scene.GetEntities(ref entities);
      List<GameEntity> all = entities.FindAll((Predicate<GameEntity>) (x => x.GetUpgradeLevelMask() == GameEntity.UpgradeLevelMask.None));
      TextObject textObject = new TextObject("{=!}Selected entity count : {SELECTED_ENTITIES}");
      textObject.SetTextVariable("SELECTED_ENTITIES", all.Count);
      MessageManager.DisplayMessage(textObject.ToString());
      if (!all.Any<GameEntity>())
        return;
      Utilities.SelectEntities(all);
    }

    private List<GameEntity> CollectEntitiesWithLevel()
    {
      List<GameEntity> gameEntities = new List<GameEntity>();
      Utilities.GetEntitiesOfSelectionSet(this.SourceSelectionSetName, ref gameEntities);
      for (int index = gameEntities.Count - 1; index >= 0; --index)
      {
        if ((gameEntities[index].GetUpgradeLevelMask() & (GameEntity.UpgradeLevelMask.Level1 | GameEntity.UpgradeLevelMask.Level2 | GameEntity.UpgradeLevelMask.Level3)) == GameEntity.UpgradeLevelMask.None)
          gameEntities.RemoveAt(index);
      }
      return gameEntities;
    }
  }
}
