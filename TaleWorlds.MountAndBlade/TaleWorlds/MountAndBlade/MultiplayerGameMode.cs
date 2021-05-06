// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.MultiplayerGameMode
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.MountAndBlade.Diamond;

namespace TaleWorlds.MountAndBlade
{
  public abstract class MultiplayerGameMode
  {
    public string Name { get; private set; }

    protected MultiplayerGameMode(string name) => this.Name = name;

    public abstract void JoinCustomGame(JoinGameData joinGameData);

    public abstract void StartMultiplayerGame(string scene);
  }
}
