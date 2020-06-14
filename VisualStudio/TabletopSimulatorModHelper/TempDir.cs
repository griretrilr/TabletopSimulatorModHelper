using System.IO;

namespace TabletopSimulatorModHelper.Tests
{
    public class TempDir : Disposable
    {
        public TempDir()
        {
            Path = System.IO.Path.GetTempFileName() + ".dir";
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public bool Keep { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (!Keep)
            {
                DirectoryUtils.RecursiveDelete(Path);
            }
        }
    }
}
