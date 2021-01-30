// Decompiled with JetBrains decompiler
// Type: RdcMan.ReconnectGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace RdcMan
{
  [Export(typeof (IBuiltInVirtualGroup))]
  internal class ReconnectGroup : BuiltInVirtualGroup<ReconnectServerRef>, IServerRefFactory
  {
    public static ReconnectGroup Instance { get; private set; }

    private ReconnectGroup()
    {
      this.Text = "Reconnect";
      ReconnectGroup.Instance = this;
    }

    public override bool CanDropServers() => true;

    public override DragDropEffects DropBehavior() => DragDropEffects.Copy;

    public override bool HandleMove(RdcTreeNode childNode)
    {
      this.AddReference(childNode as ServerBase).Start(false);
      return true;
    }

    public ServerRef Create(Server server) => (ServerRef) new ReconnectServerRef(server);
  }
}
