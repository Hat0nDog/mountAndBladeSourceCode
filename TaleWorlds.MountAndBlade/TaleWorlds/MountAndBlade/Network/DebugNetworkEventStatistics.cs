// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.Network.DebugNetworkEventStatistics
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Network
{
  public static class DebugNetworkEventStatistics
  {
    private static DebugNetworkEventStatistics.TotalData _totalData = new DebugNetworkEventStatistics.TotalData();
    private static int _curEventType = -1;
    private static Dictionary<int, DebugNetworkEventStatistics.PerEventData> _statistics = new Dictionary<int, DebugNetworkEventStatistics.PerEventData>();
    private static int _samplesPerSecond;
    public static int MaxGraphPointCount;
    private static bool _showUploadDataText = false;
    private static bool _useAbsoluteMaximum = false;
    private static float _collectSampleCheck = 0.0f;
    private static float _collectFpsSampleCheck = 0.0f;
    private static float _curMaxGraphHeight = 0.0f;
    private static float _targetMaxGraphHeight = 0.0f;
    private static float _currMaxLossGraphHeight = 0.0f;
    private static float _targetMaxLossGraphHeight = 0.0f;
    private static DebugNetworkEventStatistics.PerSecondEventData UploadPerSecondEventData;
    private static readonly Queue<DebugNetworkEventStatistics.TotalEventData> _eventSamples = new Queue<DebugNetworkEventStatistics.TotalEventData>();
    private static readonly Queue<float> _lossSamples = new Queue<float>();
    private static DebugNetworkEventStatistics.TotalEventData _prevEventData = new DebugNetworkEventStatistics.TotalEventData();
    private static DebugNetworkEventStatistics.TotalEventData _currEventData = new DebugNetworkEventStatistics.TotalEventData();
    private static readonly List<float> _fpsSamplesUntilNextSampling = new List<float>();
    private static readonly Queue<float> _fpsSamples = new Queue<float>();
    private static bool _useImgui = !GameNetwork.IsDedicatedServer;
    public static bool TrackFps = false;

    public static event Action<IEnumerable<DebugNetworkEventStatistics.TotalEventData>> OnEventDataUpdated;

    public static event Action<DebugNetworkEventStatistics.PerSecondEventData> OnPerSecondEventDataUpdated;

    public static event Action<IEnumerable<float>> OnFPSEventUpdated;

    public static event Action OnOpenExternalMonitor;

    public static int SamplesPerSecond
    {
      get => DebugNetworkEventStatistics._samplesPerSecond;
      set
      {
        DebugNetworkEventStatistics._samplesPerSecond = value;
        DebugNetworkEventStatistics.MaxGraphPointCount = value * 5;
      }
    }

    public static bool IsActive { get; private set; }

    static DebugNetworkEventStatistics() => DebugNetworkEventStatistics.SamplesPerSecond = 10;

    internal static void StartEvent(string eventName, int eventType)
    {
      if (!DebugNetworkEventStatistics.IsActive)
        return;
      DebugNetworkEventStatistics._curEventType = eventType;
      if (!DebugNetworkEventStatistics._statistics.ContainsKey(DebugNetworkEventStatistics._curEventType))
        DebugNetworkEventStatistics._statistics.Add(DebugNetworkEventStatistics._curEventType, new DebugNetworkEventStatistics.PerEventData()
        {
          Name = eventName
        });
      ++DebugNetworkEventStatistics._statistics[DebugNetworkEventStatistics._curEventType].Count;
      ++DebugNetworkEventStatistics._totalData.TotalCount;
    }

    internal static void EndEvent()
    {
      if (!DebugNetworkEventStatistics.IsActive)
        return;
      DebugNetworkEventStatistics.PerEventData statistic = DebugNetworkEventStatistics._statistics[DebugNetworkEventStatistics._curEventType];
      statistic.DataSize = statistic.TotalDataSize / statistic.Count;
      DebugNetworkEventStatistics._curEventType = -1;
    }

    internal static void AddDataToStatistic(int bitCount)
    {
      if (!DebugNetworkEventStatistics.IsActive)
        return;
      DebugNetworkEventStatistics._statistics[DebugNetworkEventStatistics._curEventType].TotalDataSize += bitCount;
      DebugNetworkEventStatistics._totalData.TotalDataSize += bitCount;
    }

    public static void OpenExternalMonitor()
    {
      if (DebugNetworkEventStatistics.OnOpenExternalMonitor == null)
        return;
      DebugNetworkEventStatistics.OnOpenExternalMonitor();
    }

    public static void ControlActivate() => DebugNetworkEventStatistics.IsActive = true;

    public static void ControlDeactivate() => DebugNetworkEventStatistics.IsActive = false;

    public static void ControlJustDump() => DebugNetworkEventStatistics.DumpData();

    public static void ControlDumpAll()
    {
      DebugNetworkEventStatistics.DumpData();
      DebugNetworkEventStatistics.DumpReplicationData();
    }

    public static void ControlClear() => DebugNetworkEventStatistics.Clear();

    public static void ClearNetGraphs()
    {
      DebugNetworkEventStatistics._eventSamples.Clear();
      DebugNetworkEventStatistics._lossSamples.Clear();
      DebugNetworkEventStatistics._prevEventData = new DebugNetworkEventStatistics.TotalEventData();
      DebugNetworkEventStatistics._currEventData = new DebugNetworkEventStatistics.TotalEventData();
      DebugNetworkEventStatistics._collectSampleCheck = 0.0f;
    }

    public static void ClearFpsGraph()
    {
      DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Clear();
      DebugNetworkEventStatistics._fpsSamples.Clear();
      DebugNetworkEventStatistics._collectFpsSampleCheck = 0.0f;
    }

    public static void ControlClearAll()
    {
      DebugNetworkEventStatistics.Clear();
      DebugNetworkEventStatistics.ClearFpsGraph();
      DebugNetworkEventStatistics.ClearNetGraphs();
      DebugNetworkEventStatistics.ClearReplicationData();
    }

    public static void ControlDumpReplicationData()
    {
      DebugNetworkEventStatistics.DumpReplicationData();
      DebugNetworkEventStatistics.ClearReplicationData();
    }

    public static void EndTick(float dt)
    {
      if (DebugNetworkEventStatistics._useImgui && Input.DebugInput.IsHotKeyPressed("DebugNetworkEventStatisticsHotkeyToggleActive"))
      {
        DebugNetworkEventStatistics.ToggleActive();
        if (DebugNetworkEventStatistics.IsActive)
          Imgui.NewFrame();
      }
      if (!DebugNetworkEventStatistics.IsActive)
        return;
      DebugNetworkEventStatistics._totalData.TotalTime += dt;
      ++DebugNetworkEventStatistics._totalData.TotalFrameCount;
      if (DebugNetworkEventStatistics._useImgui)
      {
        Imgui.BeginMainThreadScope();
        Imgui.Begin("Network panel");
        if (Imgui.Button("Disable Network Panel"))
          DebugNetworkEventStatistics.ToggleActive();
        Imgui.Separator();
        if (Imgui.Button("Show Upload Data (screen)"))
          DebugNetworkEventStatistics._showUploadDataText = !DebugNetworkEventStatistics._showUploadDataText;
        Imgui.Separator();
        if (Imgui.Button("Clear Data"))
          DebugNetworkEventStatistics.Clear();
        if (Imgui.Button("Dump Data (console)"))
          DebugNetworkEventStatistics.DumpData();
        Imgui.Separator();
        if (Imgui.Button("Clear Replication Data"))
          DebugNetworkEventStatistics.ClearReplicationData();
        if (Imgui.Button("Dump Replication Data (console)"))
          DebugNetworkEventStatistics.DumpReplicationData();
        if (Imgui.Button("Dump & Clear Replication Data (console)"))
        {
          DebugNetworkEventStatistics.DumpReplicationData();
          DebugNetworkEventStatistics.ClearReplicationData();
        }
        if (DebugNetworkEventStatistics._showUploadDataText)
        {
          Imgui.Separator();
          DebugNetworkEventStatistics.ShowUploadData();
        }
        Imgui.End();
      }
      if (!DebugNetworkEventStatistics.IsActive)
        return;
      DebugNetworkEventStatistics.CollectFpsSample(dt);
      DebugNetworkEventStatistics._collectSampleCheck += dt;
      if ((double) DebugNetworkEventStatistics._collectSampleCheck >= 1.0 / (double) DebugNetworkEventStatistics.SamplesPerSecond)
      {
        DebugNetworkEventStatistics._currEventData = DebugNetworkEventStatistics.GetCurrentEventData();
        if (DebugNetworkEventStatistics._currEventData.HasData && DebugNetworkEventStatistics._prevEventData.HasData && DebugNetworkEventStatistics._currEventData != DebugNetworkEventStatistics._prevEventData)
        {
          DebugNetworkEventStatistics._lossSamples.Enqueue(GameNetwork.GetAveragePacketLossRatio());
          DebugNetworkEventStatistics._eventSamples.Enqueue(DebugNetworkEventStatistics._currEventData - DebugNetworkEventStatistics._prevEventData);
          DebugNetworkEventStatistics._prevEventData = DebugNetworkEventStatistics._currEventData;
          if (DebugNetworkEventStatistics._eventSamples.Count > DebugNetworkEventStatistics.MaxGraphPointCount)
          {
            DebugNetworkEventStatistics._eventSamples.Dequeue();
            double num = (double) DebugNetworkEventStatistics._lossSamples.Dequeue();
          }
          if (DebugNetworkEventStatistics._eventSamples.Count >= DebugNetworkEventStatistics.SamplesPerSecond)
          {
            List<DebugNetworkEventStatistics.TotalEventData> range = DebugNetworkEventStatistics._eventSamples.ToList<DebugNetworkEventStatistics.TotalEventData>().GetRange(DebugNetworkEventStatistics._eventSamples.Count - DebugNetworkEventStatistics.SamplesPerSecond, DebugNetworkEventStatistics.SamplesPerSecond);
            DebugNetworkEventStatistics.UploadPerSecondEventData = new DebugNetworkEventStatistics.PerSecondEventData(range.Sum<DebugNetworkEventStatistics.TotalEventData>((Func<DebugNetworkEventStatistics.TotalEventData, int>) (x => x.TotalUpload)), range.Sum<DebugNetworkEventStatistics.TotalEventData>((Func<DebugNetworkEventStatistics.TotalEventData, int>) (x => x.TotalConstantsUpload)), range.Sum<DebugNetworkEventStatistics.TotalEventData>((Func<DebugNetworkEventStatistics.TotalEventData, int>) (x => x.TotalReliableUpload)), range.Sum<DebugNetworkEventStatistics.TotalEventData>((Func<DebugNetworkEventStatistics.TotalEventData, int>) (x => x.TotalReplicationUpload)), range.Sum<DebugNetworkEventStatistics.TotalEventData>((Func<DebugNetworkEventStatistics.TotalEventData, int>) (x => x.TotalUnreliableUpload)), range.Sum<DebugNetworkEventStatistics.TotalEventData>((Func<DebugNetworkEventStatistics.TotalEventData, int>) (x => x.TotalOtherUpload)));
            if (DebugNetworkEventStatistics.OnPerSecondEventDataUpdated != null)
              DebugNetworkEventStatistics.OnPerSecondEventDataUpdated(DebugNetworkEventStatistics.UploadPerSecondEventData);
          }
          if (DebugNetworkEventStatistics.OnEventDataUpdated != null)
            DebugNetworkEventStatistics.OnEventDataUpdated((IEnumerable<DebugNetworkEventStatistics.TotalEventData>) DebugNetworkEventStatistics._eventSamples.ToList<DebugNetworkEventStatistics.TotalEventData>());
          DebugNetworkEventStatistics._collectSampleCheck -= 1f / (float) DebugNetworkEventStatistics.SamplesPerSecond;
        }
      }
      if (DebugNetworkEventStatistics._useImgui)
      {
        Imgui.Begin("Network Graph panel");
        float[] array = DebugNetworkEventStatistics._eventSamples.Select<DebugNetworkEventStatistics.TotalEventData, float>((Func<DebugNetworkEventStatistics.TotalEventData, float>) (x => (float) x.TotalUpload / 8192f)).ToArray<float>();
        float val1_1 = ((IEnumerable<float>) array).Any<float>() ? ((IEnumerable<float>) array).Max() : 0.0f;
        DebugNetworkEventStatistics._targetMaxGraphHeight = DebugNetworkEventStatistics._useAbsoluteMaximum ? Math.Max(val1_1, DebugNetworkEventStatistics._targetMaxGraphHeight) : val1_1;
        float amount1 = MBMath.ClampFloat(3f * dt, 0.0f, 1f);
        DebugNetworkEventStatistics._curMaxGraphHeight = MBMath.Lerp(DebugNetworkEventStatistics._curMaxGraphHeight, DebugNetworkEventStatistics._targetMaxGraphHeight, amount1);
        if (DebugNetworkEventStatistics.UploadPerSecondEventData != null)
          Imgui.Text("Taking " + (object) DebugNetworkEventStatistics.SamplesPerSecond + " samples per second. Total KiB per second:" + (object) (float) ((double) DebugNetworkEventStatistics.UploadPerSecondEventData.TotalUploadPerSecond / 8192.0));
        Imgui.PlotLines("", DebugNetworkEventStatistics._eventSamples.Select<DebugNetworkEventStatistics.TotalEventData, float>((Func<DebugNetworkEventStatistics.TotalEventData, float>) (x => (float) x.TotalConstantsUpload / 8192f)).ToArray<float>(), DebugNetworkEventStatistics._eventSamples.Count, 0, "Constants upload (in KiB)", 0.0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
        Imgui.SameLine();
        Imgui.Text("Y-range: " + (object) DebugNetworkEventStatistics._curMaxGraphHeight);
        Imgui.PlotLines("", DebugNetworkEventStatistics._eventSamples.Select<DebugNetworkEventStatistics.TotalEventData, float>((Func<DebugNetworkEventStatistics.TotalEventData, float>) (x => (float) x.TotalReliableUpload / 8192f)).ToArray<float>(), DebugNetworkEventStatistics._eventSamples.Count, 0, "Reliable upload (in KiB)", 0.0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
        Imgui.SameLine();
        Imgui.Text("Y-range: " + (object) DebugNetworkEventStatistics._curMaxGraphHeight);
        Imgui.PlotLines("", DebugNetworkEventStatistics._eventSamples.Select<DebugNetworkEventStatistics.TotalEventData, float>((Func<DebugNetworkEventStatistics.TotalEventData, float>) (x => (float) x.TotalReplicationUpload / 8192f)).ToArray<float>(), DebugNetworkEventStatistics._eventSamples.Count, 0, "Replication upload (in KiB)", 0.0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
        Imgui.SameLine();
        Imgui.Text("Y-range: " + (object) DebugNetworkEventStatistics._curMaxGraphHeight);
        Imgui.PlotLines("", DebugNetworkEventStatistics._eventSamples.Select<DebugNetworkEventStatistics.TotalEventData, float>((Func<DebugNetworkEventStatistics.TotalEventData, float>) (x => (float) x.TotalUnreliableUpload / 8192f)).ToArray<float>(), DebugNetworkEventStatistics._eventSamples.Count, 0, "Unreliable upload (in KiB)", 0.0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
        Imgui.SameLine();
        Imgui.Text("Y-range: " + (object) DebugNetworkEventStatistics._curMaxGraphHeight);
        Imgui.PlotLines("", DebugNetworkEventStatistics._eventSamples.Select<DebugNetworkEventStatistics.TotalEventData, float>((Func<DebugNetworkEventStatistics.TotalEventData, float>) (x => (float) x.TotalOtherUpload / 8192f)).ToArray<float>(), DebugNetworkEventStatistics._eventSamples.Count, 0, "Other upload (in KiB)", 0.0f, DebugNetworkEventStatistics._curMaxGraphHeight, 400f, 45f, 4);
        Imgui.SameLine();
        Imgui.Text("Y-range: " + (object) DebugNetworkEventStatistics._curMaxGraphHeight);
        Imgui.Separator();
        Imgui.PlotLines("", DebugNetworkEventStatistics._eventSamples.Select<DebugNetworkEventStatistics.TotalEventData, float>((Func<DebugNetworkEventStatistics.TotalEventData, float>) (x => (float) ((double) x.TotalUpload / (double) x.TotalPackets / 8.0))).ToArray<float>(), DebugNetworkEventStatistics._eventSamples.Count, 0, "Data per package (in B)", 0.0f, 1400f, 400f, 45f, 4);
        Imgui.SameLine();
        Imgui.Text("Y-range: " + (object) 1400);
        Imgui.Separator();
        float val1_2 = DebugNetworkEventStatistics._lossSamples.Any<float>() ? DebugNetworkEventStatistics._lossSamples.Max() : 0.0f;
        DebugNetworkEventStatistics._targetMaxLossGraphHeight = DebugNetworkEventStatistics._useAbsoluteMaximum ? Math.Max(val1_2, DebugNetworkEventStatistics._targetMaxLossGraphHeight) : val1_2;
        float amount2 = MBMath.ClampFloat(3f * dt, 0.0f, 1f);
        DebugNetworkEventStatistics._currMaxLossGraphHeight = MBMath.Lerp(DebugNetworkEventStatistics._currMaxLossGraphHeight, DebugNetworkEventStatistics._targetMaxLossGraphHeight, amount2);
        Imgui.PlotLines("", DebugNetworkEventStatistics._lossSamples.ToArray(), DebugNetworkEventStatistics._lossSamples.Count, 0, "Averaged loss ratio", 0.0f, DebugNetworkEventStatistics._currMaxLossGraphHeight, 400f, 45f, 4);
        Imgui.SameLine();
        Imgui.Text("Y-range: " + (object) DebugNetworkEventStatistics._currMaxLossGraphHeight);
        Imgui.Checkbox("Use absolute Maximum", ref DebugNetworkEventStatistics._useAbsoluteMaximum);
        Imgui.End();
      }
      Imgui.EndMainThreadScope();
    }

    private static void CollectFpsSample(float dt)
    {
      if (!DebugNetworkEventStatistics.TrackFps)
        return;
      float fps = Utilities.GetFps();
      if (!float.IsInfinity(fps) && !float.IsNegativeInfinity(fps) && !float.IsNaN(fps))
        DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Add(fps);
      DebugNetworkEventStatistics._collectFpsSampleCheck += dt;
      if ((double) DebugNetworkEventStatistics._collectFpsSampleCheck < 1.0 / (double) DebugNetworkEventStatistics.SamplesPerSecond)
        return;
      if (DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Any<float>())
      {
        DebugNetworkEventStatistics._fpsSamples.Enqueue(DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Min());
        DebugNetworkEventStatistics._fpsSamplesUntilNextSampling.Clear();
        if (DebugNetworkEventStatistics._fpsSamples.Count > DebugNetworkEventStatistics.MaxGraphPointCount)
        {
          double num = (double) DebugNetworkEventStatistics._fpsSamples.Dequeue();
        }
        if (DebugNetworkEventStatistics.OnFPSEventUpdated != null)
          DebugNetworkEventStatistics.OnFPSEventUpdated((IEnumerable<float>) DebugNetworkEventStatistics._fpsSamples.ToList<float>());
      }
      DebugNetworkEventStatistics._collectFpsSampleCheck -= 1f / (float) DebugNetworkEventStatistics.SamplesPerSecond;
    }

    private static void ToggleActive() => DebugNetworkEventStatistics.IsActive = !DebugNetworkEventStatistics.IsActive;

    private static void Clear()
    {
      DebugNetworkEventStatistics._totalData = new DebugNetworkEventStatistics.TotalData();
      DebugNetworkEventStatistics._statistics = new Dictionary<int, DebugNetworkEventStatistics.PerEventData>();
      GameNetwork.ResetDebugUploads();
      DebugNetworkEventStatistics._curEventType = -1;
    }

    private static void DumpData()
    {
      MBStringBuilder outStr = new MBStringBuilder();
      outStr.Initialize(callerMemberName: nameof (DumpData));
      outStr.AppendLine();
      outStr.AppendLine<string>("///GENERAL DATA///");
      outStr.AppendLine<string>("Total elapsed time: " + (object) DebugNetworkEventStatistics._totalData.TotalTime + " seconds.");
      outStr.AppendLine<string>("Total frame count: " + (object) DebugNetworkEventStatistics._totalData.TotalFrameCount);
      outStr.AppendLine<string>("Total avg packet count: " + (object) (int) ((double) DebugNetworkEventStatistics._totalData.TotalTime / 60.0));
      outStr.AppendLine<string>("Total event data size: " + (object) DebugNetworkEventStatistics._totalData.TotalDataSize + " bits.");
      outStr.AppendLine<string>("Total event count: " + (object) DebugNetworkEventStatistics._totalData.TotalCount);
      outStr.AppendLine();
      outStr.AppendLine<string>("///ALL DATA///");
      List<DebugNetworkEventStatistics.PerEventData> perEventDataList = new List<DebugNetworkEventStatistics.PerEventData>();
      perEventDataList.AddRange((IEnumerable<DebugNetworkEventStatistics.PerEventData>) DebugNetworkEventStatistics._statistics.Values);
      perEventDataList.Sort();
      foreach (DebugNetworkEventStatistics.PerEventData perEventData in perEventDataList)
      {
        outStr.AppendLine<string>("Event name: " + perEventData.Name);
        outStr.AppendLine<string>("\tEvent size (for one event): " + (object) perEventData.DataSize + " bits.");
        outStr.AppendLine<string>("\tTotal count: " + (object) perEventData.Count);
        outStr.AppendLine<string>("\tTotal size: " + (object) perEventData.TotalDataSize + "bits | ~" + (object) (perEventData.TotalDataSize / 8 + (perEventData.TotalDataSize % 8 == 0 ? 0 : 1)) + " bytes.");
        outStr.AppendLine<string>("\tTotal count per frame: " + (object) (float) ((double) perEventData.Count / (double) DebugNetworkEventStatistics._totalData.TotalFrameCount));
        outStr.AppendLine<string>("\tTotal size per frame: " + (object) (float) ((double) perEventData.TotalDataSize / (double) DebugNetworkEventStatistics._totalData.TotalFrameCount) + " bits per frame.");
        outStr.AppendLine();
      }
      DebugNetworkEventStatistics.GetFormattedDebugUploadDataOutput(ref outStr);
      outStr.AppendLine<string>("NetworkEventStaticticsLogLength: " + (object) outStr.Length + "\n");
      MBDebug.Print(outStr.ToStringAndRelease());
    }

    private static void GetFormattedDebugUploadDataOutput(ref MBStringBuilder outStr)
    {
      GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct = new GameNetwork.DebugNetworkPacketStatisticsStruct();
      GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct = new GameNetwork.DebugNetworkPositionCompressionStatisticsStruct();
      GameNetwork.GetDebugUploadsInBits(ref networkStatisticsStruct, ref posStatisticsStruct);
      outStr.AppendLine<string>("REAL NETWORK UPLOAD PERCENTS");
      if (networkStatisticsStruct.TotalUpload == 0)
      {
        outStr.AppendLine<string>("Total Upload is ZERO");
      }
      else
      {
        int num1 = networkStatisticsStruct.TotalUpload - (networkStatisticsStruct.TotalConstantsUpload + networkStatisticsStruct.TotalReliableEventUpload + networkStatisticsStruct.TotalReplicationUpload + networkStatisticsStruct.TotalUnreliableEventUpload);
        if (num1 == networkStatisticsStruct.TotalUpload)
        {
          outStr.AppendLine<string>("USE_DEBUG_NETWORK_PACKET_PERCENTS not defined!");
        }
        else
        {
          outStr.AppendLine<string>("\tAverage Ping: " + (object) networkStatisticsStruct.average_ping_time);
          outStr.AppendLine<string>("\tTime out period: " + (object) networkStatisticsStruct.time_out_period);
          outStr.AppendLine<string>("\tLost Percent: " + (object) networkStatisticsStruct.lost_percent);
          outStr.AppendLine<string>("\tlost_count: " + (object) networkStatisticsStruct.lost_count);
          outStr.AppendLine<string>("\ttotal_count_on_lost_check: " + (object) networkStatisticsStruct.total_count_on_lost_check);
          outStr.AppendLine<string>("\tround_trip_time: " + (object) networkStatisticsStruct.round_trip_time);
          float totalUpload = (float) networkStatisticsStruct.TotalUpload;
          float num2 = 1f / (float) networkStatisticsStruct.TotalPackets;
          outStr.AppendLine<string>("\tConstants Upload: percent: " + (object) (float) ((double) networkStatisticsStruct.TotalConstantsUpload / (double) totalUpload * 100.0) + "; size in bits: " + (object) (float) ((double) networkStatisticsStruct.TotalConstantsUpload * (double) num2) + ";");
          outStr.AppendLine<string>("\tReliable Upload: percent: " + (object) (float) ((double) networkStatisticsStruct.TotalReliableEventUpload / (double) totalUpload * 100.0) + "; size in bits: " + (object) (float) ((double) networkStatisticsStruct.TotalReliableEventUpload * (double) num2) + ";");
          outStr.AppendLine<string>("\tReplication Upload: percent: " + (object) (float) ((double) networkStatisticsStruct.TotalReplicationUpload / (double) totalUpload * 100.0) + "; size in bits: " + (object) (float) ((double) networkStatisticsStruct.TotalReplicationUpload * (double) num2) + ";");
          outStr.AppendLine<string>("\tUnreliable Upload: percent: " + (object) (float) ((double) networkStatisticsStruct.TotalUnreliableEventUpload / (double) totalUpload * 100.0) + "; size in bits: " + (object) (float) ((double) networkStatisticsStruct.TotalUnreliableEventUpload * (double) num2) + ";");
          outStr.AppendLine<string>("\tOthers (headers, ack etc.) Upload: percent: " + (object) (float) ((double) num1 / (double) totalUpload * 100.0) + "; size in bits: " + (object) (float) ((double) num1 * (double) num2) + ";");
          int num3 = posStatisticsStruct.totalPositionCoarseBitCountX + posStatisticsStruct.totalPositionCoarseBitCountY + posStatisticsStruct.totalPositionCoarseBitCountZ;
          double num4 = 1.0 / (double) networkStatisticsStruct.debug_total_cell_priority_checks;
          outStr.AppendLine<string>("\n\tTotal PPS: " + (object) (float) ((double) networkStatisticsStruct.TotalPackets / (double) DebugNetworkEventStatistics._totalData.TotalTime) + "; bps: " + (object) (float) ((double) networkStatisticsStruct.TotalUpload / (double) DebugNetworkEventStatistics._totalData.TotalTime) + ";");
        }
        outStr.AppendLine<string>("\n\tTotal packets: " + (object) networkStatisticsStruct.TotalPackets + "; bits per packet: " + (object) (float) ((double) networkStatisticsStruct.TotalUpload / (double) networkStatisticsStruct.TotalPackets) + ";");
        outStr.AppendLine<string>("Total Upload: " + (object) networkStatisticsStruct.TotalUpload + " in bits");
      }
    }

    private static void ShowUploadData()
    {
      MBStringBuilder outStr = new MBStringBuilder();
      outStr.Initialize(callerMemberName: nameof (ShowUploadData));
      DebugNetworkEventStatistics.GetFormattedDebugUploadDataOutput(ref outStr);
      string stringAndRelease = outStr.ToStringAndRelease();
      char[] chArray = new char[1]{ '\n' };
      foreach (string text in stringAndRelease.Split(chArray))
        Imgui.Text(text);
    }

    private static DebugNetworkEventStatistics.TotalEventData GetCurrentEventData()
    {
      GameNetwork.DebugNetworkPacketStatisticsStruct networkStatisticsStruct = new GameNetwork.DebugNetworkPacketStatisticsStruct();
      GameNetwork.DebugNetworkPositionCompressionStatisticsStruct posStatisticsStruct = new GameNetwork.DebugNetworkPositionCompressionStatisticsStruct();
      GameNetwork.GetDebugUploadsInBits(ref networkStatisticsStruct, ref posStatisticsStruct);
      return new DebugNetworkEventStatistics.TotalEventData(networkStatisticsStruct.TotalPackets, networkStatisticsStruct.TotalUpload, networkStatisticsStruct.TotalConstantsUpload, networkStatisticsStruct.TotalReliableEventUpload, networkStatisticsStruct.TotalReplicationUpload, networkStatisticsStruct.TotalUnreliableEventUpload);
    }

    private static void DumpReplicationData() => GameNetwork.PrintReplicationTableStatistics();

    private static void ClearReplicationData() => GameNetwork.ClearReplicationTableStatistics();

    public class TotalEventData
    {
      public readonly int TotalPackets;
      public readonly int TotalUpload;
      public readonly int TotalConstantsUpload;
      public readonly int TotalReliableUpload;
      public readonly int TotalReplicationUpload;
      public readonly int TotalUnreliableUpload;
      public readonly int TotalOtherUpload;

      protected bool Equals(DebugNetworkEventStatistics.TotalEventData other) => this.TotalPackets == other.TotalPackets && this.TotalUpload == other.TotalUpload && (this.TotalConstantsUpload == other.TotalConstantsUpload && this.TotalReliableUpload == other.TotalReliableUpload) && (this.TotalReplicationUpload == other.TotalReplicationUpload && this.TotalUnreliableUpload == other.TotalUnreliableUpload) && this.TotalOtherUpload == other.TotalOtherUpload;

      public override bool Equals(object obj)
      {
        if (obj == null)
          return false;
        if ((object) this == obj)
          return true;
        return obj.GetType() == this.GetType() && this.Equals((DebugNetworkEventStatistics.TotalEventData) obj);
      }

      public override int GetHashCode() => (((((this.TotalPackets * 397 ^ this.TotalUpload) * 397 ^ this.TotalConstantsUpload) * 397 ^ this.TotalReliableUpload) * 397 ^ this.TotalReplicationUpload) * 397 ^ this.TotalUnreliableUpload) * 397 ^ this.TotalOtherUpload;

      public TotalEventData()
      {
      }

      public TotalEventData(
        int totalPackets,
        int totalUpload,
        int totalConstants,
        int totalReliable,
        int totalReplication,
        int totalUnreliable)
      {
        this.TotalPackets = totalPackets;
        this.TotalUpload = totalUpload;
        this.TotalConstantsUpload = totalConstants;
        this.TotalReliableUpload = totalReliable;
        this.TotalReplicationUpload = totalReplication;
        this.TotalUnreliableUpload = totalUnreliable;
        this.TotalOtherUpload = totalUpload - (totalConstants + totalReliable + totalReplication + totalUnreliable);
      }

      public bool HasData => this.TotalUpload > 0;

      public static DebugNetworkEventStatistics.TotalEventData operator -(
        DebugNetworkEventStatistics.TotalEventData d1,
        DebugNetworkEventStatistics.TotalEventData d2)
      {
        return new DebugNetworkEventStatistics.TotalEventData(d1.TotalPackets - d2.TotalPackets, d1.TotalUpload - d2.TotalUpload, d1.TotalConstantsUpload - d2.TotalConstantsUpload, d1.TotalReliableUpload - d2.TotalReliableUpload, d1.TotalReplicationUpload - d2.TotalReplicationUpload, d1.TotalUnreliableUpload - d2.TotalUnreliableUpload);
      }

      public static bool operator ==(
        DebugNetworkEventStatistics.TotalEventData d1,
        DebugNetworkEventStatistics.TotalEventData d2)
      {
        return d1.TotalPackets == d2.TotalPackets && d1.TotalUpload == d2.TotalUpload && (d1.TotalConstantsUpload == d2.TotalConstantsUpload && d1.TotalReliableUpload == d2.TotalReliableUpload) && d1.TotalReplicationUpload == d2.TotalReplicationUpload && d1.TotalUnreliableUpload == d2.TotalUnreliableUpload;
      }

      public static bool operator !=(
        DebugNetworkEventStatistics.TotalEventData d1,
        DebugNetworkEventStatistics.TotalEventData d2)
      {
        return !(d1 == d2);
      }
    }

    private class PerEventData : IComparable<DebugNetworkEventStatistics.PerEventData>
    {
      public string Name;
      public int DataSize;
      public int TotalDataSize;
      public int Count;

      public int CompareTo(DebugNetworkEventStatistics.PerEventData other) => other.TotalDataSize - this.TotalDataSize;
    }

    public class PerSecondEventData
    {
      public readonly int TotalUploadPerSecond;
      public readonly int ConstantsUploadPerSecond;
      public readonly int ReliableUploadPerSecond;
      public readonly int ReplicationUploadPerSecond;
      public readonly int UnreliableUploadPerSecond;
      public readonly int OtherUploadPerSecond;

      public PerSecondEventData(
        int totalUploadPerSecond,
        int constantsUploadPerSecond,
        int reliableUploadPerSecond,
        int replicationUploadPerSecond,
        int unreliableUploadPerSecond,
        int otherUploadPerSecond)
      {
        this.TotalUploadPerSecond = totalUploadPerSecond;
        this.ConstantsUploadPerSecond = constantsUploadPerSecond;
        this.ReliableUploadPerSecond = reliableUploadPerSecond;
        this.ReplicationUploadPerSecond = replicationUploadPerSecond;
        this.UnreliableUploadPerSecond = unreliableUploadPerSecond;
        this.OtherUploadPerSecond = otherUploadPerSecond;
      }
    }

    private class TotalData
    {
      public float TotalTime;
      public int TotalFrameCount;
      public int TotalCount;
      public int TotalDataSize;
    }
  }
}
