// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.WeaponSpawner
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.ObjectSystem;

namespace TaleWorlds.MountAndBlade
{
  public class WeaponSpawner : ScriptComponentBehaviour
  {
    public void SpawnWeapon()
    {
      this.OnPreInit();
      this.GameEntity.RemoveAllChildren();
      MissionWeapon weapon = new MissionWeapon(MBObjectManager.Instance.GetObject<ItemObject>(this.GameEntity.Name), (ItemModifier) null, (Banner) null);
      GameEntity gameEntity = Mission.Current.SpawnWeaponWithNewEntity(ref weapon, Mission.WeaponSpawnFlags.WithPhysics, this.GameEntity.GetGlobalFrame());
      List<string> stringList = new List<string>();
      foreach (string tag in this.GameEntity.Tags)
      {
        gameEntity.AddTag(tag);
        stringList.Add(tag);
      }
      for (int index = 0; index < stringList.Count; ++index)
        this.GameEntity.RemoveTag(stringList[index]);
    }
  }
}
