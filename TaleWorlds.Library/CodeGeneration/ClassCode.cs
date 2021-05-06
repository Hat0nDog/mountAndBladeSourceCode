// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.CodeGeneration.ClassCode
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
  public class ClassCode
  {
    public string Name { get; set; }

    public bool IsGeneric { get; set; }

    public int GenericTypeCount { get; set; }

    public bool IsPartial { get; set; }

    public ClassCodeAccessModifier AccessModifier { get; set; }

    public bool IsClass { get; set; }

    public List<string> InheritedInterfaces { get; private set; }

    public List<ClassCode> NestedClasses { get; private set; }

    public List<MethodCode> Methods { get; private set; }

    public List<ConstructorCode> Constructors { get; private set; }

    public List<VariableCode> Variables { get; private set; }

    public CommentSection CommentSection { get; set; }

    public ClassCode()
    {
      this.IsClass = true;
      this.IsGeneric = false;
      this.GenericTypeCount = 0;
      this.InheritedInterfaces = new List<string>();
      this.NestedClasses = new List<ClassCode>();
      this.Methods = new List<MethodCode>();
      this.Constructors = new List<ConstructorCode>();
      this.Variables = new List<VariableCode>();
      this.AccessModifier = ClassCodeAccessModifier.DoNotMention;
      this.Name = "UnnamedClass";
      this.CommentSection = (CommentSection) null;
    }

    public void GenerateInto(CodeGenerationFile codeGenerationFile)
    {
      if (this.CommentSection != null)
        this.CommentSection.GenerateInto(codeGenerationFile);
      string str1 = "";
      if (this.AccessModifier == ClassCodeAccessModifier.Public)
        str1 += "public ";
      else if (this.AccessModifier == ClassCodeAccessModifier.Internal)
        str1 += "internal ";
      if (this.IsPartial)
        str1 += "partial ";
      string str2 = "class";
      if (!this.IsClass)
        str2 = "struct";
      string line1 = str1 + str2 + " " + this.Name;
      if (this.InheritedInterfaces.Count > 0)
      {
        line1 += " : ";
        for (int index = 0; index < this.InheritedInterfaces.Count; ++index)
        {
          string inheritedInterface = this.InheritedInterfaces[index];
          line1 = line1 + " " + inheritedInterface;
          if (index + 1 != this.InheritedInterfaces.Count)
            line1 += ", ";
        }
      }
      if (this.IsGeneric)
      {
        string str3 = line1 + "<";
        for (int index = 0; index < this.GenericTypeCount; ++index)
        {
          str3 = this.GenericTypeCount != 1 ? str3 + "T" + (object) index : str3 + "T";
          if (index + 1 != this.GenericTypeCount)
            str3 += ", ";
        }
        line1 = str3 + ">";
      }
      codeGenerationFile.AddLine(line1);
      codeGenerationFile.AddLine("{");
      foreach (ClassCode nestedClass in this.NestedClasses)
        nestedClass.GenerateInto(codeGenerationFile);
      foreach (VariableCode variable in this.Variables)
      {
        string line2 = variable.GenerateLine();
        codeGenerationFile.AddLine(line2);
      }
      if (this.Variables.Count > 0)
        codeGenerationFile.AddLine("");
      foreach (ConstructorCode constructor in this.Constructors)
      {
        constructor.GenerateInto(codeGenerationFile);
        codeGenerationFile.AddLine("");
      }
      foreach (MethodCode method in this.Methods)
      {
        method.GenerateInto(codeGenerationFile);
        codeGenerationFile.AddLine("");
      }
      codeGenerationFile.AddLine("}");
    }

    public void AddVariable(VariableCode variableCode) => this.Variables.Add(variableCode);

    public void AddNestedClass(ClassCode clasCode) => this.NestedClasses.Add(clasCode);

    public void AddMethod(MethodCode methodCode) => this.Methods.Add(methodCode);

    public void AddConsturctor(ConstructorCode constructorCode)
    {
      constructorCode.Name = this.Name;
      this.Constructors.Add(constructorCode);
    }

    public void AddInterface(string interfaceName) => this.InheritedInterfaces.Add(interfaceName);
  }
}
