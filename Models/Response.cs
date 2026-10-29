using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace i_KimMobileClient.Models
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public object Value { get; set; }
        public string ErrorMessage { get; set; }
    }
}
