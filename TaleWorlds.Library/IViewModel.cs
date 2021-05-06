// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.IViewModel
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System.ComponentModel;

namespace TaleWorlds.Library
{
  public interface IViewModel : INotifyPropertyChanged
  {
    object GetViewModelAtPath(BindingPath path);

    object GetViewModelAtPath(BindingPath path, bool isList);

    object GetPropertyValue(string name);

    object GetPropertyValue(string name, PropertyTypeFeeder propertyTypeFeeder);

    void SetPropertyValue(string name, object value);

    void ExecuteCommand(string commandName, object[] parameters);

    event PropertyChangedWithValueEventHandler PropertyChangedWithValue;
  }
}
