// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.PlatformFileHelperPC
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaleWorlds.Library
{
  public class PlatformFileHelperPC : IPlatformFileHelper
  {
    public string DocumentsPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal);

    public async Task<bool> DownloadAndSaveFile(string url, string path)
    {
      bool flag;
      using (WebClient client = new WebClient())
      {
        Task downloadTask = client.DownloadFileTaskAsync(new Uri(url), path);
        await downloadTask;
        flag = downloadTask.Status == TaskStatus.RanToCompletion;
      }
      return flag;
    }

    public void Serialize(string path, object value)
    {
      FileInfo fileInfo = new FileInfo(path);
      if (!Directory.Exists(fileInfo.Directory.FullName))
        Directory.CreateDirectory(fileInfo.Directory.FullName);
      using (StreamWriter text = System.IO.File.CreateText(path))
        new JsonSerializer()
        {
          Formatting = Formatting.Indented
        }.Serialize((TextWriter) text, value);
    }

    public bool FileExists(string path) => System.IO.File.Exists(path);

    public async Task<string> GetFileContentAsync(string path)
    {
      if (!this.FileExists(path))
        return (string) null;
      string empty = string.Empty;
      using (FileStream sourceStream = System.IO.File.Open(path, FileMode.Open))
      {
        byte[] buffer = new byte[sourceStream.Length];
        int num = await sourceStream.ReadAsync(buffer, 0, (int) sourceStream.Length);
        empty = Encoding.ASCII.GetString(buffer);
        buffer = (byte[]) null;
      }
      return empty;
    }

    public void DeleteFile(string path) => System.IO.File.Delete(path);
  }
}
