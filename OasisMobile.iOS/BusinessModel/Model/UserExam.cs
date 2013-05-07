

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    public partial class UserExam
    {
        [PrimaryKey, AutoIncrement, Column("pkUserExamID")]
        public int UserExamID {get; set;}
        [Ignore]
        public bool IsNew {get {return UserExamID == 0;}}
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

    public UserExam() {}

    public UserExam(int NewUserID, 
                  int NewExamID, 
                  Boolean NewIsCompleted, 
                  Boolean NewIsSubmitted, 
                  Boolean NewIsLearningMode, 
                  Boolean NewHasReadDisclosure, 
                  Boolean NewHasReadPrivacyPolicy, 
                  int NewSecondsSpent, 
                  Boolean NewIsDownloaded, 
                  Boolean NewDoSync, 
                  int NewMainSystemID)
    {
              UserID = NewUserID;
                 ExamID = NewExamID;
                 IsCompleted = NewIsCompleted;
                 IsSubmitted = NewIsSubmitted;
                 IsLearningMode = NewIsLearningMode;
                 HasReadDisclosure = NewHasReadDisclosure;
                 HasReadPrivacyPolicy = NewHasReadPrivacyPolicy;
                 SecondsSpent = NewSecondsSpent;
                 IsDownloaded = NewIsDownloaded;
                 DoSync = NewDoSync;
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
            string _sql = string.Format("select * from UserExam where pkUserExamID = {0}", UserExamID);
            return GetFirstUserExamBySQL(_sql);
        }
    }

    public static List<UserExam> GetUserExamsByUserExamIDs(List<int> UserExamIDs)
    {
        if (UserExamIDs == null || UserExamIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from UserExam where pkUserExamID in ({0})", string.Join(",", UserExamIDs.ToArray()));

        return GetUserExamsBySQL(_sql);;
    }

    public static List<UserExam> GetUserExamsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<UserExam>(sql).ToList();
        }
    }

    public static UserExam GetFirstUserExamBySQL(string sql)
    {
        lock(Repository.Locker) {
            List<UserExam> _matches = GetUserExamsBySQL(sql);

            if (_matches == null || _matches.Count == 0)
                return null;
            else
                return _matches[0];
        }
    }


        public static void SaveAll(List<UserExam> UserExams)
        {
            lock (Repository.Locker)
            {
                List<UserExam> _newUserExams = new List<UserExam>();
                List<UserExam> _existingUserExams = new List<UserExam>();

                foreach (UserExam _UserExam in UserExams)
                {
                    if (_UserExam.IsNew)
                        _newUserExams.Add(_UserExam);
                    else
                        _existingUserExams.Add(_UserExam);
                }

                Repository.Instance.InsertAll(_newUserExams);
                Repository.Instance.UpdateAll(_existingUserExams);
            }
        }

    public static List<UserExam> GetUserExamsByExamID(int ExamID)
    {
        string _sql = "select * from UserExam where fkExamID = " + ExamID;
        return GetUserExamsBySQL(_sql);
    }

    public static List<UserExam> GetUserExamsByUserID(int UserID)
    {
        string _sql = "select * from UserExam where fkUserID = " + UserID;
        return GetUserExamsBySQL(_sql);
    }

        public static UserExam GetUserExamByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from UserExam where MainSystemID = " + TargetMainSystemID;
            return GetFirstUserExamBySQL(_sql);
        }
    }

}

