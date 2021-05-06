// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.AnimatedFlag
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade
{
  public class AnimatedFlag : ScriptComponentBehaviour
  {
    private float _prevTheta;
    private float _prevSkew;
    private Vec3 _prevFlagMeshFrame;
    private float _time;

    public AnimatedFlag()
    {
      this._prevFlagMeshFrame = new Vec3();
      this._prevTheta = 0.0f;
      this._prevSkew = 0.0f;
      this._time = 0.0f;
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this.SetScriptComponentToTick(this.GetTickRequirement());
      MBDebug.Print("AnimatedFlag : OnInit called.", color: Debug.DebugColor.Yellow);
    }

    private void SmoothTheta(ref float theta, float dt)
    {
      float val1 = theta - this._prevTheta;
      if ((double) val1 > 3.14159274101257)
      {
        val1 -= 6.283185f;
        this._prevTheta += 6.283185f;
      }
      else if ((double) val1 < -3.14159274101257)
      {
        val1 += 6.283185f;
        this._prevTheta -= 6.283185f;
      }
      float num = Math.Min(val1, 150f * dt);
      theta = this._prevTheta + num * 0.05f;
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt)
    {
      if ((double) dt == 0.0)
        return;
      MatrixFrame globalFrame = this.GameEntity.GetGlobalFrame();
      MetaMesh metaMesh = this.GameEntity.GetMetaMesh(0);
      if ((NativeObject) metaMesh == (NativeObject) null)
        return;
      Vec3 vec3_1 = globalFrame.origin - this._prevFlagMeshFrame;
      vec3_1.x /= dt;
      vec3_1.y /= dt;
      vec3_1.z /= dt;
      Vec3 vec3_2 = new Vec3(20f, z: -10f) * 0.1f - vec3_1;
      if ((double) vec3_2.LengthSquared < 9.99999993922529E-09)
        return;
      Vec3 local = globalFrame.rotation.TransformToLocal(vec3_2);
      local.z = 0.0f;
      double num1 = (double) local.Normalize();
      float theta = (float) Math.Atan2((double) local.y, (double) local.x);
      this.SmoothTheta(ref theta, dt);
      Vec3 scaleVector = metaMesh.Frame.rotation.GetScaleVector();
      MatrixFrame identity = MatrixFrame.Identity;
      identity.Scale(scaleVector);
      identity.rotation.RotateAboutUp(theta);
      this._prevTheta = theta;
      float num2 = this._prevSkew + Math.Min((float) Math.Acos((double) Vec3.DotProduct(vec3_2, globalFrame.rotation.u) / (double) vec3_2.Length) - this._prevSkew, 150f * dt) * 0.05f;
      this._prevSkew = num2;
      float num3 = MBMath.ClampFloat(vec3_2.Length, 1f / 1000f, 10000f);
      this._time += (float) ((double) dt * (double) num3 * 0.5);
      metaMesh.Frame = identity;
      metaMesh.VectorUserData = new Vec3((float) Math.Cos((double) num2), 1f - (float) Math.Sin((double) num2), w: this._time);
      this._prevFlagMeshFrame = globalFrame.origin;
    }

    protected internal override bool IsOnlyVisual() => true;
  }
}
