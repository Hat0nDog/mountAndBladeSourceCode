// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.CodeGeneration.NamespaceCode
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
  public class NamespaceCode
  {
    public string Name { get; set; }

    public List<ClassCode> Classes { get; private set; }

    public NamespaceCode() => this.Classes = new List<ClassCode>();

    public void GenerateInto(CodeGenerationFile codeGenerationFile)
    {
      codeGenerationFile.AddLine("namespace " + this.Name);
      codeGenerationFile.AddLine("{");
      foreach (ClassCode classCode in this.Classes)
      {
        classCode.GenerateInto(codeGenerationFile);
        codeGenerationFile.AddLine("");
      }
      codeGenerationFile.AddLine("}");
    }

    public void AddClass(ClassCode clasCode) => this.Classes.Add(clasCode);
  }
}
