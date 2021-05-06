// Decompiled with JetBrains decompiler
// Type: TaleWorlds.ObjectSystem.MbObjectXmlInformation
// Assembly: TaleWorlds.ObjectSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 525679E6-68C3-48B8-A030-465E146E69EE
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.ObjectSystem.dll

using System.Collections.Generic;

namespace TaleWorlds.ObjectSystem
{
  public struct MbObjectXmlInformation
  {
    public string Id;
    public string Name;
    public string ModuleName;
    public List<string> GameTypesIncluded;

    public MbObjectXmlInformation(
      string id,
      string name,
      string moduleName,
      List<string> gameTypesIncluded)
    {
      this.Id = id;
      this.Name = name;
      this.ModuleName = moduleName;
      this.GameTypesIncluded = gameTypesIncluded;
    }
  }
}
