// Decompiled with JetBrains decompiler
// Type: RdcMan.SortExtensions
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RdcMan
{
  internal static class SortExtensions
  {
    public static void SortBuiltinGroups(this ServerTree tree)
    {
      if (tree.SuppressSorting)
        return;
      List<TreeNode> list = tree.Nodes.OfType<IBuiltInVirtualGroup>().Cast<TreeNode>().ToList<TreeNode>();
      tree.Nodes.SortAndRebuildNodeList(list, SortOrder.ByName);
    }

    public static void SortAllNodes(this ServerTree tree)
    {
      using (Helpers.Timer("sorting all nodes"))
        tree.Operation(OperationBehavior.RestoreSelected, (Action) (() =>
        {
          tree.SortHelper(tree.Nodes, true);
          tree.SortBuiltinGroups();
        }));
    }

    public static void SortRoot(this ServerTree tree) => tree.Operation(OperationBehavior.RestoreSelected | OperationBehavior.SuspendUpdate, (Action) (() =>
    {
      tree.SortHelper(tree.Nodes, false);
      tree.SortBuiltinGroups();
    }));

    public static bool SortGroup(this ServerTree tree, GroupBase group) => tree.SortGroup(group, false);

    public static bool SortGroup(this ServerTree tree, GroupBase group, bool recurse)
    {
      bool flag = false;
      if (group.AllowSort)
        flag = tree.SortHelper(group.Nodes, recurse);
      return flag;
    }

    public static bool SortNode(this ServerTree tree, RdcTreeNode node)
    {
      if (node.Parent is GroupBase parent)
        return tree.SortGroup(parent);
      if (node.Parent == null)
        tree.SortRoot();
      return false;
    }

    private static bool SortHelper(this ServerTree tree, TreeNodeCollection nodes, bool recurse)
    {
      if (tree.SuppressSorting)
        return false;
      bool anyChanged = false;
      tree.Operation(OperationBehavior.RestoreSelected | OperationBehavior.SuspendUpdate, (Action) (() => anyChanged = tree.SortNodes(nodes, recurse)));
      return anyChanged;
    }

    private static bool SortNodes(this ServerTree tree, TreeNodeCollection nodes, bool recurse)
    {
      List<TreeNode> treeNodeList = new List<TreeNode>(nodes.Count);
      List<TreeNode> list = new List<TreeNode>(nodes.Count);
      foreach (TreeNode node in nodes)
      {
        if (node is ServerBase)
          list.Add(node);
        else
          treeNodeList.Add(node);
      }
      bool flag = false;
      if (recurse)
      {
        foreach (GroupBase groupBase in treeNodeList.OfType<GroupBase>().Where<GroupBase>((Func<GroupBase, bool>) (g => g.AllowSort)))
          flag |= tree.SortNodes(groupBase.Nodes, true);
      }
      return flag | nodes.SortAndRebuildNodeList(treeNodeList, Program.Preferences.GroupSortOrder) | nodes.SortAndRebuildNodeList(list, Program.Preferences.ServerSortOrder);
    }

    private static bool SortAndRebuildNodeList(
      this TreeNodeCollection nodes,
      List<TreeNode> list,
      SortOrder sortOrder)
    {
      if (list.Count == 0 || sortOrder == SortOrder.None)
        return false;
      list.Sort((IComparer<TreeNode>) new SortExtensions.ServerTreeSortComparer(sortOrder));
      TreeNode treeNode = nodes[0];
      bool flag = false;
      foreach (TreeNode node in list)
      {
        if (node == treeNode)
        {
          treeNode = treeNode.NextNode;
        }
        else
        {
          flag = true;
          nodes.Remove(node);
          nodes.Insert(treeNode.Index, node);
        }
      }
      return flag;
    }

    private class ServerTreeSortComparer : IComparer<TreeNode>
    {
      private SortOrder _sortOrder;

      public ServerTreeSortComparer(SortOrder sortOrder) => this._sortOrder = sortOrder;

      public int Compare(TreeNode treeNode1, TreeNode treeNode2)
      {
        if (this._sortOrder == SortOrder.ByStatus)
        {
          ImageConstants imageConstants = ServerTree.TranslateImage((ImageConstants) treeNode1.ImageIndex, false);
          int num = ServerTree.TranslateImage((ImageConstants) treeNode2.ImageIndex, false) - imageConstants;
          if (num != 0)
            return num;
        }
        return Helpers.NaturalCompare(treeNode1.Text, treeNode2.Text);
      }
    }
  }
}
