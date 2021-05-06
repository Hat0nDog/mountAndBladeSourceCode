// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.PostBoxId
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;

namespace TaleWorlds.Network
{
  public class PostBoxId : Attribute
  {
    public string Id { get; private set; }

    public PostBoxId(string id) => this.Id = id;
  }
}
