// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.TestScript
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
  public class TestScript : ScriptComponentBehaviour
  {
    public string testString;
    public float rotationSpeed;
    public float waterSplashPhaseOffset;
    public float waterSplashIntervalMultiplier = 1f;
    public bool isWaterMill;
    private float currentRotation;
    public float MoveAxisX = 1f;
    public float MoveAxisY;
    public float MoveAxisZ;
    public float MoveSpeed = 0.0001f;
    public float MoveDistance = 10f;
    protected float MoveDirection = 1f;
    protected float CurrentDistance;
    public GameEntity sideRotatingEntity;
    public GameEntity forwardRotatingEntity;

    private void Move(float dt)
    {
      if ((double) MBMath.Absf(this.MoveDistance) < 9.99999974737875E-06)
        return;
      Vec3 vec3_1 = new Vec3(this.MoveAxisX, this.MoveAxisY, this.MoveAxisZ);
      double num1 = (double) vec3_1.Normalize();
      Vec3 vec3_2 = vec3_1 * this.MoveSpeed * this.MoveDirection;
      float num2 = vec3_2.Length * this.MoveDirection;
      if ((double) this.CurrentDistance + (double) num2 <= -(double) this.MoveDistance)
      {
        this.MoveDirection = 1f;
        num2 *= -1f;
        vec3_2 *= -1f;
      }
      else if ((double) this.CurrentDistance + (double) num2 >= (double) this.MoveDistance)
      {
        this.MoveDirection = -1f;
        num2 *= -1f;
        vec3_2 *= -1f;
      }
      this.CurrentDistance += num2;
      MatrixFrame frame = this.GameEntity.GetFrame();
      frame.origin += vec3_2;
      this.GameEntity.SetFrame(ref frame);
    }

    private void Rotate(float dt)
    {
      MatrixFrame frame = this.GameEntity.GetFrame();
      frame.rotation.RotateAboutUp(this.rotationSpeed * (1f / 1000f) * dt);
      this.GameEntity.SetFrame(ref frame);
      this.currentRotation += this.rotationSpeed * (1f / 1000f) * dt;
    }

    private bool isRotationPhaseInsidePhaseBoundries(
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
      float num = 6.28f / childCount;
      foreach (GameEntity gameEntity in children)
      {
        int integerFromStringEnd = TestScript.GetIntegerFromStringEnd(gameEntity.Name);
        float currentPhase = this.currentRotation % 6.28f;
        float startPhase = (float) (((double) num * (double) integerFromStringEnd + (double) this.waterSplashPhaseOffset) % 6.28000020980835);
        float endPhase = (float) (((double) startPhase + (double) num * (double) this.waterSplashIntervalMultiplier) % 6.28000020980835);
        if (this.isRotationPhaseInsidePhaseBoundries(currentPhase, startPhase, endPhase))
          gameEntity.ResumeParticleSystem(true);
        else
          gameEntity.PauseParticleSystem(true);
      }
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      this.Rotate(dt);
      this.Move(dt);
      if (!this.isWaterMill)
        return;
      this.DoWaterMillCalculation();
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this.Rotate(dt);
      this.Move(dt);
      if (this.isWaterMill)
        this.DoWaterMillCalculation();
      if ((NativeObject) this.sideRotatingEntity != (NativeObject) null)
      {
        MatrixFrame frame = this.sideRotatingEntity.GetFrame();
        frame.rotation.RotateAboutSide(this.rotationSpeed * 0.01f * dt);
        this.sideRotatingEntity.SetFrame(ref frame);
      }
      if (!((NativeObject) this.forwardRotatingEntity != (NativeObject) null))
        return;
      MatrixFrame frame1 = this.forwardRotatingEntity.GetFrame();
      frame1.rotation.RotateAboutSide(this.rotationSpeed * 0.005f * dt);
      this.forwardRotatingEntity.SetFrame(ref frame1);
    }
  }
}
