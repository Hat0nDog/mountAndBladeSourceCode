// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBGame
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBGame
  {
    [EngineMethod("start_new", false)]
    void StartNew();

    [EngineMethod("load_module_data", false)]
    void LoadModuleData(bool isLoadGame);
  }
}
