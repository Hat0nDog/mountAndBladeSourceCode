// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Material
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.Engine
{
  public sealed class Material : Resource
  {
    public static Material GetDefaultMaterial() => EngineApplicationInterface.IMaterial.GetDefaultMaterial();

    public static Material GetOutlineMaterial(Mesh mesh) => EngineApplicationInterface.IMaterial.GetOutlineMaterial(mesh.GetMaterial().Pointer);

    public static Material GetDefaultTableauSampleMaterial(bool transparency) => !transparency ? Material.GetFromResource("sample_shield_matte") : Material.GetFromResource("tableau_with_transparency");

    public static Material CreateTableauMaterial(
      RenderTargetComponent.TextureUpdateEventHandler eventHandler,
      object objectRef,
      Material sampleMaterial,
      int tableauSizeX,
      int tableauSizeY,
      bool continuousTableau = false)
    {
      if ((NativeObject) sampleMaterial == (NativeObject) null)
        sampleMaterial = Material.GetDefaultTableauSampleMaterial(true);
      Material copy = sampleMaterial.CreateCopy();
      uint materialShaderFlagMask = (uint) copy.GetShader().GetMaterialShaderFlagMask("use_tableau_blending");
      ulong shaderFlags = copy.GetShaderFlags();
      copy.SetShaderFlags(shaderFlags | (ulong) materialShaderFlagMask);
      string str = "";
      System.Type type = objectRef.GetType();
      MaterialCacheIDGetMethodDelegate getMethodDelegate;
      if (!continuousTableau && HasTableauCache.TableauCacheTypes.TryGetValue(type, out getMethodDelegate))
      {
        str = getMethodDelegate(objectRef).ToLower();
        Texture fromResource = Texture.CheckAndGetFromResource(str);
        if ((NativeObject) fromResource != (NativeObject) null)
        {
          copy.SetTexture(Material.MBTextureType.DiffuseMap2, fromResource);
          return copy;
        }
      }
      if (str != "")
        Texture.ScaleTextureWithRatio(ref tableauSizeX, ref tableauSizeY);
      Texture tableauTexture = Texture.CreateTableauTexture(str, eventHandler, objectRef, tableauSizeX, tableauSizeY);
      if (str != "")
      {
        TableauView tableauView = tableauTexture.TableauView;
        tableauView.SetSaveFinalResultToDisk(true);
        tableauView.SetFileNameToSaveResult(str);
        tableauView.SetFileTypeToSave(View.TextureSaveFormat.TextureTypeDds);
      }
      if (str != "")
        tableauTexture.TransformRenderTargetToResource(str);
      copy.SetTexture(Material.MBTextureType.DiffuseMap2, tableauTexture);
      return copy;
    }

    internal Material(UIntPtr sourceMaterialPointer)
      : base(sourceMaterialPointer)
    {
    }

    public Material CreateCopy() => EngineApplicationInterface.IMaterial.CreateCopy(this.Pointer);

    public static Material GetFromResource(string materialName) => EngineApplicationInterface.IMaterial.GetFromResource(materialName);

    public void SetShader(Shader shader) => EngineApplicationInterface.IMaterial.SetShader(this.Pointer, shader.Pointer);

    public Shader GetShader() => EngineApplicationInterface.IMaterial.GetShader(this.Pointer);

    public ulong GetShaderFlags() => EngineApplicationInterface.IMaterial.GetShaderFlags(this.Pointer);

    public void SetShaderFlags(ulong flagEntry) => EngineApplicationInterface.IMaterial.SetShaderFlags(this.Pointer, flagEntry);

    public void SetMeshVectorArgument(float x, float y, float z, float w) => EngineApplicationInterface.IMaterial.SetMeshVectorArgument(this.Pointer, x, y, z, w);

    public void SetTexture(Material.MBTextureType textureType, Texture texture) => EngineApplicationInterface.IMaterial.SetTexture(this.Pointer, (int) textureType, texture.Pointer);

    public void SetTextureAtSlot(int textureSlot, Texture texture) => EngineApplicationInterface.IMaterial.SetTextureAtSlot(this.Pointer, textureSlot, texture.Pointer);

    public void SetAreaMapScale(float scale) => EngineApplicationInterface.IMaterial.SetAreaMapScale(this.Pointer, scale);

    public void SetEnableSkinning(bool enable) => EngineApplicationInterface.IMaterial.SetEnableSkinning(this.Pointer, enable);

    public bool UsingSkinning() => EngineApplicationInterface.IMaterial.UsingSkinning(this.Pointer);

    public Texture GetTexture(Material.MBTextureType textureType) => EngineApplicationInterface.IMaterial.GetTexture(this.Pointer, (int) textureType);

    public string Name
    {
      get => EngineApplicationInterface.IMaterial.GetName(this.Pointer);
      set => EngineApplicationInterface.IMaterial.SetName(this.Pointer, value);
    }

    public static Material GetAlphaMaskTableauMaterial() => EngineApplicationInterface.IMaterial.GetFromResource("tableau_with_alpha_mask");

    public Material.MBAlphaBlendMode GetAlphaBlendMode() => (Material.MBAlphaBlendMode) EngineApplicationInterface.IMaterial.GetAlphaBlendMode(this.Pointer);

    public void SetAlphaBlendMode(Material.MBAlphaBlendMode alphaBlendMode) => EngineApplicationInterface.IMaterial.SetAlphaBlendMode(this.Pointer, (int) alphaBlendMode);

    public void SetAlphaTestValue(float alphaTestValue) => EngineApplicationInterface.IMaterial.SetAlphaTestValue(this.Pointer, alphaTestValue);

    public float GetAlphaTestValue() => EngineApplicationInterface.IMaterial.GetAlphaTestValue(this.Pointer);

    private bool CheckMaterialShaderFlag(Material.MBMaterialShaderFlags flagEntry) => (EngineApplicationInterface.IMaterial.GetShaderFlags(this.Pointer) & (ulong) flagEntry) > 0UL;

    private void SetMaterialShaderFlag(Material.MBMaterialShaderFlags flagEntry, bool value)
    {
      ulong shaderFlags = (ulong) ((long) EngineApplicationInterface.IMaterial.GetShaderFlags(this.Pointer) & ~(long) flagEntry | (long) flagEntry & (value ? (long) byte.MaxValue : 0L));
      EngineApplicationInterface.IMaterial.SetShaderFlags(this.Pointer, shaderFlags);
    }

    public void AddMaterialShaderFlag(string flagName, bool showErrors) => EngineApplicationInterface.IMaterial.AddMaterialShaderFlag(this.Pointer, flagName, showErrors);

    public bool UsingSpecular
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecular);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecular, value);
    }

    public bool UsingSpecularMap
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecularMap);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecularMap, value);
    }

    public bool UsingEnvironmentMap
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseEnvironmentMap);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseEnvironmentMap, value);
    }

    public bool UsingSpecularAlpha
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecularAlpha);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSpecularAlpha, value);
    }

    public bool UsingDynamicLight
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseDynamicLight);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseDynamicLight, value);
    }

    public bool UsingSunLight
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSunLight);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseSunLight, value);
    }

    public bool UsingFresnel
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseFresnel);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseFresnel, value);
    }

    public bool IsSunShadowReceiver
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.SunShadowReceiver);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.SunShadowReceiver, value);
    }

    public bool IsDynamicShadowReceiver
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.DynamicShadowReceiver);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.DynamicShadowReceiver, value);
    }

    public bool UsingDiffuseAlphaMap
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseDiffuseAlphaMap);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseDiffuseAlphaMap, value);
    }

    public bool UsingParallaxMapping
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseParallaxMapping);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseParallaxMapping, value);
    }

    public bool UsingParallaxOcclusion
    {
      get => this.CheckMaterialShaderFlag(Material.MBMaterialShaderFlags.UseParallaxOcclusion);
      set => this.SetMaterialShaderFlag(Material.MBMaterialShaderFlags.UseParallaxOcclusion, value);
    }

    public MaterialFlags Flags
    {
      get => (MaterialFlags) EngineApplicationInterface.IMaterial.GetFlags(this.Pointer);
      set => EngineApplicationInterface.IMaterial.SetFlags(this.Pointer, (uint) value);
    }

    public enum MBTextureType
    {
      DiffuseMap,
      DiffuseMap2,
      BumpMap,
      EnvironmentMap,
      SpecularMap,
    }

    public enum MBAlphaBlendMode : byte
    {
      None,
      Modulate,
      AddAlpha,
      Multiply,
      Add,
      Max,
      Factor,
      AddModulateCombined,
      NoAlphaBlendNoWrite,
      ModulateNoWrite,
      GbufferAlphaBlend,
      GbufferAlphaBlendwithVtResolve,
    }

    [System.Flags]
    private enum MBMaterialShaderFlags
    {
      UseSpecular = 1,
      UseSpecularMap = 2,
      UseHemisphericalAmbient = 4,
      UseEnvironmentMap = 8,
      UseDXT5Normal = 16, // 0x00000010
      UseDynamicLight = 32, // 0x00000020
      UseSunLight = 64, // 0x00000040
      UseSpecularAlpha = 128, // 0x00000080
      UseFresnel = 256, // 0x00000100
      SunShadowReceiver = 512, // 0x00000200
      DynamicShadowReceiver = 1024, // 0x00000400
      UseDiffuseAlphaMap = 2048, // 0x00000800
      UseParallaxMapping = 4096, // 0x00001000
      UseParallaxOcclusion = 8192, // 0x00002000
      UseAlphaTestingBit0 = 16384, // 0x00004000
      UseAlphaTestingBit1 = 32768, // 0x00008000
      UseAreaMap = 65536, // 0x00010000
      UseDetailNormalMap = 131072, // 0x00020000
      UseGroundSlopeAlpha = 262144, // 0x00040000
      UseSelfIllumination = 524288, // 0x00080000
      UseColorMapping = 1048576, // 0x00100000
      UseCubicAmbient = 2097152, // 0x00200000
    }
  }
}
