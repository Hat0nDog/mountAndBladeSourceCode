// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.CampaignSaveMetaDataArgs
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;

namespace TaleWorlds.Core
{
  public readonly struct CampaignSaveMetaDataArgs
  {
    public readonly string[] ModuleNames;
    public readonly KeyValuePair<string, string>[] OtherData;

    public CampaignSaveMetaDataArgs(
      string[] moduleName,
      params KeyValuePair<string, string>[] otherArgs)
    {
      this.ModuleNames = moduleName;
      this.OtherData = otherArgs;
    }
  }
}
