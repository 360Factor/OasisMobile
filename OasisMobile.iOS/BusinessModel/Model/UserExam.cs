

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public class UserExam
    {
        [PrimaryKey, AutoIncrement, Column("pkUserExamID")]
        public int UserExamID {get; set;}
        [Column("fkUserID")]
        public int UserID {get; set;}
        [Column("fkExamID")]
        public int ExamID {get; set;}
        public Boolean IsCompleted {get; set;}
        public Boolean IsSubmitted {get; set;}
        public Boolean IsLearningMode {get; set;}
        public Boolean HasReadDisclosure {get; set;}
        public Boolean HasReadPrivacyPolicy {get; set;}
        public int SecondsSpent {get; set;}
        public Boolean IsDownloaded {get; set;}
        public Boolean DoSync {get; set;}
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
            if (this.UserExamID != 0) {
                Repository.Instance.Update(this);
                return this.UserExamID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<UserExam> GetAllUserExams()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<UserExam>() select i).ToList();
        }
    }

    public static UserExam GetUserExamByUserExamID(int UserExamID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<UserExam>().Where(x => x.UserExamID == UserExamID).FirstOrDefault();
        }
    }

    public static List<UserExam> GetUserExamsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<UserExam>(sql).ToList();
        }
    }

    }

}

