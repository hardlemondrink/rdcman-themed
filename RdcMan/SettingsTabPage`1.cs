// Decompiled with JetBrains decompiler
// Type: RdcMan.SettingsTabPage`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Drawing;
using System.Windows.Forms;

namespace RdcMan
{
  public abstract class SettingsTabPage<TSettingsGroup> : SettingsTabPage, ISettingsTabPage
    where TSettingsGroup : SettingsGroup
  {
    protected SettingsTabPage(TabbedSettingsDialog dialog, TSettingsGroup settingsGroup)
      : this(dialog, settingsGroup, settingsGroup.Name)
    {
    }

    protected SettingsTabPage(
      TabbedSettingsDialog dialog,
      TSettingsGroup settingsGroup,
      string name)
    {
      this.Dialog = dialog;
      this.Settings = settingsGroup;
      this.Location = FormTools.TopLeftLocation();
      this.Size = new Size(512, 334);
      this.Text = name;
    }

    protected InheritanceControl InheritanceControl { get; private set; }

    protected TSettingsGroup Settings { get; private set; }

    protected TabbedSettingsDialog Dialog { get; private set; }

    protected void CreateInheritanceControl(ref int rowIndex, ref int tabIndex)
    {
      if (this.Settings.InheritSettingsType.Mode == InheritanceMode.Disabled)
        return;
      this.InheritanceControl = new InheritanceControl(this.Dialog, this.Settings.Name);
      this.InheritanceControl.Create((Control) this, ref rowIndex, ref tabIndex);
    }

    InheritanceControl ISettingsTabPage.InheritanceControl => this.InheritanceControl;

    void ISettingsTabPage.UpdateControls() => this.UpdateControls();

    void ISettingsTabPage.UpdateSettings() => this.UpdateSettings();

    protected override void UpdateControls()
    {
      if (this.InheritanceControl != null)
      {
        this.InheritanceControl.UpdateControlsFromSettings(this.Settings.InheritSettingsType);
      }
      else
      {
        foreach (Control flattenControl in this.Controls.FlattenControls())
          flattenControl.Enabled = true;
      }
      base.UpdateControls();
    }

    protected override void UpdateSettings()
    {
      if (this.InheritanceControl != null)
        this.Settings.InheritSettingsType.Mode = this.InheritanceControl.GetInheritanceMode();
      base.UpdateSettings();
    }
  }
}
