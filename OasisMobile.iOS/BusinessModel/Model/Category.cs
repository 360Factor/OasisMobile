

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BussinessLogicLayer
{

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
            return Repository.Instance.Table<Category>().Where(x => x.CategoryID == CategoryID).FirstOrDefault();
        }
    }

    public static List<Category> GetCategorysByCategoryIDs(List<int> CategoryIDs)
    {
        if (CategoryIDs == null || CategoryIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from Category where pkCategoryID in ({0})", string.Join(",", CategoryIDs.ToArray()));

        return GetCategorysBySQL(_sql);;
    }

    public static List<Category> GetCategorysBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Category>(sql).ToList();
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
    }

}

