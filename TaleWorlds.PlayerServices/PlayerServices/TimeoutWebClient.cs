// Decompiled with JetBrains decompiler
// Type: TaleWorlds.PlayerServices.TimeoutWebClient
// Assembly: TaleWorlds.PlayerServices, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FC0579B2-CBA1-4D57-8C1B-BA25E3FC058C
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.PlayerServices.dll

using System;
using System.Net;

namespace TaleWorlds.PlayerServices
{
  public class TimeoutWebClient : WebClient
  {
    public TimeoutWebClient() => this.Timeout = 15000;

    public TimeoutWebClient(int timeout) => this.Timeout = timeout;

    public int Timeout { get; set; }

    protected override WebRequest GetWebRequest(Uri address)
    {
      WebRequest webRequest = base.GetWebRequest(address);
      webRequest.Timeout = this.Timeout;
      return webRequest;
    }
  }
}
