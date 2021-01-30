// Decompiled with JetBrains decompiler
// Type: RdcMan.Setting`1
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace RdcMan
{
  public abstract class Setting<T> : BaseSetting<T>
  {
    protected Setting(object o)
      : base(o)
    {
    }

    public T Value
    {
      get => this.Value;
      set => this.Value = value;
    }
  }
}
