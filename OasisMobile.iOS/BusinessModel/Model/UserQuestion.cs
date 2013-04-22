

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public class UserQuestion
    {
        [PrimaryKey, AutoIncrement, Column("pkUserQuestionID")]
        public int UserQuestionID {get; set;}
        [Column("fkQuestionID")]
        public int QuestionID {get; set;}
        [Column("fkUserExamID")]
        public int UserExamID {get; set;}
        public int Sequence {get; set;}
        public DateTime? AnsweredDateTime {get; set;}
        public Boolean HasAnswered {get; set;}
        public Boolean HasAnsweredCorrectly {get; set;}
        public int SecondsSpent {get; set;}
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
            if (this.UserQuestionID != 0) {
                Repository.Instance.Update(this);
                return this.UserQuestionID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<UserQuestion> GetAllUserQuestions()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<UserQuestion>() select i).ToList();
        }
    }

    public static UserQuestion GetUserQuestionByUserQuestionID(int UserQuestionID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<UserQuestion>().Where(x => x.UserQuestionID == UserQuestionID).FirstOrDefault();
        }
    }

    public static List<UserQuestion> GetUserQuestionsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<UserQuestion>(sql).ToList();
        }
    }

    }

}

