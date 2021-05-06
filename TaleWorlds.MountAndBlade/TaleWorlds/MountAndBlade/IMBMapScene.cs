// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IMBMapScene
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  [ScriptingInterfaceBase]
  internal interface IMBMapScene
  {
    [EngineMethod("get_accessible_point_near_position", false)]
    Vec3 GetAccessiblePointNearPosition(UIntPtr scenePointer, Vec2 position, float radius);

    [EngineMethod("remove_zero_corner_bodies", false)]
    void RemoveZeroCornerBodies(UIntPtr scenePointer);

    [EngineMethod("load_atmosphere_data", false)]
    void LoadAtmosphereData(UIntPtr scenePointer);

    [EngineMethod("get_face_index_for_multiple_positions", false)]
    void GetFaceIndexForMultiplePositions(
      UIntPtr scenePointer,
      int movedPartyCount,
      float[] positionArray,
      PathFaceRecord[] resultArray,
      bool check_if_disabled,
      bool check_height);

    [EngineMethod("set_sound_parameters", false)]
    void SetSoundParameters(UIntPtr scenePointer, float tod, int season, float cameraHeight);

    [EngineMethod("tick_step_sound", false)]
    void TickStepSound(
      UIntPtr scenePointer,
      UIntPtr strategicEntityId,
      int faceIndexterrainType,
      int soundType);

    [EngineMethod("tick_ambient_sounds", false)]
    void TickAmbientSounds(UIntPtr scenePointer, int terrainType);

    [EngineMethod("tick_visuals", false)]
    void TickVisuals(
      UIntPtr scenePointer,
      float tod,
      UIntPtr[] ticked_map_meshes,
      int tickedMapMeshesCount);

    [EngineMethod("validate_terrain_sound_ids", false)]
    void ValidateTerrainSoundIds();

    [EngineMethod("set_political_color", false)]
    void SetPoliticalColor(UIntPtr scenePointer, string value);

    [EngineMethod("set_frame_for_atmosphere", false)]
    void SetFrameForAtmosphere(UIntPtr scenePointer, float tod, float cameraElevation);

    [EngineMethod("get_color_grade_grid_data", false)]
    void GetColorGradeGridData(UIntPtr scenePointer, byte[] snowData);

    [EngineMethod("set_terrain_dynamic_params", false)]
    void SetTerrainDynamicParams(UIntPtr scenePointer, Vec3 dynamic_params);

    [EngineMethod("get_mouse_visible", false)]
    bool GetMouseVisible();

    [EngineMethod("send_mouse_key_down_event", false)]
    void SendMouseKeyEvent(int keyId, bool isDown);

    [EngineMethod("set_mouse_visible", false)]
    void SetMouseVisible(bool value);

    [EngineMethod("set_mouse_pos", false)]
    void SetMousePos(int posX, int posY);
  }
}
