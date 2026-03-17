using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeenaSikhoPusher
{
    class GlobalConfig
    {
        public static string strConnLimsDB = ConfigurationManager.ConnectionStrings["strConnLimsDB"].ToString();
    }
}
