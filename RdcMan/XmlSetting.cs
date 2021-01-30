// Decompiled with JetBrains decompiler
// Type: RdcMan.XmlSetting
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Xml;

namespace RdcMan
{
  public class XmlSetting : Setting<XmlNode>
  {
    public XmlSetting(object o)
      : base(o)
    {
    }

    public override void ReadXml(XmlNode xmlNode, RdcTreeNode node) => this.Value = xmlNode;

    public override void WriteXml(XmlTextWriter tw, RdcTreeNode node) => this.Value.WriteTo((XmlWriter) tw);
  }
}
