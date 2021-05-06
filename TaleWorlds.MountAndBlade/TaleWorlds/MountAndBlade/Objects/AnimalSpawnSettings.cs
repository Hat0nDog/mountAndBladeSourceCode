// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Objects.AnimalSpawnSettings
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Objects
{
  public class AnimalSpawnSettings : ScriptComponentBehaviour
  {
    public bool DisableWandering = true;

    public static void CheckAndSetAnimalAgentFlags(GameEntity spawnEntity, Agent animalAgent)
    {
      if (!spawnEntity.HasScriptOfType<AnimalSpawnSettings>() || !spawnEntity.GetFirstScriptOfType<AnimalSpawnSettings>().DisableWandering)
        return;
      animalAgent.SetAgentFlags(animalAgent.GetAgentFlags() & ~AgentFlag.CanWander);
    }
  }
}
