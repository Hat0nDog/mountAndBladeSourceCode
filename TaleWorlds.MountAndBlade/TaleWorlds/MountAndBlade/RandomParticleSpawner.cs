// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.RandomParticleSpawner
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class RandomParticleSpawner : ScriptComponentBehaviour
  {
    private Timer nextParticleSpawnTimer;
    public float spawnInterval = 3f;

    private void InitScript() => this.nextParticleSpawnTimer = new Timer(MBCommon.GetTime(MBCommon.TimeType.Mission), this.spawnInterval);

    private void CheckSpawnParticle(float time)
    {
      if (!this.nextParticleSpawnTimer.Check(time))
        return;
      int childCount = this.GameEntity.ChildCount;
      if (childCount > 0)
      {
        GameEntity child = this.GameEntity.GetChild(MBRandom.RandomInt(childCount));
        int componentCount = child.GetComponentCount(GameEntity.ComponentType.ParticleSystemInstanced);
        for (int index = 0; index < componentCount; ++index)
          ((ParticleSystem) child.GetComponentAtIndex(index, GameEntity.ComponentType.ParticleSystemInstanced)).Restart();
      }
      this.nextParticleSpawnTimer = new Timer(time, this.spawnInterval);
    }

    protected internal override void OnInit()
    {
      base.OnInit();
      this.InitScript();
      this.SetScriptComponentToTick(this.GetTickRequirement());
    }

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      this.OnInit();
    }

    protected override ScriptComponentBehaviour.TickRequirement GetTickRequirement() => ScriptComponentBehaviour.TickRequirement.Tick;

    protected internal override void OnTick(float dt) => this.CheckSpawnParticle(MBCommon.GetTime(MBCommon.TimeType.Mission));

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this.CheckSpawnParticle(MBCommon.GetTime(MBCommon.TimeType.Application));
    }

    protected internal override bool MovesEntity() => true;
  }
}
