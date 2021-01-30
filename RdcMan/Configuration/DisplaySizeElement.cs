// Decompiled with JetBrains decompiler
// Type: RdcMan.Configuration.DisplaySizeElement
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Configuration;

namespace RdcMan.Configuration
{
  public class DisplaySizeElement : ConfigurationElement
  {
    [ConfigurationProperty("size")]
    public string Size
    {
      get => (string) this["size"];
      set => this["size"] = (object) value.ToString();
    }
  }
}
