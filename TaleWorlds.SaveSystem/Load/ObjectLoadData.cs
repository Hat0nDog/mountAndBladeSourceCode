// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.ObjectLoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
  public class ObjectLoadData
  {
    private short _propertyCount;
    private List<PropertyLoadData> _propertyValues;
    private List<FieldLoadData> _fieldValues;
    private List<MemberLoadData> _memberValues;
    private SaveId _saveId;
    private List<ObjectLoadData> _childStructs;
    private short _childStructCount;

    public int Id { get; private set; }

    public object Target { get; private set; }

    public LoadContext Context { get; private set; }

    public TypeDefinition TypeDefinition { get; private set; }

    public object GetDataBySaveId(int localSaveId) => this._memberValues.SingleOrDefault<MemberLoadData>((Func<MemberLoadData, bool>) (value => (int) value.MemberSaveId.LocalSaveId == localSaveId))?.GetDataToUse();

    public object GetDataValueBySaveId(int localSaveId) => this._memberValues.SingleOrDefault<MemberLoadData>((Func<MemberLoadData, bool>) (value => (int) value.MemberSaveId.LocalSaveId == localSaveId))?.GetDataToUse();

    public ObjectLoadData(LoadContext context, int id)
    {
      this.Context = context;
      this.Id = id;
      this._propertyValues = new List<PropertyLoadData>();
      this._fieldValues = new List<FieldLoadData>();
      this._memberValues = new List<MemberLoadData>();
      this._childStructs = new List<ObjectLoadData>();
    }

    public ObjectLoadData(ObjectHeaderLoadData headerLoadData)
    {
      this.Id = headerLoadData.Id;
      this.Target = headerLoadData.Target;
      this.Context = headerLoadData.Context;
      this.TypeDefinition = headerLoadData.TypeDefinition;
      this._propertyValues = new List<PropertyLoadData>();
      this._fieldValues = new List<FieldLoadData>();
      this._memberValues = new List<MemberLoadData>();
      this._childStructs = new List<ObjectLoadData>();
    }

    public void InitializeReaders(SaveEntryFolder saveEntryFolder)
    {
      BinaryReader binaryReader = saveEntryFolder.GetEntry(new EntryId(-1, SaveEntryExtension.Basics)).GetBinaryReader();
      this._saveId = SaveId.ReadSaveIdFrom((IReader) binaryReader);
      this._propertyCount = binaryReader.ReadShort();
      this._childStructCount = binaryReader.ReadShort();
      for (int id = 0; id < (int) this._childStructCount; ++id)
        this._childStructs.Add(new ObjectLoadData(this.Context, id));
      foreach (SaveEntry childEntry in saveEntryFolder.ChildEntries)
      {
        EntryId id = childEntry.Id;
        if (id.Extension == SaveEntryExtension.Property)
        {
          PropertyLoadData propertyLoadData = new PropertyLoadData(this, (IReader) childEntry.GetBinaryReader());
          this._propertyValues.Add(propertyLoadData);
          this._memberValues.Add((MemberLoadData) propertyLoadData);
        }
        else
        {
          id = childEntry.Id;
          if (id.Extension == SaveEntryExtension.Field)
          {
            FieldLoadData fieldLoadData = new FieldLoadData(this, (IReader) childEntry.GetBinaryReader());
            this._fieldValues.Add(fieldLoadData);
            this._memberValues.Add((MemberLoadData) fieldLoadData);
          }
        }
      }
      for (int index = 0; index < (int) this._childStructCount; ++index)
        this._childStructs[index].InitializeReaders(saveEntryFolder.GetChildFolder(new FolderId(index, SaveFolderExtension.Struct)));
    }

    public void CreateStruct()
    {
      this.TypeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this._saveId) as TypeDefinition;
      if (this.TypeDefinition != null)
        this.Target = FormatterServices.GetUninitializedObject(this.TypeDefinition.Type);
      foreach (ObjectLoadData childStruct in this._childStructs)
        childStruct.CreateStruct();
    }

    public void FillCreatedObject()
    {
      foreach (ObjectLoadData childStruct in this._childStructs)
        childStruct.CreateStruct();
    }

    public void Read()
    {
      foreach (ObjectLoadData childStruct in this._childStructs)
        childStruct.Read();
      foreach (MemberLoadData memberValue in this._memberValues)
      {
        memberValue.Read();
        if (memberValue.SavedMemberType == SavedMemberType.CustomStruct)
        {
          object target = this._childStructs[(int) memberValue.Data].Target;
          memberValue.SetCustomStructData(target);
        }
      }
    }

    public void FillObject()
    {
      foreach (ObjectLoadData childStruct in this._childStructs)
        childStruct.FillObject();
      foreach (FieldLoadData fieldValue in this._fieldValues)
        fieldValue.FillObject();
      foreach (PropertyLoadData propertyValue in this._propertyValues)
        propertyValue.FillObject();
    }
  }
}
