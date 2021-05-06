// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.ArchiveSerializer
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Generic;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
  internal class ArchiveSerializer : IArchiveContext
  {
    private BinaryWriter _writer;
    private int _entryCount;
    private int _folderCount;
    private List<SaveEntryFolder> _folders;

    public ArchiveSerializer()
    {
      this._writer = BinaryWriterFactory.GetBinaryWriter();
      this._folders = new List<SaveEntryFolder>();
    }

    public void SerializeEntry(SaveEntry entry)
    {
      this._writer.Write3ByteInt(entry.FolderId);
      this._writer.Write3ByteInt(entry.Id.Id);
      this._writer.WriteByte((byte) entry.Id.Extension);
      this._writer.WriteShort((short) entry.Data.Length);
      this._writer.WriteBytes(entry.Data);
      ++this._entryCount;
    }

    public void SerializeFolder(SaveEntryFolder folder)
    {
      foreach (SaveEntry allEntry in folder.AllEntries)
        this.SerializeEntry(allEntry);
    }

    public SaveEntryFolder CreateFolder(
      SaveEntryFolder parentFolder,
      FolderId folderId,
      int entryCount)
    {
      int folderCount = this._folderCount;
      ++this._folderCount;
      SaveEntryFolder saveEntryFolder = new SaveEntryFolder(parentFolder, folderCount, folderId, entryCount);
      parentFolder.AddChildFolderEntry(saveEntryFolder);
      this._folders.Add(saveEntryFolder);
      return saveEntryFolder;
    }

    public byte[] FinalizeAndGetBinaryData()
    {
      BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
      binaryWriter.WriteInt(this._folderCount);
      for (int index = 0; index < this._folderCount; ++index)
      {
        SaveEntryFolder folder = this._folders[index];
        int parentGlobalId = folder.ParentGlobalId;
        int globalId = folder.GlobalId;
        int localId = folder.FolderId.LocalId;
        SaveFolderExtension extension = folder.FolderId.Extension;
        binaryWriter.Write3ByteInt(parentGlobalId);
        binaryWriter.Write3ByteInt(globalId);
        binaryWriter.Write3ByteInt(localId);
        binaryWriter.WriteByte((byte) extension);
      }
      binaryWriter.WriteInt(this._entryCount);
      binaryWriter.AppendData(this._writer);
      byte[] data = binaryWriter.Data;
      BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
      BinaryWriterFactory.ReleaseBinaryWriter(this._writer);
      this._writer = (BinaryWriter) null;
      return data;
    }
  }
}
