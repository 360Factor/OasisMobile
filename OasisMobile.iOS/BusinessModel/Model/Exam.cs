

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
        [Ignore]
        public bool IsNew {get {return ExamID == 0;}}
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

    public static List<Exam> GetExamsByExamIDs(List<int> ExamIDs)
    {
        if (ExamIDs == null || ExamIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from Exam where pkExamID in ({0})", string.Join(",", ExamIDs.ToArray()));

        return GetExamsBySQL(_sql);;
    }

    public static List<Exam> GetExamsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Exam>(sql).ToList();
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

        public static Exam GetExamByExamName(int TargetExamName)
        {
            string _sql = "select * from Exam where ExamName = " + TargetExamName;
            List<Exam> _Exams = GetExamsBySQL(_sql);

            if (_Exams == null || _Exams.Count == 0)
                return null;
            else
                return _Exams[0];
        }

        public static Exam GetExamByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from Exam where MainSystemID = " + TargetMainSystemID;
            List<Exam> _Exams = GetExamsBySQL(_sql);

            if (_Exams == null || _Exams.Count == 0)
                return null;
            else
                return _Exams[0];
        }
    }

}

