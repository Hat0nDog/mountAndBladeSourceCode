// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.IFaceGeneratorHandler
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

namespace TaleWorlds.MountAndBlade
{
  public interface IFaceGeneratorHandler
  {
    void ChangeToBodyCamera();

    void ChangeToEyeCamera();

    void ChangeToNoseCamera();

    void ChangeToMouthCamera();

    void ChangeToFaceCamera();

    void ChangeToHairCamera();

    void RefreshCharacterEntity();

    void MakeVoice(int gender, float pitch);

    void SetFacialAnimation(string faceAnimation, bool loop);

    void Done();

    void Cancel();

    void UndressCharacterEntity();

    void DressCharacterEntity();
  }
}
