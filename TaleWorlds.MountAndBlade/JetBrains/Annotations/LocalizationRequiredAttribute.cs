// Decompiled with JetBrains decompiler
// Type: JetBrains.Annotations.LocalizationRequiredAttribute
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace JetBrains.Annotations
{
  [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
  public sealed class LocalizationRequiredAttribute : Attribute
  {
    public LocalizationRequiredAttribute(bool required) => this.Required = required;

    [UsedImplicitly]
    public bool Required { get; set; }

    public override bool Equals(object obj) => obj is LocalizationRequiredAttribute requiredAttribute && requiredAttribute.Required == this.Required;

    public override int GetHashCode() => base.GetHashCode();
  }
}
