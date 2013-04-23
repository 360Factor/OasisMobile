

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public partial class ExamAccess
    {
        [PrimaryKey, AutoIncrement, Column("pkExamAccessID")]
        public int ExamAccessID {get; set;}
        [Ignore]
        public bool IsNew {get {return ExamAccessID == 0;}}
        [Column("fkUserID")]
        public int UserID {get; set;}
        [Column("fkExamID")]
        public int ExamID {get; set;}
        public Boolean HasAccess {get; set;}

    public void Delete()
    {
        lock(Repository.Locker) {
            Repository.Instance.Delete(this);
        }
    }

    public int Save()
    {
        lock(Repository.Locker) {
            if (this.ExamAccessID != 0) {
                Repository.Instance.Update(this);
                return this.ExamAccessID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<ExamAccess> GetAllExamAccesss()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<ExamAccess>() select i).ToList();
        }
    }

    public static ExamAccess GetExamAccessByExamAccessID(int ExamAccessID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<ExamAccess>().Where(x => x.ExamAccessID == ExamAccessID).FirstOrDefault();
        }
    }

    public static List<ExamAccess> GetExamAccesssByExamAccessIDs(List<int> ExamAccessIDs)
    {
        if (ExamAccessIDs == null || ExamAccessIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from ExamAccess where pkExamAccessID in ({0})", string.Join(",", ExamAccessIDs.ToArray()));

        return GetExamAccesssBySQL(_sql);;
    }

    public static List<ExamAccess> GetExamAccesssBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<ExamAccess>(sql).ToList();
        }
    }


        public static void SaveAll(List<ExamAccess> ExamAccesss)
        {
            lock (Repository.Locker)
            {
                List<ExamAccess> _newExamAccesss = new List<ExamAccess>();
                List<ExamAccess> _existingExamAccesss = new List<ExamAccess>();

                foreach (ExamAccess _ExamAccess in ExamAccesss)
                {
                    if (_ExamAccess.IsNew)
                        _newExamAccesss.Add(_ExamAccess);
                    else
                        _existingExamAccesss.Add(_ExamAccess);
                }

                Repository.Instance.InsertAll(_newExamAccesss);
                Repository.Instance.UpdateAll(_existingExamAccesss);
            }
        }

    public static List<ExamAccess> GetExamAccesssByExamID(int ExamID)
    {
        string _sql = "select * from ExamAccess where fkExamID = " + ExamID;
        return GetExamAccesssBySQL(_sql);
    }

    public static List<ExamAccess> GetExamAccesssByUserID(int UserID)
    {
        string _sql = "select * from ExamAccess where fkUserID = " + UserID;
        return GetExamAccesssBySQL(_sql);
    }
    }

}

