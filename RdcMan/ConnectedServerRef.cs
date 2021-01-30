// Decompiled with JetBrains decompiler
// Type: RdcMan.ConnectedServerRef
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  internal class ConnectedServerRef : ServerRef
  {
    public DateTime LastFocusTime { get; set; }

    public ConnectedServerRef(Server server)
      : base(server)
    {
    }

    public override bool ConfirmRemove(bool askUser)
    {
      FormTools.InformationDialog("Disconnect the server to remove it from the Connected group");
      return false;
    }
  }
}
