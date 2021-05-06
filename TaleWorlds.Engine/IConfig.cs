// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IConfig
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IConfig
  {
    [EngineMethod("check_gfx_support_status", false)]
    bool CheckGFXSupportStatus(int enum_id);

    [EngineMethod("is_dlss_available", false)]
    bool IsDlssAvailable();

    [EngineMethod("get_dlss_technique", false)]
    int GetDlssTechnique();

    [EngineMethod("get_dlss_option_count", false)]
    int GetDlssOptionCount();

    [EngineMethod("get_disable_sound", false)]
    bool GetDisableSound();

    [EngineMethod("get_cheat_mode", false)]
    bool GetCheatMode();

    [EngineMethod("get_development_mode", false)]
    bool GetDevelopmentMode();

    [EngineMethod("get_do_localization_check_at_startup", false)]
    bool GetDoLocalizationCheckAtStartup();

    [EngineMethod("get_tableau_cache_mode", false)]
    bool GetTableauCacheMode();

    [EngineMethod("get_enable_edit_mode", false)]
    bool GetEnableEditMode();

    [EngineMethod("get_enable_cloth_simulation", false)]
    bool GetEnableClothSimulation();

    [EngineMethod("get_character_detail", false)]
    int GetCharacterDetail();

    [EngineMethod("get_invert_mouse", false)]
    bool GetInvertMouse();

    [EngineMethod("get_last_opened_scene", false)]
    string GetLastOpenedScene();

    [EngineMethod("set_rgl_config", false)]
    void SetRGLConfig(int type, float value);

    [EngineMethod("apply_config_changes", false)]
    void ApplyConfigChanges(bool resizeWindow);

    [EngineMethod("get_rgl_config_for_default_settings", false)]
    float GetRGLConfigForDefaultSettings(int type, int defaultSettings);

    [EngineMethod("get_rgl_config", false)]
    float GetRGLConfig(int type);

    [EngineMethod("get_default_rgl_config", false)]
    float GetDefaultRGLConfig(int type);

    [EngineMethod("save_rgl_config", false)]
    void SaveRGLConfig();

    [EngineMethod("set_brightness", false)]
    void SetBrightness(float brightness);

    [EngineMethod("set_sharpen_amount", false)]
    void SetSharpenAmount(float sharpen_amount);

    [EngineMethod("get_sound_device_name", false)]
    string GetSoundDeviceName(int i);

    [EngineMethod("get_current_sound_device_index", false)]
    int GetCurrentSoundDeviceIndex();

    [EngineMethod("get_sound_device_count", false)]
    int GetSoundDeviceCount();

    [EngineMethod("get_resolution_count", false)]
    int GetResolutionCount();

    [EngineMethod("get_refresh_rate_count", false)]
    int GetRefreshRateCount();

    [EngineMethod("get_refresh_rate_at_index", false)]
    int GetRefreshRateAtIndex(int index);

    [EngineMethod("get_resolution", false)]
    void GetResolution(ref int width, ref int height);

    [EngineMethod("get_desktop_resolution", false)]
    void GetDesktopResolution(ref int width, ref int height);

    [EngineMethod("get_resolution_at_index", false)]
    Vec2 GetResolutionAtIndex(int index);

    [EngineMethod("set_custom_resolution", false)]
    void SetCustomResolution(int width, int height);

    [EngineMethod("refresh_options_data ", false)]
    void RefreshOptionsData();

    [EngineMethod("set_sound_device", false)]
    void SetSoundDevice(int i);

    [EngineMethod("apply", false)]
    void Apply(
      int texture_budget,
      int sharpen_amount,
      int hdr,
      int dof_mode,
      int motion_blur,
      int ssr,
      int size,
      int texture_filtering,
      int trail_amount);

    [EngineMethod("set_default_game_config", false)]
    void SetDefaultGameConfig();

    [EngineMethod("get_deterministic_mode", false)]
    bool GetDeterministicMode();

    [EngineMethod("auto_save_in_minutes", false)]
    int AutoSaveInMinutes();

    [EngineMethod("get_ui_debug_mode", false)]
    bool GetUIDebugMode();

    [EngineMethod("get_ui_do_not_use_generated_prefabs", false)]
    bool GetUIDoNotUseGeneratedPrefabs();

    [EngineMethod("get_debug_login_username", false)]
    string GetDebugLoginUserName();

    [EngineMethod("get_debug_login_password", false)]
    string GetDebugLoginPassword();

    [EngineMethod("get_disable_gui_messages", false)]
    bool GetDisableGuiMessages();

    [EngineMethod("get_auto_gfx_quality", false)]
    int GetAutoGFXQuality();

    [EngineMethod("set_auto_config_wrt_hardware", false)]
    void SetAutoConfigWrtHardware();

    [EngineMethod("get_monitor_device_name", false)]
    string GetMonitorDeviceName(int i);

    [EngineMethod("get_video_device_name", false)]
    string GetVideoDeviceName(int i);

    [EngineMethod("get_monitor_device_count", false)]
    int GetMonitorDeviceCount();

    [EngineMethod("get_video_device_count", false)]
    int GetVideoDeviceCount();
  }
}
