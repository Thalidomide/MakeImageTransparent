namespace ImageUtils
{
    public class Start
    {
        public static void Main(string[] args)
        {
            var generator = new RunGenerateImages();
            generator.Run(GlobalConfig.Path, GlobalConfig.SourceFileOrBlankForAll);
        }
    }
}
