using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace TabletopSimulatorModHelper.Tests
{
    [TestClass]
    public class TestAssembly
    {
        const string ProjectName = "TabletopSimulatorModHelper.Tests";
        const string TestDataDirName = "TestData";
        const string DummyFileName = "dummy.txt";

        [TestMethod]
        public void TestName()
        {
            Assert.AreEqual(Assembly.Name, ProjectName);
        }

        [TestMethod]
        public void TestProjectDir()
        {
            Assert.IsTrue(Directory.Exists(Assembly.ProjectDir));
            Assert.AreEqual(Path.GetFileName(Assembly.ProjectDir), ProjectName);
        }

        [TestMethod]
        public void TestTestDataDir()
        {
            Assert.IsTrue(Directory.Exists(Assembly.TestDataDir));
            Assert.AreEqual(Path.GetFileName(Assembly.TestDataDir), TestDataDirName);
        }

        [TestMethod]
        public void TestTestDataPath()
        {
            Assert.IsTrue(File.Exists(Assembly.TestDataPath(DummyFileName)));
            Assert.AreEqual(Path.GetFileName(Assembly.TestDataPath(DummyFileName)), DummyFileName);
        }
    }
}
