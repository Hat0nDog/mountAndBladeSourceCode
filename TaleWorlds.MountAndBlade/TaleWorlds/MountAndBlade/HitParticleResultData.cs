// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.HitParticleResultData
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade
{
  [EngineStruct("Hit_particle_result_data")]
  internal struct HitParticleResultData
  {
    public int StartHitParticleIndex;
    public int ContinueHitParticleIndex;
    public int EndHitParticleIndex;

    public void Reset()
    {
      this.StartHitParticleIndex = -1;
      this.ContinueHitParticleIndex = -1;
      this.EndHitParticleIndex = -1;
    }
  }
}
