// Decompiled with JetBrains decompiler
// Type: RdcMan.RdcTextBox
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  public class RdcTextBox : TextBox, ISettingControl
  {
    public StringSetting Setting { get; set; }

    public Func<string> Validate { private get; set; }

    void ISettingControl.UpdateControl()
    {
      if (this.Setting == null)
        return;
      this.Text = this.Setting.Value;
    }

    void ISettingControl.UpdateSetting()
    {
      if (this.Setting == null)
        return;
      this.Setting.Value = this.Text;
    }

    string ISettingControl.Validate() => this.Validate != null ? this.Validate() : (string) null;
  }
}
