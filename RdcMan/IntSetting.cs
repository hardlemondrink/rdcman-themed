// Decompiled with JetBrains decompiler
// Type: RdcMan.IntSetting
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Xml;

namespace RdcMan
{
  public class IntSetting : Setting<int>
  {
    public IntSetting(object o)
      : base(o)
    {
    }

    public override void ReadXml(XmlNode xmlNode, RdcTreeNode node) => this.Value = int.Parse(xmlNode.FirstChild.InnerText);
  }
}
