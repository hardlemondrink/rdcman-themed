// Decompiled with JetBrains decompiler
// Type: Win32.WinTrustFileInfo
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Runtime.InteropServices;

namespace Win32
{
  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
  public class WinTrustFileInfo
  {
    private uint StructSize = (uint) Marshal.SizeOf(typeof (WinTrustFileInfo));
    private IntPtr pszFilePath;
    private IntPtr hFile = IntPtr.Zero;
    private IntPtr pgKnownSubject = IntPtr.Zero;

    public WinTrustFileInfo(string _filePath) => this.pszFilePath = Marshal.StringToCoTaskMemAuto(_filePath);

    ~WinTrustFileInfo() => Marshal.FreeCoTaskMem(this.pszFilePath);
  }
}
