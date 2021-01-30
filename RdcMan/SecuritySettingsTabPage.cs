// Decompiled with JetBrains decompiler
// Type: RdcMan.SecuritySettingsTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;

namespace RdcMan
{
  public class SecuritySettingsTabPage : SettingsTabPage<SecuritySettings>
  {
    public SecuritySettingsTabPage(TabbedSettingsDialog dialog, SecuritySettings settings)
      : base(dialog, settings)
    {
      int tabIndex = 0;
      int rowIndex = 0;
      this.CreateInheritanceControl(ref rowIndex, ref tabIndex);
      FormTools.AddLabeledEnumDropDown<RdpClient.AuthenticationLevel>((Control) this, "&Authentication", settings.AuthenticationLevel, ref rowIndex, ref tabIndex, new Func<RdpClient.AuthenticationLevel, string>(RdpClient.AuthenticationLevelToString));
    }
  }
}
