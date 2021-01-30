// Decompiled with JetBrains decompiler
// Type: RdcMan.ReconnectServerRef
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Threading;
using System.Windows.Forms;

namespace RdcMan
{
  internal class ReconnectServerRef : ServerRef
  {
    private bool _selectedInConnectedGroup;

    public bool NeedToReconnect { get; private set; }

    private bool RemoveAfterConnection { get; set; }

    static ReconnectServerRef() => Server.ConnectionStateChanged += new Action<ConnectionStateChangedEventArgs>(ReconnectServerRef.Server_ConnectionStateChanged);

    private static void Server_ConnectionStateChanged(ConnectionStateChangedEventArgs args)
    {
      ReconnectServerRef serverRef = args.Server.FindServerRef<ReconnectServerRef>();
      if (serverRef == null)
        return;
      switch (args.State)
      {
        case RdpClient.ConnectionState.Disconnected:
          serverRef.OnDisconnected();
          break;
        case RdpClient.ConnectionState.Connecting:
          serverRef.OnConnecting();
          break;
        case RdpClient.ConnectionState.Connected:
          serverRef.OnConnected();
          break;
      }
    }

    public ReconnectServerRef(Server server)
      : base(server)
    {
    }

    public void Start(bool removeAfterConnection)
    {
      this.RemoveAfterConnection = removeAfterConnection;
      this.NeedToReconnect = true;
      ConnectedServerRef serverRef = this.ServerNode.FindServerRef<ConnectedServerRef>();
      if (serverRef != null)
      {
        this._selectedInConnectedGroup = serverRef.IsSelected;
        if (this._selectedInConnectedGroup)
          ServerTree.Instance.SelectedNode = (TreeNode) this.ServerNode;
      }
      if (!this.ServerNode.IsConnected)
        this.ServerNode.Connect();
      else
        this.ServerNode.Disconnect();
    }

    public override bool CanRemove(bool popUI) => true;

    public override void Reconnect()
    {
      this.NeedToReconnect = true;
      this.ServerNode.Disconnect();
    }

    public override void Disconnect()
    {
      this.NeedToReconnect = false;
      base.Disconnect();
    }

    public override void LogOff()
    {
      this.NeedToReconnect = false;
      base.LogOff();
    }

    private void OnConnecting()
    {
      if (!this.RemoveAfterConnection)
        return;
      this.NeedToReconnect = false;
    }

    private void OnConnected()
    {
      this.NeedToReconnect = false;
      if (!this.RemoveAfterConnection)
        return;
      if (ServerTree.Instance.SelectedNode == this)
        ServerTree.Instance.SelectedNode = (TreeNode) this.ServerNode;
      else if (this._selectedInConnectedGroup && ServerTree.Instance.SelectedNode == this.ServerNode)
        ServerTree.Instance.SelectedNode = (TreeNode) this.ServerNode.FindServerRef<ConnectedServerRef>();
      ServerTree.Instance.RemoveNode((RdcTreeNode) this);
    }

    private void OnDisconnected()
    {
      if (this.NeedToReconnect)
        ThreadPool.QueueUserWorkItem((WaitCallback) (o => this.ServerNode.ParentForm.Invoke((Delegate) (() => this.ServerNode.Connect()))));
      else
        ServerTree.Instance.RemoveNode((RdcTreeNode) this);
    }
  }
}
