// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ParameterFile
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.IO;
using System.Xml;

namespace TaleWorlds.Library
{
  public class ParameterFile
  {
    private int _failedAttemptsCount;
    private const int MaxFailedAttemptsCount = 100;

    public string Path { get; private set; }

    public DateTime LastCheckedTime { get; private set; }

    public ParameterContainer ParameterContainer { get; private set; }

    public ParameterFile(string path)
    {
      this.ParameterContainer = new ParameterContainer();
      this.Path = path;
      this.LastCheckedTime = DateTime.MinValue;
    }

    public bool CheckIfNeedsToBeRefreshed() => File.GetLastWriteTime(this.Path) > this.LastCheckedTime;

    public void Refresh()
    {
      this.ParameterContainer.ClearParameters();
      DateTime lastWriteTime = File.GetLastWriteTime(this.Path);
      XmlDocument xmlDocument = new XmlDocument();
      try
      {
        xmlDocument.Load(this.Path);
      }
      catch
      {
        ++this._failedAttemptsCount;
        int failedAttemptsCount = this._failedAttemptsCount;
        return;
      }
      this._failedAttemptsCount = 0;
      foreach (XmlElement childNode in xmlDocument.FirstChild.ChildNodes)
        this.ParameterContainer.AddParameter(childNode.GetAttribute("name"), childNode.GetAttribute("value"), true);
      this.LastCheckedTime = lastWriteTime;
    }
  }
}
