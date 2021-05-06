// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.IMesh
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [ApplicationInterfaceBase]
  internal interface IMesh
  {
    [EngineMethod("create_mesh", false)]
    Mesh CreateMesh(bool editable);

    [EngineMethod("get_base_mesh", false)]
    Mesh GetBaseMesh(UIntPtr ptr);

    [EngineMethod("create_mesh_with_material", false)]
    Mesh CreateMeshWithMaterial(UIntPtr ptr);

    [EngineMethod("create_mesh_copy", false)]
    Mesh CreateMeshCopy(UIntPtr meshPointer);

    [EngineMethod("set_color_and_stroke", false)]
    void SetColorAndStroke(UIntPtr meshPointer, bool drawStroke);

    [EngineMethod("set_mesh_render_order", false)]
    void SetMeshRenderOrder(UIntPtr meshPointer, int renderorder);

    [EngineMethod("has_tag", false)]
    bool HasTag(UIntPtr meshPointer, string tag);

    [EngineMethod("get_mesh_from_resource", false)]
    Mesh GetMeshFromResource(string materialName);

    [EngineMethod("get_random_mesh_with_vdecl", false)]
    Mesh GetRandomMeshWithVdecl(int vdecl);

    [EngineMethod("set_material_by_name", false)]
    void SetMaterialByName(UIntPtr meshPointer, string materialName);

    [EngineMethod("set_material", false)]
    void SetMaterial(UIntPtr meshPointer, UIntPtr materialpointer);

    [EngineMethod("set_vector_argument", false)]
    void SetVectorArgument(
      UIntPtr meshPointer,
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3);

    [EngineMethod("set_vector_argument_2", false)]
    void SetVectorArgument2(
      UIntPtr meshPointer,
      float vectorArgument0,
      float vectorArgument1,
      float vectorArgument2,
      float vectorArgument3);

    [EngineMethod("get_material", false)]
    Material GetMaterial(UIntPtr meshPointer);

    [EngineMethod("release_resources", false)]
    void ReleaseResources(UIntPtr meshPointer);

    [EngineMethod("add_face_corner", false)]
    int AddFaceCorner(
      UIntPtr meshPointer,
      Vec3 vertexPosition,
      Vec3 vertexNormal,
      Vec2 vertexUVCoordinates,
      uint vertexColor,
      UIntPtr lockHandle);

    [EngineMethod("add_face", false)]
    int AddFace(
      UIntPtr meshPointer,
      int faceCorner0,
      int faceCorner1,
      int faceCorner2,
      UIntPtr lockHandle);

    [EngineMethod("clear_mesh", false)]
    void ClearMesh(UIntPtr meshPointer);

    [EngineMethod("set_name", false)]
    void SetName(UIntPtr meshPointer, string name);

    [EngineMethod("get_name", false)]
    string GetName(UIntPtr meshPointer);

    [EngineMethod("set_morph_time", false)]
    void SetMorphTime(UIntPtr meshPointer, float newTime);

    [EngineMethod("set_culling_mode", false)]
    void SetCullingMode(UIntPtr meshPointer, uint newCullingMode);

    [EngineMethod("set_color", false)]
    void SetColor(UIntPtr meshPointer, uint newColor);

    [EngineMethod("get_color", false)]
    uint GetColor(UIntPtr meshPointer);

    [EngineMethod("set_color_2", false)]
    void SetColor2(UIntPtr meshPointer, uint newColor2);

    [EngineMethod("get_color_2", false)]
    uint GetColor2(UIntPtr meshPointer);

    [EngineMethod("set_color_alpha", false)]
    void SetColorAlpha(UIntPtr meshPointer, uint newColorAlpha);

    [EngineMethod("get_face_count", false)]
    uint GetFaceCount(UIntPtr meshPointer);

    [EngineMethod("get_face_corner_count", false)]
    uint GetFaceCornerCount(UIntPtr meshPointer);

    [EngineMethod("compute_normals", false)]
    void ComputeNormals(UIntPtr meshPointer);

    [EngineMethod("compute_tangents", false)]
    void ComputeTangents(UIntPtr meshPointer);

    [EngineMethod("add_mesh_to_mesh", false)]
    void AddMeshToMesh(UIntPtr meshPointer, UIntPtr newMeshPointer, ref MatrixFrame meshFrame);

    [EngineMethod("set_local_frame", false)]
    void SetLocalFrame(UIntPtr meshPointer, ref MatrixFrame meshFrame);

    [EngineMethod("get_local_frame", false)]
    void GetLocalFrame(UIntPtr meshPointer, ref MatrixFrame outFrame);

    [EngineMethod("update_bounding_box", false)]
    void UpdateBoundingBox(UIntPtr meshPointer);

    [EngineMethod("set_as_not_effected_by_season", false)]
    void SetAsNotEffectedBySeason(UIntPtr meshPointer);

    [EngineMethod("get_bounding_box_width", false)]
    float GetBoundingBoxWidth(UIntPtr meshPointer);

    [EngineMethod("get_bounding_box_height", false)]
    float GetBoundingBoxHeight(UIntPtr meshPointer);

    [EngineMethod("get_bounding_box_min", false)]
    Vec3 GetBoundingBoxMin(UIntPtr meshPointer);

    [EngineMethod("get_bounding_box_max", false)]
    Vec3 GetBoundingBoxMax(UIntPtr meshPointer);

    [EngineMethod("add_triangle", false)]
    void AddTriangle(
      UIntPtr meshPointer,
      Vec3 p1,
      Vec3 p2,
      Vec3 p3,
      Vec2 uv1,
      Vec2 uv2,
      Vec2 uv3,
      uint color,
      UIntPtr lockHandle);

    [EngineMethod("add_triangle_with_vertex_colors", false)]
    void AddTriangleWithVertexColors(
      UIntPtr meshPointer,
      Vec3 p1,
      Vec3 p2,
      Vec3 p3,
      Vec2 uv1,
      Vec2 uv2,
      Vec2 uv3,
      uint c1,
      uint c2,
      uint c3,
      UIntPtr lockHandle);

    [EngineMethod("hint_indices_dynamic", false)]
    void HintIndicesDynamic(UIntPtr meshPointer);

    [EngineMethod("hint_vertices_dynamic", false)]
    void HintVerticesDynamic(UIntPtr meshPointer);

    [EngineMethod("recompute_bounding_box", false)]
    void RecomputeBoundingBox(UIntPtr meshPointer);

    [EngineMethod("get_billboard", false)]
    BillboardType GetBillboard(UIntPtr meshPointer);

    [EngineMethod("set_billboard", false)]
    void SetBillboard(UIntPtr meshPointer, BillboardType value);

    [EngineMethod("get_visibility_mask", false)]
    VisibilityMaskFlags GetVisibilityMask(UIntPtr meshPointer);

    [EngineMethod("set_visibility_mask", false)]
    void SetVisibilityMask(UIntPtr meshPointer, VisibilityMaskFlags value);

    [EngineMethod("get_edit_data_face_corner_count", false)]
    int GetEditDataFaceCornerCount(UIntPtr meshPointer);

    [EngineMethod("set_edit_data_face_corner_vertex_color", false)]
    void SetEditDataFaceCornerVertexColor(UIntPtr meshPointer, int index, uint color);

    [EngineMethod("get_edit_data_face_corner_vertex_color", false)]
    uint GetEditDataFaceCornerVertexColor(UIntPtr meshPointer, int index);

    [EngineMethod("preload_for_rendering", false)]
    void PreloadForRendering(UIntPtr meshPointer);

    [EngineMethod("set_contour_color", false)]
    void SetContourColor(UIntPtr meshPointer, Vec3 color, bool alwaysVisible, bool maskMesh);

    [EngineMethod("disable_contour", false)]
    void DisableContour(UIntPtr meshPointer);

    [EngineMethod("set_external_bounding_box", false)]
    void SetExternalBoundingBox(UIntPtr meshPointer, ref BoundingBox bbox);

    [EngineMethod("add_edit_data_user", false)]
    void AddEditDataUser(UIntPtr meshPointer);

    [EngineMethod("release_edit_data_user", false)]
    void ReleaseEditDataUser(UIntPtr meshPointer);

    [EngineMethod("set_edit_data_policy", false)]
    void SetEditDataPolicy(UIntPtr meshPointer, EditDataPolicy policy);

    [EngineMethod("lock_edit_data_write", false)]
    UIntPtr LockEditDataWrite(UIntPtr meshPointer);

    [EngineMethod("unlock_edit_data_write", false)]
    void UnlockEditDataWrite(UIntPtr meshPointer, UIntPtr handle);
  }
}
