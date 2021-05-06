// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Camera
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglCamera_object")]
  public sealed class Camera : NativeObject
  {
    internal Camera(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static Camera CreateCamera() => EngineApplicationInterface.ICamera.CreateCamera();

    public void ReleaseCamera() => EngineApplicationInterface.ICamera.Release(this.Pointer);

    public void ReleaseCameraEntity()
    {
      EngineApplicationInterface.ICamera.ReleaseCameraEntity(this.Pointer);
      this.ReleaseCamera();
    }

    ~Camera()
    {
    }

    public void LookAt(Vec3 position, Vec3 target, Vec3 upVector) => EngineApplicationInterface.ICamera.LookAt(this.Pointer, position, target, upVector);

    public void ScreenSpaceRayProjection(Vec2 screenPosition, ref Vec3 rayBegin, ref Vec3 rayEnd)
    {
      EngineApplicationInterface.ICamera.ScreenSpaceRayProjection(this.Pointer, screenPosition, ref rayBegin, ref rayEnd);
      if (!((NativeObject) this.Entity != (NativeObject) null))
        return;
      rayBegin = this.Entity.GetGlobalFrame().TransformToParent(rayBegin);
      rayEnd = this.Entity.GetGlobalFrame().TransformToParent(rayEnd);
    }

    public bool CheckEntityVisibility(GameEntity entity) => EngineApplicationInterface.ICamera.CheckEntityVisibility(this.Pointer, entity.Pointer);

    public void SetViewVolume(
      bool perspective,
      float dLeft,
      float dRight,
      float dBottom,
      float dTop,
      float dNear,
      float dFar)
    {
      EngineApplicationInterface.ICamera.SetViewVolume(this.Pointer, perspective, dLeft, dRight, dBottom, dTop, dNear, dFar);
    }

    public static void GetNearPlanePointsStatic(
      ref MatrixFrame cameraFrame,
      float verticalFov,
      float aspectRatioXY,
      float newDNear,
      float newDFar,
      Vec3[] nearPlanePoints)
    {
      EngineApplicationInterface.ICamera.GetNearPlanePointsStatic(ref cameraFrame, verticalFov, aspectRatioXY, newDNear, newDFar, nearPlanePoints);
    }

    public void GetNearPlanePoints(Vec3[] nearPlanePoints) => EngineApplicationInterface.ICamera.GetNearPlanePoints(this.Pointer, nearPlanePoints);

    public void SetFovVertical(
      float verticalFov,
      float aspectRatioXY,
      float newDNear,
      float newDFar)
    {
      EngineApplicationInterface.ICamera.SetFovVertical(this.Pointer, verticalFov, aspectRatioXY, newDNear, newDFar);
    }

    public void SetFovHorizontal(
      float horizontalFov,
      float aspectRatioXY,
      float newDNear,
      float newDFar)
    {
      EngineApplicationInterface.ICamera.SetFovHorizontal(this.Pointer, horizontalFov, aspectRatioXY, newDNear, newDFar);
    }

    public void GetViewProjMatrix(ref MatrixFrame viewProj) => EngineApplicationInterface.ICamera.GetViewProjMatrix(this.Pointer, ref viewProj);

    public float GetFovVertical() => EngineApplicationInterface.ICamera.GetFovVertical(this.Pointer);

    public float GetFovHorizontal() => EngineApplicationInterface.ICamera.GetFovHorizontal(this.Pointer);

    public float GetAspectRatio() => EngineApplicationInterface.ICamera.GetAspectRatio(this.Pointer);

    public void FillParametersFrom(Camera otherCamera) => EngineApplicationInterface.ICamera.FillParametersFrom(this.Pointer, otherCamera.Pointer);

    public void RenderFrustrum() => EngineApplicationInterface.ICamera.RenderFrustrum(this.Pointer);

    public GameEntity Entity
    {
      get => EngineApplicationInterface.ICamera.GetEntity(this.Pointer);
      set => EngineApplicationInterface.ICamera.SetEntity(this.Pointer, value.Pointer);
    }

    public Vec3 Position
    {
      get => this.Frame.origin;
      set => EngineApplicationInterface.ICamera.SetPosition(this.Pointer, value);
    }

    public Vec3 Direction => -this.Frame.rotation.u;

    public MatrixFrame Frame
    {
      get
      {
        MatrixFrame outFrame = new MatrixFrame();
        EngineApplicationInterface.ICamera.GetFrame(this.Pointer, ref outFrame);
        return outFrame;
      }
      set => EngineApplicationInterface.ICamera.SetFrame(this.Pointer, ref value);
    }

    public float Near => EngineApplicationInterface.ICamera.GetNear(this.Pointer);

    public float Far => EngineApplicationInterface.ICamera.GetFar(this.Pointer);

    public float HorizontalFov => EngineApplicationInterface.ICamera.GetHorizontalFov(this.Pointer);

    public void ViewportPointToWorldRay(ref Vec3 rayBegin, ref Vec3 rayEnd, Vec2 viewportPoint) => EngineApplicationInterface.ICamera.ViewportPointToWorldRay(this.Pointer, ref rayBegin, ref rayEnd, viewportPoint.ToVec3());

    public Vec3 WorldPointToViewPortPoint(ref Vec3 worldPoint) => EngineApplicationInterface.ICamera.WorldPointToViewportPoint(this.Pointer, ref worldPoint);

    public bool EnclosesPoint(Vec3 pointInWorldSpace) => EngineApplicationInterface.ICamera.EnclosesPoint(this.Pointer, pointInWorldSpace);

    public static MatrixFrame ConstructCameraFromPositionElevationBearing(
      Vec3 position,
      float elevation,
      float bearing)
    {
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.ICamera.ConstructCameraFromPositionElevationBearing(position, elevation, bearing, ref outFrame);
      return outFrame;
    }
  }
}
