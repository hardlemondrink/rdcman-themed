// Decompiled with JetBrains decompiler
// Type: RdcMan.ThrottledAction
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Threading;

namespace RdcMan
{
  internal class ThrottledAction : IDisposable
  {
    private const int Max = 3;
    private int _numActive;
    private bool _disposed;
    private Semaphore _actionSemaphore;
    private List<ServerBase> _servers;
    private Action _preAction;
    private Action<ServerBase> _action;
    private int _delayInMilliseconds;
    private Action _postAction;

    public ThrottledAction(
      List<ServerBase> servers,
      Action preAction,
      Action<ServerBase> action,
      int delayInMilliseconds,
      Action postAction)
    {
      this._servers = servers;
      this._preAction = preAction;
      this._action = action;
      this._delayInMilliseconds = delayInMilliseconds;
      this._postAction = postAction;
    }

    ~ThrottledAction() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public void Execute()
    {
      this._numActive = 0;
      using (this._actionSemaphore = new Semaphore(3, 3))
      {
        try
        {
          this._preAction();
          foreach (ServerBase server in this._servers)
          {
            this._actionSemaphore.WaitOne();
            Interlocked.Increment(ref this._numActive);
            ThreadPool.QueueUserWorkItem((WaitCallback) (s => this._action(s as ServerBase)), (object) server);
            Thread.Sleep(this._delayInMilliseconds);
          }
          this.WaitForCompletion();
        }
        finally
        {
          this._postAction();
        }
      }
    }

    public void CompleteAction()
    {
      this._actionSemaphore.Release();
      Interlocked.Decrement(ref this._numActive);
    }

    private void WaitForCompletion()
    {
      while (Thread.VolatileRead(ref this._numActive) > 0)
        Thread.Sleep(this._delayInMilliseconds);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      if (disposing && this._actionSemaphore != null)
      {
        this._actionSemaphore.Close();
        this._actionSemaphore = (Semaphore) null;
      }
      this._disposed = true;
    }
  }
}
