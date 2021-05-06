// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IHighlights
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IHighlights
  {
    [EngineMethod("initialize", false)]
    void Initialize();

    [EngineMethod("open_group", false)]
    void OpenGroup(string id);

    [EngineMethod("close_group", false)]
    void CloseGroup(string id, bool destroy = false);

    [EngineMethod("save_screenshot", false)]
    void SaveScreenshot(string highlightId, string groupId);

    [EngineMethod("save_video", false)]
    void SaveVideo(string highlightId, string groupId, int startDelta, int endDelta);

    [EngineMethod("open_summary", false)]
    void OpenSummary(string groups);

    [EngineMethod("add_highlight", false)]
    void AddHighlight(string id, string name);

    [EngineMethod("remove_highlight", false)]
    void RemoveHighlight(string id);
  }
}
