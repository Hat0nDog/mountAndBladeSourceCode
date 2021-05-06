﻿// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IGameEntityComponent
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IGameEntityComponent
  {
    [EngineMethod("get_entity", false)]
    GameEntity GetEntity(GameEntityComponent entityComponent);

    [EngineMethod("get_first_meta_mesh", false)]
    MetaMesh GetFirstMetaMesh(GameEntityComponent entityComponent);
  }
}
