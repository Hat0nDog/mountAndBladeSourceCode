﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.NewsManager.NewsType
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TaleWorlds.Library.NewsManager
{
  public struct NewsType
  {
    [JsonConverter(typeof (StringEnumConverter))]
    public NewsItem.NewsTypes Type { get; set; }

    public int Index { get; set; }
  }
}