using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeenaSikhoPusher
{
   public class ResultResponse
    {
        public bool status { get; set; }
        public int httpCode { get; set; }
        public string message { get; set; }
        public int order_id { get; set; }
        public string apiEndPoint { get; set; }

    }
}
