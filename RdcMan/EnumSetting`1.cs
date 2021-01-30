// Decompiled with JetBrains decompiler
// Type: RdcMan.EnumSetting`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Xml;

namespace RdcMan
{
  public class EnumSetting<TEnum> : Setting<TEnum> where TEnum : struct
  {
    public EnumSetting(object o)
      : base(o)
    {
    }

    public override void ReadXml(XmlNode xmlNode, RdcTreeNode node)
    {
      int result;
      if (int.TryParse(xmlNode.InnerText, out result))
        this.Value = (TEnum) (ValueType) result;
      else
        this.Value = (TEnum) Enum.Parse(typeof (TEnum), xmlNode.InnerText);
    }

    public override void WriteXml(XmlTextWriter tw, RdcTreeNode node) => tw.WriteString(this.Value.ToString());
  }
}
