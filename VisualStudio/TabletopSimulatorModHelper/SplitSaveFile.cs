using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace TabletopSimulatorModHelper
{
    public class SplitSaveFile
    {
        public static readonly string ScriptsPath = "Scripts";

        public static readonly string SplitSavePath = $"Save.split-json";
        public static readonly string GlobalScriptPath = $"{ScriptsPath}/Global.lua";
        public static readonly string GlobalUIPath = $"UI.xml";

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
            FileUtils.Delete(Path.Join(splitDir, GlobalScriptPath));
            FileUtils.Delete(Path.Join(splitDir, GlobalUIPath));
        }

        public static void Split(string fromSaveFile, string toDir)
        {
            Clean(toDir);

            JObject saveFile = LoadJson(fromSaveFile);
            Split((JValue)saveFile[SaveFormat.LuaScriptKey], toDir, GlobalScriptPath);
            Split((JValue)saveFile[SaveFormat.UIKey], toDir, GlobalUIPath);
            SaveJson(saveFile, Path.Join(toDir, SplitSavePath));
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
