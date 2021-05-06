// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.ContainerLoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
  internal class ContainerLoadData
  {
    private SaveId _saveId;
    private int _elementCount;
    private ContainerType _containerType;
    private ElementLoadData[] _keys;
    private ElementLoadData[] _values;
    private Dictionary<int, ObjectLoadData> _childStructs;

    public int Id => this.ContainerHeaderLoadData.Id;

    public object Target => this.ContainerHeaderLoadData.Target;

    public LoadContext Context => this.ContainerHeaderLoadData.Context;

    public ContainerDefinition TypeDefinition => this.ContainerHeaderLoadData.TypeDefinition;

    public ContainerHeaderLoadData ContainerHeaderLoadData { get; private set; }

    public ContainerLoadData(ContainerHeaderLoadData headerLoadData)
    {
      this.ContainerHeaderLoadData = headerLoadData;
      this._childStructs = new Dictionary<int, ObjectLoadData>();
      this._saveId = headerLoadData.SaveId;
      this._containerType = headerLoadData.ContainerType;
      this._elementCount = headerLoadData.ElementCount;
      this._keys = new ElementLoadData[this._elementCount];
      this._values = new ElementLoadData[this._elementCount];
    }

    private FolderId[] GetChildStructNames(SaveEntryFolder saveEntryFolder)
    {
      List<FolderId> folderIdList = new List<FolderId>();
      foreach (SaveEntryFolder childFolder in saveEntryFolder.ChildFolders)
      {
        if (childFolder.FolderId.Extension == SaveFolderExtension.Struct && !folderIdList.Contains(childFolder.FolderId))
          folderIdList.Add(childFolder.FolderId);
      }
      return folderIdList.ToArray();
    }

    public void InitializeReaders(SaveEntryFolder saveEntryFolder)
    {
      foreach (FolderId childStructName in this.GetChildStructNames(saveEntryFolder))
      {
        int localId = childStructName.LocalId;
        ObjectLoadData objectLoadData = new ObjectLoadData(this.Context, localId);
        this._childStructs.Add(localId, objectLoadData);
      }
      for (int id = 0; id < this._elementCount; ++id)
      {
        ElementLoadData elementLoadData1 = new ElementLoadData(this, (IReader) saveEntryFolder.GetEntry(new EntryId(id, SaveEntryExtension.Value)).GetBinaryReader());
        this._values[id] = elementLoadData1;
        if (this._containerType == ContainerType.Dictionary)
        {
          ElementLoadData elementLoadData2 = new ElementLoadData(this, (IReader) saveEntryFolder.GetEntry(new EntryId(id, SaveEntryExtension.Key)).GetBinaryReader());
          this._keys[id] = elementLoadData2;
        }
      }
      foreach (KeyValuePair<int, ObjectLoadData> childStruct in this._childStructs)
      {
        int key = childStruct.Key;
        childStruct.Value.InitializeReaders(saveEntryFolder.GetChildFolder(new FolderId(key, SaveFolderExtension.Struct)));
      }
    }

    public void FillCreatedObject()
    {
      foreach (ObjectLoadData objectLoadData in this._childStructs.Values)
        objectLoadData.CreateStruct();
    }

    public void Read()
    {
      for (int index = 0; index < this._elementCount; ++index)
      {
        this._values[index].Read();
        if (this._containerType == ContainerType.Dictionary)
          this._keys[index].Read();
      }
      foreach (ObjectLoadData objectLoadData in this._childStructs.Values)
        objectLoadData.Read();
    }

    private static Assembly GetAssemblyByName(string name) => ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).SingleOrDefault<Assembly>((Func<Assembly, bool>) (assembly => assembly.GetName().FullName == name));

    public void FillObject()
    {
      foreach (ObjectLoadData objectLoadData in this._childStructs.Values)
        objectLoadData.FillObject();
      for (int index1 = 0; index1 < this._elementCount; ++index1)
      {
        if (this._containerType == ContainerType.List)
        {
          IList target = (IList) this.Target;
          ElementLoadData elementLoadData = this._values[index1];
          if (elementLoadData.SavedMemberType == SavedMemberType.CustomStruct)
          {
            ObjectLoadData childStruct = this._childStructs[(int) elementLoadData.Data];
            elementLoadData.SetCustomStructData(childStruct.Target);
          }
          object dataToUse = elementLoadData.GetDataToUse();
          target?.Add(dataToUse);
        }
        else if (this._containerType == ContainerType.Dictionary)
        {
          IDictionary target = (IDictionary) this.Target;
          ElementLoadData key = this._keys[index1];
          ElementLoadData elementLoadData = this._values[index1];
          if (key.SavedMemberType == SavedMemberType.CustomStruct)
          {
            ObjectLoadData childStruct = this._childStructs[(int) key.Data];
            key.SetCustomStructData(childStruct.Target);
          }
          if (elementLoadData.SavedMemberType == SavedMemberType.CustomStruct)
          {
            ObjectLoadData childStruct = this._childStructs[(int) elementLoadData.Data];
            elementLoadData.SetCustomStructData(childStruct.Target);
          }
          object dataToUse1 = key.GetDataToUse();
          object dataToUse2 = elementLoadData.GetDataToUse();
          if (target != null && dataToUse1 != null)
            target.Add(dataToUse1, dataToUse2);
        }
        else if (this._containerType == ContainerType.Array)
        {
          Array target = (Array) this.Target;
          ElementLoadData elementLoadData = this._values[index1];
          if (elementLoadData.SavedMemberType == SavedMemberType.CustomStruct)
          {
            ObjectLoadData childStruct = this._childStructs[(int) elementLoadData.Data];
            elementLoadData.SetCustomStructData(childStruct.Target);
          }
          object dataToUse = elementLoadData.GetDataToUse();
          int index2 = index1;
          target.SetValue(dataToUse, index2);
        }
        else if (this._containerType == ContainerType.Queue)
        {
          ICollection target = (ICollection) this.Target;
          ElementLoadData elementLoadData = this._values[index1];
          if (elementLoadData.SavedMemberType == SavedMemberType.CustomStruct)
          {
            ObjectLoadData childStruct = this._childStructs[(int) elementLoadData.Data];
            elementLoadData.SetCustomStructData(childStruct.Target);
          }
          object dataToUse = elementLoadData.GetDataToUse();
          target.GetType().GetMethod("Enqueue").Invoke((object) target, new object[1]
          {
            dataToUse
          });
        }
      }
    }
  }
}
