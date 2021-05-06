// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Missions.SiegeWeaponCollection
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Missions
{
  public class SiegeWeaponCollection
  {
    private readonly Dictionary<System.Type, int> _weapons;

    public SiegeWeaponCollection(Dictionary<System.Type, int> availableWeapons) => this._weapons = availableWeapons;

    public int GetMaxDeployableWeaponCount(System.Type t) => this._weapons.ContainsKey(t) ? this._weapons[t] : 0;

    public static System.Type GetWeaponType(ScriptComponentBehaviour weapon) => weapon is UsableGameObjectGroup ? weapon.GameEntity.GetChildren().SelectMany<GameEntity, ScriptComponentBehaviour>((Func<GameEntity, IEnumerable<ScriptComponentBehaviour>>) (c => c.GetScriptComponents((Func<ScriptComponentBehaviour, bool>) (s => s is IFocusable)))).First<ScriptComponentBehaviour>().GetType() : weapon.GetType();
  }
}
