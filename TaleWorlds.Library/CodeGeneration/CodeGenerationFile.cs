// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.CodeGeneration.CodeGenerationFile
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Collections.Generic;
using System.Text;

namespace TaleWorlds.Library.CodeGeneration
{
  public class CodeGenerationFile
  {
    private List<string> _lines;

    public CodeGenerationFile() => this._lines = new List<string>();

    public void AddLine(string line) => this._lines.Add(line);

    public string GenerateText()
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (string line in this._lines)
      {
        if (line == "}")
          --num;
        string str1 = "";
        for (int index = 0; index < num; ++index)
          str1 += "\t";
        string str2 = str1 + line + "\n";
        if (line == "{")
          ++num;
        stringBuilder.Append(str2);
      }
      return stringBuilder.ToString();
    }
  }
}
