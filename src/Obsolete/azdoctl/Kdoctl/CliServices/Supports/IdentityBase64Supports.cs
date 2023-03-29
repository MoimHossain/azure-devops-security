

using System;
using System.Collections.Generic;
using System.Text;

namespace Kdoctl.CliServices.Supports
{
    public class IdentityBase64Supports
    {
        public static string GetSid(string descriptor)
        {
            if (!string.IsNullOrWhiteSpace(descriptor))
            {
                var b64s = descriptor.Substring(descriptor.IndexOf(".") + 1);
                var raw = DecodeUrlBase64(b64s);
                return $"Microsoft.TeamFoundation.Identity;{raw}";
            }
            return string.Empty;
        }

        public static string DecodeUrlBase64(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/').PadRight(4 * ((s.Length + 3) / 4), '=');
            return ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(s));
        }
    }
}
