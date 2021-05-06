// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.FieldLoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
  internal class FieldLoadData : MemberLoadData
  {
    private FieldInfo _fieldInfo;

    public FieldLoadData(ObjectLoadData objectLoadData, IReader reader)
      : base(objectLoadData, reader)
    {
    }

    public void FillObject()
    {
      FieldDefinition definitionWithId;
      if (this.ObjectLoadData.TypeDefinition == null || (definitionWithId = this.ObjectLoadData.TypeDefinition.GetFieldDefinitionWithId(this.MemberSaveId)) == null)
        return;
      this._fieldInfo = definitionWithId.FieldInfo;
      object target = this.ObjectLoadData.Target;
      object dataToUse = this.GetDataToUse();
      if (dataToUse != null && !this._fieldInfo.FieldType.IsAssignableFrom(dataToUse.GetType()) && !LoadContext.TryConvertType(dataToUse.GetType(), this._fieldInfo.FieldType, ref dataToUse))
        return;
      this._fieldInfo.SetValue(target, dataToUse);
    }
  }
}
