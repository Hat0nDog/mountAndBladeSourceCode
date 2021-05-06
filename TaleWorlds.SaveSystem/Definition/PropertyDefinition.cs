// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.PropertyDefinition
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
  public class PropertyDefinition : MemberDefinition
  {
    public PropertyInfo PropertyInfo { get; private set; }

    public SaveablePropertyAttribute SaveablePropertyAttribute { get; private set; }

    public MethodInfo GetMethod { get; private set; }

    public MethodInfo SetMethod { get; private set; }

    public GetPropertyValueDelegate GetPropertyValueMethod { get; private set; }

    public PropertyDefinition(PropertyInfo propertyInfo, MemberTypeId id)
      : base((MemberInfo) propertyInfo, id)
    {
      this.PropertyInfo = propertyInfo;
      this.SaveablePropertyAttribute = propertyInfo.GetCustomAttribute<SaveablePropertyAttribute>();
      this.SetMethod = this.PropertyInfo.GetSetMethod(true);
      if (this.SetMethod == (MethodInfo) null && this.PropertyInfo.DeclaringType != (Type) null)
      {
        PropertyInfo property = this.PropertyInfo.DeclaringType.GetProperty(this.PropertyInfo.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property != (PropertyInfo) null)
          this.SetMethod = property.GetSetMethod(true);
      }
      if (this.SetMethod == (MethodInfo) null)
        throw new Exception("Property " + this.PropertyInfo.Name + " at Type " + this.PropertyInfo.DeclaringType.FullName + " does not have setter method.");
      this.GetMethod = this.PropertyInfo.GetGetMethod(true);
      if (this.GetMethod == (MethodInfo) null && this.PropertyInfo.DeclaringType != (Type) null)
      {
        PropertyInfo property = this.PropertyInfo.DeclaringType.GetProperty(this.PropertyInfo.Name);
        if (property != (PropertyInfo) null)
          this.GetMethod = property.GetGetMethod(true);
      }
      if (this.GetMethod == (MethodInfo) null)
        throw new Exception("Property " + this.PropertyInfo.Name + " at Type " + this.PropertyInfo.DeclaringType.FullName + " does not have getter method.");
    }

    public override Type GetMemberType() => this.PropertyInfo.PropertyType;

    public override object GetValue(object target) => this.GetPropertyValueMethod == null ? this.GetMethod.Invoke(target, new object[0]) : this.GetPropertyValueMethod(target);

    public void InitializeForAutoGeneration(GetPropertyValueDelegate getPropertyValueMethod) => this.GetPropertyValueMethod = getPropertyValueMethod;
  }
}
