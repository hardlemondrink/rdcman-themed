// Decompiled with JetBrains decompiler
// Type: RdcMan.ConnectionStateChangedEventArgs
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  public class ConnectionStateChangedEventArgs : EventArgs
  {
    public ConnectionStateChangedEventArgs(Server server, RdpClient.ConnectionState state)
    {
      this.Server = server;
      this.State = state;
    }

    public Server Server { get; private set; }

    public RdpClient.ConnectionState State { get; private set; }
  }
}
