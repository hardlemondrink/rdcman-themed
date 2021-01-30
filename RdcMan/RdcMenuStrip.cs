// Decompiled with JetBrains decompiler
// Type: RdcMan.RdcMenuStrip
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Windows.Forms;
using Win32;

namespace RdcMan
{
  public class RdcMenuStrip : MenuStrip
  {
    public bool IsActive { get; private set; }

    public RdcMenuStrip()
    {
      this.MenuActivate += (EventHandler) ((s, e) => this.IsActive = true);
      this.MenuDeactivate += (EventHandler) ((s, e) =>
      {
        this.IsActive = false;
        if (!Program.Preferences.HideMainMenu)
          return;
        bool flag1 = ((int) User.GetAsyncKeyState(164) & 32768) != 0;
        bool flag2 = ((int) User.GetAsyncKeyState(165) & 32768) != 0;
        if (flag1 || flag2)
          return;
        ((RdcBaseForm) this.FindForm()).SetMainMenuVisibility(false);
      });
    }
  }
}
