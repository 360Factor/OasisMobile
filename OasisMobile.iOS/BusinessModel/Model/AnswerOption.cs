

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    [Table("tblAnswerOption")]
    public partial class AnswerOption
    {
        [PrimaryKey, AutoIncrement, Column("pkAnswerOptionID")]
        public int AnswerOptionID {get; set;}
        [Ignore]
        public bool IsNew {get {return AnswerOptionID == 0;}}
        [Column("fkQuestionID")]
        public int QuestionID {get; set;}
        public string AnswerText {get; set;}
        public Boolean IsCorrect {get; set;}
        public int MainSystemID {get; set;}

    public AnswerOption() {}

    public AnswerOption(int NewQuestionID, 
                  string NewAnswerText, 
                  Boolean NewIsCorrect, 
                  int NewMainSystemID)
    {
              QuestionID = NewQuestionID;
                 AnswerText = NewAnswerText;
                 IsCorrect = NewIsCorrect;
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
            string _sql = string.Format("select * from tblAnswerOption where pkAnswerOptionID = {0}", AnswerOptionID);
            return GetFirstAnswerOptionBySQL(_sql);
        }
    }

    public static List<AnswerOption> GetAnswerOptionsByAnswerOptionIDs(List<int> AnswerOptionIDs)
    {
        if (AnswerOptionIDs == null || AnswerOptionIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from tblAnswerOption where pkAnswerOptionID in ({0})", string.Join(",", AnswerOptionIDs.ToArray()));

        return GetAnswerOptionsBySQL(_sql);;
    }

    public static List<AnswerOption> GetAnswerOptionsBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<AnswerOption>(sql).ToList();
        }
    }

    public static AnswerOption GetFirstAnswerOptionBySQL(string sql)
    {
        lock(Repository.Locker) {
            List<AnswerOption> _matches = GetAnswerOptionsBySQL(sql);

            if (_matches == null || _matches.Count == 0)
                return null;
            else
                return _matches[0];
        }
    }


        public static void SaveAll(List<AnswerOption> AnswerOptions)
        {
            lock (Repository.Locker)
            {
                List<AnswerOption> _newAnswerOptions = new List<AnswerOption>();
                List<AnswerOption> _existingAnswerOptions = new List<AnswerOption>();

                foreach (AnswerOption _AnswerOption in AnswerOptions)
                {
                    if (_AnswerOption.IsNew)
                        _newAnswerOptions.Add(_AnswerOption);
                    else
                        _existingAnswerOptions.Add(_AnswerOption);
                }

                Repository.Instance.InsertAll(_newAnswerOptions);
                Repository.Instance.UpdateAll(_existingAnswerOptions);
            }
        }

    public static List<AnswerOption> GetAnswerOptionsByQuestionID(int QuestionID)
    {
        string _sql = "select * from tblAnswerOption where fkQuestionID = " + QuestionID;
        return GetAnswerOptionsBySQL(_sql);
    }

        public static AnswerOption GetAnswerOptionByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from tblAnswerOption where MainSystemID = " + TargetMainSystemID;
            return GetFirstAnswerOptionBySQL(_sql);
        }
    }

}

