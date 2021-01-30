// Decompiled with JetBrains decompiler
// Type: Win32.WinTrustDataChoice
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

namespace Win32
{
  public enum WinTrustDataChoice : uint
  {
    File = 1,
    Catalog = 2,
    Blob = 3,
    Signer = 4,
    Certificate = 5,
  }
}
