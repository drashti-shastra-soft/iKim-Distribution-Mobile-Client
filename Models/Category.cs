using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace i_KimMobileClient.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Multiply { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string ImagePath { get; set; }
        public string Base64String { get; set; }

        //Extra
        //[JsonIgnore]
        public int Index { get; set; }
        [JsonIgnore]
        public ImageSource? ImageSourceCached { get; set; }
    }
}
