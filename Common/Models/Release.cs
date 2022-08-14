using Caliburn.Micro;
using OMNITools.CommonObjects.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Common.Models
{
  public class Release : PropertyChangedBase, ISearchable
  {
    internal const string Delimiter_OMNIAVersion = "[Version]";
    internal const string Delimiter_Patch = "[Patch]";
    internal const string Delimiter_ReleaseDate = "[Release Date]";
    internal const string Delimiter_Comment = "[Comments]";
    internal const string Delimiter_GeneralRestriction = "[Restrictions]";
    public string[] Delimiters = { Delimiter_Comment, Delimiter_GeneralRestriction, Delimiter_OMNIAVersion, Delimiter_Patch, Delimiter_ReleaseDate };
    public double? Version { get; set; }
    public int? Patch { get; set; }
    public string ReleaseDate { get; set; } = DateTime.Now.ToString(OMNITools.CommonObjects.Constant.DateTimeConstant.DateFormat);
    public string Comment { get; set; }
    public string RestrictionComment { get; set; }
    public List<Tool> ToolList { get; set; } = new List<Tool>();
    public List<Tool> AllToolsList { get; set; } = new List<Tool>();
    private bool _isSelectedForCompare = false;
    public bool IsSelectedForCompare
    {
      get => _isSelectedForCompare;
      set
      {
        if (value.Equals(_isSelectedForCompare)) return;
        _isSelectedForCompare = value;
        NotifyOfPropertyChange(() => IsSelectedForCompare);
      }
    }

    internal Release Clone()
    {
      if (ToolList == null || AllToolsList == null)
        return null;

      return new Release
      {
        Version = Version,
        Patch = Patch,
        ReleaseDate = ReleaseDate,
        Comment = Comment,
        RestrictionComment = RestrictionComment,
        ToolList = ToolList.Select(t => t.Clone()).ToList(),
        AllToolsList = AllToolsList.Select(t => t.Clone()).ToList()
      };
    }

    public static Release Deserialize(string serializedReleaseText)
    {
      Release toReturn = new Release();

      if (string.IsNullOrEmpty(serializedReleaseText))
        return toReturn;

      string[] componentFromReleaseSeparator = { "[Component]" };

      string[] splittedText = serializedReleaseText.Split(componentFromReleaseSeparator, StringSplitOptions.None);

      if (string.IsNullOrEmpty(splittedText[0]))
        return toReturn;

      List<string> splitted = splittedText[0].Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

      foreach (string line in splitted)
      {
        string[] property = line.Split(':');
        if (property.Length != 2) continue;

        string key = property[0].Trim();
        string value = property[1].Trim();

        switch (key)
        {
          case "[Version]": toReturn.Version = double.Parse(value, CultureInfo.InvariantCulture); break;
          case "[Patch]": toReturn.Patch = Convert.ToInt32(value); break;
          case "[Release Date]": toReturn.ReleaseDate = value; break;
          case "[Comments]": toReturn.Comment = value; break;
          case "[Restrictions]": toReturn.RestrictionComment = value; break;

          default: MessageBox.Show($"Unknown property met. Property was {key}. Version: {toReturn.Version} Patch: {toReturn.Patch}"); break;
        }
      }

      toReturn.ToolList = Tool.DeserializeToolList(serializedReleaseText);
      return toReturn;
    }

    public string PrintRelease()
    {
      return string.Join(Environment.NewLine,
                         $"{Delimiter_OMNIAVersion} : {Version.ToString().Replace(',', '.')}",
                         $"{Delimiter_Patch} : {Patch}",
                         $"{Delimiter_ReleaseDate} : {ReleaseDate}",
                         $"{Delimiter_GeneralRestriction} : {RestrictionComment}",
                         $"{Delimiter_Comment} : {Comment}",
                         $"{PrintTools()}");
    }

    private string PrintTools()
    {
      string output = "";
      if (ToolList == null)
        return output;

      foreach (Tool tool in ToolList)
        output += tool.ToString();

      return output;
    }

    public string ToSearchString()
    {
      string VersionString = (Version % 1 == 0) ? $"{Version}.0" : $"{Version.ToString().Replace(',', '.')}";

      List<object> releases = new List<object>
      {
        VersionString,
        Patch,
        ReleaseDate,
        RestrictionComment,
        Comment,
        PrintTools()
      };
      return string.Join(", ", releases);
    }
  }
}