using System.Configuration;

namespace ImageUtils
{
    public class GlobalConfig
    {
        public static string Path => ConfigurationManager.AppSettings["Path"];
        public static string SourceFileOrBlankForAll => ConfigurationManager.AppSettings["SourceFileOrBlankForAll"];
    }
}
