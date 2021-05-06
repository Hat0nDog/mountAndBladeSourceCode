// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Save.FieldSaveData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
  internal class FieldSaveData : MemberSaveData
  {
    public FieldDefinition FieldDefinition { get; private set; }

    public MemberTypeId SaveId { get; private set; }

    public FieldSaveData(
      ObjectSaveData objectSaveData,
      FieldDefinition fieldDefinition,
      MemberTypeId saveId)
      : base(objectSaveData)
    {
      this.FieldDefinition = fieldDefinition;
      this.SaveId = saveId;
    }

    public override void Initialize(TypeDefinitionBase typeDefinition)
    {
      object data = this.FieldDefinition.GetValue(this.ObjectSaveData.Target);
      this.InitializeData(this.SaveId, this.FieldDefinition.FieldInfo.FieldType, typeDefinition, data);
    }

    public override void InitializeAsCustomStruct(int structId) => this.InitializeDataAsCustomStruct(this.SaveId, structId);
  }
}
