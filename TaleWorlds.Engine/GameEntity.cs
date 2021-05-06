// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.GameEntity
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
  [EngineClass("rglEntity")]
  public sealed class GameEntity : NativeObject
  {
    public Scene Scene => EngineApplicationInterface.IGameEntity.GetScene(this.Pointer);

    internal GameEntity(UIntPtr pointer)
      : base(pointer)
    {
    }

    public override string ToString() => this.Pointer.ToString();

    public void ClearEntityComponents(bool resetAll, bool removeScripts, bool deleteChildEntities) => EngineApplicationInterface.IGameEntity.ClearEntityComponents(this.Pointer, resetAll, removeScripts, deleteChildEntities);

    public void ClearComponents() => EngineApplicationInterface.IGameEntity.ClearComponents(this.Pointer);

    public void ClearOnlyOwnComponents() => EngineApplicationInterface.IGameEntity.ClearOnlyOwnComponents(this.Pointer);

    public void RemovePhysics(bool clearingTheScene = false) => EngineApplicationInterface.IGameEntity.RemovePhysics(this.Pointer, clearingTheScene);

    public bool GetPhysicsState() => EngineApplicationInterface.IGameEntity.GetPhysicsState(this.Pointer);

    public bool HasPhysicsDefinitionWithoutFlags(int excludeFlags) => EngineApplicationInterface.IGameEntity.HasPhysicsDefinition(this.Pointer, excludeFlags);

    public void SetPhysicsState(bool isEnabled, bool setChildren) => EngineApplicationInterface.IGameEntity.SetPhysicsState(this.Pointer, isEnabled, setChildren);

    public void RemoveEnginePhysics() => EngineApplicationInterface.IGameEntity.RemoveEnginePhysics(this.Pointer);

    public bool IsEngineBodySleeping() => EngineApplicationInterface.IGameEntity.IsEngineBodySleeping(this.Pointer);

    public bool IsDynamicBodyStationary() => EngineApplicationInterface.IGameEntity.IsDynamicBodyStationary(this.Pointer);

    public bool CheckResources(bool addToQueue) => EngineApplicationInterface.IGameEntity.CheckResources(this.Pointer, addToQueue);

    public void SetMobility(GameEntity.Mobility mobility) => EngineApplicationInterface.IGameEntity.SetMobility(this.Pointer, (int) mobility);

    public void AddMesh(Mesh mesh, bool recomputeBoundingBox = true) => EngineApplicationInterface.IGameEntity.AddMesh(this.Pointer, mesh.Pointer, recomputeBoundingBox);

    public void AddMultiMeshToSkeleton(MetaMesh metaMesh) => EngineApplicationInterface.IGameEntity.AddMultiMeshToSkeleton(this.Pointer, metaMesh.Pointer);

    public void AddMultiMeshToSkeletonBone(MetaMesh metaMesh, sbyte boneIndex) => EngineApplicationInterface.IGameEntity.AddMultiMeshToSkeletonBone(this.Pointer, metaMesh.Pointer, boneIndex);

    public IEnumerable<Mesh> GetAllMeshesWithTag(string tag)
    {
      GameEntity gameEntity1 = this;
      List<GameEntity> children = new List<GameEntity>();
      gameEntity1.GetChildrenRecursive(ref children);
      children.Add(gameEntity1);
      foreach (GameEntity gameEntity2 in children)
      {
        GameEntity entity = gameEntity2;
        for (int i = 0; i < entity.MultiMeshComponentCount; ++i)
        {
          MetaMesh multiMesh = entity.GetMetaMesh(i);
          for (int j = 0; j < multiMesh.MeshCount; ++j)
          {
            Mesh meshAtIndex = multiMesh.GetMeshAtIndex(j);
            if (meshAtIndex.HasTag(tag))
              yield return meshAtIndex;
          }
          multiMesh = (MetaMesh) null;
        }
        entity = (GameEntity) null;
      }
    }

    public void SetColor(uint color1, uint color2, string meshTag)
    {
      foreach (Mesh mesh in this.GetAllMeshesWithTag(meshTag))
      {
        mesh.Color = color1;
        mesh.Color2 = color2;
      }
    }

    public uint GetFactorColor() => EngineApplicationInterface.IGameEntity.GetFactorColor(this.Pointer);

    public void SetFactorColor(uint color) => EngineApplicationInterface.IGameEntity.SetFactorColor(this.Pointer, color);

    public void SetAsReplayEntity() => EngineApplicationInterface.IGameEntity.SetAsReplayEntity(this.Pointer);

    public void SetClothMaxDistanceMultiplier(float multiplier) => EngineApplicationInterface.IGameEntity.SetClothMaxDistanceMultiplier(this.Pointer, multiplier);

    public void RemoveMultiMeshFromSkeleton(MetaMesh metaMesh) => EngineApplicationInterface.IGameEntity.RemoveMultiMeshFromSkeleton(this.Pointer, metaMesh.Pointer);

    public void RemoveMultiMeshFromSkeletonBone(MetaMesh metaMesh, sbyte boneIndex) => EngineApplicationInterface.IGameEntity.RemoveMultiMeshFromSkeletonBone(this.Pointer, metaMesh.Pointer, boneIndex);

    public bool RemoveComponentWithMesh(Mesh mesh) => EngineApplicationInterface.IGameEntity.RemoveComponentWithMesh(this.Pointer, mesh.Pointer);

    public void AddComponent(GameEntityComponent component) => EngineApplicationInterface.IGameEntity.AddComponent(this.Pointer, component.Pointer);

    public bool HasComponent(GameEntityComponent component) => EngineApplicationInterface.IGameEntity.HasComponent(this.Pointer, component.Pointer);

    public bool RemoveComponent(GameEntityComponent component) => EngineApplicationInterface.IGameEntity.RemoveComponent(this.Pointer, component.Pointer);

    public void AddSphereAsBody(ref Vec3 center, float radius, uint bodyFlags) => EngineApplicationInterface.IGameEntity.AddSphereAsBody(this.Pointer, center, radius, bodyFlags);

    public string GetGuid() => EngineApplicationInterface.IGameEntity.GetGuid(this.Pointer);

    public bool IsGuidValid() => EngineApplicationInterface.IGameEntity.IsGuidValid(this.Pointer);

    public float GetLodLevelForDistanceSq(float distSq) => EngineApplicationInterface.IGameEntity.GetLodLevelForDistanceSq(this.Pointer, distSq);

    public void GetQuickBoneEntitialFrame(sbyte index, ref MatrixFrame frame) => EngineApplicationInterface.IGameEntity.GetQuickBoneEntitialFrame(this.Pointer, index, ref frame);

    public void UpdateVisibilityMask() => EngineApplicationInterface.IGameEntity.UpdateVisibilityMask(this.Pointer);

    public static GameEntity CreateEmpty(Scene scene, bool isModifiableFromEditor = true) => EngineApplicationInterface.IGameEntity.CreateEmpty(scene.Pointer, isModifiableFromEditor, (UIntPtr) 0UL);

    public static GameEntity CreateEmptyDynamic(Scene scene, bool isModifiableFromEditor = true)
    {
      GameEntity empty = GameEntity.CreateEmpty(scene, isModifiableFromEditor);
      empty.SetMobility(GameEntity.Mobility.dynamic);
      return empty;
    }

    public static GameEntity CreateEmptyWithoutScene() => EngineApplicationInterface.IGameEntity.CreateEmptyWithoutScene();

    public static GameEntity CopyFrom(Scene scene, GameEntity entity) => EngineApplicationInterface.IGameEntity.CreateEmpty(scene.Pointer, false, entity.Pointer);

    public static GameEntity Instantiate(
      Scene scene,
      string prefabName,
      bool callScriptCallbacks)
    {
      return EngineApplicationInterface.IGameEntity.CreateFromPrefab(scene.Pointer, prefabName, callScriptCallbacks);
    }

    public void CallScriptCallbacks() => EngineApplicationInterface.IGameEntity.CallScriptCallbacks(this.Pointer);

    public static GameEntity Instantiate(
      Scene scene,
      string prefabName,
      MatrixFrame frame)
    {
      return EngineApplicationInterface.IGameEntity.CreateFromPrefabWithInitialFrame(scene.Pointer, prefabName, ref frame);
    }

    private int ScriptCount => EngineApplicationInterface.IGameEntity.GetScriptComponentCount(this.Pointer);

    public bool IsGhostObject() => EngineApplicationInterface.IGameEntity.IsGhostObject(this.Pointer);

    public void CreateAndAddScriptComponent(string name) => EngineApplicationInterface.IGameEntity.CreateAndAddScriptComponent(this.Pointer, name);

    public static bool PrefabExists(string name) => EngineApplicationInterface.IGameEntity.PrefabExists(name);

    public void RemoveScriptComponent(UIntPtr scriptComponent, int removeReason) => EngineApplicationInterface.IGameEntity.RemoveScriptComponent(this.Pointer, scriptComponent, removeReason);

    public void SetEntityEnvMapVisibility(bool value) => EngineApplicationInterface.IGameEntity.SetEntityEnvMapVisibility(this.Pointer, value);

    internal ScriptComponentBehaviour GetScriptAtIndex(int index) => EngineApplicationInterface.IGameEntity.GetScriptComponentAtIndex(this.Pointer, index);

    public bool HasScriptComponent(string scName) => EngineApplicationInterface.IGameEntity.HasScriptComponent(this.Pointer, scName);

    public IEnumerable<ScriptComponentBehaviour> GetScriptComponents()
    {
      int count = this.ScriptCount;
      for (int i = 0; i < count; ++i)
        yield return this.GetScriptAtIndex(i);
    }

    public IEnumerable<T> GetScriptComponents<T>() where T : ScriptComponentBehaviour
    {
      int count = this.ScriptCount;
      for (int i = 0; i < count; ++i)
      {
        if (this.GetScriptAtIndex(i) is T scriptAtIndex1)
          yield return scriptAtIndex1;
      }
    }

    public IEnumerable<ScriptComponentBehaviour> GetScriptComponents(
      Func<ScriptComponentBehaviour, bool> predicate)
    {
      return this.GetScriptComponents().Where<ScriptComponentBehaviour>(predicate);
    }

    public IEnumerable<ScriptComponentBehaviour> GetScriptComponents(
      Func<ScriptComponentBehaviour, int, bool> predicate)
    {
      return this.GetScriptComponents().Where<ScriptComponentBehaviour>(predicate);
    }

    public IEnumerable<T> GetScriptComponents<T>(Func<T, bool> predicate) where T : ScriptComponentBehaviour => this.GetScriptComponents<T>().Where<T>(predicate);

    public IEnumerable<T> GetScriptComponents<T>(Func<T, int, bool> predicate) where T : ScriptComponentBehaviour => this.GetScriptComponents<T>().Where<T>(predicate);

    public bool HasScriptOfType(System.Type t) => this.GetScriptComponents().Any<ScriptComponentBehaviour>((Func<ScriptComponentBehaviour, bool>) (sc => sc.GetType() == t));

    public bool HasScriptOfType<T>() where T : ScriptComponentBehaviour => this.GetScriptComponents<T>().Any<T>();

    public ScriptComponentBehaviour GetFirstScriptOfType(System.Type t) => this.GetScriptComponents().FirstOrDefault<ScriptComponentBehaviour>((Func<ScriptComponentBehaviour, bool>) (sc => sc.GetType() == t));

    public T GetFirstScriptOfTypeInFamily<T>() where T : ScriptComponentBehaviour
    {
      T firstScriptOfType = this.GetFirstScriptOfType<T>();
      for (GameEntity gameEntity = this; (object) firstScriptOfType == null && (NativeObject) gameEntity.Parent != (NativeObject) null; firstScriptOfType = gameEntity.GetFirstScriptOfType<T>())
        gameEntity = gameEntity.Parent;
      return firstScriptOfType;
    }

    public T GetFirstScriptOfType<T>() where T : ScriptComponentBehaviour
    {
      int scriptCount = this.ScriptCount;
      for (int index = 0; index < scriptCount; ++index)
      {
        if (this.GetScriptAtIndex(index) is T scriptAtIndex1)
          return scriptAtIndex1;
      }
      return default (T);
    }

    internal static GameEntity GetFirstEntityWithName(Scene scene, string entityName) => EngineApplicationInterface.IGameEntity.FindWithName(scene.Pointer, entityName);

    public string Name
    {
      get => EngineApplicationInterface.IGameEntity.GetName(this.Pointer);
      set => EngineApplicationInterface.IGameEntity.SetName(this.Pointer, value);
    }

    public void SetAlpha(float alpha) => EngineApplicationInterface.IGameEntity.SetAlpha(this.Pointer, alpha);

    public void SetVisibilityExcludeParents(bool visible) => EngineApplicationInterface.IGameEntity.SetVisibilityExcludeParents(this.Pointer, visible);

    public void SetReadyToRender(bool ready) => EngineApplicationInterface.IGameEntity.SetReadyToRender(this.Pointer, ready);

    public bool GetVisibilityExcludeParents() => EngineApplicationInterface.IGameEntity.GetVisibilityExcludeParents(this.Pointer);

    public bool IsVisibleIncludeParents() => EngineApplicationInterface.IGameEntity.IsVisibleIncludeParents(this.Pointer);

    public uint GetVisibilityLevelMaskIncludingParents() => EngineApplicationInterface.IGameEntity.GetVisibilityLevelMaskIncludingParents(this.Pointer);

    public bool GetEditModeLevelVisibility() => EngineApplicationInterface.IGameEntity.GetEditModeLevelVisibility(this.Pointer);

    public GameEntity GetMostParent()
    {
      GameEntity gameEntity = this;
      while ((NativeObject) gameEntity.Parent != (NativeObject) null)
        gameEntity = gameEntity.Parent;
      return gameEntity;
    }

    public void Remove(int removeReason) => EngineApplicationInterface.IGameEntity.Remove(this.Pointer, removeReason);

    internal static GameEntity GetFirstEntityWithTag(Scene scene, string tag) => EngineApplicationInterface.IGameEntity.GetFirstEntityWithTag(scene.Pointer, tag);

    internal static GameEntity GetNextEntityWithTag(
      Scene scene,
      GameEntity startEntity,
      string tag)
    {
      return (NativeObject) startEntity == (NativeObject) null ? GameEntity.GetFirstEntityWithTag(scene, tag) : EngineApplicationInterface.IGameEntity.GetNextEntityWithTag(startEntity.Pointer, tag);
    }

    internal static GameEntity GetFirstEntityWithTagExpression(
      Scene scene,
      string tagExpression)
    {
      return EngineApplicationInterface.IGameEntity.GetFirstEntityWithTagExpression(scene.Pointer, tagExpression);
    }

    internal static GameEntity GetNextEntityWithTagExpression(
      Scene scene,
      GameEntity startEntity,
      string tagExpression)
    {
      return (NativeObject) startEntity == (NativeObject) null ? GameEntity.GetFirstEntityWithTagExpression(scene, tagExpression) : EngineApplicationInterface.IGameEntity.GetNextEntityWithTagExpression(startEntity.Pointer, tagExpression);
    }

    public static IEnumerable<GameEntity> GetAllPrefabs()
    {
      GameEntity entity = (GameEntity) null;
      while (true)
      {
        entity = GameEntity.GetNextPrefab(entity);
        if (!((NativeObject) entity == (NativeObject) null))
          yield return entity;
        else
          break;
      }
    }

    internal static GameEntity GetNextPrefab(GameEntity current) => (NativeObject) current == (NativeObject) null ? EngineApplicationInterface.IGameEntity.GetNextPrefab(new UIntPtr(0U)) : EngineApplicationInterface.IGameEntity.GetNextPrefab(current.Pointer);

    public static GameEntity CopyFromPrefab(GameEntity prefab) => (NativeObject) prefab != (NativeObject) null ? EngineApplicationInterface.IGameEntity.CopyFromPrefab(prefab.Pointer) : (GameEntity) null;

    public void SetUpgradeLevelMask(GameEntity.UpgradeLevelMask mask) => EngineApplicationInterface.IGameEntity.SetUpgradeLevelMask(this.Pointer, (uint) mask);

    public GameEntity.UpgradeLevelMask GetUpgradeLevelMask() => (GameEntity.UpgradeLevelMask) EngineApplicationInterface.IGameEntity.GetUpgradeLevelMask(this.Pointer);

    public string GetOldPrefabName() => EngineApplicationInterface.IGameEntity.GetOldPrefabName(this.Pointer);

    public string GetPrefabName() => EngineApplicationInterface.IGameEntity.GetPrefabName(this.Pointer);

    public void CopyScriptComponentFromAnotherEntity(GameEntity otherEntity, string scriptName) => EngineApplicationInterface.IGameEntity.CopyScriptComponentFromAnotherEntity(this.Pointer, otherEntity.Pointer, scriptName);

    internal static IEnumerable<GameEntity> GetEntitiesWithTag(
      Scene scene,
      string tag)
    {
      for (GameEntity entity = GameEntity.GetFirstEntityWithTag(scene, tag); (NativeObject) entity != (NativeObject) null; entity = GameEntity.GetNextEntityWithTag(scene, entity, tag))
        yield return entity;
    }

    internal static IEnumerable<GameEntity> GetEntitiesWithTagExpression(
      Scene scene,
      string tagExpression)
    {
      for (GameEntity entity = GameEntity.GetFirstEntityWithTagExpression(scene, tagExpression); (NativeObject) entity != (NativeObject) null; entity = GameEntity.GetNextEntityWithTagExpression(scene, entity, tagExpression))
        yield return entity;
    }

    public void SetFrame(ref MatrixFrame frame) => EngineApplicationInterface.IGameEntity.SetFrame(this.Pointer, ref frame);

    public void SetClothComponentKeepState(MetaMesh metaMesh, bool state) => EngineApplicationInterface.IGameEntity.SetClothComponentKeepState(this.Pointer, metaMesh.Pointer, state);

    public void SetClothComponentKeepStateOfAllMeshes(bool state) => EngineApplicationInterface.IGameEntity.SetClothComponentKeepStateOfAllMeshes(this.Pointer, state);

    public void SetPreviousFrameInvalid() => EngineApplicationInterface.IGameEntity.SetPreviousFrameInvalid(this.Pointer);

    public MatrixFrame GetFrame()
    {
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.IGameEntity.GetFrame(this.Pointer, ref outFrame);
      return outFrame;
    }

    public void GetFrame(out MatrixFrame frame) => frame = this.GetFrame();

    public void AddMeshToBone(sbyte boneIndex, Mesh mesh)
    {
      if (!((NativeObject) this.Skeleton != (NativeObject) null))
        return;
      this.Skeleton.AddMeshToBone(mesh.Pointer, boneIndex);
    }

    public void UpdateTriadFrameForEditor() => EngineApplicationInterface.IGameEntity.UpdateTriadFrameForEditor(this.Pointer);

    public void UpdateTriadFrameForEditorForAllChildren()
    {
      this.UpdateTriadFrameForEditor();
      List<GameEntity> children = new List<GameEntity>();
      this.GetChildrenRecursive(ref children);
      foreach (GameEntity gameEntity in children)
        EngineApplicationInterface.IGameEntity.UpdateTriadFrameForEditor(gameEntity.Pointer);
    }

    public MatrixFrame GetGlobalFrame()
    {
      MatrixFrame outFrame = new MatrixFrame();
      EngineApplicationInterface.IGameEntity.GetGlobalFrame(this.Pointer, ref outFrame);
      return outFrame;
    }

    public void SetGlobalFrame(MatrixFrame frame) => EngineApplicationInterface.IGameEntity.SetGlobalFrame(this.Pointer, ref frame);

    public void SetLocalPosition(Vec3 position) => EngineApplicationInterface.IGameEntity.SetLocalPosition(this.Pointer, position);

    public EntityFlags EntityFlags
    {
      get => (EntityFlags) EngineApplicationInterface.IGameEntity.GetEntityFlags(this.Pointer);
      set => EngineApplicationInterface.IGameEntity.SetEntityFlags(this.Pointer, (uint) value);
    }

    public EntityVisibilityFlags EntityVisibilityFlags
    {
      get => (EntityVisibilityFlags) EngineApplicationInterface.IGameEntity.GetEntityVisibilityFlags(this.Pointer);
      set => EngineApplicationInterface.IGameEntity.SetEntityVisibilityFlags(this.Pointer, (uint) value);
    }

    public BodyFlags BodyFlag
    {
      get => (BodyFlags) EngineApplicationInterface.IGameEntity.GetBodyFlags(this.Pointer);
      set => EngineApplicationInterface.IGameEntity.SetBodyFlags(this.Pointer, (uint) value);
    }

    public BodyFlags PhysicsDescBodyFlag => (BodyFlags) EngineApplicationInterface.IGameEntity.GetPhysicsDescBodyFlags(this.Pointer);

    public float Mass => EngineApplicationInterface.IGameEntity.GetMass(this.Pointer);

    public Vec3 CenterOfMass => EngineApplicationInterface.IGameEntity.GetCenterOfMass(this.Pointer);

    public void SetBodyFlags(BodyFlags bodyFlags) => this.BodyFlag = bodyFlags;

    public void AddBodyFlags(BodyFlags bodyFlags, bool applyToChildren = true)
    {
      this.BodyFlag |= bodyFlags;
      if (!applyToChildren)
        return;
      foreach (GameEntity child in this.GetChildren())
        child.AddBodyFlags(bodyFlags);
    }

    public void RemoveBodyFlags(BodyFlags bodyFlags, bool applyToChildren = true)
    {
      this.BodyFlag &= ~bodyFlags;
      if (!applyToChildren)
        return;
      foreach (GameEntity child in this.GetChildren())
        child.RemoveBodyFlags(bodyFlags);
    }

    public Vec3 GetGlobalScale() => EngineApplicationInterface.IGameEntity.GetGlobalScale(this);

    public PhysicsShape GetBodyShape() => EngineApplicationInterface.IGameEntity.GetBodyShape(this);

    public void SetBodyShape(PhysicsShape shape) => EngineApplicationInterface.IGameEntity.SetBodyShape(this.Pointer, (NativeObject) shape == (NativeObject) null ? (UIntPtr) 0UL : shape.Pointer);

    public void AddPhysics(
      float mass,
      Vec3 localCenterOfMass,
      PhysicsShape body,
      Vec3 initialVelocity,
      Vec3 angularVelocity,
      PhysicsMaterial physicsMaterial,
      bool isStatic,
      int collisionGroupID)
    {
      EngineApplicationInterface.IGameEntity.AddPhysics(this.Pointer, (NativeObject) body != (NativeObject) null ? body.Pointer : UIntPtr.Zero, mass, ref localCenterOfMass, ref initialVelocity, ref angularVelocity, physicsMaterial.Index, isStatic, collisionGroupID);
      this.BodyFlag |= BodyFlags.Moveable;
    }

    public void ApplyImpulseToDynamicBody(Vec3 position, Vec3 impulse) => EngineApplicationInterface.IGameEntity.ApplyImpulseToDynamicBody(this.Pointer, ref position, ref impulse);

    public void DisableDynamicBodySimulation() => EngineApplicationInterface.IGameEntity.DisableDynamicBodySimulation(this.Pointer);

    public void EnableDynamicBody() => EngineApplicationInterface.IGameEntity.EnableDynamicBody(this.Pointer);

    public Vec3 GlobalPosition => this.GetGlobalFrame().origin;

    public void SetAnimationSoundActivation(bool activate)
    {
      if ((NativeObject) this.Skeleton != (NativeObject) null)
        EngineApplicationInterface.IGameEntity.SetAnimationSoundActivation(this.Pointer, activate);
      foreach (GameEntity child in this.GetChildren())
        child.SetAnimationSoundActivation(activate);
    }

    public void SetBodyFlagsRecursive(BodyFlags bodyFlags) => EngineApplicationInterface.IGameEntity.SetBodyFlagsRecursive(this.Pointer, (uint) bodyFlags);

    public void CopyComponentsToSkeleton() => EngineApplicationInterface.IGameEntity.CopyComponentsToSkeleton(this.Pointer);

    public void ActivateRagdoll()
    {
      Skeleton skeleton = this.Skeleton;
      if (!((NativeObject) skeleton != (NativeObject) null))
        return;
      skeleton.ActivateRagdoll();
    }

    public void PauseSkeletonAnimation()
    {
      if (!((NativeObject) this.Skeleton != (NativeObject) null))
        return;
      this.Skeleton.Freeze(true);
    }

    public void ResumeSkeletonAnimation()
    {
      if (!((NativeObject) this.Skeleton != (NativeObject) null))
        return;
      this.Skeleton.Freeze(false);
    }

    public bool IsSkeletonAnimationPaused() => (NativeObject) this.Skeleton != (NativeObject) null && this.Skeleton.IsFrozen();

    public byte GetBoneCount() => (NativeObject) this.Skeleton != (NativeObject) null ? this.Skeleton.GetBoneCount() : (byte) 0;

    public MatrixFrame GetBoneEntitialFrameWithIndex(byte boneIndex) => (NativeObject) this.Skeleton != (NativeObject) null ? this.Skeleton.GetBoneEntitialFrameWithIndex(boneIndex) : MatrixFrame.Identity;

    public MatrixFrame GetBoneEntitialFrameWithName(string boneName) => (NativeObject) this.Skeleton != (NativeObject) null ? this.Skeleton.GetBoneEntitialFrameWithName(boneName) : MatrixFrame.Identity;

    public string[] Tags => EngineApplicationInterface.IGameEntity.GetTags(this.Pointer).Split(' ');

    public void AddTag(string tag) => EngineApplicationInterface.IGameEntity.AddTag(this.Pointer, tag);

    public void RemoveTag(string tag) => EngineApplicationInterface.IGameEntity.RemoveTag(this.Pointer, tag);

    public bool HasTag(string tag) => EngineApplicationInterface.IGameEntity.HasTag(this.Pointer, tag);

    public void AddChild(GameEntity mbGameEntity, bool autoLocalizeFrame = false) => EngineApplicationInterface.IGameEntity.AddChild(this.Pointer, mbGameEntity.Pointer, autoLocalizeFrame);

    public void RemoveChild(
      GameEntity childEntity,
      bool keepPhysics,
      bool keepScenePointer,
      bool callScriptCallbacks,
      int removeReason)
    {
      EngineApplicationInterface.IGameEntity.RemoveChild(this.Pointer, childEntity.Pointer, keepPhysics, keepScenePointer, callScriptCallbacks, removeReason);
    }

    public void BreakPrefab() => EngineApplicationInterface.IGameEntity.BreakPrefab(this.Pointer);

    public int ChildCount => EngineApplicationInterface.IGameEntity.GetChildCount(this.Pointer);

    public GameEntity GetChild(int index) => EngineApplicationInterface.IGameEntity.GetChild(this.Pointer, index);

    public GameEntity Parent => EngineApplicationInterface.IGameEntity.GetParent(this.Pointer);

    public bool HasComplexAnimTree() => EngineApplicationInterface.IGameEntity.HasComplexAnimTree(this.Pointer);

    public GameEntity Root
    {
      get
      {
        GameEntity gameEntity = this;
        while ((NativeObject) gameEntity.Parent != (NativeObject) null)
          gameEntity = gameEntity.Parent;
        return gameEntity;
      }
    }

    public void AddMultiMesh(MetaMesh metaMesh, bool updateVisMask = true) => EngineApplicationInterface.IGameEntity.AddMultiMesh(this.Pointer, metaMesh.Pointer, updateVisMask);

    public bool RemoveMultiMesh(MetaMesh metaMesh) => EngineApplicationInterface.IGameEntity.RemoveMultiMesh(this.Pointer, metaMesh.Pointer);

    public int MultiMeshComponentCount => EngineApplicationInterface.IGameEntity.GetComponentCount(this.Pointer, GameEntity.ComponentType.MetaMesh);

    public int GetComponentCount(GameEntity.ComponentType componentType) => EngineApplicationInterface.IGameEntity.GetComponentCount(this.Pointer, componentType);

    public void AddAllMeshesOfGameEntity(GameEntity gameEntity) => EngineApplicationInterface.IGameEntity.AddAllMeshesOfGameEntity(this.Pointer, gameEntity.Pointer);

    public GameEntityComponent GetComponentAtIndex(
      int index,
      GameEntity.ComponentType componentType)
    {
      return EngineApplicationInterface.IGameEntity.GetComponentAtIndex(this.Pointer, componentType, index);
    }

    public MetaMesh GetMetaMesh(int metaMeshIndex) => (MetaMesh) EngineApplicationInterface.IGameEntity.GetComponentAtIndex(this.Pointer, GameEntity.ComponentType.MetaMesh, metaMeshIndex);

    public void SetVectorArgument(
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3)
    {
      EngineApplicationInterface.IGameEntity.SetVectorArgument(this.Pointer, vectorArgument0, vectorArgument1, vectorArgument2, vectorArgument3);
    }

    public void SetMaterialForAllMeshes(Material material) => EngineApplicationInterface.IGameEntity.SetMaterialForAllMeshes(this.Pointer, material.Pointer);

    public bool AddLight(Light light) => EngineApplicationInterface.IGameEntity.AddLight(this.Pointer, light.Pointer);

    public Light GetLight() => EngineApplicationInterface.IGameEntity.GetLight(this.Pointer);

    public void AddParticleSystemComponent(string particleid) => EngineApplicationInterface.IGameEntity.AddParticleSystemComponent(this.Pointer, particleid);

    public void RemoveAllParticleSystems() => EngineApplicationInterface.IGameEntity.RemoveAllParticleSystems(this.Pointer);

    public bool CheckPointWithOrientedBoundingBox(Vec3 point) => EngineApplicationInterface.IGameEntity.CheckPointWithOrientedBoundingBox(this.Pointer, point);

    public void PauseParticleSystem(bool doChildren) => EngineApplicationInterface.IGameEntity.PauseParticleSystem(this.Pointer, doChildren);

    public void ResumeParticleSystem(bool doChildren) => EngineApplicationInterface.IGameEntity.ResumeParticleSystem(this.Pointer, doChildren);

    public void BurstEntityParticle(bool doChildren) => EngineApplicationInterface.IGameEntity.BurstEntityParticle(this.Pointer, doChildren);

    public void SetRuntimeEmissionRateMultiplier(float emissionRateMultiplier) => EngineApplicationInterface.IGameEntity.SetRuntimeEmissionRateMultiplier(this.Pointer, emissionRateMultiplier);

    public bool HasBody() => EngineApplicationInterface.IGameEntity.HasBody(this.Pointer);

    public Vec3 GetBoundingBoxMin() => EngineApplicationInterface.IGameEntity.GetBoundingBoxMin(this.Pointer);

    public Vec3 GetBoundingBoxMax() => EngineApplicationInterface.IGameEntity.GetBoundingBoxMax(this.Pointer);

    public bool HasFrameChanged => EngineApplicationInterface.IGameEntity.HasFrameChanged(this.Pointer);

    public Mesh GetFirstMesh() => EngineApplicationInterface.IGameEntity.GetFirstMesh(this.Pointer);

    public void SetContourColor(uint? color, bool alwaysVisible = true)
    {
      if (color.HasValue)
      {
        EngineApplicationInterface.IGameEntity.SetAsContourEntity(this.Pointer, color.Value);
        EngineApplicationInterface.IGameEntity.SetContourState(this.Pointer, alwaysVisible);
      }
      else
        EngineApplicationInterface.IGameEntity.DisableContour(this.Pointer);
    }

    public void SetExternalReferencesUsage(bool value) => EngineApplicationInterface.IGameEntity.SetExternalReferencesUsage(this.Pointer, value);

    public void SetMorphFrameOfComponents(float value) => EngineApplicationInterface.IGameEntity.SetMorphFrameOfComponents(this.Pointer, value);

    public void UpdateGlobalBounds() => EngineApplicationInterface.IGameEntity.UpdateGlobalBounds(this.Pointer);

    public void ValidateBoundingBox() => EngineApplicationInterface.IGameEntity.ValidateBoundingBox(this.Pointer);

    public void AddEditDataUserToAllMeshes(bool entity_components, bool skeleton_components) => EngineApplicationInterface.IGameEntity.AddEditDataUserToAllMeshes(this.Pointer, entity_components, skeleton_components);

    public void ReleaseEditDataUserToAllMeshes(bool entity_components, bool skeleton_components) => EngineApplicationInterface.IGameEntity.ReleaseEditDataUserToAllMeshes(this.Pointer, entity_components, skeleton_components);

    public void GetCameraParamsFromCameraScript(Camera cam, ref Vec3 dof_params) => EngineApplicationInterface.IGameEntity.GetCameraParamsFromCameraScript(this.Pointer, cam.Pointer, ref dof_params);

    public void GetMeshBendedFrame(MatrixFrame worldSpacePosition, ref MatrixFrame output) => EngineApplicationInterface.IGameEntity.GetMeshBendedPosition(this.Pointer, ref worldSpacePosition, ref output);

    public void SetAnimTreeChannelParameterForceUpdate(float phase, int channel_no) => EngineApplicationInterface.IGameEntity.SetAnimTreeChannelParameter(this.Pointer, phase, channel_no);

    public void RecomputeBoundingBox() => EngineApplicationInterface.IGameEntity.RecomputeBoundingBox(this);

    public void SetBoundingboxDirty() => EngineApplicationInterface.IGameEntity.SetBoundingboxDirty(this.Pointer);

    public Vec3 GlobalBoxMax => EngineApplicationInterface.IGameEntity.GetGlobalBoxMax(this.Pointer);

    public Vec3 GlobalBoxMin => EngineApplicationInterface.IGameEntity.GetGlobalBoxMin(this.Pointer);

    public float GetRadius() => EngineApplicationInterface.IGameEntity.GetRadius(this.Pointer);

    public void AddSphereAsBody(Vec3 sphere, float radius, BodyFlags bodyFlags) => EngineApplicationInterface.IGameEntity.AddSphereAsBody(this.Pointer, sphere, radius, (uint) bodyFlags);

    public void ChangeMetaMeshOrRemoveItIfNotExists(MetaMesh entityMetaMesh, MetaMesh newMetaMesh) => EngineApplicationInterface.IGameEntity.ChangeMetaMeshOrRemoveItIfNotExists(this.Pointer, (NativeObject) entityMetaMesh != (NativeObject) null ? entityMetaMesh.Pointer : UIntPtr.Zero, (NativeObject) newMetaMesh != (NativeObject) null ? newMetaMesh.Pointer : UIntPtr.Zero);

    public void AttachNavigationMeshFaces(
      int faceGroupId,
      bool isConnected,
      bool isBlocker = false,
      bool autoLocalize = false)
    {
      EngineApplicationInterface.IGameEntity.AttachNavigationMeshFaces(this.Pointer, faceGroupId, isConnected, isBlocker, autoLocalize);
    }

    public void RemoveSkeleton() => this.Skeleton = (Skeleton) null;

    public Skeleton Skeleton
    {
      get => EngineApplicationInterface.IGameEntity.GetSkeleton(this.Pointer);
      set => EngineApplicationInterface.IGameEntity.SetSkeleton(this.Pointer, value.Pointer);
    }

    public void RemoveAllChildren() => EngineApplicationInterface.IGameEntity.RemoveAllChildren(this.Pointer);

    public IEnumerable<GameEntity> GetChildren()
    {
      int count = this.ChildCount;
      for (int i = 0; i < count; ++i)
        yield return this.GetChild(i);
    }

    public IEnumerable<GameEntity> GetEntityAndChildren()
    {
      GameEntity gameEntity = this;
      yield return gameEntity;
      int count = gameEntity.ChildCount;
      for (int i = 0; i < count; ++i)
        yield return gameEntity.GetChild(i);
    }

    public void GetChildrenRecursive(ref List<GameEntity> children)
    {
      int childCount = this.ChildCount;
      for (int index = 0; index < childCount; ++index)
      {
        GameEntity child = this.GetChild(index);
        children.Add(child);
        child.GetChildrenRecursive(ref children);
      }
    }

    public bool IsSelectedOnEditor() => EngineApplicationInterface.IGameEntity.IsEntitySelectedOnEditor(this.Pointer);

    public void SelectEntityOnEditor() => EngineApplicationInterface.IGameEntity.SelectEntityOnEditor(this.Pointer);

    public void DeselectEntityOnEditor() => EngineApplicationInterface.IGameEntity.DeselectEntityOnEditor(this.Pointer);

    public void SetCullMode(MBMeshCullingMode cullMode) => EngineApplicationInterface.IGameEntity.SetCullMode(this.Pointer, cullMode);

    public enum ComponentType
    {
      MetaMesh,
      Light,
      CompositeComponent,
      ClothSimulator,
      ParticleSystemInstanced,
      CustomType1,
      Decal,
    }

    public enum Mobility
    {
      stationary,
      dynamic,
      dynamic_forced,
    }

    [Flags]
    public enum UpgradeLevelMask
    {
      None = 0,
      Level0 = 1,
      Level1 = 2,
      Level2 = 4,
      Level3 = 8,
      LevelAll = Level3 | Level2 | Level1 | Level0, // 0x0000000F
    }
  }
}
