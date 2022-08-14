using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Common.Models
{
  public class Tool : IComparable<Tool>
  {
    internal const string Delimiter_Tool = "[Component]";
    internal const string Delimiter_Name = "Name";
    internal const string Delimiter_Version = "Version";
    internal const string Delimiter_Comment = "Comment";
    internal const string Delimiter_Restriction = "Restriction";
    internal const string Delimiter_FileName = "File Name";
    public double? OMNIAVersion { get; set; }
    public int? Patch { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Version { get; set; }
    public string Restriction { get; set; }
    public string FileName { get; set; }
    public string Comment { get; set; }
    public ToolType Type { get; set; }

    public string Name
    {
      get
      {
        return Type
                  .GetType()
                  .GetMember(Type.ToString())
                  .FirstOrDefault()
                  ?.GetCustomAttribute<DescriptionAttribute>()
                  ?.Description;
      }
    }

    public static Tool Deserialize(string serializedText)
    {
      Tool toReturn = new Tool();
      if (string.IsNullOrWhiteSpace(serializedText)) return toReturn;

      string[] lines = serializedText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
      foreach (string line in lines)
      {
        if (line.Equals(string.Empty)) continue;

        string[] property = line.Split(':');

        if (property.Length != 2) continue;

        string key = property[0].Trim();
        string value = property[1].Trim();

        if (int.TryParse(value, out _)) continue;

        switch (key)
        {
          case "Name": toReturn.Type = (ToolType)Enum.Parse(typeof(ToolType), value); break;
          case "Version": toReturn.Version = value; break;
          case "File Name": toReturn.FileName = value; break;
          case "Restriction": toReturn.Restriction = value; break;
          case "Comment": toReturn.Comment = value; break;

          default: MessageBox.Show($"Unknown property met. Property was {key}. Name: {toReturn.Name} Version: {toReturn.Version}"); break;
        }
      }
      return toReturn;
    }

    internal Tool Clone()
    {
      return new Tool
      {
        Type = Type,
        Version = Version,
        FileName = FileName,
        Restriction = Restriction,
        Comment = Comment,
        Patch = Patch,
        OMNIAVersion = OMNIAVersion
      };
    }

    public static List<Tool> DeserializeToolList(string serializedFileText)
    {
      if (string.IsNullOrWhiteSpace(serializedFileText))
        return new List<Tool>();

      List<string> serializedStringList = serializedFileText.Split(new[] { $"{Delimiter_Tool}" }, StringSplitOptions.None).Skip(1).ToList();
      return serializedStringList.Select(Deserialize).Where(t => t != null).ToList();


    }

    public override string ToString()
    {
      string output = string.Join(Environment.NewLine,
                         $"{Delimiter_Tool}",
                         $"{Delimiter_Name} : {Type}",
                         $"{Delimiter_Version} : {Version}",
                         $"{Delimiter_FileName} : {FileName}",
                         $"{Delimiter_Restriction} : {Restriction}",
                         $"{Delimiter_Comment} : {Comment}",
                         Environment.NewLine);
      return Type != ToolType.Empty ? output : output.Replace($"{Type}", string.Empty);
    }

    public static ToolType CheckToolName(string nameToCheck)
    {
      switch (nameToCheck.Replace("®", string.Empty).ToLower())
      {
        case "omnicon connect": return ToolType.OMNICONConnect;

        case "omnicon wired muc": return ToolType.OMNICONWiredMUC;

        case "omnicon":
        case "omnicon concentrator": return ToolType.OMNICONConcentrator;

        case "omnisoft visionair": return ToolType.OMNISOFTVisionAir;

        case "omnia datahub": return ToolType.OMNIADataHub;

        case "omnisoft utilidriver": return ToolType.OMNISOFTUtiliDriver;

        case "omnisoft utilidriver network manager": return ToolType.OMNISOFTUtiliDriverNetworkManager;

        case "omnisoft key management service": return ToolType.OMNISOFTKeyManagementService;

        case "omnisoft keyfetcher":
        case "omnisoft key fetcher": return ToolType.OMNISOFTKeyFetcher;

        case "omnisoft omnia preparation tool  (opt)":
        case "omnisoft omnia preparation tool (opt)":
        case "omnisoft omnia preparation tool(opt)":
        case "omnisoft omnia preparation tool": return ToolType.OMNISOFTPreparationTool;

        case "omnia datavault":
        case "omnia data vault": return ToolType.OMNIADataVault;

        case "omnisoft p2p controller": return ToolType.OMNISOFTP2PController;

        case "omnisoft m2m": return ToolType.OMNISOFTM2M;

        case "omnia snmp proxy": return ToolType.OMNIASNMPProxy;

        case "omnisoftalarmserver":
        case "omnisoft alarmserver": return ToolType.OMNISOFTAlarmServer;

        case "omnisoft infrastructure provisioning tool (ipt)":
        case "infrastructure provisioning tool (ipt)":
        case "infrastructure provisioning tool": return ToolType.OMNISOFTIPT;

        case "omnigrid connect": return ToolType.OMNIGRIDConnect;

        case "omnia gofetch": return ToolType.OMNIAGoFetch;

        case "omnicon muc": return ToolType.OMNICONMUC;

        case "omnicon 2g modem":
        case "omnicon 2gmodem": return ToolType.OMNICON2GModem;

        case "omnicon 4g modem":
        case "omnicon 4gmodem": return ToolType.OMNICON4GModem;


        case "omnipower meter variant 1 direct - (fw number 5098736)": return ToolType.OMNIPOWERMeterVar1FW5098736;

        case "omnipower direct variant 2 (50981173)":
        case "omnipower meter variant 2 direct  - (fw number 50981173)": return ToolType.OMNIPOWERMeterVar2FW50981173;

        case "omnipower meter variant 2 direct  - restricted (radius) - (fw number 50981235)": return ToolType.OMNIPOWERMeterVar2FW50981235;

        case "omnipower meter variant 2 direct  - restricted (radius) - (fw number 50981285)": return ToolType.OMNIPOWERMeterVar2FW50981285;

        case "omnipower ct variant 1 - (fw number50981040)": return ToolType.OMNIPOWERCTVar1FW50981040;

        case "omnipower meter variant 2 direct aron meter - (fw number 50981165)":
        case "omnipower  variant 2 aron meter  (50981165)":
        case "omnipower variant 2 aron meter  (50981165)": return ToolType.OMNIPOWERMeterVar2AronFW50981165;

        case "omnipower ct variant 2 - (fw number50981251)":
        case "omnipower  ct variant 2 (50981251)":
        case "omnipower ct variant 2 (50981251)": return ToolType.OMNIPOWERCTVar2FW50981251;

        case "omnipower ct variant 2   - restricted  (austria) - (fw number 50981444)":
        case "omnipower  ct variant 2 (50981444 austria)": return ToolType.OMNIPOWERCTVar2FW50981444;

        case "omnipower meter variant 2 direct  - restricted  (austria) - (fw number 50981266)": return ToolType.OMNIPOWERMeterVar2FW50981266;

        case "omnipower ct variant 2 -  restricted (radius) - (fw number 50981304)": return ToolType.OMNIPOWERCTVar2FW50981304;

        case "omnisoftcim interface (oci)":
        case "omnisoft cim interface (oci)":
        case "omnisoftcim interface":
        case "omnisoft cim interface": return ToolType.OMNISOFTCIMInterface;

        case "omnitools": return ToolType.OMNITools;

        default: return ToolType.UNDEFINED;
      }
    }

    public int CompareTo(Tool other)
    {
      return Type.CompareTo(other.Type);
    }
  }
}


