// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerGameTypes
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade
{
  public static class MultiplayerGameTypes
  {
    private static Dictionary<string, MultiplayerGameTypeInfo> _multiplayerGameTypeInfos;

    public static void Initialize()
    {
      MultiplayerGameTypes.CreateGameTypeInformations();
      MultiplayerGameTypes.LoadMultiplayerSceneInformations();
    }

    public static bool CheckGameTypeInfoExists(string gameType) => MultiplayerGameTypes._multiplayerGameTypeInfos.ContainsKey(gameType);

    public static MultiplayerGameTypeInfo GetGameTypeInfo(string gameType)
    {
      if (MultiplayerGameTypes._multiplayerGameTypeInfos.ContainsKey(gameType))
        return MultiplayerGameTypes._multiplayerGameTypeInfos[gameType];
      Debug.Print("Cannot find game type:" + gameType);
      return (MultiplayerGameTypeInfo) null;
    }

    private static void LoadMultiplayerSceneInformations()
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(ModuleHelper.GetModuleFullPath("Native") + "ModuleData/Multiplayer/MultiplayerScenes.xml");
      foreach (XmlNode xmlNode in xmlDocument.FirstChild)
      {
        if (xmlNode.NodeType != XmlNodeType.Comment)
        {
          string innerText1 = xmlNode.Attributes["name"].InnerText;
          foreach (XmlNode childNode in xmlNode.ChildNodes)
          {
            if (childNode.NodeType != XmlNodeType.Comment)
            {
              string innerText2 = childNode.Attributes["name"].InnerText;
              if (MultiplayerGameTypes._multiplayerGameTypeInfos.ContainsKey(innerText2))
                MultiplayerGameTypes._multiplayerGameTypeInfos[innerText2].Scenes.Add(innerText1);
            }
          }
        }
      }
    }

    private static void CreateGameTypeInformations()
    {
      MultiplayerGameTypes._multiplayerGameTypeInfos = new Dictionary<string, MultiplayerGameTypeInfo>();
      foreach (MultiplayerGameTypeInfo multiplayerGameType in Module.CurrentModule.GetMultiplayerGameTypes())
        MultiplayerGameTypes._multiplayerGameTypeInfos.Add(multiplayerGameType.GameType, multiplayerGameType);
    }
  }
}
