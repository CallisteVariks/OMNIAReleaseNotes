using Caliburn.Micro;
using OMNIToolNetworkAnalyzer.Core.Events;
using OMNIToolNetworkAnalyzer.Core.UserControls.SearchControl;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Common.Models;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Data;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Presentation.ReleaseNotes
{
  public class ReleaseNotesViewModel : PropertyChangedBase
  {
    #region properties
    public SearchControlViewModel<Release> ReleasesSearchViewModel { get; } = new SearchControlViewModel<Release>();
    public List<Release> ReleaseList => ReleasesSearchViewModel.SearchResult.ToList();
    public List<Release> SelectedReleasesForCompare => ReleaseList.Where(r => r.IsSelectedForCompare).ToList();
    public List<Release> UpdatedRelList { get; set; } = new List<Release>();
    public List<string> ExistingToolsList => new List<string>(ReleaseDb.GetToolsNamesForPatches(SelectedReleasesForCompare).OrderBy(val => val));
    public bool CanCompareSelectedReleases => SelectedReleasesForCompare.Count > 0;
    public bool CanDeselectSelectedReleases => ReleasesSearchViewModel.AllSearchableObjects.Any(r => r.IsSelectedForCompare);
    #endregion

    public ReleaseNotesViewModel()
    {
      ReleasesSearchViewModel.SearchEvent += SearchViewModel_SearchEvent;
      Loaded();
    }

    #region Search
    private void SearchViewModel_SearchEvent(object sender, SearchControlEvent<Release> e)
    {
      if (e.SearchControlAction == SearchControlEvent.SearchCompleted)
      {
        foreach (Release selectedRelease in ReleaseList.Where(r => r.IsSelectedForCompare))
            selectedRelease.IsSelectedForCompare = false;

        UpdatedRelList = new List<Release>();
        NotifyOfPropertyChange(() => ExistingToolsList);
        NotifyOfPropertyChange(() => ReleaseList);
        NotifyOfPropertyChange(() => SelectedReleasesForCompare);
        NotifyOfPropertyChange(() => UpdatedRelList);
        NotifyOfPropertyChange(() => CanCompareSelectedReleases);
        NotifyOfPropertyChange(() => CanDeselectSelectedReleases);
      }
      if (e.SearchControlAction == SearchControlEvent.AllSearchableObjectsCreated)
        BindIsSelectedForCompare();
    }

    public void BindIsSelectedForCompare()
    {
      foreach (Release release in ReleasesSearchViewModel.AllSearchableObjects)
        release.PropertyChanged += (sender, eventArgs) =>
        {
          NotifyOfPropertyChange(() => CanCompareSelectedReleases);
          NotifyOfPropertyChange(() => CanDeselectSelectedReleases);
          NotifyOfPropertyChange(() => UpdatedRelList);
        };
    }
    #endregion

    public void Loaded()
    {
      UpdatedRelList = new List<Release>();
      ReleasesSearchViewModel.InputText = string.Empty;
      ReleasesSearchViewModel.ClearResults();
      ReleasesSearchViewModel.CreateSearchableData(ReleaseDb.GetAllReleases());
    }

    #region Compare selected releases
    public void CompareSelectedReleases()
    {
      List<Release> updatedReleaseList = new List<Release>();

      foreach (Release item in SelectedReleasesForCompare)
        updatedReleaseList.Add(DataHelper.GetUpdatedRelease(item, ExistingToolsList));

      UpdatedRelList = updatedReleaseList.OrderBy(release => release.Version)
                                         .ThenBy(r => r.Patch)
                                         .ToList();

      NotifyOfPropertyChange(() => SelectedReleasesForCompare);
      NotifyOfPropertyChange(() => ExistingToolsList);
      NotifyOfPropertyChange(() => UpdatedRelList);
    }

    public void DeselectSelectedReleases()
    {
      foreach (Release selectedRelease in ReleasesSearchViewModel.AllSearchableObjects.Where(r => r.IsSelectedForCompare))
        selectedRelease.IsSelectedForCompare = false;
      NotifyOfPropertyChange(() => SelectedReleasesForCompare);
      NotifyOfPropertyChange(() => UpdatedRelList);
    }
    #endregion
  }
}