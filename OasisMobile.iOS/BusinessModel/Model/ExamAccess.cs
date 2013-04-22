

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public class ExamAccess
    {
        [PrimaryKey, AutoIncrement, Column("pkExamAccessID")]
        public int ExamAccessID {get; set;}
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

    public static List<ExamAccess> GetExamAccesssBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<ExamAccess>(sql).ToList();
        }
    }

    }

}

