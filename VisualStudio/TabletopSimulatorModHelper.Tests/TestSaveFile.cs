using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                string tabState0Path = Path.Join(splitSavePath, string.Format(SplitSaveFile.TabStateFilePathFormat, "0", "Rules"));
                string tabState2Path = Path.Join(splitSavePath, string.Format(SplitSaveFile.TabStateFilePathFormat, "2", "Brown"));
                string objScriptPath = Path.Join(splitSavePath, string.Format(SplitSaveFile.ObjectScriptPathFormat, "Custom_Board", "26c34b"));
                string objUIPath = Path.Join(splitSavePath, string.Format(SplitSaveFile.ObjectUIPathFormat, "Custom_Board", "26c34b"));
                string rejoinedSavePath = Path.Join(tempDir.Path, "rejoined_save.json");

                SplitSaveFile.Split(saveFilePath, splitSavePath);
                SplitSaveFile.Join(splitSavePath, rejoinedSavePath);
                Assert.IsTrue(File.ReadAllText(globalScriptPath).StartsWith("--[[ Lua code 1!"));
                Assert.IsTrue(File.ReadAllText(globalUIPath).StartsWith("<!-- Xml UI 1!"));
                Assert.AreEqual("Body 0 (Rules)", File.ReadAllText(tabState0Path));
                Assert.AreEqual("Body 2 (Brown)", File.ReadAllText(tabState2Path));
                Assert.AreEqual("--[[ Custom board LUA! --]]", File.ReadAllText(objScriptPath));
                Assert.AreEqual("<!-- Custom board UI! -->", File.ReadAllText(objUIPath));
                Assert.AreEqual(File.ReadAllText(saveFilePath), File.ReadAllText(rejoinedSavePath));

                SplitSaveFile.Split(saveFile2Path, splitSavePath);
                SplitSaveFile.Join(splitSavePath, rejoinedSavePath);
                Assert.IsTrue(File.ReadAllText(globalScriptPath).StartsWith("--[[ Lua code 2!"));
                Assert.IsTrue(File.ReadAllText(globalUIPath).StartsWith("<!-- Xml UI 2!"));
                Assert.IsFalse(File.Exists(tabState0Path));
                Assert.IsFalse(File.Exists(tabState2Path));
                Assert.IsFalse(File.Exists(objScriptPath));
                Assert.IsFalse(File.Exists(objUIPath));
                Assert.AreEqual(File.ReadAllText(saveFile2Path), File.ReadAllText(rejoinedSavePath));

                SplitSaveFile.Split(saveFile3Path, splitSavePath);
                SplitSaveFile.Join(splitSavePath, rejoinedSavePath);
                Assert.IsFalse(File.Exists(globalScriptPath));
                Assert.IsFalse(File.Exists(globalUIPath));
                Assert.IsFalse(File.Exists(tabState0Path));
                Assert.IsFalse(File.Exists(tabState2Path));
                Assert.IsFalse(File.Exists(objScriptPath));
                Assert.IsFalse(File.Exists(objUIPath));
                Assert.AreEqual(File.ReadAllText(saveFile3Path), File.ReadAllText(rejoinedSavePath));
            }
        }
    }
}
