using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Converter
{
    public class IndexToColorConverter : IValueConverter
    {
        // Define your palette — add/remove as many as you want
        private static readonly Color[] Palette =
        {
        Color.FromArgb("#A8D5A0"), // Green
        Color.FromArgb("#F4A6B8"), // Pink
        Color.FromArgb("#F5B942"), // Yellow
        Color.FromArgb("#8B93E8"), // Blue/Purple
        Color.FromArgb("#A8D5A0"), // Blue/Purple
        Color.FromArgb("#F4A6B8"), // Blue/Purple
        Color.FromArgb("#F5B942"), // Blue/Purple
        Color.FromArgb("#8B93E8"), // Blue/Purple
    };

        public object Convert(object? value, Type targetType,
            object? parameter, CultureInfo culture)
        {
            if (value is int index)
                return Palette[index % Palette.Length]; // cycles automatically

            return Colors.Transparent;
        }

        public object ConvertBack(object? value, Type targetType,
            object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
