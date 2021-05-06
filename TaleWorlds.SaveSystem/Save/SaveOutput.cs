// Decompiled with JetBrains decompiler
// Type: TaleWorlds.SaveSystem.Save.SaveOutput
// Assembly: TaleWorlds.SaveSystem, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 786EF53D-D2EC-43A3-8B8C-4717B7406D76
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.SaveSystem.dll

using System.Collections.Generic;
using System.Linq;
using TaleWorlds.Library;

namespace TaleWorlds.SaveSystem.Save
{
  public class SaveOutput
  {
    public GameData Data { get; private set; }

    public bool Successful { get; private set; }

    public SaveError[] Errors { get; private set; }

    private SaveOutput()
    {
    }

    internal static SaveOutput CreateSuccessful(GameData data) => new SaveOutput()
    {
      Data = data,
      Successful = true
    };

    internal static SaveOutput CreateFailed(IEnumerable<SaveError> errors) => new SaveOutput()
    {
      Successful = false,
      Errors = errors.ToArray<SaveError>()
    };

    public void PrintStatus()
    {
      if (this.Successful)
      {
        Debug.Print("Successfully saved");
      }
      else
      {
        Debug.Print("Couldn't save because of errors listed below.");
        for (int index = 0; index < this.Errors.Length; ++index)
        {
          SaveError error = this.Errors[index];
          Debug.Print("[" + (object) index + "]" + error.Message);
        }
        Debug.Print("--------------------");
      }
    }
  }
}
