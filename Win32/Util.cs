// Decompiled with JetBrains decompiler
// Type: Win32.Util
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Runtime.InteropServices;

namespace Win32
{
  public class Util
  {
    public const uint MvkVkeyToScanCode = 0;

    [DllImport("user32.dll")]
    public static extern uint MapVirtualKey(uint uCode, uint uMapType);
  }
}
