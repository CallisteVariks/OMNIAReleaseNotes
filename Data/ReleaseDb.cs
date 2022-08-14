using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OMNITools.CommonObjects.Util;
using OMNIToolNetworkAnalyzer.Common;
using System.Reflection;

namespace OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Data
{
  public class ReleaseDb
  {
    private static readonly string _directoryPath = Path.Combine($"{new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName}\\ReleaseNotes");

    #region fields and properties
    public static List<string> ExistingTools { get; set; } = new List<string>();
    public static List<string> VersionList { get; set; } = new List<string>();
    public static List<Release> ReleaseList { get; set; } = new List<Release>();
    #endregion

    public static List<Release> GetAllReleases()
    {
      List<Release> allReleases = new List<Release>();

      foreach (string file in Directory.EnumerateFiles(_directoryPath, "*.txt"))
      {
        try
        {
          Release fileRelease = Release.Deserialize(FileHandler.LoadFileIntoString(file, false));
          allReleases.Add(fileRelease);
        }
        catch (Exception ex)
        {
          Log.Default.Error($"{ex}", "Error while reading ReleaseNote file");
        }
      }

      ReleaseList = allReleases;
      return allReleases.OrderByDescending(release => release.Version).ThenByDescending(release => release.Patch).ToList();
    }

    internal static List<string> GetToolsNamesForPatches(List<Release> releaseList)
    {
      if (releaseList.Count == 0) return new List<string>();

      foreach (Release rls in releaseList)
        SetToolsNamesForPatch(rls);

      ExistingTools = releaseList.SelectMany(rel => rel.AllToolsList)
                                                    .Select(tool => tool.Name)
                                                    .Distinct().ToList();
      ExistingTools.Sort();
      return ExistingTools;
    }

    public static void SetToolsNamesForPatch(Release release)
    {
      List<Tool> releaseTools = new List<Tool>();

      foreach (Release rls in ReleaseList.Where(r => r.Version == release.Version).Where(rel => rel.Patch <= release.Patch))
      {
        releaseTools.AddRange(rls.ToolList);
      }
      releaseTools = releaseTools.GroupBy(elem => elem.Name)
                                 .Select(group => group.FirstOrDefault())
                                 .ToList();
      releaseTools.Sort();
      release.AllToolsList = releaseTools;
    }
  }
}
