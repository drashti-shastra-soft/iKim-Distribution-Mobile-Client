using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Converter
{
    public class IntToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType,
       object? parameter, CultureInfo culture)
        {
            if (value is not string status) return Colors.Transparent;

            return status.Trim().ToLower() switch
            {
                "pending" => Color.FromArgb("#dc3545"), // Red
                "partially fulfilled" => Color.FromArgb("#ffc107"), // Orange/Yellow
                "fulfilled" => Color.FromArgb("#28a745"), // Green
                _ => Color.FromArgb("#9E9E9E")  // Gray fallback
            };
        }

        public object ConvertBack(object? value, Type targetType,
            object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
