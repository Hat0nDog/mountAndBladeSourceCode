// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Load.MemberLoadData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Load
{
  internal abstract class MemberLoadData : VariableLoadData
  {
    public ObjectLoadData ObjectLoadData { get; private set; }

    protected MemberLoadData(ObjectLoadData objectLoadData, IReader reader)
      : base(objectLoadData.Context, reader)
    {
      this.ObjectLoadData = objectLoadData;
    }
  }
}
