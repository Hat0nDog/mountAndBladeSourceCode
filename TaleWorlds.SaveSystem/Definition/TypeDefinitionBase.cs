// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.TypeDefinitionBase
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;

namespace TaleWorlds.SaveSystem.Definition
{
  public class TypeDefinitionBase
  {
    public SaveId SaveId { get; private set; }

    public Type Type { get; private set; }

    public byte TypeLevel { get; private set; }

    protected TypeDefinitionBase(Type type, SaveId saveId)
    {
      this.Type = type;
      this.SaveId = saveId;
      this.TypeLevel = TypeDefinitionBase.GetClassLevel(type);
    }

    public static byte GetClassLevel(Type type)
    {
      byte num = 1;
      if (type.IsClass)
      {
        for (Type type1 = type; type1 != typeof (object); type1 = type1.BaseType)
          ++num;
      }
      return num;
    }
  }
}
