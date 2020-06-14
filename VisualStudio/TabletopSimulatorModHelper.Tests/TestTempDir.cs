using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace TabletopSimulatorModHelper.Tests
{
    [TestClass]
    public class TestTempDir
    {
        private TempDir CreateTempDir()
        {
            var tempDir = new TempDir();

            Assert.IsTrue(Directory.Exists(tempDir.Path), "temp dir exists and is a directory");
            Assert.IsTrue(!Directory.EnumerateFileSystemEntries(tempDir.Path).Any(), "temp dir starts empty");

            return tempDir;
        }

        private string CreateTempDirReturnPath(bool addFiles)
        {
            var tempDir = CreateTempDir();
            string tempDirPath = tempDir.Path;
            if (addFiles)
            {
                AddFilesAndSubdirectories(tempDir);
            }
            return tempDirPath;
        }

        const int NumFileSystemEntries = 5;

        private void AddFilesAndSubdirectories(TempDir tempDir)
        {
            Directory.CreateDirectory(Path.Join(tempDir.Path, "dir"));
            Directory.CreateDirectory(Path.Join(tempDir.Path, "dir/dir2"));
            File.Create(Path.Join(tempDir.Path, "file1"));
            File.Create(Path.Join(tempDir.Path, "dir/file2"));
            File.Create(Path.Join(tempDir.Path, "dir/dir2/file3"));
        }

        private int CountFileSystemEntries(string dir) => Directory.EnumerateFileSystemEntries(dir, "*", new EnumerationOptions { RecurseSubdirectories = true }).Count();

        [TestMethod]
        public void TestEmpty()
        {
            string tempDirPath;
            using (TempDir tempDir = CreateTempDir())
            {
                tempDirPath = tempDir.Path;
            }

            Assert.IsTrue(!Directory.Exists(tempDirPath), "temp dir deleted on dispose");
        }

        [TestMethod]
        public void TestWithFilesAndSubdirectories()
        {
            string tempDirPath;
            using (TempDir tempDir = CreateTempDir())
            {
                tempDirPath = tempDir.Path;
                AddFilesAndSubdirectories(tempDir);
            }

            Assert.IsTrue(!Directory.Exists(tempDirPath), "temp dir deleted on dispose");
        }

        [TestMethod]
        public void TestFinalize()
        {
            string tempDirPath = CreateTempDirReturnPath(true);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, true);
            Assert.IsTrue(!Directory.Exists(tempDirPath), "temp dir deleted on finalize");
        }

        [TestMethod]
        public void TestKeep()
        {
            string tempDirPath;
            using (TempDir tempDir = CreateTempDir())
            {
                tempDirPath = tempDir.Path;
                AddFilesAndSubdirectories(tempDir);
                tempDir.Keep = true;
            }

            Assert.IsTrue(Directory.Exists(tempDirPath), $"temp dir not deleted on dispose when {nameof(TempDir.Keep)} is true");
            Assert.AreEqual(NumFileSystemEntries, CountFileSystemEntries(tempDirPath));
        }
    }
}
