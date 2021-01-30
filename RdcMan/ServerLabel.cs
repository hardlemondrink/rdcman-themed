// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerLabel
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  internal class ServerLabel : Button
  {
    private static ContextMenuStrip _menu = new ContextMenuStrip();

    public new static int Height { get; private set; }

    public ServerBase AssociatedNode { get; private set; }

    public Server Server { get; private set; }

    public int ThumbnailIndex { get; set; }

    static ServerLabel()
    {
      ServerLabel._menu.Opening += new CancelEventHandler(ServerLabel.MenuPopup);
      Button button = new Button();
      button.FlatStyle = FlatStyle.Flat;
      button.Font = new Font(ServerTree.Instance.Font, FontStyle.Bold);
      ServerLabel.Height = button.Height;
    }

    public ServerLabel(ServerBase node)
    {
      this.AssociatedNode = node;
      this.Server = node.ServerNode;
      this.Enabled = true;
      this.TabStop = true;
      this.ContextMenuStrip = ServerLabel._menu;
      this.TextAlign = ContentAlignment.MiddleCenter;
      this.FlatStyle = FlatStyle.Flat;
      this.Font = new Font(ServerTree.Instance.Font, FontStyle.Bold);
      this.Hide();
      this.CopyServerData();
      this.UpdateVisual();
    }

    public void CopyServerData() => this.Text = this.Server.DisplayName;

    protected override void OnMouseDown(MouseEventArgs e)
    {
      if (e.Button != MouseButtons.Left)
        return;
      if (e.Clicks == 1)
      {
        this.Focus();
      }
      else
      {
        ServerTree.Instance.SelectedNode = (TreeNode) this.AssociatedNode;
        this.Server.Connect();
        this.Server.Focus();
      }
    }

    private static void MenuPopup(object sender, CancelEventArgs e)
    {
      ServerLabel._menu.Items.Clear();
      ServerBase server = ((sender as ContextMenuStrip).SourceControl as ServerLabel).AssociatedNode;
      MenuHelper.AddSessionMenuItems(ServerLabel._menu, server);
      ServerLabel._menu.Items.Add("-");
      ServerLabel._menu.Items.Add((ToolStripItem) new DelegateMenuItem("E&xpand", MenuNames.SessionExpand, (Action) (() =>
      {
        ServerTree.Instance.SelectedNode = (TreeNode) server;
        if (!server.IsConnected)
          return;
        server.Focus();
      })));
      MenuHelper.AddDockingMenuItems(ServerLabel._menu, server);
      ServerLabel._menu.Items.Add("-");
      MenuHelper.AddMaintenanceMenuItems(ServerLabel._menu, server);
      Program.PluginAction((Action<IPlugin>) (p => p.OnContextMenu(ServerLabel._menu, (RdcTreeNode) server)));
      e.Cancel = false;
    }

    protected override void OnGotFocus(EventArgs e) => this.UpdateVisual();

    protected override void OnLostFocus(EventArgs e)
    {
      Program.TheForm.RecordLastFocusedServerLabel(this);
      this.UpdateVisual();
    }

    protected void UpdateVisual()
    {
      if (this.Focused)
      {
        this.ForeColor = SystemColors.ActiveCaptionText;
        this.BackColor = SystemColors.ActiveCaption;
      }
      else
      {
        this.ForeColor = SystemColors.InactiveCaptionText;
        this.BackColor = SystemColors.InactiveCaption;
      }
    }
  }
}
