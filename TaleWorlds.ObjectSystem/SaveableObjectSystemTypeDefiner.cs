// Decompiled with JetBrains decompiler
// Type: TaleWorlds.ObjectSystem.SaveableObjectSystemTypeDefiner
// Assembly: TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 525679E6-68C3-48B8-A030-465E146E69EE
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll

using System.Collections.Generic;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.ObjectSystem
{
  public class SaveableObjectSystemTypeDefiner : SaveableTypeDefiner
  {
    public SaveableObjectSystemTypeDefiner()
      : base(10000)
    {
    }

    protected override void DefineBasicTypes()
    {
      base.DefineBasicTypes();
      this.AddBasicTypeDefinition(typeof (MBGUID), 1005, (IBasicTypeSerializer) new MMOGUIDBasicTypeSerializer());
    }

    protected override void DefineClassTypes()
    {
      this.AddClassDefinition(typeof (MBObjectBase), 34);
      this.AddClassDefinition(typeof (ObsoleteObjectManager), 35);
      this.AddClassDefinition(typeof (ObsoleteObjectManager.ObjectTypeRecord<>), 48);
    }

    protected override void DefineStructTypes()
    {
    }

    protected override void DefineEnumTypes()
    {
    }

    protected override void DefineInterfaceTypes() => this.AddInterfaceDefinition(typeof (ObsoleteObjectManager.IObjectTypeRecord), 3001);

    protected override void DefineRootClassTypes()
    {
    }

    protected override void DefineGenericClassDefinitions()
    {
    }

    protected override void DefineGenericStructDefinitions()
    {
    }

    protected override void DefineContainerDefinitions() => this.ConstructContainerDefinition(typeof (List<ObsoleteObjectManager.IObjectTypeRecord>));
  }
}
