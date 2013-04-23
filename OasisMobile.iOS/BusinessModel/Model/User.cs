

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

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

    public static List<User> GetUsersByUserIDs(List<int> UserIDs)
    {
        if (UserIDs == null || UserIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from User where pkUserID in ({0})", string.Join(",", UserIDs.ToArray()));

        return GetUsersBySQL(_sql);;
    }

    public static List<User> GetUsersBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<User>(sql).ToList();
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

        public static User GetUserByLoginName(int TargetLoginName)
        {
            string _sql = "select * from User where LoginName = " + TargetLoginName;
            List<User> _Users = GetUsersBySQL(_sql);

            if (_Users == null || _Users.Count == 0)
                return null;
            else
                return _Users[0];
        }

        public static User GetUserByEmailAddress(int TargetEmailAddress)
        {
            string _sql = "select * from User where EmailAddress = " + TargetEmailAddress;
            List<User> _Users = GetUsersBySQL(_sql);

            if (_Users == null || _Users.Count == 0)
                return null;
            else
                return _Users[0];
        }

        public static User GetUserByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from User where MainSystemID = " + TargetMainSystemID;
            List<User> _Users = GetUsersBySQL(_sql);

            if (_Users == null || _Users.Count == 0)
                return null;
            else
                return _Users[0];
        }
    }

}

