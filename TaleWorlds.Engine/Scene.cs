// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Scene
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglScene")]
  public sealed class Scene : NativeObject
  {
    public const float AutoClimbHeight = 1.5f;
    public const float NavMeshHeightLimit = 1.5f;

    internal Scene(UIntPtr pointer)
      : base(pointer)
    {
    }

    public string TakePhotoModePicture() => EngineApplicationInterface.IScene.TakePhotoModePicture(this);

    public string GetAllColorGradeNames() => EngineApplicationInterface.IScene.GetAllColorGradeNames(this);

    public string GetAllFilterNames() => EngineApplicationInterface.IScene.GetAllFilterNames(this);

    public float GetPhotoModeRoll() => EngineApplicationInterface.IScene.GetPhotoModeRoll(this);

    public bool GetPhotoModeOrbit() => EngineApplicationInterface.IScene.GetPhotoModeOrbit(this);

    public bool GetPhotoModeOn() => EngineApplicationInterface.IScene.GetPhotoModeOn(this);

    public void GetPhotoModeFocus(
      ref float focus,
      ref float focusStart,
      ref float focusEnd,
      ref float exposure,
      ref bool vignetteOn)
    {
      EngineApplicationInterface.IScene.GetPhotoModeFocus(this, ref focus, ref focusStart, ref focusEnd, ref exposure, ref vignetteOn);
    }

    public int GetSceneColorGradeIndex() => EngineApplicationInterface.IScene.GetSceneColorGradeIndex(this);

    public int GetSceneFilterIndex() => EngineApplicationInterface.IScene.GetSceneFilterIndex(this);

    public void SetPhotoModeRoll(float roll) => EngineApplicationInterface.IScene.SetPhotoModeRoll(this, roll);

    public void SetPhotoModeOrbit(bool orbit) => EngineApplicationInterface.IScene.SetPhotoModeOrbit(this, orbit);

    public void SetPhotoModeOn(bool on) => EngineApplicationInterface.IScene.SetPhotoModeOn(this, on);

    public void SetPhotoModeFocus(float focusStart, float focusEnd, float focus, float exposure) => EngineApplicationInterface.IScene.SetPhotoModeFocus(this, focusStart, focusEnd, focus, exposure);

    public void SetPhotoModeVignette(bool vignetteOn) => EngineApplicationInterface.IScene.SetPhotoModeVignette(this, vignetteOn);

    public void SetSceneColorGradeIndex(int index) => EngineApplicationInterface.IScene.SetSceneColorGradeIndex(this, index);

    public int SetSceneFilterIndex(int index) => EngineApplicationInterface.IScene.SetSceneFilterIndex(this, index);

    public void SetSceneColorGrade(string textureName) => EngineApplicationInterface.IScene.SetSceneColorGrade(this, textureName);

    public void SetUpgradeLevel(int level) => EngineApplicationInterface.IScene.SetUpgradeLevel(this.Pointer, level);

    public void CreateBurstParticle(int particleId, MatrixFrame frame) => EngineApplicationInterface.IScene.CreateBurstParticle(this, particleId, ref frame);

    public float[] GetTerrainHeightData(int nodeXIndex, int nodeYIndex)
    {
      float[] heightArray = new float[EngineApplicationInterface.IScene.GetNodeDataCount(this, nodeXIndex, nodeYIndex)];
      EngineApplicationInterface.IScene.FillTerrainHeightData(this, nodeXIndex, nodeYIndex, heightArray);
      return heightArray;
    }

    public short[] GetTerrainPhysicsMaterialIndexData(int nodeXIndex, int nodeYIndex)
    {
      short[] materialIndexArray = new short[EngineApplicationInterface.IScene.GetNodeDataCount(this, nodeXIndex, nodeYIndex)];
      EngineApplicationInterface.IScene.FillTerrainPhysicsMaterialIndexData(this, nodeXIndex, nodeYIndex, materialIndexArray);
      return materialIndexArray;
    }

    public void GetTerrainData(
      out Vec2i nodeDimension,
      out float nodeSize,
      out int layerCount,
      out int layerVersion)
    {
      EngineApplicationInterface.IScene.GetTerrainData(this, out nodeDimension, out nodeSize, out layerCount, out layerVersion);
    }

    public void GetTerrainNodeData(
      int xIndex,
      int yIndex,
      out int vertexCountAlongAxis,
      out float quadLength,
      out float minHeight,
      out float maxHeight)
    {
      EngineApplicationInterface.IScene.GetTerrainNodeData(this, xIndex, yIndex, out vertexCountAlongAxis, out quadLength, out minHeight, out maxHeight);
    }

    public PhysicsMaterial GetTerrainPhysicsMaterialAtLayer(int layerIndex) => new PhysicsMaterial(EngineApplicationInterface.IScene.GetTerrainPhysicsMaterialIndexAtLayer(this, layerIndex));

    public void SetSceneColorGrade(Scene scene, string textureName) => EngineApplicationInterface.IScene.SetSceneColorGrade(scene, textureName);

    public float GetWaterLevel() => EngineApplicationInterface.IScene.GetWaterLevel(this);

    public bool GetPathBetweenAIFaces(
      UIntPtr startingFace,
      UIntPtr endingFace,
      Vec2 startingPosition,
      Vec2 endingPosition,
      float agentRadius,
      NavigationPath path)
    {
      int length = path.PathPoints.Length;
      if (EngineApplicationInterface.IScene.GetPathBetweenAIFacePointers(this.Pointer, startingFace, endingFace, startingPosition, endingPosition, agentRadius, path.PathPoints, ref length))
      {
        path.Size = length;
        return true;
      }
      path.Size = 0;
      return false;
    }

    public bool GetPathBetweenAIFaces(
      int startingFace,
      int endingFace,
      Vec2 startingPosition,
      Vec2 endingPosition,
      float agentRadius,
      NavigationPath path)
    {
      int length = path.PathPoints.Length;
      if (EngineApplicationInterface.IScene.GetPathBetweenAIFaceIndices(this.Pointer, startingFace, endingFace, startingPosition, endingPosition, agentRadius, path.PathPoints, ref length))
      {
        path.Size = length;
        return true;
      }
      path.Size = 0;
      return false;
    }

    public bool GetPathDistanceBetweenAIFaces(
      int startingAiFace,
      int endingAiFace,
      Vec2 startingPosition,
      Vec2 endingPosition,
      float agentRadius,
      float distanceLimit,
      out float distance)
    {
      return EngineApplicationInterface.IScene.GetPathDistanceBetweenAIFaces(this.Pointer, startingAiFace, endingAiFace, startingPosition, endingPosition, agentRadius, distanceLimit, out distance);
    }

    public void GetNavMeshFaceIndex(
      ref PathFaceRecord record,
      Vec2 position,
      bool checkIfDisabled,
      bool ignoreHeight = false)
    {
      EngineApplicationInterface.IScene.GetNavMeshFaceIndex(this.Pointer, ref record, position, checkIfDisabled, ignoreHeight);
    }

    public void GetNavMeshFaceIndex(ref PathFaceRecord record, Vec3 position, bool checkIfDisabled) => EngineApplicationInterface.IScene.GetNavMeshFaceIndex3(this.Pointer, ref record, position, checkIfDisabled);

    public static Scene CreateNewScene(bool initialize_physics = true) => EngineApplicationInterface.IScene.CreateNewScene(initialize_physics);

    public MetaMesh CreatePathMesh(string baseEntityName, bool isWaterPath) => EngineApplicationInterface.IScene.CreatePathMesh(this.Pointer, baseEntityName, isWaterPath);

    public void SetActiveVisibilityLevels(List<string> levelsToActivate)
    {
      string levelsAppended = "";
      for (int index = 0; index < levelsToActivate.Count; ++index)
      {
        if (!levelsToActivate[index].Contains("$"))
        {
          if (index != 0)
            levelsAppended += "$";
          levelsAppended += levelsToActivate[index];
        }
      }
      EngineApplicationInterface.IScene.SetActiveVisibilityLevels(this.Pointer, levelsAppended);
    }

    public MetaMesh CreatePathMesh(IList<GameEntity> pathNodes, bool isWaterPath = false) => EngineApplicationInterface.IScene.CreatePathMesh2(this.Pointer, pathNodes.Select<GameEntity, UIntPtr>((Func<GameEntity, UIntPtr>) (e => e.Pointer)).ToArray<UIntPtr>(), pathNodes.Count, isWaterPath);

    public GameEntity GetEntityWithGuid(string guid) => EngineApplicationInterface.IScene.GetEntityWithGuid(this.Pointer, guid);

    public bool IsEntityFrameChanged(string containsName) => EngineApplicationInterface.IScene.CheckPathEntitiesFrameChanged(this.Pointer, containsName);

    public void GetTerrainHeightAndNormal(Vec2 position, out float height, out Vec3 normal) => EngineApplicationInterface.IScene.GetTerrainHeightAndNormal(this.Pointer, position, out height, out normal);

    public void StallLoadingRenderingsUntilFurtherNotice() => EngineApplicationInterface.IScene.StallLoadingRenderingsUntilFurtherNotice(this.Pointer);

    public void ResumeLoadingRenderings() => EngineApplicationInterface.IScene.ResumeLoadingRenderings(this.Pointer);

    public uint GetUpgradeLevelMask() => EngineApplicationInterface.IScene.GetUpgradeLevelMask(this.Pointer);

    public void SetUpgradeLevelVisibility(uint mask) => EngineApplicationInterface.IScene.SetUpgradeLevelVisibilityWithMask(this.Pointer, mask);

    public void SetUpgradeLevelVisibility(List<string> levels)
    {
      string str = "";
      for (int index = 0; index < levels.Count - 1; ++index)
        str = str + levels[index] + "|";
      string concatLevels = str + levels[levels.Count - 1];
      EngineApplicationInterface.IScene.SetUpgradeLevelVisibility(this.Pointer, concatLevels);
    }

    public int GetIdOfNavMeshFace(int faceIndex) => EngineApplicationInterface.IScene.GetIdOfNavMeshFace(this.Pointer, faceIndex);

    public void SetClothSimulationState(bool state) => EngineApplicationInterface.IScene.SetClothSimulationState(this.Pointer, state);

    public void GetNavMeshCenterPosition(int faceIndex, ref Vec3 centerPosition) => EngineApplicationInterface.IScene.GetNavMeshFaceCenterPosition(this.Pointer, faceIndex, ref centerPosition);

    public GameEntity GetFirstEntityWithName(string name) => EngineApplicationInterface.IScene.GetFirstEntityWithName(this.Pointer, name);

    public GameEntity GetFirstEntityWithScriptComponent<T>() where T : ScriptComponentBehaviour
    {
      string name = typeof (T).Name;
      return EngineApplicationInterface.IScene.GetFirstEntityWithScriptComponent(this.Pointer, name);
    }

    public uint GetUpgradeLevelMaskOfLevelName(string levelName) => EngineApplicationInterface.IScene.GetUpgradeLevelMaskOfLevelName(this.Pointer, levelName);

    public float GetWinterTimeFactor() => EngineApplicationInterface.IScene.GetWinterTimeFactor(this.Pointer);

    public float GetNavMeshFaceFirstVertexZ(int faceIndex) => EngineApplicationInterface.IScene.GetNavMeshFaceFirstVertexZ(this.Pointer, faceIndex);

    public void SetWinterTimeFactor(float winterTimeFactor) => EngineApplicationInterface.IScene.SetWinterTimeFactor(this.Pointer, winterTimeFactor);

    public void SetDrynessFactor(float drynessFactor) => EngineApplicationInterface.IScene.SetDrynessFactor(this.Pointer, drynessFactor);

    public float GetFog() => EngineApplicationInterface.IScene.GetFog(this.Pointer);

    public void SetFog(float fogDensity, ref Vec3 fogColor, float fogFalloff) => EngineApplicationInterface.IScene.SetFog(this.Pointer, fogDensity, ref fogColor, fogFalloff);

    public void SetFogAdvanced(
      float fogFalloffOffset,
      float fogFalloffMinFog,
      float fogFalloffStartDist)
    {
      EngineApplicationInterface.IScene.SetFogAdvanced(this.Pointer, fogFalloffOffset, fogFalloffMinFog, fogFalloffStartDist);
    }

    public void SetFogAmbientColor(ref Vec3 fogAmbientColor) => EngineApplicationInterface.IScene.SetFogAmbientColor(this.Pointer, ref fogAmbientColor);

    public void SetTemperature(float temperature) => EngineApplicationInterface.IScene.SetTemperature(this.Pointer, temperature);

    public void SetHumidity(float humidity) => EngineApplicationInterface.IScene.SetHumidity(this.Pointer, humidity);

    public void SetDynamicShadowmapCascadesRadiusMultiplier(float multiplier) => EngineApplicationInterface.IScene.SetDynamicShadowmapCascadesRadiusMultiplier(this.Pointer, multiplier);

    public void SetEnvironmentMultiplier(bool useMultiplier, float multiplier) => EngineApplicationInterface.IScene.SetEnvironmentMultiplier(this.Pointer, useMultiplier, multiplier);

    public void SetSkyRotation(float rotation) => EngineApplicationInterface.IScene.SetSkyRotation(this.Pointer, rotation);

    public void SetSkyBrightness(float brightness) => EngineApplicationInterface.IScene.SetSkyBrigthness(this.Pointer, brightness);

    public void SetForcedSnow(bool value) => EngineApplicationInterface.IScene.SetForcedSnow(this.Pointer, value);

    public void SetSunLight(ref Vec3 color, ref Vec3 direction) => EngineApplicationInterface.IScene.SetSunLight(this.Pointer, color, direction);

    public void SetSunDirection(ref Vec3 direction) => EngineApplicationInterface.IScene.SetSunDirection(this.Pointer, direction);

    public void SetSun(ref Vec3 color, float altitude, float angle, float intensity) => EngineApplicationInterface.IScene.SetSun(this.Pointer, color, altitude, angle, intensity);

    public void SetSunAngleAltitude(float angle, float altitude) => EngineApplicationInterface.IScene.SetSunAngleAltidude(this.Pointer, angle, altitude);

    public void SetSunSize(float size) => EngineApplicationInterface.IScene.SetSunSize(this.Pointer, size);

    public void SetSunShaftStrength(float strength) => EngineApplicationInterface.IScene.SetSunShaftStrength(this.Pointer, strength);

    public float GetRainDensity() => EngineApplicationInterface.IScene.GetRainDensity(this.Pointer);

    public void SetRainDensity(float density) => EngineApplicationInterface.IScene.SetRainDensity(this.Pointer, density);

    public float GetSnowDensity() => EngineApplicationInterface.IScene.GetSnowDensity(this.Pointer);

    public void SetSnowDensity(float density) => EngineApplicationInterface.IScene.SetSnowDensity(this.Pointer, density);

    public void AddDecalInstance(Decal decal, string decalSetID, bool deletable) => EngineApplicationInterface.IScene.AddDecalInstance(this.Pointer, decal.Pointer, decalSetID, deletable);

    public void SetShadow(bool shadowEnabled) => EngineApplicationInterface.IScene.SetShadow(this.Pointer, shadowEnabled);

    public int AddPointLight(ref Vec3 position, float radius) => EngineApplicationInterface.IScene.AddPointLight(this.Pointer, position, radius);

    public int AddDirectionalLight(ref Vec3 position, ref Vec3 direction, float radius) => EngineApplicationInterface.IScene.AddDirectionalLight(this.Pointer, position, direction, radius);

    public void SetLightPosition(int lightIndex, ref Vec3 position) => EngineApplicationInterface.IScene.SetLightPosition(this.Pointer, lightIndex, position);

    public void SetLightDiffuseColor(int lightIndex, ref Vec3 diffuseColor) => EngineApplicationInterface.IScene.SetLightDiffuseColor(this.Pointer, lightIndex, diffuseColor);

    public void SetLightDirection(int lightIndex, ref Vec3 direction) => EngineApplicationInterface.IScene.SetLightDirection(this.Pointer, lightIndex, direction);

    public void SetMieScatterFocus(float strength) => EngineApplicationInterface.IScene.SetMieScatterFocus(this.Pointer, strength);

    public void SetMieScatterStrength(float strength) => EngineApplicationInterface.IScene.SetMieScatterStrength(this.Pointer, strength);

    public void SetBrightpassThreshold(float threshold) => EngineApplicationInterface.IScene.SetBrightpassTreshold(this.Pointer, threshold);

    public void SetLensDistortion(float amount) => EngineApplicationInterface.IScene.SetLensDistortion(this.Pointer, amount);

    public void SetHexagonVignetteAlpha(float amount) => EngineApplicationInterface.IScene.SetHexagonVignetteAlpha(this.Pointer, amount);

    public void SetMinExposure(float minExposure) => EngineApplicationInterface.IScene.SetMinExposure(this.Pointer, minExposure);

    public void SetMaxExposure(float maxExposure) => EngineApplicationInterface.IScene.SetMaxExposure(this.Pointer, maxExposure);

    public void SetTargetExposure(float targetExposure) => EngineApplicationInterface.IScene.SetTargetExposure(this.Pointer, targetExposure);

    public void SetMiddleGray(float middleGray) => EngineApplicationInterface.IScene.SetMiddleGray(this.Pointer, middleGray);

    public void SetBloomStrength(float bloomStrength) => EngineApplicationInterface.IScene.SetBloomStrength(this.Pointer, bloomStrength);

    public void SetBloomAmount(float bloomAmount) => EngineApplicationInterface.IScene.SetBloomAmount(this.Pointer, bloomAmount);

    public void SetGrainAmount(float grainAmount) => EngineApplicationInterface.IScene.SetGrainAmount(this.Pointer, grainAmount);

    public GameEntity AddItemEntity(ref MatrixFrame placementFrame, MetaMesh metaMesh) => EngineApplicationInterface.IScene.AddItemEntity(this.Pointer, ref placementFrame, metaMesh.Pointer);

    public void RemoveEntity(GameEntity entity, int removeReason) => EngineApplicationInterface.IScene.RemoveEntity(this.Pointer, entity.Pointer, removeReason);

    public bool AttachEntity(GameEntity entity, bool showWarnings = false) => EngineApplicationInterface.IScene.AttachEntity(this.Pointer, entity.Pointer, showWarnings);

    public void AddEntityWithMesh(Mesh mesh, ref MatrixFrame frame) => EngineApplicationInterface.IScene.AddEntityWithMesh(this.Pointer, mesh.Pointer, ref frame);

    public void AddEntityWithMultiMesh(MetaMesh mesh, ref MatrixFrame frame) => EngineApplicationInterface.IScene.AddEntityWithMultiMesh(this.Pointer, mesh.Pointer, ref frame);

    public void Tick(float dt) => EngineApplicationInterface.IScene.Tick(this.Pointer, dt);

    public void ClearAll() => EngineApplicationInterface.IScene.ClearAll(this.Pointer);

    public void SetDefaultLighting()
    {
      Vec3 color = new Vec3(1.15f, 1.2f, 1.25f);
      Vec3 direction = new Vec3(1f, -1f, -1f);
      double num = (double) direction.Normalize();
      this.SetSunLight(ref color, ref direction);
      this.SetShadow(false);
    }

    public bool CalculateEffectiveLighting() => EngineApplicationInterface.IScene.CalculateEffectiveLighting(this.Pointer);

    public bool GetPathDistanceBetweenPositions(
      ref WorldPosition point0,
      ref WorldPosition point1,
      float agentRadius,
      out float pathDistance)
    {
      pathDistance = 0.0f;
      return EngineApplicationInterface.IScene.GetPathDistanceBetweenPositions(this.Pointer, ref point0, ref point1, agentRadius, ref pathDistance);
    }

    public bool IsLineToPointClear(
      ref WorldPosition position,
      ref WorldPosition destination,
      float agentRadius)
    {
      return EngineApplicationInterface.IScene.IsLineToPointClear2(this.Pointer, position.GetNavMesh(), position.AsVec2, destination.AsVec2, agentRadius);
    }

    public bool IsLineToPointClear(
      int startingFace,
      Vec2 position,
      Vec2 destination,
      float agentRadius)
    {
      return EngineApplicationInterface.IScene.IsLineToPointClear(this.Pointer, startingFace, position, destination, agentRadius);
    }

    public Vec2 GetLastPointWithinNavigationMeshForLineToPoint(
      int startingFace,
      Vec2 position,
      Vec2 destination)
    {
      return EngineApplicationInterface.IScene.GetLastPointWithinNavigationMeshForLineToPoint(this.Pointer, startingFace, position, destination);
    }

    public bool DoesPathExistBetweenFaces(
      int firstNavMeshFace,
      int secondNavMeshFace,
      bool ignoreDisabled)
    {
      return EngineApplicationInterface.IScene.DoesPathExistBetweenFaces(this.Pointer, firstNavMeshFace, secondNavMeshFace, ignoreDisabled);
    }

    public bool GetHeightAtPoint(Vec2 point, BodyFlags excludeBodyFlags, ref float height) => EngineApplicationInterface.IScene.GetHeightAtPoint(this.Pointer, point, excludeBodyFlags, ref height);

    public Vec3 GetNormalAt(Vec2 position) => EngineApplicationInterface.IScene.GetNormalAt(this.Pointer, position);

    public void GetEntities(ref List<GameEntity> entities)
    {
      NativeObjectArray nativeObjectArray = NativeObjectArray.Create();
      EngineApplicationInterface.IScene.GetEntities(this.Pointer, nativeObjectArray.Pointer);
      for (int index = 0; index < nativeObjectArray.Count; ++index)
        entities.Add(nativeObjectArray.GetElementAt(index) as GameEntity);
    }

    public void GetRootEntities(NativeObjectArray entities) => EngineApplicationInterface.IScene.GetRootEntities(this, entities);

    public int RootEntityCount => EngineApplicationInterface.IScene.GetRootEntityCount(this.Pointer);

    public int SelectEntitiesInBoxWithScriptComponent<T>(
      ref Vec3 boundingBoxMin,
      ref Vec3 boundingBoxMax,
      GameEntity[] entitiesOutput,
      UIntPtr[] entityIds)
      where T : ScriptComponentBehaviour
    {
      string name = typeof (T).Name;
      int num = EngineApplicationInterface.IScene.SelectEntitiesInBoxWithScriptComponent(this.Pointer, ref boundingBoxMin, ref boundingBoxMax, entityIds, entitiesOutput.Length, name);
      for (int index = 0; index < num; ++index)
        entitiesOutput[index] = new GameEntity(entityIds[index]);
      return num;
    }

    public int SelectEntitiesCollidedWith(
      ref Ray ray,
      GameEntity[] entitiesOutput,
      Intersection[] intersectionsOutput,
      UIntPtr[] entityIds)
    {
      int num = EngineApplicationInterface.IScene.SelectEntitiesCollidedWith(this.Pointer, ref ray, entityIds, intersectionsOutput);
      for (int index = 0; index < num; ++index)
      {
        if (entityIds[index] != (UIntPtr) 0UL)
          entitiesOutput[index] = new GameEntity(entityIds[index]);
      }
      return num;
    }

    public int GenerateContactsWithCapsule(
      ref CapsuleData capsule,
      BodyFlags exclude_flags,
      Intersection[] intersectionsOutput)
    {
      return EngineApplicationInterface.IScene.GenerateContactsWithCapsule(this.Pointer, ref capsule, exclude_flags, intersectionsOutput);
    }

    public void InvalidateTerrainPhysicsMaterials() => EngineApplicationInterface.IScene.InvalidateTerrainPhysicsMaterials(this.Pointer);

    public void Read(string sceneName)
    {
      SceneInitializationData initData = new SceneInitializationData(true);
      EngineApplicationInterface.IScene.Read(this.Pointer, sceneName, ref initData);
    }

    public void Read(string sceneName, SceneInitializationData initData) => EngineApplicationInterface.IScene.Read(this.Pointer, sceneName, ref initData);

    public MatrixFrame ReadAndCalculateInitialCamera()
    {
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.IScene.ReadAndCalculateInitialCamera(this.Pointer, ref outFrame);
      return outFrame;
    }

    public void OptimizeScene(bool optimizeFlora = true, bool optimizeOro = false) => EngineApplicationInterface.IScene.OptimizeScene(this.Pointer, optimizeFlora, optimizeOro);

    public float GetTerrainHeight(Vec2 position, bool checkHoles = true) => EngineApplicationInterface.IScene.GetTerrainHeight(this.Pointer, position, checkHoles);

    public void CheckResources() => EngineApplicationInterface.IScene.CheckResources(this.Pointer);

    public void ForceLoadResources() => EngineApplicationInterface.IScene.ForceLoadResources(this.Pointer);

    public void SetDepthOfFieldParameters(
      float depthOfFieldFocusStart,
      float depthOfFieldFocusEnd,
      bool isVignetteOn)
    {
      EngineApplicationInterface.IScene.SetDofParams(this.Pointer, depthOfFieldFocusStart, depthOfFieldFocusEnd, isVignetteOn);
    }

    public void SetDepthOfFieldFocus(float depthOfFieldFocus) => EngineApplicationInterface.IScene.SetDofFocus(this.Pointer, depthOfFieldFocus);

    public void ResetDepthOfFieldParams()
    {
      EngineApplicationInterface.IScene.SetDofFocus(this.Pointer, 0.0f);
      EngineApplicationInterface.IScene.SetDofParams(this.Pointer, 0.0f, 0.0f, true);
    }

    public bool HasTerrainHeightmap => EngineApplicationInterface.IScene.HasTerrainHeightmap(this.Pointer);

    public bool ContainsTerrain => EngineApplicationInterface.IScene.ContainsTerrain(this.Pointer);

    public float TimeOfDay
    {
      set => EngineApplicationInterface.IScene.SetTimeOfDay(this.Pointer, value);
      get => EngineApplicationInterface.IScene.GetTimeOfDay(this.Pointer);
    }

    public bool IsAtmosphereIndoor => EngineApplicationInterface.IScene.IsAtmosphereIndoor(this.Pointer);

    public void PreloadForRendering() => EngineApplicationInterface.IScene.PreloadForRendering(this.Pointer);

    public Vec3 LastFinalRenderCameraPosition => EngineApplicationInterface.IScene.GetLastFinalRenderCameraPosition(this.Pointer);

    public MatrixFrame LastFinalRenderCameraFrame
    {
      get
      {
        MatrixFrame outFrame = new MatrixFrame();
        EngineApplicationInterface.IScene.GetLastFinalRenderCameraFrame(this.Pointer, ref outFrame);
        return outFrame;
      }
    }

    public void SetColorGradeBlend(string texture1, string texture2, float alpha) => EngineApplicationInterface.IScene.SetColorGradeBlend(this.Pointer, texture1, texture2, alpha);

    public float GetGroundHeightAtPosition(
      Vec3 position,
      BodyFlags excludeFlags = BodyFlags.CommonCollisionExcludeFlags,
      bool excludeInvisibleEntities = true)
    {
      return EngineApplicationInterface.IScene.GetGroundHeightAtPosition(this.Pointer, position, (uint) excludeFlags, excludeInvisibleEntities);
    }

    public float GetGroundHeightAtPosition(
      Vec3 position,
      out Vec3 normal,
      BodyFlags excludeFlags = BodyFlags.CommonCollisionExcludeFlags,
      bool excludeInvisibleEntities = true)
    {
      normal = Vec3.Invalid;
      return EngineApplicationInterface.IScene.GetGroundHeightAndNormalAtPosition(this.Pointer, position, ref normal, (uint) excludeFlags, excludeInvisibleEntities);
    }

    public void PauseSceneSounds() => EngineApplicationInterface.IScene.PauseSceneSounds(this.Pointer);

    public void ResumeSceneSounds() => EngineApplicationInterface.IScene.ResumeSceneSounds(this.Pointer);

    public void FinishSceneSounds() => EngineApplicationInterface.IScene.FinishSceneSounds(this.Pointer);

    public bool BoxCastOnlyForCamera(
      Vec3[] boxPoints,
      Vec3 centerPoint,
      bool castSupportRay,
      Vec3 supportRaycastPoint,
      Vec3 dir,
      float distance,
      out float collisionDistance,
      out Vec3 closestPoint,
      out GameEntity collidedEntity,
      bool preFilter = true,
      bool postFilter = true)
    {
      collisionDistance = float.NaN;
      closestPoint = Vec3.Invalid;
      UIntPtr zero = UIntPtr.Zero;
      bool flag = castSupportRay && EngineApplicationInterface.IScene.RayCastForClosestEntityOrTerrain(this.Pointer, ref supportRaycastPoint, ref centerPoint, 0.0f, ref collisionDistance, ref closestPoint, ref zero, BodyFlags.CameraCollisionRayCastExludeFlags | BodyFlags.DontCollideWithCamera);
      if (!flag)
        flag = EngineApplicationInterface.IScene.BoxCastOnlyForCamera(this.Pointer, boxPoints, ref centerPoint, ref dir, distance, ref collisionDistance, ref closestPoint, ref zero, BodyFlags.CameraCollisionRayCastExludeFlags | BodyFlags.DontCollideWithCamera, preFilter, postFilter);
      collidedEntity = !flag || !(zero != UIntPtr.Zero) ? (GameEntity) null : new GameEntity(zero);
      return flag;
    }

    public bool RayCastForClosestEntityOrTerrain(
      Vec3 sourcePoint,
      Vec3 targetPoint,
      out float collisionDistance,
      out Vec3 closestPoint,
      out GameEntity collidedEntity,
      float rayThickness = 0.01f,
      BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
    {
      collisionDistance = float.NaN;
      closestPoint = Vec3.Invalid;
      UIntPtr zero = UIntPtr.Zero;
      int num = EngineApplicationInterface.IScene.RayCastForClosestEntityOrTerrain(this.Pointer, ref sourcePoint, ref targetPoint, rayThickness, ref collisionDistance, ref closestPoint, ref zero, excludeBodyFlags) ? 1 : 0;
      if (num != 0 && zero != UIntPtr.Zero)
      {
        collidedEntity = new GameEntity(zero);
        return num != 0;
      }
      collidedEntity = (GameEntity) null;
      return num != 0;
    }

    public bool RayCastForClosestEntityOrTerrain(
      Vec3 sourcePoint,
      Vec3 targetPoint,
      out float collisionDistance,
      out GameEntity collidedEntity,
      float rayThickness = 0.01f,
      BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
    {
      return this.RayCastForClosestEntityOrTerrain(sourcePoint, targetPoint, out collisionDistance, out Vec3 _, out collidedEntity, rayThickness, excludeBodyFlags);
    }

    public bool RayCastForClosestEntityOrTerrain(
      Vec3 sourcePoint,
      Vec3 targetPoint,
      out float collisionDistance,
      out Vec3 closestPoint,
      float rayThickness = 0.01f,
      BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
    {
      collisionDistance = float.NaN;
      closestPoint = Vec3.Invalid;
      UIntPtr zero = UIntPtr.Zero;
      return EngineApplicationInterface.IScene.RayCastForClosestEntityOrTerrain(this.Pointer, ref sourcePoint, ref targetPoint, rayThickness, ref collisionDistance, ref closestPoint, ref zero, excludeBodyFlags);
    }

    public bool RayCastForClosestEntityOrTerrain(
      Vec3 sourcePoint,
      Vec3 targetPoint,
      out float collisionDistance,
      float rayThickness = 0.01f,
      BodyFlags excludeBodyFlags = BodyFlags.CommonFocusRayCastExcludeFlags)
    {
      return this.RayCastForClosestEntityOrTerrain(sourcePoint, targetPoint, out collisionDistance, out Vec3 _, rayThickness, excludeBodyFlags);
    }

    public void ImportNavigationMeshPrefab(string navMeshPrefabName, int navMeshGroupShift) => EngineApplicationInterface.IScene.LoadNavMeshPrefab(this.Pointer, navMeshPrefabName, navMeshGroupShift);

    public void SetAbilityOfFacesWithId(int faceGroupId, bool isEnabled) => EngineApplicationInterface.IScene.SetAbilityOfFacesWithId(this.Pointer, faceGroupId, isEnabled);

    public void SwapFaceConnectionsWithID(
      int hubFaceGroupID,
      int toBeSeparatedFaceGroupId,
      int toBeMergedFaceGroupId)
    {
      EngineApplicationInterface.IScene.SwapFaceConnectionsWithId(this.Pointer, hubFaceGroupID, toBeSeparatedFaceGroupId, toBeMergedFaceGroupId);
    }

    public void MergeFacesWithId(int faceGroupId0, int faceGroupId1, int newFaceGroupId) => EngineApplicationInterface.IScene.MergeFacesWithId(this.Pointer, faceGroupId0, faceGroupId1, newFaceGroupId);

    public void SeparateFacesWithId(int faceGroupId0, int faceGroupId1) => EngineApplicationInterface.IScene.SeparateFacesWithId(this.Pointer, faceGroupId0, faceGroupId1);

    public bool IsAnyFaceWithId(int faceGroupId) => EngineApplicationInterface.IScene.IsAnyFaceWithId(this.Pointer, faceGroupId);

    public bool GetNavigationMeshForPosition(ref Vec3 position) => this.GetNavigationMeshForPosition(ref position, out int _);

    public bool GetNavigationMeshForPosition(
      ref Vec3 position,
      out int faceGroupId,
      float heightDifferenceLimit = 1.5f)
    {
      faceGroupId = int.MinValue;
      return EngineApplicationInterface.IScene.GetNavigationMeshFaceForPosition(this.Pointer, ref position, ref faceGroupId, heightDifferenceLimit);
    }

    public bool DoesPathExistBetweenPositions(WorldPosition position, WorldPosition destination) => EngineApplicationInterface.IScene.DoesPathExistBetweenPositions(this.Pointer, position, destination);

    public void EnsurePostfxSystem() => EngineApplicationInterface.IScene.EnsurePostfxSystem(this.Pointer);

    public void SetBloom(bool mode) => EngineApplicationInterface.IScene.SetBloom(this.Pointer, mode);

    public void SetDofMode(bool mode) => EngineApplicationInterface.IScene.SetDofMode(this.Pointer, mode);

    public void SetOcclusionMode(bool mode) => EngineApplicationInterface.IScene.SetOcclusionMode(this.Pointer, mode);

    public void SetExternalInjectionTexture(Texture texture) => EngineApplicationInterface.IScene.SetExternalInjectionTexture(this.Pointer, texture.Pointer);

    public void SetSunshaftMode(bool mode) => EngineApplicationInterface.IScene.SetSunshaftMode(this.Pointer, mode);

    public Vec3 GetSunDirection() => EngineApplicationInterface.IScene.GetSunDirection(this.Pointer);

    public float GetNorthAngle() => EngineApplicationInterface.IScene.GetNorthAngle(this.Pointer);

    public bool GetTerrainMinMaxHeight(out float minHeight, out float maxHeight)
    {
      minHeight = 0.0f;
      maxHeight = 0.0f;
      return EngineApplicationInterface.IScene.GetTerrainMinMaxHeight(this, ref minHeight, ref maxHeight);
    }

    public void GetPhysicsMinMax(ref Vec3 min_max) => EngineApplicationInterface.IScene.GetPhysicsMinMax(this.Pointer, ref min_max);

    public bool IsEditorScene() => EngineApplicationInterface.IScene.IsEditorScene(this.Pointer);

    public void SetMotionBlurMode(bool mode) => EngineApplicationInterface.IScene.SetMotionBlurMode(this.Pointer, mode);

    public void SetAntialiasingMode(bool mode) => EngineApplicationInterface.IScene.SetAntialiasingMode(this.Pointer, mode);

    public void SetDLSSMode(bool mode) => EngineApplicationInterface.IScene.SetDLSSMode(this.Pointer, mode);

    public IEnumerable<GameEntity> FindEntitiesWithTag(string tag) => GameEntity.GetEntitiesWithTag(this, tag);

    public GameEntity FindEntityWithTag(string tag) => GameEntity.GetFirstEntityWithTag(this, tag);

    public GameEntity FindEntityWithName(string name) => GameEntity.GetFirstEntityWithName(this, name);

    public IEnumerable<GameEntity> FindEntitiesWithTagExpression(
      string expression)
    {
      return GameEntity.GetEntitiesWithTagExpression(this, expression);
    }

    public Path GetPathWithName(string name) => EngineApplicationInterface.IScene.GetPathWithName(this.Pointer, name);

    public void DeletePathWithName(string name) => EngineApplicationInterface.IScene.DeletePathWithName(this.Pointer, name);

    public void AddPath(string name) => EngineApplicationInterface.IScene.AddPath(this.Pointer, name);

    public void AddPathPoint(string name, MatrixFrame frame) => EngineApplicationInterface.IScene.AddPathPoint(this.Pointer, name, ref frame);

    public void GetBoundingBox(out Vec3 min, out Vec3 max)
    {
      min = Vec3.Invalid;
      max = Vec3.Invalid;
      EngineApplicationInterface.IScene.GetBoundingBox(this.Pointer, ref min, ref max);
    }

    public bool SlowMotionMode
    {
      get => EngineApplicationInterface.IScene.GetSlowMotionMode(this.Pointer);
      set => EngineApplicationInterface.IScene.SetSlowMotionMode(this.Pointer, value);
    }

    public void SetName(string name) => EngineApplicationInterface.IScene.SetName(this.Pointer, name);

    public string GetName() => EngineApplicationInterface.IScene.GetName(this.Pointer);

    public float SlowMotionFactor
    {
      get => EngineApplicationInterface.IScene.GetSlowMotionFactor(this.Pointer);
      set => EngineApplicationInterface.IScene.SetSlowMotionFactor(this.Pointer, value);
    }

    public void SetOwnerThread() => EngineApplicationInterface.IScene.SetOwnerThread(this.Pointer);

    public Path[] GetPathsWithNamePrefix(string prefix)
    {
      int pathsWithNamePrefix = EngineApplicationInterface.IScene.GetNumberOfPathsWithNamePrefix(this.Pointer, prefix);
      UIntPtr[] points = new UIntPtr[pathsWithNamePrefix];
      EngineApplicationInterface.IScene.GetPathsWithNamePrefix(this.Pointer, points, prefix);
      Path[] pathArray = new Path[pathsWithNamePrefix];
      for (int index = 0; index < pathsWithNamePrefix; ++index)
      {
        UIntPtr pointer = points[index];
        pathArray[index] = new Path(pointer);
      }
      return pathArray;
    }

    public void SetUseConstantTime(bool value) => EngineApplicationInterface.IScene.SetUseConstantTime(this.Pointer, value);

    public bool CheckPointCanSeePoint(Vec3 source, Vec3 target, float? distanceToCheck = null)
    {
      if (!distanceToCheck.HasValue)
        distanceToCheck = new float?(source.Distance(target));
      return EngineApplicationInterface.IScene.CheckPointCanSeePoint(this.Pointer, source, target, distanceToCheck.Value);
    }

    public void SetPlaySoundEventsAfterReadyToRender(bool value) => EngineApplicationInterface.IScene.SetPlaySoundEventsAfterReadyToRender(this.Pointer, value);

    public void DisableStaticShadows(bool value) => EngineApplicationInterface.IScene.DisableStaticShadows(this.Pointer, value);

    public Mesh GetSkyboxMesh() => EngineApplicationInterface.IScene.GetSkyboxMesh(this.Pointer);

    public void SetAtmosphereWithName(string name) => EngineApplicationInterface.IScene.SetAtmosphereWithName(this.Pointer, name);

    public void FillEntityWithHardBorderPhysicsBarrier(GameEntity entity) => EngineApplicationInterface.IScene.FillEntityWithHardBorderPhysicsBarrier(this.Pointer, entity.Pointer);

    public void ClearDecals() => EngineApplicationInterface.IScene.ClearDecals(this.Pointer);
  }
}
