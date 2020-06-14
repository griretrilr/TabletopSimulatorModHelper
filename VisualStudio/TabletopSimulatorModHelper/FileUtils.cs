using System.IO;

namespace TabletopSimulatorModHelper
{
    public static class FileUtils
    {
        public static void Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (DirectoryNotFoundException) { }
            catch (FileNotFoundException) { }
        }
    }
}
