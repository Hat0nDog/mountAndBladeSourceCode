// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Objects.Siege.SpawnerBase
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade.Objects.Siege
{
  public class SpawnerBase : ScriptComponentBehaviour
  {
    [EditorVisibleScriptComponentVariable(true)]
    public string ToBeSpawnedOverrideName = "";
    [EditorVisibleScriptComponentVariable(true)]
    public string ToBeSpawnedOverrideNameForFireVersion = "";
    protected SpawnerEntityEditorHelper _spawnerEditorHelper;
    protected SpawnerEntityMissionHelper _spawnerMissionHelper;
    protected SpawnerEntityMissionHelper _spawnerMissionHelperFire;

    public virtual void AssignParameters(SpawnerEntityMissionHelper _spawnerMissionHelper)
    {
    }

    public virtual bool GetFireVersion() => false;

    protected internal override bool OnCheckForProblems() => !this._spawnerEditorHelper.IsValid;
  }
}
