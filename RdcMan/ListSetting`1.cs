// Decompiled with JetBrains decompiler
// Type: RdcMan.ListSetting`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Xml;

namespace RdcMan
{
  public class ListSetting<T> : Setting<List<T>> where T : class
  {
    private const string XmlNodeName = "item";

    public ListSetting(object o)
      : base(o)
    {
      if (this.Value != null)
        return;
      this.Value = new List<T>();
    }

    public override void ReadXml(XmlNode xmlNode, RdcTreeNode node)
    {
      List<T> objList = new List<T>();
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        if (childNode.Name != "item")
          throw new Exception();
        objList.Add(childNode.InnerText as T);
      }
      this.Value = objList;
    }

    public override void WriteXml(XmlTextWriter tw, RdcTreeNode node)
    {
      foreach (T obj in this.Value)
        tw.WriteElementString("item", obj.ToString());
    }
  }
}
