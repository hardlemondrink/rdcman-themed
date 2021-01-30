// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerTreeVisibilityMenuItem
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  internal class ServerTreeVisibilityMenuItem : EnumMenuItem<ControlVisibility>
  {
    public ServerTreeVisibilityMenuItem(string text, ControlVisibility value)
      : base(text, value)
    {
    }

    protected override ControlVisibility Value
    {
      get => Program.TheForm.ServerTreeVisibility;
      set => Program.TheForm.ServerTreeVisibility = value;
    }
  }
}
