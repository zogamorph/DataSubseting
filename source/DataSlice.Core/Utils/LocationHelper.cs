using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataSlice.Core.Utils
{
    public class LocationHelper
    {
        public static string GetExeFileLocation()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static string EnsureExePathDirectory(string subDirectory)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);


            string directory = Path.GetDirectoryName(path);

            string newDirectory = Path.Combine(directory, subDirectory);

            if (!Directory.Exists(newDirectory))
            {
                Directory.CreateDirectory(newDirectory);
            }

            return newDirectory;
        }
    }
}
