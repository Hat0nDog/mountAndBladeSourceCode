// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.CodeGeneration.VariableCode
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

namespace TaleWorlds.Library.CodeGeneration
{
  public class VariableCode
  {
    public string Name { get; set; }

    public string Type { get; set; }

    public bool IsStatic { get; set; }

    public VariableCodeAccessModifier AccessModifier { get; set; }

    public VariableCode()
    {
      this.Type = "System.Object";
      this.Name = "Unnamed variable";
      this.IsStatic = false;
      this.AccessModifier = VariableCodeAccessModifier.Private;
    }

    public string GenerateLine()
    {
      string str = "";
      if (this.AccessModifier == VariableCodeAccessModifier.Public)
        str += "public ";
      else if (this.AccessModifier == VariableCodeAccessModifier.Protected)
        str += "protected ";
      else if (this.AccessModifier == VariableCodeAccessModifier.Private)
        str += "private ";
      else if (this.AccessModifier == VariableCodeAccessModifier.Internal)
        str += "internal ";
      if (this.IsStatic)
        str += "static ";
      return str + this.Type + " " + this.Name + ";";
    }
  }
}
