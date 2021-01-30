// Decompiled with JetBrains decompiler
// Type: RdcMan.AddNodeDialogHelper
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RdcMan
{
  public static class AddNodeDialogHelper
  {
    public static void AddServersDialog() => AddNodeDialogHelper.AddServersDialog(ServerTree.Instance.SelectedNode);

    public static void AddServersDialog(TreeNode suggestedParentNode)
    {
      if (!ServerTree.Instance.AnyOpenedEditableFiles())
      {
        AddNodeDialogHelper.NotifyUserFileNeeded();
      }
      else
      {
        if (!ServerTree.Instance.Nodes.OfType<FileGroup>().Any<FileGroup>() && FormTools.YesNoDialog("RDCMan does not allow mixing of servers and groups. If you add a server to a top-level file group you will not be able to add any groups to that file. Continue?") != DialogResult.Yes)
          return;
        GroupBase groupBase = AddNodeDialogHelper.GetParentGroupForServerAdd(suggestedParentNode);
        ServerPropertiesDialog dlg = ServerPropertiesDialog.NewAddDialog(groupBase);
        if (dlg == null)
        {
          FormTools.InformationDialog("RDCMan does not allow mixing of servers and groups. In order to add a group to this file you must first remove the servers.");
        }
        else
        {
          using (dlg)
          {
            if (dlg.ShowDialog() != DialogResult.OK)
              return;
            groupBase = dlg.PropertiesPage.ParentGroup;
            (dlg.AssociatedNode as Server).UpdateSettings((NodePropertiesDialog) dlg);
            ServerTree.Instance.Operation(OperationBehavior.SuspendSort | OperationBehavior.SuspendUpdate | OperationBehavior.SuspendGroupChanged, (Action) (() =>
            {
              List<string> expandedServerNames = (dlg.PropertiesPage as ServerPropertiesTabPage).ExpandedServerNames;
              if (expandedServerNames.Count == 1)
              {
                Server.Create(dlg);
              }
              else
              {
                foreach (string name in expandedServerNames)
                  Server.Create(name, dlg);
              }
            }));
          }
          AddNodeDialogHelper.FinishAddServers(groupBase);
        }
      }
    }

    private static void FinishAddServers(GroupBase parentGroup)
    {
      ServerTree.Instance.SortGroup(parentGroup);
      ServerTree.Instance.OnGroupChanged(parentGroup, ChangeType.TreeChanged);
      ServerTree.Instance.SelectedNode = (TreeNode) parentGroup;
      parentGroup.Expand();
    }

    public static void ImportServersDialog() => AddNodeDialogHelper.ImportServersDialog(ServerTree.Instance.SelectedNode);

    public static void ImportServersDialog(TreeNode parentGroup)
    {
      if (!ServerTree.Instance.AnyOpenedEditableFiles())
      {
        AddNodeDialogHelper.NotifyUserFileNeeded();
      }
      else
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        AddNodeDialogHelper.\u003C\u003Ec__DisplayClass6 cDisplayClass6_1 = new AddNodeDialogHelper.\u003C\u003Ec__DisplayClass6();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass6_1.dlg = ServerPropertiesDialog.NewImportDialog(AddNodeDialogHelper.GetParentGroupForServerAdd(parentGroup));
        try
        {
          // ISSUE: reference to a compiler-generated field
          if (cDisplayClass6_1.dlg.ShowDialog() != DialogResult.OK)
            return;
          // ISSUE: reference to a compiler-generated field
          GroupBase group = cDisplayClass6_1.dlg.PropertiesPage.ParentGroup;
          ServerTree.Instance.Operation(OperationBehavior.SuspendSort | OperationBehavior.SuspendUpdate | OperationBehavior.SuspendGroupChanged, (Action) (() =>
          {
            Server associatedNode = dlg.AssociatedNode as Server;
            associatedNode.UpdateSettings((NodePropertiesDialog) dlg);
            foreach (string expandedServerName in (dlg.PropertiesPage as ImportServersPropertiesPage).ExpandedServerNames)
            {
              // ISSUE: variable of a compiler-generated type
              AddNodeDialogHelper.\u003C\u003Ec__DisplayClass6 cDisplayClass6 = cDisplayClass6_1;
              string serverName = expandedServerName;
              Server server = group.Nodes.OfType<Server>().Where<Server>((Func<Server, bool>) (s => s.ServerName == serverName)).FirstOrDefault<Server>();
              if (server != null)
                server.UpdateFromTemplate(associatedNode);
              else
                Server.Create(serverName, dlg);
            }
          }));
          AddNodeDialogHelper.FinishAddServers(group);
        }
        finally
        {
          // ISSUE: reference to a compiler-generated field
          if (cDisplayClass6_1.dlg != null)
          {
            // ISSUE: reference to a compiler-generated field
            cDisplayClass6_1.dlg.Dispose();
          }
        }
      }
    }

    public static void AddGroupDialog() => AddNodeDialogHelper.AddGroupDialog(ServerTree.Instance.SelectedNode);

    public static void AddGroupDialog(TreeNode suggestedParentNode)
    {
      if (!ServerTree.Instance.AnyOpenedEditableFiles())
      {
        AddNodeDialogHelper.NotifyUserFileNeeded();
      }
      else
      {
        GroupPropertiesDialog dlg = GroupPropertiesDialog.NewAddDialog(AddNodeDialogHelper.GetParentGroupForGroupAdd(suggestedParentNode));
        if (dlg == null)
        {
          FormTools.InformationDialog("RDCMan does not allow mixing of servers and groups. In order to add a group to this file you must first remove the servers.");
        }
        else
        {
          using (dlg)
          {
            if (dlg.ShowDialog() != DialogResult.OK)
              return;
            ServerTree.Instance.SelectedNode = (TreeNode) Group.Create(dlg);
          }
        }
      }
    }

    public static void AddSmartGroupDialog() => AddNodeDialogHelper.AddSmartGroupDialog(ServerTree.Instance.SelectedNode);

    public static void AddSmartGroupDialog(TreeNode suggestedParentNode)
    {
      if (!ServerTree.Instance.AnyOpenedEditableFiles())
      {
        AddNodeDialogHelper.NotifyUserFileNeeded();
      }
      else
      {
        SmartGroupPropertiesDialog dlg = SmartGroupPropertiesDialog.NewAddDialog(AddNodeDialogHelper.GetParentGroupForGroupAdd(suggestedParentNode));
        if (dlg == null)
        {
          FormTools.InformationDialog("RDCMan does not allow mixing of servers and groups. In order to add a group to this file you must first remove the servers.");
        }
        else
        {
          using (dlg)
          {
            if (dlg.ShowDialog() != DialogResult.OK)
              return;
            ServerTree.Instance.SelectedNode = (TreeNode) SmartGroup.Create(dlg);
          }
        }
      }
    }

    private static GroupBase GetParentGroupForServerAdd(TreeNode node)
    {
      GroupBase groupBase1 = (GroupBase) null;
      if (node != null)
      {
        if (!(node is GroupBase groupBase2))
          groupBase2 = node.Parent as GroupBase;
        groupBase1 = groupBase2;
        while (groupBase1 != null && !groupBase1.CanAddServers())
          groupBase1 = groupBase1.Parent as GroupBase;
      }
      return groupBase1;
    }

    private static GroupBase GetParentGroupForGroupAdd(TreeNode node)
    {
      GroupBase groupBase1 = (GroupBase) null;
      if (node != null)
      {
        if (!(node is GroupBase groupBase2))
          groupBase2 = node.Parent as GroupBase;
        groupBase1 = groupBase2;
        while (groupBase1 != null && !groupBase1.CanAddGroups())
          groupBase1 = groupBase1.Parent as GroupBase;
      }
      return groupBase1;
    }

    private static void NotifyUserFileNeeded() => FormTools.InformationDialog("Please open an existing non-read-only file (File.Open) or create a new one (File.New) before adding servers/groups");
  }
}
