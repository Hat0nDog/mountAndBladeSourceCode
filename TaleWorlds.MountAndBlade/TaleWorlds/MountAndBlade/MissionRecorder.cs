// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MissionRecorder
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public class MissionRecorder
  {
    private readonly Mission _mission;

    public MissionRecorder(Mission mission) => this._mission = mission;

    public void RestartRecord() => MBAPI.IMBMission.RestartRecord(this._mission.Pointer);

    public void ProcessRecordUntilTime(float time) => MBAPI.IMBMission.ProcessRecordUntilTime(this._mission.Pointer, time);

    public bool IsEndOfRecord() => MBAPI.IMBMission.EndOfRecord(this._mission.Pointer);

    public void StartRecording() => MBAPI.IMBMission.StartRecording();

    public void RecordCurrentState() => MBAPI.IMBMission.RecordCurrentState(this._mission.Pointer);

    public void BackupRecordToFile(string fileName, string gameType, string sceneLevels) => MBAPI.IMBMission.BackupRecordToFile(this._mission.Pointer, fileName, gameType, sceneLevels);

    public void RestoreRecordFromFile(string fileName) => MBAPI.IMBMission.RestoreRecordFromFile(this._mission.Pointer, fileName);

    public void ClearRecordBuffers() => MBAPI.IMBMission.ClearRecordBuffers(this._mission.Pointer);

    public static string GetSceneNameForReplay(string fileName) => MBAPI.IMBMission.GetSceneNameForReplay(fileName);

    public static string GetGameTypeForReplay(string fileName) => MBAPI.IMBMission.GetGameTypeForReplay(fileName);

    public static string GetSceneLevelsForReplay(string fileName) => MBAPI.IMBMission.GetSceneLevelsForReplay(fileName);

    public static string GetAtmosphereNameForReplay(string fileName) => MBAPI.IMBMission.GetAtmosphereNameForReplay(fileName);

    public static int GetAtmosphereSeasonForReplay(string fileName) => MBAPI.IMBMission.GetAtmosphereSeasonForReplay(fileName);
  }
}
