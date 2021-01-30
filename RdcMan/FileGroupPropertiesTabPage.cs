// Decompiled with JetBrains decompiler
// Type: RdcMan.FileGroupPropertiesTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  internal class FileGroupPropertiesTabPage : GroupBasePropertiesTabPage<FileGroupSettings>
  {
    private readonly TextBox _pathnameTextBox;

    public FileGroupPropertiesTabPage(TabbedSettingsDialog dialog, FileGroupSettings settings)
      : base(dialog, settings, settings.Name)
    {
      int rowIndex = 0;
      int tabIndex = 0;
      this.AddGroupName(ref rowIndex, ref tabIndex);
      this._pathnameTextBox = (TextBox) FormTools.AddLabeledTextBox((Control) this, "Path name:", ref rowIndex, ref tabIndex);
      this.AddComment(ref rowIndex, ref tabIndex).Setting = this.Settings.Comment;
    }

    protected override void UpdateControls()
    {
      base.UpdateControls();
      this._pathnameTextBox.Enabled = false;
      this._pathnameTextBox.Text = ((this.Dialog as FileGroupPropertiesDialog).AssociatedNode as FileGroup).Pathname;
    }
  }
}
