using MvvmCross.Platform.Converters;
using System;
using System.Globalization;

namespace XamarinCore.Converters
{
	public class InvertBoolValueConverter : MvxValueConverter<bool, bool>
	{
		protected override bool Convert(bool value, Type targetType, object parameter, CultureInfo culture)
		{
			return !value;
		}

		protected override bool ConvertBack(bool value, Type targetType, object parameter, CultureInfo culture)
		{
			return !value;
		}
	}
}
