using Caliburn.Micro;
using OMNIToolNetworkAnalyzer.Core.Enums;
using OMNIToolNetworkAnalyzer.Core.Interfaces;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Presentation.Micro;
using OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Presentation.ReleaseNotes;

namespace OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Presentation.Main
{
  public class MainReleaseNotesViewModel : PropertyChangedBase, IMenuItem
  {
    public string Name => "OMNIA release notes";
    public string Image => "/OMNIToolNetworkAnalyzer; component/Assets/Icons/Folder-64.png";
    public PropertyChangedBase ViewModel => this;
    public MenuType MenuType => MenuType.About;
    public ReleaseNotesViewModel ReleaseNotesViewModel { get; internal set; }
    public MicroViewModel MicroViewModel { get; internal set; }

    public MainReleaseNotesViewModel(ReleaseNotesViewModel releaseNotesViewModel, MicroViewModel microViewModel)
    {
      ReleaseNotesViewModel = releaseNotesViewModel;
      MicroViewModel = microViewModel;
    }
  }
}

