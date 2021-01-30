// Decompiled with JetBrains decompiler
// Type: RdcMan.NodePropertiesPage`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RdcMan
{
  public abstract class NodePropertiesPage<TSettingsGroup> : 
    SettingsTabPage<TSettingsGroup>,
    INodePropertiesPage
    where TSettingsGroup : SettingsGroup
  {
    protected ValueComboBox<GroupBase> _parentComboBox;

    protected NodePropertiesPage(TabbedSettingsDialog dialog, TSettingsGroup settings, string name)
      : base(dialog, settings, name)
    {
    }

    public event Action<GroupBase> ParentGroupChanged;

    public GroupBase ParentGroup => this._parentComboBox == null || !this._parentComboBox.Enabled ? (GroupBase) null : this._parentComboBox.SelectedValue;

    public bool PopulateParentDropDown(GroupBase excludeGroup, GroupBase defaultParent)
    {
      this.PopulateParentDropDown(excludeGroup);
      if (defaultParent != null && this.CanBeParent(defaultParent))
      {
        this._parentComboBox.SelectedValue = defaultParent;
      }
      else
      {
        if (this._parentComboBox.ItemCount == 0)
          return false;
        this._parentComboBox.SelectedIndex = 0;
      }
      return true;
    }

    public void SetParentDropDown(GroupBase group)
    {
      this._parentComboBox.AddItem(group.FullPath, group);
      this._parentComboBox.SelectedIndex = 0;
    }

    protected abstract bool CanBeParent(GroupBase group);

    private void PopulateParentDropDown(GroupBase excludeGroup) => ServerTree.Instance.Nodes.VisitNodes((Func<RdcTreeNode, NodeVisitorResult>) (node =>
    {
      if (node == excludeGroup)
        return NodeVisitorResult.NoRecurse;
      if (node is GroupBase group && this.CanBeParent(group))
        this._parentComboBox.AddItem(group.FullPath, group);
      return NodeVisitorResult.Continue;
    }));

    protected void AddParentCombo(ref int rowIndex, ref int tabIndex)
    {
      this._parentComboBox = FormTools.AddLabeledValueDropDown<GroupBase>((Control) this, "&Parent:", ref rowIndex, ref tabIndex, (Func<GroupBase, string>) null, (IEnumerable<GroupBase>) null);
      this._parentComboBox.SelectedIndexChanged += new EventHandler(this.ParentGroupChangedHandler);
    }

    protected RdcTextBox AddComment(ref int rowIndex, ref int tabIndex)
    {
      Label label = FormTools.NewLabel("&Comment:", 0, rowIndex);
      RdcTextBox rdcTextBox = FormTools.NewTextBox(1, rowIndex++, tabIndex++, 7);
      rdcTextBox.Enabled = true;
      this.Controls.Add((Control) label, (Control) rdcTextBox);
      return rdcTextBox;
    }

    protected virtual void ParentGroupChangedHandler(object sender, EventArgs e) => this.ParentGroupChanged(this._parentComboBox.SelectedValue);
  }
}
