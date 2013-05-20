

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    [Table("tblCategory")]
    public partial class Category
    {
        [PrimaryKey, AutoIncrement, Column("pkCategoryID")]
        public int CategoryID {get; set;}
        [Ignore]
        public bool IsNew {get {return CategoryID == 0;}}
        public string CategoryName {get; set;}
        public int DisplayOrder {get; set;}
        public int? ParentCategoryID {get; set;}
        public string ExpandedCategoryName {get; set;}
        public int MainSystemID {get; set;}

    public Category() {}

    public Category(string NewCategoryName, 
                  int NewDisplayOrder, 
                  int? NewParentCategoryID, 
                  string NewExpandedCategoryName, 
                  int NewMainSystemID)
    {
              CategoryName = NewCategoryName;
                 DisplayOrder = NewDisplayOrder;
                 ParentCategoryID = NewParentCategoryID;
                 ExpandedCategoryName = NewExpandedCategoryName;
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
            if (this.CategoryID != 0) {
                Repository.Instance.Update(this);
                return this.CategoryID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<Category> GetAllCategorys()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<Category>() select i).ToList();
        }
    }

    public static Category GetCategoryByCategoryID(int CategoryID)
    {
        lock(Repository.Locker) {
            string _sql = string.Format("select * from tblCategory where pkCategoryID = {0}", CategoryID);
            return GetFirstCategoryBySQL(_sql);
        }
    }

    public static List<Category> GetCategorysByCategoryIDs(List<int> CategoryIDs)
    {
        if (CategoryIDs == null || CategoryIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from tblCategory where pkCategoryID in ({0})", string.Join(",", CategoryIDs.ToArray()));

        return GetCategorysBySQL(_sql);;
    }

    public static List<Category> GetCategorysBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Category>(sql).ToList();
        }
    }

    public static Category GetFirstCategoryBySQL(string sql)
    {
        lock(Repository.Locker) {
            List<Category> _matches = GetCategorysBySQL(sql);

            if (_matches == null || _matches.Count == 0)
                return null;
            else
                return _matches[0];
        }
    }


        public static void SaveAll(List<Category> Categorys)
        {
            lock (Repository.Locker)
            {
                List<Category> _newCategorys = new List<Category>();
                List<Category> _existingCategorys = new List<Category>();

                foreach (Category _Category in Categorys)
                {
                    if (_Category.IsNew)
                        _newCategorys.Add(_Category);
                    else
                        _existingCategorys.Add(_Category);
                }

                Repository.Instance.InsertAll(_newCategorys);
                Repository.Instance.UpdateAll(_existingCategorys);
            }
        }

        public static Category GetCategoryByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from tblCategory where MainSystemID = " + TargetMainSystemID;
            return GetFirstCategoryBySQL(_sql);
        }
    }

}

