// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.ScenePropDecal
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class ScenePropDecal : ScriptComponentBehaviour
  {
    public string DiffuseTexture;
    public string NormalTexture;
    public string SpecularTexture;
    public string MaskTexture;
    public bool UseBaseNormals;
    public float TilingSize = 1f;
    public float TilingOffset;
    public float AlphaTestValue;
    public float TextureSweepX;
    public float TextureSweepY;
    public string MaterialName = "deferred_decal_material";
    protected Material UniqueMaterial;

    protected internal override void OnInit()
    {
      base.OnInit();
      this.SetUpMaterial();
    }

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      this.SetUpMaterial();
    }

    protected internal override void OnEditorVariableChanged(string variableName)
    {
      base.OnEditorVariableChanged(variableName);
      this.SetUpMaterial();
    }

    private void EnsureUniqueMaterial() => this.UniqueMaterial = Material.GetFromResource(this.MaterialName).CreateCopy();

    private void SetUpMaterial()
    {
      this.EnsureUniqueMaterial();
      Texture fromResource1 = Texture.CheckAndGetFromResource(this.DiffuseTexture);
      Texture fromResource2 = Texture.CheckAndGetFromResource(this.NormalTexture);
      Texture fromResource3 = Texture.CheckAndGetFromResource(this.SpecularTexture);
      Texture fromResource4 = Texture.CheckAndGetFromResource(this.MaskTexture);
      if ((NativeObject) fromResource1 != (NativeObject) null)
        this.UniqueMaterial.SetTexture(Material.MBTextureType.DiffuseMap, fromResource1);
      if ((NativeObject) fromResource2 != (NativeObject) null)
        this.UniqueMaterial.SetTexture(Material.MBTextureType.BumpMap, fromResource2);
      if ((NativeObject) fromResource3 != (NativeObject) null)
        this.UniqueMaterial.SetTexture(Material.MBTextureType.SpecularMap, fromResource3);
      if ((NativeObject) fromResource4 != (NativeObject) null)
      {
        this.UniqueMaterial.SetTexture(Material.MBTextureType.DiffuseMap2, fromResource4);
        this.UniqueMaterial.AddMaterialShaderFlag("use_areamap", false);
      }
      this.UniqueMaterial.SetAlphaTestValue(this.AlphaTestValue);
      this.GameEntity.SetMaterialForAllMeshes(this.UniqueMaterial);
      Mesh firstMesh = this.GameEntity.GetFirstMesh();
      if (!((NativeObject) firstMesh != (NativeObject) null))
        return;
      if ((NativeObject) this.UniqueMaterial != (NativeObject) null)
        firstMesh.SetVectorArgument(this.TilingSize, this.TilingSize, this.TilingOffset, this.TilingOffset);
      firstMesh.SetVectorArgument2(this.TextureSweepX, this.TextureSweepY, 0.0f, this.UseBaseNormals ? 1f : 0.0f);
    }
  }
}
