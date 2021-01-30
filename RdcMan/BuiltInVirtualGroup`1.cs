// Decompiled with JetBrains decompiler
// Type: RdcMan.BuiltInVirtualGroup`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace RdcMan
{
  internal abstract class BuiltInVirtualGroup<TServerRef> : VirtualGroup, IBuiltInVirtualGroup
    where TServerRef : ServerRef
  {
    protected new static Dictionary<string, Helpers.ReadXmlDelegate> NodeActions = new Dictionary<string, Helpers.ReadXmlDelegate>((IDictionary<string, Helpers.ReadXmlDelegate>) GroupBase.NodeActions);

    string IBuiltInVirtualGroup.Text => this.Text;

    string IBuiltInVirtualGroup.ConfigPropertyName => this.ConfigName;

    string IBuiltInVirtualGroup.XmlNodeName => this.XmlNodeName;

    bool IBuiltInVirtualGroup.IsInTree
    {
      get => this.TreeView != null;
      set => this.UpdateVisibleInTree(value);
    }

    bool IBuiltInVirtualGroup.IsVisibilityConfigurable => this.IsVisibilityConfigurable;

    void IBuiltInVirtualGroup.ReadXml(
      XmlNode xmlNode,
      FileGroup fileGroup,
      ICollection<string> errors)
    {
      this.ReadXml(xmlNode, fileGroup, errors);
    }

    void IBuiltInVirtualGroup.WriteXml(XmlTextWriter tw, FileGroup fileGroup) => this.WriteXml(tw, fileGroup);

    bool IBuiltInVirtualGroup.ShouldWriteNode(
      ServerRef serverRef,
      FileGroup file)
    {
      return this.ShouldWriteNode((RdcTreeNode) serverRef, file);
    }

    static BuiltInVirtualGroup() => BuiltInVirtualGroup<TServerRef>.NodeActions["server"] = (Helpers.ReadXmlDelegate) ((childNode, parent, errors) =>
    {
      TreeNode nodeByName = ServerTree.Instance.FindNodeByName(childNode.InnerText);
      if (nodeByName == null || !(nodeByName is Server server))
        return;
      (parent as BuiltInVirtualGroup<TServerRef>).AddReference((ServerBase) server);
    });

    public virtual string ConfigName => this.Text;

    public bool IsInTree
    {
      get => this.TreeView != null;
      set => this.UpdateVisibleInTree(value);
    }

    public override void OnRemoving() => this.Hide();

    public override sealed bool ConfirmRemove(bool askUser)
    {
      FormTools.InformationDialog("Use the View menu to hide the " + this.Text + " group");
      return false;
    }

    public override sealed bool CanDropOnTarget(RdcTreeNode targetNode) => false;

    public override bool CanDropServers() => false;

    public override sealed bool CanRemove(bool popUI) => false;

    public override bool HasProperties => false;

    public override void DoPropertiesDialog(Form parentForm, string activeTabName) => throw new NotImplementedException();

    internal override void ReadXml(XmlNode xmlNode, ICollection<string> errors) => throw new NotImplementedException();

    internal override void WriteXml(XmlTextWriter tw) => throw new NotImplementedException();

    protected virtual void ReadXml(
      XmlNode xmlNode,
      FileGroup fileGroup,
      ICollection<string> errors)
    {
      if (string.IsNullOrEmpty(this.XmlNodeName))
        return;
      this.ReadXml(BuiltInVirtualGroup<TServerRef>.NodeActions, xmlNode, errors);
      if (!this.Properties.Expanded.Value)
        return;
      this.Expand();
    }

    protected virtual void WriteXml(XmlTextWriter tw, FileGroup file)
    {
      if (string.IsNullOrEmpty(this.XmlNodeName))
        return;
      tw.WriteStartElement(this.XmlNodeName);
      if (file == null)
        this.WriteXmlSettingsGroups(tw);
      foreach (TreeNode node in this.Nodes)
      {
        TServerRef serverRef = node as TServerRef;
        if (this.ShouldWriteNode((RdcTreeNode) serverRef, file))
          tw.WriteElementString("server", serverRef.ServerNode.FullPath);
      }
      tw.WriteEndElement();
    }

    protected virtual bool ShouldWriteNode(RdcTreeNode node, FileGroup file) => node.FileGroup == file;

    public virtual TServerRef AddReference(ServerBase serverBase)
    {
      if (serverBase == null)
        return default (TServerRef);
      Server serverNode = serverBase.ServerNode;
      TServerRef serverRef = serverNode.FindServerRef<TServerRef>();
      if ((object) serverRef == null)
      {
        serverRef = this.ServerRefFactory.Create(serverNode) as TServerRef;
        ServerTree.Instance.AddNode((RdcTreeNode) serverRef, (GroupBase) this);
      }
      return serverRef;
    }

    protected virtual string XmlNodeName => (string) null;

    protected virtual bool IsVisibilityConfigurable => true;

    protected void UpdateVisibleInTree(bool isVisible)
    {
      if (isVisible)
      {
        if (this.TreeView == null)
        {
          ServerTree.Instance.AddNode((RdcTreeNode) this, ServerTree.Instance.RootNode);
          this.Expand();
          ServerTree.Instance.Operation(OperationBehavior.RestoreSelected, (Action) (() => ServerTree.Instance.SortBuiltinGroups()));
        }
      }
      else if (this.TreeView != null)
        ServerTree.Instance.RemoveNode((RdcTreeNode) this);
      if (!this.IsVisibilityConfigurable)
        return;
      Program.Preferences.SetBuiltInGroupVisibility((IBuiltInVirtualGroup) this, isVisible);
    }
  }
}
