// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ResourceDepotFile
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library
{
  public class ResourceDepotFile
  {
    public ResourceDepotLocation ResourceDepotLocation { get; private set; }

    public string BasePath => this.ResourceDepotLocation.BasePath;

    public string Location => this.ResourceDepotLocation.Path;

    public string FileName { get; private set; }

    public string FullPath { get; private set; }

    public string FullPathLowerCase { get; private set; }

    public ResourceDepotFile(
      ResourceDepotLocation resourceDepotLocation,
      string fileName,
      string fullPath)
    {
      this.ResourceDepotLocation = resourceDepotLocation;
      this.FileName = fileName;
      this.FullPath = fullPath;
      this.FullPathLowerCase = fullPath.ToLower();
    }
  }
}
