// Decompiled with JetBrains decompiler
// Type: Win32.WinTrustDataProvFlags
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;

namespace Win32
{
  [Flags]
  public enum WinTrustDataProvFlags : uint
  {
    UseIe4TrustFlag = 1,
    NoIe4ChainFlag = 2,
    NoPolicyUsageFlag = 4,
    RevocationCheckNone = 16, // 0x00000010
    RevocationCheckEndCert = 32, // 0x00000020
    RevocationCheckChain = 64, // 0x00000040
    RevocationCheckChainExcludeRoot = 128, // 0x00000080
    SaferFlag = 256, // 0x00000100
    HashOnlyFlag = 512, // 0x00000200
    UseDefaultOsverCheck = 1024, // 0x00000400
    LifetimeSigningFlag = 2048, // 0x00000800
    CacheOnlyUrlRetrieval = 4096, // 0x00001000
    DisableMD2andMD4 = 8192, // 0x00002000
  }
}
