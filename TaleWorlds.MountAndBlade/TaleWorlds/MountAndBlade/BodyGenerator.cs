// Decompiled with JetBrains decompiler
// Type: TaleWorlds.MountAndBlade.BodyGenerator
// Assembly: TaleWorlds.MountAndBlade, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C0E3F42-756F-4407-B817-06BA412A78B0
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.MountAndBlade.dll

using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TaleWorlds.MountAndBlade
{
  public class BodyGenerator
  {
    public const string FaceGenTeethAnimationName = "facegen_teeth";
    public BodyProperties CurrentBodyProperties;
    public BodyProperties BodyPropertiesMin;
    public BodyProperties BodyPropertiesMax;
    public bool IsFemale;

    public BasicCharacterObject Character { get; private set; }

    public BodyGenerator(BasicCharacterObject troop)
    {
      this.Character = troop;
      this.SetCharacter(troop);
      this.IsFemale = this.Character.IsFemale;
    }

    public FaceGenerationParams InitBodyGenerator()
    {
      this.CurrentBodyProperties = this.Character.GetBodyProperties(this.Character.Equipment);
      FaceGenerationParams faceGenerationParams = FaceGenerationParams.Create();
      faceGenerationParams._currentGender = this.Character.IsFemale ? 1 : 0;
      faceGenerationParams._curAge = this.Character.Age;
      MBBodyProperties.GetParamsFromKey(ref faceGenerationParams, this.CurrentBodyProperties, this.Character.Equipment.EarsAreHidden);
      faceGenerationParams.SetGenderAndAdjustParams(faceGenerationParams._currentGender, (int) faceGenerationParams._curAge);
      return faceGenerationParams;
    }

    public void RefreshFace(FaceGenerationParams faceGenerationParams)
    {
      MBBodyProperties.ProduceNumericKeyWithParams(faceGenerationParams, this.Character.Equipment.EarsAreHidden, ref this.CurrentBodyProperties);
      this.IsFemale = faceGenerationParams._currentGender == 1;
    }

    public void SetCharacter(BasicCharacterObject character)
    {
      this.Character = character;
      Game.Current.LastFaceEditedCharacter = character;
      MBDebug.Print("FaceGen set character> character face key: " + (object) character.GetBodyProperties(character.Equipment));
    }

    public void SaveCurrentCharacter()
    {
      this.Character.UpdatePlayerCharacterBodyProperties(this.CurrentBodyProperties, this.IsFemale);
      this.SaveTraitChanges();
    }

    private void SaveTraitChanges()
    {
    }
  }
}
