// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.DedicatedServerConsoleCommandManager
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Network;

namespace TaleWorlds.MountAndBlade
{
  public static class DedicatedServerConsoleCommandManager
  {
    private static readonly List<System.Type> _commandHandlerTypes = new List<System.Type>();

    static DedicatedServerConsoleCommandManager() => DedicatedServerConsoleCommandManager.AddType(typeof (DedicatedServerConsoleCommandManager));

    public static void AddType(System.Type type) => DedicatedServerConsoleCommandManager._commandHandlerTypes.Add(type);

    internal static void HandleConsoleCommand(string command)
    {
      int length = command.IndexOf(' ');
      string str1 = "";
      string concatName;
      if (length > 0)
      {
        concatName = command.Substring(0, length);
        str1 = command.Substring(length + 1);
      }
      else
        concatName = command;
      bool flag1 = false;
      MultiplayerOptionsProperty multiplayerOptionsProperty = (MultiplayerOptionsProperty) null;
      bool flag2 = false;
      MultiplayerOptions.OptionType optionType;
      for (optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; ++optionType)
      {
        multiplayerOptionsProperty = optionType.GetOptionProperty();
        if (multiplayerOptionsProperty != null && optionType.ToString().Equals(concatName))
        {
          flag2 = true;
          break;
        }
      }
      if (flag2)
      {
        if (str1 == "?")
        {
          Debug.Print("--" + (object) optionType + ": " + multiplayerOptionsProperty.Description, debugFilter: 17179869184UL);
          string str2;
          if (!multiplayerOptionsProperty.HasBounds)
            str2 = "";
          else
            str2 = "Min: " + (object) multiplayerOptionsProperty.BoundsMin + ", Max: " + (object) multiplayerOptionsProperty.BoundsMax + ". ";
          string valueText = optionType.GetValueText();
          Debug.Print("--" + str2 + "Current value: " + valueText, debugFilter: 17179869184UL);
        }
        else if (str1 != "")
        {
          if (multiplayerOptionsProperty.OptionValueType == MultiplayerOptions.OptionValueType.String)
            optionType.SetValue(str1);
          else if (multiplayerOptionsProperty.OptionValueType == MultiplayerOptions.OptionValueType.Integer)
          {
            int result;
            if (int.TryParse(str1, out result))
              optionType.SetValue(result);
          }
          else if (multiplayerOptionsProperty.OptionValueType == MultiplayerOptions.OptionValueType.Enum)
          {
            int result;
            if (int.TryParse(str1, out result))
              optionType.SetValue(result);
          }
          else
          {
            bool result;
            if (multiplayerOptionsProperty.OptionValueType == MultiplayerOptions.OptionValueType.Bool && bool.TryParse(str1, out result))
              optionType.SetValue(result);
          }
          Debug.Print("--Changed: " + (object) optionType + ", to: " + optionType.GetValueText(), debugFilter: 17179869184UL);
        }
        else
          Debug.Print("--Value of: " + (object) optionType + ", is: " + optionType.GetValueText(), debugFilter: 17179869184UL);
        flag1 = true;
      }
      if (!flag1)
      {
        foreach (System.Type commandHandlerType in DedicatedServerConsoleCommandManager._commandHandlerTypes)
        {
          foreach (MethodInfo method in commandHandlerType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
          {
            foreach (object customAttribute in method.GetCustomAttributes(false))
            {
              if (customAttribute is ConsoleCommandMethod consoleCommandMethod4 && consoleCommandMethod4.CommandName.Equals(concatName))
              {
                if (str1 == "?")
                {
                  Debug.Print("--" + consoleCommandMethod4.CommandName + ": " + consoleCommandMethod4.Description, debugFilter: 17179869184UL);
                }
                else
                {
                  List<object> objectList1;
                  if (!str1.IsStringNoneOrEmpty())
                    objectList1 = new List<object>()
                    {
                      (object) str1
                    };
                  else
                    objectList1 = (List<object>) null;
                  List<object> objectList2 = objectList1;
                  method.Invoke((object) null, objectList2?.ToArray());
                }
                flag1 = true;
              }
            }
          }
        }
      }
      if (!flag1)
      {
        bool found;
        string message = CommandLineFunctionality.CallFunction(concatName, str1, out found);
        if (found)
        {
          Debug.Print(message, debugFilter: 17179869184UL);
          flag1 = true;
        }
      }
      if (flag1)
        return;
      Debug.Print("--Invalid command is given.", debugFilter: 17179869184UL);
    }

    [UsedImplicitly]
    [ConsoleCommandMethod("list", "Displays a list of all multiplayer options, their values and other possible commands")]
    private static void ListAllCommands()
    {
      Debug.Print("--List of all multiplayer command and their current values:", debugFilter: 17179869184UL);
      for (MultiplayerOptions.OptionType optionType = MultiplayerOptions.OptionType.ServerName; optionType < MultiplayerOptions.OptionType.NumOfSlots; ++optionType)
        Debug.Print("----" + (object) optionType + ": " + optionType.GetValueText(), debugFilter: 17179869184UL);
      Debug.Print("--List of additional commands:", debugFilter: 17179869184UL);
      foreach (System.Type commandHandlerType in DedicatedServerConsoleCommandManager._commandHandlerTypes)
      {
        foreach (MemberInfo method in commandHandlerType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic))
        {
          foreach (object customAttribute in method.GetCustomAttributes(false))
          {
            if (customAttribute is ConsoleCommandMethod consoleCommandMethod3)
              Debug.Print("----" + consoleCommandMethod3.CommandName, debugFilter: 17179869184UL);
          }
        }
      }
      Debug.Print("--Add '?' after a command to get a more detailed description.", debugFilter: 17179869184UL);
    }

    [UsedImplicitly]
    [ConsoleCommandMethod("set_winner_team", "Sets the winner team of flag domination based multiplayer missions.")]
    private static void SetWinnerTeam(string winnerTeamAsString) => MissionMultiplayerFlagDomination.SetWinnerTeam(int.Parse(winnerTeamAsString));

    [UsedImplicitly]
    [ConsoleCommandMethod("stats", "Displays some game statistics, like FPS and players on the server.")]
    private static void ShowStats()
    {
      Debug.Print("--Current FPS: " + (object) Utilities.GetFps(), debugFilter: 17179869184UL);
      Debug.Print("--Active Players: " + (object) GameNetwork.NetworkPeers.Count<NetworkCommunicator>((Func<NetworkCommunicator, bool>) (x => x.IsSynchronized)), debugFilter: 17179869184UL);
    }

    [UsedImplicitly]
    [ConsoleCommandMethod("open_monitor", "Opens up the monitor window with continuous data-representations on server performance and network usage.")]
    private static void OpenMonitor()
    {
      DebugNetworkEventStatistics.ControlActivate();
      DebugNetworkEventStatistics.OpenExternalMonitor();
    }

    [UsedImplicitly]
    [ConsoleCommandMethod("crash_game", "Crashes the game process.")]
    private static void CrashGame()
    {
      Debug.Print("Crashing the process...", debugFilter: 17179869184UL);
      throw new Exception("Game crashed by user command");
    }
  }
}
