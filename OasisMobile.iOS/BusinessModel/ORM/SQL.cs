
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OasisMobile.BusinessModel
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
            query = Regex.Replace ( query," FROM ", " FROM IntObj FROM ",RegexOptions.IgnoreCase);

            List<IntWrapper> _data = Repository.Instance.Query<IntWrapper>(query, args);

            return (from _d in _data select _d.IntObj).ToList<int>();
        }

        public static Dictionary<int, int> ExecuteIntIntDictionary(string query, params object[] args)
        {
            // we are converting something like: select a, b from c
            query = query.Replace(",", " AS IntObj1,");
			query = Regex.Replace (query," FROM "," AS IntObj2 FROM ",RegexOptions.IgnoreCase); 

            List<IntIntWrapper> _data = Repository.Instance.Query<IntIntWrapper>(query, args);

            return (from _d in _data select _d).ToDictionary(x => x.IntObj1, x => x.IntObj2);
        }

        public static Dictionary<string, string> ExecuteStringStringDictionary(string query, params object[] args)
        {
            // we are converting something like: select a, b from c
            query = query.Replace(",", " AS StringObj1,");
			query = Regex.Replace (query," FROM ", " AS StringObj2 FROM ", RegexOptions.IgnoreCase);

            List<StringStringWrapper> _data = Repository.Instance.Query<StringStringWrapper>(query, args);

            return (from _d in _data select _d).ToDictionary(x => x.StringObj1, x => x.StringObj2);
        }

        public static Dictionary<int, string> ExecuteIntStringDictionary(string query, params object[] args)
        {
            // we are converting something like: select a, b from c
            query = query.Replace(",", " AS IntObj1,");
			query = Regex.Replace (query," FROM ", " AS StringObj2 FROM ",RegexOptions.IgnoreCase);

            List<IntStringWrapper> _data = Repository.Instance.Query<IntStringWrapper>(query, args);

            return (from _d in _data select _d).ToDictionary(x => x.IntObj1, x => x.StringObj2);
        }

        public static Dictionary<string, int> ExecuteStringIntDictionary(string query, params object[] args)
        {
            // we are converting something like: select a, b from c
            query = query.Replace(",", " AS StringObj1,");
			query = Regex.Replace (query," FROM ", " AS IntObj2 FROM ",RegexOptions.IgnoreCase);

            List<StringIntWrapper> _data = Repository.Instance.Query<StringIntWrapper>(query, args);

            return (from _d in _data select _d).ToDictionary(x => x.StringObj1, x => x.IntObj2);
        }

        public static List<string> ExecuteStringList(string query, params object[] args)
        {
			query = Regex.Replace (query," FROM ", " AS StringObj FROM ",RegexOptions.IgnoreCase);

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
            Regex _regex = new Regex("SELECT (.+) FROM (.+)",RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("SELECT {0} AS StringTuple1, {1} AS StringTuple2 FROM {2}", _returnTokens[0], _returnTokens[1], _afterFrom);

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
            Regex _regex = new Regex("SELECT (.+) FROM (.+)",RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = 
                    string.Format("SELECT {0} AS StringTuple1, {1} AS StringTuple2, {2} AS StringTuple3 FROM {3}", 
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
            Regex _regex = new Regex("SELECT (.+) FROM (.+)",RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("SELECT {0} AS StringTuple1, {1} AS StringTuple2, {2} AS StringTuple3, {3} AS StringTuple4 FROM {4}", 
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
            Regex _regex = new Regex("SELECT (.+) FROM (.+)",RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("SELECT {0} AS StringTuple1, {1} AS StringTuple2, {2} AS StringTuple3, {3} AS StringTuple4, {4} AS StringTuple5 FROM {5}", 
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
            Regex _regex = new Regex("SELECT (.+) FROM (.+)",RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("SELECT {0} AS StringTuple1, {1} AS StringTuple2, {2} AS StringTuple3, {3} AS StringTuple4, {4} AS StringTuple5, {5} AS StringTuple6 FROM {6}", 
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
            Regex _regex = new Regex("SELECT (.+) FROM (.+)",RegexOptions.IgnoreCase);
            Match _match = _regex.Match(query);

            if (_match.Success && _match.Groups.Count == 3)
            {
                List<string> _returnTokens = new List<string>(_match.Groups[1].Value.Split(','));
                string _afterFrom = _match.Groups[2].Value;
                string _newQuery = string.Format("SELECT {0} AS StringTuple1, {1} AS StringTuple2, {2} AS StringTuple3, {3} AS StringTuple4, {4} AS StringTuple5, {5} AS StringTuple6, {6} AS StringTuple7 FROM {7}",
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
