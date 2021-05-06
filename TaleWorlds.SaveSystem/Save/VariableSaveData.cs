// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Save.VariableSaveData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
  internal abstract class VariableSaveData
  {
    private TypeDefinitionBase _typeDefinition;

    public ISaveContext Context { get; private set; }

    public SavedMemberType MemberType { get; private set; }

    public object Value { get; private set; }

    public MemberTypeId MemberSaveId { get; private set; }

    protected VariableSaveData(ISaveContext context) => this.Context = context;

    protected void InitializeDataAsNullObject(MemberTypeId memberSaveId)
    {
      this.MemberSaveId = memberSaveId;
      this.MemberType = SavedMemberType.Object;
      this.Value = (object) -1;
    }

    protected void InitializeDataAsCustomStruct(MemberTypeId memberSaveId, int structId)
    {
      this.MemberSaveId = memberSaveId;
      this.MemberType = SavedMemberType.CustomStruct;
      this.Value = (object) structId;
    }

    protected void InitializeData(
      MemberTypeId memberSaveId,
      Type memberType,
      TypeDefinitionBase definition,
      object data)
    {
      this.MemberSaveId = memberSaveId;
      this._typeDefinition = definition;
      TypeDefinition typeDefinition = this._typeDefinition as TypeDefinition;
      if (this._typeDefinition is ContainerDefinition)
      {
        int num = -1;
        if (data != null)
          num = this.Context.GetContainerId(data);
        this.MemberType = SavedMemberType.Container;
        this.Value = (object) num;
      }
      else if (typeof (string) == memberType)
      {
        this.MemberType = SavedMemberType.String;
        this.Value = data;
      }
      else if (typeDefinition != null && typeDefinition.IsClassDefinition || this._typeDefinition is InterfaceDefinition || this._typeDefinition == null && memberType.IsInterface)
      {
        int num = -1;
        if (data != null)
          num = this.Context.GetObjectId(data);
        this.MemberType = SavedMemberType.Object;
        this.Value = (object) num;
      }
      else if (this._typeDefinition is EnumDefinition)
      {
        this.MemberType = SavedMemberType.Enum;
        this.Value = data;
      }
      else if (this._typeDefinition is BasicTypeDefinition)
      {
        this.MemberType = SavedMemberType.BasicType;
        this.Value = data;
      }
      else
      {
        this.MemberType = SavedMemberType.CustomStruct;
        this.Value = data;
      }
    }

    public void SaveTo(IWriter writer)
    {
      writer.WriteByte((byte) this.MemberType);
      writer.WriteByte(this.MemberSaveId.TypeLevel);
      writer.WriteShort(this.MemberSaveId.LocalSaveId);
      if (this.MemberType == SavedMemberType.Object)
        writer.WriteInt((int) this.Value);
      else if (this.MemberType == SavedMemberType.Container)
        writer.WriteInt((int) this.Value);
      else if (this.MemberType == SavedMemberType.String)
      {
        int stringId = this.Context.GetStringId((string) this.Value);
        writer.WriteInt(stringId);
      }
      else if (this.MemberType == SavedMemberType.Enum)
      {
        this._typeDefinition.SaveId.WriteTo(writer);
        writer.WriteString(this.Value.ToString());
      }
      else if (this.MemberType == SavedMemberType.BasicType)
      {
        this._typeDefinition.SaveId.WriteTo(writer);
        ((BasicTypeDefinition) this._typeDefinition).Serializer.Serialize(writer, this.Value);
      }
      else
      {
        if (this.MemberType != SavedMemberType.CustomStruct)
          return;
        writer.WriteInt((int) this.Value);
      }
    }
  }
}
