// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerTree
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace RdcMan
{
  internal class ServerTree : TreeView, IServerTree
  {
    private const int DelayedFocusDelayMilliseconds = 100;
    private static readonly Color NotFocusedForeColor = Color.FromKnownColor(KnownColor.ControlDark);
    private static readonly Color NotFocusedBackColor = Color.White;
    private static readonly Color FocusedForeColor = Color.Black;
    private static readonly Color FocusedBackColor = Color.White;
    private static readonly ImageConstants[,] ImageConstantLookup = new ImageConstants[2, 9];
    private int _noSortCounter;
    private int _noSelectCounter;
    private int _noGroupChanged;
    private bool _contextViaMouse;
    private RdcTreeNode _draggedNode;
    private TreeNode _preDragSelectedNode;
    private ServerBase _delayedFocusServer;
    private readonly System.Threading.Timer _delayedFocusTimer;
    private readonly object _delayedFocusSyncObject = new object();
    private readonly GroupBase _rootNode = (GroupBase) new ServerTree.RootNodeGroup();

    internal event Action<ServerChangedEventArgs> ServerChanged;

    internal event Action<GroupChangedEventArgs> GroupChanged;

    internal static ServerTree Instance { get; private set; }

    static ServerTree()
    {
      foreach (ImageConstants enumValue in Helpers.EnumValues<ImageConstants>())
      {
        ServerTree.ImageConstantLookup[0, (int) enumValue] = enumValue;
        ServerTree.ImageConstantLookup[1, (int) enumValue] = enumValue;
      }
      ServerTree.ImageConstantLookup[0, 4] = ImageConstants.ConnectedServer;
      ServerTree.ImageConstantLookup[0, 2] = ImageConstants.ConnectingServer;
      ServerTree.ImageConstantLookup[1, 3] = ImageConstants.ConnectedSelectedServer;
      ServerTree.ImageConstantLookup[1, 1] = ImageConstants.ConnectingSelectedServer;
      ServerTree.Instance = new ServerTree();
    }

    private ServerTree()
    {
      this.BorderStyle = BorderStyle.None;
      this.AllowDrop = true;
      this.Scrollable = true;
      this.HideSelection = false;
      this._delayedFocusTimer = new System.Threading.Timer((TimerCallback) (o => this.CheckDelayedFocusServer()), (object) null, -1, -1);
    }

    public GroupBase RootNode => this._rootNode;

    internal bool SuppressSorting => this._noSortCounter > 0;

    internal FileGroup GetSelectedFile()
    {
      FileGroup fileGroup = (FileGroup) null;
      TreeNode selectedNode = this.SelectedNode;
      if (selectedNode != null)
        fileGroup = (selectedNode as RdcTreeNode).FileGroup;
      return fileGroup;
    }

    internal void Operation(OperationBehavior behavior, Action operation)
    {
      RdcTreeNode selectedNode = this.SelectedNode as RdcTreeNode;
      try
      {
        if (behavior.HasFlag((Enum) OperationBehavior.SuspendUpdate))
          this.BeginUpdate();
        if (behavior.HasFlag((Enum) OperationBehavior.SuspendSort))
          this.SuspendSort();
        if (behavior.HasFlag((Enum) OperationBehavior.SuspendSelect))
          this.SuspendSelect();
        if (behavior.HasFlag((Enum) OperationBehavior.SuspendGroupChanged))
          this.SuspendGroupChanged();
        if (behavior.HasFlag((Enum) OperationBehavior.RestoreSelected))
          this.SelectedNode = (TreeNode) null;
        operation();
      }
      finally
      {
        if (behavior.HasFlag((Enum) OperationBehavior.RestoreSelected))
        {
          this.SelectedNode = (TreeNode) selectedNode;
          Program.TheForm.SetTitle();
        }
        if (behavior.HasFlag((Enum) OperationBehavior.SuspendGroupChanged))
          this.ResumeGroupChanged();
        if (behavior.HasFlag((Enum) OperationBehavior.SuspendSelect))
          this.ResumeSelect();
        if (behavior.HasFlag((Enum) OperationBehavior.SuspendSort))
          this.ResumeSort();
        if (behavior.HasFlag((Enum) OperationBehavior.SuspendUpdate))
          this.EndUpdate();
      }
    }

    internal void UpdateColors()
    {
      if (Program.Preferences.DimNodesWhenInactive)
      {
        if (this.Focused)
        {
          this.ForeColor = ServerTree.FocusedForeColor;
          this.BackColor = ServerTree.FocusedBackColor;
        }
        else
        {
          this.ForeColor = ServerTree.NotFocusedForeColor;
          this.BackColor = ServerTree.NotFocusedBackColor;
        }
      }
      else
      {
        this.ForeColor = ServerTree.FocusedForeColor;
        this.BackColor = ServerTree.FocusedBackColor;
      }
    }

    internal void Init(Assembly myAssembly)
    {
      this.ImageList = new ImageList();
      this.ImageList.ColorDepth = ColorDepth.Depth8Bit;
      this.ImageList.ImageSize = new Size(16, 16);
      this.ImageList.Images.Add(new Icon(myAssembly.GetManifestResourceStream("RdcMan.Resources.disconnected.ico")));
      this.ImageList.Images.Add(new Icon(myAssembly.GetManifestResourceStream("RdcMan.Resources.connecting.ico")));
      this.ImageList.Images.Add(new Icon(myAssembly.GetManifestResourceStream("RdcMan.Resources.connectingselected.ico")));
      this.ImageList.Images.Add(new Icon(myAssembly.GetManifestResourceStream("RdcMan.Resources.connected.ico")));
      this.ImageList.Images.Add(new Icon(myAssembly.GetManifestResourceStream("RdcMan.Resources.connectedselected.ico")));
      this.ImageList.Images.Add(new Icon(myAssembly.GetManifestResourceStream("RdcMan.Resources.group.ico")));
      this.ImageList.Images.Add(new Icon(myAssembly.GetManifestResourceStream("RdcMan.Resources.smartgroup.ico")));
      this.ImageList.Images.Add(new Icon(myAssembly.GetManifestResourceStream("RdcMan.Resources.app.ico")));
      ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
      contextMenuStrip.Opening += new CancelEventHandler(this.OnContextMenu);
      this.ContextMenuStrip = contextMenuStrip;
    }

    public void AddNode(RdcTreeNode node, GroupBase parent)
    {
      switch (node)
      {
        case null:
          throw new ArgumentNullException(nameof (node));
        case ServerBase _:
        case GroupBase _:
          if (parent == null)
            throw new ArgumentNullException(nameof (parent));
          if (parent == this.RootNode)
          {
            this.Nodes.Add((TreeNode) node);
          }
          else
          {
            parent.Nodes.Add((TreeNode) node);
            this.SortGroup(parent);
          }
          this.OnGroupChanged(parent, ChangeType.TreeChanged);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (node), "Node must derive from ServerBase or GroupBase");
      }
    }

    public void RemoveNode(RdcTreeNode node)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      TreeNode treeNode = this.SelectedNode;
      TreeNodeCollection parentNodes = node.Parent != null ? node.Parent.Nodes : this.Nodes;
      if (treeNode != null)
      {
        bool inSelectedPath = false;
        (treeNode as RdcTreeNode).VisitNodeAndParents((Action<RdcTreeNode>) (n =>
        {
          if (n != node)
            return;
          inSelectedPath = true;
        }));
        if (inSelectedPath)
        {
          (treeNode as RdcTreeNode).Hide();
          node.Hide();
          treeNode = node.Index <= 0 ? (node.Index >= parentNodes.Count - 1 ? node.Parent : parentNodes[node.Index + 1]) : parentNodes[node.Index - 1];
          this.SelectedNode = (TreeNode) null;
        }
      }
      this.Operation(OperationBehavior.RestoreSelected, (Action) (() =>
      {
        GroupBase parent = node.Parent as GroupBase;
        node.OnRemoving();
        parentNodes.Remove((TreeNode) node);
        if (parent == null)
          return;
        this.OnGroupChanged(parent, ChangeType.TreeChanged);
      }));
      this.SelectedNode = treeNode;
    }

    private void CheckDelayedFocusServer()
    {
      lock (this._delayedFocusSyncObject)
      {
        if (this._delayedFocusServer != null)
          Program.TheForm.Invoke((Delegate) (() => this._delayedFocusServer.FocusConnectedClient()));
        this._delayedFocusServer = (ServerBase) null;
      }
    }

    private void SetDelayedFocusServer(ServerBase server)
    {
      lock (this._delayedFocusSyncObject)
      {
        this._delayedFocusServer = server;
        this._delayedFocusTimer.Change(100, -1);
      }
    }

    protected override void OnGotFocus(EventArgs e)
    {
      base.OnGotFocus(e);
      this.UpdateColors();
    }

    protected override void OnLostFocus(EventArgs e)
    {
      base.OnLostFocus(e);
      this.UpdateColors();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
        this._contextViaMouse = true;
      else
        base.OnMouseDown(e);
    }

    protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
    {
      base.OnBeforeSelect(e);
      if (this._noSelectCounter > 0 || this.SelectedNode == null)
        return;
      RdcTreeNode selectedNode = this.SelectedNode as RdcTreeNode;
      if (selectedNode is ServerBase serverBase && !serverBase.IsClientUndocked && serverBase.IsClientFullScreen)
        return;
      selectedNode.Hide();
    }

    protected override void OnAfterSelect(TreeViewEventArgs e)
    {
      if (this._noSelectCounter > 0)
        return;
      if (this.SelectedNode is RdcTreeNode selectedNode)
      {
        if (selectedNode is ServerBase server)
        {
          if (server.IsClientUndocked || !server.IsClientFullScreen)
            server.ServerNode.SetNormalView();
          if (!Helpers.IsControlKeyPressed && Program.Preferences.FocusOnClick && (e.Action == TreeViewAction.ByMouse && server.IsConnected))
            this.SetDelayedFocusServer(server);
        }
        if (!selectedNode.IsVisible)
          selectedNode.EnsureVisible();
        selectedNode.Show();
      }
      Program.TheForm.SetTitle();
      base.OnAfterSelect(e);
    }

    private void OnContextMenu(object sender, CancelEventArgs e)
    {
      RdcTreeNode contextNode = this.SelectedNode as RdcTreeNode;
      if (this._contextViaMouse)
      {
        Point client = this.PointToClient(Control.MousePosition);
        contextNode = this.GetNodeAt(client.X, client.Y) as RdcTreeNode;
        this._contextViaMouse = false;
      }
      this.PopulateNodeContextMenu(this.ContextMenuStrip, contextNode);
      Program.PluginAction((Action<IPlugin>) (p => p.OnContextMenu(this.ContextMenuStrip, contextNode)));
      e.Cancel = false;
    }

    private void PopulateNodeContextMenu(ContextMenuStrip menu, RdcTreeNode node)
    {
      menu.Items.Clear();
      if (node == null)
      {
        if (this.AnyOpenedEditableFiles())
        {
          menu.Items.Add((ToolStripItem) new DelegateMenuItem("&Add server...", MenuNames.EditAddServer, new Action(AddNodeDialogHelper.AddServersDialog)));
          menu.Items.Add((ToolStripItem) new DelegateMenuItem("&Import servers...", MenuNames.EditImportServers, new Action(AddNodeDialogHelper.ImportServersDialog)));
          menu.Items.Add("-");
          menu.Items.Add((ToolStripItem) new DelegateMenuItem("Add &group...", MenuNames.EditAddGroup, new Action(AddNodeDialogHelper.AddGroupDialog)));
        }
        else
        {
          ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem("Please open or create a file");
          toolStripMenuItem.Enabled = false;
          menu.Items.Add((ToolStripItem) toolStripMenuItem);
        }
      }
      else if (node is GroupBase groupBase)
      {
        bool anyConnected;
        bool allConnected;
        groupBase.AnyOrAllConnected(out anyConnected, out allConnected);
        ToolStripMenuItem toolStripMenuItem1 = (ToolStripMenuItem) new DelegateMenuItem("&Connect group", MenuNames.SessionConnect, new Action(((RdcTreeNode) groupBase).Connect));
        toolStripMenuItem1.Enabled = !allConnected;
        menu.Items.Add((ToolStripItem) toolStripMenuItem1);
        ToolStripMenuItem toolStripMenuItem2 = (ToolStripMenuItem) new DelegateMenuItem("&Connect group as...", MenuNames.SessionConnectAs, new Action(((RdcTreeNode) groupBase).DoConnectAs));
        toolStripMenuItem2.Enabled = !allConnected;
        menu.Items.Add((ToolStripItem) toolStripMenuItem2);
        ToolStripMenuItem toolStripMenuItem3 = (ToolStripMenuItem) new DelegateMenuItem("R&econnect group", MenuNames.SessionReconnect, new Action(((RdcTreeNode) groupBase).Reconnect));
        toolStripMenuItem3.Enabled = anyConnected;
        menu.Items.Add((ToolStripItem) toolStripMenuItem3);
        menu.Items.Add("-");
        ToolStripMenuItem toolStripMenuItem4 = (ToolStripMenuItem) new DelegateMenuItem("&Disconnect group", MenuNames.SessionDisconnect, new Action(((RdcTreeNode) groupBase).Disconnect));
        toolStripMenuItem4.Enabled = anyConnected;
        menu.Items.Add((ToolStripItem) toolStripMenuItem4);
        menu.Items.Add("-");
        ToolStripMenuItem toolStripMenuItem5 = (ToolStripMenuItem) new DelegateMenuItem("Log off group", MenuNames.SessionLogOff, new Action(((RdcTreeNode) groupBase).LogOff));
        toolStripMenuItem5.Enabled = !Policies.DisableLogOff && anyConnected;
        menu.Items.Add((ToolStripItem) toolStripMenuItem5);
        menu.Items.Add("-");
        ToolStripMenuItem toolStripMenuItem6 = (ToolStripMenuItem) new DelegateMenuItem("&Add server...", MenuNames.EditAddServer, (Action) (() => AddNodeDialogHelper.AddServersDialog((TreeNode) node)));
        toolStripMenuItem6.Enabled = groupBase.CanAddServers();
        menu.Items.Add((ToolStripItem) toolStripMenuItem6);
        ToolStripMenuItem toolStripMenuItem7 = (ToolStripMenuItem) new DelegateMenuItem("Add &group...", MenuNames.EditAddGroup, (Action) (() => AddNodeDialogHelper.AddGroupDialog((TreeNode) node)));
        toolStripMenuItem7.Enabled = groupBase.CanAddGroups();
        menu.Items.Add((ToolStripItem) toolStripMenuItem7);
        ToolStripMenuItem toolStripMenuItem8 = (ToolStripMenuItem) new DelegateMenuItem("Add smart group...", MenuNames.EditAddSmartGroup, (Action) (() => AddNodeDialogHelper.AddSmartGroupDialog((TreeNode) node)));
        toolStripMenuItem8.Enabled = groupBase.CanAddGroups();
        menu.Items.Add((ToolStripItem) toolStripMenuItem8);
        menu.Items.Add("-");
        FileGroup file = node as FileGroup;
        if (file != null)
        {
          ToolStripMenuItem toolStripMenuItem9 = (ToolStripMenuItem) new DelegateMenuItem("Save " + file.GetFilename(), MenuNames.FileSave, (Action) (() => Program.TheForm.DoFileSave(file)));
          toolStripMenuItem9.Enabled = file.AllowEdit(false);
          menu.Items.Add((ToolStripItem) toolStripMenuItem9);
          menu.Items.Add((ToolStripItem) new DelegateMenuItem("Close " + file.GetFilename(), MenuNames.FileClose, (Action) (() => Program.TheForm.DoFileClose(file))));
        }
        else
        {
          ToolStripMenuItem toolStripMenuItem9 = (ToolStripMenuItem) new DelegateMenuItem("Remo&ve servers", MenuNames.EditRemoveServers, (Action) (() => this.DoRemoveChildren(node)));
          toolStripMenuItem9.Enabled = groupBase.CanRemoveChildren();
          menu.Items.Add((ToolStripItem) toolStripMenuItem9);
          ToolStripMenuItem toolStripMenuItem10 = (ToolStripMenuItem) new DelegateMenuItem("Remove group", MenuNames.EditRemove, (Action) (() => this.ConfirmRemove(node, true)));
          toolStripMenuItem10.Enabled = node.CanRemove(false);
          menu.Items.Add((ToolStripItem) toolStripMenuItem10);
        }
        menu.Items.Add("-");
        ToolStripMenuItem toolStripMenuItem11 = (ToolStripMenuItem) new DelegateMenuItem("P&roperties", MenuNames.EditProperties, new Action(node.DoPropertiesDialog));
        toolStripMenuItem11.Enabled = node.HasProperties;
        menu.Items.Add((ToolStripItem) toolStripMenuItem11);
      }
      else
      {
        ServerBase server = node as ServerBase;
        MenuHelper.AddSessionMenuItems(menu, server);
        menu.Items.Add("-");
        MenuHelper.AddDockingMenuItems(menu, server);
        menu.Items.Add("-");
        MenuHelper.AddMaintenanceMenuItems(menu, server);
      }
    }

    public bool AnyOpenedEditableFiles() => this.Nodes.OfType<FileGroup>().Any<FileGroup>((Func<FileGroup, bool>) (file => file.AllowEdit(false)));

    private TreeNode FindNodeInList(TreeNodeCollection nodes, string name) => nodes.Cast<TreeNode>().Where<TreeNode>((Func<TreeNode, bool>) (node => node.Text == name)).FirstOrDefault<TreeNode>();

    public TreeNode FindNodeByName(string name)
    {
      if (name == this.RootNode.Text)
        return (TreeNode) this.RootNode;
      string[] strArray = name.Split(new string[1]
      {
        this.PathSeparator
      }, StringSplitOptions.None);
      TreeNodeCollection nodes = this.Nodes;
      TreeNode treeNode = (TreeNode) null;
      foreach (string name1 in strArray)
      {
        treeNode = this.FindNodeInList(nodes, name1);
        if (treeNode != null)
          nodes = treeNode.Nodes;
        else
          break;
      }
      return treeNode;
    }

    public void ConfirmRemove(RdcTreeNode node, bool askUser)
    {
      if (!node.ConfirmRemove(askUser))
        return;
      this.RemoveNode(node);
    }

    private void DoRemoveChildren(RdcTreeNode node)
    {
      GroupBase groupBase = node as GroupBase;
      if (groupBase.Nodes.Count > 0 && FormTools.YesNoDialog("Remove all children of the " + groupBase.Text + " group?") != DialogResult.Yes)
        return;
      groupBase.RemoveChildren();
    }

    protected override void OnItemDrag(ItemDragEventArgs e)
    {
      base.OnItemDrag(e);
      this._draggedNode = e.Item as RdcTreeNode;
      this._preDragSelectedNode = this.SelectedNode;
      int num = (int) this.DoDragDrop((object) this._draggedNode, DragDropEffects.Move);
    }

    protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
    {
      base.OnQueryContinueDrag(e);
      if ((e.KeyState & 3) != 0)
        return;
      this.SuspendSelect();
      this.SelectedNode = this._preDragSelectedNode;
      this._preDragSelectedNode = (TreeNode) null;
      this.ResumeSelect();
    }

    protected override void OnDragOver(DragEventArgs e)
    {
      base.OnDragOver(e);
      if (this.GetNodeAt(this.PointToClient(new Point(e.X, e.Y))) is RdcTreeNode nodeAt && this._draggedNode.CanDropOnTarget(nodeAt))
      {
        this.SuspendSelect();
        this.SelectedNode = (TreeNode) nodeAt;
        this.ResumeSelect();
        e.Effect = e.AllowedEffect;
      }
      else
      {
        this.SuspendSelect();
        this.SelectedNode = (TreeNode) this._draggedNode;
        this.ResumeSelect();
        e.Effect = DragDropEffects.None;
      }
    }

    protected override void OnDragDrop(DragEventArgs e)
    {
      base.OnDragDrop(e);
      if (!(this.GetNodeAt(this.PointToClient(new Point(e.X, e.Y))) is RdcTreeNode nodeAt))
        return;
      if (!(nodeAt is GroupBase groupBase))
        groupBase = nodeAt.Parent as GroupBase;
      GroupBase newParent = groupBase;
      if (newParent == this._draggedNode || newParent == this._draggedNode.Parent)
        return;
      this.MoveNode(this._draggedNode, newParent);
    }

    protected override void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
    {
      base.OnNodeMouseDoubleClick(e);
      if (e.Button != MouseButtons.Left || Helpers.IsControlKeyPressed || !(this.SelectedNode is ServerBase selectedNode))
        return;
      selectedNode.Connect();
      this.SetDelayedFocusServer(selectedNode);
    }

    public void MoveNode(RdcTreeNode node, GroupBase newParent)
    {
      if (newParent != null && newParent.HandleMove(node))
        return;
      this.Operation(OperationBehavior.RestoreSelected, (Action) (() =>
      {
        if (node.Parent == null)
        {
          this.Nodes.Remove((TreeNode) node);
          this.OnGroupChanged(this.RootNode, ChangeType.TreeChanged);
        }
        else
        {
          if (node is ServerBase && (node as ServerBase).ServerNode is TemporaryServer serverNode)
            node = (RdcTreeNode) serverNode;
          GroupBase parent = node.Parent as GroupBase;
          parent.Nodes.Remove((TreeNode) node);
          this.OnGroupChanged(parent, ChangeType.TreeChanged);
        }
        if (newParent == null)
        {
          this.Nodes.Add((TreeNode) node);
          this.OnGroupChanged(this.RootNode, ChangeType.TreeChanged);
        }
        else
        {
          newParent.Nodes.Add((TreeNode) node);
          this.OnGroupChanged(newParent, ChangeType.TreeChanged);
        }
        this.OnNodeChanged(node, ChangeType.TreeChanged);
        if (node.IsVisible)
          return;
        node.EnsureVisible();
      }));
    }

    public void OnGroupChanged(GroupBase group, ChangeType changeType)
    {
      if (this._noGroupChanged > 0 || group == null)
        return;
      Log.Write("OnGroupChanged({1}) {0}", (object) group.Text, (object) changeType);
      HashSet<RdcTreeNode> set = new HashSet<RdcTreeNode>();
      if (group == this.RootNode)
      {
        if (changeType.HasFlag((Enum) ChangeType.PropertyChanged))
          this.Nodes.VisitNodes((Action<RdcTreeNode>) (n => set.Add(n)));
      }
      else
      {
        group.CollectNodesToInvalidate(true, set);
        group.VisitParents((Action<RdcTreeNode>) (parent => parent.CollectNodesToInvalidate(false, set)));
      }
      this.InvalidateNodes(set);
      Action<GroupChangedEventArgs> groupChanged = this.GroupChanged;
      if (groupChanged == null)
        return;
      groupChanged(new GroupChangedEventArgs(group, changeType));
    }

    public void OnNodeChanged(RdcTreeNode node, ChangeType changeType)
    {
      Log.Write("OnNodeChanged({1}) {0}", (object) node.Text, (object) changeType);
      if (this.SortNode(node) && node.Parent is GroupBase parent)
        this.OnGroupChanged(parent, ChangeType.InvalidateUI);
      if (node is GroupBase group)
        this.OnGroupChanged(group, changeType);
      else
        this.OnServerChanged(node as ServerBase, changeType);
      Program.TheForm.SetTitle();
    }

    private void OnServerChanged(ServerBase serverBase, ChangeType changeType)
    {
      HashSet<RdcTreeNode> set = new HashSet<RdcTreeNode>();
      serverBase.CollectNodesToInvalidate(false, set);
      this.InvalidateNodes(set);
      Action<ServerChangedEventArgs> serverChanged = this.ServerChanged;
      if (serverChanged == null)
        return;
      serverChanged(new ServerChangedEventArgs(serverBase, changeType));
    }

    public void SuspendSelect() => Interlocked.Increment(ref this._noSelectCounter);

    public void ResumeSelect() => Interlocked.Decrement(ref this._noSelectCounter);

    public void SuspendSort() => Interlocked.Increment(ref this._noSortCounter);

    public void ResumeSort() => Interlocked.Decrement(ref this._noSortCounter);

    public void SuspendGroupChanged() => Interlocked.Increment(ref this._noGroupChanged);

    public void ResumeGroupChanged() => Interlocked.Decrement(ref this._noGroupChanged);

    public static ImageConstants TranslateImage(ImageConstants index, bool toSelected) => ServerTree.ImageConstantLookup[toSelected ? 1 : 0, (int) index];

    private void InvalidateNodes(HashSet<RdcTreeNode> set)
    {
      foreach (RdcTreeNode rdcTreeNode in (IEnumerable<RdcTreeNode>) set.OrderByDescending<RdcTreeNode, RdcTreeNode>((Func<RdcTreeNode, RdcTreeNode>) (n => n), (IComparer<RdcTreeNode>) new ServerTree.InvalidateComparer()))
        rdcTreeNode.InvalidateNode();
    }

    private class RootNodeGroup : GroupBase
    {
      public RootNodeGroup() => this.Text = "[root]";

      public override void OnRemoving() => throw new NotImplementedException();

      public override void DoPropertiesDialog(Form parentForm, string activeTabName) => throw new NotImplementedException();

      protected override void InitSettings()
      {
      }
    }

    private class InvalidateComparer : IComparer<RdcTreeNode>
    {
      public int Compare(RdcTreeNode x, RdcTreeNode y) => this.GetOrdinal(x) - this.GetOrdinal(y);

      private int GetOrdinal(RdcTreeNode node)
      {
        switch (node)
        {
          case Server _:
            return 9999;
          case ServerRef _:
            return 8888;
          default:
            return node.GetPathLength();
        }
      }
    }
  }
}
