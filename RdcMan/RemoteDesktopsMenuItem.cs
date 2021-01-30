// Decompiled with JetBrains decompiler
// Type: RdcMan.RemoteDesktopsMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  internal class RemoteDesktopsMenuItem : RdcMenuItem
  {
    private const string AllItem = "[All]";

    public RemoteDesktopsMenuItem()
      : base("Remote Desktops")
    {
      this.Name = MenuNames.RemoteDesktops.ToString();
      ServerTree.Instance.GroupChanged += new Action<GroupChangedEventArgs>(this.GroupChanged);
      ServerTree.Instance.ServerChanged += new Action<ServerChangedEventArgs>(this.ServerChanged);
      this.HasChangedSinceMenuUpdate = true;
    }

    private bool HasChangedSinceMenuUpdate { get; set; }

    private void ServerChanged(ServerChangedEventArgs e)
    {
      if (!e.ChangeType.HasFlag((Enum) ChangeType.TreeChanged) && !e.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged))
        return;
      this.HasChangedSinceMenuUpdate = true;
    }

    private void GroupChanged(GroupChangedEventArgs e)
    {
      if (!e.ChangeType.HasFlag((Enum) ChangeType.TreeChanged) && !e.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged))
        return;
      this.HasChangedSinceMenuUpdate = true;
    }

    public override void Update()
    {
      if (!this.HasChangedSinceMenuUpdate)
        return;
      this.HasChangedSinceMenuUpdate = false;
      this.DropDownItems.Clear();
      foreach (TreeNode node in ServerTree.Instance.Nodes)
        this.PopulateRemoteDesktopsMenuItems(this.DropDownItems, node);
    }

    protected override void OnClick()
    {
    }

    private void PopulateRemoteDesktopsMenuItems(ToolStripItemCollection items, TreeNode treeNode)
    {
      RdcTreeNode node1 = treeNode as RdcTreeNode;
      ToolStripMenuItem toolStripMenuItem;
      if (node1 is GroupBase groupBase && (groupBase.Nodes.Count > 1 || groupBase.HasGroups))
      {
        toolStripMenuItem = new ToolStripMenuItem(node1.Text);
        ServerMenuItem serverMenuItem = new ServerMenuItem(node1);
        serverMenuItem.Text = "[All]";
        toolStripMenuItem.DropDownItems.Add((ToolStripItem) serverMenuItem);
      }
      else
        toolStripMenuItem = (ToolStripMenuItem) new ServerMenuItem(node1);
      foreach (TreeNode node2 in node1.Nodes)
        this.PopulateRemoteDesktopsMenuItems(toolStripMenuItem.DropDownItems, node2);
      items.Add((ToolStripItem) toolStripMenuItem);
    }
  }
}
