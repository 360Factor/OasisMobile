

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public class Question
    {
        [PrimaryKey, AutoIncrement, Column("pkQuestionID")]
        public int QuestionID {get; set;}
        public string Stem {get; set;}
        public string LeadIn {get; set;}
        public string Commentary {get; set;}
        public string Reference {get; set;}
        [Column("fkCategoryID")]
        public int CategoryID {get; set;}
        [Column("fkExamID")]
        public int ExamID {get; set;}
        public double? PopulationCorrectPct {get; set;}
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
            if (this.QuestionID != 0) {
                Repository.Instance.Update(this);
                return this.QuestionID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<Question> GetAllQuestions()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<Question>() select i).ToList();
        }
    }

    public static Question GetQuestionByQuestionID(int QuestionID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<Question>().Where(x => x.QuestionID == QuestionID).FirstOrDefault();
        }
    }

    public static List<Question> GetQuestionsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Question>(sql).ToList();
        }
    }

    }

}

