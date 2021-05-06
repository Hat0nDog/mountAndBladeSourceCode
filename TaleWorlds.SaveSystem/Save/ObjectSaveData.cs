// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Save.ObjectSaveData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
  internal class ObjectSaveData
  {
    private Dictionary<PropertyInfo, PropertySaveData> _propertyValues;
    private Dictionary<FieldInfo, FieldSaveData> _fieldValues;
    private List<MemberSaveData> _stringMembers;
    private TypeDefinition _typeDefinition;
    private Dictionary<MemberDefinition, ObjectSaveData> _childStructs;

    public int ObjectId { get; private set; }

    public ISaveContext Context { get; private set; }

    public object Target { get; private set; }

    public Type Type { get; private set; }

    public bool IsClass { get; private set; }

    internal int PropertyCount => this._propertyValues.Count;

    internal int FieldCount => this._fieldValues.Count;

    public ObjectSaveData(ISaveContext context, int objectId, object target, bool isClass)
    {
      this.ObjectId = objectId;
      this.Context = context;
      this.Target = target;
      this.IsClass = isClass;
      this._stringMembers = new List<MemberSaveData>();
      this.Type = target.GetType();
      this._typeDefinition = !this.IsClass ? context.DefinitionContext.GetStructDefinition(this.Type) : context.DefinitionContext.GetClassDefinition(this.Type);
      this._childStructs = new Dictionary<MemberDefinition, ObjectSaveData>(3);
      this._propertyValues = new Dictionary<PropertyInfo, PropertySaveData>(this._typeDefinition.PropertyDefinitions.Count);
      this._fieldValues = new Dictionary<FieldInfo, FieldSaveData>(this._typeDefinition.FieldDefinitions.Count);
      if (this._typeDefinition == null)
        throw new Exception("Could not find type definition of type: " + (object) this.Type);
    }

    public void CollectMembers()
    {
      for (int index = 0; index < this._typeDefinition.MemberDefinitions.Count; ++index)
      {
        MemberDefinition memberDefinition = this._typeDefinition.MemberDefinitions[index];
        MemberSaveData memberSaveData = (MemberSaveData) null;
        switch (memberDefinition)
        {
          case PropertyDefinition _:
            PropertyDefinition propertyDefinition = (PropertyDefinition) memberDefinition;
            PropertyInfo propertyInfo = propertyDefinition.PropertyInfo;
            MemberTypeId id1 = propertyDefinition.Id;
            PropertySaveData propertySaveData = new PropertySaveData(this, propertyDefinition, id1);
            this._propertyValues.Add(propertyInfo, propertySaveData);
            memberSaveData = (MemberSaveData) propertySaveData;
            break;
          case FieldDefinition _:
            FieldDefinition fieldDefinition = (FieldDefinition) memberDefinition;
            FieldInfo fieldInfo = fieldDefinition.FieldInfo;
            MemberTypeId id2 = fieldDefinition.Id;
            FieldSaveData fieldSaveData = new FieldSaveData(this, fieldDefinition, id2);
            this._fieldValues.Add(fieldInfo, fieldSaveData);
            memberSaveData = (MemberSaveData) fieldSaveData;
            break;
        }
        TypeDefinitionBase typeDefinition1 = this.Context.DefinitionContext.GetTypeDefinition(memberDefinition.GetMemberType());
        if (typeDefinition1 is TypeDefinition typeDefinition3 && !typeDefinition3.IsClassDefinition)
        {
          ObjectSaveData childStruct = this._childStructs[memberDefinition];
          memberSaveData.InitializeAsCustomStruct(childStruct.ObjectId);
        }
        else
          memberSaveData.Initialize(typeDefinition1);
        if (memberSaveData.MemberType == SavedMemberType.String)
          this._stringMembers.Add(memberSaveData);
      }
      foreach (ObjectSaveData objectSaveData in this._childStructs.Values)
        objectSaveData.CollectMembers();
    }

    public void CollectStringsInto(List<string> collection)
    {
      for (int index = 0; index < this._stringMembers.Count; ++index)
      {
        string str = (string) this._stringMembers[index].Value;
        collection.Add(str);
      }
      foreach (ObjectSaveData objectSaveData in this._childStructs.Values)
        objectSaveData.CollectStringsInto(collection);
    }

    public void CollectStrings()
    {
      for (int index = 0; index < this._stringMembers.Count; ++index)
        this.Context.AddOrGetStringId((string) this._stringMembers[index].Value);
      foreach (ObjectSaveData objectSaveData in this._childStructs.Values)
        objectSaveData.CollectStrings();
    }

    public void CollectStructs()
    {
      int objectId = 0;
      for (int index = 0; index < this._typeDefinition.MemberDefinitions.Count; ++index)
      {
        MemberDefinition memberDefinition = this._typeDefinition.MemberDefinitions[index];
        if (this.Context.DefinitionContext.GetStructDefinition(memberDefinition.GetMemberType()) != null)
        {
          object target = memberDefinition.GetValue(this.Target);
          ObjectSaveData objectSaveData = new ObjectSaveData(this.Context, objectId, target, false);
          this._childStructs.Add(memberDefinition, objectSaveData);
          ++objectId;
        }
      }
      foreach (ObjectSaveData objectSaveData in this._childStructs.Values)
        objectSaveData.CollectStructs();
    }

    public void SaveHeaderTo(SaveEntryFolder parentFolder, IArchiveContext archiveContext)
    {
      SaveFolderExtension extension = this.IsClass ? SaveFolderExtension.Object : SaveFolderExtension.Struct;
      SaveEntryFolder folder = archiveContext.CreateFolder(parentFolder, new FolderId(this.ObjectId, extension), 1);
      BinaryWriter binaryWriter = BinaryWriterFactory.GetBinaryWriter();
      this._typeDefinition.SaveId.WriteTo((IWriter) binaryWriter);
      binaryWriter.WriteShort((short) this._propertyValues.Count);
      binaryWriter.WriteShort((short) this._childStructs.Count);
      EntryId entryId = new EntryId(-1, SaveEntryExtension.Basics);
      folder.CreateEntry(entryId).FillFrom(binaryWriter);
      BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter);
    }

    public void SaveTo(SaveEntryFolder parentFolder, IArchiveContext archiveContext)
    {
      SaveFolderExtension extension = this.IsClass ? SaveFolderExtension.Object : SaveFolderExtension.Struct;
      int entryCount = 1 + this._fieldValues.Values.Count + this._propertyValues.Values.Count;
      SaveEntryFolder folder = archiveContext.CreateFolder(parentFolder, new FolderId(this.ObjectId, extension), entryCount);
      BinaryWriter binaryWriter1 = BinaryWriterFactory.GetBinaryWriter();
      this._typeDefinition.SaveId.WriteTo((IWriter) binaryWriter1);
      binaryWriter1.WriteShort((short) this._propertyValues.Count);
      binaryWriter1.WriteShort((short) this._childStructs.Count);
      folder.CreateEntry(new EntryId(-1, SaveEntryExtension.Basics)).FillFrom(binaryWriter1);
      BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter1);
      int id1 = 0;
      foreach (FieldSaveData fieldSaveData in this._fieldValues.Values)
      {
        BinaryWriter binaryWriter2 = BinaryWriterFactory.GetBinaryWriter();
        BinaryWriter binaryWriter3 = binaryWriter2;
        fieldSaveData.SaveTo((IWriter) binaryWriter3);
        folder.CreateEntry(new EntryId(id1, SaveEntryExtension.Field)).FillFrom(binaryWriter2);
        BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter2);
        ++id1;
      }
      int id2 = 0;
      foreach (PropertySaveData propertySaveData in this._propertyValues.Values)
      {
        BinaryWriter binaryWriter2 = BinaryWriterFactory.GetBinaryWriter();
        BinaryWriter binaryWriter3 = binaryWriter2;
        propertySaveData.SaveTo((IWriter) binaryWriter3);
        folder.CreateEntry(new EntryId(id2, SaveEntryExtension.Property)).FillFrom(binaryWriter2);
        BinaryWriterFactory.ReleaseBinaryWriter(binaryWriter2);
        ++id2;
      }
      foreach (ObjectSaveData objectSaveData in this._childStructs.Values)
        objectSaveData.SaveTo(folder, archiveContext);
    }

    internal static void GetChildObjectFrom(
      ISaveContext context,
      object target,
      MemberDefinition memberDefinition,
      List<object> collectedObjects)
    {
      Type memberType = memberDefinition.GetMemberType();
      if (memberType.IsClass || memberType.IsInterface)
      {
        if (!(memberType != typeof (string)))
          return;
        object obj = memberDefinition.GetValue(target);
        if (obj == null)
          return;
        collectedObjects.Add(obj);
      }
      else
      {
        TypeDefinition structDefinition = context.DefinitionContext.GetStructDefinition(memberType);
        if (structDefinition == null)
          return;
        object target1 = memberDefinition.GetValue(target);
        for (int index = 0; index < structDefinition.MemberDefinitions.Count; ++index)
        {
          MemberDefinition memberDefinition1 = structDefinition.MemberDefinitions[index];
          ObjectSaveData.GetChildObjectFrom(context, target1, memberDefinition1, collectedObjects);
        }
      }
    }

    public IEnumerable<object> GetChildObjects()
    {
      List<object> collectedObjects = new List<object>();
      ObjectSaveData.GetChildObjects(this.Context, this._typeDefinition, this.Target, collectedObjects);
      return (IEnumerable<object>) collectedObjects;
    }

    public static void GetChildObjects(
      ISaveContext context,
      TypeDefinition typeDefinition,
      object target,
      List<object> collectedObjects)
    {
      if (typeDefinition.CollectObjectsMethod != null)
      {
        typeDefinition.CollectObjectsMethod(target, collectedObjects);
      }
      else
      {
        for (int index = 0; index < typeDefinition.MemberDefinitions.Count; ++index)
        {
          MemberDefinition memberDefinition = typeDefinition.MemberDefinitions[index];
          ObjectSaveData.GetChildObjectFrom(context, target, memberDefinition, collectedObjects);
        }
      }
    }
  }
}
