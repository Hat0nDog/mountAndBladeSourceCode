// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.NewsManager.NewsItem
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace TaleWorlds.Library.NewsManager
{
  public struct NewsItem
  {
    public string Title { get; set; }

    public string Description { get; set; }

    public string ImageSourcePath { get; set; }

    public List<NewsType> Feeds { get; set; }

    public string NewsLink { get; set; }

    [JsonConverter(typeof (StringEnumConverter))]
    public enum NewsTypes
    {
      LauncherSingleplayer,
      LauncherMultiplayer,
      MultiplayerLobby,
    }
  }
}
