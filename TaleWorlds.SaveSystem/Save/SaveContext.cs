// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Save.SaveContext
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
  internal class SaveContext : ISaveContext
  {
    private List<object> _childObjects;
    private Dictionary<object, int> _idsOfChildObjects;
    private List<object> _childContainers;
    private Dictionary<object, int> _idsOfChildContainers;
    private List<string> _strings;
    private Dictionary<string, int> _idsOfStrings;
    private List<object> _temporaryCollectedObjects;
    private object _locker;
    private Queue<object> _objectsToIterate;

    public object RootObject { get; private set; }

    public GameData SaveData { get; private set; }

    public DefinitionContext DefinitionContext { get; private set; }

    public SaveContext(DefinitionContext definitionContext)
    {
      this.DefinitionContext = definitionContext;
      this._childObjects = new List<object>(131072);
      this._idsOfChildObjects = new Dictionary<object, int>(131072);
      this._strings = new List<string>(131072);
      this._idsOfStrings = new Dictionary<string, int>(131072);
      this._childContainers = new List<object>(131072);
      this._idsOfChildContainers = new Dictionary<object, int>(131072);
      this._temporaryCollectedObjects = new List<object>(4096);
      this._locker = new object();
    }

    private void CollectObjects()
    {
      using (new PerformanceTestBlock("SaveContext::CollectObjects"))
      {
        this._objectsToIterate = new Queue<object>(1024);
        this._objectsToIterate.Enqueue(this.RootObject);
        while (this._objectsToIterate.Count > 0)
        {
          object parent = this._objectsToIterate.Dequeue();
          ContainerType containerType;
          if (parent.GetType().IsContainer(out containerType))
            this.CollectContainerObjects(containerType, parent);
          else
            this.CollectObjects(parent);
        }
      }
    }

    private void CollectContainerObjects(ContainerType containerType, object parent)
    {
      if (this._idsOfChildContainers.ContainsKey(parent))
        return;
      int count = this._childContainers.Count;
      this._childContainers.Add(parent);
      this._idsOfChildContainers.Add(parent, count);
      ContainerSaveData.GetChildObjects((ISaveContext) this, this.DefinitionContext.GetContainerDefinition(parent.GetType()), containerType, parent, this._temporaryCollectedObjects);
      for (int index = 0; index < this._temporaryCollectedObjects.Count; ++index)
      {
        object temporaryCollectedObject = this._temporaryCollectedObjects[index];
        if (temporaryCollectedObject != null)
          this._objectsToIterate.Enqueue(temporaryCollectedObject);
      }
      this._temporaryCollectedObjects.Clear();
    }

    private void CollectObjects(object parent)
    {
      if (this._idsOfChildObjects.ContainsKey(parent))
        return;
      int count = this._childObjects.Count;
      this._childObjects.Add(parent);
      this._idsOfChildObjects.Add(parent, count);
      Type type = parent.GetType();
      ObjectSaveData.GetChildObjects((ISaveContext) this, this.DefinitionContext.GetClassDefinition(type) ?? throw new Exception("Could not find type definition of type: " + (object) type), parent, this._temporaryCollectedObjects);
      for (int index = 0; index < this._temporaryCollectedObjects.Count; ++index)
      {
        object temporaryCollectedObject = this._temporaryCollectedObjects[index];
        if (temporaryCollectedObject != null)
          this._objectsToIterate.Enqueue(temporaryCollectedObject);
      }
      this._temporaryCollectedObjects.Clear();
    }

    public void AddStrings(List<string> texts)
    {
      lock (this._locker)
      {
        for (int index = 0; index < texts.Count; ++index)
        {
          string text = texts[index];
          if (text != null && !this._idsOfStrings.ContainsKey(text))
          {
            int count = this._strings.Count;
            this._idsOfStrings.Add(text, count);
            this._strings.Add(text);
          }
        }
      }
    }

    public int AddOrGetStringId(string text)
    {
      int num1 = -1;
      if (text == null)
      {
        num1 = -1;
      }
      else
      {
        lock (this._locker)
        {
          int num2;
          if (this._idsOfStrings.TryGetValue(text, out num2))
          {
            num1 = num2;
          }
          else
          {
            num1 = this._strings.Count;
            this._idsOfStrings.Add(text, num1);
            this._strings.Add(text);
          }
        }
      }
      return num1;
    }

    public int GetObjectId(object target) => this._idsOfChildObjects[target];

    public int GetContainerId(object target) => this._idsOfChildContainers[target];

    public int GetStringId(string target)
    {
      if (target == null)
        return -1;
      lock (this._locker)
        return this._idsOfStrings[target];
    }

    private static void SaveStringTo(SaveEntryFolder stringsFolder, int id, string value)
    {
      BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
      binaryWriter.WriteString(value);
      stringsFolder.CreateEntry(new EntryId(id, SaveEntryExtension.Txt)).FillFrom(binaryWriter);
      BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
    }

    public bool Save(object target, MetaData metaData)
    {
      Debug.Print("SaveContext::Save");
      try
      {
        this.RootObject = target;
        using (new PerformanceTestBlock("SaveContext::Save"))
        {
          BinaryWriterFactory.Initialize();
          this.CollectObjects();
          ArchiveConcurrentSerializer headerSerializer = new ArchiveConcurrentSerializer();
          byte[][] objectData = new byte[this._childObjects.Count][];
          using (new PerformanceTestBlock("SaveContext::Saving Objects"))
            TWParallel.For(0, this._childObjects.Count, (Action<int>) (i => this.SaveSingleObject(headerSerializer, objectData, i)));
          byte[][] containerData = new byte[this._childContainers.Count][];
          using (new PerformanceTestBlock("SaveContext::Saving Containers"))
            TWParallel.For(0, this._childContainers.Count, (Action<int>) (i => this.SaveSingleContainer(headerSerializer, containerData, i)));
          using (new PerformanceTestBlock("SaveContext::SaveObjects config"))
          {
            SaveEntryFolder rootFolder = SaveEntryFolder.CreateRootFolder();
            BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
            binaryWriter.WriteInt(this._idsOfChildObjects.Count);
            binaryWriter.WriteInt(this._strings.Count);
            binaryWriter.WriteInt(this._idsOfChildContainers.Count);
            rootFolder.CreateEntry(new EntryId(-1, SaveEntryExtension.Config)).FillFrom(binaryWriter);
            headerSerializer.SerializeFolderConcurrent(rootFolder);
            BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
          }
          ArchiveSerializer archiveSerializer = new ArchiveSerializer();
          using (new PerformanceTestBlock("SaveContext::SaveObjects strings"))
          {
            SaveEntryFolder rootFolder = SaveEntryFolder.CreateRootFolder();
            SaveEntryFolder folder = archiveSerializer.CreateFolder(rootFolder, new FolderId(-1, SaveFolderExtension.Strings), this._strings.Count);
            for (int index = 0; index < this._strings.Count; ++index)
            {
              string str = this._strings[index];
              SaveContext.SaveStringTo(folder, index, str);
            }
            archiveSerializer.SerializeFolder(rootFolder);
          }
          byte[] header = (byte[]) null;
          byte[] strings = (byte[]) null;
          using (new PerformanceTestBlock("SaveContext::FinalizeAndGetBinaryHeaderDataConcurrent"))
            header = headerSerializer.FinalizeAndGetBinaryDataConcurrent();
          using (new PerformanceTestBlock("SaveContext::FinalizeAndGetBinaryStringDataConcurrent"))
            strings = archiveSerializer.FinalizeAndGetBinaryData();
          this.SaveData = new GameData(header, strings, objectData, containerData);
          BinaryWriterFactory.Release();
        }
        return true;
      }
      catch
      {
        return false;
      }
    }

    private void SaveSingleObject(
      ArchiveConcurrentSerializer headerSerializer,
      byte[][] objectData,
      int id)
    {
      object childObject = this._childObjects[id];
      ArchiveSerializer archiveSerializer = new ArchiveSerializer();
      SaveEntryFolder rootFolder1 = SaveEntryFolder.CreateRootFolder();
      SaveEntryFolder rootFolder2 = SaveEntryFolder.CreateRootFolder();
      ObjectSaveData objectSaveData = new ObjectSaveData((ISaveContext) this, id, childObject, true);
      objectSaveData.CollectStructs();
      objectSaveData.CollectMembers();
      objectSaveData.CollectStrings();
      objectSaveData.SaveHeaderTo(rootFolder2, (IArchiveContext) headerSerializer);
      objectSaveData.SaveTo(rootFolder1, (IArchiveContext) archiveSerializer);
      headerSerializer.SerializeFolderConcurrent(rootFolder2);
      archiveSerializer.SerializeFolder(rootFolder1);
      byte[] binaryData = archiveSerializer.FinalizeAndGetBinaryData();
      objectData[id] = binaryData;
    }

    private void SaveSingleContainer(
      ArchiveConcurrentSerializer headerSerializer,
      byte[][] containerData,
      int id)
    {
      object childContainer = this._childContainers[id];
      ArchiveSerializer archiveSerializer = new ArchiveSerializer();
      SaveEntryFolder rootFolder1 = SaveEntryFolder.CreateRootFolder();
      SaveEntryFolder rootFolder2 = SaveEntryFolder.CreateRootFolder();
      ContainerType containerType;
      childContainer.GetType().IsContainer(out containerType);
      ContainerSaveData containerSaveData = new ContainerSaveData((ISaveContext) this, id, childContainer, containerType);
      containerSaveData.CollectChildren();
      containerSaveData.CollectStructs();
      containerSaveData.CollectMembers();
      containerSaveData.CollectStrings();
      containerSaveData.SaveHeaderTo(rootFolder2, (IArchiveContext) headerSerializer);
      containerSaveData.SaveTo(rootFolder1, (IArchiveContext) archiveSerializer);
      headerSerializer.SerializeFolderConcurrent(rootFolder2);
      archiveSerializer.SerializeFolder(rootFolder1);
      byte[] binaryData = archiveSerializer.FinalizeAndGetBinaryData();
      containerData[id] = binaryData;
    }
  }
}
