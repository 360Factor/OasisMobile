
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OasisMobile.BusinessModel
{
    public static class StringExtensions
    {
        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            int startIndex = 0;
            while (true)
            {
                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1)
                    break;

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }

    }
    
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

        private class IntIntWrapper
        {
            public int IntObj1 { get; set; }
            public int IntObj2 { get; set; }
        }

        private class StringStringWrapper
        {
            public string StringObj1 { get; set; }
            public string StringObj2 { get; set; }
        }

        private class IntStringWrapper
        {
            public int IntObj1 { get; set; }
            public string StringObj2 { get; set; }
        }

        private class StringIntWrapper
        {
            public string StringObj1 { get; set; }
            public int IntObj2 { get; set; }
        }

        private class TupleWrapper
        {
            public string StringTuple1 { get; set; }
            public string StringTuple2 { get; set; }
            public string StringTuple3 { get; set; }
            public string StringTuple4 { get; set; }
            public string StringTuple5 { get; set; }
            public string StringTuple6 { get; set; }
            public string StringTuple7 { get; set; }
        }
        #endregion

        public static T ExecuteScalar<T>(string query, params object[] args)
        {
            return Repository.Instance.ExecuteScalar<T>(query, args);
        }

        public static List<int> ExecuteIntList(string query, params object[] args)
        {
            query = query.Replace(" from ", " as IntObj from ", StringComparison.CurrentCultureIgnoreCase);

            List<IntWrapper> _data = Repository.Instance.Query<IntWrapper>(query, args);

            return (from _d in _data select _d.IntObj).ToList<int>();
        }

        public static Dictionary<int, int> ExecuteIntIntDictionary(string query, params object[] args)
        {
            // we are converting something like: select a, b from c
            query = query.Replace(",", " as IntObj1,");
            query = query.Replace(" from ", " as IntObj2 from ", StringComparison.CurrentCultureIgnoreCase);
            
            List<IntIntWrapper> _data = Repository.Instance.Query<IntIntWrapper>(query, args);

            return (from _d in _data select _d).ToDictionary(x => x.IntObj1, x => x.IntObj2);
        }

        public static Dictionary<string, string> ExecuteStringStringDictionary(string query, params object[] args)
        {
            // we are converting something like: select a, b from c
            query = query.Replace(",", " as StringObj1,");
            query = query.Replace(" from ", " as StringObj2 from ", StringComparison.CurrentCultureIgnoreCase);

            List<StringStringWrapper> _data = Repository.Instance.Query<StringStringWrapper>(query, args);

            return (from _d in _data select _d).ToDictionary(x => x.StringObj1, x => x.StringObj2);
        }

        public static Dictionary<int, string> ExecuteIntStringDictionary(string query, params object[] args)
        {
            // we are converting something like: select a, b from c
            query = query.Replace(",", " as IntObj1,");
            query = query.Replace(" from ", " as StringObj2 from ", StringComparison.CurrentCultureIgnoreCase);

            List<IntStringWrapper> _data = Repository.Instance.Query<IntStringWrapper>(query, args);

            return (from _d in _data select _d).ToDictionary(x => x.IntObj1, x => x.StringObj2);
        }

        public static Dictionary<string, int> ExecuteStringIntDictionary(string query, params object[] args)
        {
            // we are converting something like: select a, b from c
            query = query.Replace(",", " as StringObj1,");
            query = query.Replace(" from ", " as IntObj2 from ", StringComparison.CurrentCultureIgnoreCase);

            List<StringIntWrapper> _data = Repository.Instance.Query<StringIntWrapper>(query, args);

            return (from _d in _data select _d).ToDictionary(x => x.StringObj1, x => x.IntObj2);
        }

        public static List<string> ExecuteStringList(string query, params object[] args)
        {
            query = query.Replace(" from ", " as StringObj from ", StringComparison.CurrentCultureIgnoreCase);

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

        #region Tuples

        public static List<Tuple<string, string>> Get2Tuples(string query, params object[] args)
        {
            List<Tuple<string, string>> _2tuples = new List<Tuple<string, string>>();

            //select a, b from c ==> select a as StringTuple1, b as StringTuple2 from c
            Regex _regex = new Regex("select (.+) from (.+)", RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("select {0} as StringTuple1, {1} as StringTuple2 from {2}", _returnTokens[0], _returnTokens[1], _afterFrom);

                List<TupleWrapper> _data = Repository.Instance.Query<TupleWrapper>(_newQuery);

                foreach (TupleWrapper _tuple in _data)
                {
                    _2tuples.Add(new Tuple<string, string>(_tuple.StringTuple1, _tuple.StringTuple2));
                }

                return _2tuples;
            }
            else
            {
                throw new Exception("Unable to project tuple wrapper against query " + query);
            }
        }

        public static List<Tuple<string, string, string>> Get3Tuples(string query, params object[] args)
        {
            List<Tuple<string, string, string>> _3tuples = new List<Tuple<string, string, string>>();

            //select a, b from c ==> select a as StringTuple1, b as StringTuple2 from c
            Regex _regex = new Regex("select (.+) from (.+)", RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = 
                    string.Format("select {0} as StringTuple1, {1} as StringTuple2, {2} as StringTuple3 from {3}", 
                                    _returnTokens[0],
                                    _returnTokens[1],
                                    _returnTokens[2], 
                                    _afterFrom);

                List<TupleWrapper> _data = Repository.Instance.Query<TupleWrapper>(_newQuery);

                foreach (TupleWrapper _tuple in _data)
                {
                    _3tuples.Add(new Tuple<string, string, string>(_tuple.StringTuple1, _tuple.StringTuple2, _tuple.StringTuple3));
                }

                return _3tuples;
            }
            else
            {
                throw new Exception("Unable to project tuple wrapper against query " + query);
            }
        }

        public static List<Tuple<string, string, string, string>> Get4Tuples(string query, params object[] args)
        {
            List<Tuple<string, string, string, string>> _4tuples = new List<Tuple<string, string, string, string>>();

            //select a, b from c ==> select a as StringTuple1, b as StringTuple2 from c
            Regex _regex = new Regex("select (.+) from (.+)", RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("select {0} as StringTuple1, {1} as StringTuple2, {2} as StringTuple3, {3} as StringTuple4 from {4}", 
                                    _returnTokens[0],
                                    _returnTokens[1],
                                    _returnTokens[2],
                                    _returnTokens[3], 
                                    _afterFrom);

                List<TupleWrapper> _data = Repository.Instance.Query<TupleWrapper>(_newQuery);

                foreach (TupleWrapper _tuple in _data)
                {
                    _4tuples.Add(new Tuple<string, string, string, string>(_tuple.StringTuple1,
                                                                            _tuple.StringTuple2,
                                                                            _tuple.StringTuple3,
                                                                            _tuple.StringTuple4));
                }

                return _4tuples;
            }
            else
            {
                throw new Exception("Unable to project tuple wrapper against query " + query);
            }
        }

        public static List<Tuple<string, string, string, string, string>> Get5Tuples(string query, params object[] args)
        {
            List<Tuple<string, string, string, string, string>> _5tuples = new List<Tuple<string, string, string, string, string>>();

            //select a, b from c ==> select a as StringTuple1, b as StringTuple2 from c
            Regex _regex = new Regex("select (.+) from (.+)", RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("select {0} as StringTuple1, {1} as StringTuple2, {2} as StringTuple3, {3} as StringTuple4, {4} as StringTuple5 from {5}", 
                                                    _returnTokens[0],
                                                    _returnTokens[1],
                                                    _returnTokens[2],
                                                    _returnTokens[3],
                                                    _returnTokens[4], 
                                                    _afterFrom);

                List<TupleWrapper> _data = Repository.Instance.Query<TupleWrapper>(_newQuery);

                foreach (TupleWrapper _tuple in _data)
                {
                    _5tuples.Add(new Tuple<string, string, string, string, string>(_tuple.StringTuple1,
                                                                                    _tuple.StringTuple2,
                                                                                    _tuple.StringTuple3,
                                                                                    _tuple.StringTuple4,
                                                                                    _tuple.StringTuple5));
                }

                return _5tuples;
            }
            else
            {
                throw new Exception("Unable to project tuple wrapper against query " + query);
            }
        }

        public static List<Tuple<string, string, string, string, string, string>> Get6Tuples(string query, params object[] args)
        {
            List<Tuple<string, string, string, string, string, string>> _6tuples = new List<Tuple<string, string, string, string, string, string>>();

            //select a, b from c ==> select a as StringTuple1, b as StringTuple2 from c
            Regex _regex = new Regex("select (.+) from (.+)", RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("select {0} as StringTuple1, {1} as StringTuple2, {2} as StringTuple3, {3} as StringTuple4, {4} as StringTuple5, {5} as StringTuple6 from {6}", 
                                                    _returnTokens[0],
                                                    _returnTokens[1],
                                                    _returnTokens[2],
                                                    _returnTokens[3],
                                                    _returnTokens[4],
                                                    _returnTokens[5],
                                                    _afterFrom);

                List<TupleWrapper> _data = Repository.Instance.Query<TupleWrapper>(_newQuery);

                foreach (TupleWrapper _tuple in _data)
                {
                    _6tuples.Add(new Tuple<string, string, string, string, string, string>(_tuple.StringTuple1,
                                                                                            _tuple.StringTuple2,
                                                                                            _tuple.StringTuple3,
                                                                                            _tuple.StringTuple4,
                                                                                            _tuple.StringTuple5,
                                                                                            _tuple.StringTuple6));
                }

                return _6tuples;
            }
            else
            {
                throw new Exception("Unable to project tuple wrapper against query " + query);
            }
        }

        public static List<Tuple<string, string, string, string, string, string, string>> Get7Tuples(string query, params object[] args)
        {
            List<Tuple<string, string, string, string, string, string, string>> _7tuples = new List<Tuple<string, string, string, string, string, string, string>>();

            //select a, b from c ==> select a as StringTuple1, b as StringTuple2 from c
            Regex _regex = new Regex("select (.+) from (.+)", RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("select {0} as StringTuple1, {1} as StringTuple2, {2} as StringTuple3, {3} as StringTuple4, {4} as StringTuple5, {5} as StringTuple6, {6} as StringTuple7 from {7}",
                                                    _returnTokens[0],
                                                    _returnTokens[1],
                                                    _returnTokens[2],
                                                    _returnTokens[3],
                                                    _returnTokens[4],
                                                    _returnTokens[5],
                                                    _returnTokens[6],
                                                    _afterFrom);

                List<TupleWrapper> _data = Repository.Instance.Query<TupleWrapper>(_newQuery);

                foreach (TupleWrapper _tuple in _data)
                {
                    _7tuples.Add(new Tuple<string, string, string, string, string, string, string>(_tuple.StringTuple1,
                                                                                                    _tuple.StringTuple2,
                                                                                                    _tuple.StringTuple3,
                                                                                                    _tuple.StringTuple4,
                                                                                                    _tuple.StringTuple5,
                                                                                                    _tuple.StringTuple6,
                                                                                                    _tuple.StringTuple7));
                }

                return _7tuples;
            }
            else
            {
                throw new Exception("Unable to project tuple wrapper against query " + query);
            }
        }

        #endregion

        #region Utilities
        public static void TruncateTable(string TableName)
        {
            List<string> _sqls = new List<string>();
            _sqls.Add(string.Format("DELETE FROM {0}", TableName));
            _sqls.Add(string.Format("DELETE FROM SQLITE_SEQUENCE WHERE NAME = '{0}'", TableName));

            SQL.Execute(_sqls);
        }
        #endregion
    }
}
