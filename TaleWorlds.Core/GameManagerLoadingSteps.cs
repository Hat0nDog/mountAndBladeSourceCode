// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.GameManagerLoadingSteps
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

namespace TaleWorlds.Core
{
  public enum GameManagerLoadingSteps
  {
    None = -1, // 0xFFFFFFFF
    PreInitializeZerothStep = 0,
    FirstInitializeFirstStep = 1,
    WaitSecondStep = 2,
    SecondInitializeThirdState = 3,
    PostInitializeFourthState = 4,
    FinishLoadingFifthStep = 5,
    LoadingIsOver = 6,
  }
}
