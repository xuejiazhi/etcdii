using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace etcdii
{
    internal class util
    {
        public static string base64Decode(string code)
        {
            byte[] outputb = Convert.FromBase64String(code);
            string str = Encoding.Default.GetString(outputb);
            return str;
        }

        public static string base64Encode(string code)
        {
            byte[] bytes = Encoding.Default.GetBytes(code);
            string str = Convert.ToBase64String(bytes);
            return str;
        }

    }
}
