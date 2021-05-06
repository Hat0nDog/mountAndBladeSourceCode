// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.LoadGameResult
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System.Collections.Generic;
using TaleWorlds.SaveSystem.Load;

namespace TaleWorlds.Core
{
  public class LoadGameResult
  {
    public LoadResult LoadResult { get; private set; }

    public List<ModuleCheckResult> ModuleCheckResults { get; private set; }

    public LoadGameResult(LoadResult loadResult, List<ModuleCheckResult> moduleCheckResults)
    {
      this.LoadResult = loadResult;
      this.ModuleCheckResults = moduleCheckResults;
    }
  }
}
