// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Save.ElementSaveData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Save
{
  internal class ElementSaveData : VariableSaveData
  {
    public object ElementValue { get; private set; }

    public int ElementIndex { get; private set; }

    public ElementSaveData(ContainerSaveData containerSaveData, object value, int index)
      : base(containerSaveData.Context)
    {
      this.ElementValue = value;
      this.ElementIndex = index;
      if (value == null)
      {
        this.InitializeDataAsNullObject(MemberTypeId.Invalid);
      }
      else
      {
        TypeDefinitionBase typeDefinition1 = containerSaveData.Context.DefinitionContext.GetTypeDefinition(value.GetType());
        if (typeDefinition1 is TypeDefinition typeDefinition4 && !typeDefinition4.IsClassDefinition)
          this.InitializeDataAsCustomStruct(MemberTypeId.Invalid, index);
        else
          this.InitializeData(MemberTypeId.Invalid, value.GetType(), typeDefinition1, value);
      }
    }
  }
}
