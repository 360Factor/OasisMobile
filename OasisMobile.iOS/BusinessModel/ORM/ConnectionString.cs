using System;

namespace OasisMobile
{
	public class ConnectionString
	{
		private static string _dbPath;
		public static string DBPath { get { return _dbPath; }}

		public static void SetDBPath(string DBPath)
		{
			_dbPath = DBPath;
		}
	}
}

