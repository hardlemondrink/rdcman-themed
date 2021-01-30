// Decompiled with JetBrains decompiler
// Type: RdcMan.ConnectToGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.ComponentModel.Composition;

namespace RdcMan
{
  [Export(typeof (IBuiltInVirtualGroup))]
  internal class ConnectToGroup : BuiltInVirtualGroup<ServerRef>, IServerRefFactory
  {
    public static ConnectToGroup Instance { get; private set; }

    private ConnectToGroup()
    {
      this.Text = "Connect To";
      ConnectToGroup.Instance = this;
    }

    public override ServerRef AddReference(ServerBase serverBase) => throw new InvalidOperationException();

    public override void InvalidateNode()
    {
      base.InvalidateNode();
      if (this.Nodes.Count != 0)
        return;
      this.IsInTree = false;
    }

    public ServerRef Create(Server server) => throw new NotImplementedException("ConnectTo does not contain ServerRef");

    protected override bool IsVisibilityConfigurable => false;
  }
}
