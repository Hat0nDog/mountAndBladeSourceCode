// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.SaveableTypeDefiner
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.SaveSystem.Definition;

namespace TaleWorlds.SaveSystem
{
  public abstract class SaveableTypeDefiner
  {
    private DefinitionContext _definitionContext;
    private readonly int _saveBaseId;

    protected SaveableTypeDefiner(int saveBaseId) => this._saveBaseId = saveBaseId;

    internal void Initialize(DefinitionContext definitionContext) => this._definitionContext = definitionContext;

    protected internal virtual void DefineBasicTypes()
    {
    }

    protected internal virtual void DefineClassTypes()
    {
    }

    protected internal virtual void DefineStructTypes()
    {
    }

    protected internal virtual void DefineInterfaceTypes()
    {
    }

    protected internal virtual void DefineEnumTypes()
    {
    }

    protected internal virtual void DefineRootClassTypes()
    {
    }

    protected internal virtual void DefineGenericClassDefinitions()
    {
    }

    protected internal virtual void DefineGenericStructDefinitions()
    {
    }

    protected internal virtual void DefineContainerDefinitions()
    {
    }

    protected void ConstructGenericClassDefinition(Type type) => this._definitionContext.ConstructGenericClassDefinition(type);

    protected void ConstructGenericStructDefinition(Type type) => this._definitionContext.ConstructGenericStructDefinition(type);

    protected void AddBasicTypeDefinition(Type type, int saveId, IBasicTypeSerializer serializer) => this._definitionContext.AddBasicTypeDefinition(new BasicTypeDefinition(type, this._saveBaseId + saveId, serializer));

    protected void AddClassDefinition(Type type, int saveId) => this._definitionContext.AddClassDefinition(new TypeDefinition(type, this._saveBaseId + saveId, (IObjectResolver) new DefaultObjectResolver()));

    protected void AddClassDefinition(Type type, int saveId, IObjectResolver resolver) => this._definitionContext.AddClassDefinition(new TypeDefinition(type, this._saveBaseId + saveId, resolver));

    protected void AddClassDefinitionWithCustomFields(
      Type type,
      int saveId,
      IEnumerable<Tuple<string, short>> fields)
    {
      TypeDefinition classDefinition = new TypeDefinition(type, this._saveBaseId + saveId, (IObjectResolver) new DefaultObjectResolver());
      this._definitionContext.AddClassDefinition(classDefinition);
      foreach (Tuple<string, short> field in fields)
        classDefinition.AddCustomField(field.Item1, field.Item2);
    }

    protected void AddStructDefinitionWithCustomFields(
      Type type,
      int saveId,
      IEnumerable<Tuple<string, short>> fields)
    {
      TypeDefinition structDefinition = new TypeDefinition(type, this._saveBaseId + saveId, (IObjectResolver) new DefaultObjectResolver());
      this._definitionContext.AddStructDefinition(structDefinition);
      foreach (Tuple<string, short> field in fields)
        structDefinition.AddCustomField(field.Item1, field.Item2);
    }

    protected void AddRootClassDefinition(Type type, int saveId) => this._definitionContext.AddRootClassDefinition(new TypeDefinition(type, this._saveBaseId + saveId, (IObjectResolver) new DefaultObjectResolver()));

    protected void AddStructDefinition(Type type, int saveId) => this._definitionContext.AddStructDefinition(new TypeDefinition(type, this._saveBaseId + saveId, (IObjectResolver) new DefaultObjectResolver()));

    protected void AddInterfaceDefinition(Type type, int saveId) => this._definitionContext.AddInterfaceDefinition(new InterfaceDefinition(type, this._saveBaseId + saveId));

    protected void AddEnumDefinition(Type type, int saveId) => this._definitionContext.AddEnumDefinition(new EnumDefinition(type, this._saveBaseId + saveId));

    protected void ConstructContainerDefinition(Type type)
    {
      Assembly assembly = this.GetType().Assembly;
      this._definitionContext.ConstructContainerDefinition(type, assembly);
    }
  }
}
