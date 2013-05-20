

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    [Table("tblUser")]
    public partial class User
    {
        [PrimaryKey, AutoIncrement, Column("pkUserID")]
        public int UserID {get; set;}
        [Ignore]
        public bool IsNew {get {return UserID == 0;}}
        public string LoginName {get; set;}
        public string Password {get; set;}
        public string EmailAddress {get; set;}
        public string UserName {get; set;}
        public int MainSystemID {get; set;}
        public DateTime LastLoginDate {get; set;}

    public User() {}

    public User(string NewLoginName, 
                  string NewPassword, 
                  string NewEmailAddress, 
                  string NewUserName, 
                  int NewMainSystemID, 
                  DateTime NewLastLoginDate)
    {
              LoginName = NewLoginName;
                 Password = NewPassword;
                 EmailAddress = NewEmailAddress;
                 UserName = NewUserName;
                 MainSystemID = NewMainSystemID;
                 LastLoginDate = NewLastLoginDate;

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
            string _sql = string.Format("select * from tblUser where pkUserID = {0}", UserID);
            return GetFirstUserBySQL(_sql);
        }
    }

    public static List<User> GetUsersByUserIDs(List<int> UserIDs)
    {
        if (UserIDs == null || UserIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from tblUser where pkUserID in ({0})", string.Join(",", UserIDs.ToArray()));

        return GetUsersBySQL(_sql);;
    }

    public static List<User> GetUsersBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<User>(sql).ToList();
        }
    }

    public static User GetFirstUserBySQL(string sql)
    {
        lock(Repository.Locker) {
            List<User> _matches = GetUsersBySQL(sql);

            if (_matches == null || _matches.Count == 0)
                return null;
            else
                return _matches[0];
        }
    }


        public static void SaveAll(List<User> Users)
        {
            lock (Repository.Locker)
            {
                List<User> _newUsers = new List<User>();
                List<User> _existingUsers = new List<User>();

                foreach (User _User in Users)
                {
                    if (_User.IsNew)
                        _newUsers.Add(_User);
                    else
                        _existingUsers.Add(_User);
                }

                Repository.Instance.InsertAll(_newUsers);
                Repository.Instance.UpdateAll(_existingUsers);
            }
        }

        public static User GetUserByLoginName(string TargetLoginName)
        {
            string _sql = "select * from tblUser where LoginName = " + TargetLoginName;
            return GetFirstUserBySQL(_sql);
        }

        public static User GetUserByEmailAddress(string TargetEmailAddress)
        {
            string _sql = "select * from tblUser where EmailAddress = " + TargetEmailAddress;
            return GetFirstUserBySQL(_sql);
        }

        public static User GetUserByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from tblUser where MainSystemID = " + TargetMainSystemID;
            return GetFirstUserBySQL(_sql);
        }
    }

}

