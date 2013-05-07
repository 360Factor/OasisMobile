

using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using OasisMobile.BusinessModel;

namespace OasisMobile.Constant
{

    public class ExamType
    {
         public const int CMEAssessment = 1;
         public const int FellowPreTest = 2;
         public const int FellowPostTest = 3;
         public const int BetaExam = 4;
         public const int FreePreview = 5;
         public const int PreExam = 6;
         public const int PostExam = 7;

        public static void SeedEnumTable()
        {
            Dictionary<int, string> _idAndValues = BusinessModel.SQL.ExecuteIntStringDictionary("select pkExamTypeID, ExamTypeName from ExamType");

             UpdateOrInsert(_idAndValues, 1, "CME Assessment");
             UpdateOrInsert(_idAndValues, 2, "Fellow PreTest");
             UpdateOrInsert(_idAndValues, 3, "Fellow PostTest");
             UpdateOrInsert(_idAndValues, 4, "Beta Exam");
             UpdateOrInsert(_idAndValues, 5, "Free Preview");
             UpdateOrInsert(_idAndValues, 6, "Pre Exam");
             UpdateOrInsert(_idAndValues, 7, "Post Exam");
        }

        private static void UpdateOrInsert(Dictionary<int, string> ExistingIDandValue, int NewID, string NewValue)
        {
            if (ExistingIDandValue == null)
                ExistingIDandValue = new Dictionary<int, string>();
    
            if (ExistingIDandValue.ContainsKey(NewID))
            {
                if (ExistingIDandValue[NewID] != NewValue)
                {
                    BusinessModel.ExamType _toUpdate = BusinessModel.ExamType.GetExamTypeByExamTypeID(NewID);
                    _toUpdate.ExamTypeName = NewValue;
                    _toUpdate.Save();
                }
            }
            else
            {
                BusinessModel.ExamType _toInsert = new BusinessModel.ExamType(NewValue);
                _toInsert.Save();
    
                if (_toInsert.ExamTypeID != NewID)
                    throw new Exception("The new value can not be sequentially added!");
                else
                    ExistingIDandValue.Add(NewID, NewValue);
            }
        }
    }
}

