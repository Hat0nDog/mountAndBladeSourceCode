// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.IHttpDriver
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System.Threading.Tasks;

namespace TaleWorlds.Network
{
  public interface IHttpDriver
  {
    Task<string> HttpGetString(string url, bool withUserToken);

    Task<byte[]> HttpDownloadData(string url);

    IHttpRequestTask CreateHttpPostRequestTask(
      string address,
      string postData,
      bool withUserToken);

    IHttpRequestTask CreateHttpGetRequestTask(string address, bool withUserToken);
  }
}
