// Decompiled with JetBrains decompiler
// Type: RdcMan.ServerChangedEventArgs
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  public class ServerChangedEventArgs : EventArgs
  {
    public ServerChangedEventArgs(ServerBase server, ChangeType changeType)
    {
      this.Server = server;
      this.ChangeType = changeType;
    }

    public ServerBase Server { get; private set; }

    public ChangeType ChangeType { get; private set; }
  }
}
