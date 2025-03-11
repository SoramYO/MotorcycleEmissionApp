using System;
using System.Globalization;
using System.Windows.Data;

namespace MotorcycleEmissionUI.Police
{
	public class BoolToPassFailConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// Handle string values (from InspectionRecord.Result)
			if (value is string stringValue)
			{
				switch (stringValue)
				{
					case "Pass":
						return "Đạt";
					case "Fail":
						return "Không đạt";
					case "Pending":
						return "Chờ xử lý";
					default:
						return stringValue;
				}
			}

			// Handle boolean values for backward compatibility
			if (value is bool boolValue)
			{
				return boolValue ? "Đạt" : "Không đạt";
			}

			return "N/A";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
