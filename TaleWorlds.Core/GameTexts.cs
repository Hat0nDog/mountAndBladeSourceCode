// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.GameTexts
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.Localization;

namespace TaleWorlds.Core
{
  public static class GameTexts
  {
    private static GameTextManager _gameTextManager;

    public static void Initialize(GameTextManager gameTextManager) => GameTexts._gameTextManager = gameTextManager;

    public static TextObject FindText(string id, string variation = null) => GameTexts._gameTextManager.FindText(id, variation);

    public static bool TryGetText(string id, out TextObject textObject, string variation = null) => GameTexts._gameTextManager.TryGetText(id, variation, out textObject);

    public static IEnumerable<TextObject> FindAllTextVariations(string id) => GameTexts._gameTextManager.FindAllTextVariations(id);

    public static void SetVariable(string variableName, string content) => MBTextManager.SetTextVariable(variableName, content, false);

    public static void SetVariable(string variableName, float content) => MBTextManager.SetTextVariable(variableName, content);

    public static void SetVariable(string variableName, int content) => MBTextManager.SetTextVariable(variableName, content);

    public static void SetVariable(string variableName, TextObject content) => MBTextManager.SetTextVariable(variableName, content, false);

    public static void ClearInstance() => GameTexts._gameTextManager = (GameTextManager) null;

    public static GameTexts.GameTextHelper AddGameTextWithVariation(string id) => new GameTexts.GameTextHelper(id);

    public static TextObject MergeTextObjectsWithComma(
      List<TextObject> textObjects,
      bool includeAnd)
    {
      TextObject textObject;
      switch (textObjects.Count)
      {
        case 0:
          textObject = TextObject.Empty;
          break;
        case 1:
          textObject = textObjects[0];
          break;
        default:
          string str1 = "{=!}";
          for (int index = 0; index < textObjects.Count - 2; ++index)
            str1 = str1 + "{VAR_" + (object) index + "}, ";
          string str2;
          if (includeAnd)
            str2 = str1 + "{VAR_" + (object) (textObjects.Count - 2) + "} " + (object) new TextObject("{=ZBmz4niK}and") + " {VAR_" + (object) (textObjects.Count - 1) + "}";
          else
            str2 = str1 + "{VAR_" + (object) (textObjects.Count - 2) + "},  {VAR_" + (object) (textObjects.Count - 1) + "}";
          textObject = new TextObject(str2);
          for (int index = 0; index < textObjects.Count; ++index)
            textObject.SetTextVariable("VAR_" + (object) index, textObjects[index]);
          break;
      }
      return textObject;
    }

    public class GameTextHelper
    {
      private string _id;

      public GameTextHelper(string id) => this._id = id;

      public GameTexts.GameTextHelper Variation(
        string text,
        params object[] propertiesAndWeights)
      {
        GameTexts._gameTextManager.AddGameText(this._id).AddVariation(text, propertiesAndWeights);
        return this;
      }
    }
  }
}
