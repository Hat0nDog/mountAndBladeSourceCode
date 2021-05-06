// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IMeshBuilder
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IMeshBuilder
  {
    [EngineMethod("create_tiling_window_mesh", false)]
    Mesh CreateTilingWindowMesh(
      string baseMeshName,
      ref Vec2 meshSizeMin,
      ref Vec2 meshSizeMax,
      ref Vec2 borderThickness,
      ref Vec2 backgroundBorderThickness);

    [EngineMethod("create_tiling_button_mesh", false)]
    Mesh CreateTilingButtonMesh(
      string baseMeshName,
      ref Vec2 meshSizeMin,
      ref Vec2 meshSizeMax,
      ref Vec2 borderThickness);

    [EngineMethod("finalize_mesh_builder", false)]
    Mesh FinalizeMeshBuilder(
      int num_vertices,
      Vec3[] vertices,
      int num_face_corners,
      MeshBuilder.FaceCorner[] faceCorners,
      int num_faces,
      MeshBuilder.Face[] faces);
  }
}
