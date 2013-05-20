

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    [Table("tblExamType")]
    public partial class ExamType
    {
        [PrimaryKey, AutoIncrement, Column("pkExamTypeID")]
        public int ExamTypeID {get; set;}
        [Ignore]
        public bool IsNew {get {return ExamTypeID == 0;}}
        public string ExamTypeName {get; set;}

    public ExamType() {}

    public ExamType(string NewExamTypeName)
    {
              ExamTypeName = NewExamTypeName;

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
            if (this.ExamTypeID != 0) {
                Repository.Instance.Update(this);
                return this.ExamTypeID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<ExamType> GetAllExamTypes()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<ExamType>() select i).ToList();
        }
    }

    public static ExamType GetExamTypeByExamTypeID(int ExamTypeID)
    {
        lock(Repository.Locker) {
            string _sql = string.Format("select * from tblExamType where pkExamTypeID = {0}", ExamTypeID);
            return GetFirstExamTypeBySQL(_sql);
        }
    }

    public static List<ExamType> GetExamTypesByExamTypeIDs(List<int> ExamTypeIDs)
    {
        if (ExamTypeIDs == null || ExamTypeIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from tblExamType where pkExamTypeID in ({0})", string.Join(",", ExamTypeIDs.ToArray()));

        return GetExamTypesBySQL(_sql);;
    }

    public static List<ExamType> GetExamTypesBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<ExamType>(sql).ToList();
        }
    }

    public static ExamType GetFirstExamTypeBySQL(string sql)
    {
        lock(Repository.Locker) {
            List<ExamType> _matches = GetExamTypesBySQL(sql);

            if (_matches == null || _matches.Count == 0)
                return null;
            else
                return _matches[0];
        }
    }


        public static void SaveAll(List<ExamType> ExamTypes)
        {
            lock (Repository.Locker)
            {
                List<ExamType> _newExamTypes = new List<ExamType>();
                List<ExamType> _existingExamTypes = new List<ExamType>();

                foreach (ExamType _ExamType in ExamTypes)
                {
                    if (_ExamType.IsNew)
                        _newExamTypes.Add(_ExamType);
                    else
                        _existingExamTypes.Add(_ExamType);
                }

                Repository.Instance.InsertAll(_newExamTypes);
                Repository.Instance.UpdateAll(_existingExamTypes);
            }
        }

        public static ExamType GetExamTypeByExamTypeName(string TargetExamTypeName)
        {
            string _sql = "select * from tblExamType where ExamTypeName = " + TargetExamTypeName;
            return GetFirstExamTypeBySQL(_sql);
        }
    }

}

