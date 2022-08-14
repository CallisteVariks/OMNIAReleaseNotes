using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Helpers
{
  public class ParseHelper
  {
    public static string[] SeparateNameAndVersionsAlphaNumeric(string line)
    {
      if (string.IsNullOrEmpty(line)) return null;

      string pattern = @"([A-Z]{1,2}\d{1,2}\s*$)";
      return Regex.Split(line, pattern, RegexOptions.IgnoreCase);
    }

    public static string[] SeparateNameAndVersionsNumeric(string line)
    {
      if (string.IsNullOrEmpty(line)) return null;

      string pattern = @"([^()]\d{1,}\.\d{1,}\.\d{1,}\.+\d+)";
      return Regex.Split(line, pattern, RegexOptions.IgnoreCase);
    }

    public static bool MatchPattern(string _input, string pattern)
    {
      if (string.IsNullOrEmpty(_input)) return false;

      Regex patchRegex = new Regex($"{pattern}");
      Match match = patchRegex.Match(_input);
      return match.Success;
    }


    public static string[] TrimArrayAndRemoveUnusedText(string[] arr)
    {
      if (arr == null || arr.Length == 0) return null;

      arr = arr.Where(entry => !string.IsNullOrEmpty(entry)).ToArray();
      int decoupleIndex = arr.Length - 1;
      foreach (string item in arr)
      {
        //removes the ___________________________________ if it exists
        if (MatchPattern(item, "^_+$"))
        {
          int indexToRemove = Array.IndexOf(arr, item);
          arr = RemoveAt(arr, indexToRemove);
        }

        //finds the decoupling point from the tools their versions and names and the unused information
        string decoupleWord = "usage";
        if (!item.ToLower().Contains(decoupleWord)) continue;

        decoupleIndex = Array.IndexOf(arr, item);
        break;

      }
      List<string> myList = arr.ToList();
      myList.RemoveRange(decoupleIndex, myList.Count - decoupleIndex);
      arr = myList.ToArray();
      return arr;

    }

    public static string[] RemoveAt(string[] arr, int indexToRemove)
    {
      if (arr == null)
        return null;

      if (0 > indexToRemove || indexToRemove >= arr.Length)
        MessageBox.Show($"Index {indexToRemove} is outside of the boudaries of the source array");

      string[] destination = new string[arr.Length - 1];
      Array.Copy(arr, 0, destination, 0, indexToRemove);
      Array.Copy(arr, indexToRemove + 1, destination, indexToRemove, arr.Length - indexToRemove - 1);

      return destination;
    }


    public static bool CheckMatchPattern(string nameOrVersion)
    {
      return MatchPattern(nameOrVersion, "^[0-9]*\\.\\s+[a-z,A-Z]+\\W{0,1}") &&
             MatchPattern(nameOrVersion, "\\d+\\.\\d+\\.\\d+\\.*\\d*") ||
             MatchPattern(nameOrVersion, "^[a-z,A-Z]{1,2}\\d{1,2}\\s*$");
    }
  }
}
