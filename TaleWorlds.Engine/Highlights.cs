// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Highlights
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System.Collections.Generic;

namespace TaleWorlds.Engine
{
  public class Highlights
  {
    public static void Initialize() => EngineApplicationInterface.IHighlights.Initialize();

    public static void OpenGroup(string id) => EngineApplicationInterface.IHighlights.OpenGroup(id);

    public static void CloseGroup(string id, bool destroy = false) => EngineApplicationInterface.IHighlights.CloseGroup(id, destroy);

    public static void SaveScreenshot(string highlightId, string groupId) => EngineApplicationInterface.IHighlights.SaveScreenshot(highlightId, groupId);

    public static void SaveVideo(string highlightId, string groupId, int startDelta, int endDelta) => EngineApplicationInterface.IHighlights.SaveVideo(highlightId, groupId, startDelta, endDelta);

    public static void OpenSummary(List<string> groups)
    {
      string groups1 = string.Join("::", (IEnumerable<string>) groups);
      EngineApplicationInterface.IHighlights.OpenSummary(groups1);
    }

    public static void AddHighlight(string id, string name) => EngineApplicationInterface.IHighlights.AddHighlight(id, name);

    public static void RemoveHighlight(string id) => EngineApplicationInterface.IHighlights.RemoveHighlight(id);

    public enum Significance
    {
      None = 0,
      ExtremelyBad = 1,
      VeryBad = 2,
      Bad = 4,
      Neutral = 16, // 0x00000010
      Good = 256, // 0x00000100
      VeryGood = 512, // 0x00000200
      ExtremelyGoods = 1024, // 0x00000400
      Max = 2048, // 0x00000800
    }

    public enum Type
    {
      None = 0,
      Milestone = 1,
      Achievement = 2,
      Incident = 4,
      StateChange = 8,
      Unannounced = 16, // 0x00000010
      Max = 32, // 0x00000020
    }
  }
}
