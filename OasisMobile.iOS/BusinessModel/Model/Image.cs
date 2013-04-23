

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile
{

    public partial class Image
    {
        [PrimaryKey, AutoIncrement, Column("pkImageID")]
        public int ImageID {get; set;}
        [Ignore]
        public bool IsNew {get {return ImageID == 0;}}
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
            if (this.ImageID != 0) {
                Repository.Instance.Update(this);
                return this.ImageID;
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

    public static Image GetImageByImageID(int ImageID)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Table<Image>().Where(x => x.ImageID == ImageID).FirstOrDefault();
        }
    }

    public static List<Image> GetImagesByImageIDs(List<int> ImageIDs)
    {
        if (ImageIDs == null || ImageIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from Image where pkImageID in ({0})", string.Join(",", ImageIDs.ToArray()));

        return GetImagesBySQL(_sql);;
    }

    public static List<Image> GetImagesBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Image>(sql).ToList();
        }
    }


        public static void SaveAll(List<Image> Images)
        {
            lock (Repository.Locker)
            {
                List<Image> _newImages = new List<Image>();
                List<Image> _existingImages = new List<Image>();

                foreach (Image _Image in Images)
                {
                    if (_Image.IsNew)
                        _newImages.Add(_Image);
                    else
                        _existingImages.Add(_Image);
                }

                Repository.Instance.InsertAll(_newImages);
                Repository.Instance.UpdateAll(_existingImages);
            }
        }

    public static List<Image> GetImagesByQuestionID(int QuestionID)
    {
        string _sql = "select * from Image where fkQuestionID = " + QuestionID;
        return GetImagesBySQL(_sql);
    }

        public static Image GetImageByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from Image where MainSystemID = " + TargetMainSystemID;
            List<Image> _Images = GetImagesBySQL(_sql);

            if (_Images == null || _Images.Count == 0)
                return null;
            else
                return _Images[0];
        }
    }

}

