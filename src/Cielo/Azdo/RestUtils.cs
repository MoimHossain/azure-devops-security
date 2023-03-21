using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cielo.Azdo
{
    public class RestUtils
    {
        public static string UriEncode(string value)
        {
            return Uri.EscapeDataString(value);
        }
    }
}
