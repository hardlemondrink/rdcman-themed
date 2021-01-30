// Decompiled with JetBrains decompiler
// Type: RdcMan.RemoteSessions
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Win32;

namespace RdcMan
{
  internal class RemoteSessions
  {
    private IntPtr _hServer;
    private readonly ServerBase _server;

    public RemoteSessions(ServerBase server)
    {
      this._server = server;
      this._hServer = (IntPtr) 0;
    }

    public bool OpenServer()
    {
      this._hServer = Wts.OpenServer(this._server.ServerName);
      return !(this._hServer == (IntPtr) 0);
    }

    public void CloseServer()
    {
      if (!(this._hServer != (IntPtr) 0))
        return;
      Wts.CloseServer(this._hServer);
      this._hServer = (IntPtr) 0;
    }

    public IList<RemoteSessionInfo> QuerySessions()
    {
      if (this._hServer == (IntPtr) 0)
        throw new Exception("QuerySessions called before OpenServer succeeded");
      IntPtr pSessionInfo;
      int count;
      if (!Wts.EnumerateSessions(this._hServer, 0, 1, out pSessionInfo, out count))
        return (IList<RemoteSessionInfo>) null;
      List<RemoteSessionInfo> remoteSessionInfoList = new List<RemoteSessionInfo>();
      Wts.SessionInfo sessionInfo = new Wts.SessionInfo();
      try
      {
        IntPtr ptr = pSessionInfo;
        for (int index = 0; index < count; ++index)
        {
          Marshal.PtrToStructure(ptr, (object) sessionInfo);
          ptr = (IntPtr) ((long) ptr + (long) Marshal.SizeOf((object) sessionInfo));
          IntPtr pBuffer;
          int bytesReturned;
          Wts.QuerySessionInformation(this._hServer, sessionInfo.SessionId, Wts.InfoClass.UserName, out pBuffer, out bytesReturned);
          string stringAuto1 = Marshal.PtrToStringAuto(pBuffer);
          if (stringAuto1.Length != 0)
          {
            Wts.QuerySessionInformation(this._hServer, sessionInfo.SessionId, Wts.InfoClass.DomainName, out pBuffer, out bytesReturned);
            string stringAuto2 = Marshal.PtrToStringAuto(pBuffer);
            Wts.QuerySessionInformation(this._hServer, sessionInfo.SessionId, Wts.InfoClass.ClientName, out pBuffer, out bytesReturned);
            string stringAuto3 = Marshal.PtrToStringAuto(pBuffer);
            remoteSessionInfoList.Add(new RemoteSessionInfo()
            {
              ClientName = stringAuto3,
              DomainName = stringAuto2,
              SessionId = sessionInfo.SessionId,
              UserName = stringAuto1,
              State = sessionInfo.State
            });
          }
        }
      }
      finally
      {
        Wts.FreeMemory(pSessionInfo);
      }
      return (IList<RemoteSessionInfo>) remoteSessionInfoList;
    }

    public bool DisconnectSession(int id) => Wts.DisconnectSession(this._hServer, id, true);

    public bool LogOffSession(int id) => Wts.LogOffSession(this._hServer, id, true);
  }
}
