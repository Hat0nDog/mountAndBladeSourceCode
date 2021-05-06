// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ICamera
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface ICamera
  {
    [EngineMethod("release", false)]
    void Release(UIntPtr cameraPointer);

    [EngineMethod("set_entity", false)]
    void SetEntity(UIntPtr cameraPointer, UIntPtr entityId);

    [EngineMethod("get_entity", false)]
    GameEntity GetEntity(UIntPtr cameraPointer);

    [EngineMethod("create_camera", false)]
    Camera CreateCamera();

    [EngineMethod("release_camera_entity", false)]
    void ReleaseCameraEntity(UIntPtr cameraPointer);

    [EngineMethod("look_at", false)]
    void LookAt(UIntPtr cameraPointer, Vec3 position, Vec3 target, Vec3 upVector);

    [EngineMethod("screen_space_ray_projection", false)]
    void ScreenSpaceRayProjection(
      UIntPtr cameraPointer,
      Vec2 screenPosition,
      ref Vec3 rayBegin,
      ref Vec3 rayEnd);

    [EngineMethod("check_entity_visibility", false)]
    bool CheckEntityVisibility(UIntPtr cameraPointer, UIntPtr entityPointer);

    [EngineMethod("set_position", false)]
    void SetPosition(UIntPtr cameraPointer, Vec3 position);

    [EngineMethod("set_view_volume", false)]
    void SetViewVolume(
      UIntPtr cameraPointer,
      bool perspective,
      float dLeft,
      float dRight,
      float dBottom,
      float dTop,
      float dNear,
      float dFar);

    [EngineMethod("get_near_plane_points_static", false)]
    void GetNearPlanePointsStatic(
      ref MatrixFrame cameraFrame,
      float verticalFov,
      float aspectRatioXY,
      float newDNear,
      float newDFar,
      Vec3[] nearPlanePoints);

    [EngineMethod("get_near_plane_points", false)]
    void GetNearPlanePoints(UIntPtr cameraPointer, Vec3[] nearPlanePoints);

    [EngineMethod("set_fov_vertical", false)]
    void SetFovVertical(
      UIntPtr cameraPointer,
      float verticalFov,
      float aspectRatio,
      float newDNear,
      float newDFar);

    [EngineMethod("get_view_proj_matrix", false)]
    void GetViewProjMatrix(UIntPtr cameraPointer, ref MatrixFrame frame);

    [EngineMethod("set_fov_horizontal", false)]
    void SetFovHorizontal(
      UIntPtr cameraPointer,
      float horizontalFov,
      float aspectRatio,
      float newDNear,
      float newDFar);

    [EngineMethod("get_fov_vertical", false)]
    float GetFovVertical(UIntPtr cameraPointer);

    [EngineMethod("get_fov_horizontal", false)]
    float GetFovHorizontal(UIntPtr cameraPointer);

    [EngineMethod("get_aspect_ratio", false)]
    float GetAspectRatio(UIntPtr cameraPointer);

    [EngineMethod("fill_parameters_from", false)]
    void FillParametersFrom(UIntPtr cameraPointer, UIntPtr otherCameraPointer);

    [EngineMethod("render_frustrum", false)]
    void RenderFrustrum(UIntPtr cameraPointer);

    [EngineMethod("set_frame", false)]
    void SetFrame(UIntPtr cameraPointer, ref MatrixFrame frame);

    [EngineMethod("get_frame", false)]
    void GetFrame(UIntPtr cameraPointer, ref MatrixFrame outFrame);

    [EngineMethod("get_near", false)]
    float GetNear(UIntPtr cameraPointer);

    [EngineMethod("get_far", false)]
    float GetFar(UIntPtr cameraPointer);

    [EngineMethod("get_horizontal_fov", false)]
    float GetHorizontalFov(UIntPtr cameraPointer);

    [EngineMethod("viewport_point_to_world_ray", false)]
    void ViewportPointToWorldRay(
      UIntPtr cameraPointer,
      ref Vec3 rayBegin,
      ref Vec3 rayEnd,
      Vec3 viewportPoint);

    [EngineMethod("world_point_to_viewport_point", false)]
    Vec3 WorldPointToViewportPoint(UIntPtr cameraPointer, ref Vec3 worldPoint);

    [EngineMethod("encloses_point", false)]
    bool EnclosesPoint(UIntPtr cameraPointer, Vec3 pointInWorldSpace);

    [EngineMethod("construct_camera_from_position_elevation_bearing", false)]
    void ConstructCameraFromPositionElevationBearing(
      Vec3 position,
      float elevation,
      float bearing,
      ref MatrixFrame outFrame);
  }
}
