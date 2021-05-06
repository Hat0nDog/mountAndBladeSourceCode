// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.CodeGeneration.ConstructorCode
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
  public class ConstructorCode
  {
    private List<string> _lines;

    public string Name { get; set; }

    public string MethodSignature { get; set; }

    public string BaseCall { get; set; }

    public bool IsStatic { get; set; }

    public MethodCodeAccessModifier AccessModifier { get; set; }

    public ConstructorCode()
    {
      this.Name = "UnassignedConstructorName";
      this.MethodSignature = "()";
      this.BaseCall = "";
      this._lines = new List<string>();
    }

    public void GenerateInto(CodeGenerationFile codeGenerationFile)
    {
      string str = "";
      if (this.AccessModifier == MethodCodeAccessModifier.Public)
        str += "public ";
      else if (this.AccessModifier == MethodCodeAccessModifier.Protected)
        str += "protected ";
      else if (this.AccessModifier == MethodCodeAccessModifier.Private)
        str += "private ";
      else if (this.AccessModifier == MethodCodeAccessModifier.Internal)
        str += "internal ";
      if (this.IsStatic)
        str += "static ";
      string line1 = str + this.Name + this.MethodSignature;
      if (!string.IsNullOrEmpty(this.BaseCall))
        line1 = line1 + " : base" + this.BaseCall;
      codeGenerationFile.AddLine(line1);
      codeGenerationFile.AddLine("{");
      foreach (string line2 in this._lines)
        codeGenerationFile.AddLine(line2);
      codeGenerationFile.AddLine("}");
    }

    public void AddLine(string line) => this._lines.Add(line);
  }
}
