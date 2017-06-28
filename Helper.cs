using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeMatesClient
{
    class Helper
    {
        Logger Log = new Logger();
        public string CleanString(string text)
        {
            return text.Replace("\n","").Replace("\r","").TrimEnd().TrimStart();
        }

        public void LogAnException(string text)
        {
            Log.WirteLog(text);
        }
    }
}
