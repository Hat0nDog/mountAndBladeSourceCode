// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BoundaryWallView
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class BoundaryWallView : ScriptComponentBehaviour
  {
    private List<Vec2> _lastPoints = new List<Vec2>();
    private List<Vec2> _lastAttackerPoints = new List<Vec2>();
    private List<Vec2> _lastDefenderPoints = new List<Vec2>();
    private float timer;

    protected internal override void OnInit() => throw new Exception("This should only be used in editor.");

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this.timer += dt;
      if (!MBEditor.BorderHelpersEnabled() || (double) this.timer < 0.200000002980232)
        return;
      this.timer = 0.0f;
      if ((NativeObject) this.Scene == (NativeObject) null)
        return;
      bool boundaries1 = this.CalculateBoundaries("walk_area_vertex", ref this._lastPoints);
      bool boundaries2 = this.CalculateBoundaries("defender_area_vertex", ref this._lastDefenderPoints);
      bool boundaries3 = this.CalculateBoundaries("attacker_area_vertex", ref this._lastAttackerPoints);
      if (this._lastPoints.Count >= 3 || this._lastDefenderPoints.Count >= 3 || this._lastAttackerPoints.Count >= 3)
      {
        if (!(boundaries1 | boundaries2 | boundaries3))
          return;
        this.GameEntity.ClearEntityComponents(true, false, true);
        this.GameEntity.Name = "editor_map_border";
        Mesh boundaryMesh1 = BoundaryWallView.CreateBoundaryMesh(this.Scene, (ICollection<Vec2>) this._lastPoints);
        if ((NativeObject) boundaryMesh1 != (NativeObject) null)
          this.GameEntity.AddMesh(boundaryMesh1);
        Mesh boundaryMesh2 = BoundaryWallView.CreateBoundaryMesh(this.Scene, (ICollection<Vec2>) this._lastDefenderPoints, new Color(0.0f, 0.0f, 0.8f).ToUnsignedInteger());
        if ((NativeObject) boundaryMesh2 != (NativeObject) null)
          this.GameEntity.AddMesh(boundaryMesh2);
        Mesh boundaryMesh3 = BoundaryWallView.CreateBoundaryMesh(this.Scene, (ICollection<Vec2>) this._lastAttackerPoints, new Color(0.0f, 0.8f, 0.8f).ToUnsignedInteger());
        if (!((NativeObject) boundaryMesh3 != (NativeObject) null))
          return;
        this.GameEntity.AddMesh(boundaryMesh3);
      }
      else
        this.GameEntity.ClearEntityComponents(true, false, true);
    }

    private bool CalculateBoundaries(string vertexTag, ref List<Vec2> lastPoints)
    {
      IEnumerable<GameEntity> source = this.Scene.FindEntitiesWithTag(vertexTag).Where<GameEntity>((Func<GameEntity, bool>) (e => !e.EntityFlags.HasAnyFlag<EntityFlags>(EntityFlags.DontSaveToScene)));
      int num = source.Count<GameEntity>();
      bool flag = false;
      if (num < 3)
        return false;
      List<Vec2> list1 = source.Select<GameEntity, Vec2>((Func<GameEntity, Vec2>) (e => e.GlobalPosition.AsVec2)).ToList<Vec2>();
      Vec2 mid = new Vec2(list1.Average<Vec2>((Func<Vec2, float>) (p => p.x)), list1.Average<Vec2>((Func<Vec2, float>) (p => p.y)));
      List<Vec2> list2 = list1.OrderBy<Vec2, double>((Func<Vec2, double>) (p => Math.Atan2((double) p.x - (double) mid.x, (double) p.y - (double) mid.y))).ToList<Vec2>();
      if (lastPoints != null && lastPoints.Count == list2.Count)
      {
        flag = true;
        for (int index = 0; index < list2.Count; ++index)
        {
          if (list2[index] != lastPoints[index])
          {
            flag = false;
            break;
          }
        }
      }
      lastPoints = list2;
      return !flag;
    }

    public static Mesh CreateBoundaryMesh(
      Scene scene,
      ICollection<Vec2> boundaryPoints,
      uint meshColor = 536918784)
    {
      if (boundaryPoints == null || boundaryPoints.Count < 3)
        return (Mesh) null;
      Mesh mesh = Mesh.CreateMesh();
      UIntPtr num = mesh.LockEditDataWrite();
      Vec3 min;
      Vec3 max;
      scene.GetBoundingBox(out min, out max);
      max.z += 50f;
      min.z -= 50f;
      for (int index = 0; index < boundaryPoints.Count; ++index)
      {
        Vec2 point1 = boundaryPoints.ElementAt<Vec2>(index);
        Vec2 point2 = boundaryPoints.ElementAt<Vec2>((index + 1) % boundaryPoints.Count);
        float height1 = 0.0f;
        float height2 = 0.0f;
        if (!scene.IsAtmosphereIndoor)
        {
          if (!scene.GetHeightAtPoint(point1, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref height1))
          {
            MBDebug.ShowWarning("GetHeightAtPoint failed at CreateBoundaryEntity!");
            return (Mesh) null;
          }
          if (!scene.GetHeightAtPoint(point2, BodyFlags.CommonCollisionExcludeFlagsForCombat, ref height2))
          {
            MBDebug.ShowWarning("GetHeightAtPoint failed at CreateBoundaryEntity!");
            return (Mesh) null;
          }
        }
        else
        {
          height1 = min.z;
          height2 = min.z;
        }
        Vec3 vec3_1 = point1.ToVec3(height1);
        Vec3 vec3_2 = point2.ToVec3(height2);
        Vec3 vec3_3 = Vec3.Up * 2f;
        Vec3 vec3_4 = vec3_1;
        Vec3 vec3_5 = vec3_2;
        Vec3 vec3_6 = vec3_1;
        Vec3 vec3_7 = vec3_2;
        vec3_4.z = Math.Min(vec3_4.z, min.z);
        vec3_5.z = Math.Min(vec3_5.z, min.z);
        vec3_6.z = Math.Max(vec3_6.z, max.z);
        vec3_7.z = Math.Max(vec3_7.z, max.z);
        Vec3 p1 = vec3_4 - vec3_3;
        Vec3 p2 = vec3_5 - vec3_3;
        vec3_6 += vec3_3;
        Vec3 p3 = vec3_7 + vec3_3;
        mesh.AddTriangle(p1, p2, vec3_6, Vec2.Zero, Vec2.Side, Vec2.Forward, meshColor, num);
        mesh.AddTriangle(vec3_6, p2, p3, Vec2.Forward, Vec2.Side, Vec2.One, meshColor, num);
      }
      mesh.SetMaterial("editor_map_border");
      mesh.VisibilityMask = VisibilityMaskFlags.Final | VisibilityMaskFlags.EditModeBorders;
      mesh.SetColorAlpha(150U);
      mesh.SetMeshRenderOrder(250);
      mesh.CullingMode = MBMeshCullingMode.None;
      float vectorArgument0 = 25f;
      if (MBEditor.IsEditModeOn && scene.IsEditorScene())
        vectorArgument0 = 100000f;
      IEnumerable<GameEntity> entitiesWithTag = scene.FindEntitiesWithTag("walk_area_vertex");
      float vectorArgument1 = entitiesWithTag.Count<GameEntity>() > 0 ? entitiesWithTag.Average<GameEntity>((Func<GameEntity, float>) (ent => ent.GlobalPosition.z)) : 0.0f;
      mesh.SetVectorArgument(vectorArgument0, vectorArgument1, 0.0f, 0.0f);
      mesh.ComputeNormals();
      mesh.ComputeTangents();
      mesh.RecomputeBoundingBox();
      mesh.UnlockEditDataWrite(num);
      return mesh;
    }
  }
}
