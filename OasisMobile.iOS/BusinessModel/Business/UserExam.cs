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
			_sqlToReturnList.Add(string.Format("DELETE FROM tblUserAnswerOption WHERE fkUserQuestionID IN (" +
			                     "SELECT tblUserQuestion.pkUserQuestionID FROM tblUserQuestion " +
			                     "WHERE tblUserQuestion.fkUserExamID={0} " +
			                     ")", this.UserExamID));
			_sqlToReturnList.Add("DELETE FROM tblUserQuestion WHERE fkUserExamID = " + this.UserExamID);
			_sqlToReturnList.Add("DELETE FROM tblUserExam WHERE pkUserExamID = " + this.UserExamID);
			return _sqlToReturnList;
		}

		public static BusinessModel.UserExam GetFirstUserExamByUserIDAndExamID(int aUserID, int aExamID){
			return BusinessModel.UserExam.GetFirstUserExamBySQL (string.Format(
				"SELECT * FROM tblUserExam " +
				"WHERE fkUserID={0} AND fkExamID={1} LIMIT 1", aUserID, aExamID));
		}



	}

}

