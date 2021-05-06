// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.WindMill
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class WindMill : ScriptComponentBehaviour
  {
    public float rotationSpeed = 100f;
    public float waterSplashPhaseOffset;
    public float waterSplashIntervalMultiplier = 1f;
    public MetaMesh testMesh;
    public Texture testTexture;
    public GameEntity testEntity;
    public bool isWaterMill;
    private float currentRotation;

    protected internal override void OnInit() => this.SetScriptComponentToTick(this.GetTickRequirement());

    private void Rotate(float dt)
    {
      GameEntity gameEntity = this.GameEntity;
      float a = this.rotationSpeed * (1f / 1000f) * dt;
      MatrixFrame frame = gameEntity.GetFrame();
      frame.rotation.RotateAboutForward(a);
      gameEntity.SetFrame(ref frame);
      this.currentRotation += a;
    }

    private static bool IsRotationPhaseInsidePhaseBoundries(
      float currentPhase,
      float startPhase,
      float endPhase)
    {
      if ((double) endPhase <= (double) startPhase)
        return (double) currentPhase > (double) startPhase;
      return (double) currentPhase > (double) startPhase && (double) currentPhase < (double) endPhase;
    }

    public static int GetIntegerFromStringEnd(string str)
    {
      string str1 = "";
      for (int index = str.Length - 1; index > -1; --index)
      {
        char ch = str[index];
        switch (ch)
        {
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
            str1 = ch.ToString() + str1;
            continue;
          default:
            goto label_4;
        }
      }
label_4:
      return Convert.ToInt32(str1);
    }

    private void DoWaterMillCalculation()
    {
      float childCount = (float) this.GameEntity.ChildCount;
      if ((double) childCount <= 0.0)
        return;
      IEnumerable<GameEntity> children = this.GameEntity.GetChildren();
      float num1 = 6.28f / childCount;
      foreach (GameEntity gameEntity in children)
      {
        int integerFromStringEnd = WindMill.GetIntegerFromStringEnd(gameEntity.Name);
        double num2 = (double) this.currentRotation % 6.28000020980835;
        float num3 = (float) (((double) num1 * (double) integerFromStringEnd + (double) this.waterSplashPhaseOffset) % 6.28000020980835);
        float num4 = (float) (((double) num3 + (double) num1 * (double) this.waterSplashIntervalMultiplier) % 6.28000020980835);
        double num5 = (double) num3;
        double num6 = (double) num4;
        if (WindMill.IsRotationPhaseInsidePhaseBoundries((float) num2, (float) num5, (float) num6))
          gameEntity.ResumeParticleSystem(true);
        else
          gameEntity.PauseParticleSystem(true);
      }
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      this.Rotate(dt);
      if (!this.isWaterMill)
        return;
      this.DoWaterMillCalculation();
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this.Rotate(dt);
      if (!this.isWaterMill)
        return;
      this.DoWaterMillCalculation();
    }

    protected internal override void OnEditorVariableChanged(string variableName)
    {
      base.OnEditorVariableChanged(variableName);
      if (variableName == "testMesh")
      {
        if (!((NativeObject) this.testMesh != (NativeObject) null))
          return;
        this.GameEntity.AddMultiMesh(this.testMesh);
      }
      else if (variableName == "testTexture")
      {
        if (!((NativeObject) this.testTexture != (NativeObject) null))
          return;
        Material copy = this.GameEntity.GetFirstMesh().GetMaterial().CreateCopy();
        copy.SetTexture(Material.MBTextureType.DiffuseMap, this.testTexture);
        this.GameEntity.SetMaterialForAllMeshes(copy);
      }
      else if (variableName == "testEntity")
      {
        int num = (NativeObject) this.testEntity != (NativeObject) null ? 1 : 0;
      }
      else
      {
        if (!(variableName == "testButton"))
          return;
        this.rotationSpeed *= 2f;
      }
    }
  }
}
