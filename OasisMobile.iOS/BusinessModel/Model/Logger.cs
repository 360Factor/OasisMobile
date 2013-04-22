using System;
using System.Diagnostics;
using System.IO;
//using ServiceStack.Text;

namespace BigTed.Utils
{
    public class BTLogger
    {
        public static bool DebugMode = true;

        public static void Dump<T>(T o)
        {
            if (o == null)
            {
                Log("LogObject was NULL");
                return;
            }

            try
            {
                //string json = JsonSerializer.SerializeToString<T>(o);
                //string.Format gets funny with all the {}
                //Log(json.Replace("{", "{{").Replace("}", "}}"));

            }
            catch (Exception ex)
            {
                Log("error logging object");
                LogException(ex);
            }
        }

        static object loggingGate = new object();

        public static void Log(string msg, params object[] param)
        {
            string res = msg;
            if (param.Length > 0)
            {
                res = string.Format(msg, param);
            }

            if (DebugMode)
            {
                Console.WriteLine(res);

                lock (loggingGate)
                {
                    using (StreamWriter sw = File.AppendText(LogFilename))
                    {
                        sw.WriteLine(DateTime.Now.ToString("yyyyMMdd/HHmmss") + ": " + res);
                        sw.Flush();
                        sw.Close();
                    }
                }

            }
        }

        public static string LogContent
        {
            get
            {
                lock (loggingGate)
                {
                    if (!File.Exists(LogFilename))
                        return "No log file present";
                    return File.ReadAllText(LogFilename);
                }
            }
        }

        public static void LogException(Exception ex)
        {
            Log("Exception: " + ex.ToString());
            Log("StackTrace: " + ex.StackTrace.ToString());
            if (ex.InnerException != null)
            {
                LogException(ex.InnerException);
            }
        }

        public static void LogCurrentStack()
        {
            StackTrace st = new StackTrace();
            Log("Stack trace: " + st.ToString());
        }

        public static void CheckLogsForCleanup()
        {

            if (File.Exists(LogFilename))
            {
                FileInfo fi = new FileInfo(LogFilename);
                //BTLogger.Log ("Log file is {0:0} kb", fi.Length / 1024);
                if (fi.Length > (70 * 1024))
                {
                    DeleteLogFile();
                    Log("Log file purged - more than 70k");
                }
            }

        }

        public static void DeleteLogFile()
        {
            lock (loggingGate)
            {
                if (File.Exists(LogFilename))
                {
                    File.Delete(LogFilename);
                }
            }
        }

        public static string LogFilename
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "bigted.log");
            }
        }
    }
}