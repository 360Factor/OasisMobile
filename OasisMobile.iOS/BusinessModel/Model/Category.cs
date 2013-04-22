

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public class Category
    {
        [PrimaryKey, AutoIncrement, Column("pkCategoryID")]
        public int CategoryID {get; set;}
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

    public static List<Category> GetCategorysBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Category>(sql).ToList();
        }
    }

    }

}

