

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public class UserAnswerOption
    {
        [PrimaryKey, AutoIncrement, Column("pkUserAnswerOptionID")]
        public int UserAnswerOptionID {get; set;}
        [Column("fkUserQuestionID")]
        public int UserQuestionID {get; set;}
        [Column("fkAnswerOptionID")]
        public int AnswerOptionID {get; set;}
        public int Sequence {get; set;}
        public Boolean IsSelected {get; set;}
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
            if (this.UserAnswerOptionID != 0) {
                Repository.Instance.Update(this);
                return this.UserAnswerOptionID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<UserAnswerOption> GetAllUserAnswerOptions()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<UserAnswerOption>() select i).ToList();
        }
    }

    public static UserAnswerOption GetUserAnswerOptionByUserAnswerOptionID(int UserAnswerOptionID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<UserAnswerOption>().Where(x => x.UserAnswerOptionID == UserAnswerOptionID).FirstOrDefault();
        }
    }

    public static List<UserAnswerOption> GetUserAnswerOptionsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<UserAnswerOption>(sql).ToList();
        }
    }

    }

}

