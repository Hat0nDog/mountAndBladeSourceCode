// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.VariableLoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
  internal abstract class VariableLoadData
  {
    private IReader _reader;
    private TypeDefinitionBase _typeDefinition;
    private SaveId _saveId;
    private object _customStructObject;

    public LoadContext Context { get; private set; }

    public MemberTypeId MemberSaveId { get; private set; }

    public SavedMemberType SavedMemberType { get; private set; }

    public object Data { get; private set; }

    protected VariableLoadData(LoadContext context, IReader reader)
    {
      this.Context = context;
      this._reader = reader;
    }

    public void Read()
    {
      this.SavedMemberType = (SavedMemberType) this._reader.ReadByte();
      this.MemberSaveId = new MemberTypeId()
      {
        TypeLevel = this._reader.ReadByte(),
        LocalSaveId = this._reader.ReadShort()
      };
      if (this.SavedMemberType == SavedMemberType.Object)
        this.Data = (object) this._reader.ReadInt();
      else if (this.SavedMemberType == SavedMemberType.Container)
        this.Data = (object) this._reader.ReadInt();
      else if (this.SavedMemberType == SavedMemberType.String)
        this.Data = (object) this._reader.ReadInt();
      else if (this.SavedMemberType == SavedMemberType.Enum)
      {
        this._saveId = SaveId.ReadSaveIdFrom(this._reader);
        this._typeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this._saveId);
        this.Data = (object) this._reader.ReadString();
      }
      else if (this.SavedMemberType == SavedMemberType.BasicType)
      {
        this._saveId = SaveId.ReadSaveIdFrom(this._reader);
        this._typeDefinition = this.Context.DefinitionContext.TryGetTypeDefinition(this._saveId);
        this.Data = ((BasicTypeDefinition) this._typeDefinition).Serializer.Deserialize(this._reader);
      }
      else
      {
        if (this.SavedMemberType != SavedMemberType.CustomStruct)
          return;
        this.Data = (object) this._reader.ReadInt();
      }
    }

    public void SetCustomStructData(object customStructObject) => this._customStructObject = customStructObject;

    public object GetDataToUse()
    {
      object obj = (object) null;
      if (this.SavedMemberType == SavedMemberType.Object)
      {
        ObjectHeaderLoadData objectWithId = this.Context.GetObjectWithId((int) this.Data);
        if (objectWithId != null)
          obj = objectWithId.Target;
      }
      else if (this.SavedMemberType == SavedMemberType.Container)
      {
        ContainerHeaderLoadData containerWithId = this.Context.GetContainerWithId((int) this.Data);
        if (containerWithId != null)
          obj = containerWithId.Target;
      }
      else if (this.SavedMemberType == SavedMemberType.String)
        obj = (object) this.Context.GetStringWithId((int) this.Data);
      else if (this.SavedMemberType == SavedMemberType.Enum)
      {
        if (this._typeDefinition == null)
        {
          obj = (object) (string) this.Data;
        }
        else
        {
          Type type = this._typeDefinition.Type;
          if (Enum.IsDefined(type, this.Data))
            obj = Enum.Parse(type, (string) this.Data);
        }
      }
      else if (this.SavedMemberType == SavedMemberType.BasicType)
        obj = this.Data;
      else if (this.SavedMemberType == SavedMemberType.CustomStruct)
        obj = this._customStructObject;
      return obj;
    }
  }
}
