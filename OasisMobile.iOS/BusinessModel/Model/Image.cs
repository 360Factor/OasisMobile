

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.BusinessModel
{

    [Table("tblImage")]
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

    public Image() {}

    public Image(int NewQuestionID, 
                  string NewTitle, 
                  Boolean NewShowInQuestion, 
                  Boolean NewShowInCommentary, 
                  string NewFilePath, 
                  string NewDownloadURL, 
                  int NewMainSystemID)
    {
              QuestionID = NewQuestionID;
                 Title = NewTitle;
                 ShowInQuestion = NewShowInQuestion;
                 ShowInCommentary = NewShowInCommentary;
                 FilePath = NewFilePath;
                 DownloadURL = NewDownloadURL;
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
            string _sql = string.Format("select * from tblImage where pkImageID = {0}", ImageID);
            return GetFirstImageBySQL(_sql);
        }
    }

    public static List<Image> GetImagesByImageIDs(List<int> ImageIDs)
    {
        if (ImageIDs == null || ImageIDs.Count == 0)
            return null;

        string _sql = string.Format("select * from tblImage where pkImageID in ({0})", string.Join(",", ImageIDs.ToArray()));

        return GetImagesBySQL(_sql);;
    }

    public static List<Image> GetImagesBySQL(string sql)
    {
        lock(Repository.Locker) {
            return Repository.Instance.Query<Image>(sql).ToList();
        }
    }

    public static Image GetFirstImageBySQL(string sql)
    {
        lock(Repository.Locker) {
            List<Image> _matches = GetImagesBySQL(sql);

            if (_matches == null || _matches.Count == 0)
                return null;
            else
                return _matches[0];
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
        string _sql = "select * from tblImage where fkQuestionID = " + QuestionID;
        return GetImagesBySQL(_sql);
    }

        public static Image GetImageByMainSystemID(int TargetMainSystemID)
        {
            string _sql = "select * from tblImage where MainSystemID = " + TargetMainSystemID;
            return GetFirstImageBySQL(_sql);
        }
    }

}

