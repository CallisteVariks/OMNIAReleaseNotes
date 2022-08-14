using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OMNIToolNetworkAnalyzer.Presentation.Main.About.OMNIAReleaseNotes.Converter
{
  public class NumbersToVersionAndPatchConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value is double doub)
        return $"OMNIA {doub.ToString("0.0").Replace(',', '.')}";

      if (value is int numb)
        return numb != 0 ? $"Patch {numb}" : "";

      return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}