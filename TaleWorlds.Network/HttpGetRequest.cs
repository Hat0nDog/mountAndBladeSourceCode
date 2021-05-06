// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.HttpGetRequest
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Network
{
  public class HttpGetRequest : IHttpRequestTask
  {
    private const int BufferSize = 1024;
    private HttpWebResponse _httpWebResponse;
    private readonly string _address;

    public HttpRequestTaskState State { get; private set; }

    public bool Successful { get; private set; }

    public string ResponseData { get; private set; }

    public HttpStatusCode ResponseStatusCode { get; private set; }

    public Exception Exception { get; private set; }

    static HttpGetRequest() => ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

    public HttpGetRequest(string address)
    {
      this._address = address;
      this.State = HttpRequestTaskState.NotStarted;
      this.ResponseData = "";
      this.ResponseStatusCode = HttpStatusCode.OK;
    }

    private void SetFinishedAsSuccessful(string responseData, HttpStatusCode statusCode)
    {
      this.Successful = true;
      this.ResponseData = responseData;
      this.ResponseStatusCode = statusCode;
      if (this._httpWebResponse != null)
        this._httpWebResponse.Close();
      this.State = HttpRequestTaskState.Finished;
    }

    private void SetFinishedAsUnsuccessful(Exception e)
    {
      this.Successful = false;
      this.Exception = e;
      if (this._httpWebResponse != null)
        this._httpWebResponse.Close();
      this.State = HttpRequestTaskState.Finished;
    }

    public void Start() => this.DoTask();

    public async Task DoTask()
    {
      this.State = HttpRequestTaskState.Working;
      try
      {
        HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(this._address);
        httpWebRequest.Accept = "application/json";
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "GET";
        httpWebRequest.UserAgent = "WarRide Server";
        this._httpWebResponse = (HttpWebResponse) await httpWebRequest.GetResponseAsync();
        Stream responseStream = this._httpWebResponse.GetResponseStream();
        byte[] readBuffer = new byte[1024];
        StringBuilder requestData = new StringBuilder("");
        int count;
        do
        {
          count = await responseStream.ReadAsync(readBuffer, 0, 1024);
          if (count > 0)
            requestData.Append(Encoding.ASCII.GetString(readBuffer, 0, count));
        }
        while (count > 0);
        string responseData = "";
        if (requestData.Length > 1)
          responseData = requestData.ToString();
        responseStream.Close();
        this.SetFinishedAsSuccessful(responseData, this._httpWebResponse.StatusCode);
        responseStream = (Stream) null;
        readBuffer = (byte[]) null;
        requestData = (StringBuilder) null;
      }
      catch (Exception ex)
      {
        this.SetFinishedAsUnsuccessful(ex);
      }
    }
  }
}
