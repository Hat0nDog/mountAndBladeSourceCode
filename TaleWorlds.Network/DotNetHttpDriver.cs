// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.DotNetHttpDriver
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System.Net;
using System.Threading.Tasks;

namespace TaleWorlds.Network
{
  public class DotNetHttpDriver : IHttpDriver
  {
    IHttpRequestTask IHttpDriver.CreateHttpPostRequestTask(
      string address,
      string postData,
      bool withUserToken)
    {
      return (IHttpRequestTask) new HttpPostRequest(address, postData);
    }

    IHttpRequestTask IHttpDriver.CreateHttpGetRequestTask(
      string address,
      bool withUserToken)
    {
      return (IHttpRequestTask) new HttpGetRequest(address);
    }

    async Task<string> IHttpDriver.HttpGetString(string url, bool withUserToken) => await new WebClient().DownloadStringTaskAsync(url);

    async Task<byte[]> IHttpDriver.HttpDownloadData(string url) => await new WebClient().DownloadDataTaskAsync(url);
  }
}
