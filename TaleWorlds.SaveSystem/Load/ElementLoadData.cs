// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.ElementLoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Load
{
  internal class ElementLoadData : VariableLoadData
  {
    public ContainerLoadData ContainerLoadData { get; private set; }

    internal ElementLoadData(ContainerLoadData containerLoadData, IReader reader)
      : base(containerLoadData.Context, reader)
    {
      this.ContainerLoadData = containerLoadData;
    }
  }
}
