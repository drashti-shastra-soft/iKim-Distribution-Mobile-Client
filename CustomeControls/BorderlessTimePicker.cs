using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.CustomeControls
{
    public class BorderlessTimePicker : TimePicker
    {
        public BorderlessTimePicker()
        {
            // Force 24hr format in constructor
            Format = "HH:mm";
        }
    }
}
