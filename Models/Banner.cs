using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Models
{
    public class Banner
    {
        public int Id { get; set; }
        public string MimeType { get; set; }
        public int Sequence { get; set; }
        public string Base64String { get; set; }

        private ImageSource? _cachedImage;

        public ImageSource? ImageSourceValue
        {
            get
            {
                if (_cachedImage != null)
                    return _cachedImage;

                if (string.IsNullOrWhiteSpace(Base64String))
                    return null;

                try
                {
                    var base64 = Base64String.Contains(",")
                        ? Base64String.Split(',')[1]
                        : Base64String;

                    var bytes = Convert.FromBase64String(base64);
                    _cachedImage = ImageSource.FromStream(() => new MemoryStream(bytes));
                    return _cachedImage;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
