// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Core.MountCreationKey
// Assembly: TaleWorlds.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 43F7B45C-9AE1-4715-A37B-2364840F62AF
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Core.dll

using System;
using System.Globalization;

namespace TaleWorlds.Core
{
  public class MountCreationKey
  {
    private const int NumLegColors = 4;

    public byte _leftFrontLegColorIndex { get; private set; }

    public byte _rightFrontLegColorIndex { get; private set; }

    public byte _leftBackLegColorIndex { get; private set; }

    public byte _rightBackLegColorIndex { get; private set; }

    public byte MaterialIndex { get; private set; }

    public byte MeshMultiplayerIndex { get; private set; }

    public MountCreationKey(
      byte leftFrontLegColorIndex,
      byte rightFrontLegColorIndex,
      byte leftBackLegColorIndex,
      byte rightBackLegColorIndex,
      byte materialIndex,
      byte meshMultiplayerIndex)
    {
      if (leftFrontLegColorIndex == (byte) 3 || rightFrontLegColorIndex == (byte) 3)
      {
        leftFrontLegColorIndex = (byte) 3;
        rightFrontLegColorIndex = (byte) 3;
      }
      this._leftFrontLegColorIndex = leftFrontLegColorIndex;
      this._rightFrontLegColorIndex = rightFrontLegColorIndex;
      this._leftBackLegColorIndex = leftBackLegColorIndex;
      this._rightBackLegColorIndex = rightBackLegColorIndex;
      this.MaterialIndex = materialIndex;
      this.MeshMultiplayerIndex = meshMultiplayerIndex;
    }

    public static MountCreationKey FromString(string str)
    {
      if (str == null)
        return new MountCreationKey((byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0, (byte) 0);
      int num = (int) uint.Parse(str, NumberStyles.HexNumber);
      return new MountCreationKey((byte) MountCreationKey.GetBitsFromKey((uint) num, 0, 2), (byte) MountCreationKey.GetBitsFromKey((uint) num, 2, 2), (byte) MountCreationKey.GetBitsFromKey((uint) num, 4, 2), (byte) MountCreationKey.GetBitsFromKey((uint) num, 6, 2), (byte) MountCreationKey.GetBitsFromKey((uint) num, 8, 2), (byte) MountCreationKey.GetBitsFromKey((uint) num, 10, 2));
    }

    public override string ToString()
    {
      uint numericKey = 0;
      this.SetBits(ref numericKey, (int) this._leftFrontLegColorIndex, 0);
      this.SetBits(ref numericKey, (int) this._rightFrontLegColorIndex, 2);
      this.SetBits(ref numericKey, (int) this._leftBackLegColorIndex, 4);
      this.SetBits(ref numericKey, (int) this._rightBackLegColorIndex, 6);
      this.SetBits(ref numericKey, (int) this.MaterialIndex, 8);
      this.SetBits(ref numericKey, (int) this.MeshMultiplayerIndex, 10);
      return numericKey.ToString("X");
    }

    private static int GetBitsFromKey(uint numericKey, int startingBit, int numBits) => (int) (numericKey >> startingBit) & (int) ((uint) Math.Pow((double) numBits, 2.0) - 1U);

    private void SetBits(ref uint numericKey, int value, int startingBit)
    {
      uint num = (uint) value << startingBit;
      numericKey |= num;
    }

    public static string GetRandomMountKey(ItemObject mountItem, int randomSeed)
    {
      Random random = new Random(randomSeed);
      if (mountItem == null)
        return new MountCreationKey((byte) random.Next(4), (byte) random.Next(4), (byte) random.Next(4), (byte) random.Next(4), (byte) 0, (byte) 0).ToString();
      HorseComponent horseComponent = mountItem.HorseComponent;
      if (horseComponent.HorseMaterialNames == null || horseComponent.HorseMaterialNames.Count <= 0)
        return new MountCreationKey((byte) MBRandom.RandomInt(4), (byte) MBRandom.RandomInt(4), (byte) MBRandom.RandomInt(4), (byte) MBRandom.RandomInt(4), (byte) 0, (byte) 0).ToString();
      int index1 = random.Next(horseComponent.HorseMaterialNames.Count);
      float num1 = random.NextFloat();
      int num2 = 0;
      float num3 = 0.0f;
      HorseComponent.MaterialProperty horseMaterialName = horseComponent.HorseMaterialNames[index1];
      for (int index2 = 0; index2 < horseMaterialName.MeshMultiplier.Count; ++index2)
      {
        num3 += horseMaterialName.MeshMultiplier[index2].Item2;
        if ((double) num1 <= (double) num3)
        {
          num2 = index2;
          break;
        }
      }
      return new MountCreationKey((byte) random.Next(4), (byte) random.Next(4), (byte) random.Next(4), (byte) random.Next(4), (byte) index1, (byte) num2).ToString();
    }
  }
}
