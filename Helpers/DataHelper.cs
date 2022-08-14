using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Common.Enums;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Common.Models;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Helpers
{
  public class DataHelper
  {
    public static List<Tool> GetLatestToolVersionsForPatch(Release release)
    {
      List<Release> patchReleases = ReleaseDb.ReleaseList.Where(rls => rls.Version == release.Version).ToList();
      List<Tool> patchTools = GetAllToolsForVersion(patchReleases);
      List<Tool> latestToolsVersions = new List<Tool>();

      foreach (Tool item in patchTools)
      {
        List<Tool> versionList = patchTools.Where(tool => tool.Name.Equals(item.Name)).ToList();
        Tool latestVer = versionList.Where(tool => tool.Patch <= release.Patch)
                                    .OrderByDescending(tool => tool.ReleaseDate)
                                    .Distinct()
                                    .FirstOrDefault();
        if (latestVer == null)
          continue;
        latestToolsVersions.Add(latestVer);
      }
      latestToolsVersions = latestToolsVersions.Distinct().ToList();

      return latestToolsVersions;
    }

    public static Release GetUpdatedRelease(Release release, List<string> existingToolNames)
    {
      if (existingToolNames.Count == 0) return new Release();

      Release toReturn = release.Clone();
      List<string> patchToolsNames = release.ToolList.Select(t => t.Name).ToList();
      List<string> toolNamesNotInPatch = existingToolNames.ToList().Except(patchToolsNames).ToList();
      List<Tool> updatedTools = GetLatestToolVersionsForPatch(release);

      foreach (string name in toolNamesNotInPatch)
      {
        ToolType type = Tool.CheckToolName(name.Trim());
        if (type == ToolType.UNDEFINED) return toReturn;

        Tool tempTool = new Tool { Type = type, Version = "--" };
        updatedTools.Add(tempTool);

      }
      updatedTools = updatedTools.GroupBy(tool => tool.Name)
                                     .Select(group => group.FirstOrDefault())
                                     .OrderBy(t => t.Name)
                                     .ToList();
      toReturn.ToolList = updatedTools;

      return toReturn;
    }

    private static List<Tool> GetAllToolsForVersion(List<Release> releasesForVersion)
    {
      List<Tool> toolsForVersion = new List<Tool>();

      foreach (Release release in releasesForVersion)
      {
        foreach (Tool tool in release.ToolList)
        {
          tool.OMNIAVersion = release.Version;
          tool.Patch = release.Patch;
          tool.ReleaseDate = Convert.ToDateTime(release.ReleaseDate);
          toolsForVersion.Add(tool);
        }
      }
      return toolsForVersion;
    }
  }
}
