// Decompiled with JetBrains decompiler
// Type: RdcMan.IBuiltInVirtualGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Xml;

namespace RdcMan
{
  internal interface IBuiltInVirtualGroup
  {
    string XmlNodeName { get; }

    string Text { get; }

    string ConfigPropertyName { get; }

    bool IsInTree { get; set; }

    bool IsVisibilityConfigurable { get; }

    void ReadXml(XmlNode xmlNode, FileGroup fileGroup, ICollection<string> errors);

    void WriteXml(XmlTextWriter tw, FileGroup fileGroup);

    bool ShouldWriteNode(ServerRef serverRef, FileGroup file);
  }
}
