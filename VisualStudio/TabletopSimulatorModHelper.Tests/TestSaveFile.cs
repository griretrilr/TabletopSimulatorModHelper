using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TabletopSimulatorModHelper.Tests
{
    [TestClass]
    public class TestSaveFile
    {
        [TestMethod]
        public void Test()
        {
            using (var tempDir = new TempDir())
            {
                string saveFilePath = Assembly.TestDataPath("save_file.json");
                string saveFile2Path = Assembly.TestDataPath("save_file_2.json");
                string saveFile3Path = Assembly.TestDataPath("save_file_3_all_empty.json");
                string splitSavePath = Path.Join(tempDir.Path, "split_save");
                string globalScriptPath = Path.Join(splitSavePath, SplitSaveFile.GlobalScriptPath);
                string globalUIPath = Path.Join(splitSavePath, SplitSaveFile.GlobalUIPath);
                string rejoinedSavePath = Path.Join(tempDir.Path, "rejoined_save.json");

                SplitSaveFile.Split(saveFilePath, splitSavePath);
                SplitSaveFile.Join(splitSavePath, rejoinedSavePath);
                Assert.IsTrue(File.ReadAllText(globalScriptPath).StartsWith("--[[ Lua code 1!"));
                Assert.IsTrue(File.ReadAllText(globalUIPath).StartsWith("<!-- Xml UI 1!"));
                Assert.AreEqual(File.ReadAllText(saveFilePath), File.ReadAllText(rejoinedSavePath));

                SplitSaveFile.Split(saveFile2Path, splitSavePath);
                SplitSaveFile.Join(splitSavePath, rejoinedSavePath);
                Assert.IsTrue(File.ReadAllText(globalScriptPath).StartsWith("--[[ Lua code 2!"));
                Assert.IsTrue(File.ReadAllText(globalUIPath).StartsWith("<!-- Xml UI 2!"));
                Assert.AreEqual(File.ReadAllText(saveFile2Path), File.ReadAllText(rejoinedSavePath));

                SplitSaveFile.Split(saveFile3Path, splitSavePath);
                SplitSaveFile.Join(splitSavePath, rejoinedSavePath);
                Assert.IsFalse(File.Exists(globalScriptPath));
                Assert.IsFalse(File.Exists(globalUIPath));
                Assert.AreEqual(File.ReadAllText(saveFile3Path), File.ReadAllText(rejoinedSavePath));
            }
        }
    }
}
