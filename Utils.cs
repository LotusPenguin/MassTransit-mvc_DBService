using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSR_Backend
{
    internal class Utils
    {
        public static string formatTimestamp(string input)
        {
            return "[" + input + "] ";
        }
        public static string printTimestamp()
        {
            return formatTimestamp(DateTime.Now.ToString());
        }
    }
}
