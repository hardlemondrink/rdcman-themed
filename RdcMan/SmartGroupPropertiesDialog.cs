// Decompiled with JetBrains decompiler
// Type: RdcMan.SmartGroupPropertiesDialog
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  internal class SmartGroupPropertiesDialog : NodePropertiesDialog
  {
    protected SmartGroupPropertiesDialog(
      SmartGroup group,
      string dialogTitle,
      string acceptButtonText,
      Form parentForm)
      : base((RdcTreeNode) group, dialogTitle, acceptButtonText, parentForm)
    {
    }

    public override void CreateControls(RdcTreeNode settings)
    {
      TabPage tabPage = settings.Properties.CreateTabPage((TabbedSettingsDialog) this);
      this.PropertiesPage = tabPage as INodePropertiesPage;
      this.AddTabPage(tabPage);
      this.InitButtons();
      this.ScaleAndLayout();
    }

    public static SmartGroupPropertiesDialog NewAddDialog(GroupBase parent)
    {
      SmartGroup forAdd = SmartGroup.CreateForAdd();
      SmartGroupPropertiesDialog propertiesDialog = new SmartGroupPropertiesDialog(forAdd, "Add Smart Group", "Add", (Form) null);
      if (parent != null && !parent.CanAddGroups())
        parent = (GroupBase) null;
      propertiesDialog.CreateControls((RdcTreeNode) forAdd);
      if (propertiesDialog.PropertiesPage.PopulateParentDropDown((GroupBase) null, parent))
        return propertiesDialog;
      propertiesDialog.Dispose();
      return (SmartGroupPropertiesDialog) null;
    }

    public static SmartGroupPropertiesDialog NewPropertiesDialog(
      SmartGroup group,
      Form parentForm)
    {
      SmartGroupPropertiesDialog propertiesDialog = new SmartGroupPropertiesDialog(group, group.Text + " Smart Group Properties", "OK", parentForm);
      propertiesDialog.CreateControls((RdcTreeNode) group);
      propertiesDialog.PropertiesPage.PopulateParentDropDown((GroupBase) group, group.Parent as GroupBase);
      return propertiesDialog;
    }
  }
}
