// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Objects.Siege.MultiplayerBatteringRamSpawner
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
  public class MultiplayerBatteringRamSpawner : BatteringRamSpawner
  {
    private const float MaxHitPoint = 12000f;
    private const float SpeedMultiplier = 1f;

    public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
    {
      base.AssignParameters(_spawnerMissionHelper);
      _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<DestructableComponent>().MaxHitPoint = 12000f;
      BatteringRam firstScriptOfType = _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<BatteringRam>();
      firstScriptOfType.MaxSpeed *= 1f;
      firstScriptOfType.MinSpeed *= 1f;
    }
  }
}
