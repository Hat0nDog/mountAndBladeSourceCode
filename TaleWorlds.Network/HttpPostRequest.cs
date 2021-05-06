// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.HttpPostRequest
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace TaleWorlds.Network
{
  public class HttpPostRequest : IHttpRequestTask
  {
    private const int BufferSize = 1024;
    private HttpWebRequest _httpWebRequest;
    private HttpWebResponse _httpWebResponse;
    private Stream _responseStream;
    private readonly byte[] _readBuffer;
    private readonly StringBuilder _requestData;
    private readonly string _address;
    private string _postData;

    public HttpRequestTaskState State { get; private set; }

    public bool Successful { get; private set; }

    public string ResponseData { get; private set; }

    public Exception Exception { get; private set; }

    static HttpPostRequest() => ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

    public HttpPostRequest(string address, string postData)
    {
      this._postData = postData;
      this._address = address;
      this.State = HttpRequestTaskState.NotStarted;
      this._readBuffer = new byte[1024];
      this._requestData = new StringBuilder("");
      this.ResponseData = "";
    }

    private void SetFinishedAsSuccessful(string responseData)
    {
      this.Successful = true;
      this.ResponseData = responseData;
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

    private async Task DoTask()
    {
      this.State = HttpRequestTaskState.Working;
      try
      {
        Debug.Print("Http Post Request to " + this._address);
        byte[] postData = Encoding.Unicode.GetBytes(this._postData);
        this._httpWebRequest = (HttpWebRequest) WebRequest.Create(this._address);
        this._httpWebRequest.Accept = "application/json";
        this._httpWebRequest.ContentType = "application/json; charset=utf-16";
        this._httpWebRequest.Method = "POST";
        this._httpWebRequest.ContentLength = (long) postData.Length;
        this._httpWebRequest.UserAgent = "TaleWorlds Client";
        this._httpWebRequest.TransferEncoding = "";
        Stream requestStreamAsync = await this._httpWebRequest.GetRequestStreamAsync();
        requestStreamAsync.Write(postData, 0, postData.Length);
        requestStreamAsync.Close();
        this._httpWebResponse = (HttpWebResponse) await this._httpWebRequest.GetResponseAsync();
        this._responseStream = this._httpWebResponse.GetResponseStream();
        int count;
        do
        {
          count = await this._responseStream.ReadAsync(this._readBuffer, 0, 1024);
          if (count > 0)
            this._requestData.Append(Encoding.ASCII.GetString(this._readBuffer, 0, count));
        }
        while (count > 0);
        string responseData = "";
        if (this._requestData.Length > 1)
          responseData = this._requestData.ToString();
        this._responseStream.Close();
        this.SetFinishedAsSuccessful(responseData);
        postData = (byte[]) null;
      }
      catch (Exception ex)
      {
        this.SetFinishedAsUnsuccessful(ex);
      }
    }
  }
}
