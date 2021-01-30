// Decompiled with JetBrains decompiler
// Type: RdcMan.GroupDisplaySettingsTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public class GroupDisplaySettingsTabPage : DisplaySettingsTabPage<GroupDisplaySettings>
  {
    private readonly RdcCheckBox _previewCheckBox;
    private readonly RdcCheckBox _interactionCheckBox;

    public GroupDisplaySettingsTabPage(TabbedSettingsDialog dialog, GroupDisplaySettings settings)
      : base(dialog, settings)
    {
      int rowIndex;
      int tabIndex;
      this.Create(out rowIndex, out tabIndex);
      this._previewCheckBox = FormTools.AddCheckBox((Control) this, "&Preview session in thumbnail", settings.SessionThumbnailPreview, 0, ref rowIndex, ref tabIndex);
      this._interactionCheckBox = FormTools.AddCheckBox((Control) this, "&Allow thumbnail session interaction", settings.AllowThumbnailSessionInteraction, 0, ref rowIndex, ref tabIndex);
      this._interactionCheckBox.Location = new Point(this._previewCheckBox.Left + 24, this._interactionCheckBox.Top);
      this._previewCheckBox.CheckedChanged += (EventHandler) ((s, e) => this.PreviewCheckBoxChanged());
      if (this.InheritanceControl != null)
        this.InheritanceControl.EnabledChanged += (Action<bool>) (enabled => this.PreviewCheckBoxChanged());
      FormTools.AddCheckBox((Control) this, "Show &disconnected thumbnails", settings.ShowDisconnectedThumbnails, 0, ref rowIndex, ref tabIndex);
    }

    private void PreviewCheckBoxChanged() => this._interactionCheckBox.Enabled = this._previewCheckBox.Checked && this._previewCheckBox.Enabled;

    protected override void UpdateControls()
    {
      base.UpdateControls();
      this.PreviewCheckBoxChanged();
    }
  }
}
