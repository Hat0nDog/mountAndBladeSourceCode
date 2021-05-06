// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.FilePaths
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;

namespace TaleWorlds.Core
{
  public static class FilePaths
  {
    public const string ApplicationName = "Mount and Blade II Bannerlord";
    public const string ModuleName = "Native";
    public const string SaveDirectoryName = "Game Saves";
    public const string RecordingsDirectoryName = "Recordings";

    public static string SavePath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Mount and Blade II Bannerlord\\Game Saves\\";

    public static string RecordingsPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\Mount and Blade II Bannerlord\\Recordings\\";
  }
}
