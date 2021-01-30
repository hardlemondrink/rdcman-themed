// Decompiled with JetBrains decompiler
// Type: RdcMan.SmartGroup
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  internal class SmartGroup : VirtualGroup
  {
    internal const string XmlNodeName = "smartGroup";
    private static List<SmartGroup> AllSmartGroups = new List<SmartGroup>();
    protected new static Dictionary<string, Helpers.ReadXmlDelegate> NodeActions = new Dictionary<string, Helpers.ReadXmlDelegate>((IDictionary<string, Helpers.ReadXmlDelegate>) GroupBase.NodeActions);

    static SmartGroup()
    {
      SmartGroup.NodeActions["ruleGroup"] = (Helpers.ReadXmlDelegate) ((childNode, node, errors) => (node as SmartGroup).RuleGroup = RuleGroup.Create(childNode, node, errors));
      ServerTree.Instance.GroupChanged += new Action<GroupChangedEventArgs>(SmartGroup.OnGroupChanged);
      ServerTree.Instance.ServerChanged += new Action<ServerChangedEventArgs>(SmartGroup.OnServerChanged);
    }

    protected SmartGroup() => SmartGroup.AllSmartGroups.Add(this);

    protected override void InitSettings()
    {
      this.Properties = (CommonNodeSettings) new SmartGroupSettings();
      this.AllSettingsGroups.Add((SettingsGroup) this.Properties);
      this.RuleGroup = new RuleGroup(RuleGroupOperator.All, Enumerable.Empty<Rule>());
    }

    public static SmartGroup CreateForAdd() => new SmartGroup();

    public static SmartGroup Create(SmartGroupPropertiesDialog dlg)
    {
      SmartGroup associatedNode = dlg.AssociatedNode as SmartGroup;
      associatedNode.UpdateSettings((NodePropertiesDialog) dlg);
      associatedNode.FinishConstruction(dlg.PropertiesPage.ParentGroup);
      associatedNode.Refresh();
      return associatedNode;
    }

    public static SmartGroup Create(
      XmlNode xmlNode,
      GroupBase parent,
      ICollection<string> errors)
    {
      SmartGroup smartGroup = new SmartGroup();
      smartGroup.FinishConstruction(parent);
      smartGroup.ReadXml(xmlNode, errors);
      smartGroup.Text = smartGroup.Properties.GroupName.Value;
      return smartGroup;
    }

    public SmartGroupSettings Properties => base.Properties as SmartGroupSettings;

    public RuleGroup RuleGroup { get; private set; }

    public override sealed bool CanDropServers() => false;

    public override void OnRemoving()
    {
      base.OnRemoving();
      SmartGroup.AllSmartGroups.Remove(this);
    }

    public static void OnGroupChanged(GroupChangedEventArgs e)
    {
      if (!e.ChangeType.HasFlag((Enum) ChangeType.TreeChanged) && !e.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged))
        return;
      SmartGroup.RefreshScope(e.Group, (Action<SmartGroup>) (group => group.Refresh()));
    }

    public static void OnServerChanged(ServerChangedEventArgs e)
    {
      if (!e.ChangeType.HasFlag((Enum) ChangeType.TreeChanged) && !e.ChangeType.HasFlag((Enum) ChangeType.PropertyChanged))
        return;
      Server server = e.Server as Server;
      if (server == null)
        return;
      bool dummy = false;
      SmartGroup.RefreshScope(server.Parent as GroupBase, (Action<SmartGroup>) (group => group.UpdateForServer(server, ref dummy)));
    }

    public static void RefreshAll(FileGroup fileGroup) => SmartGroup.AllSmartGroups.ForEach((Action<SmartGroup>) (group =>
    {
      if (group.FileGroup != fileGroup)
        return;
      group.Refresh();
    }));

    private static void RefreshScope(GroupBase scope, Action<SmartGroup> process)
    {
      if (scope == null)
        return;
      scope.VisitParents((Action<RdcTreeNode>) (parent =>
      {
        foreach (SmartGroup smartGroup in parent.Nodes.OfType<SmartGroup>())
          process(smartGroup);
      }));
    }

    public override sealed void DoPropertiesDialog(Form parentForm, string activeTabName)
    {
      using (SmartGroupPropertiesDialog propertiesDialog = SmartGroupPropertiesDialog.NewPropertiesDialog(this, parentForm))
      {
        propertiesDialog.SetActiveTab(activeTabName);
        if (propertiesDialog.ShowDialog() != DialogResult.OK)
          return;
        this.UpdateSettings((NodePropertiesDialog) propertiesDialog);
        this.Refresh();
      }
    }

    public override sealed bool CanRemoveChildren() => false;

    internal override void ReadXml(XmlNode xmlNode, ICollection<string> errors) => this.ReadXml(SmartGroup.NodeActions, xmlNode, errors);

    internal override void WriteXml(XmlTextWriter tw)
    {
      tw.WriteStartElement("smartGroup");
      this.Properties.Expanded.Value = this.IsExpanded;
      this.WriteXmlSettingsGroups(tw);
      this.RuleGroup.WriteXml(tw);
      tw.WriteEndElement();
    }

    public void Refresh()
    {
      bool changed = false;
      using (Helpers.Timer("refreshing smart group {0}", (object) this.Text))
        ServerTree.Instance.Operation(OperationBehavior.SuspendSort | OperationBehavior.SuspendUpdate | OperationBehavior.SuspendGroupChanged, (Action) (() =>
        {
          HashSet<SmartServerRef> set = new HashSet<SmartServerRef>();
          this.Nodes.ForEach((Action<TreeNode>) (s => set.Add(s as SmartServerRef)));
          this.GetParentNodes().VisitNodes((Func<RdcTreeNode, NodeVisitorResult>) (node =>
          {
            switch (node)
            {
              case VirtualGroup _:
                return NodeVisitorResult.NoRecurse;
              case Server server:
                set.Remove(this.UpdateForServer(server, ref changed));
                break;
            }
            return NodeVisitorResult.Continue;
          }));
          if (set.Count <= 0)
            return;
          changed = true;
          set.ForEach<SmartServerRef>((Action<SmartServerRef>) (s => ServerTree.Instance.RemoveNode((RdcTreeNode) s)));
        }));
      if (!changed)
        return;
      ServerTree.Instance.SortGroup((GroupBase) this);
      ServerTree.Instance.OnGroupChanged((GroupBase) this, ChangeType.InvalidateUI);
    }

    private SmartServerRef UpdateForServer(Server server, ref bool changed)
    {
      SmartServerRef smartServerRef = server.FindServerRef<SmartServerRef>((GroupBase) this);
      bool flag = this.RuleGroup != null && this.RuleGroup.Evaluate(server);
      if (smartServerRef != null != flag)
      {
        changed = true;
        if (flag)
        {
          smartServerRef = new SmartServerRef(server);
          ServerTree.Instance.AddNode((RdcTreeNode) smartServerRef, (GroupBase) this);
        }
        else
          ServerTree.Instance.RemoveNode((RdcTreeNode) smartServerRef);
      }
      return smartServerRef;
    }

    private void FinishConstruction(GroupBase parent)
    {
      ServerTree.Instance.AddNode((RdcTreeNode) this, parent);
      this.ChangeImageIndex(ImageConstants.SmartGroup);
    }
  }
}
