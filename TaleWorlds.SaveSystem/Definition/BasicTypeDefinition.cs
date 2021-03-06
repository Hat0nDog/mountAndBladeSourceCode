// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Definition.BasicTypeDefinition
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;

namespace TaleWorlds.SaveSystem.Definition
{
  internal class BasicTypeDefinition : TypeDefinitionBase
  {
    public IBasicTypeSerializer Serializer { get; private set; }

    public BasicTypeDefinition(Type type, int saveId, IBasicTypeSerializer serializer)
      : base(type, (SaveId) new TypeSaveId(saveId))
    {
      this.Serializer = serializer;
    }
  }
}
