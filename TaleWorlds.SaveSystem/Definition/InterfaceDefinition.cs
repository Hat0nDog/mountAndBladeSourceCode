// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.InterfaceDefinition
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class InterfaceDefinition : TypeDefinitionBase
  {
    public InterfaceDefinition(Type type, SaveId saveId)
      : base(type, saveId)
    {
    }

    public InterfaceDefinition(Type type, int saveId)
      : base(type, (SaveId) new TypeSaveId(saveId))
    {
    }
  }
}
