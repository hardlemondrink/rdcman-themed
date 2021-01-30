// Decompiled with JetBrains decompiler
// Type: RdcMan.LogonCredentialsDialogOptions
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace RdcMan
{
  [Flags]
  public enum LogonCredentialsDialogOptions
  {
    None = 0,
    AllowInheritance = 1,
    ShowProfiles = 2,
    All = 255, // 0x000000FF
  }
}
