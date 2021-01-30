// Decompiled with JetBrains decompiler
// Type: RdcMan.SettingsTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Linq;
using System.Windows.Forms;

namespace RdcMan
{
  public class SettingsTabPage : TabPage, ISettingsTabPage
  {
    private const string InvalidSuffix = " (!)";

    InheritanceControl ISettingsTabPage.InheritanceControl => throw new NotImplementedException();

    Control ISettingsTabPage.FocusControl => this.FocusControl;

    void ISettingsTabPage.UpdateControls() => this.UpdateControls();

    bool ISettingsTabPage.Validate()
    {
      string str = this.Text.Replace(" (!)", string.Empty);
      if (this.IsValid())
      {
        this.Text = str;
        return true;
      }
      this.Text = str + " (!)";
      return false;
    }

    void ISettingsTabPage.UpdateSettings() => this.UpdateSettings();

    protected Control FocusControl { get; set; }

    protected virtual void UpdateControls()
    {
      foreach (ISettingControl settingControl in this.Controls.FlattenControls().OfType<ISettingControl>())
        settingControl.UpdateControl();
    }

    protected virtual bool IsValid() => (this.FindForm() as RdcDialog).ValidateControls(this.Controls.FlattenControls(), true);

    protected virtual void UpdateSettings()
    {
      foreach (ISettingControl settingControl in this.Controls.FlattenControls().OfType<ISettingControl>())
        settingControl.UpdateSetting();
    }
  }
}
