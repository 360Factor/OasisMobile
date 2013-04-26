

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BussinessLogicLayer
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

        public static UserExam GetUserExamByfkUserID(int TargetfkUserID)
        {
            string _sql = "select * from UserExam where fkUserID = " + TargetfkUserID;
            List<UserExam> _UserExams = GetUserExamsBySQL(_sql);

            if (_UserExams == null || _UserExams.Count == 0)
                return null;
            else
                return _UserExams[0];
        }

        public static UserExam GetUserExamByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from UserExam where MainSystemID = " + TargetMainSystemID;
            List<UserExam> _UserExams = GetUserExamsBySQL(_sql);

            if (_UserExams == null || _UserExams.Count == 0)
                return null;
            else
                return _UserExams[0];
        }
    }

}

