// Decompiled with JetBrains decompiler
// Type: RdcMan.OperationBehavior
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  [Flags]
  internal enum OperationBehavior
  {
    None = 0,
    SuspendSelect = 1,
    SuspendSort = 2,
    SuspendUpdate = 4,
    SuspendGroupChanged = 8,
    RestoreSelected = 17, // 0x00000011
  }
}
