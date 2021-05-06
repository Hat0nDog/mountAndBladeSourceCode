// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.SceneInitializationData
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  [EngineStruct("rglScene_initialization_data")]
  public struct SceneInitializationData
  {
    public bool InitPhysicsWorld;
    public bool LoadNavMesh;
    public bool InitFloraNodes;
    public bool UsePhysicsMaterials;
    public bool EnableFloraPhysics;
    public bool UseTerrainMeshBlending;
    public bool DoNotUseLoadingScreen;
    public bool CreateOros;
    public bool ForTerrainShaderCompile;

    public SceneInitializationData(bool initializeWithDefaults)
    {
      if (initializeWithDefaults)
      {
        this.InitPhysicsWorld = true;
        this.LoadNavMesh = true;
        this.InitFloraNodes = true;
        this.UsePhysicsMaterials = true;
        this.EnableFloraPhysics = true;
        this.UseTerrainMeshBlending = true;
        this.DoNotUseLoadingScreen = false;
        this.CreateOros = false;
        this.ForTerrainShaderCompile = false;
      }
      else
      {
        this.InitPhysicsWorld = false;
        this.LoadNavMesh = false;
        this.InitFloraNodes = false;
        this.UsePhysicsMaterials = false;
        this.EnableFloraPhysics = false;
        this.UseTerrainMeshBlending = false;
        this.DoNotUseLoadingScreen = false;
        this.CreateOros = false;
        this.ForTerrainShaderCompile = false;
      }
    }
  }
}
