// Decompiled with JetBrains decompiler
// Type: RdcMan.SettingAttribute
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
  public class SettingAttribute : Attribute
  {
    public SettingAttribute(string xmlName) => this.XmlName = xmlName;

    public string XmlName { get; set; }

    public object DefaultValue { get; set; }

    public bool IsObsolete { get; set; }
  }
}
