// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerRef
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public abstract class ServerRef : ServerBase
  {
    private Server _server;

    protected ServerRef(Server server)
    {
      this._server = server;
      this._server.AddServerRef(this);
      this.Text = server.Text;
      this.ChangeImageIndex(ImageConstants.DisconnectedServer);
    }

    protected override void InitSettings()
    {
    }

    public override Server ServerNode => this._server;

    public override FileGroup FileGroup => this._server.FileGroup;

    public override bool IsClientDocked => this._server.IsClientDocked;

    public override bool IsClientUndocked => this._server.IsClientUndocked;

    public override bool IsClientFullScreen => this._server.IsClientFullScreen;

    public override string ToString() => string.Format("{0}: {1}", (object) this.GetType().Name, (object) this.Text);

    internal override void Show() => this._server.Show();

    internal override void Hide() => this._server.Hide();

    public override void Connect() => this._server.Connect();

    public override void ConnectAs(
      LogonCredentials logonSettings,
      ConnectionSettings connectionsettings)
    {
      this._server.ConnectAs(logonSettings, connectionsettings);
    }

    public override void Reconnect() => this._server.Reconnect();

    public override void Disconnect() => this._server.Disconnect();

    public override void LogOff() => this._server.LogOff();

    public override void DoPropertiesDialog(Form parentForm, string activeTabName) => this._server.DoPropertiesDialog(parentForm, activeTabName);

    public override bool CanRemove(bool popUI) => false;

    public override bool IsConnected => this._server.IsConnected;

    public override bool CanDropOnTarget(RdcTreeNode targetNode)
    {
      if (!(targetNode is GroupBase groupBase))
        groupBase = targetNode.Parent as GroupBase;
      GroupBase groupBase1 = groupBase;
      switch (groupBase1.DropBehavior())
      {
        case DragDropEffects.Copy:
          return groupBase1.CanDropServers();
        case DragDropEffects.Link:
          return groupBase1.CanDropServers() && this.AllowEdit(false);
        default:
          return false;
      }
    }

    public override void CollectNodesToInvalidate(bool recurseChildren, HashSet<RdcTreeNode> set)
    {
      set.Add((RdcTreeNode) this);
      set.Add(this.Parent as RdcTreeNode);
    }

    internal override void WriteXml(XmlTextWriter tw) => throw new NotImplementedException();

    public override ServerSettings Properties => this._server.Properties;

    public override CommonDisplaySettings DisplaySettings => this._server.DisplaySettings;

    public override sealed void ChangeImageIndex(ImageConstants index)
    {
      this.ImageIndex = this._server.ImageIndex;
      this.SelectedImageIndex = this._server.SelectedImageIndex;
    }

    public override string RemoveTypeDescription => "server reference";

    public override ServerBase.DisplayStates DisplayState
    {
      get => this._server.DisplayState;
      set => this._server.DisplayState = value;
    }

    public override Size Size
    {
      get => this._server.Size;
      set => this._server.Size = value;
    }

    public override Point Location
    {
      get => this._server.Location;
      set => this._server.Location = value;
    }

    internal override void Focus() => this._server.Focus();

    internal override void FocusConnectedClient() => this._server.FocusConnectedClient();

    internal virtual void OnRemoveServer() => ServerTree.Instance.RemoveNode((RdcTreeNode) this);

    public override void OnRemoving() => this._server.RemoveServerRef(this);

    internal override void GoFullScreen() => this._server.GoFullScreen();

    internal override void LeaveFullScreen() => this._server.LeaveFullScreen();

    internal override void Undock() => this._server.Undock();

    internal override void Dock() => this._server.Dock();

    internal override void ScreenCapture() => this._server.ScreenCapture();
  }
}
