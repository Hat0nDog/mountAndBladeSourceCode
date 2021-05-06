// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.NativeConfig
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Engine.Options;

namespace TaleWorlds.Engine
{
  public static class NativeConfig
  {
    public static bool DisableSound => EngineApplicationInterface.IConfig.GetDisableSound();

    public static bool CheatMode => EngineApplicationInterface.IConfig.GetCheatMode();

    public static bool IsDevelopmentMode => EngineApplicationInterface.IConfig.GetDevelopmentMode();

    public static bool TableauCacheEnabled => EngineApplicationInterface.IConfig.GetTableauCacheMode();

    public static bool DoLocalizationCheckAtStartup => EngineApplicationInterface.IConfig.GetDoLocalizationCheckAtStartup();

    public static bool EnableEditMode => EngineApplicationInterface.IConfig.GetEnableEditMode();

    public static bool EnableClothSimulation => EngineApplicationInterface.IConfig.GetEnableClothSimulation();

    public static int CharacterDetail => EngineApplicationInterface.IConfig.GetCharacterDetail();

    public static bool InvertMouse => EngineApplicationInterface.IConfig.GetInvertMouse();

    public static string LastOpenedScene => EngineApplicationInterface.IConfig.GetLastOpenedScene();

    public static bool DeterministicMode => EngineApplicationInterface.IConfig.GetDeterministicMode();

    public static int AutoSaveInMinutes => EngineApplicationInterface.IConfig.AutoSaveInMinutes();

    public static bool GetUIDebugMode => EngineApplicationInterface.IConfig.GetUIDebugMode();

    public static bool GetUIDoNotUseGeneratedPrefabs => EngineApplicationInterface.IConfig.GetUIDoNotUseGeneratedPrefabs();

    public static string DebugLoginUsername => EngineApplicationInterface.IConfig.GetDebugLoginUserName();

    public static string DebugLogicPassword => EngineApplicationInterface.IConfig.GetDebugLoginPassword();

    public static bool DisableGuiMessages => EngineApplicationInterface.IConfig.GetDisableGuiMessages();

    public static NativeOptions.ConfigQuality AutoGFXQuality => (NativeOptions.ConfigQuality) EngineApplicationInterface.IConfig.GetAutoGFXQuality();

    public static void SetAutoConfigWrtHardware() => EngineApplicationInterface.IConfig.SetAutoConfigWrtHardware();
  }
}
