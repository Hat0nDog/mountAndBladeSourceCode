// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Objects.Siege.MangonelSpawner
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
  public class MangonelSpawner : SpawnerBase
  {
    [EditorVisibleScriptComponentVariable(false)]
    public MatrixFrame projectile_pile = MatrixFrame.Zero;
    [EditorVisibleScriptComponentVariable(true)]
    public string AddOnDeployTag = "";
    [EditorVisibleScriptComponentVariable(true)]
    public string RemoveOnDeployTag = "";
    [EditorVisibleScriptComponentVariable(true)]
    public bool ammo_pos_a_enabled = true;
    [EditorVisibleScriptComponentVariable(true)]
    public bool ammo_pos_b_enabled = true;
    [EditorVisibleScriptComponentVariable(true)]
    public bool ammo_pos_c_enabled = true;
    [EditorVisibleScriptComponentVariable(true)]
    public bool ammo_pos_d_enabled = true;
    [EditorVisibleScriptComponentVariable(true)]
    public bool ammo_pos_e_enabled = true;
    [EditorVisibleScriptComponentVariable(true)]
    public bool ammo_pos_f_enabled = true;
    [EditorVisibleScriptComponentVariable(true)]
    public bool ammo_pos_g_enabled = true;
    [EditorVisibleScriptComponentVariable(true)]
    public bool ammo_pos_h_enabled = true;

    protected internal override void OnEditorInit()
    {
      base.OnEditorInit();
      this._spawnerEditorHelper = new SpawnerEntityEditorHelper((ScriptComponentBehaviour) this);
    }

    protected internal override void OnEditorTick(float dt)
    {
      base.OnEditorTick(dt);
      this._spawnerEditorHelper.Tick(dt);
      this.RefreshAmmoPositions();
    }

    protected internal override void OnEditorVariableChanged(string variableName)
    {
      base.OnEditorVariableChanged(variableName);
      if (!(variableName == "ammo_pos_a_enabled") && !(variableName == "ammo_pos_b_enabled") && (!(variableName == "ammo_pos_c_enabled") && !(variableName == "ammo_pos_d_enabled")) && (!(variableName == "ammo_pos_e_enabled") && !(variableName == "ammo_pos_f_enabled") && (!(variableName == "ammo_pos_g_enabled") && !(variableName == "ammo_pos_h_enabled"))))
        return;
      this.RefreshAmmoPositions();
    }

    private void RefreshAmmoPositions()
    {
      if (this.ammo_pos_a_enabled)
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_a").SetVisibilityExcludeParents(true);
      else
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_a").SetVisibilityExcludeParents(false);
      if (this.ammo_pos_b_enabled)
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_b").SetVisibilityExcludeParents(true);
      else
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_b").SetVisibilityExcludeParents(false);
      if (this.ammo_pos_c_enabled)
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_c").SetVisibilityExcludeParents(true);
      else
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_c").SetVisibilityExcludeParents(false);
      if (this.ammo_pos_d_enabled)
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_d").SetVisibilityExcludeParents(true);
      else
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_d").SetVisibilityExcludeParents(false);
      if (this.ammo_pos_e_enabled)
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_e").SetVisibilityExcludeParents(true);
      else
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_e").SetVisibilityExcludeParents(false);
      if (this.ammo_pos_f_enabled)
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_f").SetVisibilityExcludeParents(true);
      else
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_f").SetVisibilityExcludeParents(false);
      if (this.ammo_pos_g_enabled)
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_g").SetVisibilityExcludeParents(true);
      else
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_g").SetVisibilityExcludeParents(false);
      if (this.ammo_pos_h_enabled)
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_h").SetVisibilityExcludeParents(true);
      else
        this._spawnerEditorHelper.GetGhostEntityOrChild("ammo_pos_h").SetVisibilityExcludeParents(false);
    }

    protected internal override void OnPreInit()
    {
      base.OnPreInit();
      this._spawnerMissionHelper = new SpawnerEntityMissionHelper((SpawnerBase) this);
      this._spawnerMissionHelperFire = new SpawnerEntityMissionHelper((SpawnerBase) this, true);
    }

    public override void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
    {
      foreach (GameEntity child in _spawnerMissionHelper.SpawnedEntity.GetChildren())
      {
        if (child.GetFirstScriptOfType<Mangonel>() != null)
        {
          child.GetFirstScriptOfType<Mangonel>().AddOnDeployTag = this.AddOnDeployTag;
          child.GetFirstScriptOfType<Mangonel>().RemoveOnDeployTag = this.RemoveOnDeployTag;
          break;
        }
      }
    }
  }
}
