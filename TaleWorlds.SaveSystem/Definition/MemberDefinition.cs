// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.MemberDefinition
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
  public abstract class MemberDefinition
  {
    public MemberTypeId Id { get; private set; }

    public MemberInfo MemberInfo { get; private set; }

    protected MemberDefinition(MemberInfo memberInfo, MemberTypeId id)
    {
      this.MemberInfo = memberInfo;
      this.Id = id;
    }

    public abstract Type GetMemberType();

    public abstract object GetValue(object target);
  }
}
