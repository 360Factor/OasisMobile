

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    public partial class Question
    {
        [PrimaryKey, AutoIncrement, Column("pkQuestionID")]
        public int QuestionID {get; set;}
        [Ignore]
        public bool IsNew {get {return QuestionID == 0;}}
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

    public Question() {}

    public Question(string NewStem, 
                  string NewLeadIn, 
                  string NewCommentary, 
                  string NewReference, 
                  int NewCategoryID, 
                  int NewExamID, 
                  double? NewPopulationCorrectPct, 
                  int NewMainSystemID)
    {
              Stem = NewStem;
                 LeadIn = NewLeadIn;
                 Commentary = NewCommentary;
                 Reference = NewReference;
                 CategoryID = NewCategoryID;
                 ExamID = NewExamID;
                 PopulationCorrectPct = NewPopulationCorrectPct;
                 MainSystemID = NewMainSystemID;

    }

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
            string _sql = string.Format("select * from Question where pkQuestionID = {0}", QuestionID);
            return GetFirstQuestionBySQL(_sql);
        }
    }

    public static List<Question> GetQuestionsByQuestionIDs(List<int> QuestionIDs)
    {
        if (QuestionIDs == null || QuestionIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from Question where pkQuestionID in ({0})", string.Join(",", QuestionIDs.ToArray()));

        return GetQuestionsBySQL(_sql);;
    }

    public static List<Question> GetQuestionsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Question>(sql).ToList();
        }
    }

    public static Question GetFirstQuestionBySQL(string sql)
    {
        lock(Repository.Locker) {
            List<Question> _matches = GetQuestionsBySQL(sql);

            if (_matches == null || _matches.Count == 0)
                return null;
            else
                return _matches[0];
        }
    }


        public static void SaveAll(List<Question> Questions)
        {
            lock (Repository.Locker)
            {
                List<Question> _newQuestions = new List<Question>();
                List<Question> _existingQuestions = new List<Question>();

                foreach (Question _Question in Questions)
                {
                    if (_Question.IsNew)
                        _newQuestions.Add(_Question);
                    else
                        _existingQuestions.Add(_Question);
                }

                Repository.Instance.InsertAll(_newQuestions);
                Repository.Instance.UpdateAll(_existingQuestions);
            }
        }

    public static List<Question> GetQuestionsByExamID(int ExamID)
    {
        string _sql = "select * from Question where fkExamID = " + ExamID;
        return GetQuestionsBySQL(_sql);
    }

    public static List<Question> GetQuestionsByCategoryID(int CategoryID)
    {
        string _sql = "select * from Question where fkCategoryID = " + CategoryID;
        return GetQuestionsBySQL(_sql);
    }

        public static Question GetQuestionByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from Question where MainSystemID = " + TargetMainSystemID;
            return GetFirstQuestionBySQL(_sql);
        }
    }

}

