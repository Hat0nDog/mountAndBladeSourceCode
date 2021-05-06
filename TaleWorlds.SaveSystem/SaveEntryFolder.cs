// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.SaveEntryFolder
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Generic;

namespace TaleWorlds.SaveSystem
{
  public class SaveEntryFolder
  {
    private Dictionary<FolderId, SaveEntryFolder> _saveEntryFolders;
    private Dictionary<EntryId, SaveEntry> _entries;

    public int GlobalId { get; private set; }

    public int ParentGlobalId { get; private set; }

    public FolderId FolderId { get; private set; }

    public IEnumerable<SaveEntry> AllEntries
    {
      get
      {
        foreach (SaveEntry saveEntry in this._entries.Values)
          yield return saveEntry;
        foreach (SaveEntryFolder saveEntryFolder in this._saveEntryFolders.Values)
        {
          foreach (SaveEntry allEntry in saveEntryFolder.AllEntries)
            yield return allEntry;
        }
      }
    }

    public Dictionary<EntryId, SaveEntry>.ValueCollection ChildEntries => this._entries.Values;

    public static SaveEntryFolder CreateRootFolder() => new SaveEntryFolder(-1, -1, new FolderId(-1, SaveFolderExtension.Root), 3);

    public Dictionary<FolderId, SaveEntryFolder>.ValueCollection ChildFolders => this._saveEntryFolders.Values;

    public SaveEntryFolder(
      SaveEntryFolder parent,
      int globalId,
      FolderId folderId,
      int entryCount)
      : this(parent.GlobalId, globalId, folderId, entryCount)
    {
    }

    public SaveEntryFolder(int parentGlobalId, int globalId, FolderId folderId, int entryCount)
    {
      this.ParentGlobalId = parentGlobalId;
      this.GlobalId = globalId;
      this.FolderId = folderId;
      this._entries = new Dictionary<EntryId, SaveEntry>(entryCount);
      this._saveEntryFolders = new Dictionary<FolderId, SaveEntryFolder>(3);
    }

    public void AddEntry(SaveEntry saveEntry) => this._entries.Add(saveEntry.Id, saveEntry);

    public SaveEntry GetEntry(EntryId entryId) => this._entries[entryId];

    public void AddChildFolderEntry(SaveEntryFolder saveEntryFolder) => this._saveEntryFolders.Add(saveEntryFolder.FolderId, saveEntryFolder);

    internal SaveEntryFolder GetChildFolder(FolderId folderId) => this._saveEntryFolders[folderId];

    public SaveEntry CreateEntry(EntryId entryId)
    {
      SaveEntry saveEntry = SaveEntry.CreateNew(this, entryId);
      this.AddEntry(saveEntry);
      return saveEntry;
    }
  }
}
