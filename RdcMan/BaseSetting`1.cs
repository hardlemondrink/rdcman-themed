// Decompiled with JetBrains decompiler
// Type: RdcMan.BaseSetting`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Xml;

namespace RdcMan
{
  public abstract class BaseSetting<T> : ISetting
  {
    internal T Value;

    protected BaseSetting(object o) => this.Value = o != null ? (T) o : default (T);

    public abstract void ReadXml(XmlNode xmlNode, RdcTreeNode node);

    public virtual void WriteXml(XmlTextWriter tw, RdcTreeNode node) => tw.WriteString(this.Value.ToString());

    public virtual void Copy(ISetting source) => this.Value = ((BaseSetting<T>) source).Value;

    public override string ToString() => this.Value.ToString();
  }
}
