// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.ContainerDefinition
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
  public class ContainerDefinition : TypeDefinitionBase
  {
    public Assembly DefinedAssembly { get; private set; }

    public CollectObjectsDelegate CollectObjectsMethod { get; private set; }

    public bool HasNoChildObject { get; private set; }

    public ContainerDefinition(Type type, ContainerSaveId saveId, Assembly definedAssembly)
      : base(type, (SaveId) saveId)
    {
      this.DefinedAssembly = definedAssembly;
    }

    public void InitializeForAutoGeneration(
      CollectObjectsDelegate collectObjectsDelegate,
      bool hasNoChildObject)
    {
      this.CollectObjectsMethod = collectObjectsDelegate;
      this.HasNoChildObject = hasNoChildObject;
    }
  }
}
