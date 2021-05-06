// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.ArchiveConcurrentSerializer
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
  internal class ArchiveConcurrentSerializer : IArchiveContext
  {
    private int _entryCount;
    private int _folderCount;
    private object _locker;
    private Dictionary<int, BinaryWriter> _writers;
    private ConcurrentBag<SaveEntryFolder> _folders;

    public ArchiveConcurrentSerializer()
    {
      this._locker = new object();
      this._writers = new Dictionary<int, BinaryWriter>();
      this._folders = new ConcurrentBag<SaveEntryFolder>();
    }

    public void SerializeFolderConcurrent(SaveEntryFolder folder)
    {
      int managedThreadId = Thread.CurrentThread.ManagedThreadId;
      BinaryWriter writer;
      lock (this._locker)
      {
        if (!this._writers.TryGetValue(managedThreadId, out writer))
        {
          writer = new BinaryWriter(262144);
          this._writers.Add(managedThreadId, writer);
        }
      }
      foreach (SaveEntry allEntry in folder.AllEntries)
        this.SerializeEntryConcurrent(allEntry, writer);
    }

    public SaveEntryFolder CreateFolder(
      SaveEntryFolder parentFolder,
      FolderId folderId,
      int entryCount)
    {
      int globalId = Interlocked.Increment(ref this._folderCount) - 1;
      SaveEntryFolder saveEntryFolder = new SaveEntryFolder(parentFolder, globalId, folderId, entryCount);
      parentFolder.AddChildFolderEntry(saveEntryFolder);
      this._folders.Add(saveEntryFolder);
      return saveEntryFolder;
    }

    private void SerializeEntryConcurrent(SaveEntry entry, BinaryWriter writer)
    {
      BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
      binaryWriter.Write3ByteInt(entry.FolderId);
      binaryWriter.Write3ByteInt(entry.Id.Id);
      binaryWriter.WriteByte((byte) entry.Id.Extension);
      binaryWriter.WriteShort((short) entry.Data.Length);
      binaryWriter.WriteBytes(entry.Data);
      byte[] data = binaryWriter.Data;
      BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
      writer.WriteBytes(data);
      Interlocked.Increment(ref this._entryCount);
    }

    public byte[] FinalizeAndGetBinaryDataConcurrent()
    {
      BinaryWriter binaryWriter = new BinaryWriter();
      binaryWriter.WriteInt(this._folderCount);
      foreach (SaveEntryFolder folder in this._folders)
      {
        int parentGlobalId = folder.ParentGlobalId;
        int globalId = folder.GlobalId;
        FolderId folderId = folder.FolderId;
        int localId = folderId.LocalId;
        folderId = folder.FolderId;
        SaveFolderExtension extension = folderId.Extension;
        binaryWriter.Write3ByteInt(parentGlobalId);
        binaryWriter.Write3ByteInt(globalId);
        binaryWriter.Write3ByteInt(localId);
        binaryWriter.WriteByte((byte) extension);
      }
      binaryWriter.WriteInt(this._entryCount);
      foreach (BinaryWriter writer in this._writers.Values)
        binaryWriter.AppendData(writer);
      return binaryWriter.Data;
    }
  }
}
