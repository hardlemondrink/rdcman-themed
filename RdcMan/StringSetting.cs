// Decompiled with JetBrains decompiler
// Type: RdcMan.StringSetting
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Xml;

namespace RdcMan
{
  public class StringSetting : Setting<string>
  {
    public StringSetting(object o)
      : base(o)
    {
      if (this.Value != null)
        return;
      this.Value = string.Empty;
    }

    public override void ReadXml(XmlNode xmlNode, RdcTreeNode node)
    {
      xmlNode = xmlNode.FirstChild;
      if (xmlNode == null)
        this.Value = string.Empty;
      else
        this.Value = xmlNode.InnerText;
    }
  }
}
