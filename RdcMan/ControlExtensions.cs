// Decompiled with JetBrains decompiler
// Type: RdcMan.ControlExtensions
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace RdcMan
{
  public static class ControlExtensions
  {
    public static IEnumerable<Control> FlattenControls(
      this Control.ControlCollection controls)
    {
      foreach (Control control in (ArrangedElementCollection) controls)
      {
        yield return control;
        if (control.Controls.Count > 0)
        {
          foreach (Control flattenControl in control.Controls.FlattenControls())
            yield return flattenControl;
        }
      }
    }
  }
}
