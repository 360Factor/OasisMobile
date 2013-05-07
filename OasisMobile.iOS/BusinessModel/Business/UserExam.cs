using System;
using System.Collections.Generic;

namespace OasisMobile.BusinessModel
{
	public partial class UserExam
	{
		public void CascadeDelete(){
			BusinessModel.SQL.Execute (GetCascadeDeleteSql());
		}

		public List<string> GetCascadeDeleteSql(){
			List<string> _sqlToReturnList = new List<string>();
			_sqlToReturnList.Add(string.Format("DELETE FROM UserAnswerOption WHERE fkUserQuestionID IN (" +
			                     "SELECT UserQuestion.pkUserQuestionID FROM UserQuestion " +
			                     "WHERE UserQuestion.fkUserExamID={0} " +
			                     ")", this.UserExamID));
			_sqlToReturnList.Add("DELETE FROM UserQuestion WHERE fkUserExamID = " + this.UserExamID);
			_sqlToReturnList.Add("DELETE FROM UserExam WHERE pkUserExamID = " + this.UserExamID);
			return _sqlToReturnList;
		}

		public static BusinessModel.UserExam GetFirstUserExamByUserIDAndExamID(int aUserID, int aExamID){
			return BusinessModel.UserExam.GetFirstUserExamBySQL (string.Format(
				"SELECT * FROM UserExam " +
				"WHERE fkUserID={0} AND fkExamID={1} LIMIT 1", aUserID, aExamID));
		}



	}

}

