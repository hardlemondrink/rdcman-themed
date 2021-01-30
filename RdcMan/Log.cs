// Decompiled with JetBrains decompiler
// Type: RdcMan.Log
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using RdcMan.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RdcMan
{
  public static class Log
  {
    private static int Indent;
    private static TextWriter Writer;

    public static bool Enabled { get; private set; }

    public static void Init()
    {
      LoggingElement logging = Current.RdcManSection.Logging;
      Log.Enabled = logging.Enabled;
      if (Log.Enabled)
      {
        string str1 = Environment.ExpandEnvironmentVariables(logging.Path);
        string format = "RDCMan-{0}.log";
        foreach (FileInfo fileInfo in ((IEnumerable<string>) Directory.GetFiles(str1, format.InvariantFormat((object) "*"), SearchOption.TopDirectoryOnly)).Select<string, FileInfo>((Func<string, FileInfo>) (n => new FileInfo(n))).OrderByDescending<FileInfo, DateTime>((Func<FileInfo, DateTime>) (i => i.CreationTime)).Skip<FileInfo>(logging.MaximumNumberOfFiles - 1))
        {
          try
          {
            fileInfo.Delete();
          }
          catch
          {
          }
        }
        string str2 = DateTime.Now.ToString("yyyyMMddHHmm");
        Log.Writer = (TextWriter) new StreamWriter((Stream) File.Open(Path.Combine(str1, format.InvariantFormat((object) str2)), FileMode.Create, FileAccess.Write, FileShare.Read));
      }
      Log.Write("RDCMan v{0} build {1}", (object) Program.TheForm.VersionText, (object) Program.TheForm.BuildText);
      Log.Write(Environment.OSVersion.ToString());
      Log.Write(".NET v{0}".InvariantFormat((object) Environment.Version));
      Log.Write("mstscax.dll v{0}".InvariantFormat((object) RdpClient.RdpControlVersion));
    }

    public static void Write(string format, params object[] args)
    {
      string str = "{0} {1} {2}".InvariantFormat((object) DateTime.Now.ToString("s"), (object) new string(' ', Log.Indent * 2), (object) format.InvariantFormat(args));
      if (!Log.Enabled)
        return;
      Log.Writer.WriteLine(str);
      Log.Writer.Flush();
    }

    public static void AdjustIndent(int delta) => Log.Indent += delta;

    public static void DumpObject<T>(T o)
    {
      HashSet<object> visited = new HashSet<object>();
      Log.Write("Fields of {0}:", (object) typeof (T));
      Log.DumpObject<T>(o, visited);
    }

    private static void DumpObject<T>(T o, HashSet<object> visited)
    {
      Type type = typeof (T);
      Log.DumpObject<T>(o, type, visited);
    }

    private static void DumpObject<T>(T o, Type type, HashSet<object> visited)
    {
      Log.AdjustIndent(1);
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        if (property.GetGetMethod() != (MethodInfo) null)
        {
          if (property.Name.IndexOf("password", StringComparison.OrdinalIgnoreCase) == -1)
          {
            try
            {
              object obj = property.GetValue((object) o, (object[]) null);
              Log.DumpValue(property, obj, visited);
            }
            catch (Exception ex)
            {
              Log.Write("{0} exception when processing: {1}", (object) property.Name, (object) ex.Message);
            }
          }
        }
      }
      Log.AdjustIndent(-1);
    }

    private static void DumpValue(PropertyInfo prop, object value, HashSet<object> visited)
    {
      Type propertyType = prop.PropertyType;
      if (value == null || propertyType.IsPrimitive || propertyType.IsEnum)
        Log.Write("{0} {1} = {2}", (object) propertyType.Name, (object) prop.Name, value ?? (object) "{null}");
      else if (propertyType.FullName.Equals("System.String"))
        Log.Write("{0} {1} = '{2}'", (object) propertyType.Name, (object) prop.Name, value);
      else if (propertyType.IsArray)
        Log.Write("{0} {1}", (object) propertyType.Name, (object) prop.Name);
      else if (visited.Add(value))
      {
        Log.Write("{0} {1}", (object) propertyType.Name, (object) prop.Name);
        Log.DumpObject<object>(value, propertyType, visited);
      }
      else
        Log.Write("{0} is a recursive reference", (object) prop.Name);
    }
  }
}
