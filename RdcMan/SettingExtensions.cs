// Decompiled with JetBrains decompiler
// Type: RdcMan.SettingExtensions
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Reflection;

namespace RdcMan
{
  public static class SettingExtensions
  {
    public static void GetSettingProperties(
      this Type type,
      out Dictionary<string, SettingProperty> settingProperties)
    {
      settingProperties = new Dictionary<string, SettingProperty>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        object[] customAttributes = property.GetCustomAttributes(typeof (SettingAttribute), false);
        if (customAttributes.Length == 1)
        {
          SettingAttribute settingAttribute = (SettingAttribute) customAttributes[0];
          settingProperties[settingAttribute.XmlName] = new SettingProperty()
          {
            Property = property,
            Attribute = settingAttribute
          };
        }
      }
    }
  }
}
