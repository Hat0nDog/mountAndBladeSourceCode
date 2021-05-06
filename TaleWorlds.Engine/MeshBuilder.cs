// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.MeshBuilder
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public class MeshBuilder
  {
    private List<Vec3> vertices;
    private List<MeshBuilder.FaceCorner> faceCorners;
    private List<MeshBuilder.Face> faces;

    public MeshBuilder()
    {
      this.vertices = new List<Vec3>();
      this.faceCorners = new List<MeshBuilder.FaceCorner>();
      this.faces = new List<MeshBuilder.Face>();
    }

    public int AddFaceCorner(Vec3 position, Vec3 normal, Vec2 uvCoord, uint color)
    {
      this.vertices.Add(new Vec3(position));
      MeshBuilder.FaceCorner faceCorner;
      faceCorner.vertexIndex = this.vertices.Count - 1;
      faceCorner.color = color;
      faceCorner.uvCoord = uvCoord;
      faceCorner.normal = normal;
      this.faceCorners.Add(faceCorner);
      return this.faceCorners.Count - 1;
    }

    public int AddFace(int patchNode0, int patchNode1, int patchNode2)
    {
      MeshBuilder.Face face;
      face.fc0 = patchNode0;
      face.fc1 = patchNode1;
      face.fc2 = patchNode2;
      this.faces.Add(face);
      return this.faces.Count - 1;
    }

    public void Clear()
    {
      this.vertices.Clear();
      this.faceCorners.Clear();
      this.faces.Clear();
    }

    public Mesh Finalize()
    {
      Vec3[] array1 = this.vertices.ToArray();
      MeshBuilder.FaceCorner[] array2 = this.faceCorners.ToArray();
      MeshBuilder.Face[] array3 = this.faces.ToArray();
      Mesh mesh = EngineApplicationInterface.IMeshBuilder.FinalizeMeshBuilder(this.vertices.Count, array1, this.faceCorners.Count, array2, this.faces.Count, array3);
      this.vertices.Clear();
      this.faceCorners.Clear();
      this.faces.Clear();
      return mesh;
    }

    public static Mesh CreateUnitMesh()
    {
      Mesh meshWithMaterial = Mesh.CreateMeshWithMaterial(Material.GetDefaultMaterial());
      Vec3 position1 = new Vec3(y: -1f);
      Vec3 position2 = new Vec3(1f, -1f);
      Vec3 position3 = new Vec3(1f);
      Vec3 position4 = new Vec3();
      Vec3 normal = new Vec3(z: 1f);
      Vec2 uvCoord1 = new Vec2(0.0f, 0.0f);
      Vec2 uvCoord2 = new Vec2(1f, 0.0f);
      Vec2 uvCoord3 = new Vec2(1f, 1f);
      Vec2 uvCoord4 = new Vec2(0.0f, 1f);
      UIntPtr num1 = meshWithMaterial.LockEditDataWrite();
      int num2 = meshWithMaterial.AddFaceCorner(position1, normal, uvCoord1, uint.MaxValue, num1);
      int patchNode1_1 = meshWithMaterial.AddFaceCorner(position2, normal, uvCoord2, uint.MaxValue, num1);
      int num3 = meshWithMaterial.AddFaceCorner(position3, normal, uvCoord3, uint.MaxValue, num1);
      int patchNode1_2 = meshWithMaterial.AddFaceCorner(position4, normal, uvCoord4, uint.MaxValue, num1);
      meshWithMaterial.AddFace(num2, patchNode1_1, num3, num1);
      meshWithMaterial.AddFace(num3, patchNode1_2, num2, num1);
      meshWithMaterial.UpdateBoundingBox();
      meshWithMaterial.UnlockEditDataWrite(num1);
      return meshWithMaterial;
    }

    public static Mesh CreateTilingWindowMesh(
      string baseMeshName,
      Vec2 meshSizeMin,
      Vec2 meshSizeMax,
      Vec2 borderThickness,
      Vec2 bgBorderThickness)
    {
      return EngineApplicationInterface.IMeshBuilder.CreateTilingWindowMesh(baseMeshName, ref meshSizeMin, ref meshSizeMax, ref borderThickness, ref bgBorderThickness);
    }

    public static Mesh CreateTilingButtonMesh(
      string baseMeshName,
      Vec2 meshSizeMin,
      Vec2 meshSizeMax,
      Vec2 borderThickness)
    {
      return EngineApplicationInterface.IMeshBuilder.CreateTilingButtonMesh(baseMeshName, ref meshSizeMin, ref meshSizeMax, ref borderThickness);
    }

    [EngineStruct("rglMeshBuilder_face_corner")]
    public struct FaceCorner
    {
      public int vertexIndex;
      public Vec2 uvCoord;
      public Vec3 normal;
      public uint color;
    }

    [EngineStruct("rglMeshBuilder_face")]
    public struct Face
    {
      public int fc0;
      public int fc1;
      public int fc2;
    }
  }
}
