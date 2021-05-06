// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.EngineApplicationInterface
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System.Collections.Generic;

namespace TaleWorlds.Engine
{
  internal class EngineApplicationInterface
  {
    internal static IPath IPath;
    internal static IShader IShader;
    internal static ITexture ITexture;
    internal static IMaterial IMaterial;
    internal static IMetaMesh IMetaMesh;
    internal static IDecal IDecal;
    internal static IClothSimulatorComponent IClothSimulatorComponent;
    internal static ICompositeComponent ICompositeComponent;
    internal static IPhysicsShape IPhysicsShape;
    internal static IBodyPart IBodyPart;
    internal static IParticleSystem IParticleSystem;
    internal static IMesh IMesh;
    internal static IMeshBuilder IMeshBuilder;
    internal static ICamera ICamera;
    internal static ISkeleton ISkeleton;
    internal static IGameEntity IGameEntity;
    internal static IGameEntityComponent IGameEntityComponent;
    internal static IScene IScene;
    internal static IScriptComponent IScriptComponent;
    internal static ILight ILight;
    internal static IPhysicsMaterial IPhysicsMaterial;
    internal static ISceneView ISceneView;
    internal static IView IView;
    internal static ITableauView ITableauView;
    internal static ITextureView ITextureView;
    internal static IVideoPlayerView IVideoPlayerView;
    internal static IThumbnailCreatorView IThumbnailCreatorView;
    internal static IDebug IDebug;
    internal static ITwoDimensionView ITwoDimensionView;
    internal static IUtil IUtil;
    internal static IEngineSizeChecker IEngineSizeChecker;
    internal static IInput IInput;
    internal static ITime ITime;
    internal static IScreen IScreen;
    internal static IMusic IMusic;
    internal static IImgui IImgui;
    internal static IMouseManager IMouseManager;
    internal static IHighlights IHighlights;
    internal static ISoundEvent ISoundEvent;
    internal static ISoundManager ISoundManager;
    internal static IConfig IConfig;
    internal static IManagedMeshEditOperations IManagedMeshEditOperations;
    private static Dictionary<string, object> _objects;

    private static T GetObject<T>() where T : class
    {
      object obj;
      return EngineApplicationInterface._objects.TryGetValue(typeof (T).FullName, out obj) ? obj as T : default (T);
    }

    internal static void SetObjects(Dictionary<string, object> objects)
    {
      EngineApplicationInterface._objects = objects;
      EngineApplicationInterface.IPath = EngineApplicationInterface.GetObject<IPath>();
      EngineApplicationInterface.IShader = EngineApplicationInterface.GetObject<IShader>();
      EngineApplicationInterface.ITexture = EngineApplicationInterface.GetObject<ITexture>();
      EngineApplicationInterface.IMaterial = EngineApplicationInterface.GetObject<IMaterial>();
      EngineApplicationInterface.IMetaMesh = EngineApplicationInterface.GetObject<IMetaMesh>();
      EngineApplicationInterface.IDecal = EngineApplicationInterface.GetObject<IDecal>();
      EngineApplicationInterface.IClothSimulatorComponent = EngineApplicationInterface.GetObject<IClothSimulatorComponent>();
      EngineApplicationInterface.ICompositeComponent = EngineApplicationInterface.GetObject<ICompositeComponent>();
      EngineApplicationInterface.IPhysicsShape = EngineApplicationInterface.GetObject<IPhysicsShape>();
      EngineApplicationInterface.IBodyPart = EngineApplicationInterface.GetObject<IBodyPart>();
      EngineApplicationInterface.IMesh = EngineApplicationInterface.GetObject<IMesh>();
      EngineApplicationInterface.IMeshBuilder = EngineApplicationInterface.GetObject<IMeshBuilder>();
      EngineApplicationInterface.ICamera = EngineApplicationInterface.GetObject<ICamera>();
      EngineApplicationInterface.ISkeleton = EngineApplicationInterface.GetObject<ISkeleton>();
      EngineApplicationInterface.IGameEntity = EngineApplicationInterface.GetObject<IGameEntity>();
      EngineApplicationInterface.IGameEntityComponent = EngineApplicationInterface.GetObject<IGameEntityComponent>();
      EngineApplicationInterface.IScene = EngineApplicationInterface.GetObject<IScene>();
      EngineApplicationInterface.IScriptComponent = EngineApplicationInterface.GetObject<IScriptComponent>();
      EngineApplicationInterface.ILight = EngineApplicationInterface.GetObject<ILight>();
      EngineApplicationInterface.IParticleSystem = EngineApplicationInterface.GetObject<IParticleSystem>();
      EngineApplicationInterface.IPhysicsMaterial = EngineApplicationInterface.GetObject<IPhysicsMaterial>();
      EngineApplicationInterface.ISceneView = EngineApplicationInterface.GetObject<ISceneView>();
      EngineApplicationInterface.IView = EngineApplicationInterface.GetObject<IView>();
      EngineApplicationInterface.ITableauView = EngineApplicationInterface.GetObject<ITableauView>();
      EngineApplicationInterface.ITextureView = EngineApplicationInterface.GetObject<ITextureView>();
      EngineApplicationInterface.IVideoPlayerView = EngineApplicationInterface.GetObject<IVideoPlayerView>();
      EngineApplicationInterface.IThumbnailCreatorView = EngineApplicationInterface.GetObject<IThumbnailCreatorView>();
      EngineApplicationInterface.IDebug = EngineApplicationInterface.GetObject<IDebug>();
      EngineApplicationInterface.ITwoDimensionView = EngineApplicationInterface.GetObject<ITwoDimensionView>();
      EngineApplicationInterface.IUtil = EngineApplicationInterface.GetObject<IUtil>();
      EngineApplicationInterface.IEngineSizeChecker = EngineApplicationInterface.GetObject<IEngineSizeChecker>();
      EngineApplicationInterface.IInput = EngineApplicationInterface.GetObject<IInput>();
      EngineApplicationInterface.ITime = EngineApplicationInterface.GetObject<ITime>();
      EngineApplicationInterface.IScreen = EngineApplicationInterface.GetObject<IScreen>();
      EngineApplicationInterface.IMusic = EngineApplicationInterface.GetObject<IMusic>();
      EngineApplicationInterface.IImgui = EngineApplicationInterface.GetObject<IImgui>();
      EngineApplicationInterface.IMouseManager = EngineApplicationInterface.GetObject<IMouseManager>();
      EngineApplicationInterface.IHighlights = EngineApplicationInterface.GetObject<IHighlights>();
      EngineApplicationInterface.ISoundEvent = EngineApplicationInterface.GetObject<ISoundEvent>();
      EngineApplicationInterface.ISoundManager = EngineApplicationInterface.GetObject<ISoundManager>();
      EngineApplicationInterface.IConfig = EngineApplicationInterface.GetObject<IConfig>();
      EngineApplicationInterface.IManagedMeshEditOperations = EngineApplicationInterface.GetObject<IManagedMeshEditOperations>();
    }
  }
}
