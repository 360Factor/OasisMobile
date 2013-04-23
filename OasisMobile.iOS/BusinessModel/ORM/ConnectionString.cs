
using System;

namespace OasisMobile
{
	public class ConnectionString
	{
        private static string _dbPath = GetDefaultDBPath();
		public static string DBPath { get { return _dbPath; }}

        private static string GetDefaultDBPath()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = System.IO.Path.Combine(documentsPath, "../Library/"); // Library folder

            return libraryPath + "OasisMobile.db3";
        }

		public static void SetDBPath(string DBPath)
		{
			_dbPath = DBPath;
		}
	}
}