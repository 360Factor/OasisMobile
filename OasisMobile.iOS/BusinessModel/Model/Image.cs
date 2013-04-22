

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public class Image
    {
        [Column("pkImageID")]
        public int ImageID {get; set;}
        [Column("fkQuestionID")]
        public int QuestionID {get; set;}
        public string Title {get; set;}
        public Boolean ShowInQuestion {get; set;}
        public Boolean ShowInCommentary {get; set;}
        public string FilePath {get; set;}
        public string DownloadURL {get; set;}
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
            if (this.MainSystemID != 0) {
                Repository.Instance.Update(this);
                return this.MainSystemID;
            } else {
                return Repository.Instance.Insert(this);
            }
        }
    }

    public static List<Image> GetAllImages()
    {
        lock(Repository.Locker) {
            return (from i in Repository.Instance.Table<Image>() select i).ToList();
        }
    }

    public static Image GetImageByMainSystemID(int MainSystemID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<Image>().Where(x => x.MainSystemID == MainSystemID).FirstOrDefault();
        }
    }

    public static List<Image> GetImagesBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Image>(sql).ToList();
        }
    }

    }

}

