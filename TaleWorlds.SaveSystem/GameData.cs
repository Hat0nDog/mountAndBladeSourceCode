// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.GameData
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem
{
  [Serializable]
  public class GameData
  {
    public byte[] Header { get; private set; }

    public byte[] Strings { get; private set; }

    public byte[][] ObjectData { get; private set; }

    public byte[][] ContainerData { get; private set; }

    public int TotalSize
    {
      get
      {
        int num = this.Header.Length + this.Strings.Length;
        for (int index = 0; index < this.ObjectData.Length; ++index)
          num += this.ObjectData[index].Length;
        for (int index = 0; index < this.ContainerData.Length; ++index)
          num += this.ContainerData[index].Length;
        return num;
      }
    }

    public GameData(byte[] header, byte[] strings, byte[][] objectData, byte[][] containerData)
    {
      this.Header = header;
      this.Strings = strings;
      this.ObjectData = objectData;
      this.ContainerData = containerData;
    }

    public void Inspect()
    {
      Debug.Print("Header Size: " + (object) this.Header.Length);
      Debug.Print("Strings Size: " + (object) this.Strings.Length);
      Debug.Print("Object Count: " + (object) this.ObjectData.Length);
      Debug.Print("Container Count: " + (object) this.ContainerData.Length);
      int num1 = 0;
      for (int index = 0; index < this.ObjectData.Length; ++index)
      {
        int length = this.ObjectData[index].Length;
        if (length > num1)
          num1 = length;
      }
      Debug.Print("Highest Object Size: " + (object) num1);
      int num2 = 0;
      for (int index = 0; index < this.ContainerData.Length; ++index)
      {
        int length = this.ContainerData[index].Length;
        if (length > num2)
          num2 = length;
      }
      Debug.Print("Highest Container Size: " + (object) num2);
      Debug.Print(string.Format("Total size: {0:##.00} MB", (object) ((float) this.TotalSize / 1048576f)));
    }

    public static GameData CreateFrom(byte[] readBytes) => (GameData) Common.DeserializeObject(readBytes);

    public byte[] GetData() => Common.SerializeObject((object) this);
  }
}
