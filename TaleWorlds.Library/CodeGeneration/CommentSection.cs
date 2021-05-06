// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.CodeGeneration.CommentSection
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.Collections.Generic;

namespace TaleWorlds.Library.CodeGeneration
{
  public class CommentSection
  {
    private List<string> _lines;

    public CommentSection() => this._lines = new List<string>();

    public void AddCommentLine(string line) => this._lines.Add(line);

    public void GenerateInto(CodeGenerationFile codeGenerationFile)
    {
      foreach (string line in this._lines)
        codeGenerationFile.AddLine("//" + line);
    }
  }
}
