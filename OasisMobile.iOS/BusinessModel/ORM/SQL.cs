
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace OasisMobile
{
    class SQL
    {
        #region Wrapper Class
        private class IntWrapper
        {
            public int IntObj { get; set; }
        }

        private class StringWrapper
        {
            public string StringObj { get; set; }
        }
        #endregion

        public static T ExecuteScalar<T>(string query, params object[] args)
        {
            return Repository.Instance.ExecuteScalar<T>(query, args);
        }

        public static List<int> ExecuteIntList(string query, params object[] args)
        {
            query = query.Replace(" from ", " as IntObj from ");

            List<IntWrapper> _data = Repository.Instance.Query<IntWrapper>(query, args);

            return (from _d in _data select _d.IntObj).ToList<int>();
        }

        public static List<string> ExecuteStringList(string query, params object[] args)
        {
            query = query.Replace(" from ", " as StringObj from ");

            List<StringWrapper> _data = Repository.Instance.Query<StringWrapper>(query, args);

            return (from _d in _data select _d.StringObj).ToList<string>();
        }

        public static int ExecuteNonQuery(string query, params object[] args)
        {
            return Repository.Instance.Execute(query, args);
        }

        public static void Execute(List<string> queries)
        {
            Repository.Instance.BeginTransaction();

            foreach (string query in queries)
            {
                Repository.Instance.Execute(query);
            }

            Repository.Instance.Commit();
        }
    }
}
