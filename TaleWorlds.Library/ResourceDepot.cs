// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ResourceDepot
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace TaleWorlds.Library
{
  public class ResourceDepot
  {
    private List<ResourceDepotLocation> _resourceLocations;
    private Dictionary<string, ResourceDepotFile> _files;
    private bool _isThereAnyUnhandledChange;

    public event ResourceChangeEvent OnResourceChange;

    public ResourceDepot()
    {
      this._resourceLocations = new List<ResourceDepotLocation>();
      this._files = new Dictionary<string, ResourceDepotFile>();
    }

    public void AddLocation(string basePath, string location) => this._resourceLocations.Add(new ResourceDepotLocation(basePath, location, Path.GetFullPath(basePath + location)));

    public void CollectResources()
    {
      this._files.Clear();
      foreach (ResourceDepotLocation resourceLocation in this._resourceLocations)
      {
        string fullPath1 = Path.GetFullPath(resourceLocation.BasePath + resourceLocation.Path);
        foreach (string file in Directory.GetFiles(resourceLocation.BasePath + resourceLocation.Path, "*", SearchOption.AllDirectories))
        {
          string fullPath2 = Path.GetFullPath(file).Replace('\\', '/');
          string fileName = fullPath2.Substring(fullPath1.Length);
          string lower = fileName.ToLower();
          ResourceDepotFile resourceDepotFile = new ResourceDepotFile(resourceLocation, fileName, fullPath2);
          if (this._files.ContainsKey(lower))
            this._files[lower] = resourceDepotFile;
          else
            this._files.Add(lower, resourceDepotFile);
        }
      }
    }

    public string[] GetFiles(string subDirectory, string extension, bool excludeSubContents = false)
    {
      string lower1 = extension.ToLower();
      List<string> stringList = new List<string>();
      foreach (ResourceDepotFile resourceDepotFile in this._files.Values)
      {
        string lower2 = Path.GetFullPath(resourceDepotFile.BasePath + resourceDepotFile.Location + subDirectory).Replace('\\', '/').ToLower();
        string fullPath = resourceDepotFile.FullPath;
        string fullPathLowerCase = resourceDepotFile.FullPathLowerCase;
        if (((excludeSubContents || !fullPathLowerCase.StartsWith(lower2) ? (!excludeSubContents ? 0 : (string.Equals(Directory.GetParent(lower2).FullName, lower2, StringComparison.CurrentCultureIgnoreCase) ? 1 : 0)) : 1) & (fullPathLowerCase.EndsWith(lower1, StringComparison.OrdinalIgnoreCase) ? 1 : 0)) != 0)
          stringList.Add(fullPath);
      }
      return stringList.ToArray();
    }

    public string GetFilePath(string file)
    {
      file = file.Replace('\\', '/');
      return this._files[file.ToLower()].FullPath;
    }

    public IEnumerable<string> GetFilesEndingWith(string fileEndName)
    {
      fileEndName = fileEndName.Replace('\\', '/');
      foreach (KeyValuePair<string, ResourceDepotFile> file in this._files)
      {
        if (file.Key.EndsWith(fileEndName.ToLower()))
          yield return file.Value.FullPath;
      }
    }

    public void StartWatchingChangesInDepot()
    {
      foreach (ResourceDepotLocation resourceLocation in this._resourceLocations)
        resourceLocation.StartWatchingChanges(new FileSystemEventHandler(this.OnAnyChangeInDepotLocations), new RenamedEventHandler(this.OnAnyRenameInDepotLocations));
    }

    public void StopWatchingChangesInDepot()
    {
      foreach (ResourceDepotLocation resourceLocation in this._resourceLocations)
        resourceLocation.StopWatchingChanges();
    }

    private void OnAnyChangeInDepotLocations(object source, FileSystemEventArgs e) => this._isThereAnyUnhandledChange = true;

    private void OnAnyRenameInDepotLocations(object source, RenamedEventArgs e) => this._isThereAnyUnhandledChange = true;

    public void CheckForChanges()
    {
      if (!this._isThereAnyUnhandledChange)
        return;
      this.CollectResources();
      ResourceChangeEvent onResourceChange = this.OnResourceChange;
      if (onResourceChange != null)
        onResourceChange();
      this._isThereAnyUnhandledChange = false;
    }
  }
}
