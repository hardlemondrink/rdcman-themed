// Decompiled with JetBrains decompiler
// Type: RdcMan.GroupBase
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using RdcMan.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  public abstract class GroupBase : RdcTreeNode
  {
    internal const string XmlGroupNameTag = "name";
    internal const string XmlExpandedTag = "expanded";
    internal const string XmlCommentTag = "comment";
    internal static int SchemaVersion;
    protected static Dictionary<string, Helpers.ReadXmlDelegate> NodeActions = new Dictionary<string, Helpers.ReadXmlDelegate>()
    {
      {
        "properties",
        (Helpers.ReadXmlDelegate) ((childNode, parent, errors) =>
        {
          if (GroupBase.SchemaVersion <= 2)
            (parent as GroupBase).ReadXml(GroupBase.PropertyActions, childNode, errors);
          else
            (parent as GroupBase).ReadXmlSettingsGroup(childNode, errors);
        })
      },
      {
        "server",
        (Helpers.ReadXmlDelegate) ((childNode, parent, errors) => LongRunningActionForm.Instance.UpdateStatus(Server.Create(childNode, parent as GroupBase, errors).Properties.DisplayName.Value))
      },
      {
        "group",
        (Helpers.ReadXmlDelegate) ((childNode, parent, errors) => LongRunningActionForm.Instance.UpdateStatus(Group.Create(childNode, parent as GroupBase, errors).Properties.GroupName.Value))
      },
      {
        "smartGroup",
        (Helpers.ReadXmlDelegate) ((childNode, parent, errors) => LongRunningActionForm.Instance.UpdateStatus(((GroupBase) SmartGroup.Create(childNode, parent as GroupBase, errors)).Properties.GroupName.Value))
      }
    };
    protected static Dictionary<string, Helpers.ReadXmlDelegate> PropertyActions = new Dictionary<string, Helpers.ReadXmlDelegate>()
    {
      {
        "name",
        (Helpers.ReadXmlDelegate) ((childNode, node, errors) => (node as GroupBase).Properties.GroupName.Value = childNode.InnerText)
      },
      {
        "expanded",
        (Helpers.ReadXmlDelegate) ((childNode, node, errors) =>
        {
          bool result;
          bool.TryParse(childNode.InnerText, out result);
          (node as GroupBase).Properties.Expanded.Value = result;
        })
      },
      {
        "comment",
        (Helpers.ReadXmlDelegate) ((childNode, node, errors) => (node as GroupBase).Properties.Comment.Value = childNode.InnerText)
      }
    };
    private int _numberOfConnectedServers = -1;
    private int _numberOfServers = -1;

    static GroupBase() => Server.ConnectionStateChanged += new Action<ConnectionStateChangedEventArgs>(GroupBase.Server_ConnectionStateChanged);

    private static void Server_ConnectionStateChanged(ConnectionStateChangedEventArgs args)
    {
      (args.Server.Parent as GroupBase).OnConnectionStateChange((ServerBase) args.Server);
      args.Server.VisitServerRefs((Action<ServerRef>) (r => (r.Parent as GroupBase).OnConnectionStateChange((ServerBase) r)));
    }

    public bool IsReadOnly { get; protected set; }

    public virtual bool CanRemoveChildren() => this.Nodes.Count > 0 && this.AllowEdit(false);

    public GroupDisplaySettings DisplaySettings => base.DisplaySettings as GroupDisplaySettings;

    public GroupSettings Properties
    {
      get => base.Properties as GroupSettings;
      set => this.Properties = (CommonNodeSettings) value;
    }

    protected override void InitSettings()
    {
      this.DisplaySettings = (CommonDisplaySettings) new GroupDisplaySettings();
      base.InitSettings();
    }

    internal override void UpdateSettings(NodePropertiesDialog dlg)
    {
      base.UpdateSettings(dlg);
      this.Text = this.Properties.GroupName.Value;
    }

    internal bool HasServers => this.Nodes.Count != 0 && this.Nodes[0] is ServerBase;

    internal int NumberOfServers
    {
      get
      {
        if (this._numberOfServers == -1)
        {
          this._numberOfServers = 0;
          foreach (TreeNode node in this.Nodes)
          {
            if (node is GroupBase groupBase)
              this._numberOfServers += groupBase.NumberOfServers;
            else
              ++this._numberOfServers;
          }
        }
        return this._numberOfServers;
      }
    }

    internal int NumberOfConnectedServers
    {
      get
      {
        if (this._numberOfConnectedServers == -1)
        {
          this._numberOfConnectedServers = 0;
          foreach (TreeNode node in this.Nodes)
          {
            if (node is GroupBase groupBase)
              this._numberOfConnectedServers += groupBase.NumberOfConnectedServers;
            else if ((node as ServerBase).IsConnected)
              ++this._numberOfConnectedServers;
          }
        }
        return this._numberOfConnectedServers;
      }
    }

    public virtual bool AllowSort => true;

    public override void InvalidateNode()
    {
      this._numberOfServers = -1;
      GroupBase.ResetConnectionStatistics(this);
      base.InvalidateNode();
    }

    internal void ResetConnectionStatistics() => this.VisitNodeAndParents((Action<RdcTreeNode>) (group => GroupBase.ResetConnectionStatistics((GroupBase) group)));

    private static void ResetConnectionStatistics(GroupBase group) => group._numberOfConnectedServers = -1;

    public virtual bool CanAddServers() => this.CanDropServers();

    public virtual bool CanAddGroups() => this.CanDropGroups();

    public virtual bool CanDropServers() => !this.HasGroups && this.AllowEdit(false);

    public virtual bool CanDropGroups() => !this.HasServers && this.AllowEdit(false);

    public virtual DragDropEffects DropBehavior() => DragDropEffects.Move;

    public bool HasGroups => this.Nodes.Count > 0 && this.Nodes[0] is GroupBase;

    internal override void Show() => Program.TheForm.ShowGroup(this);

    internal override void Hide() => Program.TheForm.HideGroup(this);

    internal override void WriteXml(XmlTextWriter tw)
    {
      this.Properties.Expanded.Value = this.IsExpanded;
      this.WriteXmlSettingsGroups(tw);
      foreach (RdcTreeNode node in this.Nodes)
        node.WriteXml(tw);
    }

    public void RemoveChildren()
    {
      ServerTree.Instance.Operation(OperationBehavior.SuspendSelect | OperationBehavior.SuspendSort | OperationBehavior.SuspendUpdate | OperationBehavior.SuspendGroupChanged, (Action) (() =>
      {
        this.Nodes.ForEach((Action<TreeNode>) (node => (node as RdcTreeNode).OnRemoving()));
        this.Nodes.Clear();
      }));
      ServerTree.Instance.OnGroupChanged(this, ChangeType.TreeChanged);
    }

    public override void OnRemoving()
    {
      this.Hide();
      this.RemoveChildren();
    }

    public override void Connect() => this.ConnectAs((LogonCredentials) null, (ConnectionSettings) null);

    public override void ConnectAs(
      LogonCredentials logonSettings,
      ConnectionSettings connectionSettings)
    {
      List<ServerBase> allChildren = this.GetAllChildren<ServerBase>((Predicate<ServerBase>) (s => !s.IsConnected));
      int count = allChildren.Count;
      if (count >= Current.RdcManSection.WarningThresholds.Connect)
      {
        if (FormTools.YesNoDialog(this.Text + " group contains " + (object) count + " disconnected servers. Are you sure?") != DialogResult.Yes)
          return;
      }
      NodeHelper.ThrottledConnectAs((IEnumerable<ServerBase>) allChildren, logonSettings, connectionSettings);
    }

    public override void Reconnect() => this.GetAllChildren<ServerBase>((Predicate<ServerBase>) (s => s.IsConnected)).ForEach((Action<ServerBase>) (server => server.Reconnect()));

    public override void Disconnect() => NodeHelper.ThrottledDisconnect((IEnumerable<ServerBase>) this.GetAllChildren<ServerBase>((Predicate<ServerBase>) (s => s.IsConnected)));

    public override void LogOff()
    {
      foreach (RdcTreeNode allChild in this.GetAllChildren<ServerBase>((Predicate<ServerBase>) (s => s.IsConnected)))
      {
        allChild.LogOff();
        Thread.Sleep(25);
      }
    }

    private void OnConnectionStateChange(ServerBase server)
    {
      this.InheritSettings();
      if (!server.IsConnected)
      {
        ReconnectServerRef serverRef = server.ServerNode.FindServerRef<ReconnectServerRef>();
        if (serverRef != null && serverRef.NeedToReconnect)
          return;
      }
      bool flag = false;
      if (Program.Preferences.ServerSortOrder == SortOrder.ByStatus)
        flag |= ServerTree.Instance.SortNode((RdcTreeNode) server);
      if (!(flag | !(server.ServerNode.Parent as GroupBase).DisplaySettings.ShowDisconnectedThumbnails.Value))
        return;
      ServerTree.Instance.OnGroupChanged(this, ChangeType.InvalidateUI);
    }

    public override bool CanDropOnTarget(RdcTreeNode targetNode)
    {
      if (targetNode == this)
        return true;
      if (!(targetNode is GroupBase groupBase) || !groupBase.CanDropGroups() || !this.AllowEdit(false))
        return false;
      for (; groupBase != null; groupBase = groupBase.Parent as GroupBase)
      {
        if (groupBase.Parent == this)
          return false;
      }
      return true;
    }

    public override bool ConfirmRemove(bool askUser)
    {
      if (!this.CanRemove(true))
        return false;
      bool anyConnected;
      this.AnyOrAllConnected(out anyConnected, out bool _);
      if (anyConnected)
      {
        FormTools.InformationDialog("There are active sessions in the " + this.Text + " group. Disconnect them before removing the group.");
        return false;
      }
      return !askUser || this.Nodes.Count <= 0 || FormTools.YesNoDialog("Remove " + this.Text + " group?") == DialogResult.Yes;
    }

    public void AnyOrAllConnected(out bool anyConnected, out bool allConnected)
    {
      bool any = false;
      bool all = true;
      this.Nodes.VisitNodes((Action<RdcTreeNode>) (node =>
      {
        if (!(node is ServerBase serverBase))
          return;
        if (serverBase.IsConnected)
          any = true;
        else
          all = false;
      }));
      anyConnected = any;
      allConnected = all;
    }

    internal virtual void ReadXml(XmlNode xmlNode, ICollection<string> errors) => this.ReadXml(GroupBase.NodeActions, xmlNode, errors);
  }
}
