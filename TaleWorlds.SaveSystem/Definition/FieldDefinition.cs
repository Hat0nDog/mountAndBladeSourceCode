// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.FieldDefinition
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
  public class FieldDefinition : MemberDefinition
  {
    public FieldInfo FieldInfo { get; private set; }

    public SaveableFieldAttribute SaveableFieldAttribute { get; private set; }

    public GetFieldValueDelegate GetFieldValueMethod { get; private set; }

    public FieldDefinition(FieldInfo fieldInfo, MemberTypeId id)
      : base((MemberInfo) fieldInfo, id)
    {
      this.FieldInfo = fieldInfo;
      this.SaveableFieldAttribute = fieldInfo.GetCustomAttribute<SaveableFieldAttribute>();
    }

    public override Type GetMemberType() => this.FieldInfo.FieldType;

    public override object GetValue(object target) => this.GetFieldValueMethod == null ? this.FieldInfo.GetValue(target) : this.GetFieldValueMethod(target);

    public void InitializeForAutoGeneration(GetFieldValueDelegate getFieldValueMethod) => this.GetFieldValueMethod = getFieldValueMethod;
  }
}
