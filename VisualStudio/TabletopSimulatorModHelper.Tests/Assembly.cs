using System.IO;

namespace TabletopSimulatorModHelper.Tests
{
    public static class Assembly
    {
        public static string Name
        {
            get
            {
                if (m_Name == null)
                {
                    m_Name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                }
                return m_Name;
            }
        }
        private static string m_Name;

        public static string ProjectDir
        {
            get
            {
                if (m_ProjectDir == null)
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
                    int index = currentDirectory.IndexOf(Name);
                    if (index < 0)
                    {
                        throw new System.Exception();
                    }
                    m_ProjectDir = currentDirectory.Substring(0, index + Name.Length);
                }
                return m_ProjectDir;
            }
        }
        private static string m_ProjectDir;

        public static string TestDataDir
        {
            get 
            { 
                if (m_TestDataDir == null)
                {
                    m_TestDataDir = Path.Join(ProjectDir, "TestData");
                }
                return m_TestDataDir; 
            }
        }
        private static string m_TestDataDir;

        public static string TestDataPath(string relativePath)
        {
            return Path.Join(TestDataDir, relativePath);
        }
    }
}
