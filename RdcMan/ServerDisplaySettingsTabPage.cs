﻿// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerDisplaySettingsTabPage
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  public class ServerDisplaySettingsTabPage : DisplaySettingsTabPage<ServerDisplaySettings>
  {
    public ServerDisplaySettingsTabPage(TabbedSettingsDialog dialog, ServerDisplaySettings settings)
      : base(dialog, settings)
      => this.Create(out int _, out int _);
  }
}
