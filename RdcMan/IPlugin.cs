// Decompiled with JetBrains decompiler
// Type: RdcMan.IPlugin
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public interface IPlugin
  {
    void PreLoad(IPluginContext context, XmlNode xmlNode);

    void PostLoad(IPluginContext context);

    XmlNode SaveSettings();

    void Shutdown();

    void OnContextMenu(ContextMenuStrip contextMenuStrip, RdcTreeNode node);

    void OnUndockServer(IUndockedServerForm form);

    void OnDockServer(ServerBase server);
  }
}
