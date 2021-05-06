// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MapAtmosphereProbe
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class MapAtmosphereProbe : ScriptComponentBehaviour
  {
    public bool visualizeRadius = true;
    public bool hideAllProbes = true;
    public static bool hideAllProbesStatic = true;
    public float minRadius = 1f;
    public float maxRadius = 2f;
    public float rainDensity;
    public float temperature;
    public string atmosphereType;
    public string colorGrade;
    private MetaMesh innerSphereMesh;
    private MetaMesh outerSphereMesh;

    public float GetInfluenceAmount(Vec3 worldPosition) => MBMath.SmoothStep(this.minRadius, this.maxRadius, worldPosition.Distance(this.GameEntity.GetGlobalFrame().origin));

    public MapAtmosphereProbe()
    {
      this.hideAllProbes = MapAtmosphereProbe.hideAllProbesStatic;
      if (!MBEditor.IsEditModeOn)
        return;
      this.innerSphereMesh = MetaMesh.GetCopy("physics_sphere_detailed");
      this.outerSphereMesh = MetaMesh.GetCopy("physics_sphere_detailed");
      this.innerSphereMesh.SetMaterial(Material.GetFromResource("light_radius_visualizer"));
      this.outerSphereMesh.SetMaterial(Material.GetFromResource("light_radius_visualizer"));
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      if (!this.visualizeRadius || MapAtmosphereProbe.hideAllProbesStatic)
        return;
      uint num1 = 16711680;
      uint num2 = 720640;
      uint factorColor1_1;
      uint factorColor1_2;
      if (MBEditor.IsEntitySelected(this.GameEntity))
      {
        factorColor1_1 = num1 | 2147483648U;
        factorColor1_2 = num2 | 2147483648U;
      }
      else
      {
        factorColor1_1 = num1 | 1073741824U;
        factorColor1_2 = num2 | 1073741824U;
      }
      this.innerSphereMesh.SetFactor1(factorColor1_1);
      this.outerSphereMesh.SetFactor1(factorColor1_2);
      MatrixFrame frame1;
      frame1.origin = this.GameEntity.GetGlobalFrame().origin;
      frame1.rotation = Mat3.Identity;
      frame1.rotation.ApplyScaleLocal(this.minRadius);
      MatrixFrame frame2;
      frame2.origin = this.GameEntity.GetGlobalFrame().origin;
      frame2.rotation = Mat3.Identity;
      frame2.rotation.ApplyScaleLocal(this.maxRadius);
      this.innerSphereMesh.SetVectorArgument(this.minRadius, this.maxRadius, 0.0f, 0.0f);
      this.outerSphereMesh.SetVectorArgument(this.minRadius, this.maxRadius, 0.0f, 0.0f);
      MBEditor.RenderEditorMesh(this.innerSphereMesh, frame1);
      MBEditor.RenderEditorMesh(this.outerSphereMesh, frame2);
    }

    protected internal override void OnEditorVariableChanged(string variableName)
    {
      base.OnEditorVariableChanged(variableName);
      if (variableName == "minRadius")
        this.minRadius = MBMath.ClampFloat(this.minRadius, 0.1f, this.maxRadius);
      if (variableName == "maxRadius")
        this.maxRadius = MBMath.ClampFloat(this.maxRadius, this.minRadius, float.MaxValue);
      if (!(variableName == "hideAllProbes"))
        return;
      MapAtmosphereProbe.hideAllProbesStatic = this.hideAllProbes;
    }
  }
}
