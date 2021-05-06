// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Objects.Siege.BallistaSpawner
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
  public class BallistaSpawner : SpawnerBase
  {
    [EditorVisibleScriptComponentVariable(false)]
    public MatrixFrame wait_pos_ground = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(true)]
    public string AddOnDeployTag = "";
    [EditorVisibleScriptComponentVariable(true)]
    public string RemoveOnDeployTag = "";

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      this._spawnerEditorHelper = new SpawnerEntityEditorHelper((ScriptComponentBehaviour) this);
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this._spawnerEditorHelper.Tick(dt);
    }

    protected internal override void OnPreInit()
    {
      base.OnPreInit();
      this._spawnerMissionHelper = new SpawnerEntityMissionHelper((SpawnerBase) this);
      this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper((SpawnerBase) this, true);
    }

    public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
    {
      _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<Ballista>().AddOnDeployTag = this.AddOnDeployTag;
      _spawnerMissionHelper.SpawnedEntity.GetFirstScriptOfType<Ballista>().RemoveOnDeployTag = this.RemoveOnDeployTag;
    }
  }
}
