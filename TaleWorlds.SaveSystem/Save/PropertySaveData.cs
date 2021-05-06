// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Save.PropertySaveData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
  internal class PropertySaveData : MemberSaveData
  {
    public PropertyDefinition PropertyDefinition { get; private set; }

    public MemberTypeId SaveId { get; private set; }

    public PropertySaveData(
      ObjectSaveData objectSaveData,
      PropertyDefinition propertyDefinition,
      MemberTypeId saveId)
      : base(objectSaveData)
    {
      this.PropertyDefinition = propertyDefinition;
      this.SaveId = saveId;
    }

    public override void Initialize(TypeDefinitionBase typeDefinition)
    {
      object data = this.PropertyDefinition.GetValue(this.ObjectSaveData.Target);
      this.InitializeData(this.SaveId, this.PropertyDefinition.PropertyInfo.PropertyType, typeDefinition, data);
    }

    public override void InitializeAsCustomStruct(int structId) => this.InitializeDataAsCustomStruct(this.SaveId, structId);
  }
}
