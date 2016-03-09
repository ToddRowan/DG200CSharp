using System;
using System.IO;

namespace kimandtodd.DG200CSharp.logging
{
    public class DG200FileLogger // : DG200LogBase
    {
        private static string filePath;
        private static int logLevel = -1;
        private static bool isInit = false;

        private DG200FileLogger() : base()
        {
            
        }

        private static void init ()
        {
            if (!DG200FileLogger.isInit)
            {
                DateTime dt = DateTime.Now;
                string format = "yyyy-dd-MM_HH-mm";
                DG200FileLogger.filePath = dt.ToString(format) + ".txt";
                DG200FileLogger.isInit = true;
            }
        }


        public static void Log(string message, int level)
        {
            if (level <= DG200FileLogger.logLevel)
            {
                DG200FileLogger.init();

                using (StreamWriter streamWriter = new StreamWriter(DG200FileLogger.filePath, true))
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
            }
        }

        public static void setLevel(int level)
        {
            DG200FileLogger.logLevel = level;
        }

        public static void setPath(string path)
        {
            DG200FileLogger.filePath = path;
        }
    }
}
