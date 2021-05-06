// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.CompressionMatchmaker
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public static class CompressionMatchmaker
  {
    public static CompressionInfo.Integer KillDeathAssistCountCompressionInfo = new CompressionInfo.Integer(-1000, 100000, true);
    public static CompressionInfo.Float MissionTimeCompressionInfo = new CompressionInfo.Float(-5f, 86400f, 20);
    public static CompressionInfo.Float MissionTimeLowPrecisionCompressionInfo = new CompressionInfo.Float(-5f, 12, 4f);
    public static CompressionInfo.Integer MissionCurrentStateCompressionInfo = new CompressionInfo.Integer(0, 6);
    public static CompressionInfo.Integer ScoreCompressionInfo = new CompressionInfo.Integer(-1000000, 21);
  }
}
