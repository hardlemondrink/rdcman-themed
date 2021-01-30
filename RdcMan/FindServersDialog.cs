// Decompiled with JetBrains decompiler
// Type: RdcMan.FindServersDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RdcMan
{
  internal class FindServersDialog : SelectServersDialogBase
  {
    private string _previousFilterText;
    private List<Server> _servers;
    private TextBox _filterTextBox;

    public FindServersDialog()
      : base("Find Servers", "Select")
    {
      int rowIndex1 = 0;
      int tabIndex1 = 0;
      this.AddLabel("Type to filter servers", ref rowIndex1, ref tabIndex1);
      int rowIndex2 = rowIndex1;
      int rowIndex3 = rowIndex2 + 1;
      int tabIndex2 = tabIndex1;
      int tabIndex3 = tabIndex2 + 1;
      this._filterTextBox = (TextBox) FormTools.NewTextBox(0, rowIndex2, tabIndex2);
      this._filterTextBox.Enabled = true;
      this._filterTextBox.Width = 500;
      this._filterTextBox.TextChanged += new EventHandler(this.Filter_TextChanged);
      this.Controls.Add((Control) this._filterTextBox);
      this.AddListView(ref rowIndex3, ref tabIndex3);
      this.ListView.ContextMenuStrip = new ContextMenuStrip();
      this.ListView.ContextMenuStrip.Opening += new CancelEventHandler(this.ContextMenuPopup);
      this.InitButtons();
      this.ScaleAndLayout();
      this._previousFilterText = string.Empty;
      this.CollectServers();
      this.PopulateList();
    }

    protected override void OnClosed(EventArgs e)
    {
      if (this.ListView.CheckedItems.Count != 0 || this.ListView.Items.Count <= 0)
        return;
      if (this.ListView.FocusedItem == null)
        this.ListView.FocusedItem = this.ListView.Items[0];
      this.ListView.FocusedItem.Checked = true;
    }

    private void CollectServers()
    {
      this._servers = new List<Server>();
      ServerTree.Instance.Nodes.VisitNodes((Action<RdcTreeNode>) (node =>
      {
        if (!(node is Server server))
          return;
        this._servers.Add(server);
      }));
    }

    private void PopulateList()
    {
      try
      {
        Regex regex = new Regex(this._filterTextBox.Text, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        this.ListView.BeginUpdate();
        this.SuspendItemChecked();
        this.ListView.Items.Clear();
        foreach (Server server in this._servers)
        {
          if (regex.IsMatch(server.FullPath))
            this.ListView.Items.Add(this.CreateListViewItem((ServerBase) server));
        }
      }
      catch (Exception ex)
      {
      }
      finally
      {
        this.ResumeItemChecked();
        this.ListView.EndUpdate();
      }
    }

    private void FilterList()
    {
      try
      {
        Regex regex = new Regex(this._filterTextBox.Text, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        int index = 0;
        while (index < this.ListView.Items.Count)
        {
          Server tag = this.ListView.Items[index].Tag as Server;
          if (!regex.IsMatch(tag.FullPath))
            this.ListView.Items.RemoveAt(index);
          else
            ++index;
        }
      }
      catch (Exception ex)
      {
      }
    }

    private void ContextMenuPopup(object menuSender, EventArgs args)
    {
      ContextMenuStrip contextMenuStrip = menuSender as ContextMenuStrip;
      List<ServerBase> selectedServers = this.SelectedServers.ToList<ServerBase>();
      if (selectedServers.Count == 0)
      {
        if (this.ListView.FocusedItem == null)
          return;
        selectedServers.Add(this.ListView.FocusedItem.Tag as ServerBase);
      }
      contextMenuStrip.Items.Clear();
      bool allConnected;
      bool anyConnected;
      NodeHelper.AnyOrAllConnected((IEnumerable<ServerBase>) selectedServers, out anyConnected, out allConnected);
      ToolStripMenuItem toolStripMenuItem1 = (ToolStripMenuItem) new DelegateMenuItem("&Connect", MenuNames.SessionConnect, (Action) (() =>
      {
        NodeHelper.ThrottledConnect((IEnumerable<ServerBase>) selectedServers);
        this.OK();
      }));
      toolStripMenuItem1.Enabled = !allConnected;
      contextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem1);
      ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem) new DelegateMenuItem("R&econnect", MenuNames.SessionReconnect, (Action) (() =>
      {
        selectedServers.ForEach((Action<ServerBase>) (server => server.Reconnect()));
        this.OK();
      }));
      toolStripMenuItem2.Enabled = anyConnected;
      contextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem2);
      ToolStripMenuItem toolStripMenuItem3 = (ToolStripMenuItem) new DelegateMenuItem("&Disconnect", MenuNames.SessionDisconnect, (Action) (() =>
      {
        NodeHelper.ThrottledDisconnect((IEnumerable<ServerBase>) selectedServers);
        this.OK();
      }));
      toolStripMenuItem3.Enabled = anyConnected;
      contextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem3);
      contextMenuStrip.Items.Add("-");
      ToolStripMenuItem toolStripMenuItem4 = (ToolStripMenuItem) new DelegateMenuItem("Log off", MenuNames.SessionLogOff, (Action) (() =>
      {
        selectedServers.ForEach((Action<ServerBase>) (server => server.LogOff()));
        this.OK();
      }));
      toolStripMenuItem4.Enabled = !Policies.DisableLogOff && anyConnected;
      contextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem4);
      contextMenuStrip.Items.Add("-");
      ToolStripMenuItem toolStripMenuItem5 = (ToolStripMenuItem) new DelegateMenuItem("Remo&ve", MenuNames.EditRemove, (Action) (() =>
      {
        if (anyConnected && FormTools.YesNoDialog("There are active sessions. Are you sure?") != DialogResult.Yes)
          return;
        selectedServers.ForEach((Action<ServerBase>) (server => ServerTree.Instance.ConfirmRemove((RdcTreeNode) server, false)));
        this.Cancel();
      }));
      contextMenuStrip.Items.Add((ToolStripItem) toolStripMenuItem5);
      contextMenuStrip.Items.Add("-");
      contextMenuStrip.Items.Add((ToolStripItem) new DelegateMenuItem("Add to favorites", MenuNames.EditAddToFavorites, (Action) (() =>
      {
        selectedServers.ForEach((Action<ServerBase>) (server => FavoritesGroup.Instance.AddReference(server)));
        this.OK();
      })));
    }

    private void Filter_TextChanged(object sender, EventArgs e)
    {
      if (this._filterTextBox.Text.StartsWith(this._previousFilterText))
        this.FilterList();
      else
        this.PopulateList();
      this._previousFilterText = this._filterTextBox.Text;
    }
  }
}
