// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.ParticleSystem
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  [EngineClass("rglParticle_system_instanced")]
  public sealed class ParticleSystem : GameEntityComponent
  {
    internal ParticleSystem(UIntPtr pointer)
      : base(pointer)
    {
    }

    public static ParticleSystem CreateParticleSystemAttachedToBone(
      string systemName,
      Skeleton skeleton,
      sbyte boneIndex,
      ref MatrixFrame boneLocalFrame)
    {
      return ParticleSystem.CreateParticleSystemAttachedToBone(ParticleSystemManager.GetRuntimeIdByName(systemName), skeleton, boneIndex, ref boneLocalFrame);
    }

    public static ParticleSystem CreateParticleSystemAttachedToBone(
      int systemRuntimeId,
      Skeleton skeleton,
      sbyte boneIndex,
      ref MatrixFrame boneLocalFrame)
    {
      return EngineApplicationInterface.IParticleSystem.CreateParticleSystemAttachedToBone(systemRuntimeId, skeleton.Pointer, boneIndex, ref boneLocalFrame);
    }

    public static ParticleSystem CreateParticleSystemAttachedToEntity(
      string systemName,
      GameEntity parentEntity,
      ref MatrixFrame boneLocalFrame)
    {
      return ParticleSystem.CreateParticleSystemAttachedToEntity(ParticleSystemManager.GetRuntimeIdByName(systemName), parentEntity, ref boneLocalFrame);
    }

    public static ParticleSystem CreateParticleSystemAttachedToEntity(
      int systemRuntimeId,
      GameEntity parentEntity,
      ref MatrixFrame boneLocalFrame)
    {
      return EngineApplicationInterface.IParticleSystem.CreateParticleSystemAttachedToEntity(systemRuntimeId, parentEntity.Pointer, ref boneLocalFrame);
    }

    public void AddMesh(Mesh mesh) => EngineApplicationInterface.IMetaMesh.AddMesh(this.Pointer, mesh.Pointer, 0U);

    public void SetEnable(bool enable) => EngineApplicationInterface.IParticleSystem.SetEnable(this.Pointer, enable);

    public void SetRuntimeEmissionRateMultiplier(float multiplier) => EngineApplicationInterface.IParticleSystem.SetRuntimeEmissionRateMultiplier(this.Pointer, multiplier);

    public void Restart() => EngineApplicationInterface.IParticleSystem.Restart(this.Pointer);

    public void SetLocalFrame(ref MatrixFrame newLocalFrame) => EngineApplicationInterface.IParticleSystem.SetLocalFrame(this.Pointer, ref newLocalFrame);

    public MatrixFrame GetLocalFrame()
    {
      MatrixFrame identity = MatrixFrame.Identity;
      EngineApplicationInterface.IParticleSystem.GetLocalFrame(this.Pointer, ref identity);
      return identity;
    }

    public void SetParticleEffectByName(string effectName) => EngineApplicationInterface.IParticleSystem.SetParticleEffectByName(this.Pointer, effectName);
  }
}
