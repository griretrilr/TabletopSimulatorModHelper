using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace TabletopSimulatorModHelper
{
    public class SplitSaveFile
    {
        public static readonly string ScriptExtension = ".lua";
        public static readonly string TabStateFileExtension = ".bb.txt";
        public static readonly string UIExtension = ".ui.xml";

        public static readonly string SplitSavePath = $"Save.split-json";

        public static readonly string ScriptsPath = "Scripts";
        public static readonly string GlobalScriptPath = $"{ScriptsPath}/Global{ScriptExtension}";
        public static readonly string ObjectScriptPathFormat = $"{ScriptsPath}/{{0}}.{{1}}{ScriptExtension}";

        public static readonly string UIPath = "UI";
        public static readonly string GlobalUIPath = $"{UIPath}/Global{UIExtension}";
        public static readonly string ObjectUIPathFormat = $"{UIPath}/{{0}}.{{1}}{UIExtension}";

        public static readonly string TabStatesPath = "Notebook";
        public static readonly string TabStateFilePathFormat = $"{TabStatesPath}/{{0}}.{{1}}{TabStateFileExtension}";

        public static readonly string SplitReferenceIndicator = "[TSMH-INCLUDE]";

        public static string PathToSplitReference(string path)
        {
            return $"{SplitReferenceIndicator}{path}";
        }

        public static bool SplitReferenceToPath(string splitReference, out string path)
        {
            if (!splitReference.StartsWith(SplitReferenceIndicator))
            {
                path = null;
                return false;
            }
            path = splitReference.Substring(SplitReferenceIndicator.Length);
            return true;
        }

        private static void Clean(string splitDir)
        {
            FileUtils.Delete(Path.Join(splitDir, SplitSavePath));

            try
            {
                foreach (string file in Directory.GetFiles(Path.Join(splitDir, ScriptsPath), $"*{ScriptExtension}"))
                {
                    FileUtils.Delete(file);
                }
            }
            catch (DirectoryNotFoundException) { }

            try
            {
                foreach (string file in Directory.GetFiles(Path.Join(splitDir, UIPath), $"*{UIExtension}"))
                {
                    FileUtils.Delete(file);
                }
            }
            catch (DirectoryNotFoundException) { }

            try
            {
                foreach (string file in Directory.GetFiles(Path.Join(splitDir, TabStatesPath), $"*{TabStateFileExtension}"))
                {
                    FileUtils.Delete(file);
                }
            }
            catch (DirectoryNotFoundException) { }
        }

        public static void Split(string fromSaveFile, string toDir)
        {
            Clean(toDir);

            JObject saveFile = LoadJson(fromSaveFile);

            Split((JValue)saveFile[SaveFormat.LuaScriptKey], toDir, GlobalScriptPath);

            Split((JValue)saveFile[SaveFormat.UIKey], toDir, GlobalUIPath);

            JObject tabStates = (JObject)saveFile[SaveFormat.TabStatesKey];
            if (tabStates != null)
            {
                foreach (var tabStatePair in tabStates)
                {
                    SplitTabStatePair(tabStatePair, toDir);
                }
            }

            JArray objects = (JArray)saveFile[SaveFormat.ObjectsKey];
            if (objects != null)
            {
                foreach (JObject obj in objects)
                {
                    SplitObject(obj, toDir);
                }
            }

            SaveJson(saveFile, Path.Join(toDir, SplitSavePath));
        }

        private static void SplitTabStatePair(KeyValuePair<string, JToken> pair, string toDir)
        {
            string key = pair.Key;
            JObject tabState = (JObject)pair.Value;
            string title = (string)((JValue)tabState[SaveFormat.TabStateTitleKey]).Value;
            string filename = string.Format(TabStateFilePathFormat, key, title);
            Split((JValue)tabState[SaveFormat.TabStateBodyKey], toDir, filename);
        }

        private static void SplitObject(JObject obj, string toDir)
        {
            string name = (string)((JValue)obj[SaveFormat.ObjectNameKey]).Value;
            string guid = (string)((JValue)obj[SaveFormat.ObjectGuidKey]).Value;
            string scriptPath = string.Format(ObjectScriptPathFormat, name, guid);
            string uiPath = string.Format(ObjectUIPathFormat, name, guid);
            Split((JValue)obj[SaveFormat.LuaScriptKey], toDir, scriptPath);
            Split((JValue)obj[SaveFormat.UIKey], toDir, uiPath);
        }

        private static void Split(JValue value, string toDir, string toPath)
        {
            if (value == null || value.Type != JTokenType.String || string.IsNullOrWhiteSpace((string)value.Value))
            {
                return;
            }
            WriteText(Path.Join(toDir, toPath), (string)value.Value);
            value.Value = PathToSplitReference(toPath);
        }

        public static void Join(string fromDir, string toSaveFile)
        {
            JObject saveFile = LoadJson(Path.Join(fromDir, SplitSavePath));
            Join(saveFile, fromDir);
            SaveJson(saveFile, toSaveFile);
        }

        private static void Join(JToken token, string basePath)
        {
            switch (token)
            {
                case JObject obj:
                    {
                        foreach (var pair in obj)
                        {
                            Join(pair.Value, basePath);
                        }
                    }
                    break;
                case JArray arr:
                    {
                        foreach (var item in arr)
                        {
                            Join(item, basePath);
                        }
                    }
                    break;
                case JValue value:
                    {
                        if (value.Type == JTokenType.String && SplitReferenceToPath((string)value.Value, out string path))
                        {
                            value.Value = File.ReadAllText(Path.Join(basePath, path));
                        }
                    }
                    break;
            }
        }

        private static JObject LoadJson(string path)
        {
            return (JObject)JsonConvert.DeserializeObject(File.ReadAllText(path));
        }

        private static void WriteText(string path, string text)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, text);
        }

        private static void SaveJson(JObject json, string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, JsonConvert.SerializeObject(json, Formatting.Indented));
        }
    }
}
