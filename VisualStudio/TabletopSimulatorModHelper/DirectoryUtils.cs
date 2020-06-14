using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Threading;

namespace TabletopSimulatorModHelper
{
    public static class DirectoryUtils
    {
        private static void RecursiveDeleteAttempt(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                RecursiveDeleteAttempt(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        public static void RecursiveDelete(string path)
        {
            RecursiveDelete(path, TimeSpan.FromSeconds(1.0));
        }

        public static void RecursiveDelete(string path, TimeSpan timeout)
        {
            RecursiveDelete(path, timeout, TimeSpan.FromMilliseconds(100.0));
        }

        public static void RecursiveDelete(string path, TimeSpan timeout, TimeSpan retryDelay)
        {
            DateTime endTime = DateTime.Now + timeout;
            Exception exception;
            bool firstRun = true;

            do
            {
                if (!firstRun)
                {
                    Thread.Sleep(retryDelay);
                    firstRun = false;
                }

                exception = null;
                try
                {
                    RecursiveDeleteAttempt(path);
                }
                catch (IOException e)
                {
                    exception = e;
                }
                catch (UnauthorizedAccessException e)
                {
                    exception = e;
                }
            }
            while (DateTime.Now < endTime && exception != null);

            if (exception != null)
                throw exception;
        }
    }
}
