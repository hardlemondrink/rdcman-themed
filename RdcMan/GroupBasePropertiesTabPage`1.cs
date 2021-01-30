// Decompiled with JetBrains decompiler
// Type: RdcMan.GroupBasePropertiesTabPage`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  internal class GroupBasePropertiesTabPage<TSettingsGroup> : NodePropertiesPage<TSettingsGroup>
    where TSettingsGroup : GroupSettings
  {
    private RdcTextBox _groupNameTextBox;

    protected GroupBasePropertiesTabPage(
      TabbedSettingsDialog dialog,
      TSettingsGroup settings,
      string name)
      : base(dialog, settings, name)
    {
    }

    protected void AddGroupName(ref int rowIndex, ref int tabIndex)
    {
      this._groupNameTextBox = FormTools.AddLabeledTextBox((Control) this, "&Group name:", this.Settings.GroupName, ref rowIndex, ref tabIndex);
      this._groupNameTextBox.Enabled = true;
      this._groupNameTextBox.Validate = (Func<string>) (() =>
      {
        this._groupNameTextBox.Text = this._groupNameTextBox.Text.Trim();
        string text = this._groupNameTextBox.Text;
        if (text.Length == 0)
          return "Please enter a group name";
        string pathSeparator = ServerTree.Instance.PathSeparator;
        return text.IndexOf(pathSeparator) != -1 ? "Group name may not contain the path separator \"" + pathSeparator + "\"" : (string) null;
      });
      this.FocusControl = (Control) this._groupNameTextBox;
    }

    protected override bool CanBeParent(GroupBase group) => group.CanAddGroups();
  }
}
