

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    [Table("tblExam")]
    public partial class Exam
    {
        [PrimaryKey, AutoIncrement, Column("pkExamID")]
        public int ExamID {get; set;}
        [Ignore]
        public bool IsNew {get {return ExamID == 0;}}
        public string ExamName {get; set;}
        [Column("fkExamTypeID")]
        public int ExamTypeID {get; set;}
        public Boolean IsExpired {get; set;}
        public int Credit {get; set;}
        public double Price {get; set;}
        public double MinimumPassingScore {get; set;}
        public string Disclosure {get; set;}
        public string PrivacyPolicy {get; set;}
        public string Description {get; set;}
        public int MainSystemID {get; set;}

    public Exam() {}

    public Exam(string NewExamName, 
                  int NewExamTypeID, 
                  Boolean NewIsExpired, 
                  int NewCredit, 
                  double NewPrice, 
                  double NewMinimumPassingScore, 
                  string NewDisclosure, 
                  string NewPrivacyPolicy, 
                  string NewDescription, 
                  int NewMainSystemID)
    {
              ExamName = NewExamName;
                 ExamTypeID = NewExamTypeID;
                 IsExpired = NewIsExpired;
                 Credit = NewCredit;
                 Price = NewPrice;
                 MinimumPassingScore = NewMinimumPassingScore;
                 Disclosure = NewDisclosure;
                 PrivacyPolicy = NewPrivacyPolicy;
                 Description = NewDescription;
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
            string _sql = string.Format("select * from tblExam where pkExamID = {0}", ExamID);
            return GetFirstExamBySQL(_sql);
        }
    }

    public static List<Exam> GetExamsByExamIDs(List<int> ExamIDs)
    {
        if (ExamIDs == null || ExamIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from tblExam where pkExamID in ({0})", string.Join(",", ExamIDs.ToArray()));

        return GetExamsBySQL(_sql);;
    }

    public static List<Exam> GetExamsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Exam>(sql).ToList();
        }
    }

    public static Exam GetFirstExamBySQL(string sql)
    {
        lock(Repository.Locker) {
            List<Exam> _matches = GetExamsBySQL(sql);

            if (_matches == null || _matches.Count == 0)
                return null;
            else
                return _matches[0];
        }
    }


        public static void SaveAll(List<Exam> Exams)
        {
            lock (Repository.Locker)
            {
                List<Exam> _newExams = new List<Exam>();
                List<Exam> _existingExams = new List<Exam>();

                foreach (Exam _Exam in Exams)
                {
                    if (_Exam.IsNew)
                        _newExams.Add(_Exam);
                    else
                        _existingExams.Add(_Exam);
                }

                Repository.Instance.InsertAll(_newExams);
                Repository.Instance.UpdateAll(_existingExams);
            }
        }

    public static List<Exam> GetExamsByExamTypeID(int ExamTypeID)
    {
        string _sql = "select * from tblExam where fkExamTypeID = " + ExamTypeID;
        return GetExamsBySQL(_sql);
    }

        public static Exam GetExamByExamName(string TargetExamName)
        {
            string _sql = "select * from tblExam where ExamName = " + TargetExamName;
            return GetFirstExamBySQL(_sql);
        }

        public static Exam GetExamByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from tblExam where MainSystemID = " + TargetMainSystemID;
            return GetFirstExamBySQL(_sql);
        }
    }

}

