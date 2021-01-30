// Decompiled with JetBrains decompiler
// Type: RdcMan.ConnectedGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.ComponentModel.Composition;

namespace RdcMan
{
  [Export(typeof (IBuiltInVirtualGroup))]
  internal class ConnectedGroup : BuiltInVirtualGroup<ConnectedServerRef>, IServerRefFactory
  {
    public static ConnectedGroup Instance { get; private set; }

    static ConnectedGroup()
    {
      Server.ConnectionStateChanged += new Action<ConnectionStateChangedEventArgs>(ConnectedGroup.Server_ConnectionStateChanged);
      Server.FocusReceived += new Action<Server>(ConnectedGroup.Server_FocusReceived);
    }

    private ConnectedGroup()
    {
      this.Text = "Connected";
      ConnectedGroup.Instance = this;
    }

    private static void Server_FocusReceived(Server server)
    {
      ConnectedServerRef serverRef = server.FindServerRef<ConnectedServerRef>();
      if (serverRef == null)
        return;
      serverRef.LastFocusTime = DateTime.Now;
      if (!ServerTree.Instance.SortGroup((GroupBase) ConnectedGroup.Instance))
        return;
      ServerTree.Instance.OnGroupChanged((GroupBase) ConnectedGroup.Instance, ChangeType.InvalidateUI);
    }

    private static void Server_ConnectionStateChanged(ConnectionStateChangedEventArgs args)
    {
      switch (args.State)
      {
        case RdpClient.ConnectionState.Disconnected:
          RdcTreeNode serverRef = (RdcTreeNode) args.Server.FindServerRef<ConnectedServerRef>();
          if (serverRef == null)
            break;
          ServerTree.Instance.RemoveNode(serverRef);
          break;
        case RdpClient.ConnectionState.Connected:
          ConnectedGroup.Instance.AddReference((ServerBase) args.Server);
          break;
      }
    }

    protected override bool ShouldWriteNode(RdcTreeNode node, FileGroup file) => file == null;

    public override bool CanRemoveChildren() => false;

    public override void Disconnect()
    {
      this.Hide();
      base.Disconnect();
    }

    public ServerRef Create(Server server) => (ServerRef) new ConnectedServerRef(server);

    protected override string XmlNodeName => "connected";
  }
}
