// Decompiled with JetBrains decompiler
// Type: RdcMan.RemoteSessionInfo
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using Win32;

namespace RdcMan
{
  internal class RemoteSessionInfo
  {
    public string ClientName;
    public string DomainName;
    public int SessionId;
    public Wts.ConnectstateClass State;
    public string UserName;
  }
}
