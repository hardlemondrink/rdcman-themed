// Decompiled with JetBrains decompiler
// Type: Win32.Kernel
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System.Runtime.InteropServices;

namespace Win32
{
  public class Kernel
  {
    public static uint MajorVersion { get; private set; }

    public static uint MinorVersion { get; private set; }

    public static uint Build { get; private set; }

    static Kernel()
    {
      uint version = Kernel.GetVersion();
      Kernel.MajorVersion = version & (uint) byte.MaxValue;
      Kernel.MinorVersion = (version & 65280U) >> 8;
      Kernel.Build = (version & 4294901760U) >> 16;
    }

    [DllImport("kernel32.dll")]
    public static extern int GetLastError();

    [DllImport("kernel32")]
    public static extern uint GetVersion();
  }
}
