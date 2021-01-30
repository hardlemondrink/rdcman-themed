// Decompiled with JetBrains decompiler
// Type: RdcMan.RdcRadioButton
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  public class RdcRadioButton : RadioButton, ISettingControl
  {
    public BoolSetting Setting { get; set; }

    void ISettingControl.UpdateControl()
    {
      if (this.Setting == null)
        return;
      this.Checked = this.Setting.Value;
    }

    void ISettingControl.UpdateSetting()
    {
      if (this.Setting == null)
        return;
      this.Setting.Value = this.Checked;
    }

    string ISettingControl.Validate() => (string) null;
  }
}
