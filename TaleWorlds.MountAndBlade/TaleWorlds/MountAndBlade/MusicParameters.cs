// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MusicParameters
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections;
using System.IO;
using System.Xml;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace TaleWorlds.MountAndBlade
{
  public static class MusicParameters
  {
    private static float[] _parameters;
    public const float ZeroIntensity = 0.0f;

    public static int SmallBattleTreshold => (int) MusicParameters._parameters[0];

    public static int MediumBattleTreshold => (int) MusicParameters._parameters[1];

    public static int LargeBattleTreshold => (int) MusicParameters._parameters[2];

    public static float SmallBattleDistanceTreshold => MusicParameters._parameters[3];

    public static float MediumBattleDistanceTreshold => MusicParameters._parameters[4];

    public static float LargeBattleDistanceTreshold => MusicParameters._parameters[5];

    public static float MaxBattleDistanceTreshold => MusicParameters._parameters[6];

    public static float MinIntensity => MusicParameters._parameters[7];

    public static float DefaultStartIntensity => MusicParameters._parameters[8];

    public static float PlayerChargeEffectOnStartIntensity => MusicParameters._parameters[9];

    public static float BattleSizeEffectOnStartIntensity => MusicParameters._parameters[10];

    public static float RandomEffectMultiplierOnStartIntensity => MusicParameters._parameters[11];

    public static float FriendlyTroopDeadEffectOnIntensity => MusicParameters._parameters[12];

    public static float EnemyTroopDeadEffectOnIntensity => MusicParameters._parameters[13];

    public static float PlayerTroopDeadEffectMultiplierOnIntensity => MusicParameters._parameters[14];

    public static float BattleRatioTresholdOnIntensity => MusicParameters._parameters[15];

    public static float BattleTurnsOneSideCooldown => MusicParameters._parameters[16];

    public static float CampaignDarkModeThreshold => MusicParameters._parameters[17];

    public static void LoadFromXml()
    {
      MusicParameters._parameters = new float[18];
      string path = ModuleHelper.GetModuleFullPath("Native") + "ModuleData/music_parameters.xml";
      XmlDocument xmlDocument = new XmlDocument();
      StreamReader streamReader = new StreamReader(path);
      string end = streamReader.ReadToEnd();
      xmlDocument.LoadXml(end);
      streamReader.Close();
      foreach (XmlNode childNode in xmlDocument.ChildNodes)
      {
        if (childNode.NodeType == XmlNodeType.Element && childNode.Name == "music_parameters")
        {
          IEnumerator enumerator = childNode.ChildNodes.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              XmlNode current = (XmlNode) enumerator.Current;
              if (current.NodeType == XmlNodeType.Element)
              {
                MusicParameters.MusicParametersEnum musicParametersEnum = (MusicParameters.MusicParametersEnum) Enum.Parse(typeof (MusicParameters.MusicParametersEnum), current.Attributes["id"].Value);
                float num = float.Parse(current.Attributes["value"].Value);
                MusicParameters._parameters[(int) musicParametersEnum] = num;
              }
            }
            break;
          }
          finally
          {
            if (enumerator is IDisposable disposable5)
              disposable5.Dispose();
          }
        }
      }
      Debug.Print("MusicParameters have been resetted.", color: Debug.DebugColor.Green, debugFilter: 281474976710656UL);
      Debug.Print("MusicParameters have been resetted.", color: Debug.DebugColor.Green, debugFilter: 64UL);
    }

    private enum MusicParametersEnum
    {
      SmallBattleTreshold,
      MediumBattleTreshold,
      LargeBattleTreshold,
      SmallBattleDistanceTreshold,
      MediumBattleDistanceTreshold,
      LargeBattleDistanceTreshold,
      MaxBattleDistanceTreshold,
      MinIntensity,
      DefaultStartIntensity,
      PlayerChargeEffectOnStartIntensity,
      BattleSizeEffectOnStartIntensity,
      RandomEffectMultiplierOnStartIntensity,
      FriendlyTroopDeadEffectOnIntensity,
      EnemyTroopDeadEffectOnIntensity,
      PlayerTroopDeadEffectMultiplierOnIntensity,
      BattleRatioTresholdOnIntensity,
      BattleTurnsOneSideCooldown,
      CampaignDarkModeThreshold,
      Count,
    }
  }
}
