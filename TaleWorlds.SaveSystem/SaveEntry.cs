// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.SaveEntry
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
  public class SaveEntry
  {
    private byte[] _data;

    public byte[] Data => this._data;

    public EntryId Id { get; private set; }

    public int FolderId { get; private set; }

    public static SaveEntry CreateFrom(int entryFolderId, EntryId entryId, byte[] data) => new SaveEntry()
    {
      FolderId = entryFolderId,
      Id = entryId,
      _data = data
    };

    public static SaveEntry CreateNew(SaveEntryFolder parentFolder, EntryId entryId) => new SaveEntry()
    {
      Id = entryId,
      FolderId = parentFolder.GlobalId
    };

    public BinaryReader GetBinaryReader() => new BinaryReader(this._data);

    public void FillFrom(BinaryWriter writer) => this._data = writer.Data;
  }
}
