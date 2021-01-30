// Decompiled with JetBrains decompiler
// Type: Win32.WinTrust
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Runtime.InteropServices;

namespace Win32
{
  public sealed class WinTrust
  {
    private const string WINTRUST_ACTION_GENERIC_VERIFY_V2 = "{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}";
    private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

    [DllImport("wintrust.dll", CharSet = CharSet.Unicode)]
    private static extern WinVerifyTrustResult WinVerifyTrust(
      [In] IntPtr hwnd,
      [MarshalAs(UnmanagedType.LPStruct), In] Guid pgActionID,
      [In] WinTrustData pWVTData);

    public static bool VerifyEmbeddedSignature(string fileName)
    {
      WinTrustData pWVTData = new WinTrustData(fileName);
      Guid pgActionID = new Guid("{00AAC56B-CD44-11d0-8CC2-00C04FC295EE}");
      return WinTrust.WinVerifyTrust(WinTrust.INVALID_HANDLE_VALUE, pgActionID, pWVTData) == WinVerifyTrustResult.Success;
    }

    private WinTrust()
    {
    }
  }
}
