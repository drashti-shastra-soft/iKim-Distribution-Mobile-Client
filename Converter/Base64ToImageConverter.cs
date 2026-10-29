using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Converter
{
    public class Base64ToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType,
        object? parameter, CultureInfo culture)
        {
            if (value is not string base64String ||
                string.IsNullOrWhiteSpace(base64String))
                return null;

            try
            {
                // Remove data URI prefix if present
                // e.g. "data:image/png;base64,iVBORw0KG..."
                if (base64String.Contains(","))
                    base64String = base64String.Split(',')[1];

                byte[] imageBytes = System.Convert.FromBase64String(base64String);

                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object? value, Type targetType,
            object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
