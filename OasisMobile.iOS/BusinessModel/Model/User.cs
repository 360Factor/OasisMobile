

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public class User
    {
        [PrimaryKey, AutoIncrement, Column("pkUserID")]
        public int UserID {get; set;}
        public string LoginName {get; set;}
        public string Password {get; set;}
        public string EmailAddress {get; set;}
        public string UserName {get; set;}
        public int MainSystemID {get; set;}
        public DateTime LastLoginDate {get; set;}

    public void Delete()
    {
        lock(Repository.Locker) {
            Repository.Instance.Delete(this);
        }
    }

    public int Save()
    {
        lock(Repository.Locker) {
            if (this.UserID != 0) {
                Repository.Instance.Update(this);
                return this.UserID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<User> GetAllUsers()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<User>() select i).ToList();
        }
    }

    public static User GetUserByUserID(int UserID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<User>().Where(x => x.UserID == UserID).FirstOrDefault();
        }
    }

    public static List<User> GetUsersBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<User>(sql).ToList();
        }
    }

    }

}

