// Decompiled with JetBrains decompiler
// Type: TaleWorlds.Library.ViewModel
// Assembly: TaleWorlds.Library, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 271D0DB8-3C5D-4850-A565-542B4CF7D347
// Assembly location: E:\SteamLibrary\steamapps\common\Mount & Blade II Bannerlord\bin\Win64_Shipping_Client\TaleWorlds.Library.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TaleWorlds.Library
{
  public abstract class ViewModel : IViewModel, INotifyPropertyChanged
  {
    public static bool UIDebugMode;
    private List<PropertyChangedEventHandler> _eventHandlers;
    private List<PropertyChangedWithValueEventHandler> _eventHandlersWithValue;
    private Type _type;
    private Dictionary<string, PropertyInfo> _propertyInfos;

    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        if (this._eventHandlers == null)
          this._eventHandlers = new List<PropertyChangedEventHandler>();
        this._eventHandlers.Add(value);
      }
      remove
      {
        if (this._eventHandlers == null)
          return;
        this._eventHandlers.Remove(value);
      }
    }

    public event PropertyChangedWithValueEventHandler PropertyChangedWithValue
    {
      add
      {
        if (this._eventHandlersWithValue == null)
          this._eventHandlersWithValue = new List<PropertyChangedWithValueEventHandler>();
        this._eventHandlersWithValue.Add(value);
      }
      remove
      {
        if (this._eventHandlersWithValue == null)
          return;
        this._eventHandlersWithValue.Remove(value);
      }
    }

    protected ViewModel()
    {
      this._type = this.GetType();
      PropertyInfo[] properties = this._type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      this._propertyInfos = new Dictionary<string, PropertyInfo>(properties.Length);
      foreach (PropertyInfo propertyInfo in properties)
        this._propertyInfos.Add(propertyInfo.Name, propertyInfo);
    }

    private PropertyInfo GetProperty(string name)
    {
      PropertyInfo propertyInfo;
      this._propertyInfos.TryGetValue(name, out propertyInfo);
      return propertyInfo;
    }

    protected bool SetField<T>(ref T field, T value, string propertyName)
    {
      if (EqualityComparer<T>.Default.Equals(field, value))
        return false;
      field = value;
      this.OnPropertyChanged(propertyName);
      return true;
    }

    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      if (this._eventHandlers == null)
        return;
      for (int index = 0; index < this._eventHandlers.Count; ++index)
        this._eventHandlers[index]((object) this, new PropertyChangedEventArgs(propertyName));
    }

    public void OnPropertyChangedWithValue(object value, [CallerMemberName] string propertyName = null)
    {
      if (this._eventHandlersWithValue == null)
        return;
      for (int index = 0; index < this._eventHandlersWithValue.Count; ++index)
        this._eventHandlersWithValue[index]((object) this, new PropertyChangedWithValueEventArgs(propertyName, value));
    }

    public object GetViewModelAtPath(BindingPath path, bool isList) => this.GetViewModelAtPath(path);

    public object GetViewModelAtPath(BindingPath path)
    {
      BindingPath subPath = path.SubPath;
      if (!(subPath != (BindingPath) null))
        return (object) this;
      PropertyInfo property = this.GetProperty(subPath.FirstNode);
      if (property != (PropertyInfo) null)
      {
        object obj = property.GetGetMethod().InvokeWithLog((object) this, (object[]) null);
        switch (obj)
        {
          case ViewModel viewModel2:
            return viewModel2.GetViewModelAtPath(subPath);
          case IMBBindingList _:
            return ViewModel.GetChildAtPath(obj as IMBBindingList, subPath);
        }
      }
      return (object) null;
    }

    private static object GetChildAtPath(IMBBindingList bindingList, BindingPath path)
    {
      BindingPath subPath = path.SubPath;
      if (subPath == (BindingPath) null)
        return (object) bindingList;
      if (bindingList.Count > 0)
      {
        int int32 = Convert.ToInt32(subPath.FirstNode);
        if (int32 >= 0 && int32 < bindingList.Count)
        {
          object binding = bindingList[int32];
          switch (binding)
          {
            case ViewModel _:
              return (binding as ViewModel).GetViewModelAtPath(subPath);
            case IMBBindingList _:
              return ViewModel.GetChildAtPath(binding as IMBBindingList, subPath);
          }
        }
      }
      return (object) null;
    }

    public object GetPropertyValue(string name, PropertyTypeFeeder propertyTypeFeeder) => this.GetPropertyValue(name);

    public object GetPropertyValue(string name)
    {
      PropertyInfo property = this.GetProperty(name);
      object obj = (object) null;
      if (property != (PropertyInfo) null)
        obj = property.GetGetMethod().InvokeWithLog((object) this, (object[]) null);
      return obj;
    }

    public Type GetPropertyType(string name)
    {
      PropertyInfo property = this.GetProperty(name);
      return property != (PropertyInfo) null ? property.PropertyType : (Type) null;
    }

    public void SetPropertyValue(string name, object value)
    {
      PropertyInfo property = this.GetProperty(name);
      if (!(property != (PropertyInfo) null))
        return;
      MethodInfo setMethod = property.GetSetMethod();
      if ((object) setMethod == null)
        return;
      setMethod.InvokeWithLog((object) this, value);
    }

    public string[] Properties
    {
      get
      {
        List<string> stringList = new List<string>();
        foreach (PropertyInfo propertyInfo in this._propertyInfos.Values)
        {
          object[] customAttributes = propertyInfo.GetCustomAttributes(typeof (DataSourceProperty), true);
          if ((customAttributes != null ? ((uint) customAttributes.Length > 0U ? 1 : 0) : 0) != 0)
            stringList.Add(propertyInfo.Name);
        }
        return stringList.ToArray();
      }
    }

    public virtual void OnFinalize()
    {
    }

    public void ExecuteCommand(string commandName, object[] parameters)
    {
      MethodInfo method = this._type.GetMethod(commandName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      if (!(method != (MethodInfo) null))
        return;
      if (method.GetParameters().Length == parameters.Length)
      {
        object[] objArray = new object[parameters.Length];
        ParameterInfo[] parameters1 = method.GetParameters();
        for (int index = 0; index < parameters.Length; ++index)
        {
          object parameter = parameters[index];
          Type parameterType = parameters1[index].ParameterType;
          objArray[index] = parameter;
          if (parameter is string && parameterType != typeof (string))
          {
            object obj = ViewModel.ConvertValueTo((string) parameter, parameterType);
            objArray[index] = obj;
          }
        }
        method.InvokeWithLog((object) this, objArray);
      }
      else
      {
        if (method.GetParameters().Length != 0)
          return;
        method.InvokeWithLog((object) this, (object[]) null);
      }
    }

    private static object ConvertValueTo(string value, Type parameterType)
    {
      object obj = (object) null;
      if (parameterType == typeof (string))
        obj = (object) value;
      else if (parameterType == typeof (int))
        obj = (object) Convert.ToInt32(value);
      else if (parameterType == typeof (float))
        obj = (object) Convert.ToSingle(value);
      return obj;
    }

    public virtual void RefreshValues()
    {
    }

    public interface IViewModelGetterInterface
    {
      bool IsValueSynced(string name);

      Type GetPropertyType(string name);

      object GetPropertyValue(string name);

      void OnFinalize();
    }

    public interface IViewModelSetterInterface
    {
      void SetPropertyValue(string name, object value);

      void OnFinalize();
    }
  }
}
