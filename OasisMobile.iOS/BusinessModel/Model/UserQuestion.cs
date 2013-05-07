

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    public partial class UserQuestion
    {
        [PrimaryKey, AutoIncrement, Column("pkUserQuestionID")]
        public int UserQuestionID {get; set;}
        [Ignore]
        public bool IsNew {get {return UserQuestionID == 0;}}
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

    public UserQuestion() {}

    public UserQuestion(int NewQuestionID, 
                  int NewUserExamID, 
                  int NewSequence, 
                  DateTime? NewAnsweredDateTime, 
                  Boolean NewHasAnswered, 
                  Boolean NewHasAnsweredCorrectly, 
                  int NewSecondsSpent, 
                  Boolean NewDoSync, 
                  int NewMainSystemID)
    {
              QuestionID = NewQuestionID;
                 UserExamID = NewUserExamID;
                 Sequence = NewSequence;
                 AnsweredDateTime = NewAnsweredDateTime;
                 HasAnswered = NewHasAnswered;
                 HasAnsweredCorrectly = NewHasAnsweredCorrectly;
                 SecondsSpent = NewSecondsSpent;
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
            string _sql = string.Format("select * from UserQuestion where pkUserQuestionID = {0}", UserQuestionID);
            return GetFirstUserQuestionBySQL(_sql);
        }
    }

    public static List<UserQuestion> GetUserQuestionsByUserQuestionIDs(List<int> UserQuestionIDs)
    {
        if (UserQuestionIDs == null || UserQuestionIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from UserQuestion where pkUserQuestionID in ({0})", string.Join(",", UserQuestionIDs.ToArray()));

        return GetUserQuestionsBySQL(_sql);;
    }

    public static List<UserQuestion> GetUserQuestionsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<UserQuestion>(sql).ToList();
        }
    }

    public static UserQuestion GetFirstUserQuestionBySQL(string sql)
    {
        lock(Repository.Locker) {
            List<UserQuestion> _matches = GetUserQuestionsBySQL(sql);

            if (_matches == null || _matches.Count == 0)
                return null;
            else
                return _matches[0];
        }
    }


        public static void SaveAll(List<UserQuestion> UserQuestions)
        {
            lock (Repository.Locker)
            {
                List<UserQuestion> _newUserQuestions = new List<UserQuestion>();
                List<UserQuestion> _existingUserQuestions = new List<UserQuestion>();

                foreach (UserQuestion _UserQuestion in UserQuestions)
                {
                    if (_UserQuestion.IsNew)
                        _newUserQuestions.Add(_UserQuestion);
                    else
                        _existingUserQuestions.Add(_UserQuestion);
                }

                Repository.Instance.InsertAll(_newUserQuestions);
                Repository.Instance.UpdateAll(_existingUserQuestions);
            }
        }

        public static UserQuestion GetUserQuestionByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from UserQuestion where MainSystemID = " + TargetMainSystemID;
            return GetFirstUserQuestionBySQL(_sql);
        }
    }

}

