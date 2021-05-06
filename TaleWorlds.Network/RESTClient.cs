// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Network.RESTClient
// Assembly: TaleWorlds.Network, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF757B2-6540-4679-910A-3C5D58B2EF82
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Network.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Network
{
  public class RESTClient
  {
    private string _serviceAddress;

    public RESTClient(string serviceAddress) => this._serviceAddress = serviceAddress;

    private ServiceException GetServiceErrorCode(Stream stream)
    {
      string end = new StreamReader(stream).ReadToEnd();
      JObject jobject = JObject.Parse(end);
      if (jobject["ExceptionMessage"] != null)
        return JsonConvert.DeserializeObject<ServiceExceptionModel>(end).ToServiceException();
      if (jobject["error_description"] == null)
        return new ServiceException("unknown", string.Empty);
      return (string) jobject["error"] == "invalid_grant" ? new ServiceException(string.Empty, "InvalidUsernameOrPassword") : new ServiceException((string) jobject["error"], (string) jobject["error_description"]);
    }

    private HttpWebRequest CreateHttpRequest(
      string service,
      List<KeyValuePair<string, string>> headers,
      string contentType,
      string method)
    {
      HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(new Uri(this._serviceAddress + service));
      httpWebRequest.Accept = "application/json";
      httpWebRequest.ContentType = contentType;
      httpWebRequest.Method = method;
      if (headers != null)
      {
        foreach (KeyValuePair<string, string> header in headers)
          httpWebRequest.Headers.Add(header.Key, header.Value);
      }
      return httpWebRequest;
    }

    public async Task<TResult> Get<TResult>(
      string service,
      List<KeyValuePair<string, string>> headers)
    {
      HttpWebRequest httpRequest = this.CreateHttpRequest(service, headers, "application/json", "GET");
      TResult result;
      try
      {
        result = JsonConvert.DeserializeObject<TResult>(new StreamReader((await httpRequest.GetResponseAsync()).GetResponseStream()).ReadToEnd());
      }
      catch (WebException ex)
      {
        if (ex.Response != null)
          throw this.GetServiceErrorCode(ex.Response.GetResponseStream());
        throw new Exception("HTTP Get Web Request Failed", (Exception) ex);
      }
      catch (Exception ex)
      {
        throw new Exception("HTTP Get Failed", ex);
      }
      return result;
    }

    public async Task Get(string service, List<KeyValuePair<string, string>> headers)
    {
      HttpWebRequest httpRequest = this.CreateHttpRequest(service, headers, "application/json", "GET");
      try
      {
        WebResponse responseAsync = await httpRequest.GetResponseAsync();
      }
      catch (WebException ex)
      {
        if (ex.Response != null)
          throw this.GetServiceErrorCode(ex.Response.GetResponseStream());
        throw new Exception("HTTP Get Web Request Failed", (Exception) ex);
      }
      catch (Exception ex)
      {
        throw new Exception("HTTP Get Failed", ex);
      }
    }

    public async Task<TResult> Post<TResult>(
      string service,
      List<KeyValuePair<string, string>> headers,
      string payLoad,
      string contentType = "application/json")
    {
      HttpWebRequest http = this.CreateHttpRequest(service, headers, contentType, "POST");
      byte[] bytes = new ASCIIEncoding().GetBytes(payLoad);
      TResult result;
      try
      {
        Stream requestStreamAsync = await http.GetRequestStreamAsync();
        requestStreamAsync.Write(bytes, 0, bytes.Length);
        requestStreamAsync.Close();
        result = JsonConvert.DeserializeObject<TResult>(new StreamReader((await http.GetResponseAsync()).GetResponseStream()).ReadToEnd());
      }
      catch (WebException ex)
      {
        HttpWebResponse httpWebResponse = ex.Response != null ? (HttpWebResponse) ex.Response : throw new Exception("HTTP Post Web Request Failed", (Exception) ex);
        if (httpWebResponse.StatusCode == HttpStatusCode.Unauthorized || httpWebResponse.StatusCode == HttpStatusCode.NotFound)
          throw ex;
        throw this.GetServiceErrorCode(ex.Response.GetResponseStream());
      }
      catch (Exception ex)
      {
        throw new Exception("HTTP Post Failed", ex);
      }
      return result;
    }

    public async Task Post(
      string service,
      List<KeyValuePair<string, string>> headers,
      string payLoad,
      string contentType = "application/json")
    {
      HttpWebRequest http = this.CreateHttpRequest(service, headers, contentType, "POST");
      byte[] bytes = new ASCIIEncoding().GetBytes(payLoad);
      try
      {
        Stream requestStreamAsync = await http.GetRequestStreamAsync();
        requestStreamAsync.Write(bytes, 0, bytes.Length);
        requestStreamAsync.Close();
        WebResponse responseAsync = await http.GetResponseAsync();
      }
      catch (WebException ex)
      {
        if (ex.Response != null)
          throw this.GetServiceErrorCode(ex.Response.GetResponseStream());
        throw new Exception("HTTP Post Web Request Failed", (Exception) ex);
      }
      catch (Exception ex)
      {
        throw new Exception("HTTP Post Failed", ex);
      }
    }
  }
}
