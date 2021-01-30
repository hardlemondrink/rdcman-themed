// Decompiled with JetBrains decompiler
// Type: RdcMan.Configuration.DisplaySizeElementCollection
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Configuration;

namespace RdcMan.Configuration
{
  public class DisplaySizeElementCollection : ConfigurationElementCollection
  {
    public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.AddRemoveClearMap;

    public DisplaySizeElement GetDisplaySize(int index) => this.BaseGet(index) as DisplaySizeElement;

    protected override ConfigurationElement CreateNewElement() => (ConfigurationElement) new DisplaySizeElement();

    protected override object GetElementKey(ConfigurationElement element) => (object) ((DisplaySizeElement) element).Size;
  }
}
