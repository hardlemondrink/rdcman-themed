// Decompiled with JetBrains decompiler
// Type: RdcMan.NodeHelper
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Win32;

namespace RdcMan
{
  internal static class NodeHelper
  {
    private const int ThrottledConnectDelayInMilliseconds = 125;
    private const int ThrottledDisconnectDelayInMilliseconds = 25;

    public static TreeNodeCollection GetParentNodes(this TreeNode node)
    {
      TreeNode parent = node.Parent;
      return parent != null ? parent.Nodes : node.TreeView.Nodes;
    }

    public static void VisitNodes(this RdcTreeNode node, Action<RdcTreeNode> callback)
    {
      callback(node);
      node.Nodes.VisitNodes(callback);
    }

    public static void VisitNodes(this ICollection nodes, Action<RdcTreeNode> callback)
    {
      foreach (RdcTreeNode node in (IEnumerable) nodes)
        node.VisitNodes(callback);
    }

    public static bool VisitNodes(
      this RdcTreeNode node,
      Func<RdcTreeNode, NodeVisitorResult> callback)
    {
      switch (callback(node))
      {
        case NodeVisitorResult.NoRecurse:
          return true;
        case NodeVisitorResult.Break:
          return false;
        default:
          return node.Nodes.VisitNodes(callback);
      }
    }

    public static bool VisitNodes(
      this ICollection nodes,
      Func<RdcTreeNode, NodeVisitorResult> callback)
    {
      foreach (RdcTreeNode node in (IEnumerable) nodes)
      {
        if (!node.VisitNodes(callback))
          return false;
      }
      return true;
    }

    public static void VisitNodeAndParents(this RdcTreeNode node, Action<RdcTreeNode> callback)
    {
      callback(node);
      node.VisitParents(callback);
    }

    public static void VisitParents(this RdcTreeNode node, Action<RdcTreeNode> callback)
    {
      for (RdcTreeNode parent = node.Parent as RdcTreeNode; parent != null; parent = parent.Parent as RdcTreeNode)
        callback(parent);
    }

    public static void VisitNodeAndParents(
      this RdcTreeNode node,
      Func<RdcTreeNode, NodeVisitorResult> callback)
    {
      if (callback(node) != NodeVisitorResult.Continue)
        return;
      node.VisitParents(callback);
    }

    public static void VisitParents(
      this RdcTreeNode node,
      Func<RdcTreeNode, NodeVisitorResult> callback)
    {
      RdcTreeNode parent = node.Parent as RdcTreeNode;
      while (parent != null && callback(parent) == NodeVisitorResult.Continue)
        parent = parent.Parent as RdcTreeNode;
    }

    public static List<TNode> GetAllChildren<TNode>(
      this RdcTreeNode parent,
      Predicate<TNode> predicate)
      where TNode : RdcTreeNode
    {
      List<TNode> children = new List<TNode>(parent.Nodes.Count);
      parent.VisitNodes((Action<RdcTreeNode>) (child =>
      {
        if (!(child is TNode node) || !predicate(node))
          return;
        children.Add(node);
      }));
      return children;
    }

    public static void AnyOrAllConnected(
      IEnumerable<ServerBase> servers,
      out bool anyConnected,
      out bool allConnected)
    {
      anyConnected = false;
      allConnected = true;
      foreach (ServerBase server in servers)
      {
        if (server.IsConnected)
          anyConnected = true;
        else
          allConnected = false;
      }
    }

    public static void SelectNewActiveConnection(bool selectPrevious)
    {
      List<ServerBase> connectedServers = new List<ServerBase>();
      ConnectedGroup.Instance.Nodes.VisitNodes((Action<RdcTreeNode>) (node => connectedServers.Add(node as ServerBase)));
      connectedServers.Sort((Comparison<ServerBase>) ((a, b) => (b as ConnectedServerRef).LastFocusTime.CompareTo((a as ConnectedServerRef).LastFocusTime)));
      ServerBase serverBase = (ServerBase) null;
      if (selectPrevious)
      {
        serverBase = connectedServers[Math.Min(1, connectedServers.Count - 1)];
      }
      else
      {
        using (SelectActiveServerForm activeServerForm = new SelectActiveServerForm((IEnumerable<ServerBase>) connectedServers))
        {
          if (activeServerForm.ShowDialog() == DialogResult.OK)
          {
            SelectActiveServerForm.SelectedObject selected = activeServerForm.Selected;
            switch (selected.Operation)
            {
              case SelectActiveServerForm.Operation.SelectServer:
                serverBase = selected.Server;
                break;
              case SelectActiveServerForm.Operation.SelectTree:
                Program.TheForm.GoToServerTree();
                break;
              case SelectActiveServerForm.Operation.MinimizeWindow:
                User.SetForegroundWindow(User.GetWindow(Program.TheForm.Handle, 2U));
                Program.TheForm.WindowState = FormWindowState.Minimized;
                break;
            }
          }
        }
      }
      if (serverBase == null)
        return;
      Program.TheForm.SwitchFullScreenClient(serverBase.ServerNode);
      ServerTree.Instance.SelectedNode = (TreeNode) serverBase;
      serverBase.Focus();
    }

    public static void ThrottledConnect(IEnumerable<ServerBase> servers) => NodeHelper.ThrottledConnectAs(servers, (LogonCredentials) null, (ConnectionSettings) null);

    public static void ThrottledConnect(
      IEnumerable<ServerBase> servers,
      Action<ServerBase> postConnectAction)
    {
      NodeHelper.ThrottledConnectAs(servers, (LogonCredentials) null, (ConnectionSettings) null, postConnectAction);
    }

    public static void ThrottledConnectAs(
      IEnumerable<ServerBase> servers,
      LogonCredentials logonCredentials,
      ConnectionSettings connectionSettings)
    {
      NodeHelper.ThrottledConnectAs(servers, logonCredentials, connectionSettings, (Action<ServerBase>) (s => {}));
    }

    public static void ThrottledConnectAs(
      IEnumerable<ServerBase> servers,
      LogonCredentials logonCredentials,
      ConnectionSettings connectionSettings,
      Action<ServerBase> postConnectAction)
    {
      NodeHelper.ThrottledOperation(servers, (IEnumerable<RdpClient.ConnectionState>) new RdpClient.ConnectionState[2]
      {
        RdpClient.ConnectionState.Connected,
        RdpClient.ConnectionState.Disconnected
      }, (Action<ServerBase>) (server =>
      {
        server.ConnectAs(logonCredentials, connectionSettings);
        postConnectAction(server);
      }), 125);
    }

    public static void ThrottledDisconnect(IEnumerable<ServerBase> servers) => NodeHelper.ThrottledOperation(servers, (IEnumerable<RdpClient.ConnectionState>) new RdpClient.ConnectionState[1], (Action<ServerBase>) (server => server.Disconnect()), 25);

    private static void ThrottledOperation(
      IEnumerable<ServerBase> servers,
      IEnumerable<RdpClient.ConnectionState> completionStates,
      Action<ServerBase> operation,
      int delayInMilliseconds)
    {
      List<ServerBase> serversList = servers.ToList<ServerBase>();
      if (serversList.Count == 0)
        return;
      ThreadPool.QueueUserWorkItem((WaitCallback) (o =>
      {
        using (RdcMan.ThrottledOperation throttledOperation = new RdcMan.ThrottledOperation(serversList, completionStates, (Action) (() => ServerTree.Instance.SuspendSort()), (Action<ServerBase>) (server => Program.TheForm.Invoke((Delegate) (() => operation(server)))), delayInMilliseconds, (Action) (() => Program.TheForm.Invoke((Delegate) (() =>
        {
          ServerTree.Instance.ResumeSort();
          ServerTree.Instance.SortAllNodes();
        })))))
          throttledOperation.Execute();
      }));
    }
  }
}
