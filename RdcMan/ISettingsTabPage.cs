// Decompiled with JetBrains decompiler
// Type: RdcMan.ISettingsTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Windows.Forms;

namespace RdcMan
{
  public interface ISettingsTabPage
  {
    InheritanceControl InheritanceControl { get; }

    Control FocusControl { get; }

    void UpdateControls();

    bool Validate();

    void UpdateSettings();
  }
}
