using Caliburn.Micro;
using OMNIToolNetworkAnalyzer.Common;
using OMNIToolNetworkAnalyzer.Core.Presentation;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Common.Enums;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Common.Models;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Helpers;
using OMNITools.CommonObjects.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Presentation.Micro
{
  public class MicroViewModel : PropertyChangedBase
  {
    #region fields and properties
    private string _inputString = string.Empty;
    private List<Tool> _toolList = new List<Tool>();
    private string _outputString = string.Empty;
    private string _fileText;
    private string[] _splitOutputText => OutputString.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
    public string EnumList => string.Join(Environment.NewLine, Enum.GetValues(typeof(ToolType)).Cast<ToolType>().Where(t => t != ToolType.Empty).Where(type => type != ToolType.UNDEFINED).ToList());
    public bool CanClearOutputField => (InputString != string.Empty || OutputString != string.Empty);
    public bool CanAddEmptyRelease => (OutputString == string.Empty);
    public bool CanAddEmptyTool => (OutputString != string.Empty);
    public bool CanSaveFile => Validate();
    public double ReleaseVersion { get; set; }
    public int ReleasePatch { get; set; }
    public string RestrictionText { get; set; }
    public ObservableCollection<string> RawOutputString = new ObservableCollection<string>();

    private string _errorSavingFile;
    public string ErrorSavingFile
    {
      get => _errorSavingFile; 
      set { _errorSavingFile = value; NotifyOfPropertyChange(() => ErrorSavingFile); }
    }

    public string OutputString
    {
      get => _outputString;
      set
      {
        _outputString = value;

        NotifyOfPropertyChange();
        NotifyOfPropertyChange(() => CanClearOutputField);
        NotifyOfPropertyChange(() => CanAddEmptyRelease);
        NotifyOfPropertyChange(() => CanAddEmptyTool);
        NotifyOfPropertyChange(() => CanSaveFile);
        Validate();
      }
    }

    public string InputString
    {
      get => _inputString;
      set
      {
        _inputString = value;
        if (_inputString != string.Empty)
        { NotifyOfPropertyChange(() => CanClearOutputField); OutputString = ConvertInputStringToOutput(); }
        NotifyOfPropertyChange();
      }
    }
    #endregion

    #region button action methods
    public void AddEmptyTool()
    {
      OutputString += new Tool { Type = ToolType.Empty, Version = String.Empty }.ToString();
    }

    public void AddEmptyRelease()
    {
      OutputString += new Release().PrintRelease();
    }

    public void ClearOutputField()
    {
      OutputString = String.Empty;
      InputString = String.Empty;
      ErrorSavingFile = String.Empty;
      NotifyOfPropertyChange(() => ErrorSavingFile);
    }
    #endregion

    #region Save file
    public void SaveFile()
    {
      string fileName = string.Empty;

      fileName = (ReleaseVersion % 1) == 0 
                   ? $"OMNIA {ReleaseVersion}.0 Patch {ReleasePatch}" 
                   : $"OMNIA {ReleaseVersion.ToString().Replace(',', '.')} Patch {ReleasePatch}";

      string pathOfFile = SaveFileHelper.OpenSaveFileDialog(SaveFileType.TXT, fileName);

      if (string.IsNullOrEmpty(pathOfFile)) return;

      _fileText = OutputString;
      FileHandler.SaveToFile(pathOfFile, _fileText, false);
    }

    private bool Validate()
    {
      if (string.IsNullOrEmpty(OutputString)) return false;

      if (!VerifyVersion() || !VerifyPatch() || !VerifyTools() || !VerifyRestrictions())
        return false;

      ErrorSavingFile = "Make sure the data is correct before saving";

      return true;
    }

    public bool VerifyVersion()
    {
      if (_splitOutputText.Length < 0)
        return false;

      if (_splitOutputText[0].Length < 13)
      {
        ErrorSavingFile = "Release version cannot be empty";
        return false;
      }

      if (Regex.IsMatch(_splitOutputText[0].Substring(11), @"\s[a-zA-Z]+$"))
      {
        ErrorSavingFile = "Release version cannot contain letters";
        return false;
      }

      if (_splitOutputText[0].Substring(11).Contains(","))
      {
        ErrorSavingFile = "Use dot (.) instead of comma (,)";
        return false;
      }

      if (!_splitOutputText[0].Substring(11).Contains("."))
      {
        ErrorSavingFile = "Release version must be a double";
        return false;
      }

      if (_splitOutputText[0].EndsWith("."))
      {
        ErrorSavingFile = "Release version is incomplete";
        return false;
      }

      if (!double.TryParse(_splitOutputText[0].Split()[2], NumberStyles.Any, CultureInfo.InvariantCulture, out double relVersion))
      {
        ErrorSavingFile = "Release version cannot be parsed as a double";
        return false;
      }

      ReleaseVersion = relVersion;

      if (relVersion <= 0)
      {
        ErrorSavingFile = "Release version cannot be zero or negative";
        return false;
      }

      if (_splitOutputText[0].Substring(11).Contains(".0"))
      {
        ErrorSavingFile = string.Empty;
        return true;
      }

      ErrorSavingFile = string.Empty;
      return true;
    }

    public bool VerifyPatch()
    {
      if (_splitOutputText[1].Length < 11)
      {
        ErrorSavingFile = "Patch input cannot be empty";
        return false;
      }

      if (Regex.IsMatch(_splitOutputText[1].Substring(8), @"\s[a-zA-Z]+$"))
      {
        ErrorSavingFile = "Patch input cannot contain letters";
        return false;
      }

      if (!int.TryParse(_splitOutputText[1].Split()[2].Trim(), out int relPatch))
      {
        ErrorSavingFile = "Patch input cannot be parsed as an int";
        return false;
      }

      ReleasePatch = relPatch;

      if (relPatch < 0)
      {
        ErrorSavingFile = "Patch input cannot be negative";
        return false;
      }

      ErrorSavingFile = string.Empty;

      return true;
    }

    public bool VerifyRestrictions()
    {
      if (_splitOutputText[3].Length == 31)
      {
        ErrorSavingFile = "Please specify restrictions";
        return false;
      }
      ErrorSavingFile = string.Empty;

      return true;
    }

    public bool VerifyTools()
    {
      if (_splitOutputText.Length < 7)
      {
        ErrorSavingFile = "Components are missing";
        return false;
      }

      try
      {
        Release checkRelease = Release.Deserialize(OutputString);
        foreach (Tool tool in checkRelease.ToolList)
        {
          if (tool.Type == ToolType.UNDEFINED)
          {
            ErrorSavingFile = "Component name must be valid";
            return false;
          }

          if (string.IsNullOrWhiteSpace(tool.Version))
          {
            ErrorSavingFile = "Component version cannot be empty";
            return false;
          }
        }
      }
      catch
      {
        ErrorSavingFile = "Release has a wrong format. Check the template.";
        return false;
      }

      ErrorSavingFile = string.Empty;
      return true;
    }
    #endregion

    #region logic
    public string ConvertInputStringToOutput()
    {
      RawOutputString = Parse(InputString);
      SetReleaseVersionAndPatch(RawOutputString[0]);
      Release releaseObject = new Release { Version = ReleaseVersion, Patch = ReleasePatch, ToolList = _toolList, RestrictionComment = RestrictionText };
      OutputString = releaseObject.PrintRelease();
      return OutputString;
    }

    private ObservableCollection<string> Parse(string _strToBeParsed)
    {
      string[] allLines = _strToBeParsed.Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                                        .Where(line => !string.IsNullOrEmpty(line))
                                        .ToArray();

      allLines = ParseHelper.TrimArrayAndRemoveUnusedText(allLines);
      _toolList = GetTools(allLines);
      SetRestrictionText(allLines);

      return new ObservableCollection<string>(allLines);
    }

    private List<Tool> GetTools(string[] arrayOfAllLines)
    {
      List<string> versionList = new List<string>();
      List<string> nameList = new List<string>();
      List<Tool> toolList = new List<Tool>();
      string[] alphaNumericVersionTools = { "OMNIPOWER", "OMNICON" };

      foreach (string line in arrayOfAllLines)
      {
        string[] oneLine;

        oneLine = (!alphaNumericVersionTools.Any(line.Contains)) ? ParseHelper.SeparateNameAndVersionsNumeric(line) : ParseHelper.SeparateNameAndVersionsAlphaNumeric(line);

        foreach (string nameOrVersion in oneLine)
        {
          if (ParseHelper.MatchPattern(nameOrVersion, "^[0-9]*\\.\\s+[a-z,A-Z]+\\W{0,1}") || nameOrVersion.Contains("OMNIPOWER"))
            nameList.Add(nameOrVersion);

          if (ParseHelper.MatchPattern(nameOrVersion, "\\d+\\.\\d+\\.\\d+\\.*\\d*") || ParseHelper.MatchPattern(nameOrVersion, "^[a-z,A-Z]{1,2}\\d{1,2}\\s*$"))
            versionList.Add(nameOrVersion);
        }
      }

      if (nameList.Count != versionList.Count) return toolList;
      for (int i = 0; i < nameList.Count; i++)
      {
        string modifiedName = nameList.ElementAt(i);
        string modifiedVersion = versionList.ElementAt(i);

        if (ParseHelper.CheckMatchPattern(modifiedName))
        {
          string[] arr = modifiedName.Trim().Split();
          arr = arr.Where((o, j) => j != arr.Length - 1).ToArray();
          modifiedName = string.Join(" ", arr);
        }

        if (ParseHelper.CheckMatchPattern(modifiedVersion))
          modifiedVersion = modifiedVersion.Split().Last();

        if (nameList.ElementAt(i).Length > 0)
          if (char.IsDigit(nameList.ElementAt(i)[0]))
            modifiedName = nameList.ElementAt(i).Substring(2).Trim();

        Tool tmpTool = new Tool { Type = Tool.CheckToolName(modifiedName.Trim()), Version = modifiedVersion };
        toolList.Add(tmpTool);
      }
      return toolList;
    }

    private void SetReleaseVersionAndPatch(string inputStr)
    {
      string[] inputArr = inputStr.Split();

      if (inputArr.Length > 4)
      {

        if (double.TryParse(inputArr[inputArr.Length - 4].Trim(), NumberStyles.Number, CultureInfo.GetCultureInfo("en-US"), out double releaseVersion))
          ReleaseVersion = releaseVersion;

        if (int.TryParse(inputArr[inputArr.Length - 2].Trim(), out int patchNumber))
          ReleasePatch = patchNumber;
      }
      else
        Log.Default.Error("Problem with getting the release version and patch from text header");
    }

    private void SetRestrictionText(string[] arr)
    {
      if (arr.Length > 0)
      {
        string[] tmpArr = new string[] { arr[0], arr[1] };

        foreach (string item in tmpArr)
        {
          if (item.Contains("Restrict") || item.Contains("only"))
            RestrictionText = "Restricted to ";
          else
            RestrictionText = string.Empty;
        }
      }
      else RestrictionText = string.Empty;
    }
    #endregion
  }
}

