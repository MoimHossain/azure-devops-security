using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Kdoctl.CliServices.Supports
{
    public class RestUtils
    {
        public static string UriEncode(string value)
        {
            return Uri.EscapeDataString(value);
        }
    }
}
