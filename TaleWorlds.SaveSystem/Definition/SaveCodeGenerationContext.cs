// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.SaveCodeGenerationContext
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TaleWorlds.SaveSystem.Definition
{
  public class SaveCodeGenerationContext
  {
    private Dictionary<Assembly, SaveCodeGenerationContextAssembly> _assemblies;
    private DefinitionContext _definitionContext;

    public SaveCodeGenerationContext(DefinitionContext definitionContext)
    {
      this._definitionContext = definitionContext;
      this._assemblies = new Dictionary<Assembly, SaveCodeGenerationContextAssembly>();
    }

    public void AddAssembly(
      Assembly assembly,
      string defaultNamespace,
      string location,
      string fileName)
    {
      SaveCodeGenerationContextAssembly generationContextAssembly = new SaveCodeGenerationContextAssembly(this._definitionContext, assembly, defaultNamespace, location, fileName);
      this._assemblies.Add(assembly, generationContextAssembly);
    }

    internal SaveCodeGenerationContextAssembly FindAssemblyInformation(
      Assembly assembly)
    {
      SaveCodeGenerationContextAssembly generationContextAssembly;
      this._assemblies.TryGetValue(assembly, out generationContextAssembly);
      return generationContextAssembly;
    }

    internal void FillFiles()
    {
      foreach (SaveCodeGenerationContextAssembly generationContextAssembly in this._assemblies.Values)
      {
        generationContextAssembly.Generate();
        string text = generationContextAssembly.GenerateText();
        File.WriteAllText(generationContextAssembly.Location + generationContextAssembly.FileName, text);
      }
    }
  }
}
