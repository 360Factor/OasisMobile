using System;
using System.Collections.Generic;

namespace OasisMobile.BusinessModel
{
	public class UserAnswerOptionDetail
	{
		public int UserAnswerOptionID { get; set; }

		public int AnswerOptionID { get; set; }

		public int UserQuestionID { get; set; }

		public int Sequence { get; set; }

		public bool IsSelected { get; set; }

		public bool IsCorrect { get; set; }

		public string AnswerOptionText { get; set; }

		public UserAnswerOptionDetail ()
		{
		
		}

		public static List<UserAnswerOptionDetail> GetUserAnswerOptionDetailListByUserQuestionID(int aUserQuestionID){
			string _query = string.Format ("SELECT tblUserAnswerOption.pkUserAnswerOptionID AS UserAnswerOptionID, " +
			                               "tblUserAnswerOption.fkAnswerOptionID AS AnswerOptionID, tblUserAnswerOption.fkUserQuestionID AS UserQuestionID, " +
			                               "tblUserAnswerOption.Sequence, tblUserAnswerOption.IsSelected, tblAnswerOption.IsCorrect, tblAnswerOption.AnswerText AS AnswerOptionText " +
			                               "FROM tblUserAnswerOption INNER JOIN tblAnswerOption " +
			                               "ON tblUserAnswerOption.fkAnswerOptionID = tblAnswerOption.pkAnswerOptionID " +
											"WHERE fkUserQuestionID={0}",aUserQuestionID);
			return Repository.Instance.Query<UserAnswerOptionDetail>(_query);
		}
	}
}

