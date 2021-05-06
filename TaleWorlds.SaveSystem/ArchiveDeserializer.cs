// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.ArchiveDeserializer
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
  internal class ArchiveDeserializer
  {
    public SaveEntryFolder RootFolder { get; private set; }

    public ArchiveDeserializer() => this.RootFolder = new SaveEntryFolder(-1, -1, new FolderId(-1, SaveFolderExtension.Root), 3);

    public void LoadFrom(byte[] binaryArchive)
    {
      Dictionary<int, SaveEntryFolder> dictionary = new Dictionary<int, SaveEntryFolder>();
      List<SaveEntry> saveEntryList = new List<SaveEntry>();
      BinaryReader binaryReader = new BinaryReader(binaryArchive);
      int num1 = binaryReader.ReadInt();
      for (int index = 0; index < num1; ++index)
      {
        SaveEntryFolder saveEntryFolder = new SaveEntryFolder(binaryReader.Read3ByteInt(), binaryReader.Read3ByteInt(), new FolderId(binaryReader.Read3ByteInt(), (SaveFolderExtension) binaryReader.ReadByte()), 3);
        dictionary.Add(saveEntryFolder.GlobalId, saveEntryFolder);
      }
      int num2 = binaryReader.ReadInt();
      for (int index = 0; index < num2; ++index)
      {
        int entryFolderId = binaryReader.Read3ByteInt();
        int id = binaryReader.Read3ByteInt();
        SaveEntryExtension extension = (SaveEntryExtension) binaryReader.ReadByte();
        short num3 = binaryReader.ReadShort();
        byte[] data = binaryReader.ReadBytes((int) num3);
        SaveEntry from = SaveEntry.CreateFrom(entryFolderId, new EntryId(id, extension), data);
        saveEntryList.Add(from);
      }
      foreach (SaveEntryFolder saveEntryFolder in dictionary.Values)
      {
        if (saveEntryFolder.ParentGlobalId != -1)
          dictionary[saveEntryFolder.ParentGlobalId].AddChildFolderEntry(saveEntryFolder);
        else
          this.RootFolder.AddChildFolderEntry(saveEntryFolder);
      }
      foreach (SaveEntry saveEntry in saveEntryList)
      {
        if (saveEntry.FolderId != -1)
          dictionary[saveEntry.FolderId].AddEntry(saveEntry);
        else
          this.RootFolder.AddEntry(saveEntry);
      }
    }
  }
}
