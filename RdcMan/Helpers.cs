// Decompiled with JetBrains decompiler
// Type: RdcMan.Helpers
// Assembly: RDCMan, Version=2.7.1406.0, Culture=neutral, PublicKeyToken=null
// MVID: 984CDC7A-116B-406E-B519-0DEE541B9019
// Assembly location: C:\Program Files (x86)\Microsoft\Remote Desktop Connection Manager\RDCMan.exe

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Win32;

namespace RdcMan
{
  public static class Helpers
  {
    public static void Add(this Control.ControlCollection collection, params Control[] controls) => collection.AddRange(controls);

    public static string GetName(this XmlNode node)
    {
      XmlAttribute attribute = node.Attributes["name"];
      return attribute != null ? attribute.Value : node.Name;
    }

    public static string GetFullPath(this XmlNode node)
    {
      StringBuilder stringBuilder = new StringBuilder(node.GetName());
      XmlNode parentNode = node.ParentNode;
      while (true)
      {
        switch (parentNode)
        {
          case null:
          case XmlDocument _:
            goto label_3;
          default:
            stringBuilder.Insert(0, "/");
            stringBuilder.Insert(0, parentNode.GetName());
            parentNode = parentNode.ParentNode;
            continue;
        }
      }
label_3:
      return stringBuilder.ToString();
    }

    public static void ForEach<TObject>(this IEnumerable<TObject> objects, Action<TObject> action)
    {
      foreach (TObject @object in objects)
        action(@object);
    }

    public static void ForEach(this TreeNodeCollection objects, Action<TreeNode> action)
    {
      foreach (TreeNode treeNode in objects)
        action(treeNode);
    }

    public static TEnum ParseEnum<TEnum>(this string value) where TEnum : struct => (TEnum) Enum.Parse(typeof (TEnum), value);

    public static IEnumerable<TEnum> EnumValues<TEnum>() where TEnum : struct => Enum.GetValues(typeof (TEnum)).Cast<TEnum>();

    public static string SortOrderToString(SortOrder sortOrder)
    {
      switch (sortOrder)
      {
        case SortOrder.ByStatus:
          return "Status.Name";
        case SortOrder.ByName:
          return "Name";
        case SortOrder.None:
          return "No sorting";
        default:
          throw new Exception("Unexpected SortOrder");
      }
    }

    public static string GetTemporaryFileName(string fileName, string suffix)
    {
      string path = fileName + suffix;
      int num = 0;
      while (File.Exists(path))
        path = fileName + suffix + num++.ToString();
      return path;
    }

    public static void MoveTemporaryToPermanent(string newFileName, string fileName, bool saveOld)
    {
      string temporaryFileName = Helpers.GetTemporaryFileName(fileName, ".old");
      if (File.Exists(fileName))
        File.Move(fileName, temporaryFileName);
      File.Move(newFileName, fileName);
      if (saveOld)
        return;
      File.Delete(temporaryFileName);
    }

    public static bool IsControlKeyPressed => ((int) User.GetAsyncKeyState(17) & 32768) != 0;

    public static int NaturalCompare(string x, string y)
    {
      int i1 = 0;
      int i2 = 0;
      while (i1 < x.Length && i2 < y.Length)
      {
        char lowerInvariant1 = char.ToLowerInvariant(x[i1]);
        char lowerInvariant2 = char.ToLowerInvariant(y[i1]);
        if (char.IsDigit(lowerInvariant1) && char.IsDigit(lowerInvariant2))
        {
          uint number1 = Helpers.ParseNumber(x, ref i1);
          uint number2 = Helpers.ParseNumber(y, ref i2);
          if ((int) number1 != (int) number2)
            return number1 >= number2 ? 1 : -1;
          if (i1 != i2)
            return i2 - i1;
        }
        else
        {
          if ((int) lowerInvariant1 != (int) lowerInvariant2)
            return (int) lowerInvariant1 - (int) lowerInvariant2;
          ++i1;
          ++i2;
        }
      }
      return x.Length - y.Length;
    }

    private static uint ParseNumber(string s, ref int i)
    {
      uint num = (uint) s[i] - 48U;
      while (++i < s.Length && char.IsDigit(s[i]))
        num = (uint) ((int) num * 10 + (int) s[i] - 48);
      return num;
    }

    public static IDisposable Timer(string format, params object[] args) => (IDisposable) new Helpers.OperationTimer(format.InvariantFormat(args));

    public static string FormatTime(int seconds)
    {
      StringBuilder builder = new StringBuilder();
      int num1 = seconds / 3600;
      seconds %= 3600;
      int num2 = seconds / 60;
      seconds %= 60;
      if (num1 > 0)
        Helpers.AppendUnitValue(builder, num1, "hour");
      if (num1 > 0 || num2 > 0)
        Helpers.AppendUnitValue(builder, num2, "minute");
      Helpers.AppendUnitValue(builder, seconds, "second");
      return builder.ToString();
    }

    private static void AppendUnitValue(StringBuilder builder, int value, string unit) => builder.AppendFormat("{0} {1}{2}", (object) value, (object) unit, value != 1 ? (object) "s" : (object) string.Empty);

    public delegate void ReadXmlDelegate(
      XmlNode childNode,
      RdcTreeNode node,
      ICollection<string> errors);

    private class OperationTimer : IDisposable
    {
      private Stopwatch _stopWatch;
      private string _text;

      public OperationTimer(string text)
      {
        this._text = text;
        this._stopWatch = new Stopwatch();
        this._stopWatch.Start();
        Log.Write("Started {0}", (object) text);
        Log.AdjustIndent(1);
      }

      void IDisposable.Dispose()
      {
        this._stopWatch.Stop();
        Log.AdjustIndent(-1);
        Log.Write("Finished {0}: {1} ms", (object) this._text, (object) this._stopWatch.ElapsedMilliseconds);
      }
    }
  }
}
