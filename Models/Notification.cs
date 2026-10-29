using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace i_KimMobileClient.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Index { get; set; }
        public bool IsRead { get; set; }
        public int? LastModifiedBy { get; set; }

        [JsonIgnore]
        public bool ShowUnreadIndicator => !IsRead;

        [JsonIgnore]
        public bool ShowHeader { get; set; }

        [JsonIgnore]
        public string HeaderText { get; set; }

        [JsonIgnore]
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.Now - CreatedDate;

                if (timeSpan.TotalSeconds < 60)
                    return "Just now";

                if (timeSpan.TotalMinutes < 60)
                    return $"{(int)timeSpan.TotalMinutes} min ago";

                if (timeSpan.TotalHours < 24)
                    return $"{(int)timeSpan.TotalHours} hour{((int)timeSpan.TotalHours > 1 ? "s" : "")} ago";

                if (timeSpan.TotalDays < 2)
                    return "1 day ago";

                if (timeSpan.TotalDays < 7)
                    return $"{(int)timeSpan.TotalDays} days ago";

                if (timeSpan.TotalDays < 30)
                    return $"{(int)(timeSpan.TotalDays / 7)} week{((int)(timeSpan.TotalDays / 7) > 1 ? "s" : "")} ago";

                if (timeSpan.TotalDays < 365)
                    return $"{(int)(timeSpan.TotalDays / 30)} month{((int)(timeSpan.TotalDays / 30) > 1 ? "s" : "")} ago";

                return $"{(int)(timeSpan.TotalDays / 365)} year{((int)(timeSpan.TotalDays / 365) > 1 ? "s" : "")} ago";
            }
        }
    }
}
