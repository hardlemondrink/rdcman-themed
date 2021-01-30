// Decompiled with JetBrains decompiler
// Type: RdcMan.ThrottledOperation
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;

namespace RdcMan
{
  internal class ThrottledOperation : IDisposable
  {
    private HashSet<Server> _serversInScope;
    private object _serversInScopeLock = new object();
    private ThrottledAction _throttledAction;
    private HashSet<RdpClient.ConnectionState> _completionStates;
    private bool _disposed;

    public ThrottledOperation(
      List<ServerBase> servers,
      IEnumerable<RdpClient.ConnectionState> completionStates,
      Action preAction,
      Action<ServerBase> action,
      int delayInMilliseconds,
      Action postAction)
    {
      ThrottledOperation throttledOperation = this;
      this._serversInScope = new HashSet<Server>();
      this._completionStates = new HashSet<RdpClient.ConnectionState>(completionStates);
      this._throttledAction = new ThrottledAction(servers, (Action) (() =>
      {
        preAction();
        Server.ConnectionStateChanged += new Action<ConnectionStateChangedEventArgs>(throttledOperation.ConnectionStateChangeConnectHandler);
      }), (Action<ServerBase>) (server =>
      {
        lock (throttledOperation._serversInScopeLock)
          throttledOperation._serversInScope.Add(server.ServerNode);
        action(server);
      }), delayInMilliseconds, (Action) (() =>
      {
        Server.ConnectionStateChanged -= new Action<ConnectionStateChangedEventArgs>(throttledOperation.ConnectionStateChangeConnectHandler);
        postAction();
      }));
    }

    ~ThrottledOperation() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing && this._throttledAction != null)
      {
        this._throttledAction.Dispose();
        this._throttledAction = (ThrottledAction) null;
      }
      this._disposed = true;
    }

    public void Execute() => this._throttledAction.Execute();

    private void ConnectionStateChangeConnectHandler(ConnectionStateChangedEventArgs args)
    {
      if (!this._completionStates.Contains(args.State))
        return;
      bool flag;
      lock (this._serversInScopeLock)
        flag = this._serversInScope.Remove(args.Server);
      if (!flag)
        return;
      this._throttledAction.CompleteAction();
    }
  }
}
