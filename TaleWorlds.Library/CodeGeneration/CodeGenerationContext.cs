// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.CodeGeneration.CodeGenerationContext
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
  public class CodeGenerationContext
  {
    public List<NamespaceCode> Namespaces { get; private set; }

    public CodeGenerationContext() => this.Namespaces = new List<NamespaceCode>();

    public NamespaceCode FindOrCreateNamespace(string name)
    {
      foreach (NamespaceCode namespaceCode in this.Namespaces)
      {
        if (namespaceCode.Name == name)
          return namespaceCode;
      }
      NamespaceCode namespaceCode1 = new NamespaceCode();
      namespaceCode1.Name = name;
      this.Namespaces.Add(namespaceCode1);
      return namespaceCode1;
    }

    public void GenerateInto(CodeGenerationFile codeGenerationFile)
    {
      foreach (NamespaceCode namespaceCode in this.Namespaces)
      {
        namespaceCode.GenerateInto(codeGenerationFile);
        codeGenerationFile.AddLine("");
      }
    }
  }
}
