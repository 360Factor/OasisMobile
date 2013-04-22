

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public partial class Exam
    {
        [PrimaryKey, AutoIncrement, Column("pkExamID")]
        public int ExamID {get; set;}
        public string ExamName {get; set;}
        public Boolean IsExpired {get; set;}
        public int Credit {get; set;}
        public double Price {get; set;}
        public double MinimumPassingScore {get; set;}
        public string Disclosure {get; set;}
        public string PrivacyPolicy {get; set;}
        public string Description {get; set;}
        public int MainSystemID {get; set;}

    public void Delete()
    {
        lock(Repository.Locker) {
            Repository.Instance.Delete(this);
        }
    }

    public int Save()
    {
        lock(Repository.Locker) {
            if (this.ExamID != 0) {
                Repository.Instance.Update(this);
                return this.ExamID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<Exam> GetAllExams()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<Exam>() select i).ToList();
        }
    }

    public static Exam GetExamByExamID(int ExamID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<Exam>().Where(x => x.ExamID == ExamID).FirstOrDefault();
        }
    }

    public static List<Exam> GetExamsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Exam>(sql).ToList();
        }
    }

    }

}

