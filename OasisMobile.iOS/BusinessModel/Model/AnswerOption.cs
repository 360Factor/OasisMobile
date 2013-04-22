

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public class AnswerOption
    {
        [PrimaryKey, AutoIncrement, Column("pkAnswerOptionID")]
        public int AnswerOptionID {get; set;}
        [Column("fkQuestionID")]
        public int QuestionID {get; set;}
        public string AnswerText {get; set;}
        public Boolean IsCorrect {get; set;}
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
            if (this.AnswerOptionID != 0) {
                Repository.Instance.Update(this);
                return this.AnswerOptionID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<AnswerOption> GetAllAnswerOptions()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<AnswerOption>() select i).ToList();
        }
    }

    public static AnswerOption GetAnswerOptionByAnswerOptionID(int AnswerOptionID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<AnswerOption>().Where(x => x.AnswerOptionID == AnswerOptionID).FirstOrDefault();
        }
    }

    public static List<AnswerOption> GetAnswerOptionsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<AnswerOption>(sql).ToList();
        }
    }

    }

}

