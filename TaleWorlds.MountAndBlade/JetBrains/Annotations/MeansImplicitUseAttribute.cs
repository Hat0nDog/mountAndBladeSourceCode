// Decompiled with JetBrains decompiler
// Type: JetBrains.Annotations.MeansImplicitUseAttribute
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;

namespace JetBrains.Annotations
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class MeansImplicitUseAttribute : Attribute
  {
    [UsedImplicitly]
    public MeansImplicitUseAttribute()
      : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
    {
    }

    [UsedImplicitly]
    public MeansImplicitUseAttribute(
      ImplicitUseKindFlags useKindFlags,
      ImplicitUseTargetFlags targetFlags)
    {
      this.UseKindFlags = useKindFlags;
      this.TargetFlags = targetFlags;
    }

    [UsedImplicitly]
    public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
      : this(useKindFlags, ImplicitUseTargetFlags.Default)
    {
    }

    [UsedImplicitly]
    public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
      : this(ImplicitUseKindFlags.Default, targetFlags)
    {
    }

    [UsedImplicitly]
    public ImplicitUseKindFlags UseKindFlags { get; private set; }

    [UsedImplicitly]
    public ImplicitUseTargetFlags TargetFlags { get; private set; }
  }
}
