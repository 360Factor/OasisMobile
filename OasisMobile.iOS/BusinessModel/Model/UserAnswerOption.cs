

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BussinessLogicLayer
{

    public partial class UserAnswerOption
    {
        [PrimaryKey, AutoIncrement, Column("pkUserAnswerOptionID")]
        public int UserAnswerOptionID {get; set;}
        [Ignore]
        public bool IsNew {get {return UserAnswerOptionID == 0;}}
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

    public static List<UserAnswerOption> GetUserAnswerOptionsByUserAnswerOptionIDs(List<int> UserAnswerOptionIDs)
    {
        if (UserAnswerOptionIDs == null || UserAnswerOptionIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from UserAnswerOption where pkUserAnswerOptionID in ({0})", string.Join(",", UserAnswerOptionIDs.ToArray()));

        return GetUserAnswerOptionsBySQL(_sql);;
    }

    public static List<UserAnswerOption> GetUserAnswerOptionsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<UserAnswerOption>(sql).ToList();
        }
    }


        public static void SaveAll(List<UserAnswerOption> UserAnswerOptions)
        {
            lock (Repository.Locker)
            {
                List<UserAnswerOption> _newUserAnswerOptions = new List<UserAnswerOption>();
                List<UserAnswerOption> _existingUserAnswerOptions = new List<UserAnswerOption>();

                foreach (UserAnswerOption _UserAnswerOption in UserAnswerOptions)
                {
                    if (_UserAnswerOption.IsNew)
                        _newUserAnswerOptions.Add(_UserAnswerOption);
                    else
                        _existingUserAnswerOptions.Add(_UserAnswerOption);
                }

                Repository.Instance.InsertAll(_newUserAnswerOptions);
                Repository.Instance.UpdateAll(_existingUserAnswerOptions);
            }
        }

    public static List<UserAnswerOption> GetUserAnswerOptionsByAnswerOptionID(int AnswerOptionID)
    {
        string _sql = "select * from UserAnswerOption where fkAnswerOptionID = " + AnswerOptionID;
        return GetUserAnswerOptionsBySQL(_sql);
    }

    public static List<UserAnswerOption> GetUserAnswerOptionsByUserQuestionID(int UserQuestionID)
    {
        string _sql = "select * from UserAnswerOption where fkUserQuestionID = " + UserQuestionID;
        return GetUserAnswerOptionsBySQL(_sql);
    }

        public static UserAnswerOption GetUserAnswerOptionByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from UserAnswerOption where MainSystemID = " + TargetMainSystemID;
            List<UserAnswerOption> _UserAnswerOptions = GetUserAnswerOptionsBySQL(_sql);

            if (_UserAnswerOptions == null || _UserAnswerOptions.Count == 0)
                return null;
            else
                return _UserAnswerOptions[0];
        }
    }

}

