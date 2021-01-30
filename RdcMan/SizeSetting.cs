// Decompiled with JetBrains decompiler
// Type: RdcMan.SizeSetting
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Drawing;
using System.Xml;

namespace RdcMan
{
  public class SizeSetting : Setting<Size>
  {
    public SizeSetting(object o)
      : base(o)
    {
    }

    public override void ReadXml(XmlNode xmlNode, RdcTreeNode node) => this.Value = SizeHelper.Parse(xmlNode.FirstChild.InnerText);

    public override void WriteXml(XmlTextWriter tw, RdcTreeNode node) => tw.WriteString(this.Value.ToFormattedString());

    public override string ToString() => this.Value.ToFormattedString();
  }
}
