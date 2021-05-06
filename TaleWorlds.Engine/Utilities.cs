// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Engine.Utilities
// Assembly: TaleWorlds.Engine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DD7DC4E8-6EB6-4CB8-8DE1-FC78E45F7FE8
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Engine.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TaleWorlds.DotNet;
using TaleWorlds.Library;

namespace TaleWorlds.Engine
{
  public static class Utilities
  {
    private static ConcurrentQueue<Utilities.MainThreadJob> jobs = new ConcurrentQueue<Utilities.MainThreadJob>();
    public static bool renderingActive = true;

    public static void ConstructMainThreadJob(Delegate function, params object[] parameters)
    {
      Utilities.MainThreadJob mainThreadJob = new Utilities.MainThreadJob(function, parameters);
      Utilities.jobs.Enqueue(mainThreadJob);
    }

    public static void ConstructMainThreadJob(
      Semaphore semaphore,
      Delegate function,
      params object[] parameters)
    {
      Utilities.MainThreadJob mainThreadJob = new Utilities.MainThreadJob(semaphore, function, parameters);
      Utilities.jobs.Enqueue(mainThreadJob);
    }

    public static void RunJobs()
    {
      Utilities.MainThreadJob result;
      while (Utilities.jobs.TryDequeue(out result))
        result.Invoke();
    }

    public static void WaitJobs()
    {
      do
        ;
      while (!Utilities.jobs.IsEmpty);
    }

    public static void OutputBenchmarkValuesToPerformanceReporter() => EngineApplicationInterface.IUtil.OutputBenchmarkValuesToPerformanceReporter();

    public static void SetBenchmarkStatus(int status, string def) => EngineApplicationInterface.IUtil.SetBenchmarkStatus(status, def);

    public static string GetNativeMemoryStatistics() => EngineApplicationInterface.IUtil.GetNativeMemoryStatistics();

    public static bool CommandLineArgumentExists(string str) => EngineApplicationInterface.IUtil.CommandLineArgumentExists(str);

    public static string GetConsoleHostMachine() => EngineApplicationInterface.IUtil.GetConsoleHostMachine();

    public static string ExportNavMeshFaceMarks(string file_name) => EngineApplicationInterface.IUtil.ExportNavMeshFaceMarks(file_name);

    public static string TakeSSFromTop(string file_name) => EngineApplicationInterface.IUtil.TakeSSFromTop(file_name);

    public static void CheckIfAssetsAndSourcesAreSame() => EngineApplicationInterface.IUtil.CheckIfAssetsAndSourcesAreSame();

    public static void FindAndClearUnusedResources(
      string scene_names,
      int length_of_scene_names_string,
      uint operation_type)
    {
      EngineApplicationInterface.IUtil.FindAndClearUnusedResources(scene_names, length_of_scene_names_string, operation_type);
    }

    public static void GetSnowAmountData(byte[] snowData) => EngineApplicationInterface.IUtil.GetSnowAmountData(snowData);

    public static void FindMeshesWithoutLods(string module_name) => EngineApplicationInterface.IUtil.FindMeshesWithoutLods(module_name);

    public static void SetDisableDumpGeneration(bool value) => EngineApplicationInterface.IUtil.SetDisableDumpGeneration(value);

    public static void SetPrintCallstackAtCrahses(bool value) => EngineApplicationInterface.IUtil.SetPrintCallstackAtCrahses(value);

    public static string[] GetModulesNames() => EngineApplicationInterface.IUtil.GetModulesCode().Split('*');

    public static string GetFullModulePath(string moduleName) => EngineApplicationInterface.IUtil.GetFullModulePath(moduleName);

    public static string[] GetFullModulePaths() => EngineApplicationInterface.IUtil.GetFullModulePaths().Split('*');

    public static string GetFullCommandLineString() => EngineApplicationInterface.IUtil.GetFullCommandLineString();

    public static void SetScreenTextRenderingState(bool state) => EngineApplicationInterface.IUtil.SetScreenTextRenderingState(state);

    public static void SetMessageLineRenderingState(bool state) => EngineApplicationInterface.IUtil.SetMessageLineRenderingState(state);

    public static bool IsTerrainShaderCompileFinished() => EngineApplicationInterface.IUtil.IsTerrainShaderCompileFinished();

    public static void CompileTerrainShaders() => EngineApplicationInterface.IUtil.CompileTerrainShaders();

    public static void SetCrashOnAsserts(bool val) => EngineApplicationInterface.IUtil.SetCrashOnAsserts(val);

    public static void SetCrashOnWarnings(bool val) => EngineApplicationInterface.IUtil.SetCrashOnWarnings(val);

    public static void CompileSingleTerrainShader(
      string scene_name,
      string module_name,
      string compile_configuration = "win64_dx11")
    {
      EngineApplicationInterface.IUtil.CompileSingleTerrainShader(scene_name, module_name, compile_configuration);
    }

    public static void ToggleRender() => EngineApplicationInterface.IUtil.ToggleRender();

    public static bool CheckShaderCompilation() => EngineApplicationInterface.IUtil.CheckShaderCompilation();

    public static void CompileAllShaders(string targetPlatform) => EngineApplicationInterface.IUtil.CompileAllShaders(targetPlatform);

    public static string GetExecutableWorkingDirectory() => EngineApplicationInterface.IUtil.GetExecutableWorkingDirectory();

    public static void SetDumpFolderPath(string path) => EngineApplicationInterface.IUtil.SetDumpFolderPath(path);

    public static void CheckSceneForProblems(string sceneName) => EngineApplicationInterface.IUtil.CheckSceneForProblems(sceneName);

    public static void ExecuteCommandLineCommand(string command) => EngineApplicationInterface.IUtil.ExecuteCommandLineCommand(command);

    public static void QuitGame() => EngineApplicationInterface.IUtil.QuitGame();

    public static string GetBasePath() => EngineApplicationInterface.IUtil.GetBaseDirectory();

    public static string GetVisualTestsValidatePath() => EngineApplicationInterface.IUtil.GetVisualTestsValidatePath();

    public static string GetVisualTestsTestFilesPath() => EngineApplicationInterface.IUtil.GetVisualTestsTestFilesPath();

    public static string GetAttachmentsPath() => EngineApplicationInterface.IUtil.GetAttachmentsPath();

    public static void SetThumbnailCreatorViewForResourceCleanup(ThumbnailCreatorView view) => EngineApplicationInterface.IUtil.SetThumbnailCreatorViewForResourceCleanup(view.Pointer);

    public static void StartScenePerformanceReport(string folderPath) => EngineApplicationInterface.IUtil.StartScenePerformanceReport(folderPath);

    public static bool IsSceneReportFinished() => EngineApplicationInterface.IUtil.IsSceneReportFinished();

    public static float GetFps() => EngineApplicationInterface.IUtil.GetFps();

    public static float GetMainFps() => EngineApplicationInterface.IUtil.GetMainFps();

    public static float GetRendererFps() => EngineApplicationInterface.IUtil.GetRendererFps();

    public static void EnableSingleGPUQueryPerFrame() => EngineApplicationInterface.IUtil.EnableSingleGPUQueryPerFrame();

    public static void FlushManagedObjectsMemory() => Common.MemoryCleanup();

    public static void DisableGlobalLoadingWindow() => EngineApplicationInterface.IUtil.DisableGlobalLoadingWindow();

    public static void EnableGlobalLoadingWindow() => EngineApplicationInterface.IUtil.EnableGlobalLoadingWindow();

    public static void EnableGlobalEditDataCacher() => EngineApplicationInterface.IUtil.EnableGlobalEditDataCacher();

    public static void DoFullBakeAllLevelsAutomated(string module, string scene) => EngineApplicationInterface.IUtil.DoFullBakeAllLevelsAutomated(module, scene);

    public static int GetReturnCode() => EngineApplicationInterface.IUtil.GetReturnCode();

    public static void DisableGlobalEditDataCacher() => EngineApplicationInterface.IUtil.DisableGlobalEditDataCacher();

    public static void DoFullBakeSingleLevelAutomated(string module, string scene) => EngineApplicationInterface.IUtil.DoFullBakeSingleLevelAutomated(module, scene);

    public static void DoLightOnlyBakeSingleLevelAutomated(string module, string scene) => EngineApplicationInterface.IUtil.DoLightOnlyBakeSingleLevelAutomated(module, scene);

    public static void DoLightOnlyBakeAllLevelsAutomated(string module, string scene) => EngineApplicationInterface.IUtil.DoLightOnlyBakeAllLevelsAutomated(module, scene);

    public static bool DidAutomatedGIBakeFinished() => EngineApplicationInterface.IUtil.DidAutomatedGIBakeFinished();

    public static void GetSelectedEntities(ref List<GameEntity> gameEntities)
    {
      int selectedEntityCount = EngineApplicationInterface.IUtil.GetEditorSelectedEntityCount();
      UIntPtr[] gameEntitiesTemp = new UIntPtr[selectedEntityCount];
      EngineApplicationInterface.IUtil.GetEditorSelectedEntities(gameEntitiesTemp);
      for (int index = 0; index < selectedEntityCount; ++index)
        gameEntities.Add(new GameEntity(gameEntitiesTemp[index]));
    }

    public static void DeleteEntitiesInEditorScene(List<GameEntity> gameEntities)
    {
      int count = gameEntities.Count;
      UIntPtr[] gameEntities1 = new UIntPtr[count];
      for (int index = 0; index < count; ++index)
        gameEntities1[index] = gameEntities[index].Pointer;
      EngineApplicationInterface.IUtil.DeleteEntitiesInEditorScene(gameEntities1, count);
    }

    public static void CreateSelectionInEditor(List<GameEntity> gameEntities, string name)
    {
      int count = gameEntities.Count;
      UIntPtr[] gameEntities1 = new UIntPtr[gameEntities.Count];
      for (int index = 0; index < count; ++index)
        gameEntities1[index] = gameEntities[index].Pointer;
      EngineApplicationInterface.IUtil.CreateSelectionInEditor(gameEntities1, count, name);
    }

    public static void SelectEntities(List<GameEntity> gameEntities)
    {
      int count = gameEntities.Count;
      UIntPtr[] gameEntities1 = new UIntPtr[count];
      for (int index = 0; index < count; ++index)
        gameEntities1[index] = gameEntities[index].Pointer;
      EngineApplicationInterface.IUtil.SelectEntities(gameEntities1, count);
    }

    public static void GetEntitiesOfSelectionSet(
      string selectionSetName,
      ref List<GameEntity> gameEntities)
    {
      int countOfSelectionSet = EngineApplicationInterface.IUtil.GetEntityCountOfSelectionSet(selectionSetName);
      UIntPtr[] gameEntitiesTemp = new UIntPtr[countOfSelectionSet];
      EngineApplicationInterface.IUtil.GetEntitiesOfSelectionSet(selectionSetName, gameEntitiesTemp);
      for (int index = 0; index < countOfSelectionSet; ++index)
        gameEntities.Add(new GameEntity(gameEntitiesTemp[index]));
    }

    public static void AddCommandLineFunction(string concatName) => EngineApplicationInterface.IUtil.AddCommandLineFunction(concatName);

    public static int GetNumberOfShaderCompilationsInProgress() => EngineApplicationInterface.IUtil.GetNumberOfShaderCompilationsInProgress();

    public static int IsDetailedSoundLogOn() => EngineApplicationInterface.IUtil.IsDetailedSoundLogOn();

    public static ulong GetCurrentCpuMemoryUsageMB() => EngineApplicationInterface.IUtil.GetCurrentCpuMemoryUsage();

    public static ulong GetGpuMemoryOfAllocationGroup(string name) => EngineApplicationInterface.IUtil.GetGpuMemoryOfAllocationGroup(name);

    public static void GetGPUMemoryStats(
      ref float totalMemory,
      ref float renderTargetMemory,
      ref float depthTargetMemory,
      ref float srvMemory,
      ref float bufferMemory)
    {
      EngineApplicationInterface.IUtil.GetGPUMemoryStats(ref totalMemory, ref renderTargetMemory, ref depthTargetMemory, ref srvMemory, ref bufferMemory);
    }

    public static void GetDetailedGPUMemoryData(
      ref int totalMemoryAllocated,
      ref int totalMemoryUsed,
      ref int emptyChunkTotalSize)
    {
      EngineApplicationInterface.IUtil.GetDetailedGPUBufferMemoryStats(ref totalMemoryAllocated, ref totalMemoryUsed, ref emptyChunkTotalSize);
    }

    public static void SetRenderMode(Utilities.EngineRenderDisplayMode mode) => EngineApplicationInterface.IUtil.SetRenderMode((int) mode);

    public static void AddPerformanceReportToken(
      string performance_type,
      string name,
      float loading_time)
    {
      EngineApplicationInterface.IUtil.AddPerformanceReportToken(performance_type, name, loading_time);
    }

    public static void AddSceneObjectReport(
      string scene_name,
      string report_name,
      float report_value)
    {
      EngineApplicationInterface.IUtil.AddSceneObjectReport(scene_name, report_name, report_value);
    }

    public static void OutputPerformanceReports() => EngineApplicationInterface.IUtil.OutputPerformanceReports();

    public static int EngineFrameNo => EngineApplicationInterface.IUtil.GetEngineFrameNo();

    public static bool EditModeEnabled => EngineApplicationInterface.IUtil.IsEditModeEnabled();

    public static void TakeScreenshot(string path) => EngineApplicationInterface.IUtil.TakeScreenshot(path);

    public static void SetAllocationAlwaysValidScene(Scene scene) => EngineApplicationInterface.IUtil.SetAllocationAlwaysValidScene((NativeObject) scene != (NativeObject) null ? scene.Pointer : UIntPtr.Zero);

    public static void CheckResourceModifications() => EngineApplicationInterface.IUtil.CheckResourceModifications();

    public static void SetGraphicsPreset(int preset) => EngineApplicationInterface.IUtil.SetGraphicsPreset(preset);

    public static string GetLocalOutputPath() => EngineApplicationInterface.IUtil.GetLocalOutputPath();

    public static string GetPCInfo() => EngineApplicationInterface.IUtil.GetPCInfo();

    public static int GetGPUMemoryMB() => EngineApplicationInterface.IUtil.GetGPUMemoryMB();

    public static int GetCurrentEstimatedGPUMemoryCostMB() => EngineApplicationInterface.IUtil.GetCurrentEstimatedGPUMemoryCostMB();

    public static void DumpGPUMemoryStatistics(string filePath) => EngineApplicationInterface.IUtil.DumpGPUMemoryStatistics(filePath);

    public static int SaveDataAsTexture(string path, int width, int height, float[] data) => EngineApplicationInterface.IUtil.SaveDataAsTexture(path, width, height, data);

    public static void ClearOldResourcesAndObjects() => EngineApplicationInterface.IUtil.ClearOldResourcesAndObjects();

    public static float GetDeltaTime(int timerId) => EngineApplicationInterface.IUtil.GetDeltaTime(timerId);

    public static void LoadSkyBoxes() => EngineApplicationInterface.IUtil.LoadSkyBoxes();

    public static string GetApplicationName() => EngineApplicationInterface.IUtil.GetApplicationName();

    public static void SetWindowTitle(string title) => EngineApplicationInterface.IUtil.SetWindowTitle(title);

    public static string ProcessWindowTitle(string title) => EngineApplicationInterface.IUtil.ProcessWindowTitle(title);

    public static uint GetCurrentProcessID() => EngineApplicationInterface.IUtil.GetCurrentProcessID();

    public static void DoDelayedexit(int returnCode) => EngineApplicationInterface.IUtil.DoDelayedexit(returnCode);

    public static void SetAssertionsAndWarningsSetExitCode(bool value) => EngineApplicationInterface.IUtil.SetAssertionsAndWarningsSetExitCode(value);

    public static void SetReportMode(bool reportMode) => EngineApplicationInterface.IUtil.SetReportMode(reportMode);

    public static void SetAssertionAtShaderCompile(bool value) => EngineApplicationInterface.IUtil.SetAssertionAtShaderCompile(value);

    public static string GetConfigsPath() => EngineApplicationInterface.IUtil.GetConfigsPath();

    public static string GetGameSavesPath() => EngineApplicationInterface.IUtil.GetGameSavesPath();

    public static string GetRecordingsPath() => EngineApplicationInterface.IUtil.GetRecordingsPath();

    public static string GetTempPath() => EngineApplicationInterface.IUtil.GetTempPath();

    public static void SetCrashReportCustomString(string customString) => EngineApplicationInterface.IUtil.SetCrashReportCustomString(customString);

    public static void SetCrashReportCustomStack(string customStack) => EngineApplicationInterface.IUtil.SetCrashReportCustomStack(customStack);

    public static int GetSteamAppId() => EngineApplicationInterface.IUtil.GetSteamAppId();

    public static void SetForceVsync(bool value)
    {
      TaleWorlds.Library.Debug.Print("Force VSync State is now " + (value ? "ACTIVE" : "DEACTIVATED"), color: TaleWorlds.Library.Debug.DebugColor.DarkBlue);
      EngineApplicationInterface.IUtil.SetForceVsync(value);
    }

    public static void SaveFile(string fileName, byte[] data) => EngineApplicationInterface.IUtil.SaveGameFile(fileName, data);

    public static byte[] LoadFile(string fileName)
    {
      ulong gameFileLength = EngineApplicationInterface.IUtil.GetGameFileLength(fileName);
      if (gameFileLength <= 0UL)
        return (byte[]) null;
      byte[] data = new byte[gameFileLength];
      EngineApplicationInterface.IUtil.LoadGameFile(fileName, data);
      return data;
    }

    public static byte[] LoadGameFileMetadata(string fileName)
    {
      ulong gameMetadataLength = EngineApplicationInterface.IUtil.GetGameMetadataLength(fileName);
      if (gameMetadataLength <= 0UL)
        return (byte[]) null;
      byte[] data = new byte[gameMetadataLength];
      EngineApplicationInterface.IUtil.LoadGameFileMetadata(fileName, data);
      return data;
    }

    public static List<Tuple<string, byte[]>> GetAllSaveFileMetadatas()
    {
      string allSaveFileNames = EngineApplicationInterface.IUtil.GetAllSaveFileNames();
      List<Tuple<string, byte[]>> tupleList = new List<Tuple<string, byte[]>>();
      if (!allSaveFileNames.IsStringNoneOrEmpty())
      {
        string str1 = allSaveFileNames;
        char[] chArray = new char[1]{ ':' };
        foreach (string fileName in str1.Split(chArray))
        {
          byte[] numArray = Utilities.LoadGameFileMetadata(fileName);
          if (numArray != null)
          {
            string str2 = fileName.EndsWith(".sav") ? fileName.Substring(0, fileName.Length - ".sav".Length) : fileName;
            tupleList.Add(new Tuple<string, byte[]>(str2, numArray));
          }
        }
      }
      return tupleList;
    }

    public static void DeleteSaveGameFile(string fileName) => EngineApplicationInterface.IUtil.DeleteSaveGameFile(fileName);

    private static string DefaultBannerlordConfigFullPath
    {
      get
      {
        string configsPath = Utilities.GetConfigsPath();
        return !string.IsNullOrEmpty(configsPath) ? configsPath + "BannerlordConfig.txt" : "";
      }
    }

    public static string LoadConfigFile(string fileName)
    {
      string bannerlordConfigFullPath = Utilities.DefaultBannerlordConfigFullPath;
      if (!File.Exists(bannerlordConfigFullPath))
        return "";
      StreamReader streamReader = new StreamReader(bannerlordConfigFullPath);
      string end = streamReader.ReadToEnd();
      streamReader.Close();
      return end;
    }

    public static void SaveConfigFile(string fileName, string configProperties)
    {
      string bannerlordConfigFullPath = Utilities.DefaultBannerlordConfigFullPath;
      try
      {
        File.WriteAllText(bannerlordConfigFullPath, configProperties.Substring(0, configProperties.Length - 1));
      }
      catch
      {
        Console.WriteLine("Could not create Bannerlord Config file");
      }
    }

    public static void OpenOnscreenKeyboard(
      string initialText,
      int maxLength,
      int keyboardTypeEnum)
    {
      EngineApplicationInterface.IUtil.OpenOnscreenKeyboard(initialText, maxLength, keyboardTypeEnum);
    }

    public static int RegisterGPUAllocationGroup(string name) => EngineApplicationInterface.IUtil.RegisterGPUAllocationGroup(name);

    public enum EngineRenderDisplayMode
    {
      ShowNone,
      ShowAlbedo,
      ShowNormals,
      ShowVertexNormals,
      ShowSpecular,
      ShowGloss,
      ShowOcclusion,
      ShowGbufferShadowMask,
      ShowTranslucency,
      ShowMotionVector,
      ShowVertexColor,
      ShowDepth,
      ShowTiledLightOverdraw,
      ShowTiledDecalOverdraw,
      ShowMaterialIds,
      ShowDisableSunLighting,
      ShowDebugTexture,
      ShowTextureDensity,
      ShowOverdraw,
      ShowVsComplexity,
      ShowPsComplexity,
      ShowDisableAmbientLighting,
      ShowEntityId,
      ShowPrtDiffuseAmbient,
      ShowLightDebugMode,
      ShowParticleShadingAtlas,
      ShowTerrainAngle,
      ShowParallaxDebug,
      ShowAlbedoValidation,
      NumDebugModes,
    }

    private class MainThreadJob
    {
      private Delegate _function;
      private object[] _parameters;
      private Semaphore wait_handle;

      internal MainThreadJob(Delegate function, object[] parameters)
      {
        this._function = function;
        this._parameters = parameters;
        this.wait_handle = (Semaphore) null;
      }

      internal MainThreadJob(Semaphore sema, Delegate function, object[] parameters)
      {
        this._function = function;
        this._parameters = parameters;
        this.wait_handle = sema;
      }

      internal void Invoke()
      {
        this._function.DynamicInvoke(this._parameters);
        if (this.wait_handle == null)
          return;
        this.wait_handle.Release();
      }
    }

    public class MainThreadPerformanceQuery : IDisposable
    {
      private string _name;
      private string _parent;
      private Stopwatch _stopWatch;

      public MainThreadPerformanceQuery(string parent, string name)
      {
        this._name = name;
        this._parent = parent;
        this._stopWatch = new Stopwatch();
        this._stopWatch.Start();
      }

      public void Dispose()
      {
        this._stopWatch.Stop();
        float seconds = (float) this._stopWatch.Elapsed.TotalMilliseconds / 1000f;
        EngineApplicationInterface.IUtil.AddMainThreadPerformanceQuery(this._parent, this._name, seconds);
      }
    }
  }
}
