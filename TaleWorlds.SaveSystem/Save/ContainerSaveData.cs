// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Save.ContainerSaveData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections;
using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
  internal class ContainerSaveData
  {
    private ContainerType _containerType;
    private ElementSaveData[] _keys;
    private ElementSaveData[] _values;
    private ContainerDefinition _typeDefinition;
    private int _elementCount;
    private List<ObjectSaveData> _childStructs;

    public int ObjectId { get; private set; }

    public ISaveContext Context { get; private set; }

    public object Target { get; private set; }

    public Type Type { get; private set; }

    internal int ElementPropertyCount => this._childStructs.Count > 0 ? this._childStructs[0].PropertyCount : 0;

    internal int ElementFieldCount => this._childStructs.Count > 0 ? this._childStructs[0].FieldCount : 0;

    public ContainerSaveData(
      ISaveContext context,
      int objectId,
      object target,
      ContainerType containerType)
    {
      this.ObjectId = objectId;
      this.Context = context;
      this.Target = target;
      this._containerType = containerType;
      this.Type = target.GetType();
      this._elementCount = this.GetElementCount();
      this._childStructs = new List<ObjectSaveData>();
      this._typeDefinition = context.DefinitionContext.GetContainerDefinition(this.Type);
      if (this._typeDefinition == null)
        throw new Exception("Could not find type definition of container type: " + (object) this.Type);
    }

    public void CollectChildren()
    {
      this._keys = new ElementSaveData[this._elementCount];
      this._values = new ElementSaveData[this._elementCount];
      if (this._containerType == ContainerType.Dictionary)
      {
        IDictionary target = (IDictionary) this.Target;
        int index = 0;
        foreach (DictionaryEntry dictionaryEntry in target)
        {
          object key = dictionaryEntry.Key;
          object obj = dictionaryEntry.Value;
          ElementSaveData elementSaveData1 = new ElementSaveData(this, key, index);
          ElementSaveData elementSaveData2 = new ElementSaveData(this, obj, this._elementCount + index);
          this._keys[index] = elementSaveData1;
          this._values[index] = elementSaveData2;
          ++index;
        }
      }
      else if (this._containerType == ContainerType.List)
      {
        IList target = (IList) this.Target;
        for (int index = 0; index < this._elementCount; ++index)
        {
          ElementSaveData elementSaveData = new ElementSaveData(this, target[index], index);
          this._values[index] = elementSaveData;
        }
      }
      else if (this._containerType == ContainerType.Queue)
      {
        ICollection target = (ICollection) this.Target;
        int index = 0;
        foreach (object obj in (IEnumerable) target)
        {
          ElementSaveData elementSaveData = new ElementSaveData(this, obj, index);
          this._values[index] = elementSaveData;
          ++index;
        }
      }
      else
      {
        if (this._containerType != ContainerType.Array)
          return;
        Array target = (Array) this.Target;
        for (int index = 0; index < this._elementCount; ++index)
        {
          ElementSaveData elementSaveData = new ElementSaveData(this, target.GetValue(index), index);
          this._values[index] = elementSaveData;
        }
      }
    }

    public void SaveHeaderTo(SaveEntryFolder parentFolder, IArchiveContext archiveContext)
    {
      SaveEntryFolder folder = archiveContext.CreateFolder(parentFolder, new FolderId(this.ObjectId, SaveFolderExtension.Container), 1);
      BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
      this._typeDefinition.SaveId.WriteTo((IWriter) binaryWriter);
      binaryWriter.WriteByte((byte) this._containerType);
      binaryWriter.WriteInt(this.GetElementCount());
      EntryId entryId = new EntryId(-1, SaveEntryExtension.Object);
      folder.CreateEntry(entryId).FillFrom(binaryWriter);
      BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
    }

    public void SaveTo(SaveEntryFolder parentFolder, IArchiveContext archiveContext)
    {
      int entryCount = this._containerType == ContainerType.Dictionary ? this._elementCount * 2 : this._elementCount;
      SaveEntryFolder folder = archiveContext.CreateFolder(parentFolder, new FolderId(this.ObjectId, SaveFolderExtension.Container), entryCount);
      for (int id = 0; id < this._elementCount; ++id)
      {
        ElementSaveData elementSaveData = this._values[id];
        BinaryWriter binaryWriter1 = BinaryWriterFactory.GetBinaryWriter();
        BinaryWriter binaryWriter2 = binaryWriter1;
        elementSaveData.SaveTo((IWriter) binaryWriter2);
        folder.CreateEntry(new EntryId(id, SaveEntryExtension.Value)).FillFrom(binaryWriter1);
        BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter1);
        if (this._containerType == ContainerType.Dictionary)
        {
          ElementSaveData key = this._keys[id];
          BinaryWriter binaryWriter3 = BinaryWriterFactory.GetBinaryWriter();
          BinaryWriter binaryWriter4 = binaryWriter3;
          key.SaveTo((IWriter) binaryWriter4);
          folder.CreateEntry(new EntryId(id, SaveEntryExtension.Key)).FillFrom(binaryWriter3);
          BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter3);
        }
      }
      foreach (ObjectSaveData childStruct in this._childStructs)
        childStruct.SaveTo(folder, archiveContext);
    }

    internal int GetElementCount()
    {
      if (this._containerType == ContainerType.List)
        return ((ICollection) this.Target).Count;
      if (this._containerType == ContainerType.Queue)
        return ((ICollection) this.Target).Count;
      if (this._containerType == ContainerType.Dictionary)
        return ((ICollection) this.Target).Count;
      return this._containerType == ContainerType.Array ? ((Array) this.Target).GetLength(0) : 0;
    }

    public void CollectStrings()
    {
      foreach (string text in this.GetChildString())
        this.Context.AddOrGetStringId(text);
    }

    public void CollectStringsInto(List<string> collection)
    {
      foreach (string str in this.GetChildString())
        collection.Add(str);
    }

    public void CollectStructs()
    {
      foreach (ElementSaveData childElementSaveData in this.GetChildElementSaveDatas())
      {
        if (childElementSaveData.MemberType == SavedMemberType.CustomStruct)
        {
          object elementValue = childElementSaveData.ElementValue;
          this._childStructs.Add(new ObjectSaveData(this.Context, childElementSaveData.ElementIndex, elementValue, false));
        }
      }
      foreach (ObjectSaveData childStruct in this._childStructs)
        childStruct.CollectStructs();
    }

    public void CollectMembers()
    {
      foreach (ObjectSaveData childStruct in this._childStructs)
        childStruct.CollectMembers();
    }

    public IEnumerable<ElementSaveData> GetChildElementSaveDatas()
    {
      for (int i = 0; i < this._elementCount; ++i)
      {
        ElementSaveData key = this._keys[i];
        ElementSaveData value = this._values[i];
        if (key != null)
          yield return key;
        if (value != null)
          yield return value;
        value = (ElementSaveData) null;
      }
    }

    public IEnumerable<object> GetChildElements() => ContainerSaveData.GetChildElements(this._containerType, this.Target);

    public static IEnumerable<object> GetChildElements(
      ContainerType containerType,
      object target)
    {
      int i;
      switch (containerType)
      {
        case ContainerType.List:
          IList list = (IList) target;
          for (i = 0; i < list.Count; ++i)
          {
            object obj = list[i];
            if (obj != null)
              yield return obj;
          }
          list = (IList) null;
          break;
        case ContainerType.Dictionary:
          IDictionary dictionary = (IDictionary) target;
          foreach (object key1 in (IEnumerable) dictionary.Keys)
          {
            object key = key1;
            yield return key;
            object obj = dictionary[key];
            if (obj != null)
              yield return obj;
            key = (object) null;
          }
          dictionary = (IDictionary) null;
          break;
        case ContainerType.Array:
          Array array = (Array) target;
          for (i = 0; i < array.Length; ++i)
          {
            object obj = array.GetValue(i);
            if (obj != null)
              yield return obj;
          }
          array = (Array) null;
          break;
        case ContainerType.Queue:
          foreach (object obj in (IEnumerable) target)
          {
            if (obj != null)
              yield return obj;
          }
          break;
      }
    }

    public IEnumerable<object> GetChildObjects(ISaveContext context)
    {
      List<object> collectedObjects = new List<object>();
      ContainerSaveData.GetChildObjects(context, this._typeDefinition, this._containerType, this.Target, collectedObjects);
      return (IEnumerable<object>) collectedObjects;
    }

    public static void GetChildObjects(
      ISaveContext context,
      ContainerDefinition containerDefinition,
      ContainerType containerType,
      object target,
      List<object> collectedObjects)
    {
      if (containerDefinition.CollectObjectsMethod != null)
      {
        if (containerDefinition.HasNoChildObject)
          return;
        containerDefinition.CollectObjectsMethod(target, collectedObjects);
      }
      else
      {
        switch (containerType)
        {
          case ContainerType.List:
            IList list = (IList) target;
            for (int index = 0; index < list.Count; ++index)
            {
              object childElement = list[index];
              if (childElement != null)
                ContainerSaveData.ProcessChildObjectElement(childElement, context, collectedObjects);
            }
            break;
          case ContainerType.Dictionary:
            IDictionary dictionary = (IDictionary) target;
            foreach (object key in (IEnumerable) dictionary.Keys)
              ContainerSaveData.ProcessChildObjectElement(key, context, collectedObjects);
            IEnumerator enumerator1 = dictionary.Values.GetEnumerator();
            try
            {
              while (enumerator1.MoveNext())
              {
                object current = enumerator1.Current;
                if (current != null)
                  ContainerSaveData.ProcessChildObjectElement(current, context, collectedObjects);
              }
              break;
            }
            finally
            {
              if (enumerator1 is IDisposable disposable8)
                disposable8.Dispose();
            }
          case ContainerType.Array:
            Array array = (Array) target;
            for (int index = 0; index < array.Length; ++index)
            {
              object childElement = array.GetValue(index);
              if (childElement != null)
                ContainerSaveData.ProcessChildObjectElement(childElement, context, collectedObjects);
            }
            break;
          case ContainerType.Queue:
            IEnumerator enumerator2 = ((IEnumerable) target).GetEnumerator();
            try
            {
              while (enumerator2.MoveNext())
              {
                object current = enumerator2.Current;
                if (current != null)
                  ContainerSaveData.ProcessChildObjectElement(current, context, collectedObjects);
              }
              break;
            }
            finally
            {
              if (enumerator2 is IDisposable disposable9)
                disposable9.Dispose();
            }
          default:
            using (IEnumerator<object> enumerator3 = ContainerSaveData.GetChildElements(containerType, target).GetEnumerator())
            {
              while (enumerator3.MoveNext())
                ContainerSaveData.ProcessChildObjectElement(enumerator3.Current, context, collectedObjects);
              break;
            }
        }
      }
    }

    private static void ProcessChildObjectElement(
      object childElement,
      ISaveContext context,
      List<object> collectedObjects)
    {
      Type type = childElement.GetType();
      bool isClass = type.IsClass;
      if (isClass && type != typeof (string))
      {
        collectedObjects.Add(childElement);
      }
      else
      {
        if (isClass)
          return;
        TypeDefinition structDefinition = context.DefinitionContext.GetStructDefinition(type);
        if (structDefinition == null)
          return;
        if (structDefinition.CollectObjectsMethod != null)
        {
          structDefinition.CollectObjectsMethod(childElement, collectedObjects);
        }
        else
        {
          for (int index = 0; index < structDefinition.MemberDefinitions.Count; ++index)
          {
            MemberDefinition memberDefinition = structDefinition.MemberDefinitions[index];
            ObjectSaveData.GetChildObjectFrom(context, childElement, memberDefinition, collectedObjects);
          }
        }
      }
    }

    private IEnumerable<object> GetChildString()
    {
      foreach (object childElement in this.GetChildElements())
      {
        if (childElement.GetType() == typeof (string))
          yield return childElement;
      }
    }
  }
}
