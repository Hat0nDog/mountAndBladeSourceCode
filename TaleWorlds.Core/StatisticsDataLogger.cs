// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.StatisticsDataLogger
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using TaleWorlds.Library;

namespace TaleWorlds.Core
{
  public static class StatisticsDataLogger
  {
    private static readonly Dictionary<uint, StatisticsDataLogger.StatData> _logTypes;
    private static readonly StatisticsDataLogger.StatData _rootData;
    private static readonly DateTime _applicationStartTime = DateTime.Now;
    private static string _filePath = (string) null;
    private static int _versionNo;

    static StatisticsDataLogger()
    {
      StatisticsDataLogger._logTypes = new Dictionary<uint, StatisticsDataLogger.StatData>();
      StatisticsDataLogger._rootData = new StatisticsDataLogger.StatData(new StatisticsDataIdentifier("LOG"), (StatisticsDataLogger.StatData) null);
      StatisticsDataLogger._logTypes.Add(StatisticsDataLogger._rootData.Identifier.UniqueId, StatisticsDataLogger._rootData);
      StatisticsDataLogger._versionNo = 0;
      StatisticsDataLogger.ClearLogs();
    }

    public static void RegisterDataIdentifier(
      StatisticsDataIdentifier value,
      StatisticsDataIdentifier parent)
    {
      if (parent == null)
        parent = StatisticsDataLogger._rootData.Identifier;
      StatisticsDataLogger.StatData statData = new StatisticsDataLogger.StatData(value, StatisticsDataLogger._logTypes[parent.UniqueId]);
      StatisticsDataLogger._logTypes.Add(value.UniqueId, statData);
    }

    public static void AddStat(StatisticsDataIdentifier id, int amount, string detail)
    {
      if (!StatisticsDataLogger._logTypes.ContainsKey(id.UniqueId))
        return;
      StatisticsDataLogger.StatData statData = StatisticsDataLogger._logTypes[id.UniqueId];
      if (detail != null)
        statData.Details.Add(detail);
      for (; statData != null; statData = statData.Parent)
        statData.Number += amount;
    }

    public static void Save(StatisticsDataIdentifier mask = null, string header = "")
    {
      using (StreamWriter file = new StreamWriter(StatisticsDataLogger.GetFileName()))
      {
        file.WriteLine("Application Time: " + (DateTime.Now - StatisticsDataLogger._applicationStartTime).ToString("G"));
        file.WriteLine("------------------------------------------");
        file.WriteLine(header);
        file.WriteLine("------------------------------------------");
        file.WriteLine("------------------------------------------");
        if (mask != null)
          StatisticsDataLogger.WriteToFile(file, StatisticsDataLogger._logTypes[mask.UniqueId], 0);
        else
          StatisticsDataLogger.WriteToFile(file, StatisticsDataLogger._rootData, 0);
      }
    }

    public static void ClearLogs(StatisticsDataIdentifier clearCategory = null)
    {
      StatisticsDataLogger.StatData statData = StatisticsDataLogger._rootData;
      if (clearCategory != null)
        statData = StatisticsDataLogger._logTypes[clearCategory.UniqueId];
      statData.Clear();
      while (File.Exists(StatisticsDataLogger.GetFileName()))
        ++StatisticsDataLogger._versionNo;
    }

    public static string GetFileName() => StatisticsDataLogger.GetPath() + "Stats_" + StatisticsDataLogger._applicationStartTime.ToString("yyyy_MM_dd_HH_mm_ss") + "_v" + (object) StatisticsDataLogger._versionNo + ".txt";

    private static string GetPath()
    {
      if (StatisticsDataLogger._filePath == null)
      {
        try
        {
          StatisticsDataLogger._filePath = BasePath.Name + "\\Stats\\";
          Directory.CreateDirectory(StatisticsDataLogger._filePath);
        }
        catch (Exception ex)
        {
          Debug.Print("Exception on StatisticsDataLogger.GetPath() " + ex.Message);
        }
      }
      return StatisticsDataLogger._filePath;
    }

    private static void WriteToFile(
      StreamWriter file,
      StatisticsDataLogger.StatData data,
      int height)
    {
      string str1 = "";
      for (int index = 0; index < height; ++index)
        str1 += "|  ";
      if (!data.Identifier.IsFiltered && data.Details.Count > 0)
      {
        string str2 = str1 + " -";
        foreach (string detail in data.Details)
          file.WriteLine(str2 + detail);
      }
      foreach (StatisticsDataLogger.StatData child in data.Children)
      {
        file.WriteLine(str1 + child.Identifier.Description + "  : " + (object) child.Number);
        StatisticsDataLogger.WriteToFile(file, child, height + 1);
      }
    }

    private class StatData
    {
      public readonly StatisticsDataIdentifier Identifier;
      public readonly StatisticsDataLogger.StatData Parent;
      public readonly List<StatisticsDataLogger.StatData> Children;
      public readonly List<string> Details;
      public int Number;

      public StatData(StatisticsDataIdentifier id, StatisticsDataLogger.StatData parent)
      {
        this.Identifier = id;
        this.Parent = parent;
        this.Children = new List<StatisticsDataLogger.StatData>();
        this.Details = new List<string>();
        if (this.Parent != null)
          this.Parent.Children.Add(this);
        this.Number = 0;
      }

      public void Clear()
      {
        foreach (StatisticsDataLogger.StatData child in this.Children)
          child.Clear();
        this.Number = 0;
      }
    }
  }
}
