// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.PropertyLoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Reflection;
using TaleWorlds.Library;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem.Load
{
  internal class PropertyLoadData : MemberLoadData
  {
    public PropertyLoadData(ObjectLoadData objectLoadData, IReader reader)
      : base(objectLoadData, reader)
    {
    }

    public void FillObject()
    {
      PropertyDefinition definitionWithId;
      if (this.ObjectLoadData.TypeDefinition == null || (definitionWithId = this.ObjectLoadData.TypeDefinition.GetPropertyDefinitionWithId(this.MemberSaveId)) == null)
        return;
      MethodInfo setMethod = definitionWithId.SetMethod;
      object target = this.ObjectLoadData.Target;
      object dataToUse = this.GetDataToUse();
      if (dataToUse != null && !definitionWithId.PropertyInfo.PropertyType.IsAssignableFrom(dataToUse.GetType()) && !LoadContext.TryConvertType(dataToUse.GetType(), definitionWithId.PropertyInfo.PropertyType, ref dataToUse))
        return;
      setMethod.Invoke(target, new object[1]{ dataToUse });
    }
  }
}
